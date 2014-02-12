var clientSvc, tblClients, divPagingLinks, currentPage, providerID, serviceID, selectedClientID;
var btnNext, btnFinish;
var listFilter, listFilterReference = "", listFilterName = "";

var tblClientsID = "ListClient";
var divPagingLinksID = "Client_PagingLinks";

function Init() {
	clientSvc = new Target.SP.Web.Apps.WebSvc.Clients_class();
	tblClients = GetElement(tblClientsID);
	divPagingLinks = GetElement(divPagingLinksID);
	serviceID = parseInt(GetQSParam(document.location.search, "serviceID"), 10);
	providerID = parseInt(GetQSParam(document.location.search, "providerID"), 10);
	btnNext = GetElement("SelectorWizard1_btnNext");
	btnFinish = GetElement("SelectorWizard1_btnFinish");
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Reference", GetElement("thRef"));
	listFilter.AddColumn("Name", GetElement("thName"));

	// populate table
	FetchClientList(currentPage, selectedClientID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Reference":
			listFilterReference = column.Filter;
			break;
		case "Name":
			listFilterName = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchClientList(1, 0);
}

/* FETCH CLIENT LIST METHODS */
function FetchClientList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	clientSvc.FetchClientsInServiceList(page, providerID, serviceID, selectedID, listFilterReference, listFilterName, FetchClientList_Callback)
}
function FetchClientList_Callback(response) {
	var clients, clientCounter;
	var tr, td, radioButton, str;
		
	if(CheckAjaxResponse(response, clientSvc.url)) {
	
		// disable next/finish buttons by default if the step is mandatory
		if(ClientStep_required) {
			if(btnNext) btnNext.disabled = true;
			if(btnFinish) btnFinish.disabled = true;
		}
			
		// populate the client table
		clients = response.value.Clients;
				
		// remove existing rows
		ClearTable(tblClients);
		for(clientCounter=0; clientCounter<clients.length; clientCounter++) {
			tr = AddRow(tblClients);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ClientSelect", clients[clientCounter].ID, RadioButton_Click);
			
			td = AddCell(tr, clients[clientCounter].Reference);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, clients[clientCounter].Name);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = clients[clientCounter].NINO
			if(!str) str = " ";
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			AddCell(tr, Date.strftime("%d/%m/%Y", clients[clientCounter].DoB));
			
			// select the client?
			if(selectedClientID == clients[clientCounter].ID || clients.length == 1) {
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
	for (x = 0; x < tblClients.tBodies[0].rows.length; x++){
		Radio = tblClients.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblClients.tBodies[0].rows[x].className = "highlightedRow"
			selectedClientID = Radio.value;
		} else {
			tblClients.tBodies[0].rows[x].className = ""
		}
	}
	if(ClientStep_required) {
		if(btnNext) btnNext.disabled = false;
		if(btnFinish) btnFinish.disabled = false;
	}
}

function GetSelectedClient() {
	for (x = 0; x < tblClients.tBodies[0].rows.length; x++){
		Radio = tblClients.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio)
			if (Radio.checked) {
				return Radio.value;
			}
	}
	return 0;
}

/* CLIENT SELECTOR WIZARD STEP */
function ClientStep_BeforeNavigate() {
	var clientID = GetSelectedClient();
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	// client is required?
	if(ClientStep_required && clientID == 0) {
		alert("Please select a service user.");
		return false;
	}
	
	url = AddQSParam(RemoveQSParam(url, "clientID"), "clientID", clientID);
	SelectorWizard_newUrl = url;
	return true;
}
