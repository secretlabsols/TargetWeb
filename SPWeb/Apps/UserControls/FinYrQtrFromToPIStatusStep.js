
/* Financial Year Quarter From/To PI Status Step */
function FinYrQtrFromToPIStatusStep_BeforeNavigate() {
	var financialYearFrom = GetElement("ctlFinancialYearQuarterFrom_cboFinancialYear_cboDropDownList").value;
	var quarterFrom = GetElement("ctlFinancialYearQuarterFrom_cboQuarter_cboDropDownList").value;	
	var financialYearTo = GetElement("ctlFinancialYearQuarterTo_cboFinancialYear_cboDropDownList").value;
	var quarterTo = GetElement("ctlFinancialYearQuarterTo_cboQuarter_cboDropDownList").value;	
	var status = GetElement("SelectorWizard1_cboStatus_cboDropDownList").value;	
 	
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	if (financialYearTo < financialYearFrom){
		window.alert("Period To must not be before Period From.");
		return false;
	}
	if (financialYearFrom == financialYearTo){
		if (quarterTo < quarterFrom){
			window.alert("Period To must not be before Period From.");
			return false;
		}
	}
	
	url = AddQSParam(RemoveQSParam(url, "finYrFrom"), "finYrFrom", financialYearFrom);
	url = AddQSParam(RemoveQSParam(url, "qtrFrom"), "qtrFrom", quarterFrom);
	url = AddQSParam(RemoveQSParam(url, "finYrTo"), "finYrTo", financialYearTo);
	url = AddQSParam(RemoveQSParam(url, "qtrTo"), "qtrTo", quarterTo);
	url = AddQSParam(RemoveQSParam(url, "status"), "status", status);
	SelectorWizard_newUrl = url;
	
	return true;
}