var occupancySvc, currentPage, clientID, establishmentID, dateFrom, dateTo, movement;
var tblOccupancy, divPagingLinks, btnViewServiceUser;

function Init() {
	occupancySvc = new Target.Abacus.Extranet.Apps.WebSvc.Occupancy_class();
	tblOccupancy = GetElement("ListOccupancy");
	divPagingLinks = GetElement("Detail_PagingLinks");
	btnViewServiceUser = GetElement("btnViewServiceUser");
	
	// populate table
	FetchOccupancyList(currentPage);
}

/* FETCH OCCUPANCY LIST METHODS */
function FetchOccupancyList(page) {
	currentPage = page;
	DisplayLoading(true);
	btnViewServiceUser.disabled = true;
	occupancySvc.FetchResOccupancyList(page, establishmentID, dateFrom, dateTo, movement, FetchOccupancyList_Callback)
}

function FetchOccupancyList_Callback(response) {
	var occupancy, remCounter, tr, td, link, radioButton;
	var str;
	var viewUrl = "ViewServiceUser.aspx?id=";

	btnViewServiceUser.disabled = true;
	
	if(CheckAjaxResponse(response, occupancySvc.url)) {
		occupancy = response.value.Occupancy;
		ClearTable(tblOccupancy);
		for(remCounter=0; remCounter<occupancy.length; remCounter++) {
			tr = AddRow(tblOccupancy);
			
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ClientsSelect", occupancy[remCounter].ID, RadioButton_Click);
			
			td = AddCell(tr, "");
			link = AddLink(td, occupancy[remCounter].Reference, viewUrl + occupancy[remCounter].ID, "Click here to view this service user.");
			link.className = "transBg";
			
			td = AddCell(tr, "");
			link = AddLink(td, occupancy[remCounter].Name, viewUrl + occupancy[remCounter].ID, "Click here to view this service user.");
			link.className = "transBg";
			
			AddCell(tr, Date.strftime("%d/%m/%Y", occupancy[remCounter].DateFrom));
			td = AddCell(tr, " ");
			if(Date.strftime("%d/%m/%Y", occupancy[remCounter].DateTo) == "31/12/9999") {
			    td.innerHTML = "(open ended)";
			} else {
			    td.innerHTML = Date.strftime("%d/%m/%Y", occupancy[remCounter].DateTo);
			}

			if (clientID == occupancy[remCounter].ID || (currentPage == 1 && occupancy.length == 1)) {
			    radioButton.click();
			}			 
			
		}		
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var x
	var Radio
	for (x = 0; x < tblOccupancy.tBodies[0].rows.length; x++){
		Radio = tblOccupancy.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblOccupancy.tBodies[0].rows[x].className = "highlightedRow"
			clientID = Radio.value;
			btnViewServiceUser.disabled = false;
		} else {
			tblOccupancy.tBodies[0].rows[x].className = ""
		}
	}
}

function btnViewServiceUser_Click() {
	if(clientID == 0)
		alert("Please select a service User.");
	else
		document.location.href = "ViewServiceUser.aspx?id=" + clientID;
}
