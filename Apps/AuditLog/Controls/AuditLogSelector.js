
var logSvc, currentPage, applicationID, tableName, parentID, logType, userName, dateFrom, dateTo;
var tblLogEntries, divPagingLinks, divDetails, divLogEntries;

function Init() {
	logSvc = new Target.Web.Apps.AuditLog.WebSvc.AuditLog_class();
	divLogEntries = GetElement("divLogEntries");
	tblLogEntries = GetElement("tblLogEntries");
	divPagingLinks = GetElement("divPagingLinks");
	divDetails = GetElement("divDetails");

    Resize();
	FetchAuditLogList(currentPage);
}
function FetchAuditLogList(page) {
	currentPage = page;
	DisplayLoading(true);
	logSvc.FetchAuditLogList(page, applicationID, logType, userName, tableName, dateFrom, dateTo, parentID, FetchAuditLogList_Callback)
}
function FetchAuditLogList_Callback(response) {
	var entries, index;
	var tr, td, link;
		
	if(CheckAjaxResponse(response, logSvc.url)) {
		
		entries = response.value.LogEntries;
		
		ClearTable(tblLogEntries);
		for(index=0; index<entries.length; index++) {
		
			tr = AddRow(tblLogEntries);
			
			td = AddCell(tr, "");
			link = AddLink(td, Date.strftime("%d/%m/%Y %H:%M:%S", entries[index].LogDate), 
				"javascript:FetchAuditLogDetail(" + entries[index].ID + ", 0);", 
				"Click here to display the details of this entry");
			link.className = "transBg"
			
			AddCell(tr, entries[index].UserName);
			AddCell(tr, entries[index].TableName);
			AddCell(tr, entries[index].LogType);
			
		}
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function FetchAuditLogDetail(id, page) {

	var logType = 0;
	var userName = null;
	var dateFrom = null;
	var dateTo = null;

	DisplayLoading(true);
	logSvc.FetchAuditLogDetailForDisplay(id, page, applicationID, logType, userName, tableName, dateFrom, dateTo, parentID, FetchAuditLogDetail_Callback);
}
function FetchAuditLogDetail_Callback(response) {
	if(CheckAjaxResponse(response, logSvc.url)) {
		divDetails.innerHTML = response.value.Value;
	}
	DisplayLoading(false);
}
function btnDisplayAll_Click() {
	FetchAuditLogDetail(0, 0);
}
function btnDisplayPage_Click() {
	FetchAuditLogDetail(0, currentPage);
}
function ToggleChildEntries(id) {
	var div = GetElement(id);
	if(div.style.display == "none")
		div.style.display = "block";
	else
		div.style.display = "none"
}
function Resize() {
    var divLogEntriesHeight, divDetailsHeight, clientHeight, newHeight;
    
    divLogEntriesHeight = ConvertEmToPx(GetCurrentStyle(divLogEntries).height);
    divDetailsHeight = ConvertEmToPx(GetCurrentStyle(divDetails).height);
    clientHeight = document.documentElement.clientHeight;
    
    newHeight = clientHeight - divLogEntriesHeight - ConvertEmToPx(6.5);
    divDetails.style.height = newHeight + "px";
}

addEvent(window, "resize", Resize);
addEvent(window, "load", Init);
