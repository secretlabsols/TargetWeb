var ServiceDeliveryFileFilter_required;

/* DATE RANGE STEP */
function ServiceDeliveryFileFilterStep_BeforeNavigate() {  
    var SubmittedByID = GetElement(SelectorWizard_SubmittedBy + "_cboDropDownList").value;
	var dateFrom = GetElement(SelectorWizard_dateFromID + "_txtTextBox").value;
	var dateTo = GetElement(SelectorWizard_dateToID + "_txtTextBox").value;
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	var processed = GetElement("SelectorWizard1_chkProcessed_chkCheckbox").checked;
	var workInProgress = GetElement("SelectorWizard1_chkWip_chkCheckbox").checked;
	var awaitingProcessing = GetElement("SelectorWizard1_chkAwaiting_chkCheckbox").checked;
	var deleted = GetElement("SelectorWizard1_chkDeleted_chkCheckbox").checked;
	var failed = GetElement("SelectorWizard1_chkFailed_chkCheckbox").checked;
	
	if(DateRangeStep_required && (dateFrom.length == 0 || dateTo.length == 0)) {
		alert("Please enter a date range.");
		return false;
	}

	url = RemoveQSParam(url, "subBy");
	url = RemoveQSParam(url, "dateFrom");
	url = RemoveQSParam(url, "dateTo");	
	url = RemoveQSParam(url, "del");
	url = RemoveQSParam(url, "proc");
	url = RemoveQSParam(url, "ap");
	url = RemoveQSParam(url, "wip");
	url = RemoveQSParam(url, "failed");
	if (SubmittedByID > 0) url = AddQSParam(url, "subBy", SubmittedByID)
	if(dateFrom.length > 0)url = AddQSParam(url, "dateFrom", dateFrom);
	if(dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);

	url = AddQSParam(url, "del", deleted);
	url = AddQSParam(url, "proc", processed);
	url = AddQSParam(url, "ap", awaitingProcessing);
	url = AddQSParam(url, "wip", workInProgress);
	url = AddQSParam(url, "failed", failed);
	SelectorWizard_newUrl = url;
	return true;
}