var paymentsSvc, tblDetailLines, divPagingLinks, btnDownload, currentPage, clientID, serviceID, dateFrom, dateTo, btnStatement;
var tblDetailLinesID = "ListDetailLines";
var divPagingLinksID = "Detail_PagingLinks";
//var btnDownloadID = "btnDownload";
var btnStatementID = "btnStatement";

function Init() {
	paymentsSvc = new Target.SP.Web.Apps.WebSvc.Payments_class();
	tblDetailLines = GetElement(tblDetailLinesID);
	divPagingLinks = GetElement(divPagingLinksID);
	btnStatement = GetElement(btnStatementID);
//	btnDownload = GetElement(btnDownloadID);
	// populate table
	FetchDetailLinesList(currentPage);
}

/* FETCH REMITTANCE DETAIL LIST METHODS */
function FetchDetailLinesList(page) {
	currentPage = page;
	DisplayLoading(true);
	btnStatement.disabled = true;
//	btnDownload.disabled = true;
	paymentsSvc.FetchRemittanceDetailForUserList(page, serviceID, clientID, dateFrom, dateTo, FetchDetailLinesList_Callback)
}
function FetchDetailLinesList_Callback(response) {
	var detailLines, counter, tr, td, str;
	
	if(CheckAjaxResponse(response, paymentsSvc.url)) {
		detailLines = response.value.DetailLines;
		
		btnStatement.disabled = (detailLines == 0);
//		btnDownload.disabled = (detailLines == 0);

		ClearTable(tblDetailLines);
		for(counter=0; counter<detailLines.length; counter++) {
			tr = AddRow(tblDetailLines);
			// reference
			td = AddCell(tr, detailLines[counter].Reference);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// comment
			td = AddCell(tr, detailLines[counter].Comment);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// amount
			td = AddCell(tr, "");
			td.innerHTML = detailLines[counter].Amount.toString().formatCurrency();
			// our ref
			str = detailLines[counter].OurRef
			if(!str) str = " ";
			if(str.length == 0) str = " ";
			AddCell(tr, str);
			// your ref
			str = detailLines[counter].YourRef
			if(!str) str = " ";
			if(str.length == 0) str = " ";
			AddCell(tr, str);
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
/*
function btnDownload_OnClick() {
	var url = "GetProviderInterfaceFile.aspx?format=target&serviceID=" + serviceID + "&clientID=" + clientID;
	if(dateFrom) url += "&dateFrom=" + Date.strftime("%d/%m/%Y", dateFrom);
	if(dateTo) url += "&dateTo=" + Date.strftime("%d/%m/%Y", dateTo);
	document.location.href = url;
}
*/
function btnStatement_OnClick() {
	window.open("ViewStatement.aspx" + document.location.search);
}

