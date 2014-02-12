
var SelectorWizard_id, SelectorWizard_dateFromID, SelectorWizard_dateToID;
var SelectorWizard_weFromID, SelectorWizard_weToID, SelectorWizard_invoiceNumberID, DateRangeStep_required;
var SelectorWizard_unPaidID, SelectorWizard_paidID, SelectorWizard_authorisedID, SelectorWizard_suspendedID;
var DomProviderInvoiceFilterStep_invoiceStatusIDs;
var DomProviderInvoiceFilterStep_invoiceStatusPrefix;
var SelectorWizard_HideRetractedAndRetractionInvoicesID;

function DomProviderInvoiceFilterStep_BeforeNavigate() {

	var dateFrom = GetElement(SelectorWizard_dateFromID + "_txtTextBox").value;
	var dateTo = GetElement(SelectorWizard_dateToID + "_txtTextBox").value;
	var weFrom = GetElement(SelectorWizard_weFromID + "_txtTextBox").value;
	var weTo = GetElement(SelectorWizard_weToID + "_txtTextBox").value;
	var invoiceNumber = GetElement(SelectorWizard_invoiceNumberID + "_txtTextBox").value;
	var unPaid = GetElement(SelectorWizard_unPaidID + "_chkCheckbox");
	var paid = GetElement(SelectorWizard_paidID + "_chkCheckbox");
	var authorised = GetElement(SelectorWizard_authorisedID + "_chkCheckbox");
	var suspended = GetElement(SelectorWizard_suspendedID + "_chkCheckbox");
	var retraction = GetElement(SelectorWizard_HideRetractedAndRetractionInvoicesID);
	
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

   
	if(DateRangeStep_required && (dateFrom.length == 0 || dateTo.length == 0)) {
		alert("Please enter a date range.");
		return false;
	}

	url = RemoveQSParam(url, "dateFrom");	
	url = RemoveQSParam(url, "dateTo");	
	url = RemoveQSParam(url, "weTo");	
	url = RemoveQSParam(url, "weFrom");
	url = RemoveQSParam(url, "invoiceNumber");	
	url = RemoveQSParam(url, "unPaid");
	url = RemoveQSParam(url, "paid");
	url = RemoveQSParam(url, "authorised");
	url = RemoveQSParam(url, "suspended");
	url = RemoveQSParam(url, "retraction");
	
	url = AddQSParam(url, "invoiceNumber", invoiceNumber)
	if(dateFrom.length > 0) url = AddQSParam(url, "dateFrom", dateFrom);
	if(dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);
	if(weFrom.length > 0) url = AddQSParam(url, "weFrom", weFrom);
	if(weTo.length > 0) url = AddQSParam(url, "weTo", weTo);
	url = AddQSParam(url, "unPaid", unPaid.checked);
	url = AddQSParam(url, "paid", paid.checked);
	url = AddQSParam(url, "authorised", authorised.checked);
	url = AddQSParam(url, "suspended", suspended.checked);
	url = AddQSParam(url, "retraction", retraction.checked);

	SelectorWizard_newUrl = url;
	return true;
}

function ShowHelp() {
    var d = new Apps.Dom.ProformaInvoice.Comment.HelpDialog("Invoice Number Help", "To filter these records by <strong>" + "Invoice Number" + "</strong>, enter filter criteria in the Invoice Number text box.<br />For example:" +
						"<ul style=\"margin:1em;\">" +
							"<li>To only show invoice(s) that <strong>start</strong> with ABC, enter <strong>ABC*</strong></li>" + 
							"<li>To only show invoice(s) that <strong>end</strong> with ABC, enter <strong>*ABC</strong></li>" + 
							"<li>To only show invoice(s) that <strong>contain</strong> ABC, enter <strong>*ABC*</strong></li>" + 
						"</ul><br /> ");
    d.SetCallback(ShowHelp_Callback)
    d.SetType(1);
    d.Show();
}
function ShowHelp_Callback(evt, args) {
    var d = args[0];
    d.Hide();
}