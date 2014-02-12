
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Reports
Imports Target.Web.Apps.Reports.Collections
Imports Target.Library.Collections
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Abacus.Web.Apps.Documents.UserControls


Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' Screen used to view/maintain a Direct Payment Contract.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     18/02/2013  A7624 ABACUS Aborts if you try to amend DP Contract start date without first amending the period 
    '''                 date. In this case contract started at 1st October but payments were set up from November. 
    '''                 Trying to amend the contract in line with commencement of payments caused the abort. 
    '''                 Amending period date first then amending contract date was fine. 
    '''                 Should print an advisory message rather than aborting.
    '''     MoTahir     30/10/2011  D12399 Copy function for Direct Payment Contracts - Changes made post review.
    '''     MoTahir     24/10/2011  D12399 Copy function for Direct Payment Contracts.
    '''     Colind      27/05/2011  SDS issue #565 - updated code to handle error messages.
    '''     Colind      03/05/2011  SDS issue #595 - updated to not allow redundant third party bugdet holders to be saved.
    '''     MikeVO      26/04/2011  SDS issue #607 - corrected behavior when displayed in popup window.
    '''     MoTahir     19/04/2011  D11971 - Sds Generic Creditor Notes
    '''     ColinD      03/03/2011  D11874 Added PopulateServiceGroups() function to populate service groups drop down. Hooked up drop down list to relevant fetch and save routines for persistence.
    '''     ColinD      04/01/2011  SDS309 Altered FillDropdownGenericContractGroup method to only display non redundant records (or currently selected)
    '''     ColinD      24/12/2010  SDS326 Altered Delete method
    '''     ColinD      24/12/2010  SDS97 Fix to control visibility of Add button for payments and periods, dependent on role commands
    '''     MikeVO      26/10/2010  Fix to DeleteClicked() so that the dpcid is removed before redirecting back to the DP contracts wizard screen.
    '''     MikeVO      15/10/2010  Fix to CancelClicked().
    '''     MikeVO      01/09/2010  Added validation summary.
    '''     JohnF       24/09/2010  Added MenuItemID for parent form (D11801)
    '''     JohnF       29/07/2010  Created (D11801)
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Edit
        Inherits Target.Web.Apps.BasePage

#Region " Constants "
        Private Const SESSION_NEW_DP_CONTRACT As String = "NewDPContractData"

        Const PERIODS_ONLY As Integer = 1
        Const PAYMENTS_ONLY As Integer = 2

        Const VIEWSTATE_KEY_PERIODS As String = "PeriodsList"
        Const VIEWSTATE_KEY_PERIODS_COUNTER As String = "PeriodsCounter"
        Const VIEWSTATE_KEY_PERIODS_REMOVED As String = "RemovedPeriodsList"
        Const VIEWSTATE_KEY_PAYMENTS As String = "PaymentsList"
        Const VIEWSTATE_KEY_PAYMENTS_COUNTER As String = "PaymentsCounter"
        Const VIEWSTATE_KEY_PAYMENTS_REMOVED As String = "RemovedPaymentsList"

        Const UNIQUEID_PREFIX_NEW_PERIOD As String = "periodN"
        Const UNIQUEID_PREFIX_UPDATE_PERIOD As String = "periodU"
        Const UNIQUEID_PREFIX_NEW_PAYMENT As String = "paymentN"
        Const UNIQUEID_PREFIX_UPDATE_PAYMENT As String = "paymentU"
#End Region

#Region " Private variables "
        Const SDS_MONIKER As String = "(Self Directed Support)"
        Private _stdBut As StdButtonsBase
        Private _stdButBack As StdButtonsBase
        Private _btnReports As HtmlInputButton = New HtmlInputButton("button")
        Private _dpContract As Target.Abacus.Library.DataClasses.DPContract = Nothing
        Private _dpContractPeriods As DPContractPeriodCollection = Nothing
        Private _dpContractDetails As DPContractDetailCollection = Nothing
        Private _dpContractID As Integer
        Private _detailItemStartupJS As StringBuilder = New StringBuilder()
        Private _newPeriodIDCounter As Integer
        Private _newPaymentIDCounter As Integer
        Private _showNotes As Boolean = True
        Private _FetchedGenericContractGroups As List(Of GenericContractGroup) = Nothing
        Private _refreshParentWindow As Boolean = False
        Private _isCopy As Boolean
        Private _dpCopyContractID As Integer
        Private _saveClicked As Boolean

#End Region

#Region " Page_Init "
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            _stdBut = Me.Master.FindControl("MPContent").FindControl("tabStrip").FindControl("tabContract").FindControl("stdButtons1")
            If Utils.ToInt32(Request.QueryString("id")) = 0 Then
                _stdBut.InitialMode = StdButtonsMode.Initial
            Else
                _stdBut.InitialMode = StdButtonsMode.Fetched
            End If
            _stdButBack = Me.Master.FindControl("MPContent").FindControl("stdButtonBack")
            With _stdButBack
                .InitialMode = StdButtonsMode.Unknown
                AddHandler .AddCustomControls, AddressOf StdButtonsBack_AddCustomControls
            End With


            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .hideDebtorRef = True
            End With
        End Sub
#End Region

#Region " Page_Load "
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim allowAdd As Boolean = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DirectPaymentContracts"), "Direct Payment")
            Me.ShowValidationSummary = True

            _isCopy = Boolean.Parse(IIf(Request.QueryString("isCopy") = Nothing, False, Request.QueryString("isCopy")))
            _dpCopyContractID = Integer.Parse(IIf(Request.QueryString("copyID") = Nothing, 0, Request.QueryString("copyID")))

            With Me.JsLinks
                .Add("Edit.js")
                .Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
                .Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
                .Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            End With

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            cboContractGroup.DropDownList.Attributes.Add("onchange", String.Format("cboContractGroup_Change('{0}', '{1}');", cboContractGroup.ClientID, hidContractGroup.ClientID))

            allowAdd = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.AddNew"))

            '++ Add a Back button outside of all the tabs on this screen..
            With _stdButBack
                .AllowBack = True
                .AllowDelete = False
                .AllowEdit = False
                .AllowFind = False
                .AllowNew = False
            End With

            With _stdBut
                .AllowBack = False
                .AllowNew = ((Utils.ToInt32(Request.QueryString("id")) = 0) And (allowAdd))
                .ShowNew = False
                If Utils.ToInt32(Request.QueryString("id")) > 0 Then
                    .InitialMode = StdButtonsMode.Fetched
                Else
                    .InitialMode = StdButtonsMode.AddNew
                End If
                .Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID
                .AllowFind = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.Delete"))
                .EditableControls.Add(pnlContract.Controls)
                .SelectedItemID = Utils.ToInt32(Request.QueryString("id"))
                btnAddPeriod.Visible = allowAdd
                btnAddPayment.Visible = allowAdd
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
            End With

            Me.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            'set _showNotes to false when creating a new record
            If _stdBut.InitialMode = StdButtonsMode.AddNew Then _showNotes = False

            InitialiseDocumentSelector()
            PopulateScreen()
            OutputAuditLogControls()

        End Sub
#End Region

#Region " ClearViewState "
        Private Sub ClearViewState(Optional ByVal whichToClear As Integer = 0)
            If whichToClear = 0 OrElse whichToClear = PERIODS_ONLY Then
                ViewState.Remove(VIEWSTATE_KEY_PERIODS)
                ViewState.Remove(VIEWSTATE_KEY_PERIODS_COUNTER)
                phPeriods.Controls.Clear()
            End If

            If whichToClear = 0 OrElse whichToClear = PAYMENTS_ONLY Then
                ViewState.Remove(VIEWSTATE_KEY_PAYMENTS)
                ViewState.Remove(VIEWSTATE_KEY_PAYMENTS_COUNTER)
                phPayments.Controls.Clear()
            End If
        End Sub
#End Region

#Region " NewClicked "
        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage

            '++ Clear any new direct payment contract data from the current session..
            _dpContractID = 0
            msg = ClearDPContract()
            If Not msg.Success Then WebUtils.DisplayError(msg)
            ShowTabPeriods(False)
            ShowTabPayments(False)
            ShowTabAuditLog(False)
            ClearNewDPContractData()
            _showNotes = False
            msg = PopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)
        End Sub
#End Region

#Region " FindClicked "
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            _dpContractID = e.ItemID
            If _dpContract Is Nothing Then
                _dpContract = New Target.Abacus.Library.DataClasses.DPContract(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            End If

            If _dpContractID <> _dpContract.ID Then
                msg = DPContractBL.Fetch(Me.DbConnection, _dpContractID, _dpContract, _dpContractPeriods, _dpContractDetails)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            ClearViewState()

            msg = PopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)
        End Sub
#End Region

#Region " CancelClicked "
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_PERIODS_REMOVED)
            ViewState.Remove(VIEWSTATE_KEY_PAYMENTS_REMOVED)

            If e.ItemID = 0 Then
                '++ Item hasn't been created yet, so redirect to the previous page..
                If Not String.IsNullOrEmpty(Request.QueryString("backUrl")) Then
                    Response.Redirect(Request.QueryString("backUrl"))
                End If
            Else
                '++ ..is an existing item, so undo any changes..
                FindClicked(e)
            End If
        End Sub
#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim builder As Target.Library.Web.UriBuilder
            Dim backUrl As String

            msg = DPContractBL.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _stdBut.SelectedItemID)
            If msg.Success Then
                If IsPopupScreen Then
                    ' if this page is displayed in a popup screen
                    ' then flag to refresh the parent window...or data will 
                    ' become out of sync!
                    _refreshParentWindow = True
                Else
                    '++ Contract has been successfully removed, so redirect to the previous page..
                    backUrl = Request.QueryString("backurl")
                    If Not String.IsNullOrEmpty(backUrl) Then
                        builder = New Target.Library.Web.UriBuilder(backUrl)
                        ' remove DP contract ID so that the DP contracts wizard screen doesn't try to re-select it
                        builder.QueryItems.Remove("dpcid")
                        Response.Redirect(builder.ToString())
                    End If
                End If
            Else
                If msg.Number = SdsTransactions.SdsTransactionBL.ErrorCannotDeleteSdsTransactionNumber Then
                    lblError.Text = "ERROR : Cannot delete this record as it is currently in use.<br />"
                Else
                    lblError.Text = msg.Message
                End If
                e.Cancel = True
            End If

        End Sub

#End Region

#Region "SaveClicked"

        ''' <summary>
        ''' Saves the direct payment contract.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As New ErrorMessage()
            Dim qsId As Integer = Utils.ToInt32(Request.QueryString("id"))

            _saveClicked = True

            ' reset the error text
            lblError.Text = String.Empty

            ' get the contract from screen
            msg = GetContractFromScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If _dpContract.ID > 0 Then
                '++ The following controls are disabled during edit mode for an existing contract,
                '++ so turn off the automatic screen validation..

                With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                    .Required = False
                    .ValidationGroup = ""
                End With
                txtDateFrom.Required = False
                txtDateFrom.ValidationGroup = ""

            Else

                With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                    .Required = True
                    .ValidationGroup = "Save"
                End With
                txtDateFrom.Required = True
                txtDateFrom.ValidationGroup = "Save"

            End If

            Me.Validate("Save")

            If Me.IsValid() Then
                ' if the direct payment contract is valid then persist

                Using transaction As SqlTransaction = SqlHelper.GetTransaction(Me.DbConnection)
                    ' create a transaction record to persist this record with

                    Try

                        ' save the record
                        If _isCopy Then
                            msg = DPContractBL.Copy(transaction, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _dpContract, Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID)), Utils.ToInt32(Request.Form(CType(budgetholder, InPlaceBudgetHolderSelector).HiddenFieldUniqueID)), _dpCopyContractID)
                        Else
                            msg = DPContractBL.Save(transaction, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _dpContract, Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID)), Utils.ToInt32(Request.Form(CType(budgetholder, InPlaceBudgetHolderSelector).HiddenFieldUniqueID)))
                        End If
                        If msg.Success Then
                            '++ Save, then reload the direct payment contract..

                            ' commit the save to db
                            transaction.Commit()

                            If qsId > 0 Then
                                ' if this was an existing record

                                e.ItemID = _dpContract.ID
                                _stdBut.SelectedItemID = _dpContract.ID
                                If IsPopupScreen Then
                                    ' if this page is displayed in a popup screen
                                    ' then flag to refresh the parent window...or data will 
                                    ' become out of sync!
                                    _refreshParentWindow = True
                                End If
                                FindClicked(e)

                            Else
                                ' else this was a new record

                                '++ When saving a new contract, refresh the screen (and the URL to include
                                '++ the returned contract ID)..
                                Response.Redirect(String.Format("Edit.aspx?id={0}&backUrl={1}&autopopup={2}&refreshParent={3}", _
                                                                _dpContract.ID, _
                                                                HttpUtility.UrlEncode(Request.QueryString("backUrl")), _
                                                                Utils.ToInt32(Request.QueryString("autopopup")), _
                                                                IIf(IsPopupScreen, "1", "0")))

                            End If

                        Else
                            '++ Display the message returned by the Save method..

                            SqlHelper.RollbackTransaction(transaction)
                            '_dpContract.Unhook()
                            lblError.Text = msg.Message
                            e.Cancel = True

                        End If

                    Catch ex As Exception
                        ' catch exception and rollback transaction

                        SqlHelper.RollbackTransaction(transaction)

                        ' wrap exception and display
                        msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                        WebUtils.DisplayError(msg)

                    End Try

                End Using

            Else
                ' direct payment contract is not valid so cancel save

                e.Cancel = True

            End If

            _saveClicked = False

        End Sub

#End Region

#Region " PopulateScreen "
        Private Function PopulateScreen() As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim selectedClientID As Integer, selectedTPBudgetHolderID As Integer
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            '++ Header tab..
            _dpContractID = _stdBut.SelectedItemID
            _stdBut.SelectedItemID = _dpContractID

            tabDocuments.Visible = SecurityBL.UserHasMenuItem(Me.DbConnection, user.ID, _
                                   Target.Library.Web.ConstantsManager.GetConstant( _
                                   "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"), _
                                   Settings.CurrentApplicationID)

            If _dpContractID > 0 Then
                If _dpContract Is Nothing Then
                    _dpContract = New Target.Abacus.Library.DataClasses.DPContract(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                End If
                msg.Success = True
                If _dpContractID <> _dpContract.ID Then
                    msg = DPContractBL.Fetch(Me.DbConnection, _dpContractID, _dpContract, _dpContractPeriods, _dpContractDetails)
                    If Not msg.Success Then Return msg
                End If

                msg = PopulateScreenDetails(selectedClientID, selectedTPBudgetHolderID)
                If Not msg.Success Then Return msg

                'populate document tab
                If tabDocuments.Visible Then CType(docSelector, DocumentSelector).InitControl(Me.Page, selectedClientID)
            ElseIf _isCopy And _dpCopyContractID > 0 Then
                _dpContract = New Target.Abacus.Library.DataClasses.DPContract(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                msg = DPContractBL.Fetch(Me.DbConnection, _dpCopyContractID, _dpContract, _dpContractPeriods, _dpContractDetails)
                If Not msg.Success Then Return msg
                msg = PopulateScreenDetails(selectedClientID, selectedTPBudgetHolderID)
                If Not msg.Success Then Return msg
                _dpContract = Nothing
                msg = GetContractFromScreen()
                If Not msg.Success Then Return msg
            Else
                '++ In midst of adding a new contract, so retrieve the field settings
                '++ into a DPContract structure..
                lblSDS.Text = ""
                msg = GetContractFromScreen()
                txtContractNum.Text = DPContractBL.AUTO_NUMBER_TEXT
                If Not msg.Success Then Return msg
            End If

            If _dpContract IsNot Nothing AndAlso _dpContract.ID > 0 Then
                Dim periodIDs As List(Of String), paymentIDs As List(Of String)

                periodIDs = GetUniquePeriodIDsFromViewState()
                paymentIDs = GetUniquePaymentIDsFromViewState()

                ClearViewState()

                tabPeriods.Enabled = True
                ShowTabPeriods(True)
                msg = PopulatePeriods(_dpContractPeriods, periodIDs)
                If Not msg.Success Then Return msg

                tabPayments.Enabled = True
                ShowTabPayments(True)
                msg = PopulatePayments(_dpContractDetails, paymentIDs)
                If Not msg.Success Then Return msg

                tabAuditLog.Enabled = True
                ShowTabAuditLog(True)
            Else
                ShowTabPeriods(False)
                ShowTabPayments(False)
                ShowTabDocuments(False)
                ShowTabAuditLog(False)
            End If

            'initialise the notes selector
            If _showNotes Then
                InitialiseNotesSelector()
            Else
                Notes1.Visible = False
            End If

            msg = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function
#End Region

#Region " PopulateScreenDetails"

        Private Function PopulateScreenDetails(ByRef selectedClientID As Integer, ByRef selectedTPBudgetHolderID As Integer _
                                               ) As ErrorMessage

            Dim msg As ErrorMessage = Nothing

            With _dpContract
                '++ Existing direct payment contract..
                txtContractNum.Text = IIf(_isCopy, DPContractBL.AUTO_NUMBER_TEXT, .Number)
                txtAltRef.Text = .AltReference
                txtDateFrom.Text = IIf(_isCopy, "", .DateFrom.ToString("dd/MM/yyyy"))
                If .DateTo <> DataUtils.MAX_DATE And Not _isCopy Then
                    txtDateTo.Text = .DateTo.ToString("dd/MM/yyyy")
                Else
                    txtDateTo.Text = "(open-ended)"
                End If
                hidDateTo.Value = txtDateTo.Text
                selectedClientID = 0
                selectedTPBudgetHolderID = 0
                If .ClientBudgetHolderID > 0 Then
                    Dim budholder As ClientBudgetHolder = New ClientBudgetHolder(Me.DbConnection, "", "")
                    msg = budholder.Fetch(.ClientBudgetHolderID)
                    If Not msg.Success Then Return msg
                    selectedClientID = budholder.ClientID
                    selectedTPBudgetHolderID = Utils.ToInt32(budholder.ThirdPartyBudgetHolderID)
                End If
                SetupClientSelector(selectedClientID)
                SetupBudgetHolderSelector(selectedTPBudgetHolderID, selectedClientID)
                txtEndReason.Text = ""
                If .EndReasonID > 0 Then
                    Dim endReason As New ContractEndReason(Me.DbConnection, "", "")
                    msg = endReason.Fetch(.EndReasonID)
                    If Not msg.Success Then Return msg
                    txtEndReason.Text = String.Format("({0})", endReason.Description)
                End If
                hidEndReason.Value = .EndReasonID
                FillDropdownGenericContractGroup(.GenericContractGroupID)
                hidContractGroup.Value = cboContractGroup.Value
                PopulateServiceGroups(.ServiceGroupID)
                If .IsSDS = TriState.True Then
                    lblSDS.Text = SDS_MONIKER
                Else
                    lblSDS.Text = ""
                End If
            End With

            Return msg

        End Function

#End Region


#Region " GetContractFromScreen "
        Private Function GetContractFromScreen() As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim selectedClientID As Integer, selectedTPBudgetHolderID As Integer

            Try
                _dpContractID = _stdBut.SelectedItemID
                msg.Success = True

                If _dpContract Is Nothing Then
                    _dpContract = New DPContract(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                End If
                If _dpContractID <> _dpContract.ID AndAlso _dpContractID <> 0 Then
                    msg = DPContractBL.Fetch(Me.DbConnection, _dpContractID, _dpContract, _dpContractPeriods, _dpContractDetails)
                    If Not msg.Success Then Return msg
                End If

                '++ Store the current field settings into the contract object..
                If Not _isCopy Or (_isCopy And _saveClicked) Then
                    With _dpContract
                        .Number = txtContractNum.GetPostBackValue()
                        txtContractNum.Text = txtContractNum.GetPostBackValue()

                        '++ For a new contract, default the service user and budget holder dropdowns..
                        selectedClientID = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))
                        If Utils.ToInt32(Request.QueryString("clientid")) > 0 AndAlso selectedClientID = 0 Then
                            selectedClientID = Utils.ToInt32(Request.QueryString("clientid"))
                        End If
                        SetupClientSelector(selectedClientID)

                        selectedTPBudgetHolderID = Utils.ToInt32(Request.Form(CType(budgetholder, InPlaceBudgetHolderSelector).HiddenFieldUniqueID))
                        If Utils.ToInt32(Request.QueryString("bhid")) > 0 AndAlso selectedTPBudgetHolderID = 0 Then
                            Dim tph As ThirdPartyBudgetHolder = Nothing
                            selectedTPBudgetHolderID = Utils.ToInt32(Request.QueryString("bhid"))
                            ' get the third party budget holder specified in the url
                            msg = BudgetHolderBL.GetBudgetHolder(DbConnection, tph, selectedTPBudgetHolderID)
                            If Not msg.Success Then Return msg
                            ' determine if the third party budget holder specified is redundant or not
                            If tph.Redundant = TriState.True Then
                                selectedTPBudgetHolderID = 0
                            End If
                        End If
                        SetupBudgetHolderSelector(selectedTPBudgetHolderID, selectedClientID)
                        .AltReference = txtAltRef.GetPostBackValue()
                        txtAltRef.Text = txtAltRef.GetPostBackValue()
                        If Utils.IsDate(txtDateFrom.GetPostBackValue()) Then
                            .DateFrom = Convert.ToDateTime(txtDateFrom.GetPostBackValue())
                            txtDateFrom.Text = txtDateFrom.GetPostBackValue()
                        End If
                        txtDateTo.Text = "(open-ended)"
                        If Utils.IsDate(hidDateTo.Value) AndAlso Convert.ToDateTime(hidDateTo.Value) <> DataUtils.MAX_DATE Then
                            .DateTo = Convert.ToDateTime(hidDateTo.Value)
                            txtDateTo.Text = hidDateTo.Value
                        End If

                        .EndReasonID = Utils.ToInt32(hidEndReason.Value)
                        txtEndReason.Text = ""
                        If .EndReasonID > 0 Then
                            Dim endReason As New ContractEndReason(Me.DbConnection, "", "")
                            msg = endReason.Fetch(.EndReasonID)
                            If Not msg.Success Then Return msg
                            txtEndReason.Text = String.Format("({0})", endReason.Description)
                        End If
                        .GenericContractGroupID = Utils.ToInt32(hidContractGroup.Value)
                        FillDropdownGenericContractGroup(.GenericContractGroupID)
                        .ServiceGroupID = Utils.ToInt32(cboServiceGroup.GetPostBackValue())
                        PopulateServiceGroups(.ServiceGroupID)
                        .IsSDS = (lblSDS.Text = SDS_MONIKER)
                    End With
                End If

            Catch ex As Exception
                msg.Success = False
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            End Try

            Return msg
        End Function
#End Region

#Region " PopulatePeriods "
        Private Function PopulatePeriods(ByRef dpcPeriods As DPContractPeriodCollection, ByVal itemIDs As List(Of String)) As ErrorMessage
            Dim msg As ErrorMessage
            Dim newPeriods As DPContractPeriodCollection = Nothing
            Dim id As String

            If _dpContract.ID > 0 Then
                pnlPeriods.Visible = True
            Else
                pnlPeriods.Visible = False
            End If

            If pnlPeriods.Visible Then
                _detailItemStartupJS.Length = 0

                If dpcPeriods IsNot Nothing Then
                    For Each dpcPeriod As DPContractPeriod In dpcPeriods
                        id = GetUniquePeriodID(dpcPeriod)
                        OutputPeriodsControls(id, dpcPeriod)
                        itemIDs.Add(id)
                    Next
                End If

                PersistUniquePeriodIDsToViewState(itemIDs)
            End If

            msg = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function
#End Region

#Region " PopulatePayments "
        Private Function PopulatePayments(ByRef dpcPayments As DPContractDetailCollection, ByVal itemIDs As List(Of String)) As ErrorMessage
            Dim msg As ErrorMessage
            Dim newPayments As DPContractDetailCollection = Nothing
            Dim id As String

            If _dpContract.ID > 0 Then
                pnlPayments.Visible = True
            Else
                pnlPayments.Visible = False
            End If

            If pnlPayments.Visible Then
                _detailItemStartupJS.Length = 0

                If dpcPayments IsNot Nothing Then
                    For Each dpcPayment As DPContractDetail In dpcPayments
                        id = GetUniquePaymentID(dpcPayment)
                        OutputPaymentsControls(id, dpcPayment)
                        itemIDs.Add(id)
                    Next
                End If

                PersistUniquePaymentIDsToViewState(itemIDs)
            End If

            msg = New ErrorMessage()
            msg.Success = True
            Return msg
        End Function
#End Region

#Region " InitialiseDocumentSelector "
        Private Sub InitialiseDocumentSelector()
            CType(docSelector, DocumentSelector).ServiceUserType = DocumentAssociationType.ServiceUser

            CType(docSelector, DocumentSelector).Show_Buttons = (ShowButtons.New + ShowButtons.View + ShowButtons.Properties)

            CType(docSelector, DocumentSelector).Show_Filters = ( _
                            ShowFilters.Created + ShowFilters.CreatedBy + ShowFilters.CreatedFrom + _
                            ShowFilters.CreatedTo + ShowFilters.DocumentType + ShowFilters.Origin + _
                            ShowFilters.PrintStatus + ShowFilters.PrintStatusBy + _
                            ShowFilters.PrintStatusCheckBoxes + ShowFilters.PrintStatusFrom + _
                            ShowFilters.PrintStatusTo)
        End Sub
#End Region

#Region " OutputPeriodsControls "
        Private Sub OutputPeriodsControls(ByVal uniqueID As String, ByVal dpcPeriod As DPContractPeriod)
            Dim row As HtmlGenericControl
            Dim divCtrl As HtmlGenericControl
            Dim periodURL As String = ""

            '++ Don't output items marked as deleted..
            divCtrl = New HtmlGenericControl("hr")
            '++ If we have no row index, then add the row at the end..
            If phPeriods.Controls.Count > 0 Then phPeriods.Controls.Add(divCtrl)

            '++ Create an instance of the period maintenance screen for the period
            '++ object currently held, using an iFrame control to house it..
            row = New HtmlGenericControl("iframe")
            row.ID = uniqueID
            phPeriods.Controls.Add(row)
            With row
                periodURL = String.Format("EditPeriod.aspx?uid={0}&id={1}&dpcid={2}", _
                                          .ClientID, dpcPeriod.ID, _stdBut.SelectedItemID)
                .Attributes.Add("src", periodURL)
                .Attributes.Add("scrolling", "no")
                .Attributes.Add("frameborder", "no")
                If uniqueID.StartsWith(UNIQUEID_PREFIX_NEW_PERIOD) Then
                    .Style.Add("height", "250px")
                Else
                    .Style.Add("height", "32px")
                End If
                .Style.Add("width", "100%")
            End With
        End Sub
#End Region

#Region " OutputPaymentsControls "
        Private Sub OutputPaymentsControls(ByVal uniqueID As String, ByVal dpcPayment As DPContractDetail)
            Dim row As HtmlGenericControl
            Dim divCtrl As HtmlGenericControl
            Dim paymentURL As String = ""

            '++ Don't output items marked as deleted..
            divCtrl = New HtmlGenericControl("hr")
            '++ If we have no row index, then add the row at the end..
            If phPayments.Controls.Count > 0 Then phPayments.Controls.Add(divCtrl)

            '++ Create an instance of the payment maintenance screen for the payment
            '++ object currently held, using an iFrame control to house it..
            row = New HtmlGenericControl("iframe")
            row.ID = uniqueID
            phPayments.Controls.Add(row)
            With row
                paymentURL = String.Format("EditPayment.aspx?uid={0}&id={1}&dpcid={2}&mode={3}", _
                                          .ClientID, dpcPayment.ID, _stdBut.SelectedItemID, _
                                          IIf(dpcPayment.ID > 0, Convert.ToByte(StdButtonsMode.Fetched), Convert.ToByte(StdButtonsMode.AddNew)))
                .Attributes.Add("src", paymentURL)
                .Attributes.Add("scrolling", "yes")
                .Attributes.Add("frameborder", "no")
                If uniqueID.StartsWith(UNIQUEID_PREFIX_NEW_PAYMENT) Then
                    .Style.Add("height", "400px")
                Else
                    .Style.Add("height", "32px")
                End If
                .Style.Add("width", "100%")
            End With
        End Sub
#End Region

#Region " OutputAuditLogControls "
        Private Sub OutputAuditLogControls()
            Dim row As HtmlGenericControl
            Dim paymentURL As String = ""

            '++ Create an instance of the audit log screen, using an iFrame control to house it..
            row = New HtmlGenericControl("iframe")
            row.ID = UniqueID
            phAuditLog.Controls.Add(row)
            With row
                paymentURL = String.Format("AuditLog.aspx?uid={0}&dpcid={1}&menuid={2}", _
                                          .ClientID, Utils.ToInt32(Request.QueryString("id")), Me.MenuItemID)
                .Attributes.Add("src", paymentURL)
                .Attributes.Add("scrolling", "no")
                .Attributes.Add("frameborder", "no")
                .Style.Add("height", "310px")
                .Style.Add("width", "100%")
            End With
        End Sub
#End Region

#Region " ClearNewDPContractData "
        Private Sub ClearNewDPContractData()
            Session(SESSION_NEW_DP_CONTRACT) = Nothing
        End Sub
#End Region

#Region " SetupInPlaceSelectors "
        Private Sub SetupClientSelector(ByVal clientID As Integer)
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .ClientDetailID = clientID
                .Required = True
                .ValidationGroup = "Save"
            End With
        End Sub

        Private Sub SetupBudgetHolderSelector(ByVal budgetHolderID As Integer, ByVal clientID As Integer)
            With CType(Me.budgetholder, InPlaceSelectors.InPlaceBudgetHolderSelector)
                .BudgetHolderID = budgetHolderID
                .FilterShowRedundant = False
                .FilterServiceUserID = clientID
                .Required = False
            End With
        End Sub
#End Region

#Region " FillDropdownGenericContractGroup "

        ''' <summary>
        ''' Fills the dropdown generic contract group.
        ''' </summary>
        ''' <param name="selectedID">The selected ID.</param>
        Private Sub FillDropdownGenericContractGroup(ByVal selectedID As Integer)

            Dim selectedItem As ListItem = Nothing

            ' add in all non redundant contract groups into the drop down
            With cboContractGroup
                With .DropDownList
                    If Not .SelectedItem Is Nothing Then
                        ' if we already have a selected item then unselect it
                        .SelectedItem.Selected = False
                    End If
                    .Items.Clear()
                    ' get all non redundant contract groups as items
                    .DataSource = (From tmpContractGroup In FetchedGenericContractGroups _
                                        Where tmpContractGroup.Redundant = TriState.False _
                                    Select tmpContractGroup).ToList()
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' add default item into list
                    .Items.Insert(0, New ListItem(" ", "0"))
                End With
            End With

            If selectedID > 0 Then
                ' if we have selected item then find it and select it

                selectedItem = cboContractGroup.DropDownList.Items.FindByValue(selectedID)

                If selectedItem Is Nothing Then
                    ' we havent found the item so add into the drop down

                    ' get the first item that matches the passed in id...
                    Dim selectedContractGroup As GenericContractGroup = (From tmpContractGroup In FetchedGenericContractGroups _
                                                                            Where tmpContractGroup.ID = selectedID _
                                                                                Select tmpContractGroup).FirstOrDefault()

                    If Not selectedContractGroup Is Nothing Then
                        ' if we have found a matching contract group then add it and select it

                        selectedItem = New ListItem(selectedContractGroup.Description, selectedContractGroup.ID)
                        selectedItem.Selected = True
                        cboContractGroup.DropDownList.Items.Add(selectedItem)

                    End If

                Else
                    ' else select the item we have found

                    selectedItem.Selected = True

                End If

            End If

        End Sub

#End Region

#Region "PopulateServiceGroups"

        ''' <summary>
        ''' Populates the service groups.
        ''' </summary>
        ''' <param name="selectedId">The selected id, 0 if none selected.</param>
        Private Sub PopulateServiceGroups(ByVal selectedId As Integer)

            Dim msg As New ErrorMessage()
            Dim serviceGroups As Target.Web.Apps.Security.Collections.ServiceGroupCollection = Nothing
            Dim serviceGroupToSelect As ListItem = Nothing

            ' clear all existing items
            cboServiceGroup.DropDownList.Items.Clear()

            ' fetch the groups to display, non redundant only
            msg = Target.Web.Apps.Security.ServiceGroup.FetchList(conn:=DbConnection, serviceGroupClassificationID:=5, auditLogTitle:=String.Empty, auditUserName:=String.Empty, list:=serviceGroups, redundant:=TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' bind data to the drop down list
            With cboServiceGroup.DropDownList
                .DataSource = serviceGroups
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
            End With

            If selectedId > 0 Then
                ' if we have an item to select

                serviceGroupToSelect = cboServiceGroup.DropDownList.Items.FindByValue(selectedId)

                If Not serviceGroupToSelect Is Nothing Then
                    ' if we have an item to select then do so

                    serviceGroupToSelect.Selected = True

                Else
                    ' else the item is redundant so fetch from the db

                    Dim serviceGroup As New Target.Web.Apps.Security.ServiceGroup(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                    ' fetch the svc group from the db and display error if occurs
                    msg = serviceGroup.Fetch(selectedId)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' create the item and select it
                    serviceGroupToSelect = New ListItem(serviceGroup.Description, serviceGroup.ID)
                    serviceGroupToSelect.Selected = True

                    ' add the item into the list
                    cboServiceGroup.DropDownList.Items.Add(serviceGroupToSelect)

                End If

            End If

            ' add the default item into the list
            cboServiceGroup.DropDownList.Items.Insert(0, New ListItem("", ""))

        End Sub

#End Region

#Region " GetUniquePeriodIDsFromViewState "
        Private Function GetUniquePeriodIDsFromViewState() As List(Of String)
            Dim list As List(Of String)

            '++ Get the details from view state..
            If ViewState.Item(VIEWSTATE_KEY_PERIODS) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_PERIODS), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_PERIODS_COUNTER) Is Nothing Then
                _newPeriodIDCounter = 0
            Else
                _newPeriodIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_PERIODS_COUNTER), Integer)
            End If

            Return list

        End Function

#End Region

#Region " GetUniquePaymentIDsFromViewState "
        Private Function GetUniquePaymentIDsFromViewState() As List(Of String)
            Dim list As List(Of String)

            '++ Get the details from view state..
            If ViewState.Item(VIEWSTATE_KEY_PAYMENTS) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_PAYMENTS), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_PAYMENTS_COUNTER) Is Nothing Then
                _newPaymentIDCounter = 0
            Else
                _newPaymentIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_PAYMENTS_COUNTER), Integer)
            End If

            Return list

        End Function

#End Region

#Region " GetUniquePeriodID "
        Private Function GetUniquePeriodID(ByVal dpcPeriod As DPContractPeriod) As String
            Dim id As String

            If dpcPeriod.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_PERIOD & _newPeriodIDCounter
                _newPeriodIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_PERIOD & dpcPeriod.ID
            End If

            Return id
        End Function
#End Region

#Region " GetUniquePaymentID "
        Private Function GetUniquePaymentID(ByVal dpcPayment As DPContractDetail) As String
            Dim id As String

            If dpcPayment.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_PAYMENT & _newPaymentIDCounter
                _newPaymentIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_PAYMENT & dpcPayment.ID
            End If

            Return id
        End Function
#End Region

#Region " PersistUniquePeriodIDsToViewState "
        Private Sub PersistUniquePeriodIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_PERIODS, list)
            ViewState.Add(VIEWSTATE_KEY_PERIODS_COUNTER, _newPeriodIDCounter)
        End Sub
#End Region

#Region " PersistUniquePaymentIDsToViewState "
        Private Sub PersistUniquePaymentIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_PAYMENTS, list)
            ViewState.Add(VIEWSTATE_KEY_PAYMENTS_COUNTER, _newPaymentIDCounter)
        End Sub
#End Region

#Region " Tab Manipulation "
        Private Sub ShowTabPeriods(ByVal isVisible As Boolean)
            tabPeriods.Visible = isVisible
            If isVisible Then
                tabPeriods.HeaderText = "Periods"
            Else
                tabPeriods.HeaderText = ""
            End If
        End Sub

        Private Sub ShowTabPayments(ByVal isVisible As Boolean)
            tabPayments.Visible = isVisible
            If isVisible Then
                tabPayments.HeaderText = "Payments"
            Else
                tabPayments.HeaderText = ""
            End If
        End Sub

        Private Sub ShowTabDocuments(ByVal isVisible As Boolean)
            tabDocuments.Visible = isVisible
            If isVisible Then
                tabDocuments.HeaderText = "Documents"
            Else
                tabDocuments.HeaderText = ""
            End If
        End Sub

        Private Sub ShowTabAuditLog(ByVal isVisible As Boolean)
            tabAuditLog.Visible = isVisible
            If isVisible Then
                tabAuditLog.HeaderText = "Audit Log"
            Else
                tabAuditLog.HeaderText = ""
            End If
        End Sub
#End Region

#Region " btnAddPeriod_Click "
        Private Sub btnAddPeriod_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPeriod.Click
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim id As String
            Dim list As List(Of String)
            Dim newPeriod As DPContractPeriod = Nothing
            Dim newPeriods As DPContractPeriodCollection = New DPContractPeriodCollection
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                msg = PopulateScreen()
                If msg.Success Then
                    '++ Add a new contract period entry..
                    newPeriod = New DPContractPeriod(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    list = GetUniquePeriodIDsFromViewState()
                    id = GetUniquePeriodID(newPeriod)

                    '++ Create a new item line for each Period held..
                    ClearViewState(PERIODS_ONLY)
                    '++ Persist the data into view state using the calculated index..
                    list.Insert(0, id)
                    PersistUniquePeriodIDsToViewState(list)

                    newPeriods.Add(newPeriod)
                    For Each dpcPeriod As DPContractPeriod In _dpContractPeriods
                        newPeriods.Add(dpcPeriod)
                    Next
                    _dpContractPeriods = newPeriods

                    hidSelectedTabIndex.Value = PERIODS_ONLY
                    tabStrip.ActiveTabIndex = PERIODS_ONLY
                End If
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            Finally
                If Not msg.Success Then lblError2.Text = msg.Message
            End Try
        End Sub
#End Region

#Region " btnAddPayment_Click "
        Private Sub btnAddPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddPayment.Click
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim id As String
            Dim list As List(Of String)
            Dim newPayment As DPContractDetail = Nothing
            Dim newPayments As DPContractDetailCollection = New DPContractDetailCollection
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                msg = PopulateScreen()
                If msg.Success Then
                    '++ Add a new contract Payment entry..
                    newPayment = New DPContractDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    list = GetUniquePaymentIDsFromViewState()
                    id = GetUniquePaymentID(newPayment)

                    '++ Create a new item line for each Payment held..
                    ClearViewState(PAYMENTS_ONLY)
                    '++ Persist the data into view state using the calculated index..
                    list.Insert(0, id)
                    PersistUniquePaymentIDsToViewState(list)

                    newPayments.Add(newPayment)
                    If Not _dpContractDetails Is Nothing Then
                        For Each dpcPayment As DPContractDetail In _dpContractDetails
                            newPayments.Add(dpcPayment)
                        Next
                    End If
                    _dpContractDetails = newPayments

                    hidSelectedTabIndex.Value = PAYMENTS_ONLY
                    tabStrip.ActiveTabIndex = PAYMENTS_ONLY
                End If
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            Finally
                If Not msg.Success Then lblError3.Text = msg.Message
            End Try
        End Sub
#End Region

#Region " ClearDPContract "
        Private Function ClearDPContract() As ErrorMessage
            Dim msg As New ErrorMessage()

            Try
                With _dpContract
                    .ID = 0
                    .ClientBudgetHolderID = 0
                    .Number = ""
                    .AltReference = ""
                    .DateFrom = Utils.VB6_NULL_DATE
                    .DateTo = Utils.VB6_NULL_DATE
                    .EndReasonID = 0
                    .GenericContractGroupID = 0
                    .IsSDS = TriState.UseDefault
                End With
                _dpContractPeriods = Nothing
                _dpContractDetails = Nothing

                msg.Success = True

            Catch ex As Exception
                msg.Success = False
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            Finally
            End Try

            Return msg
        End Function
#End Region

#Region " Page_PreRenderComplete "

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If IsMaintainedExternally Then
                ' if we maintain this contract from an external source then do not allow editing etc

                With _stdBut
                    .AllowDelete = False
                    .AllowEdit = False
                    .AllowNew = False
                End With

                btnAddPayment.Visible = False

            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim tabID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("tabid"))
            Dim js As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim itemIDs As List(Of String)
            Dim forceRefreshParentWindow As Nullable(Of Boolean) = Utils.ToBoolean(Request.QueryString("refreshParent"))

            ' set the notes tab visibility
            tabNotes.Visible = _showNotes

            msg = PopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)
            '++ Set the current active tab based on whether there are any periods being
            '++ edited (determined by the ID of each period loaded)..

            '++ Has a specific tab been passed (due to a screen refresh perhaps)..?
            If Utils.ToInt32(hidSelectedTabIndex.Value) = 0 AndAlso tabID <> 0 Then hidSelectedTabIndex.Value = tabID
            tabStrip.ActiveTabIndex = Utils.ToInt32(hidSelectedTabIndex.Value)

            btnAddPeriod.Enabled = True
            itemIDs = GetUniquePeriodIDsFromViewState()
            For Each itemID As String In itemIDs
                If itemID.StartsWith(UNIQUEID_PREFIX_NEW_PERIOD) Then
                    '++ Also, prevent creation of more than one new period at a time..
                    btnAddPeriod.Enabled = False
                    Exit For
                End If
            Next

            btnAddPayment.Enabled = True
            itemIDs = GetUniquePaymentIDsFromViewState()
            For Each itemID As String In itemIDs
                If itemID.StartsWith(UNIQUEID_PREFIX_NEW_PAYMENT) Then
                    '++ Also, prevent creation of more than one new payment at a time..
                    btnAddPayment.Enabled = False
                    Exit For
                End If
            Next

            '++ Certain fields cannot be changed for existing contracts..
            If _dpContract.ID > 0 Then
                lblClient.Enabled = False
                WebUtils.RecursiveDisable(client.Controls, True)

                lblBudgetHolder.Enabled = False
                WebUtils.RecursiveDisable(budgetholder.Controls, True)
            End If

            If txtContractNum.Enabled And tabStrip.ActiveTabIndex = 0 Then txtContractNum.SetFocus = True
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If IsPopupScreen AndAlso (_refreshParentWindow OrElse (forceRefreshParentWindow.HasValue AndAlso forceRefreshParentWindow.Value = True)) Then
                ' if we have flagged the page to refresh the 
                ' parent window then output some js to do so

                js.Append("if (window.opener.DirectPaymentContractSelector_Refresh) { window.opener.DirectPaymentContractSelector_Refresh(); }")

                If _stdBut.SelectedItemID = 0 Then
                    ' if this value is 0 then we have deleted the record so 
                    ' we might as well close the parent window

                    js.Append("window.close();")

                End If

            End If

            js.AppendFormat("budgetholderID='{0}';", budgetholder.ClientID)
            js.AppendFormat("selectedClientID={0};", CType(client, InPlaceClientSelector).ClientDetailID)
            js.AppendFormat("selectedID={0};", _dpContract.ID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

        End Sub
#End Region

#Region " Page_Unload "
        Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
            If _dpContract IsNot Nothing Then
                ClearDPContract()
                _dpContract = Nothing
            End If
        End Sub
#End Region

#Region "InitialiseNotesSelector"
        Private Sub InitialiseNotesSelector()
            'check if there is a client id
            If Utils.ToInt32(CType(Me.client, InPlaceSelectors.InPlaceClientSelector).ClientDetailID) > 0 Then
                'load the notes control
                With CType(pnlNotes.FindControl("Notes1"), Target.Abacus.Web.Apps.UserControls.NotesSelector)
                    .FilterNoteType = Abacus.Library.Notes.NoteTypes.ServiceUser
                    .FilterNoteTypeChildID = Utils.ToInt32(CType(Me.client, InPlaceSelectors.InPlaceClientSelector).ClientDetailID)
                    .ViewNoteInNewWindow = True
                    .InitControl(Me.Page)
                End With
            End If

        End Sub
#End Region

        ''' <summary>
        ''' Gets the fetched generic contract groups.
        ''' </summary>
        ''' <value>The fetched generic contract groups.</value>
        Private ReadOnly Property FetchedGenericContractGroups() As List(Of GenericContractGroup)
            Get
                If _FetchedGenericContractGroups Is Nothing Then
                    Dim msg As New ErrorMessage()
                    Dim contractGroups As GenericContractGroupCollection = Nothing
                    msg = GenericContractGroup.FetchList(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty, list:=contractGroups)
                    ' throw an exception in the event of no fetch
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    ' init the list 
                    _FetchedGenericContractGroups = New List(Of GenericContractGroup)()
                    For Each contractGroup As GenericContractGroup In contractGroups
                        If (contractGroup.Usage And ContractEndReasonUsage.DPContracts) = ContractEndReasonUsage.DPContracts Then
                            ' if end reason is a dp related reaosn then add to the collection
                            _FetchedGenericContractGroups.Add(contractGroup)
                        End If
                    Next
                End If
                Return _FetchedGenericContractGroups
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance is maintained externally.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance is maintained externally; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property IsMaintainedExternally() As Boolean
            Get
                Return (Not _dpContract Is Nothing AndAlso _dpContract.IsMaintainedExternally = TriState.True)
            End Get
        End Property

        ''' <summary>
        ''' Adds additional controls to the standard buttons control
        ''' </summary>
        ''' <param name="controls">The controls collection to add to.</param>
        Private Sub StdButtonsBack_AddCustomControls(ByRef controls As ControlCollection)

            If IsMaintainedExternally Then
                ' if we should show the padlock then do so 

                Dim imgPadlock As New HtmlImage()

                With imgPadlock
                    .ID = "imgPadlock"
                    .Src = WebUtils.GetVirtualPath("Images/PadLock.gif")
                    .Attributes.Add("title", "The selected Direct Payment Contract is maintained via an Electronic Interface and, as a result, may not be edited")
                End With
                controls.Add(imgPadlock)

            End If

        End Sub

    End Class

End Namespace