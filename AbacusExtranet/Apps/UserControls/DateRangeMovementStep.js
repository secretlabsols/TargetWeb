var DateRangeStep_required;

/* DATE RANGE STEP */
function DateRangeMovementStep_BeforeNavigate() {
	var dateFrom = document.getElementById(SelectorWizard_dateFromID + "_txtTextBox").value;
	var dateTo = document.getElementById(SelectorWizard_dateToID + "_txtTextBox").value;
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	var movementControl_0 = document.getElementById(SelectorWizard_movementID + "_0");
	var movementControl_1 = document.getElementById(SelectorWizard_movementID + "_1");
	var movementControl_2 = document.getElementById(SelectorWizard_movementID + "_2");
	var movementControl_3 = document.getElementById(SelectorWizard_movementID + "_3");
	var movement;
	
	if(DateRangeStep_required && (dateFrom.length == 0 || dateTo.length == 0)) {
		alert("Please enter a date range.");
		return false;
	}
	
	if(movementControl_0.checked == true){
		movement = 0;
	}else if(movementControl_1.checked == true){
		movement = 1;
	}else if(movementControl_2.checked == true){
		movement = 2;
	}else if(movementControl_3.checked == true){
		movement = 3;
	}
	
	url = RemoveQSParam(url, "dateFrom");
	url = RemoveQSParam(url, "dateTo");	
	url = RemoveQSParam(url, "movement");
	if(dateFrom.length > 0) url = AddQSParam(url, "dateFrom", dateFrom);
	if(dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);
	url = AddQSParam(url, "movement", movement);
	SelectorWizard_newUrl = url;
	return true;
}