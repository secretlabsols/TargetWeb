Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Jobs.Exports.FinancialExportInterface.DomCreditors
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Jobs.Core
Imports System.Collections.Generic
Imports Target.Abacus.Web.Apps.InPlaceSelectors

Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' Screen used to view/edit the content of a DP contract period.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD      01/11/2010  D11801 - SDS Issue 336 : Configured Gross/Net drop down to use system setting for defaults when creating new periods
    '''     MikeVO      05/10/2010  UI tidy up following changes to collapsible panel.
    '''     JohnF       24/09/2010  Added MenuItemID for parent form (D11801)
    '''     JohnF       30/07/2010  Created (D11801)
    ''' </history>
    Partial Class EditPeriod
        Inherits Target.Web.Apps.BasePage

        Private Const VALUE_NET As Integer = 0
        Private Const VALUE_GROSS As Integer = 1
        Private Const NEW_PERIOD_TEXT As String = "(NEW PERIOD)"
        Private _stdBut As StdButtonsBase
        Private _dpConPeriod As DPContractPeriod
        Private _DefaultPeriodType As Nullable(Of DPContractBL.BudgetPeriodType)

        Private Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            _stdBut = Me.Master.FindControl("MPContent").FindControl("cp").FindControl("stdButtons1")
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim periodID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim showExpanded As Integer = Target.Library.Utils.ToInt32(Request.QueryString("showexp"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim js As String
            Dim style As New StringBuilder
            Dim msg As ErrorMessage = Nothing

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DirectPaymentContracts"), "")
            Me.UseJQuery = True

            With _stdBut
                .SelectedItemID = periodID
                .ShowNew = False
                .ShowSave = True
                .AllowNew = .ShowNew
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.Edit"))
                .AllowFind = False
                .AllowDelete = .AllowEdit
                If periodID > 0 Then
                    .InitialMode = StdButtonsMode.Fetched
                Else
                    .InitialMode = StdButtonsMode.AddNew
                    '++ The following is used for removing new periods when Cancel is pressed
                    '++ during the editing session..
                    .OnCancelClientClick = String.Format("window.parent.EditPeriod_CancelClicked('{0}');", frameUID)
                End If
                .Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DirectPaymentContracts")
                .EditableControls.Add(pnlPeriod.Controls)
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
            End With

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Sds/DPContracts/EditPeriod.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DPContract))

            '++ Show the form as expanded (this flag exists only after a redirect from a successful Save)..
            If showExpanded = 1 Then cp.Expanded = True
            PopulateScreen()

            js = String.Format("periodID={0};", periodID)
            '++ Re-enable the parent screen 'Add Period' button after a successful Save..
            If showExpanded Then js &= String.Format("window.parent.EditPeriod_SaveClicked('{0}');", frameUID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
        End Sub

#Region " FindClicked "
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            PopulateScreen()
        End Sub
#End Region

#Region " SaveClicked "
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim msg As New ErrorMessage()
            Dim trans As SqlTransaction = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim warning As String = String.Empty

            Try
                msg.Success = True
                Me.Validate("AddPeriod")

                If Me.IsValid() Then
                    '++ Populate a DPContractPeriod object with the current field sttings..
                    msg = GetPeriodFromScreen()
                    If msg.Success Then
                        trans = SqlHelper.GetTransaction(Me.DbConnection)
                        msg = DPContractBL.SavePeriod(trans, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), _dpConPeriod, warning)
                    End If
                    If msg.Success Then
                        '++ Save, then reload the direct payment contract..
                        trans.Commit()
                        trans = Nothing

                        '++ Refresh the parent contract screen, since there may be changes
                        '++ to more than one contract period to be shown..
                        Me.ClientScript.RegisterStartupScript( _
                            Me.GetType(), _
                            "SaveDPContractPeriod", _
                            String.Format("window.parent.EditPeriod_AfterSuccessfulSave('{0}');", warning), _
                            True)
                    Else
                        If trans IsNot Nothing Then
                            SqlHelper.RollbackTransaction(trans)
                            trans = Nothing
                        End If
                        e.Cancel = True
                    End If
                End If
            Catch ex As Exception
                msg.Success = False
                msg.Message = ex.Message
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
                If trans IsNot Nothing Then
                    SqlHelper.RollbackTransaction(trans)
                    trans = Nothing
                End If
            Finally
                If Not msg.Success Then
                    lblError.Text = msg.Message
                Else
                    lblError.Text = warning
                End If
            End Try
        End Sub
#End Region

#Region " CancelClicked "
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            Dim periodID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))

            If periodID = 0 Then
                '++ Item hasn't been saved before, so remove this DP period
                '++ via the OnCancelClientClick javascript..
            Else
                '++ ..is an existing item, so reload..
                FindClicked(e)
            End If
            lblError.Text = ""
        End Sub
#End Region

#Region " DeleteClicked "
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim periodID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim trans As SqlTransaction = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)
                msg = DPContractBL.DeletePeriod(trans, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), periodID)
                If msg.Success Then
                    '++ Save, then reload the direct payment contract..
                    trans.Commit()
                Else
                    If trans IsNot Nothing Then
                        SqlHelper.RollbackTransaction(trans)
                        trans = Nothing
                    End If
                    e.Cancel = True
                End If
            Catch ex As Exception
                msg.Success = False
                msg.Message = ex.Message
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
                If trans IsNot Nothing Then
                    SqlHelper.RollbackTransaction(trans)
                    trans = Nothing
                End If
            Finally
                If Not msg.Success Then
                    lblError.Text = msg.Message
                Else
                    lblError.Text = ""
                    '++ Remove this period (i.e. the entire ASPX) from the periods tab..
                    Me.ClientScript.RegisterStartupScript( _
                        Me.GetType(), _
                        "DeletePeriod", _
                        "window.parent.EditPeriod_AfterSuccessfulSave();", _
                        True)
                End If
            End Try
        End Sub
#End Region

#Region " FillDropdownGrossNet "
        Private Sub FillDropdownGrossNet(Optional ByVal currentValue As Boolean = True)

            Dim cboItem As ListItem = Nothing

            If cboGrossNet.DropDownList.Items.Count = 0 Then
                cboItem = New ListItem("Net", VALUE_NET.ToString)
                cboGrossNet.DropDownList.Items.Add(cboItem)
                cboItem = New ListItem("Gross", VALUE_GROSS.ToString)
                cboGrossNet.DropDownList.Items.Add(cboItem)
            End If

            For Each cboItem In cboGrossNet.DropDownList.Items
                If CBool(cboItem.Value) = currentValue Then
                    cboGrossNet.DropDownList.SelectedValue = IIf(currentValue, VALUE_GROSS, VALUE_NET).ToString
                    Exit For
                End If
            Next

        End Sub
#End Region

#Region " PopulateScreen "
        Private Sub PopulateScreen()
            Dim periodID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim existingContract As DPContract = Nothing
            Dim existingPeriods As DPContractPeriodCollection = Nothing
            Dim existingDetails As DPContractDetailCollection = Nothing

            Try
                If _dpConPeriod Is Nothing Then
                    _dpConPeriod = New DPContractPeriod(Me.DbConnection, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))
                End If
                msg.Success = True
                If periodID <> _dpConPeriod.ID AndAlso periodID <> 0 Then
                    msg = DPContractBL.FetchPeriod(Me.DbConnection, periodID, _dpConPeriod)
                End If
                _stdBut.SelectedItemID = periodID

                If msg.Success Then
                    Dim sDateFrom As String, sDateTo As String, dtFrom As Date, dtTo As Date
                    With _dpConPeriod
                        If .ID = 0 AndAlso .DPContractID = 0 Then
                            .DPContractID = Target.Library.Utils.ToInt32(Request.QueryString("dpcid"))
                            '++ For this new period, determine the From and To dates based on the 
                            '++ most recent saved period for this contract (if any), and the 
                            '++ contract itself..
                            If .DPContractID <> 0 Then
                                msg = DPContractBL.Fetch(Me.DbConnection, .DPContractID, existingContract, _
                                                       existingPeriods, existingDetails)
                                dtFrom = existingContract.DateFrom
                                dtTo = existingContract.DateTo
                                For Each period As DPContractPeriod In existingPeriods
                                    If period.DateFrom > dtFrom Then dtFrom = period.DateFrom
                                    If period.DateTo > dtFrom AndAlso period.DateTo <> DataUtils.MAX_DATE Then
                                        dtFrom = period.DateTo
                                    End If
                                Next
                                If existingPeriods IsNot Nothing AndAlso existingPeriods.Count > 0 Then
                                    dtFrom = DateAdd(DateInterval.Day, 1, dtFrom)
                                End If
                                If dtFrom > dtTo Then dtTo = dtFrom
                                .DateFrom = dtFrom
                                .DateTo = dtTo
                                '++ Default the Gross/Net for a new period to 'gross'..
                                If DefaultPeriodType = DPContractBL.BudgetPeriodType.Gross Then
                                    .Gross = True
                                Else
                                    .Gross = False
                                End If
                            End If
                        End If
                        If Utils.IsDate(.DateFrom) AndAlso .DateFrom <> DataUtils.MAX_DATE Then
                            dteDateFrom.Text = .DateFrom.ToString("dd/MM/yyyy")
                            sDateFrom = .DateFrom.ToString("dd MMM yyyy")
                        Else
                            dteDateFrom.Text = ""
                            sDateFrom = "(open-ended)"
                        End If
                        If Utils.IsDate(.DateTo) AndAlso .DateTo <> DataUtils.MAX_DATE Then
                            txtDateTo.Text = .DateTo.ToString("dd/MM/yyyy")
                            sDateTo = .DateTo.ToString("dd MMM yyyy")
                        Else
                            txtDateTo.Text = ""
                            sDateTo = "(open-ended)"
                        End If
                        hidDateTo.Value = txtDateTo.Text
                        CType(team, InPlaceTeamSelector).TeamID = .TeamID
                        CType(clientGroup, InPlaceClientGroupSelector).ClientGroupID = .ClientGroupID
                        CType(txtFinCode1, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode1
                        CType(txtFinCode2, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode2
                    End With
                    With cp
                        If _dpConPeriod.ID = 0 Then
                            .HeaderLinkText = String.Format("{0} ({1}) - {2} {3}", sDateFrom, IIf(_dpConPeriod.Gross, "Gross", "Net"), sDateTo, NEW_PERIOD_TEXT)
                            .Expanded = True
                        Else
                            .HeaderLinkText = String.Format("{0} ({1}) - {2}", sDateFrom, IIf(_dpConPeriod.Gross, "Gross", "Net"), sDateTo)
                        End If                        
                        .CollapsedJS = String.Format("Toggle('{0}');", frameUID)
                        .ExpandedJS = .CollapsedJS
                    End With
                    FillDropdownGrossNet(_dpConPeriod.Gross)
                End If
            Catch ex As Exception
                msg.Success = False
                msg.Message = ex.Message
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            End Try
            If Not msg.Success Then lblError.Text = msg.Message
        End Sub
#End Region

#Region " GetPeriodFromScreen "
        Private Function GetPeriodFromScreen() As ErrorMessage
            Dim periodID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim dpcID As String = Target.Library.Utils.ToString(Request.QueryString("dpcid"))
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg.Success = True
            Try
                If _dpConPeriod Is Nothing Then
                    _dpConPeriod = New DPContractPeriod(Me.DbConnection, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))
                End If
                msg.Success = True
                If periodID <> _dpConPeriod.ID AndAlso periodID <> 0 Then
                    msg = DPContractBL.FetchPeriod(Me.DbConnection, periodID, _dpConPeriod)
                End If
                _dpConPeriod.DPContractID = dpcID

                '++ Store the current field settings into the contract period object..
                With _dpConPeriod
                    .AuditLogOverriddenParentID = dpcID
                    If Utils.IsDate(dteDateFrom.GetPostBackValue()) Then
                        .DateFrom = Convert.ToDateTime(dteDateFrom.GetPostBackValue())
                        dteDateFrom.Text = dteDateFrom.GetPostBackValue()
                    End If
                    txtDateTo.Text = ""
                    If Utils.IsDate(hidDateTo.Value) AndAlso Convert.ToDateTime(hidDateTo.Value) <> DataUtils.MAX_DATE Then
                        .DateTo = Convert.ToDateTime(hidDateTo.Value)
                        txtDateTo.Text = hidDateTo.Value
                    End If
                    .Gross = (Utils.ToInt32(cboGrossNet.GetPostBackValue()) = VALUE_GROSS)
                    FillDropdownGrossNet(cboGrossNet.GetPostBackValue())
                    .TeamID = Utils.ToInt32(Request.Form(CType(team, InPlaceTeamSelector).HiddenFieldUniqueID))
                    .ClientGroupID = Utils.ToInt32(Request.Form(CType(clientGroup, InPlaceClientGroupSelector).HiddenFieldUniqueID))
                    Dim fcRec As FinanceCode = New FinanceCode(Me.DbConnection)
                    Dim fcID As Integer
                    fcID = Utils.ToInt32(Request.Form(CType(txtFinCode1, InPlaceFinanceCodeSelector).HiddenFieldUniqueID))
                    If fcID > 0 Then
                        msg = fcRec.Fetch(fcID)
                        If msg.Success Then .FinanceCode1 = fcRec.Code
                    Else
                        .FinanceCode1 = CType(txtFinCode1, InPlaceFinanceCodeSelector).FinanceCodeText
                    End If
                    CType(txtFinCode1, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode1
                    fcID = Utils.ToInt32(Request.Form(CType(txtFinCode2, InPlaceFinanceCodeSelector).HiddenFieldUniqueID))
                    If fcID > 0 Then
                        msg = fcRec.Fetch(fcID)
                        If msg.Success Then .FinanceCode2 = fcRec.Code
                    Else
                        .FinanceCode2 = CType(txtFinCode2, InPlaceFinanceCodeSelector).FinanceCodeText
                    End If
                    CType(txtFinCode2, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode2
                End With
            Catch ex As Exception
                msg.Success = False
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            End Try

            Return msg
        End Function
#End Region

#Region " Page_PreRenderComplete "
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim dpPayments As DPPaymentCollection = New DPPaymentCollection()
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim hasPayments As Boolean

            '++ Disable the Gross/Net dropdown if payments have been made against this period..
            hasPayments = False
            msg = DirectPaymentPaymentsBL.FetchList(Me.DbConnection, dpPayments, _dpConPeriod.DPContractID)
            If msg.Success Then
                If dpPayments IsNot Nothing AndAlso dpPayments.Count > 1 Then
                    For Each payment As Target.Abacus.Library.DataClasses.DPPayment In dpPayments
                        If (payment.DateFrom > _dpConPeriod.DateFrom) And (payment.DateFrom < _dpConPeriod.DateTo) Then
                            hasPayments = True
                            Exit For
                        End If
                    Next
                End If
            End If
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the default period type.
        ''' </summary>
        ''' <value>The default type of the period.</value>
        Public ReadOnly Property DefaultPeriodType() As Nullable(Of DPContractBL.BudgetPeriodType)
            Get
                If _DefaultPeriodType Is Nothing OrElse _DefaultPeriodType.HasValue = False Then
                    ' if we havent already fetched the default period type then do so
                    Dim defaultPT As DPContractBL.BudgetPeriodType = DPContractBL.BudgetPeriodType.Gross
                    Dim msg As ErrorMessage = DPContractBL.GetDefaultBudgetPeriodType(DbConnection, defaultPT)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _DefaultPeriodType = defaultPT
                End If
                Return _DefaultPeriodType
            End Get
        End Property

#End Region

    End Class
End Namespace