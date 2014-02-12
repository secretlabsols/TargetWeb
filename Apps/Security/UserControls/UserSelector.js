
var securitySvc, currentPage;
var UserSelector_selectedUserID;
var listFilter, listFilterFirstName = "", listFilterSurname = "", listFilterEmail = "", listFilterExternalAccount = "";
var listSorter, listSorterColumnID, listSorterDirection;
var tblUsers, divPagingLinks;

function Init() {
	securitySvc = new Target.Web.Apps.Security.WebSvc.Security_class();
	tblUsers = GetElement("tblUsers");
	divPagingLinks = GetElement("User_PagingLinks");
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("First Name", GetElement("thFirstName"));
	listFilter.AddColumn("Surame", GetElement("thSurname"));
	listFilter.AddColumn("Email", GetElement("thEmail"));
	listFilter.AddColumn("External Account", GetElement("thExternalAccount"));
	
	// setup list sorters
	listSorter = new Target.Web.ListSorter(ListSorter_Callback);
	listSorter.AddColumn("First Name", GetElement("thFirstName"), 1);
	listSorter.AddColumn("Surame", GetElement("thSurname"), 2);
	listSorter.AddColumn("Email/Username", GetElement("thEmail"), 3);
	listSorter.AddColumn("External Account", GetElement("thExternalAccount"), 4);
	
	// default list sorting
	listSorterColumnID = 2;		// Surname
	listSorterDirection = 1;	// ASC
	
	// populate table
	FetchUserList(currentPage, UserSelector_selectedUserID);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "First Name":
			listFilterFirstName = column.Filter;
			break;
		case "Surname":
			listFilterSurname = column.Filter;
			break;
		case "Email/Username":
			listFilterEmail = column.Filter;
			break;
		case "External Account":
			listFilterExternalAccount = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchUserList(1, 0);
}

function ListSorter_Callback(column) {
	listSorterColumnID = column.SortColumnID;
	listSorterDirection = column.Direction;
	FetchUserList(1, 0);
}

/* FETCH USER LIST METHODS */
function FetchUserList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;
			
	securitySvc.FetchUserList(page, 
                            selectedID, 
                            listFilterFirstName,
                            listFilterSurname,
                            listFilterEmail,
                            listFilterExternalAccount,
                            listSorterColumnID, 
                            listSorterDirection, 
                            FetchUserList_Callback);
}

function FetchUserList_Callback(response) {
	var users, userCounter;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, securitySvc.url)) {
	   
		users = response.value.Users;

		// remove existing rows
		ClearTable(tblUsers);
		for(userCounter=0; userCounter<users.length; userCounter++) {
		
			tr = AddRow(tblUsers);
									
			// first name
			td = AddCell(tr, " ");
		    link = AddLink(td, users[userCounter].FirstName, GetUrl(users[userCounter].ID, users[userCounter].StatusID), "");
		    link.className = "transBg";
            
            // surname
			td = AddCell(tr, " ");
		    link = AddLink(td, users[userCounter].Surname, GetUrl(users[userCounter].ID, users[userCounter].StatusID), "");
		    link.className = "transBg";
            
            // email
            td = AddCell(tr, " ");
            if(users[userCounter].Email.length > 0) SetInnerText(td, users[userCounter].Email);
            
            // external account
            td = AddCell(tr, users[userCounter].ExternalUserName);
			
			// status
            td = AddCell(tr, users[userCounter].Status);
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var index, rdo, selectedRow;
	for (index = 0; index < tblUsers.tBodies[0].rows.length; index++){
		rdo = tblUsers.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblUsers.tBodies[0].rows[index];
			tblUsers.tBodies[0].rows[index].className = "highlightedRow"
			UserSelector_selectedUserID = rdo.value;
		} else {
			tblUsers.tBodies[0].rows[index].className = ""
		}
	}
}

function GetUrl(userID, statusID) {
    if(statusID == Target.Web.Apps.Security.WebSecurityUserStatus.Created)
	    url = "UserActivate.aspx?id=" + userID;
    else
        url = "UserEdit.aspx?id=" + userID + "&mode=1";
    url += "&backUrl=" + escape(document.location.href);
    
    return url;
}

function btnView_Click() {
    document.location.href = "UserEdit.aspx?id=" + UserSelector_selectedUserID + "&mode=1";
}

addEvent(window, "load", Init);
