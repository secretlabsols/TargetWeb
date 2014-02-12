
/* Financial Year Quarter PI Status Step */
function FinancialYrQtrPIStatusStep_BeforeNavigate() {
	var financialYear = GetElement("ctlFinancialYearQuarter_cboFinancialYear_cboDropDownList").value;
	var quarter = GetElement("ctlFinancialYearQuarter_cboQuarter_cboDropDownList").value;	
	var status = GetElement("SelectorWizard1_cboStatus_cboDropDownList").value;	
 	
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	url = AddQSParam(RemoveQSParam(url, "finYr"), "finYr", financialYear);
	url = AddQSParam(RemoveQSParam(url, "qtr"), "qtr", quarter);
	url = AddQSParam(RemoveQSParam(url, "status"), "status", status);
	SelectorWizard_newUrl = url;
	
	return true;
}