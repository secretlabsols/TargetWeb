Imports System.Collections.Generic
Imports System.Text
Imports System.Web.Script.Serialization
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.ProformaInvoices
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.Dom.ProformaInvoice

    ''' <summary>
    ''' Web Form for Maintaining Non Visit Based Pro Forma Invoices
    ''' </summary>
    ''' <history>
    ''' ColinD   22/11/2011 I393   - Updated PaymentScheduleRateCategories property to exclude Rate Categories without a Service Type defined.
    ''' ColinD   23/09/2011 I68    - Updated - _PageTitle from Pro Forma Invoices to Pro forma Invoices.
    ''' ColinD   18/08/2011 D12102 - Created
    ''' </history>
    Partial Class ViewNonVisitInvoices
        Inherits BasePage

#Region "Fields"

        ' constants
        Private Const _ImgInformationAmber As String = "info18_amber.png"
        Private Const _ImgInformationBlue As String = "info18_blue.png"
        Private Const _ImgInformationGrey As String = "info18_grey.png"
        Private Const _PageTitle As String = "Pro forma Invoices"
        Private Const _QsPaymentScheduleID As String = "pScheduleId"
        Private Const _QsProformaInvoiceStatusShowAwaitingVerification As String = "await"
        Private Const _QsProformaInvoiceStatusShowVerified As String = "ver"
        Private Const _ReportInvoicesID As String = "AbacusExtranet.WebReport.UnProcessedNonVisitBasedProformaInvoiceList"
        Private Const _WebCmdDeleteInvoiceKey As String = "AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.ProformaInvoice.Delete"
        Private Const _WebCmdEditProformaInvoiceKey As String = "AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.Delete.EditProformaInvoices"
        Private Const _WebCmdVerifyUnverifyInvoiceKey As String = "AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.ProformaInvoice.Verify"
        Private Const _WebCmdRecalculateInvoiceKey As String = "AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.Recalculate"
        Private Const _WebNavMenuItemKey As String = "AbacusExtranet.WebNavMenuItem.PaymentSchedules"

        ' locals
        Private _PaymentSchedule As PaymentSchedule = Nothing
        Private _PaymentScheduleRateCategories As DomRateCategoryCollection = Nothing
        Private _PaymentSchedulesDomProformaInvoices As List(Of ViewableDomProformaInvoice) = Nothing
        Private _WeekEndingDate As DateTime = DateTime.MinValue

        Private _currentContractIsBlockGuarantee As Boolean = False
        Private _currentContractIsPeriodicBlock As Boolean = False
        Private _verificationText As String = String.Empty
#End Region

#Region "Properties"

#Region "Authorisation Properties"

        ''' <summary>
        ''' Gets a value indicating whether can delete invoice records.
        ''' </summary>
        ''' <value><c>true</c> if user can delete invoice records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasDeleteInvoiceCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdDeleteInvoiceKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [user has edit proforma invoice command].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [user has edit proforma invoice command]; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property UserHasEditProformaInvoiceCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdEditProformaInvoiceKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can delete invoice records.
        ''' </summary>
        ''' <value><c>true</c> if user can delete invoice records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasVerifyUnverifyInvoiceCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdVerifyUnverifyInvoiceKey))
            End Get
        End Property


        Private ReadOnly Property UserHasRecalculateCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(WebUtils.ConstantsManager.GetConstant(_WebCmdRecalculateInvoiceKey))
            End Get
        End Property

        Private _InvoiceVText As String
        ''' <summary>
        ''' Gets or sets the invoice verification text.
        ''' </summary>
        ''' <value>
        ''' The invoice v text.
        ''' </value>
        Public Property InvoiceVText() As String
            Get
                Return _InvoiceVText
            End Get
            Set(ByVal value As String)
                _InvoiceVText = value
            End Set
        End Property

#End Region

#Region "QueryString Properties"

        ''' <summary>
        ''' Gets the payment schedule ID from the query string.
        ''' </summary>
        ''' <value>The payment schedule ID.</value>
        Private ReadOnly Property QsPaymentScheduleID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(_QsPaymentScheduleID))
            End Get
        End Property

        ''' <summary>
        ''' Gets the whether to show proforma invoice awaiting verification.
        ''' </summary>
        ''' <value>Whether to show proforma invoice awaiting verification.</value>
        Private ReadOnly Property QsProformaInvoiceStatusShowAwaitingVerification() As Nullable(Of Boolean)
            Get
                Return Utils.ToBoolean(Request.QueryString(_QsProformaInvoiceStatusShowAwaitingVerification))
            End Get
        End Property

        ''' <summary>
        ''' Gets the whether to show proforma invoice which are verified.
        ''' </summary>
        ''' <value>Whether to show proforma invoice which are verified.</value>
        Private ReadOnly Property QsProformaInvoiceStatusShowVerified() As Nullable(Of Boolean)
            Get
                Return Utils.ToBoolean(Request.QueryString(_QsProformaInvoiceStatusShowVerified))
            End Get
        End Property

#End Region

#Region "Report Properties"

        ''' <summary>
        ''' Gets the report button for the report.
        ''' </summary>
        ''' <value>The report button report.</value>
        Private ReadOnly Property ReportInvoicesButton() As IReportsButton
            Get
                Return CType(rptPrint, IReportsButton)
            End Get
        End Property

        ''' <summary>
        ''' Gets the report ID.
        ''' </summary>
        ''' <value>The report ID.</value>
        Private ReadOnly Property ReportInvoicesID() As Integer
            Get
                Return Target.Library.Web.ConstantsManager.GetConstant(_ReportInvoicesID)
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Gets the payment schedule.
        ''' </summary>
        ''' <value>The payment schedule.</value>
        Private ReadOnly Property PaymentSchedule() As PaymentSchedule
            Get
                If _PaymentSchedule Is Nothing Then
                    Dim msg As ErrorMessage
                    _PaymentSchedule = New PaymentSchedule(conn:=DbConnection, _
                                                           auditUserName:=String.Empty, _
                                                           auditLogTitle:=String.Empty)
                    msg = _PaymentSchedule.Fetch(QsPaymentScheduleID)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _PaymentSchedule
            End Get
        End Property

        ''' <summary>
        ''' Gets the payment schedule header control.
        ''' </summary>
        ''' <value>The payment schedule header control.</value>
        Private ReadOnly Property PaymentScheduleHeaderControl() As Apps.UserControls.PaymentScheduleHeader
            Get
                Return CType(cpDetail.FindControl("psHeader1"), Apps.UserControls.PaymentScheduleHeader)
            End Get
        End Property

        ''' <summary>
        ''' Gets the payment schedule rate categories.
        ''' </summary>
        ''' <value>The payment schedule rate categories.</value>
        Private ReadOnly Property PaymentScheduleRateCategories() As DomRateCategoryCollection
            Get
                If _PaymentScheduleRateCategories Is Nothing Then
                    ' if we havent fetched rate categories then do so
                    Dim tmpPaymentScheduleRateCategories As New DomRateCategoryCollection()
                    Dim msg As ErrorMessage = DomContract_DomRateCategory.FetchDomRateCategoryList(DbConnection, tmpPaymentScheduleRateCategories, String.Empty, String.Empty, PaymentSchedule.DomContractID)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                    _PaymentScheduleRateCategories = New DomRateCategoryCollection()
                    _PaymentScheduleRateCategories.AddRange((From tmpRateCategory In tmpPaymentScheduleRateCategories.ToArray() _
                                                                Select tmpRateCategory _
                                                                Where (tmpRateCategory.DomServiceTypeID > 0) _
                                                                Order By tmpRateCategory.Description Ascending, _
                                                                    tmpRateCategory.SortOrder Ascending).ToArray())
                End If
                Return _PaymentScheduleRateCategories
            End Get
        End Property

        ''' <summary>
        ''' Gets the current payment schedules dom proforma invoices.
        ''' </summary>
        ''' <value>The current payment schedules dom proforma invoices.</value>
        Private ReadOnly Property PaymentSchedulesDomProformaInvoices() As List(Of ViewableDomProformaInvoice)
            Get
                If _PaymentSchedulesDomProformaInvoices Is Nothing Then
                    ' if we havent fetched the item then do so, throw error if encountered
                    Dim msg As ErrorMessage = DomProformaInvoiceBL.GetDomProformaInvoicesByPaymentSchedule(DbConnection, QsPaymentScheduleID, 0, _PaymentSchedulesDomProformaInvoices)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)

                    If _PaymentSchedulesDomProformaInvoices.Where(Function(c) c.IsBlockGuranteeContract = True).ToList().Count > 0 Then
                        _currentContractIsBlockGuarantee = True
                    End If

                    If _PaymentSchedulesDomProformaInvoices.Where(Function(c) c.IsPeriodicBlockContract = True).ToList().Count > 0 Then
                        _currentContractIsPeriodicBlock = True
                    End If

                    '' check if custom verifivation text is available for this contract
                    Dim vText As VerificationText = New VerificationText(Me.DbConnection)
                    If _PaymentSchedulesDomProformaInvoices.Where(Function(c) c.VerificationTextID > 0).ToList().Count > 0 Then
                        msg = New ErrorMessage
                        msg = vText.Fetch(_PaymentSchedulesDomProformaInvoices.FirstOrDefault().VerificationTextID)
                        If Not msg.Success Then WebUtils.Utils.DisplayError(msg)

                        _verificationText = vText.Description
                    Else
                        '' get default verification text
                        msg = New ErrorMessage
                        msg = GetNonResidentialVerificationText(_verificationText)
                        If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                    End If

                End If
                Return _PaymentSchedulesDomProformaInvoices
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

        ''' <summary>
        ''' Gets the week ending date.
        ''' </summary>
        ''' <value>The week ending date.</value>
        Private ReadOnly Property WeekEndingDate() As DateTime
            Get
                If _WeekEndingDate = DateTime.MinValue Then
                    _WeekEndingDate = Target.Abacus.Library.DomContractBL.GetWeekEndingDate(DbConnection, Nothing)
                End If
                Return _WeekEndingDate
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
                .AllowDelete = False
                .AllowFind = False
                .InitialMode = StdButtonsMode.Fetched
                .SelectedItemID = QsPaymentScheduleID
            End With

            ' populate proforma invoices
            PopulateProformaInvoices()

            ' setup css/js/reports
            SetupButtonAvailability()  
            SetupJavaScript()
            SetupPaymentScheduleHeaderControl()
            SetupReports()

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
            Dim jsonPaymentSchedule As String = "[]"

            ' serialize the objects to a json strings
            With jsSerializer
                If Not PaymentScheduleRateCategories Is Nothing Then
                    jsonRateCategories = .Serialize(PaymentScheduleRateCategories.ToArray())
                End If
                jsonPaymentSchedule = .Serialize(PaymentSchedule)
            End With

            js.AppendFormat("var qsPaymentScheduleID = {0};", QsPaymentScheduleID.ToString().ToLower())
            js.AppendFormat("var qsProformaInvoiceStatusShowAwaitingVerification = {0};", QsProformaInvoiceStatusShowAwaitingVerification.ToString().ToLower())
            js.AppendFormat("var qsProformaInvoiceStatusShowVerified = {0};", QsProformaInvoiceStatusShowVerified.ToString().ToLower())
            js.AppendFormat("var userHasDeleteInvoiceCommand = {0};", UserHasDeleteInvoiceCommand.ToString().ToLower())
            js.AppendFormat("var userHasEditProformaInvoiceCommand = {0};", UserHasEditProformaInvoiceCommand.ToString().ToLower())
            js.AppendFormat("var userHasVerifyUnverifyInvoiceCommand = {0};", UserHasVerifyUnverifyInvoiceCommand.ToString().ToLower())
            js.AppendFormat("var userHasRecalculateCommand = {0};", UserHasRecalculateCommand.ToString().ToLower())
            js.AppendFormat("var proformaInvoicesRateCats = {0};", jsonRateCategories)
            js.AppendFormat("var weekEndingDay = {0};", CType(WeekEndingDate.DayOfWeek, Integer))
            js.AppendFormat("var paymentSchedule = {0};", jsonPaymentSchedule)
            js.AppendFormat("var currentContractIsBlockGuarantee = {0};", _currentContractIsBlockGuarantee.ToString().ToLower())
            js.AppendFormat("var currentContractIsPeriodicBlock = {0};", _currentContractIsPeriodicBlock.ToString().ToLower())
            js.AppendFormat("var verificationText= '{0}';", _verificationText)

            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub

#End Region

#Region "Functions/Methods"

        ''' <summary>
        ''' Gets the non residential verification text.
        ''' </summary>
        ''' <param name="text">The text.</param>
        ''' <returns></returns>
        Public Function GetNonResidentialVerificationText(ByRef text As String) As ErrorMessage

            Dim msg As New ErrorMessage

            Dim vTextCollection As VerificationTextCollection = New VerificationTextCollection
            msg = VerificationText.FetchList(conn:=Me.DbConnection, list:=vTextCollection, isDefault:=TriState.True, isForNonResidential:=TriState.True)

            text = Utils.ToString(vTextCollection.ToArray().FirstOrDefault().Description)

            Return msg

        End Function


        ''' <summary>
        ''' Checks if a string is null or empty and returns nbsp if it is
        ''' </summary>
        ''' <param name="val">The val.</param>
        ''' <returns></returns>
        Protected Function CheckNullOrEmpty(ByVal val As String) As String

            If String.IsNullOrEmpty(val) Then

                Return "&nbsp;"

            End If

            Return val

        End Function

        ''' <summary>
        ''' Populates Proforma Invoices.
        ''' </summary>
        Private Sub PopulateProformaInvoices()

            Dim invoice As List(Of ViewableDomProformaInvoice) = PaymentSchedulesDomProformaInvoices

            With rptProformas
                .DataSource = invoice
                .DataBind()
            End With

        End Sub

        ''' <summary>
        ''' Sets up any buttons availability.
        ''' </summary>
        Private Sub SetupButtonAvailability()

            btnDeleteProforma.Visible = UserHasDeleteInvoiceCommand
            btnUnVerifyProforma.Visible = UserHasVerifyUnverifyInvoiceCommand
            btnVerifyProforma.Visible = UserHasVerifyUnverifyInvoiceCommand
            btnRecalculate.Visible = UserHasRecalculateCommand

        End Sub

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add in js link for handlers
            JsLinks.Add("ViewNonVisitInvoices.js")

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add the date js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))

            ' add reports js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/NonDelivery/NonDeliveryUnitBasedDialog.js"))

            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/NonDelivery/NonDeliveryVisitBasedDialog.js"))


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

            ' add the jquery tooltip
            UseJqueryTooltip = True

            UseExt = True
            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomProfomaInvoice))

        End Sub

        ''' <summary>
        ''' Setups the payment schedule header control.
        ''' </summary>
        Private Sub SetupPaymentScheduleHeaderControl()

            With PaymentScheduleHeaderControl
                .SingleLiner = False
                .PaymentScheduleId = QsPaymentScheduleID
            End With

        End Sub

        ''' <summary>
        ''' Set up the reports.
        ''' </summary>
        Private Sub SetupReports()

            ' setup register report setup
            With ReportInvoicesButton
                .ReportID = ReportInvoicesID
                .ButtonText = "Print"
                .Parameters.Add("intPaymentScheduleID", QsPaymentScheduleID)
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.TopRight
                If (QsProformaInvoiceStatusShowAwaitingVerification.HasValue AndAlso QsProformaInvoiceStatusShowAwaitingVerification.Value = True) _
                    AndAlso QsProformaInvoiceStatusShowVerified.HasValue AndAlso QsProformaInvoiceStatusShowVerified.Value = True Then
                    .Parameters.Add("intBatchStatus", 0)
                ElseIf (QsProformaInvoiceStatusShowAwaitingVerification.HasValue AndAlso QsProformaInvoiceStatusShowAwaitingVerification.Value = True) Then
                    .Parameters.Add("intBatchStatus", Target.Abacus.Library.DomProformaInvoiceBatchStatus.AwaitingVerification)
                ElseIf QsProformaInvoiceStatusShowVerified.HasValue AndAlso QsProformaInvoiceStatusShowVerified.Value = True Then
                    .Parameters.Add("intBatchStatus", Target.Abacus.Library.DomProformaInvoiceBatchStatus.Verified)
                End If
            End With

        End Sub

#End Region

        Public Function HasUserVerifyUnverify() As Boolean
            Return UserHasVerifyUnverifyInvoiceCommand
        End Function

    End Class

End Namespace