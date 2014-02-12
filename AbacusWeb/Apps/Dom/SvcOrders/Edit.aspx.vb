Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports FcRules = Target.Abacus.Library.FinanceCodeRules
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.FinanceCodes
Imports Target.Abacus.Library.PaymentTolerance
Imports Target.Abacus.Library.Results.Messages.SearcherSettings.Items
Imports Target.Abacus.Library.Selectors.Messages
Imports Target.Abacus.Library.Selectors.Utilities
Imports Target.Abacus.Library.ServiceOrder
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.Extensions.AjaxControlToolkit
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.SvcOrders

    ''' <summary>
    ''' Screen used to maintain a domiciliary service orders.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     JohnF       29/11/2013  Bypass error found when incorrect start date specified on new order (#8353)
    '''     JohnF       23/10/2013  Reworked back URL in Delete event using browser-friendly logic (#8272)
    '''     JohnF       23/10/2013  Remember previous search settings before redirect (#8272)
    '''     JohnF       22/10/2013  Redirect to the Service Order search screen at all times after order deletion (#8272)
    '''     JohnF       17/10/2013  Reload previous page on svc order deletion (#8229)
    '''     JohnF       09/08/2013  Format expenditure unit cost with pound-sign (#8073)
    '''     JohnF       07/06/2013  Changes for overriding unit costs (D12460)
    '''     JohnF       07/06/2013  Reinstated accidentally-knocked-out #7864 (#7918)
    '''     JohnF       21/05/2013  Allow edit of selected fields when in 'lock' mode (D12479)
    '''     MoTahir     16/11/2012  D12343 - Remove Framework Type From Service Group
    '''     PaulW       11/01/2012  Beta Testing Issue 701 - Units Formated correctly.
    '''     PaulW       24/11/2011  Beta Testing Issue 368 - Minutes not formatted correctly. 
    '''     PaulW       10/11/2011  A4WA#7074 - DSO errors instead of friendly message when validation fails.
    '''     Motahir     12/10/2011  B162 Beta 2 Testing Issue 194 - Payments Tolerance TAB on service order displays no information. 
    '''     ColinD      11/10/2011  D12203 - Replace wording for Non Residential Service Order to Service Order
    '''     MoTahir     13/08/2011  D11766 - eInvoicing - Provider Invoice Tolerances
    '''     ColinD      09/08/2011  D11965 - Numerous changes as stated in spec.
    '''     MoTahir     06/07/2011  A4WA#6910 - The Effective from date is disabled although the warning states that this is required
    '''     MoTahir     22/06/2011  A4WA#6854 - You cannot save a Day Care Service Order when the Day Care Contract is not set up to Record Day of Week Against Care Plan Summary
    '''     Iftikhar    11/05/2011  SDS issue #674 - fixed Documents security issue
    '''     MikeVO      26/04/2011  SDS issue #607 - corrected behavior when displayed in popup window.
    '''     MikeVO      21/04/2011  SDS issue #606 - changed "Documents" tab to use an iframe.
    '''     MikeVO      21/04/2011  SDS issue #413 - changed "domiciliary" text to "non-residential".
    '''                             Also corrected when "Documents" tab is displayed.
    '''     PaulW       08/04/2011  SDS Issue 543, Summary tab available when adding new record.
    '''     ColinD      07/11/2010  Altered StdButtons_AddCustomControls to only add the padlock image control if required, prevents small circle being displayed when not required.
    '''     MikeVO      01/09/2010  Added validation summary.
    '''     PaulW       10/06/2010  A4WA#6361 - populate project code drop down differently
    '''     PaulW       10/06/2010  A4WA#6357 - Project Code not saving for new records.
    '''     Mo          20/05/2010  D11806 - Rate category ordering
    '''     ColinD      06/05/2010  D11756 - provider ability to select from a list of project codes
    '''     ColinD      22/04/2010  D11807 - provided ability to copy service orders
    '''     MikeVO      01/04/2010  A4WA#6196 - prevent caching of finance code rule segments.
    '''     MikeVO      23/02/2010  Corrected code to remove compiler warnings.
    '''     PaulW       11/02/2010  D11769 DSO Attendance Extend Validation
    '''     PaulW       21/01/2010  A4WA#6041 - Attendance detail not saving
    '''     MikeVO      28/01/2010  D11649 - added expenditure code to Expenditure tab.
    '''     PaulW       06/01/2010  A4WA#5963 - Various issues found during testing of D11680
    '''     PaulW       05/01/2010  A4WA#5999 - Tabs not persisting values when navigating between Attendence revisions
    '''     PaulW       18/09/2009  D11680 - attendance Tab
    '''     MikeVO      05/06/2009  A4WA#5492 - fix to #5406 when disabling dom contract selector.
    '''     MikeVO      02/06/2009  D11554 - support for first week of service.
    '''     MikeVO      20/04/2009  A4WA#5386 – various fixes.
    '''     MikeVO      16/04/2009  A4WA#5386 – various fixes.
    '''     MikeVO      23/01/2009  D11469 - fix to configuration of Summary rows.
    '''     MikeVO      23/01/2009  D11490 - display units on Expenditure tab in same format as Summary tab.
    '''     MikeVO      20/01/2009  D11472 - added suspensions tab.
    '''     MikeVO      13/01/2009  D11470 - end reason changes.
    '''     MikeVO      13/01/2009  D11490 - added Expenditure tab.
    '''     MikeVO      08/01/2009  D11469 - support for units of measure.
    '''     MikeVO      06/01/2009  D11468 - support for non-visit-based services.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Public Class Edit
        Inherits BasePage

#Region " Consts "

        Const VIEWSTATE_KEY_DATA_SUMMARY As String = "SummaryDataList"
        Const VIEWSTATE_KEY_COUNTER_NEW_SUMMARY As String = "NewSummaryCounter"
        Const VIEWSTATE_KEY_DATA_VISITS As String = "VisitsDataList"
        Const VIEWSTATE_KEY_COUNTER_NEW_VISITS As String = "NewVisitCounter"
        Const VIEWSTATE_KEY_DATA_EXPENDITURE As String = "ExpDataList"
        Const VIEWSTATE_KEY_DATA_ATTENDANCE As String = "AttendanceDataList"
        Const VIEWSTATE_KEY_COUNTER_NEW_ATTENDANCE As String = "NewAttCounter"
        Const VIEWSTATE_KEY_DATA_ATTENDANCESUMMARY As String = "AttendanceSummaryDataList"
        Const VIEWSTATE_KEY_COUNTER_NEW_ATTENDANCESUMMARY As String = "NewAttSummaryCounter"

        Const CTRL_PREFIX_SUMMARY_LINE_DATE_FROM As String = "sumLineDateFrom"
        Const CTRL_PREFIX_SUMMARY_LINE_DATE_TO As String = "sumLineDateTo"
        Const CTRL_PREFIX_SUMMARY_RATE_CATEGORY As String = "sumRateCat"
        Const CTRL_PREFIX_SUMMARY_DOW As String = "sumDoW"
        Const CTRL_PREFIX_SUMMARY_MON As String = "sumMon"
        Const CTRL_PREFIX_SUMMARY_TUE As String = "sumTue"
        Const CTRL_PREFIX_SUMMARY_WED As String = "sumWed"
        Const CTRL_PREFIX_SUMMARY_THU As String = "sumThu"
        Const CTRL_PREFIX_SUMMARY_FRI As String = "sumFri"
        Const CTRL_PREFIX_SUMMARY_SAT As String = "sumSat"
        Const CTRL_PREFIX_SUMMARY_SUN As String = "sumSun"
        Const CTRL_PREFIX_SUMMARY_ALLDAYS As String = "sumAll"
        Const CTRL_PREFIX_SUMMARY_MON_DSOD As String = "sumMonDsod"
        Const CTRL_PREFIX_SUMMARY_TUE_DSOD As String = "sumTueDsod"
        Const CTRL_PREFIX_SUMMARY_WED_DSOD As String = "sumWedDsod"
        Const CTRL_PREFIX_SUMMARY_THU_DSOD As String = "sumThuDsod"
        Const CTRL_PREFIX_SUMMARY_FRI_DSOD As String = "sumFriDsod"
        Const CTRL_PREFIX_SUMMARY_SAT_DSOD As String = "sumSatDsod"
        Const CTRL_PREFIX_SUMMARY_SUN_DSOD As String = "sumSunDsod"
        Const CTRL_PREFIX_SUMMARY_UNITS As String = "sumUnits"
        Const CTRL_PREFIX_SUMMARY_UNITS_HOURS As String = "sumUnitsHours"
        Const CTRL_PREFIX_SUMMARY_UNITS_MINUTES As String = "sumUnitsMins"
        Const CTRL_PREFIX_SUMMARY_SUMINUTES As String = "sumMinutes"
        Const CTRL_PREFIX_SUMMARY_VISITS As String = "sumVisits"
        Const CTRL_PREFIX_SUMMARY_FREQUENCY As String = "sumFreq"
        Const CTRL_PREFIX_SUMMARY_FIRST_WEEK As String = "sumFirstWeek"
        Const CTRL_PREFIX_SUMMARY_REMOVED As String = "sumRemove"

        Const CTRL_PREFIX_VISITS_LINE_DATE_FROM As String = "visitLineDateFrom"
        Const CTRL_PREFIX_VISITS_LINE_DATE_TO As String = "visitLineDateTo"
        Const CTRL_PREFIX_VISITS_SVC_TYPE As String = "visitSvcType"
        Const CTRL_PREFIX_VISITS_START_TIME As String = "visitStart"
        Const CTRL_PREFIX_VISITS_DURATION As String = "visitDur"
        Const CTRL_PREFIX_VISITS_CARERS As String = "visitCarers"
        Const CTRL_PREFIX_VISITS_MON As String = "visitMon"
        Const CTRL_PREFIX_VISITS_TUE As String = "visitTue"
        Const CTRL_PREFIX_VISITS_WED As String = "visitWed"
        Const CTRL_PREFIX_VISITS_THU As String = "visitThu"
        Const CTRL_PREFIX_VISITS_FRI As String = "visitFri"
        Const CTRL_PREFIX_VISITS_SAT As String = "visitSat"
        Const CTRL_PREFIX_VISITS_SUN As String = "visitSun"
        Const CTRL_PREFIX_VISITS_ALLDAYS As String = "visitAll"
        Const CTRL_PREFIX_VISITS_FREQUENCY As String = "visitFreq"
        Const CTRL_PREFIX_VISITS_FIRST_WEEK As String = "visitFirstWeek"
        Const CTRL_PREFIX_VISITS_PRIMARY As String = "visitPrimary"
        Const CTRL_PREFIX_VISITS_REMOVED As String = "visitRemove"

        Const CTRL_PREFIX_ATT_SUMMARY_RATE_CAT As String = "attSumRateCat"
        Const CTRL_PREFIX_ATT_SUMMARY_MON As String = "attSumMon"
        Const CTRL_PREFIX_ATT_SUMMARY_TUE As String = "attSumTue"
        Const CTRL_PREFIX_ATT_SUMMARY_WED As String = "attSumWed"
        Const CTRL_PREFIX_ATT_SUMMARY_THU As String = "attSumThu"
        Const CTRL_PREFIX_ATT_SUMMARY_FRI As String = "attSumFri"
        Const CTRL_PREFIX_ATT_SUMMARY_SAT As String = "attSumSat"
        Const CTRL_PREFIX_ATT_SUMMARY_SUN As String = "attSumSun"
        Const CTRL_PREFIX_ATT_SUMMARY_UNITS As String = "attSumUnits"
        Const CTRL_PREFIX_ATT_SUMMARY_UNITS_HOURS_MINS As String = "attSumUnitsHoursMins"
        Const CTRL_PREFIX_ATT_SUMMARY_MEASURED_IN As String = "attSumMeasuredIn"
        Const CTRL_PREFIX_ATT_SUMMARY_FREQUENCY As String = "attSumFreq"
        Const CTRL_PREFIX_ATT_SUMMARY_FIRST_WEEK As String = "attSumFirstWeek"
        Const CTRL_PREFIX_ATT_SUMMARY_REMOVED As String = "attSumRemove"

        Const CTRL_PREFIX_ATT_RATE_CAT As String = "attRateCat"
        Const CTRL_PREFIX_ATT_MON As String = "attMon"
        Const CTRL_PREFIX_ATT_TUE As String = "attTue"
        Const CTRL_PREFIX_ATT_WED As String = "attWed"
        Const CTRL_PREFIX_ATT_THU As String = "attThu"
        Const CTRL_PREFIX_ATT_FRI As String = "attFri"
        Const CTRL_PREFIX_ATT_SAT As String = "attSat"
        Const CTRL_PREFIX_ATT_SUN As String = "attSun"
        Const CTRL_PREFIX_ATT_UNITS As String = "attUnits"
        Const CTRL_PREFIX_ATT_UNITS_HOURS_MINS As String = "attUnitsHoursMins"
        Const CTRL_PREFIX_ATT_MEASURED_IN As String = "attMeasuredIn"
        Const CTRL_PREFIX_ATT_FREQUENCY As String = "attFreq"
        Const CTRL_PREFIX_ATT_FIRST_WEEK As String = "attFirstWeek"
        Const CTRL_PREFIX_ATT_REMOVED As String = "attRemove"

        Const CTRL_PREFIX_EXPENDITURE_RATE_CATEGORY As String = "expRateCategory"
        Const CTRL_PREFIX_EXPENDITURE_UNITS As String = "expUnits"
        Const CTRL_PREFIX_EXPENDITURE_MEASURED_IN As String = "expMeasuredIn"
        Const CTRL_PREFIX_EXPENDITURE_UNIT_COST As String = "expUnitCost"
        Const CTRL_PREFIX_EXPENDITURE_OVERRIDDEN As String = "expOverridden"
        Const CTRL_PREFIX_EXPENDITURE_TOTAL_COST As String = "expTotalCost"
        Const CTRL_PREFIX_EXPENDITURE_FINANCE_CODE As String = "expFinanceCode"
        Const CTRL_PREFIX_EXPENDITURE_HID_UNIT_COST As String = "expHidUnitCost"
        Const CTRL_PREFIX_EXPENDITURE_HID_OVERRIDDEN As String = "expHidOverridden"
        Const CTRL_PREFIX_EXPENDITURE_HID_OVERRIDDEN_UNIT_COST As String = "expHidOverriddenUnitCost"
        Const CTRL_PREFIX_EXPENDITURE_HID_OVERRIDDEN_UNITS As String = "expHidOverriddenUnits"
        Const CTRL_PREFIX_EXPENDITURE_OVERRIDE As String = "expOverride"

        Const UNIQUEID_PREFIX_NEW_SUMMARY As String = "sumN"
        Const UNIQUEID_PREFIX_UPDATE_SUMMARY As String = "sumU"
        Const UNIQUEID_PREFIX_DELETE_SUMMARY As String = "sumD"

        Const UNIQUEID_PREFIX_NEW_VISITS As String = "visitN"
        Const UNIQUEID_PREFIX_UPDATE_VISITS As String = "visitU"
        Const UNIQUEID_PREFIX_DELETE_VISITS As String = "visitD"

        Const UNIQUEID_PREFIX_NEW_SUMMARY_ATTENDANCE As String = "attSumN"
        Const UNIQUEID_PREFIX_UPDATE_SUMMARY_ATTENDANCE As String = "attSumU"
        Const UNIQUEID_PREFIX_DELETE_SUMMARY_ATTENDANCE As String = "attSumD"

        Const UNIQUEID_PREFIX_NEW_ATTENDANCE As String = "attN"
        Const UNIQUEID_PREFIX_UPDATE_ATTENDANCE As String = "attU"
        Const UNIQUEID_PREFIX_DELETE_ATTENDANCE As String = "attD"

        Const UNIQUEID_PREFIX_UPDATE_EXPENDITURE As String = "expU"

        'Const DISABLED_CELL_PREFIX As String = "allowDisable_"
#End Region

#Region " Private variables "

        Private _newSummaryIDCounter As Integer, _newVisitIDCounter As Integer
        Private _dsoID As Integer
        Private _estabID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _enableDateToValidator As Boolean = True
        Private _enableEndReasonValidator As Boolean = True
        Private _enableFinanceCode2Validator As Boolean = True
        Private _order As DomServiceOrder = Nothing
        Private _contract As DomContract = Nothing
        Private _rateCategories As DomRateCategoryCollection = Nothing
        Private _serviceTypes As List(Of ViewablePair) = Nothing
        Private _stdBut As StdButtonsBase
        Private _domContractVSTs As DomContractVisitServiceTypeCollection = Nothing
        Private _forceDisableVisits As List(Of TextBoxEx) = Nothing
        Private _uoms As DomUnitsOfMeasureCollection = Nothing
        Private _startup2JS As StringBuilder = New StringBuilder()
        Private _endReasons As DomServiceOrderEndReasonCollection = Nothing
        Private _projectCodes As ProjectCodeCollection = Nothing
        Private _financeCodesOverriden As Boolean
        Private _imgPadlock As HtmlImage = New HtmlImage
        Private _showPadlock As Boolean
        Private _frType As FrameworkType = Nothing
        Private _newAttendanceIDCounter As Integer
        Private _newAttendanceSummaryIDCounter As Integer
        Private _attendanceEditClicked As Boolean
        Private _currentAttendanceEffectiveDate As Date = Date.MinValue
        Private _serviceOrderFundingComplex As TriState = TriState.UseDefault
        Private _serviceOrderFundingPresent As Boolean
        Private _fcr As FcRules.FinanceCodeRulesBL
        Private _expCodeInputs As FcRules.FinanceCodeRulesBL.DomExpenditureCodeInputs
        Private _serviceOrderFundingByServiceType As Hashtable
        Private _AutoGenerateOrderReference As Nullable(Of Boolean)
        Private _canEdit As Boolean, _canAmendAttendance As Boolean
        Private _maintainedExternally As Boolean
        Private _establishment As Establishment = Nothing
        Private _documentsAccessible As Boolean = False
        Private _paymentTolerancesEditable As Boolean
        Private _hasPaymentToleranceGroups As Boolean
        Private _refreshParentWindow As Boolean = False
        Private _weHaveFundingRecords As Boolean
        Private _externalFields As External_FieldCollection = Nothing
        Private _editableFieldsFound As Boolean = False
        Private _editMode As Boolean
        Private _canOverrideUnitCosts As Boolean
#End Region

#Region " Properties "


        Public Property weHaveFundingRecords() As Boolean
            Get
                Return _weHaveFundingRecords
            End Get
            Set(ByVal value As Boolean)
                _weHaveFundingRecords = value
            End Set
        End Property



#End Region

#Region " Page_Load "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            AddHandler _stdBut.AddCustomControls, AddressOf StdButtons_AddCustomControls

            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .hideCreditorRef = True
            End With


            Dim serviceOrderCssPath As String = WebUtils.GetVirtualPath("CustomCss/ServiceOrderworkArroundTab.css")

            If Me.CssLinks.Contains(serviceOrderCssPath) = False Then
                Me.CssLinks.Add(serviceOrderCssPath)

            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryServiceOrders"), "Service Order")
            Me.ShowValidationSummary = True

            Dim msg As ErrorMessage
            Dim sumList As List(Of String), visitList As List(Of String)
            Dim attendanceList As List(Of String)
            Dim attendanceSummaryList As List(Of String)
            Dim genericContractID As Integer
            Dim genContract As GenericContract = New GenericContract(Me.DbConnection)
            Dim selectorTypes As New List(Of SelectorTypes)()

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("id")) > 0 Then
                _dsoID = Utils.ToInt32(Request.QueryString("id"))
            End If
            _estabID = Utils.ToInt32(Request.QueryString("estabID"))
            genericContractID = Utils.ToInt32(Request.QueryString("contractID"))
            If genericContractID > 0 Then
                msg = genContract.Fetch(genericContractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _contractID = genContract.ChildID
            Else
                _contractID = 0
            End If

            _clientID = Utils.ToInt32(Request.QueryString("clientID"))

            _canEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.Edit"))

            _canAmendAttendance = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.CanAmendAttendanceWhenOrderLocked"))

            _canOverrideUnitCosts = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.OverrideProviderUnitCosts"))


            ' setup buttons
            With _stdBut
                .EditableControls.Add(tabHeader.Controls)
                .EditableControls.Add(fundingPanel.Controls)
                .EditableControls.Add(tabSuspensions.Controls)
                .EditableControls.Add(tabSummary.Controls)
                .EditableControls.Add(tabVisits.Controls)
                .EditableControls.Add(tabExpenditure.Controls)
                .EditableControls.Add(tabAttendanceSummary.Controls)
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.AddNew"))
                .ShowNew = False
                .AllowFind = False
                .AllowBack = True
                .AllowEdit = _canEdit
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.Delete"))
                .AuditLogTableNames.Add("DomServiceOrder")
                .AuditLogTableNames.Add("DomServiceOrderDetail")
                .AuditLogTableNames.Add("DomServiceOrderVisit")
                .AuditLogTableNames.Add("DomServiceOrderProviderUnitCostOverride")
                .AuditLogTableNames.Add("DomServiceOrderAttendance")
                .AuditLogTableNames.Add("DomServiceOrderPaymentTolerance")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            ' get order
            If _dsoID > 0 Then
                _order = New DomServiceOrder(Me.DbConnection, String.Empty, String.Empty)
                msg = _order.Fetch(_dsoID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                msg = _contract.Fetch(_order.DomContractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DbConnection, _contract.DomRateFrameworkID, _frType)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                'See if the order should be locked
                msg = ServiceOrderBL.DomServiceOrderMaintainedExternally(Me.DbConnection, _dsoID, _maintainedExternally)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                ' fetch establishment
                _establishment = New Establishment(Me.DbConnection)
                msg = _establishment.Fetch(_order.ProviderID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            ' output javascript
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Date.js"))
            Me.JsLinks.Add("Edit.js")
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            Me.cboEndReason.DropDownList.Attributes.Add("onchange", "cboEndReason_Changed();")
            'Me.btnSvcOrderFunding.Attributes.Add("onclick", "btnSvcOrderFunding_Click();")

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                 String.Format("Edit_domContractID='{0}';InPlaceDomContractSelector_providerID={1};Edit_endReasonID='{2}';Edit_dateToID='{3}';DSO_ID={4};Edit_attPlanID='{5}';dteDatesEffectiveDateID='{6}';edit_EffectiveDateID='{7}';contractID='{8}';Edit_dateFromID='{9}';", _
                  domContract.ClientID, _estabID, cboEndReason.ClientID, dteDateTo.ClientID, _dsoID, divPlan.ClientID, dteDatesEffectiveDate.ClientID, dteEffectiveFrom.ClientID, _contractID, dteDateFrom.ClientID), True)
            End If

            ' re-create the list of summary lines (from view state)
            sumList = GetSummaryUniqueIDsFromViewState()
            _forceDisableVisits = New List(Of TextBoxEx)
            _startup2JS.Length = 0
            For Each id As String In sumList
                OutputSummaryControls(id, Nothing, Not _order.VisitBased)
            Next

            ' re-create the list of attendance Summary (from view state)
            attendanceSummaryList = GetAttendanceSummaryUniqueIDsFromViewState()
            phAttendanceSummary.Controls.Clear()
            For Each id As String In attendanceSummaryList
                OutputAttendanceSummaryControls(id)
            Next

            ' re-create the list of attendance (from view state)
            attendanceList = GetAttendanceUniqueIDsFromViewState()
            phAttendance.Controls.Clear()
            For Each id As String In attendanceList
                OutputAttendanceControls(id, Nothing)
            Next

            '' re-create the list of visits (from view state)
            visitList = GetVisitUniqueIDsFromViewState()
            For Each id As String In visitList
                OutputVisitControls(id, Nothing)
            Next

            ' re-create the list of expenditure (from view state)
            OutputExpenditureControlsFromViewState()

            'set document security 'access' flag
            _documentsAccessible = SecurityBL.UserHasMenuItem(Me.DbConnection, SecurityBL.GetCurrentUser().ID, _
                                                              Target.Library.Web.ConstantsManager.GetConstant( _
                                                              "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"), _
                                                              Settings.CurrentApplicationID)
            'set the payment tolerances 'editable' flag
            SetPaymentTolerancesEditableFlag()

            UseJQuery = True
            UseJqueryUI = True
            UseJqueryTextboxClearer = True
            UseJqueryTooltip = True
            UseExt = True
            ' add the templates pluggin
            UseJqueryTemplates = True

            ' add any selectors for registration
            selectorTypes.Add(Target.Abacus.Library.Selectors.Messages.SelectorTypes.DomServiceOrderProviderUnitCostOverride)

            ' register the selector
            SelectorRegistrationUtility.Register(Page, selectorTypes, False)

            AjaxPro.Utility.RegisterTypeForAjax(GetType(ServiceOrder.ServiceOrderService))

            With txtComment
                .TextBox.TextMode = TextBoxMode.MultiLine
                .TextBox.Rows = 9
                .SetupRegEx()
            End With

            With txtVisitsFilterDate
                .AllowableDays = DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing, Now.Date).DayOfWeek
                If Not _order Is Nothing AndAlso _order.ID > 0 Then
                    .MinimumValue = _order.DateFrom
                    If _order.DateTo <> Utils.MAX_END_DATE Then
                        .MaximumValue = DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing, _order.DateTo)
                    End If
                End If
            End With
        End Sub

#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            ' If _showPadlock Then
            ' if we should show the padlock then do so 

            With _imgPadlock
                .ID = "imgPadlock"
                .Src = WebUtils.GetVirtualPath("Images/PadLock.gif")
            End With
            controls.Add(_imgPadlock)

            'End If

        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA_SUMMARY)
            phSummary.Controls.Clear()
            ViewState.Remove(VIEWSTATE_KEY_DATA_VISITS)
            phVisits.Controls.Clear()
            ViewState.Remove(VIEWSTATE_KEY_DATA_EXPENDITURE)
            phExpenditure.Controls.Clear()
            ViewState.Remove(VIEWSTATE_KEY_DATA_ATTENDANCE)
            phAttendance.Controls.Clear()
            ViewState.Remove(VIEWSTATE_KEY_DATA_ATTENDANCESUMMARY)
            phAttendanceSummary.Controls.Clear()
        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            PopulateDropdowns(0)
            SetupProviderSelector(_estabID)
            SetupContractSelector(_contractID, _estabID)
            SetupClientSelector(_clientID)
            SetupValidators(Nothing)

            If AutoGenerateOrderReference Then

                txtOrderRef.Text = "[Auto-Generated]"

            End If

        End Sub

#End Region

#Region " EditClicked "

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            _editMode = True

            FindClicked(e)


        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim contract As DomContract
            Dim orderDetails As List(Of vwDomServiceOrderDetail) = Nothing
            Dim orderVisits As DomServiceOrderVisitCollection = Nothing
            Dim AttendanceSchedules As vwDomServiceOrderAttendanceCollection = Nothing
            Dim AttSummaryDetails As DomServiceOrderAttendanceCollection = Nothing
            'Dim sumList As List(Of String), visitList As List(Of String)
            'Dim id As String

            PopulateDropdowns(e.ItemID)

            _maintainedExternally = False
            _order = New DomServiceOrder(Me.DbConnection, String.Empty, String.Empty)
            msg = _order.Fetch(e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = _contract.Fetch(_order.DomContractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' cannot edit DSOs if maintained externally
            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = contract.Fetch(_order.DomContractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = ServiceOrderBL.DomServiceOrderMaintainedExternally(Me.DbConnection, _dsoID, _maintainedExternally)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With _order
                SetupProviderSelector(.ProviderID)
                SetupContractSelector(.DomContractID, .ProviderID)
                SetupClientSelector(.ClientID)
                SetupDSOAdditionalDetails()
                SetupValidators(_order)
                'SetupDSOFundingControl(_order.ID)
                txtOrderRef.Text = .OrderReference
                dteDateFrom.Text = .DateFrom
                If .DateTo <> DataUtils.MAX_DATE Then
                    dteDateTo.Text = .DateTo
                    cboEndReason.DropDownList.SelectedValue = .DomServiceOrderEndReasonID
                Else
                    dteDateTo.Text = String.Empty
                End If
                CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode

                CType(careManager, InPlaceCareManagerSelector).CareManagerID = .CareManagerID
                CType(team, InPlaceTeamSelector).TeamID = .TeamID
                CType(clientGroup, InPlaceClientGroupSelector).ClientGroupID = .ClientGroupID
                CType(clientSubGroup, InPlaceClientSubGroupSelector).ClientSubGroupID = .ClientSubGroupID
                SetProjectCode(.ProjectCode)
                txtComment.Text = .Comment
                chkExcludeFromAnomalies.CheckBox.Checked = .AcceptedAssessAnomaly
            End With

            FetchServiceTypes()

            ClearViewState(e)

            Dim frType As FrameworkType = New FrameworkType(Me.DbConnection, String.Empty, String.Empty)
            msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DbConnection, contract.DomRateFrameworkID, frType)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If frType.ID = FrameworkTypes.ElectricMonitoring Or frType.ID = FrameworkTypes.CommunityGeneral Then

                Dim filterdate As Date = DomContractBL.GetWeekEndingDate(Me.DbConnection, _
                                                                         Nothing, _
                                                                         Utils.MaxDate(_order.DateFrom, Date.Now))
                If _order.DateTo <> Utils.MAX_END_DATE Then
                    filterdate = DomContractBL.GetWeekEndingDate(Me.DbConnection, _
                                                                Nothing, _
                                                                Utils.MinDate(_order.DateTo, filterdate))
                End If
                txtVisitsFilterDate.Text = filterdate.ToShortDateString

                msg = populateSummaryVisitsAndExpenditure()
                If Not msg.Success Then WebUtils.DisplayError(msg)

            ElseIf frType.ID = FrameworkTypes.ServiceRegister Then
                '========================================================
                '====================== ATTENDANCE ======================
                '========================================================

                PopulateAttendanceSummaryTab()

                PopulateAttendanceTab()

            End If

            'Populate funding
            PopulateFundingTab(e.ItemID)

            ' populate the expenditure tab
            PopulateExpenditureTab(e.ItemID)

            ' populate order suspensions tab
            PopulateSuspensions(e.ItemID)


        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            ClearViewState(e)
            If e.ItemID = 0 Then
                PopulateDropdowns(0)
                SetupProviderSelector(_estabID)
                SetupContractSelector(_contractID, _estabID)
                SetupClientSelector(_clientID)
                SetupDSOAdditionalDetails()
                SetupValidators(Nothing)
                txtOrderRef.Text = String.Empty
                dteDateFrom.Text = String.Empty
                dteDateTo.Text = String.Empty
                cboEndReason.DropDownList.SelectedValue = String.Empty
                CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
                'CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
                'CType(pct, InPlacePctSelector).PctID = 0
                CType(careManager, InPlaceCareManagerSelector).CareManagerID = 0
                CType(team, InPlaceTeamSelector).TeamID = 0
                CType(clientGroup, InPlaceClientGroupSelector).ClientGroupID = 0
                CType(clientSubGroup, InPlaceClientSubGroupSelector).ClientSubGroupID = 0
                SetProjectCode(String.Empty)
                txtComment.Text = String.Empty
                chkExcludeFromAnomalies.CheckBox.Checked = TriState.False
            Else
                FindClicked(e)
            End If
            _editMode = False
        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = ServiceOrderBL.DeleteServiceOrder(Me.DbConnection, Nothing, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then
                If msg.Number = ServiceOrderBL.ERR_COULD_NOT_DELETE_DSO Or msg.Number = "E0503" Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    FindClicked(e)
                Else
                    WebUtils.DisplayError(msg)
                End If
            Else
                If IsPopupScreen Then
                    ' if this page is displayed in a popup screen
                    ' then flag to refresh the parent window...or data will 
                    ' become out of sync!
                    _refreshParentWindow = True
                End If
                ClearViewState(e)

                Dim backUrl As String = HttpUtility.UrlDecode(Request.QueryString("backUrl"))
                If backUrl <> "" Then
                    Response.Redirect(backUrl)
                End If
            End If
            _editMode = False
        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim orderDetail As DomServiceOrderDetail
            Dim orderDetails As DomServiceOrderDetailCollection = New DomServiceOrderDetailCollection()
            Dim orderVisit As DomServiceOrderVisit
            Dim orderVisits As DomServiceOrderVisitCollection = New DomServiceOrderVisitCollection()
            Dim unitCostOverride As DomServiceOrderProviderUnitCostOverride
            Dim unitCostOverrides As DomServiceOrderProviderUnitCostOverrideCollection = New DomServiceOrderProviderUnitCostOverrideCollection()
            Dim orderAttendance As DomServiceOrderAttendance = Nothing
            Dim orderAttendancies As DomServiceOrderAttendanceCollection = New DomServiceOrderAttendanceCollection
            Dim attendanceToDelete As List(Of String) = Nothing, attendanceList As List(Of String) = Nothing
            Dim sumToDelete As List(Of String) = Nothing, sumList As List(Of String) = Nothing
            Dim visitToDelete As List(Of String) = Nothing, visitList As List(Of String) = Nothing
            Dim expToDelete As List(Of String) = Nothing, expList As List(Of String)
            Dim time As TimePicker
            Dim isNewOrder As Boolean
            Dim options As SaveServiceOrderOptions
            Dim dateTo As String, endReasonID As Integer, dateToFixed As Boolean
            'Dim contract As DomContract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            Dim updatingAttendanceOnly As Boolean = False
            Dim nextServiceOrderIndex As Integer = 0
            Dim deriveFromMatrix As Boolean = False
            Dim hasFundingRecords As Boolean = False
            Dim fundList As DomServiceOrderFundingRevisionCollection = Nothing

            _contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)

            'check if derive from matrix aplication setting present and configured.
            msg = FinanceCodeMatrixBL.GetDeriveFromMatrixApplicationSetting(Me.DbConnection, Nothing, deriveFromMatrix)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' as we are not using viewstate because it fecks other things up, dropdowns need a bit of extra work to pass validation
            PopulateDropdowns(e.ItemID)
            cboEndReason.SelectPostBackValue()
            cboProjectCode.SelectPostBackValue()

            If _maintainedExternally Then
                _showPadlock = True
            ElseIf Not _canEdit AndAlso _canAmendAttendance Then
                _showPadlock = True
            End If

            If e.ItemID > 0 Then
                msg = DomServiceOrderFundingRevision.FetchList(conn:=Me.DbConnection, list:=fundList, domServiceOrderID:=e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                hasFundingRecords = (fundList.Count > 0)
            Else
                hasFundingRecords = False
            End If

            If _showPadlock Then
                '++ One or more fields are available for editing when the service order is locked, so fetch the order
                '++ in its entirety and then update only the specific items tallying with these fields..

                _order = New DomServiceOrder(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                With _order
                    msg = .Fetch(e.ItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    .AmendedBy = currentUser.ExternalUsername
                    .DateAmended = DateTime.Now

                    '++ Update the order with the current (editable) fields..
                    FetchExternalFields()
                    For Each extField As External_Field In _externalFields
                        Select Case extField.ExternalField
                            Case "ProjectCode"
                                SetProjectCode(.ProjectCode)    ' this will insert any redundant items associated with order into list
                                cboProjectCode.SelectPostBackValue()    ' this will select the actual value selected
                                .ProjectCode = IIf(cboProjectCode.DropDownList.SelectedValue.Trim().Length > 0, cboProjectCode.DropDownList.SelectedValue, Nothing)
                            Case "Comment"
                                .Comment = txtComment.GetPostBackValue
                            Case "FinanceCode"
                                If Not hasFundingRecords Then
                                    .FinanceCode = IIf(CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText.Trim().Length > 0, CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText, String.Empty)
                                End If
                            Case "Team"
                                If Not hasFundingRecords Then
                                    .TeamID = Utils.ToInt32(Request.Form(CType(team, InPlaceTeamSelector).HiddenFieldUniqueID))
                                End If
                            Case "ClientGroup"
                                If Not hasFundingRecords Then
                                    .ClientGroupID = Utils.ToInt32(Request.Form(CType(clientGroup, InPlaceClientGroupSelector).HiddenFieldUniqueID))
                                End If
                            Case "ClientSubGroup"
                                If Not hasFundingRecords Then
                                    .ClientSubGroupID = Utils.ToInt32(Request.Form(CType(clientSubGroup, InPlaceClientSubGroupSelector).HiddenFieldUniqueID))
                                End If
                            Case "CareManager"
                                If Not hasFundingRecords Then
                                    .CareManagerID = Utils.ToInt32(Request.Form(CType(careManager, InPlaceCareManagerSelector).HiddenFieldUniqueID))
                                End If
                            Case "AcceptedAssessmentAnomaly"
                                .AcceptedAssessAnomaly = chkExcludeFromAnomalies.CheckBox.Checked
                            Case Else
                        End Select
                    Next

                    msg = _contract.Fetch(.DomContractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With

                '++ Fetch the existing child constructs off this service order..
                msg = DomServiceOrderDetail.FetchList(Me.DbConnection, orderDetails, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _order.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                msg = DomServiceOrderVisit.FetchList(Me.DbConnection, orderVisits, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _order.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                msg = DomServiceOrderProviderUnitCostOverride.FetchList(Me.DbConnection, unitCostOverrides, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _order.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                msg = DomServiceOrderAttendance.FetchList(Me.DbConnection, orderAttendancies, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _order.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                options = New SaveServiceOrderOptions()
                With options
                    .EditingAttendanceOnly = updatingAttendanceOnly
                    .UsingElectronicInterface = False
                    .CanOverrrideProviderUnitCosts = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.OverrideProviderUnitCosts"))
                End With

                sumToDelete = Nothing
                visitToDelete = Nothing
                attendanceToDelete = Nothing

                '++ Save the service order and its (unchanged) child constructs..
                msg = ServiceOrderBL.SaveServiceOrder(Me.DbConnection, _
                                        Nothing, _
                                        _order, _
                                        orderDetails, sumToDelete, _
                                        orderVisits, visitToDelete, _
                                        orderAttendancies, attendanceToDelete, _
                                        options, Me.Settings.CurrentApplicationID)

                If Not msg.Success Then
                    If msg.Number = ServiceOrderBL.ERR_COULD_NOT_SAVE_DSO Or msg.Number = DomContractConvertTimeToUnits.ERR_COULD_NOT_CATEGORISE Then
                        ' could not save DSO or could not categorise visits
                        lblError.Text = msg.Message.Replace(vbCrLf, "<br />")
                        e.Cancel = True
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                Else

                    If deriveFromMatrix And CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText.Trim().Length = 0 Then
                        Dim RaiseWarning As Boolean = False
                        msg = ServiceOrderBL.CreateFundingRecordsFromMatrix(Me.DbConnection, _order, orderDetails, currentUser.ExternalUsername, RaiseWarning)
                        If Not msg.Success Then
                            WebUtils.DisplayError(msg)
                        End If

                        If RaiseWarning Then
                            lblWarning.Text = "No matching entries were found in the finance code matrix for this service order."
                        Else
                            Response.Redirect( _
                                    String.Format("Edit.aspx?id={0}&backUrl={1}&mode=1&autopopup={2}&refreshParent={3}", _
                                                  _dsoID, _
                                                  HttpUtility.UrlEncode(Utils.ToString(Request.QueryString("backUrl"))), _
                                                  Utils.ToInt32(Request.QueryString("autopopup")), _
                                                  IIf(IsPopupScreen, "1", "0") _
                                    ) _
                                )
                        End If
                    End If

                    FindClicked(e)
                    Exit Sub
                End If
            Else
                endReasonID = Utils.ToInt32(cboEndReason.DropDownList.SelectedValue)
                dateTo = dteDateTo.Text
                If endReasonID > 0 Then
                    For Each er As DomServiceOrderEndReason In _endReasons
                        If er.ID = endReasonID AndAlso er.Type = 1 Then
                            ' a "system" end reason has been chosen so DateTo cannot be changed
                            dateToFixed = True
                            Exit For
                        End If
                    Next
                End If

                ' sort out the validators
                SetupValidators(dateTo, cboEndReason.DropDownList.SelectedValue)

                Me.Validate("Save")

                If Me.IsValid Then

                    Dim isCopyServiceOrder As Boolean = False
                    Dim orderToCopyFrom As DomServiceOrder = Nothing

                    ' if we have a copy from id then this is a copy request
                    If CopyFromID.HasValue Then

                        isCopyServiceOrder = True
                        orderToCopyFrom = New DomServiceOrder(Me.DbConnection, String.Empty, String.Empty)
                        msg = orderToCopyFrom.Fetch(CopyFromID.Value)

                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    End If

                    ' get the order
                    _order = New DomServiceOrder(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                    With _order
                        If e.ItemID > 0 Then
                            ' update
                            msg = .Fetch(e.ItemID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            SetProjectCode(.ProjectCode)    ' this will insert any redundant items associated with order into list
                            cboProjectCode.SelectPostBackValue()    ' this will select the actual value selected
                            SetProjectCode(cboProjectCode.DropDownList.SelectedValue)   ' this will select the selected value

                            .AmendedBy = currentUser.ExternalUsername
                            .DateAmended = DateTime.Now

                            msg = _contract.Fetch(.DomContractID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        Else

                            Dim providerID As Integer
                            Dim contractID As Integer

                            ' provider, contract and client are set upon creation and can never be changed
                            isNewOrder = True

                            If isCopyServiceOrder Then

                                contractID = orderToCopyFrom.DomContractID
                                providerID = orderToCopyFrom.ProviderID

                            Else

                                contractID = Utils.ToInt32(Request.Form(CType(domContract, InPlaceDomContractSelector).HiddenFieldUniqueID))
                                providerID = Utils.ToInt32(Request.Form(CType(provider, InPlaceEstablishmentSelector).HiddenFieldUniqueID))

                            End If

                            msg = _contract.Fetch(contractID)

                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            If _contract.DSOsMaintainedViaElectronicInterface Then
                                lblError.Text = "Cannot save this order as orders for the contract specified are maintained via an electronic interface."
                                e.Cancel = True
                                Exit Sub
                            End If

                            .ProviderID = providerID
                            .DomContractID = contractID
                            .ClientID = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))
                            .CreatedBy = currentUser.ExternalUsername
                            .DateCreated = DateTime.Now
                            .NewStyle = TriState.True
                        End If
                        .DateFrom = dteDateFrom.Text
                        .DomServiceOrderEndReasonID = endReasonID
                        If dateToFixed Then
                            dteDateTo.Text = .DateTo
                        ElseIf Utils.IsDate(dateTo) Then
                            .DateTo = dateTo
                        Else
                            .DateTo = DataUtils.MAX_DATE
                        End If
                        .FinanceCode = IIf(CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText.Trim().Length > 0, CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText, String.Empty)
                        '.FinanceCode2 = IIf(CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText.Trim().Length > 0, CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText, String.Empty)

                        '.PrimaryCareTrustID = Utils.ToInt32(Request.Form(CType(pct, InPlacePctSelector).HiddenFieldUniqueID))
                        If Not hasFundingRecords Then
                            .CareManagerID = Utils.ToInt32(Request.Form(CType(careManager, InPlaceCareManagerSelector).HiddenFieldUniqueID))
                            .TeamID = Utils.ToInt32(Request.Form(CType(team, InPlaceTeamSelector).HiddenFieldUniqueID))
                            .ClientGroupID = Utils.ToInt32(Request.Form(CType(clientGroup, InPlaceClientGroupSelector).HiddenFieldUniqueID))
                            .ClientSubGroupID = Utils.ToInt32(Request.Form(CType(clientSubGroup, InPlaceClientSubGroupSelector).HiddenFieldUniqueID))
                        End If
                        .ProjectCode = IIf(cboProjectCode.DropDownList.SelectedValue.Trim().Length > 0, cboProjectCode.DropDownList.SelectedValue, Nothing)
                        .Comment = IIf(txtComment.Text.Trim().Length > 0, txtComment.Text, Nothing)
                        .AcceptedAssessAnomaly = chkExcludeFromAnomalies.CheckBox.Checked

                        msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DbConnection, _contract.DomRateFrameworkID, _frType)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        If _frType.ID = FrameworkTypes.ElectricMonitoring Or _frType.ID = FrameworkTypes.CommunityGeneral Then
                            'orderDetails = Nothing
                            If _order.VisitBased Then
                                ' we only look at visits and construct the summary records from the visits
                                visitToDelete = New List(Of String)
                                visitList = GetVisitUniqueIDsFromViewState()
                                For Each uniqueID As String In visitList
                                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_VISITS) Then
                                        ' we are deleting
                                        visitToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE_VISITS, String.Empty))
                                    Else
                                        ' create the empty visit record
                                        orderVisit = New DomServiceOrderVisit(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                        If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE_VISITS) Then
                                            ' we are updating
                                            msg = orderVisit.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_VISITS, String.Empty)))
                                            If Not msg.Success Then WebUtils.DisplayError(msg)
                                        End If
                                        ' set the visit properties
                                        With orderVisit
                                            .DateFrom = Utils.ToDateTime(CType(phVisits.FindControl(CTRL_PREFIX_VISITS_LINE_DATE_FROM & uniqueID), TextBoxEx).Text)
                                            If Utils.IsDate(CType(phVisits.FindControl(CTRL_PREFIX_VISITS_LINE_DATE_TO & uniqueID), TextBoxEx).Text) Then
                                                .DateTo = Utils.ToDateTime(CType(phVisits.FindControl(CTRL_PREFIX_VISITS_LINE_DATE_TO & uniqueID), TextBoxEx).Text)
                                            Else
                                                .DateTo = Nothing
                                            End If
                                            .DomServiceTypeID = Utils.ToInt32(CType(phVisits.FindControl(CTRL_PREFIX_VISITS_SVC_TYPE & uniqueID), DropDownListEx).GetPostBackValue())
                                            time = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_START_TIME & uniqueID), TimePicker)
                                            .StartTime = DateTime.Parse(time.ToString(DomContractBL.TIME_ONLY_DATE))
                                            time = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_DURATION & uniqueID), TimePicker)
                                            .Duration = (time.Hours * 60) + time.Minutes

                                            If Convert.ToInt32(CType(phVisits.FindControl(CTRL_PREFIX_VISITS_CARERS & uniqueID), TextBoxEx).Text) <= 0 _
                                                        Or .NumberOfCarers > Integer.MaxValue Then
                                                msg = New ErrorMessage()
                                                msg.Number = ServiceOrderBL.ERR_COULD_NOT_SAVE_DSO
                                                msg.Message = String.Format(msg.Message, _
                                                     "<br/>The number of carers is invalid.")
                                                lblError.Text = msg.Message
                                                e.Cancel = True
                                                Exit Sub
                                            End If
                                            .NumberOfCarers = Convert.ToInt32(CType(phVisits.FindControl(CTRL_PREFIX_VISITS_CARERS & uniqueID), TextBoxEx).Text)
                                            .OnMonday = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_MON & uniqueID), CheckBoxEx).CheckBox.Checked
                                            .OnTuesday = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_TUE & uniqueID), CheckBoxEx).CheckBox.Checked
                                            .OnWednesday = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_WED & uniqueID), CheckBoxEx).CheckBox.Checked
                                            .OnThursday = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_THU & uniqueID), CheckBoxEx).CheckBox.Checked
                                            .OnFriday = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_FRI & uniqueID), CheckBoxEx).CheckBox.Checked
                                            .OnSaturday = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_SAT & uniqueID), CheckBoxEx).CheckBox.Checked
                                            .OnSunday = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_SUN & uniqueID), CheckBoxEx).CheckBox.Checked
                                            .Primary = CType(phVisits.FindControl(CTRL_PREFIX_VISITS_PRIMARY & uniqueID), CheckBoxEx).CheckBox.Checked
                                            .Frequency = Utils.ToInt32(CType(phVisits.FindControl(CTRL_PREFIX_VISITS_FREQUENCY & uniqueID), DropDownListEx).GetPostBackValue())
                                            .FirstWeekOfService = 1
                                        End With
                                        ' add to the collection
                                        orderVisits.Add(orderVisit)
                                    End If
                                Next
                            Else
                                ' we are entering the summary records manually
                                sumToDelete = New List(Of String)
                                sumList = GetSummaryUniqueIDsFromViewState()
                                For Each uniqueID As String In sumList
                                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_SUMMARY) Then
                                        ' we are deleting
                                        'sumToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE_SUMMARY, String.Empty))
                                    Else

                                        ' create the empty summary record
                                        orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                                        If ShowDayOfWeekColumn() Then
                                            If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_MON & uniqueID), CheckBoxEx).CheckBox.Checked Then
                                                orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                                If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_MON_DSOD & uniqueID), HtmlInputHidden).Value > 0 Then
                                                    ' we are updating
                                                    msg = orderDetail.Fetch(Convert.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_MON_DSOD & uniqueID), HtmlInputHidden).Value))
                                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                                End If
                                                msg = PopulateDetailFromSummaryViewstate(uniqueID, orderDetail)

                                                orderDetail.DayOfWeek = 1
                                                ' add to the collection
                                                orderDetails.Add(orderDetail)
                                            End If
                                            If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_TUE & uniqueID), CheckBoxEx).CheckBox.Checked Then
                                                orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                                If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_TUE_DSOD & uniqueID), HtmlInputHidden).Value > 0 Then
                                                    ' we are updating
                                                    msg = orderDetail.Fetch(Convert.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_TUE_DSOD & uniqueID), HtmlInputHidden).Value))
                                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                                End If
                                                msg = PopulateDetailFromSummaryViewstate(uniqueID, orderDetail)

                                                orderDetail.DayOfWeek = 2
                                                ' add to the collection
                                                orderDetails.Add(orderDetail)
                                            End If
                                            If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_WED & uniqueID), CheckBoxEx).CheckBox.Checked Then
                                                orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                                If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_WED_DSOD & uniqueID), HtmlInputHidden).Value > 0 Then
                                                    ' we are updating
                                                    msg = orderDetail.Fetch(Convert.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_WED_DSOD & uniqueID), HtmlInputHidden).Value))
                                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                                End If
                                                msg = PopulateDetailFromSummaryViewstate(uniqueID, orderDetail)

                                                orderDetail.DayOfWeek = 3
                                                ' add to the collection
                                                orderDetails.Add(orderDetail)
                                            End If
                                            If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_THU & uniqueID), CheckBoxEx).CheckBox.Checked Then
                                                orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                                If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_THU_DSOD & uniqueID), HtmlInputHidden).Value > 0 Then
                                                    ' we are updating
                                                    msg = orderDetail.Fetch(Convert.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_THU_DSOD & uniqueID), HtmlInputHidden).Value))
                                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                                End If
                                                msg = PopulateDetailFromSummaryViewstate(uniqueID, orderDetail)

                                                orderDetail.DayOfWeek = 4
                                                ' add to the collection
                                                orderDetails.Add(orderDetail)
                                            End If
                                            If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_FRI & uniqueID), CheckBoxEx).CheckBox.Checked Then
                                                orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                                If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_FRI_DSOD & uniqueID), HtmlInputHidden).Value > 0 Then
                                                    ' we are updating
                                                    msg = orderDetail.Fetch(Convert.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_FRI_DSOD & uniqueID), HtmlInputHidden).Value))
                                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                                End If
                                                msg = PopulateDetailFromSummaryViewstate(uniqueID, orderDetail)

                                                orderDetail.DayOfWeek = 5
                                                ' add to the collection
                                                orderDetails.Add(orderDetail)
                                            End If
                                            If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_SAT & uniqueID), CheckBoxEx).CheckBox.Checked Then
                                                orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                                If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_SAT_DSOD & uniqueID), HtmlInputHidden).Value > 0 Then
                                                    ' we are updating
                                                    msg = orderDetail.Fetch(Convert.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_SAT_DSOD & uniqueID), HtmlInputHidden).Value))
                                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                                End If
                                                msg = PopulateDetailFromSummaryViewstate(uniqueID, orderDetail)

                                                orderDetail.DayOfWeek = 6
                                                ' add to the collection
                                                orderDetails.Add(orderDetail)
                                            End If
                                            If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_SUN & uniqueID), CheckBoxEx).CheckBox.Checked Then
                                                orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                                If CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_SUN_DSOD & uniqueID), HtmlInputHidden).Value > 0 Then
                                                    ' we are updating
                                                    msg = orderDetail.Fetch(Convert.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_SUN_DSOD & uniqueID), HtmlInputHidden).Value))
                                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                                End If
                                                msg = PopulateDetailFromSummaryViewstate(uniqueID, orderDetail)

                                                orderDetail.DayOfWeek = 0
                                                ' add to the collection
                                                orderDetails.Add(orderDetail)
                                            End If

                                        Else
                                            orderDetail = New DomServiceOrderDetail(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                            If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE_SUMMARY) Then
                                                ' we are updating
                                                msg = orderDetail.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_SUMMARY, String.Empty)))
                                                If Not msg.Success Then WebUtils.DisplayError(msg)
                                            End If
                                            ' set the summary properties
                                            With orderDetail
                                                Dim hoursCtrl As TextBoxEx = CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_UNITS_HOURS & uniqueID), TextBoxEx)
                                                Dim minsCtrl As TextBoxEx = CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_UNITS_MINUTES & uniqueID), TextBoxEx)

                                                .DateFrom = Utils.ToDateTime(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_LINE_DATE_FROM & uniqueID), TextBoxEx).Text)
                                                If Utils.IsDate(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_LINE_DATE_TO & uniqueID), TextBoxEx).Text) Then
                                                    .DateTo = Utils.ToDateTime(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_LINE_DATE_TO & uniqueID), TextBoxEx).Text)
                                                Else
                                                    .DateTo = Nothing
                                                End If
                                                .DomRateCategoryID = Utils.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_RATE_CATEGORY & uniqueID), DropDownListEx).GetPostBackValue())

                                                ' note that ProviderUnits/Minutes are primed here blindly from the UI
                                                ' the DomContractBL.SaveServiceOrder() method changes these values according to the correct business logic
                                                .ProviderUnits = Utils.ToDecimal(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_UNITS & uniqueID), TextBoxEx).Text)
                                                .ProviderMinutes = (Utils.ToInt32(hoursCtrl.Text) * 60) + Utils.ToInt32(minsCtrl.Text)
                                                If ShowSvcUserMinutesColumn() Then
                                                    .ServiceUserMinutes = Utils.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_SUMINUTES & uniqueID), TextBoxEx).Text)
                                                End If
                                                If ShowVisitsColumn() Then
                                                    .Visits = Utils.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_VISITS & uniqueID), TextBoxEx).Text)
                                                End If
                                                .Frequency = Utils.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_FREQUENCY & uniqueID), DropDownListEx).GetPostBackValue())
                                                .FirstWeekOfService = 1
                                            End With
                                            ' add to the collection
                                            orderDetails.Add(orderDetail)
                                    End If

                                    End If
                                Next
                            End If

                        ElseIf _frType.ID = FrameworkTypes.ServiceRegister Then
                            ' ATTENDANCE RECORDS

                            msg = BuildAttendanceSummaryCollectionsForSave(orderAttendancies, attendanceToDelete)
                            If Not msg.Success Then WebUtils.DisplayError(msg)


                        End If
                        ' provider unit cost overrides
                        expToDelete = New List(Of String)
                        expList = GetExpenditureUniqueIDsFromViewState()
                        For Each uniqueID As String In expList

                            Dim overriddenRateCategoryID As Integer = Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_EXPENDITURE, String.Empty))
                            Dim overridden As Boolean = Convert.ToBoolean(CType(phExpenditure.FindControl(CTRL_PREFIX_EXPENDITURE_HID_OVERRIDDEN & uniqueID), HiddenField).Value)
                            Dim overriddenUnitCost As Decimal = Utils.ToDecimal(CType(phExpenditure.FindControl(CTRL_PREFIX_EXPENDITURE_HID_OVERRIDDEN_UNIT_COST & uniqueID), HiddenField).Value)
                            Dim overrideID As Integer

                            ' find the existing override ID
                            msg = ServiceOrderBL.FindDomServiceOrderProviderUnitCostOverride(Me.DbConnection, Nothing, _order.ID, overriddenRateCategoryID, _order.DateFrom, overrideID, 0)
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            If overridden Then
                                ' set up the override
                                unitCostOverride = New DomServiceOrderProviderUnitCostOverride(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                With unitCostOverride
                                    If overrideID > 0 Then
                                        ' update
                                        msg = .Fetch(overrideID)
                                        If Not msg.Success Then WebUtils.DisplayError(msg)
                                    Else
                                        .DomServiceOrderID = _order.ID
                                        .DomRateCategoryID = overriddenRateCategoryID
                                    End If
                                    .UnitCost = overriddenUnitCost

                                    .DateFrom = _order.DateFrom
                                    .DateTo = _order.DateTo
                                    .OverriddenBy = currentUser.ExternalUsername
                                    .OverriddenOn = DateTime.Now
                                End With
                                unitCostOverrides.Add(unitCostOverride)
                            ElseIf overrideID > 0 Then
                                ' mark for deletion
                                expToDelete.Add(overrideID)
                            End If
                        Next

                        ' validate/save the order
                        options = New SaveServiceOrderOptions()
                        With options
                            .EditingAttendanceOnly = updatingAttendanceOnly
                            .UsingElectronicInterface = False
                            .CanOverrrideProviderUnitCosts = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.OverrideProviderUnitCosts"))
                        End With

                        If AutoGenerateOrderReference = False Then
                            ' if we don't want to auto generate order refs then always 
                            ' use the value of the txtOrderRef text box

                            _order.OrderReference = txtOrderRef.Text

                        ElseIf isNewOrder AndAlso AutoGenerateOrderReference Then
                            ' if this is a new order and we want to auto generate the order ref

                            nextServiceOrderIndex = _contract.LastServiceOrderNo + 1
                            _order.OrderReference = String.Format("{0}-SO-{1}", _
                                                           _contract.Number, _
                                                           nextServiceOrderIndex.ToString("00000"))


                        End If

                        ' if needs copying
                        If isCopyServiceOrder Then

                            msg = ServiceOrderBL.CopyServiceOrder(Me.DbConnection, _
                                                                 _order, _
                                                                orderToCopyFrom.ID, _
                                                                Me.Settings.CurrentApplicationID, _
                                                                options)

                        Else
                            ' else this a fresh, not copied, service order

                            msg = ServiceOrderBL.SaveServiceOrder(Me.DbConnection, _
                                                                  Nothing, _
                                                                 _order, _
                                                                orderDetails, _
                                                                sumToDelete, _
                                                                orderVisits, _
                                                                visitToDelete, _
                                                             orderAttendancies, attendanceToDelete, _
                                                             options, Me.Settings.CurrentApplicationID)

                        End If

                        If Not msg.Success Then
                            If msg.Number = ServiceOrderBL.ERR_COULD_NOT_SAVE_DSO Or msg.Number = DomContractConvertTimeToUnits.ERR_COULD_NOT_CATEGORISE Then
                                ' could not save DSO or could not categorise visits
                                lblError.Text = msg.Message.Replace(vbCrLf, "<br />")
                                e.Cancel = True
                            Else
                                WebUtils.DisplayError(msg)
                            End If
                        Else
                            If msg.Message <> String.Empty Then
                                lblWarning.Text = msg.Message
                            End If
                            e.ItemID = .ID
                            _dsoID = .ID

                            If deriveFromMatrix And CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText.Trim().Length = 0 Then
                                Dim RaiseWarning As Boolean = False
                                msg = ServiceOrderBL.CreateFundingRecordsFromMatrix(Me.DbConnection, _order, orderDetails, currentUser.ExternalUsername, RaiseWarning)
                                If Not msg.Success Then
                                    WebUtils.DisplayError(msg)
                                End If

                                If RaiseWarning Then
                                    lblWarning.Text = "No matching entries were found in the finance code matrix for this service order."
                                Else
                                    Response.Redirect( _
                                    String.Format("Edit.aspx?id={0}&backUrl={1}&mode=1&autopopup={2}&refreshParent={3}", _
                                                  _dsoID, _
                                                  HttpUtility.UrlEncode(Utils.ToString(Request.QueryString("backUrl"))), _
                                                  Utils.ToInt32(Request.QueryString("autopopup")), _
                                                  IIf(IsPopupScreen, "1", "0") _
                                    ) _
                                )
                                End If
                            End If
                            ' for new orders, redirect
                            If isNewOrder Then
                                If nextServiceOrderIndex > 0 AndAlso AutoGenerateOrderReference Then
                                    ' if we are auto generating order refs and have allocated
                                    ' the next index then update contract

                                    _contract.LastServiceOrderNo = nextServiceOrderIndex
                                    msg = _contract.Save()
                                    If Not msg.Success Then WebUtils.DisplayError(msg)

                                End If
                                Response.Redirect( _
                                    String.Format("Edit.aspx?id={0}&backUrl={1}&mode=1&autopopup={2}&refreshParent={3}", _
                                                  _dsoID, _
                                                  HttpUtility.UrlEncode(Utils.ToString(Request.QueryString("backUrl"))), _
                                                  Utils.ToInt32(Request.QueryString("autopopup")), _
                                                  IIf(IsPopupScreen, "1", "0") _
                                    ) _
                                )
                            Else
                                ' for visit based plans, re-populate the list of summary records (standard line only)
                                'If _order.VisitBased Then
                                If _frType.ID = FrameworkTypes.ElectricMonitoring Then
                                    populateSummaryVisitsAndExpenditure()
                                End If
                                ' the following selectors are disabled so aren't persisted in the postback and need to be reset
                                With _order
                                    SetupProviderSelector(.ProviderID)
                                    SetupContractSelector(.DomContractID, .ProviderID)
                                    SetupClientSelector(.ClientID)
                                    SetupDSOAdditionalDetails()
                                    txtOrderRef.Text = .OrderReference
                                End With
                                If IsPopupScreen Then
                                    ' if this page is displayed in a popup screen
                                    ' then flag to refresh the parent window...or data will 
                                    ' become out of sync!
                                    _refreshParentWindow = True
                                End If
                            End If
                            _editMode = False
                        End If

                    End With
                Else
                    e.Cancel = True
                End If
            End If

            If _frType.ID = FrameworkTypes.ServiceRegister Then
                PopulateAfterPostback()
                PopulateAttendanceTab()
            Else
                'populate Funding
                PopulateFundingTab(e.ItemID)

                ' populate the expenditure tab
                PopulateExpenditureTab(e.ItemID)
                ' populate order suspensions tab
                PopulateSuspensions(e.ItemID)
            End If
        End Sub

#End Region

        Private Function PopulateDetailFromSummaryViewstate(ByVal uniqueID As String, ByRef orderDetail As DomServiceOrderDetail) As ErrorMessage
            Dim msg As ErrorMessage

            ' set the summary properties
            With orderDetail
                Dim hoursCtrl As TextBoxEx = CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_UNITS_HOURS & uniqueID), TextBoxEx)
                Dim minsCtrl As TextBoxEx = CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_UNITS_MINUTES & uniqueID), TextBoxEx)

                .DateFrom = Utils.ToDateTime(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_LINE_DATE_FROM & uniqueID), TextBoxEx).Text)
                If Utils.IsDate(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_LINE_DATE_TO & uniqueID), TextBoxEx).Text) Then
                    .DateTo = Utils.ToDateTime(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_LINE_DATE_TO & uniqueID), TextBoxEx).Text)
                Else
                    .DateTo = Nothing
                End If
                .DomRateCategoryID = Utils.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_RATE_CATEGORY & uniqueID), DropDownListEx).GetPostBackValue())

                ' note that ProviderUnits/Minutes are primed here blindly from the UI
                ' the DomContractBL.SaveServiceOrder() method changes these values according to the correct business logic
                .ProviderUnits = Utils.ToDecimal(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_UNITS & uniqueID), TextBoxEx).Text)
                .ProviderMinutes = (Utils.ToInt32(hoursCtrl.Text) * 60) + Utils.ToInt32(minsCtrl.Text)
                If ShowSvcUserMinutesColumn() Then
                    .ServiceUserMinutes = Utils.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_SUMINUTES & uniqueID), TextBoxEx).Text)
                End If
                If ShowVisitsColumn() Then
                    .Visits = Utils.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_VISITS & uniqueID), TextBoxEx).Text)
                End If
                .Frequency = Utils.ToInt32(CType(phSummary.FindControl(CTRL_PREFIX_SUMMARY_FREQUENCY & uniqueID), DropDownListEx).GetPostBackValue())
                .FirstWeekOfService = 1
            End With


            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function


#Region " btnAddSummary_Click "

        Private Sub btnAddSummary_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSummary.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetSummaryUniqueIDsFromViewState()
            Dim newOrderDetail As DomServiceOrderSummary = New DomServiceOrderSummary 'DomServiceOrderDetail(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If _order Is Nothing Then
                PopulateDropdowns(0)
            Else
                PopulateDropdowns(_order.ID)
            End If

            cboEndReason.SelectPostBackValue()
            cboProjectCode.SelectPostBackValue()

            'With _order
            '    SetupProviderSelector(.ProviderID)
            '    SetupContractSelector(.DomContractID, .ProviderID)
            '    SetupClientSelector(.ClientID)
            'End With
            'SetupValidators(_order)
            PopulateAfterPostback()

            ' add a new row to the summary list
            id = GetSummaryUniqueID(newOrderDetail)
            ' create the controls
            OutputSummaryControls(id, newOrderDetail, True)
            ' persist the data into view state
            list.Add(id)
            PersistSummaryUniqueIDsToViewState(list)

            tabExpenditure.Enabled = False

        End Sub

#End Region

#Region " btnRemoveSummary_Click "

        Private Sub btnRemoveSummary_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetSummaryUniqueIDsFromViewState()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_SUMMARY_REMOVED, String.Empty)

            If _order Is Nothing Then
                PopulateDropdowns(0)
            Else
                PopulateDropdowns(_order.ID)
            End If
            cboEndReason.SelectPostBackValue()
            cboProjectCode.SelectPostBackValue()
            'With _order
            '    SetupProviderSelector(.ProviderID)
            '    SetupContractSelector(.DomContractID, .ProviderID)
            '    SetupClientSelector(.ClientID)
            'End With
            'SetupValidators(_order)
            PopulateAfterPostback()

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_SUMMARY) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_SUMMARY, UNIQUEID_PREFIX_DELETE_SUMMARY)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phSummary.Controls.Count - 1
                If phSummary.Controls(index).ID = id Then
                    phSummary.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistSummaryUniqueIDsToViewState(list)

            tabExpenditure.Enabled = False

        End Sub

#End Region

#Region " btnAddVisit_Click "

        Private Sub btnAddVisit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddVisit.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetVisitUniqueIDsFromViewState()
            Dim newVisit As DomServiceOrderVisit = New DomServiceOrderVisit(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If _order Is Nothing Then
                PopulateDropdowns(0)
            Else
                PopulateDropdowns(_order.ID)
            End If
            cboEndReason.SelectPostBackValue()
            cboProjectCode.SelectPostBackValue()
            'With _order
            '    SetupProviderSelector(.ProviderID)
            '    SetupContractSelector(.DomContractID, .ProviderID)
            '    SetupClientSelector(.ClientID)
            'End With
            'SetupValidators(_order)
            PopulateAfterPostback()

            ' add a new row to the visit list
            id = GetVisitUniqueID(newVisit)
            ' create the controls
            OutputVisitControls(id, newVisit)
            ' persist the data into view state
            list.Add(id)
            PersistVisitUniqueIDsToViewState(list)

            tabExpenditure.Enabled = False

        End Sub

#End Region

#Region " btnRemoveVisit_Click "

        Private Sub btnRemoveVisit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetVisitUniqueIDsFromViewState()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_VISITS_REMOVED, String.Empty)

            If _order Is Nothing Then
                PopulateDropdowns(0)
            Else
                PopulateDropdowns(_order.ID)
            End If
            cboEndReason.SelectPostBackValue()
            cboProjectCode.SelectPostBackValue()
            'With _order
            '    SetupProviderSelector(.ProviderID)
            '    SetupContractSelector(.DomContractID, .ProviderID)
            '    SetupClientSelector(.ClientID)
            'End With
            'SetupValidators(_order)
            PopulateAfterPostback()

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_VISITS) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_VISITS, UNIQUEID_PREFIX_DELETE_VISITS)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phVisits.Controls.Count - 1
                If phVisits.Controls(index).ID = id Then
                    phVisits.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistVisitUniqueIDsToViewState(list)

            tabExpenditure.Enabled = False

        End Sub

#End Region

#Region " OutputSummaryControls "

        'Private Sub OutputSummaryControls(ByVal uniqueID As String, Optional ByVal orderDetail As vwDomServiceOrderDetail = Nothing, Optional ByVal required As Boolean = False)

        '    Dim row As HtmlTableRow
        '    Dim cell As HtmlTableCell
        '    Dim cboRateCategory As DropDownListEx
        '    Dim cboDayOfWeek As DropDownListEx
        '    Dim txtUnits As TextBoxEx
        '    Dim txtUnitsHours As TextBoxEx
        '    Dim txtUnitsMins As TextBoxEx
        '    Dim lblUnitsHours As Label
        '    Dim lblMeasuredIn As Label
        '    Dim txtSuMinutes As TextBoxEx
        '    Dim txtVisits As TextBoxEx
        '    Dim cboFrequency As DropDownListEx ', cboFirstWeek As DropDownListEx
        '    Dim btnRemove As HtmlInputImage
        '    Dim rateCategory As DomRateCategory
        '    Dim rateCategoryVisitBased As Boolean
        '    Dim uom As DomUnitsOfMeasure
        '    Dim measuredInDesc As String = String.Empty
        '    Dim measuredInComment As String = String.Empty
        '    Dim span As HtmlGenericControl
        '    Dim durationHours As Integer, durationMinutes As Integer
        '    Dim rateCat As DomRateCategory = New DomRateCategory(Me.DbConnection, String.Empty, String.Empty)
        '    Dim rateFram As New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
        '    Dim framType As New FrameworkType(Me.DbConnection, String.Empty, String.Empty)
        '    Dim msg As New ErrorMessage()
        '    Dim txtLineDateFrom As TextBoxEx
        '    Dim txtLineDateTo As TextBoxEx

        '    ' don't output items marked as deleted
        '    If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_SUMMARY) Then

        '        row = New HtmlTableRow()
        '        row.ID = uniqueID
        '        phSummary.Controls.Add(row)

        '        'Line Date From
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        txtLineDateFrom = New TextBoxEx()
        '        With txtLineDateFrom
        '            .ID = CTRL_PREFIX_SUMMARY_LINE_DATE_FROM & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.DateFormatJquery
        '            .Width = New Unit(6, UnitType.Em)
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '            End If
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .Text = orderDetail.DateFrom
        '            Else
        '                .Text = _order.DateFrom
        '            End If

        '        End With
        '        span = New HtmlGenericControl("span")
        '        span.Controls.Add(txtLineDateFrom)
        '        cell.Controls.Add(span)

        '        'Line Date To
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        txtLineDateTo = New TextBoxEx()
        '        With txtLineDateTo
        '            .ID = CTRL_PREFIX_SUMMARY_LINE_DATE_TO & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.DateFormatJquery
        '            .Width = New Unit(6, UnitType.Em)
        '            .Required = False
        '            'If .Required Then
        '            '    .RequiredValidatorErrMsg = "* Required"
        '            '    .ValidationGroup = "Save"
        '            'End If
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                If orderDetail.DateTo <> Utils.MAX_END_DATE Then
        '                    .Text = orderDetail.DateTo
        '                End If
        '            Else
        '                If _order.DateTo <> Utils.MAX_END_DATE Then
        '                    .Text = _order.DateTo
        '                End If
        '            End If
        '            .AllowClear = True
        '        End With
        '        span = New HtmlGenericControl("span")
        '        span.Controls.Add(txtLineDateTo)
        '        cell.Controls.Add(span)

        '        If txtLineDateTo.Text <> String.Empty Then
        '            txtLineDateFrom.MaximumValue = txtLineDateTo.Text
        '        End If
        '        If txtLineDateFrom.Text <> String.Empty Then
        '            txtLineDateTo.MinimumValue = txtLineDateFrom.Text
        '        End If

        '        ' rate category
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        cboRateCategory = New DropDownListEx()
        '        With cboRateCategory
        '            .ID = CTRL_PREFIX_SUMMARY_RATE_CATEGORY & uniqueID
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '            End If
        '            LoadRateCategoryDropdown(cboRateCategory)
        '            If Not orderDetail Is Nothing Then .DropDownList.SelectedValue = orderDetail.DomRateCategoryID
        '        End With
        '        cell.Controls.Add(cboRateCategory)
        '        cboRateCategory.DropDownList.Attributes.Add("onchange", String.Format("cboRateCategory_Change('{0}');", cboRateCategory.ClientID))
        '        _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboRateCategory_Change('{0}');}}", cboRateCategory.ClientID)

        '        If Not orderDetail Is Nothing Then
        '            rateCategory = DomContractBL.FindRateCategory(_rateCategories, orderDetail.DomRateCategoryID)
        '            If Not rateCategory Is Nothing Then
        '                uom = DomContractBL.FindUnitOfMeasure(_uoms, rateCategory.DomUnitsOfMeasureID)
        '                If Not uom Is Nothing Then
        '                    measuredInDesc = uom.Description
        '                    measuredInComment = uom.Comment
        '                End If
        '                rateCategoryVisitBased = (uom.MinutesPerUnit <> 0)
        '            End If
        '        End If

        '        ' day of week
        '        If ShowDayOfWeekColumn() Then
        '            cell = New HtmlTableCell()
        '            row.Controls.Add(cell)
        '            cell.Style.Add("vertical-align", "top")
        '            cboDayOfWeek = New DropDownListEx()
        '            With cboDayOfWeek
        '                .ID = CTRL_PREFIX_SUMMARY_DOW & uniqueID
        '                .Required = required
        '                If .Required Then
        '                    .RequiredValidatorErrMsg = "* Required"
        '                    .ValidationGroup = "Save"
        '                End If
        '                LoadDayOfWeekDropdown(cboDayOfWeek)
        '                If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then .DropDownList.SelectedValue = orderDetail.DayOfWeek
        '            End With
        '            cell.Controls.Add(cboDayOfWeek)
        '        End If

        '        ' units
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        txtUnits = New TextBoxEx()
        '        With txtUnits
        '            .ID = CTRL_PREFIX_SUMMARY_UNITS & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
        '            .Width = New Unit(3, UnitType.Em)
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '            End If
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then .Text = orderDetail.ProviderUnits.ToString("F")

        '        End With
        '        span = New HtmlGenericControl("span")
        '        span.Controls.Add(txtUnits)
        '        cell.Controls.Add(span)

        '        ' calculate the hours based on minutes
        '        If Not orderDetail Is Nothing Then
        '            durationHours = Math.DivRem(orderDetail.ProviderMinutes, 60, durationMinutes)
        '        End If

        '        ' hours control
        '        txtUnitsHours = New TextBoxEx()
        '        With txtUnitsHours
        '            .ID = CTRL_PREFIX_SUMMARY_UNITS_HOURS & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
        '            .OutputBrAfter = False
        '            .Width = New Unit(2.5, UnitType.Em)
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '                .MinimumValue = 0
        '                .MaximumValue = 9999
        '            End If
        '            .TextBox.MaxLength = 4
        '            .Text = "0"
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .Text = durationHours
        '            End If
        '        End With
        '        span = New HtmlGenericControl("span")
        '        span.Controls.Add(txtUnitsHours)
        '        cell.Controls.Add(span)

        '        ' label to separate hours and mins
        '        lblUnitsHours = New Label()
        '        lblUnitsHours.Text = ":"
        '        span.Controls.Add(lblUnitsHours)

        '        ' minutes control
        '        txtUnitsMins = New TextBoxEx()
        '        With txtUnitsMins
        '            .ID = CTRL_PREFIX_SUMMARY_UNITS_MINUTES & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
        '            .OutputBrAfter = False
        '            .Width = New Unit(1.5, UnitType.Em)
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '                .MinimumValue = 0
        '                .MaximumValue = 59
        '            End If
        '            .TextBox.MaxLength = 2
        '            .Text = "00"
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .Text = durationMinutes.ToString.PadLeft(2, "0")
        '            End If
        '        End With
        '        span.Controls.Add(txtUnitsMins)
        '        cell.Controls.Add(span)

        '        ' measured in
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        lblMeasuredIn = New Label()
        '        With lblMeasuredIn
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .Text = measuredInDesc
        '                .ToolTip = measuredInComment
        '            End If
        '        End With
        '        cell.Controls.Add(lblMeasuredIn)

        '        ' su minutes
        '        If ShowSvcUserMinutesColumn() Then

        '            cell = New HtmlTableCell()
        '            row.Controls.Add(cell)
        '            cell.Style.Add("vertical-align", "top")
        '            txtSuMinutes = New TextBoxEx()
        '            With txtSuMinutes
        '                .ID = CTRL_PREFIX_SUMMARY_SUMINUTES & uniqueID
        '                .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
        '                .Width = New Unit(3, UnitType.Em)
        '                .Required = required
        '                If .Required Then
        '                    .RequiredValidatorErrMsg = "* Required"
        '                    .ValidationGroup = "Save"
        '                End If
        '                If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then .Text = orderDetail.ServiceUserMinutes
        '            End With
        '            cell.Controls.Add(txtSuMinutes)
        '            txtUnitsHours.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", txtSuMinutes.ClientID, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
        '            txtUnitsMins.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", txtSuMinutes.ClientID, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
        '            txtUnits.TextBox.Attributes.Add("onblur", String.Format("txtUnits_Change('{0}_txtTextBox', '{1}', '{2}_txtTextBox');", txtUnits.ClientID, cboRateCategory.ClientID, txtSuMinutes.ClientID))
        '        Else
        '            txtUnitsHours.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", String.Empty, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
        '            txtUnitsMins.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", String.Empty, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
        '            txtUnits.TextBox.Attributes.Add("onblur", String.Format("txtUnits_Change('{0}_txtTextBox', '{1}', '{2}_txtTextBox');", txtUnits.ClientID, cboRateCategory.ClientID, String.Empty))
        '        End If

        '        ' visits
        '        If ShowVisitsColumn() Then
        '            cell = New HtmlTableCell()
        '            row.Controls.Add(cell)
        '            cell.Style.Add("vertical-align", "top")
        '            txtVisits = New TextBoxEx()
        '            With txtVisits
        '                .ID = CTRL_PREFIX_SUMMARY_VISITS & uniqueID
        '                .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
        '                .Width = New Unit(3, UnitType.Em)
        '                .Required = required
        '                If .Required Then
        '                    .RequiredValidatorErrMsg = "* Required"
        '                    .ValidationGroup = "Save"
        '                End If
        '                If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then .Text = orderDetail.Visits Else .Text = "0"
        '                If Not rateCategoryVisitBased Then
        '                    _forceDisableVisits.Add(txtVisits)
        '                    .Text = String.Empty
        '                End If
        '            End With
        '            cell.Controls.Add(txtVisits)
        '        End If

        '        ' frequency
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        cboFrequency = New DropDownListEx()
        '        With cboFrequency
        '            .ID = CTRL_PREFIX_SUMMARY_FREQUENCY & uniqueID
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '            End If
        '            LoadFrequencyDropdown(cboFrequency)
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .DropDownList.SelectedValue = orderDetail.Frequency
        '            Else
        '                .DropDownList.SelectedValue = "0"
        '            End If
        '        End With
        '        cell.Controls.Add(cboFrequency)
        '        cboFrequency.DropDownList.Attributes.Add("onchange", String.Format("cboFrequency_Change('{0}');", cboFrequency.ClientID))
        '        _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboFrequency_Change('{0}');}}", cboFrequency.ClientID)

        '        ' remove button
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "middle")
        '        btnRemove = New HtmlInputImage()
        '        With btnRemove
        '            .ID = CTRL_PREFIX_SUMMARY_REMOVED & uniqueID
        '            .Src = WebUtils.GetVirtualPath("images/delete.png")
        '            .Attributes("title") = "Remove this line"
        '            .ValidationGroup = "RemoveSummary"
        '            AddHandler .ServerClick, AddressOf btnRemoveSummary_Click
        '        End With
        '        cell.Controls.Add(btnRemove)

        '    End If

        'End Sub

#End Region

#Region " OutputSummaryControls "

        Private Sub OutputSummaryControls(ByVal uniqueID As String, ByVal orderSummary As DomServiceOrderSummary, ByVal required As Boolean)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim cboRateCategory As DropDownListEx
            Dim txtUnits As TextBoxEx
            Dim txtUnitsHours As TextBoxEx
            Dim txtUnitsMins As TextBoxEx
            Dim lblUnitsHours As Label
            Dim lblMeasuredIn As Label
            Dim txtSuMinutes As TextBoxEx
            Dim txtVisits As TextBoxEx
            Dim cboFrequency As DropDownListEx
            Dim btnRemove As HtmlInputImage
            Dim rateCategory As DomRateCategory
            Dim rateCategoryVisitBased As Boolean
            Dim uom As DomUnitsOfMeasure
            Dim measuredInDesc As String = String.Empty
            Dim measuredInComment As String = String.Empty
            Dim span As HtmlGenericControl
            Dim durationHours As Integer, durationMinutes As Integer
            Dim rateCat As DomRateCategory = New DomRateCategory(Me.DbConnection, String.Empty, String.Empty)
            Dim rateFram As New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
            Dim framType As New FrameworkType(Me.DbConnection, String.Empty, String.Empty)
            Dim msg As New ErrorMessage()
            Dim txtLineDateFrom As TextBoxEx
            Dim txtLineDateTo As TextBoxEx
            Dim chkAllDays As CheckBoxEx
            Dim chkMon As CheckBoxEx, chkTue As CheckBoxEx, chkWed As CheckBoxEx, chkThu As CheckBoxEx
            Dim chkFri As CheckBoxEx, chkSat As CheckBoxEx, chkSun As CheckBoxEx
            Dim chkdsodMon As HtmlInputHidden, chkdsodTue As HtmlInputHidden, chkdsodWed As HtmlInputHidden
            Dim chkdsodThu As HtmlInputHidden, chkdsodFri As HtmlInputHidden, chkdsodSat As HtmlInputHidden, chkdsodSun As HtmlInputHidden

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_SUMMARY) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phSummary.Controls.Add(row)

                'Line Date From
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                txtLineDateFrom = New TextBoxEx()
                With txtLineDateFrom
                    .ID = CTRL_PREFIX_SUMMARY_LINE_DATE_FROM & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.DateFormatJquery
                    .Width = New Unit(6, UnitType.Em)
                    .Required = required
                    If .Required Then
                        .RequiredValidatorErrMsg = "* Required"
                        .ValidationGroup = "Save"
                    End If
                    If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                        .Text = orderSummary.DateFrom
                    Else
                        .Text = _order.DateFrom
                    End If


                End With
                span = New HtmlGenericControl("span")
                span.Controls.Add(txtLineDateFrom)
                cell.Controls.Add(span)

                'Line Date To
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                txtLineDateTo = New TextBoxEx()
                With txtLineDateTo
                    .ID = CTRL_PREFIX_SUMMARY_LINE_DATE_TO & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.DateFormatJquery
                    .Width = New Unit(6, UnitType.Em)
                    .Required = False

                    If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                        If orderSummary.DateTo <> Utils.MAX_END_DATE Then
                            .Text = orderSummary.DateTo
                        End If
                    Else
                        If _order.DateTo <> Utils.MAX_END_DATE Then
                            .Text = _order.DateTo
                        End If
                    End If
                    .AllowClear = True
                End With
                span = New HtmlGenericControl("span")
                span.Controls.Add(txtLineDateTo)
                cell.Controls.Add(span)

                If txtLineDateTo.Text <> String.Empty Then
                    txtLineDateFrom.MaximumValue = txtLineDateTo.Text
                End If
                If txtLineDateFrom.Text <> String.Empty Then
                    txtLineDateTo.MinimumValue = txtLineDateFrom.Text
                End If

                ' rate category
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboRateCategory = New DropDownListEx()
                With cboRateCategory
                    .ID = CTRL_PREFIX_SUMMARY_RATE_CATEGORY & uniqueID
                    .Required = required
                    If .Required Then
                        .RequiredValidatorErrMsg = "* Required"
                        .ValidationGroup = "Save"
                    End If
                    LoadRateCategoryDropdown(cboRateCategory)
                    If Not orderSummary Is Nothing Then .DropDownList.SelectedValue = orderSummary.RateCategoryID
                End With
                cell.Controls.Add(cboRateCategory)
                cboRateCategory.DropDownList.Attributes.Add("onchange", String.Format("cboRateCategory_Change('{0}');", cboRateCategory.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboRateCategory_Change('{0}');}}", cboRateCategory.ClientID)

                If Not orderSummary Is Nothing Then
                    rateCategory = DomContractBL.FindRateCategory(_rateCategories, orderSummary.RateCategoryID)
                    If Not rateCategory Is Nothing Then
                        uom = DomContractBL.FindUnitOfMeasure(_uoms, rateCategory.DomUnitsOfMeasureID)
                        If Not uom Is Nothing Then
                            measuredInDesc = uom.Description
                            measuredInComment = uom.Comment
                        End If
                        rateCategoryVisitBased = (uom.MinutesPerUnit <> 0)
                    End If
                End If

                ' day of week
                If ShowDayOfWeekColumn() Then

                    ' days
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")

                    ' All Days
                    chkAllDays = New CheckBoxEx()
                    With chkAllDays
                        .ID = CTRL_PREFIX_SUMMARY_ALLDAYS & uniqueID
                        .CheckBox.Text = "All"
                        .CheckBox.TextAlign = TextAlign.Right

                        'If all days are already ticked then tick all days too
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                            If orderSummary.OnMonday > 0 And orderSummary.OnTuesday > 0 And _
                               orderSummary.OnWednesday > 0 And orderSummary.OnThursday > 0 And _
                               orderSummary.OnFriday > 0 And orderSummary.OnSaturday > 0 And _
                               orderSummary.OnSunday > 0 Then

                                chkAllDays.CheckBox.Checked = True

                            Else

                                chkAllDays.CheckBox.Checked = False

                            End If
                        End If

                    End With

                    cell.Controls.Add(chkAllDays)
                    cell.Controls.Add(CreateVisitDaysSpacer())

                    ' mon
                    chkMon = New CheckBoxEx()
                    With chkMon
                        .ID = CTRL_PREFIX_SUMMARY_MON & uniqueID
                        .CheckBox.Text = DayOfWeek.Monday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .CheckBox.Checked = (orderSummary.OnMonday > 0)
                    End With

                    chkdsodMon = New HtmlInputHidden
                    With chkdsodMon
                        .ID = CTRL_PREFIX_SUMMARY_MON_DSOD & uniqueID
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                            .Value = orderSummary.OnMonday
                        Else
                            .Value = 0
                        End If

                    End With

                    cell.Controls.Add(chkdsodMon)

                    cell.Controls.Add(chkMon)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' tue
                    chkTue = New CheckBoxEx()
                    With chkTue
                        .ID = CTRL_PREFIX_SUMMARY_TUE & uniqueID
                        .CheckBox.Text = DayOfWeek.Tuesday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .CheckBox.Checked = (orderSummary.OnTuesday > 0)
                    End With

                    chkdsodTue = New HtmlInputHidden
                    With chkdsodTue
                        .ID = CTRL_PREFIX_SUMMARY_TUE_DSOD & uniqueID
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                            .Value = orderSummary.OnTuesday
                        Else
                            .Value = 0
                        End If
                    End With

                    cell.Controls.Add(chkdsodTue)

                    cell.Controls.Add(chkTue)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' wed
                    chkWed = New CheckBoxEx()
                    With chkWed
                        .ID = CTRL_PREFIX_SUMMARY_WED & uniqueID
                        .CheckBox.Text = DayOfWeek.Wednesday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .CheckBox.Checked = (orderSummary.OnWednesday > 0)
                    End With

                    chkdsodWed = New HtmlInputHidden
                    With chkdsodWed
                        .ID = CTRL_PREFIX_SUMMARY_WED_DSOD & uniqueID
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                            .Value = orderSummary.OnWednesday
                        Else
                            .Value = 0
                        End If
                    End With

                    cell.Controls.Add(chkdsodWed)

                    cell.Controls.Add(chkWed)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' thu
                    chkThu = New CheckBoxEx()
                    With chkThu
                        .ID = CTRL_PREFIX_SUMMARY_THU & uniqueID
                        .CheckBox.Text = DayOfWeek.Thursday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .CheckBox.Checked = (orderSummary.OnThursday > 0)
                    End With

                    chkdsodThu = New HtmlInputHidden
                    With chkdsodThu
                        .ID = CTRL_PREFIX_SUMMARY_THU_DSOD & uniqueID
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                            .Value = orderSummary.OnThursday
                        Else
                            .Value = 0
                        End If
                    End With

                    cell.Controls.Add(chkdsodThu)

                    cell.Controls.Add(chkThu)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' fri
                    chkFri = New CheckBoxEx()
                    With chkFri
                        .ID = CTRL_PREFIX_SUMMARY_FRI & uniqueID
                        .CheckBox.Text = DayOfWeek.Friday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .CheckBox.Checked = (orderSummary.OnFriday > 0)
                    End With

                    chkdsodFri = New HtmlInputHidden
                    With chkdsodFri
                        .ID = CTRL_PREFIX_SUMMARY_FRI_DSOD & uniqueID
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                            .Value = orderSummary.OnFriday
                        Else
                            .Value = 0
                        End If
                    End With

                    cell.Controls.Add(chkdsodFri)

                    cell.Controls.Add(chkFri)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' sat
                    chkSat = New CheckBoxEx()
                    With chkSat
                        .ID = CTRL_PREFIX_SUMMARY_SAT & uniqueID
                        .CheckBox.Text = DayOfWeek.Saturday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .CheckBox.Checked = (orderSummary.OnSaturday > 0)
                    End With

                    chkdsodSat = New HtmlInputHidden
                    With chkdsodSat
                        .ID = CTRL_PREFIX_SUMMARY_SAT_DSOD & uniqueID
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                            .Value = orderSummary.OnSaturday
                        Else
                            .Value = 0
                        End If
                    End With

                    cell.Controls.Add(chkdsodSat)

                    cell.Controls.Add(chkSat)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' sun
                    chkSun = New CheckBoxEx()
                    With chkSun
                        .ID = CTRL_PREFIX_SUMMARY_SUN & uniqueID
                        .CheckBox.Text = DayOfWeek.Sunday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .CheckBox.Checked = (orderSummary.OnSunday > 0)
                        .SetOriginalValue(.CheckBox.Checked)
                    End With

                    chkdsodSun = New HtmlInputHidden
                    With chkdsodSun
                        .ID = CTRL_PREFIX_SUMMARY_SUN_DSOD & uniqueID
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                            .Value = orderSummary.OnSunday
                        Else
                            .Value = 0
                        End If
                    End With

                    cell.Controls.Add(chkdsodSun)

                    cell.Controls.Add(chkSun)

                    chkAllDays.CheckBox.Attributes.Add("onclick", String.Format("days_TickUntickAll('{0}');", chkAllDays.ClientID))

                End If

                ' units
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                txtUnits = New TextBoxEx()
                With txtUnits
                    .ID = CTRL_PREFIX_SUMMARY_UNITS & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                    .Width = New Unit(3, UnitType.Em)
                    .Required = required
                    If .Required Then
                        .RequiredValidatorErrMsg = "* Required"
                        .ValidationGroup = "Save"
                    End If
                    If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .Text = orderSummary.ProviderUnits
                End With
                span = New HtmlGenericControl("span")
                span.Controls.Add(txtUnits)
                cell.Controls.Add(span)

                ' calculate the hours based on minutes
                If Not orderSummary Is Nothing Then
                    durationHours = Math.DivRem(orderSummary.ProviderMinutes, 60, durationMinutes)
                End If

                ' hours control
                txtUnitsHours = New TextBoxEx()
                With txtUnitsHours
                    .ID = CTRL_PREFIX_SUMMARY_UNITS_HOURS & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .OutputBrAfter = False
                    .Width = New Unit(2.5, UnitType.Em)
                    .Required = required
                    If .Required Then
                        .RequiredValidatorErrMsg = "* Required"
                        .ValidationGroup = "Save"
                        .MinimumValue = 0
                        .MaximumValue = 9999
                    End If
                    .TextBox.MaxLength = 4
                    .Text = "0"
                    If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                        .Text = durationHours
                    End If
                End With
                span = New HtmlGenericControl("span")
                span.Controls.Add(txtUnitsHours)
                cell.Controls.Add(span)

                ' label to separate hours and mins
                lblUnitsHours = New Label()
                lblUnitsHours.Text = ":"
                span.Controls.Add(lblUnitsHours)

                ' minutes control
                txtUnitsMins = New TextBoxEx()
                With txtUnitsMins
                    .ID = CTRL_PREFIX_SUMMARY_UNITS_MINUTES & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .OutputBrAfter = False
                    .Width = New Unit(1.5, UnitType.Em)
                    .Required = required
                    If .Required Then
                        .RequiredValidatorErrMsg = "* Required"
                        .ValidationGroup = "Save"
                        .MinimumValue = 0
                        .MaximumValue = 59
                    End If
                    .TextBox.MaxLength = 2
                    .Text = "00"
                    If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                        .Text = durationMinutes.ToString.PadLeft(2, "0")
                    End If
                End With
                span.Controls.Add(txtUnitsMins)
                cell.Controls.Add(span)

                ' measured in
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lblMeasuredIn = New Label()
                With lblMeasuredIn
                    If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                        .Text = measuredInDesc
                        .ToolTip = measuredInComment
                    End If
                End With
                cell.Controls.Add(lblMeasuredIn)

                ' su minutes
                If ShowSvcUserMinutesColumn() Then
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")
                    txtSuMinutes = New TextBoxEx()
                    With txtSuMinutes
                        .ID = CTRL_PREFIX_SUMMARY_SUMINUTES & uniqueID
                        .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                        .Width = New Unit(3, UnitType.Em)
                        .Required = required
                        If .Required Then
                            .RequiredValidatorErrMsg = "* Required"
                            .ValidationGroup = "Save"
                        End If
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .Text = orderSummary.ServiceUserMinutes
                    End With
                    cell.Controls.Add(txtSuMinutes)
                    txtUnitsHours.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", txtSuMinutes.ClientID, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
                    txtUnitsMins.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", txtSuMinutes.ClientID, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
                    txtUnits.TextBox.Attributes.Add("onblur", String.Format("txtUnits_Change('{0}_txtTextBox', '{1}', '{2}_txtTextBox');", txtUnits.ClientID, cboRateCategory.ClientID, txtSuMinutes.ClientID))
                Else
                    txtUnitsHours.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", String.Empty, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
                    txtUnitsMins.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", String.Empty, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
                    txtUnits.TextBox.Attributes.Add("onblur", String.Format("txtUnits_Change('{0}_txtTextBox', '{1}', '{2}_txtTextBox');", txtUnits.ClientID, cboRateCategory.ClientID, String.Empty))
                End If

                ' visits
                If ShowVisitsColumn() Then
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")
                    txtVisits = New TextBoxEx()
                    With txtVisits
                        .ID = CTRL_PREFIX_SUMMARY_VISITS & uniqueID
                        .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                        .Width = New Unit(3, UnitType.Em)
                        .Required = required
                        If .Required Then
                            .RequiredValidatorErrMsg = "* Required"
                            .ValidationGroup = "Save"
                        End If
                        If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then .Text = orderSummary.Visits Else .Text = "0"
                        If Not rateCategoryVisitBased Then
                            _forceDisableVisits.Add(txtVisits)
                            .Text = String.Empty
                        End If
                    End With
                    cell.Controls.Add(txtVisits)
                End If

                ' frequency
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboFrequency = New DropDownListEx()
                With cboFrequency
                    .ID = CTRL_PREFIX_SUMMARY_FREQUENCY & uniqueID
                    .Required = required
                    If .Required Then
                        .RequiredValidatorErrMsg = "* Required"
                        .ValidationGroup = "Save"
                    End If
                    LoadFrequencyDropdown(cboFrequency)
                    If Not orderSummary Is Nothing AndAlso orderSummary.ID > 0 Then
                        .DropDownList.SelectedValue = orderSummary.Frequency
                    Else
                        .DropDownList.SelectedValue = "0"
                    End If
                End With
                cell.Controls.Add(cboFrequency)
                cboFrequency.DropDownList.Attributes.Add("onchange", String.Format("cboFrequency_Change('{0}');", cboFrequency.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboFrequency_Change('{0}');}}", cboFrequency.ClientID)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "middle")
                btnRemove = New HtmlInputImage()
                With btnRemove
                    .ID = CTRL_PREFIX_SUMMARY_REMOVED & uniqueID
                    .Src = WebUtils.GetVirtualPath("images/delete.png")
                    .Attributes("title") = "Remove this line"
                    .ValidationGroup = "RemoveSummary"
                    AddHandler .ServerClick, AddressOf btnRemoveSummary_Click
                End With
                cell.Controls.Add(btnRemove)

            End If

        End Sub


        'Private Sub OutputSummaryControls(ByVal uniqueID As String, ByVal orderDetail As DomServiceOrderDetail, ByVal required As Boolean)

        '    Dim row As HtmlTableRow
        '    Dim cell As HtmlTableCell
        '    Dim cboRateCategory As DropDownListEx
        '    Dim cboDayOfWeek As DropDownListEx
        '    Dim txtUnits As TextBoxEx
        '    Dim txtUnitsHours As TextBoxEx
        '    Dim txtUnitsMins As TextBoxEx
        '    Dim lblUnitsHours As Label
        '    Dim lblMeasuredIn As Label
        '    Dim txtSuMinutes As TextBoxEx
        '    Dim txtVisits As TextBoxEx
        '    Dim cboFrequency As DropDownListEx
        '    Dim btnRemove As HtmlInputImage
        '    Dim rateCategory As DomRateCategory
        '    Dim rateCategoryVisitBased As Boolean
        '    Dim uom As DomUnitsOfMeasure
        '    Dim measuredInDesc As String = String.Empty
        '    Dim measuredInComment As String = String.Empty
        '    Dim span As HtmlGenericControl
        '    Dim durationHours As Integer, durationMinutes As Integer
        '    Dim rateCat As DomRateCategory = New DomRateCategory(Me.DbConnection, String.Empty, String.Empty)
        '    Dim rateFram As New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
        '    Dim framType As New FrameworkType(Me.DbConnection, String.Empty, String.Empty)
        '    Dim msg As New ErrorMessage()
        '    Dim txtLineDateFrom As TextBoxEx
        '    Dim txtLineDateTo As TextBoxEx

        '    ' don't output items marked as deleted
        '    If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_SUMMARY) Then

        '        row = New HtmlTableRow()
        '        row.ID = uniqueID
        '        phSummary.Controls.Add(row)

        '        'Line Date From
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        txtLineDateFrom = New TextBoxEx()
        '        With txtLineDateFrom
        '            .ID = CTRL_PREFIX_SUMMARY_LINE_DATE_FROM & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.DateFormatJquery
        '            .Width = New Unit(6, UnitType.Em)
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '            End If
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .Text = orderDetail.DateFrom
        '            Else
        '                .Text = _order.DateFrom
        '            End If


        '        End With
        '        span = New HtmlGenericControl("span")
        '        span.Controls.Add(txtLineDateFrom)
        '        cell.Controls.Add(span)

        '        'Line Date To
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        txtLineDateTo = New TextBoxEx()
        '        With txtLineDateTo
        '            .ID = CTRL_PREFIX_SUMMARY_LINE_DATE_TO & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.DateFormatJquery
        '            .Width = New Unit(6, UnitType.Em)
        '            .Required = False
        '            'If .Required Then
        '            '    .RequiredValidatorErrMsg = "* Required"
        '            '    .ValidationGroup = "Save"
        '            'End If
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                If orderDetail.DateTo <> Utils.MAX_END_DATE Then
        '                    .Text = orderDetail.DateTo
        '                End If
        '            Else
        '                If _order.DateTo <> Utils.MAX_END_DATE Then
        '                    .Text = _order.DateTo
        '                End If
        '            End If
        '            .AllowClear = True
        '        End With
        '        span = New HtmlGenericControl("span")
        '        span.Controls.Add(txtLineDateTo)
        '        cell.Controls.Add(span)

        '        If txtLineDateTo.Text <> String.Empty Then
        '            txtLineDateFrom.MaximumValue = txtLineDateTo.Text
        '        End If
        '        If txtLineDateFrom.Text <> String.Empty Then
        '            txtLineDateTo.MinimumValue = txtLineDateFrom.Text
        '        End If

        '        ' rate category
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        cboRateCategory = New DropDownListEx()
        '        With cboRateCategory
        '            .ID = CTRL_PREFIX_SUMMARY_RATE_CATEGORY & uniqueID
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '            End If
        '            LoadRateCategoryDropdown(cboRateCategory)
        '            If Not orderDetail Is Nothing Then .DropDownList.SelectedValue = orderDetail.DomRateCategoryID
        '        End With
        '        cell.Controls.Add(cboRateCategory)
        '        cboRateCategory.DropDownList.Attributes.Add("onchange", String.Format("cboRateCategory_Change('{0}');", cboRateCategory.ClientID))
        '        _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboRateCategory_Change('{0}');}}", cboRateCategory.ClientID)

        '        If Not orderDetail Is Nothing Then
        '            rateCategory = DomContractBL.FindRateCategory(_rateCategories, orderDetail.DomRateCategoryID)
        '            If Not rateCategory Is Nothing Then
        '                uom = DomContractBL.FindUnitOfMeasure(_uoms, rateCategory.DomUnitsOfMeasureID)
        '                If Not uom Is Nothing Then
        '                    measuredInDesc = uom.Description
        '                    measuredInComment = uom.Comment
        '                End If
        '                rateCategoryVisitBased = (uom.MinutesPerUnit <> 0)
        '            End If
        '        End If

        '        ' day of week
        '        If ShowDayOfWeekColumn() Then
        '            cell = New HtmlTableCell()
        '            row.Controls.Add(cell)
        '            cell.Style.Add("vertical-align", "top")
        '            cboDayOfWeek = New DropDownListEx()
        '            With cboDayOfWeek
        '                .ID = CTRL_PREFIX_SUMMARY_DOW & uniqueID
        '                .Required = required
        '                If .Required Then
        '                    .RequiredValidatorErrMsg = "* Required"
        '                    .ValidationGroup = "Save"
        '                End If
        '                LoadDayOfWeekDropdown(cboDayOfWeek)
        '                If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then .DropDownList.SelectedValue = orderDetail.DayOfWeek
        '            End With
        '            cell.Controls.Add(cboDayOfWeek)
        '        End If

        '        ' units
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        txtUnits = New TextBoxEx()
        '        With txtUnits
        '            .ID = CTRL_PREFIX_SUMMARY_UNITS & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
        '            .Width = New Unit(3, UnitType.Em)
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '            End If
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then .Text = orderDetail.ProviderUnits
        '        End With
        '        span = New HtmlGenericControl("span")
        '        span.Controls.Add(txtUnits)
        '        cell.Controls.Add(span)

        '        ' calculate the hours based on minutes
        '        If Not orderDetail Is Nothing Then
        '            durationHours = Math.DivRem(orderDetail.ProviderMinutes, 60, durationMinutes)
        '        End If

        '        ' hours control
        '        txtUnitsHours = New TextBoxEx()
        '        With txtUnitsHours
        '            .ID = CTRL_PREFIX_SUMMARY_UNITS_HOURS & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
        '            .OutputBrAfter = False
        '            .Width = New Unit(2.5, UnitType.Em)
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '                .MinimumValue = 0
        '                .MaximumValue = 9999
        '            End If
        '            .TextBox.MaxLength = 4
        '            .Text = "0"
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .Text = durationHours
        '            End If
        '        End With
        '        span = New HtmlGenericControl("span")
        '        span.Controls.Add(txtUnitsHours)
        '        cell.Controls.Add(span)

        '        ' label to separate hours and mins
        '        lblUnitsHours = New Label()
        '        lblUnitsHours.Text = ":"
        '        span.Controls.Add(lblUnitsHours)

        '        ' minutes control
        '        txtUnitsMins = New TextBoxEx()
        '        With txtUnitsMins
        '            .ID = CTRL_PREFIX_SUMMARY_UNITS_MINUTES & uniqueID
        '            .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
        '            .OutputBrAfter = False
        '            .Width = New Unit(1.5, UnitType.Em)
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '                .MinimumValue = 0
        '                .MaximumValue = 59
        '            End If
        '            .TextBox.MaxLength = 2
        '            .Text = "00"
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .Text = durationMinutes.ToString.PadLeft(2, "0")
        '            End If
        '        End With
        '        span.Controls.Add(txtUnitsMins)
        '        cell.Controls.Add(span)

        '        ' measured in
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        lblMeasuredIn = New Label()
        '        With lblMeasuredIn
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .Text = measuredInDesc
        '                .ToolTip = measuredInComment
        '            End If
        '        End With
        '        cell.Controls.Add(lblMeasuredIn)

        '        ' su minutes
        '        If ShowSvcUserMinutesColumn() Then
        '            cell = New HtmlTableCell()
        '            row.Controls.Add(cell)
        '            cell.Style.Add("vertical-align", "top")
        '            txtSuMinutes = New TextBoxEx()
        '            With txtSuMinutes
        '                .ID = CTRL_PREFIX_SUMMARY_SUMINUTES & uniqueID
        '                .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
        '                .Width = New Unit(3, UnitType.Em)
        '                .Required = required
        '                If .Required Then
        '                    .RequiredValidatorErrMsg = "* Required"
        '                    .ValidationGroup = "Save"
        '                End If
        '                If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then .Text = orderDetail.ServiceUserMinutes
        '            End With
        '            cell.Controls.Add(txtSuMinutes)
        '            txtUnitsHours.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", txtSuMinutes.ClientID, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
        '            txtUnitsMins.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", txtSuMinutes.ClientID, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
        '            txtUnits.TextBox.Attributes.Add("onblur", String.Format("txtUnits_Change('{0}_txtTextBox', '{1}', '{2}_txtTextBox');", txtUnits.ClientID, cboRateCategory.ClientID, txtSuMinutes.ClientID))
        '        Else
        '            txtUnitsHours.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", String.Empty, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
        '            txtUnitsMins.TextBox.Attributes.Add("onblur", String.Format("ctlUnitsHoursMins_Change(this, '{0}_txtTextBox', '{1}_txtTextBox', '{2}_txtTextBox');", String.Empty, txtUnitsHours.ClientID, txtUnitsMins.ClientID))
        '            txtUnits.TextBox.Attributes.Add("onblur", String.Format("txtUnits_Change('{0}_txtTextBox', '{1}', '{2}_txtTextBox');", txtUnits.ClientID, cboRateCategory.ClientID, String.Empty))
        '        End If

        '        ' visits
        '        If ShowVisitsColumn() Then
        '            cell = New HtmlTableCell()
        '            row.Controls.Add(cell)
        '            cell.Style.Add("vertical-align", "top")
        '            txtVisits = New TextBoxEx()
        '            With txtVisits
        '                .ID = CTRL_PREFIX_SUMMARY_VISITS & uniqueID
        '                .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
        '                .Width = New Unit(3, UnitType.Em)
        '                .Required = required
        '                If .Required Then
        '                    .RequiredValidatorErrMsg = "* Required"
        '                    .ValidationGroup = "Save"
        '                End If
        '                If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then .Text = orderDetail.Visits Else .Text = "0"
        '                If Not rateCategoryVisitBased Then
        '                    _forceDisableVisits.Add(txtVisits)
        '                    .Text = String.Empty
        '                End If
        '            End With
        '            cell.Controls.Add(txtVisits)
        '        End If

        '        ' frequency
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "top")
        '        cboFrequency = New DropDownListEx()
        '        With cboFrequency
        '            .ID = CTRL_PREFIX_SUMMARY_FREQUENCY & uniqueID
        '            .Required = required
        '            If .Required Then
        '                .RequiredValidatorErrMsg = "* Required"
        '                .ValidationGroup = "Save"
        '            End If
        '            LoadFrequencyDropdown(cboFrequency)
        '            If Not orderDetail Is Nothing AndAlso orderDetail.ID > 0 Then
        '                .DropDownList.SelectedValue = orderDetail.Frequency
        '            Else
        '                .DropDownList.SelectedValue = "0"
        '            End If
        '        End With
        '        cell.Controls.Add(cboFrequency)
        '        cboFrequency.DropDownList.Attributes.Add("onchange", String.Format("cboFrequency_Change('{0}');", cboFrequency.ClientID))
        '        _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboFrequency_Change('{0}');}}", cboFrequency.ClientID)

        '        ' remove button
        '        cell = New HtmlTableCell()
        '        row.Controls.Add(cell)
        '        cell.Style.Add("vertical-align", "middle")
        '        btnRemove = New HtmlInputImage()
        '        With btnRemove
        '            .ID = CTRL_PREFIX_SUMMARY_REMOVED & uniqueID
        '            .Src = WebUtils.GetVirtualPath("images/delete.png")
        '            .Attributes("title") = "Remove this line"
        '            .ValidationGroup = "RemoveSummary"
        '            AddHandler .ServerClick, AddressOf btnRemoveSummary_Click
        '        End With
        '        cell.Controls.Add(btnRemove)

        '    End If

        'End Sub

#End Region

#Region " OutputVisitControls "

        Private Sub OutputVisitControls(ByVal uniqueID As String, ByVal visit As DomServiceOrderVisit)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim cboServiceType As DropDownListEx
            Dim ctlStartTime As TimePicker
            Dim ctlDuration As TimePicker
            Dim txtCarers As TextBoxEx
            Dim chkMon As CheckBoxEx, chkTue As CheckBoxEx, chkWed As CheckBoxEx, chkThu As CheckBoxEx
            Dim chkFri As CheckBoxEx, chkSat As CheckBoxEx, chkSun As CheckBoxEx
            Dim chkAllDays As CheckBoxEx
            Dim cboFrequency As DropDownListEx
            Dim chkPrimary As CheckBoxEx
            Dim btnRemove As HtmlInputImage
            Dim durationHours As Integer, durationMinutes As Integer
            Dim txtLineDateFrom As TextBoxEx
            Dim txtLineDateTo As TextBoxEx
            Dim span As HtmlGenericControl

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_VISITS) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phVisits.Controls.Add(row)

                'Line Date From
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                txtLineDateFrom = New TextBoxEx()
                With txtLineDateFrom
                    .ID = CTRL_PREFIX_VISITS_LINE_DATE_FROM & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.DateFormatJquery
                    .Width = New Unit(7, UnitType.Em)
                    .Required = True
                    If .Required Then
                        .RequiredValidatorErrMsg = "* Required"
                        .ValidationGroup = "Save"
                    End If

                    If Not _order Is Nothing AndAlso _order.ID > 0 Then
                        If Utils.ToDateTime(_order.DateTo) <> Utils.MAX_END_DATE Then
                            .MaximumValue = _order.DateTo
                        End If
                        .MinimumValue = _order.DateFrom
                    End If

                    If Not visit Is Nothing AndAlso visit.ID > 0 Then
                        .Text = visit.DateFrom
                    Else
                        .Text = _order.DateFrom
                    End If
                    .AllowClear = True

                End With
                span = New HtmlGenericControl("span")
                span.Controls.Add(txtLineDateFrom)
                cell.Controls.Add(span)

                'Line Date To
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                txtLineDateTo = New TextBoxEx()
                With txtLineDateTo
                    .ID = CTRL_PREFIX_VISITS_LINE_DATE_TO & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.DateFormatJquery
                    .Width = New Unit(7, UnitType.Em)
                    .Required = False
                    If Not _order Is Nothing AndAlso _order.ID > 0 Then
                        If Utils.ToDateTime(_order.DateTo) <> Utils.MAX_END_DATE Then
                            .MaximumValue = _order.DateTo
                        End If
                        .MinimumValue = _order.DateFrom
                    End If
                    'If .Required Then
                    '    .RequiredValidatorErrMsg = "* Required"
                    '    .ValidationGroup = "Save"
                    'End If
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then
                        If visit.DateTo <> Utils.MAX_END_DATE Then
                            .Text = visit.DateTo
                        End If
                    Else
                        If _order.DateTo <> Utils.MAX_END_DATE Then
                            .Text = _order.DateTo
                        End If
                    End If
                    .AllowClear = True
                End With
                span = New HtmlGenericControl("span")
                span.Controls.Add(txtLineDateTo)
                cell.Controls.Add(span)

                If txtLineDateTo.Text <> String.Empty Then
                    txtLineDateFrom.MaximumValue = txtLineDateTo.Text
                End If
                If txtLineDateFrom.Text <> String.Empty Then
                    txtLineDateTo.MinimumValue = txtLineDateFrom.Text
                End If

                ' service type
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboServiceType = New DropDownListEx()
                With cboServiceType
                    .ID = CTRL_PREFIX_VISITS_SVC_TYPE & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    LoadServiceTypeDropdown(cboServiceType)
                    If Not visit Is Nothing Then .DropDownList.SelectedValue = visit.DomServiceTypeID
                End With
                cell.Controls.Add(cboServiceType)

                ' from
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                ctlStartTime = New TimePicker()
                With ctlStartTime
                    .ID = CTRL_PREFIX_VISITS_START_TIME & uniqueID
                    .ShowSeconds = False
                    If Not visit Is Nothing Then
                        .Hours = visit.StartTime.Hour
                        .Minutes = visit.StartTime.Minute
                    End If
                End With
                cell.Controls.Add(ctlStartTime)

                ' duration
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                ctlDuration = New TimePicker()
                With ctlDuration
                    .ID = CTRL_PREFIX_VISITS_DURATION & uniqueID
                    .ShowSeconds = False
                    If Not visit Is Nothing Then
                        durationHours = Math.DivRem(visit.Duration, 60, durationMinutes)
                        .Hours = durationHours
                        .Minutes = durationMinutes
                    End If
                    If _maintainedExternally Then .ControlDisplayMode = TimePicker.DisplayMode.TextBoxes
                End With
                cell.Controls.Add(ctlDuration)

                ' carers
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                txtCarers = New TextBoxEx()
                With txtCarers
                    .ID = CTRL_PREFIX_VISITS_CARERS & uniqueID
                    .Width = New Unit(3, UnitType.Em)
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .Required = True
                    .RequiredValidatorErrMsg = "*"
                    .ValidationGroup = "Save"
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then
                        .Text = visit.NumberOfCarers
                    Else
                        .Text = "1"
                    End If
                End With
                cell.Controls.Add(txtCarers)

                ' days
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")

                ' All Days
                chkAllDays = New CheckBoxEx()
                With chkAllDays
                    .ID = CTRL_PREFIX_VISITS_ALLDAYS & uniqueID
                    .CheckBox.Text = "All"
                    .CheckBox.TextAlign = TextAlign.Right

                    'If all days are already ticked then tick all days too
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then
                        If visit.OnMonday And visit.OnTuesday And _
                           visit.OnWednesday And visit.OnThursday And _
                           visit.OnFriday And visit.OnSaturday And _
                           visit.OnSunday Then

                            chkAllDays.CheckBox.Checked = True

                        Else

                            chkAllDays.CheckBox.Checked = False

                        End If
                    End If

                End With
                cell.Controls.Add(chkAllDays)
                cell.Controls.Add(CreateVisitDaysSpacer())

                ' mon
                chkMon = New CheckBoxEx()
                With chkMon
                    .ID = CTRL_PREFIX_VISITS_MON & uniqueID
                    .CheckBox.Text = DayOfWeek.Monday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then .CheckBox.Checked = visit.OnMonday
                End With
                cell.Controls.Add(chkMon)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' tue
                chkTue = New CheckBoxEx()
                With chkTue
                    .ID = CTRL_PREFIX_VISITS_TUE & uniqueID
                    .CheckBox.Text = DayOfWeek.Tuesday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then .CheckBox.Checked = visit.OnTuesday
                End With
                cell.Controls.Add(chkTue)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' wed
                chkWed = New CheckBoxEx()
                With chkWed
                    .ID = CTRL_PREFIX_VISITS_WED & uniqueID
                    .CheckBox.Text = DayOfWeek.Wednesday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then .CheckBox.Checked = visit.OnWednesday
                End With
                cell.Controls.Add(chkWed)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' thu
                chkThu = New CheckBoxEx()
                With chkThu
                    .ID = CTRL_PREFIX_VISITS_THU & uniqueID
                    .CheckBox.Text = DayOfWeek.Thursday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then .CheckBox.Checked = visit.OnThursday
                End With
                cell.Controls.Add(chkThu)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' fri
                chkFri = New CheckBoxEx()
                With chkFri
                    .ID = CTRL_PREFIX_VISITS_FRI & uniqueID
                    .CheckBox.Text = DayOfWeek.Friday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then .CheckBox.Checked = visit.OnFriday
                End With
                cell.Controls.Add(chkFri)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' sat
                chkSat = New CheckBoxEx()
                With chkSat
                    .ID = CTRL_PREFIX_VISITS_SAT & uniqueID
                    .CheckBox.Text = DayOfWeek.Saturday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then .CheckBox.Checked = visit.OnSaturday
                End With
                cell.Controls.Add(chkSat)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' sun
                chkSun = New CheckBoxEx()
                With chkSun
                    .ID = CTRL_PREFIX_VISITS_SUN & uniqueID
                    .CheckBox.Text = DayOfWeek.Sunday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then .CheckBox.Checked = visit.OnSunday
                    .SetOriginalValue(.CheckBox.Checked)
                End With
                cell.Controls.Add(chkSun)

                chkAllDays.CheckBox.Attributes.Add("onclick", String.Format("days_TickUntickAll('{0}');", chkAllDays.ClientID))

                ' frequency
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboFrequency = New DropDownListEx()
                With cboFrequency
                    .ID = CTRL_PREFIX_VISITS_FREQUENCY & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "*"
                    .ValidationGroup = "Save"
                    LoadFrequencyDropdown(cboFrequency)
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then
                        .DropDownList.SelectedValue = visit.Frequency
                    Else
                        .DropDownList.SelectedValue = "0"
                    End If
                End With
                cell.Controls.Add(cboFrequency)
                cboFrequency.DropDownList.Attributes.Add("onchange", String.Format("cboFrequency_Change('{0}');", cboFrequency.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboFrequency_Change('{0}');}}", cboFrequency.ClientID)

                ' primary
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                chkPrimary = New CheckBoxEx()
                With chkPrimary
                    .ID = CTRL_PREFIX_VISITS_PRIMARY & uniqueID
                    If Not visit Is Nothing AndAlso visit.ID > 0 Then
                        .CheckBox.Checked = visit.Primary
                    Else
                        .CheckBox.Checked = True
                    End If
                End With
                cell.Controls.Add(chkPrimary)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "middle")
                cell.Style.Add("text-align", "right")
                btnRemove = New HtmlInputImage()
                With btnRemove
                    .ID = CTRL_PREFIX_VISITS_REMOVED & uniqueID
                    .Src = WebUtils.GetVirtualPath("images/delete.png")
                    .Attributes("title") = "Remove this line"
                    .ValidationGroup = "RemoveVisit"
                    AddHandler .ServerClick, AddressOf btnRemoveVisit_Click
                End With
                cell.Controls.Add(btnRemove)

            End If

        End Sub

        Private Function CreateAttDaysSpacer() As HtmlGenericControl
            Dim spacer As HtmlGenericControl = New HtmlGenericControl("div")
            spacer.Style.Add("width", "3px")
            spacer.Style.Add("float", "left")
            spacer.InnerHtml = "&nbsp;"
            Return spacer
        End Function

#End Region

#Region " OutputExpenditureControls "

        Private Sub OutputExpenditureControls(ByVal uniqueID As String, ByVal exp As DSOSummary)

            Dim row As TableRow
            Dim cell As TableCell
            'Dim btn As HtmlInputButton
            Dim hidField As HiddenField
            Dim lit As Literal

            row = New TableRow()
            phExpenditure.Controls.Add(row)

            ' rate category
            cell = New TableCell()
            cell.ID = CTRL_PREFIX_EXPENDITURE_RATE_CATEGORY & uniqueID
            row.Cells.Add(cell)
            If Not exp Is Nothing Then cell.Text = exp.RateCategoryDesc

            ' units
            cell = New TableCell()
            cell.ID = CTRL_PREFIX_EXPENDITURE_UNITS & uniqueID
            row.Cells.Add(cell)
            If Not exp Is Nothing Then
                If Convert.ToBoolean(exp.UnitsDisplayedAsHoursMins) Then
                    Dim hours As Integer, mins As Integer
                    hours = Math.DivRem(Utils.ToInt32(exp.ProviderMinutes), 60, mins)
                    cell.Text = String.Format("{0}:{1}", hours.ToString().PadLeft(2, "0"), mins.ToString().PadLeft(2, "0"))
                Else
                    cell.Text = Utils.ToDecimal(exp.ProviderUnits).ToString("F2")
                End If
            End If

            ' measured in
            cell = New TableCell()
            cell.ID = CTRL_PREFIX_EXPENDITURE_MEASURED_IN & uniqueID
            row.Cells.Add(cell)
            If Not exp Is Nothing Then cell.Text = exp.MeasuredIn

            ' unit cost
            cell = New TableCell()
            cell.ID = CTRL_PREFIX_EXPENDITURE_UNIT_COST & uniqueID
            row.Cells.Add(cell)
            If Not exp Is Nothing Then
                cell.Text = Utils.ToDecimal(exp.UnitCost).ToString("C")
                If Convert.ToBoolean(exp.UCOverRidden) Then
                    cell.CssClass = "orangebg"
                End If
            End If

            ' overridden
            cell = New TableCell()
            cell.ID = CTRL_PREFIX_EXPENDITURE_OVERRIDDEN & uniqueID
            row.Cells.Add(cell)
            If Not exp Is Nothing Then
                cell.Text = Utils.BooleanToYesNo(Convert.ToBoolean(exp.UCOverRidden))
                If Convert.ToBoolean(exp.UCOverRidden) Then
                    cell.CssClass = "orangebg"
                End If
            End If

            ' total cost
            cell = New TableCell()
            cell.ID = CTRL_PREFIX_EXPENDITURE_TOTAL_COST & uniqueID
            row.Cells.Add(cell)
            cell.Style.Add("text-align", "right")
            If Not exp Is Nothing Then cell.Text = Utils.ToDecimal(exp.TotalCost).ToString("C")

            ' expenditure code
            cell = New TableCell()
            cell.ID = CTRL_PREFIX_EXPENDITURE_FINANCE_CODE & uniqueID
            row.Cells.Add(cell)
            cell.Style.Add("text-align", "right")
            lit = New Literal()
            lit.Text = "&nbsp;"
            cell.Controls.Add(lit)
            If Not exp Is Nothing Then cell.Controls.Add(GetExpenditureFinanceCode(exp.DomRatecategoryID, exp.DomServiceTypeID))

            ' hidden fields and button
            cell = New TableCell()
            cell.Style.Add("text-align", "right")
            row.Cells.Add(cell)
            ' unit cost
            hidField = New HiddenField()
            hidField.ID = CTRL_PREFIX_EXPENDITURE_HID_UNIT_COST & uniqueID
            If Not exp Is Nothing Then hidField.Value = Utils.ToDecimal(exp.UnitCost).ToString("F2")
            cell.Controls.Add(hidField)
            ' overridden
            hidField = New HiddenField()
            hidField.ID = CTRL_PREFIX_EXPENDITURE_HID_OVERRIDDEN & uniqueID
            If Not exp Is Nothing Then hidField.Value = exp.UCOverRidden.ToString()
            cell.Controls.Add(hidField)
            ' overridden unit cost
            hidField = New HiddenField()
            hidField.ID = CTRL_PREFIX_EXPENDITURE_HID_OVERRIDDEN_UNIT_COST & uniqueID
            If Not exp Is Nothing Then hidField.Value = Utils.ToDecimal(exp.UnitCost).ToString("F2")
            cell.Controls.Add(hidField)
            ' units
            hidField = New HiddenField()
            hidField.ID = CTRL_PREFIX_EXPENDITURE_HID_OVERRIDDEN_UNITS & uniqueID
            If Not exp Is Nothing Then hidField.Value = Utils.ToDecimal(exp.ProviderUnits).ToString("F2")
            cell.Controls.Add(hidField)

        End Sub

#End Region

#Region " GetExpenditureFinanceCode "

        Private Function GetExpenditureFinanceCode(ByVal rateCategoryID As Integer, _
                                                   ByVal domServiceTypeID As Integer) As HtmlGenericControl

            Dim result As HtmlGenericControl
            Dim msg As ErrorMessage
            Dim complex As Boolean
            Dim financeCodeList As List(Of FinanceCodeMoney) = Nothing
            Dim filterDate As Date

            If txtVisitsFilterDate.Text = String.Empty Then
                filterDate = DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing, DateTime.Now)
            Else
                filterDate = txtVisitsFilterDate.Text
            End If

            result = New HtmlGenericControl("span")

            Try
                ' service order funding complex? (only do the check once)
                If _serviceOrderFundingComplex = TriState.UseDefault Then
                    ' are any service order funding records present?
                    msg = FinanceCodeBL.IsServiceOrderFundingPresent(Me.DbConnection, _order.ID, _serviceOrderFundingPresent)
                    If Not msg.Success Then
                        WebUtils.DisplayError(msg)
                    End If
                    ' are they complex?
                    If _serviceOrderFundingPresent Then
                        msg = FinanceCodeBL.IsServiceOrderFundingComplex(Me.DbConnection, _order.ID, complex)
                        If Not msg.Success Then
                            WebUtils.DisplayError(msg)
                        End If
                        _serviceOrderFundingComplex = complex
                    Else
                        _serviceOrderFundingComplex = TriState.False
                    End If
                End If

                If _serviceOrderFundingComplex = TriState.True Then
                    ' "complex" service order funding
                    result.InnerText = "(complex)"
                    result.Attributes.Add( _
                        "title", _
                        "The expenditure finance code is too complex to derive at this point " & _
                        "due to the nature of the ""Service Order Funding"" details entered. " & _
                        "Please review these details (via the ""Financial"" tab) to determine the finance code." _
                    )
                ElseIf _serviceOrderFundingPresent Then
                    ' "simple" service order funding
                    If _serviceOrderFundingByServiceType Is Nothing Then
                        _serviceOrderFundingByServiceType = New Hashtable()
                    End If
                    If _serviceOrderFundingByServiceType.ContainsKey(domServiceTypeID) Then
                        ' codes for this service type already exist
                        financeCodeList = _serviceOrderFundingByServiceType(domServiceTypeID)
                    Else
                        ' get codes for this service type
                        msg = FinanceCodeBL.GetApportionedFinanceCodes(Me.DbConnection, _
                                                                   domServiceTypeID, _
                                                                   _order, _
                                                                   0.0, _
                                                                   0.0, _
                                                                   filterDate, _
                                                                   financeCodeList, _
                                                                   String.Empty)
                        If Not msg.Success Then
                            WebUtils.DisplayError(msg)
                        End If
                        _serviceOrderFundingByServiceType.Add(domServiceTypeID, financeCodeList)
                    End If
                    ' we should only have one code per service type, otherwise it is "complex"
                    result.InnerText = financeCodeList(0).ExpenditureCode

                Else
                    result.InnerText = IIf(String.IsNullOrEmpty(_order.FinanceCode), "None", _order.FinanceCode)
                    ' determine finance code using rules
                    'If _fcr Is Nothing Then
                    '    _fcr = DataCache.FinanceCodeRulesBL(Me.DbConnection, _
                    '                                       Nothing, _
                    '                                       Me.Settings, _
                    '                                       False _
                    '    )
                    '    ' prevent caching
                    '    _fcr.FCSegmentsBuiltUpSourceQueuesLimit = 0
                    'End If
                    'If _expCodeInputs Is Nothing Then
                    '    _expCodeInputs = GetExpFinanceCodeRuleInputs()
                    'End If
                    'With _expCodeInputs
                    '    .RateCategoryID = rateCategoryID
                    '    .ServiceTypeID = domServiceTypeID
                    'End With

                    'result.InnerText = _fcr.DomExpenditureCode(Me.DbConnection, _
                    '                                            Nothing, _
                    '                                            _expCodeInputs _
                    ')

                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(msg)
            End Try

            Return result

        End Function

#End Region

#Region " GetExpFinanceCodeRuleInputs "

        Private Function GetExpFinanceCodeRuleInputs() As FcRules.FinanceCodeRulesBL.DomExpenditureCodeInputs

            Dim msg As ErrorMessage
            Dim result As FcRules.FinanceCodeRulesBL.DomExpenditureCodeInputs
            Dim assessmentID As Integer

            Try
                result = New FcRules.FinanceCodeRulesBL.DomExpenditureCodeInputs()

                ' get the relevant assessment ID for this client
                msg = AssessmentBL.GetDomAssessmentID(Me.DbConnection, _
                                                        Nothing, _
                                                        _order.ClientID, _
                                                        DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing), _
                                                        False, _
                                                        assessmentID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If assessmentID > 0 Then
                    ' get the relevant inputs
                    result = New FcRules.FinanceCodeRulesBL.DomExpenditureCodeInputs(Me.DbConnection, Nothing, assessmentID)
                End If

                With result
                    '.AccommodationTypeID = 0
                    '.AdmissionStatusID = 0
                    '.Band = ""
                    '.ClientGroupID = 0
                    '.ClientID = 0
                    .DsoClientGroupID = _order.ClientGroupID
                    .DsoEstablishmentID = _order.ProviderID
                    .DsoID = _order.ID
                    .DsoTeamID = _order.TeamID
                    .EstablishmentID = _order.ProviderID
                    .Private = _establishment.Private
                    '.LocalityID = 0
                    '.PlacementFC = ""
                    '.PlacementID = 0
                    '.Private = False
                    '.RateCategoryID = 0
                    '.ServiceTypeID = 0
                    '.TeamID = 0
                    '.TransactionTypeID = 0
                End With

            Catch ex As Exception
                Throw
            End Try

            Return result

        End Function

#End Region

#Region " OutputExpenditureControlsFromViewState "

        Private Sub OutputExpenditureControlsFromViewState()
            Dim expList As List(Of String) = GetExpenditureUniqueIDsFromViewState()
            For Each id As String In expList
                OutputExpenditureControls(id, Nothing)
            Next
        End Sub

#End Region

#Region " PopulateExpenditureTab "

        Private Sub PopulateExpenditureTab(ByVal orderID As Integer)

            Dim msg As ErrorMessage
            Dim expList As List(Of String)
            Dim orderExp As List(Of DSOSummary) = New List(Of DSOSummary)

            tabExpenditure.Enabled = True

            ViewState.Remove(VIEWSTATE_KEY_DATA_EXPENDITURE)
            phExpenditure.Controls.Clear()

            msg = FetchExpenditureLines(orderID, orderExp)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            expList = GetExpenditureUniqueIDsFromViewState()
            For Each exp As DSOSummary In orderExp
                ID = GetExpenditureUniqueID(exp)
                OutputExpenditureControls(ID, exp)
                expList.Add(ID)
            Next
            PersistExpenditureUniqueIDsToViewState(expList)

        End Sub

#End Region

#Region " PopulateFundingTab "

        Private Sub PopulateFundingTab(ByVal orderID As Integer)
            Dim msg As ErrorMessage
            Dim currentRevisionID As Integer = 0
            Dim revisions As DomServiceOrderFundingRevisionCollection = Nothing

            msg = DomServiceOrderFundingRevision.FetchList(Me.DbConnection, revisions, orderID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If revisions.Count > 0 Then
                fundingButton.Visible = False
                fundingPanel.Visible = False
                'svcOrderFundingPanel.Visible = True
                weHaveFundingRecords = True
            Else
                weHaveFundingRecords = False
            End If


            'For Each revision As DomServiceOrderFundingRevision In revisions
            '    If revision.EffectiveDate < Date.Today Or currentRevisionID = 0 Then
            '        currentRevisionID = revision.ID
            '    End If
            'Next
            'If currentRevisionID <> 0 Then

            '    'CType(dsoFunding, ServiceOrderFunding).RevisionID = currentRevisionID
            'End If
            Me.StdButtonsMode = _stdBut.ButtonsMode
            CType(dsoFunding, ServiceOrderFunding).populateFromSvcOrderScreen(orderID)
        End Sub

#End Region

#Region " PopulateDropdowns "

        Private Sub PopulateDropdowns(ByVal orderID As Integer, Optional ByVal cboName As String = "", _
                                      Optional ByVal selIndex As Integer = 0)

            Dim msg As ErrorMessage

            If cboName = "" Or cboName = "cboEndReason" Then
                With cboEndReason
                    ' get a list of reasons available to the order
                    If _endReasons Is Nothing Then
                        msg = ServiceOrderBL.FetchEndReasonsAvailableToOrder(Me.DbConnection, orderID, TriState.False, 0, _endReasons)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If
                    With .DropDownList
                        .Items.Clear()
                        .DataSource = _endReasons
                        .DataTextField = "Description"
                        .DataValueField = "ID"
                        .DataBind()
                        ' insert a blank at the top
                        .Items.Insert(0, New ListItem(String.Empty))
                        '++ Select the passed index (if any)..
                        If selIndex > 0 Then .SelectedIndex = selIndex
                    End With
                End With
            End If

            If cboName = "" Or cboName = "cboProjectCode" Then
                With cboProjectCode
                    ' get a list of project codes
                    If _projectCodes Is Nothing Then
                        msg = ProjectCode.FetchList(conn:=DbConnection, _
                                                    auditLogTitle:=String.Empty, _
                                                    auditUserName:=String.Empty, _
                                                    list:=_projectCodes, _
                                                    redundant:=TriState.False)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If
                    With .DropDownList
                        .Items.Clear()
                        For Each pc As ProjectCode In _projectCodes
                            Dim i As New ListItem
                            i.Value = pc.Code
                            If pc.Description <> String.Empty Then
                                i.Text = String.Format("{0} - {1}", pc.Code, pc.Description)
                            Else
                                i.Text = pc.Code
                            End If
                            .Items.Add(i)
                        Next
                        ' insert a blank at the top
                        .Items.Insert(0, New ListItem(String.Empty))
                        '++ Select the passed index (if any)..
                        If selIndex > 0 Then .SelectedIndex = selIndex
                    End With
                End With
            End If

        End Sub

#End Region

#Region " PopulateAttendanceSummaryTab "

        Private Sub PopulateAttendanceSummaryTab()
            Dim attendanceSummaryList As New List(Of String)
            Dim AttSummaryDetails As DomServiceOrderAttendanceCollection = Nothing
            Dim msg As ErrorMessage

            msg = ServiceOrderBL.BuildAttendanceCollectionFromDSODetail(Me.DbConnection, _order.ID, AttSummaryDetails)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            hidAttendanceHasDetail.Value = (AttSummaryDetails.Count > 0)

            'populate the attendance summary tab
            attendanceSummaryList = GetAttendanceSummaryUniqueIDsFromViewState()
            phAttendanceSummary.Controls.Clear()
            For Each attendance As DomServiceOrderAttendance In AttSummaryDetails
                ID = GetAttendanceSummaryUniqueID(attendance)
                OutputAttendanceSummaryControls(ID, attendance)
                attendanceSummaryList.Add(ID)
            Next
            PersistAttendanceSummaryUniqueIDsToViewState(attendanceSummaryList)

        End Sub

#End Region

#Region " PopulateAttendanceTab "

        Private Sub PopulateAttendanceTab()
            Dim currentRecordPos As Integer = 0
            Dim attendanceList As New List(Of String)
            Dim attendanceEffectiveDates As List(Of Date) = Nothing
            Dim AttendanceSchedules As vwDomServiceOrderAttendanceCollection = Nothing
            Dim msg As ErrorMessage
            'OutputPlannedAttendance()

            msg = ServiceOrderBL.FetchDomServiceOrderAttendanceAffectiveDates(Me.DbConnection, _order.ID, attendanceEffectiveDates)
            If Not msg.Success Then WebUtils.DisplayError(msg)


            If Not hid_AttendanceNewClicked.Value = "True" Then
                If hid_AttendanceEditClicked.Value = "" Then
                    hidCurrentEffectiveDate.Value = Date.MinValue
                    If attendanceEffectiveDates.Count = 1 Then
                        hidCurrentEffectiveDate.Value = attendanceEffectiveDates(0)
                        currentRecordPos += 1
                    Else
                        For Each effectiveDate As Date In attendanceEffectiveDates
                            'if the effective date of this current record is greater than today (Attendance is effective in the future)
                            'And the affectiveDate has already been set, the exit this loop.
                            If effectiveDate > Date.Today AndAlso hidCurrentEffectiveDate.Value <> Date.MinValue Then Exit For
                            'set the affective date
                            hidCurrentEffectiveDate.Value = effectiveDate
                            currentRecordPos += 1
                        Next

                    End If
                Else
                    Dim newEffectiveDate As Date
                    For Each effectiveDate As Date In attendanceEffectiveDates
                        'If this is the date we want to look at then leave it there.
                        currentRecordPos += 1
                        If hidCurrentEffectiveDate.Value = effectiveDate Then
                            newEffectiveDate = effectiveDate
                            Exit For
                        End If


                    Next
                    hidCurrentEffectiveDate.Value = newEffectiveDate
                End If

                If currentRecordPos = 0 Then currentRecordPos += 1

                lblCurrentRecordNo.Text = currentRecordPos.ToString
                lblTotalNoRecords.Text = attendanceEffectiveDates.Count.ToString

                dteEffectiveFrom.Text = hidCurrentEffectiveDate.Value
                hidOriginalEffectiveDate.Value = hidCurrentEffectiveDate.Value
            Else
                hidCurrentEffectiveDate.Value = attendanceEffectiveDates(attendanceEffectiveDates.Count - 1)
            End If

            'hidCurrentEffectiveDate.Value

            ' refresh the list of existing attendance Schedules and save in View State
            If IsDate(hidCurrentEffectiveDate.Value) Then

                msg = vwDomServiceOrderAttendance.FetchList(Me.DbConnection, AttendanceSchedules, _order.ID, hidCurrentEffectiveDate.Value)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            Else
                AttendanceSchedules = New vwDomServiceOrderAttendanceCollection
            End If

            hidAttendanceHasDetail.Value = (AttendanceSchedules.Count > 0)

            'attendanceList = GetAttendanceUniqueIDsFromViewState()
            phAttendance.Controls.Clear()
            For Each attendance As vwDomServiceOrderAttendance In AttendanceSchedules
                ID = GetAttendanceUniqueID(attendance)
                OutputAttendanceControls(ID, attendance)
                attendanceList.Add(ID)
            Next
            PersistAttendanceUniqueIDsToViewState(attendanceList)
        End Sub

#End Region

#Region " SetupInPlaceSelectors "

        Private Sub SetupProviderSelector(ByVal providerID As Integer)
            With CType(Me.provider, InPlaceSelectors.InPlaceEstablishmentSelector)
                .EstablishmentID = providerID
                .Required = True
                .ValidationGroup = "Save"
            End With
        End Sub

        Private Sub SetupContractSelector(ByVal contractID As Integer, ByVal providerID As Integer)
            With CType(Me.domContract, InPlaceSelectors.InPlaceDomContractSelector)
                .ContractID = contractID
                .Required = True
                .ValidationGroup = "Save"
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
                .Required = True
                .ValidationGroup = "Save"
            End With
        End Sub

        Private Sub SetupDSOAdditionalDetails()
            With CType(DSOAdditionalDetails, Target.Web.Library.UserControls.DSOBasicDetails)
                '.InitControl(Me.Page, Me.DbConnection, _order.ClientID, _order)
            End With
        End Sub

#End Region

#Region " SetupValidators "

        Private Sub SetupValidators(ByVal order As DomServiceOrder)
            If order Is Nothing Then
                _enableDateToValidator = False
                _enableEndReasonValidator = False
                _enableFinanceCode2Validator = False
            Else
                Dim dateTo As String, endReason As String
                dateTo = IIf(order.DateTo = DataUtils.MAX_DATE, String.Empty, order.DateTo.ToString())
                endReason = IIf(order.DomServiceOrderEndReasonID = 0, String.Empty, order.DomServiceOrderEndReasonID.ToString())

                SetupValidators(dateTo, endReason)
            End If
        End Sub

        Private Sub SetupValidators(ByVal dateTo As String, ByVal endReason As String)

            ' date to and end reason
            If Not Utils.IsDate(dateTo) Then
                cboEndReason.DropDownList.SelectedValue = String.Empty
                endReason = String.Empty
                cboEndReason.RequiredValidator.Enabled = False
                dteDateTo.RequiredValidator.Enabled = False
                _enableDateToValidator = False
                _enableEndReasonValidator = False
            Else
                cboEndReason.RequiredValidator.Enabled = True
                _enableEndReasonValidator = True
            End If
            If endReason = String.Empty Then
                dteDateTo.RequiredValidator.Enabled = False
                _enableDateToValidator = False
            Else
                dteDateTo.RequiredValidator.Enabled = True
                _enableDateToValidator = True
            End If

        End Sub

#End Region

#Region " GetSummaryUniqueIDsFromViewState "

        Private Function GetSummaryUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_SUMMARY) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_SUMMARY), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_SUMMARY) Is Nothing Then
                _newSummaryIDCounter = 0
            Else
                _newSummaryIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_SUMMARY), Integer)
            End If

            Return list

        End Function

#End Region

#Region " GetSummaryUniqueID "

        Private Function GetSummaryUniqueID(ByVal orderDetail As vwDomServiceOrderDetail) As String

            Dim id As String

            If orderDetail.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_SUMMARY & _newSummaryIDCounter
                _newSummaryIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_SUMMARY & orderDetail.ID
            End If

            Return id

        End Function

#End Region

#Region " GetSummaryUniqueID "

        Private Function GetSummaryUniqueID(ByVal orderDetail As DomServiceOrderSummary) As String

            Dim id As String

            If orderDetail.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_SUMMARY & _newSummaryIDCounter
                _newSummaryIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_SUMMARY & orderDetail.ID
            End If

            Return id

        End Function

#End Region

#Region " PersistSummaryUniqueIDsToViewState "

        Private Sub PersistSummaryUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_SUMMARY, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_SUMMARY, _newSummaryIDCounter)
        End Sub

#End Region

#Region " GetVisitUniqueIDsFromViewState "

        Private Function GetVisitUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_VISITS) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_VISITS), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_VISITS) Is Nothing Then
                _newVisitIDCounter = 0
            Else
                _newVisitIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_VISITS), Integer)
            End If

            Return list

        End Function

#End Region

#Region " GetVisitUniqueID "

        Private Function GetVisitUniqueID(ByVal visit As DomServiceOrderVisit) As String

            Dim id As String

            If visit.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_VISITS & _newVisitIDCounter
                _newVisitIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_VISITS & visit.ID
            End If

            Return id

        End Function

#End Region

#Region " PersistVisitUniqueIDsToViewState "

        Private Sub PersistVisitUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_VISITS, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_VISITS, _newVisitIDCounter)
        End Sub

#End Region

#Region " GetExpenditureUniqueIDsFromViewState "

        Private Function GetExpenditureUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_EXPENDITURE) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_EXPENDITURE), List(Of String))
            End If

            Return list

        End Function

#End Region

#Region " GetExpenditureUniqueID "

        Private Function GetExpenditureUniqueID(ByVal exp As DSOSummary) As String

            Dim id As String

            id = UNIQUEID_PREFIX_UPDATE_EXPENDITURE & exp.DomRatecategoryID

            Return id

        End Function

#End Region

#Region " PersistExpenditureUniqueIDsToViewState "

        Private Sub PersistExpenditureUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_EXPENDITURE, list)
        End Sub

#End Region

#Region " LoadRateCategoryDropdown "

        Private Sub LoadRateCategoryDropdown(ByVal dropdown As DropDownListEx)

            Dim msg As ErrorMessage
            Const SP_NAME As String = "spxDomRateCategory_FetchListForSummaryOrder"

            Dim dateDataset As DataSet = Nothing

            Try
                If _order Is Nothing Then Exit Sub

                ' create SP parameters
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = _order.DomContractID

                dateDataset = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                _rateCategories = New DomRateCategoryCollection
                Dim rateCatTable As DataTable = dateDataset.Tables(0)
                For Each rateCatDataRow As DataRow In rateCatTable.Rows
                    Dim rateCat As New DomRateCategory(Me.DbConnection, String.Empty, String.Empty)
                    msg = rateCat.Fetch(rateCatDataRow("ID"))
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _rateCategories.Add(rateCat)
                Next

                If _uoms Is Nothing Then
                    msg = DomUnitsOfMeasure.FetchList(Me.DbConnection, _uoms, String.Empty, String.Empty)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                With dropdown.DropDownList
                    .Items.Clear()
                    .DataSource = rateCatTable '_rateCategories
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    .Items.Insert(0, String.Empty)
                End With

                ' close
                rateCatTable = Nothing
                dateDataset = Nothing

                'msg.Success = True

                'If _rateCategories Is Nothing Then
                '    msg = DomContract_DomRateCategory.FetchDomRateCategoryList(Me.DbConnection, _rateCategories, String.Empty, String.Empty, _order.DomContractID)
                '    If Not msg.Success Then WebUtils.DisplayError(msg)
                '    _rateCategories.Sort(New CollectionSorter("Description", SortDirection.Ascending))
                'End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(msg)
            End Try

        End Sub

#End Region

#Region " LoadDayOfWeekDropdown "

        Private Sub LoadDayOfWeekDropdown(ByVal dropdown As DropDownListEx)

            With dropdown.DropDownList.Items
                .Clear()
                For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                    .Add(New ListItem([Enum].GetName(GetType(DayOfWeek), day).Substring(0, 2), Convert.ToInt32(day)))
                Next
                .Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region " LoadFrequencyDropdown "

        Private Sub LoadFrequencyDropdown(ByVal dropdown As DropDownListEx)

            With dropdown.DropDownList.Items
                .Clear()
                For Each freq As Frequency In [Enum].GetValues(GetType(Frequency))
                    .Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(Frequency), freq)), Convert.ToInt32(freq)))
                Next
                .Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region " LoadServiceTypeDropdown "

        Private Sub LoadServiceTypeDropdown(ByVal dropdown As DropDownListEx)

            FetchServiceTypes()

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _serviceTypes
                .DataTextField = "Text"
                .DataValueField = "Value"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region " LoadFirstWeekofServiceDropdown "

        Private Sub LoadFirstWeekofServiceDropdown(ByVal dropdown As DropDownListEx)

            With dropdown.DropDownList.Items
                .Clear()
                For index As Byte = 1 To 4
                    .Add(New ListItem(index, index))
                Next
            End With

        End Sub

#End Region

#Region " FetchServiceTypes "

        Private Sub FetchServiceTypes()

            Dim msg As ErrorMessage

            If _serviceTypes Is Nothing Then
                msg = DomContractBL.FetchServiceTypesAvailableToContract(Me.DbConnection, _order.DomContractID, _serviceTypes)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

        End Sub

#End Region

#Region " ShowDayOfWeekColumn "

        Protected Function ShowDayOfWeekColumn() As Boolean
            If _order Is Nothing Then
                Return False
            Else
                Return (_order.SpecifyDayOfWeek)
            End If
        End Function

#End Region

#Region " ShowSvcUserMinutesColumn "

        Protected Function ShowSvcUserMinutesColumn() As Boolean
            If _order Is Nothing Then
                Return False
            Else
                Return _order.VisitBasedReturns
            End If
        End Function

#End Region

#Region " ShowVisitsColumn "

        Protected Function ShowVisitsColumn() As Boolean

            'Dim msg As ErrorMessage
            Dim result As Boolean

            If Not _order Is Nothing Then

                If _frType Is Nothing Then
                    result = False
                Else
                    result = DomRateFrameworkBL.IsFrameworkTimeBased(Me.DbConnection, _contract.DomRateFrameworkID)
                End If

            End If

            Return result

        End Function

#End Region

#Region " RollUpSummaryByRateCatFreq "

        Private Function RollUpSummaryByRateCatFreq(ByVal orderDetails As List(Of vwDomServiceOrderDetail)) As Dictionary(Of String, DomServiceOrderDetail)

            Dim result As Dictionary(Of String, DomServiceOrderDetail) = New Dictionary(Of String, DomServiceOrderDetail)
            Dim key As String

            For Each orderDetail As vwDomServiceOrderDetail In orderDetails
                Dim order As DomServiceOrderDetail = New DomServiceOrderDetail(Me.DbConnection, String.Empty, String.Empty)

                Try
                    Utils.SetProperties(orderDetail.GetType.GetProperties, _
                                        order.GetType.GetProperties, orderDetail, order)
                Catch ex As Exception
                    Throw
                End Try

                With order
                    If Not order.Enhanced Then
                        key = String.Format("{0}-{1}-{2}-{3}", .DomRateCategoryID, .Frequency, .DayOfWeek, .FirstWeekOfService)
                        If result.ContainsKey(key) Then
                            result(key).ProviderUnits += .ProviderUnits
                            result(key).Visits += .Visits
                            result(key).ServiceUserMinutes += .ServiceUserMinutes
                            result(key).ProviderMinutes += .ProviderMinutes
                        Else
                            result.Add(key, order)
                        End If
                    End If
                End With
            Next

            Return result

        End Function

        Private Function RollUpSummaryByRateCatFreq(ByVal orderDetails As DomServiceOrderDetailCollection) As Dictionary(Of String, DomServiceOrderDetail)

            Dim result As Dictionary(Of String, DomServiceOrderDetail) = New Dictionary(Of String, DomServiceOrderDetail)
            Dim key As String

            For Each orderDetail As DomServiceOrderDetail In orderDetails
                With orderDetail
                    If Not orderDetail.Enhanced Then
                        key = String.Format("{0}-{1}-{2}-{3}", .DomRateCategoryID, .Frequency, .DayOfWeek, .FirstWeekOfService)
                        If result.ContainsKey(key) Then
                            result(key).ProviderUnits += .ProviderUnits
                            result(key).Visits += .Visits
                            result(key).ServiceUserMinutes += .ServiceUserMinutes
                            result(key).ProviderMinutes += .ProviderMinutes
                        Else
                            result.Add(key, orderDetail)
                        End If
                    End If
                End With
            Next

            Return result

        End Function

#End Region

#Region " FetchExpenditureLines "

        Private Function FetchExpenditureLines(ByVal orderID As Integer, ByRef dsoSummaryList As List(Of DSOSummary)) As ErrorMessage

            Dim msg As ErrorMessage
            Dim overallCost As Decimal
            Dim filterDate As Date

            If txtVisitsFilterDate.Text = String.Empty Then
                filterDate = DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing, DateTime.Now)
            Else
                filterDate = txtVisitsFilterDate.Text
            End If

            Dim dsobl As ServiceOrderBL = New ServiceOrderBL()
            dsobl.GetExpenditureDetails(Me.DbConnection, _order.ClientID, filterDate, _order.ProviderID, _order.DomContractID, dsoSummaryList)




            Try

                For Each exp As DSOSummary In dsoSummaryList

                    overallCost += Utils.ToDecimal(exp.TotalCost)
                Next

                litExpOverallCost.Text = overallCost.ToString("C")
                litExpStatement.Text = _
                    String.Format( _
                        "Standard rates shown above are those in effect on {0}<br />" & _
                        "Expenditure codes shown above are those in effect on {0}", _
                        Convert.ToDateTime(filterDate).ToString("dd/MM/yyyy") _
                    )

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region

#Region " PopulateSuspensions "

        Private Function PopulateSuspensions(ByVal orderID As Integer) As ErrorMessage

            Const SP_NAME As String = "spxDomServiceOrder_FetchSuspensionView"

            Dim msg As ErrorMessage
            Dim spParams As SqlParameter()
            Dim suspensions As DataTable

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = orderID
                suspensions = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams).Tables(0)

                With rptSuspensions
                    .DataSource = suspensions
                    .DataBind()
                End With

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region

#Region " FormatSuspensionDate "

        Protected Function FormatSuspensionDate(ByVal theDate As Date) As String
            Return DataUtils.DisplayEndDate(theDate)
        End Function

#End Region

#Region " Page_PreRenderXXX "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim msg As ErrorMessage = Nothing

            'check if associated framework's rate categories has one or more payment tolerance groups
            If Not _order Is Nothing Then
                msg = PaymentToleranceBL.DoesContractHaveOneOrMorePaymentToleranceGroups(Me.DbConnection, _
                                                                                         _order.DomContractID, _
                                                                                   _hasPaymentToleranceGroups)

                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            dteDateTo.EnsureChildControls()
            dteDateTo.Required = _enableDateToValidator
            cboEndReason.Required = _enableEndReasonValidator

            tabDocuments.Visible = (_documentsAccessible AndAlso (_stdBut.ButtonsMode = StdButtonsMode.Fetched OrElse _
                                                                  _stdBut.ButtonsMode = StdButtonsMode.Edit))

            'check if new service order or if existing then diplay the tab payment tolerances
            TabPaymentTolerancesSetVisibility()

            If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then Exit Sub

            If _maintainedExternally Then
                If _canAmendAttendance AndAlso _frType.ID = FrameworkTypes.ServiceRegister AndAlso hidAttendanceHasDetail.Value = "True" Then
                    _imgPadlock.Attributes("title") = "This Service Order is maintained via an Electronic Interface. Attendance patterns may be maintained."
                Else
                    _imgPadlock.Attributes("title") = "This Service Order is maintained via an Electronic Interface."
                End If
                _showPadlock = True
            Else
                If Not _canEdit AndAlso _canAmendAttendance Then
                    _imgPadlock.Attributes("title") = "This Service Order is locked. Attendance patterns may be maintained."
                    _showPadlock = True
                End If
            End If

            If _showPadlock Then
                FetchExternalFields()

                _stdBut.AllowDelete = False
                _stdBut.AllowEdit = _editableFieldsFound
            End If

            If Not _showPadlock Then
                _stdBut.ClearCustomControls()
            End If

            'WebUtils.RecursiveDisable(txtVisitsFilterDate.Controls, chkDontFilterCommitment.Checked)
            'txtVisitsFilterDate.Visible = chkDontFilterCommitment.Checked
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            ' enable/disable/hide tabs
            Dim disableSummary As Boolean
            Dim js As StringBuilder
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim attendanceEffectiveDates As List(Of Date) = Nothing
            Dim inEditMode As Boolean = False

            ' see if any controls should be forced to be disabled
            For Each ctrl As TextBoxEx In _forceDisableVisits
                ctrl.Enabled = False
            Next

            If dteDateTo.TextBox.Text = "" Then
                cboEndReason.Enabled = False
            End If


            If Not _order Is Nothing AndAlso _order.ID > 0 Then
                tabFunding.Visible = True
                If _frType.ID = FrameworkTypes.ServiceRegister Then
                    tabVisits.Visible = False
                    tabSummary.Visible = False
                    tabAttendance.Visible = hidAttendanceHasDetail.Value = "True"
                    WebUtils.RecursiveDisable(tabAttendance.Controls, True)
                    btnNextSchedule.Enabled = True
                    btnPreviousSchedule.Enabled = True
                    tabAttendanceSummary.Visible = True

                Else
                    If _order.VisitBased Then
                        disableSummary = True
                        tabVisits.Visible = True
                    Else
                        tabVisits.Visible = False
                    End If
                    tabAttendance.Visible = False
                    tabAttendanceSummary.Visible = False
                End If

                'Funding button should be enabled when an order exists
                'If _stdBut.ButtonsMode <> StdButtonsMode.Edit Then
                '    btnSvcOrderFunding.Disabled = False
                '    btnSvcOrderFunding.Visible = SecurityBL.UserHasMenuItem(Me.DbConnection, _
                '                                                            currentUser.ID, _
                '                                                            Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderFunding"), _
                '                                                            Me.Settings.CurrentApplicationID)
                'Else
                '    btnSvcOrderFunding.Disabled = True
                'End If
                divDSOAdditionalDetails.Visible = True
            Else
                tabSummary.Visible = False
                tabVisits.Visible = False
                tabFunding.Visible = False
                tabExpenditure.Visible = False
                tabSuspensions.Visible = False
                tabAttendance.Visible = False
                tabAttendanceSummary.Visible = False
                'Funding button should be disabled and hidden when a new order is being entered
                'btnSvcOrderFunding.Disabled = True
                'btnSvcOrderFunding.Visible = False
                divDSOAdditionalDetails.Visible = False
            End If

            If _stdBut.ButtonsMode = StdButtonsMode.Edit Or _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                inEditMode = True
                fundingButton.Visible = False
            End If

            If Not tabAttendanceSummary.Visible Then tabAttendanceSummary.HeaderText = String.Empty
            If Not tabAttendance.Visible Then tabAttendance.HeaderText = String.Empty
            If Not tabSummary.Visible Then tabSummary.HeaderText = String.Empty
            If Not tabAttendanceSummary.Visible Then tabAttendanceSummary.HeaderText = String.Empty
            If Not tabVisits.Visible Then tabVisits.HeaderText = String.Empty
            If Not tabExpenditure.Visible Then tabExpenditure.HeaderText = String.Empty
            If Not tabSuspensions.Visible Then tabSuspensions.HeaderText = String.Empty
            If Not tabFunding.Visible Then tabFunding.HeaderText = String.Empty
            ' select the current tab
            tabStrip.SetActiveTabByHeaderText(hidSelectedTab.Value)

            If disableSummary Then
                WebUtils.RecursiveDisable(tabSummary.Controls, True)
            End If

            ' disable provider/contract/client controls once the order has been created
            If Not _order Is Nothing AndAlso _order.ID > 0 Then
                WebUtils.RecursiveDisable(provider.Controls, True)
                WebUtils.RecursiveDisable(domContract.Controls, True)
                WebUtils.RecursiveDisable(client.Controls, True)
            ElseIf (CopyFromID.HasValue) Then
                WebUtils.RecursiveDisable(provider.Controls, True)
                WebUtils.RecursiveDisable(domContract.Controls, True)
            End If

            If _stdBut.ButtonsMode = StdButtonsMode.Edit Or _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                If Utils.ToInt32(Request.Form(CType(provider, InPlaceEstablishmentSelector).HiddenFieldUniqueID)) = 0 _
                        AndAlso _estabID = 0 Then
                    WebUtils.RecursiveDisable(domContract.Controls, True)
                Else
                    WebUtils.RecursiveDisable(domContract.Controls, False)
                End If

            End If

            '++ Enable the controls listed as editable when the service order is locked..
            If _stdBut.ButtonsMode = StdButtonsMode.Edit AndAlso _showPadlock _
                    AndAlso (_externalFields IsNot Nothing) AndAlso (_externalFields.Count > 0) Then
                Dim selProjectCode As Integer = Utils.ToInt32(cboProjectCode.DropDownList.SelectedIndex)

                '++ Disable all fields by default..
                WebUtils.RecursiveDisable(tabAttendanceSummary.Controls, True)
                WebUtils.RecursiveDisable(tabExpenditure.Controls, True)
                WebUtils.RecursiveDisable(tabFunding.Controls, True)
                WebUtils.RecursiveDisable(tabHeader.Controls, True)
                WebUtils.RecursiveDisable(tabSummary.Controls, True)
                WebUtils.RecursiveDisable(tabSuspensions.Controls, True)
                WebUtils.RecursiveDisable(tabVisits.Controls, True)

                For Each extField As External_Field In _externalFields
                    Select Case extField.ExternalField
                        Case "ProjectCode"
                            WebUtils.RecursiveDisable(cboProjectCode.Controls, False)
                        Case "Comment"
                            WebUtils.RecursiveDisable(txtComment.Controls, False)
                        Case "FinanceCode"
                            WebUtils.RecursiveDisable(lblFinanceCode.Controls, False)
                            WebUtils.RecursiveDisable(txtFinanceCode.Controls, False)
                        Case "Team"
                            WebUtils.RecursiveDisable(lblTeam.Controls, False)
                            WebUtils.RecursiveDisable(team.Controls, False)
                        Case "ClientGroup"
                            WebUtils.RecursiveDisable(lblClientGroup.Controls, False)
                            WebUtils.RecursiveDisable(clientGroup.Controls, False)
                        Case "ClientSubGroup"
                            WebUtils.RecursiveDisable(lblClientSubGroup.Controls, False)
                            WebUtils.RecursiveDisable(clientSubGroup.Controls, False)
                        Case "CareManager"
                            WebUtils.RecursiveDisable(lblCareManager.Controls, False)
                            WebUtils.RecursiveDisable(careManager.Controls, False)
                        Case "AcceptedAssessmentAnomaly"
                            WebUtils.RecursiveDisable(chkExcludeFromAnomalies.Controls, False)
                        Case Else
                    End Select
                Next
            End If

            js = New StringBuilder()
            With js
                ' output a JS collection of (available) rate category IDs together with their "For use with Visit Based Returns" and UoM values
                ' used in the cboRateCategory_Change() JS method
                .Append("Edit_rateCategories=new Collection();")
                If Not _rateCategories Is Nothing Then
                    For Each rc As DomRateCategory In _rateCategories
                        Dim framType As New FrameworkType(Me.DbConnection, String.Empty, String.Empty)
                        Dim rateCategoryVisitBased As Boolean = False
                        Dim rateFram As New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
                        msg = rateFram.Fetch(rc.DomRateFrameworkID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If rateFram.FrameworkTypeId > 0 Then
                            msg = framType.Fetch(rateFram.FrameworkTypeId)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            rateCategoryVisitBased = (framType.Abbreviation = "V")
                        End If
                        .AppendFormat("Edit_rateCategories.add({0},new Array({1},{2},{3},{4}));", _
                                      rc.ID, _
                                      rateCategoryVisitBased.ToString().ToLower(), _
                                      rc.DomUnitsOfMeasureID, _
                                      Utils.ToInt32(rc.DomServiceTypeID), _
                                      rc.OneUnitPerOrder.ToString().ToLower())
                    Next
                End If

                ' output a JS collection of units of measure IDs together with their Description/Comment values
                ' used in the cboRateCategory_Change() JS method
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

                ' output a JS collection of end reason IDs together with their Type value
                ' used in the cboEndReason_Change() JS method
                .Append("Edit_endReasons=new Collection();")
                If Not _endReasons Is Nothing Then
                    For Each er As DomServiceOrderEndReason In _endReasons
                        .AppendFormat("Edit_endReasons.add({0},{1});", _
                                      er.ID, _
                                      er.Type)
                    Next
                End If

                If Not _frType Is Nothing Then
                    If _frType.ID = FrameworkTypes.ServiceRegister Then
                        'Pass Effective Dates to Javascript
                        msg = ServiceOrderBL.FetchDomServiceOrderAttendanceAffectiveDates(Me.DbConnection, _order.ID, attendanceEffectiveDates)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        ' output a JS collection of Effective Dates
                        ' used in the btnDeleteAttendance_Click() JS method
                        .Append("Edit_AttendanceEffectiveDates=new Collection();")
                        If Not attendanceEffectiveDates Is Nothing Then
                            Dim counter As Integer = 0
                            For Each effectiveDate As Date In attendanceEffectiveDates
                                counter += 1
                                .AppendFormat("Edit_AttendanceEffectiveDates.add({0}, '{1}');", _
                                             counter, effectiveDate.Date)
                            Next
                        End If
                    End If
                End If
                .AppendFormat("Edit_showDayOfWeekColumn={0};Edit_showSvcUserMinutesColumn={1};Edit_showVisitColumn={2};cboEndReason_Changed();dsoID={3};Edit_InEditMode={4};", _
                                Me.ShowDayOfWeekColumn.ToString().ToLower(), _
                                Me.ShowSvcUserMinutesColumn.ToString().ToLower(), _
                                Me.ShowVisitsColumn.ToString().ToLower(), _
                                _dsoID, inEditMode.ToString().ToLower() _
                )
                If Not _order Is Nothing Then .AppendFormat("Edit_clientID={0};", _order.ClientID)
            End With

            _imgPadlock.Visible = _showPadlock

            If hidAttendanceHasDetail.Value = "True" Then
                Dim svcGroup As ServiceGroup = New ServiceGroup(Me.DbConnection, String.Empty, String.Empty)
                msg = svcGroup.Fetch(_contract.ServiceGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                Dim frType As FrameworkType = Nothing
                msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DbConnection, _contract.DomRateFrameworkID, frType)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If _canAmendAttendance Then
                    fldButtons.Visible = True
                    'Were only intersted if this is an attentdance style order
                    If frType.ID = FrameworkTypes.ServiceRegister Then
                        'get the list of effective dates of schedules for this order. 
                        'there is at least one attendance record
                        If attendanceEffectiveDates.Count > 0 Then
                            'has the edit button on the attendance tab been clicked?
                            If hid_AttendanceEditClicked.Value = "True" Or hid_AttendanceNewClicked.Value = "True" Then
                                'Edit button has been clicked
                                btnAttendanceEdit.Visible = False
                                btnAttendanceDelete.Visible = False
                                btnAttendanceSave.Visible = True
                                divAttendanceNavigation.Visible = False
                                'Enable controls on the attendance tab
                                WebUtils.RecursiveDisable(tabAttendance.Controls, False)
                                'disable the effective date if we are editing
                                If hid_AttendanceEditClicked.Value = "True" AndAlso _
                                    hid_AttendanceNewClicked.Value <> "True" Then
                                    WebUtils.RecursiveDisable(divEffectiveFrom.Controls, True)
                                End If

                                btnNextSchedule.Enabled = False
                                btnPreviousSchedule.Enabled = False
                            Else
                                'Edit button has not been clicked
                                btnAttendanceNew.Visible = True
                                btnAttendanceEdit.Visible = True
                                btnAttendanceSave.Visible = False
                                divAttendanceNavigation.Visible = True
                                'Only show the Attendance delete button if we are looking at the last attendance record. 
                                'If (attendanceEffectiveDates(attendanceEffectiveDates.Count - 1) = hidCurrentEffectiveDate.Value) Then
                                '    btnAttendanceDelete.Visible = True
                                '    btnAttendanceDelete.Disabled = False
                                'Else
                                '    btnAttendanceDelete.Visible = False
                                '    btnAttendanceDelete.Disabled = True
                                'End If

                                WebUtils.RecursiveDisable(tabAttendance.Controls, True)

                                btnNextSchedule.Enabled = True
                                btnPreviousSchedule.Enabled = True
                            End If

                            fldButtons.Disabled = False

                            divEffectiveFrom.Visible = True
                            divRevised.Visible = True

                            WebUtils.RecursiveDisable(fldButtons.Controls, False)
                            If hid_AttendanceNewClicked.Value = "True" Then
                                btnAttendanceNew.Enabled = False
                            End If
                            btnAttendanceViewPlan.Visible = True
                            btnAttendanceViewPlan.Disabled = False
                            OutputPlannedAttendance()

                        Else
                            fldButtons.Visible = False
                            WebUtils.RecursiveDisable(fldButtons.Controls, True)
                            divEffectiveFrom.Visible = False
                            divRevised.Visible = False
                            divAttendanceNavigation.Visible = False
                            btnAttendanceViewPlan.Visible = False
                            btnAttendanceViewPlan.Disabled = True
                        End If

                    End If
                Else
                    OutputPlannedAttendance()
                    fldButtons.Visible = False
                End If
                'sort out the navigation buttons
                If lblCurrentRecordNo.Text = lblTotalNoRecords.Text Then
                    btnNextSchedule.Enabled = False
                    btnNextSchedule.Style.Add("cursor", "default")
                End If

                If lblCurrentRecordNo.Text = "1" Then
                    btnPreviousSchedule.Enabled = False
                    btnPreviousSchedule.Style.Add("cursor", "default")
                    btnAttendanceDates.Disabled = True
                End If

                If lblTotalNoRecords.Text = "1" Then
                    btnNextSchedule.Enabled = False
                    btnPreviousSchedule.Enabled = False
                    btnPreviousSchedule.Style.Add("cursor", "default")
                    btnNextSchedule.Style.Add("cursor", "default")
                End If

                If _stdBut.ButtonsMode = StdButtonsMode.Edit Or _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                    fldButtons.Visible = False
                End If
            End If

            hidCurrentEffectiveDate.Disabled = False

            js.Append(_startup2JS.ToString())

            Dim forceRefreshParentWindow As Nullable(Of Boolean) = Utils.ToBoolean(Request.QueryString("refreshParent"))

            If IsPopupScreen AndAlso (_refreshParentWindow OrElse (forceRefreshParentWindow.HasValue AndAlso forceRefreshParentWindow.Value = True)) Then
                ' if we have flagged the page to refresh the 
                ' parent window then output some js to do so

                js.Append("if (window.opener.ServiceOrderSelector_Refresh) { window.opener.ServiceOrderSelector_Refresh(); }")

                If _stdBut.SelectedItemID = 0 Then
                    ' if this value is 0 then we have deleted the record so 
                    ' we might as well close the parent window

                    js.Append("window.close();")

                End If

            End If

            'hide the buttons on the DSO funding control if we are editing the svc order.
            CType(dsoFunding, ServiceOrderFunding).hideButtons(inEditMode)

            If weHaveFundingRecords Then
                js.Append("showSvcOrderFundingPanel();")
            End If
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

            msg = CopyServiceOrderDetailsAndFinancials()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If AutoGenerateOrderReference Then
                ' if we want to auto generate order refs then disable input

                txtOrderRef.TextBox.Enabled = False
                txtOrderRef.TextBox.ToolTip = "This field cannot be amended as order references are automatically generated"

            End If

            If (inEditMode Or _order Is Nothing) Or (Not _frType Is Nothing AndAlso _frType.ID = FrameworkTypes.ServiceRegister) Then
                divFilterDate.Style.Add("display", "none")
                divDontFilterCommitmentSummary.Visible = False
                divDontFilterCommitmentVisit.Visible = False
                chkDontFilterCommitmentSummary.Visible = False
                chkDontFilterCommitmentVisit.Visible = False
            Else
                divFilterDate.Style.Add("display", "block")
                WebUtils.RecursiveDisable(txtVisitsFilterDate.Controls, False)
                txtVisitsFilterDate.Width = New Unit(6, UnitType.Em)
                If (Not _order Is Nothing AndAlso _order.VisitBased) Then
                    divDontFilterCommitmentSummary.Visible = False
                    divDontFilterCommitmentVisit.Visible = True
                    chkDontFilterCommitmentSummary.Visible = False
                    chkDontFilterCommitmentVisit.Visible = True
                Else
                    divDontFilterCommitmentSummary.Visible = True
                    divDontFilterCommitmentVisit.Visible = False
                    chkDontFilterCommitmentSummary.Visible = True
                    chkDontFilterCommitmentVisit.Visible = False
                End If
            End If

            If (Not _contract Is Nothing AndAlso _contract.AllowProviderUnitCostOverride) AndAlso _canOverrideUnitCosts Then
                btnOverrideUnitCosts.Visible = True
                btnOverrideUnitCosts.Disabled = False
            Else
                btnOverrideUnitCosts.Visible = False
            End If
        End Sub

#End Region

#Region " PopulateAfterPostback "

        Protected Sub PopulateAfterPostback()
            With _order
                SetupProviderSelector(.ProviderID)
                SetupContractSelector(.DomContractID, .ProviderID)
                SetupClientSelector(.ClientID)
                SetupDSOAdditionalDetails()
                SetupValidators(_order)
                txtOrderRef.Text = .OrderReference
                dteDateFrom.Text = .DateFrom
                If .DateTo <> DataUtils.MAX_DATE Then
                    PopulateDropdowns(.ID)
                    dteDateTo.Text = .DateTo
                    cboEndReason.DropDownList.SelectedValue = .DomServiceOrderEndReasonID
                End If
                CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode
                'CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode2

                'CType(pct, InPlacePctSelector).PctID = .PrimaryCareTrustID
                CType(careManager, InPlaceCareManagerSelector).CareManagerID = .CareManagerID
                CType(team, InPlaceTeamSelector).TeamID = .TeamID
                CType(clientGroup, InPlaceClientGroupSelector).ClientGroupID = .ClientGroupID
                CType(clientSubGroup, InPlaceClientSubGroupSelector).ClientSubGroupID = .ClientSubGroupID
                'SetProjectCode(.ProjectCode)
                txtComment.Text = .Comment
                chkExcludeFromAnomalies.CheckBox.Checked = .AcceptedAssessAnomaly
            End With

            'Populate funding
            PopulateFundingTab(_order.ID)

            ' populate the expenditure tab
            PopulateExpenditureTab(_order.ID)

            ' populate order suspensions tab
            PopulateSuspensions(_order.ID)
        End Sub

#End Region

#Region "CopyServiceOrderDetailsAndFinancials"

        ''' <summary>
        ''' Copies service order details and financials from another order
        ''' if the property CopyFromID is not nothing. This does not commit any 
        ''' changes to the database, just copies detials from one service order to 
        ''' the controls of this page. If a value for CopyFromID is not specified then 
        ''' the returned ErrorMessage success will be equal to True
        ''' </summary>
        ''' <remarks></remarks>
        ''' <returns>ErrorMessage object indicating success</returns>
        Private Function CopyServiceOrderDetailsAndFinancials() As ErrorMessage

            Dim msg As ErrorMessage

            ' if we have an order to copy from continue
            If CopyFromID.HasValue Then

                Dim orderToCopyFrom As DomServiceOrder

                orderToCopyFrom = New DomServiceOrder(Me.DbConnection, String.Empty, String.Empty)
                msg = orderToCopyFrom.Fetch(CopyFromID.Value)

                ' if we found the specified order to copy
                If msg.Success Then

                    ' setup values for order tab
                    SetupContractSelector(orderToCopyFrom.DomContractID, orderToCopyFrom.ProviderID)
                    SetupProviderSelector(orderToCopyFrom.ProviderID)
                    SetupClientSelector(orderToCopyFrom.ClientID)
                    SetupDSOAdditionalDetails()
                    SetProjectCode(orderToCopyFrom.ProjectCode)
                    WebUtils.RecursiveDisable(provider.Controls, True)
                    WebUtils.RecursiveDisable(domContract.Controls, True)

                    ' setup values for financial tab
                    CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = orderToCopyFrom.FinanceCode
                    'CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = orderToCopyFrom.FinanceCode2
                    'CType(pct, InPlacePctSelector).PctID = orderToCopyFrom.PrimaryCareTrustID
                    CType(careManager, InPlaceCareManagerSelector).CareManagerID = orderToCopyFrom.CareManagerID
                    CType(team, InPlaceTeamSelector).TeamID = orderToCopyFrom.TeamID
                    CType(clientGroup, InPlaceClientGroupSelector).ClientGroupID = orderToCopyFrom.ClientGroupID
                    CType(clientSubGroup, InPlaceClientSubGroupSelector).ClientSubGroupID = orderToCopyFrom.ClientSubGroupID

                End If

            Else
                ' else no id to copy from specified so return a true message

                msg = New ErrorMessage()
                msg.Success = True

            End If

            Return msg

        End Function

#End Region

#Region "SetProjectCode"

        ''' <summary>
        ''' Sets the project code
        ''' </summary>
        ''' <param name="code">The code to set</param>
        ''' <remarks></remarks>
        Private Sub SetProjectCode(ByVal code As String)

            If cboProjectCode.DropDownList.Items.FindByValue(code) Is Nothing Then
                ' if the item is not found then it is probably redundant, select all redundant codes
                ' and see if it is available to add to order

                Dim redundantCodes As New ProjectCodeCollection()
                Dim redundantCode As ProjectCode = Nothing

                ProjectCode.FetchList(conn:=DbConnection, _
                                      auditLogTitle:=String.Empty, _
                                      auditUserName:=String.Empty, _
                                      code:=code, _
                                      list:=redundantCodes, _
                                      redundant:=TriState.True)

                If redundantCodes.Count > 0 Then
                    ' if we have some redundant code

                    For Each rCode As ProjectCode In redundantCodes
                        ' loop each redundant code

                        If String.Compare(rCode.Code, code, True) = 0 Then
                            ' if the codes match then exit

                            redundantCode = rCode
                            Exit For

                        End If

                    Next

                End If

                If Not redundantCode Is Nothing Then
                    ' if we found that this code is redundant add to drop down list

                    cboProjectCode.DropDownList.Items.Add(New ListItem(redundantCode.Description, _
                                                                         redundantCode.Code))

                End If

            End If

            cboProjectCode.DropDownList.SelectedValue = code

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the service order id to copy from, if one exists in the request query string (cid)
        ''' </summary>
        ''' <value>The service order id to copy from</value>
        ''' <returns>The service order id to copy from, returns Nothing if no order to copy from</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CopyFromID() As Nullable(Of Integer)
            Get
                Dim tmpCopyFromID As String = Request.QueryString("cid")
                Dim testInt As Integer

                ' try and convert cid into a valid integer and return it
                If String.IsNullOrEmpty(tmpCopyFromID) = False _
                    AndAlso tmpCopyFromID.Trim().Length > 0 _
                    AndAlso Integer.TryParse(tmpCopyFromID, testInt) Then

                    Return testInt

                Else

                    Return Nothing

                End If

            End Get
        End Property

        ''' <summary>
        ''' Gets if order references should be auto generated rather than manually entered
        ''' </summary>
        ''' <value></value>
        ''' <returns>If order references should be auto generated</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AutoGenerateOrderReference() As Boolean
            Get

                If Not _AutoGenerateOrderReference.HasValue Then

                    Dim msg As ErrorMessage
                    Dim settings As New Target.Library.Collections.ApplicationSettingCollection()
                    Dim settingKey As String = "AutoGenerateServiceOrderRefs"

                    msg = ApplicationSetting.FetchList(conn:=DbConnection, _
                                                       applicationID:=2, _
                                                       auditLogTitle:=String.Empty, _
                                                       auditUserName:=String.Empty, _
                                                       list:=settings, _
                                                       settingKey:=settingKey)

                    If msg.Success Then

                        If settings.Count > 0 Then

                            _AutoGenerateOrderReference = (String.Compare(settings(0).SettingValue, _
                                                                          "true", _
                                                                          True) _
                                                                          = 0)

                        Else

                            _AutoGenerateOrderReference = False

                        End If

                    Else

                        WebUtils.DisplayError(msg)

                    End If

                End If

                Return _AutoGenerateOrderReference.Value

            End Get
        End Property

#End Region

#Region " Attendance Schedule Tab "

#Region "           GetAttendanceUniqueIDsFromViewState "

        Private Function GetAttendanceUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_ATTENDANCE) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_ATTENDANCE), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_ATTENDANCE) Is Nothing Then
                _newAttendanceIDCounter = 0
            Else
                _newAttendanceIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_ATTENDANCE), Integer)
            End If

            Return list

        End Function

#End Region

#Region "           OutputAttendanceControls "

        Private Sub OutputAttendanceControls(ByVal uniqueID As String, ByVal attendance As vwDomServiceOrderAttendance)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim cboRateCat As DropDownListEx
            Dim lblMeasuredIn As Label
            Dim chkMon As CheckBoxEx = Nothing, chkTue As CheckBoxEx = Nothing, chkWed As CheckBoxEx = Nothing, chkThu As CheckBoxEx = Nothing
            Dim chkFri As CheckBoxEx = Nothing, chkSat As CheckBoxEx = Nothing, chkSun As CheckBoxEx = Nothing
            Dim cboFrequency As DropDownListEx, cboFirstWeek As DropDownListEx
            Dim btnRemove As ImageButton 'HtmlInputImage
            Dim unitsCount As Int16 = 0


            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_ATTENDANCE) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phAttendance.Controls.Add(row)

                ' Rate Category
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboRateCat = New DropDownListEx()
                With cboRateCat
                    .ID = CTRL_PREFIX_ATT_RATE_CAT & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "SaveAttendance"
                    LoadRateCategoryDropdown(cboRateCat)
                    If Not attendance Is Nothing Then .DropDownList.SelectedValue = attendance.DomRateCategoryID
                End With
                cell.Controls.Add(cboRateCat)
                cboRateCat.DropDownList.Attributes.Add("onchange", String.Format("cboRateCatAtt_Change('{0}');", cboRateCat.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboRateCatAtt_Change('{0}');}}", cboRateCat.ClientID)

                'If ShowDayOfWeekColumn() Then
                ' days
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")

                ' mon
                chkMon = New CheckBoxEx()
                With chkMon
                    .ID = CTRL_PREFIX_ATT_MON & uniqueID
                    .CheckBox.Text = DayOfWeek.Monday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then .CheckBox.Checked = attendance.OnMonday
                End With
                cell.Controls.Add(chkMon)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' tue
                chkTue = New CheckBoxEx()
                With chkTue
                    .ID = CTRL_PREFIX_ATT_TUE & uniqueID
                    .CheckBox.Text = DayOfWeek.Tuesday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then .CheckBox.Checked = attendance.OnTuesday
                End With
                cell.Controls.Add(chkTue)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' wed
                chkWed = New CheckBoxEx()
                With chkWed
                    .ID = CTRL_PREFIX_ATT_WED & uniqueID
                    .CheckBox.Text = DayOfWeek.Wednesday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then .CheckBox.Checked = attendance.OnWednesday
                End With
                cell.Controls.Add(chkWed)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' thu
                chkThu = New CheckBoxEx()
                With chkThu
                    .ID = CTRL_PREFIX_ATT_THU & uniqueID
                    .CheckBox.Text = DayOfWeek.Thursday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then .CheckBox.Checked = attendance.OnThursday
                End With
                cell.Controls.Add(chkThu)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' fri
                chkFri = New CheckBoxEx()
                With chkFri
                    .ID = CTRL_PREFIX_ATT_FRI & uniqueID
                    .CheckBox.Text = DayOfWeek.Friday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then .CheckBox.Checked = attendance.OnFriday
                End With
                cell.Controls.Add(chkFri)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' sat
                chkSat = New CheckBoxEx()
                With chkSat
                    .ID = CTRL_PREFIX_ATT_SAT & uniqueID
                    .CheckBox.Text = DayOfWeek.Saturday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then .CheckBox.Checked = attendance.OnSaturday
                End With
                cell.Controls.Add(chkSat)
                cell.Controls.Add(CreateVisitDaysSpacer())
                ' sun
                chkSun = New CheckBoxEx()
                With chkSun
                    .ID = CTRL_PREFIX_ATT_SUN & uniqueID
                    .CheckBox.Text = DayOfWeek.Sunday.ToString().Substring(0, 2)
                    .CheckBox.TextAlign = TextAlign.Right
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then .CheckBox.Checked = attendance.OnSunday
                End With
                cell.Controls.Add(chkSun)

                If chkMon.CheckBox.Checked Then unitsCount += 1
                If chkTue.CheckBox.Checked Then unitsCount += 1
                If chkWed.CheckBox.Checked Then unitsCount += 1
                If chkThu.CheckBox.Checked Then unitsCount += 1
                If chkFri.CheckBox.Checked Then unitsCount += 1
                If chkSat.CheckBox.Checked Then unitsCount += 1
                If chkSun.CheckBox.Checked Then unitsCount += 1
                'End If

                ' units
                'If ShowDayOfWeekColumn() Then
                Dim txtUnits As Label
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                txtUnits = New Label
                With txtUnits
                    .ID = CTRL_PREFIX_ATT_UNITS & uniqueID
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then
                        .Text = attendance.ProviderUnits
                    Else
                        .Text = unitsCount.ToString
                    End If
                End With
                cell.Controls.Add(txtUnits)

                chkMon.CheckBox.Attributes.Add("onclick", String.Format("days_Change('{0}');", txtUnits.ClientID))
                chkTue.CheckBox.Attributes.Add("onclick", String.Format("days_Change('{0}');", txtUnits.ClientID))
                chkWed.CheckBox.Attributes.Add("onclick", String.Format("days_Change('{0}');", txtUnits.ClientID))
                chkThu.CheckBox.Attributes.Add("onclick", String.Format("days_Change('{0}');", txtUnits.ClientID))
                chkFri.CheckBox.Attributes.Add("onclick", String.Format("days_Change('{0}');", txtUnits.ClientID))
                chkSat.CheckBox.Attributes.Add("onclick", String.Format("days_Change('{0}');", txtUnits.ClientID))
                chkSun.CheckBox.Attributes.Add("onclick", String.Format("days_Change('{0}');", txtUnits.ClientID))
                _startup2JS.AppendFormat("days_Change('{0}');", txtUnits.ClientID)

                'Label
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lblMeasuredIn = New Label
                With lblMeasuredIn
                    .ID = CTRL_PREFIX_ATT_MEASURED_IN & uniqueID
                End With
                cell.Controls.Add(lblMeasuredIn)

                ' frequency
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboFrequency = New DropDownListEx()
                With cboFrequency
                    .ID = CTRL_PREFIX_ATT_FREQUENCY & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "*"
                    .ValidationGroup = "SaveAttendance"
                    LoadFrequencyDropdown(cboFrequency)
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then
                        .DropDownList.SelectedValue = attendance.Frequency
                    Else
                        .DropDownList.SelectedValue = "1"
                    End If
                End With
                cell.Controls.Add(cboFrequency)
                cboFrequency.DropDownList.Attributes.Add("onchange", String.Format("cboFrequency_Change('{0}');", cboFrequency.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboFrequency_Change('{0}');}}", cboFrequency.ClientID)

                ' first week
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboFirstWeek = New DropDownListEx()
                With cboFirstWeek
                    .ID = CTRL_PREFIX_ATT_FIRST_WEEK & uniqueID
                    .ValidationGroup = "SaveAttendance"
                    LoadFirstWeekofServiceDropdown(cboFirstWeek)
                    If Not attendance Is Nothing AndAlso attendance.ID > 0 Then
                        .DropDownList.SelectedValue = attendance.FirstWeekOfService
                    Else
                        .DropDownList.SelectedValue = "0"
                    End If
                End With
                cell.Controls.Add(cboFirstWeek)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "middle")
                cell.Style.Add("text-align", "right")
                btnRemove = New ImageButton 'Button
                With btnRemove
                    .ID = CTRL_PREFIX_ATT_REMOVED & uniqueID
                    .ImageUrl = WebUtils.GetVirtualPath("images/delete.png")
                    .ValidationGroup = "RemoveAtt"
                    AddHandler .Click, AddressOf btnRemoveAttendance_Click
                End With
                cell.Controls.Add(btnRemove)

            End If

        End Sub

        Private Function CreateVisitDaysSpacer() As HtmlGenericControl
            Dim spacer As HtmlGenericControl = New HtmlGenericControl("div")
            spacer.Style.Add("width", "3px")
            spacer.Style.Add("float", "left")
            spacer.InnerHtml = "&nbsp;"
            Return spacer
        End Function

#End Region

#Region "           GetAttendanceUniqueID "

        Private Function GetAttendanceUniqueID(ByVal attendance As vwDomServiceOrderAttendance) As String

            Dim id As String

            If attendance.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_ATTENDANCE & _newAttendanceIDCounter
                _newAttendanceIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_ATTENDANCE & attendance.ID
            End If

            Return id

        End Function

#End Region

#Region "           PersistAttendanceUniqueIDsToViewState "

        Private Sub PersistAttendanceUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_ATTENDANCE, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_ATTENDANCE, _newAttendanceIDCounter)
        End Sub

#End Region

#Region "           btnAddAttendance_Click "

        Private Sub btnAddAttendance_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddAttendance.Click
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetAttendanceUniqueIDsFromViewState()
            Dim newAttendance As vwDomServiceOrderAttendance = New vwDomServiceOrderAttendance()

            If _order Is Nothing Then
                PopulateDropdowns(0)
            Else
                PopulateDropdowns(_order.ID)
            End If
            cboEndReason.SelectPostBackValue()
            cboProjectCode.SelectPostBackValue()
            'With _order
            '    SetupProviderSelector(.ProviderID)
            '    SetupContractSelector(.DomContractID, .ProviderID)
            '    SetupClientSelector(.ClientID)
            'End With
            'SetupValidators(_order)
            dteEffectiveFrom.Text = hidCurrentEffectiveDate.Value
            PopulateAfterPostback()
            PopulateAttendanceSummaryTab()

            ' add a new row to the visit list
            id = GetAttendanceUniqueID(newAttendance)
            ' create the controls
            OutputAttendanceControls(id, newAttendance)
            ' persist the data into view state
            list.Add(id)
            PersistAttendanceUniqueIDsToViewState(list)

            'tabExpenditure.Enabled = False
        End Sub

#End Region

#Region "           btnRemoveAttendance_Click "

        Private Sub btnRemoveAttendance_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetAttendanceUniqueIDsFromViewState()
            'Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_ATT_REMOVED, String.Empty)
            Dim id As String = CType(sender, ImageButton).ID.Replace(CTRL_PREFIX_ATT_REMOVED, String.Empty)

            If _order Is Nothing Then
                PopulateDropdowns(0)
            Else
                PopulateDropdowns(_order.ID)
            End If
            cboEndReason.SelectPostBackValue()
            cboProjectCode.SelectPostBackValue()
            'With _order
            '    SetupProviderSelector(.ProviderID)
            '    SetupContractSelector(.DomContractID, .ProviderID)
            '    SetupClientSelector(.ClientID)
            '    If dteDateFrom.Enabled = False Then dteDateFrom.Text = .DateFrom
            '    If txtOrderRef.Enabled = False Then txtOrderRef.Text = .OrderReference
            'End With
            'SetupValidators(_order)
            dteEffectiveFrom.Text = hidCurrentEffectiveDate.Value
            PopulateAfterPostback()

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_ATTENDANCE) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_ATTENDANCE, UNIQUEID_PREFIX_DELETE_ATTENDANCE)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phAttendance.Controls.Count - 1
                If phAttendance.Controls(index).ID = id Then
                    phAttendance.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistAttendanceUniqueIDsToViewState(list)
            'hidAttendanceHasDetail.Value = True
            'tabExpenditure.Enabled = False

        End Sub

#End Region

#Region "           BuildAttendanceCollectionsForSave "

        Protected Function BuildAttendanceCollectionsForSave(ByRef orderAttendancies As DomServiceOrderAttendanceCollection, ByRef attendanceToDelete As List(Of String)) As ErrorMessage
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim orderAttendance As DomServiceOrderAttendance = Nothing
            Dim attendanceList As List(Of String) = Nothing

            orderAttendancies = New DomServiceOrderAttendanceCollection
            attendanceToDelete = New List(Of String)
            attendanceList = GetAttendanceUniqueIDsFromViewState()
            For Each uniqueID As String In attendanceList
                If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_ATTENDANCE) Then
                    ' we are deleting
                    attendanceToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE_ATTENDANCE, String.Empty))
                Else
                    ' create the empty visit record
                    orderAttendance = New DomServiceOrderAttendance(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE_ATTENDANCE) Then
                        ' we are updating
                        msg = orderAttendance.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_ATTENDANCE, String.Empty)))
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If
                    ' set the Attendance properties
                    With orderAttendance
                        .DomRateCategoryID = Utils.ToInt32(CType(phAttendance.FindControl(CTRL_PREFIX_ATT_RATE_CAT & uniqueID), DropDownListEx).GetPostBackValue())
                        'If ShowDayOfWeekColumn() Then
                        .ProviderUnits = 0
                        If CType(phAttendance.FindControl(CTRL_PREFIX_ATT_MON & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                        If CType(phAttendance.FindControl(CTRL_PREFIX_ATT_TUE & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                        If CType(phAttendance.FindControl(CTRL_PREFIX_ATT_WED & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                        If CType(phAttendance.FindControl(CTRL_PREFIX_ATT_THU & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                        If CType(phAttendance.FindControl(CTRL_PREFIX_ATT_FRI & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                        If CType(phAttendance.FindControl(CTRL_PREFIX_ATT_SAT & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                        If CType(phAttendance.FindControl(CTRL_PREFIX_ATT_SUN & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                        '.ProviderUnits = Convert.ToInt32(CType(phAttendance.FindControl(CTRL_PREFIX_ATT_UNITS & uniqueID), Label).Text)
                        .OnMonday = CType(phAttendance.FindControl(CTRL_PREFIX_ATT_MON & uniqueID), CheckBoxEx).CheckBox.Checked
                        .OnTuesday = CType(phAttendance.FindControl(CTRL_PREFIX_ATT_TUE & uniqueID), CheckBoxEx).CheckBox.Checked
                        .OnWednesday = CType(phAttendance.FindControl(CTRL_PREFIX_ATT_WED & uniqueID), CheckBoxEx).CheckBox.Checked
                        .OnThursday = CType(phAttendance.FindControl(CTRL_PREFIX_ATT_THU & uniqueID), CheckBoxEx).CheckBox.Checked
                        .OnFriday = CType(phAttendance.FindControl(CTRL_PREFIX_ATT_FRI & uniqueID), CheckBoxEx).CheckBox.Checked
                        .OnSaturday = CType(phAttendance.FindControl(CTRL_PREFIX_ATT_SAT & uniqueID), CheckBoxEx).CheckBox.Checked
                        .OnSunday = CType(phAttendance.FindControl(CTRL_PREFIX_ATT_SUN & uniqueID), CheckBoxEx).CheckBox.Checked
                        'Else
                        '    .ProviderUnits = Convert.ToInt32(CType(phAttendance.FindControl(CTRL_PREFIX_ATT_UNITS & uniqueID), TextBoxEx).Text)
                        '    'The service needs to be put against one day.
                        '    .OnSunday = TriState.True
                        'End If

                        .Frequency = Utils.ToInt32(CType(phAttendance.FindControl(CTRL_PREFIX_ATT_FREQUENCY & uniqueID), DropDownListEx).GetPostBackValue())
                        If .Frequency = Frequency.EveryWeek Then
                            .FirstWeekOfService = 1
                        Else
                            .FirstWeekOfService = Utils.ToInt32(CType(phAttendance.FindControl(CTRL_PREFIX_ATT_FIRST_WEEK & uniqueID), DropDownListEx).GetPostBackValue())
                        End If
                    End With

                    'If the order is maintained by an external interface
                    'the effective date could be different to the order Date From
                    If dteEffectiveFrom.Text <> "" Then
                        orderAttendance.EffectiveDate = dteEffectiveFrom.Text
                    Else
                        If Not hid_AttendanceNewClicked.Value = "True" Then
                            orderAttendance.EffectiveDate = hidCurrentEffectiveDate.Value
                        Else
                            orderAttendance.EffectiveDate = Nothing
                        End If
                    End If

                    ' add to the collection
                    orderAttendancies.Add(orderAttendance)
                End If
            Next

            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

#End Region

#Region "           btnAttendanceNew_Click "

        Private Sub btnAttendanceNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAttendanceNew.Click
            Dim args As StdButtonEventArgs = New StdButtonEventArgs(False, _dsoID, _stdBut)

            hid_AttendanceNewClicked.Value = True
            hidCurrentEffectiveDate.Value = ""

            _stdBut.AllowDelete = False
            _stdBut.AllowEdit = False

            FindClicked(args)

        End Sub

#End Region

#Region "btnAttendanceEdit_Click "

        Private Sub btnAttendanceEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAttendanceEdit.Click
            Dim args As StdButtonEventArgs = New StdButtonEventArgs(False, _dsoID, _stdBut)
            hid_AttendanceEditClicked.Value = True

            _stdBut.AllowDelete = False
            _stdBut.AllowEdit = False

            FindClicked(args)
        End Sub

#End Region

#Region "           btnAttendanceSave_Click "

        Private Sub btnAttendanceSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAttendanceSave.Click
            Dim args As StdButtonEventArgs = New StdButtonEventArgs(False, _dsoID, _stdBut)
            Dim orderDetails As DomServiceOrderDetailCollection = New DomServiceOrderDetailCollection()
            Dim orderVisits As DomServiceOrderVisitCollection = New DomServiceOrderVisitCollection()
            'Dim unitCostOverrides As DomServiceOrderProviderUnitCostOverrideCollection = New DomServiceOrderProviderUnitCostOverrideCollection()
            Dim orderAttendancies As DomServiceOrderAttendanceCollection = New DomServiceOrderAttendanceCollection
            Dim attendanceToDelete As List(Of String) = Nothing
            Dim sumToDelete As List(Of String) = Nothing
            Dim visitToDelete As List(Of String) = Nothing
            'Dim expToDelete As List(Of String) = Nothing
            Dim options As SaveServiceOrderOptions
            Dim msg As ErrorMessage
            Dim attendanceEffectiveDates As List(Of Date) = Nothing
            Dim effectiveDateChanged As Boolean = False

            _attendanceEditClicked = True
            'the fact we have got here means that the order has detail
            'hidAttendanceHasDetail.Value = True

            If dteEffectiveFrom.Text <> "" Then
                effectiveDateChanged = (hidOriginalEffectiveDate.Value <> dteEffectiveFrom.Text)
            Else
                effectiveDateChanged = (hidOriginalEffectiveDate.Value <> hidCurrentEffectiveDate.Value)
            End If

            msg = BuildAttendanceCollectionsForSave(orderAttendancies, attendanceToDelete)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' validate/save the order
            options = New SaveServiceOrderOptions()
            With options
                .EditingAttendanceOnly = True
                .UsingElectronicInterface = False
                .CanOverrrideProviderUnitCosts = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.OverrideProviderUnitCosts"))
                .IgnoreEffectiveDateValidation = Not effectiveDateChanged '(hidEffectiveDateEnabled.Value = "False")
            End With

            If effectiveDateChanged Then
                'fetch a distinct list of attendance effective dates for this order
                msg = ServiceOrderBL.FetchDomServiceOrderAttendanceAffectiveDates(Me.DbConnection, _order.ID, attendanceEffectiveDates)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                Dim dateFoundInCollection As Boolean = False
                For Each effDate As Date In attendanceEffectiveDates
                    For Each attendance As DomServiceOrderAttendance In orderAttendancies
                        If effDate = attendance.EffectiveDate Then
                            dateFoundInCollection = True
                            Exit For
                        End If
                    Next
                Next

                If Not dateFoundInCollection Or effectiveDateChanged Then
                    For Each attendance As DomServiceOrderAttendance In orderAttendancies
                        attendance.Unhook()
                    Next
                    attendanceToDelete = New List(Of String)
                End If
            End If

            msg = ServiceOrderBL.SaveServiceOrder(Me.DbConnection, Nothing, _order, _
                                                 orderDetails, sumToDelete, _
                                                 orderVisits, visitToDelete, _
                                                 orderAttendancies, attendanceToDelete, _
                                                 options, Me.Settings.CurrentApplicationID)
            If Not msg.Success Then
                If msg.Number = ServiceOrderBL.ERR_COULD_NOT_SAVE_DSO Or msg.Number = DomContractConvertTimeToUnits.ERR_COULD_NOT_CATEGORISE Then
                    ' could not save DSO or could not categorise visits
                    lblError.Text = msg.Message.Replace(vbCrLf, "<br />")
                Else
                    WebUtils.DisplayError(msg)
                End If

                'If dteEffectiveFrom.Text = "" Then dteEffectiveFrom.Text = hidCurrentEffectiveDate.Value
                UpdateNavigationControls()
                PopulateAfterPostback()
            Else
                If msg.Message <> String.Empty Then
                    lblWarning.Text = msg.Message
                End If

                If dteEffectiveFrom.Text <> "" Then
                    hidCurrentEffectiveDate.Value = dteEffectiveFrom.Text
                End If
                hid_AttendanceEditClicked.Value = ""
                hid_AttendanceNewClicked.Value = ""
                FindClicked(args)
            End If

        End Sub

#End Region

#Region "           btnAttendanceCancel_Click "

        Private Sub btnAttendanceCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAttendanceCancel.Click
            Dim args As StdButtonEventArgs = New StdButtonEventArgs(False, _dsoID, _stdBut)
            hid_AttendanceEditClicked.Value = False
            hid_AttendanceNewClicked.Value = False
            FindClicked(args)
        End Sub

#End Region

#Region "           btnNextSchedule_Click "

        Private Sub btnNextSchedule_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnNextSchedule.Click
            AttendanceNavigation(False, True)
        End Sub

#End Region

#Region "           btnPreviousSchedule_Click "

        Private Sub btnPreviousSchedule_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnPreviousSchedule.Click
            AttendanceNavigation(True, False)
        End Sub

#End Region

#Region "           UpdateNavigationControls "

        Protected Sub UpdateNavigationControls()
            Dim currentRecordPos As Integer = 0
            Dim msg As ErrorMessage
            Dim attendanceEffectiveDates As List(Of Date) = Nothing

            msg = ServiceOrderBL.FetchDomServiceOrderAttendanceAffectiveDates(Me.DbConnection, _order.ID, attendanceEffectiveDates)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each effectiveDate As Date In attendanceEffectiveDates
                currentRecordPos += 1
                If effectiveDate = hidCurrentEffectiveDate.Value Then Exit For
            Next

            lblCurrentRecordNo.Text = currentRecordPos.ToString
            lblTotalNoRecords.Text = attendanceEffectiveDates.Count.ToString

        End Sub

#End Region

#Region "           AttendanceNavigation "

        Protected Sub AttendanceNavigation(ByVal movePrevious As Boolean, ByVal moveNext As Boolean)
            Dim currentRecordPos As Integer = 0
            Dim msg As ErrorMessage
            Dim AttendanceSchedules As vwDomServiceOrderAttendanceCollection = Nothing
            Dim attendanceList As New List(Of String)
            Dim attendanceEffectiveDates As List(Of Date) = Nothing
            Dim nextEffectiveDate As Date

            msg = ServiceOrderBL.FetchDomServiceOrderAttendanceAffectiveDates(Me.DbConnection, _order.ID, attendanceEffectiveDates)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If movePrevious Then
                nextEffectiveDate = attendanceEffectiveDates(0)
                For Each effectiveDate As Date In attendanceEffectiveDates
                    If Convert.ToDateTime(effectiveDate) = Convert.ToDateTime(hidCurrentEffectiveDate.Value) Then Exit For
                    nextEffectiveDate = effectiveDate
                    currentRecordPos += 1
                Next
                If currentRecordPos = 0 Then currentRecordPos += 1
                hidCurrentEffectiveDate.Value = nextEffectiveDate
            ElseIf moveNext Then
                nextEffectiveDate = attendanceEffectiveDates(attendanceEffectiveDates.Count - 1)
                For Each effectiveDate As Date In attendanceEffectiveDates
                    nextEffectiveDate = effectiveDate
                    currentRecordPos += 1
                    If Convert.ToDateTime(nextEffectiveDate) > Convert.ToDateTime(hidCurrentEffectiveDate.Value) Then Exit For
                Next
                hidCurrentEffectiveDate.Value = nextEffectiveDate

            End If

            lblCurrentRecordNo.Text = currentRecordPos.ToString
            lblTotalNoRecords.Text = attendanceEffectiveDates.Count.ToString

            dteEffectiveFrom.Text = nextEffectiveDate

            ' refresh the list of existing attendance Schedules and save in View State
            msg = vwDomServiceOrderAttendance.FetchList(Me.DbConnection, AttendanceSchedules, _order.ID, nextEffectiveDate)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ViewState.Remove(VIEWSTATE_KEY_DATA_ATTENDANCE)
            phAttendance.Controls.Clear()
            'attendanceList = GetAttendanceUniqueIDsFromViewState()
            phAttendance.Controls.Clear()
            For Each attendance As vwDomServiceOrderAttendance In AttendanceSchedules
                ID = GetAttendanceUniqueID(attendance)
                OutputAttendanceControls(ID, attendance)
                attendanceList.Add(ID)
                'hidAttendanceHasDetail.Value = True
            Next
            PersistAttendanceUniqueIDsToViewState(attendanceList)

            PopulateAfterPostback()
            PopulateAttendanceSummaryTab()

            'OutputPlannedAttendance()
        End Sub

#End Region

#Region "           OutputPlannedAttendance "

        Protected Sub OutputPlannedAttendance()
            Dim measuredInTotals As SortedDictionary(Of String, Integer) = New SortedDictionary(Of String, Integer)
            Dim dsoDetails As DomServiceOrderDetailCollection = Nothing
            Dim msg As ErrorMessage

            msg = DomServiceOrderDetail.FetchList(Me.DbConnection, dsoDetails, String.Empty, String.Empty, _order.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            'Build up a collection of 'Measured in' and provider units accociated to that.
            msg = ServiceOrderBL.BuildMeasuredInCollectionFromDSODetail(Me.DbConnection, dsoDetails, measuredInTotals)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'Output the Planned units to the UI
            divPlan.Controls.Clear()

            'also output the colection to Javascript
            _startup2JS.Append("plannedAttendance=new Collection();")



            For Each uomKey As String In measuredInTotals.Keys
                Dim measuredInLabel As Label = New Label
                Dim unitsLabel As Label = New Label

                With measuredInLabel
                    .Text = uomKey
                    .Style.Add("padding-right", "0.5em")
                End With
                divPlan.Controls.Add(measuredInLabel)
                With unitsLabel
                    .Text = measuredInTotals(uomKey)
                    .CssClass = "warningText"
                    .Style.Add("padding-right", "2em")
                End With
                divPlan.Controls.Add(unitsLabel)

                _startup2JS.AppendFormat("plannedAttendance.add('{0}',{1});", _
                          uomKey, _
                          measuredInTotals(uomKey))
            Next

            _startup2JS.Append("set_PlannedCounts();")
        End Sub

#End Region

#Region "           btnAttendanceDelete_Click "

        Private Sub btnAttendanceDelete_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAttendanceDelete.ServerClick
            Dim msg As ErrorMessage
            Dim args As StdButtonEventArgs = New StdButtonEventArgs(False, _dsoID, _stdBut)

            msg = ServiceOrderBL.DeleteDomServiceOrderAttendanceByEffectiveDate(Me.DbConnection, _dsoID, hidCurrentEffectiveDate.Value)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
            Else
                FindClicked(args)
            End If

        End Sub

#End Region

#End Region

#Region " Attendance Summary Tab "

#Region "           OutputAttendanceSummaryControls "

        Private Sub OutputAttendanceSummaryControls(ByVal uniqueID As String, ByVal attendance As DomServiceOrderAttendance)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim cboRateCat As DropDownListEx
            Dim lblMeasuredIn As Label
            Dim span As HtmlGenericControl
            Dim chkMon As CheckBoxEx = Nothing, chkTue As CheckBoxEx = Nothing, chkWed As CheckBoxEx = Nothing, chkThu As CheckBoxEx = Nothing
            Dim chkFri As CheckBoxEx = Nothing, chkSat As CheckBoxEx = Nothing, chkSun As CheckBoxEx = Nothing
            Dim cboFrequency As DropDownListEx, cboFirstWeek As DropDownListEx
            Dim btnRemove As ImageButton 'HtmlInputImage
            Dim unitsCount As Int16 = 0


            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_SUMMARY_ATTENDANCE) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phAttendanceSummary.Controls.Add(row)

                ' Rate Category
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboRateCat = New DropDownListEx()
                With cboRateCat
                    .ID = CTRL_PREFIX_ATT_SUMMARY_RATE_CAT & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    LoadRateCategoryDropdown(cboRateCat)
                    If Not attendance Is Nothing Then .DropDownList.SelectedValue = attendance.DomRateCategoryID
                End With
                cell.Controls.Add(cboRateCat)
                cboRateCat.DropDownList.Attributes.Add("onchange", String.Format("cboRateCatAttSummary_Change('{0}');", cboRateCat.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboRateCatAttSummary_Change('{0}');}}", cboRateCat.ClientID)

                If ShowDayOfWeekColumn() Then
                    ' days
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")

                    ' mon
                    chkMon = New CheckBoxEx()
                    With chkMon
                        .ID = CTRL_PREFIX_ATT_SUMMARY_MON & uniqueID
                        .CheckBox.Text = DayOfWeek.Monday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnMonday
                    End With
                    cell.Controls.Add(chkMon)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' tue
                    chkTue = New CheckBoxEx()
                    With chkTue
                        .ID = CTRL_PREFIX_ATT_SUMMARY_TUE & uniqueID
                        .CheckBox.Text = DayOfWeek.Tuesday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnTuesday
                    End With
                    cell.Controls.Add(chkTue)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' wed
                    chkWed = New CheckBoxEx()
                    With chkWed
                        .ID = CTRL_PREFIX_ATT_SUMMARY_WED & uniqueID
                        .CheckBox.Text = DayOfWeek.Wednesday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnWednesday
                    End With
                    cell.Controls.Add(chkWed)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' thu
                    chkThu = New CheckBoxEx()
                    With chkThu
                        .ID = CTRL_PREFIX_ATT_SUMMARY_THU & uniqueID
                        .CheckBox.Text = DayOfWeek.Thursday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnThursday
                    End With
                    cell.Controls.Add(chkThu)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' fri
                    chkFri = New CheckBoxEx()
                    With chkFri
                        .ID = CTRL_PREFIX_ATT_SUMMARY_FRI & uniqueID
                        .CheckBox.Text = DayOfWeek.Friday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnFriday
                    End With
                    cell.Controls.Add(chkFri)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' sat
                    chkSat = New CheckBoxEx()
                    With chkSat
                        .ID = CTRL_PREFIX_ATT_SUMMARY_SAT & uniqueID
                        .CheckBox.Text = DayOfWeek.Saturday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnSaturday
                    End With
                    cell.Controls.Add(chkSat)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' sun
                    chkSun = New CheckBoxEx()
                    With chkSun
                        .ID = CTRL_PREFIX_ATT_SUMMARY_SUN & uniqueID
                        .CheckBox.Text = DayOfWeek.Sunday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnSunday
                    End With
                    cell.Controls.Add(chkSun)

                    If chkMon.CheckBox.Checked Then unitsCount += 1
                    If chkTue.CheckBox.Checked Then unitsCount += 1
                    If chkWed.CheckBox.Checked Then unitsCount += 1
                    If chkThu.CheckBox.Checked Then unitsCount += 1
                    If chkFri.CheckBox.Checked Then unitsCount += 1
                    If chkSat.CheckBox.Checked Then unitsCount += 1
                    If chkSun.CheckBox.Checked Then unitsCount += 1
                End If

                ' units
                If ShowDayOfWeekColumn() Then
                    Dim txtUnits As Label
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")
                    txtUnits = New Label
                    With txtUnits
                        .ID = CTRL_PREFIX_ATT_SUMMARY_UNITS & uniqueID
                        If Not attendance Is Nothing Then
                            .Text = attendance.ProviderUnits
                        Else
                            .Text = unitsCount.ToString
                        End If
                    End With
                    cell.Controls.Add(txtUnits)

                    chkMon.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkTue.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkWed.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkThu.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkFri.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkSat.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkSun.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    _startup2JS.AppendFormat("days_ChangeSummary('{0}');", txtUnits.ClientID)
                Else
                    Dim txtUnits As TextBoxEx
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")
                    txtUnits = New TextBoxEx()
                    With txtUnits
                        .ID = CTRL_PREFIX_ATT_SUMMARY_UNITS & uniqueID
                        .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                        .Width = New Unit(3, UnitType.Em)
                        '.Required = True
                        .Enabled = Not ShowDayOfWeekColumn()
                        '.RequiredValidatorErrMsg = "* Required"
                        '.ValidationGroup = "Save"
                        If Not attendance Is Nothing Then
                            .Text = attendance.ProviderUnits
                        Else
                            .Text = 0
                        End If

                        ' setup values for order tab
                        'SetupContractSelector(orderToCopyFrom.DomContractID, orderToCopyFrom.ProviderID)
                        'SetupProviderSelector(orderToCopyFrom.ProviderID)
                        'SetupClientSelector(orderToCopyFrom.ClientID)
                        'SetProjectCode(orderToCopyFrom.ProjectCode)
                        WebUtils.RecursiveDisable(provider.Controls, True)
                        WebUtils.RecursiveDisable(domContract.Controls, True)

                    End With
                    span = New HtmlGenericControl("span")
                    span.Controls.Add(txtUnits)
                    cell.Controls.Add(span)

                    '_startup2JS.AppendFormat("function {0}_Changed(){{set_PlannedCounts();}}", txtUnits.ID)

                End If
                'Label
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lblMeasuredIn = New Label
                With lblMeasuredIn
                    .ID = CTRL_PREFIX_ATT_SUMMARY_MEASURED_IN & uniqueID
                End With
                cell.Controls.Add(lblMeasuredIn)

                ' frequency
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboFrequency = New DropDownListEx()
                With cboFrequency
                    .ID = CTRL_PREFIX_ATT_SUMMARY_FREQUENCY & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "*"
                    .ValidationGroup = "Save"
                    LoadFrequencyDropdown(cboFrequency)
                    If Not attendance Is Nothing Then .DropDownList.SelectedValue = attendance.Frequency
                End With
                cell.Controls.Add(cboFrequency)
                cboFrequency.DropDownList.Attributes.Add("onchange", String.Format("cboFrequency_Change('{0}');", cboFrequency.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboFrequency_Change('{0}');}}", cboFrequency.ClientID)

                ' first week
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboFirstWeek = New DropDownListEx()
                With cboFirstWeek
                    .ID = CTRL_PREFIX_ATT_SUMMARY_FIRST_WEEK & uniqueID
                    .ValidationGroup = "Save"
                    LoadFirstWeekofServiceDropdown(cboFirstWeek)
                    If Not attendance Is Nothing AndAlso attendance.FirstWeekOfService > 0 Then .DropDownList.SelectedValue = attendance.FirstWeekOfService
                End With
                cell.Controls.Add(cboFirstWeek)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "middle")
                cell.Style.Add("text-align", "right")
                btnRemove = New ImageButton 'Button
                With btnRemove
                    .ID = CTRL_PREFIX_ATT_SUMMARY_REMOVED & uniqueID
                    .ImageUrl = WebUtils.GetVirtualPath("images/delete.png")
                    .ValidationGroup = "RemoveAtt"
                    AddHandler .Click, AddressOf btnRemoveAttendanceSummary_Click
                End With
                cell.Controls.Add(btnRemove)

            End If

        End Sub

#End Region

#Region "           OutputAttendanceSummaryControls "

        Private Sub OutputAttendanceSummaryControls(ByVal uniqueID As String, _
                                                    Optional ByVal attendance As vwDomServiceOrderAttendance = Nothing)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim cboRateCat As DropDownListEx
            Dim lblMeasuredIn As Label
            Dim span As HtmlGenericControl
            Dim chkMon As CheckBoxEx = Nothing, chkTue As CheckBoxEx = Nothing, chkWed As CheckBoxEx = Nothing, chkThu As CheckBoxEx = Nothing
            Dim chkFri As CheckBoxEx = Nothing, chkSat As CheckBoxEx = Nothing, chkSun As CheckBoxEx = Nothing
            Dim cboFrequency As DropDownListEx, cboFirstWeek As DropDownListEx
            Dim btnRemove As ImageButton 'HtmlInputImage
            Dim unitsCount As Int16 = 0


            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_SUMMARY_ATTENDANCE) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phAttendanceSummary.Controls.Add(row)

                ' Rate Category
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboRateCat = New DropDownListEx()
                With cboRateCat
                    .ID = CTRL_PREFIX_ATT_SUMMARY_RATE_CAT & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    LoadRateCategoryDropdown(cboRateCat)
                    If Not attendance Is Nothing Then .DropDownList.SelectedValue = attendance.DomRateCategoryID
                End With
                cell.Controls.Add(cboRateCat)
                cboRateCat.DropDownList.Attributes.Add("onchange", String.Format("cboRateCatAttSummary_Change('{0}');", cboRateCat.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboRateCatAttSummary_Change('{0}');}}", cboRateCat.ClientID)

                If ShowDayOfWeekColumn() Then
                    ' days
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")

                    ' mon
                    chkMon = New CheckBoxEx()
                    With chkMon
                        .ID = CTRL_PREFIX_ATT_SUMMARY_MON & uniqueID
                        .CheckBox.Text = DayOfWeek.Monday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnMonday
                    End With
                    cell.Controls.Add(chkMon)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' tue
                    chkTue = New CheckBoxEx()
                    With chkTue
                        .ID = CTRL_PREFIX_ATT_SUMMARY_TUE & uniqueID
                        .CheckBox.Text = DayOfWeek.Tuesday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnTuesday
                    End With
                    cell.Controls.Add(chkTue)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' wed
                    chkWed = New CheckBoxEx()
                    With chkWed
                        .ID = CTRL_PREFIX_ATT_SUMMARY_WED & uniqueID
                        .CheckBox.Text = DayOfWeek.Wednesday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnWednesday
                    End With
                    cell.Controls.Add(chkWed)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' thu
                    chkThu = New CheckBoxEx()
                    With chkThu
                        .ID = CTRL_PREFIX_ATT_SUMMARY_THU & uniqueID
                        .CheckBox.Text = DayOfWeek.Thursday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnThursday
                    End With
                    cell.Controls.Add(chkThu)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' fri
                    chkFri = New CheckBoxEx()
                    With chkFri
                        .ID = CTRL_PREFIX_ATT_SUMMARY_FRI & uniqueID
                        .CheckBox.Text = DayOfWeek.Friday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnFriday
                    End With
                    cell.Controls.Add(chkFri)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' sat
                    chkSat = New CheckBoxEx()
                    With chkSat
                        .ID = CTRL_PREFIX_ATT_SUMMARY_SAT & uniqueID
                        .CheckBox.Text = DayOfWeek.Saturday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnSaturday
                    End With
                    cell.Controls.Add(chkSat)
                    cell.Controls.Add(CreateVisitDaysSpacer())
                    ' sun
                    chkSun = New CheckBoxEx()
                    With chkSun
                        .ID = CTRL_PREFIX_ATT_SUMMARY_SUN & uniqueID
                        .CheckBox.Text = DayOfWeek.Sunday.ToString().Substring(0, 2)
                        .CheckBox.TextAlign = TextAlign.Right
                        If Not attendance Is Nothing Then .CheckBox.Checked = attendance.OnSunday
                    End With
                    cell.Controls.Add(chkSun)

                    If chkMon.CheckBox.Checked Then unitsCount += 1
                    If chkTue.CheckBox.Checked Then unitsCount += 1
                    If chkWed.CheckBox.Checked Then unitsCount += 1
                    If chkThu.CheckBox.Checked Then unitsCount += 1
                    If chkFri.CheckBox.Checked Then unitsCount += 1
                    If chkSat.CheckBox.Checked Then unitsCount += 1
                    If chkSun.CheckBox.Checked Then unitsCount += 1
                End If

                ' units
                If ShowDayOfWeekColumn() Then
                    Dim txtUnits As Label
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")
                    txtUnits = New Label
                    With txtUnits
                        .ID = CTRL_PREFIX_ATT_SUMMARY_UNITS & uniqueID
                        If Not attendance Is Nothing Then
                            .Text = attendance.ProviderUnits
                        Else
                            .Text = unitsCount.ToString
                        End If
                    End With
                    cell.Controls.Add(txtUnits)

                    chkMon.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkTue.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkWed.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkThu.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkFri.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkSat.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    chkSun.CheckBox.Attributes.Add("onclick", String.Format("days_ChangeSummary('{0}');", txtUnits.ClientID))
                    _startup2JS.AppendFormat("days_ChangeSummary('{0}');", txtUnits.ClientID)
                Else
                    Dim txtUnits As TextBoxEx
                    cell = New HtmlTableCell()
                    row.Controls.Add(cell)
                    cell.Style.Add("vertical-align", "top")
                    txtUnits = New TextBoxEx()
                    With txtUnits
                        .ID = CTRL_PREFIX_ATT_SUMMARY_UNITS & uniqueID
                        .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                        .Width = New Unit(3, UnitType.Em)
                        '.Required = True
                        .Enabled = Not ShowDayOfWeekColumn()
                        '.RequiredValidatorErrMsg = "* Required"
                        '.ValidationGroup = "Save"
                        If Not attendance Is Nothing Then
                            .Text = attendance.ProviderUnits
                        Else
                            .Text = 0
                        End If

                        ' setup values for order tab
                        'SetupContractSelector(orderToCopyFrom.DomContractID, orderToCopyFrom.ProviderID)
                        'SetupProviderSelector(orderToCopyFrom.ProviderID)
                        'SetupClientSelector(orderToCopyFrom.ClientID)
                        'SetProjectCode(orderToCopyFrom.ProjectCode)
                        WebUtils.RecursiveDisable(provider.Controls, True)
                        WebUtils.RecursiveDisable(domContract.Controls, True)

                    End With
                    span = New HtmlGenericControl("span")
                    span.Controls.Add(txtUnits)
                    cell.Controls.Add(span)

                    '_startup2JS.AppendFormat("function {0}_Changed(){{set_PlannedCounts();}}", txtUnits.ID)

                End If
                'Label
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                lblMeasuredIn = New Label
                With lblMeasuredIn
                    .ID = CTRL_PREFIX_ATT_SUMMARY_MEASURED_IN & uniqueID
                End With
                cell.Controls.Add(lblMeasuredIn)

                ' frequency
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboFrequency = New DropDownListEx()
                With cboFrequency
                    .ID = CTRL_PREFIX_ATT_SUMMARY_FREQUENCY & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "*"
                    .ValidationGroup = "Save"
                    LoadFrequencyDropdown(cboFrequency)
                    If Not attendance Is Nothing Then .DropDownList.SelectedValue = attendance.Frequency
                End With
                cell.Controls.Add(cboFrequency)
                cboFrequency.DropDownList.Attributes.Add("onchange", String.Format("cboFrequency_Change('{0}');", cboFrequency.ClientID))
                _startup2JS.AppendFormat("if(GetElement('{0}_cboDropDownList',true)){{cboFrequency_Change('{0}');}}", cboFrequency.ClientID)

                ' first week
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboFirstWeek = New DropDownListEx()
                With cboFirstWeek
                    .ID = CTRL_PREFIX_ATT_SUMMARY_FIRST_WEEK & uniqueID
                    .ValidationGroup = "Save"
                    LoadFirstWeekofServiceDropdown(cboFirstWeek)
                    If Not attendance Is Nothing AndAlso attendance.FirstWeekOfService > 0 Then .DropDownList.SelectedValue = attendance.FirstWeekOfService
                End With
                cell.Controls.Add(cboFirstWeek)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "middle")
                cell.Style.Add("text-align", "right")
                btnRemove = New ImageButton 'Button
                With btnRemove
                    .ID = CTRL_PREFIX_ATT_SUMMARY_REMOVED & uniqueID
                    .ImageUrl = WebUtils.GetVirtualPath("images/delete.png")
                    .ValidationGroup = "RemoveAtt"
                    AddHandler .Click, AddressOf btnRemoveAttendanceSummary_Click
                End With
                cell.Controls.Add(btnRemove)

            End If

        End Sub

#End Region

#Region "           btnRemoveAttendanceSummary_Click "

        Private Sub btnRemoveAttendanceSummary_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetAttendanceSummaryUniqueIDsFromViewState()
            Dim id As String = CType(sender, ImageButton).ID.Replace(CTRL_PREFIX_ATT_SUMMARY_REMOVED, String.Empty)

            If _order Is Nothing Then
                PopulateDropdowns(0)
            Else
                PopulateDropdowns(_order.ID)
            End If
            cboEndReason.SelectPostBackValue()

            PopulateAfterPostback()
            'PopulateAttendanceSummaryTab()

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_SUMMARY_ATTENDANCE) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_SUMMARY_ATTENDANCE, UNIQUEID_PREFIX_DELETE_SUMMARY_ATTENDANCE)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phAttendanceSummary.Controls.Count - 1
                If phAttendanceSummary.Controls(index).ID = id Then
                    phAttendanceSummary.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistAttendanceSummaryUniqueIDsToViewState(list)

        End Sub

#End Region

#Region "           GetAttendanceSummaryUniqueIDsFromViewState "

        Private Function GetAttendanceSummaryUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_ATTENDANCESUMMARY) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_ATTENDANCESUMMARY), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_ATTENDANCESUMMARY) Is Nothing Then
                _newAttendanceSummaryIDCounter = 0
            Else
                _newAttendanceSummaryIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_ATTENDANCESUMMARY), Integer)
            End If

            Return list

        End Function

#End Region

#Region "           PersistAttendanceSummaryUniqueIDsToViewState "

        Private Sub PersistAttendanceSummaryUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_ATTENDANCESUMMARY, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_ATTENDANCESUMMARY, _newAttendanceSummaryIDCounter)
        End Sub

#End Region

#Region "           GetAttendanceSummaryUniqueID "

        Private Function GetAttendanceSummaryUniqueID(ByVal attendance As DomServiceOrderAttendance) As String

            Dim id As String

            If attendance.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_SUMMARY_ATTENDANCE & _newAttendanceSummaryIDCounter
                _newAttendanceSummaryIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_SUMMARY_ATTENDANCE & attendance.ID
            End If

            Return id

        End Function

#End Region

#Region "           GetAttendanceSummaryUniqueID "

        Private Function GetAttendanceSummaryUniqueID(ByVal attendance As vwDomServiceOrderAttendance) As String

            Dim id As String

            If attendance.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_SUMMARY_ATTENDANCE & _newAttendanceSummaryIDCounter
                _newAttendanceSummaryIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_SUMMARY_ATTENDANCE & attendance.ID
            End If

            Return id

        End Function

#End Region

#Region "           btnAddAttendanceSummary_Click "

        Private Sub btnAddAttendanceSummary_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddAttendanceSummary.Click
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetAttendanceSummaryUniqueIDsFromViewState()
            Dim newAttendance As DomServiceOrderAttendance = New DomServiceOrderAttendance(String.Empty, String.Empty)

            If _order Is Nothing Then
                PopulateDropdowns(0)
            Else
                PopulateDropdowns(_order.ID)
            End If
            cboEndReason.SelectPostBackValue()
            PopulateAfterPostback()
            PopulateAttendanceTab()

            ' add a new row to the visit list
            id = GetAttendanceSummaryUniqueID(newAttendance)
            ' create the controls
            OutputAttendanceSummaryControls(id, newAttendance)
            ' persist the data into view state
            list.Add(id)
            PersistAttendanceSummaryUniqueIDsToViewState(list)

        End Sub

#End Region

#Region "           BuildAttendanceSummaryCollectionsForSave "

        Protected Function BuildAttendanceSummaryCollectionsForSave(ByRef orderAttendancies As DomServiceOrderAttendanceCollection, ByRef attendanceToDelete As List(Of String)) As ErrorMessage
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim orderAttendance As DomServiceOrderAttendance = Nothing
            Dim attendanceList As List(Of String) = Nothing

            orderAttendancies = New DomServiceOrderAttendanceCollection
            attendanceToDelete = New List(Of String)
            attendanceList = GetAttendanceSummaryUniqueIDsFromViewState()
            For Each uniqueID As String In attendanceList
                If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_SUMMARY_ATTENDANCE) Then
                    ' we are deleting
                    attendanceToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE_SUMMARY_ATTENDANCE, String.Empty))
                Else
                    ' create the empty visit record
                    orderAttendance = New DomServiceOrderAttendance(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE_SUMMARY_ATTENDANCE) Then
                        ' we are updating
                        msg = orderAttendance.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_SUMMARY_ATTENDANCE, String.Empty)))
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If
                    ' set the Attendance properties
                    With orderAttendance
                        .DomRateCategoryID = Utils.ToInt32(CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_RATE_CAT & uniqueID), DropDownListEx).GetPostBackValue())
                        If ShowDayOfWeekColumn() Then
                            .ProviderUnits = 0
                            If CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_MON & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                            If CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_TUE & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                            If CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_WED & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                            If CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_THU & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                            If CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_FRI & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                            If CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_SAT & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                            If CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_SUN & uniqueID), CheckBoxEx).CheckBox.Checked Then .ProviderUnits += 1
                            '.ProviderUnits = Convert.ToInt32(CType(phAttendance.FindControl(CTRL_PREFIX_ATT_UNITS & uniqueID), Label).Text)
                            .OnMonday = CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_MON & uniqueID), CheckBoxEx).CheckBox.Checked
                            .OnTuesday = CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_TUE & uniqueID), CheckBoxEx).CheckBox.Checked
                            .OnWednesday = CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_WED & uniqueID), CheckBoxEx).CheckBox.Checked
                            .OnThursday = CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_THU & uniqueID), CheckBoxEx).CheckBox.Checked
                            .OnFriday = CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_FRI & uniqueID), CheckBoxEx).CheckBox.Checked
                            .OnSaturday = CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_SAT & uniqueID), CheckBoxEx).CheckBox.Checked
                            .OnSunday = CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_SUN & uniqueID), CheckBoxEx).CheckBox.Checked
                        Else
                            .ProviderUnits = Convert.ToInt32(CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_UNITS & uniqueID), TextBoxEx).Text)
                            'The service needs to be put against one day.
                            .OnSunday = TriState.True
                        End If

                        .Frequency = Utils.ToInt32(CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_FREQUENCY & uniqueID), DropDownListEx).GetPostBackValue())
                        If .Frequency = Frequency.EveryWeek Then
                            .FirstWeekOfService = 1
                        Else
                            .FirstWeekOfService = Utils.ToInt32(CType(phAttendanceSummary.FindControl(CTRL_PREFIX_ATT_SUMMARY_FIRST_WEEK & uniqueID), DropDownListEx).GetPostBackValue())
                        End If
                    End With

                    orderAttendance.EffectiveDate = _order.DateFrom

                    ' add to the collection
                    orderAttendancies.Add(orderAttendance)
                    'hidAttendanceHasDetail.Value = True
                End If
            Next

            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function


#End Region

#End Region

#Region " Payment Tolerances Tab "

        Private Sub SetPaymentTolerancesEditableFlag()
            'set the payment tolerances 'editable' flag
            _paymentTolerancesEditable = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, SecurityBL.GetCurrentUser().ID, _
                                                              Target.Library.Web.ConstantsManager.GetConstant( _
                                                              "AbacusIntranet.WebNavMenuItemCommand.DomiciliaryServiceOrders.OverridePaymentTolerances"), _
                                                              Settings.CurrentApplicationID)
        End Sub

        Private Sub TabPaymentTolerancesSetVisibility()
            'check if new service order or if existing then diplay the tab payment tolerances
            tabPaymentTolerances.Visible = (_hasPaymentToleranceGroups And _stdBut.ButtonsMode = StdButtonsMode.Fetched Or _
                                            _hasPaymentToleranceGroups And _stdBut.ButtonsMode = StdButtonsMode.Edit)
        End Sub

#End Region

#Region " SetupDSOFundingControl "

        'Private Sub SetupDSOFundingControl(ByVal dsoID As Integer)
        '    With CType(Me.dsoFunding, ServiceOrderFunding)
        '        .populateControl(dsoID)
        '        '.InitControl()
        '    End With
        'End Sub

#End Region

#Region " FetchExternalFields "

        Private Sub FetchExternalFields()
            Dim msg As New ErrorMessage

            '++ See if there are any fields defined as editable (even when service order is locked)..
            msg = External_Field.FetchList(conn:=Me.DbConnection, list:=_externalFields, _
                                           externalTable:="External_DomServiceOrder", _
                                           allowEditInAbacus:=TriState.True)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If (_externalFields IsNot Nothing) AndAlso (_externalFields.Count > 0) Then
                _editableFieldsFound = True
            End If
        End Sub

#End Region

#Region " populateSummaryAndVisits "

        Private Function populateSummaryVisitsAndExpenditure() As ErrorMessage

            Dim msg As ErrorMessage
            Dim orderDetails As List(Of DomServiceOrderDetail) = Nothing
            Dim orderSummarys As List(Of DomServiceOrderSummary) = Nothing
            Dim orderVisits As List(Of DomServiceOrderVisit) = Nothing
            Dim AttendanceSchedules As vwDomServiceOrderAttendanceCollection = Nothing
            Dim AttSummaryDetails As DomServiceOrderAttendanceCollection = Nothing
            Dim sumList As List(Of String), visitList As List(Of String)
            Dim id As String
            Dim filterDate As Nullable(Of Date) = Nothing

            If Not _editMode AndAlso ((Not _order.VisitBased AndAlso Not chkDontFilterCommitmentSummary.Checked) Or _
                (_order.VisitBased AndAlso Not chkDontFilterCommitmentVisit.Checked)) Then
                filterDate = txtVisitsFilterDate.Text
            End If

            PopulateAfterPostback()

            phSummary.Controls.Clear()
            phVisits.Controls.Clear()
            ViewState.Remove(VIEWSTATE_KEY_DATA_SUMMARY)
            phSummary.Controls.Clear()
            ViewState.Remove(VIEWSTATE_KEY_DATA_VISITS)
            phVisits.Controls.Clear()


            ' refresh the list of existing summary lines and save in view state
            msg = ServiceOrderBL.GetDomServiceOrderSummary(Me.DbConnection, _
                                                          _order.ID, _
                                                          _order.VisitBased, _
                                                          filterDate, _
                                                          orderSummarys)
            If Not msg.Success Then Return msg

            'If Not msg.Success Then Return msg
            sumList = GetSummaryUniqueIDsFromViewState()
            _forceDisableVisits = New List(Of TextBoxEx)
            _startup2JS.Length = 0
            ' roll up bay rate category and frequency for visit based orders (also filter out the Enhanced line versions)
            If _order.VisitBased Then
                For Each orderDetail As DomServiceOrderSummary In orderSummarys 'RollUpSummaryByRateCatFreq(orderDetails).Values
                    id = GetSummaryUniqueID(orderDetail)
                    OutputSummaryControls(id, orderDetail, Not _order.VisitBased)
                    sumList.Add(id)
                Next
            Else
                For Each orderDetail As DomServiceOrderSummary In orderSummarys
                    'If Not orderDetail.Enhanced Then
                    id = GetSummaryUniqueID(orderDetail)
                    OutputSummaryControls(id, orderDetail, Not _order.VisitBased)
                    sumList.Add(id)
                    'End If
                Next
            End If
            PersistSummaryUniqueIDsToViewState(sumList)

            ' refresh the list of existing visits and save in view state
            msg = ServiceOrderBL.GetDomServiceOrderVisit(Me.DbConnection, _
                                                         _order.ID, _
                                                         filterDate, _
                                                         orderVisits)
            If Not msg.Success Then Return msg
            visitList = GetVisitUniqueIDsFromViewState()
            For Each visit As DomServiceOrderVisit In orderVisits
                id = GetVisitUniqueID(visit)
                OutputVisitControls(id, visit)
                visitList.Add(id)
            Next
            PersistVisitUniqueIDsToViewState(visitList)
            'End If

            PopulateExpenditureTab(_order.ID)

            msg = New ErrorMessage
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " txtDummyFilterDate_TextChanged "

        Private Sub txtDummyFilterDate_TextChanged(sender As Object, e As System.EventArgs) Handles txtDummyFilterDate.TextChanged
            Dim msg As ErrorMessage = Nothing
            msg = populateSummaryVisitsAndExpenditure()
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

    End Class

End Namespace