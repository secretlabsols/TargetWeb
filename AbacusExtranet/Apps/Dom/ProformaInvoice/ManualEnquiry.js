

function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function SelectorWizard_CustomBack() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}