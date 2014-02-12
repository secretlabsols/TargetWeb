
var jobSvc, jobListCreatedBy, jobListStatusID, jobTypeID, jobListPage;
var currentBatchID, currentJobID, currentJobStepID, currentJobStepXml, canCreateJobs;
var jobStepListTable;
var TAB_PANEL_PREFIX = "TabStrip_TabPanel";
var DATE_FORMAT = "%d/%m/%y %H:%M:%S";

function Init() {
	currentBatchID = GetQSParam(document.location.search, "batchid");
	currentJobID = GetQSParam(document.location.search, "jobid");
	
	// Get the job step(s) for the passed job ID..
	jobSvc = new Target.Abacus.Web.Apps.Jobs.WebSvc.JobService_class();
	FetchJobStepList(currentJobID);
}

/* JOB STEP LIST METHODS */
function FetchJobStepList(jobID) {
	DisplayLoading(true);
	jobSvc.FetchJobStepList(jobID, FetchJobStepList_Callback);
}
function FetchJobStepList_Callback(response) {
	if(!response.value.ErrMsg.Success) {
		DisplayError(response.value.ErrMsg, jobSvc.url);
	} else {
		jobStepListTable = GetElement("JobStepList_Table");
		var steps = response.value.Steps;
		var tr, td, but, prog;

		// remove existing rows
		ClearTable(jobStepListTable);
		// for each job, add a row to the table
		for(i=0; i<steps.length; i++) {
			tr = AddRow(jobStepListTable);
			AddCell(tr, steps[i].StepNumber);
			td = AddCell(tr, steps[i].JobStepTypeName);
			td.title = steps[i].JobStepTypeDesc;
			if (steps[i].JobStepStatusID == Target.Abacus.Library.Core.JobStepStatus.InProgress) {
				// display a progress bar and running time info
				td = AddCell(tr, "");
				var start = steps[i].StartDateTime;
				var now = new Date();
				var hours = start.diff("h", now);
				var mins = start.diff("n", now) - (hours * 60);
				var secs = start.diff("s", now) - (hours * 60 * 60) - (mins * 60);
				td.title = steps[i].PercentComplete + "% Complete. Running time " + 
					lPadStr(hours, 2, "0") + ":" + lPadStr(mins, 2, "0") + ":" + lPadStr(secs, 2, "0");
				prog = new ProgressBar(td, 100, 12);
				prog.setPercent(steps[i].PercentComplete/100);
			} else {
				// display status name
				td = AddCell(tr, steps[i].JobStepStatusName);
				td.title = steps[i].JobStepStatusDesc;
				td.className = steps[i].JobStepStatusCssClass;
			}
			td = AddCell(tr, Date.strftime(DATE_FORMAT, steps[i].StartDateTime));
			td.className = "nowrap";
			td = AddCell(tr, Date.strftime(DATE_FORMAT, steps[i].EndDateTime));
			td.className = "nowrap";
			td = AddCell(tr, "")
			
			but = AddImg(td, "../../../Images/Jobs/outputs.png", "View results", "pointer", JobStepList_ViewXml, 
				new Array(
					steps[i].ID, 
					steps[i].StepNumber, 
					steps[i].JobStepTypeName,
					Target.Abacus.Library.Core.JobStepXml.Output)
				);
			but.id = "btnResults" + steps[i].ID;
		}
	}
	DisplayLoading(false);
}
function RefreshJobStepList() {
	FetchJobStepList(currentJobID);
	// clear the input, progress and results views
	GetElement(TAB_PANEL_PREFIX + "1").innerHTML = "Loading...";
}

function JobStepList_ViewXml(evt, args) {
    var index, stepID;
	currentJobStepID = args[0];
	var jobStepNumber = args[1];
	var jobStepTypeDesc = args[2];
	currentJobStepXml = args[3];
	var xmlTypeName;
	var tabStrip;

    // highlight the row containing the clicked on results icon
    for (index = 0; index < jobStepListTable.tBodies[0].rows.length; index++){
	    stepID = jobStepListTable.tBodies[0].rows[index].cells[0].innerHTML;
        if (jobStepNumber == stepID) {
	        jobStepListTable.tBodies[0].rows[index].className = "highlightedRow"
	    } else {
	        jobStepListTable.tBodies[0].rows[index].className = ""
	    }
    }
	
	// work out the type of Xml
	switch(currentJobStepXml) {
	    case Target.Abacus.Library.Core.JobStepXml.Output:
			xmlTypeName = "Results";
			break;		
	}
	DisplayLoading(true);
	// display the correct tab
	tabStrip = $find(MPCONTENT_PREFIX + "TabStrip");
	if (currentJobStepXml == Target.Abacus.Library.Core.JobStepXml.Output) {
	    tabStrip.set_activeTabIndex(0);
	    // clear the requested view
	    GetElement(TAB_PANEL_PREFIX + "1").innerHTML = "Loading...";
	    // fetch step Xml
	    FetchJobStepXml(currentJobStepID, currentJobStepXml);
	}
}

/* JOB STEP XML METHODS */
function FetchJobStepXml(jobStepID, jobStepXml) {
	jobSvc.FetchJobStepXml(jobStepID, jobStepXml, FetchJobStepXml_Callback);
}

function FetchJobStepXml_Callback(response) {
    var tabStrip = $find(MPCONTENT_PREFIX + "TabStrip");
    var activeTab = GetElement(TAB_PANEL_PREFIX + "1");
	if(!response.value.ErrMsg.Success) {
		DisplayError(response.value.ErrMsg, jobSvc.url);
	} else {
		if(response.value.Data.length == 0) {
			activeTab.innerHTML = "No further information is available.";
		} else {
			activeTab.innerHTML = response.value.Data;
		}
	}
	DisplayLoading(false);
}
