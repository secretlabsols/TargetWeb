
var lookupSvc, currentPage;
var ClientGroupSelector_selectedClientGroupID;
var listFilter, listFilterReference = "", listFilterName = "";
var tblClientGroups, divPagingLinks;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	tblClientGroups = GetElement("tblClientGroups");
	divPagingLinks = GetElement("ClientGroup_PagingLinks");
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
	listFilter.AddColumn("Reference", GetElement("thRef"));
		
	// populate table
	FetchClientGroupList(currentPage, ClientGroupSelector_selectedClientGroupID);
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
	FetchClientGroupList(1, 0);
}

function FetchClientGroupList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	
	lookupSvc.FetchClientGroupList(page, selectedID, listFilterReference, listFilterName, FetchClientGroupList_Callback)
}

function FetchClientGroupList_Callback(response) {
	var clientGroups, index;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		// populate the table
		clientGroups = response.value.ClientGroups;

		// remove existing rows
		ClearTable(tblClientGroups);
		for(index=0; index<clientGroups.length; index++) {
		
			tr = AddRow(tblClientGroups);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "ClientGroupSelect", clientGroups[index].ID, RadioButton_Click);
			
			str = clientGroups[index].Name;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = clientGroups[index].Reference;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the team?
			if(ClientGroupSelector_selectedClientGroupID == clientGroups[index].ID || (currentPage == 1 && clientGroups.length == 1)) {
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
	for (index = 0; index < tblClientGroups.tBodies[0].rows.length; index++){
		rdo = tblClientGroups.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblClientGroups.tBodies[0].rows[index];
			tblClientGroups.tBodies[0].rows[index].className = "highlightedRow"
			ClientGroupSelector_selectedClientGroupID = rdo.value;
		} else {
			tblClientGroups.tBodies[0].rows[index].className = ""
		}
	}
	if(typeof ClientGroupSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = ClientGroupSelector_selectedClientGroupID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);
		ClientGroupSelector_SelectedItemChange(args);
	}
}

addEvent(window, "load", Init);
