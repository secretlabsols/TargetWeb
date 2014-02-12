var btnTerminate, btnAutoBalance, navToAutoBalanceScreen, cboEndReason, dteEndReason, qsDpContractID, dpContractSvc, confirmTerminateMsg = 'Are you sure you wish to terminate this payment?', hasAutoBalancePermission;

$(document).ready(function() {
    dpContractSvc = new Target.Abacus.Web.Apps.WebSvc.DPContract_class();
    cboEndReason = $('select[id$=\'cboEndReason_cboDropDownList\']').change(function() {
        resetControlVisibility();
    });
    btnTerminate = $('input[id$=\'btnTerminate\']');
    btnAutoBalance = $('input[id$=\'btnAutoBalance\']');
    dteEndReason = $('input[id$=\'dteTerminateDate_txtTextBox\']').change(function() {
        resetControlVisibility();
        dteTerminateDate_Changed();
    });
    resetControlVisibility();
});

function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    if (!url) {
        window.close();
    }
    else {
        document.location.href = unescape(url);
    }
}

function btnTerminate_Click(AutoBalance) {
    navToAutoBalanceScreen = AutoBalance;
    terminate();
}

function disableForm(disabled) {
    $(':button, :submit, tr, input, select').attr('disabled', disabled);
}

function displayLoadingWithDisabling(display, disabled) {
    DisplayLoading(display);
    disableForm(disabled);
}

function dteTerminateDate_Changed() {
    if (DpContractProjectedTerminationsUpdateManager) {
        DpContractProjectedTerminationsUpdateManager.Update({ dpContractID: qsDpContractID, dpContractEndDate: dteEndReason.val().toDate() });
    }
}

function getSelectedEndDate() {
    return dteEndReason.val().toDate();
}

function getSelectedEndReason() {
    var returnVal = parseInt(cboEndReason.val());
    return (isNaN(returnVal) ? 0 : returnVal);
}

function resetControlVisibility() {
    var disallowTerminate = true, disallowAutoBalance = true,
    disallowTerminateToolTip = 'Please Select a Required End Date and/or an End Reason', 
    disallowAutoBalanceToolTip = disallowTerminateToolTip;
    if (getSelectedEndReason() > 0 && dteEndReason.val() != '') {
        disallowTerminate = false;
        disallowAutoBalance = !hasAutoBalancePermission;
        disallowTerminateToolTip = 'Terminate?';
        disallowAutoBalanceToolTip = (hasAutoBalancePermission ? 'Terminate and Auto Balance this Direct Payment?' : 'You do not have permissions to Auto Balance this Direct Payment');
    }
    btnTerminate.attr({ disabled: disallowTerminate, title: disallowTerminateToolTip });
    btnAutoBalance.attr({ disabled: disallowAutoBalance, title: disallowAutoBalanceToolTip });
}

function terminate() {
    if (window.confirm(confirmTerminateMsg)) {        
        displayLoadingWithDisabling(true, true);
        dpContractSvc.TerminateDpContract(qsDpContractID, getSelectedEndDate(), getSelectedEndReason(), terminateCallBack);
    }
}

function terminateCallBack(serviceResponse) {
    if (CheckAjaxResponse(serviceResponse, dpContractSvc.url)) {
        if (navToAutoBalanceScreen === true && hasAutoBalancePermission) {
            document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Balance.aspx?id=" + qsDpContractID + "&backUrl=" + balance_GetBackUrl();
        } else {
            btnBack_Click();
            return false;
        }
    }
    
    displayLoadingWithDisabling(false, false);
    resetControlVisibility();
}

function balance_GetBackUrl() {
    var url = unescape(GetQSParam(document.location.search, "backUrl"));
    return escape(url);
}





