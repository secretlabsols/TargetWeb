
var DateRangeStep_required;

/* DATE RANGE STEP */
function DateRangeStep_BeforeNavigate() {
	var dateFrom = GetElement(SelectorWizard_dateFromID + "_txtTextBox").value;
	var dateTo = GetElement(SelectorWizard_dateToID + "_txtTextBox").value;
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	if(DateRangeStep_required && (dateFrom.length == 0 || dateTo.length == 0)) {
		alert("Please enter a date range.");
		return false;
	}
	
	url = RemoveQSParam(url, "dateFrom");
	url = RemoveQSParam(url, "dateTo");	
	if(dateFrom.length > 0) url = AddQSParam(url, "dateFrom", dateFrom);
	if(dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);
	SelectorWizard_newUrl = url;
	return true;
}
