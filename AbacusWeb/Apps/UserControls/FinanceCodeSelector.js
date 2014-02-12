var lookupSvc, currentPage;
var FinanceCodeSelector_selectedFinanceCodeID, FinanceCodeSelector_Category;
var tblFinanceCodes, divPagingLinks;
var listFilter, listFilterFinanceCode = "", listFilterFinanceCodeDescription = "";
var FinanceCode_SelectedexpenditureAccountGroupID = 0;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	tblFinanceCodes = GetElement("tblFinanceCodes");
	divPagingLinks = GetElement("FinanceCode_PagingLinks");
	
    // setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Description", GetElement("thCode"));
	listFilter.AddColumn("Framework Type", GetElement("thDescription"));
	// populate table
	FetchFinanceCodeList(currentPage, FinanceCodeSelector_selectedFinanceCodeID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Code":
			listFilterFinanceCode = column.Filter;
			break;
		case "Description":
			listFilterFinanceCodeDescription = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchFinanceCodeList(1);
}

function FetchFinanceCodeList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	if(listFilterFinanceCode == undefined) listFilterFinanceCode = "";
	if (listFilterFinanceCodeDescription == undefined) listFilterFinanceCodeDescription = "";
    lookupSvc.FetchFinanceCodeList(page, listFilterFinanceCode, listFilterFinanceCodeDescription, selectedID, FinanceCodeSelector_Category, FinanceCode_SelectedexpenditureAccountGroupID, FetchFinanceCodeList_Callback)
}

function FetchFinanceCodeList_Callback(response) {
	var fcs, index;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		// populate the table
		fcs = response.value.FinanceCodes;

		// remove existing rows
		ClearTable(tblFinanceCodes);
		for(index=0; index<fcs.length; index++) {
		
			tr = AddRow(tblFinanceCodes);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "FinanceCodeSelect", fcs[index].ID, RadioButton_Click);
			
			str = fcs[index].Code;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = fcs[index].Description;
			if(!str || str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the fc?
			if (FinanceCodeSelector_selectedFinanceCodeID == fcs[index].ID || (currentPage == 1 && fcs.length == 1)) {
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
	for (index = 0; index < tblFinanceCodes.tBodies[0].rows.length; index++){
		rdo = tblFinanceCodes.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblFinanceCodes.tBodies[0].rows[index];
			tblFinanceCodes.tBodies[0].rows[index].className = "highlightedRow"
			FinanceCodeSelector_selectedFinanceCodeID = rdo.value;
		} else {
			tblFinanceCodes.tBodies[0].rows[index].className = ""
		}
	}
	if(typeof FinanceCodeSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = FinanceCodeSelector_selectedFinanceCodeID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		FinanceCodeSelector_SelectedItemChange(args);
	}
}




addEvent(window, "load", Init);
