var chkUnpaidId, chkSuspendedId, chkAuthorisedId, chkPaidId, dteFromId, dteToId;
var chkUnpaid, chkSuspended, chkAuthorised, chkPaid, dteFrom, dteTo;
var pScheduleId;

function Init() {

    chkUnpaid = GetElement(chkUnpaidId)
    chkSuspended = GetElement(chkSuspendedId)
    chkAuthorised = GetElement(chkAuthorisedId)
    chkPaid = GetElement(chkPaidId)
    dteFrom = GetElement(dteFromId + "_txtTextBox")
    dteTo = GetElement(dteToId + "_txtTextBox")
    
    pScheduleId = GetQSParam(document.location.search, "pScheduleId");
}

function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}

function btnApplyfilter_Click() {
    var url = document.location.href;
    url = RemoveQSParam(url, "pScheduleId");
    url = AddQSParam(url, "pScheduleId", pScheduleId);
    url = RemoveQSParam(url, "id");
    url = AddQSParam(url, "id", pScheduleId);
    url = RemoveQSParam(url, "unpaid");
    url = AddQSParam(url, "unpaid", chkUnpaid.checked);
    url = RemoveQSParam(url, "sus");
    url = AddQSParam(url, "sus", chkSuspended.checked);
    url = RemoveQSParam(url, "auth");
    url = AddQSParam(url, "auth", chkAuthorised.checked);
    url = RemoveQSParam(url, "paid");
    url = AddQSParam(url, "paid", chkPaid.checked);
    url = RemoveQSParam(url, "dtfrom");
    url = AddQSParam(url, "dtfrom", dteFrom.value);
    url = RemoveQSParam(url, "dtto");
    url = AddQSParam(url, "dtto", dteTo.value);

    document.location.href = url;
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

addEvent(window, "load", Init);