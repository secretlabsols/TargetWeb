
var sdsSvc, mode, originalBudget;
var txtOverriddenPointScore_ClientID, cboOverriddenPointScoreReason_ClientID;
var txtOverriddenBudget_ClientID, cboOverriddenBudgetReason_ClientID;
var chkOverridePointScore, txtOverriddenPointScore, cboOverriddenPointScoreReason, valOverriddenPointScore, valOverriddenPointScoreReason; 
var chkOverrideBudget, txtOverriddenBudget, cboOverriddenBudgetReason, valOverriddenBudget, valOverriddenBudgetReason;
var cboRASType, dteRASDate, txtPointScore, txtBudget, spnBudgetRecalcWarning;

function Init() {
    sdsSvc = new Target.Abacus.Web.Apps.WebSvc.Sds_class();
    
    chkOverridePointScore = GetElement("chkOverridePointScore");
    txtOverriddenPointScore = GetElement(txtOverriddenPointScore_ClientID + "_txtTextBox");
    cboOverriddenPointScoreReason = GetElement(cboOverriddenPointScoreReason_ClientID + "_cboDropDownList");
    valOverriddenPointScore = GetElement(txtOverriddenPointScore_ClientID + "_valRequired");
    valOverriddenPointScoreReason = GetElement(cboOverriddenPointScoreReason_ClientID + "_valRequired");
    
    chkOverrideBudget = GetElement("chkOverrideBudget");
    txtOverriddenBudget = GetElement(txtOverriddenBudget_ClientID + "_txtTextBox");
    cboOverriddenBudgetReason = GetElement(cboOverriddenBudgetReason_ClientID + "_cboDropDownList");
    valOverriddenBudget = GetElement(txtOverriddenBudget_ClientID + "_valRequired");
    valOverriddenBudgetReason = GetElement(cboOverriddenBudgetReason_ClientID + "_valRequired");
    
    cboRASType = GetElement("cboRasType_cboDropDownList");
    dteRASDate = GetElement("dteRasDate_txtTextBox");
    txtPointScore = GetElement("txtPointScore_txtTextBox");
    txtBudget = GetElement("txtBudget_txtTextBox");
    spnBudgetRecalcWarning = GetElement("spnBudgetRecalcWarning");

    originalBudget = parseFloat(txtBudget.value);
    if(isNaN(originalBudget)) originalBudget = 0.00;
        
    chkOverridePointScore_Click();
    chkOverrideBudget_Click();
}

function chkOverridePointScore_Click() {
    var disable = !chkOverridePointScore.checked;
        
    if (mode == Target.Library.Web.UserControls.StdButtonsMode.Edit || mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew)
        txtOverriddenPointScore.disabled = disable;
    else
        txtOverriddenPointScore.disabled = true;
    if (disable) txtOverriddenPointScore.value = "";
    ValidatorEnable(valOverriddenPointScore, !disable);
    
    if (mode == Target.Library.Web.UserControls.StdButtonsMode.Edit || mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew)
        cboOverriddenPointScoreReason.disabled = disable;
    else
        cboOverriddenPointScoreReason.disabled = true;
    if (disable) cboOverriddenPointScoreReason.value = "";
    ValidatorEnable(valOverriddenPointScoreReason, !disable);
    
    CalculateBudget();
}

function chkOverrideBudget_Click() {
    var disable = !chkOverrideBudget.checked;
    
    if (mode == Target.Library.Web.UserControls.StdButtonsMode.Edit || mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew)
        txtOverriddenBudget.disabled = disable;
    else
        txtOverriddenBudget.disabled = true;
    if (disable) txtOverriddenBudget.value = "";
    ValidatorEnable(valOverriddenBudget, !disable);
    
    if (mode == Target.Library.Web.UserControls.StdButtonsMode.Edit || mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew)
        cboOverriddenBudgetReason.disabled = disable;
    else
        cboOverriddenBudgetReason.disabled = true;
    if (disable) cboOverriddenBudgetReason.value = "";
    ValidatorEnable(valOverriddenBudgetReason, !disable);
}

function dteRasDate_Changed(id) {
    CalculateBudget();
}
function txtPointScore_Changed(id) {
    CalculateBudget();
}
function txtOverriddenPointScore_Changed(id) {
    CalculateBudget();
}

function CalculateBudget() {
    var rasTypeID = parseInt(cboRASType.value, 10);
    var rasDate = IsDate(dteRASDate.value) ? dteRASDate.value.toDate() : new Date(0,0,0);
    var pointScore = 0;
    
    if(chkOverridePointScore.checked)
        pointScore = parseInt(txtOverriddenPointScore.value, 10);
    else
        pointScore = parseInt(txtPointScore.value, 10);
    
    DisplayLoading(true);
	sdsSvc.CalculatePersonalBudget(rasTypeID, rasDate, pointScore, CalculatePersonalBudget_Callback)
}

function CalculatePersonalBudget_Callback(response) {
	if(CheckAjaxResponse(response, sdsSvc.url)) {
        txtBudget.value = response.value.Value.toFixed(2);
        if (mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
            if(originalBudget != parseFloat(txtBudget.value)) {
                spnBudgetRecalcWarning.innerHTML = "WARNING: This personal budget now calculates to a different value. " +
                                                    "The original calculated value was £" + originalBudget + ".<br />";
            } else {
                spnBudgetRecalcWarning.innerHTML = "";
            }
        }
	}
	DisplayLoading(false);
}

addEvent(window, "load", Init);