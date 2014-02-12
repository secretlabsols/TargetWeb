
var contractSvc, currentPage, providerID, contractID, clientID, batchType, batchStatus, dateFrom, dateTo, pScheduleId;
var selectedInvoiceID, selectedInvoiceBatchID, btnCopyID, btnNewID, btnVerifyID, btnUnVerifyID, btnDeleteID, btnViewID, btnEditID, btnViewBatchID,btnViewInvoiceID;
var tblInvoices, divPagingLinks, btnView, btnVerify, btnUnVerify, btnDelete, btnCopy, btnNew, btnEdit,btnViewBatch, btnViewInvoice, dteCopyWeekEndingID;

function Init() {
	contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
	tblInvoices = GetElement("tblInvoices");
	divPagingLinks = GetElement("Invoice_PagingLinks");
	btnView = GetElement(btnViewID, true);
	btnVerify = GetElement(btnVerifyID, true);
	btnUnVerify = GetElement(btnUnVerifyID, true);
	btnDelete = GetElement(btnDeleteID, true);
	btnCopy = GetElement(btnCopyID, true);
	btnNew = GetElement(btnNewID, true);
	btnEdit = GetElement(btnEditID, true);
	btnViewBatch = GetElement(btnViewBatchID, true);
	btnViewInvoice = GetElement(btnViewInvoiceID, true);

    EnableButtons(false, null, null);

    FetchInvoiceList(currentPage, selectedInvoiceID);

}
function FetchInvoiceList(page, selectedID) {
    currentPage = page;
    if (selectedID == undefined) selectedID = 0;
	DisplayLoading(true);
	EnableButtons(false, null, null);
	contractSvc.FetchManualDomProformaInvoiceEnquiryResults(currentPage, selectedID, providerID, contractID, clientID, batchType, batchStatus, dateFrom, dateTo, FetchInvoiceList_Callback);
}
function FetchInvoiceList_Callback(response) {
    var index, invoices, str;
    if(CheckAjaxResponse(response, contractSvc.url)) {
        
        invoices = response.value.Invoices;
        ClearTable(tblInvoices);
                
        for(index=0; index<invoices.length; index++) {
        
            tr = AddRow(tblInvoices);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "InvoiceSelect", invoices[index].BatchID, RadioButton_Click);
			AddInput(td, "hidBatchTypeID" + invoices[index].InvoiceID, "hidden", "", "", invoices[index].BatchTypeID, null, null);
			AddInput(td, "hidBatchStatusID" + invoices[index].InvoiceID, "hidden", "", "", invoices[index].BatchStatusID, null, null);
			AddInput(td, "hidInvoiceID" + invoices[index].InvoiceID, "hidden", "", "", invoices[index].InvoiceID, null, null);
			
			AddCell(tr, Date.strftime("%d/%m/%Y", invoices[index].WETo));
			
			td = AddCell(tr, "");
			td.innerHTML = invoices[index].CalculatedPayment.toString().formatCurrency();
			
			AddCell(tr, invoices[index].BatchStatus);
			
			AddCell(tr, Date.strftime("%d/%m/%Y", invoices[index].StatusDate));
			
			str = invoices[index].Reference;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
			
			if(invoices[index].InvoiceID == selectedInvoiceID || ( currentPage == 1 && invoices.length == 1))
			    radioButton.click();
        
        }
        
        // load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}
function ChangeProformaInvoiceBatchStatus(invoiceBatchID, newStatus) {
    ShowProcessingModalDIV();
    contractSvc.ChangeProformaInvoiceBatchStatus(invoiceBatchID, newStatus, ChangeProformaInvoiceBatchStatus_Callback);
}
function ChangeProformaInvoiceBatchStatus_Callback(response) {
    if(CheckAjaxResponse(response, contractSvc.url)) {
        FetchInvoiceList(currentPage);
    }
    HideModalDIV();
}
function RadioButton_Click() {
    var index, rdo, selectedRow, batchTypeID, batchStatusID;
	for (index = 0; index < tblInvoices.tBodies[0].rows.length; index++){
		rdo = tblInvoices.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblInvoices.tBodies[0].rows[index];
			tblInvoices.tBodies[0].rows[index].className = "highlightedRow"
			selectedInvoiceBatchID = rdo.value;
			batchTypeID = parseInt(selectedRow.cells[0].getElementsByTagName("INPUT")[1].value, 10);
			batchStatusID = parseInt(selectedRow.cells[0].getElementsByTagName("INPUT")[2].value, 10);
			selectedInvoiceID = parseInt(selectedRow.cells[0].getElementsByTagName("INPUT")[3].value, 10);
			EnableButtons(true, batchTypeID, batchStatusID);
			
		} else {
			tblInvoices.tBodies[0].rows[index].className = ""
		}
	}
}
function EnableButtons(enable, batchTypeID, batchStatusID) {
    var enableDelete = false;
    var enableVerify = false;
    var enableUnVerify = false;
    var enableEdit = false;
    
	if(batchTypeID && batchTypeID == Target.Abacus.Library.DomProformaInvoiceBatchType.ManuallyEntered) {
	    switch(batchStatusID) {
            case Target.Abacus.Library.DomProformaInvoiceBatchStatus.AwaitingVerification:
                enableDelete = true;
	            enableVerify = true;
	            enableEdit = true;
                break;
            case Target.Abacus.Library.DomProformaInvoiceBatchStatus.Verified:
                enableUnVerify = true;
                break;
        }
    }
        
    if(btnView) btnView.disabled = !enable;
	if(btnCopy) btnCopy.disabled = !enable;
	if(btnDelete) btnDelete.disabled = !enableDelete;
    if(btnVerify) btnVerify.disabled = !enableVerify;
    if(btnUnVerify) btnUnVerify.disabled = !enableUnVerify;
    if(btnEdit) btnEdit.disabled = !enableEdit;
}


function btnEdit_Click() {
    document.location.href = "ManualEnter.aspx" + document.location.search + "&id=" + selectedInvoiceID + "&mode=3&backUrl=" + GetBackUrl();
}
function btnDelete_Click() {
    if(window.confirm("Once an invoice batch is deleted it cannot be re-instated.\n\nAre you sure you wish to delete this invoice batch?")) 
        ChangeProformaInvoiceBatchStatus(selectedInvoiceBatchID, Target.Abacus.Library.DomProformaInvoiceBatchStatus.Deleted);
}
function btnUnVerify_Click() {
    if(window.confirm("Are you sure you wish to un-verify this invoice batch?")) 
        ChangeProformaInvoiceBatchStatus(selectedInvoiceBatchID, Target.Abacus.Library.DomProformaInvoiceBatchStatus.AwaitingVerification);
}
function btnVerify_Click() {
    if(window.confirm("Are you sure you wish to verify this invoice batch?")) 
        ChangeProformaInvoiceBatchStatus(selectedInvoiceBatchID, Target.Abacus.Library.DomProformaInvoiceBatchStatus.Verified);
}

function GetBackUrl() {
    var url = document.location.href;
    url = AddQSParam(RemoveQSParam(url, "invoiceID"), "invoiceID", selectedInvoiceID);
    return escape(url);
}

function btnView_Click() {
    var url = "ManualEnterInvoice.aspx?=null" +
    "&estabid=" + GetQSParam(document.location.href, "estabid") +
    "&contractid=" + GetQSParam(document.location.href, "contractid") +
    "&pscheduleid=" + GetQSParam(document.location.href, "pscheduleid") +
    "&clientid=" + GetQSParam(document.location.href, "clientid") +
    "&invoiceid=" + selectedInvoiceID +
    "&id=" + selectedInvoiceID +
    "&mode=1" +
    "&pSWE=" + GetQSParam(document.location.href, "pSWE") +
    "&pscheduleref=" + GetQSParam(document.location.href, "pscheduleref") +
    "&copyvisit=true" +
    "&backUrl=" + GetBackUrl();  
    //+  "&copyUrl=" + GetBackUrl();
    document.location.href = url;
}


function btnCopy_Click() {

    var url = "ManualEnterInvoice.aspx?copyFromID=" + selectedInvoiceID +
                                    "&copyFromWE=" + GetQSParam(document.location.search, "pSWE") +
                                    "&pSWE=" + GetQSParam(document.location.search, "pSWE") +
                                    "&pscheduleid=" + pScheduleId +
                                    "&copyvisit=true" +
                                    "&mode=2&backUrl=" + GetBackUrl();  
                                    //+  "&copyUrl=" + GetBackUrl();

    document.location.href = url;

}
function btnNew_Click() {

    var url = "ManualEnterInvoice.aspx" + document.location.search +
              "&mode=2&backUrl=" + GetBackUrl() +
              "&copyvisit=true";  
              //+ "&copyUrl=" + GetBackUrl();
    document.location.href = url;
    //document.location.href = "ManualEnter.aspx" + document.location.search + "&mode=2&backUrl=" + GetBackUrl();
}

function CopyDialog_Callback(evt, args) {
    var d = args[0];
    var answer = args[1];
    var emptyDialogContent, copyDialogContentContainer, copyDialogContent;
    var valRequired, weekEndingDate, response;
    
    // answer == 1 means OK
    if(answer == 1) {
        if(Page_ClientValidate("Copy")) {
            weekEndingDate = GetElement(dteCopyWeekEndingID + "_txtTextBox").value;
            valRequired = GetElement(dteCopyWeekEndingID + "_valRequired");
            
            response = contractSvc.WeekendingDateValid(weekEndingDate.toDate()).value;
            if(response.Success == false) {
                alert(response.Message);
                return;
            } else {

            document.location.href = "ManualEnterInvoice.aspx?copyFromID=" + selectedInvoiceID +
                                    "&copyFromWE=" + weekEndingDate +
                                    "&pscheduleid=" + pScheduleId +
                                    "&mode=2&backUrl=" + GetBackUrl();
            }
        } else {
            return;
        }
    }
    copyDialogContentContainer = GetElement("divCopyDialogContentContainer");
    emptyDialogContent = copyDialogContentContainer.getElementsByTagName("DIV")[0];
    copyDialogContent = d._content.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = copyDialogContentContainer.removeChild(emptyDialogContent);
    copyDialogContent = d._content.removeChild(copyDialogContent);
    copyDialogContentContainer.appendChild(copyDialogContent);
    d.AddContent(emptyDialogContent);
    
    d.Hide();
}


addEvent(window, "load", Init);
