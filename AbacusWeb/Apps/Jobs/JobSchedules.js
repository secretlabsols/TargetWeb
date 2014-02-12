
var lookupSvc, currentPage, jobScheduleID;
var tblJobs, divPagingLinks, btnView, btnNew;
var DATE_FORMAT = "%d/%m/%y %H:%M:%S";

function Init() {
	lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
	tblJobs = GetElement("tblJobs");
	divPagingLinks = GetElement("JobSchedule_PagingLinks");
	btnView = GetElement("btnView", true);
	btnNew = GetElement("btnNew", true);
	currentPage = 1;
	if(btnView) btnView.disabled = true;
		
	FetchJobScheduleList(currentPage);
}


function FetchJobScheduleList(page) {
	currentPage = page;
	
	DisplayLoading(true);

	lookupSvc.FetchJobScheduleList(page, FetchJobScheduleList_Callback)
}

function FetchJobScheduleList_Callback(response) {
	var jobs, index;
	var tr, td, radioButton;
	var str;
	var link;

	if (btnView) btnView.disabled = true;
	
	if(CheckAjaxResponse(response, lookupSvc.url)) {
		
		// populate the table
		jobs = response.value.JobSchedules;
		
		// remove existing rows
		ClearTable(tblJobs);
		for(index=0; index<jobs.length; index++) {
		
			tr = AddRow(tblJobs);
			td = AddCell(tr, "");
			radioButton = AddRadio(td, "", "JobScheduleSelect", jobs[index].ID, RadioButton_Click);
			
			AddCell(tr, jobs[index].Description);
	
			AddCell(tr, jobs[index].JobType);
			
			AddCell(tr, jobs[index].Enabled);

            AddCell(tr, Date.strftime(DATE_FORMAT, jobs[index].NextRunDate));

			if(jobs[index].ID == jobScheduleID || (currentPage == 1 && jobs.length == 1))
			    radioButton.click();
			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function RadioButton_Click() {
	var index, rdo, hid;
	
	for (index = 0; index < tblJobs.tBodies[0].rows.length; index++){
		rdo = tblJobs.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			tblJobs.tBodies[0].rows[index].className = "highlightedRow"
			jobScheduleID = rdo.value;
			hid = tblJobs.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[1];

			if(btnView) btnView.disabled = false;

		} else {
			tblJobs.tBodies[0].rows[index].className = ""
		}
	}
}


function btnNew_Click() {
	// mode=2 means AddNew
	document.location.href = "JobScheduleMaintenance.aspx?mode=2&backUrl=" + GetBackUrl();
}

function btnView_Click() {
	// mode=1 means Fetched
	document.location.href = "JobScheduleMaintenance.aspx?id=" + jobScheduleID + "&mode=1&backUrl=" + GetBackUrl();
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}


addEvent(window, "load", Init);
