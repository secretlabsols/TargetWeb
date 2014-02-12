
var notifSvc, securitySvc, dteFrom, dteTo, cboStatus, cboRequestedBy, tblNotifs, divPagingLinks;
var currentPage, currentDateFrom, currentDateTo, currentStatus, currentRequestedBy;
var dteFromID = "dteFrom_txtTextBox";
var dteToID = "dteTo_txtTextBox";
var cboStatusID = "cboStatus"
var cboRequestedByID = "cboRequestedBy";
var tblNotifsID = "Notifs_Table";
var divPagingLinksID = "Notifs_PagingLinks";
var listFilter, listFilterReference = "", listFilterServiceUser = "";

function Init() {
	notifSvc = new Target.SP.Web.Apps.WebSvc.SUNotif_class();
	securitySvc = new Target.Web.Apps.Security.WebSvc.Security_class();
	dteFrom = GetElement(dteFromID);
	dteTo = GetElement(dteToID);
	cboStatus = GetElement(cboStatusID);
	cboRequestedBy = GetElement(cboRequestedByID);
	tblNotifs = GetElement(tblNotifsID);
	divPagingLinks = GetElement(divPagingLinksID);
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Reference", GetElement("thRef"));
	listFilter.AddColumn("Service User", GetElement("thName"));
	
	// set the default filter values
	if(IsDate(currentDateFrom))
		dteFrom.value = currentDateFrom;
	if(IsDate(currentDateTo))
		dteTo.value = currentDateTo;
	cboStatus.value = currentStatus;
	if(cboRequestedBy)
		cboRequestedBy.value = currentRequestedBy;	
	
	// populate requested by list (if available)
	if(cboRequestedBy) FetchRequestedByList();
	FetchNotifList(1);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Reference":
			listFilterReference = column.Filter;
			break;
		case "Service User":
			listFilterServiceUser = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchNotifList(1);
}

function btnFilter_OnClick() {
	FetchNotifList(1);
}

/* REQUESTED BY LIST METHODS */
function FetchRequestedByList() {	
	DisplayLoading(true);
	securitySvc.FetchExternalUserList(FetchRequestedByList_Callback)
}
function FetchRequestedByList_Callback(response) {
	var keys, keyCounter, items;
	
	if(CheckAjaxResponse(response, securitySvc.url)) {
		// populate the To filter dropdown
		cboRequestedBy.options.length = 0;
		cboRequestedBy.options.add(new Option("", 0));
		items = response.value.List;
		keys = items.getKeys();
		for(keyCounter=0; keyCounter<keys.length; keyCounter++) {
			cboRequestedBy.options.add(new Option(keys[keyCounter], items.getValue(keys[keyCounter])));
		}
		// set default
		cboRequestedBy.value = currentRequestedBy;
	}
	DisplayLoading(false);
}

/* NOTIF LIST METHODS */
function FetchNotifList(page) {
	currentPage = page;
	currentDateFrom = IsDate(dteFrom.value) ? dteFrom.value.toDate() : null;
	currentDateTo = IsDate(dteTo.value) ? dteTo.value.toDate() : null;
	currentStatus = cboStatus.value.length > 0 ? cboStatus.value : 0;
	if(cboRequestedBy)
		currentRequestedBy = cboRequestedBy.value.length > 0 ? cboRequestedBy.value : 0;
	DisplayLoading(true);
	notifSvc.FetchSUNotifList(currentPage, currentDateFrom, currentDateTo, currentStatus, currentRequestedBy, listFilterReference, listFilterServiceUser, FetchNotifList_Callback)
}
function FetchNotifList_Callback(response) {
	var notifs, notifCounter;
	var tr, td, text, title, button, url;
	
	if(CheckAjaxResponse(response, notifSvc.url)) {
		// populate the notifications table
		notifs = response.value.Notifications;
		// remove existing rows
		ClearTable(tblNotifs);
		for(notifCounter=0; notifCounter<notifs.length; notifCounter++) {
			tr = AddRow(tblNotifs);
			AddCell(tr, notifs[notifCounter].Reference);
			AddCell(tr, notifs[notifCounter].ServiceUser);
			AddCell(tr, notifs[notifCounter].TypeDesc);
			td = AddCell(tr, " ");
			if(notifs[notifCounter].Created > 1)
				SetInnerText(td, Date.strftime("%d/%m/%Y", notifs[notifCounter].Created));
			td = AddCell(tr, " ");
			if(notifs[notifCounter].Submitted > 1)
				SetInnerText(td, Date.strftime("%d/%m/%Y", notifs[notifCounter].Submitted));
			td = AddCell(tr, " ");
			if(notifs[notifCounter].Completed > 1)
				SetInnerText(td, Date.strftime("%d/%m/%Y", notifs[notifCounter].Completed));
			AddCell(tr, notifs[notifCounter].StatusDesc);
	
			url = notifs[notifCounter].LinkUrl.toUpperCase();
			if(url.indexOf("PROCESS") >= 0) {
				// process
				text = "Process";
				title = "Click here to process this notification.";
			} else if(url.indexOf("VIEW") >= 0) {
				// view
				text = "View";
				title = "Click here to view this notification.";
			} else if(url.indexOf("NEW") >= 0) {
				// submit
				text = "Continue";
				title = "Click here to upload the signed notification document and submit this notification.";
			}
			td = AddCell(tr, " ");
			button = AddButton(td, text, title, btnAction_OnClick, new Array(notifs[notifCounter].LinkUrl, notifs[notifCounter].LinkInNewWindow));
			button.style.width = "6em";
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function btnAction_OnClick(evt, args) {
	var url = args[0];
	var newWindow = args[1];
	if(newWindow)
		window.open(url);
	else
		document.location.href = url;
}




