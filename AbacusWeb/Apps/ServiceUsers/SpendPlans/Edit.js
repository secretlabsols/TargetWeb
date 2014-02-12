var Edit_uoms, Edit_budgetCategories, Edit_detailTotalID, Edit_spendPlanID;
var dteDateTo, dteDateToID, lblEndReason, lblEndReasonID;
var cboBudgetCatChanged = false;
var Edit_DefaultBudgetPeriod, Edit_dateFrom, Edit_dteDateFromID, Edit_ClientID;
var Edit_spendPlanSvc;

function Init() {
    dteDateTo = GetElement(dteDateToID);
    lblEndReason = GetElement(lblEndReasonID);
    Edit_spendPlanSvc = new Target.Abacus.Web.Apps.WebSvc.SpendPlans_class();
}

function tabStrip_ActiveTabChanged(sender, args) {
    var hidSelectedTab = GetElement("hidSelectedTab");
    hidSelectedTab.value = sender.get_activeTab().get_headerText();
}

function cboBudgetCat_Change(cboBudgetCategoryID, txtUnitsID, cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID, cboServiceDetailFrequencyID, lblAnnualUnitsID, lblGrossAnnualCostID) {

    var cboServiceDeliveredVia = GetElement(cboServiceDeliveredViaID + "_cboDropDownList");
    cboServiceDeliveredVia.value = '';
    cboServiceDeliveredVia_Change(cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID);
    cboBudgetCatChanged = true;
    PrimeMeasuredIn(cboBudgetCategoryID, txtUnitsID, cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID, cboServiceDetailFrequencyID, lblAnnualUnitsID, lblGrossAnnualCostID);
    cboBudgetCatChanged = false;
}

function cboServiceDeliveredVia_Change(cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID) {

    var cboServiceDeliveredVia = GetElement(cboServiceDeliveredViaID + "_cboDropDownList");
    var cboServiceDeliveredViaHidden = GetElement(cboServiceDeliveredViaHiddenID);

    cboServiceDeliveredViaHidden.value = cboServiceDeliveredVia.value;
}

function cboServiceDetailFrequency_Change(cboBudgetCategoryID, txtUnitsID, cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID, cboServiceDetailFrequencyID, lblAnnualUnitsID, lblGrossAnnualCostID) {
    PrimeMeasuredIn(cboBudgetCategoryID, txtUnitsID, cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID, cboServiceDetailFrequencyID, lblAnnualUnitsID, lblGrossAnnualCostID);
}

function txtUnits_Change(cboBudgetCategoryID, txtUnitsID, cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID, cboServiceDetailFrequencyID, lblAnnualUnitsID, lblGrossAnnualCostID) {
    PrimeMeasuredIn(cboBudgetCategoryID, txtUnitsID, cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID, cboServiceDetailFrequencyID, lblAnnualUnitsID, lblGrossAnnualCostID);
}

function PrimeMeasuredIn(cboBudgetCategoryID, txtUnitsID, cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID, cboServiceDetailFrequencyID, lblAnnualUnitsID, lblGrossAnnualCostID) {

    var tdUnitsIndex = 2;
    var tdMeasuredInIndex = 3;
    var tdCostIndex = 6;
    var cost = 0;

    var cbo = GetElement(cboBudgetCategoryID + "_cboDropDownList");
    var txtUnits = GetElement(txtUnitsID + "_txtTextBox");
    var row = cbo.parentNode.parentNode;
    var tdMeasuredIn = row.cells[tdMeasuredInIndex];
    var tdCost = row.cells[tdCostIndex];
    var spanMeasuredIn = tdMeasuredIn.getElementsByTagName("SPAN")[0];
    var spanCost = tdCost.getElementsByTagName("SPAN")[0];
    var cboServiceDeliveredVia = GetElement(cboServiceDeliveredViaID + "_cboDropDownList");
    var cboServiceDeliveredViaHidden = GetElement(cboServiceDeliveredViaHiddenID);
    var cboServiceDetailFrequency = GetElement(cboServiceDetailFrequencyID + "_cboDropDownList");
    var lblAnnualUnits = GetElement(lblAnnualUnitsID);
    var lblGrossAnnualCost = GetElement(lblGrossAnnualCostID);
    var annualUnits = 0;
    var grossAnnualUnits = 0;

    var uomID = 0;
    var uomDesc = " ";
    var uomCost = "";

    if (Edit_budgetCategories.item(cbo.value)) {
        uomID = Edit_budgetCategories.item(cbo.value)[0];
            if (Edit_budgetCategories.item(cbo.value)[2] == '1') {
            //monetary
            uomDesc = "£"
            cost = txtUnits.value.replace(/,/g, "");
        } else {
            //Units of Service
            uomDesc = Edit_uoms.item(uomID)[0];
            cost = Edit_budgetCategories.item(cbo.value)[1];
        }

        
        if (Edit_budgetCategories.item(cbo.value)[3] == '3') {
            // service catgeory is cash so select dp only
            cboServiceDeliveredVia.value = '1';
            cboServiceDeliveredVia.disabled = true;
            cboServiceDeliveredVia_Change(cboServiceDeliveredViaID, cboServiceDeliveredViaHiddenID);       
        }
        else {            
            cboServiceDeliveredVia.disabled = false;
        }        
    }

    if (cboServiceDetailFrequency.value != '') {
        annualUnits = Math.abs(cboServiceDetailFrequency.value);
    }

    annualUnits = annualUnits * parseFloat(txtUnits.value.replace(/,/g,""));
    grossAnnualUnits = annualUnits * parseFloat(cost);

    SetInnerText(lblAnnualUnits, parseFloat(annualUnits).toFixed(2));
    SetInnerText(lblGrossAnnualCost, parseFloat(grossAnnualUnits).toFixed(2));
    SetInnerText(spanMeasuredIn, uomDesc);
    spanCost.innerHTML = parseFloat(cost).toFixed(2);
    PrimeTotalcost();
}

function PrimeTotalcost() {
    var tblDetail, cellCost;
    var totalCost = 0;
    var txtTotal = GetElement(Edit_detailTotalID);
    tblDetail = GetElement("tblDetails");
    for (index = 0; index < tblDetail.tBodies[0].rows.length; index++) {
        cellCost = Number(GetInnerText(tblDetail.tBodies[0].rows[index].cells[7]).replace(/[^0-9\.]+/g, ""));
        totalCost += cellCost;
    }
    txtTotal.innerHTML = totalCost.toString().formatCurrency();
}

function btnTerminate_Click() {
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/SpendPlans/Terminate.aspx?id=" + Edit_spendPlanID + "&backUrl=" + GetBackUrl();
    var dialog = OpenDialog(url, 40, 24, window);
}

function GetBackUrl() {
    var url = document.location.href;
    url = RemoveQSParam(url, "id");
    url = AddQSParam(url, "id", Edit_spendPlanID);
    
    return escape(url);
}

function SpendPlan_Terminated(endDate, endReason) {
    SetInnerText(dteDateTo, endDate);
    lblEndReason.innerText = '(' + endReason + ')';
}

function spendPlan_CanSave() {
    var response;
    if (Page_ClientValidate('Save')) {

        if (Edit_DefaultBudgetPeriod == 3) {
            var txtdatefrom = GetElement(Edit_dteDateFromID + "_txtTextBox");
            response = Edit_spendPlanSvc.DisplayOverlappingWarningMessage(Edit_spendPlanID, Edit_ClientID, txtdatefrom.value)
            if (response.value) {
                return window.confirm('Direct Payments exist that overlap with this Spend Plan. ' +
                                'Changing the day of the week upon which the Spend Plan starts ' +
                                'may cause an inconsistency between when budget is allocated and when payments are made.' +
                                '\r\rAre you sure you wish to continue?');
            } else {
                return true;
            }
        } else {
            return true;
        }
    } else {
        return false;
    }
}

addEvent(window, "load", Init);