var estabid, contractId, clientId, invoiceid, mode, pScheduleId, pSWE;

function ClientSelector_SelectedItemChange(args) {
    clientId = args[0];
    var btnView = GetElement("btnNext");
    btnView.disabled = false;
}

function btnNext_Click() {
    document.location.href = SITE_VIRTUAL_ROOT +
    "AbacusExtranet/Apps/Dom/ProformaInvoice/ManualEnterInvoice.aspx?mode=2&estabid=" + estabid +
    "&contractid=" + contractId +
    "&pScheduleId=" + pScheduleId +
    "&pSWE=" + pSWE +
    "&clientid=" + clientId + 
    "&backUrl=" + GetBackUrl();
}

function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

