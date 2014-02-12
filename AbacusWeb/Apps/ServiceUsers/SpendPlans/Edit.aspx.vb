
Imports System.Text
Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Library.Web.Extensions.AjaxControlToolkit
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Abacus.Web.Apps.Documents.UserControls

Namespace Apps.ServiceUsers.SpendPlans

    ''' <summary>
    ''' Screen used to maintain a Spend Plan.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD     16/05/2011  SDS Issue 700 - Changes to DeleteClicked handler to ensure that data is repopulated correctly when encountering errors.
    '''     MikeVO     26/04/2011  SDS Issue 562 - Error occurs when deleting spend plans (previous change was only a partial fix).
    '''     MoTahir    21/04/2011  SDS Issue 562 - Error occurs when deleting spend plans
    '''     MoTahir    29/04/2011  D11971 - SDS Generic Creditor Notes
    '''     ColinD     30/03/2011  SDS557 - Altered SaveClicked to allow Cash budget categories to be saved i.e. budget categories which disable the service delivered via drop down list.
    '''     ColinD     24/03/2011  D11884A - Added the Financial Tab - Added new controls Finance Code 1/2, Team, Client Group. Moved Care Manager control from Plan tab to new tab.
    '''     PaulW      24/02/2011  D12013 - SDS Setup Costs in Spend Plans
    '''     ColinD     04/01/2011  SDS143 - Enabled validation of Date From field for spend plans which have not yet been saved i.e. setup of validator controls
    '''     ColinD     04/01/2011  SDS416 - Moved indicative budget summary to pre render complete event rather than within find method, always called this way.
    '''     ColinD     29/11/2010  D11964A - Altered budget category drop down to show only budget categories that are associated with a dom service type 
    '''     ColinD     01/11/2010  D11796 - Added ability to handle details, txtUnits js change event
    '''     ColinD     20/10/2010  D11918 - Numerous amendments to all aspects of interface
    '''     MikeVO     01/09/2010  Added validation summary.
    '''     MoTahir    15/09/2010  D11814 - Service User Enquiry
    '''     PaulW      13/07/2010  D11796 - SDS Spend Plans.
    ''' </history>
    Partial Public Class Edit
        Inherits BasePage

        Const VIEWSTATE_KEY_DATA_DETAIL As String = "DetailsDataList"
        Const VIEWSTATE_KEY_COUNTER_NEW_DETAIL As String = "NewDetailCounter"

        Const UNIQUEID_PREFIX_NEW_DETAIL As String = "detailN"
        Const UNIQUEID_PREFIX_UPDATE_DETAIL As String = "detailU"
        Const UNIQUEID_PREFIX_DELETE_DETAIL As String = "detailD"

        Const CTRL_PREFIX_DETAIL_BUDGET_CAT As String = "detailBudgetCat"
        Const CTRL_PREFIX_DETAIL_UNITS As String = "detailUnits"
        Const CTRL_PREFIX_DETAIL_MEASURED_IN As String = "detailMeasuredIn"
        Const CTRL_PREFIX_DETAIL_COST As String = "detailCost"
        Const CTRL_PREFIX_DETAIL_REMOVED As String = "detailRemove"
        Const CTRL_PREFIX_DETAIL_SERVICE_DELIVERED_VIA As String = "detailServiceDeliveredVia"
        Const CTRL_PREFIX_DETAIL_SERVICE_DELIVERED_VIA_HIDDEN As String = "detailServiceDeliveredViaHidden"
        Const CTRL_PREFIX_DETAIL_SERVICE_DETAIL_FREQUENCY As String = "detailServiceDetailFrequency"
        Const CTRL_PREFIX_DETAIL_SERVICE_DETAIL_ANNUAL_UNITS As String = "detailServiceDetailAnnualUnits"
        Const CTRL_PREFIX_DETAIL_SERVICE_DETAIL_GROSS_ANNUAL_COST As String = "detailServiceDetailGrossAnnualCost"
        Private Const _DateFormat As String = "dd/MM/yyyy"

        Private _newIDCounter As Integer
        Private _stdBut As StdButtonsBase
        Private _clientID As Integer
        Private _SpendPlanID As Integer
        Private _startup2JS As StringBuilder = New StringBuilder()
        Private _endReasons As SpendPlanEndReasonCollection = Nothing
        Private _budgetCategories As BudgetCategoryCollection = Nothing
        Private _uoms As DomUnitsOfMeasureCollection = Nothing
        Private _showCostsTab As Boolean
        Private _showDetailsTab As Boolean
        Private _showDocumentsTab As Boolean
        Private _costsNeedReconsidering As Boolean = False
        Private _showBackButton As Boolean
        Private _BudgetCategoriesDropDownsToInit As New Dictionary(Of String, String)()
        Private _detailsJS As New StringBuilder()
        Private _documentsTabAllowed As Boolean = False
        Private _showNotes As Boolean = True
        Private _refreshParentWindow As Boolean = False

#Region " Page_Load "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .hideDebtorRef = True
            End With
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Const SCRIPT_STARTUP As String = "Startup"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.SpendPlans"), "Spend Plan Maintenance")
            Me.ShowValidationSummary = True

            _documentsTabAllowed = SecurityBL.UserHasMenuItem(Me.DbConnection, user.ID, _
                                   Target.Library.Web.ConstantsManager.GetConstant( _
                                   "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"), _
                                   Settings.CurrentApplicationID)

            Dim detailList As List(Of String)
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("id")) > 0 Then
                _SpendPlanID = Utils.ToInt32(Request.QueryString("id"))
            End If
            _clientID = Utils.ToInt32(Request.QueryString("clientID"))


            If Utils.ToInt32(Request.QueryString("closebutton")) = 1 Then
                _showBackButton = False
            Else
                _showBackButton = True
            End If

            ' setup buttons
            With _stdBut
                .EditableControls.Add(tabPlan.Controls)
                .EditableControls.Add(tabFinancial.Controls)
                .EditableControls.Add(tabDetails.Controls)
                .EditableControls.Add(tabCosts.Controls)
                '.EditableControls.Add(tabDocuments.Controls)
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SpendPlan.AddNew"))
                .ShowNew = False
                .AllowFind = False
                .AllowBack = _showBackButton
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SpendPlan.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SpendPlan.Delete"))
                .AuditLogTableNames.Add("SpendPlan")
                .AuditLogTableNames.Add("SpendPlanDetail")
                .AuditLogTableNames.Add("SpendPlanCost")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            ' output javascript
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Date.js"))
            Me.JsLinks.Add("Edit.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.SpendPlans))

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                 String.Format("Edit_spendPlanID='{0}';", _SpendPlanID), True)
            End If

            imgReconsiderWarningText.ImageUrl = WebUtils.GetVirtualPath("Images/WarningHS.png")

            ' re-create the list of attendance (from view state)
            detailList = GetUniqueIDsFromViewState()
            phDetails.Controls.Clear()
            For Each id As String In detailList
                _showDetailsTab = True
                _showDocumentsTab = _documentsTabAllowed
                OutputPlanDetailControls(id, Nothing)
            Next

            InitialiseDocumentSelector()

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim plan As SpendPlan = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim spDetail As SpendPlanDetail = Nothing
            Dim spDetails As SpendPlanDetailCollection
            Dim detailList As List(Of String) = Nothing
            Dim detailToDeleteList As List(Of String) = Nothing
            Dim serviceDeliveredViaControl As DropDownListEx = Nothing
            Dim serviceDeliveredViaControlHidden As HiddenField = Nothing

            For Each id As String In GetUniqueIDsFromViewState()
                ' loop each detail line and set the service delivered via drop down to that of the 
                ' hidden service delivered via field

                serviceDeliveredViaControl = CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_SERVICE_DELIVERED_VIA & id), DropDownListEx)
                If Not serviceDeliveredViaControl Is Nothing Then
                    serviceDeliveredViaControlHidden = CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_SERVICE_DELIVERED_VIA_HIDDEN & id), HiddenField)
                    If Not serviceDeliveredViaControlHidden Is Nothing Then
                        serviceDeliveredViaControl.DropDownList.SelectedValue = serviceDeliveredViaControlHidden.Value
                    Else
                        serviceDeliveredViaControl.Required = False
                        serviceDeliveredViaControl.RequiredValidator.Enabled = False
                    End If
                End If

            Next

            Me.Validate("Save")

            If Me.IsValid Then
                plan = New SpendPlan(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                With plan
                    If e.ItemID > 0 Then
                        plan.Fetch(e.ItemID)
                        If optAwaitingApproval.Checked Then .Status = SpendPlanBL.SpendPlanStatus.AwaitingApproval
                        If optApproved.Checked Then .Status = SpendPlanBL.SpendPlanStatus.Approved
                        If optCancelled.Checked Then .Status = SpendPlanBL.SpendPlanStatus.Cancelled
                    Else
                        .DateTo = DataUtils.MAX_DATE
                        .ClientID = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))
                    End If
                    .Reference = txtReference.Text

                    .DateFrom = dteDateFrom.Text
                    .CareManagerID = Utils.ToInt32(Request.Form(CType(careManager, InPlaceCareManagerSelector).HiddenFieldUniqueID))
                    .FinanceCode1 = txtFinanceCode1.Text
                    .FinanceCode2 = txtFinanceCode2.Text
                    .TeamID = Utils.ToInt32(Request.Form(CType(team, InPlaceTeamSelector).HiddenFieldUniqueID))
                    .ClientGroupID = Utils.ToInt32(Request.Form(CType(clientGroup, InPlaceClientGroupSelector).HiddenFieldUniqueID))
                End With

                spDetails = New SpendPlanDetailCollection
                detailToDeleteList = New List(Of String)
                detailList = GetUniqueIDsFromViewState()
                For Each uniqueID As String In detailList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_DETAIL) Then
                        ' we are deleting
                        detailToDeleteList.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE_DETAIL, String.Empty))
                    Else
                        ' create the empty visit record
                        spDetail = New SpendPlanDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE_DETAIL) Then
                            ' we are updating
                            msg = SpendPlanBL.FetchSpendPlanDetail(Me.DbConnection, _
                                                                   Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_DETAIL, String.Empty)), _
                                                                   currentUser.ExternalUsername, _
                                                                   AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                                   spDetail)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        ' set the Attendance properties
                        With spDetail
                            .BudgetCategoryID = Utils.ToInt32(CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_BUDGET_CAT & uniqueID), DropDownListEx).GetPostBackValue())
                            .ServiceDeliveredVia = Utils.ToInt32(CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_SERVICE_DELIVERED_VIA_HIDDEN & uniqueID), HiddenField).Value)
                            If CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_UNITS & uniqueID), TextBoxEx).Text < Decimal.MaxValue Or _
                                CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_UNITS & uniqueID), TextBoxEx).Text > Decimal.MinValue Then
                                .FrequencyUnits = Convert.ToDecimal(CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_UNITS & uniqueID), TextBoxEx).Text)
                            Else
                                'The unit value entered is invalid
                                lblError.Text = "Please enter valid Unit values."
                                e.Cancel = True
                                PopulateAfterPostback()
                                Exit Sub
                            End If

                            .Frequency = Utils.ToInt32(CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_SERVICE_DETAIL_FREQUENCY & uniqueID), DropDownListEx).GetPostBackValue())

                            .Units = Math.Abs(.Frequency) * .FrequencyUnits

                            'If Utils.ToInt32(CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_SERVICE_DETAIL_FREQUENCY & uniqueID), DropDownListEx).GetPostBackValue()) = -1 Then
                            '    'Once has been selected, so we should not store a frequency
                            '    .Frequency = Nothing

                            'Else
                            '    .Frequency = Utils.ToInt32(CType(phDetails.FindControl(CTRL_PREFIX_DETAIL_SERVICE_DETAIL_FREQUENCY & uniqueID), DropDownListEx).GetPostBackValue())
                            '    .Units = .Frequency * .FrequencyUnits
                            'End If

                            ' add to the collection
                            spDetails.Add(spDetail)
                        End With
                    End If
                Next

                msg = SpendPlanBL.SaveSpendPlan(Me.DbConnection, _
                                                    currentUser.ExternalUsername, _
                                                    AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                    currentUser.ID, _
                                                    ApplicationName.AbacusIntranet, _
                                                    Me.Settings, _
                                                    plan, _
                                                    detailToDeleteList, _
                                                    spDetails)

                ' sort out the validators
                SetupValidators(plan)

                If Not msg.Success Then
                    If msg.Number = SpendPlanBL.ERR_COULD_NOT_SAVE_SPENDPLAN Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        PopulateAfterPostback()
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                Else
                    e.ItemID = plan.ID
                    If IsPopupScreen Then
                        ' if this page is displayed in a popup screen
                        ' then flag to refresh the parent window...or data will 
                        ' become out of sync!
                        _refreshParentWindow = True
                    End If
                    FindClicked(e)
                End If
            Else
                e.Cancel = True
            End If
        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            SetupClientSelector(_clientID)
            SetupCareManagerSelector(0)
            SetupClientGroupSelector(0)
            SetupTeamSelector(0)
            SetupValidators(Nothing)
            _showDetailsTab = False
            _showDetailsTab = False
            _showDocumentsTab = False
            _showNotes = False
            hidSpendPlanDateTo.Value = "Open-Ended"
        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID > 0 Then
                FindClicked(e)
            Else
                SetupClientSelector(0)
                SetupCareManagerSelector(0)
                SetupClientGroupSelector(0)
                SetupTeamSelector(0)
                SetupValidators(Nothing)
                txtReference.Text = String.Empty
                dteDateFrom.Text = String.Empty
                dteDateTo.Text = String.Empty
                dteStatusDate.Text = String.Empty
                optApproved.Checked = False
                optAwaitingApproval.Checked = False
                optCancelled.Checked = False
                ClearViewState()
            End If

        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = SpendPlanBL.DeleteSpendPlan(Me.DbConnection, e.ItemID, currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), currentUser.ID)
            If Not msg.Success Then
                If msg.Number = SdsTransactions.SdsTransactionBL.ErrorCannotDeleteSdsTransactionNumber Then
                    lblError.Text = "ERROR : Cannot delete this record as it is currently in use.<br />"
                    CancelClicked(e)
                    e.Cancel = True
                ElseIf msg.Number = SpendPlanBL.ERR_COULD_NOT_SAVE_SPENDPLAN Then
                    lblError.Text = msg.Message
                    CancelClicked(e)
                    e.Cancel = True
                Else
                    WebUtils.DisplayError(msg)
                End If
            Else
                If IsPopupScreen Then
                    ' if this page is displayed in a popup screen
                    ' then flag to refresh the parent window...or data will 
                    ' become out of sync!
                    _refreshParentWindow = True
                Else
                    ' else is normal window so redirect back to previous page
                    If Not String.IsNullOrEmpty(Request.QueryString("backUrl")) Then
                        Response.Redirect(Request.QueryString("backUrl"))
                    Else
                        ClearViewState()
                        _showNotes = False
                    End If
                End If
            End If

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim plan As New SpendPlan(String.Empty, String.Empty)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim spendPlanDetailList As New List(Of String)
            Dim spendPlanDetails As SpendPlanDetailCollection = Nothing

            msg = SpendPlanBL.FetchSpendPlan(Me.DbConnection, e.ItemID, _
                                                currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                plan, spendPlanDetails)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With plan
                _showDetailsTab = True
                _showDocumentsTab = _documentsTabAllowed
                SetupClientSelector(.ClientID)
                txtReference.Text = .Reference
                dteDateFrom.Text = .DateFrom
                hidSpendPlanDateFrom.Value = .DateFrom
                hidDefaultBudgetPeriod.Value = .BudgetPeriod
                'lblDetailsSummaryDateFrom.Text = hidSpendPlanDateFrom.Value' .DateFrom
                lblDetailsSummaryTotalCost.Text = "£0.00"
                txtFinanceCode1.Text = .FinanceCode1
                txtFinanceCode2.Text = .FinanceCode2

                If Not .DateTo = DataUtils.MAX_DATE Then

                    If .EndReasonID > 0 Then
                        ' if we have an end reason then display it, should normally be set

                        Dim endReason As New SpendPlanEndReason(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                        ' fetch the end reason for this spend plan
                        msg = endReason.Fetch(.EndReasonID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        lblEndReason.Text = String.Format("({0})", endReason.Description)

                    End If

                    dteDateTo.Text = .DateTo
                    hidSpendPlanDateTo.Value = .DateTo
                Else
                    dteDateTo.Text = "Open-Ended"
                    hidSpendPlanDateTo.Value = "Open-Ended"
                End If
                SetupCareManagerSelector(.CareManagerID)
                SetupClientGroupSelector(.ClientGroupID)
                SetupTeamSelector(.TeamID)

                Select Case .Status
                    Case SpendPlanBL.SpendPlanStatus.AwaitingApproval
                        optAwaitingApproval.Checked = True
                    Case SpendPlanBL.SpendPlanStatus.Approved
                        optApproved.Checked = True
                    Case SpendPlanBL.SpendPlanStatus.Cancelled
                        optCancelled.Checked = True
                End Select
                dteStatusDate.Text = .StatusDate
            End With

            'populate the details tab
            ClearViewState()
            spendPlanDetailList = GetUniqueIDsFromViewState()
            phDetails.Controls.Clear()
            For Each spDetail As SpendPlanDetail In spendPlanDetails
                ID = GetDetailUniqueID(spDetail)
                OutputPlanDetailControls(ID, spDetail)
                spendPlanDetailList.Add(ID)
            Next
            PersistUniqueIDsToViewState(spendPlanDetailList)

            'populate cost tab
            msg = PopulateCosts(e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _showCostsTab = (spendPlanDetails.Count > 0)

            SetupValidators(plan)

            'populate document tab
            If _documentsTabAllowed Then CType(docSelector, DocumentSelector).InitControl(Me.Page, plan.ClientID)

        End Sub

#End Region

#Region " PopulateCosts "

        Private Function PopulateCosts(ByVal spendPlanID As Integer) As ErrorMessage


            Dim msg As ErrorMessage
            Dim costs As SpendPlanCostCollection = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try

                msg = SpendPlanBL.FetchSpendPlanCosts(Me.DbConnection, spendPlanID, currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                costs)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                With rptCosts
                    .DataSource = costs
                    .DataBind()
                End With

                'Check if any costs need reconsidering
                For Each cost As SpendPlanCost In costs
                    If cost.Reconsider Then
                        _costsNeedReconsidering = True
                        Exit For
                    End If
                Next

                If costs.Count <= 0 Then _costsNeedReconsidering = False

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region

#Region " SetupValidators "

        ''' <summary>
        ''' Setups the validators for the page.
        ''' </summary>
        ''' <param name="plan">The plan.</param>
        Private Sub SetupValidators(ByVal plan As SpendPlan)

            Dim msg As New ErrorMessage()

            If Not plan Is Nothing Then
                ' we have a spend plan to work with

                Dim numberOfActiveOrPaidContractDetailRecords As Integer = 0
                Dim numberOfActiveOrPaidContractDetailRecordsKey As String = "vsNumberOfActiveOrPaidContractDetailRecordsKey"
                Dim spendPlanDateFromMin As DateTime = DateTime.MinValue
                Dim spendPlanDateFromMinKey As String = "vsSpendPlanDateFromMinKey"
                Dim spendPlanDateFromMax As DateTime = DateTime.MaxValue
                Dim spendPlanDateFromMaxKey As String = "vsSpendPlanDateFromMaxKey"
                Dim spendPlanIdKey As String = "vsSpendPlanIdKey"
                Dim spendPlanID As Integer = Utils.ToInt32(ViewState(spendPlanIdKey))

                If Not IsPostBack OrElse spendPlanID = 0 Then
                    ' if first page hit then get data from db or the 
                    ' spend plan id is 0 (i.e. we have just created a record)

                    ' get the min and max date to dates 
                    msg = SpendPlanBL.GetMinAndMaxSpendPlanDateFromAndDateTo(DbConnection, _
                                                                             plan, _
                                                                             spendPlanDateFromMin, _
                                                                             spendPlanDateFromMax, _
                                                                             New DateTime(), _
                                                                             New DateTime(), _
                                                                             numberOfActiveOrPaidContractDetailRecords)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' add these items into view state so we only do this once per page hit
                    ViewState.Add(spendPlanDateFromMinKey, spendPlanDateFromMin)
                    ViewState.Add(spendPlanDateFromMaxKey, spendPlanDateFromMax)
                    ViewState.Add(numberOfActiveOrPaidContractDetailRecordsKey, numberOfActiveOrPaidContractDetailRecords)
                    ViewState.Add(spendPlanIdKey, spendPlanID)

                Else
                    ' else subsequent hit so fetch from view state

                    spendPlanDateFromMin = ViewState(spendPlanDateFromMinKey)
                    spendPlanDateFromMax = ViewState(spendPlanDateFromMaxKey)
                    numberOfActiveOrPaidContractDetailRecords = ViewState(numberOfActiveOrPaidContractDetailRecordsKey)

                End If

                If numberOfActiveOrPaidContractDetailRecords > 0 Then
                    ' if we have at least one active or paid line then disable deletion of record

                    _stdBut.AllowDelete = False

                End If

                If spendPlanDateFromMin > spendPlanDateFromMax Then
                    ' cap the spend plan min date at the max date

                    spendPlanDateFromMin = spendPlanDateFromMax

                End If

                ' setup range validator for date from field
                With dteDateFrom
                    .MinimumValue = spendPlanDateFromMin.ToString(_DateFormat)
                    .MaximumValue = spendPlanDateFromMax.ToString(_DateFormat)
                    .SetupRangeValidator()
                End With

            Else

                Dim defaultSpendPlansFromDate As Nullable(Of DateTime)

                ' get the default spend plans date from app settings
                msg = SpendPlanBL.GetSpendPlansFromDate(DbConnection, defaultSpendPlansFromDate)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If defaultSpendPlansFromDate Is Nothing OrElse defaultSpendPlansFromDate.HasValue = False Then
                    ' if we have no default date then use the min date possible

                    defaultSpendPlansFromDate = DateTime.MinValue

                End If

                ' setup range validator for date from field
                With dteDateFrom
                    .MinimumValue = defaultSpendPlansFromDate.Value.ToString(_DateFormat)
                    .MaximumValue = DateTime.MaxValue.ToString(_DateFormat)
                    .SetupRangeValidator()
                End With

            End If

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

        Private Sub SetupCareManagerSelector(ByVal careManagerID As Integer)
            With CType(Me.careManager, InPlaceSelectors.InPlaceCareManagerSelector)
                .CareManagerID = careManagerID
                .Required = False
            End With
        End Sub

        Private Sub SetupTeamSelector(ByVal teamID As Integer)
            With CType(Me.team, InPlaceSelectors.InPlaceTeamSelector)
                .TeamID = teamID
                .Required = False
            End With
        End Sub

        Private Sub SetupClientGroupSelector(ByVal clientGroupID As Integer)
            With CType(Me.clientGroup, InPlaceSelectors.InPlaceClientGroupSelector)
                .ClientGroupID = clientGroupID
                .Required = False
            End With
        End Sub

#End Region

#Region " FormatCostsDateFrom "

        Protected Function FormatCostsDateFrom(ByVal theDate As Date) As String
            Return DataUtils.DisplayEndDate(theDate)
        End Function

#End Region

#Region " FormatCostsCurrency "

        Protected Function FormatCostsCurrency(ByVal theValue As Decimal) As String
            Return theValue.ToString("C")
        End Function

#End Region

#Region " rptCosts_ItemDataBound "

        Private Sub rptCosts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptCosts.ItemDataBound
            If CType(e.Item.DataItem, SpendPlanCost).Reconsider = TriState.True Then
                With CType(e.Item.FindControl("imgCostReconsider"), WebControls.Image)
                    .Visible = True
                    .ImageUrl = WebUtils.GetVirtualPath("Images/WarningHS.png")
                End With
                '_costsNeedReconsidering = True
            Else
                CType(e.Item.FindControl("imgCostReconsider"), WebControls.Image).Visible = False
                '_costsNeedReconsidering = False
            End If
        End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            'initialise the notes selector control
            If _showNotes Then
                InitialiseNotesSelector()
            Else
                Notes1.Visible = False
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim js As StringBuilder
            Dim msg As ErrorMessage
            Dim plan As New SpendPlan(String.Empty, String.Empty)
            Dim iBudget As IndicativeBudget = Nothing
            Dim isMostRecentPlan As Boolean
            Dim dateFromMin As DateTime = DateTime.Now
            Dim dateFromMax As DateTime = DateTime.MaxValue

            tabStrip.SetActiveTabByHeaderText(Request.QueryString("displayTab"))
            lblDetailsSummaryDateFrom.Text = hidSpendPlanDateFrom.Value

            tabCosts.Visible = _showCostsTab
            tabDetails.Visible = _showDetailsTab
            tabDocuments.Visible = _showDocumentsTab
            tabNotes.Visible = _showNotes

            If _stdBut.ButtonsMode = StdButtonsMode.AddNew Or _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                optApproved.Disabled = Not SpendPlanBL.CanApproveSpendPlan(Me.DbConnection, currentUser.ID, ApplicationName.AbacusIntranet)
                optCancelled.Disabled = Not SpendPlanBL.CanCancelSpendPlan(Me.DbConnection, currentUser.ID, ApplicationName.AbacusIntranet)
                If hidSpendPlanDateTo.Value = "Open-Ended" Then lblEndReason.Text = String.Empty
                dteDateTo.Text = hidSpendPlanDateTo.Value
                btnTerminate.Visible = False
            Else
                msg = SpendPlanBL.IsTheMostRecentSpendPlanForClient(Me.DbConnection, _SpendPlanID, isMostRecentPlan)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                btnTerminate.Visible = isMostRecentPlan And Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SpendPlan.Terminate"))
            End If

            If _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                If dteDateTo.Text = String.Empty Then
                    lblEndReason.Text = String.Empty
                End If
                WebUtils.RecursiveDisable(CType(client, InPlaceClientSelector).Controls, True)
            End If

            If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                If SpendPlanBL.CanApproveSpendPlan(Me.DbConnection, currentUser.ID, ApplicationName.AbacusIntranet) Then
                    optApproved.Checked = True
                    optAwaitingApproval.Checked = False
                    optCancelled.Checked = False
                Else
                    optApproved.Checked = False
                    optAwaitingApproval.Checked = True
                    optCancelled.Checked = False
                End If
            End If

            btnReconsiderCosts.Visible = _costsNeedReconsidering
            lblReconsideredCosts.Visible = _costsNeedReconsidering

            js = New StringBuilder()
            With js
                ' output a JS collection of (available) budget category IDs together with their "For use with Visit Based Returns" and UoM values
                ' used in the cboBudgetCategory_Change() JS method
                If IsDate(hidSpendPlanDateFrom.Value) Then
                    msg = BudgetCategoryBL.FetchList(Me.DbConnection, _budgetCategories, 0, Nothing, TriState.UseDefault, Nothing)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    .AppendFormat("Edit_detailTotalID='{0}';Edit_budgetCategories=new Collection();", _
                                  lblDetailsSummaryTotalCost.ClientID)
                    If Not _budgetCategories Is Nothing Then
                        For Each bc As BudgetCategory In _budgetCategories
                            Dim rate As ViewableBudgetCategoryRates = Nothing
                            Dim cost As Decimal = 0
                            msg = BudgetCategoryBL.FetchBudgetCategoryRate(Me.DbConnection, bc.ID, CDate(hidSpendPlanDateFrom.Value), Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID)), rate)
                            If Not rate Is Nothing Then
                                cost = rate.ExpenditureUnitRate
                            End If

                            .AppendFormat("Edit_budgetCategories.add({0}, new Array('{1}','{2}', '{3}', '{4}'));", _
                                          bc.ID, bc.DomUnitsOfMeasureID, cost, bc.Type, bc.ServiceCategory)
                        Next
                    End If
                End If
                ' output a JS collection of units of measure IDs together with their Description/Comment values
                ' used in the cboBudgetCategory_Change() JS method
                .Append("Edit_uoms=new Collection();")
                If Not _uoms Is Nothing Then
                    For Each uom As DomUnitsOfMeasure In _uoms
                        .AppendFormat("Edit_uoms.add({0},new Array('{1}','{2}',{3}, {4}));", _
                                      uom.ID, _
                                      uom.Description, _
                                      uom.Comment, _
                                      uom.UnitsDisplayedAsHoursMins.ToString().ToLower(), _
                                      Utils.ToInt32(uom.MinutesPerUnit))
                    Next
                End If

                js.AppendFormat("dteDateToID='{0}_lblReadOnlyContent';", dteDateTo.ClientID)
                js.AppendFormat("lblEndReasonID='{0}';", lblEndReason.ClientID)
                js.AppendFormat("Edit_DefaultBudgetPeriod={0};Edit_dateFrom='{1}';", _
                                        IIf(hidDefaultBudgetPeriod.Value = "", 0, hidDefaultBudgetPeriod.Value), dteDateFrom.Text)
                js.AppendFormat("Edit_dteDateFromID='{0}';", dteDateFrom.ClientID)


                If _BudgetCategoriesDropDownsToInit.Count > 0 Then
                    ' if we have some budget categories to init on the client side then do so

                    Dim vsDetailIds As List(Of String) = GetUniqueIDsFromViewState()    ' get detail ids from vs

                    For Each bcToInit As String In _BudgetCategoriesDropDownsToInit.Keys
                        ' loop each bc to init and check whether it is in vs

                        If vsDetailIds.Contains(bcToInit) Then
                            ' if in vs then init on client side

                            _startup2JS.Append(_BudgetCategoriesDropDownsToInit(bcToInit))

                        End If

                    Next

                End If

                js.Append(_startup2JS.ToString())
                js.Append(_detailsJS.ToString())

                If IsPopupScreen AndAlso _refreshParentWindow Then
                    ' if we have flagged the page to refresh the 
                    ' parent window then output some js to do so

                    js.Append("if (window.opener.SpendPlanSelector_Refresh) { window.opener.SpendPlanSelector_Refresh(); }")

                    If _stdBut.SelectedItemID = 0 Then
                        ' if this value is 0 then we have deleted the record so 
                        ' we might as well close the parent window

                        js.Append("window.close();")

                    End If

                End If

                Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

                divStatus.Visible = False

            End With

            _SpendPlanID = _stdBut.SelectedItemID

            If _SpendPlanID > 0 Then
                ' if we have a spend plan then fetch its indicative budget 

                msg = SpendPlanBL.FetchSpendPlan(DbConnection, _SpendPlanID, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), plan)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' get the indicative budget for this period
                msg = SdsBL.FetchIndicativeBudgetByClientAndDate(DbConnection, plan.ClientID, plan.DateFrom, iBudget)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If Not iBudget Is Nothing Then
                    ' if we have found an indicative budget for this spend plan

                    lblDetailSummaryIndicativeBudget.Text = String.Format("Indicative budget as at {0} = {1}", _
                                                                          plan.DateFrom.ToString(_DateFormat), _
                                                                          Decimal.Round(iBudget.Value, 2).ToString("C"))

                Else
                    ' else we have not found an indicative budget for this spend plan

                    lblDetailSummaryIndicativeBudget.Text = String.Format("An indicative budget covering {0} could not be located", _
                                                                           plan.DateFrom.ToString(_DateFormat))

                End If

            Else
                ' else no spend plan id as yet

                lblDetailSummaryIndicativeBudget.Text = ""

            End If

        End Sub

#End Region

#Region " Details Tab  "

#Region "           ClearViewState "

        Private Sub ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_DATA_DETAIL)
            phDetails.Controls.Clear()
        End Sub

#End Region

#Region "           GetUniqueIDsFromViewState "

        Private Function GetUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_DETAIL) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_DETAIL), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_DETAIL) Is Nothing Then
                _newIDCounter = 0
            Else
                _newIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_DETAIL), Integer)
            End If

            Return list

        End Function

#End Region

#Region "           PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_DETAIL, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_DETAIL, _newIDCounter)
        End Sub

#End Region

#Region "           GetDetailUniqueID "

        Private Function GetDetailUniqueID(ByVal spDetail As SpendPlanDetail) As String

            Dim id As String

            If spDetail.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_DETAIL & _newIDCounter
                _newIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_DETAIL & spDetail.ID
            End If

            Return id

        End Function

#End Region

#Region "           OutputPlanDetailControls "

        Private Sub OutputPlanDetailControls(ByVal uniqueID As String, _
                                                    Optional ByVal spDetail As SpendPlanDetail = Nothing)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim cboBudgetCat As DropDownListEx
            Dim cboServiceDeliveredVia As DropDownListEx
            Dim hdnServiceDeliveredVia As HiddenField
            Dim cboServiceDetailFrequency As DropDownListEx
            Dim lblMeasuredIn As Label
            Dim lblCost As Label
            Dim lblAnnualUnits As Label
            Dim lblGrossAnnualCost As Label
            Dim btnRemove As ImageButton 'HtmlInputImage
            Dim unitsCount As Int16 = 0
            Dim msg As ErrorMessage = Nothing

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_DETAIL) Then

                If spDetail Is Nothing AndAlso uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE_DETAIL) Then
                    spDetail = New SpendPlanDetail(Me.DbConnection, String.Empty, String.Empty)
                    msg = spDetail.Fetch(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_DETAIL, String.Empty))
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                row = New HtmlTableRow()
                row.ID = uniqueID
                phDetails.Controls.Add(row)

                ' Budget Category
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboBudgetCat = New DropDownListEx()
                With cboBudgetCat
                    .ID = CTRL_PREFIX_DETAIL_BUDGET_CAT & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"

                    If Not spDetail Is Nothing Then
                        LoadBudgetCategoryDropdown(cboBudgetCat, spDetail.BudgetCategoryID)
                        .DropDownList.SelectedValue = spDetail.BudgetCategoryID
                    Else
                        LoadBudgetCategoryDropdown(cboBudgetCat, 0)
                    End If

                End With
                cell.Controls.Add(cboBudgetCat)

                ' Service Delivered Via
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboServiceDeliveredVia = New DropDownListEx()
                With cboServiceDeliveredVia
                    .ID = CTRL_PREFIX_DETAIL_SERVICE_DELIVERED_VIA & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    LoadServiceDeliveredViaDropdown(cboServiceDeliveredVia)
                    If Not spDetail Is Nothing AndAlso spDetail.ServiceDeliveredVia > 0 Then
                        .DropDownList.SelectedValue = spDetail.ServiceDeliveredVia
                    End If
                End With
                cell.Controls.Add(cboServiceDeliveredVia)

                ' Hidden service delivered via field as we disable the associated cbo at points
                hdnServiceDeliveredVia = New HiddenField()
                With hdnServiceDeliveredVia
                    .ID = CTRL_PREFIX_DETAIL_SERVICE_DELIVERED_VIA_HIDDEN & uniqueID
                    .Value = cboServiceDeliveredVia.DropDownList.SelectedValue
                End With
                cell.Controls.Add(hdnServiceDeliveredVia)

                '' units
                Dim txtUnits As TextBoxEx
                txtUnits = New TextBoxEx()
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                With txtUnits
                    .ID = CTRL_PREFIX_DETAIL_UNITS & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                    .Width = New Unit(3, UnitType.Em)
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    If Not spDetail Is Nothing Then
                        .Text = spDetail.FrequencyUnits
                    Else
                        .Text = 0
                    End If
                End With
                cell.Controls.Add(txtUnits)

                'Label
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lblMeasuredIn = New Label
                With lblMeasuredIn
                    .ID = CTRL_PREFIX_DETAIL_MEASURED_IN & uniqueID
                End With
                cell.Controls.Add(lblMeasuredIn)

                ' Frequency drop down
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboServiceDetailFrequency = New DropDownListEx()
                With cboServiceDetailFrequency
                    .ID = CTRL_PREFIX_DETAIL_SERVICE_DETAIL_FREQUENCY & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    LoadSpendPlanDetailFrequencyDropdown(cboServiceDetailFrequency)
                    If Not spDetail Is Nothing AndAlso spDetail.Frequency <> 0 Then
                        .DropDownList.SelectedValue = spDetail.Frequency
                    End If
                End With
                cell.Controls.Add(cboServiceDetailFrequency)

                ' Annual Units Label
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lblAnnualUnits = New Label
                With lblAnnualUnits
                    .ID = CTRL_PREFIX_DETAIL_SERVICE_DETAIL_ANNUAL_UNITS & uniqueID
                    If Not spDetail Is Nothing Then
                        .Text = spDetail.Units
                    Else
                        .Text = 0
                    End If
                End With
                cell.Controls.Add(lblAnnualUnits)

                'Label
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lblCost = New Label
                With lblCost
                    .ID = CTRL_PREFIX_DETAIL_COST & uniqueID
                    .Text = " "
                End With
                cell.Controls.Add(lblCost)

                ' Gross Annual Cost Label
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lblGrossAnnualCost = New Label
                With lblGrossAnnualCost
                    .ID = CTRL_PREFIX_DETAIL_SERVICE_DETAIL_GROSS_ANNUAL_COST & uniqueID
                    .Text = "0"
                End With
                cell.Controls.Add(lblGrossAnnualCost)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "middle")
                cell.Style.Add("text-align", "right")
                btnRemove = New ImageButton 'Button
                With btnRemove
                    .ID = CTRL_PREFIX_DETAIL_REMOVED & uniqueID
                    .ImageUrl = WebUtils.GetVirtualPath("images/delete.png")
                    .ValidationGroup = "RemoveDet"
                    AddHandler .Click, AddressOf btnRemoveDetail_Click
                End With

                ' add js to drop down lists
                cboBudgetCat.DropDownList.Attributes.Add("onchange", String.Format("cboBudgetCat_Change('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');", cboBudgetCat.ClientID, txtUnits.ClientID, cboServiceDeliveredVia.ClientID, hdnServiceDeliveredVia.ClientID, cboServiceDetailFrequency.ClientID, lblAnnualUnits.ClientID, lblGrossAnnualCost.ClientID))
                cboServiceDeliveredVia.DropDownList.Attributes.Add("onchange", String.Format("cboServiceDeliveredVia_Change('{0}', '{1}');", cboServiceDeliveredVia.ClientID, hdnServiceDeliveredVia.ClientID, cboServiceDetailFrequency.ClientID, lblAnnualUnits.ClientID, lblGrossAnnualCost.ClientID))
                cboServiceDetailFrequency.DropDownList.Attributes.Add("onchange", String.Format("cboServiceDetailFrequency_Change('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');", cboBudgetCat.ClientID, txtUnits.ClientID, cboServiceDeliveredVia.ClientID, hdnServiceDeliveredVia.ClientID, cboServiceDetailFrequency.ClientID, lblAnnualUnits.ClientID, lblGrossAnnualCost.ClientID))
                _detailsJS.AppendFormat("function {7}_Changed(id) {{ txtUnits_Change('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}'); }};", cboBudgetCat.ClientID, txtUnits.ClientID, cboServiceDeliveredVia.ClientID, hdnServiceDeliveredVia.ClientID, cboServiceDetailFrequency.ClientID, lblAnnualUnits.ClientID, lblGrossAnnualCost.ClientID, txtUnits.ID)

                ' add budget categories to init into the dictionary, this is js init on client side
                If _BudgetCategoriesDropDownsToInit.ContainsKey(uniqueID) Then
                    ' if key already exists then over write it
                    _BudgetCategoriesDropDownsToInit(uniqueID) = String.Format("PrimeMeasuredIn('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", txtUnits.ID, cboBudgetCat.ClientID, txtUnits.ClientID, cboServiceDeliveredVia.ClientID, hdnServiceDeliveredVia.ClientID, cboServiceDetailFrequency.ClientID, lblAnnualUnits.ClientID, lblGrossAnnualCost.ClientID)
                Else
                    ' else doesnt exist so add new
                    _BudgetCategoriesDropDownsToInit.Add(uniqueID, String.Format("PrimeMeasuredIn('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", txtUnits.ID, cboBudgetCat.ClientID, txtUnits.ClientID, cboServiceDeliveredVia.ClientID, hdnServiceDeliveredVia.ClientID, cboServiceDetailFrequency.ClientID, lblAnnualUnits.ClientID, lblGrossAnnualCost.ClientID))
                End If

                cell.Controls.Add(btnRemove)

            End If

        End Sub

#End Region

#Region "           PopulateAfterPostback "

        Private Sub PopulateAfterPostback()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            optApproved.Disabled = Not SpendPlanBL.CanApproveSpendPlan(Me.DbConnection, currentUser.ID, ApplicationName.AbacusIntranet)
            optCancelled.Disabled = Not SpendPlanBL.CanCancelSpendPlan(Me.DbConnection, currentUser.ID, ApplicationName.AbacusIntranet)
            dteDateTo.Text = hidSpendPlanDateTo.Value
            btnTerminate.Visible = False
            If hidSpendPlanDateTo.Value = "Open-Ended" Then lblEndReason.Text = String.Empty
            SetupValidators(Nothing)
        End Sub
#End Region

#Region "           LoadBudgetCategoryDropdown "

        Private Sub LoadBudgetCategoryDropdown(ByVal dropdown As DropDownListEx, ByVal bcID As Integer)

            Dim msg As ErrorMessage
            Dim inList As Boolean

            If _uoms Is Nothing Then
                msg = DomUnitsOfMeasure.FetchList(conn:=Me.DbConnection, list:=_uoms, _
                                                  auditUserName:=String.Empty, auditLogTitle:=String.Empty)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            If bcID = 0 AndAlso dropdown.DropDownList.SelectedValue.Length > 0 Then bcID = dropdown.DropDownList.SelectedValue

            With dropdown
                ' get a list of reasons available to the order
                'If _budgetCategories Is Nothing Then
                msg = BudgetCategoryBL.FetchList(Me.DbConnection, _budgetCategories, 0, Nothing, TriState.False, Nothing)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                'End If
                With .DropDownList
                    .Items.Clear()
                    inList = False
                    For Each bc As BudgetCategory In _budgetCategories
                        If bc.DomServiceTypeID > 0 AndAlso bc.SystemType <> BudgetCategoryBL.BudgetCategorySystemType.DirectPaymentBalancing Then
                            .Items.Add(New ListItem(bc.Description, bc.ID))
                            If bcID > 0 AndAlso bcID = bc.ID Then inList = True
                        End If
                    Next
                    'If the item is not found in the returned list,
                    'This must be a redundant Item, so we need to add it
                    If Not inList AndAlso bcID > 0 Then
                        Dim bc As New BudgetCategory(Me.DbConnection, String.Empty, String.Empty)
                        msg = bc.Fetch(bcID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If bc.SystemType <> BudgetCategoryBL.BudgetCategorySystemType.DirectPaymentBalancing Then
                            .Items.Add(New ListItem(bc.Description, bc.ID))
                            _budgetCategories.Add(bc)
                        End If
                    End If

                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With

            End With

        End Sub

#End Region

#Region "           LoadServiceDeliveredViaDropdown"

        ''' <summary>
        ''' Loads the service delivered via dropdown.
        ''' </summary>
        ''' <param name="dropdown">The dropdown.</param>
        Private Sub LoadServiceDeliveredViaDropdown(ByVal dropdown As DropDownListEx)

            With dropdown.DropDownList
                .Items.Clear()
                For Each serviceDeliveryOption As Byte In [Enum].GetValues(GetType(SpendPlanBL.SpendPlanDetailServiceDeliveredVia))
                    ' loop each item in the SpendPlanServiceDeliveredVia enum

                    .Items.Add(New ListItem( _
                        Utils.SplitOnCapitals([Enum].GetName(GetType(SpendPlanBL.SpendPlanDetailServiceDeliveredVia), serviceDeliveryOption)), _
                        serviceDeliveryOption) _
                    )

                Next
                .Items.Insert(0, New ListItem(String.Empty))
            End With

        End Sub

#End Region

#Region "           LoadSpendPlanDetailFrequencyDropdown"

        ''' <summary>
        ''' Loads the spend plan detail frequency dropdown.
        ''' </summary>
        ''' <param name="dropdown">The dropdown.</param>
        Private Sub LoadSpendPlanDetailFrequencyDropdown(ByVal dropdown As DropDownListEx)

            ' add values of SpendPlanBL.SpendPlanDetailFrequency enum and add into a list
            Dim frequencies As New List(Of Integer)([Enum].GetValues(GetType(SpendPlanBL.SpendPlanDetailFrequency)))

            ' reverse the frequencies so starts with weekly
            frequencies.Reverse()

            ' setup items in the drop down list
            With dropdown.DropDownList
                ' clear any items out of the drop down prior to adding new items
                .Items.Clear()
                'Add the frequency Once.
                .Items.Add(New ListItem("Once", -1))

                For Each frequency As Integer In frequencies
                    ' loop each item in the freqency enum and add to drop down
                    If hidDefaultBudgetPeriod.Value = 3 Then
                        'The default 
                        If [Enum].GetName(GetType(SpendPlanBL.SpendPlanDetailFrequency), frequency) = "Monthly" Then
                            'Do nothing as we dont want to add this to the dropdown
                        ElseIf [Enum].GetName(GetType(SpendPlanBL.SpendPlanDetailFrequency), frequency) = "Quarterly" Then
                            ' add an item to the drop down splitting the name of the member on caps
                            .Items.Add(New ListItem("Quarterly (13 Weeks)", frequency))
                        ElseIf [Enum].GetName(GetType(SpendPlanBL.SpendPlanDetailFrequency), frequency) = "HalfYearly" Then
                            .Items.Add(New ListItem("HalfYearly (26 Weeks)", frequency))
                        ElseIf [Enum].GetName(GetType(SpendPlanBL.SpendPlanDetailFrequency), frequency) = "Annually" Then
                            .Items.Add(New ListItem("Annually (52 Weeks)", frequency))
                        Else
                            ' add an item to the drop down splitting the name of the member on caps
                            .Items.Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(SpendPlanBL.SpendPlanDetailFrequency), frequency)), _
                                                    frequency))
                        End If
                    Else
                        ' add an item to the drop down splitting the name of the member on caps
                        .Items.Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(SpendPlanBL.SpendPlanDetailFrequency), frequency)), _
                                                frequency))
                    End If
                Next
                ' add blank value into the drop down
                .Items.Insert(0, New ListItem(String.Empty))
            End With

        End Sub

#End Region

#Region "           InitialiseDocumentSelector "

        Private Sub InitialiseDocumentSelector()            

            If Not _documentsTabAllowed Then Exit Sub

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

#Region "           btnRemoveDetail_Click "

        Private Sub btnRemoveDetail_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim id As String = CType(sender, ImageButton).ID.Replace(CTRL_PREFIX_DETAIL_REMOVED, String.Empty)

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_DETAIL) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_DETAIL, UNIQUEID_PREFIX_DELETE_DETAIL)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phDetails.Controls.Count - 1
                If phDetails.Controls(index).ID = id Then
                    phDetails.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list)

            PopulateAfterPostback()

        End Sub

#End Region

#Region "           btnAddDetail_Click "

        Private Sub btnAddDetail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddDetail.Click
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newDetail As SpendPlanDetail = New SpendPlanDetail(String.Empty, String.Empty)

            _showDetailsTab = True
            _showDocumentsTab = _documentsTabAllowed

            ' add a new row to the visit list
            id = GetDetailUniqueID(newDetail)
            ' create the controls
            OutputPlanDetailControls(id, newDetail)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState(list)
            PopulateAfterPostback()

            'optApproved.Disabled = Not SpendPlanBL.CanApproveSpendPlan(Me.DbConnection, currentUser.ID, ApplicationName.AbacusIntranet)
            'optCancelled.Disabled = Not SpendPlanBL.CanCancelSpendPlan(Me.DbConnection, currentUser.ID, ApplicationName.AbacusIntranet)
            'dteDateTo.Text = hidSpendPlanDateTo.Value
            'btnTerminate.Visible = False
            'PopulateDropdowns(_SpendPlanID)
            'cboEndReason.SelectPostBackValue()
            'If hidSpendPlanDateTo.Value = "Open-Ended" Then cboEndReason.Enabled = False

            SetupValidators(Nothing)
        End Sub

#End Region

#Region "InitialiseNotesSelector"
        Private Sub InitialiseNotesSelector()
            Dim msg As ErrorMessage
            Dim sPlan As SpendPlan = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            'get the spendplanid

            'get the clientid from spend plan
            msg = SpendPlanBL.FetchSpendPlan(Me.DbConnection, IIf(_stdBut.SelectedItemID = 0, _SpendPlanID, _stdBut.SelectedItemID), _
                                             currentUser.ExternalUsername, _
                                             AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), sPlan)
            'check if there is a client id
            If sPlan.ClientID > 0 Then
                'load the notes control
                With CType(Notes1, Target.Abacus.Web.Apps.UserControls.NotesSelector)
                    .FilterNoteType = Abacus.Library.Notes.NoteTypes.ServiceUser
                    .FilterNoteTypeChildID = sPlan.ClientID
                    .ViewNoteInNewWindow = True
                    .InitControl(Me.Page)
                End With
            End If

        End Sub
#End Region

#End Region

#Region " btnReconsiderCosts_Click "

        Private Sub btnReconsiderCosts_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReconsiderCosts.Click
            Dim msg As ErrorMessage
            Dim plan As New SpendPlan(String.Empty, String.Empty)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim url As StringBuilder = New StringBuilder(Request.Url.ToString)

            'fetch the spend plan
            msg = SpendPlanBL.FetchSpendPlan(Me.DbConnection, _SpendPlanID, _
                                                currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                plan)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            'Reconsider the spend plan costs
            msg = SpendPlanBL.ReconsiderSpendPlanCosts(Me.DbConnection, _
                                               ApplicationName.AbacusIntranet, _
                                                currentUser.ID, _
                                                currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                Me.Settings, _
                                                plan.DateFrom, _
                                                plan.DateTo, _
                                                plan.ClientID, _
                                                plan.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            url.Append("&displayTab=Costs")
            Response.Redirect(url.ToString)

        End Sub

#End Region

    End Class

End Namespace