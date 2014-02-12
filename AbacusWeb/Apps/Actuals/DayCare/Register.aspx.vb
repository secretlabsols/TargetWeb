Imports System.Collections.Generic
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.ServiceRegisters
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Library
Imports System.Text
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Web.Script.Serialization

Namespace Apps.Actuals.DayCare

    ''' <summary>
    ''' Web Form for Maintaining Service Registers
    ''' </summary>
    ''' <history>
    ''' MoTahir  11/01/2013 A7703 - Following the failure of A4WA#7434, When attempting to submit a service 
    '''                             register when the System setting Prevent entry of actual service for future 
    '''                             periods?’ is set to ‘True’, the screen just freezes and no error message 
    '''                             is displayed.
    ''' ColinD   10/10/2011 I155   - Updated - Register Reports button text set to Show instead of Report
    ''' ColinD   05/07/2011 D11240 - Created
    ''' </history>
    Partial Public Class Register
        Inherits BasePage

#Region "Fields"

        ' constants
        Private Const _AuditLogTableRegister As String = "Register"
        Private Const _AuditLogTableRegisterDay As String = "RegisterDay"
        Private Const _AuditLogTableRegisterRow As String = "RegisterRow"
        Private Const _AuditLogTableRegisterColumn As String = "RegisterColumn"
        Private Const _AuditLogTableRegisterCell As String = "RegisterCell"
        Private Const _AuditLogTableRegisterClientStatus As String = "RegisterClientStatus"
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _PageTitle As String = "Service Register"
        Private Const _QsRegisterID As String = "ID"
        Private Const _ReportRegisterReportID As String = "AbacusIntranet.WebReport.DayCareRegisterDetails"
        Private Const _WebCmdDeleteKey As String = "AbacusIntranet.WebNavMenuItemCommand.DayCare.Delete"
        Private Const _WebCmdEditKey As String = "AbacusIntranet.WebNavMenuItemCommand.DayCare.Edit"
        Private Const _WebCmdSubmitKey As String = "AbacusIntranet.WebNavMenuItemCommand.DayCare.Submit"
        Private Const _WebCmdUnSubmitKey As String = "AbacusIntranet.WebNavMenuItemCommand.DayCare.UnSubmit"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.DayCare"

        ' locals
        Private _Register As ViewableRegister = Nothing
        Private _RegisterClientStatuses As List(Of ViewableRegisterClientStatus) = Nothing
        Private _RegisterDays As List(Of ViewableRegisterDay) = Nothing
        Private _RegisterRateCategories As List(Of ViewableRateCategory) = Nothing
        Private _RegisterRateCategoryPreclusions As List(Of Target.Abacus.Library.ViewableRatePreclusion) = Nothing
        Private _RegisterServiceOutcomes As List(Of ViewableRegisterServiceOutcome) = Nothing
        Private _RegisterRateCategoryInclusions As Target.Abacus.Library.DataClasses.Collections.vwDomRateCategoryInclusionsCollection = Nothing
        Private _ReportButton As HtmlInputButton = Nothing

#End Region

#Region "Properties"

#Region "Authorisation Properties"

        ''' <summary>
        ''' Gets a value indicating whether can delete new records.
        ''' </summary>
        ''' <value><c>true</c> if user can delete new records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasDeleteCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdDeleteKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can edit records.
        ''' </summary>
        ''' <value><c>true</c> if user can edit records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasEditCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdEditKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can submit records.
        ''' </summary>
        ''' <value><c>true</c> if user can submit records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasSubmitCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdSubmitKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can un submit records.
        ''' </summary>
        ''' <value><c>true</c> if user can un submit records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasUnSubmitCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdUnSubmitKey))
            End Get
        End Property

#End Region

#Region "QueryString Properties"

        ''' <summary>
        ''' Gets the register ID from the query string.
        ''' </summary>
        ''' <value>The register ID.</value>
        Private ReadOnly Property RegisterID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(_QsRegisterID))
            End Get
        End Property

#End Region

#Region "Report Properties"

        ''' <summary>
        ''' Gets the report button.
        ''' </summary>
        ''' <value>The report button.</value>
        Private ReadOnly Property ReportButton() As HtmlInputButton
            Get
                If _ReportButton Is Nothing Then
                    ' if not created the reports button do so
                    _ReportButton = New HtmlInputButton()
                    With _ReportButton
                        .ID = "_btnReports"
                        .Value = "Reports"
                    End With
                End If
                Return _ReportButton
            End Get
        End Property

        ''' <summary>
        ''' Gets the report button for the register report.
        ''' </summary>
        ''' <value>The report button register report.</value>
        Private ReadOnly Property ReportButtonRegisterReport() As IReportsButton
            Get
                Return CType(rptBtnRegisterReport, IReportsButton)
            End Get
        End Property

        ''' <summary>
        ''' Gets the report button for the blank printable register report.
        ''' </summary>
        ''' <value>The report button printable register report.</value>
        Private ReadOnly Property ReportButtonPrintableBlankRegisterReport() As HtmlInputButton
            Get
                Return rptBtnPrintableRegisterBlank
            End Get
        End Property

        ''' <summary>
        ''' Gets the report button for the complete register report.
        ''' </summary>
        ''' <value>The report button printable register report.</value>
        Private ReadOnly Property ReportButtonPrintableCompleteRegisterReport() As HtmlInputButton
            Get
                Return rptBtnPrintableRegisterComplete
            End Get
        End Property

        ''' <summary>
        ''' Gets the reports container.
        ''' </summary>
        ''' <value>The reports container.</value>
        Private ReadOnly Property ReportsContainer() As AjaxControlToolkit.CollapsiblePanelExtender
            Get
                Return CType(cpeReportsContainer, AjaxControlToolkit.CollapsiblePanelExtender)
            End Get
        End Property

        ''' <summary>
        ''' Gets the report register report ID.
        ''' </summary>
        ''' <value>The report register report ID.</value>
        Private ReadOnly Property ReportRegisterReportID() As Integer
            Get
                Return Target.Library.Web.ConstantsManager.GetConstant(_ReportRegisterReportID)
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Gets the register.
        ''' </summary>
        ''' <value>The register.</value>
        Private ReadOnly Property Register() As ViewableRegister
            Get
                If _Register Is Nothing Then
                    ' if we havent fetched the item then do so, throw error if encountered
                    Dim msg As ErrorMessage = ServiceRegisterBL.GetRegister(DbConnection, RegisterID, _Register)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _Register
            End Get
        End Property

        ''' <summary>
        ''' Gets the register client statuses.
        ''' </summary>
        ''' <value>The register client statuses.</value>
        Private ReadOnly Property RegisterClientStatuses() As List(Of ViewableRegisterClientStatus)
            Get
                If _RegisterClientStatuses Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = ServiceRegisterBL.GetRegisterClientStatuses(DbConnection, RegisterID, _RegisterClientStatuses)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _RegisterClientStatuses
            End Get
        End Property

        ''' <summary>
        ''' Gets the register days.
        ''' </summary>
        ''' <value>The register days.</value>
        Private ReadOnly Property RegisterDays() As List(Of ViewableRegisterDay)
            Get
                If _RegisterDays Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = ServiceRegisterBL.GetRegisterDays(DbConnection, RegisterID, _RegisterDays)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _RegisterDays
            End Get
        End Property

        ''' <summary>
        ''' Gets the registers rate categories.
        ''' </summary>
        ''' <value>The registers rate categories.</value>
        Private ReadOnly Property RegisterRateCategories() As List(Of ViewableRateCategory)
            Get
                If _RegisterRateCategories Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim rateCategories As DomRateCategoryCollection = Nothing
                    Dim msg As ErrorMessage = DomRateCategory.FetchList(conn:=DbConnection, list:=rateCategories, domRateFrameworkID:=Register.DomRateFrameworkID, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                    _RegisterRateCategories = New List(Of ViewableRateCategory)()
                    For Each rateCategory As DomRateCategory In rateCategories
                        _RegisterRateCategories.Add(New ViewableRateCategory(rateCategory.Description, rateCategory.ID))
                    Next
                End If
                Return _RegisterRateCategories
            End Get
        End Property

        ''' <summary>
        ''' Gets the register service outcomes.
        ''' </summary>
        ''' <value>The register service outcomes.</value>
        Private ReadOnly Property RegisterServiceOutcomes() As List(Of ViewableRegisterServiceOutcome)
            Get
                If _RegisterServiceOutcomes Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = ServiceRegisterBL.GetRegisterServiceOutcomes(DbConnection, RegisterID, _RegisterServiceOutcomes)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _RegisterServiceOutcomes
            End Get
        End Property

        ''' <summary>
        ''' Gets the register rate category inclusions.
        ''' </summary>
        ''' <value>The register rate category inclusions.</value>
        Private ReadOnly Property RegisterRateCategoryInclusions() As Target.Abacus.Library.DataClasses.Collections.vwDomRateCategoryInclusionsCollection
            Get
                If _RegisterRateCategoryInclusions Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = Target.Abacus.Library.DataClasses.vwDomRateCategoryInclusions.FetchList(conn:=DbConnection, domRateFrameworkID:=Register.DomRateFrameworkID, list:=_RegisterRateCategoryInclusions)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _RegisterRateCategoryInclusions
            End Get
        End Property

        ''' <summary>
        ''' Gets the register rate category preclusions.
        ''' </summary>
        ''' <value>The register rate category preclusions.</value>
        Private ReadOnly Property RegisterRateCategoryPreclusions() As List(Of Target.Abacus.Library.ViewableRatePreclusion)
            Get
                If _RegisterRateCategoryPreclusions Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = Target.Abacus.Library.DomContractBL.FetchRatePreclusionsList(DbConnection, Register.DomContractID, _RegisterRateCategoryPreclusions)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _RegisterRateCategoryPreclusions
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        ''' <value>The standard buttons control.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)

            ' setup the standard buttons control
            With StandardButtonsControl
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = False
                .AllowDelete = UserHasDeleteCommand
                .AllowFind = False
                .AuditLogTableNames.Add(_AuditLogTableRegister)
                .AuditLogTableNames.Add(_AuditLogTableRegisterDay)
                .AuditLogTableNames.Add(_AuditLogTableRegisterRow)
                .AuditLogTableNames.Add(_AuditLogTableRegisterColumn)
                .AuditLogTableNames.Add(_AuditLogTableRegisterCell)
                .AuditLogTableNames.Add(_AuditLogTableRegisterClientStatus)
                .InitialMode = StdButtonsMode.Fetched
                .SelectedItemID = RegisterID
            End With

            ' populate register details i.e. status and headings
            PopulateRegister()

            ' populate the client statuses
            PopulateRegisterClientStatuses()

            ' setup js/reports

            SetupJavaScript()
            SetupReports()

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim registerToDisplay As ViewableRegister = Register

            If registerToDisplay.RegisterStatus <> ServiceRegisterBL.RegisterStatus.InProgress Then
                ' we can only delete registers if it is in progress

                StandardButtonsControl.AllowDelete = False

            End If

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim js As New StringBuilder()
            Dim jsSerializer As New JavaScriptSerializer()
            Dim jsonRateCategories As String = "[]"
            Dim jsonRateCategoryInclusions As String = "[]"
            Dim jsonRateCategoryPreclusions As String = "[]"
            Dim jsonRegisterDays As String = "[]"
            Dim jsonRegisterServiceOutcomes As String = "[]"
            Dim registerToDisplay As ViewableRegister = Register
            Dim serviceRegisterEditable As Boolean = False

            ' serialize the object to a json string
            With jsSerializer
                If Not RegisterRateCategories Is Nothing Then
                    jsonRateCategories = .Serialize(RegisterRateCategories.ToArray())
                End If
                If Not RegisterDays Is Nothing Then
                    jsonRegisterDays = .Serialize(RegisterDays.ToArray())
                End If
                If Not RegisterServiceOutcomes Is Nothing Then
                    jsonRegisterServiceOutcomes = .Serialize(RegisterServiceOutcomes.ToArray())
                End If
                If Not RegisterRateCategoryPreclusions Is Nothing Then
                    jsonRateCategoryPreclusions = .Serialize(RegisterRateCategoryPreclusions.ToArray())
                End If
                If Not RegisterRateCategoryInclusions Is Nothing Then
                    jsonRateCategoryInclusions = .Serialize((From tmpInclusion As vwDomRateCategoryInclusions In RegisterRateCategoryInclusions.ToArray() Select tmpInclusion Order By tmpInclusion.DomRateCategoryID).ToList())
                End If
            End With

            ' setup button visibility etc
            btnSubmit.Visible = False
            btnAddServiceUser.Visible = False
            btnClearAttendance.Visible = False

            If UserHasEditCommand Then
                ' only allow edit controls if 

                Select Case Register.RegisterStatus

                    Case ServiceRegisterBL.RegisterStatus.InProgress, ServiceRegisterBL.RegisterStatus.Amended
                        ' if in progress then user can submit

                        With btnSubmit
                            .Attributes.Add("onclick", "SubmitRegister();")
                            .Attributes.Add("title", "Submit this Service Register?")
                            .Value = "Submit"
                            .Visible = UserHasSubmitCommand
                        End With

                        btnAddServiceUser.Visible = True
                        If Register.RegisterStatus = ServiceRegisterBL.RegisterStatus.InProgress Then
                            btnClearAttendance.Visible = True
                        End If
                        serviceRegisterEditable = True

                    Case ServiceRegisterBL.RegisterStatus.Processed
                        ' if processed then we can only allow users to amend 

                        With btnSubmit
                            .Attributes.Add("onclick", "AmendRegister();")
                            .Attributes.Add("title", "Amend this Service Register?")
                            .Value = "Amend"
                            .Visible = True
                        End With

                    Case ServiceRegisterBL.RegisterStatus.Submitted
                        ' if submitted then user can only unsubmit

                        With btnSubmit
                            .Attributes.Add("onclick", "UnSubmitRegister();")
                            .Attributes.Add("title", "UnSubmit this Service Register?")
                            .Value = "UnSubmit"
                            .Visible = UserHasUnSubmitCommand
                        End With

                End Select

            End If

            ' setup js to be output to client
            js.AppendFormat("serviceRegisterID = {0};", registerToDisplay.ID)
            js.AppendFormat("var serviceRegisterRateCats = {0};", jsonRateCategories)
            js.AppendFormat("var serviceRegisterDays = {0};", jsonRegisterDays)
            js.AppendFormat("var serviceRegisterServiceOutcomes = {0};", jsonRegisterServiceOutcomes)
            js.AppendFormat("var serviceRegisterRateCategoryPreclusions = {0};", jsonRateCategoryPreclusions)
            js.AppendFormat("var serviceRegisterRateCategoryInclusions = {0};", jsonRateCategoryInclusions)
            js.AppendFormat("var serviceRegisterEditable = {0};", serviceRegisterEditable.ToString().ToLower())

            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub

#End Region

#Region "Functions/Methods"

        ''' <summary>
        ''' Standards the buttons control_ add custom controls.
        ''' </summary>
        ''' <param name="controls">The controls.</param>
        Private Sub StandardButtonsControl_AddCustomControls(ByRef controls As ControlCollection)

            controls.Add(ReportButton)

            With ReportsContainer
                .ExpandControlID = ReportButton.ClientID
                .CollapseControlID = .ExpandControlID
                .Collapsed = True
            End With

        End Sub

        ''' <summary>
        ''' Populates the register.
        ''' </summary>
        Private Sub PopulateRegister()

            Dim registerToDisplay As ViewableRegister = Register

            fsControlsLegend.InnerHtml = String.Format("Week Ending: {0} &nbsp; &nbsp; Provider: {1} &nbsp; &nbsp; Contract: {2} &nbsp; &nbsp; Status: {3}", registerToDisplay.WeekEnding.ToString(_DateFormat), registerToDisplay.ProviderName, registerToDisplay.DomContractTitle, registerToDisplay.RegisterStatusDescription)

        End Sub

        ''' <summary>
        ''' Populates the register client statuses table.
        ''' </summary>
        Private Sub PopulateRegisterClientStatuses()

            Dim statuses As List(Of ViewableRegisterClientStatus) = RegisterClientStatuses

            With rptRegisterClientStatuses
                .DataSource = statuses
                .DataBind()
            End With

        End Sub

       

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add in js link for handlers
            JsLinks.Add("Register.js")

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add reports js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large amounts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.ServiceRegisters))


            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Apps.UserControls.ServiceOrderReports", _
                 Target.Library.Web.Utils.WrapClientScript(String.Format("Report_lstReportlistId='{0}';", _
                                                             lstReports.ClientID _
                                                )) _
                                        )
        End Sub

        ''' <summary>
        ''' Set up the reports.
        ''' </summary>
        Private Sub SetupReports()

            AddHandler StandardButtonsControl.AddCustomControls, AddressOf StandardButtonsControl_AddCustomControls

            With lstReports
                .Rows = 3
                .Attributes.Add("onchange", "lstReports_Change();")
                With .Items
                    .Add(New ListItem("Printable Register (Blank)", divPrintableRegisterBlank.ClientID))
                    .Add(New ListItem("Printable Register (Complete)", divPrintableRegisterComplete.ClientID))
                    .Add(New ListItem("Register Report", divRegisterReport.ClientID))
                End With
            End With

            ' setup register report setup
            With ReportButtonRegisterReport
                .ButtonText = "Show"
                .ReportID = ReportRegisterReportID
                .Parameters.Add("intSelectedRegisterID", Register.ID)
                .Parameters.Add("blnInProgress", True)
                .Parameters.Add("blnSubmitted", True)
                .Parameters.Add("blnAmended", True)
                .Parameters.Add("blnProcessed", True)
            End With

            ' setup printable blank register
            With ReportButtonPrintableBlankRegisterReport
                .Attributes.Add("onclick", String.Format("ShowPrintableBlankRegisterReport();", Register.ID))
            End With

            ' setup printable complete register
            With ReportButtonPrintableCompleteRegisterReport
                .Attributes.Add("onclick", String.Format("ShowPrintableCompleteRegisterReport();", Register.ID))
            End With

        End Sub

#End Region

    End Class

End Namespace

