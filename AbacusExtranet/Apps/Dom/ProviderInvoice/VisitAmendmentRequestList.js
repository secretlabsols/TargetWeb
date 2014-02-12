var ddlRequestById, rdbCouncilId, rdbProviderId, rdbBothId, pScheduleId;
var ddlRequestById, rdbCouncil, rdbProvider, rdbBoth;


function Init() {

    pScheduleId = GetQSParam(document.location.search, "pScheduleId");
 }

 function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}

function SelectorWizard_CustomBack() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

addEvent(window, "load", Init);