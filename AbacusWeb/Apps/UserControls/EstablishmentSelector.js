
var estabSvc, currentPage, selectedEstablishmentID, EstablishmentStep_required, establishmentSelector_mode, estabSelector_Redundant;
var listFilter, listFilterReference = "", listFilterName = "";
var tblEstablishments, divPagingLinks, btnNext, btnFinish;

function Init() {
	estabSvc = new Target.Abacus.Web.Apps.WebSvc.Establishments_class();
	tblEstablishments = GetElement("tblEstablishments");
	divPagingLinks = GetElement("Establishment_PagingLinks");
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Reference", GetElement("thRef"));
	listFilter.AddColumn("Name", GetElement("thName"));
	
	// populate table
	FetchEstabList(currentPage, selectedEstablishmentID);
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
	FetchEstabList(1, 0);
}

/* FETCH ESTABLISHMENT LIST METHODS */
function FetchEstabList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;

	switch (establishmentSelector_mode) {
	    case 1: // All Establishments
	        estabSvc.FetchProviderList(page, selectedID, listFilterReference, listFilterName, estabSelector_Redundant, FetchEstabList_Callback)
	        break;
		case 3: // Dom Providers
			estabSvc.FetchDomProviderList(page, selectedID, listFilterReference, listFilterName, estabSelector_Redundant, FetchEstabList_Callback)
			break;
		case 4: // Dom Providers
			estabSvc.FetchDomProviderList(page, selectedID, listFilterReference, listFilterName, estabSelector_Redundant, FetchEstabList_Callback)
			break;
		default:
			alert("Invalid establishmentSelector_mode value (" + establishmentSelector_mode + ").");
			break;
	}	
}

function FetchEstabList_Callback(response) {
	var establishments, estabCounter;
	var tr, td, radioButton;
	var str;
	var link;

	// disable next/finish buttons by default if the step is mandatory
	if (EstablishmentStep_required) {
	    if (btnNext) btnNext.disabled = true;
	    if (btnFinish) btnFinish.disabled = true;
	}
		
	if(CheckAjaxResponse(response, estabSvc.url)) {
		
		// populate the conversation table
		establishments = response.value.Establishments;
		
		// remove existing rows
		ClearTable(tblEstablishments);
		for(estabCounter=0; estabCounter<establishments.length; estabCounter++) {
		
			tr = AddRow(tblEstablishments);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "EstablishmentSelect", establishments[estabCounter].ID, RadioButton_Click);
			
			str = establishments[estabCounter].Reference;
			if (!str || str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, establishments[estabCounter].Name);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			str = establishments[estabCounter].Address
			if (!str || str.length == 0) str = " ";
			td = AddCell(tr, str.replace(/\r\n/g, ", "));
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			// select the establishment?
			if(selectedEstablishmentID == establishments[estabCounter].ID || ( currentPage == 1 && establishments.length == 1)) {
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
	
	for (index = 0; index < tblEstablishments.tBodies[0].rows.length; index++){
		rdo = tblEstablishments.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblEstablishments.tBodies[0].rows[index];
			tblEstablishments.tBodies[0].rows[index].className = "highlightedRow"
			selectedEstablishmentID = rdo.value;
		} else {
			tblEstablishments.tBodies[0].rows[index].className = ""
		}
	}
	if(EstablishmentStep_required) {
		if(btnNext) btnNext.disabled = false;
		if(btnFinish) btnFinish.disabled = false;
	}
	if(typeof EstablishmentSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = selectedEstablishmentID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);;
		EstablishmentSelector_SelectedItemChange(args);
	}

}

function GetSelectedEstablishment() {
	var index, rdo;
	
	for (index = 0; index < tblEstablishments.tBodies[0].rows.length; index++){
		rdo = tblEstablishments.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo)
			if (rdo.checked) {
				return rdo.value;
			}
	}
	return 0;
}

function EstablishmentStep_BeforeNavigate() {
	var originalEstablishmentID = GetQSParam(document.location.search, "estabID");
	var establishmentID = GetSelectedEstablishment();
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	// establishment is required?
	if(EstablishmentStep_required && establishmentID == 0) {
		alert("Please select a provider.");
		return false;
	}
	
	// if the selected establishment has changed, blank out the client ID
	if(originalEstablishmentID != establishmentID) url = RemoveQSParam(url, "clientID");
	
	url = AddQSParam(RemoveQSParam(url, "estabID"), "estabID", establishmentID);
	SelectorWizard_newUrl = url;
	return true;
}

if(typeof EstablishmentSelector_GetBackUrl != "function") {
	EstablishmentSelector_GetBackUrl = function() {
		// not used but still needs to be defined
		return "";
	}
}

function EstablishmentSelector_MruOnChange(mruListKey, selectedValue) {
    if(selectedValue.length > 0) {
        var url = document.location.href;
        url = RemoveQSParam(url, "estabID");
        url = AddQSParam(url, "estabID", selectedValue);
        document.location.href = url;
    }
}

addEvent(window, "load", Init);
