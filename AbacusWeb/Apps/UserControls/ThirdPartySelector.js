
var clientSvc, currentPage;
var ThirdPartySelector_selectedThirdPartyID, ThirdPartySelector_clientID, ClientStep_required;
var listFilter, listFilterName = "";
var tblThirdPartys, divPagingLinks, btnNext, btnFinish;

function Init() {
	clientSvc = new Target.Abacus.Web.Apps.WebSvc.Clients_class();
	tblThirdPartys = GetElement("tblTPs");
	divPagingLinks = GetElement("TP_PagingLinks");
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Surname", GetElement("thSurname"));
	// populate table
	FetchThirdPartyList(currentPage, ThirdPartySelector_clientID, ThirdPartySelector_selectedThirdPartyID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
//		case "Reference":
//			listFilterReference = column.Filter;
//			break;
		case "Surname":
			listFilterName = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchThirdPartyList(1, ThirdPartySelector_clientID,  0);
}

/* FETCH CLIENT LIST METHODS */
function FetchThirdPartyList(page, clientID, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	
	clientSvc.FetchThirdPartyList(page, clientID, selectedID, listFilterName, FetchTPList_Callback)
}

function FetchTPList_Callback(response) {
	var thirdPartys, thirdPartyCounter;
	var tr, td, radioButton;
	var str;
	var link;
	
	if(CheckAjaxResponse(response, clientSvc.url)) {
	    
	    // disable next/finish buttons by default if the step is mandatory
		if(ClientStep_required) {
			if(btnNext) btnNext.disabled = true;
			if(btnFinish) btnFinish.disabled = true;
		}
	
		// populate the Third Party table
		thirdPartys = response.value.ThirdPartys;

		// remove existing rows
		ClearTable(tblThirdPartys);
		for(thirdPartyCounter=0; thirdPartyCounter<thirdPartys.length; thirdPartyCounter++) {
		
			tr = AddRow(tblThirdPartys);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ThirdPartySelect", thirdPartys[thirdPartyCounter].ID, RadioButton_Click);
			
			str = thirdPartys[thirdPartyCounter].Title
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
			
//			td = AddCell(tr, thirdPartys[thirdPartyCounter].Title);
//			td.style.textOverflow = "ellipsis";
//			td.style.overflow = "hidden";
			
			str = thirdPartys[thirdPartyCounter].Surname;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
			
//			td = AddCell(tr, thirdPartys[thirdPartyCounter].Surname);
//			td.style.textOverflow = "ellipsis";
//			td.style.overflow = "hidden";
			
			
			str = thirdPartys[thirdPartyCounter].Address;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
						
			// select the client?
			if(ThirdPartySelector_selectedThirdPartyID == thirdPartys[thirdPartyCounter].ID || ( currentPage == 1 && thirdPartys.length == 1)) {
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
	for (index = 0; index < tblThirdPartys.tBodies[0].rows.length; index++){
		rdo = tblThirdPartys.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblThirdPartys.tBodies[0].rows[index];
			tblThirdPartys.tBodies[0].rows[index].className = "highlightedRow"
			ThirdPartySelector_selectedThirdPartyID = rdo.value;
		} else {
			tblThirdPartys.tBodies[0].rows[index].className = ""
		}
	}

	if(typeof ThirdPartySelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = ThirdPartySelector_selectedThirdPartyID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);;
		ThirdPartySelector_SelectedItemChange(args);
	}
}

//function ClientStep_BeforeNavigate() {
//	var originalClientID = GetQSParam(document.location.search, "clientID");
//	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
//	
//	// client is required?
//	if(ClientStep_required && ClientSelector_selectedClientID == 0) {
//		alert("Please select a service user.");
//		return false;
//	}
//	
//	url = AddQSParam(RemoveQSParam(url, "clientID"), "clientID", ClientSelector_selectedClientID);
//	SelectorWizard_newUrl = url;
//	return true;
//}

//function ClientSelector_MruOnChange(mruListKey, selectedValue) {
//    if(selectedValue.length > 0) {
//        var url = document.location.href;
//        url = RemoveQSParam(url, "clientID");
//        url = AddQSParam(url, "clientID", selectedValue);
//        document.location.href = url;
//    }
//}

addEvent(window, "load", Init);
