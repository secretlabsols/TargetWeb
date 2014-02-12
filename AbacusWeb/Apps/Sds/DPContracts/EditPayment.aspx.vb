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
Imports Target.Abacus.Library.SDS
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Jobs.Core
Imports System.Collections.Generic
Imports Target.Library.Web.Adapters
Imports Target.Abacus.Web.Apps.InPlaceSelectors

Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' Screen used to view/edit the content of a DP contract payment.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD      28/11/2012  #7636 - Order budget categories by description when has no budget catgeory validation method.
    '''     ColinD      30/10/2012  D12394 - Alterations to cater for new first week direct payment contract frequency - provide friendly errors for breakdown records when errored.
    '''     ColinD      29/10/2012  D12417 - Alterations to cater for mutiple frequencies for breakdown records.
    '''     ColinD      25/10/2012  D12394 - Alterations to cater for new first week direct payment contract frequency.
    '''     Colind      13/06/2011  D12140 - updated finance code to utilise finance code selector tool instead of free text, please note that only expenditure codes are listed and that we only store the code not the id of the selected code.
    '''     Colind      27/05/2011  SDS issue #565 - updated code to handle error messages.
    '''     ColinD      09/05/2011  SDS651 - Changes to handle removal of OverridenWeeklyCharge from BudgetCategoryRates.
    '''     ColinD      24/03/2011  D11884A - Added Finance Code.
    '''     ColinD      04/02/2011  D12008 - Changes to ensure budget categories are displayed correctly, to handle that some bc rate stored procs have now been deleted.
    '''     ColinD      27/01/2010  D12007 - Changes to ensure that Reconsideration button is available if at least one payment line requires reconsideration. Minor changes to the way in which payments are saved to ensure that records are reconsidered if required.
    '''     ColinD      08/12/2010  D11964A - Removed ability to select a budget category which is not linked to a dom service type record.
    '''     ColinD      23/11/2010  D11801 - SDS Issue 378, Configured interface to persist selection of Active, Provisional or Suspended in when creating new payment lines
    '''     ColinD      27/10/2010  D11801 - SDS Issue 311, Configured interface to set IncomeUnitCost and OverriddenWeeklyCharge on DPContractDetailBreakdown records
    '''     ColinD      20/10/2010  D11918 - UI enhancements to handle <see cref="DPContractBL.BudgetCategoryValidationMethod.PreventWhenNotInSpendPlanWarnOnNotDirectPayment" /> budget category validation method
    '''     MikeVO      05/10/2010  UI tidy up following changes to collapsible panel.
    '''     JohnF       04/10/2010  Remove payments cancelled during reconsideration (D11801)
    '''     JohnF       24/09/2010  Ensure DateTo's calculated correctly (D11801)
    '''     JohnF       24/09/2010  Apply security options to Status drowdown entries (D11801)
    '''     JohnF       24/09/2010  Added MenuItemID for parent form (D11801)
    '''     JohnF       16/08/2010  Created (D11801)
    ''' </history>
    Partial Class EditPayment
        Inherits Target.Web.Apps.BasePage

#Region " Module variables "
        Private Const VALUE_NET As Integer = 0
        Private Const VALUE_GROSS As Integer = 1
        Private Const NEW_PAYMENT_TEXT As String = "(NEW PAYMENT)"
        Private Const VIEWSTATE_KEY_DATA_BREAKDOWNS As String = "DetailDataList"
        Private Const VIEWSTATE_KEY_COUNTER_NEW_BREAKDOWNS As String = "DetailCounter"
        Private Const VIEWSTATE_KEY_DATA_BREAKDOWNS_REMOVED As String = "RemovedDetailDataList"

        Private Const CTRL_PREFIX_BREAKDOWN_BUDGET_CATEGORY As String = "brkdwnBudgetCat"
        Private Const CTRL_PREFIX_BREAKDOWN_UNITS As String = "brkdwnThisUnits"
        Private Const CTRL_PREFIX_BREAKDOWN_UOM As String = "brkdwnThisUOM"
        Private Const CTRL_PREFIX_BREAKDOWN_FREQUENCY As String = "brkdwnThisFreq"
        Private Const CTRL_PREFIX_BREAKDOWN_UNITSPAID As String = "brkdwnThisUnitsPaid"
        Private Const CTRL_PREFIX_BREAKDOWN_UNITCOST As String = "brkdwnThisRate"
        Private Const CTRL_PREFIX_BREAKDOWN_LINECOST As String = "brkdwnThisCost"

        Private Const CTRL_PREFIX_BREAKDOWN_REMOVE As String = "brkdwnRemove"

        Private Const UNIQUEID_PREFIX_NEW_BREAKDOWN As String = "brkdwnN"
        Private Const UNIQUEID_PREFIX_UPDATE_BREAKDOWN As String = "brkdwnU"
        Private Const UNIQUEID_PREFIX_DELETE_BREAKDOWN As String = "brkdwnD"

        Private Const RECONSIDER_TEXT As String = "This contract has one or more payments (marked with a *) that need to be reconsidered due to changes with the associated Budget Category."
        Private Const _BudgetCategoryNotInSpendPlanOptionGroup As String = "Not in the Spend Plan"
        Private Const _BudgetCategoryInSpendPlanOptionGroup As String = "In the Spend Plan"
        Private Const _BudgetCategoryInSpendPlanNotDpOptionGroup As String = "In the Spend Plan, Not Direct Payments"

        Private _stdBut As StdButtonsBase
        Private _dpConPayment As DPContractDetail
        Private _extraJS As StringBuilder = New StringBuilder()
        Private _newDetailIDCounter As Integer
        Private btnReconsider As Button = New Button
        Private _ImageBlocked As String = Target.Library.Web.Utils.GetVirtualPath("Images/Blocked.png")
        Private _ImageBlockedSoft As String = Target.Library.Web.Utils.GetVirtualPath("Images/BlockedOrange.png")
        Private _ImageComplete As String = Target.Library.Web.Utils.GetVirtualPath("Images/Complete.png")
        Private _ImageWarning As String = Target.Library.Web.Utils.GetVirtualPath("Images/WarningRed.png")
        Private _ImageWarningSoft As String = Target.Library.Web.Utils.GetVirtualPath("Images/WarningHS.png")
        Private _IsDeleteOperation As Boolean = True
        Private _editSuspendProhibited As Boolean
        Private _Frequencies As List(Of ViewableDirectPaymentContractFrequency) = Nothing

#End Region

#Region " (page events) "
        Private Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            _stdBut = Me.Master.FindControl("MPContent").FindControl("cp").FindControl("stdButtons1")

            AddHandler _stdBut.AddCustomControls, AddressOf StdButtons_AddCustomControls
            AddHandler btnReconsider.Click, AddressOf btnReconsider_Click
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim buttMode As Integer = Target.Library.Utils.ToInt32(Request.QueryString("mode"))
            Dim showExpanded As Integer = Target.Library.Utils.ToInt32(Request.QueryString("showexp"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage = Nothing

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DirectPaymentContracts"), "")
            Me.UseJQuery = True
            pnlLegend.Style.Add("display", "none")

            With _stdBut
                .SelectedItemID = PaymentID
                If buttMode = StdButtonsMode.AddNew Then
                    .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.Edit"))
                Else
                    .AllowNew = False
                End If
                .ShowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.Edit"))
                .ShowSave = True
                .AllowFind = False
                .AllowDelete = .AllowEdit
                If PaymentID > 0 Then
                    .InitialMode = StdButtonsMode.Fetched
                    .OnCancelClientClick = "EditPayment_CancelClicked();"
                Else
                    .InitialMode = StdButtonsMode.AddNew
                    '++ The following is used for removing new payments when Cancel is pressed
                    '++ during the editing session..
                    .OnCancelClientClick = String.Format("window.parent.EditPayment_CancelClicked('{0}');", frameUID)
                End If
                .Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DirectPaymentContracts")
                .OnSaveClientClick = "return ClientSaveClicked();"
                .EditableControls.Add(pnlPayment.Controls)
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
            End With

            ' add date utility javascript
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility javascript link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog javascript
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter javascript
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page javascript
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Sds/DPContracts/EditPayment.js"))
            ' add fancy drop down list jquery script
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/ImageDropdown/ImageDropdown.js"))
            ' add fancy drop down css
            Me.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/ImageDropdown/ImageDropdown.css"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DPContract))


            cboFrequency.DropDownList.Attributes.Add("onchange", "cboFrequency_OnChange();")
            cboEndReason.DropDownList.Attributes.Add("onchange", "cboEndReason_OnChange();")
            txtNumPayments.TextBox.Attributes.Add("onchange", "txtNumOfPayments_OnChange();")
            With FinanceCodeSelector
                .Required = False
                .FinanceCodeCategoryID = 1 ' expenditure
                .FinanceCodeID = 0
            End With

            '++ Show the form as expanded (this flag exists only after a redirect from a successful Save)..
            If showExpanded = 1 Then cp.Expanded = True

            ' re-create the list of bands (from view state)
            Dim list As List(Of String) = GetBreakdownIDsFromViewState()
            For Each id As String In list
                OutputBreakdownControls(id, Nothing, Nothing)
            Next
            If PaymentID = 0 Then PopulateScreen()

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the EditPayment control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub EditPayment_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim contractId As Integer = ParentContractID

            If contractId > 0 Then

                Dim msg As New ErrorMessage()
                Dim dateFromAllowableDay As Integer = 0
                Dim dateFromDefaultDate As DateTime = DateTime.MinValue
                Dim dateFromMinimumDate As DateTime = DateTime.MinValue
                Dim dateToAllowableDay As Integer = 0
                Dim dateToDefaultDate As DateTime = DateTime.MaxValue
                Dim dateToMinimumDate As DateTime = DateTime.MaxValue
                Dim existingContract As New DPContract(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                Dim budgetHolder As New ClientBudgetHolder(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                ' get the contract
                msg = existingContract.Fetch(contractId)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                ' get the budget holder
                msg = budgetHolder.Fetch(existingContract.ClientBudgetHolderID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                ' get the payment record
                GetAndSetCurrentPayment()
                ' get the dates
                msg = DPContractBL.GetDPContractDates(DbConnection, existingContract, budgetHolder.ClientID, dateFromAllowableDay, dateFromMinimumDate, dateFromDefaultDate, dateToAllowableDay, dateToMinimumDate, dateToDefaultDate, _dpConPayment)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                ' set the dates
                With dteDateFrom
                    .AllowableDays = dateFromAllowableDay.ToString()
                    .MinimumValue = dateFromMinimumDate.ToString("dd/MM/yyyy")
                End With

            End If

            '++ Disable the field(s) which are always read-only..
            If Utils.IsDate(hidPaidUpTo.Value) AndAlso Convert.ToDateTime(hidPaidUpTo.Value) <> DataUtils.MAX_DATE Then
                _stdBut.AllowDelete = False
            End If

            divExternalReference.Visible = False

            If IsMaintainedExternally Then
                ' if we maintain this contract from an external source then do not allow editing etc

                With _stdBut
                    .AllowDelete = False
                    .AllowEdit = False
                    .AllowNew = False
                End With

                divExternalReference.Visible = True

                With txtExternalReference
                    .Text = _dpConPayment.Reference
                End With

            End If

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim showExpanded As Integer = Target.Library.Utils.ToInt32(Request.QueryString("showexp"))
            Dim js As StringBuilder = New StringBuilder()
            Dim budgetCategoryValidationMethod As DPContractBL.BudgetCategoryValidationMethod = GetBudgetCategoryValidationMethod()
            Dim contractId As Integer = ParentContractID
            Dim firstWeekStartDate As DateTime = ParentContract.DateFrom
            Dim firstWeekEndDate As DateTime = DomContractBL.GetWeekEndingDate(DbConnection, Nothing, ParentContract.DateFrom)
            Dim frequencies As List(Of ViewableDirectPaymentContractFrequency) = GetFrequencies()

            If Not _dpConPayment Is Nothing Then
                js.AppendFormat("isBalancingPayment={0};", _dpConPayment.IsBalancingPayment.ToString().ToLower())
            End If
            js.AppendFormat("dteDateFromID='{0}';", dteDateFrom.ClientID)
            js.AppendFormat("cboEndReasonID='{0}';", cboEndReason.ClientID)
            js.AppendFormat("cboFreqID='{0}';", cboFrequency.ClientID)
            js.AppendFormat("txtNumPaymentsID='{0}';", txtNumPayments.ClientID)
            js.AppendFormat("chkForceGrossID='{0}';", chkForceGross.ClientID)
            js.AppendFormat("btnAddBreakdownID='{0}';", btnAddBreakdown.ClientID)
            js.AppendFormat("hidFrequencyID='{0}';", hidFrequency.ClientID)
            js.AppendFormat("hidDateFromID='{0}';", hidDateFrom.ClientID)
            js.AppendFormat("hidDateToID='{0}';", hidDateTo.ClientID)
            js.AppendFormat("hidEndReasonID='{0}';", hidEndReason.ClientID)
            js.AppendFormat("hidNumPaymentsID='{0}';", hidNumPayments.ClientID)
            js.AppendFormat("hidAmountID='{0}';", hidAmount.ClientID)
            js.AppendFormat("hidPaidUpToID='{0}';", hidPaidUpTo.ClientID)
            js.AppendFormat("hidForceGrossID='{0}';", hidForceGross.ClientID)
            js.AppendFormat("inEditMode='{0}';", IIf(_stdBut.ButtonsMode = StdButtonsMode.AddNew Or _stdBut.ButtonsMode = StdButtonsMode.Edit, "Y", "N"))
            js.AppendFormat("function {0}_Changed(id) {{ txtNumOfPayments_OnChange(id); }};", txtNumPayments.ID)
            js.AppendFormat("pnlLegendID='{0}';", pnlLegend.ClientID)
            js.AppendFormat("pnlLegendInSpendPlanNotDPID='{0}';", pnlLegendInSpendPlanNotDP.ClientID)
            js.AppendFormat("pnlLegendNotInSpendPlanID='{0}';", pnlLegendNotInSpendPlan.ClientID)
            js.AppendFormat("lblErrorID='{0}';", lblError.ClientID)
            js.AppendFormat("budgetCategoryNotInSpendPlanOptionGroup='{0}';", _BudgetCategoryNotInSpendPlanOptionGroup)
            js.AppendFormat("budgetCategoryInSpendPlanNotDpOptionGroup='{0}';", _BudgetCategoryInSpendPlanNotDpOptionGroup)
            js.AppendFormat("editSuspendForbidden='{0}';", IIf(_editSuspendProhibited, "Y", "N"))
            js.AppendFormat("optStatusProvID='{0}';", optStatusProv.ClientID)
            js.AppendFormat("optStatusActiveID='{0}';", optStatusActive.ClientID)
            js.AppendFormat("spnAmountID='{0}';", txtAmount.ClientID)
            js.AppendFormat("spnDateToID='{0}';", txtDateTo.ClientID)
            js.AppendFormat("editPayment_DPContractID={0};", ParentContractID)
            js.AppendFormat("editPayment_CurrentPeriodID='{0}';", lblCurrentDateRange.ClientID)
            js.AppendFormat("editPayment_NextPeriodID='{0}';", lblNextDateRange.ClientID)
            js.AppendFormat("editPayment_CurrentWillBePaidID='{0}';", lblCurrentPaymentsWillBePaid.ClientID)
            js.AppendFormat("editPayment_CurrentHasBeenPaidID='{0}';", lblCurrentPaymentsHaveBeedPaid.ClientID)
            js.AppendFormat("editPayment_NextWillBePaidID='{0}';", lblNextPaymentsWillBePaid.ClientID)
            js.AppendFormat("editPayment_NextHasBeenPaidID='{0}';", lblNextPaymentsHaveBeedPaid.ClientID)
            js.AppendFormat("editPayment_overlappsID='{0}';", lblOverlappingPayments.ClientID)
            js.AppendFormat("hdnFinanceCode1ID = '{0}';", hdnFinanceCode1.ClientID)
            js.AppendFormat("firstWeekEndDate = {0};", WebUtils.GetDateAsJavascriptString(firstWeekEndDate))
            js.AppendFormat("firstWeekStartDate = {0};", WebUtils.GetDateAsJavascriptString(firstWeekStartDate))

            If GetIfRequiresBudgetCategoryValidation() Then
                ' if we require budget category validation

                Dim saveButton As Button = _stdBut.FindControl("btnSave")

                If SpendPlanID Is Nothing OrElse SpendPlanID.HasValue = False Then
                    ' if we havent fetched the spend plan id yet try again

                    GetDpContractBudgetCategories()

                End If

                If Not SpendPlanID Is Nothing AndAlso SpendPlanID.HasValue Then
                    ' if we have found a spend plan ouput to js

                    js.AppendFormat("spendPlanID='{0}';", SpendPlanID.Value)

                End If

                ' setup the not in spend plan legend
                If budgetCategoryValidationMethod = DPContractBL.BudgetCategoryValidationMethod.PreventWhenNotInSpendPlan _
                    OrElse budgetCategoryValidationMethod = DPContractBL.BudgetCategoryValidationMethod.PreventWhenNotInSpendPlanWarnOnNotDirectPayment Then
                    ' if prevent or prevent warn then output suitable legend

                    lblLegendNotInSpendPlan.Text = String.Format("Budget categories marked with a <img src=""{0}"" border=""0"" /> are <u>not</u> contained within the spend plan for the period. You may not save this payment line.", _
                                                                 _ImageBlocked)

                Else
                    ' if warning then output suitable legend

                    lblLegendNotInSpendPlan.Text = String.Format("Budget categories marked with a <img src=""{0}"" border=""0"" /> are not contained within the spend plan for the period. You may still save this payment line.", _
                                                                _ImageWarning)

                End If

                ' setup the in spend plan but not dp legend
                If budgetCategoryValidationMethod = DPContractBL.BudgetCategoryValidationMethod.PreventWhenNotInSpendPlan Then
                    ' if prevent then output suitable legend

                    lblLegenedInSpendPlanNotDP.Text = String.Format("Budget categories marked with a <img src=""{0}"" border=""0"" /> are contained within the spend plan for the period but are <u>not</u> expected to be delivered via direct payments. You may not save this payment line.", _
                                                                    _ImageBlockedSoft)

                Else
                    ' if warning then output suitable legend

                    lblLegenedInSpendPlanNotDP.Text = String.Format("Budget categories marked with a <img src=""{0}"" border=""0"" /> are contained within the spend plan for the period but are <u>not</u> expected to be delivered via direct payments. You may still save this payment line.", _
                                                                _ImageWarningSoft)

                End If

                js.AppendFormat("saveButtonID='{0}';", saveButton.ClientID)

            End If

            ' output the budget category validation method
            js.AppendFormat("budgetCategoryValidationMethod='{0}';editPayment_BudgetPeriod={1};editPayment_isSDS={2};editPayment_clientID={3};", _
                                CType(budgetCategoryValidationMethod, Integer), hidBudgetPeriod.Value, IIf(hidISSDS.Value, "true", "false"), _
                                IIf(hidClientID.Value = "", 0, hidClientID.Value))

            '++ Add all possible FREQUENCY values to the js, as this needs to perform screen updates
            '++ based on the current frequency dropdown setting..
            For Each freq As ViewableDirectPaymentContractFrequency In frequencies
                ' loop each frequency for this payment

                js.Append(String.Concat("freq", freq.DisplayName, String.Format("='{0}';", freq.NumberOfDays)))
                js.Append(String.Concat("freq", freq.DisplayName, String.Format("id='{0}';", freq.ID)))

            Next

            ' add any extra js from page
            js.Append(_extraJS.ToString())

            '++ Disable the field(s) which are always read-only..
            If Utils.IsDate(hidPaidUpTo.Value) AndAlso Convert.ToDateTime(hidPaidUpTo.Value) <> DataUtils.MAX_DATE Then
                dteDateFrom.Enabled = False
                dteDateFrom.TextBox.BackColor = Color.LightGray
                cboFrequency.Enabled = False
                cboFrequency.DropDownList.BackColor = Color.LightGray
            End If

            '++ Re-enable the parent screen 'Add Payment' button after a successful Save..
            If showExpanded Then
                js.Append(String.Format("window.parent.EditPayment_SaveClicked('{0}');", frameUID))
            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js.ToString))

            _editSuspendProhibited = False

            fldCurrentPeriod.Disabled = False
            fldNextPeriod.Disabled = False
            grpPaymentSummary.Disabled = False
            'spnDateTo.Disabled = True
            'spnAmount.Enabled = False

            'If payments have been made, do not allow the user to change the paid gross checkbox.
            If _dpConPayment.PaymentsMade > 0 Then
                chkForceGross.Enabled = False
            End If

            fldCurrentPeriod.Visible = hidISSDS.Value
            fldNextPeriod.Visible = hidISSDS.Value

        End Sub
#End Region

#Region " StdButtons_AddCustomControls "
        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            If Not IsMaintainedExternally Then

                With btnReconsider
                    .ID = "btnReconsider"
                    .Text = "Reconsider"
                End With

                controls.Add(btnReconsider)

            End If
            
        End Sub
#End Region

#Region " EditClicked "
        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            ClearViewState()
            PopulateScreen()

            '++ Prevent editing if the payment status is Suspended and the user doesn't have
            '++ rights to suspend payments..
            _editSuspendProhibited = False
            If _dpConPayment.Status = DPContractDetailStatus.Suspended AndAlso _
                    Not Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.SetPaymentStatusSuspended")) Then
                _editSuspendProhibited = True
                e.Cancel = True
            End If
        End Sub
#End Region

#Region " FindClicked "
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            ClearViewState()
            PopulateScreen()
        End Sub
#End Region

#Region " SaveClicked "

        ''' <summary>
        ''' Saves the record
        ''' </summary>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim breakdownsToDel As List(Of String), idList As List(Of String)
            Dim breakdownRec As DPContractDetailBreakdown
            Dim breakdownColl As DPContractDetailBreakdownCollection = New DPContractDetailBreakdownCollection
            Dim newURL As String = ""
            Dim atLeastOneBudgetCategoryNotInSpendPlan As Boolean = False
            Dim tmpDecimal As Decimal = 0

            Me.Validate("Save")

            If Me.IsValid() Then

                Dim budgetCategoryValidationMethod As DPContractBL.BudgetCategoryValidationMethod = GetBudgetCategoryValidationMethod()
                Dim budgetCategories As List(Of ViewableBudgetCategoryRates) = GetDpContractBudgetCategories()
                Dim inSpendPlanBudgetCategory As ViewableBudgetCategoryRates = Nothing
                Dim hasPayments As Boolean = False
                Dim reconsiderPaymentBreakdowns As Boolean = False
                Dim oldPayment As DPContractDetail = Nothing
                Dim bcWarning As String = Nothing

                '++ Populate a DPContractDetail object with the current field sttings..
                msg = GetPaymentFromScreen()
                If Not msg.Success Then
                    e.Cancel = True
                    lblError.Text = msg.Message
                    Return
                End If

                Using trans As SqlTransaction = SqlHelper.GetTransaction(Me.DbConnection)

                    Try

                        Dim parentContract As DPContract = Nothing

                        hasPayments = (Utils.IsDate(_dpConPayment.PaidUpTo) AndAlso _dpConPayment.PaidUpTo <> DataUtils.MAX_DATE)
                        reconsiderPaymentBreakdowns = (_dpConPayment.ID <= 0)

                        If _dpConPayment.ID > 0 Then
                            ' if this is an existing payment

                            ' get the old payment so we can compare dates etc
                            msg = DPContractBL.GetDPContractDetail(trans, _dpConPayment.ID, oldPayment)
                            If Not msg.Success Then
                                SqlHelper.RollbackTransaction(trans)
                                e.Cancel = True
                                WebUtils.DisplayError(msg)
                            End If

                            If _dpConPayment.DateFrom <> oldPayment.DateFrom _
                                OrElse _dpConPayment.DateTo <> oldPayment.DateTo Then
                                ' if the payment has changed, then ensure all child records are reconsidered

                                reconsiderPaymentBreakdowns = True

                            End If

                        End If

                        ''++ See if the on-screen breakdown records use distinct budget categories.
                        ''++ Block the save if not the case..
                        msg = DPContractBL.SavePayment(trans, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), _dpConPayment)
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            e.Cancel = True
                            If msg.Number = DPContractBL.ERR_CONTRACT_DETAIL_INVALID Then
                                lblError.Text = msg.Message
                                Exit Sub
                            Else
                                WebUtils.DisplayError(msg)
                            End If
                        End If

                        If hasPayments = False Then
                            ' if we have saved the contract detail record successfully and the contract has no payments against it
                            ' then we are allowed to save detail breakdown records, else just ignore any changes

                            breakdownsToDel = New List(Of String)
                            idList = GetBreakdownIDsFromViewState()

                            For Each uniqueID As String In idList

                                If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_BREAKDOWN) Then
                                    ' we are deleting
                                    breakdownsToDel.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE_BREAKDOWN, String.Empty))
                                Else
                                    ' create the empty band
                                    breakdownRec = New DPContractDetailBreakdown(trans, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))
                                    If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE_BREAKDOWN) Then
                                        ' we are updating
                                        msg = breakdownRec.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_BREAKDOWN, String.Empty)))
                                        If Not msg.Success Then
                                            SqlHelper.RollbackTransaction(trans)
                                            e.Cancel = True
                                            WebUtils.DisplayError(msg)
                                        End If
                                    End If
                                    ' set the band properties
                                    With breakdownRec
                                        .AuditLogOverriddenParentID = _dpConPayment.ID
                                        Dim bcTokens() As String = CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_BUDGET_CATEGORY & "H" & uniqueID), HiddenField).Value.Split("$")
                                        .BudgetCategoryID = Utils.ToInt32(bcTokens(0))
                                        .ServiceDeliveredVia = Utils.ToInt32(bcTokens(4))
                                        ' get the income unit cost from the budget category
                                        If Decimal.TryParse(bcTokens(5), tmpDecimal) AndAlso tmpDecimal > 0 Then
                                            ' try cast the value to decimal and use if larger than 0
                                            .IncomeUnitCost = tmpDecimal
                                        Else
                                            ' else default to 0
                                            .IncomeUnitCost = 0
                                        End If
                                        ' get the overriden weekly charge from the budget category
                                        If Decimal.TryParse(bcTokens(6), tmpDecimal) AndAlso tmpDecimal > 0 Then
                                            ' try cast the value to decimal and use if larger than 0
                                            .OverriddenWeeklyCharge = tmpDecimal
                                        Else
                                            ' else default to 0
                                            .OverriddenWeeklyCharge = 0
                                        End If
                                        .Units = CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_UNITS & "H" & uniqueID), HiddenField).Value
                                        .UnitCost = CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_UNITCOST & "H" & uniqueID), HiddenField).Value
                                        .Frequency = CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_FREQUENCY & "H" & uniqueID), HiddenField).Value
                                        .FrequencyUnits = CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_UNITSPAID & "H" & uniqueID), HiddenField).Value
                                    End With

                                    ' fetch a the budget cat if is in spend plan
                                    inSpendPlanBudgetCategory = (From tmpBudgetCategory In budgetCategories _
                                                                    Where tmpBudgetCategory.ID = breakdownRec.BudgetCategoryID _
                                                                        AndAlso tmpBudgetCategory.SpendPlanID > 0 _
                                                                    Select tmpBudgetCategory).FirstOrDefault()

                                    If inSpendPlanBudgetCategory Is Nothing Then
                                        ' budget cat not in spend plan

                                        atLeastOneBudgetCategoryNotInSpendPlan = True

                                    End If

                                    ' add to the collection
                                    breakdownColl.Add(breakdownRec)

                                End If

                            Next

                            '++ Remove any breakdowns marked for deletion..
                            If breakdownsToDel IsNot Nothing AndAlso breakdownsToDel.Count > 0 Then
                                For Each deleteID As String In breakdownsToDel
                                    msg = DPContractBL.DeletePaymentBreakdown(trans, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), Utils.ToInt32(deleteID))
                                    If Not msg.Success Then
                                        SqlHelper.RollbackTransaction(trans)
                                        e.Cancel = True
                                        WebUtils.DisplayError(msg)
                                    End If
                                Next
                            End If

                            '++ Save any payment breakdown records there may be on-screen..
                            If breakdownColl IsNot Nothing AndAlso breakdownColl.Count > 0 Then

                                Dim duplicates As Integer = breakdownColl.ToArray().GroupBy(Function(x) x.BudgetCategoryID).SelectMany(Function(g) g.Skip(1)).ToList().Count
                                If duplicates > 0 Then
                                    ''++ duplicate use of same category
                                    SqlHelper.RollbackTransaction(trans)
                                    lblError.Text = "There are multiple payment breakdowns incorrectly sharing a single Budget Category."
                                    e.Cancel = True
                                    Exit Sub
                                End If

                                For Each rec As DPContractDetailBreakdown In breakdownColl
                                    With rec
                                        .DPContractDetailID = _dpConPayment.ID
                                        If reconsiderPaymentBreakdowns Then
                                            .Reconsider = TriState.True
                                        End If
                                        msg = DPContractBL.SavePaymentBreakdown(trans, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), rec, bcWarning)
                                        If Not msg.Success Then
                                            SqlHelper.RollbackTransaction(trans)
                                            e.Cancel = True
                                            If msg.Number = DPContractBL.ERR_CONTRACT_DETAIL_BREAKDOWN_INVALID Then
                                                lblError.Text = msg.Message
                                                Exit Sub
                                            Else
                                                WebUtils.DisplayError(msg)
                                            End If
                                        End If
                                    End With
                                Next
                            Else
                                '++ A payment must have at least one breakdown record, so reject
                                '++ the save and display an error..
                                SqlHelper.RollbackTransaction(trans)
                                lblError.Text = "The payment must be saved with at least one payment breakdown item in place."
                                e.Cancel = True
                                Exit Sub
                            End If

                        End If

                        ' reconsider payment, will only process records with reconsider flag = true
                        msg = DPContractBL.ReconsiderPayment(trans, _dpConPayment, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), Nothing)
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            e.Cancel = True
                            WebUtils.DisplayError(msg)
                        End If

                        ' get the dp contract as we want to resave it to force update of other data
                        msg = DPContractBL.GetDPContract(trans, _dpConPayment.DPContractID, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), currentUser.ExternalUsername, parentContract)
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            e.Cancel = True
                            WebUtils.DisplayError(msg)
                        End If

                        ' save the parent contract
                        msg = DPContractBL.Save(trans, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), parentContract)
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            e.Cancel = True
                            WebUtils.DisplayError(msg)
                        End If

                        ' commit work to db
                        trans.Commit()

                        cp.HeaderLinkText = hidTitle.Value
                        dteDateFrom.Text = hidDateFrom.Value
                        Dim reasonID As String
                        If hidEndReason.Value.IndexOf("<") <> -1 Then
                            reasonID = hidEndReason.Value.Substring(0, hidEndReason.Value.IndexOf("<"))
                        Else
                            reasonID = hidEndReason.Value
                        End If
                        FillDropdownEndReason(reasonID)

                        '++ Refresh the parent contract screen, since there may be additional
                        '++ payments which need to also be displayed..
                        Me.ClientScript.RegisterStartupScript( _
                            Me.GetType(), _
                            "ReconsiderPayment", _
                            "window.parent.EditPayment_ReconsiderClicked();", _
                            True)

                    Catch ex As Exception
                        ' catch exception and wrap

                        SqlHelper.RollbackTransaction(trans)
                        e.Cancel = True
                        msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                        WebUtils.DisplayError(msg)

                    End Try

                End Using

            Else

                e.Cancel = True

            End If

        End Sub

#End Region

#Region " CancelClicked "
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If PaymentID = 0 Then
                '++ Item hasn't been saved before, so remove this DP contract payment
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
            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim trans As SqlTransaction = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)
                msg = DPContractBL.DeletePayment(trans, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), PaymentID)
                If msg.Success Then
                    '++ Save, then reload the direct payment contract..
                    _IsDeleteOperation = True
                    trans.Commit()
                Else
                    If trans IsNot Nothing Then
                        SqlHelper.RollbackTransaction(trans)
                        trans = Nothing
                    End If
                    e.Cancel = True
                End If
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                If trans IsNot Nothing Then
                    SqlHelper.RollbackTransaction(trans)
                    trans = Nothing
                End If
            Finally
                If Not msg.Success Then
                    lblError.Text = msg.Message
                Else
                    lblError.Text = ""
                    '++ Remove this payment (i.e. the entire ASPX) from the payments tab..
                    Me.ClientScript.RegisterStartupScript( _
                        Me.GetType(), _
                        "DeletePayment", _
                        String.Format("window.parent.EditPayment_DeleteClicked('{0}');", frameUID), _
                        True)
                End If
            End Try
        End Sub
#End Region

#Region " FillDropdownEndReason "
        Private Sub FillDropdownEndReason(Optional ByVal selectedID As Integer = 0)
            Const SP_GET_END_REASONS As String = "spxContractEndReason_FetchListWithPaging"
            Dim reader As SqlDataReader = Nothing
            Dim cboItem As ListItem = Nothing
            Dim spParams As SqlParameter() = Nothing

            If cboEndReason.DropDownList.Items.Count = 0 Then
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_GET_END_REASONS, False)
                spParams(0).Value = ""
                spParams(1).Value = ""
                spParams(2).Value = 1
                spParams(3).Value = 9999    '++ Retrieve all records without regard to paging..
                spParams(4).Direction = ParameterDirection.InputOutput
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_GET_END_REASONS, spParams)

                cboItem = New ListItem(" ", "0<0")
                cboEndReason.DropDownList.Items.Add(cboItem)
                While reader.Read
                    If (reader("Usage") And ContractEndReasonUsage.DPContractDetails) <> 0 Then
                        If (reader("Redundant") = False) Or reader("ID") = selectedID Then
                            cboItem = New ListItem(reader("Description"), String.Format("{0}<{1}", reader("ID"), reader("SystemType")))
                            cboEndReason.DropDownList.Items.Add(cboItem)
                        End If
                    End If
                End While
                reader.Close()
                reader = Nothing
            End If

            Dim itemVal As String
            For Each cboItem In cboEndReason.DropDownList.Items
                '++ Strip off the superfluous other values from the ID before evaluation..
                If cboItem.Value.IndexOf("<") <> -1 Then
                    itemVal = cboItem.Value.Substring(0, cboItem.Value.IndexOf("<"))
                Else
                    itemVal = cboItem.Value
                End If

                If Utils.ToInt32(itemVal) = selectedID Then
                    cboEndReason.DropDownList.SelectedValue = cboItem.Value
                    hidEndReason.Value = cboItem.Value
                    Exit For
                End If
            Next
        End Sub
#End Region

#Region " SetStatusFields "
        Private Sub SetStatusFields(Optional ByVal selectedID As Integer = 0)
            optStatusProv.Checked = False
            optStatusActive.Checked = False
            optStatusSuspended.Checked = False

            optStatusProv.Visible = True
            If selectedID = DPContractDetailStatus.Provisional Then optStatusProv.Checked = True
            '++ Activate the 'Active' option only if the access rights allow or the current item is 'Active'..
            If Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.SetPaymentStatusActive")) _
                    Or selectedID = DPContractDetailStatus.Active Then
                optStatusActive.Visible = True
                If selectedID = DPContractDetailStatus.Active Then optStatusActive.Checked = True
            Else
                optStatusActive.Visible = False
            End If
            '++ Activate the 'Suspended' option only if the access rights allow or the current item is 'Suspended'..
            If Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.SetPaymentStatusSuspended")) _
                    Or selectedID = DPContractDetailStatus.Suspended Then
                optStatusSuspended.Visible = True
                If selectedID = DPContractDetailStatus.Suspended Then optStatusSuspended.Checked = True
            Else
                optStatusSuspended.Visible = False
            End If
        End Sub
#End Region

#Region " FillDropdownFrequency "

        ''' <summary>
        ''' Gets the frequencies for the current payment.
        ''' </summary>
        ''' <returns>The frequencies that apply to the current payment</returns>
        Private Function GetFrequencies() As List(Of ViewableDirectPaymentContractFrequency)

            If _Frequencies Is Nothing Then
                ' if we have no frequencies then get them from the business layer
                ' and store them just in case we call them again...which we do.

                Dim msg As New ErrorMessage()
                Dim paymentCreated As Boolean = False

                If _dpConPayment Is Nothing Then
                    ' if we have no payment then fetch one

                    _dpConPayment = GetAndSetCurrentPayment()

                    ' indicate that we have created the payment
                    paymentCreated = True

                End If

                ' get the frequencies and set the global variable
                msg = DPContractBL.GetDirectContractPaymentPaymentFrequencies(DbConnection, _dpConPayment, _Frequencies)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If paymentCreated Then
                    ' if we have created a payment here then null it out just in case it affects elsewhere

                    _dpConPayment = Nothing

                End If

            End If

            Return _Frequencies

        End Function

        ''' <summary>
        ''' Sets up the frequency drop down.
        ''' </summary>
        ''' <param name="dropDown">The drop down to setup.</param>
        Private Sub SetupFrequencyDropDown(ByVal dropDown As DropDownListEx, ByVal selectedID As Integer)

            Dim listItemsToFill As ListItemCollection = dropDown.DropDownList.Items
            Dim listItemToSelect As ListItem = Nothing

            ' setup the width 
            dropDown.Width = New Unit(15, UnitType.Em)

            If listItemsToFill.Count = 0 Then
                ' if we have no frequencies in the combo already then fill it

                Dim frequencies As List(Of ViewableDirectPaymentContractFrequency) = GetFrequencies()

                ' add an empty item into the drop down
                listItemsToFill.Add(New ListItem(" ", "-1"))

                For Each freq As ViewableDirectPaymentContractFrequency In frequencies
                    ' loop each frequency and add into the drop down list

                    listItemsToFill.Add(New ListItem(freq.Name, freq.ID))

                Next

            End If

            ' find the value in the list items 
            listItemToSelect = listItemsToFill.FindByValue(selectedID)

            If Not listItemToSelect Is Nothing Then
                ' if we have an item in the list matching the selected id then select it

                dropDown.DropDownList.SelectedValue = selectedID

            End If

        End Sub

#End Region

#Region " PopulateScreen "
        Private Sub PopulateScreen()
            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim existingContract As DPContract = Nothing
            Dim budgetHolder As ClientBudgetHolder = Nothing
            Dim existingPeriods As DPContractPeriodCollection = Nothing
            Dim existingPayments As DPContractDetailCollection = Nothing
            Dim dpcBreakdowns As DPContractDetailBreakdownCollection = Nothing
            Dim breakdownRecs As List(Of DPContractDetailBreakdown) = New List(Of DPContractDetailBreakdown)

            Try
                If _dpConPayment Is Nothing Then
                    _dpConPayment = New DPContractDetail(Me.DbConnection, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))
                End If
                If PaymentID <> _dpConPayment.ID AndAlso PaymentID <> 0 Then
                    msg = DPContractBL.FetchPayment(Me.DbConnection, PaymentID, _dpConPayment)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                _stdBut.SelectedItemID = PaymentID

                Dim sDateFrom As String, sDateTo As String
                With _dpConPayment
                    .DPContractID = ParentContractID
                    If .DPContractID <> 0 Then
                        msg = DPContractBL.Fetch(Me.DbConnection, .DPContractID, existingContract, existingPeriods, existingPayments)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If
                    ' get the client budget holder
                    budgetHolder = New ClientBudgetHolder(Me.DbConnection, String.Empty, String.Empty)
                    msg = budgetHolder.Fetch(existingContract.ClientBudgetHolderID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    ' setup date validation
                    Dim dateFromAllowableDay As Integer = 0
                    Dim dateFromDefaultDate As DateTime = existingContract.DateFrom
                    Dim dateFromMinimumDate As DateTime = existingContract.DateFrom
                    Dim dateToAllowableDay As Integer = 0
                    Dim dateToDefaultDate As DateTime = existingContract.DateTo
                    Dim dateToMinimumDate As DateTime = existingContract.DateTo
                    msg = DPContractBL.GetDPContractDates(DbConnection, existingContract, budgetHolder.ClientID, dateFromAllowableDay, dateFromMinimumDate, dateFromDefaultDate, dateToAllowableDay, dateToMinimumDate, dateToDefaultDate, _dpConPayment)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    With dteDateFrom
                        .AllowableDays = dateFromAllowableDay.ToString()
                        .MinimumValue = dateFromMinimumDate.ToString("dd/MM/yyyy")
                    End With
                    If .ID = 0 Then
                        .FrequencyID = -1
                        btnReconsider.Enabled = False
                        If existingPayments.Count = 0 Then
                            .DateFrom = dateFromDefaultDate
                            .DateTo = dateFromDefaultDate
                        End If
                    End If

                    If Utils.IsDate(hidDateFrom.Value) Then
                        .DateFrom = Convert.ToDateTime(hidDateFrom.Value)
                        If Utils.IsDate(hidDateTo.Value) Then
                            .DateTo = Convert.ToDateTime(hidDateTo.Value)
                        Else
                            .DateTo = DataUtils.MAX_DATE
                        End If
                        If hidFrequency.Value <> "" Then
                            .FrequencyID = Convert.ToInt32(hidFrequency.Value)
                        Else
                            .FrequencyID = -1
                        End If

                        Dim reasonID As String
                        If hidEndReason.Value.IndexOf("<") <> -1 Then
                            reasonID = hidEndReason.Value.Substring(0, hidEndReason.Value.IndexOf("<"))
                        Else
                            reasonID = hidEndReason.Value
                        End If
                        .EndReasonID = Convert.ToInt32(reasonID)

                        If Utils.IsDate(hidPaidUpTo.Value) Then
                            .PaidUpTo = Convert.ToDateTime(hidPaidUpTo.Value)
                        Else
                            .PaidUpTo = Nothing
                        End If
                        .PaymentsMade = Convert.ToInt32(hidPaymentsMade.Value)
                    End If

                    If Utils.IsDate(.DateFrom) AndAlso .DateFrom <> DataUtils.MAX_DATE Then
                        dteDateFrom.Text = .DateFrom.ToString("dd/MM/yyyy")
                        sDateFrom = .DateFrom.ToString("dd MMM yyyy")
                    Else
                        dteDateFrom.Text = ""
                        sDateFrom = "(open-ended)"
                    End If
                    hidDateFrom.Value = dteDateFrom.Text

                    If Utils.IsDate(.DateTo) AndAlso .DateTo <> DataUtils.MAX_DATE Then
                        txtDateTo.Text = .DateTo.ToString("dd/MM/yyyy")
                        sDateTo = .DateTo.ToString("dd MMM yyyy")
                    Else
                        sDateTo = "(open-ended)"
                        txtDateTo.Text = sDateTo
                    End If
                    hidDateTo.Value = txtDateTo.Text
                    hidBudgetPeriod.Value = existingContract.BudgetPeriod
                    hidISSDS.Value = existingContract.IsSDS

                    budgetHolder = New ClientBudgetHolder(Me.DbConnection, String.Empty, String.Empty)
                    msg = budgetHolder.Fetch(existingContract.ClientBudgetHolderID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    hidClientID.Value = budgetHolder.ClientID

                    FillDropdownEndReason(.EndReasonID)
                    SetupFrequencyDropDown(cboFrequency, .FrequencyID)
                    hidFrequency.Value = cboFrequency.DropDownList.SelectedValue

                    If .ID <= 0 Then
                        ' if a new payment line then get status from screen

                        If optStatusSuspended.Checked Then

                            .Status = DPContractDetailStatus.Suspended

                        ElseIf optStatusActive.Checked Then

                            .Status = DPContractDetailStatus.Active

                        Else

                            .Status = DPContractDetailStatus.Provisional

                        End If

                    End If

                    SetStatusFields(.Status)

                    '++ Calculate the number of payments set up for this payment
                    '++ by the start date, frequency and end date values (if any)..
                    Dim payCount As Integer
                    If sDateTo = "(open-ended)" Then
                        txtNumPayments.Text = "0"
                    ElseIf cboFrequency.Text = "ONCE" Then
                        txtNumPayments.Text = "1"
                    Else
                        msg = GetNumberOfPayments(.DateFrom, .DateTo, .FrequencyID, payCount)
                        If msg.Success Then
                            txtNumPayments.Text = payCount.ToString
                        Else
                            txtNumPayments.Text = "0"
                        End If
                    End If

                    chkForceGross.Checked = .ForceGross
                    hidPaymentsMade.Value = .PaymentsMade.ToString
                    If Utils.IsDate(.PaidUpTo) AndAlso .PaidUpTo <> DataUtils.MAX_DATE Then
                        spnPaidUpTo.InnerText = String.Format("Paid up to {0}; {1} payment(s) generated totalling £{2}", _
                                                              .PaidUpTo.ToString("dd MMM yyyy"), _
                                                              .PaymentsMade, _
                                                              Convert.ToDecimal(.PaymentsMade * .Amount).ToString("F2"))
                        hidPaidUpTo.Value = .PaidUpTo.ToString("dd/MM/yyyy")
                    Else
                        spnPaidUpTo.InnerText = "No payments made"
                        hidPaidUpTo.Value = ""
                    End If
                    'spnAmount.Text = "£" & Utils.ToDecimal(.Amount).ToString("0.00")
                    txtAmount.Text = .Amount.ToString("c")

                    With cp
                        Dim statusValue As String = ""
                        If optStatusProv.Checked Then
                            statusValue = optStatusProv.Text
                        ElseIf optStatusActive.Checked Then
                            statusValue = optStatusActive.Text
                        Else
                            statusValue = optStatusSuspended.Text
                        End If
                        .HeaderLinkText = String.Format("{0} - {1}{2}{3}{4}{5}{6}{7}{8}", _
                                                            sDateFrom, sDateTo, Blanks(7), _
                                                            _dpConPayment.Amount.ToString("£0.00"), _
                                                            Blanks(7), cboFrequency.DropDownList.SelectedItem.Text.ToUpper, _
                                                            Blanks(7), _
                                                            statusValue, _
                                                            IIf(_dpConPayment.ForceGross, String.Format("{0}{1}", Blanks(7), "(Paid Gross)"), ""))
                        hidTitle.Value = .HeaderLinkText
                    End With

                    If .ID > 0 Then
                        dpcBreakdowns = New DPContractDetailBreakdownCollection()
                        msg = DPContractBL.FetchListPaymentBreakdown(Me.DbConnection, dpcBreakdowns, .ID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        PopulatePaymentBreakdowns(dpcBreakdowns)
                    End If
                    FinanceCodeSelector.FinanceCodeText = .FinanceCode
                End With

                With cp
                    If _dpConPayment.ID = 0 Then
                        .HeaderLinkText = String.Format("{0} - {1}", sDateFrom, NEW_PAYMENT_TEXT)
                        hidTitle.Value = .HeaderLinkText
                        .Expanded = True
                    End If
                    .CollapsedJS = String.Format("Toggle('{0}');", frameUID)
                    .ExpandedJS = .CollapsedJS
                End With
                btnAddBreakdown.Enabled = True

            Catch ex As Exception
                msg.Success = False
                msg.Message = String.Concat(ex.Message, ex.StackTrace)
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            End Try
            If Not msg.Success Then lblError.Text = msg.Message
        End Sub
#End Region

#Region " GetPaymentFromScreen "
        Private Function GetPaymentFromScreen() As ErrorMessage
            Dim frameUID As String = Target.Library.Utils.ToString(Request.QueryString("uid"))
            Dim dpcID As String = ParentContractID
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg.Success = True
            Try
                If _dpConPayment Is Nothing Then
                    _dpConPayment = New DPContractDetail(Me.DbConnection, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))
                End If
                msg.Success = True
                If PaymentID <> _dpConPayment.ID AndAlso PaymentID <> 0 Then
                    msg = DPContractBL.FetchPayment(Me.DbConnection, PaymentID, _dpConPayment)
                    If Not msg.Success Then Return msg
                End If
                _dpConPayment.DPContractID = dpcID

                '++ Store the current field settings into the contract payment object..
                With _dpConPayment
                    .AuditLogOverriddenParentID = dpcID
                    If Utils.IsDate(hidDateFrom.Value) Then
                        .DateFrom = Convert.ToDateTime(hidDateFrom.Value)
                        dteDateFrom.Text = .DateFrom.ToString("dd/MM/yyyy")
                    End If
                    If Utils.IsDate(hidDateTo.Value) AndAlso Convert.ToDateTime(hidDateTo.Value) <> DataUtils.MAX_DATE Then
                        .DateTo = Convert.ToDateTime(hidDateTo.Value)
                        txtDateTo.Text = .DateTo.ToString("dd/MM/yyyy")
                    Else
                        .DateTo = DataUtils.MAX_DATE
                        txtDateTo.Text = "(open-ended)"
                    End If

                    Dim reasonID As String
                    If hidEndReason.Value.IndexOf("<") <> -1 Then
                        reasonID = hidEndReason.Value.Substring(0, hidEndReason.Value.IndexOf("<"))
                    Else
                        reasonID = hidEndReason.Value
                    End If
                    .EndReasonID = Convert.ToInt32(reasonID)
                    FillDropdownEndReason(.EndReasonID)
                    .FrequencyID = Utils.ToInt32(hidFrequency.Value)
                    SetupFrequencyDropDown(cboFrequency, .FrequencyID)
                    hidFrequency.Value = cboFrequency.DropDownList.SelectedValue

                    If optStatusProv.Checked Then
                        .Status = DPContractDetailStatus.Provisional
                    ElseIf optStatusActive.Checked Then
                        .Status = DPContractDetailStatus.Active
                    Else
                        .Status = DPContractDetailStatus.Suspended
                    End If

                    .PaymentsMade = Utils.ToInt32(hidPaymentsMade.Value)
                    If Utils.IsDate(hidPaidUpTo.Value) AndAlso Convert.ToDateTime(hidPaidUpTo.Value) <> DataUtils.MAX_DATE Then
                        .PaidUpTo = Convert.ToDateTime(hidPaidUpTo.Value)
                        spnPaidUpTo.InnerText = String.Format("Paid up to {0}; {1} payment(s) made totalling £{2} ({3})", _
                                                              .PaidUpTo.ToString("dd MMM yyyy"), _
                                                              .PaymentsMade, _
                                                              1234.56, _
                                                              "gross")
                    Else
                        .PaidUpTo = Nothing
                        spnPaidUpTo.InnerText = "No payments made"
                    End If

                    .Amount = hidAmount.Value.Replace("£", "")
                    'spnAmount.Text = "£" & .Amount.ToString("0.00")
                    txtAmount.Text = .Amount.ToString("C")
                    .ForceGross = (hidForceGross.Value = 1)
                    chkForceGross.Checked = .ForceGross

                    '++ Calculate the number of payments set up for this payment
                    '++ by the start date, frequency and end date values (if any)..
                    Dim payCount As Integer
                    If .DateTo = DataUtils.MAX_DATE Then
                        txtNumPayments.Text = "0"
                    ElseIf cboFrequency.Text = "ONCE" Then
                        txtNumPayments.Text = "1"
                    Else
                        msg = GetNumberOfPayments(.DateFrom, .DateTo, .FrequencyID, payCount)
                        If msg.Success Then
                            txtNumPayments.Text = payCount.ToString
                        Else
                            txtNumPayments.Text = "0"
                        End If
                    End If

                    .FinanceCode = Utils.ToString(Request.Form(String.Format("{0}$txtName", FinanceCodeSelector.UniqueID)))

                    btnAddBreakdown.Enabled = True
                End With
            Catch ex As Exception
                msg.Success = False
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            End Try

            Return msg
        End Function
#End Region

#Region " OutputBreakdownControls "

        ''' <summary>
        ''' Outputs the breakdown controls.
        ''' </summary>
        ''' <param name="uniqueID">The unique ID.</param>
        ''' <param name="breakdownRec">The breakdown rec.</param>
        ''' <param name="rowIndex">Index of the row.</param>
        Private Sub OutputBreakdownControls(ByVal uniqueID As String, ByVal breakdownRec As DPContractDetailBreakdown, ByVal rowIndex As Nullable(Of Integer))

            Dim msg As New ErrorMessage()
            Dim budCategory As New DataClasses.BudgetCategory(DbConnection, String.Empty, String.Empty)
            Dim row As TableRow
            Dim cell As TableCell
            Dim txt As TextBoxEx
            Dim cbo As DropDownListEx
            Dim ctrlHidden As HiddenField
            Dim btnRemove As HtmlInputImage
            Dim paymentDate As Date = Nothing
            Dim budgetCategoryOptionGroup As String
            Dim spanValue As String

            '++ Don't output items marked as deleted..
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_BREAKDOWN) Then
                row = New TableRow()
                row.ID = uniqueID

                If Not rowIndex.HasValue Then
                    '++ If we have no row index, then add the row at the end..
                    phPaymentBreakdown.Controls.Add(row)
                Else
                    '++ We have a row index, so insert at that position..
                    phPaymentBreakdown.Controls.AddAt(rowIndex.Value, row)
                End If

                '++ Budget category field..
                cell = New TableCell()
                row.Cells.Add(cell)
                If Utils.IsDate(hidDateFrom.Value) Then
                    paymentDate = Convert.ToDateTime(hidDateFrom.Value)
                End If
                cbo = CreateBudgetCategoryDropdown(CTRL_PREFIX_BREAKDOWN_BUDGET_CATEGORY & uniqueID, _
                                                    paymentDate, breakdownRec)
                cell.Controls.Add(cbo)
                cbo.DropDownList.Attributes.Add("onchange", String.Format("cboBudgetCategory_Change('{0}')", cbo.ClientID))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_BREAKDOWN_BUDGET_CATEGORY & "H" & uniqueID, cbo.DropDownList.SelectedValue))

                budgetCategoryOptionGroup = cbo.DropDownList.SelectedItem.Attributes(DropDownListAdapter.OPTGROUP_ATTRIBUTE_NAME)

                If budgetCategoryOptionGroup = _BudgetCategoryNotInSpendPlanOptionGroup Then
                    ' if we have at least one budget category not in spend plan
                    ' then show the legend to advise what this means

                    pnlLegend.Style.Add("display", "block")

                End If

                '++ Units field..
                cell = New TableCell()
                row.Cells.Add(cell)
                If breakdownRec IsNot Nothing Then
                    txt = CreateTextField(CTRL_PREFIX_BREAKDOWN_UNITS & uniqueID, _
                                          TextBoxEx.TextBoxExFormat.CurrencyFormat, _
                                          breakdownRec.Units.ToString("0.00"))
                Else
                    txt = CreateTextField(CTRL_PREFIX_BREAKDOWN_UNITS & uniqueID, _
                                          TextBoxEx.TextBoxExFormat.CurrencyFormat, _
                                          "0.00")
                End If
                cell.Controls.Add(txt)
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_BREAKDOWN_UNITS & "H" & uniqueID, txt.Value))
                _extraJS.AppendFormat("function {0}_Changed(id) {{ txtBreakdownUnits_Change(id); }};", txt.ID)

                '++ Unit Of Measure field..
                cell = New TableCell()
                row.Cells.Add(cell)
                spanValue = ""
                cell.Controls.Add(CreateSpan(spanValue))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_BREAKDOWN_UOM & uniqueID, spanValue))

                '++ Frequency field..
                Dim filterFreqID As Integer = -1
                cell = New TableCell()
                row.Cells.Add(cell)
                cbo = New DropDownListEx()
                cbo.ID = CTRL_PREFIX_BREAKDOWN_FREQUENCY & uniqueID
                SetupBreakdownFrequencyDropDown(cbo, breakdownRec)
                cell.Controls.Add(cbo)
                cbo.DropDownList.Attributes.Add("onchange", String.Format("cboBreakdownFrequency_OnChange('{0}')", cbo.ClientID))
                ctrlHidden = CreateHiddenField(CTRL_PREFIX_BREAKDOWN_FREQUENCY & "H" & uniqueID, cbo.DropDownList.SelectedValue)
                cell.Controls.Add(ctrlHidden)

                '++ Units Paid field..
                cell = New TableCell()
                row.Cells.Add(cell)
                If breakdownRec IsNot Nothing Then
                    spanValue = breakdownRec.FrequencyUnits.ToString("F2")
                Else
                    spanValue = "0.00"
                End If
                cell.Controls.Add(CreateSpan(spanValue))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_BREAKDOWN_UNITSPAID & "H" & uniqueID, spanValue))

                '++ Unit Cost field..
                cell = New TableCell()
                row.Cells.Add(cell)
                If breakdownRec IsNot Nothing Then
                    spanValue = breakdownRec.UnitCost.ToString("F2")
                Else
                    spanValue = "0.00"
                End If
                cell.Controls.Add(CreateSpan(spanValue))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_BREAKDOWN_UNITCOST & "H" & uniqueID, spanValue))

                '++ Amount (Units x Unit Cost) field..
                cell = New TableCell()
                row.Cells.Add(cell)

                If breakdownRec IsNot Nothing Then
                    spanValue = Convert.ToDecimal(breakdownRec.Units * breakdownRec.UnitCost).ToString("F2")
                Else
                    spanValue = "0.00"
                End If
                cell.Controls.Add(CreateSpan(spanValue))
                cell.Controls.Add(CreateHiddenField(CTRL_PREFIX_BREAKDOWN_LINECOST & "H" & uniqueID, spanValue))

                '++ Remove button..
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "right"
                btnRemove = New HtmlInputImage()
                With btnRemove
                    .ID = CTRL_PREFIX_BREAKDOWN_REMOVE & uniqueID
                    .Src = WebUtils.GetVirtualPath("images/delete.png")
                    .Alt = "Remove this payment breakdown line"
                    AddHandler .ServerClick, AddressOf btnRemoveBreakdown_Click
                    .Attributes.Add("onclick", "return btnRemoveBreakdown_Click();")
                End With
                cell.Controls.Add(btnRemove)
            End If

        End Sub

#End Region

#Region " Blanks "
        Private Function Blanks(Optional ByVal count As Integer = 1) As String
            Dim sSpace As String = "&nbsp;"
            Dim sReturn As String = ""

            For n As Integer = 1 To count
                sReturn = String.Concat(sReturn, sSpace)
            Next
            Return sReturn
        End Function
#End Region

#Region "SetupBreakdownFrequencyDropDown"

        ''' <summary>
        ''' Sets up the breakdown frequency drop down.
        ''' </summary>
        ''' <param name="dropDown">The drop down to setup.</param>
        Private Sub SetupBreakdownFrequencyDropDown(ByVal dropDown As DropDownListEx, ByVal breakdown As DPContractDetailBreakdown)

            Dim listItemsToFill As ListItemCollection = dropDown.DropDownList.Items
            Dim listItemToSelect As ListItem = Nothing

            ' setup the width 
            dropDown.Width = New Unit(15, UnitType.Em)

            If listItemsToFill.Count = 0 Then
                ' if we have no frequencies in the combo already then fill it

                Dim msg As New ErrorMessage()
                Dim frequencies As List(Of ViewableDirectPaymentContractFrequency) = Nothing

                ' get the frequencies for the current breakdown
                msg = DPContractBL.GetDirectContractPaymentBreakdownFrequencies(DbConnection, Nothing, GetAndSetCurrentPayment(), frequencies)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                For Each freq As ViewableDirectPaymentContractFrequency In frequencies
                    ' loop each frequency and add into the drop down list

                    listItemsToFill.Add(New ListItem(freq.Name, freq.ID))

                Next

            End If

            If Not breakdown Is Nothing Then
                ' if we have a breakdown then try and select its frequency from the
                ' list of frequencies in the drop down list

                ' find the value in the list items 
                listItemToSelect = listItemsToFill.FindByValue(breakdown.Frequency)

                If Not listItemToSelect Is Nothing Then
                    ' if we have an item in the list matching the selected id then select it

                    dropDown.DropDownList.SelectedValue = breakdown.Frequency

                End If

            End If

        End Sub

#End Region

#Region " CreateBudgetCategoryDropdown "
        Private Function CreateBudgetCategoryDropdown(ByVal uniqueID As String, _
                                                      ByVal paymentStartDate As Date, _
                                                      ByVal breakdownRec As DPContractDetailBreakdown) As DropDownListEx

            Dim ctrl As DropDownListEx

            ctrl = New DropDownListEx()
            With ctrl
                .ID = uniqueID
            End With

            If GetIfRequiresBudgetCategoryValidation() Then
                ' if the contract is sds and we have a validation method defined

                FillValidatedDropdownBudgetCategories(ctrl, paymentStartDate, breakdownRec)

            Else
                ' else use the original method of outputting budget categories

                FillDropdownBudgetCategory(ctrl, paymentStartDate, breakdownRec)

            End If

            Return ctrl

        End Function
#End Region

#Region " CreateTextField "
        Private Function CreateTextField(ByVal uniqueID As String, _
                                          ByVal textFormat As TextBoxEx.TextBoxExFormat, _
                                          ByVal textValue As String) As TextBoxEx
            Dim txt As TextBoxEx = New TextBoxEx()

            With txt
                .ID = uniqueID
                .Format = textFormat
                .Width = New Unit(4, UnitType.Em)
                .Text = textValue
                .Required = True
                .RequiredValidatorErrMsg = "A value is required"
                .ValidationGroup = "AddPayment"
            End With

            Return txt
        End Function
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

#Region " FillDropdownBudgetCategory "

        Private Sub FillDropdownBudgetCategory(ByRef cboField As DropDownListEx, ByVal paymentStartDate As Date, ByVal breakdownRec As DPContractDetailBreakdown)

            Dim cboItem As ListItem = Nothing
            Dim foundRedundant As Boolean = False
            Dim desc As String = "", type As String = "", redundantType As String = ""
            Dim rate As Decimal = 0
            Dim incomeRate As Decimal = 0
            Dim msg As New ErrorMessage()
            Dim rates As List(Of ViewableBudgetCategoryRates) = Nothing

            If cboField.DropDownList.Items.Count = 0 Then

                Dim dpContract As DPContract = Nothing
                Dim cbh As ClientBudgetHolder = Nothing

                ' date failsafe - should never be needed due to upstream screen checks..
                If paymentStartDate = Nothing Then paymentStartDate = DataUtils.MAX_DATE

                ' get contract
                msg = DPContractBL.GetDPContract(DbConnection, ParentContractID, dpContract)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' get client budget holder
                msg = BudgetHolderBL.GetClientBudgetHolder(DbConnection, dpContract.ClientBudgetHolderID, String.Empty, String.Empty, cbh)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' get rates for client and start date
                msg = BudgetCategoryBL.FetchBudgetCategoryRate(DbConnection, Nothing, paymentStartDate, cbh.ClientID, rates)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                cboItem = New ListItem(" ", String.Format("{0}${1}${2}$(blank)$0$-1$-1", 0, 1, -1))
                cboField.DropDownList.Items.Add(cboItem)

                ' loop each non redundant rate with a service type, or the current budget category id...if redundant
                For Each bcRate As ViewableBudgetCategoryRates In (From tmpRate In rates _
                                                                    Where (tmpRate.DomServiceTypeID > 0 _
                                                                            AndAlso tmpRate.Redundant = False) _
                                                                        OrElse (breakdownRec IsNot Nothing _
                                                                                AndAlso tmpRate.ID = breakdownRec.BudgetCategoryID) _
                                                                    Order By _
                                                                        tmpRate.Description _
                                                                    Select tmpRate).ToList()

                    type = CType(bcRate.Type, Integer)

                    If breakdownRec IsNot Nothing AndAlso breakdownRec.ID <> 0 AndAlso breakdownRec.BudgetCategoryID = bcRate.ID Then
                        ' if is existing record use its values

                        rate = breakdownRec.UnitCost
                        incomeRate = breakdownRec.IncomeUnitCost

                    Else
                        ' else use rates values

                        rate = bcRate.ExpenditureUnitRate
                        incomeRate = bcRate.IncomeUnitRate

                    End If

                    cboItem = New ListItem(bcRate.Description, String.Format("{0}${1}${2}${3}$0${4}${5}", _
                                                            bcRate.ID, _
                                                            type, _
                                                            rate.ToString("0.00"), _
                                                            bcRate.DomUnitsOfMeasureDesc, _
                                                            incomeRate.ToString("0.00"), _
                                                            "-1"))

                    cboField.DropDownList.Items.Add(cboItem)

                Next

            End If

            If breakdownRec IsNot Nothing Then
                Dim itemVal As String
                '++ Select the item referenced by the passed ID..
                For Each cboItem In cboField.DropDownList.Items
                    '++ Strip off the superfluous other values from the ID before evaluation..
                    If cboItem.Value.IndexOf("$") <> -1 Then
                        itemVal = cboItem.Value.Substring(0, cboItem.Value.IndexOf("$"))
                    Else
                        itemVal = cboItem.Value
                    End If

                    If Utils.ToInt32(itemVal) = breakdownRec.BudgetCategoryID Then
                        cboField.DropDownList.SelectedValue = cboItem.Value
                        Exit For
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' Fills the budget category drop down list with budget categories that are validatable.
        ''' </summary>
        ''' <param name="cboField">The cbo field.</param>
        ''' <param name="paymentStartDate">The payment start date.</param>
        ''' <param name="breakdownRec">The breakdown rec.</param>
        Private Sub FillValidatedDropdownBudgetCategories(ByRef cboField As DropDownListEx, ByVal paymentStartDate As Date, ByVal breakdownRec As DPContractDetailBreakdown)

            Dim msg As ErrorMessage = Nothing

            If cboField.DropDownList.Items.Count = 0 AndAlso GetIfRequiresBudgetCategoryValidation() Then
                ' if the contract is sds and we have a validation method defined

                Dim budgetCategories As New List(Of ViewableBudgetCategoryRates)()
                Dim cboItem As New ListItem(" ", "0$1$-1$(blank)$0$-1$-1")
                Dim validationMethod As DPContractBL.BudgetCategoryValidationMethod = GetBudgetCategoryValidationMethod()

                ' get the dp contract budget categories for this payment
                budgetCategories = GetDpContractBudgetCategories()

                ' add intial blank item into the collection
                cboField.DropDownList.Items.Add(cboItem)
                cboItem.Selected = False

                If Not budgetCategories Is Nothing AndAlso budgetCategories.Count > 0 Then
                    ' if we have some items to add into the list

                    Dim tmpBudgetCategories As New List(Of ViewableBudgetCategoryRates)()

                    ' add in budget categories that are in a spend plan, order by service delivered via and description
                    tmpBudgetCategories.AddRange(From tmpBudgetCategory As ViewableBudgetCategoryRates In budgetCategories _
                                                     Where _
                                                        tmpBudgetCategory.SpendPlanID > 0 _
                                                        AndAlso tmpBudgetCategory.DomServiceTypeID > 0 _
                                                     Order By _
                                                        tmpBudgetCategory.SpendPlanServiceDeliveredVia, _
                                                        tmpBudgetCategory.Description _
                                                     Select tmpBudgetCategory)

                    ' add in budget categories that not in a spend plan and redundant, order by description
                    tmpBudgetCategories.AddRange(From tmpBudgetCategory As ViewableBudgetCategoryRates In budgetCategories _
                                                     Where _
                                                        tmpBudgetCategory.SpendPlanID <= 0 _
                                                        AndAlso tmpBudgetCategory.Redundant = False _
                                                        AndAlso tmpBudgetCategory.DomServiceTypeID > 0 _
                                                     Order By _
                                                        tmpBudgetCategory.Description _
                                                     Select tmpBudgetCategory)

                    ' allocate reorder/filtered budget categories to budget categories
                    budgetCategories = tmpBudgetCategories

                    For Each budgetCategory As ViewableBudgetCategoryRates In budgetCategories
                        ' loop each budget category and add into the drop down list

                        If budgetCategory.SpendPlanID > 0 OrElse (budgetCategory.SpendPlanID <= 0 AndAlso budgetCategory.Redundant = False) Then
                            ' only add items that are in a spend plan or arent in a spend plan and arent redundant

                            AddValidatedDropdownBudgetCategory(cboField, breakdownRec, budgetCategory, validationMethod)

                        End If

                    Next

                    If (cboField.DropDownList.SelectedItem Is Nothing OrElse cboField.DropDownList.SelectedItem.Text.Trim().Length = 0) AndAlso Not breakdownRec Is Nothing Then
                        ' if we havent selected an item then we need to add a redundant budget category
                        ' into the list as it was previously selected and now redundant

                        Dim cboFieldItemTokens() As String

                        For Each cboFieldItem As ListItem In cboField.DropDownList.Items
                            ' loop each item in the drop down list and try and select one

                            ' split the value of this item into an array
                            cboFieldItemTokens = cboFieldItem.Value.Split("$")

                            If breakdownRec.BudgetCategoryID = Utils.ToInt32(cboFieldItemTokens(0)) Then
                                ' if the budget categories match then select this item and exit the loop

                                cboFieldItem.Selected = True
                                Exit For

                            End If

                        Next

                        If cboField.DropDownList.SelectedItem Is Nothing Then
                            ' no item has been selected so fetch the budget category from the db and insert

                            Dim budgetCategory As ViewableBudgetCategoryRates = (From bcs In budgetCategories _
                                                                                  Where bcs.ID = breakdownRec.BudgetCategoryID _
                                                                                  Select bcs).FirstOrDefault()

                            AddValidatedDropdownBudgetCategory(cboField, breakdownRec, budgetCategory, validationMethod)

                        End If

                    End If

                End If

            Else
                ' else use the original method of outputting budget categories

                FillDropdownBudgetCategory(cboField, paymentStartDate, breakdownRec)

            End If

        End Sub

        ''' <summary>
        ''' Adds the validated dropdown budget category.
        ''' </summary>
        ''' <param name="cboField">The cbo field.</param>
        ''' <param name="breakdownRec">The breakdown rec.</param>
        ''' <param name="budgetCategory">The budget category.</param>
        ''' <param name="validationMethod">The validation method.</param>
        Private Sub AddValidatedDropdownBudgetCategory(ByRef cboField As DropDownListEx, _
                                                       ByVal breakdownRec As DPContractDetailBreakdown, _
                                                       ByVal budgetCategory As ViewableBudgetCategoryRates, _
                                                       ByVal validationMethod As DPContractBL.BudgetCategoryValidationMethod)

            If Not budgetCategory Is Nothing Then

                Dim cboItem As New ListItem(budgetCategory.Description)
                Dim rate As Decimal = 0.0
                Dim bcIncomeUnitRate As Decimal = -1
                Dim bcOverriddenWeeklyCharge As Decimal = -1

                If breakdownRec IsNot Nothing AndAlso breakdownRec.ID <> 0 _
                    AndAlso breakdownRec.BudgetCategoryID = budgetCategory.ID _
                    AndAlso breakdownRec.ServiceDeliveredVia = CType(budgetCategory.SpendPlanServiceDeliveredVia, Integer) Then
                    ' if the breakdown record matches the budget category and service delivered via flag

                    rate = breakdownRec.UnitCost
                    cboItem.Selected = True

                Else

                    If Not budgetCategory.ExpenditureUnitRate Is Nothing AndAlso budgetCategory.ExpenditureUnitRate.HasValue Then

                        rate = budgetCategory.ExpenditureUnitRate

                    Else

                        rate = 0.0

                    End If

                End If

                If budgetCategory.SpendPlanID > 0 Then
                    ' if this budget category is in the spend plan then group it

                    If budgetCategory.SpendPlanServiceDeliveredVia = SpendPlanBL.SpendPlanDetailServiceDeliveredVia.DirectPayment Then
                        ' if this bc is delivered via direct payment

                        cboItem.Attributes(DropDownListAdapter.OPTGROUP_ATTRIBUTE_NAME) = _BudgetCategoryInSpendPlanOptionGroup
                        cboItem.Attributes("title") = _ImageComplete

                    Else
                        ' else not delivered by direct payment

                        cboItem.Attributes(DropDownListAdapter.OPTGROUP_ATTRIBUTE_NAME) = _BudgetCategoryInSpendPlanNotDpOptionGroup

                        Select Case validationMethod

                            Case DPContractBL.BudgetCategoryValidationMethod.PreventWhenNotInSpendPlan

                                cboItem.Attributes("title") = _ImageBlockedSoft

                            Case DPContractBL.BudgetCategoryValidationMethod.WarnWhenNotInSpendPlan, _
                                 DPContractBL.BudgetCategoryValidationMethod.PreventWhenNotInSpendPlanWarnOnNotDirectPayment

                                cboItem.Attributes("title") = _ImageWarningSoft

                        End Select

                    End If

                Else
                    ' else this budget category isnt in the spend plan then group it elsewhere

                    cboItem.Attributes(DropDownListAdapter.OPTGROUP_ATTRIBUTE_NAME) = _BudgetCategoryNotInSpendPlanOptionGroup

                    If validationMethod = DPContractBL.BudgetCategoryValidationMethod.PreventWhenNotInSpendPlan _
                        OrElse validationMethod = DPContractBL.BudgetCategoryValidationMethod.PreventWhenNotInSpendPlanWarnOnNotDirectPayment Then

                        cboItem.Attributes("title") = _ImageBlocked

                    Else

                        cboItem.Attributes("title") = _ImageWarning

                    End If

                End If

                If budgetCategory.IncomeUnitRate.HasValue Then

                    bcIncomeUnitRate = budgetCategory.IncomeUnitRate.Value

                End If

                cboItem.Value = String.Format("{0}${1}${2}${3}${4}${5}${6}", _
                                               budgetCategory.ID, _
                                               CType(budgetCategory.Type, Integer), _
                                               rate.ToString("0.00"), _
                                               budgetCategory.DomUnitsOfMeasureDesc, _
                                               CType(budgetCategory.SpendPlanServiceDeliveredVia, Integer), _
                                               bcIncomeUnitRate.ToString("0.00"), _
                                               bcOverriddenWeeklyCharge.ToString("0.00"))

                cboField.DropDownList.Items.Add(cboItem)

            End If

        End Sub

#End Region

#Region " PopulatePaymentBreakdowns "
        Private Function PopulatePaymentBreakdowns(ByVal dpcBreakdowns As DPContractDetailBreakdownCollection) As ErrorMessage
            Dim recList As List(Of DPContractDetailBreakdown) = New List(Of DPContractDetailBreakdown)
            Dim msg As ErrorMessage = New ErrorMessage()
            btnReconsider.Enabled = False
            For Each dpcBreakdown As DPContractDetailBreakdown In dpcBreakdowns
                recList.Add(dpcBreakdown)
            Next
            msg = PopulatePaymentBreakdowns(recList)
            Return msg
        End Function

        Private Function PopulatePaymentBreakdowns(ByVal dpcBreakdownList As List(Of DPContractDetailBreakdown)) As ErrorMessage

            Dim msg As ErrorMessage
            Dim itemIDs As List(Of String), newItemIDs As List(Of String) = New List(Of String)
            Dim id As String
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reconsiderThisPayment As Boolean = False

            itemIDs = GetBreakdownIDsFromViewState()
            ClearViewState()

            '++ Create a new item line for each payment breakdown held..
            Dim counter As Integer = 0
            If dpcBreakdownList IsNot Nothing AndAlso dpcBreakdownList.Count > 0 Then
                For Each dpcBreakdown As DPContractDetailBreakdown In dpcBreakdownList
                    id = FetchBreakdownID(dpcBreakdown)
                    OutputBreakdownControls(id, dpcBreakdown, Nothing)
                    itemIDs.Add(id)

                    If dpcBreakdown.Reconsider = TriState.True Then
                        lblError.Text = RECONSIDER_TEXT
                        reconsiderThisPayment = True
                    End If
                Next
            End If

            '++ Flag this payment as requiring reconsideration if any breakdowns so marked..
            If reconsiderThisPayment Then

                Dim allowReconsider As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.Reconsider"))

                cp.HeaderLinkText = String.Format("*{0}{1}", Blanks(3), cp.HeaderLinkText)
                hidTitle.Value = cp.HeaderLinkText

                If allowReconsider Then

                    btnReconsider.Enabled = True

                Else

                    btnReconsider.Enabled = False

                End If

            Else
                ' else this payment line does not need reconsideration

                btnReconsider.Enabled = False

            End If

            PersistBreakdownIDsToViewState(itemIDs)

            msg = New ErrorMessage()
            msg.Success = True

            Return msg

        End Function
#End Region

#Region " GetBreakdownRecordsFromScreen "
        Private Function GetBreakdownRecordsFromScreen(ByRef detailLines As List(Of DPContractDetailBreakdown)) As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim id As String
            Dim detailList As List(Of String)
            Dim budCategoryID As Integer, budCatValue As String
            Dim units As Decimal, unitRate As Decimal
            Dim freqID As Integer, freqUnits As Decimal
            Dim line As DPContractDetailBreakdown = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            detailList = GetBreakdownIDsFromViewState()
            detailLines = New List(Of DPContractDetailBreakdown)

            For Each id In detailList
                If Not id.StartsWith(UNIQUEID_PREFIX_DELETE_BREAKDOWN) Then
                    '++ Populate a new DPContractDetailBreakdown instance..
                    line = New DPContractDetailBreakdown(Me.DbConnection, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))

                    '++ Discard the "Type, etc" section of the compound Dropdown value (ID<Type>UnitCost?UOM)..
                    budCatValue = Utils.ToString(CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_BUDGET_CATEGORY & "H" & id), HiddenField).Value)
                    If budCatValue.IndexOf("$") <> -1 Then
                        budCategoryID = Utils.ToInt32(budCatValue.Substring(0, budCatValue.IndexOf("$")))
                    Else
                        budCategoryID = 0
                    End If

                    units = Utils.ToDecimal(CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_UNITS & "H" & id), HiddenField).Value)
                    unitRate = Utils.ToDecimal(CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_UNITCOST & "H" & id), HiddenField).Value)
                    freqID = Utils.ToInt32(CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_FREQUENCY & "H" & id), HiddenField).Value)
                    freqUnits = Utils.ToDecimal(CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_UNITSPAID & "H" & id), HiddenField).Value)

                    With line
                        .BudgetCategoryID = budCategoryID
                        .Units = units
                        .UnitCost = unitRate
                        .Frequency = freqID
                        .FrequencyUnits = freqUnits
                    End With
                    detailLines.Add(line)
                End If
            Next

            msg = New ErrorMessage()
            msg.Success = True

            Return msg
        End Function
#End Region

#Region " btnAddBreakdown_Click "
        Private Sub btnAddBreakdown_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddBreakdown.Click
            Dim newID As String
            Dim list As List(Of String)
            Dim newLine As DPContractDetailBreakdown = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            '++ Add a new DPContractDetailBreakdown line..
            newLine = New DPContractDetailBreakdown(Me.DbConnection, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))
            list = GetBreakdownIDsFromViewState()
            newID = FetchBreakdownID(newLine)

            '++ Create the controls for the new line..
            OutputBreakdownControls(newID, newLine, Nothing)

            cp.HeaderLinkText = hidTitle.Value
            dteDateFrom.Text = hidDateFrom.Value
            Dim reasonID As String
            If hidEndReason.Value.IndexOf("<") <> -1 Then
                reasonID = hidEndReason.Value.Substring(0, hidEndReason.Value.IndexOf("<"))
            Else
                reasonID = hidEndReason.Value
            End If
            FillDropdownEndReason(reasonID)

            '++ Persist the data into view state..
            list.Add(newID)
            PersistBreakdownIDsToViewState(list)
        End Sub

#End Region

#Region " btnRemoveDetail_Click "
        Private Sub btnRemoveBreakdown_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim list As List(Of String)
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_BREAKDOWN_REMOVE, String.Empty)

            list = GetBreakdownIDsFromViewState()

            ' change the id in viewstate
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_BREAKDOWN) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_BREAKDOWN, UNIQUEID_PREFIX_DELETE_BREAKDOWN)
                    End If
                    Exit For
                End If
            Next

            For Each oldID As String In list
                If Not oldID.StartsWith(UNIQUEID_PREFIX_DELETE_BREAKDOWN) Then
                    Dim itemID As String
                    Dim hidItemID As String = Utils.ToString(CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_BUDGET_CATEGORY & "H" & oldID), HiddenField).Value)

                    If hidItemID <> "" Then
                        If hidItemID.IndexOf("$") <> -1 Then
                            hidItemID = hidItemID.Substring(0, hidItemID.IndexOf("$"))
                        End If
                        For Each item As ListItem In CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_BUDGET_CATEGORY & oldID), DropDownListEx).DropDownList.Items
                            If item.Value.IndexOf("$") <> -1 Then
                                itemID = item.Value.Substring(0, item.Value.IndexOf("$"))
                            Else
                                itemID = item.Value
                            End If
                            If itemID = hidItemID Then
                                CType(phPaymentBreakdown.FindControl(CTRL_PREFIX_BREAKDOWN_BUDGET_CATEGORY & oldID), DropDownListEx).DropDownList.SelectedValue = item.Value
                                Exit For
                            End If
                        Next
                    End If
                End If
            Next

            ' remove from the grid
            For index As Integer = 0 To phPaymentBreakdown.Controls.Count - 1
                If phPaymentBreakdown.Controls(index).ID = id Then
                    phPaymentBreakdown.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            cp.HeaderLinkText = hidTitle.Value
            dteDateFrom.Text = hidDateFrom.Value
            Dim reasonID As String
            If hidEndReason.Value.IndexOf("<") <> -1 Then
                reasonID = hidEndReason.Value.Substring(0, hidEndReason.Value.IndexOf("<"))
            Else
                reasonID = hidEndReason.Value
            End If
            FillDropdownEndReason(reasonID)

            ' persist the data into view state
            PersistBreakdownIDsToViewState(list)
        End Sub
#End Region

#Region " btnReconsider_Click "
        Protected Sub btnReconsider_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim paymentRec As DPContractDetail = Nothing
            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            '++ No need to reconsider a payment not yet saved..
            If PaymentID > 0 Then
                paymentRec = New DPContractDetail(Me.DbConnection, _
                                                  currentUser.ExternalUsername, _
                                                  String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))

                msg = DPContractBL.FetchPayment(Me.DbConnection, PaymentID, paymentRec)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If msg.Success Then
                    msg = DPContractBL.ReconsiderPayment(DbConnection, paymentRec, _
                                                         currentUser.ExternalUsername, _
                                                         String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"), Nothing)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                    Else
                        lblError.Text = ""
                        '++ Refresh the parent contract screen, since there may be additional
                        '++ payments which need to also be displayed..
                        Me.ClientScript.RegisterStartupScript( _
                            Me.GetType(), _
                            "ReconsiderPayment", _
                            "window.parent.EditPayment_ReconsiderClicked();", _
                            True)
                    End If
                End If
            End If
        End Sub
#End Region

#Region " ClearViewState "
        Private Sub ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_DATA_BREAKDOWNS)
            ViewState.Remove(VIEWSTATE_KEY_COUNTER_NEW_BREAKDOWNS)
            phPaymentBreakdown.Controls.Clear()
        End Sub
#End Region

#Region " GetBreakdownIDsFromViewState "
        Private Function GetBreakdownIDsFromViewState() As List(Of String)
            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_BREAKDOWNS) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_BREAKDOWNS), List(Of String))
            End If

            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_BREAKDOWNS) Is Nothing Then
                _newDetailIDCounter = 0
            Else
                _newDetailIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_BREAKDOWNS), Integer)
            End If
            Return list

        End Function

#End Region

#Region " FetchBreakdownID "
        Private Function FetchBreakdownID(ByVal detail As DPContractDetailBreakdown) As String
            Dim id As String

            If detail.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_BREAKDOWN & _newDetailIDCounter
                'Do Until CType(phPaymentBreakdown.FindControl(id), HiddenField) Is Nothing
                _newDetailIDCounter += 1
                'id = UNIQUEID_PREFIX_NEW_BREAKDOWN & _newDetailIDCounter
                'Loop
            Else
                id = UNIQUEID_PREFIX_UPDATE_BREAKDOWN & detail.ID
            End If

            Return id
        End Function
#End Region

#Region " PersistBreakdownIDsToViewState "
        Private Sub PersistBreakdownIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_BREAKDOWNS, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_BREAKDOWNS, _newDetailIDCounter)
        End Sub

#End Region

#Region " GetRemovedDetailsToViewState "

        Private Function GetRemovedDetailsToViewState() As List(Of Triplet)

            Dim list As List(Of Triplet)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_BREAKDOWNS_REMOVED) Is Nothing Then
                list = New List(Of Triplet)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_BREAKDOWNS_REMOVED), List(Of Triplet))
            End If

            Return list

        End Function

#End Region

#Region " PersistRemovedDetailsToViewState "

        Private Sub PersistRemovedDetailsToViewState(ByVal list As List(Of Triplet))
            ViewState.Add(VIEWSTATE_KEY_DATA_BREAKDOWNS_REMOVED, list)
        End Sub

#End Region

#Region " GetNumberOfPayments "
        ''' <summary>
        ''' Determine the number of payments indicated by the passed start and end dates
        ''' as well as the current selection in the Frequency dropdown.
        ''' </summary>
        ''' <param name="startDate">The contract payment start date</param>
        ''' <param name="endDate">The contract payment end date</param>
        ''' <param name="frequencyID">The selected frequency ID</param>
        ''' <returns></returns>
        ''' <remarks>The </remarks>
        Private Function GetNumberOfPayments(ByVal startDate As Date, _
                                             ByVal endDate As Date, _
                                             ByVal frequencyID As String, _
                                             ByRef count As Integer) As ErrorMessage
            Dim msg As New ErrorMessage
            Dim intervalType As DateInterval = DateInterval.Month
            Dim freqRec As Lookup = New Lookup(Me.DbConnection)
            Dim freqID As Integer
            Dim increment As Integer
            Dim targetDate As Date = startDate

            Try
                count = 0
                freqID = Utils.ToInt32(frequencyID)
                If freqID = 0 OrElse freqID = -1 Then
                    msg.Success = True
                    Return msg
                End If

                msg = freqRec.Fetch(freqID)
                If Not msg.Success Then Return msg

                Select Case freqRec.Description
                    Case "WEEKLY", "TWO-WEEKLY", "FOUR-WEEKLY", "QUARTERLY (13 weeks)", "HALF-YEARLY (26 weeks)", "ANNUALLY (56 weeks)"
                        increment = Convert.ToDecimal(7 / freqRec.InfoValue).ToString("0")
                        intervalType = DateInterval.Day
                    Case "MONTHLY"
                        increment = 1
                    Case "QUARTERLY"
                        increment = 3
                    Case "HALF-YEARLY"
                        increment = 6
                    Case "ANNUALLY"
                        increment = 12
                    Case Else
                        increment = Convert.ToDecimal(7 / freqRec.InfoValue).ToString("0")
                End Select

                Do Until targetDate >= endDate
                    count += 1
                    targetDate = DateAdd(intervalType, Utils.ToInt32(Convert.ToDecimal(count * increment).ToString("0")), startDate)
                    targetDate = DateAdd(DateInterval.Day, -1, targetDate)
                Loop

            Catch ex As Exception
                ' catch and wrap exception

                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)

            End Try

            Return msg
        End Function
#End Region

#Region "Properties"

        Private _SpendPlanID As Nullable(Of Integer)

        ''' <summary>
        ''' Gets or sets the spend plan ID.
        ''' </summary>
        ''' <value>The spend plan ID.</value>
        Public Property SpendPlanID() As Nullable(Of Integer)
            Get
                Return _SpendPlanID
            End Get
            Set(ByVal value As Nullable(Of Integer))
                _SpendPlanID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the finance code selector.
        ''' </summary>
        ''' <value>The finance code selector.</value>
        Private ReadOnly Property FinanceCodeSelector() As InPlaceFinanceCodeSelector
            Get
                Return CType(txtFinCode1, InPlaceFinanceCodeSelector)
            End Get
        End Property

        Private _ParentContract As DPContract = Nothing

        ''' <summary>
        ''' Gets the parent contract.
        ''' </summary>
        ''' <value>The parent contract.</value>
        Private ReadOnly Property ParentContract() As DPContract
            Get
                If _ParentContract Is Nothing AndAlso DbConnection IsNot Nothing Then
                    Dim msg As New ErrorMessage()
                    _ParentContract = New DPContract(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    msg = _ParentContract.Fetch(ParentContractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _ParentContract
            End Get
        End Property

        ''' <summary>
        ''' Gets the parent contract ID.
        ''' </summary>
        ''' <value>The parent contract ID.</value>
        Private ReadOnly Property ParentContractID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString("dpcid"))
            End Get
        End Property

        ''' <summary>
        ''' Gets the payment ID.
        ''' </summary>
        Private ReadOnly Property PaymentID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString("id"))
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
                Return (Not ParentContract Is Nothing AndAlso ParentContract.IsMaintainedExternally = TriState.True)
            End Get
        End Property

#End Region

#Region "Helper Methods"

        Private _GetBudgetCategoryValidationMethodResult As Nullable(Of DPContractBL.BudgetCategoryValidationMethod) = Nothing

        ''' <summary>
        ''' Gets the budget category validation method.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetBudgetCategoryValidationMethod() As DPContractBL.BudgetCategoryValidationMethod

            Dim msg As New ErrorMessage()
            Dim method As DPContractBL.BudgetCategoryValidationMethod = DPContractBL.BudgetCategoryValidationMethod.NotSpecified
            Dim contract As DPContract = GetDpContract()

            If contract.IsSDS Then
                ' if the dp contract is sds then determine the validation method

                If _GetBudgetCategoryValidationMethodResult Is Nothing OrElse _GetBudgetCategoryValidationMethodResult.HasValue = False Then
                    ' if we havent fetched the method already then attempt to fetch

                    msg = DPContractBL.GetBudgetCategoryValidationMethod(DbConnection, method)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _GetBudgetCategoryValidationMethodResult = method

                Else
                    ' else already fetched so return this result

                    method = _GetBudgetCategoryValidationMethodResult

                End If

            Else
                ' else isnt sds so return not specified

                method = DPContractBL.BudgetCategoryValidationMethod.NotSpecified

            End If

            Return method

        End Function

        Private _GetDpContractResult As DPContract = Nothing

        ''' <summary>
        ''' Gets the dp contract for this payment line. Basic criteria only.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetDpContract() As DPContract

            Dim contract As DPContract = Nothing
            Dim msg As New ErrorMessage()

            If _GetDpContractResult Is Nothing Then
                ' if we havent fetched the contract already then attempt to fetch

                msg = DPContractBL.GetDPContract(DbConnection, ParentContractID, contract)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _GetDpContractResult = contract

            Else

                contract = _GetDpContractResult

            End If

            Return contract

        End Function

        Private _GetDpContractBudgetCategoriesResult As List(Of ViewableBudgetCategoryRates)

        ''' <summary>
        ''' Gets the dp contract budget categories.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetDpContractBudgetCategories() As List(Of ViewableBudgetCategoryRates)

            Dim list As List(Of ViewableBudgetCategoryRates) = Nothing
            Dim msg As New ErrorMessage()

            If _GetDpContractBudgetCategoriesResult Is Nothing Then
                ' if we havent fetched the list already then attempt to fetch

                Dim cbh As ClientBudgetHolder = Nothing
                Dim contract As DPContract = Nothing
                Dim dateFrom As DateTime = DateTime.MaxValue

                If Utils.IsDate(hidDateFrom.Value) Then
                    ' if we have date from 

                    dateFrom = Convert.ToDateTime(hidDateFrom.Value)

                Else

                    Dim paymentLineID As Integer = PaymentID
                    Dim paymentLine As DPContractDetail = Nothing

                    If paymentLineID > 0 Then
                        ' if we have fetched an id for the payment line then fetch the payment line

                        msg = DPContractBL.GetDPContractDetail(DbConnection, Nothing, paymentLineID, paymentLine)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        dateFrom = paymentLine.DateFrom

                    End If

                End If

                ' get the dp contract
                contract = GetDpContract()

                ' get the client budget holder so we can get a client id
                msg = BudgetHolderBL.GetClientBudgetHolder(DbConnection, contract.ClientBudgetHolderID, String.Empty, String.Empty, cbh)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' get the budget categories
                msg = BudgetCategoryBL.FetchBudgetCategoryRate(DbConnection, Nothing, dateFrom, cbh.ClientID, list)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _GetDpContractBudgetCategoriesResult = list

                ' get the first spend plan id in the list, will only ever be one spend plan id as filtering by client, if not rpesent will use 0
                SpendPlanID = (From tmpBc In list Where tmpBc.SpendPlanID > 0 Select tmpBc.SpendPlanID).FirstOrDefault()

            Else
                ' else already fetched so return this result

                list = _GetDpContractBudgetCategoriesResult

            End If

            Return list

        End Function

        ''' <summary>
        ''' Gets if requires budget category validation.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetIfRequiresBudgetCategoryValidation() As Boolean

            Dim contract As DPContract = Nothing
            Dim validationMethod As DPContractBL.BudgetCategoryValidationMethod = DPContractBL.BudgetCategoryValidationMethod.NotSpecified

            ' fetch the dp contract record for this payment line
            contract = GetDpContract()

            ' fetch the validation method
            validationMethod = GetBudgetCategoryValidationMethod()

            If contract.IsSDS AndAlso validationMethod <> DPContractBL.BudgetCategoryValidationMethod.NotSpecified Then

                Return True

            Else

                Return False

            End If

        End Function

        ''' <summary>
        ''' Gets the current payment from the db and caches it for reuse.
        ''' </summary>
        ''' <returns>The payment</returns>
        Private Function GetAndSetCurrentPayment() As DPContractDetail

            If _dpConPayment Is Nothing Then
                ' if we have no cached copy of the payment then fetch it

                Dim msg As New ErrorMessage()
                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

                ' init a new contract payment
                _dpConPayment = New DPContractDetail(Me.DbConnection, currentUser.ExternalUsername, String.Format("{0}:{1}", Me.Settings.CurrentApplication, "Direct Payment Contracts"))

                If PaymentID > 0 AndAlso _IsDeleteOperation = False Then
                    ' if we have a payment id then fetch one back

                    msg = DPContractBL.FetchPayment(DbConnection, PaymentID, _dpConPayment)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                Else
                    ' else just create a dummy item

                    With _dpConPayment
                        .DPContractID = ParentContractID
                        .FrequencyID = Utils.ToInt32(hidFrequency.Value)
                    End With

                End If

            End If

            Return _dpConPayment

        End Function

#End Region

    End Class
End Namespace