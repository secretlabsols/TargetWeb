var qsDpContractID, dpContractSvc;

$(document).ready(function() {
    dpContractSvc = new Target.Abacus.Web.Apps.WebSvc.DPContract_class();
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

function btnReinstate_Click() {
    reinstate();
}

function disableForm(disabled) {
    $(':button, :submit, tr, input, select').attr('disabled', disabled);
}

function displayLoadingWithDisabling(display, disabled) {
    DisplayLoading(display);
    disableForm(disabled);
}

function reinstate() {
    if (window.confirm('Are you sure you wish to re-instate this payment?')) {
        displayLoadingWithDisabling(true, true);
        dpContractSvc.ReInstateDpContract(qsDpContractID, reinstateCallBack);
    }
}

function reinstateCallBack(serviceResponse) {
    if (CheckAjaxResponse(serviceResponse, dpContractSvc.url)) {
        btnBack_Click();
        return false;
    }
    displayLoadingWithDisabling(false, false);
}