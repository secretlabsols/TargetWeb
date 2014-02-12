
var lookupSvc, currentPage;
var ClientSubGroupSelector_selectedClientSubGroupID;
var listFilter, listFilterReference = "", listFilterName = "";
var tblClientSubGroups, divPagingLinks;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	tblClientSubGroups = GetElement("tblClientSubGroups");
	divPagingLinks = GetElement("ClientSubGroup_PagingLinks");
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
	listFilter.AddColumn("Reference", GetElement("thRef"));
		
	// populate table
	FetchClientSubGroupList(currentPage, ClientSubGroupSelector_selectedClientSubGroupID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Name":
			listFilterName = column.Filter;
			break;
		case "Reference":
			listFilterReference = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchClientSubGroupList(1, 0);
}

function FetchClientSubGroupList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	
	lookupSvc.FetchClientSubGroupList(page, selectedID, listFilterReference, listFilterName, FetchClientSubGroupList_Callback)
}

function FetchClientSubGroupList_Callback(response) {
	var clientSubGroups, index;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		// populate the table
		clientSubGroups = response.value.ClientSubGroups;

		// remove existing rows
		ClearTable(tblClientSubGroups);
		for(index=0; index<clientSubGroups.length; index++) {
		
			tr = AddRow(tblClientSubGroups);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ClientSubGroupSelect", clientSubGroups[index].ID, RadioButton_Click);
			
			str = clientSubGroups[index].Name;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = clientSubGroups[index].Reference;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the team?
			if(ClientSubGroupSelector_selectedClientSubGroupID == clientSubGroups[index].ID || (currentPage == 1 && clientSubGroups.length == 1)) {
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
	for (index = 0; index < tblClientSubGroups.tBodies[0].rows.length; index++){
		rdo = tblClientSubGroups.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblClientSubGroups.tBodies[0].rows[index];
			tblClientSubGroups.tBodies[0].rows[index].className = "highlightedRow"
			ClientSubGroupSelector_selectedClientSubGroupID = rdo.value;
		} else {
			tblClientSubGroups.tBodies[0].rows[index].className = ""
		}
	}
	if(typeof ClientSubGroupSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = ClientSubGroupSelector_selectedClientSubGroupID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);
		ClientSubGroupSelector_SelectedItemChange(args);
	}
}

addEvent(window, "load", Init);
