
var amendReqSvc, securitySvc;
var currentPage, currentFromDate, currentToDate, currentStatus, currentUser;
var tblRequests, divPagingLinks, dteFrom, dteTo, cboStatus, cboRequestedBy;
var tblRequestsID = "Requests_Table";
var divPagingLinksID = "Requests_PagingLinks";
var dteFromID = "dteFrom_txtTextBox";
var dteToID = "dteTo_txtTextBox";
var cboStatusID = "cboStatus";
var cboRequestedByID = "cboRequestedBy";

function Init() {
	amendReqSvc = new Target.Web.Apps.AmendReq.WebSvc.AmendmentRequests_class();
	securitySvc = new Target.Web.Apps.Security.WebSvc.Security_class();
	tblRequests = GetElement(tblRequestsID);
	divPagingLinks = GetElement(divPagingLinksID);
	dteFrom = GetElement(dteFromID);
	dteTo = GetElement(dteToID);
	cboStatus = GetElement(cboStatusID);
	cboRequestedBy = GetElement(cboRequestedByID);
	
	cboStatus.value = currentStatus;
	
	// populate requested by list (if available)
	if(cboRequestedBy) FetchRequestedByList();
	FetchRequestList(1);
}

function btnFilter_OnClick() {
	FetchRequestList(1);
}
function btnView_OnClick(evt, requestID) {
	document.location.href = "ViewAmendReq.aspx?id=" + requestID;
}

/* FETCH REQUESTS METHODS */
function FetchRequestList(page) {
	currentPage = page;
	currentFromDate = IsDate(dteFrom.value) ? dteFrom.value.toDate() : null;
	currentToDate = IsDate(dteTo.value) ? dteTo.value.toDate() : null;
	currentStatus = cboStatus.value.length > 0 ? cboStatus.value : 0;
	if(cboRequestedBy)
		currentUser = cboRequestedBy.value.length > 0 ? cboRequestedBy.value : 0;
	DisplayLoading(true);
	amendReqSvc.FetchAmendmentRequestList(currentPage, 0, currentFromDate, currentToDate, currentStatus, currentUser, false, FetchRequestList_Callback)
}
function FetchRequestList_Callback(response) {
	var requests, reqCounter;
	if(CheckAjaxResponse(response, amendReqSvc.url)) {
		// populate the requests table
		requests = response.value.Requests;
		// remove existing rows
		ClearTable(tblRequests);
		for(reqCounter=0; reqCounter<requests.length; reqCounter++) {
			tr = AddRow(tblRequests);
			AddCell(tr, requests[reqCounter].Reference);
			AddCell(tr, Date.strftime("%d/%m/%Y", requests[reqCounter].CreatedDate));
			td = AddCell(tr, requests[reqCounter].RequestDescription);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			AddCell(tr, requests[reqCounter].StatusDesc);
			td = AddCell(tr, "");
			AddButton(td, "View", "Click here to view the details of this request.", btnView_OnClick, requests[reqCounter].ID);
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
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
		cboRequestedBy.value = currentUser;
	}
	DisplayLoading(false);
}

addEvent(window, "load", Init);

