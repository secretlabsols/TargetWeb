
var lookupSvc, currentPage;
var TeamSelector_selectedTeamID, TeamSelector_availableToRes, TeamSelector_availableToDom;
var listFilter, listFilterReference = "", listFilterName = "";
var tblTeams, divPagingLinks;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	tblTeams = GetElement("tblTeams");
	divPagingLinks = GetElement("Team_PagingLinks");
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
	listFilter.AddColumn("Reference", GetElement("thRef"));
		
	// populate table
	FetchTeamList(currentPage, TeamSelector_selectedTeamID);
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
	FetchTeamList(1, 0);
}

function FetchTeamList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	
	lookupSvc.FetchTeamList(page, selectedID, TeamSelector_availableToRes, TeamSelector_availableToDom, listFilterReference, listFilterName, FetchTeamList_Callback)
}

function FetchTeamList_Callback(response) {
	var teams, index;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		// populate the table
		teams = response.value.Teams;

		// remove existing rows
		ClearTable(tblTeams);
		for(index=0; index<teams.length; index++) {
		
			tr = AddRow(tblTeams);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "TeamSelect", teams[index].ID, RadioButton_Click);
			
			str = teams[index].Name;
			if(str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = teams[index].Reference;
			if(!str || str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the team?
			if(TeamSelector_selectedTeamID == teams[index].ID || ( currentPage == 1 && teams.length == 1)) {
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
	for (index = 0; index < tblTeams.tBodies[0].rows.length; index++){
		rdo = tblTeams.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblTeams.tBodies[0].rows[index];
			tblTeams.tBodies[0].rows[index].className = "highlightedRow"
			TeamSelector_selectedTeamID = rdo.value;
		} else {
			tblTeams.tBodies[0].rows[index].className = ""
		}
	}
	if(typeof TeamSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = TeamSelector_selectedTeamID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);
		TeamSelector_SelectedItemChange(args);
	}
}

addEvent(window, "load", Init);
