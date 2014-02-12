
var financeCodeSvc, currentPage;
var OtherFundingOrgSelector_selectedOrgID, OtherFundingOrgSelector_selectedOrgType;
var listFilter, listFilterReference = "", listFilterName = "";
var tblOrganizations, divPagingLinks, btnNext, btnFinish;

function Init() {
	financeCodeSvc = new Target.Abacus.Web.Apps.WebSvc.FinanceCodes_class();
	tblOrganizations = GetElement("tblOrganizations");
	divPagingLinks = GetElement("Organization_PagingLinks");
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
	
	// populate table
	FetchOrgList(currentPage, OtherFundingOrgSelector_selectedOrgID, OtherFundingOrgSelector_selectedOrgType);

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
	FetchClientList(1, 0);
}

/* FETCH CLIENT LIST METHODS */
function FetchOrgList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	
	financeCodeSvc.FetchOtherFundingOrganizationList(page, selectedID, OtherFundingOrgSelector_selectedOrgType, listFilterName, FetchOrgList_Callback)
}

function FetchOrgList_Callback(response) {
	var orgs, orgCounter;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, financeCodeSvc.url)) {
	    
	
		// populate the conversation table
		orgs = response.value.Organizations;

		// remove existing rows
		ClearTable(tblOrganizations);
		for(orgCounter=0; orgCounter<orgs.length; orgCounter++) {
		
			tr = AddRow(tblOrganizations);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "OrgsSelect", orgs[orgCounter].ID, RadioButton_Click);
						
			td = AddCell(tr, orgs[orgCounter].Name);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = orgs[orgCounter].Address;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
			
			str = orgs[orgCounter].Postcode;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
						
			// select the client?
			if(OtherFundingOrgSelector_selectedOrgID == orgs[orgCounter].ID || ( currentPage == 1 &&  orgs.length == 1)) {
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
	for (index = 0; index < tblOrganizations.tBodies[0].rows.length; index++){
		rdo = tblOrganizations.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblOrganizations.tBodies[0].rows[index];
			tblOrganizations.tBodies[0].rows[index].className = "highlightedRow"
			OtherFundingOrgSelector_selectedOrgID = rdo.value;
		} else {
			tblOrganizations.tBodies[0].rows[index].className = ""
		}
	}

	if(typeof InPlaceOtherFundingOrganizationSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = OtherFundingOrgSelector_selectedOrgID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		InPlaceOtherFundingOrganizationSelector_SelectedItemChange(args);
	}
}


addEvent(window, "load", Init);
