var viewInvoiceId, viewCostedVisitId, find;

function Init() {
    if (find) {
        GetElement(viewInvoiceId).href = unescape(GetQSParam(document.location.search, "invoices"));
        GetElement(viewCostedVisitId).href = unescape(GetQSParam(document.location.search, "costed"));
    }
}

addEvent(window, "load", Init);