Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Abacus.Library.DomProviderInvoice
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Configuration.ConfigurationManager
Imports System.Web.UI
Imports System.Text
Imports System.Collections.Generic
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Collections

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen used to Maintain Dom Provider Invoice Batch Suspensions.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir        04/03/2010  A4WA#6684
    '''     Mo Tahir       18/01/2010  Created (D11939)
    ''' </history>
    Partial Class BatchSuspensions
        Inherits Target.Web.Apps.BasePage

        Private Const SESSION_NEW_DOM_PROVIDER_INVOICE As String = "NewDomProviderInvoiceData"

        Private _stdBut As StdButtonsBase
        Private _invoiceID As Integer
        Private _estabID As Integer
        Private _dpi As DomProviderInvoiceBL
        Private WithEvents _btnAddComment As Button = New Button
        Private _InvoiceStatus As DomProviderInvoiceStatus
        Private _selectedDomProviderInvoiceID, _batchID, _providerID, _
        _domContractID, _clientID, _additionalFilter As Integer

        Private _providerName As String = Nothing
        Private _domContractNum As String = Nothing
        Private _clientName As String = Nothing
        Private _weFrom As String = Nothing
        Private _weTo As String = Nothing
        Private _invoiceNumber As String = Nothing
        Private _invoiceRef As String = Nothing
        Private _statusDateFrom As String = Nothing
        Private _statusDateTo As String = Nothing
        Private _exclude As String = Nothing
        Private _invNumFilter As String = Nothing
        Private _invRefFilter As String = Nothing

        Dim _statusIsUnpaid, _statusIsAuthorised, _statusIsPaid, _statusIsSuspended As Boolean

        Const NO_ADDITIONAL_FILTERING_DESCRIPTION As String = "No Additional Filtering"
        Const COST_EXCEEDED_DESCRIPTION As String = "Cost Exceeded"
        Const COST_EXCEEDED_WITHIN_TOLERANCE_DESCRIPTION As String = "Cost Exceeded Within Tolerance"
        Const MANUALLY_SUSPENDED_DESCRIPTION As String = "Manually Suspended"
        Const UNITS_EXCEEDED_DESCRIPTION As String = "Units Exceeded"
        Const UNITS_EXCEEDED_WITHIN_COST_DESCRIPTION As String = "Units Exceeded Within Cost"
        Const UNPLANNED_SERVICE_DESCRIPTION As String = "Unplanned Service"
        Const EX_FROM_CREDITORS_SHOW_ALL_DESCRIPTION As String = "Show All Invoices"
        Const EX_FROM_CREDITORS_SHOW_NON_CREDITORS_EXCLUDED_DESCRIPTION As String = "Show non creditors-excluded only"
        Const EX_FROM_CREDITORS_SHOW_CREDITORS_EXCLUDED_DESCRIPTION As String = "Show creditors-excluded only"
        Const CTRL_CHK_SUFFIX As String = "$chk"
        Const UNITS_EXCEEDED_WITHIN_COST_COLOUR As String = "Orange"
        Const COST_EXCEEDED_COLOUR As String = "Red"
        Const UNPLANNED_SERVICE_COLOUR As String = "Yellow"
        Const NO_BACKGROUND_COLOUR As String = "White"

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

        End Sub

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoiceSuspensions"), "Domiciliary Provider Invoice Batch Suspensions")
            'Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryProviderInvoiceSuspensions.BatchSuspensions"), "Domiciliary Provider Invoice Batch Suspensions")
            Dim msg As ErrorMessage = Nothing
            Dim style As New StringBuilder
            Dim sysInfo As SystemInfo
            Dim currentUser As WebSecurityUser

            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add("BatchSuspensions.js")

            With _stdBut
                .AllowDelete = False
                .AllowEdit = False
                .AllowFind = False
                .AllowNew = False
                .AllowBack = True
            End With

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProviderInvoiceSuspensionReasonType))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProviderInvoiceSuspensionReasonAutoType))

            sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
            currentUser = SecurityBL.GetCurrentUser()

            'Access COM DomProviderInvoce Instantiate
            _dpi = New DomProviderInvoiceBL(ConnectionStrings("Abacus").ConnectionString, _
                    sysInfo.LicenceNo, currentUser.ExternalUsername, currentUser.ExternalUserID, _
                    AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings).Substring(0, 50))

            'Populate the screen
            msg = PopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            cboComment.DropDownList.Attributes.Add("onchange", "cboComment_Change();")

        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim js As String
            Dim hasUnsuspendPermission As Boolean = False
            Dim hasSuspendPermission As Boolean = False
            Dim hasAddCommentPermission As Boolean = False
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            hasSuspendPermission = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryProviderInvoiceSuspensions.Suspend"), _
                    Me.Settings.CurrentApplicationID)

            hasUnsuspendPermission = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryProviderInvoiceSuspensions.Unsuspend"), _
                    Me.Settings.CurrentApplicationID)

            hasAddCommentPermission = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryProviderInvoiceSuspensions.AddComment"), _
                    Me.Settings.CurrentApplicationID)

            'In Abacus you cant add a comment to invoices that have been retracted.
            'it has been decided that in intranet that this restriction is to be removed.
            If _statusIsPaid Then
                optSuspend.Disabled = True
                optUnsuspend.Disabled = True
                optAddComment.Disabled = True
                _btnAddComment.Enabled = False
            End If

            'Select the first available option button
            If _statusIsSuspended Then
                optAddComment.Disabled = Not hasAddCommentPermission
                optAddComment.Checked = True
                optSuspend.Disabled = True
                optUnsuspend.Disabled = Not hasUnsuspendPermission
            ElseIf _statusIsUnpaid Or _statusIsAuthorised Then
                optSuspend.Disabled = Not hasSuspendPermission
                optSuspend.Checked = True
                optAddComment.Disabled = True
                optUnsuspend.Disabled = True
            End If

            If optAddComment.Disabled And optSuspend.Disabled And optUnsuspend.Disabled Then
                _btnAddComment.Enabled = False
                cboComment.Enabled = False
            End If

            lblSuspend.Disabled = optSuspend.Disabled
            lblUnsuspend.Disabled = optUnsuspend.Disabled
            lblAddComment.Disabled = optAddComment.Disabled

            js = String.Format("btnAddComment_ClientID='{0}';cboComment_ClientID='{1}';statusIsSuspended='{2}';statusIsPaid='{3}';chkChecked_ClientID='{4}'", _
                                    _btnAddComment.ClientID, cboComment.ClientID, IIf(_statusIsSuspended, "true", "false"), IIf(_statusIsPaid, "true", "false"), chkCheckAll.ClientID)
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "BatchSuspensions.Startup", _
                            Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

#End Region

#Region " PrimeDpiClass "

        Private Function PrimeDpiClass(ByVal invoiceID As Integer, ByVal establishmentID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim newDpiData As NewDomProviderInvoiceData

            If invoiceID > 0 Then
                ' editing an existing invoice
                ' re-fetch the invoice
                msg = _dpi.Fetch(invoiceID, establishmentID)
            Else
                ' creating a new invoice
                newDpiData = FetchNewDpiData()
                ' re-call AddNew() to re-create the suggested invoice
                msg = _dpi.AddNew(newDpiData, DomProviderInvoiceStyle.SummaryLevel)
            End If

            Return msg

        End Function

#End Region

#Region " FetchNewDpiData "

        Private Function FetchNewDpiData() As NewDomProviderInvoiceData
            Return Session(SESSION_NEW_DOM_PROVIDER_INVOICE)
        End Function

#End Region

#Region " PopulateScreen "

        Private Function PopulateScreen() As ErrorMessage

            Dim msg As ErrorMessage = Nothing
            Dim suspHistory As List(Of DomProviderInvoiceSuspensionHistory) = Nothing

            Dim result As Target.Abacus.Web.Apps.WebSvc.FetchDomProviderInvoiceListResult = New Target.Abacus.Web.Apps.WebSvc.FetchDomProviderInvoiceListResult
            Dim totalRecords As Integer
            Dim pageSize As Integer = 0
            Dim page As Integer = 1
            Dim est As Establishment = Nothing
            Dim domCon As DomContract = Nothing
            Dim client As ClientDetail = Nothing
            Dim appSettingCollection As ApplicationSettingCollection = Nothing

            GetQueryStringParameters()

            Try
                If _providerID > 0 Then
                    est = New Establishment(Me.DbConnection)
                    With est
                        .Fetch(_providerID)
                        lblProvider.Text = .AltReference + " : " + .Name
                    End With
                Else
                    lblProvider.Text = "All"
                End If

                If _domContractID > 0 Then
                    domCon = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                    With domCon
                        .Fetch(_domContractID)
                        lblContract.Text = .Number + " : " + .Title
                    End With
                Else
                    lblContract.Text = "All"
                End If

                If _clientID > 0 Then
                    client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
                    With client
                        .Fetch(_clientID)
                        lblServiceUser.Text = .Reference + " : " + .Name
                    End With
                Else
                    lblServiceUser.Text = "All"
                End If

                If _invoiceNumber <> "" AndAlso _invoiceNumber <> "null" Then
                    lblInvoiceNumber.Text = _invoiceNumber
                Else
                    lblInvoiceNumber.Text = "All"
                End If

                If _invoiceRef <> "" AndAlso _invoiceRef <> "null" Then
                    lblInvoiceRef.Text = _invoiceRef
                Else
                    lblInvoiceRef.Text = "All"
                End If

                If _weFrom <> "" AndAlso _weFrom <> "null" Then
                    lblWEDateRange.Text = "From " + _weFrom
                End If

                If _weTo <> "" AndAlso _weTo <> "null" Then
                    lblWEDateRange.Text = lblWEDateRange.Text + " To" + _weTo
                ElseIf _weFrom = "" Or _weFrom = "null" And _
                _weTo = "" Or _weTo = "null" Then
                    lblWEDateRange.Text = "All"
                End If

                lblInvoiceStatus.Text = "All"

                If _statusIsUnpaid Then
                    lblInvoiceStatus.Text = "Unpaid"
                End If

                If _statusIsAuthorised Then
                    lblInvoiceStatus.Text = IIf(lblInvoiceStatus.Text = "All", "Authorised", lblInvoiceStatus.Text + ", Authorised")
                End If

                If _statusIsPaid Then
                    lblInvoiceStatus.Text = IIf(lblInvoiceStatus.Text = "All", "Paid", lblInvoiceStatus.Text + ", Paid")
                End If

                If _statusIsSuspended Then
                    lblInvoiceStatus.Text = IIf(lblInvoiceStatus.Text = "All", "Suspended", lblInvoiceStatus.Text + ", Suspended")
                End If

                lblAdditionalFilters.Text = GetAdditionalFilterDescription(CByte(_additionalFilter))

                If _exclude <> "" Then
                    lblExcluded.Text = GetExcludeDescription(_exclude)
                End If

                If _statusDateFrom <> "" AndAlso _statusDateFrom <> "null" Then
                    lblDateRange.Text = "From " + _statusDateFrom
                End If

                If _statusDateTo <> "" AndAlso _statusDateTo <> "null" Then
                    lblDateRange.Text = lblDateRange.Text + " To" + _statusDateTo
                ElseIf _statusDateFrom = "" Or _statusDateFrom = "null" And _
                _statusDateTo = "" Or _statusDateTo = "null" Then
                    lblDateRange.Text = "All"
                End If

                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                ' get the row limit

                ApplicationSetting.FetchList(conn:=Me.DbConnection, list:=appSettingCollection, auditUserName:=String.Empty, _
                    auditLogTitle:=String.Empty, settingKey:="Apps.Dom.ProviderInvoices.BatchSuspensions.RowLimit")

                If appSettingCollection.Count = 1 Then
                    pageSize = appSettingCollection(0).SettingValue
                End If

                'If Not Settings found
                If pageSize = 0 Then
                    pageSize = 100
                End If

                If Not IsPostBack Then
                msg = DomContractBL.FetchDomProviderInvoiceList( _
                 Me.DbConnection, page, pageSize, _selectedDomProviderInvoiceID, _batchID, _
                 _providerID, _providerName, _domContractID, _domContractNum, _
                 _clientID, _clientName, _
                 _weFrom, _weTo, _invoiceNumber, _invoiceRef, _statusDateFrom, _statusDateTo, _
                 _statusIsUnpaid, _statusIsAuthorised, _statusIsPaid, _statusIsSuspended, _
                 _exclude, _invNumFilter, _invRefFilter, _additionalFilter, totalRecords, result.Invoices)
                If Not msg.Success Then
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                    InvoicesInMemory = result.Invoices
                End If


                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                End With

                If Not IsPostBack Then
                rptInvoices.DataSource = result.Invoices
                Else
                    rptInvoices.DataSource = InvoicesInMemory
                End If

                rptInvoices.DataBind()

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End Try

            _dpi.Dispose()

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " cboComment_AfterDropDownControlAdded "

        Private Sub cboComment_AfterDropDownControlAdded(ByVal sender As DropDownListEx) Handles cboComment.AfterDropDownControlAdded
            Dim space As Literal = New Literal

            space.Text = "&nbsp;"
            sender.Controls.Add(space)
            _btnAddComment.Text = "Update"
            sender.Controls.Add(_btnAddComment)
        End Sub

#End Region

#Region "  _btnAddComment_Click "

        Private Sub _btnAddComment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _btnAddComment.Click

            Dim allKeysList As String()
            Dim changedControlsList As ArrayList = New ArrayList
            Dim chk As CheckBox
            Dim provInvoice As DomProviderInvoice = New DomProviderInvoice(Me.DbConnection)
            Dim establishmentID As Integer = 0
            Dim msg As ErrorMessage = Nothing
            Dim newResult As List(Of Target.Abacus.Library.ViewableDomProviderInvoice) = Nothing
            Dim result As List(Of Target.Abacus.Library.ViewableDomProviderInvoice) = Nothing
            Dim found As Boolean
            Dim sysInfo As SystemInfo
            Dim currentUser As WebSecurityUser

            sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
            currentUser = SecurityBL.GetCurrentUser()

            _dpi = New DomProviderInvoiceBL( _
             ConnectionStrings("Abacus").ConnectionString, _
             sysInfo.LicenceNo, _
             currentUser.ExternalUsername, _
              currentUser.ExternalUserID, _
             AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings).Substring(0, 50) _
            )

            allKeysList = Request.Form.AllKeys()
            For Each key As String In allKeysList
                If key.Contains(CTRL_CHK_SUFFIX) Then
                    chk = Page.FindControl(key)
                     If Not chk Is Nothing AndAlso chk.Checked AndAlso Not chk.Attributes("internalid") Is Nothing Then
                        changedControlsList.Add(chk.Attributes("internalid"))
                    End If
                End If
            Next

            For Each invoiceID As Integer In changedControlsList
                If invoiceID > 0 Then
                    With provInvoice
                        .Fetch(invoiceID)
                        establishmentID = .ProviderID
                    End With
                End If

                If invoiceID > 0 And establishmentID > 0 Then
                    ' fetch
                    msg = PrimeDpiClass(invoiceID, establishmentID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                If Utils.ToInt32(cboComment.GetPostBackValue) <> 0 Then
                    msg = _dpi.AmendSuspension(cboComment.GetPostBackValue)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
            Next

            'remove the invoices unsuspended from the copy in memory
            If IsPostBack And optUnsuspend.Checked Then
                result = InvoicesInMemory
                newResult = New List(Of Target.Abacus.Library.ViewableDomProviderInvoice)
                For Each inv As ViewableDomProviderInvoice In result
                    For Each checkedKey As Integer In changedControlsList
                        If inv.ID = checkedKey Then
                            found = True
                            Exit For
                        End If
                    Next
                    If Not found Then
                        newResult.Add(inv)
                    End If
                    found = False
                Next
                InvoicesInMemory = newResult
            End If
           

            If IsPostBack Then
                If _statusIsAuthorised Or _statusIsUnpaid Then
                    _statusIsAuthorised = False
                    _statusIsUnpaid = False
                    _statusIsSuspended = True
                    _statusIsPaid = False
                    'ElseIf _statusIsSuspended And Not optAddComment.Checked Then
                    '    _statusIsAuthorised = False
                    '    _statusIsUnpaid = True
                    '    _statusIsSuspended = False
                    '    _statusIsPaid = False
                End If

                Me.ViewState.Remove("statusIsAuthorised")
                Me.ViewState.Remove("statusIsUnpaid")
                Me.ViewState.Remove("statusIsSuspended")
                Me.ViewState.Remove("statusIsPaid")

                Me.ViewState.Add("statusIsAuthorised", _statusIsAuthorised)
                Me.ViewState.Add("statusIsUnpaid", _statusIsUnpaid)
                Me.ViewState.Add("statusIsSuspended", _statusIsSuspended)
                Me.ViewState.Add("statusIsPaid", _statusIsPaid)
            End If

            PopulateScreen()

        End Sub

#End Region

#Region " Render "

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

            ' output postback javascript for add comment button
            Dim options As PostBackOptions = New PostBackOptions(_btnAddComment)
            If Not options Is Nothing Then
                Page.ClientScript.RegisterForEventValidation(options)
            End If
            Page.ClientScript.RegisterForEventValidation(_btnAddComment.UniqueID)
            MyBase.Render(writer)


        End Sub

#End Region

#Region " GetAdditionalFilterDescription "

        Private Function GetAdditionalFilterDescription(ByVal filter As AdditionalFilterOptions) As String

            Select Case filter
                Case AdditionalFilterOptions.NoAdditionalFiltering
                    GetAdditionalFilterDescription = NO_ADDITIONAL_FILTERING_DESCRIPTION
                Case AdditionalFilterOptions.CostExceeded
                    GetAdditionalFilterDescription = COST_EXCEEDED_DESCRIPTION
                Case AdditionalFilterOptions.CostExceededWithinTolerance
                    GetAdditionalFilterDescription = COST_EXCEEDED_WITHIN_TOLERANCE_DESCRIPTION
                Case AdditionalFilterOptions.ManuallySuspended
                    GetAdditionalFilterDescription = MANUALLY_SUSPENDED_DESCRIPTION
                Case AdditionalFilterOptions.UnitsExceeded
                    GetAdditionalFilterDescription = UNITS_EXCEEDED_DESCRIPTION
                Case AdditionalFilterOptions.UnitsExceededWithinCost
                    GetAdditionalFilterDescription = UNITS_EXCEEDED_WITHIN_COST_DESCRIPTION
                Case AdditionalFilterOptions.UnplannedService
                    GetAdditionalFilterDescription = UNPLANNED_SERVICE_DESCRIPTION
                Case Else
                    GetAdditionalFilterDescription = ""
            End Select

        End Function

#End Region

#Region " GetExcludeDescription "

        Private Function GetExcludeDescription(ByVal exclude As String) As String

            Select Case exclude
                Case "null"
                    GetExcludeDescription = EX_FROM_CREDITORS_SHOW_ALL_DESCRIPTION
                Case "false"
                    GetExcludeDescription = EX_FROM_CREDITORS_SHOW_NON_CREDITORS_EXCLUDED_DESCRIPTION
                Case "true"
                    GetExcludeDescription = EX_FROM_CREDITORS_SHOW_CREDITORS_EXCLUDED_DESCRIPTION
                Case Else
                    GetExcludeDescription = ""
            End Select

        End Function

#End Region

#Region " GetQueryStringParameters "

        Private Sub GetQueryStringParameters()

            _providerID = IIf(Request.QueryString("estabid") <> "0", Utils.ToInt32(Request.QueryString("estabid")), 0)
            _domContractID = Utils.ToInt32(Request.QueryString("contractID"))
            _clientID = Utils.ToInt32(Request.QueryString("clientID"))
            If Utils.IsDate(Request.QueryString("weFrom")) Then _weFrom = Request.QueryString("weFrom")
            If Utils.IsDate(Request.QueryString("weTo")) Then _weTo = Request.QueryString("weTo")
            _exclude = IIf(Request.QueryString("exclude") = "" Or Request.QueryString("exclude") = "null", "null", Request.QueryString("exclude"))
            If Utils.IsDate(Request.QueryString("statusDateFrom")) Then _statusDateFrom = Request.QueryString("statusDateFrom")
            If Utils.IsDate(Request.QueryString("statusDateTo")) Then _statusDateTo = Request.QueryString("statusDateTo")
            _statusIsUnpaid = IIf(Request.QueryString("statusIsUnpaid") = "true", True, False)
            _statusIsAuthorised = IIf(Request.QueryString("statusIsAuthorised") = "true", True, False)
            _statusIsPaid = IIf(Request.QueryString("statusIsPaid") = "true", True, False)
            _statusIsSuspended = IIf(Request.QueryString("statusIsSuspended") = "true", True, False)
            _invoiceNumber = Request.QueryString("invoiceNumber")
            _invoiceRef = Request.QueryString("invoiceRef")
            _additionalFilter = Utils.ToInt32(Request.QueryString("additionalFilter"))

            If IsPostBack Then
                _statusIsUnpaid = IIf(Me.ViewState("statusIsUnpaid") IsNot Nothing, Me.ViewState("statusIsUnpaid"), _statusIsUnpaid)
                _statusIsAuthorised = IIf(Me.ViewState("statusIsAuthorised") IsNot Nothing, Me.ViewState("statusIsAuthorised"), _statusIsAuthorised)
                _statusIsPaid = IIf(Me.ViewState("statusIsPaid") IsNot Nothing, Me.ViewState("statusIsPaid"), _statusIsPaid)
                _statusIsSuspended = IIf(Me.ViewState("statusIsSuspended") IsNot Nothing, Me.ViewState("statusIsSuspended"), _statusIsSuspended)
            End If

        End Sub

#End Region

#Region " rptInvoices_ItemDataBound "
        Protected Sub rptInvoices_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptInvoices.ItemDataBound

            Dim row As ViewableDomProviderInvoice
            Dim chk As CheckBox
            Dim td As HtmlTableCell
            Dim cellColour As String = NO_BACKGROUND_COLOUR

            If (e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem) Then
                row = e.Item.DataItem
                chk = e.Item.FindControl("chk")
                chk.Attributes.Add("internalid", row.ID.ToString())
                td = e.Item.FindControl("statusColumn")
                If row.SuspendReason = DomContractBL.ADDITIONAL_FILTER_OPTION_UNPLANNED_SERVICE Then
                    cellColour = UNPLANNED_SERVICE_COLOUR
                ElseIf row.SuspendReason = DomContractBL.ADDITIONAL_FILTER_OPTION_UNITS_EXCEEDED_WITHIN_COST Then
                    cellColour = UNITS_EXCEEDED_WITHIN_COST_COLOUR
                ElseIf row.SuspendReason = DomContractBL.ADDITIONAL_FILTER_OPTION_COST_EXCEEDED Then
                    cellColour = COST_EXCEEDED_COLOUR
                End If
                td.Style.Add("background-color", cellColour)
            End If


        End Sub
#End Region

#Region " InvoicesInMemory "
        Private Property InvoicesInMemory() As List(Of Target.Abacus.Library.ViewableDomProviderInvoice)
            Get
                Dim value As Object = ViewState("invoices")
                Return DirectCast(value, List(Of Target.Abacus.Library.ViewableDomProviderInvoice))
            End Get
            Set(ByVal value As List(Of Target.Abacus.Library.ViewableDomProviderInvoice))
                ViewState("invoices") = value
            End Set
        End Property
#End Region

    End Class

End Namespace