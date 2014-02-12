
var contractSvc, currentPage;
var selectedWeekID, providerID, contractID, weDateFrom, weDateTo, closureDateFrom, closureDateTo;
var listFilter, listFilterReOpenedBy = "";
var tblWeeks, divPagingLinks;
var btnNewID;
var btnNew, btnView;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	tblWeeks = GetElement("tblWeeks");
	divPagingLinks = GetElement("divPagingLinks");
	btnNew = GetElement(btnNewID, true);
	btnView = GetElement("btnView");
	
	// disable new button if we don't have a contract
	var contractID = GetQSParam(document.location.search, "contractID");
	//if(!contractID || contractID == null || contractID.length == 0) btnNew.disabled = true;
	if(btnNew) {
	    if(contractID == 0 || contractID == null || contractID.length == 0) btnNew.disabled = true;
	}
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Re-Opened By", GetElement("thReOpenedBy"));
		
	// populate table
	FetchReOpenedWeekList(currentPage);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Re-Opened By":
			listFilterReOpenedBy = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchReOpenedWeekList(1);
}

function FetchReOpenedWeekList(page) {
	currentPage = page;
	DisplayLoading(true);
	
	contractSvc.FetchReOpenedWeekList(page, providerID, contractID, weDateFrom, weDateTo, closureDateFrom, closureDateTo, selectedWeekID, listFilterReOpenedBy, FetchReOpenedWeekList_Callback)
}

function FetchReOpenedWeekList_Callback(response) {
	var weeks, index;
	var tr, td, radioButton;
	var str;
	var link;

	btnView.disabled = true;
		
	if(CheckAjaxResponse(response, contractSvc.url)) {
		// populate the table
		weeks = response.value.Weeks;

		// remove existing rows
		ClearTable(tblWeeks);
		for(index=0; index<weeks.length; index++) {
		
			tr = AddRow(tblWeeks);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "WeeksSelect", weeks[index].ID, RadioButton_Click);
			
			AddCell(tr, weeks[index].ProviderName);
			
			AddCell(tr, weeks[index].ContractNumber);
			
			AddCell(tr, Date.strftime("%d/%m/%Y", weeks[index].WeekEnding));
			
			AddCell(tr, weeks[index].ReOpenedBy);
			
			// select the week?
			if(selectedWeekID == weeks[index].ID || (currentPage == 1 && weeks.length == 1)) {
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
	for (index = 0; index < tblWeeks.tBodies[0].rows.length; index++){
		rdo = tblWeeks.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblWeeks.tBodies[0].rows[index];
			tblWeeks.tBodies[0].rows[index].className = "highlightedRow"
			selectedWeekID = rdo.value;
			btnView.disabled = false;
		} else {
			tblWeeks.tBodies[0].rows[index].className = ""
		}
	}
}
function btnNew_Click() {
    var url = document.location.search;
    url = RemoveQSParam(url, "weekID");
    url = RemoveQSParam(url, "currentStep");
    url = "ReOpenWeekEdit.aspx" + url + "&mode=2&backUrl=" + GetBackUrl();
    document.location.href = url;
}
function btnView_Click() {
    var url = "ReOpenWeekEdit.aspx?id=" + selectedWeekID + "&mode=1&backUrl=" + GetBackUrl();
    document.location.href = url;
}
function GetBackUrl() {
    var url = document.location.href;
    url = RemoveQSParam(url, "weekID");
    if(selectedWeekID > 0) url = AddQSParam(url, "weekID", selectedWeekID);
    return escape(url);
}

addEvent(window, "load", Init);
