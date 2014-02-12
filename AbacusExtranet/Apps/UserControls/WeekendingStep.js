var SelectorWizard_weekendingID, contractSvc, stepRequired; 

function WeekendingStep_BeforeNavigate() {
	var weeekendingDate = GetElement(SelectorWizard_weekendingID + "_txtTextBox").value;
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
	
	
	// validate weekending date if one is entered
	if (weeekendingDate.length != 0){
	    var response = contractSvc.WeekendingDateValid(weeekendingDate.toDate()).value;
	    if(response.Success == false) {
	        alert(response.Message);
	        return false;
        }
	}
	
	// weekending date is required?
	if(stepRequired && weeekendingDate.length == 0) {
		alert("Please select a week ending date.");
		return false;
	}

	url = AddQSParam(RemoveQSParam(url, "weekending"), "weekending", weeekendingDate);
	SelectorWizard_newUrl = url;
	return true;
}
