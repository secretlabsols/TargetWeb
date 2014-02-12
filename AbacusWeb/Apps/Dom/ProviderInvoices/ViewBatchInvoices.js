
var contractSvc, currentPage;
var currentInvoiceID, currentDPIBatchID, currentProviderID, currentContractID;
var currentClientID, weFrom, weTo;
var invoiceNumber, invoiceRef, statusDateFrom, statusDateTo;
var statusIsUnpaid, statusIsAuthorised, statusIsPaid, statusIsSuspended;
var listFilter, listProviderName = "", listContractNum = "", listSUName = "";
var tblInvoices, divPagingLinks;
var lblInvoiceCount, lblInvoiceValueNet, lblInvoiceValueVAT, lblInvoiceValueGross;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	tblInvoices = GetElement("tblInvoices");
	divPagingLinks = GetElement("DomProviderInvoiceBatchInvoices_PagingLinks");
	lblInvoiceCount = GetElement("lblInvoiceCount");
	lblInvoiceValueNet = GetElement("lblInvoiceValueNet");
	lblInvoiceValueVAT = GetElement("lblInvoiceValueVAT");
	lblInvoiceValueGross = GetElement("lblInvoiceValueGross");
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListDPIInvFilter_Callback);
	listFilter.AddColumn("Provider", GetElement("thProviderName"));
	listFilter.AddColumn("Contract No.", GetElement("thContractNum"));
	listFilter.AddColumn("Svc User Name", GetElement("thSUName"));
			
	// populate table
	currentDPIBatchID = GetQSParam(document.location.search, "batchid");
	currentProviderID = GetQSParam(document.location.search, "providerid");
	currentContractID = GetQSParam(document.location.search, "contractid");
	
	FetchDomProviderInvoiceList(currentPage);
}

function ListDPIInvFilter_Callback(column) {
	switch(column.Name) {
		case "Provider":
			listProviderName = column.Filter;
			break;
		case "Contract No.":
			listContractNum = column.Filter;
			break;
		case "Svc User Name":
			listSUName = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchDomProviderInvoiceList(1);
}

function FetchDomProviderInvoiceList(page) {
	currentPage = page;
	DisplayLoading(true);
	if(currentInvoiceID == undefined || currentInvoiceID == "null") currentInvoiceID = 0;
	if(currentDPIBatchID == undefined || currentDPIBatchID == "null") currentDPIBatchID = 0;
	if(currentProviderID == undefined || currentProviderID == "null") currentProviderID = 0;
	if(currentContractID == undefined || currentContractID == "null") currentContractID = 0;
	if(listProviderName == "") listProviderName="null";
	if(listContractNum == "") listContractNum="null";
	if(listSUName == "") listSUName="null";

	contractSvc.FetchDomProviderInvoiceList(page, currentInvoiceID, 
	    currentDPIBatchID, currentProviderID, listProviderName,
	    currentContractID, listContractNum, currentClientID, listSUName, 
	    weFrom, weTo, invoiceNumber, invoiceRef, 
	    statusDateFrom, statusDateTo, true, true, 
	    true, true, "", "", FetchDomProvInvoiceList_Callback)
}

function FetchDomProvInvoiceList_Callback(response) {
	var invoices, index;
	var tr, td;
	var str;
	var link;

	if(CheckAjaxResponse(response, contractSvc.url)) {		
		// populate the table
		invoices = response.value.Invoices;

		// remove existing rows
		ClearTable(tblInvoices);
		for(index=0; index<invoices.length; index++) {
		    
			tr = AddRow(tblInvoices);

			td = AddCell(tr, invoices[index].ProviderName);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, invoices[index].ContractNumber);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, invoices[index].ClientName);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
		
		    td = AddCell(tr, "");
		    str = "javascript:EditInvoice(" + invoices[index].ID + ", " + invoices[index].ProviderID + ");"
		    link = AddLink(td, invoices[index].InvoiceNumber, str, "Click here to edit this invoice.");
			link.className = "transBg";


			td = AddCell(tr, invoices[index].InvoiceRef);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";		
			
			td = AddCell(tr, invoices[index].InvoiceTotal);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function btnPrint_Click() {
    if(listProviderName == undefined || listProviderName == "null") listProviderName = "";
    if(listContractNum == undefined || listContractNum == "null") listContractNum = "";
    if(listSUName == undefined || listSUName == "null") listSUName = "";
    var url = "PrintBatchInvoice.aspx?invID=0" + 
                "&batchID=" + currentDPIBatchID + "&providerID=" + currentProviderID + "&providerName=" + listProviderName + 
                "&contractID=" + currentContractID + "&contractNum=" + listContractNum + 
                "&clientID=" + currentClientID + "&clientName=" + listSUName
    OpenPopup(url, 75, 50, 1);
}

function EditInvoice(invoiceID, providerID) {
	// mode=1 means Fetched
	var url = "Edit.aspx?mode=1&id=" + invoiceID + "&estabID=" + providerID + "&backUrl=" + escape(document.location.href);
	document.location.href = url;
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

addEvent(window, "load", Init);
