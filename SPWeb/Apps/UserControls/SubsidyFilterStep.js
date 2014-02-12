/* SubsidyFilterStep */

var SubsidyFilterStep_required;

function SubsidyFilterStep_BeforeNavigate() {
	var chkActive = GetElement("SelectorWizard1_chkActive_chkCheckbox");
	var chkCancelled = GetElement("SelectorWizard1_chkCancelled_chkCheckbox");
	var chkProvisional = GetElement("SelectorWizard1_chkProvisional_chkCheckbox");
	var chkDocumentary = GetElement("SelectorWizard1_chkDocumentary_chkCheckbox");
	var chkSuspended = GetElement("SelectorWizard1_chkSuspended_chkCheckbox");
	var url;
	var StatusValue = 0;
	
	if (!DateRangeStep_BeforeNavigate()) {
		return false;
	}
	url = SelectorWizard_newUrl;
	
	if (chkActive.checked)
	{
		StatusValue = StatusValue + 1;
	}
	if (chkCancelled.checked)
	{
		StatusValue = StatusValue + 2;
	}
	if (chkProvisional.checked)
	{
		StatusValue = StatusValue + 4;
	}
	if (chkDocumentary.checked)
	{
		StatusValue = StatusValue + 8;
	}
	if (chkSuspended.checked)
	{
		StatusValue = StatusValue + 16;
	}
	
	if(SubsidyFilterStep_required && (StatusValue == 0)) {
		alert("Please enter at least one status to search on.");
		return false;
	}
	url = AddQSParam(RemoveQSParam(url, "Status"), "Status", StatusValue);
	SelectorWizard_newUrl = url;
	return true;
}