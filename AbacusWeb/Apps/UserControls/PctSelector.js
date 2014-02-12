
var lookupSvc, currentPage;
var PctSelector_selectedPctID;
var listFilter, listFilterName = "";
var tblPcts, divPagingLinks;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	tblPcts = GetElement("tblPcts");
	divPagingLinks = GetElement("Pct_PagingLinks");
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
		
	// populate table
	FetchPctList(currentPage, PctSelector_selectedPctID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Name":
			listFilterName = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchPctList(1, 0);
}

function FetchPctList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	
	lookupSvc.FetchPctList(page, selectedID, listFilterName, FetchPctList_Callback)
}

function FetchPctList_Callback(response) {
	var pcts, index;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		// populate the conversation table
		pcts = response.value.Pcts;

		// remove existing rows
		ClearTable(tblPcts);
		for(index=0; index<pcts.length; index++) {
		
			tr = AddRow(tblPcts);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "PctSelect", pcts[index].ID, RadioButton_Click);
			
			str = pcts[index].Name;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = pcts[index].Address;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the pct?
			if(PctSelector_selectedPctID == pcts[index].ID || ( currentPage == 1 && pcts.length == 1)) {
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
	for (index = 0; index < tblPcts.tBodies[0].rows.length; index++){
		rdo = tblPcts.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblPcts.tBodies[0].rows[index];
			tblPcts.tBodies[0].rows[index].className = "highlightedRow"
			PctSelector_selectedPctID = rdo.value;
		} else {
			tblPcts.tBodies[0].rows[index].className = ""
		}
	}
	if(typeof PctSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = PctSelector_selectedPctID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		PctSelector_SelectedItemChange(args);
	}
}

addEvent(window, "load", Init);
