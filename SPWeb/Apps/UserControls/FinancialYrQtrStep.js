
/* Financial Year Quarter Step */
var FinancialYrQtrStep_required;

function FinancialYrQtrStep_BeforeNavigate() {
	var financialYear = GetElement("ctlFinancialYearQuarter_cboFinancialYear_cboDropDownList").value;
	var quarter = GetElement("ctlFinancialYearQuarter_cboQuarter_cboDropDownList").value;	

	if(FinancialYrQtrStep_required && ((financialYear.length == 0) || (quarter.length == 0))) {
		alert("Please enter a Financial Year and Quarter.");
		return false;
	}
	
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	url = AddQSParam(RemoveQSParam(url, "finYr"), "finYr", financialYear);
	url = AddQSParam(RemoveQSParam(url, "qtr"), "qtr", quarter);
	SelectorWizard_newUrl = url;
	
	return true;
}