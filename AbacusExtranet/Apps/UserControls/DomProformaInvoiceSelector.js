
var contractSvc, currentPage, providerID, contractID, batchType, batchStatus, dateFrom, dateTo, fileID;
var selectedBatchID, domContractID, btnViewContractID, btnViewBatchID, btnViewInvoicesID;
var tblBatches, divPagingLinks, btnViewContract, btnViewBatch, btnViewInvoices;

function Init() {
    alert();
	contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
	tblBatches = GetElement("tblBatches");
	divPagingLinks = GetElement("Invoice_PagingLinks");
	btnViewContract = GetElement(btnViewContractID, true);
	btnViewBatch = GetElement(btnViewBatchID, true);
	btnViewInvoices = GetElement(btnViewInvoicesID, true);

    FetchBatchList(currentPage, selectedBatchID);
}
function FetchBatchList(page, selectedID) {
    currentPage = page;
	DisplayLoading(true);

	if(selectedID == undefined) selectedID = 0;

	if(btnViewContract) btnViewContract.disabled = true;
	if(btnViewBatch) btnViewBatch.disabled = true;
	if(btnViewInvoices) btnViewInvoices.disabled = true;
		
	contractSvc.FetchDomProformaInvoiceEnquiryResults(currentPage, fileID, selectedID, providerID, contractID, batchType, batchStatus, dateFrom, dateTo, FetchBatchList_Callback);
}
function FetchBatchList_Callback(response) {
    var index, batches, str;
    
    if(CheckAjaxResponse(response, contractSvc.url)) {
        batches = response.value.Batches;
        ClearTable(tblBatches);     
        for(index=0; index<batches.length; index++) {
        
            tr = AddRow(tblBatches);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "BatchSelect", batches[index].BatchID, RadioButton_Click);
			AddInput(td, "hidDomContractID" + batches[index].BatchID, "hidden", "", "", batches[index].DomContractID, null, null);
			
			AddCell(tr, batches[index].Reference);
			AddCell(tr, batches[index].ContractNumber);
			AddCell(tr, batches[index].ContractTitle);
			AddCell(tr, batches[index].BatchStatus);
			AddCell(tr, Date.strftime("%d/%m/%Y", batches[index].StatusDate));
			
			td = AddCell(tr, "");
			td.innerHTML = batches[index].NetPayment.toString().formatCurrency();
			
			if(batches[index].BatchID == selectedBatchID || ( currentPage == 1 && batches.length == 1))
			    radioButton.click();
        
        }
        
        // load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}

function RadioButton_Click() {
    var index, rdo, selectedRow, batchTypeID, batchStatusID;
	for (index = 0; index < tblBatches.tBodies[0].rows.length; index++){
		rdo = tblBatches.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblBatches.tBodies[0].rows[index];
			domContractID = parseInt(selectedRow.cells[0].getElementsByTagName("INPUT")[1].value, 10);
			tblBatches.tBodies[0].rows[index].className = "highlightedRow";
			selectedBatchID = rdo.value;
	
			if(btnViewContract) btnViewContract.disabled = false;
	        if(btnViewBatch) btnViewBatch.disabled = false;
	        if(btnViewInvoices) btnViewInvoices.disabled = false;
		} else {
			tblBatches.tBodies[0].rows[index].className = ""
		}
    }
    alert("radio invoice clicked");
}

function GetBackUrl() {
    var url = document.location.href;
    url = AddQSParam(RemoveQSParam(url, "invoiceID"), "invoiceID", selectedBatchID);
    return escape(url);
}
function btnViewContract_Click() {
    document.location.href = "../Contracts/ViewDomiciliaryContract.aspx?id=" + domContractID + "&backUrl=" + GetBackUrl();
}
function btnViewBatch_Click() {
    document.location.href = "ViewInvoiceBatch.aspx?id=" + selectedBatchID + "&backUrl=" + GetBackUrl();
}
function btnViewInvoices_Click() {
    document.location.href = "ViewInvoices.aspx?id=" + selectedBatchID + "&backUrl=" + GetBackUrl();
}

addEvent(window, "load", Init);
