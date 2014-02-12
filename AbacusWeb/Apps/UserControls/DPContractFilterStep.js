var DPContractFilterStep_dateFromID, DPContractFilterStep_dateToID;
var DPContractFilterStep_SdsRadioBtnBoth, DPContractFilterStep_SdsRadioBtnSds, DPContractFilterStep_SdsRadioBtnNonSds;

function DPContractFilterStep_BeforeNavigate() {
    var dateFrom = GetElement(DPContractFilterStep_dateFromID + "_txtTextBox").value;
    var dateTo = GetElement(DPContractFilterStep_dateToID + "_txtTextBox").value;
    var IsSDS = GetRDBValue();
    if (IsSDS == null) {
        IsSDS = -2;
    }
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    url = RemoveQSParam(url, "dateFrom");
    url = RemoveQSParam(url, "dateTo");
    url = RemoveQSParam(url, "IsSDS");

    if (dateFrom.length > 0) url = AddQSParam(url, "dateFrom", dateFrom);
    if (dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);
    url = AddQSParam(url, "IsSDS", IsSDS);
    SelectorWizard_newUrl = url;
    return true;
}

function GetRDBValue() {
    var radio = GetElement(DPContractFilterStep_SdsRadioBtnBoth);
    if (radio.checked) {
        return -2;
    }
    radio = GetElement(DPContractFilterStep_SdsRadioBtnSds);
    if (radio.checked) {    
        return -1;
    }
    radio = GetElement(DPContractFilterStep_SdsRadioBtnNonSds);
    if (radio.checked) {
        return 0;
    }
}