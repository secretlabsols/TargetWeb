
var clientSvc, currentPage ;
var ClientStep_clientID, ClientStep_establishmentID, ClientStep_required, ClientStep_mode, ClientStep_dateFrom, ClientStep_dateTo, ClientStep_viewBaseUrl, ClientStep_contractID, InPlaceClient;
var listFilter, listFilterReference = "", listFilterName = "";
var tblClients, divPagingLinks, btnNext, btnFinish, btnViewServiceUser;
var pScheduleId, showServiceOrderWithValidPeriod;

function Init() {
	clientSvc = new Target.Abacus.Extranet.Apps.WebSvc.Clients_class();
	tblClients = GetElement("tblClients");
	divPagingLinks = GetElement("Client_PagingLinks");
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	btnViewServiceUser = GetElement("btnViewServiceUser");
	
	if(ClientStep_viewBaseUrl.length > 0) btnViewServiceUser.style.display = "block";
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Reference", GetElement("thRef"));
	listFilter.AddColumn("Name", GetElement("thName"));
	
	// populate table
	FetchClientList(currentPage, ClientStep_clientID);
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
	if(ClientStep_viewBaseUrl.length > 0) btnViewServiceUser.disabled = true;

	switch(ClientStep_mode) {
	    case Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ResidentialClients: // Res
	        clientSvc.FetchResClientList(page, ClientStep_establishmentID, selectedID, listFilterReference, listFilterName, ClientStep_dateFrom, ClientStep_dateTo, FetchClientList_Callback)
	        break;
	    case Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithDomSvcOrders: // dom svc orders
	        if (showServiceOrderWithValidPeriod) {
	            clientSvc.FetchValidDomSvcOrderClientList(page, ClientStep_establishmentID, ClientStep_contractID, selectedID, listFilterReference, listFilterName, ClientStep_dateFrom, ClientStep_dateTo, pScheduleId, FetchClientList_Callback)
	        }
	        else {
	            clientSvc.FetchDomSvcOrderClientList(page, ClientStep_establishmentID, ClientStep_contractID, selectedID, listFilterReference, listFilterName, ClientStep_dateFrom, ClientStep_dateTo, FetchClientList_Callback)
	        }
	        break;
        case Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithDomProviderInvoices:
		    clientSvc.FetchDomProviderInvoiceClientList(page, ClientStep_establishmentID, ClientStep_contractID, selectedID, listFilterReference, listFilterName, 0, FetchClientList_Callback)
		    break;
		case Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithVisitBasedDomProviderInvoices:
		    clientSvc.FetchDomProviderInvoiceClientList(page, ClientStep_establishmentID, ClientStep_contractID, selectedID, listFilterReference, listFilterName, 1, FetchClientList_Callback)
		    break;
		default:
			alert("Invalid ClientStep_mode value (" + ClientStep_mode + ").");
			break;
	}	
}

function FetchClientList_Callback(response) {
	var clients, clientCounter;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, clientSvc.url)) {
		// disable next/finish buttons by default if the step is mandatory
		if(ClientStep_required) {
			if(btnNext) btnNext.disabled = true;
			if(btnFinish) btnFinish.disabled = true;
		}
		
		// populate the conversation table
		clients = response.value.Clients;

		// remove existing rows
		ClearTable(tblClients);
		for(clientCounter=0; clientCounter<clients.length; clientCounter++) {
		
			tr = AddRow(tblClients);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ClientsSelect", clients[clientCounter].ID, RadioButton_Click);
			
			if(ClientStep_viewBaseUrl.length == 0) {
				td = AddCell(tr, clients[clientCounter].Reference);
			} else {
				td = AddCell(tr, "");
				link = AddLink(td, clients[clientCounter].Reference, ClientStep_viewBaseUrl + clients[clientCounter].ID, "Click here to view this service user.");
				link.className = "transBg";
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			if(ClientStep_viewBaseUrl.length == 0) {
				td = AddCell(tr, clients[clientCounter].Name);
			} else {
				td = AddCell(tr, "");
				link = AddLink(td, clients[clientCounter].Name, ClientStep_viewBaseUrl + clients[clientCounter].ID, "Click here to view this service user.");
				link.className = "transBg";
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			if(ClientStep_mode == Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithDomSvcOrders ||
			        ClientStep_mode == Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithDomProviderInvoices ||
			        ClientStep_mode == Target.Abacus.Extranet.Apps.UserControls.ClientStepMode.ClientsWithVisitBasedDomProviderInvoices) {
			    if(clients[clientCounter].Address)
			        str = clients[clientCounter].Address + " " + clients[clientCounter].Postcode;
			    else
			        str = " ";
			    td = AddCell(tr, str.replace(/\r\n/g, ", "));
			    td.style.textOverflow = "ellipsis";
			    td.style.overflow = "hidden";
            } else {
                str = clients[clientCounter].NINo;
			    if(!str || str.length == 0) str = " ";
			    AddCell(tr, str);
    		    AddCell(tr, Date.strftime("%d/%m/%Y", clients[clientCounter].BirthDate, false));
    		    AddCell(tr, Date.strftime("%d/%m/%Y", clients[clientCounter].DeathDate, true));			
            }
			
			// select the client?
			if(ClientStep_clientID == clients[clientCounter].ID || (currentPage == 1 && clients.length == 1)) {
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
			ClientStep_clientID = Radio.value;
			var args = [];            
			args[0] = ClientStep_clientID;
			args[1] = tblClients.tBodies[0].rows[x].cells[1].innerText;
			args[2] = tblClients.tBodies[0].rows[x].cells[2].innerText;
			if (InPlaceClient == "true") {
			    ClientSelector_SelectedItemChange(args);			
			}
		    
			if(ClientStep_viewBaseUrl.length > 0) btnViewServiceUser.disabled = false;
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

function ClientStep_BeforeNavigate() {
	var originalClientID = GetQSParam(document.location.search, "clientID");
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

if(typeof ClientSelector_GetBackUrl != "function") {
	ClientSelector_GetBackUrl = function() {
		// not used but still needs to be defined
		return "";
	}
}

function btnViewServiceUser_Click() {
	if(ClientStep_clientID == 0)
		alert("Please select a service User.");
	else
		document.location.href = ClientStep_viewBaseUrl + ClientStep_clientID;
}

addEvent(window, "load", Init);
