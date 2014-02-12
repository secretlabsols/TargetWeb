var OccupancySvc, securitySvc, tblOccupancy, divPagingLinks, currentPage;
var webSecurityCompanyID;
var tblOccupancyID = "ListOccupancy";
var divPagingLinksID = "Occupancy_PagingLinks";
var dteFromID = "txtDateFrom_txtTextBox";
var dteToID = "txtDateTo_txtTextBox";
var txtDateFrom, txtDateTo;
var tmpStr;
var listFilter, listFilterReference = "", listFilterName = "";
var listSorter, listSorterColumnID, listSorterDirection;

function Init() {
	OccupancySvc = new Target.SP.Web.Apps.WebSvc.OccupancyEnquiry_class();
	tblOccupancy = GetElement(tblOccupancyID);
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
	FetchOccupancy(currentPage);
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
	FetchOccupancy(1);
}

function ListSorter_Callback(column) {
	listSorterColumnID = column.SortColumnID;
	listSorterDirection = column.Direction;
	FetchOccupancy(1);
}

/* OCC LIST METHODS */
function FetchOccupancy(page) {
	currentPage = page;
	DisplayLoading(true);
	txtDateFrom = GetElement(dteFromID);
	txtDateTo = GetElement(dteToID);
	tmpStr = txtDateFrom.value;
	tmpStr = tmpStr.toString();
	if (tmpStr.length > 0) {
		DateFrom = txtDateFrom.value
	} else {
		DateFrom = "01/01/1900"
	}
	tmpStr = txtDateTo.value;
	tmpStr = tmpStr.toString();
	if (tmpStr.length > 0) {
		DateTo = txtDateTo.value
	} else {
		DateTo = "31/12/2079"
	}
	Status = GetStatusValues();
	OccupancySvc.FetchOccupancyEnqList(page, ServiceID,PropertyID, DateFrom.toDate(), DateTo.toDate(), Status, listFilterReference, listFilterName, listSorterColumnID, listSorterDirection, FetchOccupancyEnqList_Callback)
}
function FetchOccupancyEnqList_Callback(response) {
	var Occupancy, OccupancyCounter;
	var tr, td, radioButton;
	var str;
	var propertyID = GetQSParam(document.location.search, "pid");
	
	if(CheckAjaxResponse(response, OccupancySvc.url)) {
		// populate the Property table
		Occupancy = response.value.OccupancyEnq;
		
		// build the View Subsidy url
		viewUrl = "ListSubsidies/ViewSubsidy.aspx?id=";
			
		// remove existing rows
		ClearTable(tblOccupancy);
		for(OccupancyCounter=0; OccupancyCounter<Occupancy.length; OccupancyCounter++) {
			tr = AddRow(tblOccupancy);
			tr.className = "PropertyList";
						
			td = AddCell(tr, "");
			link = AddLink(td, Occupancy[OccupancyCounter].ServiceUserReference, viewUrl + Occupancy[OccupancyCounter].SubsidyID, "Click here to view the subsidy details");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, "");
			link = AddLink(td, Occupancy[OccupancyCounter].ServiceUserName, viewUrl + Occupancy[OccupancyCounter].SubsidyID, "Click here to view the subsidy details");
			link.className = "transBg"
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";	
			
			str = Date.strftime("%d/%m/%Y" , Occupancy[OccupancyCounter].DateFrom);
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";			
			
			str = Date.strftime("%d/%m/%Y", Occupancy[OccupancyCounter].DateTo);
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, "");
			td.innerHTML = Occupancy[OccupancyCounter].Subsidy.toString().formatCurrency();
			
			str = Occupancy[OccupancyCounter].ProviderReference;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = Occupancy[OccupancyCounter].Level;
			if (str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			switch(parseInt(Occupancy[OccupancyCounter].Status, 10))
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
			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

//function RadioButton_Click() {
//	var x
//	var Radio
//	for (x = 0; x < tblProperties.tBodies[0].rows.length; x++){
//		Radio = tblProperties.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
//		if (Radio.checked) {
//			tblProperties.tBodies[0].rows[x].className = "highlightedRow"
//		} else {
//			tblProperties.tBodies[0].rows[x].className = ""
//		}
//	}
//}

function Filter_Click()
{
	// Re-populate table
	FetchOccupancy(currentPage);
}


function GetStatusValues() 
{
	var Active, Provisional, Suspended, Cancelled, Documentary;
	var StatusValue = 0;
	
	Active = GetElement("chkActive_chkCheckbox");
	Provisional = GetElement("chkProvisional_chkCheckbox");
	Suspended = GetElement("chkSuspended_chkCheckbox");
	Cancelled = GetElement("chkCancelled_chkCheckbox");
	Documentary = GetElement("chkDocumentary_chkCheckbox");
	
	if (Active.checked)
	{
		StatusValue = StatusValue + 1;
	}
	if (Cancelled.checked)
	{
		StatusValue = StatusValue + 2;
	}
	if (Provisional.checked)
	{
		StatusValue = StatusValue + 4;
	}
	if (Documentary.checked)
	{
		StatusValue = StatusValue + 8;
	}
	if (Suspended.checked)
	{
		StatusValue = StatusValue + 16;
	}
	return(StatusValue);
}

function OccupancyEnquiry_GetBackUrl(selectedID) {
	// build the current Url
	//return escape("ListServices.aspx?page=" + currentPage + "&id=" + GetQSParam(document.location.search, "id") + "&selectedServiceID=" + selectedID);
	return escape(document.location.href + "&pid=" + selectedID);
}