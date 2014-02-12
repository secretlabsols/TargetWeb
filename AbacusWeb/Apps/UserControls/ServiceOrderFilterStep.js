var ServiceOrderFilterStep_dateFromID, ServiceOrderFilterStep_dateToID;
var ServiceOrderFilterStep_serviceGroupID;

function ServiceOrderFilterStep_BeforeNavigate() {

    var dateFrom = GetElement(ServiceOrderFilterStep_dateFromID + "_txtTextBox").value;
    var dateTo = GetElement(ServiceOrderFilterStep_dateToID + "_txtTextBox").value;
    var serviceGroup = GetElement(ServiceOrderFilterStep_serviceGroupID + "_cboDropDownList").value;
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    url = RemoveQSParam(url, "dateFrom");
    url = RemoveQSParam(url, "dateTo");;
    url = RemoveQSParam(url, "svcGroupID");
    if (dateFrom.length > 0) url = AddQSParam(url, "dateFrom", dateFrom);
    if (dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);
    url = AddQSParam(url, "svcGroupID", serviceGroup);
    SelectorWizard_newUrl = url;
    return true;
}