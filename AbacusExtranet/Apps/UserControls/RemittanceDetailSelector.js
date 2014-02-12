var paymentsSvc, tblDetailLines, divPagingLinks, currentPage, establishmentID, clientID, dateFrom, dateTo, btnStatement;

function Init() {
	paymentsSvc = new Target.Abacus.Extranet.Apps.WebSvc.ResPayments_class();
	tblDetailLines = GetElement("ListDetailLines");
	divPagingLinks = GetElement("Detail_PagingLinks");
	btnStatement = GetElement("btnStatement");
	// populate table
	FetchDetailLinesList(currentPage);
}

/* FETCH REMITTANCE DETAIL LIST METHODS */
function FetchDetailLinesList(page) {
	currentPage = page;
	DisplayLoading(true);
	btnStatement.disabled = true;
	paymentsSvc.FetchRemittanceDetailForUserList(page, establishmentID, clientID, dateFrom, dateTo, FetchDetailLinesList_Callback)
}
function FetchDetailLinesList_Callback(response) {
	var detailLines, counter, tr, td, str;
	
	if(CheckAjaxResponse(response, paymentsSvc.url)) {
		detailLines = response.value.DetailLines;
		
		btnStatement.disabled = (detailLines == 0);

		ClearTable(tblDetailLines);
		for(counter=0; counter<detailLines.length; counter++) {
			tr = AddRow(tblDetailLines);
			// reference
			td = AddCell(tr, detailLines[counter].Reference);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// dates
			AddCell(tr, Date.strftime("%d/%m/%Y", detailLines[counter].DateFrom));
			AddCell(tr, Date.strftime("%d/%m/%Y", detailLines[counter].DateTo));
			// duration
			td = AddCell(tr, detailLines[counter].Duration);
			td.style.textAlign = "center";
			// gross
			td = AddCell(tr, "");
			td.innerHTML = detailLines[counter].GrossRate.toString().formatCurrency();
			// direct income
			td = AddCell(tr, "");
			td.innerHTML = (detailLines[counter].GrossRate - detailLines[counter].NetRate).toString().formatCurrency();
			// net rate
			td = AddCell(tr, "");
			td.innerHTML = detailLines[counter].NetRate.toString().formatCurrency();
			// line value
			td = AddCell(tr, "");
			td.innerHTML = detailLines[counter].LineValue.toString().formatCurrency();
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function btnStatement_OnClick() {
	window.open("ViewStatement.aspx" + document.location.search);
}

