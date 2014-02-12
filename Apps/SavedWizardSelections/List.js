
var selSvc, currentPage, selectedSelectionID;
var listFilter, listFilterName = "", listFilterScreen = "", listFilterOwner = "";
var listSorter, listSorterColumnID;
var tblSelections, divPagingLinks, btnView;

function Init() {
	selSvc = new Target.Web.Apps.SavedWizardSelections.WebSvc.Selections_class();
	tblSelections = GetElement("tblSelections");
	divPagingLinks = GetElement("Selections_PagingLinks");
	btnView = GetElement("btnView");
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
	listFilter.AddColumn("Screen", GetElement("thScreen"));
	listFilter.AddColumn("Owner", GetElement("thOwner"));
	
	// populate table
	FetchSelectionList(currentPage, 0);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Name":
			listFilterName = column.Filter;
			break;
		case "Screen":
			listFilterScreen = column.Filter;
			break;
		case "Owner":
			listFilterOwner = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchSelectionList(1, 0);
}

/* FETCH LIST METHODS */
function FetchSelectionList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0
			
	selSvc.FetchSelectionsList(page, 
	                        selectedID,
                            listFilterName,
                            listFilterScreen,
                            listFilterOwner,
                            FetchSelectionsList_Callback);
}

function FetchSelectionsList_Callback(response) {
	var sels, selCounter;
	var tr, td, radioButton;
	var str;
	var link;
	
	if(CheckAjaxResponse(response, selSvc.url)) {
		sels = response.value.Selections;
		
		btnView.disabled = true;
		ClearTable(tblSelections);
		
		for(selCounter=0; selCounter<sels.length; selCounter++) {
			tr = AddRow(tblSelections);
			
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "Selector", sels[selCounter].ID, RadioButton_Click);
			
			// name
			td = AddCell(tr, " ");
		    link = AddLink(td, sels[selCounter].Name, GetUrl(sels[selCounter].ID), "");
		    link.className = "transBg";
            // screen
			td = AddCell(tr, sels[selCounter].Screen);
            // owner
            td = AddCell(tr, sels[selCounter].Owner);
            // global
            td = AddCell(tr, sels[selCounter].GlobalSelection ? "Yes" : "No");
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var index, rdo, selectedRow;
	for (index = 0; index < tblSelections.tBodies[0].rows.length; index++){
		rdo = tblSelections.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblSelections.tBodies[0].rows[index];
			tblSelections.tBodies[0].rows[index].className = "highlightedRow"
			selectedSelectionID = rdo.value;
			btnView.disabled = false;
		} else {
			tblSelections.tBodies[0].rows[index].className = ""
		}
	}
}

function GetUrl(id) {
    var url = "Edit.aspx?id=" + id + "&mode=1";
    url += "&backUrl=" + escape(document.location.href);
    return url;
}

function btnView_Click() {
    document.location.href = GetUrl(selectedSelectionID);
}

function btnBack_Click() {
    var backURL = unescape(GetQSParam(document.location.search, 'backUrl'));

    if (backURL && backURL != 'null') {
        document.location.href = backURL;
    } else {
        history.go(-1); 
    }
}

addEvent(window, "load", Init);
