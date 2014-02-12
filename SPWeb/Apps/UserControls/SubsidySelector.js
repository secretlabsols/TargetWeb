var subsidiesSvc, tblSubsidies, divPagingLinks, currentPage, providerID, serviceID, clientID, dateFrom, dateTo, selectedSubsidyID, status;
var tblSubsidyID = "ListSubsidies";
var divPagingLinksID = "Subsidy_PagingLinks";
var listFilter, listFilterReference = "", listFilterName = "";
var listSorter, listSorterColumnID, listSorterDirection;

function Init() {
	subsidiesSvc = new Target.SP.Web.Apps.WebSvc.Subsidies_class();
	tblSubsidies = GetElement(tblSubsidyID);
	divPagingLinks = GetElement(divPagingLinksID);
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Svc User Ref", GetElement("thRef"));
	listFilter.AddColumn("Name", GetElement("thName"));
	
	// setup list sorters
	listSorter = new Target.Web.ListSorter(ListSorter_Callback);
	listSorter.AddColumn("Name", GetElement("thName"), 1);
	listSorter.AddColumn("Date From", GetElement("thDateFrom"), 2);
	
	// default list sorting
	listSorterColumnID = 1;		// Name
	listSorterDirection = 1;	// ASC
	
	// populate table
	FetchSubsidyList(currentPage);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Svc User Ref":
			listFilterReference = column.Filter;
			break;
		case "Name":
			listFilterName = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchSubsidyList(1);
}

function ListSorter_Callback(column) {
	listSorterColumnID = column.SortColumnID;
	listSorterDirection = column.Direction;
	FetchSubsidyList(1);
}

/* FETCH REMITTANCE LIST METHODS */
function FetchSubsidyList(page) {
	currentPage = page;
	DisplayLoading(true);
	subsidiesSvc.FetchSubsidiesList(page, serviceID, clientID, providerID, dateFrom, dateTo, status, listFilterReference, listFilterName, listSorterColumnID, listSorterDirection, FetchSubsidyList_Callback)
}
function FetchSubsidyList_Callback(response) {
	var subsidies, subCounter, tr, td, link;
	var viewUrl;
	var str;
	var radioButton;
	
	if(CheckAjaxResponse(response, subsidiesSvc.url)) {
		subsidies = response.value.Subsidies;
			
		// build the View Subsidy Url to include the current Url for its Back button
		viewUrl = "ViewSubsidy.aspx?id=";
					
		ClearTable(tblSubsidies);
		for(subCounter=0; subCounter<subsidies.length; subCounter++) {
			tr = AddRow(tblSubsidies);
			// selector
			td = AddCell(tr, "");
			radioButton = AddRadio(td, "", "SubsidySelect", subsidies[subCounter].ID, rdoViewSubsidy_OnClick);
			
			// Service User reference
			str = subsidies[subCounter].ServiceUserReference;
			if (str.length == 0) str = " ";
			td = AddCell(tr, "");
			link = AddLink(td, str, viewUrl + subsidies[subCounter].ID, "Click here to view the subsidy details");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			// service User name
			str = subsidies[subCounter].ServiceUserName;
			if (str.length == 0) str = " ";
			td = AddCell(tr, "");
			link = AddLink(td, str, viewUrl + subsidies[subCounter].ID, "Click here to view the subsidy details");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = Date.strftime("%d/%m/%Y", subsidies[subCounter].DateFrom);
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = Date.strftime("%d/%m/%Y", subsidies[subCounter].DateTo);
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, "");
			td.innerHTML = subsidies[subCounter].Subsidy.toString().formatCurrency();
						
			str = subsidies[subCounter].ProviderRef;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = subsidies[subCounter].Level;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			switch(parseInt(subsidies[subCounter].Status, 10))
			{
			case 1:  
				str = "Active";
				break;
			case 2: 
				str = "Suspended";
				break;
			case 4: 
				str = "Provisional";
				break;
			case 8: 
				str = "Documentry";
				break;
			case 16: 
				str = "Suspended";
				break;
			}
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the subsidy?
			if(selectedSubsidyID == subsidies[subCounter].ProviderID || subsidies.length == 1) {
				radioButton.click();
			}	

		}		
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function rdoViewSubsidy_OnClick() {
	var x
	var Radio
	for (x = 0; x < tblSubsidies.tBodies[0].rows.length; x++){
		Radio = tblSubsidies.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblSubsidies.tBodies[0].rows[x].className = "highlightedRow"
			selectedSubsidyID = Radio.value;
		} else {
			tblSubsidies.tBodies[0].rows[x].className = ""
		}
	}
}

function btnViewSubsidy_Click() {
	var x
	var Radio
	var itemFound
	var Url
	var viewUrl
	var value
	
	itemFound = false
	
	// build the View Subsidy Url to include the current Url for its Back button
	viewUrl = "ViewSubsidy.aspx?id=";

	value = selectedSubsidyID;
	if(value > 0) {
		Url = viewUrl + value;
		itemFound = true
	}
	
	if (itemFound) {
		window.location.href = Url
	} else {
		alert("Please select a subsidy.")
	}
}