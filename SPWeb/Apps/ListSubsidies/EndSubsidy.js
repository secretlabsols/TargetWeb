
function btnEndSubsidy_Click()
{
	var endDate, endReason;
	
	endDate = GetElement("txtEndDate_txtTextBox");
	endReason = GetElement("cboEndReason");

	//Validate User Input
	if (endDate.value == "")
	{
		alert("Please enter an end date for the subsidy.");
		return;
	} 
	if (endReason.value == 0)
	{
		alert("Please enter a reason for ending the subsidy.");
		return;
	}	
	answer = confirm("Are you sure you wish to end this subsidy?");
	if(answer) {
		document.forms[0].submit();
	}
}