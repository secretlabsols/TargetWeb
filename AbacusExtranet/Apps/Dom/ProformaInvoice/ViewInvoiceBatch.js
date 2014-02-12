var btnVerify, btnDelete, theform, currentStatus, selectedInvoiceBatchID, batchTypeID, btnVerifyID, btnDeleteID;
var contractSvc;

function Init() {
	contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
	btnVerify = GetElement(btnVerifyID, true);
	btnDelete = GetElement(btnDeleteID, true);
	
	if(btnDelete) btnDelete.disabled = true;
	if(btnVerify) btnVerify.disabled = true;
	
	if (batchTypeID != Target.Abacus.Library.DomProformaInvoiceBatchType.VisitAmendment && batchTypeID != Target.Abacus.Library.DomProformaInvoiceBatchType.RecalculationAdjustment) {
        switch (currentStatus)
	    {
	        case Target.Abacus.Library.DomProformaInvoiceBatchStatus.AwaitingVerification:
	            if(btnVerify) {
	                btnVerify.value = "Verify";
	                btnVerify.disabled = false;
                }
	            if(btnDelete) btnDelete.disabled = false;
	            break;
	        case Target.Abacus.Library.DomProformaInvoiceBatchStatus.Verified:
	            if(btnVerify) {
	                btnVerify.value = "UnVerify";
	                btnVerify.disabled = false;
                }
	            if(btnDelete) btnDelete.disabled = false;
	            break;
	    }
	}
	
}

function btnVerify_Click() {
    switch (btnVerify.value)
	{
        case "Verify":
            if(window.confirm("Are you sure you wish to verify this invoice batch?")) 
                ChangeProformaInvoiceBatchStatus(selectedInvoiceBatchID, Target.Abacus.Library.DomProformaInvoiceBatchStatus.Verified);
            break;
        case "UnVerify":
            if(window.confirm("Are you sure you wish to un-verify this invoice batch?")) 
                ChangeProformaInvoiceBatchStatus(selectedInvoiceBatchID, Target.Abacus.Library.DomProformaInvoiceBatchStatus.AwaitingVerification);
            break;  
     }
}

function btnDelete_Click() {
    if(window.confirm("Once an invoice batch is deleted it cannot be re-instated.\n\nAre you sure you wish to delete this invoice batch?")) 
        ChangeProformaInvoiceBatchStatus(selectedInvoiceBatchID, Target.Abacus.Library.DomProformaInvoiceBatchStatus.Deleted);
}

function GetBackUrl() {
    var url = document.location.href;
    url = AddQSParam(RemoveQSParam(url, "id"), "id", selectedInvoiceBatchID);
    return escape(url);
}

function btnViewInvoices_Click() {
    document.location.href = "ViewInvoices.aspx?id=" + selectedInvoiceBatchID
}

function ChangeProformaInvoiceBatchStatus(invoiceBatchID, newStatus) {
    ShowProcessingModalDIV();
    contractSvc.ChangeProformaInvoiceBatchStatus(invoiceBatchID, newStatus, ChangeProformaInvoiceBatchStatus_Callback);
}
function ChangeProformaInvoiceBatchStatus_Callback(response) {
    if(CheckAjaxResponse(response, contractSvc.url)) {
        document.location.reload();
    }
    HideModalDIV();
}