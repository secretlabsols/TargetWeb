
var InvoiceHasNotes;
var proformainvoiceSvc;
var NonDeliveryOfService;
var psService;

function Init() {
    if (InvoiceHasNotes == 'true') {
        AddNotesLink();
    }
    proformainvoiceSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomProfomaInvoice_class();
    NonDeliveryOfService = new Target.Abacus.Extranet.Apps.WebSvc.DomProviderInvoice_class();
    psService = new Target.Abacus.Extranet.Apps.WebSvc.PaymentSchedule_class();
}

function AddNotesLink() {
    $('#imgNotes').prepend('<img alt="View provider-entered note" style="cursor:pointer;" onclick="DisplayNotes();" id="theImg" src="../../../../images/Notes.png" />');
}

function ShowDeliverydetail(paymentScheduleId,invoiceId, invoiceDetailId, plannedUnits, UnitCost, timebased) {
    if (timebased == "true") {
        loadTimeBased(paymentScheduleId, invoiceId, invoiceDetailId, plannedUnits, UnitCost);
    }
    else {

        loadNonTimeBased(paymentScheduleId, invoiceId, invoiceDetailId, plannedUnits, UnitCost);
    }
}

function loadTimeBased(paymentScheduleId, invoiceid, invoiceDetailId, plannedUnits, UnitCost) {
    var $invoice = NonDeliveryOfService.FetchNonDeliveryCurrentProviderInvoice(invoiceid);
    var $invoiceDetail = NonDeliveryOfService.FetchNonDeliveryCurrentProviderInvoiceDetail(invoiceDetailId);
    var $ps = psService.FetchPaymenstSchedule(paymentScheduleId);

    if ($invoiceDetail.value.ErrMsg.Success) {
        $invoiceDetail.value.CurrentProformaDetail.PlannedUnits = plannedUnits;
        $invoiceDetail.value.CurrentProformaDetail.UnitCost = UnitCost;
    }

    var $response = NonDeliveryOfService.FetchDomProviderInvoiceDetailNonDeliveryVisitBasedEnquiryResults(invoiceDetailId);

    var initSettings = {
        serviceDomProformas: proformainvoiceSvc,
        paymentSchedule: $ps.value.PaymentSchedule,
        proformaInvoice: $invoice.value.CurrentProforma,
        onOkd: function(args) {
            onUpdatedNonDeliveryVisitBasedRecords(args);
        },
        onCancelled: null
    };
   
    var showSettings = {
        id: invoiceDetailId,
        weekEnding: $invoiceDetail.value.CurrentProformaDetail.WeekEnding,
        isEditable: false,
        overriddenItems: $response.value.NonDeliveryVisits,
        proformaInvoiceDetail: $invoiceDetail.value.CurrentProformaDetail
    };
    $(document).nonDeliveryVisitBasedProformaInvoiceLineDialog(initSettings);
    $(document).nonDeliveryVisitBasedProformaInvoiceLineDialog('show', showSettings);
    
}

function loadNonTimeBased(paymentScheduleId, invoiceid, invoiceDetailId, plannedUnits, UnitCost) {

    var $invoice = NonDeliveryOfService.FetchNonDeliveryCurrentProviderInvoice(invoiceid);
    var $invoiceDetail = NonDeliveryOfService.FetchNonDeliveryCurrentProviderInvoiceDetail(invoiceDetailId);
    var $ps = psService.FetchPaymenstSchedule(paymentScheduleId);

    if ($invoiceDetail.value.ErrMsg.Success) {
        $invoiceDetail.value.CurrentProformaDetail.PlannedUnits = plannedUnits;
        $invoiceDetail.value.CurrentProformaDetail.UnitCost = UnitCost;
    }

    var $response = NonDeliveryOfService.FetchDomProviderInvoiceDetailNonDeliveryUnitBasedEnquiryResults(invoiceDetailId);

    var initSettings = {
        serviceDomProformas: proformainvoiceSvc,
        paymentSchedule: $ps.value.PaymentSchedule,
        proformaInvoice: $invoice.value.CurrentProforma,
        onOkd: function(args) {
            onUpdatedNonDeliveryVisitBasedRecords(args);
        },
        onCancelled: null
    };
    var showSettings = {
        id: invoiceDetailId,
        weekEnding: $invoiceDetail.value.CurrentProformaDetail.WeekEnding,
        isEditable: false,
        overriddenItems: $response.value.NonDeliveryUnits,
        proformaInvoiceDetail: $invoiceDetail.value.CurrentProformaDetail
    };
    $(document).nonDeliveryUnitBasedProformaInvoiceLineDialog(initSettings);
    $(document).nonDeliveryUnitBasedProformaInvoiceLineDialog('show', showSettings);

}
addEvent(window, "load", Init);