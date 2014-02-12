
var SelectorWizard_id, SelectorWizard_dateFromID, SelectorWizard_dateToID, DateRangeStep_required;
var DomProformaInvoiceBatchFilterStep_batchTypeIDs, DomProformaInvoiceBatchFilterStep_batchStatusIDs;
var DomProformaInvoiceBatchFilterStep_batchTypePrefix, DomProformaInvoiceBatchFilterStep_batchStatusPrefix;

function DomProformaInvoiceBatchFilterStep_BeforeNavigate() {

	var dateFrom = GetElement(SelectorWizard_dateFromID + "_txtTextBox").value;
	var dateTo = GetElement(SelectorWizard_dateToID + "_txtTextBox").value;
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    var selectedType = DomProformaInvoiceBatchFilterStep_GetSelectedTotal(DomProformaInvoiceBatchFilterStep_batchTypeIDs, DomProformaInvoiceBatchFilterStep_batchTypePrefix);
    var selectedStatus = DomProformaInvoiceBatchFilterStep_GetSelectedTotal(DomProformaInvoiceBatchFilterStep_batchStatusIDs, DomProformaInvoiceBatchFilterStep_batchStatusPrefix);
	
	if(DateRangeStep_required && (dateFrom.length == 0 || dateTo.length == 0)) {
		alert("Please enter a date range.");
		return false;
	}
	if(selectedType == 0) {
	    alert("Please select at least one Batch Type.");
	    return false;
	}
	if(selectedStatus == 0) {
	    alert("Please select at least one Batch Status.");
	    return false;
	}

	url = RemoveQSParam(url, "batchType");
	url = RemoveQSParam(url, "batchStatus");
	url = RemoveQSParam(url, "dateFrom");	
	url = RemoveQSParam(url, "dateTo");	
	url = AddQSParam(url, "batchType", selectedType)
	url = AddQSParam(url, "batchStatus", selectedStatus)
	if(dateFrom.length > 0) url = AddQSParam(url, "dateFrom", dateFrom);
	if(dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);

	SelectorWizard_newUrl = url;
	return true;
}
function DomProformaInvoiceBatchFilterStep_GetSelectedTotal(idArray, idPrefix) {
    var result = 0;
    for(index=0; index<idArray.length; index++) {
        var id = idArray[index] + "_chkCheckbox";
        var chk = GetElement(id);
        if(chk.checked) {
            id = id.replace(SelectorWizard_id, "");
            id = id.replace(idPrefix, "");
            id = id.replace("_chkCheckbox", "");
            id = id.replace("_", "");
            var value = parseInt(id, 10);
            result += value;
        }
    }
    return result;
}