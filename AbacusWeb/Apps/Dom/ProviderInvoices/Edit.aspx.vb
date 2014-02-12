
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.Extensions.AjaxControlToolkit
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Library.Collections
Imports Target.Abacus.Library.PaymentTolerance
Imports System.Web.Script.Serialization

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen used to view/maintain a domiciliary provider invoice.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir 06/12/2011  BTI409 Unit of measure disappears from the (Planned) Units column when adding a new line
    ''' MoTahir 04/12/2011  BTI541 Tolerances placed on the contract are not taking affect  
    ''' PaulW   23/11/2011  Beta Testing Issue 397 - Add and Remove buttons reinstated for Summary level invoices
    ''' PaulW   17/11/2011  Beta testing Issue 343 - Invoice date included time element. 
    ''' MoTahir 01/11/2011  D12229 – eInvoicing - Amend Non-Residential Creditor Payment Filters
    ''' ColinD  13/10/2011  I208   - Fixed issue with attempting to serialize js for PaymentTolerances when working with new provider invoices
    ''' MoTahir 18/08/2011  D11766 - eInvoicing - Provider Invoice Tolerances
    ''' ColinD  09/08/2011  D11965 - Numerous changes as stated in spec.
    ''' ColinD  17/05/2011  SDS705 - Validate that establishments stated in the querystring are valid dom establisments.
    ''' PaulW   12/05/2011  A4WA#6242 - Extra validation for future invoice dates.
    ''' JohnF   06/04/2011  A4WA#6174 - Ensure view state reset on Page Load
    ''' ColinD  03/03/2011  D11874 - Security changes for introduction of Creditor Payments. Small changes to the way the window behaves if in popup window.
    ''' MoTahir 22/12/2010  A4WA#6580 - VAT Rate was not working correctly
    ''' ColinD  01/11/2010  SDS Issue 262 - Removed valdiation of provider, contract and service user after the Create button has been clicked
    ''' MikeVO  01/09/2010  Added validation summary.
    ''' ColinD  08/06/2010  A4WA#6321 - adjusted CancelClicked method to redirect user to previous page if the record has not been saved
    ''' ColinD  07/06/2010  A4WA#6320 - various minor issues fixed
    ''' ColinD  21/05/2010  D11845 - Various minor issues
    ''' Mo Tahir19/05/2010  D11806 - Rate Category Ordering 
    ''' ColinD  11/05/2010  D11746 - Prevent entry of future dates
    ''' JohnF   05/05/2010  A4WA#6246 - Show 'S/U Units' column only for summary-based invs
    ''' JohnF   29/04/2010  A4WA#6064 - Ensure entered VAT is saved on DPI record
    ''' ColinD  15/04/2010  D11745 - altered PopulateSummary() method to hide Penalty Payment entry if application setting PreventEntryPenaltyPayments = 1
    ''' MikeVO  08/04/2010  A4WA#6207 - ensure newly added lines default S/U Units to Units value. 
    ''' JohnF   17/03/2010  D11744 - default S/U Units to Units value on new inv details
    ''' JohnF   15/03/2010  D11744 - added 'service user units' column
    ''' PaulW   05/01/2010  A4WA#5983 summary tab not shown for attendance style invoices.
    ''' MikeVO  30/07/2009  D11547 - various minor issues.
    ''' MikeVO  02/06/2009  D11554 - support for first week of service.
    ''' MikeVO  09/03/2009  A4WA#5289 - fixes after testing of D11492.
    ''' MikeVO  26/02/2009  D11492 - created.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Edit
        Inherits BasePage


#Region " Properties "
        Private _ShowNonDelivery As Boolean = True
        Public Property ShowNonDelivery() As Boolean
            Get
                Return _ShowNonDelivery
            End Get
            Set(ByVal value As Boolean)
                _ShowNonDelivery = value
            End Set
        End Property

        Private _InvoiceID As Integer
        Public Property InvoiceID() As Integer
            Get
                Return _InvoiceID
            End Get
            Set(ByVal value As Integer)
                _InvoiceID = value
                Dim thePage As BasePage = CType(Me.Page, BasePage)
                GetInvoiceNotes(_InvoiceID)
            End Set
        End Property

        Private _InvoiceHasNotes As Boolean
        Public Property InvoiceHasNotes() As Boolean
            Get
                Return _InvoiceHasNotes
            End Get
            Set(ByVal value As Boolean)
                _InvoiceHasNotes = value
            End Set
        End Property


#End Region

#Region " Consts "

        Private Const SESSION_NEW_DOM_PROVIDER_INVOICE As String = "NewDomProviderInvoiceData"

        Const VIEWSTATE_KEY_DATA_DETAILS As String = "DetailDataList"
        Const VIEWSTATE_KEY_COUNTER_NEW_DETAILS As String = "DetailCounter"
        Const VIEWSTATE_KEY_DATA_DETAILS_REMOVED As String = "RemovedDetailDataList"

        Const CTRL_PREFIX_DETAIL_WE As String = "detailWE"
        Const CTRL_PREFIX_DETAIL_RATE_CATEGORY As String = "detailRateCat"

        Const CTRL_PREFIX_DETAIL_PLANNED_UNITS As String = "detailPlannedUnits"
        Const CTRL_PREFIX_DETAIL_PLANNED_RATE As String = "detailPlannedRate"
        Const CTRL_PREFIX_DETAIL_PLANNED_COST As String = "detailPlannedCost"

        Const CTRL_PREFIX_DETAIL_OTHER_UNITS As String = "detailOtherUnits"
        Const CTRL_PREFIX_DETAIL_OTHER_COST As String = "detailOtherCost"

        Const CTRL_PREFIX_DETAIL_THIS_UNITS As String = "detailThisUnits"
        Const CTRL_PREFIX_DETAIL_THIS_SU_UNITS As String = "detailThisSUUnits"
        Const CTRL_PREFIX_DETAIL_THIS_RATE As String = "detailThisRate"
        Const CTRL_PREFIX_DETAIL_THIS_COST As String = "detailThisCost"

        Const CTRL_PREFIX_DETAIL_REMOVE As String = "detailRemove"

        Const UNIQUEID_PREFIX_NEW_DETAIL As String = "detailN"
        Const UNIQUEID_PREFIX_UPDATE_DETAIL As String = "detailU"
        Const UNIQUEID_PREFIX_DELETE_DETAIL As String = "detailD"
        Const DateAndTimeFormat As String = "dd/MM/yyyy"
        Const CTRL_TOLERANCE_ID As String = "toleranceID"
        Const CTRL_TOLERANCE_TYPE As String = "toleranceType"

        Const CTRL_PREFIX_DETAIL_PLANNED_UNITS_MEASURED_IN As String = "detailPlannedUnitsMeasuredIn"


#End Region

#Region " Private variables "

        Private _stdBut As StdButtonsBase
        Private _estabID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _dpi As DomProviderInvoiceBL
        Private _newDetailIDCounter As Integer
        Private _rateCategories As DomRateCategoryCollection = Nothing
        Private _uoms As DomUnitsOfMeasureCollection = Nothing
        Private _detailLineStartupJS As StringBuilder = New StringBuilder()
        Private _canEditLineCost As Boolean, _canEditSuspendedInvoices As Boolean
        Private _btnAuditDetails As HtmlInputButton = New HtmlInputButton("button")
        Private _isAutoSuspensionReason As Boolean
        Private _naturalPeriodOfInvoicesScript As String = String.Empty
        Private _preventEntryOfFutureDates As Boolean = False
        Private _jsoPaymentTolerances As String = "[]"
        Private _jsoPaymentToleranceGroups As String = "[]"
        Private _controlsToenable As New List(Of String)
#End Region

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            With _stdBut
                AddHandler .AddCustomControls, AddressOf StdButtons_AddCustomControls
            End With

            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .hideCreditorRef = True
                .hideDebtorRef = True
            End With

        End Sub

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.CreditorPayments"), "Non-Residential Provider Invoice")
            Me.ShowValidationSummary = True

            Dim msg As ErrorMessage
            Dim sysInfo As SystemInfo
            Dim currentUser As WebSecurityUser
            Dim css As StringBuilder = New StringBuilder()
            Dim detailList As List(Of String)
            Dim establishment As New Establishment(DbConnection)
            Dim jsSerializer As New JavaScriptSerializer()

            With Me.JsLinks
                .Add("Edit.js")
                .Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
                .Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
                .Add(WebUtils.GetVirtualPath("Library/JavaScript/date.js"))
                .Add(WebUtils.GetVirtualPath("Library/JavaScript/Utils.js"))
                .Add(WebUtils.GetVirtualPath("Library/JavaScript/ToleranceCalculator.js"))
            End With

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProviderInvoiceStyle))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            ' add enums required for payment tolerance functionality
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.ToleranceType))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.ToleranceCombinationMethod))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.PaymentToleranceGroupSystemTypes))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomContractedExceeded))

            With css
                .Append("td.detailCell {vertical-align:top;} ")
                .Append("td.headerGroup {text-align:center;border-width:0px;} ")
                .Append("td.plannedCell {background-color: #DFEAED !important;}")
                .Append("td.otherInvoiceCell {background-color:#fffea6;} ")
                .Append("th.header {padding-bottom:0px;} ")
                .Append("th.right, td.right {text-align:right;} ")
                Me.AddExtraCssStyle(.ToString())
            End With

            sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
            _canEditLineCost = Not sysInfo.DPIPreventEditOfCosts
            _canEditSuspendedInvoices = (Utils.ToInt32(sysInfo.DPISuspensionCommentID_Edit) > 0)
            currentUser = SecurityBL.GetCurrentUser()

            _estabID = Utils.ToInt32(Request.QueryString("estabID"))
            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _clientID = Utils.ToInt32(Request.QueryString("clientID"))
            _InvoiceID = Utils.ToInt32(Request.QueryString("id"))


            If _invoiceID > 0 Then
                InvoiceID = _InvoiceID
            End If

            If _estabID > 0 Then
                ' if we have a provider ensure that is a valid dom provider

                ' fetch back the establishment record and return error if encountered
                msg = establishment.Fetch(_estabID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If establishment.DomCareProvider = TriState.False _
                    AndAlso establishment.DayCentreProvider = TriState.False Then
                    ' if the establishment record is not a dom care provider or day centre provider reset provider id

                    _estabID = 0

                End If

            End If

            _dpi = New DomProviderInvoiceBL( _
                ConnectionStrings("Abacus").ConnectionString, _
                sysInfo.LicenceNo, _
                currentUser.ExternalUsername, _
                currentUser.ExternalUserID, _
                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings) _
            )

            If Not Me.IsPostBack Then
                ClearNewDpiData()
            End If

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.AddNew"))
                .ShowNew = False
                .AllowBack = Not IsPopupWindow
                .AllowFind = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.Edit"))
                .AllowDelete = False
                .EditableControls.Add(tabHeader.Controls)
                .EditableControls.Add(tabDetails.Controls)
                .EditableControls.Add(tabSummary.Controls)
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
            End With

            chkSuspend.CheckBox.Attributes.Add("onclick", "chkSuspend_Click();")

            With txtNotes.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 3
            End With

            cboAddDetailRateCategory.DropDownList.Attributes.Add("onchange", "cboAddDetailRateCategory_Change();")

            chkSummaryVat.CheckBox.Attributes.Add("onclick", "chkSummaryVat_Click();")

            ' fetch
            msg = PrimeDpiClass()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' re-create the list of details (from view state)
            detailList = GetDetailUniqueIDsFromViewState()
            For Each id As String In detailList
                OutputDetailControls(id, New DetailLine(Me.DbConnection))
            Next

            msg = DomProviderInvoiceBL.ShouldPreventEntryOfActualServiceForFuturePeriods(DbConnection, _
                                                                                        _preventEntryOfFutureDates)

            If msg.Success Then

                If _preventEntryOfFutureDates Then

                    SetUpPreventEntryOfFutureDatesRangeValidator(rvWeekEndingFrom, txtWeekEndingFrom)
                    SetUpPreventEntryOfFutureDatesRangeValidator(rvWeekEndingTo, txtWeekEndingTo)
                    SetUpPreventEntryOfFutureDatesRangeValidator(rvInvoiceDate, txtInvoiceDate)
                    SetUpPreventEntryOfFutureDatesRangeValidator(rvAddDetailWeekEnding, dteAddDetailWeekEnding)

                Else

                    rvWeekEndingFrom.Visible = False
                    rvWeekEndingTo.Visible = False
                    rvInvoiceDate.Visible = False
                    rvAddDetailWeekEnding.Visible = False

                End If

            Else

                WebUtils.DisplayError(msg)

            End If

            UseJQuery = True
            UseJqueryUI = True
            UseJqueryTextboxClearer = True

            'populate the javascript arrays to use client side
            If _invoiceID > 0 Then
                ' if we are working with an existing invoice
                With jsSerializer
                    If Not _dpi.PaymentTolerances Is Nothing AndAlso _dpi.PaymentTolerances.Count > 0 Then
                        _jsoPaymentTolerances = .Serialize(_dpi.PaymentTolerances.ToArray)
                    End If
                    If Not _dpi.PaymentToleranceGroups Is Nothing AndAlso _dpi.PaymentToleranceGroups.Count > 0 Then
                        _jsoPaymentToleranceGroups = .Serialize(_dpi.PaymentToleranceGroups.ToArray)
                    End If
                End With
            End If

            IncludeJavascript()
        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_DATA_DETAILS)
            ViewState.Remove(VIEWSTATE_KEY_COUNTER_NEW_DETAILS)
            phDetailsSummaryVisitLevel.Controls.Clear()
        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            ' clear new DPI data from session
            ClearNewDpiData()

            msg = PopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            e.ButtonsControl.ShowSave = False

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            With _dpi
                ' ensure not legacy invoice
                If .InvoiceStyle = DomProviderInvoiceStyle.Legacy Then WebUtils.DisplayNotFound()

                ClearViewState()

                msg = PopulateScreen()
                If Not msg.Success Then WebUtils.DisplayError(msg)

            End With

        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_DATA_DETAILS_REMOVED)
            If e.ItemID = 0 Then
                ' item hasn't been created yet so redirect to previous page
                Dim backUrl As String = Request.QueryString("backUrl")
                If String.IsNullOrEmpty(backUrl) = False _
                    AndAlso backUrl.Trim().Length > 0 Then
                    ' if we have a back url specified then redirect to it
                    ' as we don't need to see this page
                    ClearNewDpiData()
                    Response.Redirect(backUrl)
                Else
                    NewClicked(e)
                End If
            Else
                ' is an existing item
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim line As DetailLine, tempLine As DetailLine = Nothing
            Dim detailLines As List(Of DetailLine) = Nothing
            Dim invoiceID As Integer
            Dim originalSuspensionReasonID As Integer
            Dim showSuspensionReasonChanged As Boolean

            msg = RepopulateScreen(True)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'Validate these date values as thay cant be in the future.
            If txtInvoiceDate.Value > Date.Today Then
                lblError.Text = "The Invoice date can not be in the future."
                e.Cancel = True
                Exit Sub
            End If

            If txtDateReceived.Value > Date.Today Then
                lblError.Text = "The date recieved can not be in the future."
                e.Cancel = True
            End If

            Me.Validate("Save")

            If Me.IsValid() Then

                With _dpi
                    ' remove all detail lines from the invoice
                    msg = .RemoveAllDetails()
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' add each detail line from the screen
                    msg = GetDetailLinesFromScreen(.DomContractID, detailLines)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    For Each line In detailLines
                        msg = .AddDetail(line.WeekEnding, line.RateCategoryID, line.ThisInvoiceRate, tempLine)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        ' tempLine is merely used to get the Identifier back so we can then update the line
                        msg = .UpdateDetail(tempLine.Identifier, line.ThisInvoiceRate, line.ThisInvoiceUnits, line.ThisInvoiceSUUnits, line.ThisInvoiceCost, line)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    Next

                    ' save the invoice
                    invoiceID = _dpi.ID

                    ' we can only save if we have some detail lines
                    If _dpi.DetailLines.Count > 0 Then
                        '++ Add the summary VAT from the screen to the DPI to be saved..
                        If txtSummaryVat.Text <> "" Then
                            .VAT = Convert.ToDecimal(txtSummaryVat.Text)
                        Else
                            .VAT = 0
                        End If

                        originalSuspensionReasonID = cboSuspensionReason.GetPostBackValue()
                        msg = .Save(Utils.ToInt32(originalSuspensionReasonID), invoiceID)

                        If msg.Success Then

                            ' if suspension reason has changed from what we think it should be
                            If .SuspensionReasonID <> originalSuspensionReasonID Then
                                showSuspensionReasonChanged = True
                            End If

                            ClearNewDpiData()

                            Response.Redirect( _
                                String.Format("Edit.aspx?mode=1&id={0}&estabID={1}&showSusWarn={2}&backUrl={3}", _
                                              invoiceID, _
                                              .ProviderID, _
                                              IIf(showSuspensionReasonChanged, showSuspensionReasonChanged.ToString(), String.Empty), _
                                              HttpUtility.UrlEncode(Request.QueryString("backUrl"))))

                        Else

                            lblError.Text = msg.Message
                            e.Cancel = True
                        End If

                    Else

                        lblError.Text = "Cannot save invoice until at least one detail line has been entered."
                        tabStrip.TabIndex = 2
                        e.Cancel = True

                    End If

                End With

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " PopulateScreen "

        Private Function PopulateScreen() As ErrorMessage

            Dim msg As ErrorMessage
            Dim newDpi As NewDomProviderInvoiceData
            Dim domContractID As Integer
            Dim showSusWarn As String
            Dim firstShow As Boolean
            Dim invoiceDateFrom, invoiceDateTo As DateTime
            Dim isBlockPeriodicContract As Boolean = False

            showSusWarn = Request.QueryString("showSusWarn")

            msg = DomProviderInvoiceBL.GetNaturalPeriodOfInvoices(DbConnection, invoiceDateFrom, invoiceDateTo)
            If Not msg.Success Then Return msg

            ' header tab
            If _dpi.ID > 0 Then
                With _dpi
                    ' existing DPI
                    If Not showSusWarn Is Nothing AndAlso showSusWarn.Trim().Length > 0 Then
                        lblSusWarning.Text = String.Format("The suspension reason of this invoice has been automatically set to '{0}'.<br /><br />", .SuspensionReasonDescription)
                        lblSusWarning.Visible = True
                    End If
                    SetupProviderSelector(.ProviderID)
                    SetupContractSelector(.DomContractID, .ProviderID)
                    domContractID = .DomContractID
                    '' check if periodic block contract
                    msg = PeriodicBlock(domContractID, isBlockPeriodicContract)
                    If Not msg.Success Then Return msg
                    SetupClientSelector(.ClientID)

                    '' if it is block periodic contract then we might need to fix something
                    If isBlockPeriodicContract Then
                        txtWeekEndingFrom.Text = .ServicedateFrom
                        txtWeekEndingTo.Text = .ServiceDateTo
                    Else
                        txtWeekEndingFrom.Text = DomContractBL.GetWeekCommencingDate(Me.DbConnection, Nothing, .WeekEndingFrom)
                        txtWeekEndingTo.Text = .WeekEndingTo
                    End If
                    
                    txtInvoiceNumber.Text = .InvoiceNumber
                    txtStatus.Text = .StatusDescription
                    txtReference.Text = .Reference
                    If Utils.IsDate(.InvoiceDate) Then
                        txtInvoiceDate.Text = Convert.ToDateTime(.InvoiceDate).ToShortDateString
                    Else
                        txtInvoiceDate.Text = String.Empty
                    End If
                    If Utils.IsDate(.InvoiceDate) Then
                        txtDateReceived.Text = Convert.ToDateTime(.DateReceived).ToShortDateString
                    Else
                        txtDateReceived.Text = String.Empty
                    End If

                    chkSuspend.CheckBox.Checked = (.SuspensionReasonID <> 0)
                    msg = PopulateSuspensionReasons(.SuspensionReasonID)
                    If Not msg.Success Then Return msg
                    txtNotes.Text = .Notes

                End With
            Else

                Dim isNewDpi As Boolean = False

                If HaveNewDpiData() Then
                    ' part-way through creating new DPI
                    newDpi = FetchNewDpiData()
                    With newDpi
                        SetupProviderSelector(.ProviderID)
                        SetupContractSelector(.DomContractID, .ProviderID)
                        domContractID = .DomContractID
                        SetupClientSelector(.ClientID)
                    End With
                Else
                    ' empty screen, ready for a new DPI
                    newDpi = New NewDomProviderInvoiceData()
                    SetupProviderSelector(_estabID)
                    SetupContractSelector(_contractID, _estabID)
                    SetupClientSelector(_clientID)
                    isNewDpi = True
                End If

                With newDpi
                    If isNewDpi Then

                        If Utils.IsDate(invoiceDateFrom) Then
                            txtWeekEndingFrom.Text = invoiceDateFrom
                        Else
                            txtWeekEndingFrom.Text = String.Empty
                        End If

                        If Utils.IsDate(invoiceDateTo) Then
                            txtWeekEndingTo.Text = invoiceDateTo
                        Else
                            txtWeekEndingTo.Text = String.Empty
                        End If

                    Else

                        If Utils.IsDate(.WeekEndingFrom) Then
                            txtWeekEndingFrom.Text = DomContractBL.GetWeekCommencingDate(Me.DbConnection, Nothing, .WeekEndingFrom)
                        Else
                            txtWeekEndingFrom.Text = String.Empty
                        End If

                        If Utils.IsDate(.WeekEndingTo) Then
                            txtWeekEndingTo.Text = .WeekEndingTo
                        Else
                            txtWeekEndingTo.Text = String.Empty
                        End If

                    End If

                    txtReference.Text = .Reference
                    If Utils.IsDate(.InvoiceDate) Then
                        txtInvoiceDate.Text = .InvoiceDate
                    Else
                        txtInvoiceDate.Text = String.Empty
                    End If
                    If Utils.IsDate(.DateReceived) Then
                        txtDateReceived.Text = .DateReceived
                    Else
                        txtDateReceived.Text = String.Empty
                    End If
                    If .SuspensionReasonID > 0 Then chkSuspend.CheckBox.Checked = True
                    msg = PopulateSuspensionReasons(.SuspensionReasonID)
                    If Not msg.Success Then Return msg
                    txtNotes.Text = .Notes

                    

                End With
            End If

            'Start D12229 
            If txtWeekEndingFrom.Text <> String.Empty Then
                txtWeekEndingTo.MinimumValue = DateTime.Parse(txtWeekEndingFrom.Text).ToString("dd/MM/yyyy")
            End If
            'End D12229

            If invoiceDateFrom <> DateTime.MinValue AndAlso invoiceDateTo <> DateTime.MinValue Then
                ' if we have invoices dates then we can calculate invoice date to date
                ' upon change of invoice date from field

                txtWeekEndingFrom.TextBox.Attributes.Add("onchange", _
                                                         "javascript:txtWeekEndingFrom_Changed();")

                _naturalPeriodOfInvoicesScript = String.Format("NaturalPeriodOfProviderInvoiceAdditionalDays={0};", _
                                                               invoiceDateTo.Subtract(invoiceDateFrom).Days)

            End If

            PopulateAuditDetails()

            ' detail tab
            firstShow = Not (_dpi.ID > 0)
            msg = PopulateDetailLines(domContractID, _dpi.DetailLines, firstShow)
            If Not msg.Success Then Return msg

            ' summary tab
            msg = PopulateSummary()
            If Not msg.Success Then Return msg

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " PopulateSummary "

        Private Function PopulateSummary() As ErrorMessage

            Dim msg As ErrorMessage
            Dim invoiceStyle As DomProviderInvoiceStyle = DomProviderInvoiceStyle.SummaryLevel
            Dim applicationSettings As New ApplicationSettingCollection()

            If _dpi.ID > 0 Then invoiceStyle = _dpi.InvoiceStyle

            pnlSummarySummaryLevel.Visible = False
            pnlSummaryVisitLevel.Visible = False
            pnlSummaryManualPayment.Visible = False

            Select Case invoiceStyle
                Case DomProviderInvoiceStyle.SummaryLevel
                    pnlSummarySummaryLevel.Visible = True
                    txtSummaryActualCost.Text = _dpi.ActualCost.ToString("F2")
                    chkSummaryVat.CheckBox.Checked = _dpi.VATOnInvoice
                    txtSummaryVat.Text = _dpi.VAT.ToString("F2")
                    txtSummaryPenalty.Text = _dpi.PenaltyPayment.ToString("F2")
                    txtSummaryClientContrib.Text = _dpi.ClientContribution.ToString("F2")
                    txtSummaryNetCost.Text = _dpi.NetCost.ToString("F2")
                    txtSummaryInvoiceTotal.Text = _dpi.InvoiceTotal.ToString("F2")

                Case DomProviderInvoiceStyle.VisitLevel
                    pnlSummaryVisitLevel.Visible = True
                    txtVisitActualCost.Text = _dpi.ActualCost.ToString("F2")
                    txtVisitVat.Text = _dpi.VAT.ToString("F2")
                    txtVisitClientContrib.Text = _dpi.ClientContribution.ToString("F2")
                    txtVisitNetCost.Text = _dpi.NetCost.ToString("F2")
                    txtVisitInvoiceTotal.Text = _dpi.InvoiceTotal.ToString("F2")

                Case DomProviderInvoiceStyle.ManualPayment
                    pnlSummaryManualPayment.Visible = True
                    txtManualNetCost.Text = _dpi.NetCost.ToString("F2")
                    txtManualVat.Text = _dpi.VAT.ToString("F2")
                    txtManualInvoiceTotal.Text = _dpi.InvoiceTotal.ToString("F2")

                Case DomProviderInvoiceStyle.Attendance
                    pnlSummaryVisitLevel.Visible = True
                    txtVisitActualCost.Text = _dpi.ActualCost.ToString("F2")
                    txtVisitVat.Text = _dpi.VAT.ToString("F2")
                    txtVisitClientContrib.Text = _dpi.ClientContribution.ToString("F2")
                    txtVisitNetCost.Text = _dpi.NetCost.ToString("F2")
                    txtVisitInvoiceTotal.Text = _dpi.InvoiceTotal.ToString("F2")

                Case DomProviderInvoiceStyle.SummaryLevelExtranet
                    pnlSummaryVisitLevel.Visible = True
                    txtVisitActualCost.Text = _dpi.ActualCost.ToString("F2")
                    txtVisitVat.Text = _dpi.VAT.ToString("F2")
                    txtVisitClientContrib.Text = _dpi.ClientContribution.ToString("F2")
                    txtVisitNetCost.Text = _dpi.NetCost.ToString("F2")
                    txtVisitInvoiceTotal.Text = _dpi.InvoiceTotal.ToString("F2")
            End Select

            ' fetch PreventEntryPenaltyPayments setting to see if we should display penalty payments
            msg = ApplicationSetting.FetchList(conn:=Me.DbConnection, list:=applicationSettings, applicationID:=Me.Settings.CurrentApplicationID, auditLogTitle:=String.Empty, auditUserName:=String.Empty, settingKey:="PreventEntryPenaltyPayments")

            If msg.Success Then

                If applicationSettings.Count > 0 Then

                    Dim preventPaymentEntrySetting As ApplicationSetting = applicationSettings(0)

                    If String.Compare(preventPaymentEntrySetting.SettingValue, "True", True) = 0 Then

                        divSummaryPenalty.Style.Add("display", "none")

                    End If

                End If

                msg = New ErrorMessage()
                msg.Success = True

            End If

            Return msg

        End Function

#End Region

#Region " PopulateDetailLines "

        Private Function PopulateDetailLines(ByVal domContractID As Integer, ByVal detailLines As List(Of DetailLine), Optional ByVal firstShow As Boolean = False) As ErrorMessage

            Dim msg As ErrorMessage
            Dim row As TableRow
            Dim cell As TableCell
            Dim invoiceStyle As DomProviderInvoiceStyle = DomProviderInvoiceStyle.SummaryLevel
            Dim detailList As List(Of String)
            Dim id As String

            If _dpi.ID > 0 Then invoiceStyle = _dpi.InvoiceStyle

            If invoiceStyle = DomProviderInvoiceStyle.ManualPayment Then
                pnlDetailsSummaryVisitLevel.Visible = False
                pnlDetailsSummaryLevelExtranet.Visible = False

                For Each dl As DetailLine In detailLines
                    With dl

                        row = New TableRow()
                        phDetailsManualPayment.Controls.Add(row)

                        ' week ending
                        cell = New TableCell()
                        row.Cells.Add(cell)
                        cell.Text = .WeekEnding.ToString(DateAndTimeFormat)

                        ' description
                        cell = New TableCell()
                        row.Cells.Add(cell)
                        cell.Text = .Comment

                        ' cost
                        cell = New TableCell()
                        row.Cells.Add(cell)
                        cell.CssClass = "right"
                        cell.Text = .ThisInvoiceCost.ToString("F2")

                    End With
                Next
            ElseIf invoiceStyle = DomProviderInvoiceStyle.SummaryLevelExtranet Then
                pnlDetailsManualPayment.Visible = False
                pnlDetailsSummaryVisitLevel.Visible = False

                For detailIndex As Integer = 0 To detailLines.Count - 1
                    OutputDetailControlsSummaryLevelExtranet(detailIndex.ToString(), detailLines(detailIndex), _dpi.ID)
                Next

            Else
                pnlDetailsManualPayment.Visible = False
                pnlDetailsSummaryLevelExtranet.Visible = False

                detailList = GetDetailUniqueIDsFromViewState()
                _detailLineStartupJS.Length = 0
                For Each dl As DetailLine In detailLines
                    If _dpi.ID <= 0 AndAlso firstShow Then
                        dl.ThisInvoiceSUUnits = dl.ThisInvoiceUnits
                    End If
                    id = GetDetailUniqueID(dl)
                    OutputDetailControls(id, dl)
                    detailList.Add(id)
                Next
                PersistDetailUniqueIDsToViewState(detailList)

                msg = LoadRateCategoryDropdown(cboAddDetailRateCategory, domContractID)
                If Not msg.Success Then Return msg

            End If

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " PopulateAuditDetails "

        Private Sub PopulateAuditDetails()

            If _dpi.ID > 0 Then
                With CType(auditDetails, IBasicAuditDetails)
                    .Collapsed = True
                    .EnteredBy = _dpi.EnteredBy
                    .DateEntered = _dpi.DateEntered
                    .LastAmendedBy = _dpi.LastAmendedBy
                    If Utils.IsDate(_dpi.DateLastAmended) Then .DateLastAmended = _dpi.DateLastAmended
                End With
            Else
                _btnAuditDetails.Visible = False
                auditDetails.Visible = False
            End If

        End Sub

#End Region

#Region " PopulateSuspensionReasons "

        Private Function PopulateSuspensionReasons(ByVal reasonID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim reasons As vwSuspensionCommentCollection = Nothing
            Dim reason As vwSuspensionComment = Nothing
            Dim isAuto As Boolean

            msg = DomProviderInvoiceBL.FetchSuspensionReasons(Me.DbConnection, _
                                                              DomProviderInvoiceSuspensionReasonType.Suspend, _
                                                              DomProviderInvoiceSuspensionReasonAutoType.Manual, _
                                                              reasons)
            If Not msg.Success Then Return msg

            With cboSuspensionReason.DropDownList
                .Items.Clear()
                .DataSource = reasons
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)


                ' add auto reason if specified by reasonID parameter
                If reasonID > 0 Then
                    ' find the reason
                    msg = DomProviderInvoiceBL.FindSuspensionReason(Me.DbConnection, reasonID, reason)
                    If Not msg.Success Then Return msg

                    ' is the specified reason an auto reason?
                    isAuto = DomProviderInvoiceBL.SuspensionReasonIsAuto(reason)
                    If isAuto Then
                        .Items.Add(New ListItem(reason.Description, reason.ID))
                        _isAutoSuspensionReason = True
                    End If

                    ' select specified reason
                    .SelectedValue = reasonID.ToString()
                Else
                    ' else check to see whether a the establishment has an auto suspension reason
                    If _dpi.ProviderID > 0 Then
                        ' if we have a provider id to work with and the invoice dates are in the future 

                        Dim estab As New Establishment(DbConnection)

                        msg = estab.Fetch(_dpi.ProviderID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        If estab.DomSuspensionCommentID > 0 AndAlso estab.DomSuspendFutureInvoices = TriState.True Then
                            ' if we have a commend id and we should suspend future invoices

                            Dim suspensionReasonItem As ListItem

                            ' see if the item exists in the list already
                            suspensionReasonItem = cboSuspensionReason.DropDownList.Items.FindByValue(estab.DomSuspensionCommentID)

                            If IsNothing(suspensionReasonItem) Then
                                ' if the item isn't in the list then create it

                                Dim lookup As New Lookup(DbConnection)

                                msg = lookup.Fetch(estab.DomSuspensionCommentID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                                suspensionReasonItem = New ListItem(lookup.Description, lookup.ID)
                                cboSuspensionReason.DropDownList.Items.Add(suspensionReasonItem)

                            End If

                            cboSuspensionReason.DropDownList.SelectedValue = estab.DomSuspensionCommentID
                            _isAutoSuspensionReason = True
                            suspensionReasonItem.Selected = True
                            chkSuspend.CheckBox.Checked = True

                        End If

                    End If

                End If

            End With

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " RepopulateScreen "

        Private Function RepopulateScreen(ByVal edit As Boolean) As ErrorMessage

            Dim msg As ErrorMessage
            Dim providerID As Integer, domContractID As Integer, clientID As Integer, suspensionReasonID As Integer
            Dim newDpiData As NewDomProviderInvoiceData
            Dim detailLines As List(Of DetailLine) = Nothing

            ' re-populate suspension reasons
            msg = PopulateSuspensionReasons(Utils.ToInt32(_dpi.SuspensionReasonID))
            If Not msg.Success Then Return msg
            cboSuspensionReason.SelectPostBackValue()
            suspensionReasonID = Utils.ToInt32(cboSuspensionReason.GetPostBackValue())
            cboSuspensionReason.RequiredValidator.Enabled = chkSuspend.CheckBox.Checked

            If _invoiceID > 0 Then
                ' editing an existing invoice

                With _dpi
                    ' re-fetch the invoice
                    msg = PrimeDpiClass()
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    If edit Then
                        msg = .Edit()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If

                    providerID = .ProviderID
                    domContractID = .DomContractID
                    clientID = .ClientID
                    txtWeekEndingFrom.Text = DomContractBL.GetWeekCommencingDate(Me.DbConnection, Nothing, .WeekEndingFrom)
                    txtWeekEndingTo.Text = .WeekEndingTo
                End With

            Else
                ' creating a new invoice

                ' overwrite header changes made on screen
                newDpiData = FetchNewDpiData()
                With newDpiData
                    providerID = .ProviderID
                    domContractID = .DomContractID
                    clientID = .ClientID
                    txtWeekEndingFrom.Text = DomContractBL.GetWeekCommencingDate(Me.DbConnection, Nothing, .WeekEndingFrom)
                    txtWeekEndingTo.Text = .WeekEndingTo
                End With
                newDpiData = StoreNewDpiData(newDpiData, providerID, domContractID, clientID, suspensionReasonID)

                msg = PrimeDpiClass()
                If Not msg.Success Then Return msg

            End If

            ' re-populate in-place selectors
            SetupProviderSelector(providerID)
            SetupContractSelector(domContractID, providerID)
            SetupClientSelector(clientID)

            ' re-populate rate category popup dropdown
            msg = LoadRateCategoryDropdown(cboAddDetailRateCategory, domContractID)
            If Not msg.Success Then Return msg
            cboAddDetailRateCategory.SelectPostBackValue()

            PopulateAuditDetails()

            ' overwrite header changes made on screen
            _dpi.Reference = txtReference.Text
            If _dpi.InvoiceDate <> txtInvoiceDate.Text Then
                If txtInvoiceDate.Value > Date.Today Then
                    lblError.Text = "The Invoice date can not be in the future."
                Else
                    msg = _dpi.SetInvoiceDate(txtInvoiceDate.Text)
                    If Not msg.Success Then Return msg
                End If
            End If
            If _dpi.DateReceived <> txtDateReceived.Text Then
                If txtDateReceived.Value > Date.Today Then
                    lblError.Text = "The date recieved can not be in the future."
                Else
                    msg = _dpi.SetDateReceived(txtDateReceived.Text)
                    If Not msg.Success Then Return msg
                End If

            End If
            _dpi.Notes = txtNotes.Text
            _dpi.Reference = txtReference.Value
            ' grab existing detail lines with those from the screen
            msg = GetDetailLinesFromScreen(domContractID, detailLines)
            If Not msg.Success Then Return msg

            ' re-populate detail lines
            ClearViewState()
            msg = PopulateDetailLines(domContractID, detailLines)
            If Not msg.Success Then Return msg

            ' overwrite summary changes made on screen
            With _dpi
                .VAT = Utils.ToDecimal(txtSummaryVat.Text)
                .PenaltyPayment = Utils.ToDecimal(txtSummaryPenalty.Text)
                .ClientContribution = Utils.ToDecimal(txtSummaryClientContrib.Text)
                txtInvoiceNumber.Text = _dpi.InvoiceNumber
                txtStatus.Text = _dpi.StatusDescription
            End With

            msg = PopulateSummary()
            If Not msg.Success Then Return msg

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " GetDetailLinesFromScreen "

        Private Function GetDetailLinesFromScreen(ByVal domContractID As Integer, ByRef detailLines As List(Of DetailLine)) As ErrorMessage

            Dim msg As ErrorMessage
            Dim id As String
            Dim detailList As List(Of String)
            Dim weekEnding As Date
            Dim weekEndingString, measuredIn As String
            Dim rateCategory As DomRateCategory
            Dim rateCategoryID As Integer
            Dim rateCategoryDesc As String
            Dim plannedUnits As Decimal, plannedRate As Decimal, plannedCost As Decimal
            Dim otherUnits As Decimal, otherCost As Decimal
            Dim units As Decimal, unitsSU As Decimal, unitCost As Decimal, lineCost As Decimal
            Dim line As DetailLine = Nothing
            Dim paymentToleranceID As Integer
            Dim paymentToleranceType As ToleranceType

            detailList = GetDetailUniqueIDsFromViewState()
            detailLines = New List(Of DetailLine)(detailList.Count)

            msg = FetchRateCategories(domContractID)
            If Not msg.Success Then Return msg

            For Each id In detailList

                weekEndingString = CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_WE & id), HiddenField).Value
                If Utils.IsDate(weekEndingString) Then weekEnding = CDate(weekEndingString)

                rateCategoryID = Utils.ToInt32(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_RATE_CATEGORY & id), HiddenField).Value)
                rateCategory = DomContractBL.FindRateCategory(_rateCategories, rateCategoryID)
                rateCategoryDesc = rateCategory.Description

                'prime the toleranceid
                paymentToleranceID = Utils.ToInt32(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_TOLERANCE_ID & id), HiddenField).Value)

                'prime the tolerancetype
                paymentToleranceType = Utils.ToInt32(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_TOLERANCE_TYPE & id), HiddenField).Value)

                ' planned
                plannedUnits = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_PLANNED_UNITS & id), HiddenField).Value)
                measuredIn = Utils.ToString(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_PLANNED_UNITS_MEASURED_IN & id), HiddenField).Value)
                plannedRate = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_PLANNED_RATE & id), HiddenField).Value)
                plannedCost = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_PLANNED_COST & id), HiddenField).Value)

                ' other
                otherUnits = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_OTHER_UNITS & id), HiddenField).Value)
                otherCost = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_OTHER_COST & id), HiddenField).Value)

                ' this
                units = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_THIS_UNITS & id), TextBoxEx).Text)
                unitsSU = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_THIS_SU_UNITS & id), TextBoxEx).Text)
                unitCost = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_THIS_RATE & id), HiddenField).Value)
                If _canEditLineCost Then
                    lineCost = Utils.ToDecimal(CType(phDetailsSummaryVisitLevel.FindControl(CTRL_PREFIX_DETAIL_THIS_COST & id), TextBoxEx).Text)
                Else
                    lineCost = (units * unitCost)
                End If

                ' populate a DetailLine instance
                line = New DetailLine(Me.DbConnection)
                With line
                    .WeekEnding = weekEnding
                    .RateCategoryID = rateCategoryID
                    .RateCategoryDescription = rateCategoryDesc
                    .PlannedUnits = plannedUnits
                    .PlannedRate = plannedRate
                    .PlannedCost = plannedCost
                    .OtherInvoiceUnits = otherUnits
                    .OtherInvoiceCost = otherCost
                    .ThisInvoiceUnits = units
                    .ThisInvoiceSUUnits = unitsSU
                    .ThisInvoiceRate = unitCost
                    .ThisInvoiceCost = lineCost
                    .ToleranceID = paymentToleranceID
                    .ToleranceType = paymentToleranceType
                    .MeasuredIn = measuredIn
                End With

                detailLines.Add(line)

            Next

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " Set Non Delivery Column Visibility "

        ''' <summary>
        ''' the ‘Non-Delivery’ column will not be displayed if the Framework Type is ‘Community General’ 
        ''' and none of the Contract Periods (of the Contract to which the Provider Invoice relates) 
        ''' have a Service Outcome Group or a Visit Code Group recorded against them
        ''' </summary>
        ''' <param name="_dpi">object of DomProviderInvoiceBL</param>
        ''' <remarks></remarks>
        Public Sub SetNondeliveryColumnVisibility(ByVal _dpi As DomProviderInvoiceBL)
            If _dpi.FrameWorkType = FrameworkTypes.CommunityGeneral Then
                Dim dcPeriodList As DataClasses.Collections.DomContractPeriodCollection = _
                    New DataClasses.Collections.DomContractPeriodCollection()
                Dim msg As New ErrorMessage
                msg = DataClasses.DomContractPeriod.FetchList(conn:=Me.DbConnection, _
                                                        list:=dcPeriodList, _
                                                        auditUserName:=String.Empty, _
                                                        auditLogTitle:=String.Empty, _
                                                        domContractID:=_dpi.DomContractID)

                If Not msg.Success Then WebUtils.DisplayError(msg)
                Dim hasServiceOutcomeGroupOrVisitCodeGroup As Boolean = False


                For Each dcPeriod As DomContractPeriod In dcPeriodList
                    If dcPeriod.VisitCodeGroupID > 0 Or dcPeriod.ServiceOutcomeGroupID > 0 Then
                        hasServiceOutcomeGroupOrVisitCodeGroup = True
                    End If
                Next

                ShowNonDelivery = hasServiceOutcomeGroupOrVisitCodeGroup

            End If

        End Sub

#End Region

#Region " Output Deatil Controls For Summary Level Extranet "

        Private Sub OutputDetailControlsSummaryLevelExtranet(ByVal uniqueID As String, _
                                                             ByVal detail As DetailLine, _
                                                             ByVal invoiceId As Integer)

            Dim inv As DataClasses.DomProviderInvoice = New DataClasses.DomProviderInvoice(Me.DbConnection)
            Dim msg As New ErrorMessage
            msg = inv.Fetch(invoiceId)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Dim row As TableRow
            Dim cell As TableCell
            'Dim txt As TextBoxEx

            row = New TableRow()
            row.ID = uniqueID
            phDetailsSummaryLevelExtranet.Controls.Add(row)

            ' week ending
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell"
            cell.Controls.Add(CreateSpan(detail.WeekEnding.ToString("dd/MM/yyyy")))

            ' rate category
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell"
            cell.Controls.Add(CreateSpan(detail.RateCategoryDescription))

            ' planned units 
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell left"
            cell.Controls.Add(CreateSpan(detail.PlannedUnits.ToString("F2")))
            cell.Controls.Add(CreateSpan(" "))
            cell.Controls.Add(CreateSpan(detail.MeasuredIn))

            ' planned rate
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell right"
            cell.Controls.Add(CreateSpan(detail.PlannedRate.ToString("F2")))

            ' planned cost
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell right"
            cell.Controls.Add(CreateSpan(detail.PlannedCost.ToString("F2")))

            ' this invoice delivered
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            cell.Controls.Add(CreateSpan(detail.DeliveredUnits.ToString("F2")))

            If ShowNonDelivery Then
                Dim numberOfNonDeliveryMsg As String = String.Format("{0}{1}", detail.NonDeliveryUnitsPaid.ToString("F2"), IIf(detail.NumberOfNonDeliveryItems > 0, String.Format(" ({0})", detail.NumberOfNonDeliveryItems), ""))
                ' this invoice Nondelivery
                cell = New TableCell()
                cell.ID = "cell_" & detail.ID
                row.Cells.Add(cell)
                cell.CssClass = "detailCell right"
                ''
                If detail.NonDeliveryDataPresent Then
                    Dim hlink As New HyperLink
                    hlink.ID = "hlink_" & detail.ID
                    hlink.Text = numberOfNonDeliveryMsg
                    hlink.Attributes.Add("onclick", _
                                         String.Format("javascript:ShowDeliverydetail('{0}','{1}','{2}',{3},{4},'{5}');", _
                                                       inv.PaymentScheduleID, _
                                                       invoiceId, _
                                                       detail.ID, _
                                                       detail.PlannedUnits, _
                                                       detail.PlannedRate, _
                                                       detail.TimeBased.ToString().ToLower()))
                    _controlsToenable.Add(hlink.ID)
                    cell.Controls.Add(hlink)
                Else
                    cell.Controls.Add(CreateSpan(numberOfNonDeliveryMsg))
                End If

            End If

            ' this invoice rate
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            cell.Controls.Add(CreateSpan(detail.ThisInvoiceRate.ToString("F2")))

            ' this invoice cost
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            cell.Controls.Add(CreateSpan(detail.ThisInvoiceCost.ToString("F2")))

        End Sub

#End Region

#Region " OutputDetailControls "

        Private Sub OutputDetailControls(ByVal uniqueID As String, ByVal detail As DetailLine)

            OutputDetailControls(uniqueID, detail, Nothing)

        End Sub

        Private Sub OutputDetailControls(ByVal uniqueID As String, ByVal detail As DetailLine, ByVal rowIndex As Nullable(Of Integer))


            Dim row As TableRow
            Dim cell As TableCell
            Dim txt As TextBoxEx
            Dim btnRemove As HtmlInputImage
            Dim isNew As Boolean

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_DETAIL) Then

                isNew = uniqueID.StartsWith(UNIQUEID_PREFIX_NEW_DETAIL)

                row = New TableRow()
                row.ID = uniqueID

                If Not rowIndex.HasValue Then
                    ' if we have no row index then add the row at the end 
                    phDetailsSummaryVisitLevel.Controls.Add(row)
                Else
                    ' else we have a row index so insert at that index
                    phDetailsSummaryVisitLevel.Controls.AddAt(rowIndex.Value, row)
                End If

                ' week ending
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell"
                cell.Controls.Add(CreateSpan(detail.WeekEnding.ToString(DateAndTimeFormat)))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_WE & uniqueID, detail.WeekEnding.ToString(DateAndTimeFormat)))

                ' rate category
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell"
                cell.Controls.Add(CreateSpan(detail.RateCategoryDescription))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_RATE_CATEGORY & uniqueID, detail.RateCategoryID))
                cell.Controls.Add(CreateHiddenField(CTRL_TOLERANCE_ID & uniqueID, detail.ToleranceID))
                cell.Controls.Add(CreateHiddenField(CTRL_TOLERANCE_TYPE & uniqueID, detail.ToleranceType))

                ' planned units 
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell plannedCell left"
                cell.Controls.Add(CreateSpan(detail.PlannedUnits.ToString("F2")))
                cell.Controls.Add(CreateSpan(" "))
                cell.Controls.Add(CreateSpan(detail.MeasuredIn))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_PLANNED_UNITS & uniqueID, detail.PlannedUnits.ToString("F2")))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_PLANNED_UNITS_MEASURED_IN & uniqueID, detail.MeasuredIn))

                ' planned rate
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell plannedCell right"
                cell.Controls.Add(CreateSpan(detail.PlannedRate.ToString("F2")))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_PLANNED_RATE & uniqueID, detail.PlannedRate.ToString("F2")))

                ' planned cost
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell plannedCell right"
                cell.Controls.Add(CreateSpan(detail.PlannedCost.ToString("F2")))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_PLANNED_COST & uniqueID, detail.PlannedCost.ToString("F2")))

                ' other invoice units
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell otherInvoiceCell right"
                cell.Controls.Add(CreateSpan(detail.OtherInvoiceUnits.ToString("F2")))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_OTHER_UNITS & uniqueID, detail.OtherInvoiceUnits.ToString("F2")))

                ' other invoice cost
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell otherInvoiceCell right"
                cell.Controls.Add(CreateSpan(detail.OtherInvoiceCost.ToString("F2")))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_OTHER_COST & uniqueID, detail.OtherInvoiceCost.ToString("F2")))

                ' this invoice units
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell right"
                txt = CreateUnitsField(CTRL_PREFIX_DETAIL_THIS_UNITS & uniqueID, detail.MeasuredInDisplayAsHoursMins, detail.ThisInvoiceUnits, detail.MeasuredInMinutesPerUnit)
                cell.Controls.Add(txt)
                _detailLineStartupJS.AppendFormat("function {0}_Changed(id) {{ txtThisInvoiceUnits_Change(id); }};", txt.ID)

                ' this invoice service user units
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell right"
                txt = CreateUnitsField(CTRL_PREFIX_DETAIL_THIS_SU_UNITS & uniqueID, detail.MeasuredInDisplayAsHoursMins, detail.ThisInvoiceSUUnits, detail.MeasuredInMinutesPerUnit)
                cell.Controls.Add(txt)
                '++ Hide this field for visit-based invoices..
                If Not InvoiceIsSummaryBased() Then
                    cell.Style.Add("display", "none")
                End If
                _detailLineStartupJS.AppendFormat("function {0}_Changed(id) {{ txtThisInvoiceSUUnits_Change(id); }};", txt.ID)

                ' this invoice rate
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell right"
                cell.Controls.Add(CreateSpan(detail.ThisInvoiceRate.ToString("F2")))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_THIS_RATE & uniqueID, detail.ThisInvoiceRate.ToString("F2")))

                ' this invoice cost
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell right"

                If _canEditLineCost Then
                    txt = New TextBoxEx()
                    With txt
                        .ID = CTRL_PREFIX_DETAIL_THIS_COST & uniqueID
                        .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                        .Width = New Unit(4, UnitType.Em)
                        .Required = True
                        .RequiredValidatorErrMsg = "*"
                        .ValidationGroup = "Save"
                        .Text = detail.ThisInvoiceCost.ToString("F2")
                    End With
                    cell.Controls.Add(txt)
                    _detailLineStartupJS.AppendFormat("function {0}_Changed(id) {{ txtThisInvoiceCost_Change(id); }};", txt.ID)
                Else
                    cell.Controls.Add(CreateSpan(detail.ThisInvoiceCost.ToString("F2")))
                    cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_DETAIL_THIS_COST & uniqueID, detail.ThisInvoiceCost.ToString("F2")))
                End If

                If _dpi.ID <= 0 Or (_dpi.ID > 0 And _dpi.InvoiceStyle = DomProviderInvoiceStyle.SummaryLevel) Then
                    ' remove button
                    cell = New TableCell()
                    row.Cells.Add(cell)
                    cell.CssClass = "right"
                    btnRemove = New HtmlInputImage()
                    With btnRemove
                        .ID = CTRL_PREFIX_DETAIL_REMOVE & uniqueID
                        .Src = WebUtils.GetVirtualPath("images/delete.png")
                        .Alt = "Remove this detail line"
                        AddHandler .ServerClick, AddressOf btnRemoveDetail_Click
                        .Attributes.Add("onclick", "return btnRemoveDetail_Click();")
                        '.Visible = False
                    End With
                    cell.Controls.Add(btnRemove)
                End If

            End If

        End Sub

#End Region

#Region " CreateHiddenField "

        Private Function CreateHiddenField(ByVal id As String, ByVal value As String) As HiddenField

            Dim hid As HiddenField

            hid = New HiddenField()
            With hid
                .ID = id
                .Value = value
            End With

            Return hid

        End Function

#End Region

#Region " CreateSpan "

        Private Function CreateSpan(ByVal value As String) As HtmlGenericControl

            Dim span As HtmlGenericControl
            span = New HtmlGenericControl("span")
            span.InnerText = value
            Return span

        End Function

#End Region

#Region " CreateUnitsField "

        Private Function CreateUnitsField(ByVal uniqueID As String, _
                                          ByVal displayAsHoursMins As Boolean, _
                                          ByVal units As Decimal, _
                                          ByVal minutes As Integer) As Control

            Dim txt As TextBoxEx


            'If displayAsHoursMins Then

            'Else

            txt = New TextBoxEx()
            With txt
                .ID = uniqueID
                .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                .Width = New Unit(3, UnitType.Em)
                .Text = units.ToString("F2")
                .Required = True
                .RequiredValidatorErrMsg = "*"
                .ValidationGroup = "Save"
            End With
            Return txt

            'End If

        End Function

#End Region

#Region " LoadRateCategoryDropdown "

        Private Function LoadRateCategoryDropdown(ByVal dropdown As DropDownListEx, ByVal domContractID As Integer) As ErrorMessage

            Dim msg As ErrorMessage

            msg = FetchRateCategories(domContractID)
            If Not msg.Success Then Return msg

            If _uoms Is Nothing Then
                msg = DomUnitsOfMeasure.FetchList(Me.DbConnection, _uoms, String.Empty, String.Empty)
                If Not msg.Success Then Return msg
            End If

            With dropdown.DropDownList
                .Items.Clear()
                For Each rc As DomRateCategory In _rateCategories
                    .Items.Add(New ListItem(rc.Description, rc.ID))
                Next
                .Items.Insert(0, String.Empty)
            End With

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " LoadRatesDropdown "

        Private Function LoadRatesDropdown(ByVal dropdown As DropDownListEx, _
                                           ByVal line As DetailLine) As ErrorMessage

            Dim msg As ErrorMessage
            Dim formattedRates As List(Of String) = Nothing

            msg = line.FetchRateList(_dpi.DomContractID, _dpi.ClientID, formattedRates)
            If Not msg.Success Then Return msg

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = formattedRates
                .DataBind()
                If formattedRates.Count > 1 Then .Items.Insert(0, String.Empty)
            End With

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " FetchRateCategories "

        Private Function FetchRateCategories(ByVal domContractID As Integer) As ErrorMessage
            Dim LstRateCat As DomRateCategoryCollection = Nothing

            Dim msg As ErrorMessage

            If _rateCategories Is Nothing Then
                msg = DomContract_DomRateCategory.FetchDomRateCategoryList(Me.DbConnection, LstRateCat, String.Empty, String.Empty, domContractID)
                If Not msg.Success Then Return msg
                _rateCategories = New DataClasses.Collections.DomRateCategoryCollection()
                _rateCategories.AddRange(LstRateCat.ToArray().Where(Function(x) x.DomServiceTypeID > 0).ToArray())
                _rateCategories.Sort(New CollectionSorter("Description", SortDirection.Ascending))
                _rateCategories.Sort(New CollectionSorter("SortOrder", SortDirection.Ascending))
            End If

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " SetupInPlaceSelectors "

        Private Sub SetupProviderSelector(ByVal providerID As Integer)
            With CType(Me.provider, InPlaceSelectors.InPlaceEstablishmentSelector)
                .EstablishmentID = providerID
                If _dpi.ID > 0 OrElse _dpi.ID = -1 Then
                    ' if this is an existing invoice do not validate
                    .Required = False
                    .ValidationGroup = String.Empty
                Else
                    ' else is existing invoice so validate
                    .Required = True
                    .ValidationGroup = "Save"
                End If
            End With
        End Sub

        Private Sub SetupContractSelector(ByVal contractID As Integer, ByVal providerID As Integer)
            With CType(Me.domContract, InPlaceSelectors.InPlaceDomContractSelector)
                .ContractID = contractID
                If _dpi.ID > 0 OrElse _dpi.ID = -1 Then
                    ' if this is an existing invoice do not validate
                    .Required = False
                    .ValidationGroup = String.Empty
                Else
                    ' else is existing invoice so validate
                    .Required = True
                    .ValidationGroup = "Save"
                End If
                If providerID > 0 Then
                    .Enabled = True
                Else
                    .Enabled = False
                End If
            End With
        End Sub

        Private Sub SetupClientSelector(ByVal clientID As Integer)
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .ClientDetailID = clientID
                If _dpi.ID > 0 OrElse _dpi.ID = -1 Then
                    ' if this is an existing invoice do not validate
                    .Required = False
                    .ValidationGroup = String.Empty
                Else
                    ' else is existing invoice so validate
                    .Required = True
                    .ValidationGroup = "Save"
                End If
            End With
        End Sub

#End Region

#Region " btnCreate_Click "

        Private Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click

            Dim msg As ErrorMessage
            Dim providerID As Integer, domContractID As Integer, clientID As Integer, suspensionReasonID As Integer
            Dim newDpiData As NewDomProviderInvoiceData

            providerID = Utils.ToInt32(Request.Form(CType(provider, InPlaceEstablishmentSelector).HiddenFieldUniqueID))
            domContractID = Utils.ToInt32(Request.Form(CType(domContract, InPlaceDomContractSelector).HiddenFieldUniqueID))
            clientID = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))

            _stdBut.ShowSave = False

            ' re-populate suspension reasons
            msg = PopulateSuspensionReasons(0)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            cboSuspensionReason.SelectPostBackValue()
            suspensionReasonID = Utils.ToInt32(cboSuspensionReason.GetPostBackValue())
            cboSuspensionReason.RequiredValidator.Enabled = chkSuspend.CheckBox.Checked

            ' re-populate in-place selectors
            SetupProviderSelector(providerID)
            SetupContractSelector(domContractID, providerID)
            SetupClientSelector(clientID)

            'Validate these date values as thay cant be in the future.
            If txtInvoiceDate.Value > Date.Today Then
                lblError.Text = "The Invoice date can not be in the future."
                Exit Sub
            End If

            If txtDateReceived.Value > Date.Today Then
                lblError.Text = "The date recieved can not be in the future."
            End If

            Me.Validate("Save")

            If Me.IsValid() Then

                msg = DomProviderInvoiceBL.ValidateInvoicePeriods(Me.DbConnection, txtWeekEndingFrom.Text, txtWeekEndingTo.Text)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    Exit Sub
                End If

                msg = DomProviderInvoiceBL.ValidateInvoiceContractPeriods(DbConnection, domContractID, DateTime.Parse(txtWeekEndingFrom.Text), DateTime.Parse(txtWeekEndingTo.Text))
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    Exit Sub
                End If

                newDpiData = StoreNewDpiData(New NewDomProviderInvoiceData(), providerID, domContractID, clientID, suspensionReasonID)

                msg = _dpi.AddNew(newDpiData, DomProviderInvoiceStyle.SummaryLevel)
                If Not msg.Success Then
                    ClearNewDpiData()
                    WebUtils.DisplayError(msg)
                End If

                msg = PopulateScreen()
                If Not msg.Success Then WebUtils.DisplayError(msg)

                _stdBut.ShowSave = True

            End If

        End Sub

#End Region

#Region " btnAddDetail_Click "

        Private Sub btnAddDetail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddDetail.Click

            Dim msg As ErrorMessage
            Dim id As String
            Dim list As List(Of String)
            Dim newLine As DetailLine = Nothing
            Dim insertDetailLineAtIndex As Integer = 0

            msg = RepopulateScreen(True)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' add a new detail line
            msg = _dpi.AddDetail(dteAddDetailWeekEnding.Text, _
                                 Utils.ToInt32(cboAddDetailRateCategory.GetPostBackValue()), _
                                 Utils.ToDecimal(cboAddDetailRate.GetPostBackValue()), _
                                 newLine)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' default SU units to be the same as provider units
            newLine.ThisInvoiceSUUnits = newLine.ThisInvoiceUnits

            list = GetDetailUniqueIDsFromViewState()
            id = GetDetailUniqueID(newLine)

            ' determine if we need to insert this row at a specific index based on the week ending date of detail lines
            insertDetailLineAtIndex = GetIndexOfNewDetailLine(newLine)

            ' create the controls
            OutputDetailControls(id, newLine, insertDetailLineAtIndex)

            ' persist the data into view state using the calculated index
            list.Insert(insertDetailLineAtIndex, id)
            PersistDetailUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " btnRemoveDetail_Click "

        Private Sub btnRemoveDetail_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim msg As ErrorMessage

            Dim list As List(Of String) = GetDetailUniqueIDsFromViewState()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_DETAIL_REMOVE, String.Empty)

            ' change the id in viewstate
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    list.RemoveAt(index)
                    Exit For
                End If
            Next

            ' remove from the grid
            For index As Integer = 0 To phDetailsSummaryVisitLevel.Controls.Count - 1
                If phDetailsSummaryVisitLevel.Controls(index).ID = id Then
                    phDetailsSummaryVisitLevel.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistDetailUniqueIDsToViewState(list)

            msg = RepopulateScreen(True)
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region " PrimeDpiClass "

        Private Function PrimeDpiClass() As ErrorMessage

            Dim msg As ErrorMessage
            Dim newDpiData As NewDomProviderInvoiceData

            If _invoiceID > 0 Then
                ' editing an existing invoice
                ' re-fetch the invoice
                msg = _dpi.Fetch(_invoiceID, _estabID)
            Else
                ' creating a new invoice
                newDpiData = FetchNewDpiData()
                ' re-call AddNew() to re-create the suggested invoice
                msg = _dpi.AddNew(newDpiData, DomProviderInvoiceStyle.SummaryLevel)
            End If

            Return msg

        End Function

#End Region

#Region " HaveNewDpiData "

        Private Function HaveNewDpiData() As Boolean
            Return Not (Session(SESSION_NEW_DOM_PROVIDER_INVOICE) Is Nothing)
        End Function

#End Region

#Region " FetchNewDpiData "

        Private Function FetchNewDpiData() As NewDomProviderInvoiceData
            Return Session(SESSION_NEW_DOM_PROVIDER_INVOICE)
        End Function

#End Region

#Region " StoreNewDpiData "

        Private Function StoreNewDpiData(ByVal newDpi As NewDomProviderInvoiceData, _
                                        ByVal providerID As Integer, _
                                        ByVal domContractID As Integer, _
                                        ByVal clientID As Integer, _
                                        ByVal suspensionReasonID As Integer) As NewDomProviderInvoiceData

            With newDpi
                .ProviderID = providerID
                .DomContractID = domContractID
                .ClientID = clientID
                If Utils.IsDate(txtWeekEndingFrom.Text) Then .WeekEndingFrom = DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing, txtWeekEndingFrom.Text)
                If Utils.IsDate(txtWeekEndingTo.Text) Then .WeekEndingTo = txtWeekEndingTo.Text
                .Reference = txtReference.Text
                .InvoiceDate = txtInvoiceDate.Text
                .DateReceived = txtDateReceived.Text
                .SuspensionReasonID = suspensionReasonID
                .Notes = txtNotes.Text
            End With
            Session(SESSION_NEW_DOM_PROVIDER_INVOICE) = newDpi

            Return newDpi

        End Function

#End Region

#Region " ClearNewDpiData "

        Private Sub ClearNewDpiData()
            Session(SESSION_NEW_DOM_PROVIDER_INVOICE) = Nothing
        End Sub

#End Region

#Region " GetDetailUniqueIDsFromViewState "

        Private Function GetDetailUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_DETAILS) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_DETAILS), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_DETAILS) Is Nothing Then
                _newDetailIDCounter = 0
            Else
                _newDetailIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_DETAILS), Integer)
            End If

            Return list

        End Function

#End Region

#Region " GetDetailUniqueID "

        Private Function GetDetailUniqueID(ByVal detail As DetailLine) As String

            Dim id As String

            If detail.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_DETAIL & _newDetailIDCounter
                _newDetailIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_DETAIL & detail.ID
            End If

            Return id

        End Function

#End Region

#Region " PersistDetailUniqueIDsToViewState "

        Private Sub PersistDetailUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_DETAILS, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_DETAILS, _newDetailIDCounter)
        End Sub

#End Region

#Region " GetRemovedDetailsToViewState "

        Private Function GetRemovedDetailsToViewState() As List(Of Triplet)

            Dim list As List(Of Triplet)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_DETAILS_REMOVED) Is Nothing Then
                list = New List(Of Triplet)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_DETAILS_REMOVED), List(Of Triplet))
            End If

            Return list

        End Function

#End Region

#Region " PersistRemovedDetailsToViewState "

        Private Sub PersistRemovedDetailsToViewState(ByVal list As List(Of Triplet))
            ViewState.Add(VIEWSTATE_KEY_DATA_DETAILS_REMOVED, list)
        End Sub

#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            With _btnAuditDetails
                .ID = "btnAuditDetails"
                .Value = "Audit Details"
            End With
            controls.Add(_btnAuditDetails)

            With CType(auditDetails, IBasicAuditDetails)
                .ToggleControlID = _btnAuditDetails.ClientID
            End With

        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim allowEdit As Boolean
            Dim weekCommencingDate As DateTime = DomContractBL.GetWeekCommencingDate(DbConnection, Nothing)
            Dim weekEndingDate As DateTime = DomContractBL.GetWeekEndingDate(DbConnection, Nothing)

            ' additional security for edit button
            With _dpi
                If _stdBut.AllowEdit Then
                    ' only need to look at this if the user currently has access to edit
                    allowEdit = False
                    If .InvoiceStyle = DomProviderInvoiceStyle.SummaryLevel Then
                        If .Status = DomProviderInvoiceStatus.Unpaid OrElse _
                                (_canEditSuspendedInvoices AndAlso .Status = DomProviderInvoiceStatus.Suspended AndAlso _isAutoSuspensionReason) Then
                            allowEdit = True
                        End If
                    End If
                    _stdBut.AllowEdit = allowEdit
                End If
            End With

            ' set the allowable days
            txtWeekEndingFrom.AllowableDays = Integer.Parse(weekCommencingDate.DayOfWeek)
            txtWeekEndingTo.AllowableDays = Integer.Parse(weekEndingDate.DayOfWeek)
            dteAddDetailWeekEnding.AllowableDays = Integer.Parse(weekEndingDate.DayOfWeek)

            With dteAddDetailWeekEnding
                .MinimumValue = txtWeekEndingFrom.Value
                .MaximumValue = txtWeekEndingTo.Value
                If _preventEntryOfFutureDates _
                    AndAlso txtWeekEndingTo.Value.ToString().Length > 0 _
                    AndAlso DateTime.Now < DateTime.Parse(txtWeekEndingTo.Value) Then
                    .MaximumValue = DateTime.Now.ToString(DateAndTimeFormat)
                End If
            End With

        End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Const SCRIPT_STARTUP As String = "Startup"

            Dim js As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage = Nothing
            Dim autoSuspendClient As Boolean = False
            Dim jsSerializer As New JavaScriptSerializer()

            ' select the current tab
            tabStrip.SetActiveTabByHeaderText(hidSelectedTab.Value)

            If _dpi.ID > 0 OrElse _dpi.ID = -1 Then
                ' disable fields when editing existing or part way through creating
                ' when editing existing, _dpi.ID > 0
                ' when part way through creating,  _dpi.ID = -1
                lblProvider.CssClass = "disabled"
                js.AppendFormat("InPlaceEstablishmentSelector_Enabled('{0}',false);", CType(Me.provider, InPlaceSelectors.InPlaceEstablishmentSelector).ClientID)
                lblContract.CssClass = "disabled"
                js.AppendFormat("InPlaceDomContractSelector_Enabled('{0}',false);", CType(Me.domContract, InPlaceSelectors.InPlaceDomContractSelector).ClientID)
                lblClient.CssClass = "disabled"
                js.AppendFormat("InPlaceClientSelector_Enabled('{0}',false);", CType(Me.client, InPlaceSelectors.InPlaceClientSelector).ClientID)
                txtWeekEndingFrom.Enabled = False
                txtWeekEndingTo.Enabled = False

                btnCreate.Visible = False

                If _dpi.InvoiceStyle = DomProviderInvoiceStyle.SummaryLevel Then
                    btnAddDetail.Visible = True
                End If

                If _dpi.ID = -1 Then
                    pnlHeaderExistingInvoice.Visible = False
                    pnlHeaderExistingInvoiceStaus.Visible = False
                    tabDetails.Enabled = True
                    tabSummary.Enabled = True
                End If

                With CType(Me.provider, InPlaceSelectors.InPlaceEstablishmentSelector)
                    .Required = False
                End With

                With CType(Me.domContract, InPlaceSelectors.InPlaceDomContractSelector)
                    .Required = False
                End With

                With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                    .Required = False
                End With

                js.Append("Edit_HeadersEditable = false;")

                'serialise payment tolerance javascript arrays for use client side.
                With jsSerializer
                    If Not _dpi.PaymentTolerances Is Nothing AndAlso _dpi.PaymentTolerances.Count > 0 Then
                        _jsoPaymentTolerances = .Serialize(_dpi.PaymentTolerances.ToArray)
                    End If
                    If Not _dpi.PaymentToleranceGroups Is Nothing AndAlso _dpi.PaymentToleranceGroups.Count > 0 Then
                        _jsoPaymentToleranceGroups = .Serialize(_dpi.PaymentToleranceGroups.ToArray)
                    End If
                End With
            Else
                ' hide fields when creating
                pnlHeaderExistingInvoice.Visible = False
                pnlHeaderExistingInvoiceStaus.Visible = False
                tabDetails.Enabled = False
                tabSummary.Enabled = False
                js.Append("Edit_HeadersEditable = true;")
                btnAddDetail.Visible = True
            End If

            If pnlSummarySummaryLevel.Visible Then
                txtSummaryActualCost.TextBox.Enabled = False
                txtSummaryNetCost.TextBox.Enabled = False
                txtSummaryInvoiceTotal.TextBox.Enabled = False
                js.AppendFormat("Edit_txtActualCostID='{0}';", txtSummaryActualCost.ClientID)
                js.AppendFormat("Edit_chkVatID='{0}';", chkSummaryVat.ClientID)
                js.AppendFormat("Edit_vatID='{0}';", txtSummaryVat.ClientID)
                js.AppendFormat("Edit_penaltyPaymentID='{0}';", txtSummaryPenalty.ClientID)
                js.AppendFormat("Edit_clientContribID='{0}';", txtSummaryClientContrib.ClientID)
                js.AppendFormat("Edit_txtNetCostID='{0}';", txtSummaryNetCost.ClientID)
                js.AppendFormat("Edit_txtInvoiceTotalID='{0}';", txtSummaryInvoiceTotal.ClientID)
            End If

            js.AppendFormat("Edit_domContractID='{0}';InPlaceDomContractSelector_providerID={1};", domContract.ClientID, _estabID)
            js.AppendFormat("Edit_chkSuspendID='{0}_chkCheckbox';", chkSuspend.ClientID)
            js.AppendFormat("Edit_cboSuspensionReasonID='{0}';", cboSuspensionReason.ClientID)
            js.AppendFormat("Edit_domContractID='{0}';", domContract.ClientID)
            js.AppendFormat("Edit_clientID='{0}';", client.ClientID)
            js.AppendFormat("Edit_dteAddDetailWeekEndingID='{0}';", dteAddDetailWeekEnding.ClientID)
            js.AppendFormat("Edit_cboAddDetailRateCategoryID='{0}';", cboAddDetailRateCategory.ClientID)
            js.AppendFormat("Edit_cboAddDetailRateID='{0}';", cboAddDetailRate.ClientID)
            js.AppendFormat("Edit_canEditLineCost={0};", _canEditLineCost.ToString().ToLower())
            js.AppendFormat("Edit_invoiceStyle={0};", Convert.ToInt32(_dpi.InvoiceStyle))
            js.AppendFormat("Edit_mode={0};", Convert.ToInt32(_stdBut.ButtonsMode))
            js.AppendFormat("Edit_txtWeekEndingFromID='{0}';", txtWeekEndingFrom.ClientID)
            js.AppendFormat("Edit_txtWeekEndingToID='{0}';", txtWeekEndingTo.ClientID)
            js.AppendFormat("Edit_preventEntryOfFutureDates={0};", _preventEntryOfFutureDates.ToString().ToLower())
            js.AppendFormat("Edit_rvInvoiceDate='{0}';", IIf(_preventEntryOfFutureDates, rvInvoiceDate.ClientID, ""))
            js.AppendFormat("Edit_invoiceDate='{0}_txtTextBox';", txtInvoiceDate.ClientID)
            js.AppendFormat("InvoiceHasNotes='{0}';", _InvoiceHasNotes.ToString().ToLower())
            If String.IsNullOrEmpty(_naturalPeriodOfInvoicesScript) = False Then js.Append(_naturalPeriodOfInvoicesScript)
            'Add pyament tolerance collections to javascript
            js.AppendFormat("var paymentTolerances = {0};", _jsoPaymentTolerances)
            js.AppendFormat("var paymentToleranceGroups = {0};", _jsoPaymentToleranceGroups)
            If _dpi.ClientID > 0 Then
                ' if we have a client id to check for auto suspension

                msg = DomProviderInvoiceBL.ShouldAutoSuspendClient(DbConnection, _dpi.ClientID, autoSuspendClient)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                js.AppendFormat("Edit_AutoSuspendClient={0};", autoSuspendClient.ToString().ToLower())

            End If

            js.AppendFormat("Init();")
            js.Append(_detailLineStartupJS.ToString())

            Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, js.ToString(), True)


            EnableSelectedControl()

        End Sub

#End Region

        Private Sub EnableSelectedControl()
            Try
                For Each Item As String In _controlsToenable

                    Dim hlink As HyperLink = New HyperLink()
                    hlink = Utils.FindControlRecursive(Me.Page, Item)
                    If Not hlink Is Nothing Then
                        hlink.Enabled = True
                        hlink.CssClass = "enabled"
                        CType(hlink.Parent, TableCell).Enabled = True
                    End If
                Next
            Catch ex As Exception

            End Try
        End Sub

#Region " Include Java script "
        Private Sub IncludeJavascript()

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

            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/NonDelivery/NonDeliveryUnitBasedDialog.js"))

            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/NonDelivery/NonDeliveryVisitBasedDialog.js"))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomProviderInvoice))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomProfomaInvoice))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.PaymentSchedule))
        End Sub
#End Region

#Region " Render "

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

            ' output postback javascript for add detail button
            Dim options As PostBackOptions = New PostBackOptions(Me.btnAddDetail)
            If Not options Is Nothing Then
                options.PerformValidation = True
                options.ValidationGroup = "AddDetail"
                Page.ClientScript.RegisterForEventValidation(options)
                litAddDetailDoPostBackJS.Text = Page.ClientScript.GetPostBackEventReference(options)
            End If
            MyBase.Render(writer)

        End Sub

#End Region

#Region " Page_Unload "

        Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
            If Not _dpi Is Nothing Then
                _dpi.Dispose()
            End If
        End Sub

#End Region

#Region " InvoiceIsSummaryBased "

        Protected Function InvoiceIsSummaryBased() As Boolean
            Return (_dpi.InvoiceStyle = DomProviderInvoiceStyle.SummaryLevel)
        End Function

#End Region

#Region "SetUpPreventEntryOfFutureDatesRangeValidator"

        ''' <summary>
        ''' Sets up prevent entry of future dates range validator.
        ''' </summary>
        ''' <param name="validator">The validator.</param>
        ''' <param name="controlToValidate">The control to validate.</param>
        Private Sub SetUpPreventEntryOfFutureDatesRangeValidator(ByVal validator As RangeValidator, _
                                                                 ByVal controlToValidate As TextBoxEx)

            With validator
                .MinimumValue = DateTime.MinValue.ToString(DateAndTimeFormat)
                .MaximumValue = DateTime.Now.ToString(DateAndTimeFormat)
                .ControlToValidate = String.Format("{0}${1}", controlToValidate.ID, controlToValidate.TextBox.ID)
                .Type = ValidationDataType.Date
                .Visible = True
            End With

        End Sub

#End Region

#Region "GetIndexOfNewDetailLine"

        ''' <summary>
        ''' Gets the index of a new detail line by looping each detail line and evaluating the week end dates
        ''' </summary>
        ''' <param name="newLine">The new line to insert</param>
        ''' <returns></returns>
        Public Function GetIndexOfNewDetailLine(ByVal newLine As DetailLine) As Integer

            Dim currentControlIndex As Integer = 0
            Dim insertDetailLineAtIndex As Integer = 0

            For Each tmpControl As Control In phDetailsSummaryVisitLevel.Controls
                ' loop each control in the phDetailsSummaryVisitLevel control

                Dim weekEndingCell As TableCell
                Dim weekEndingCellSpan As HtmlGenericControl
                Dim weekEndingCellSpanValue As String
                Dim weekEndingDate As DateTime

                weekEndingCell = CType(tmpControl.Controls(0), TableCell)   ' get the 1st cell of the row, which is the week ending date
                weekEndingCellSpan = CType(weekEndingCell.Controls(0), HtmlGenericControl)  ' get the span control which contains the week ending date
                weekEndingCellSpanValue = weekEndingCellSpan.InnerText

                If String.IsNullOrEmpty(weekEndingCellSpanValue) = False _
                    AndAlso weekEndingCellSpanValue.Trim().Length > 0 _
                    AndAlso DateTime.TryParse(weekEndingCellSpanValue, weekEndingDate) Then
                    ' if the value in the span control is a valid date then compare to this row

                    If newLine.WeekEnding >= weekEndingDate Then
                        ' if the week ending date on the new line is >= row week ending date
                        ' increment insertDetailLineAtIndex and keep incrementing until the rows
                        ' week ending date is larger

                        insertDetailLineAtIndex = currentControlIndex + 1

                    ElseIf newLine.WeekEnding < weekEndingDate Then
                        ' the new lines week ending date is smaller than the rows
                        ' so exit for loop accepting value of insertDetailLineAtIndex

                        Exit For

                    End If

                End If

                currentControlIndex += 1

            Next

            Return insertDetailLineAtIndex

        End Function

#End Region

#Region "IsPopupWindow"

        ''' <summary>
        ''' Gets a value indicating whether this instance is popup window.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance is popup window; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property IsPopupWindow() As Boolean
            Get
                Return (Target.Library.Utils.ToInt32(Request.QueryString("autopopup")) = 1)
            End Get
        End Property

#End Region

#Region " View Invoice Notes "
        Private Sub GetInvoiceNotes(ByVal id As Integer)
            Dim msg As New ErrorMessage
            Dim invoice As DomProviderInvoice = _
            New DomProviderInvoice(Me.DbConnection)
            msg = invoice.Fetch(id)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
                Return
            End If
            If Target.Library.Utils.ToInt32(invoice.ProviderInvoiceNoteID) <> 0 Then
                Dim invNotes As DomProviderInvoiceNotes = New DomProviderInvoiceNotes(Me.DbConnection)
                msg = New ErrorMessage
                msg = invNotes.Fetch(invoice.ProviderInvoiceNoteID)
                If Not msg.Success Then
                    WebUtils.DisplayError(msg)
                    Return
                End If
                invNotes.Notes = HandleLineFeedCarriageReturns(invNotes.Notes)


                Dim js As String = String.Empty
                Dim ctrl As New LiteralControl
                ctrl.Text = invNotes.Notes
                pnlNotes.Controls.Add(ctrl)
                lblDate.Text = invNotes.Date.ToString("dd/MM/yyyy")
                lblat.Text = invNotes.Time.ToString("hh:mm:ss")
                'IsPopUp.Visible = displayAsPopUp

                'Dim ctrlEmbeded As New LiteralControl
                'ctrlEmbeded.Text = invNotes.Notes
                'pnlNotesEmbeded.Controls.Add(ctrlEmbeded)
                'lblDateEmbeded.Text = invNotes.Date.ToString("dd/MM/yyyy")
                'lblatEmbeded.Text = invNotes.Time.ToString("hh:mm:ss")
                'IsNotPopUp.Visible = Not displayAsPopUp

                'thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
                InvoiceHasNotes = True

            End If
        End Sub

        Private Function HandleLineFeedCarriageReturns(ByVal notes As String) As String
            Dim regExp As RegularExpressions.Regex = New RegularExpressions.Regex("(\r\n|\r|\n)+")
            notes = regExp.Replace(notes, "<br/>")
            Return notes
        End Function

#End Region

#Region "Periodic Block"
        Private Function PeriodicBlock(domcontractId As Integer, ByRef isBlockPeriodicContract As Boolean) As ErrorMessage
            Dim contract As DomContract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            Dim msg As New ErrorMessage
            msg = contract.Fetch(domcontractId)
            If Not msg.Success Then Return msg

            If contract.ContractType = [Enum].GetName(GetType(DomContractType), DomContractType.BlockPeriodic) Then
                isBlockPeriodicContract = True
            End If
            Return msg
        End Function
#End Region
    End Class

End Namespace