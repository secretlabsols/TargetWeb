
var SelectorWizard_id, SelectorWizard_dateFromID, SelectorWizard_dateToID;
var SelectorWizard_weFromID, SelectorWizard_weToID, SelectorWizard_invoiceNumberID, SelectorWizard_invoiceRefID, DateRangeStep_required;
var SelectorWizard_unPaidID, SelectorWizard_paidID, SelectorWizard_authorisedID, SelectorWizard_suspendedID;
var SelectorWizard_optExcAllID, SelectorWizard_optExcExcludedOnlyID, SelectorWizard_optExcNonExcludedOnlyID;
var DomProviderInvoiceFilterStep_invoiceStatusIDs, SelectorWizard_optNoAdditionalFilteringControlID, SelectorWizard_optUplannedServiceControlID;
var DomProviderInvoiceFilterStep_invoiceStatusPrefix, SelectorWizard_optCostExceededControlID;
var SelectorWizard_optCostExceededWithinToleranceControlID, SelectorWizard_optUnitsExceededControlID;
var SelectorWizard_optUnitsExceededWithinCostControlID, SelectorWizard_optManuallySuspendedControlID;
var noAdditionalFiltering, unplannedService, costExceeded, costExceededWithinTolerance, unitsExceeded,
unitsExceededWithinCost, manuallySuspended;
//constants to store enum values for AdditionalFilterOptions
var CONST_NO_ADDITIONAL_FILTERING, CONST_UNPLANNED_SERVICE, CONST_COST_EXCEEDED, CONST_COST_EXCEEDED_WITHIN_TOLERANCE,
CONST_UNITS_EXCEEDED, CONST_UNITS_EXCEEDED_WITHIN_COST, CONST_MANUALLY_SUSPENDED;


//set the contstants to match AdditionalFilterOptions enum in DomProviderInvoiceFilterStep.vb
CONST_NO_ADDITIONAL_FILTERING = 0;
CONST_UNPLANNED_SERVICE = 1;
CONST_COST_EXCEEDED = 2;
CONST_COST_EXCEEDED_WITHIN_TOLERANCE = 3;
CONST_UNITS_EXCEEDED = 4;
CONST_UNITS_EXCEEDED_WITHIN_COST = 5;
CONST_MANUALLY_SUSPENDED = 6;

function DomProviderInvoiceFilterStep_BeforeNavigate() {

	var dateFrom = GetElement(SelectorWizard_dateFromID + "_txtTextBox").value;
	var dateTo = GetElement(SelectorWizard_dateToID + "_txtTextBox").value;
	var weFrom = GetElement(SelectorWizard_weFromID + "_txtTextBox").value;
	var weTo = GetElement(SelectorWizard_weToID + "_txtTextBox").value;
	var invoiceNumber = GetElement(SelectorWizard_invoiceNumberID + "_txtTextBox").value;
	var invoiceRef = GetElement(SelectorWizard_invoiceRefID + "_txtTextBox").value;
	var unPaid = GetElement(SelectorWizard_unPaidID + "_chkCheckbox");
	var paid = GetElement(SelectorWizard_paidID + "_chkCheckbox");
	var authorised = GetElement(SelectorWizard_authorisedID + "_chkCheckbox");
	var suspended = GetElement(SelectorWizard_suspendedID + "_chkCheckbox");
	var excludedAll = GetElement(SelectorWizard_optExcAllID);
	var excludedNonExcludedOnly = GetElement(SelectorWizard_optExcNonExcludedOnlyID);
	var excludedExcludedOnly = GetElement(SelectorWizard_optExcExcludedOnlyID);
	
	noAdditionalFiltering = GetElement(SelectorWizard_optNoAdditionalFilteringControlID);
	unplannedService = GetElement(SelectorWizard_optUplannedServiceControlID);
	costExceeded = GetElement(SelectorWizard_optCostExceededControlID);
	costExceededWithinTolerance = GetElement(SelectorWizard_optCostExceededWithinToleranceControlID);
	unitsExceeded = GetElement(SelectorWizard_optUnitsExceededControlID);
	unitsExceededWithinCost = GetElement(SelectorWizard_optUnitsExceededWithinCostControlID);
	manuallySuspended = GetElement(SelectorWizard_optManuallySuspendedControlID);
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
	url = RemoveQSParam(url, "invoiceRef");	
	url = RemoveQSParam(url, "unPaid");
	url = RemoveQSParam(url, "paid");
	url = RemoveQSParam(url, "authorised");
	url = RemoveQSParam(url, "suspended");
	url = RemoveQSParam(url, "exclude");
	url = RemoveQSParam(url, "additionalfilter");
	
	url = AddQSParam(url, "invoiceNumber", invoiceNumber)
	url = AddQSParam(url, "invoiceRef", invoiceRef)
	if(dateFrom.length > 0) url = AddQSParam(url, "dateFrom", dateFrom);
	if(dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);
	if(weFrom.length > 0) url = AddQSParam(url, "weFrom", weFrom);
	if(weTo.length > 0) url = AddQSParam(url, "weTo", weTo);
	url = AddQSParam(url, "unPaid", unPaid.checked);
	url = AddQSParam(url, "paid", paid.checked);
	url = AddQSParam(url, "authorised", authorised.checked);
	url = AddQSParam(url, "suspended", suspended.checked);
	url = AddQSParam(url, "additionalfilter", getSelectedAdditionalFilter());
	if (excludedExcludedOnly.checked) {
	    url = AddQSParam(url, "exclude", "true");
	}
	else if (excludedNonExcludedOnly.checked) {
	    url = AddQSParam(url, "exclude", "false");
	}
	else {
	    url = AddQSParam(url, "exclude", "null");
	}
	
	SelectorWizard_newUrl = url;
	return true;
}

function ShowHelp(filterName) {
    var d = new Target.Abacus.Web.Apps.Dom.ProviderInvoice.HelpDialog(filterName + " Help", "To filter these records by <strong>" + filterName + "</strong>, enter filter criteria in the " + filterName + " text box.<br />For example:" +
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

//*********************************************************************************************************
// Help DIALOG
//*********************************************************************************************************
addNamespace("Target.Abacus.Web.Apps.Dom.ProviderInvoice.HelpDialog");

Target.Abacus.Web.Apps.Dom.ProviderInvoice.HelpDialog = function(title, caption) { 
	this.SetTitle(title);
	this.clearContent;
	this._type = 1;
	this.SetContentText(caption);
	this.SetWidth("39");
	
}

// inherit from base
Target.Abacus.Web.Apps.Dom.ProviderInvoice.HelpDialog.prototype = new Target.Web.Dialog();

// override show method
Target.Abacus.Web.Apps.Dom.ProviderInvoice.HelpDialog.prototype.Show = function() {
	this.ClearButtons();
	switch(this._type) {
		case 1:
			// Alert
			this.AddButton("OK", "", this._callback, new Array(this, 1));	// 1 = OK
			break;
		default:
			alert("Target.Abacus.Web.Apps.Dom.ProviderInvoice.HelpDialog: unknown dialog type specified.");
			break;
	}
	Target.Web.Dialog.prototype.Show.call(this);
}

function getSelectedAdditionalFilter() 
{    
    if (noAdditionalFiltering.checked) 
    {
	    return CONST_NO_ADDITIONAL_FILTERING; 
	}
	if (unplannedService.checked) 
    {
	    return CONST_UNPLANNED_SERVICE; 
	}
	if (costExceeded.checked) 
    {
	    return CONST_COST_EXCEEDED; 
	}
	if (costExceededWithinTolerance.checked) 
    {
	    return CONST_COST_EXCEEDED_WITHIN_TOLERANCE; 
	}
	if (unitsExceeded.checked) 
    {
        return CONST_UNITS_EXCEEDED; 
	}
	if (unitsExceededWithinCost.checked) 
    {
        return CONST_UNITS_EXCEEDED_WITHIN_COST; 
	}
	if (manuallySuspended.checked) 
    {
        return CONST_MANUALLY_SUSPENDED;  
	}
}
