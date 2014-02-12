var ProvSvc, tblProvs, divPagingLinks, currentPage, allowViewProvider, selectedProviderID;
var btnNext, btnFinish;
var ProviderStep_required;
var listFilter, listFilterReference = "", listFilterName = "";

var tblProvsID = "ListProvider";
var divPagingLinksID = "Provider_PagingLinks";

function Init() {
	ProvSvc = new Target.SP.Web.Apps.WebSvc.Providers_class();
	tblProvs = GetElement(tblProvsID);
	divPagingLinks = GetElement(divPagingLinksID);
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Reference", GetElement("thRef"));
	listFilter.AddColumn("Name", GetElement("thName"));
	
	// populate table
	FetchProvList(currentPage, selectedProviderID);
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
	FetchProvList(1, 0);
}

/* FETCH PROVIDER LIST METHODS */
function FetchProvList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	ProvSvc.FetchProviderList(page, selectedID, listFilterReference, listFilterName, FetchProvList_Callback)
}
function FetchProvList_Callback(response) {
	var Providers, ProvCounter;
	var tr, td, radioButton;
	var viewUrl;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, ProvSvc.url)) {
		// disable next/finish buttons by default if the step is mandatory
		if(ProviderStep_required) {
			if(btnNext) btnNext.disabled = true;
			if(btnFinish) btnFinish.disabled = true;
		}
		
		// populate the conversation table
		Providers = response.value.Providers;
		
		// remove existing rows
		ClearTable(tblProvs);
		for(ProvCounter=0; ProvCounter<Providers.length; ProvCounter++) {
		
			// build the View Conv Url to include the current Url for its Back button
			viewUrl = "ViewProvider.aspx?backUrl=" + ProviderSelector_GetBackUrl(Providers[ProvCounter].ProviderID) + "&id=";
		
			tr = AddRow(tblProvs);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ProviderSelect", Providers[ProvCounter].ProviderID, RadioButton_Click);
			
			if(allowViewProvider) {
				td = AddCell(tr, "");
				link = AddLink(td, Providers[ProvCounter].Reference, viewUrl + Providers[ProvCounter].ProviderID, "Click here to view this providers details");
				link.className = "transBg"
			} else {
				td = AddCell(tr, Providers[ProvCounter].Reference);
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			if(allowViewProvider) {
				td = AddCell(tr, "");
				link = AddLink(td, Providers[ProvCounter].Name, viewUrl + Providers[ProvCounter].ProviderID, "Click here to view this providers details");
				link.className = "transBg"
			} else {
				td = AddCell(tr, Providers[ProvCounter].Name);
			}
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			str = Providers[ProvCounter].Address
			if (!str || str.length == 0) str = " ";
			td = AddCell(tr, str.replace(/\r\n/g, ", "));
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			str = Providers[ProvCounter].PostCode
			if (!str || str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// select the provider?
			if(selectedProviderID == Providers[ProvCounter].ProviderID || Providers.length == 1) {
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
	for (x = 0; x < tblProvs.tBodies[0].rows.length; x++){
		Radio = tblProvs.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblProvs.tBodies[0].rows[x].className = "highlightedRow"
			selectedProviderID = Radio.value;
		} else {
			tblProvs.tBodies[0].rows[x].className = ""
		}
	}
	if(ProviderStep_required) {
		if(btnNext) btnNext.disabled = false;
		if(btnFinish) btnFinish.disabled = false;
	}
}

function viewProviderButton_Click() {
	var x
	var Radio
	var itemFound
	var Url
	var viewUrl
	var value
	
	itemFound = false
	
	// build the View Conv Url to include the current Url for its Back button
	viewUrl = "ViewProvider.aspx?backUrl=" + ProviderSelector_GetBackUrl(selectedProviderID) + "&id=";
	//loop round items in the table finding a selected item, if an item is found
	//we used the ProviderID stored in the value element of the radio button
	//to build the URL.
	value = GetSelectedProvider();
	if(value > 0) {
		Url = viewUrl + value;
		itemFound = true
	}
	
	if (itemFound) {
		window.location.href = Url
	} else {
		alert("Please select a provider.")
	}
}

function GetSelectedProvider() {
	for (x = 0; x < tblProvs.tBodies[0].rows.length; x++){
		Radio = tblProvs.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio)
			if (Radio.checked) {
				return Radio.value;
			}
	}
	return 0;
}

/* PROVIDER SELECTOR WIZARD STEP */
function ProviderStep_BeforeNavigate() {
	var originalProviderID = GetQSParam(document.location.search, "providerID");
	var providerID = GetSelectedProvider();
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	// provider is required?
	if(ProviderStep_required && providerID == 0) {
		alert("Please select a provider.");
		return false;
	}
	
	// if the selected provider has changed, blank out the service ID
	if(originalProviderID != providerID) url = RemoveQSParam(url, "serviceID");
	
	url = AddQSParam(RemoveQSParam(url, "providerID"), "providerID", providerID);
	SelectorWizard_newUrl = url;
	return true;
}
if(typeof ProviderSelector_GetBackUrl != "function") {
	ProviderSelector_GetBackUrl = function() {
		// not used but still needs to be defined
		return "";
	}
}
