
var lookupSvc, currentPage;
var ServiceGroupSelector_selectedServiceGroupID, ServiceGroupSelector_onlyShowGroupsAvailableToUser;
var tblServiceGroups, divPagingLinks;

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	tblServiceGroups = GetElement("tblServiceGroups");
	divPagingLinks = GetElement("ServiceGroup_PagingLinks");
		
	// populate table
	FetchServiceGroupList(currentPage, ServiceGroupSelector_selectedServiceGroupID);
}

function FetchServiceGroupList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;

	lookupSvc.FetchServiceGroupList(page, selectedID, ServiceGroupSelector_onlyShowGroupsAvailableToUser, FetchServiceGroupList_Callback)
}

function FetchServiceGroupList_Callback(response) {
	var sgs, index;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		// populate the table
		sgs = response.value.ServiceGroups;

		// remove existing rows
		ClearTable(tblServiceGroups);
		for(index=0; index<sgs.length; index++) {
		        
		        tr = AddRow(tblServiceGroups);
		        td = AddCell(tr, "");
		        radioButton = AddRadio(td, "", "ServiceGroupSelect", sgs[index].ID, RadioButton_Click);

		        str = sgs[index].Description;
		        if (str.length == 0) str = " ";
		        td = AddCell(tr, str);
		        td.style.textOverflow = "ellipsis";
		        td.style.overflow = "hidden";

		        str = sgs[index].ServiceGroupClassification;
		        if (!str || str.length == 0) str = " ";
		        td = AddCell(tr, str);
		        td.style.textOverflow = "ellipsis";
		        td.style.overflow = "hidden";

		        str = sgs[index].ServiceCategoryDescription;
		        if (!str || str.length == 0) str = " ";
		        td = AddCell(tr, str);
		        td.style.textOverflow = "ellipsis";
		        td.style.overflow = "hidden";

		        // select the sg?
		        if (ServiceGroupSelector_selectedServiceGroupID == sgs[index].ID || ( currentPage == 1 && sgs.length == 1)) {
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
	for (index = 0; index < tblServiceGroups.tBodies[0].rows.length; index++){
		rdo = tblServiceGroups.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblServiceGroups.tBodies[0].rows[index];
			tblServiceGroups.tBodies[0].rows[index].className = "highlightedRow"
			ServiceGroupSelector_selectedServiceGroupID = rdo.value;
		} else {
			tblServiceGroups.tBodies[0].rows[index].className = ""
		}
	}         
	if(typeof ServiceGroupSelector_SelectedItemChange == "function") {
		var args = new Array(3);
		args[0] = ServiceGroupSelector_selectedServiceGroupID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		ServiceGroupSelector_SelectedItemChange(args);
	}
}

addEvent(window, "load", Init);
