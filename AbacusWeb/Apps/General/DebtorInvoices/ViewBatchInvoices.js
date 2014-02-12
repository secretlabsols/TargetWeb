var NULL_DATE_FROM = "1900-01-01", NULL_DATE_TO = "9999-12-31";
var INVTYPE_RES = "RES", INVTYPE_DOM = "DOM", INVTYPE_LD = "LD";
var INVTYPE_CLIENT = "CL", INVTYPE_TP = "TP", INVTYPE_PROP = "PROP";
var INVTYPE_OLA = "OLA", INVTYPE_PENCOLL = "PEN", INVTYPE_HOMECOLL = "HC";
var INVTYPE_STD = "STD", INVTYPE_MANUAL = "MAN", INVTYPE_SDS = "SDS";

var contractSvc, currentPage, invoiceID, batchID, clientID, invoices;
var invRes, invDom, invLD, invStd, invManual, invSDS;
var invClient, invTP, invProp, invOLA, invPenColl, invHomeColl;
var invDateFrom, invDateTo, invBatchSel, invExclude;
var invActual, invProvisional, invRetracted, invViaRetract;
var listFilter, listDebtorName = "", listInvoiceNum = "";
var tblInvoices, divPagingLinks;
var lblInvoiceCount, lblInvoiceValue;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	tblInvoices = GetElement("tblInvoices");
	divPagingLinks = GetElement("DebtorInvoiceBatchInvoices_PagingLinks");
	lblInvoiceCount = GetElement("lblInvoiceCount");
	lblInvoiceValue = GetElement("lblInvoiceValue");
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListInvoiceFilter_Callback);
	listFilter.AddColumn("Debtor", GetElement("thDebtor"));
	listFilter.AddColumn("Inv No.", GetElement("thInvNum"));
			
	// populate table
	batchID = GetQSParam(document.location.search, "batchid");

    currentPage = 1;	
	FetchDebtorInvoiceList(currentPage);
}

function ListInvoiceFilter_Callback(column) {
	switch(column.Name) {
		case "Debtor":
			listDebtorName = column.Filter;
			break;
		case "Inv No.":
			listInvoiceNum = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchDebtorInvoiceList(1);
}

function FetchDebtorInvoiceList(page) {
	currentPage = page;
	DisplayLoading(true);

	contractSvc.FetchDebtorInvoiceList(page, "0", "0",
	    "true", "true", "true", "true", "true", "true",
        "true", "true", "true", "true", "true", "true",
        "true", "true", "true", "true",
        "null", "null", "1", "false",
        listDebtorName, listInvoiceNum, batchID,
        FetchDebtorInvoiceList_Callback);
}

function FetchDebtorInvoiceList_Callback(response) {
	var invoices, index;
	var tr, td;
	var str;
	var link;

	if (CheckAjaxResponse(response, contractSvc.url)) {		
		// populate the table
		invoices = response.value.Invoices;

		// remove existing rows
		ClearTable(tblInvoices);
		for (index=0; index<invoices.length; index++) {

			tr = AddRow(tblInvoices);

		    if (invoices[index].IsTPInvoice == "Yes") {
			    td = AddCell(tr, invoices[index].ClientName + " / " + invoices[index].ClientRef + " (c/o " + invoices[index].ThirdPartyName + ")");
			} else if (invoices[index].IsPropertyInvoice == "Yes") {
			    td = AddCell(tr, invoices[index].ClientName + " / " + invoices[index].ClientRef + " (PROPERTY)");
			} else if (invoices[index].IsOLAInvoice == "Yes") {
			    td = AddCell(tr, invoices[index].ClientName + " / " + invoices[index].ClientRef + " (OLA)");
			} else if (invoices[index].IsPenCollectInvoice == "Yes") {
			    td = AddCell(tr, invoices[index].ClientName + " / " + invoices[index].ClientRef + " (PENSION)");
			} else if (invoices[index].IsHomeCollectInvoice == "Yes") {
			    td = AddCell(tr, invoices[index].ClientName + " / " + invoices[index].ClientRef + " (HOME COLLECT)");
			} else {
			    td = AddCell(tr, invoices[index].ClientName + " / " + invoices[index].ClientRef);
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// Create a key to highlight all the invoice types met by the current invoice:
			invTypes = "";
			if (invoices[index].IsResidentialInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_RES;
			if (invoices[index].IsDomiciliaryInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_DOM;
			if (invoices[index].IsLearnDisabInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_LD;
			//if (invoices[index].IsClientInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_CLIENT;
			//if (invoices[index].IsTPInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_TP;
			//if (invoices[index].IsPropertyInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_PROP;
			//if (invoices[index].IsOLAInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_OLA;
			//if (invoices[index].IsPenCollectInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_PENCOLL;
			//if (invoices[index].IsHomeCollectInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_HOMECOLL;
			if (invoices[index].IsStandardInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_STD;
			if (invoices[index].IsManualInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_MANUAL;
			if (invoices[index].IsSDSInvoice == "Yes") invTypes = invTypes + ", " + INVTYPE_SDS;
			invTypes = invTypes.substring(2);
			td = AddCell(tr, invTypes);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, invoices[index].InvoiceNumber);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, Date.strftime("%d/%m/%Y", invoices[index].DateCreated));
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, invoices[index].InvoiceTotal);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			if (invoices[index].IsRetractedInvoice == "Yes") {
			    td = AddCell(tr, "Retracted");
			} else if (invoices[index].IsViaRetraction == "Yes") {
			    td = AddCell(tr, "Retraction");
			} else if (invoices[index].IsProvisionalInvoice == "Yes") {
                td = AddCell(tr, "Provisional");
			} else {
			    td = AddCell(tr, "Actual");
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function btnPrint_Click() {
    if (listDebtorName == undefined || listDebtorName == "null") listDebtorName = "";
    if (listInvoiceNum == undefined || listInvoiceNum == "null") listInvoiceNum = "";
    var url = "PrintBatchInvoice.aspx?invID=0" + 
                "&batchID=" + batchID + 
                "&filterDebtor=" + listDebtorName + "&filterInvNum=" + listInvoiceNum
    OpenPopup(url, 75, 50, 1);
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

addEvent(window, "load", Init);
