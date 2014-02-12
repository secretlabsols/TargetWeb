
var DomContractReOpenWeeksFilterStep_weDateFromID, DomContractReOpenWeeksFilterStep_weDateToID;
var DomContractReOpenWeeksFilterStep_closureDateFromID, DomContractReOpenWeeksFilterStep_closureDateToID;

function DomContractReOpenWeeksFilterStep_BeforeNavigate() {
	var weDateFrom = GetElement(DomContractReOpenWeeksFilterStep_weDateFromID + "_txtTextBox").value;
	var weDateTo = GetElement(DomContractReOpenWeeksFilterStep_weDateToID + "_txtTextBox").value;
	var closureDateFrom = GetElement(DomContractReOpenWeeksFilterStep_closureDateFromID + "_txtTextBox").value;
	var closureDateTo = GetElement(DomContractReOpenWeeksFilterStep_closureDateToID + "_txtTextBox").value;
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	url = RemoveQSParam(url, "weDateFrom");
	url = RemoveQSParam(url, "weDateTo");
	url = RemoveQSParam(url, "closureDateFrom");
	url = RemoveQSParam(url, "closureDateTo");
	if(weDateFrom.length > 0) url = AddQSParam(url, "weDateFrom", weDateFrom);
	if(weDateTo.length > 0) url = AddQSParam(url, "weDateTo", weDateTo);
	if(closureDateFrom.length > 0) url = AddQSParam(url, "closureDateFrom", closureDateFrom);
	if(closureDateTo.length > 0) url = AddQSParam(url, "closureDateTo", closureDateTo);
	SelectorWizard_newUrl = url;
	return true;
}