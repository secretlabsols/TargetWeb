
var url, SelectorWizard_currentStep;

// Header Details
var SelectorWizard_ReferenceID, Reference;
var SelectorWizard_PeriodFromID, PeriodFrom;
var SelectorWizard_PeriodToID, PeriodTo;
var SelectorWizard_VisitBasedYesID, VisitBasedYes;
var SelectorWizard_VisitBasedNoID, VisitBasedNo;

// Unprocessed Pro forma Invoices
var SelectorWizard_ProformaInvoicesNoneID, ProformaInvoicesNone;
var SelectorWizard_ProformaInvoicesAwaitingID, ProformaInvoicesAwaiting;
var SelectorWizard_ProformaInvoicesVerifiedID, ProformaInvoicesVerified;

// Provider Invoices
var SelectorWizard_InvoicesUnpaidID, InvoicesUnpaid;
var SelectorWizard_InvoicesSuspendedID, InvoicesSuspended;
var SelectorWizard_InvoicesAuthorisedID, InvoicesAuthorised;
var SelectorWizard_InvoicesPaidID, InvoicesPaid;

// Unprocessed Visit Amendment Requests
var SelectorWizard_VARawaitingID, VARawaiting;
var SelectorWizard_VARverifiedID, VARverified;
var SelectorWizard_VARdeclinedID, VARdeclined;

var NULL_DATE_FROM = "1900-01-01";
var NULL_DATE_TO   = "9999-12-31";

function PaymentSchedulesFilter_BeforeNavigate() {

    SelectorWizard_InitialiseElements();

	url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

	SelectorWizard_RemoveQSParams();

	SelectorWizard_AddQSParams();

	SelectorWizard_newUrl = url;
	
	return true;
}

function SelectorWizard_InitialiseElements() {
    Reference = GetElement(SelectorWizard_ReferenceID + "_txtTextBox");
    PeriodFrom = GetElement(SelectorWizard_PeriodFromID + "_txtTextBox");
    PeriodTo = GetElement(SelectorWizard_PeriodToID + "_txtTextBox");
    VisitBasedYes = GetElement(SelectorWizard_VisitBasedYesID + "_chkCheckbox");
    VisitBasedNo = GetElement(SelectorWizard_VisitBasedNoID + "_chkCheckbox");

    ProformaInvoicesNone = GetElement(SelectorWizard_ProformaInvoicesNoneID + "_chkCheckbox");
    ProformaInvoicesAwaiting = GetElement(SelectorWizard_ProformaInvoicesAwaitingID + "_chkCheckbox");
    ProformaInvoicesVerified = GetElement(SelectorWizard_ProformaInvoicesVerifiedID + "_chkCheckbox");

    InvoicesUnpaid = GetElement(SelectorWizard_InvoicesUnpaidID + "_chkCheckbox");
    InvoicesSuspended = GetElement(SelectorWizard_InvoicesSuspendedID + "_chkCheckbox");
    InvoicesAuthorised = GetElement(SelectorWizard_InvoicesAuthorisedID + "_chkCheckbox");
    InvoicesPaid = GetElement(SelectorWizard_InvoicesPaidID + "_chkCheckbox");

    VARawaiting = GetElement(SelectorWizard_VARawaitingID + "_chkCheckbox");
    VARverified = GetElement(SelectorWizard_VARverifiedID + "_chkCheckbox");
    VARdeclined = GetElement(SelectorWizard_VARdeclinedID + "_chkCheckbox");
}

function SelectorWizard_RemoveQSParams() {
    url = RemoveQSParam(url, "ref");
    url = RemoveQSParam(url, "periodFrom");
    url = RemoveQSParam(url, "periodTo");
    url = RemoveQSParam(url, "visitYes");
    url = RemoveQSParam(url, "visitNo");

    url = RemoveQSParam(url, "pfNone");
    url = RemoveQSParam(url, "pfAwait");
    url = RemoveQSParam(url, "pfVer");

    url = RemoveQSParam(url, "invUnpaid");
    url = RemoveQSParam(url, "invSusp");
    url = RemoveQSParam(url, "invAuth");
    url = RemoveQSParam(url, "invPaid");

    url = RemoveQSParam(url, "varAwait");
    url = RemoveQSParam(url, "varVer");
    url = RemoveQSParam(url, "varDec");
}

function SelectorWizard_AddQSParams() { 
    url = AddQSParam(url, "ref", Reference.value);

    if (PeriodFrom.value != "")
        url = AddQSParam(url, "periodFrom", PeriodFrom.value);
    else
        url = AddQSParam(url, "periodFrom", "null");

    if (PeriodTo.value != "")
        url = AddQSParam(url, "periodTo", PeriodTo.value);
    else
        url = AddQSParam(url, "periodTo", "null");

    url = AddQSParam(url, "visitYes", VisitBasedYes.checked);
    url = AddQSParam(url, "visitNo", VisitBasedNo.checked);

    url = AddQSParam(url, "pfNone", ProformaInvoicesNone.checked);
    url = AddQSParam(url, "pfAwait", ProformaInvoicesAwaiting.checked);
    url = AddQSParam(url, "pfVer", ProformaInvoicesVerified.checked);

    url = AddQSParam(url, "invUnpaid", InvoicesUnpaid.checked);
    url = AddQSParam(url, "invSusp", InvoicesSuspended.checked);
    url = AddQSParam(url, "invAuth", InvoicesAuthorised.checked);
    url = AddQSParam(url, "invPaid", InvoicesPaid.checked);

    url = AddQSParam(url, "varAwait", VARawaiting.checked);
    url = AddQSParam(url, "varVer", VARverified.checked);
    url = AddQSParam(url, "varDec", VARdeclined.checked);
}

function ShowHelp() {
    var d = new Target.Abacus.Web.Apps.Documents.DocumentTypes.HelpDialog("Reference Number Help", "<blockquote>" +
				"To filter these records by <b>Reference</b>, enter filter criteria in the Reference Number textbox." +
				 "<br />For example" +
				 "</blockquote>" +
				 "<ul style=\"margin-bottom:0em;\">" +
				 "    <li>To only show reference number(s) that <b>start</b> with ABC, enter ABC*" +
				 "    <li>To only show reference number(s) that <b>end</b> with ABC, enter *ABC" +
				 "    <li>To only show reference number(s) that <b>contain</b> with ABC, enter *ABC*" +
				 "</ul>");

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
addNamespace("Target.Abacus.Web.Apps.Documents.DocumentTypes.HelpDialog");

Target.Abacus.Web.Apps.Documents.DocumentTypes.HelpDialog = function(title, caption) {
    this.SetTitle(title);
    this.ClearContent;
    this.SetType(1);
    this.SetContentText(caption);
    this.SetWidth("45");
}

// inherit from base
Target.Abacus.Web.Apps.Documents.DocumentTypes.HelpDialog.prototype = new Target.Web.Dialog.Msg();

//*********************************************************************************************************
// End of Help DIALOG
//*********************************************************************************************************
