var contractSvc, currentPage;
var providerID, careWorkerID, required, clientID;
var listFilter, listFilterName = "";
var tblCareWorkers, divPagingLinks;

function Init() {
 	contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
	tblCareWorkers = GetElement("tblCareWorkers");
	divPagingLinks = GetElement("CareWorker_PagingLinks");
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Name", GetElement("thName"));
	
	// populate table
	FetchCareWorkerList(currentPage, careWorkerID)
}

/* FETCH CLIENT LIST METHODS */
function FetchCareWorkerList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(careWorkerID == undefined) careWorkerID = 0;
	
	contractSvc.FetchCareWorkerList(page, selectedID, providerID, listFilterName, FetchCareWorkerList_Callback)	
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
	FetchCareWorkerList(1, 0);
}

function FetchCareWorkerList_Callback(response) {
	var careWorkers, index;
	var tr, td, radioButton;
	var str;
	var link;
		
	if(CheckAjaxResponse(response, contractSvc.url)) {
		// disable next/finish buttons by default if the step is mandatory
		if(required) {
			if(btnNext) btnNext.disabled = true;
			if(btnFinish) btnFinish.disabled = true;
		}
		
		// populate the care worker table
		careWorkers = response.value.CareWorkers;
		// remove existing rows
		ClearTable(tblCareWorkers);
		for(index=0; index<careWorkers.length; index++) {
		
			tr = AddRow(tblCareWorkers);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "CareWorkerSelect", careWorkers[index].CareWorkerID, RadioButton_Click);
			
			
			td = AddCell(tr, careWorkers[index].Reference);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, careWorkers[index].Name);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			// select the care Worker?
			if(careWorkerID == careWorkers[index].CareWorkerID || (currentPage == 1 && careWorkers.length == 1)) {
				radioButton.click();
			}			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var x
	var Radio
	for (x = 0; x < tblCareWorkers.tBodies[0].rows.length; x++){
		Radio = tblCareWorkers.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblCareWorkers.tBodies[0].rows[x].className = "highlightedRow"
			careWorkerID = Radio.value;
		} else {
			tblCareWorkers.tBodies[0].rows[x].className = ""
		}
	}
	if(required) {
		if(btnNext) btnNext.disabled = false;
		if(btnFinish) btnFinish.disabled = false;
	}
}

function CareWorkerStep_BeforeNavigate() {
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
    if (clientID == 0 && careWorkerID == 0) {
        alert("Either a Service User or a Care Worker must be selected.");
        return false
    } else {
        url = AddQSParam(RemoveQSParam(url, "careWorkerID"), "careWorkerID", careWorkerID);
	    SelectorWizard_newUrl = url;
	    return true;
    }
}

addEvent(window, "load", Init);
