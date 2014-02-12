Imports Target.Abacus.Library
Imports System.Text
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.DomProviderInvoice


Namespace Apps.Dom.ProformaInvoice

    Partial Public Class ManualEnterInvoice 
        Inherits Target.Web.Apps.BasePage

#Region " Constants "
        Const DeleteCommandName As String = "Delete_"
        Const VS_ExpandedPanelID As String = "ExpendedPanelID"
        Const cpManualVisitsID As String = "cpManualVisits"
        Const VS_ObjectIndex As String = "VS_ObjectIndex"
        Const _doPostBack_Recalc As String = "Recalc"
        Const _doPostBack_copyVisits As String = "copyVisits"
        Const _doPostBack_customPostBack As String = "customPostBack"
        Const _doPostBack_editCareWorker As String = "editCareWorker"
        Const __EVENTARGUMENT As String = "__EVENTARGUMENT"

#End Region

#Region " Private Variables "

        Private _batchId As Integer
        Private _originalInvoiceID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _newVisitIDCounter As Integer
        Private _serviceTypes As List(Of ViewablePair) = Nothing
        Private _client As ClientDetail
        Private _copyFromID As Integer
        Private _copyFromWE As Date
        Private _stdBut As StdButtonsBase
        Private _tabStripClientId As String
        Private _mode As String
        Private _date As Date = Date.Parse("1/1/1800 00:00:00")
        Private _pScheduleId As Integer
        Private _IfAddNewTab As Boolean
        Private _IfEditTab As Boolean
        Private _IfCopyVisits As Boolean
        Private _IfCollapsablePanelExpanded As Boolean
        Private _auditUserName As String
        Private _auditLogtitle As String
        Private _new As String
        Private _NotSpecified As String = "Not Specified"

#End Region

#Region " properties "
        Public Property ControlLoaded() As Boolean
            Get
                Dim vs As Object = ViewState("ControlLoaded")
                If Not vs Is Nothing Then
                    Return Boolean.Parse(ViewState("ControlLoaded").ToString())
                End If
                Return False
            End Get
            Set(ByVal value As Boolean)
                ViewState("ControlLoaded") = value
            End Set
        End Property

        Public Property invViewState() As eInvoice.Invoice
            Get
                Dim invoice As eInvoice.Invoice = New eInvoice.Invoice
                If Not Session("inv") Is Nothing Then
                    invoice = Session("inv")
                End If
                Return invoice
            End Get
            Set(ByVal value As eInvoice.Invoice)
                Session("inv") = value
            End Set
        End Property

        Public Property HasEditClicked() As Boolean
            Get
                If ViewState("EditClicked") Is Nothing Then
                    ViewState("EditClicked") = False
                End If
                Return ViewState("EditClicked")
            End Get
            Set(ByVal value As Boolean)
                ViewState("EditClicked") = value
            End Set
        End Property

        Private _backUrl As String
        Public Property backUrl() As String
            Get
                Dim bkurl As Object = ViewState("backUrl")
                If Not bkurl Is Nothing Then
                    Return Convert.ToString(ViewState("backUrl"))
                End If
                Return String.Empty
            End Get
            Set(ByVal value As String)
                ViewState("backUrl") = value
            End Set
        End Property



        Private _reffererUrl As String
        Public Property ReffererUrl() As String
            Get
                Return _reffererUrl
            End Get
            Set(ByVal value As String)
                _reffererUrl = value
            End Set
        End Property

        Public Property WeekEndingDate() As Date
            Get
                If ViewState("WeekEndingDate") Is Nothing Then
                    ViewState("WeekEndingDate") = Date.MaxValue
                End If
                Return ViewState("WeekEndingDate")
            End Get
            Set(ByVal value As Date)
                ViewState("WeekEndingDate") = value
            End Set
        End Property

#End Region

#Region " Page Events "

        Private Sub ManualEnterInvoice_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Manually Entered Visits")
            Me.UseJQuery = True


            Dim style As New StringBuilder
            style.Append("label.label { float:left; width:12em; font-weight:bold; }")
            style.Append("span.label { float:left; width:12em; padding-right:1em; font-weight:bold; }")
            style.Append(".Amendment {padding-left:2em; color:red; font-style:italic; )")
            Me.AddExtraCssStyle(style.ToString)

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim invoice As DataClasses.DomProformaInvoice = New DataClasses.DomProformaInvoice(Me.DbConnection, String.Empty, String.Empty)
            Dim msg As ErrorMessage

            _auditUserName = user.ExternalUsername
            _auditLogtitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)

            pScheduleHeader.SingleLiner = True
            pScheduleHeader.LabelWidth = "11.4em"
            pScheduleHeader.BoldLabels = False

            '' if this call is after saving the record then it must have the invoice id in view state

            If ControlLoaded Then
                Dim inv As eInvoice.Invoice = New eInvoice.Invoice()
                inv = invViewState
                _originalInvoiceID = inv.ID
            Else
                _originalInvoiceID = Utils.ToInt32(Request.QueryString("id"))

            End If

            If _originalInvoiceID > 0 Then
                Dim tmpInv As DataClasses.DomProformaInvoice = New DataClasses.DomProformaInvoice(Me.DbConnection, String.Empty, String.Empty)
                msg = tmpInv.Fetch(_originalInvoiceID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _clientID = tmpInv.ClientID
            Else
                _clientID = Utils.ToInt32(Request.QueryString("clientID"))
            End If

            Dim canDoNew As Boolean, canDoCopy As Boolean
            _pScheduleId = Utils.ToInt32(Request.QueryString("pscheduleid"))
            _providerID = Utils.ToInt32(Request.QueryString("estabID"))
            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            '_clientID = Utils.ToInt32(Request.QueryString("clientID"))
            _copyFromID = Utils.ToInt32(Request.QueryString("copyFromID"))
            If Utils.IsDate(Request.QueryString("copyFromWE")) Then
                _copyFromWE = Convert.ToDateTime(Request.QueryString("copyFromWE"))
            End If
            _reffererUrl = Utils.ToString(Request.QueryString("backUrl"))
            _mode = Utils.ToString(Request.QueryString("mode"))
            WeekEndingDate = Utils.ToDateTime(Request.QueryString("pSWE"))
            _new = Utils.ToString(Request.QueryString("new"))
            If WeekEndingDate <> Date.MaxValue Then
                dteWeekEnding.Text = WeekEndingDate
            End If
            ' if call for copy or add new then enable to add care workers
            If _mode = "2" Then
                EnableAddTab.Value = 1
            End If

            ' check security
            canDoNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentSchedules.AddCopyVisits"))
            canDoCopy = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentSchedules.AddCopyVisits"))
            If Not (canDoNew Or canDoCopy) Then
                WebUtils.DisplayAccessDenied()
            End If
            If _copyFromID = 0 Then
                If Not canDoNew Then
                    WebUtils.DisplayAccessDenied()
                End If
            Else
                If Not canDoCopy Then
                    WebUtils.DisplayAccessDenied()
                End If
            End If


            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowBack = True
                .AllowNew = True
                .AllowEdit = IsInvoiceBatchManuallyEntered(_originalInvoiceID)
                .AllowDelete = IsInvoiceBatchManuallyEntered(_originalInvoiceID)
                .AllowFind = False
                .ShowNew = False
                .EditableControls.Add(pnlInvoice.Controls)
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .CancelClicked, AddressOf CancelClick
                AddHandler .DeleteClicked, AddressOf DeleClick
            End With

            handleBackUrl()


            Me.JsLinks.Add("ManualEnterInvoice.js")

            _IfAddNewTab = IfAddNewTab()
            _IfCopyVisits = IfCopyVisits()
            _IfEditTab = IfEditTab()
            _IfCollapsablePanelExpanded = IfCollapsablePanelExpanded()
            ''''''''''''' always before is post back
            If ControlLoaded Then
                Dim inv As eInvoice.Invoice = New eInvoice.Invoice()
                inv = invViewState
                _contractID = inv.ContractID
                RenderControls(inv)
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''
            If Not Page.IsPostBack Then
                '' set back url on first page load 
                'set back url

                'Utils.ToString(Request.QueryString("backUrl"))
                'RemoveinvoiceIdFromUrl()
                ''''''''''''''''''''''''''''''''''''''''
                Dim inv As eInvoice.Invoice = New eInvoice.Invoice
                Dim invoiceVisits As vwDomProformaInvoiceVisitCollection = New vwDomProformaInvoiceVisitCollection
                ' copy from?
                If _copyFromID > 0 Then
                    inv.IsCopiedFrom = True
                    inv.IsNew = True
                    LoadCopyFromData(Not Me.IsPostBack, _copyFromID, _copyFromWE, inv)
                    LoadInvoiceVisits(_copyFromID, invoiceVisits)
                    ' enable add new tab
                    EnableAddTab.Value = 1
                End If
                LoadReadonlyData(inv)
                '' call to fetch
                If _mode = "1" Then
                    inv.ID = _originalInvoiceID
                    LoadInvoiceData(inv)
                    LoadInvoiceVisits(inv.ID, invoiceVisits)
                End If

                PopulateEinvoiceObject(invoiceVisits, inv)
                RenderControls(inv)
                invViewState = inv
                ControlLoaded = True

            End If

            '' re calculate the visits on post back caused by collapsable panel expand
            ReCalc()

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.CareWorkerSelector.Startup", _
               Target.Library.Web.Utils.WrapClientScript(String.Format("providerID={0};" & _
                                                                       "txtHidReferenceID=""{1}"";" & _
                                                                       "txtHidNameID=""{2}"";" & _
                                                                       "txtHidID=""{3}"";" & _
                                                                       "tabStripID=""{4}"";" & _
                                                                       "txtCopyToNameID=""{5}"";" & _
                                                                       "txtCopyToReferenceID=""{6}"";" & _
                                                                       "txtCopyVisitsID=""{7}"";" & _
                                                                       "OriginalValueChanged=""{8}"";" & _
                                                                       "EnableAddTab=""{9}"";" & _
                                                                       "backUrl=""{10}"";" & _
                                                                       "dteWeekEndingId=""{11}"";" & _
                                                                       "txtNoOfVisitsId=""{12}"";" & _
                                                                       "txtNoOfHoursId=""{13}""; " & _
                                                                       "btnstd=""{14}"";" & _
                                                                       "txtEditCareProviderId_ClientId=""{15}"";" & _
                                                                       "txtEditCareProviderName_ClientId=""{16}"";" & _
                                                                       "txtEditCareProviderRef_ClientId=""{17}"";" & _
                                                                       "txtExistingCareWorkerList_ClientId=""{18}"";", _
                                                                       _providerID, _
                                                                       txtHidReference.ClientID, _
                                                                       txtHidName.ClientID, _
                                                                       txtHidId.ClientID, _
                                                                       _tabStripClientId, _
                                                                       txtCopyToName.ClientID, _
                                                                       txtCopyToReference.ClientID, _
                                                                       txtCopyVisits.ClientID, _
                                                                       OriginalValueChanged.ClientID, _
                                                                       EnableAddTab.ClientID, _
                                                                       backUrl, _
                                                                       dteWeekEnding.ClientID, _
                                                                       txtNoOfVisits.ClientID, _
                                                                       txtNoOfHours.ClientID, _
                                                                       _stdBut.ClientID, _
                                                                       txtEditCareProviderId.ClientID, _
                                                                       txtEditCareProviderName.ClientID, _
                                                                       txtEditCareProviderRef.ClientID, _
                                                                       txtExistingCareWorkerList.ClientID _
                                                                       )) _
                                                          )
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            If _stdBut.ButtonsMode = StdButtonsMode.Fetched Then
                tabStripVisits.Enabled = True
                EnableDisableContainerControls(False)
            ElseIf _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                tabStripVisits.Enabled = True
                EnableDisableContainerControls(True)
                '' if page is in edit mode then disable payment schedule control hyperlink
                pScheduleHeader.EnablePaymentScheduleHyperlink = False
            End If

            txtNoOfHours.TextBox.Enabled = False
            txtNoOfVisits.TextBox.Enabled = False

            If _stdBut.ButtonsMode = StdButtonsMode.Fetched Or _stdBut.ButtonsMode = StdButtonsMode.Initial Then
                btnInvoiceLines.Disabled = False
            Else
                btnInvoiceLines.Disabled = True
            End If

        End Sub

        Private Sub EnableDisableContainerControls(ByVal enabled As Boolean)

            For Each tab As AjaxControlToolkit.TabPanel In tabStripVisits.Tabs
                tab.Enabled = True
                Dim btnDeleteCareWorder As ImageButton
                Dim btnEditCareWorker As HtmlImage

                btnDeleteCareWorder = WebUtils.FindControlRecursive(tab, "btnDeleteCareWorder")
                '' we have to apply null check as last tab panel have only + sign
                If Not btnDeleteCareWorder Is Nothing Then
                    btnDeleteCareWorder.Visible = enabled
                End If

                btnEditCareWorker = WebUtils.FindControlRecursive(tab, "btnEditCareWorker")
                '' we have to apply null check as last tab panel have only + sign
                If Not btnEditCareWorker Is Nothing Then
                    btnEditCareWorker.Visible = enabled
                End If
            Next

            Dim inv As eInvoice.Invoice = New eInvoice.Invoice
            inv = invViewState
            For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                Dim cpVisitsID As String = cpManualVisitsID & cProvider.ObjectIndex
                Dim cpManualVisits As ManualEnterVisits = _
                DirectCast(WebUtils.FindControlRecursive(PnlVisits, cpVisitsID), ManualEnterVisits)
                If cpManualVisits Is Nothing Then
                    Continue For
                End If

                Dim btnAdd As Button
                Dim btnCopy As Button

                Dim txtNoOfVisits As TextBox = WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfVisits")
                txtNoOfVisits.Enabled = enabled
                Dim txtNoOfHours As TextBox = WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours")
                txtNoOfHours.Enabled = enabled

                ' Monday visits
                Dim VisitsMonday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridMonday")
                Dim grdMonday As GridView = WebUtils.FindControlRecursive(VisitsMonday, "gvVisits")
                EnableDisableGridelements(grdMonday, enabled)
                grdMonday.Enabled = enabled
                btnAdd = WebUtils.FindControlRecursive(VisitsMonday, "btnAdd")
                btnAdd.Enabled = enabled
                btnCopy = WebUtils.FindControlRecursive(VisitsMonday, "btnCopy")
                btnCopy.Enabled = enabled
                ' Tuesday visits
                Dim VisitsTuesday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridTuesday")
                Dim grdTuesday As GridView = WebUtils.FindControlRecursive(VisitsTuesday, "gvVisits")
                grdTuesday.Enabled = enabled
                btnAdd = WebUtils.FindControlRecursive(VisitsTuesday, "btnAdd")
                btnAdd.Enabled = enabled
                btnCopy = WebUtils.FindControlRecursive(VisitsTuesday, "btnCopy")
                btnCopy.Enabled = enabled
                ' Wednesday visits
                Dim VisitsWednesday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridWednesday")
                Dim grdWednesday As GridView = WebUtils.FindControlRecursive(VisitsWednesday, "gvVisits")
                grdWednesday.Enabled = enabled
                btnAdd = WebUtils.FindControlRecursive(VisitsWednesday, "btnAdd")
                btnAdd.Enabled = enabled
                btnCopy = WebUtils.FindControlRecursive(VisitsWednesday, "btnCopy")
                btnCopy.Enabled = enabled
                ' Thursday visits
                Dim VisitsThurdsay As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridThursday")
                Dim grdThursday As GridView = WebUtils.FindControlRecursive(VisitsThurdsay, "gvVisits")
                grdThursday.Enabled = enabled
                btnAdd = WebUtils.FindControlRecursive(VisitsThurdsay, "btnAdd")
                btnAdd.Enabled = enabled
                btnCopy = WebUtils.FindControlRecursive(VisitsThurdsay, "btnCopy")
                btnCopy.Enabled = enabled
                ' Friday visits
                Dim VisitsFriday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridFriday")
                Dim grdFriday As GridView = WebUtils.FindControlRecursive(VisitsFriday, "gvVisits")
                grdFriday.Enabled = enabled
                btnAdd = WebUtils.FindControlRecursive(VisitsFriday, "btnAdd")
                btnAdd.Enabled = enabled
                btnCopy = WebUtils.FindControlRecursive(VisitsFriday, "btnCopy")
                btnCopy.Enabled = enabled
                ' Saturday visits
                Dim VisitsSaturday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridSaturday")
                Dim grdSaturday As GridView = WebUtils.FindControlRecursive(VisitsSaturday, "gvVisits")
                grdSaturday.Enabled = enabled
                btnAdd = WebUtils.FindControlRecursive(VisitsSaturday, "btnAdd")
                btnAdd.Enabled = enabled
                btnCopy = WebUtils.FindControlRecursive(VisitsSaturday, "btnCopy")
                btnCopy.Enabled = enabled
                ' Sunday visits
                Dim VisitsSunday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridSunday")
                Dim grdSunday As GridView = WebUtils.FindControlRecursive(VisitsSunday, "gvVisits")
                grdSunday.Enabled = enabled
                btnAdd = WebUtils.FindControlRecursive(VisitsSunday, "btnAdd")
                btnAdd.Enabled = enabled
                btnCopy = WebUtils.FindControlRecursive(VisitsSunday, "btnCopy")
                btnCopy.Enabled = enabled
            Next

        End Sub

        Private Sub EnableDisableGridelements(ByVal grd As GridView, ByVal enabled As Boolean)

            For Each dr As GridViewRow In grd.Rows
                Dim chkBox As New CheckBox
                chkBox = dr.FindControl("chkSecondaryVisit")
                If Not chkBox Is Nothing Then
                    'Dim str As String = "1"
                    'chkBox.Enabled = enabled
                End If
                'Dim visitObjectIndex As Integer = grd.DataKeys(dr.RowIndex).Value
                'For Each visit As eInvoice.DailyVisitDetail In cProvider.listDailyVisitAllDays
                '    If visit.ObjectIndex = visitObjectIndex Then
                '        markAsUpdated = False
                '        ' service type 
                '        ddlServiceType = dr.Cells(0).FindControl("ddlServiceType")
                '        If visit.ServiceTypeID <> Utils.ToInt32(ddlServiceType.SelectedValue) Then
                '            markAsUpdated = True
                '            visit.ServiceTypeID = Utils.ToInt32(ddlServiceType.SelectedValue)
                '        End If
                '        ' start time hours
                '        ddlHours = dr.Cells(1).FindControl("ddlStartTimeHours")
                '        If visit.StartTimeHours <> ddlHours.SelectedValue Then
                '            markAsUpdated = True
                '        End If
                '        ' start time  minutes
                '        ddlMinutes = dr.Cells(1).FindControl("ddlStartTimeMinutes")
                '        If visit.StartTimeMinutes <> ddlMinutes.SelectedValue Then
                '            markAsUpdated = True
                '        End If
                '        visit.StartTime = New TimeSpan(TimeSpan.FromMinutes(ddlHours.SelectedValue * 60 + ddlMinutes.SelectedValue).Ticks)
                '        ' end time hours
                '        ddlHours = dr.Cells(2).FindControl("ddlEndTimeHours")
                '        If visit.EndTimeHours <> ddlHours.SelectedValue Then
                '            markAsUpdated = True
                '        End If
                '        ' End time minutes
                '        ddlMinutes = dr.Cells(2).FindControl("ddlEndTimeMinutes")
                '        If visit.EndTimeMinutes <> ddlMinutes.SelectedValue Then
                '            markAsUpdated = True
                '        End If
                '        visit.EndTime = New TimeSpan(TimeSpan.FromMinutes(ddlHours.SelectedValue * 60 + ddlMinutes.SelectedValue).Ticks)
                '        ' Duration Claimed hours
                '        ddlHours = dr.Cells(3).FindControl("ddlDurationClaimedHours")
                '        If visit.DurationClaimedHours <> ddlHours.SelectedValue Then
                '            markAsUpdated = True
                '        End If
                '        ' Duration claimed minutes
                '        ddlMinutes = dr.Cells(3).FindControl("ddlDurationClaimedMinutes")
                '        If visit.DurationClaimedMinutes <> ddlHours.SelectedValue Then
                '            markAsUpdated = True
                '        End If
                '        visit.DurationClaimed = New TimeSpan(TimeSpan.FromMinutes(ddlHours.SelectedValue * 60 + ddlMinutes.SelectedValue).Ticks)
                '        ' Actual duration hours
                '        ddlHours = dr.Cells(4).FindControl("ddlActualDurationHours")
                '        If visit.ActualDurationHours <> ddlHours.SelectedValue Then
                '            markAsUpdated = True
                '        End If
                '        ' Actual duration minutes
                '        ddlMinutes = dr.Cells(4).FindControl("ddlActualDurationMinutes")
                '        If visit.ActualDurationMinutes <> ddlMinutes.SelectedValue Then
                '            markAsUpdated = True
                '        End If
                '        visit.ActualDuration = New TimeSpan(TimeSpan.FromMinutes(ddlHours.SelectedValue * 60 + ddlMinutes.SelectedValue).Ticks)
                '        ' number of careres
                '        txtNoOfCarers = dr.Cells(5).FindControl("txtNumberOfCarers")
                '        If visit.NumberOfCarers <> txtNoOfCarers.Text Then
                '            markAsUpdated = True
                '            visit.NumberOfCarers = txtNoOfCarers.Text
                '        End If
                '        ' Secondary visit
                '        chkSecondaryVisit = dr.Cells(6).FindControl("chkSecondaryVisit")
                '        visit.SecondaryVisit = chkSecondaryVisit.Checked
                '        'If visit.SecondaryVisit <> chkSecondaryVisit.Checked Then
                '        'markAsUpdated = True
                '        'visit.SecondaryVisit = chkSecondaryVisit.Checked
                '        'End If
                '        ' Visit Code 
                '        ddlVisitCode = dr.Cells(7).FindControl("ddlVisitCode")
                '        If visit.VisitCodeID <> Utils.ToInt32(ddlVisitCode.SelectedValue) Then
                '            markAsUpdated = True
                '            visit.VisitCodeID = Utils.ToInt32(ddlVisitCode.SelectedValue)
                '        End If
                '    End If
                '    ' mark the visitCode detail for final enteries in database.
                '    visit.MarkedToUpdate = markAsUpdated
            Next

        End Sub
        
#End Region

        Private Sub handleBackUrl()
            Dim copy As Boolean = False
            Dim view As Boolean = False

            Dim copyfromId As Integer = 0

            If Not Request.QueryString("copyFromID") Is Nothing Then
                copyfromId = Request.QueryString("copyFromID")
            End If

            If Not Request.QueryString("copyvisit") Is Nothing Then
                copy = Boolean.Parse(Request.QueryString("copyvisit"))
            End If

            If Not Request.QueryString("view") Is Nothing Then
                view = Boolean.Parse(Request.QueryString("view"))
            End If

            If copy Or view Then
                'If _mode = 1 Or (_mode = 2 And copyfromId <> 0) Then
                backUrl = String.Empty
            Else
                backUrl = String.Format("AbacusExtranet/Apps/Dom/PaymentSchedules/PaymentSchedules.aspx?mode=1&id={0}", _
                                    _pScheduleId)
            End If

            If _new.ToLower() = True.ToString().ToLower() Then
                backUrl = String.Format("AbacusExtranet/Apps/Dom/PaymentSchedules/PaymentSchedules.aspx?mode=1&id={0}", _
                                    _pScheduleId)
            End If
        End Sub

#Region " E Invoice Methods "

        Private Sub LoadReadonlyData(ByRef inv As eInvoice.Invoice)

            Dim msg As ErrorMessage
            Dim provider As Establishment
            Dim contract As DomContract

            provider = New Establishment(Me.DbConnection)
            With provider
                msg = .Fetch(_providerID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                'txtProvider.Text = String.Format("{0}: {1}", .AltReference, .Name)
                inv.ProviderID = .ID
                inv.ProviderName = .Name
                inv.ProviderRef = .AltReference
            End With

            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(_contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                'txtContract.Text = String.Format("{0}: {1}", .Number, .Title)
                inv.ContractID = .ID
                inv.ContractRef = .Number
                inv.ContractTitle = .Title
            End With

            _client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
            With _client
                msg = .Fetch(_clientID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                'txtClient.Text = String.Format("{0}: {1} {2}", .Reference, .FirstNames, .LastName)
                inv.ClientID = .ID
            End With

            '' populate eInvoice.Invoice object 
            With inv
                .ServiceUserDetails = String.Format("{0}: {1} {2}", _client.Reference, _client.FirstNames, _client.LastName)
                .ContractRef = contract.Number
                .ContractTitle = contract.Title
                .ProviderID = provider.ID
                .ProviderName = provider.Name
                .ProviderRef = provider.AltReference
            End With


        End Sub

        Private Sub LoadCopyFromData(ByVal createControls As Boolean, _
                                     ByVal invID As Integer, _
                                     ByVal weekEnding As Date, _
                                     ByRef inv As eInvoice.Invoice)

            Dim msg As ErrorMessage
            Dim batch As DomProformaInvoiceBatch
            Dim invoice As DomProformaInvoice
            Dim invoiceVisits As DomProformaInvoiceVisitCollection = Nothing

            ' get the invoice to copy
            invoice = New DomProformaInvoice(Me.DbConnection, _
                                             _auditUserName, _
                                             _auditLogtitle)
            msg = invoice.Fetch(invID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _clientID = invoice.ClientID

            ' set batch id to use for delete method
            _batchId = invoice.DomProformaInvoiceBatchID
            ' get the batch
            batch = New DomProformaInvoiceBatch(Me.DbConnection, _
                                                auditUserName:=_auditUserName, _
                                                auditLogTitle:=_auditLogtitle)
            With batch
                msg = .Fetch(invoice.DomProformaInvoiceBatchID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _providerID = .ProviderID
                _contractID = .DomContractID
            End With

            '' populate eInvoice.Invoice object 
            With inv
                If Not .IsCopiedFrom Then .DirectIncome = invoice.ServiceUserContribution
                If Not .IsCopiedFrom Then .PaymentClaimed = invoice.PaymentClaimed
                If Not .IsCopiedFrom Then .Reference = invoice.OurReference
                If Not .IsCopiedFrom Then .WETo = invoice.WETo Else .WETo = _copyFromWE
                If Not .IsCopiedFrom Then .DirectIncome = invoice.ServiceUserContribution
                If Not .IsCopiedFrom Then .ClientID = invoice.ClientID
                If Not .IsCopiedFrom Then .DomProformaInvoiceBatchID = invoice.DomProformaInvoiceBatchID

                inv.ProviderID = batch.ProviderID
                inv.ContractID = batch.DomContractID
            End With



        End Sub

        Private Function LoadInvoiceData(ByRef inv As eInvoice.Invoice) As ErrorMessage

            Dim msg As ErrorMessage
            msg = New ErrorMessage
            msg.Success = True

            Dim batch As DomProformaInvoiceBatch
            Dim provider As Establishment
            Dim contract As DomContract
            Dim invoice As DomProformaInvoice = New DomProformaInvoice(Me.DbConnection, _
                                                                       auditUserName:=_auditUserName, _
                                                                       auditLogTitle:=_auditLogtitle)

            msg = invoice.Fetch(inv.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get the batch
            batch = New DomProformaInvoiceBatch(Me.DbConnection, _
                                                auditUserName:=_auditUserName, _
                                                auditLogTitle:=_auditLogtitle)
            With batch
                msg = .Fetch(invoice.DomProformaInvoiceBatchID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _providerID = .ProviderID
                _contractID = .DomContractID
            End With

            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = contract.Fetch(batch.DomContractID)

            provider = New Establishment(Me.DbConnection)
            msg = provider.Fetch(_providerID)

            _client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
            msg = _client.Fetch(invoice.ClientID)


            '' populate eInvoice.Invoice object 
            With inv

                .ServiceUserDetails = String.Format("{0}: {1} {2}", _client.Reference, _client.FirstNames, _client.LastName)
                .ContractRef = contract.Number
                .ContractTitle = contract.Title
                .ProviderID = provider.ID
                .ProviderName = provider.Name
                .ProviderRef = provider.AltReference

                If Not .IsCopiedFrom Then .DirectIncome = invoice.ServiceUserContribution
                If Not .IsCopiedFrom Then .PaymentClaimed = invoice.PaymentClaimed
                If Not .IsCopiedFrom Then .Reference = invoice.OurReference
                If Not .IsCopiedFrom Then .WETo = invoice.WETo
                If Not .IsCopiedFrom Then .DirectIncome = invoice.ServiceUserContribution
                If Not .IsCopiedFrom Then .ClientID = invoice.ClientID
                If Not .IsCopiedFrom Then .DomProformaInvoiceBatchID = invoice.DomProformaInvoiceBatchID

                inv.ProviderID = batch.ProviderID
                inv.ContractID = batch.DomContractID

            End With


            Return msg
        End Function

        Private Function LoadInvoiceVisits(ByVal invoiceId As Integer, _
                                           ByRef invoiceVisits As vwDomProformaInvoiceVisitCollection _
                                           ) As ErrorMessage
            Dim msg As ErrorMessage
            msg = New ErrorMessage
            msg.Success = True

            msg = vwDomProformaInvoiceVisit.FetchList(Me.DbConnection, invoiceVisits, invoiceId)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            Return msg

        End Function

        Private Function IfAddNewTab() As Boolean
            If Page.Request.Params(__EVENTARGUMENT) = _doPostBack_customPostBack Then
                ' get invoice from view state
                Dim inv As eInvoice.Invoice = New eInvoice.Invoice
                inv = invViewState
                ' create care worker object 
                Dim cw As eInvoice.CareProvider = New eInvoice.CareProvider
                cw.CareProviderID = txtHidId.Value
                cw.CareProviderName = txtHidName.Value
                cw.Reference = txtHidReference.Value
                If cw.CareProviderName.Trim().ToLower <> _NotSpecified.ToLower() And cw.Reference.Trim().Length > 0 And cw.CareProviderID = "0" Then
                    cw.IsNew = True
                End If
                ' donot user count -1 as we are adding the care provider in next statement
                cw.ObjectIndex = inv.ListCareProvider.Count
                inv.ListCareProvider.Add(cw)
                invViewState = inv
                ' enable add tabs
                Return True
            Else
                Return False
            End If
        End Function

        Private Function IfEditTab() As Boolean
            If Page.Request.Params(__EVENTARGUMENT) = _doPostBack_editCareWorker Then
                ' get invoice from view state
                Dim inv As eInvoice.Invoice = New eInvoice.Invoice
                inv = invViewState
                For Each Item As eInvoice.CareProvider In inv.ListCareProvider
                    If txtEditCareProviderId.Value = Item.CareProviderID And _
                    txtEditCareProviderName.Value = Item.CareProviderName And _
                    txtEditCareProviderRef.Value = Item.Reference Then
                        'if this ia matched in invoice list then we have to replace this.
                        Item.CareProviderID = txtHidId.Value
                        Item.CareProviderName = txtHidName.Value
                        Item.Reference = txtHidReference.Value
                    End If
                Next

                invViewState = inv
                OriginalValueChanged.Value = "true"
                Return True
            Else
                Return False
            End If
        End Function

        Private Function IfCopyVisits() As Boolean
            If Page.Request.Params(__EVENTARGUMENT) = _doPostBack_copyVisits Then

                ' get invoice from view state
                Dim inv As eInvoice.Invoice = New eInvoice.Invoice
                inv = invViewState
                ' get the visits to copy
                Dim visitToCopy As String() = txtCopyVisits.Value.ToString().Split("|")
                ' if no visits to copy just return
                If visitToCopy.Length <= 0 Then
                    Return False
                End If
                ' create an empty list to be filled
                Dim listCopiedVisits As New List(Of eInvoice.DailyVisitDetail)

                ' main iteration
                For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                    If Utils.ToString(cProvider.Reference) = txtCopyToReference.Value.Trim() And cProvider.CareProviderName = txtCopyToName.Value.Trim() Then
                        ' for loop 
                        For index As Integer = 0 To visitToCopy.Length - 1
                            ' loop care worker
                            For Each cWorker As eInvoice.CareProvider In inv.ListCareProvider
                                cWorker.MarkedToDelete = False
                                For Each OldVisit As eInvoice.DailyVisitDetail In cWorker.listDailyVisitAllDays
                                    If OldVisit.ObjectIndex = visitToCopy(index) Then
                                        Dim newVisit As eInvoice.DailyVisitDetail = New eInvoice.DailyVisitDetail()
                                        newVisit = OldVisit.Clone()
                                        '' this value is not copied in clone.
                                        newVisit.ServiceTypeID = OldVisit.ServiceTypeID
                                        ' mark object index
                                        Dim ObjectIndex As Integer = ViewState(VS_ObjectIndex)
                                        ObjectIndex += 1
                                        newVisit.ObjectIndex = ObjectIndex
                                        ViewState(VS_ObjectIndex) = ObjectIndex
                                        ' attach the visit to the copy to care provider as new one 
                                        newVisit.VisitDay = txtCopyToWeekDay.Value
                                        newVisit.MarkedToAdd = True
                                        newVisit.MarkedToDelete = False
                                        newVisit.MarkedToUpdate = False
                                        newVisit.CareProviderId = cProvider.CareProviderID
                                        newVisit.CareProviderName = cProvider.CareProviderName
                                        listCopiedVisits.Add(newVisit)
                                    End If
                                Next
                            Next
                            ' loop careworker ends
                        Next
                        ' for loop ends

                    End If
                Next
                ' main iteration ends

                '''' copy the new visits to the care worker visits
                For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                    If Utils.ToString(cProvider.Reference) = txtCopyToReference.Value.Trim() And cProvider.CareProviderName = txtCopyToName.Value.Trim() Then
                        cProvider.listDailyVisitAllDays.AddRange(listCopiedVisits)
                    End If
                Next

                '''''''''''''''''
                ViewState(VS_ExpandedPanelID) = txtCollapsablePanel.Value
                invViewState = inv
                EnableAddTab.Value = 1

                Return True
            Else
                Return False
            End If
        End Function

        Public Function IfCollapsablePanelExpanded() As Boolean
            Dim rtnValue As Boolean = False
            If Page.Request.Params(__EVENTARGUMENT) = _doPostBack_Recalc Then
                If CollapsablePanel.Value.Length > 1 Then
                    ViewState(VS_ExpandedPanelID) = CollapsablePanel.Value
                    rtnValue = True
                End If
            End If
            Return rtnValue
        End Function

        Private Sub ReCalc()
            If _IfCollapsablePanelExpanded Then
                ' get invoice 
                Dim inv As eInvoice.Invoice
                inv = invViewState
                ' Refill invoice from page 
                inv = RefilleInvoiceFromPage(inv)
            End If
        End Sub

        Private Function PopulateEinvoiceObject(ByVal invoiceVisits As vwDomProformaInvoiceVisitCollection, _
                                                ByRef inv As eInvoice.Invoice) As ErrorMessage
            Dim msg As ErrorMessage
            msg = New ErrorMessage
            msg.Success = True
            Dim cWorker As CareWorker
            ' mark the object index
            Dim index As Integer = 0

            Dim distinctCareWorkers As New List(Of String)
            ' get distinct care workers
            distinctCareWorkers = GetDistinctListOfCareWorkers(invoiceVisits)

            ' if contract has rounding rules
            Dim dcrContractList As DataClasses.Collections.DurationClaimedRoundingDomContractCollection = _
            New DataClasses.Collections.DurationClaimedRoundingDomContractCollection()
            Dim contractHasRoundingRules As Boolean = False
            msg = New ErrorMessage
            msg = DataClasses.DurationClaimedRoundingDomContract.FetchList(Me.DbConnection, dcrContractList, String.Empty, String.Empty, Nothing, inv.ContractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If dcrContractList.Count > 0 Then
                contractHasRoundingRules = True
            End If

            inv.ContractHasRoundingRules = contractHasRoundingRules

            ' make list of care workers
            Dim listCareWorker As New List(Of eInvoice.CareProvider)
            ' loop through care workers
            For Each worker As String In distinctCareWorkers
                ' empty daily visits to refill for current worker
                Dim listdailyVisit As New List(Of eInvoice.DailyVisitDetail)
                ' create care provider
                Dim careWorker As eInvoice.CareProvider = New eInvoice.CareProvider
                careWorker.CareProviderID = worker
                careWorker.ObjectIndex = index
                'careWorker worker not specified
                If worker = "0" Then
                    careWorker.CareProviderName = _NotSpecified
                    careWorker.CareWorkerNotSpecified = True
                Else
                    ' get name from db
                    cWorker = New CareWorker(Me.DbConnection)
                    msg = cWorker.Fetch(careWorker.CareProviderID)
                    If Not msg.Success Then Return msg

                    careWorker.CareProviderName = cWorker.Name
                    careWorker.MarkedToDelete = False
                    careWorker.Reference = cWorker.Reference
                    careWorker.CareWorkerNotSpecified = False
                End If
                For Each visit As vwDomProformaInvoiceVisit In invoiceVisits
                    If Utils.ToInt32(visit.CareWorkerID) = worker Then
                        ' create list of care worker's visits
                        Dim dailyVisit As eInvoice.DailyVisitDetail = New eInvoice.DailyVisitDetail(eInvoice.DailyVisitDetail.WeekDays.AllWeekDays, index)
                        dailyVisit.VisitID = visit.DomProformaInvoiceVisitID
                        dailyVisit.ServiceTypeID = visit.DomServiceTypeID
                        dailyVisit.StartTime = visit.StartTimeClaimed.TimeOfDay
                        dailyVisit.EndTime = visit.StartTimeClaimed.TimeOfDay.Add(visit.DurationClaimed.TimeOfDay)
                        dailyVisit.ActualDuration = visit.ActualDuration.TimeOfDay
                        dailyVisit.DurationClaimed = visit.DurationClaimed.TimeOfDay
                        dailyVisit.IsNew = False
                        dailyVisit.MarkedToUpdate = True
                        dailyVisit.MarkedToAdd = False
                        dailyVisit.MarkedToDelete = False
                        dailyVisit.NumberOfCarers = visit.NumberOfCarers
                        dailyVisit.SecondaryVisit = visit.SecondaryVisit
                        dailyVisit.VisitCodeID = visit.DomVisitCodeID
                        dailyVisit.IgnoreRounding = visit.IgnoreRounding
                        dailyVisit.ContractHasRoundingRules = inv.ContractHasRoundingRules
                        If Utils.ToInt32(_copyFromID) = 0 Then
                            dailyVisit.PreRoundedDurationClaimed = visit.PreRoundedDurationClaimed
                        End If
                        dailyVisit.VisitDay = GetDayOfWeek(visit.VisitDate.DayOfWeek)
                        '' Added for viewstate handle only 
                        dailyVisit.CareProviderId = careWorker.CareProviderID
                        dailyVisit.CareProviderName = careWorker.CareProviderName
                        index += 1
                        ' add visit in daily visit list
                        listdailyVisit.Add(dailyVisit)
                    End If
                Next
                ' attach all visits to the care provider
                careWorker.listDailyVisitAllDays = listdailyVisit
                ' attach care provider to the invoice
                inv.ListCareProvider.Add(careWorker)
            Next

            'set the visit object index for future purpose
            ViewState(VS_ObjectIndex) = index
            Return msg
        End Function

        Private Function GetDistinctListOfCareWorkers(ByVal invoiceVisits As vwDomProformaInvoiceVisitCollection) As List(Of String)
            Dim distinctCareWorkers As New List(Of String)
            For Each visit As vwDomProformaInvoiceVisit In invoiceVisits
                Dim careWorkerId As String = Utils.ToInt32(visit.CareWorkerID)
                If Not distinctCareWorkers.Contains(careWorkerId) Then
                    'Dim SucessFind As String = distinctCareWorkers.Find(Function(careWorkers As String) distinctCareWorkers.Equals(careWorkerId))
                    distinctCareWorkers.Add(careWorkerId)
                End If
            Next
            Return distinctCareWorkers
        End Function

        Public Function GetDayOfWeek(ByVal dow As DayOfWeek) As eInvoice.DailyVisitDetail.WeekDays
            If dow = DayOfWeek.Monday Then
                Return eInvoice.DailyVisitDetail.WeekDays.Monday
            ElseIf dow = DayOfWeek.Tuesday Then
                Return eInvoice.DailyVisitDetail.WeekDays.Tuesday
            ElseIf dow = DayOfWeek.Wednesday Then
                Return eInvoice.DailyVisitDetail.WeekDays.Wednesday
            ElseIf dow = DayOfWeek.Thursday Then
                Return eInvoice.DailyVisitDetail.WeekDays.Thursday
            ElseIf dow = DayOfWeek.Friday Then
                Return eInvoice.DailyVisitDetail.WeekDays.Friday
            ElseIf dow = DayOfWeek.Saturday Then
                Return eInvoice.DailyVisitDetail.WeekDays.Saturday
            ElseIf dow = DayOfWeek.Sunday Then
                Return eInvoice.DailyVisitDetail.WeekDays.Sunday
            End If

        End Function

#Region " Check Invoice batch Type "
        Private Function IsInvoiceBatchManuallyEntered(ByVal invoiceId As Integer) As Boolean
            Dim invoices As vwDomProformaInvoiceCollection = Nothing
            Dim domProformaInvoiceBatchTypeId As Integer = 2 ' for mannually entered batch
            Dim domProformaInvoiceBatchStatusId As Integer = 1 ' for Awaiting verification
            vwDomProformaInvoice.FetchList(Me.DbConnection, invoices, invoiceId, domProformaInvoiceBatchTypeId, domProformaInvoiceBatchStatusId)
            If invoices.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End Function
#End Region

#End Region

#Region " Render Controls "

        ''' <summary>
        ''' Takes the invoice object and Render controls, TabPanel, Tabs and then PopulateControls
        ''' </summary>
        ''' <param name="inv">Invoice Object</param>
        ''' <remarks></remarks>
        Private Sub RenderControls(ByVal inv As eInvoice.Invoice)

            Dim ExistinIDs As String = String.Empty

            For Each cp As eInvoice.CareProvider In inv.ListCareProvider
                If Not cp.MarkedToDelete Then
                    ExistinIDs = ExistinIDs & "," & cp.CareProviderID
                End If
            Next
            ExistinIDs = ExistinIDs.TrimEnd(",")
            ExistinIDs = ExistinIDs.TrimStart(",")

            tabStripVisits.Controls.Clear()
            Dim ActiveTabIndex As Integer = 0
            Dim tabPnlVisit As AjaxControlToolkit.TabPanel
            For Each cp As eInvoice.CareProvider In inv.ListCareProvider
                If Not cp.MarkedToDelete Then
                    cp.ExistingCareWorkers = ExistinIDs
                    txtExistingCareWorkerList.Value = ExistinIDs
                    ' create tab panel for each careprovider
                    tabPnlVisit = New AjaxControlToolkit.TabPanel
                    ' add a unique panel id based on care provider object / care provider may be the new one , so use the object index
                    tabPnlVisit.ID = "tabPnlVisit" & cp.ObjectIndex
                    ' add care worker name as panel name 
                    Dim atemp As New ajaxTabHeaderEx(cp, tabPnlVisit.ID)
                    AddHandler atemp.imgBtnDelete.Click, AddressOf CareTabHeaderDelete_Click
                    tabPnlVisit.HeaderTemplate = atemp
                    ' manual enter visit control for each care provider
                    Dim cpManualVisits As Control = LoadControl("~/AbacusExtranet/Apps/UserControls/ManualEnterVisits.ascx")
                    ' name control id with the cp object index
                    cpManualVisits.ID = cpManualVisitsID & cp.ObjectIndex
                    'add visits control to tab panel
                    tabPnlVisit.Controls.Add(cpManualVisits)
                    'add tab panel to tabstrip
                    tabStripVisits.Controls.Add(tabPnlVisit)
                    tabStripVisits.ActiveTabIndex = ActiveTabIndex
                    ActiveTabIndex += 1
                End If
            Next

            If tabStripVisits.Tabs.Count = 0 Then
                tabPnlVisit = New AjaxControlToolkit.TabPanel
                tabPnlVisit.ID = "tabPnlVisit_AddNewCWorker1"
                'tabPnlVisit.HeaderText = "Click + to add care provider"
                tabStripVisits.Controls.Add(tabPnlVisit)
                'tabStripVisits.OnClientActiveTabChanged = "clientClicked"
                _tabStripClientId = tabStripVisits.ClientID
            End If

            ' add Empty tab to add new tabs
            tabPnlVisit = New AjaxControlToolkit.TabPanel
            tabPnlVisit.ID = "tabPnlVisit_AddNewCWorker"
            Dim aTemplate As New ajaxTabHeader("+", "Add Care Worker")
            tabPnlVisit.HeaderTemplate = aTemplate
            tabStripVisits.Controls.Add(tabPnlVisit)
            tabStripVisits.OnClientActiveTabChanged = "clientClicked"
            _tabStripClientId = tabStripVisits.ClientID

            txthidtabCount.Value = tabStripVisits.Tabs.Count

            '
            Populatecontrols(inv)
        End Sub

        Public Sub CareTabHeaderDelete_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
            If DirectCast(sender, ImageButton).CommandName = "DELETE" Then

                Dim PanelToEditORDelete As String = DirectCast(sender, ImageButton).CommandArgument

                Dim strArr() As String
                strArr = PanelToEditORDelete.Split("_")
                Dim careProviderId As Integer = strArr(0)
                Dim careProviderName As String = strArr(1)
                Dim ObjectIndex As Integer = strArr(2)

                Dim inv As eInvoice.Invoice
                inv = invViewState

                ' Refill invoice from page first and then mark CareWorker as Deleted deleted
                inv = RefilleInvoiceFromPage(inv)

                For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                    If cProvider.CareProviderID = careProviderId And cProvider.CareProviderName = careProviderName Then
                        cProvider.MarkedToDelete = True
                        For Each dailyVisit As eInvoice.DailyVisitDetail In cProvider.listDailyVisitAllDays
                            dailyVisit.MarkedToDelete = True
                        Next
                    End If
                Next

                txtNoOfHours.Text = inv.NumberOfHoursAsString
                txtNoOfVisits.Text = inv.NumberOfVisits

                OriginalValueChanged.Value = "true"
                RenderControls(inv)
            End If


        End Sub

        Private Sub Populatecontrols(ByVal inv As eInvoice.Invoice)
            With inv
                txtProvider.Text = String.Format("{0}: {1}", .ProviderRef, .ProviderName)
                txtContract.Text = String.Format("{0}: {1}", .ContractRef, .ContractTitle)
                txtClient.Text = .ServiceUserDetails
            End With

            If Not Page.IsPostBack Or Not HasEditClicked Then
                With inv
                    If Not .WETo = Date.MaxValue Then dteWeekEnding.Text = .WETo
                    txtReference.Text = .Reference
                    txtPaymentClaimed.Text = .PaymentClaimed.ToString("0.00")

                    txtDirectIncome.Text = .DirectIncome.ToString("0.00")
                    txtNoOfHours.Text = .NumberOfHoursAsString
                    txtNoOfVisits.Text = .NumberOfVisits
                    ''txtClient.Text = String.Format("{0}: {1} {2}", .Reference, .FirstNames, .LastName)
                End With
            Else
                With inv
                    If Date.TryParse(dteWeekEnding.Text, .WETo) Then
                        .WETo = dteWeekEnding.Text
                    End If
                    .Reference = txtReference.TextBox.Text
                    .PaymentClaimed = txtPaymentClaimed.TextBox.Text
                    .DirectIncome = txtDirectIncome.TextBox.Text
                    'txtClient.Text = String.Format("{0}: {1} {2}", .Reference, .FirstNames, .LastName)
                End With
            End If

            For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                Dim cpVisitsID As String = cpManualVisitsID & cProvider.ObjectIndex
                Dim cpManualVisits As Control = _
                DirectCast(WebUtils.FindControlRecursive(PnlVisits, cpVisitsID), ManualEnterVisits)
                If cpManualVisits Is Nothing Then
                    Continue For
                End If

                PopulateCollapsablePanelsForGrids(cProvider, _
                                                  inv.ContractID, _
                                                  inv.WETo, _
                                                  cProvider.listDailyVisitAllDays, _
                                                  cpManualVisits _
                                                  )

                ' hookup events with user controls and bid Grids
                AttachEventsAndBindGrids(cpManualVisits, cProvider)

            Next

            txtNoOfHours.Text = inv.NumberOfHoursAsString
            txtNoOfVisits.Text = inv.NumberOfVisits

        End Sub

        Public Sub AttachEventsAndBindGrids(ByRef cpManualVisits As Control, ByVal cProvider As eInvoice.CareProvider)

            cProvider.SplitAllDailyVisits(cProvider.listDailyVisitAllDays)

            Dim btnAdd As Button
            Dim btnCopy As Button

            Dim VisitsMonday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridMonday")
            Dim grdMonday As GridView = WebUtils.FindControlRecursive(VisitsMonday, "gvVisits")
            btnAdd = New Button
            btnAdd = WebUtils.FindControlRecursive(VisitsMonday, "btnAdd")
            btnAdd.CommandArgument = String.Format("{0}_{1}_{2}", _
                                                   cProvider.CareProviderID, _
                                                   cProvider.CareProviderName, _
                                                   CType(eInvoice.DailyVisitDetail.WeekDays.Monday, Integer))
            btnCopy = New Button
            btnCopy = WebUtils.FindControlRecursive(VisitsMonday, "btnCopy")
            btnCopy.Attributes.Add("onclick", String.Format("return btnCopy_Click(""{0}"",""{1}"",""{2}"",""{3}_{4}_{5}"");", _
                                                           HttpUtility.UrlEncode(cProvider.CareProviderName), _
                                                            cProvider.Reference, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Monday, Integer), _
                                                            cProvider.CareProviderID, _
                                                            cProvider.CareProviderName, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Monday, Integer) _
                                                            ))


            AddHandler grdMonday.RowCommand, AddressOf gvVisits_RowCommand
            AddHandler grdMonday.RowDataBound, AddressOf gvVisits_RowDataBound
            AddHandler btnAdd.Command, AddressOf AddEmptyRowToGrid


            Dim VisitsTuesday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridTuesday")
            Dim grdTuesday As GridView = WebUtils.FindControlRecursive(VisitsTuesday, "gvVisits")
            'btnAdd = New Button
            btnAdd = WebUtils.FindControlRecursive(VisitsTuesday, "btnAdd")
            btnAdd.CommandArgument = String.Format("{0}_{1}_{2}", _
                                       cProvider.CareProviderID, _
                                       cProvider.CareProviderName, _
                                       CType(eInvoice.DailyVisitDetail.WeekDays.Tuesday, Integer))
            'btnCopy = New Button
            btnCopy = WebUtils.FindControlRecursive(VisitsTuesday, "btnCopy")
            btnCopy.Attributes.Add("onclick", String.Format("return btnCopy_Click(""{0}"",""{1}"",""{2}"",""{3}_{4}_{5}"");", _
                                                            cProvider.CareProviderName, _
                                                            cProvider.Reference, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Tuesday, Integer), _
                                                            cProvider.CareProviderID, _
                                                            cProvider.CareProviderName, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Tuesday, Integer) _
                                                            ))

            AddHandler grdTuesday.RowCommand, AddressOf gvVisits_RowCommand
            AddHandler grdTuesday.RowDataBound, AddressOf gvVisits_RowDataBound
            AddHandler btnAdd.Command, AddressOf AddEmptyRowToGrid

            Dim VisitsWednesday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridWednesday")
            Dim grdWednesday As GridView = WebUtils.FindControlRecursive(VisitsWednesday, "gvVisits")
            'btnAdd = New Button
            btnAdd = WebUtils.FindControlRecursive(VisitsWednesday, "btnAdd")
            btnAdd.CommandArgument = String.Format("{0}_{1}_{2}", _
                                       cProvider.CareProviderID, _
                                       cProvider.CareProviderName, _
                                       CType(eInvoice.DailyVisitDetail.WeekDays.Wednesday, Integer))
            'btnCopy = New Button
            btnCopy = WebUtils.FindControlRecursive(VisitsWednesday, "btnCopy")
            btnCopy.Attributes.Add("onclick", String.Format("return btnCopy_Click(""{0}"",""{1}"",""{2}"",""{3}_{4}_{5}"");", _
                                                            cProvider.CareProviderName, _
                                                            cProvider.Reference, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Wednesday, Integer), _
                                                            cProvider.CareProviderID, _
                                                            cProvider.CareProviderName, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Wednesday, Integer) _
                                                            ))

            AddHandler grdWednesday.RowCommand, AddressOf gvVisits_RowCommand
            AddHandler grdWednesday.RowDataBound, AddressOf gvVisits_RowDataBound
            AddHandler btnAdd.Command, AddressOf AddEmptyRowToGrid

            Dim VisitsThursday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridThursday")
            Dim grdThursday As GridView = WebUtils.FindControlRecursive(VisitsThursday, "gvVisits")
            'btnAdd = New Button
            btnAdd = WebUtils.FindControlRecursive(VisitsThursday, "btnAdd")
            btnAdd.CommandArgument = String.Format("{0}_{1}_{2}", _
                                       cProvider.CareProviderID, _
                                       cProvider.CareProviderName, _
                                       CType(eInvoice.DailyVisitDetail.WeekDays.Thursday, Integer))
            'btnCopy = New Button
            btnCopy = WebUtils.FindControlRecursive(VisitsThursday, "btnCopy")
            btnCopy.Attributes.Add("onclick", String.Format("return btnCopy_Click(""{0}"",""{1}"",""{2}"",""{3}_{4}_{5}"");", _
                                                            cProvider.CareProviderName, _
                                                            cProvider.Reference, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Thursday, Integer), _
                                                            cProvider.CareProviderID, _
                                                            cProvider.CareProviderName, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Thursday, Integer) _
                                                            ))

            AddHandler grdThursday.RowCommand, AddressOf gvVisits_RowCommand
            AddHandler grdThursday.RowDataBound, AddressOf gvVisits_RowDataBound
            AddHandler btnAdd.Command, AddressOf AddEmptyRowToGrid

            Dim VisitsFriday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridFriday")
            Dim grdFriday As GridView = WebUtils.FindControlRecursive(VisitsFriday, "gvVisits")
            'btnAdd = New Button
            btnAdd = WebUtils.FindControlRecursive(VisitsFriday, "btnAdd")
            btnAdd.CommandArgument = String.Format("{0}_{1}_{2}", _
                                       cProvider.CareProviderID, _
                                       cProvider.CareProviderName, _
                                       CType(eInvoice.DailyVisitDetail.WeekDays.Friday, Integer))
            'btnCopy = New Button
            btnCopy = WebUtils.FindControlRecursive(VisitsFriday, "btnCopy")
            btnCopy.Attributes.Add("onclick", String.Format("return btnCopy_Click(""{0}"",""{1}"",""{2}"",""{3}_{4}_{5}"");", _
                                                            cProvider.CareProviderName, _
                                                            cProvider.Reference, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Friday, Integer), _
                                                            cProvider.CareProviderID, _
                                                            cProvider.CareProviderName, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Friday, Integer) _
                                                            ))

            AddHandler grdFriday.RowCommand, AddressOf gvVisits_RowCommand
            AddHandler grdFriday.RowDataBound, AddressOf gvVisits_RowDataBound
            AddHandler btnAdd.Command, AddressOf AddEmptyRowToGrid

            Dim VisitsSaturday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridSaturday")
            Dim grdSaturday As GridView = WebUtils.FindControlRecursive(VisitsSaturday, "gvVisits")
            'btnAdd = New Button
            btnAdd = WebUtils.FindControlRecursive(VisitsSaturday, "btnAdd")
            btnAdd.CommandArgument = String.Format("{0}_{1}_{2}", _
                                       cProvider.CareProviderID, _
                                       cProvider.CareProviderName, _
                                       CType(eInvoice.DailyVisitDetail.WeekDays.Saturday, Integer))
            'btnCopy = New Button
            btnCopy = WebUtils.FindControlRecursive(VisitsSaturday, "btnCopy")
            btnCopy.Attributes.Add("onclick", String.Format("return btnCopy_Click(""{0}"",""{1}"",""{2}"",""{3}_{4}_{5}"");", _
                                                            cProvider.CareProviderName, _
                                                            cProvider.Reference, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Saturday, Integer), _
                                                            cProvider.CareProviderID, _
                                                            cProvider.CareProviderName, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Saturday, Integer) _
                                                            ))

            AddHandler grdSaturday.RowCommand, AddressOf gvVisits_RowCommand
            AddHandler grdSaturday.RowDataBound, AddressOf gvVisits_RowDataBound
            AddHandler btnAdd.Command, AddressOf AddEmptyRowToGrid

            Dim VisitsSunday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridSunday")
            Dim grdSunday As GridView = WebUtils.FindControlRecursive(VisitsSunday, "gvVisits")
            'btnAdd = New Button
            btnAdd = WebUtils.FindControlRecursive(VisitsSunday, "btnAdd")
            btnAdd.CommandArgument = String.Format("{0}_{1}_{2}", _
                                       cProvider.CareProviderID, _
                                       cProvider.CareProviderName, _
                                       CType(eInvoice.DailyVisitDetail.WeekDays.Sunday, Integer))
            'btnCopy = New Button
            btnCopy = WebUtils.FindControlRecursive(VisitsSunday, "btnCopy")
            btnCopy.Attributes.Add("onclick", String.Format("return btnCopy_Click(""{0}"",""{1}"",""{2}"",""{3}_{4}_{5}"");", _
                                                            cProvider.CareProviderName, _
                                                            cProvider.Reference, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Sunday, Integer), _
                                                            cProvider.CareProviderID, _
                                                            cProvider.CareProviderName, _
                                                            CType(eInvoice.DailyVisitDetail.WeekDays.Sunday, Integer) _
                                                            ))

            AddHandler grdSunday.RowCommand, AddressOf gvVisits_RowCommand
            AddHandler grdSunday.RowDataBound, AddressOf gvVisits_RowDataBound
            AddHandler btnAdd.Command, AddressOf AddEmptyRowToGrid


            grdMonday.DataSource = cProvider.listDailyVisitMonday
            grdMonday.DataBind()

            EnableDisableGridelements(grdMonday, True)

            grdTuesday.DataSource = cProvider.listDailyVisitTuesday
            grdTuesday.DataBind()


            grdWednesday.DataSource = cProvider.listDailyVisitWednesday
            grdWednesday.DataBind()

            grdThursday.DataSource = cProvider.listDailyVisitThursday
            grdThursday.DataBind()

            grdFriday.DataSource = cProvider.listDailyVisitFriday
            grdFriday.DataBind()

            grdSaturday.DataSource = cProvider.listDailyVisitSaturday
            grdSaturday.DataBind()

            grdSunday.DataSource = cProvider.listDailyVisitSunday
            grdSunday.DataBind()

        End Sub

#End Region

#Region "  Grid Methods "

#Region " drop down populate Methods "

#Region " Dropdown for hours and minutes "

        Private Sub PopulateDropDownList(ByRef ddl As DropDownList, ByVal hours As Boolean)
            For index As Integer = 0 To 59
                ddl.Items.Add(New ListItem(index.ToString.PadLeft(2, "0"c), index))
                If hours And index = 23 Then
                    Exit For
                End If
            Next
        End Sub

#End Region

#Region " PopulateServiceTypeDropdown "

        Private Sub PopulateServiceTypeDropdown(ByRef dropdown As DropDownList, ByVal ContractId As Integer)

            Dim msg As ErrorMessage

            Dim thePage As Target.Web.Apps.BasePage = CType(Me.Page, Target.Web.Apps.BasePage)

            'SqlHelper.GetConnection(
            If _serviceTypes Is Nothing Then
                msg = DomContractBL.FetchServiceTypesAvailableToContract(thePage.DbConnection, ContractId, _serviceTypes)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            With dropdown
                .Items.Clear()
                .DataSource = _serviceTypes
                .DataTextField = "Text"
                .DataValueField = "Value"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

            For Each Item As ListItem In dropdown.Items
                Item.Attributes.Add("title", Item.Text)
            Next

        End Sub

#End Region

#Region " PopulateVisitCodeDropdown "

        Private Sub PopulateVisitCodeDropdown(ByRef dropdown As DropDownList, _
                                              ByVal uniqueID As String, _
                                              ByVal ContractId As Integer, _
                                              ByVal WeekEnding As Date, _
                                              ByVal visitDay As eInvoice.DailyVisitDetail.WeekDays)

            Dim msg As ErrorMessage
            Dim visitDate As Date = WeekEnding
            ''Dim postedBackDoW As String = CType(phVisits.FindControl(CTRL_PREFIX_VISIT_DOW & uniqueID), DropDownListEx).GetPostBackValue()
            ''Dim postedBackVisitCode As String = CType(phVisits.FindControl(CTRL_PREFIX_VISIT_CODE & uniqueID), DropDownListEx).GetPostBackValue()
            ''Dim dow As DayOfWeek
            Dim visitCodes As DomVisitCodeCollection = Nothing

            ' if we have a dow then we should use that to work back from the week ending date
            ''If Not postedBackDoW Is Nothing AndAlso postedBackDoW.Length > 0 Then
            '' dow = [Enum].Parse(GetType(DayOfWeek), postedBackDoW)
            While visitDate.DayOfWeek <> visitDay
                visitDate = visitDate.AddDays(-1)
            End While
            'End If
            Dim thePage As Target.Web.Apps.BasePage = CType(Me.Page, Target.Web.Apps.BasePage)
            msg = DomContractBL.FetchVisitCodesAvailableForVisit(thePage.DbConnection, ContractId, visitDate, visitCodes)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' load the dropdown
            With dropdown
                .Items.Clear()
                .DataSource = visitCodes
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)

                'If Not postedBackVisitCode Is Nothing AndAlso postedBackVisitCode.Length > 0 Then
                '    ' select posted back value
                '    .SelectedValue = postedBackVisitCode
                'Else
                '    ' select default
                '    For Each code As DomVisitCode In visitCodes
                '        If code.DefaultCode Then
                '            .SelectedValue = code.ID
                '            Exit For
                '        End If
                '    Next
                'End If
            End With


            For Each Item As ListItem In dropdown.Items
                Item.Attributes.Add("title", Item.Text)
            Next

            '' if only one visit code then set as selected.
            If (visitCodes.Count = 1) Then
                dropdown.SelectedIndex = 1
            ElseIf (visitCodes.Count > 1) Then
                ' if more than 1 visit codes are available then select the default one.
                For Each Item As DomVisitCode In visitCodes
                    If Item.DefaultCode = TriState.True Then
                        For Each lstItem As ListItem In dropdown.Items
                            If lstItem.Value = Item.ID.ToString() Then
                                lstItem.Selected = True
                            End If
                        Next
                    End If
                Next

            End If




        End Sub

#End Region

#End Region

#Region " Populate Collapsable Panel "

        Public Sub PopulateCollapsablePanelsForGrids(ByVal cProvider As eInvoice.CareProvider, _
                                  ByVal contractId As Integer, _
                                  ByVal weekEnding As Date, _
                                  ByVal listDailyVisits As List(Of eInvoice.DailyVisitDetail), _
                                  ByVal cpManualVisits As System.Web.UI.Control)
            ' get the id of panel to set as expanded
            Dim expandedPanelID As String = String.Empty
            If Not ViewState(VS_ExpandedPanelID) Is Nothing Then
                expandedPanelID = ViewState(VS_ExpandedPanelID)
            End If
            ' generate collesablepanel ID to match for Expanded panel
            Dim collepsablePanelId As String = String.Empty

            cProvider.SplitAllDailyVisits(cProvider.listDailyVisitAllDays)

            Dim cp As New Target.Library.Web.Controls.CollapsiblePanel
            cp.MaintainClientState = True
            cp = New Target.Library.Web.Controls.CollapsiblePanel
            cp = WebUtils.FindControlRecursive(cpManualVisits, "cpVisitsMonday")
            cp.HeaderLinkText = String.Format("Monday - {0} visits totalling {1} hours/mins", _
                                                            cProvider.NumberOfVisitsMonday, _
                                                             cProvider.NumberOfHoursMondayAsString)

            '' Retain the expanded panel 
            collepsablePanelId = String.Format("{0}_{1}_{2}", _
                                               cProvider.CareProviderID, _
                                               cProvider.CareProviderName, _
                                               CType(eInvoice.DailyVisitDetail.WeekDays.Monday, Integer))

            If expandedPanelID <> String.Empty And expandedPanelID = collepsablePanelId Then
                cp.Expanded = True
            End If

            cp.Attributes.Add("onClick", String.Format("Collapse('{0}','{1}','{2}','{3}_VisitGridMonday_btnAdd')", _
                                                      cp.ClientID, _
                                                      cp.Expanded.ToString(), _
                                                      collepsablePanelId, _
                                                      cp.ClientID))

            cp = New Target.Library.Web.Controls.CollapsiblePanel
            cp = WebUtils.FindControlRecursive(cpManualVisits, "cpVisitsTuesday")
            cp.HeaderLinkText = String.Format("Tuesday - {0} visits totalling {1} hours/mins", _
                                                    cProvider.NumberOfVisitsTuesday, _
                                                      cProvider.NumberOfHoursTuesdayAsString)
            '' Retain the expanded panel 
            collepsablePanelId = String.Format("{0}_{1}_{2}", _
                                   cProvider.CareProviderID, _
                                   cProvider.CareProviderName, _
                                  CType(eInvoice.DailyVisitDetail.WeekDays.Tuesday, Integer))

            If expandedPanelID <> String.Empty And expandedPanelID = collepsablePanelId Then
                cp.Expanded = True
            End If

            cp.Attributes.Add("onClick", String.Format("Collapse('{0}','{1}','{2}','{3}_VisitGridTuesday_btnAdd')", _
                                          cp.ClientID, _
                                          cp.Expanded.ToString(), _
                                          collepsablePanelId, _
                                          cp.ClientID))

            cp = New Target.Library.Web.Controls.CollapsiblePanel
            cp = WebUtils.FindControlRecursive(cpManualVisits, "cpVisitsWednesday")
            cp.HeaderLinkText = String.Format("Wednesday - {0} visits totalling {1} hours/mins", _
                                                     cProvider.NumberOfVisitsWednesday, _
                                                      cProvider.NumberOfHoursWednesdayAsString)
            '' Retain the expanded panel 
            collepsablePanelId = String.Format("{0}_{1}_{2}", _
                                   cProvider.CareProviderID, _
                                   cProvider.CareProviderName, _
                                   CType(eInvoice.DailyVisitDetail.WeekDays.Wednesday, Integer))


            If expandedPanelID <> String.Empty And expandedPanelID = collepsablePanelId Then
                cp.Expanded = True
            End If
          
            cp.Attributes.Add("onClick", String.Format("Collapse('{0}','{1}','{2}','{3}_VisitGridWednesday_btnAdd')", _
                                          cp.ClientID, _
                                          cp.Expanded.ToString(), _
                                          collepsablePanelId, _
                                          cp.ClientID))

            cp = New Target.Library.Web.Controls.CollapsiblePanel
            cp = WebUtils.FindControlRecursive(cpManualVisits, "cpVisitsThursday")
            cp.HeaderLinkText = String.Format("Thursday - {0} visits totalling {1} hours/mins", _
                                                      cProvider.NumberOfVisitsThursday, _
                                                      cProvider.NumberOfHoursThursdayAsString)
            '' Retain the expanded panel 
            collepsablePanelId = String.Format("{0}_{1}_{2}", _
                       cProvider.CareProviderID, _
                       cProvider.CareProviderName, _
                       CType(eInvoice.DailyVisitDetail.WeekDays.Thursday, Integer))

            If expandedPanelID <> String.Empty And expandedPanelID = collepsablePanelId Then
                cp.Expanded = True
            End If

            cp.Attributes.Add("onClick", String.Format("Collapse('{0}','{1}','{2}','{3}_VisitGridThursday_btnAdd')", _
                                          cp.ClientID, _
                                          cp.Expanded.ToString(), _
                                          collepsablePanelId, _
                                          cp.ClientID))

            cp = New Target.Library.Web.Controls.CollapsiblePanel
            cp = WebUtils.FindControlRecursive(cpManualVisits, "cpVisitsFriday")
            cp.HeaderLinkText = String.Format("Friday - {0} visits totalling {1} hours/mins", _
                                                      cProvider.NumberOfVisitsFriday, _
                                                      cProvider.NumberOfHoursFridayAsString)
            '' Retain the expanded panel 
            collepsablePanelId = String.Format("{0}_{1}_{2}", _
                       cProvider.CareProviderID, _
                       cProvider.CareProviderName, _
                       CType(eInvoice.DailyVisitDetail.WeekDays.Friday, Integer))

            If expandedPanelID <> String.Empty And expandedPanelID = collepsablePanelId Then
                cp.Expanded = True
            End If

            cp.Attributes.Add("onClick", String.Format("Collapse('{0}','{1}','{2}','{3}_VisitGridFriday_btnAdd')", _
                                          cp.ClientID, _
                                          cp.Expanded.ToString(), _
                                          collepsablePanelId, _
                                          cp.ClientID))

            cp = New Target.Library.Web.Controls.CollapsiblePanel
            cp = WebUtils.FindControlRecursive(cpManualVisits, "cpVisitsSaturday")
            cp.HeaderLinkText = String.Format("Saturday - {0} visits totalling {1} hours/mins", _
                                          cProvider.NumberOfVisitsSaturday, _
                                          cProvider.NumberOfHoursSaturdayAsString)
            '' Retain the expanded panel 
            collepsablePanelId = String.Format("{0}_{1}_{2}", _
                       cProvider.CareProviderID, _
                       cProvider.CareProviderName, _
                       CType(eInvoice.DailyVisitDetail.WeekDays.Saturday, Integer))

            If expandedPanelID <> String.Empty And expandedPanelID = collepsablePanelId Then
                cp.Expanded = True
            End If

            cp.Attributes.Add("onClick", String.Format("Collapse('{0}','{1}','{2}','{3}_VisitGridSaturday_btnAdd')", _
                                          cp.ClientID, _
                                          cp.Expanded.ToString(), _
                                          collepsablePanelId, _
                                          cp.ClientID))


            cp = New Target.Library.Web.Controls.CollapsiblePanel
            cp = WebUtils.FindControlRecursive(cpManualVisits, "cpVisitsSunday")

            cp.HeaderLinkText = String.Format("Sunday - {0} visits totalling {1} hours/mins", _
                                                      cProvider.NumberOfVisitsSunday, _
                                                      cProvider.NumberOfHoursSundayAsString)
            '' Retain the expanded panel 
            collepsablePanelId = String.Format("{0}_{1}_{2}", _
                       cProvider.CareProviderID, _
                       cProvider.CareProviderName, _
                       CType(eInvoice.DailyVisitDetail.WeekDays.Sunday, Integer))

            If expandedPanelID <> String.Empty And expandedPanelID = collepsablePanelId Then
                cp.Expanded = True
            End If

            cp.Attributes.Add("onClick", String.Format("Collapse('{0}','{1}','{2}','{3}_VisitGridSunday_btnAdd')", _
                                          cp.ClientID, _
                                          cp.Expanded.ToString(), _
                                          collepsablePanelId, _
                                          cp.ClientID))

            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfVisits"), TextBox).Text = _
            cProvider.TotalNumberOfVisits.ToString()
            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours"), TextBox).Text = _
            cProvider.TotalNumberOfHoursAsString

            'Fill hidden fields for all Week Days
            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours_Monday"), HiddenField).Value = _
            cProvider.NumberOfHoursMondayAsString
            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours_Tuesday"), HiddenField).Value = _
            cProvider.NumberOfHoursTuesdayAsString
            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours_Wednesday"), HiddenField).Value = _
            cProvider.NumberOfHoursWednesdayAsString
            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours_Thursday"), HiddenField).Value = _
            cProvider.NumberOfHoursThursdayAsString
            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours_Friday"), HiddenField).Value = _
            cProvider.NumberOfHoursFridayAsString
            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours_Saturday"), HiddenField).Value = _
            cProvider.NumberOfHoursSaturdayAsString
            CType(WebUtils.FindControlRecursive(cpManualVisits, "txtCpNumberOfHours_Sunday"), HiddenField).Value = _
            cProvider.NumberOfHoursSundayAsString

        End Sub

#End Region

#End Region

#Region " Grid Events "

        Private Sub gvVisits_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
            Dim calculateTime As String = String.Empty
            If e.Row.RowType = DataControlRowType.DataRow Or _
            e.Row.RowType = DataControlRowType.Footer _
             Then
                'e.Row.RowType = DataControlRowType.EmptyDataRow _
                ' if row is footer row then only fill the drop down list and donot try to assign the value. as dataitem is null
                Dim isFooter As Boolean = e.Row.RowType = DataControlRowType.Footer
                Dim isEmptyDataRow As Boolean = e.Row.RowType = DataControlRowType.EmptyDataRow

                If e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Or e.Row.RowState = DataControlRowState.Insert Then
                    '''''''''''''''''' get vist day ''''''''''''''''''''''
                    'Dim visitday As eInvoice.DailyVisitDetail.WeekDays = DirectCast(DirectCast(DirectCast(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object), eInvoice.DailyVisitDetail).VisitDay
                    Dim visitday As eInvoice.DailyVisitDetail.WeekDays
                    If Not e.Row.DataItem Is Nothing Then
                        visitday = DirectCast(e.Row.DataItem, eInvoice.DailyVisitDetail).VisitDay
                    End If
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    Dim selectedValue As String = "0"
                    Dim SecondryVisit As Boolean
                    Dim ddlHoursST As DropDownList
                    Dim ddlMinutesST As DropDownList
                    Dim ddlHoursET As DropDownList
                    Dim ddlMinutesET As DropDownList
                    Dim ddlHoursDC As DropDownList
                    Dim ddlMinutesDC As DropDownList
                    Dim ddlHoursAD As DropDownList
                    Dim ddlMinutesAD As DropDownList
                    Dim ddlServiceType As DropDownList
                    Dim ddlVisitCode As DropDownList
                    Dim btnDelete As ImageButton
                    Dim txtNoOfCarers As TextBox
                    Dim chkSecondaryVisit As CheckBox
                    Dim pnlIgnoreRounding As Panel
                    ' service type 
                    ddlServiceType = e.Row.FindControl("ddlServiceType")
                    If Not ddlServiceType Is Nothing Then
                        ddlServiceType.Attributes.Add("onchange", "valuechanged();")
                        ddlServiceType.Items.Clear()
                        PopulateServiceTypeDropdown(ddlServiceType, _contractID)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        '+ if there are 2 elements in ddl then select first by default
                        If ddlServiceType.Items.Count = 2 Then
                            ddlServiceType.SelectedIndex = 1
                        Else
                            selectedValue = _
                      CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                      Target.Abacus.Library.eInvoice.DailyVisitDetail).ServiceTypeID
                            ddlServiceType.SelectedValue = selectedValue
                        End If
                    End If

                    ' start time hours
                    ddlHoursST = e.Row.FindControl("ddlStartTimeHours")
                    ddlMinutesST = e.Row.FindControl("ddlStartTimeMinutes")
                    ddlHoursET = e.Row.FindControl("ddlEndTimeHours")
                    ddlMinutesET = e.Row.FindControl("ddlEndTimeMinutes")
                    ddlHoursDC = e.Row.FindControl("ddlDurationClaimedHours")
                    ddlMinutesDC = e.Row.FindControl("ddlDurationClaimedMinutes")
                    ddlHoursAD = e.Row.FindControl("ddlActualDurationHours")
                    ddlMinutesAD = e.Row.FindControl("ddlActualDurationMinutes")
                    pnlIgnoreRounding = e.Row.FindControl("ignoreRounding")

                    If Not ddlMinutesST Is Nothing AndAlso Not ddlHoursST Is Nothing _
                    AndAlso Not ddlMinutesET Is Nothing AndAlso Not ddlHoursET Is Nothing _
                    AndAlso Not ddlMinutesDC Is Nothing AndAlso Not ddlHoursDC Is Nothing _
                    AndAlso Not ddlMinutesAD Is Nothing AndAlso Not ddlHoursAD Is Nothing _
                    Then
                        calculateTime = String.Format("calculateTime(""{0}"",""{1}"",""{2}"",""{3}"",""{4}"",""{5}"",""{6}"",""{7}"");", _
                                                      ddlHoursST.ClientID, _
                                                      ddlMinutesST.ClientID, _
                                                      ddlHoursET.ClientID, _
                                                      ddlMinutesET.ClientID, _
                                                      ddlHoursDC.ClientID, _
                                                      ddlMinutesDC.ClientID, _
                                                      ddlHoursAD.ClientID, _
                                                      ddlMinutesAD.ClientID)
                    End If

                    If Not ddlHoursST Is Nothing Then
                        '' Set calculateTime
                        ddlHoursST.Attributes.Add("onchange", String.Format("valuechanged();{0}", calculateTime))
                        ddlHoursST.Items.Clear()
                        PopulateDropDownList(ddlHoursST, True)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).StartTimeHours
                        ddlHoursST.SelectedValue = selectedValue
                    End If
                    ' start time minutes
                    If Not ddlMinutesST Is Nothing Then
                        ddlMinutesST.Attributes.Add("onchange", String.Format("valuechanged();{0}", calculateTime))
                        ddlMinutesST.Items.Clear()
                        PopulateDropDownList(ddlMinutesST, False)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).StartTimeMinutes
                        ddlMinutesST.SelectedValue = selectedValue
                    End If
                    ' end Time hours

                    If Not ddlHoursET Is Nothing Then
                        ddlHoursET.Attributes.Add("onchange", String.Format("valuechanged();{0}", calculateTime))
                        ddlHoursET.Items.Clear()
                        PopulateDropDownList(ddlHoursET, True)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).EndTimeHours
                        ddlHoursET.SelectedValue = selectedValue
                    End If
                    ' End Time minutes

                    If Not ddlMinutesET Is Nothing Then
                        ddlMinutesET.Attributes.Add("onchange", String.Format("valuechanged();{0}", calculateTime))
                        ddlMinutesET.Items.Clear()
                        PopulateDropDownList(ddlMinutesET, False)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).EndTimeMinutes
                        ddlMinutesET.SelectedValue = selectedValue
                    End If
                    '===========================================================================================
                    'Duration Claimed 
                    '===========================================================================================
                    ' Duration Claimed hours


                    If Not ddlHoursDC Is Nothing Then
                        ddlHoursDC.Attributes.Add("onchange", String.Format("valuechanged();ReCalculateHoursForAll();CalculateHours(""{0}"");", ddlHoursDC.ClientID))
                        ddlHoursDC.Items.Clear()
                        PopulateDropDownList(ddlHoursDC, True)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                                        Target.Abacus.Library.eInvoice.DailyVisitDetail).DurationClaimedHours
                        ddlHoursDC.SelectedValue = selectedValue
                    End If

                    If Not ddlMinutesDC Is Nothing Then
                        ddlMinutesDC.Attributes.Add("onchange", String.Format("valuechanged();ReCalculateHoursForAll();CalculateHours(""{0}"");", ddlHoursDC.ClientID))
                        ddlMinutesDC.Items.Clear()
                        PopulateDropDownList(ddlMinutesDC, False)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).DurationClaimedMinutes
                        ddlMinutesDC.SelectedValue = selectedValue
                    End If

                    If Not isFooter And Not isEmptyDataRow Then
                        Dim contractHasRules As Boolean = False
                        Dim ignoreRounding As Boolean
                        contractHasRules = _
                                            CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                                            Target.Abacus.Library.eInvoice.DailyVisitDetail).ContractHasRoundingRules
                        ignoreRounding = _
                                            CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                                            Target.Abacus.Library.eInvoice.DailyVisitDetail).IgnoreRounding

                        pnlIgnoreRounding = HandleIgnoreRounding(pnlIgnoreRounding, contractHasRules, ignoreRounding)
                    End If
                    '===========================================================================================
                    'Duration Claimed 
                    '===========================================================================================
                    ' Actual duration hours

                    If Not ddlHoursAD Is Nothing Then
                        ddlHoursAD.Attributes.Add("onchange", "valuechanged();")
                        ddlHoursAD.Items.Clear()
                        PopulateDropDownList(ddlHoursAD, True)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).ActualDurationHours
                        ddlHoursAD.SelectedValue = selectedValue
                    End If
                    ' Actual duration minutes

                    If Not ddlMinutesAD Is Nothing Then
                        ddlMinutesAD.Attributes.Add("onchange", "valuechanged();")
                        ddlMinutesAD.Items.Clear()
                        PopulateDropDownList(ddlMinutesAD, False)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).ActualDurationMinutes
                        ddlMinutesAD.SelectedValue = selectedValue
                    End If

                    ' number of careres
                    txtNoOfCarers = e.Row.FindControl("txtNumberOfCarers")
                    If Not txtNoOfCarers Is Nothing Then
                        txtNoOfCarers.Attributes.Add("onchange", "valuechanged();checkNum(this);")
                        If Not isFooter And Not isEmptyDataRow Then
                            txtNoOfCarers.Enabled = Not (CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).IsSingleCarer)
                            If CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).IsSingleCarer Then
                                txtNoOfCarers.Text = 1
                            End If
                        End If
                    End If
                    ' secondary visit
                    chkSecondaryVisit = e.Row.FindControl("chkSecondaryVisit")
                    If Not chkSecondaryVisit Is Nothing Then
                        chkSecondaryVisit.Attributes.Add("onclick", "valuechanged();")
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        SecondryVisit = CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).SecondaryVisit
                        chkSecondaryVisit.Checked = SecondryVisit
                    End If

                    ' Visit Code 
                    ddlVisitCode = e.Row.FindControl("ddlVisitCode")
                    If Not ddlVisitCode Is Nothing Then
                        ddlVisitCode.Attributes.Add("onchange", "valuechanged();")
                        ddlVisitCode.Items.Clear()
                        PopulateVisitCodeDropdown(ddlVisitCode, 0, _contractID, dteWeekEnding.Text, visitday)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                      CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                      Target.Abacus.Library.eInvoice.DailyVisitDetail).VisitCodeID
                        ddlVisitCode.SelectedValue = selectedValue
                    End If

                    btnDelete = e.Row.FindControl("btnDel")
                    If Not btnDelete Is Nothing Then
                        btnDelete.ImageUrl = WebUtils.GetVirtualPath("Images/delete.png")
                        btnDelete.CommandName = DeleteCommandName '& DayOfWeek
                        Dim commandArgument As String = String.Empty
                        commandArgument = String.Format("{0}_{1}_{2}_{3}", _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                      Target.Abacus.Library.eInvoice.DailyVisitDetail).CareProviderId, _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                      Target.Abacus.Library.eInvoice.DailyVisitDetail).CareProviderName, _
                      CType(CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                      Target.Abacus.Library.eInvoice.DailyVisitDetail).VisitDay, Integer), _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                      Target.Abacus.Library.eInvoice.DailyVisitDetail).ObjectIndex _
                      )
                        btnDelete.CommandArgument = commandArgument
                    End If
                End If
            End If
        End Sub

        Private Function HandleIgnoreRounding(ByVal pnl As Panel, _
                                              ByVal contractHasRules As Boolean, _
                                              ByVal ignoreRounding As Boolean) As Panel
            If contractHasRules And ignoreRounding Then
                Dim img As New System.Web.UI.WebControls.Image
                img.AlternateText = "ignore rounding"
                img.ToolTip = "Duration Claimed Rounding rules are ignored"
                img.ImageUrl = WebUtils.GetVirtualPath("Images/IgnoreRounding.png")
                pnl.Controls.Add(img)
            End If
            Return pnl
        End Function

        Private Function GenerateIdForTxtCpNumberOfHours(ByVal dropdownlistId As String) As String
            Dim str As String() = dropdownlistId.Split("_")
            Dim txtID As String = String.Empty
            For index As Integer = 0 To 4
                txtID += str(index) + "_"
            Next
            txtID += "txtCpNumberOfHours"
            Return txtID

        End Function

        Private Sub gvVisits_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs)
            If e.CommandName.Equals(DeleteCommandName) Then
                Dim strArr() As String
                strArr = e.CommandArgument.ToString().Split("_")
                Dim careProviderId As Integer = strArr(0)
                Dim careProviderName As String = strArr(1)
                Dim visitDay As eInvoice.DailyVisitDetail.WeekDays = strArr(2)
                Dim ObjectIndex As Integer = strArr(3)

                ViewState(VS_ExpandedPanelID) = String.Format("{0}_{1}_{2}", _
                                                              careProviderId, _
                                                              careProviderName, _
                                                              CType(visitDay, Integer))

                Dim inv As eInvoice.Invoice
                inv = invViewState

                ' Refill invoice from page first and then mark the visit as deleted
                inv = RefilleInvoiceFromPage(inv)

                For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                    For Each dailyVisit As eInvoice.DailyVisitDetail In cProvider.listDailyVisitAllDays
                        If dailyVisit.ObjectIndex = ObjectIndex Then
                            dailyVisit.MarkedToDelete = True
                        End If
                    Next
                Next

                txtNoOfHours.Text = inv.NumberOfHoursAsString
                txtNoOfVisits.Text = inv.NumberOfVisits
                RenderControls(inv)
            End If
        End Sub

        Private Sub AddEmptyRowToGrid(ByVal sender As Object, ByVal e As CommandEventArgs)
            ' check proper date in week ending 
            Dim weekEndingDate As Date
            If dteWeekEnding.Text.Trim() = "" Then
                lblError.Text = "Please enter a week ending date before adding any visits"
                Return
            End If
            If Date.TryParse(dteWeekEnding.Text, weekEndingDate) Then
                If Not weekEndingDate.DayOfWeek = DayOfWeek.Sunday Then
                    lblError.Text = "The date is not a valid week ending date. The date must be a Sunday"
                    Return
                End If
            Else
                lblError.Text = "The date is not a valid week ending date. The date must be a Sunday"
            End If

            ' add empty row at the end. but call RefilleInvoiceFromPage so that if there is already a new row 
            ' that shoudl be added to the inv

            ViewState(VS_ExpandedPanelID) = e.CommandArgument
            ' get details , where to add the new visit
            Dim strArr() As String
            strArr = e.CommandArgument.ToString().Split("_")
            Dim careProviderId As Integer = strArr(0)
            Dim careProviderName As String = strArr(1)
            Dim visitDay As eInvoice.DailyVisitDetail.WeekDays = strArr(2)

            ' create an empty visit details
            Dim visitDetail As New eInvoice.DailyVisitDetail

            ' get invoice 
            Dim inv As eInvoice.Invoice
            inv = invViewState
            ' Refill invoice from page first and then add a new Row.
            inv = RefilleInvoiceFromPage(inv)

            '' if this post back was due to collapsable panel only calculate 
            '' which is done and donot add empty row
            If Boolean.Parse(AddNewRow.Value) Then
                ' find the care worker and attach the empty visit details
                For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                    If cProvider.CareProviderID = careProviderId And cProvider.CareProviderName = careProviderName Then
                        ' fill default values in visit details
                        visitDetail.VisitID = 0
                        visitDetail.MarkedToAdd = True
                        visitDetail.MarkedToDelete = False
                        visitDetail.MarkedToUpdate = False
                        visitDetail.VisitDay = visitDay
                        ' assign incremented object index
                        visitDetail.ObjectIndex = ViewState(VS_ObjectIndex) + 1
                        ' set the new object index
                        ViewState(VS_ObjectIndex) = visitDetail.ObjectIndex
                        visitDetail.CareProviderId = cProvider.CareProviderID
                        visitDetail.CareProviderName = cProvider.CareProviderName
                        visitDetail.ContractHasRoundingRules = inv.ContractHasRoundingRules
                        cProvider.listDailyVisitAllDays.Add(visitDetail)
                        If visitDetail.CareProviderName <> _NotSpecified Then
                            visitDetail.IsSingleCarer = True
                        End If
                        visitDetail.NumberOfCarers = 1
                    End If
                Next
            Else
                AddNewRow.Value = "True"
            End If
            txtNoOfVisits.Text = inv.NumberOfVisits
            txtNoOfHours.Text = inv.NumberOfHoursAsString

            OriginalValueChanged.Value = "true"
            RenderControls(inv)

        End Sub

        Public Sub UpdateInvoiceHeader(ByRef inv As eInvoice.Invoice)
            inv.WETo = dteWeekEnding.Text
            inv.Reference = txtReference.Text
            inv.PaymentClaimed = txtPaymentClaimed.Text
            inv.DirectIncome = txtDirectIncome.Text
        End Sub

#End Region

#Region " Std Buttons Events "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            HasEditClicked = True
            'enable add a new care worker
            EnableAddTab.Value = 1
            'EnableDisableContainerControls(True)
            Page_Load(Me, Nothing)
        End Sub

        Private Sub DeleClick(ByRef e As StdButtonEventArgs)

            Dim inv As eInvoice.Invoice = New eInvoice.Invoice
            inv = invViewState

            Dim user As WebSecurityUser
            user = SecurityBL.GetCurrentUser()

            Dim invoice As DomProformaInvoice
            Dim invoiceVisits As DomProformaInvoiceVisitCollection = Nothing
            Dim msg As ErrorMessage
            msg = New ErrorMessage()
            ' get the invoice to copy
            invoice = New DomProformaInvoice(Me.DbConnection, _auditUserName, _auditLogtitle)
            msg = invoice.Fetch(inv.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = New ErrorMessage()
            msg = DomContractBL.ChangeProformaInvoiceBatchStatus(Me.DbConnection, _
                                                           invoice.DomProformaInvoiceBatchID, _
                                                           DomProformaInvoiceBatchStatus.Deleted, _
                                                           user.ExternalUsername, _
                                                           "DomContractWebSvc.ChangeProformaInvoiceBatchStatus()" _
                                                           )
            If Not msg.Success Then WebUtils.DisplayError(msg)

            '' Recalculate Payment Schedule 
            msg = New ErrorMessage()
            msg = DomContractBL.PaymentscheduleRecalculateCountsAttributesAndNetValues(Me.DbConnection, invoice.PaymentScheduleID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If _new.ToLower() = True.ToString().ToLower() Then
                _reffererUrl = String.Format("../PaymentSchedules/PaymentSchedules.aspx?mode=1&id={0}", _
                                    _pScheduleId)
            End If

            Response.Redirect(_reffererUrl)
        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            HasEditClicked = True
        End Sub

        Private Sub CancelClick(ByRef e As StdButtonEventArgs)

            Dim copyfromId As Integer = 0
            If Not Request.QueryString("copyFromID") Is Nothing Then
                copyfromId = Request.QueryString("copyFromID")
            End If

            If _stdBut.ButtonsMode = StdButtonsMode.AddNew And copyfromId <> 0 Then
                Dim copyUrl As String = Utils.ToString(Request.QueryString("backUrl"))
                Response.Redirect(copyUrl)
            ElseIf _stdBut.ButtonsMode = StdButtonsMode.AddNew And copyfromId = 0 Then
                Response.Redirect(String.Format("~/AbacusExtranet/Apps/Dom/PaymentSchedules/PaymentSchedules.aspx?mode=1&id={0}", _pScheduleId))
            Else
                'HasEditClicked = False
                ' disable adding a new care worker
                EnableAddTab.Value = 0

                Dim backUrl As String = Request.QueryString("backUrl")
                Dim builder As Target.Library.Web.UriBuilder = New Target.Library.Web.UriBuilder(Request.Url)
                builder.QueryItems.Remove("backUrl")

                Dim url As String = builder.ToString()
                url = url & "&backUrl=" & HttpUtility.UrlEncode(backUrl)

                Response.Redirect(url)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Me.Validate()
            Dim msg As New ErrorMessage
            Dim inv As eInvoice.Invoice = New eInvoice.Invoice
            inv = invViewState

            If Me.IsValid Then
                HasEditClicked = False
                inv = RefilleInvoiceFromPage(inv)
                '' try to validate before save 
                msg = ValidateVisits(inv)
                If msg.Success Then
                    InsertUpdateInvoice(e, inv)
                    invViewState = inv
                    RenderControls(inv)
                    OriginalValueChanged.Value = "false"
                Else
                    lblError.Text = msg.Message
                    e.Cancel = True
                End If
            Else
                ValidateSave(inv)
                e.Cancel = True
                'CancelClick(e)
            End If
        End Sub

#End Region


        Private Function ValidateVisits(ByVal inv As eInvoice.Invoice) As ErrorMessage
            Dim initialMsg As String = "The proforma invoice could not be saved for the following reason(s): <br/> "
            Dim errMmsg As StringBuilder = New StringBuilder
            Dim msg As New ErrorMessage

            If txtReference.Text.Trim() = "" Then
                errMmsg.Append("Reference number is required. <br/>")
            End If
            If inv.NumberOfVisits = 0 Then
                errMmsg.Append("No visit attached to invoice. <br/>")
            End If

            For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                For Each visitDetail As eInvoice.DailyVisitDetail In cProvider.listDailyVisitAllDays
                    If visitDetail.MarkedToDelete Then
                        Continue For
                    End If
                    If visitDetail.DurationClaimedHours = 0 And visitDetail.DurationClaimedMinutes = 0 Then
                        errMmsg.Append(String.Format("Invalid visit: Duration claimed is not valid for care worker ({0}), {1}. <br/>", cProvider.CareProviderName, visitDetail.DayOfWeek))
                    End If
                    If Utils.ToInt32(visitDetail.ServiceTypeID) <= 0 Then
                        errMmsg.Append(String.Format("Invalid visit: Service type is not selected for care worker ({0}), {1}. <br/>", cProvider.CareProviderName, visitDetail.DayOfWeek))
                    End If
                    If Utils.ToInt32(visitDetail.VisitCodeID) <= 0 Then
                        errMmsg.Append(String.Format("Invalid visit: Visit code is not selected for care worker ({0}), {1}. <br/>", cProvider.CareProviderName, visitDetail.DayOfWeek))
                    End If
                Next
            Next

            msg.Message = String.Format("{0} {1} ", initialMsg, errMmsg.ToString())
            If errMmsg.ToString().Length > 1 Then
                msg.Success = False
            Else
                msg.Success = True
            End If
            Return msg
        End Function

        Private Sub ValidateSave(ByVal inv As eInvoice.Invoice)
            Dim errMmsg As StringBuilder = New StringBuilder

            If dteWeekEnding.Text.Trim() = "" Then
                errMmsg.Append("Please enter week ending date. <br/>")
            End If
            If txtReference.Text.Trim() = "" Then
                errMmsg.Append("Please enter Reference number. <br/>")
            End If
            If inv.NumberOfVisits = 0 Then
                errMmsg.Append("Please enter a visit. <br/>")
            End If
            lblError.Text = errMmsg.ToString()
        End Sub

        Private Sub InsertUpdateInvoice(ByRef e As StdButtonEventArgs, ByRef inv As eInvoice.Invoice)
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim batch As DomProformaInvoiceBatch = New DomProformaInvoiceBatch(Me.DbConnection, _
                                                                               auditUserName:=_auditUserName, _
                                                                               auditLogTitle:=_auditLogtitle)
            Dim invoice As DomProformaInvoice = New DomProformaInvoice(Me.DbConnection, _
                                                                       auditUserName:=_auditUserName, _
                                                                       auditLogTitle:=_auditLogtitle)
            Dim visit As DomProformaInvoiceVisitEx
            Dim visits As List(Of DomProformaInvoiceVisitEx) = Nothing
            Dim cNewWorkers As List(Of DomProformaInvoiceVisit_NewCareWorker) = Nothing
            Dim cNewWorker As DomProformaInvoiceVisit_NewCareWorker
            Dim visitToDelete As List(Of String)
            Dim dow As DayOfWeek
            Dim visitDate As Date
            Dim indicator As DomManuallyAmendedIndicator = Nothing
            If Me.IsValid Then

                ' check weekending date validity between payment schedule
                Dim pschedule As DataClasses.PaymentSchedule = New DataClasses.PaymentSchedule(Me.DbConnection, _
                                                                                               auditUserName:=_auditUserName, _
                                                                                               auditLogTitle:=_auditLogtitle)
                msg = New ErrorMessage
                msg = pschedule.Fetch(_pScheduleId)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    Exit Sub
                End If
                If Not pschedule Is Nothing Then
                    If Date.Parse(dteWeekEnding.Text) < pschedule.DateFrom _
                    Or Date.Parse(dteWeekEnding.Text) > pschedule.DateTo Then
                        lblError.Text = "Week ending date must fall within the date range of the Payment Schedule."
                        e.Cancel = True
                        Exit Sub
                    End If
                End If

                ' check week ending date
                msg = DomContractBL.ValidateWeekEndingDate(Me.DbConnection, dteWeekEnding.Text)
                If Not msg.Success Then
                    If msg.Number = DomContractBL.ERR_INVALID_WEEK_ENDING Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        Exit Sub
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If


                ''''''' perform  insert update or delete dunctions
                If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                    ' create the batch
                    With batch
                        .ProviderID = inv.ProviderID
                        .DomContractID = inv.ContractID
                        .UserID = currentUser.ExternalUserID
                        .DateCreated = DateTime.Now
                        .CreatedBy = String.Format("{0} {1}", currentUser.FirstName, currentUser.Surname)
                        If .CreatedBy.Length > 50 Then .CreatedBy = .CreatedBy.Substring(0, 50)
                        .DomProformaInvoiceBatchTypeID = DomProformaInvoiceBatchType.ManuallyEntered
                        .VisitBasedReturn = TriState.True
                        .DomProformaInvoiceBatchStatusID = DomProformaInvoiceBatchStatus.AwaitingVerification
                        .StatusDate = .DateCreated
                        .StatusChangedBy = .CreatedBy
                    End With

                ElseIf _stdBut.ButtonsMode = StdButtonsMode.Edit Then

                    'Fetch the Original Invoice, as we need to use the batch id off it
                    'to find the batch.

                    msg = invoice.Fetch(inv.ID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    'Fetch the existing Batch
                    msg = batch.Fetch(invoice.DomProformaInvoiceBatchID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                End If
                With invoice
                    .ClientID = inv.ClientID
                    .ServiceUserDetails = inv.ServiceUserDetails
                    .WETo = inv.WETo
                    .WEFrom = inv.WETo '.AddDays(-6)
                    .OurReference = inv.Reference
                    .ServiceUserContribution = inv.DirectIncome
                    Decimal.TryParse(inv.PaymentClaimed, .PaymentClaimed)
                    .Query = TriState.False
                    .InvoiceDate = DateTime.Today
                    ' for visit based this value as to be 1. weeks in proforma invoice
                End With

                ' 

                ' visits to insert
                cNewWorkers = New List(Of DomProformaInvoiceVisit_NewCareWorker)
                visits = New List(Of DomProformaInvoiceVisitEx)
                ' visits to delete
                visitToDelete = New List(Of String)
                ' cprovider loop
                For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider

                    For Each visitDetail As eInvoice.DailyVisitDetail In cProvider.listDailyVisitAllDays
                        ' set to be deleted
                        If visitDetail.MarkedToDelete And Utils.ToInt32(visitDetail.VisitID) <> 0 Then
                            visitToDelete.Add(visitDetail.VisitID)
                        Else

                            ' create the empty visit record
                            visit = New DomProformaInvoiceVisitEx(_auditUserName, _auditLogtitle)
                            visit.DbConnection = Me.DbConnection
                            ' create dom proforma invoice Visit care worker object 
                            cNewWorker = New DomProformaInvoiceVisit_NewCareWorker(cProvider.Reference, _
                                                                                   cProvider.CareProviderName, _
                                                                                   cProvider.CareProviderID, _
                                                                                   visit.UniqueIdForCareWorker)

                            ' we are updating
                            'If Not visitDetail.MarkedToDelete And Utils.ToInt32(visitDetail.VisitID) <> 0 Then
                            'msg = visit.Fetch(visitDetail.VisitID)
                            'If Not msg.Success Then WebUtils.DisplayError(msg)
                            'End If
                            ' set the visit properties
                            If Not visitDetail.MarkedToDelete Then
                                With visit
                                    .DomServiceTypeID = visitDetail.ServiceTypeID
                                    ' start with the w/e date and wind back to the specified dow
                                    visitDate = inv.WETo
                                    dow = [Enum].Parse(GetType(DayOfWeek), visitDetail.VisitDay)
                                    While visitDate.DayOfWeek <> dow
                                        visitDate = visitDate.AddDays(-1)
                                    End While
                                    .VisitDate = visitDate
                                    .StartTimeClaimed = _date.Add(visitDetail.StartTime)
                                    .DurationClaimed = _date.Add(visitDetail.DurationClaimed) 'DateTime.Parse(time.ToString(DomContractBL.TIME_ONLY_DATE)) 'DateTime.Parse(visitDetail.StartTime.ToString())
                                    .NumberOfCarers = visitDetail.NumberOfCarers
                                    .SecondaryVisit = visitDetail.SecondaryVisit
                                    .DomVisitCodeID = visitDetail.VisitCodeID
                                    .ActualStartTime = .StartTimeClaimed
                                    .ActualDuration = _date.Add(visitDetail.ActualDuration)
                                    msg = DomContractBL.GetManuallyAmendedIndicatorForManualVisit(Me.DbConnection, _contractID, .VisitDate, indicator)
                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                    .DomManuallyAmendedIndicatorID = indicator.ID
                                End With
                                visits.Add(visit)
                                cNewWorkers.Add(cNewWorker)
                            End If
                        End If
                    Next
                    ' visit details loop
                Next
                ' cprovider loop

                '==================================================================================================================
                ' Create transaction and saveproforma invoice in transc 
                '==================================================================================================================
                Dim trans As SqlTransaction = Nothing
                trans = SqlHelper.GetTransaction(Me.DbConnection)
                'save the invoice
                msg = DomContractBL.SaveProformaInvoice(trans, _
                                                        currentUser.ExternalUsername, _
                                                        AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                        batch, _
                                                        invoice, _
                                                        visits, _
                                                        cNewWorkers, _
                                                        Me.Settings.CurrentApplicationID, _
                                                        _pScheduleId _
                                                        )
                If msg.Success Then
                    trans.Commit()
                Else
                    SqlHelper.RollbackTransaction(trans)
                End If
                '==================================================================================================================
                If Not msg.Success Then
                    If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_PROFORMA_INVOICE Or _
                            msg.Number = DomContractConvertTimeToUnits.ERR_COULD_NOT_CATEGORISE Or _
                            msg.Number = DomContractConvertTimeToUnits.ERR_COULD_NOT_DETERMINE_COSTS Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                Else
                    inv.ID = invoice.ID
                    invViewState = inv

                    Dim url As StringBuilder = New StringBuilder
                    url.Append(String.Format("{0}?=null" & _
                                             "&estabid={1}" & _
                                             "&contractid={2}" & _
                                             "&pscheduleid={3}" & _
                                             "&clientid={4}" & _
                                             "&invoiceid={5}" & _
                                             "&id={6}" & _
                                             "&mode=1" & _
                                             "&pSWE={7}" & _
                                             "&new={8}" & _
                                             "&backUrl=-1", _
                                             Request.Url.AbsolutePath, _
                                             batch.ProviderID, _
                                             batch.DomContractID, _
                                             invoice.PaymentScheduleID, _
                                             invoice.ClientID, _
                                             invoice.ID, _
                                             invoice.ID, _
                                             WeekEndingDate.ToString(), _
                                             True.ToString() _
                                             ))

                    Response.Redirect(HttpUtility.UrlDecode(url.ToString()))
                End If

                OriginalValueChanged.Value = "false"

            End If
        End Sub

        Private Function RefilleInvoiceFromPage(ByVal inv As eInvoice.Invoice) As eInvoice.Invoice

            For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                Dim cpVisitsID As String = cpManualVisitsID & cProvider.ObjectIndex
                Dim cpManualVisits As ManualEnterVisits = _
                DirectCast(WebUtils.FindControlRecursive(PnlVisits, cpVisitsID), ManualEnterVisits)
                If cpManualVisits Is Nothing Then
                    Continue For
                End If
                ' Monday visits
                Dim VisitsMonday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridMonday")
                Dim grdMonday As GridView = WebUtils.FindControlRecursive(VisitsMonday, "gvVisits")
                RefillVisitDetails(grdMonday, cProvider)
                ' Tuesday visits
                Dim VisitsTuesday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridTuesday")
                Dim grdTuesday As GridView = WebUtils.FindControlRecursive(VisitsTuesday, "gvVisits")
                RefillVisitDetails(grdTuesday, cProvider)
                ' Wednesday visits
                Dim VisitsWednesday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridWednesday")
                Dim grdWednesday As GridView = WebUtils.FindControlRecursive(VisitsWednesday, "gvVisits")
                RefillVisitDetails(grdWednesday, cProvider)
                ' Thursday visits
                Dim VisitsThurdsay As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridThursday")
                Dim grdThursday As GridView = WebUtils.FindControlRecursive(VisitsThurdsay, "gvVisits")
                RefillVisitDetails(grdThursday, cProvider)
                ' Friday visits
                Dim VisitsFriday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridFriday")
                Dim grdFriday As GridView = WebUtils.FindControlRecursive(VisitsFriday, "gvVisits")
                RefillVisitDetails(grdFriday, cProvider)
                ' Saturday visits
                Dim VisitsSaturday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridSaturday")
                Dim grdSaturday As GridView = WebUtils.FindControlRecursive(VisitsSaturday, "gvVisits")
                RefillVisitDetails(grdSaturday, cProvider)
                ' Sunday visits
                Dim VisitsSunday As UserControl = WebUtils.FindControlRecursive(cpManualVisits, "VisitGridSunday")
                Dim grdSunday As GridView = WebUtils.FindControlRecursive(VisitsSunday, "gvVisits")
                RefillVisitDetails(grdSunday, cProvider)

            Next

            Return inv
        End Function

        Public Sub RefillVisitDetails(ByVal grd As GridView, ByRef cProvider As eInvoice.CareProvider)

            Dim selectedValue As String = "0"
            Dim ddlHours As DropDownList
            Dim ddlMinutes As DropDownList
            Dim ddlServiceType As DropDownList
            Dim ddlVisitCode As DropDownList
            Dim txtNoOfCarers As TextBox
            Dim chkSecondaryVisit As CheckBox
            Dim markAsUpdated As Boolean = False

            For Each dr As GridViewRow In grd.Rows
                Dim visitObjectIndex As Integer = grd.DataKeys(dr.RowIndex).Value
                For Each visit As eInvoice.DailyVisitDetail In cProvider.listDailyVisitAllDays
                    If visit.ObjectIndex = visitObjectIndex Then
                        markAsUpdated = False
                        ' service type 
                        ddlServiceType = dr.Cells(0).FindControl("ddlServiceType")
                        If visit.ServiceTypeID <> Utils.ToInt32(ddlServiceType.SelectedValue) Then
                            markAsUpdated = True
                            visit.ServiceTypeID = Utils.ToInt32(ddlServiceType.SelectedValue)
                        End If
                        ' start time hours
                        ddlHours = dr.Cells(1).FindControl("ddlStartTimeHours")
                        If visit.StartTimeHours <> ddlHours.SelectedValue Then
                            markAsUpdated = True
                        End If
                        ' start time  minutes
                        ddlMinutes = dr.Cells(1).FindControl("ddlStartTimeMinutes")
                        If visit.StartTimeMinutes <> ddlMinutes.SelectedValue Then
                            markAsUpdated = True
                        End If
                        visit.StartTime = New TimeSpan(TimeSpan.FromMinutes(ddlHours.SelectedValue * 60 + ddlMinutes.SelectedValue).Ticks)
                        ' end time hours
                        ddlHours = dr.Cells(2).FindControl("ddlEndTimeHours")
                        If visit.EndTimeHours <> ddlHours.SelectedValue Then
                            markAsUpdated = True
                        End If
                        ' End time minutes
                        ddlMinutes = dr.Cells(2).FindControl("ddlEndTimeMinutes")
                        If visit.EndTimeMinutes <> ddlMinutes.SelectedValue Then
                            markAsUpdated = True
                        End If
                        visit.EndTime = New TimeSpan(TimeSpan.FromMinutes(ddlHours.SelectedValue * 60 + ddlMinutes.SelectedValue).Ticks)
                        ' Duration Claimed hours
                        ddlHours = dr.Cells(3).FindControl("ddlDurationClaimedHours")
                        If visit.DurationClaimedHours <> ddlHours.SelectedValue Then
                            markAsUpdated = True
                        End If
                        ' Duration claimed minutes
                        ddlMinutes = dr.Cells(3).FindControl("ddlDurationClaimedMinutes")
                        If visit.DurationClaimedMinutes <> ddlHours.SelectedValue Then
                            markAsUpdated = True
                        End If
                        visit.DurationClaimed = New TimeSpan(TimeSpan.FromMinutes(ddlHours.SelectedValue * 60 + ddlMinutes.SelectedValue).Ticks)
                        ' Actual duration hours
                        ddlHours = dr.Cells(4).FindControl("ddlActualDurationHours")
                        If visit.ActualDurationHours <> ddlHours.SelectedValue Then
                            markAsUpdated = True
                        End If
                        ' Actual duration minutes
                        ddlMinutes = dr.Cells(4).FindControl("ddlActualDurationMinutes")
                        If visit.ActualDurationMinutes <> ddlMinutes.SelectedValue Then
                            markAsUpdated = True
                        End If
                        visit.ActualDuration = New TimeSpan(TimeSpan.FromMinutes(ddlHours.SelectedValue * 60 + ddlMinutes.SelectedValue).Ticks)
                        ' number of careres
                        txtNoOfCarers = dr.Cells(5).FindControl("txtNumberOfCarers")
                        If visit.NumberOfCarers <> txtNoOfCarers.Text Then
                            markAsUpdated = True
                            visit.NumberOfCarers = txtNoOfCarers.Text
                        End If
                        ' Secondary visit
                        chkSecondaryVisit = dr.Cells(6).FindControl("chkSecondaryVisit")
                        visit.SecondaryVisit = chkSecondaryVisit.Checked
                        'If visit.SecondaryVisit <> chkSecondaryVisit.Checked Then
                        'markAsUpdated = True
                        'visit.SecondaryVisit = chkSecondaryVisit.Checked
                        'End If
                        ' Visit Code 
                        ddlVisitCode = dr.Cells(7).FindControl("ddlVisitCode")
                        If visit.VisitCodeID <> Utils.ToInt32(ddlVisitCode.SelectedValue) Then
                            markAsUpdated = True
                            visit.VisitCodeID = Utils.ToInt32(ddlVisitCode.SelectedValue)
                        End If
                    End If
                    ' mark the visitCode detail for final enteries in database.
                    visit.MarkedToUpdate = markAsUpdated
                Next
            Next

        End Sub

        'Public Sub RemoveinvoiceIdFromUrl()
        '    Dim url As Uri = New Uri(HttpUtility.UrlDecode(backUrl))
        '    Dim namevalues As System.Collections.Specialized.NameValueCollection = _
        '    HttpUtility.ParseQueryString(url.Query)
        '    Dim absoluteUrl As String = url.AbsolutePath
        '    namevalues.Remove("invoiceid")
        '    backUrl = HttpUtility.UrlEncode(absoluteUrl & "?" & namevalues.ToString())
        'End Sub



    End Class

   

End Namespace