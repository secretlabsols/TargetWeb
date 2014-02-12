var paymentsSvc, tblRemittances, divPagingLinks, currentPage, providerID, serviceID, dateFrom, dateTo, selectedRemittanceID, btnViewRemittance;
var tblRemittancesID = "ListRemittaances";
var divPagingLinksID = "Remittance_PagingLinks";
var btnViewRemittanceID = "btnViewRemittance";

function Init() {
	paymentsSvc = new Target.SP.Web.Apps.WebSvc.Payments_class();
	tblRemittances = GetElement(tblRemittancesID);
	divPagingLinks = GetElement(divPagingLinksID);
	btnViewRemittance = GetElement(btnViewRemittanceID);
	// populate table
	FetchRemittanceList(currentPage);
}

/* FETCH REMITTANCE LIST METHODS */
function FetchRemittanceList(page) {
	currentPage = page;
	DisplayLoading(true);
	btnViewRemittance.disabled = true;
	paymentsSvc.FetchRemittanceList(page, providerID, serviceID, dateFrom, dateTo, FetchRemittanceList_Callback)
}
function FetchRemittanceList_Callback(response) {
	var remittances, remCounter, tr, td, link, radioButton;
	var viewUrl = "ViewRemittance.aspx?id=";
	
	if(CheckAjaxResponse(response, paymentsSvc.url)) {
		remittances = response.value.Remittances;
		
		btnViewRemittance.disabled = (remittances == 0);
		
		ClearTable(tblRemittances);
		for(remCounter=0; remCounter<remittances.length; remCounter++) {
			tr = AddRow(tblRemittances);
			// selector
			td = AddCell(tr, "");
			radioButton = AddRadio(td, "", "RemittanceSelect", remittances[remCounter].ID, rdoViewRemittance_OnClick);
			// reference
			td = AddCell(tr, "");
			link = AddLink(td, remittances[remCounter].Reference, viewUrl + remittances[remCounter].ID, "Click here to view this remittance.");
			link.setAttribute("target", "_blank");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// service ref and name
			td = AddCell(tr, "");
			link = AddLink(td, remittances[remCounter].ServiceRef + ": " + remittances[remCounter].ServiceName, viewUrl + remittances[remCounter].ID, "Click here to view this remittance.");
			link.setAttribute("target", "_blank");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// dates
			AddCell(tr, Date.strftime("%d/%m/%Y", remittances[remCounter].DateFrom));
			AddCell(tr, Date.strftime("%d/%m/%Y", remittances[remCounter].DateTo));
			// amount
			td = AddCell(tr, "");
			td.innerHTML = remittances[remCounter].Amount.toString().formatCurrency();
			// create date
			AddCell(tr, Date.strftime("%d/%m/%Y", remittances[remCounter].CreatedDate));
			// select the remittance?
			if(remittances.length == 1) {
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
}

function btnViewRemittance_Click() {
	if(selectedRemittanceID == undefined)
		alert("Please select a remittance.");
	else
		window.open("ViewRemittance.aspx?id=" + selectedRemittanceID);
}