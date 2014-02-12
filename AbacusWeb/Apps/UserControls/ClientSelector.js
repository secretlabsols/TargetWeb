
var clientSvc, currentPage;
var ClientSelector_selectedClientID, ClientStep_required;
var listFilter, listFilterReference = "", listFilterName = "", listFilterDebtorNumber = "", listFilterCreditorReference = "";
var tblClients, divPagingLinks, btnNext, btnFinish, btnView, btnNewEnquiry, btnBudgetPeriods, btnViewID, btnBudgetPeriodsID;
var hide_DebtorRef, hide_CreditorRef;

function Init() {
	clientSvc = new Target.Abacus.Web.Apps.WebSvc.Clients_class();
	tblClients = GetElement("tblClients");
	divPagingLinks = GetElement("Client_PagingLinks");
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	btnView = GetElement(btnViewID, true);
	btnBudgetPeriods = GetElement(btnBudgetPeriodsID, true);
	
	if (btnView) btnView.disabled = true;
	if (btnBudgetPeriods) btnBudgetPeriods.disabled = true;
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Reference", GetElement("thRef"));
	listFilter.AddColumn("Name", GetElement("thName"));
	if (!hide_DebtorRef) {
	    listFilter.AddColumn("Debtor Ref", GetElement("thDebtorRef"));
	}
	if (!hide_CreditorRef) {
	    listFilter.AddColumn("Creditor Ref", GetElement("thCreditorRef"));
	}

	// populate table
	FetchClientList(currentPage, ClientSelector_selectedClientID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Reference":
			listFilterReference = column.Filter;
			break;
		case "Name":
			listFilterName = column.Filter;
			break;
		case "Debtor Ref":
		    if (!hide_DebtorRef){
			    listFilterDebtorNumber = column.Filter;
			}
			break;
		case "Creditor Ref":
		    if (!hide_CreditorRef){
			    listFilterCreditorReference = column.Filter;
			}
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
	
	clientSvc.FetchClientList(page, selectedID, listFilterReference, listFilterName, listFilterDebtorNumber, listFilterCreditorReference, FetchClientList_Callback)
}

function FetchClientList_Callback(response) {
	var clients, clientCounter;
	var tr, td, radioButton;
	var str;
	var link;
	
    // disable next/finish buttons by default if the step is mandatory
    if (ClientStep_required) {
        if (btnNext) btnNext.disabled = true;
        if (btnFinish) btnFinish.disabled = true;
    }
    if (btnView) btnView.disabled = true;
    if (btnBudgetPeriods) btnBudgetPeriods.disabled = true;
    
	if(CheckAjaxResponse(response, clientSvc.url)) {
	    
	    // populate the table
		clients = response.value.Clients;

		// remove existing rows
		ClearTable(tblClients);
		for(clientCounter=0; clientCounter<clients.length; clientCounter++) {
		
			tr = AddRow(tblClients);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ClientsSelect", clients[clientCounter].ID, RadioButton_Click);

			if (btnView) {
			    td = AddCell(tr, "");
			    link = AddLink(td, clients[clientCounter].Reference, ClientSelector_GetViewURL(clients[clientCounter].ID), "Click here to view this service user.");
			    link.className = "transBg";
			} else {
			    td = AddCell(tr, clients[clientCounter].Reference);
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, clients[clientCounter].Name);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			AddCell(tr, Date.strftime("%d/%m/%Y", clients[clientCounter].BirthDate, false));
			
			str = clients[clientCounter].Address;
			if(!str || str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			if (!hide_DebtorRef) {
			    str = clients[clientCounter].DebtorNumber;
			    if(!str || str.length == 0) str = " ";
			    td = AddCell(tr, str);
			    td.style.textOverflow = "ellipsis";
			    td.style.overflow = "hidden";
			}

			if (!hide_CreditorRef) {
			    str = clients[clientCounter].CreditorReference;
			    if(!str || str.length == 0) str = " ";
			    td = AddCell(tr, str);
			    td.style.textOverflow = "ellipsis";
			    td.style.overflow = "hidden";
            }

						
			// select the client?
			if(ClientSelector_selectedClientID == clients[clientCounter].ID || ( currentPage == 1 && clients.length == 1)) {
				radioButton.click();
			}			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
    }
	DisplayLoading(false);
}

function RadioButton_Click() {
	var index, rdo, selectedRow;
	for (index = 0; index < tblClients.tBodies[0].rows.length; index++){
		rdo = tblClients.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblClients.tBodies[0].rows[index];
			tblClients.tBodies[0].rows[index].className = "highlightedRow"
			ClientSelector_selectedClientID = rdo.value;
			if (btnView) btnView.disabled = false;
			if (btnBudgetPeriods) btnBudgetPeriods.disabled = false;
		} else {
			tblClients.tBodies[0].rows[index].className = ""
		}
	}
	if(ClientStep_required) {
		if(btnNext) btnNext.disabled = false;
		if(btnFinish) btnFinish.disabled = false;
	}
	if(typeof ClientSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = ClientSelector_selectedClientID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);;
		ClientSelector_SelectedItemChange(args);
	}
}

function ClientStep_BeforeNavigate() {
	var originalClientID = GetQSParam(document.location.search, "clientID");
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	// client is required?
	if(ClientStep_required && ClientSelector_selectedClientID == 0) {
		alert("Please select a service user.");
		return false;
	}

	// if the selected client has changed, blank out the budget holder ID
	if (originalClientID != ClientSelector_selectedClientID) url = RemoveQSParam(url, "bhID");

	url = AddQSParam(RemoveQSParam(url, "clientID"), "clientID", ClientSelector_selectedClientID);
	SelectorWizard_newUrl = url;
	return true;
}

function ClientSelector_MruOnChange(mruListKey, selectedValue) {
    if(selectedValue.length > 0) {
        var url = document.location.href;
        url = RemoveQSParam(url, "clientID");
        url = RemoveQSParam(url, "bhID");
        url = AddQSParam(url, "clientID", selectedValue);
        document.location.href = url;
    }
}

function btnNew_Click() {
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");
    document.location.href = "../Enquiry/Edit.aspx" + qs + "&mode=2&backUrl=" + GetBackUrl();
}

function btnView_Click() {
    // mode=1 means Fetched
    document.location.href = ClientSelector_GetViewURL(ClientSelector_selectedClientID);
}

function btnViewBudgetPeriods_Click() {
    // mode=1 means Fetched
    document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Admin/ServiceUsers/ServiceUserBudgetPeriods.aspx?clientid=" + ClientSelector_selectedClientID + "&mode=1&backUrl=" + GetBackUrl();
}

function GetBackUrl(clientID) {
    if (!clientID) clientID = ClientSelector_selectedClientID;
    var url = document.location.href;
    url = RemoveQSParam(url, "clientid");
    url = AddQSParam(url, "clientid", clientID);
    
    return escape(url);
}

function ClientSelector_GetViewURL(id) {
    return "../Enquiry/Edit.aspx?clientid=" + id + "&mode=1&backUrl=" + GetBackUrl(id);
}

addEvent(window, "load", Init);
