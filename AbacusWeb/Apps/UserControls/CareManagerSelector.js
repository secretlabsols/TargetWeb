
var lookupSvc, currentPage;
var CareManagerSelector_selectedCmID;
var listFilter, listFilterReference = "", listFilterName = "";
var tblCMs, divPagingLinks;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	tblCMs = GetElement("tblCMs");
	divPagingLinks = GetElement("CareManager_PagingLinks");
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
	listFilter.AddColumn("Reference", GetElement("thRef"));
		
	// populate table
	FetchCareManagerList(currentPage, CareManagerSelector_selectedCmID);
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
	FetchCareManagerList(1, 0);
}

function FetchCareManagerList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	
	lookupSvc.FetchCareManagerList(page, selectedID, listFilterReference, listFilterName, FetchCareManagerList_Callback)
}

function FetchCareManagerList_Callback(response) {
	var careManagers, index;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		// populate the table
		careManagers = response.value.CareManagers;

		// remove existing rows
		ClearTable(tblCMs);
		for(index=0; index<careManagers.length; index++) {
		
			tr = AddRow(tblCMs);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "CareManagerSelect", careManagers[index].ID, RadioButton_Click);
			
			str = careManagers[index].Name;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = careManagers[index].Reference;
			if(!str || str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the pct?
			if(CareManagerSelector_selectedCmID == careManagers[index].ID ||  (currentPage == 1 && careManagers.length == 1)) {
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
	for (index = 0; index < tblCMs.tBodies[0].rows.length; index++){
		rdo = tblCMs.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblCMs.tBodies[0].rows[index];
			tblCMs.tBodies[0].rows[index].className = "highlightedRow"
			CareManagerSelector_selectedCmID = rdo.value;
		} else {
			tblCMs.tBodies[0].rows[index].className = ""
		}
	}
	if(typeof CareManagerSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = CareManagerSelector_selectedCmID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);
		CareManagerSelector_SelectedItemChange(args);
	}
}

addEvent(window, "load", Init);
