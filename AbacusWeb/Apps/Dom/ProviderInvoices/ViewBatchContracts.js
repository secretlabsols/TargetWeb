
var contractSvc, currentPage;
var currentDPIBatchID, currentProviderID, currentContractID;
var listFilter, listProviderName = "", listContractNum = "";
var tblContracts, divPagingLinks;
var lblInvoiceCount, lblInvoiceValueNet, lblInvoiceValueVAT, lblInvoiceValueGross;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	tblContracts = GetElement("tblContracts");
	divPagingLinks = GetElement("DomProviderInvoiceBatchContracts_PagingLinks");
	lblInvoiceCount = GetElement("lblInvoiceCount");
	lblInvoiceValueNet = GetElement("lblInvoiceValueNet");
	lblInvoiceValueVAT = GetElement("lblInvoiceValueVAT");
	lblInvoiceValueGross = GetElement("lblInvoiceValueGross");
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListDPIBatchContractsFilter_Callback);
	listFilter.AddColumn("Provider Name", GetElement("thProviderName"));
	listFilter.AddColumn("Contract No.", GetElement("thContractNum"));
			
	// populate table
	currentDPIBatchID = GetQSParam(document.location.search, "batchid");
	currentProviderID = GetQSParam(document.location.search, "providerid");
	currentContractID = GetQSParam(document.location.search, "contractid");
	
	FetchDPIBatchContractsList(currentPage);
}

function ListDPIBatchContractsFilter_Callback(column) {
	switch(column.Name) {
		case "Provider Name":
			listProviderName = column.Filter;
			break;
		case "Contract No.":
			listContractNum = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchDPIBatchContractsList(1);
}

function FetchDPIBatchContractsList(page) {
	currentPage = page;
	DisplayLoading(true);
	if(currentDPIBatchID == undefined || currentDPIBatchID == "null") currentDPIBatchID = 0;
	if(currentProviderID == undefined || currentProviderID == "null") currentProviderID = 0;
	if(currentContractID == undefined || currentContractID == "null") currentContractID = 0;
	if(listProviderName == "") listProviderName="null";
	if(listContractNum == "") listContractNum="null";

	contractSvc.FetchDomProviderContractsByBatchList(page, currentDPIBatchID, 
	    currentProviderID, listProviderName, currentContractID, listContractNum, 
	    FetchDPIBatchContractsList_Callback)
}

function FetchDPIBatchContractsList_Callback(response) {
	var contracts, index;
	var tr, td;
	var str;
	var link;
    var contractKey;
    

	if(CheckAjaxResponse(response, contractSvc.url)) {		
		// populate the table
		contracts = response.value.Contracts;

        if (contracts.length == 0) {
            lblInvoiceCount.innerHTML = "0";
            lblInvoiceValueNet.innerHTML = '&pound;' + "0.00";
            lblInvoiceValueVAT.innerHTML = '&pound;' + "0.00";
            lblInvoiceValueGross.innerHTML = '&pound;' + "0.00";
        }
        else {
	        lblInvoiceCount.innerHTML = contracts[0].InvoiceCountTotal;
	        lblInvoiceValueNet.innerHTML = contracts[0].InvoiceValueNetTotal;
	        lblInvoiceValueVAT.innerHTML = contracts[0].InvoiceValueVATTotal;
	        lblInvoiceValueGross.innerHTML = contracts[0].InvoiceValueGrossTotal;
        }
        
		// remove existing rows
		ClearTable(tblContracts);
		for(index=0; index<contracts.length; index++) {
            currentDPIBatchID = contracts[index].BatchID;
            currentProviderID = contracts[index].ProviderID;
            currentContractID = contracts[index].ContractID;
		
			tr = AddRow(tblContracts);

			td = AddCell(tr, contracts[index].ProviderName);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, contracts[index].ContractNumber);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, contracts[index].InvoiceCount);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			td = AddCell(tr, contracts[index].InvoiceValueNet);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, contracts[index].InvoiceValueVAT);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";		
			
			td = AddCell(tr, contracts[index].InvoiceValueGross);
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
    var url = "PrintBatchContract.aspx?batchID=" + currentDPIBatchID + 
        "&providerID=" + currentProviderID + "&providerName=" + listProviderName + 
        "&contractID=" + currentContractID + "&contractNum=" + listContractNum;
    OpenPopup(url, 75, 50, 1);
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

addEvent(window, "load", Init);
