
var securitySvc, currentPage;
var RoleSelector_selectedRoleID;
var listFilter, listFilterName = "";
var listSorter, listSorterColumnID, listSorterDirection;
var btnViewID;
var tblRoles, divPagingLinks, btnView, showViewButton;

function Init() {
	securitySvc = new Target.Web.Apps.Security.WebSvc.Security_class();
	tblRoles = GetElement("tblRoles");
	divPagingLinks = GetElement("Role_PagingLinks");
	btnView = GetElement(btnViewID, true);
	
	if(btnView) btnView.disabled = true;
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
	
	// setup list sorters
	listSorter = new Target.Web.ListSorter(ListSorter_Callback);
	listSorter.AddColumn("Name", GetElement("thName"), 1);
	
	// default list sorting
	listSorterColumnID = 1;		// Name
	listSorterDirection = 1;	// ASC
	
	// populate table
	FetchRoleList(currentPage, RoleSelector_selectedRoleID);
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
	FetchRoleList(1, 0);
}

function ListSorter_Callback(column) {
	listSorterColumnID = column.SortColumnID;
	listSorterDirection = column.Direction;
	FetchRoleList(1, 0);
}

/* FETCH ROLE LIST METHODS */
function FetchRoleList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
	
	if(btnView) btnView.disabled = true;
		
	securitySvc.FetchRoleList(page, selectedID, listFilterName, listSorterColumnID, listSorterDirection, FetchRoleList_Callback)
}

function FetchRoleList_Callback(response) {
	var roles, roleCounter;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, securitySvc.url)) {
	   
		roles = response.value.Roles;

		// remove existing rows
		ClearTable(tblRoles);
		for(roleCounter=0; roleCounter<roles.length; roleCounter++) {
		
			tr = AddRow(tblRoles);
			td = AddCell(tr, "");
			if(showViewButton)
			    radioButton = AddRadio(td, "", "RoleSelect", roles[roleCounter].ID, RadioButton_Click);
            else
                SetInnerText(td, " ");
			
			td = AddCell(tr, "");
			if(showViewButton) {
			    link = AddLink(td, roles[roleCounter].Name, "RoleEdit.aspx?id=" + roles[roleCounter].ID + "&mode=1&backUrl=" + GetBackUrl(), "Edit this role");
			    link.className = "transBg";
            } else {
                SetInnerText(td, roles[roleCounter].Name);
            }
			
			AddCell(tr, roles[roleCounter].Description);
						
			// select the role?
			if(RoleSelector_selectedRoleID == roles[roleCounter].ID || (currentPage == 1 && roles.length == 1)) {
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
	for (index = 0; index < tblRoles.tBodies[0].rows.length; index++){
		rdo = tblRoles.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblRoles.tBodies[0].rows[index];
			tblRoles.tBodies[0].rows[index].className = "highlightedRow"
			RoleSelector_selectedRoleID = rdo.value;
			if(btnView) btnView.disabled = false;
		} else {
			tblRoles.tBodies[0].rows[index].className = ""
		}
	}
}

function GetBackUrl() {
    var backUrl = document.location.href;
    backUrl = AddQSParam(RemoveQSParam(backUrl, "roleID"), "roleID", RoleSelector_selectedRoleID);
    backUrl = escape(backUrl);
    return backUrl;
}

function btnView_Click() {
    document.location.href = "RoleEdit.aspx?id=" + RoleSelector_selectedRoleID + "&mode=1&backUrl=" + GetBackUrl();
}

function btnNew_Click() {
    document.location.href = "RoleEdit.aspx?mode=2&backUrl=" + GetBackUrl();
}

addEvent(window, "load", Init);
