
var contractSvc;
var invoiceID, contractPeriodID, systemAccountID, addID, invoiceTotalID, selectedPeriodID, selectedSystemAccountID, weekendingID;
var cboPeriod, cboSysAcc, btnAdd, lblInvoiceTotal, lblLineValueTotal, tblDetails;
var providerID, contractID, dateFrom, dateTo, dteWeekending, lblLineValueTotalID;
var manualPayment_domContractID, manualPayment_providerID, manualPayment_mode;
var btnAddClicked;

var CELL_INDEX_RATE_CATEGORY = 0;
var CELL_INDEX_UNITS = 2;
var CELL_INDEX_UNIT_COST = 3;
var CELL_INDEX_LINE_VALUE = 4;
var CELL_INDEX_FINANCE_CODE = 5;

function Init() {
    contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
    cboPeriod = GetElement(contractPeriodID + "_cboDropDownList", true);
    cboSysAcc = GetElement(systemAccountID + "_cboDropDownList", true);
    dteWeekending = GetElement(weekendingID + "_txtTextBox");
        
    btnAdd = GetElement(addID);
    lblInvoiceTotal = GetElement(invoiceTotalID + "_lblReadOnlyContent");
    lblLineValueTotal = GetElement(lblLineValueTotalID + "_lblReadOnlyContent");
    tblDetails = GetElement("tblDetails");
    
    if(providerID > 0) {
        InPlaceDomContractSelector_Enabled(manualPayment_domContractID, true);
        InPlaceDomContractSelector_providerID = providerID;
    }
    
    if(manualPayment_mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew) {
        InPlaceDomContractSelector_Enabled(manualPayment_domContractID, (contractID != 0));
        cboPeriod.disabled = (selectedPeriodID == 0);
        cboSysAcc.disabled = (selectedSystemAccountID == 0);
    } else if (manualPayment_mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
        InPlaceEstablishmentSelector_Enabled(manualPayment_providerID, false);
        InPlaceDomContractSelector_Enabled(manualPayment_domContractID, false);
        cboPeriod.disabled = true;
    } else {
        InPlaceDomContractSelector_Enabled(manualPayment_domContractID, false);
        cboPeriod.disabled = true;
        cboSysAcc.disabled = true;
    }
    
    if(manualPayment_mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew) {
        if (providerID != undefined && providerID != 0) {
            InPlaceDomContractSelector_Enabled(manualPayment_domContractID, true);
            if (contractID != undefined && contractID != 0) {
                cboPeriod.disabled = false;
            }
        }
    }

    if (invoiceID == 0 && btnAddClicked == false) cboPeriod_OnChange();
    if(invoiceID != 0) CalculateInvoiceTotal();
}

function FetchSystemAccounts() {
	DisplayLoading(true);
	contractSvc.FetchSystemAccountListByPeriod(providerID, contractID, cboPeriod.value, FetchSystemAccounts_Callback)
}
function FetchSystemAccounts_Callback(response) {
    var sysAccs, opt;
    if(CheckAjaxResponse(response, contractSvc.url)) {
        sysAccs = response.value.List;
    
		// clear
	    cboSysAcc.options.length = 0;
	    // add blank		
	    opt = document.createElement("OPTION");
	    cboSysAcc.options.add(opt);
	    SetInnerText(opt, "");
	    opt.value = "";
		
		for(index=0; index<sysAccs.length; index++) {
		    opt = document.createElement("OPTION");
		    cboSysAcc.options.add(opt);
		    SetInnerText(opt, sysAccs[index].Text);
		    opt.value = sysAccs[index].Value;
		}
		
		// select existing value
	    cboSysAcc.value = selectedSystemAccountID;
	    
        if(selectedPeriodID > 0) {
            cboSysAcc.disabled = false;
        } else {
            cboSysAcc.disabled = true;
        }
	    
	}
	DisplayLoading(false);
}

function FetchPeriods() {
	DisplayLoading(true);
	//contractSvc.FetchSystemAccountList(selectedPeriodID, FetchPeriods_Callback)
	contractSvc.FetchContractPeriodList(contractID, FetchPeriods_Callback)
}
function FetchPeriods_Callback(response) {
    var periods, opt;
    if(CheckAjaxResponse(response, contractSvc.url)) {
        periods = response.value.List;
    
		// clear
	    cboPeriod.options.length = 0;
	    // add blank		
	    opt = document.createElement("OPTION");
	    cboPeriod.options.add(opt);
	    SetInnerText(opt, "");
	    opt.value = "";
		
		for(index=0; index<periods.length; index++) {
		    opt = document.createElement("OPTION");
		    cboPeriod.options.add(opt);
		    SetInnerText(opt, periods[index].Text);
		    opt.value = periods[index].Value;
		}
		
		// select existing value
		if (selectedPeriodID != 0) {
	        cboPeriod.value = selectedPeriodID;
	        cboSysAcc.disabled = false;
	        FetchSystemAccounts();
	    }
	    
	}
	DisplayLoading(false);
}


function InPlaceEstablishment_Changed(newID) {
	var enabled = false;
	if(newID.trim().length > 0) enabled = true;
	providerID = newID;
	InPlaceDomContractSelector_ClearStoredID(manualPayment_domContractID);
    if(enabled) {
	    InPlaceDomContractSelector_Enabled(manualPayment_domContractID, true);
    } else {
	    InPlaceDomContractSelector_Enabled(manualPayment_domContractID, false);
    }
    InPlaceDomContractSelector_providerID = newID;
}

function InPlaceDomContract_Changed(newID) {
	var enabled = false;
	if(newID.trim().length > 0) enabled = true;
	contractID = newID
	cboPeriod.options.length = 0
	cboSysAcc.options.length = 0
    if(enabled) {
	    cboPeriod.disabled = false;
	    cboSysAcc.disabled = true;
	    FetchPeriods();
    } else {
	    cboPeriod.disabled = true;
	    cboSysAcc.disabled = true;		
    }
	InPlaceDomContractSelector_providerID = newID;
}

function FetchUnitCost(contractPeriodID, rateCategoryID, rowID) {
    DisplayLoading(true);
	contractSvc.GetUnitCost(contractPeriodID, rateCategoryID, rowID, FetchUnitCost_Callback)
}
function FetchUnitCost_Callback(response) {
    var txtUnitCost;
    if(CheckAjaxResponse(response, contractSvc.url)) {
        txtUnitCost = GetUnitCostField(response.value.RowID);
        txtUnitCost.value = response.value.UnitCost.toString().formatCurrency(true);
        CalculateLineValue(response.value.RowID);
	}
	DisplayLoading(false);
}

function cboPeriod_OnChange() {
    var response, weekendingDate;
    selectedPeriodID = (cboPeriod.value.length == 0) ? 0 : parseInt(cboPeriod.value, 10);
    if(manualPayment_mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew || manualPayment_mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
        btnAdd.disabled = (selectedPeriodID == 0);
    }
    if (selectedPeriodID != 0) {
        FetchSystemAccounts();
        response = contractSvc.GetManualInvoiceWeekendingDate(selectedPeriodID, weekendingDate).value;
        if(response.ErrMsg.Success == false) {
            alert(response.ErrMsg.Message);
            return;
        } else {
            dteWeekending.value = response.Value;
        }
    } else {
        dteWeekending.value = "";
    }
    // refresh unit cost and line value on each detail line
    for(index=0; index<tblDetails.tBodies[0].rows.length; index++) {
        if(tblDetails.tBodies[0].rows[index].id.length > 0) {
            cboRateCategory_OnChange(tblDetails.tBodies[0].rows[index].id);
        }
    }
    
}
function cboRateCategory_OnChange(rowID) {
    // refresh unit cost on the current detail line
    // (the line value is refreshed by the FetchUnitCost() callback function
    var cboRateCategory = GetRateCategory(rowID);
    var rateCategoryID = (cboRateCategory.value.length == 0) ? 0 : parseInt(cboRateCategory.value, 10);
    FetchUnitCost(selectedPeriodID, rateCategoryID, rowID);
}
function txtUnits_Changed(id) {
    var txtUnits = GetElement(id + "_txtTextBox");
    var rowID = txtUnits.parentNode.parentNode.id;
   
    // refresh line value
    CalculateLineValue(rowID);
        
    // if units <> zero then we need a finance code
    var valFC = GetFinanceCodeValidator(rowID);
    var units = (txtUnits.value.trim().length == 0) ? 0 : parseInt(txtUnits.value, 10);
    ValidatorEnable(valFC, (units != 0));
}

function CalculateLineValue(rowID) {
    var txtUnits = GetUnitsField(rowID); var lineUnits;
    var txtUnitCost = GetUnitCostField(rowID); var lineUnitCost;
    var txtLineValue = GetLineValueField(rowID); var lineValue;

    lineUnits = txtUnits.value.replace(",", "");
    lineUnitCost = txtUnitCost.value.replace(",", "");
    lineValue = (lineUnits * lineUnitCost).toString().formatCurrency(true);
    txtLineValue.value = lineValue.replace(",", "");
    CalculateInvoiceTotal();
}
function CalculateInvoiceTotal() {
    var total = 0;  var lineValue = "";
    ToggleDetailLineFields(false);
    for(index=0; index<tblDetails.tBodies[0].rows.length -1; index++) {
        if (GetLineValueField(tblDetails.tBodies[0].rows[index].id).value != "") {        
            lineValue = GetLineValueField(tblDetails.tBodies[0].rows[index].id).value;
            total += parseFloat(lineValue);
        }
    }
    lblInvoiceTotal.innerHTML = total.toString().formatCurrency();
    lblLineValueTotal.innerHTML = total.toString().formatCurrency(); 
}
function GetRow(rowID) {
    return GetElement(rowID);
}
function GetRateCategory(rowID) {
    var row = GetRow(rowID);
    return row.cells[CELL_INDEX_RATE_CATEGORY].getElementsByTagName("SELECT")[0];
}
function GetUnitsField(rowID) {
    var row = GetRow(rowID);
    return row.cells[CELL_INDEX_UNITS].getElementsByTagName("INPUT")[0];
}
function GetUnitCostField(rowID) {
    var row = GetRow(rowID);
    return row.cells[CELL_INDEX_UNIT_COST].getElementsByTagName("INPUT")[0];
}
function GetLineValueField(rowID) {
    var row = GetRow(rowID);
    return row.cells[CELL_INDEX_LINE_VALUE].getElementsByTagName("INPUT")[0];
}
function GetFinanceCode(rowID) {
    var row = GetRow(rowID);
    return row.cells[CELL_INDEX_FINANCE_CODE].getElementsByTagName("INPUT")[0];
}
function GetFinanceCodeValidator(rowID) {
    var fc = GetFinanceCode(rowID);
    return GetElement(fc.id.replace("_txtName", "_valRequired"));
}

function btnBack_Click() {
    var url = unescape(GetQSParam(document.location.search, "backUrl"));
    document.location.href = url;
}

function ToggleDetailLineFields(enable) {
    for(index=0; index<tblDetails.tBodies[0].rows.length -1; index++) {
        GetLineValueField(tblDetails.tBodies[0].rows[index].id).disabled = !enable;
        GetUnitCostField(tblDetails.tBodies[0].rows[index].id).disabled = !enable;
    }
}

function btnAdd_Click() {
    ToggleDetailLineFields(true);
}

addEvent(window, "load", Init);
