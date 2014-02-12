
var paymentsSvc, currentPage, establishmentID, dateFrom, dateTo, selectedRemittanceID;
var tblRemittances, divPagingLinks, btnViewRemittance;

function Init() {
	paymentsSvc = new Target.Abacus.Extranet.Apps.WebSvc.ResPayments_class();
	tblRemittances = GetElement("ListRemittances");
	divPagingLinks = GetElement("Remittance_PagingLinks");
	btnViewRemittance = GetElement("btnViewRemittance");
	// populate table
	FetchRemittanceList(currentPage);
}

/* FETCH REMITTANCE LIST METHODS */
function FetchRemittanceList(page) {
	currentPage = page;
	DisplayLoading(true);
	btnViewRemittance.disabled = true;
	paymentsSvc.FetchRemittanceList(page, establishmentID, dateFrom, dateTo, FetchRemittanceList_Callback)
}

function FetchRemittanceList_Callback(response) {
	var remittances, remCounter, tr, td, link, radioButton;
	var viewUrl = "ViewRemittance.aspx?id=";
	var str;

	btnViewRemittance.disabled = true;
	
	if(CheckAjaxResponse(response, paymentsSvc.url)) {
		remittances = response.value.Remittances;
		
		ClearTable(tblRemittances);
		for(remCounter=0; remCounter<remittances.length; remCounter++) {
			tr = AddRow(tblRemittances);
			// selector
			td = AddCell(tr, "");
			radioButton = AddRadio(td, "", "RemittanceSelect", remittances[remCounter].ID, rdoViewRemittance_OnClick);
			// reference
			td = AddCell(tr, "");
			link = AddLink(td, remittances[remCounter].RemittanceNumber, viewUrl + remittances[remCounter].ID, "Click here to view this remittance.");
			link.setAttribute("target", "_blank");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// home ref and name
			td = AddCell(tr, "");
			str = remittances[remCounter].HomeRef;
			if(str.length > 0) str += " / ";
			str += remittances[remCounter].HomeName;
			link = AddLink(td, str, viewUrl + remittances[remCounter].ID, "Click here to view this remittance.");
			link.setAttribute("target", "_blank");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// dates
			AddCell(tr, Date.strftime("%d/%m/%Y", remittances[remCounter].DateFrom));
			AddCell(tr, Date.strftime("%d/%m/%Y", remittances[remCounter].DateTo));
			// amount
			td = AddCell(tr, "");
			td.innerHTML = remittances[remCounter].AmountPaid.toString().formatCurrency();
			// paid date
			AddCell(tr, Date.strftime("%d/%m/%Y", remittances[remCounter].PaidDate));
			// select the remittance?
			if (selectedRemittanceID == remittances[remCounter].ID || (currentPage == 1 && remittances.length == 1)) {
				radioButton.click();
			}
		}		
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function rdoViewRemittance_OnClick() {
	var x
	var Radio
	for (x = 0; x < tblRemittances.tBodies[0].rows.length; x++){
		Radio = tblRemittances.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblRemittances.tBodies[0].rows[x].className = "highlightedRow"
			selectedRemittanceID = Radio.value;
		} else {
			tblRemittances.tBodies[0].rows[x].className = ""
		}
	}
	btnViewRemittance.disabled = false;
}

function btnViewRemittance_Click() {
	if(selectedRemittanceID == undefined)
		alert("Please select a remittance.");
	else
		window.open("ViewRemittance.aspx?id=" + selectedRemittanceID);
}



