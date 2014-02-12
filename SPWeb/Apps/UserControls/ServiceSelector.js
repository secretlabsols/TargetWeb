var ServiceSvc, securitySvc, tblServices, divPagingLinks, currentPage, allowViewService, selectedServiceID;
var webSecurityCompanyID;
var tblServiceID = "ListService";
var divPagingLinksID = "Service_PagingLinks";
var btnNext, btnFinish;
var ServiceStep_required;
var listFilter, listFilterReference = "", listFilterName = "";

function Init() {
	ServiceSvc = new Target.SP.Web.Apps.WebSvc.Services_class();
	tblServices = GetElement(tblServiceID);
	divPagingLinks = GetElement(divPagingLinksID);
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Reference", GetElement("thRef"));
	listFilter.AddColumn("Name", GetElement("thName"));

	// populate table
	FetchServiceList(currentPage, selectedServiceID);
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
	FetchServiceList(1, 0);
}

function GetCurrentUrl() {
	// build the current Url
	return escape("ListServices.aspxpage=" + currentPage);
}

/* CONV LIST METHODS */
function FetchServiceList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	ServiceSvc.FetchServiceList(page, ProviderID, selectedID, listFilterReference, listFilterName, FetchServiceList_Callback)
}
function FetchServiceList_Callback(response) {
	var Services, ServiceCounter;
	var tr, td, radioButton;
	var viewUrl, litService;
	var str;
	var link;	
	
	if(CheckAjaxResponse(response, ServiceSvc.url)) {	
		// disable next/finish buttons by default if the step is mandatory
		if(ServiceStep_required) {
			if(btnNext) btnNext.disabled = true;
			if(btnFinish) btnFinish.disabled = true;
		}
		
		// populate the conversation table
		Services = response.value.Services;
				
		// remove existing rows
		ClearTable(tblServices);
		for(ServiceCounter=0; ServiceCounter<Services.length; ServiceCounter++) {
		
			// build the View Service Url to include the current Url for its Back button
			viewUrl = "ViewService.aspx?backUrl=" + ServiceSelector_GetBackUrl(Services[ServiceCounter].ServiceID) + "&id=";
		
			tr = AddRow(tblServices);
			tr.className = "ServiceList";
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ServiceSelect", Services[ServiceCounter].ServiceID, RadioButton_Click);
			
			if(allowViewService) {
				td = AddCell(tr, "");
				link = AddLink(td, Services[ServiceCounter].Reference, viewUrl + Services[ServiceCounter].ServiceID, "Click here to view this Service's details");
				link.className = "transBg"
			} else {
				td = AddCell(tr, Services[ServiceCounter].Reference);
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			if(allowViewService) {
				td = AddCell(tr, "");
				link = AddLink(td, Services[ServiceCounter].Name, viewUrl + Services[ServiceCounter].ServiceID, "Click here to view this Service's details");
				link.className = "transBg"
			} else {
				td = AddCell(tr, Services[ServiceCounter].Name);
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			str = Services[ServiceCounter].Description;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			str = Services[ServiceCounter].Type;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// select the service?
			if(selectedServiceID == Services[ServiceCounter].ServiceID || Services.length == 1) {
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
	for (x = 0; x < tblServices.tBodies[0].rows.length; x++){
		Radio = tblServices.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblServices.tBodies[0].rows[x].className = "highlightedRow"
			selectedServiceID = Radio.value;
		} else {
			tblServices.tBodies[0].rows[x].className = ""
		}
	}
	if(ServiceStep_required) {
		if(btnNext) btnNext.disabled = false;
		if(btnFinish) btnFinish.disabled = false;
	}
}

function viewServiceButton_Click() {
	var x
	var Radio
	var itemFound
	var Url
	var viewUrl
	var value
	
	itemFound = false
	
	// build the View Conv Url to include the current Url for its Back button
	viewUrl = "ViewService.aspx?backUrl=" + ServiceSelector_GetBackUrl(selectedServiceID) + "&id=";
	//loop round items in the table finding a selected item, if an item is found
	//we used the ServiceID stored in the value element of the radio button
	//to build the URL.
	value = GetSelectedService();
	if(value > 0) {
		Url = viewUrl + value;
		itemFound = true
	}
	
	if (itemFound) {
		window.location.href = Url
	} else {
		alert("Please select a service.")
	}
}

function GetSelectedService() {
	for (x = 0; x < tblServices.tBodies[0].rows.length; x++){
		Radio = tblServices.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio)
			if (Radio.checked) {
				return Radio.value;
			}
	}
	return 0;
}

/* SERVICE SELECTOR WIZARD STEP */
function ServiceStep_BeforeNavigate() {
	var originalServiceID = GetQSParam(document.location.search, "serviceID");
	var serviceID = GetSelectedService();
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	// service is required?
	if(ServiceStep_required && serviceID == 0) {
		alert("Please select a service.");
		return false;
	}
	
	// if the selected service has changed, blank out the client ID
	if(originalServiceID != serviceID) url = RemoveQSParam(url, "clientID");
	
	url = AddQSParam(RemoveQSParam(url, "serviceID"), "serviceID", serviceID);
	SelectorWizard_newUrl = url;
	return true;
}
if(typeof ServiceSelector_GetBackUrl != "function") {
	ServiceSelector_GetBackUrl = function() {
		// not used but still needs to be defined
		return "";
	}
}