
var jobSvc, jobListCreatedBy, jobListStatusID, jobTypeID, jobListPage, currentJobID, currentJobStepID, currentJobStepXml;
var canCreateJobs, canCopyJobs, canCancelJobs, canDeleteJobs;
var steps, currentJobStepNumber, maxJobStepNumber;
var firedActiveTabChanged = false;
var TAB_PANEL_PREFIX = "TabStrip_TabPanel";
var DATE_FORMAT = "%d/%m/%y %H:%M:%S";

function Init() {
	jobSvc = new Target.Abacus.Web.Apps.Jobs.WebSvc.JobService_class();
}

function ActiveTabChanged(sender, e) {
    var tabIndex = sender.get_activeTabIndex() + 1;
    var btnName;
    switch(tabIndex) {
        case Target.Abacus.Library.Core.JobStepXml.Input:
            btnName = "btnInputs";
            break;
        case Target.Abacus.Library.Core.JobStepXml.Progress:
            btnName = "btnProgress";
            break;
        case Target.Abacus.Library.Core.JobStepXml.Output:
            btnName = "btnResults";
            break;
    }
    if(ie) {
        GetElement(btnName + currentJobStepID).click();
    } else {
        // FireFox seems to fire the ActiveTabChanged() event infinitely.
        // Hence we work around that here.
        if(firedActiveTabChanged) {
            firedActiveTabChanged = false;
            return;
        }
        firedActiveTabChanged = true;
        fireEvent(GetElement(btnName + currentJobStepID), "click");
    }
}

/* JOB LIST METHODS */
function FetchJobList(createdBy, statusID, typeID, page) {
	DisplayLoading(true);
	jobListCreatedBy = createdBy;
	jobListStatusID = statusID;
	jobTypeID = typeID;
	if (!page) page = 1;
	jobListPage = page;
	jobSvc.FetchJobList((!createdBy || createdBy.length==0 ? null : createdBy), statusID, typeID, page, FetchJobList_Callback);
}
function FetchJobList_Callback(response) {

	if(CheckAjaxResponse(response, jobSvc.url)) {
		var jobListTable = GetElement("JobList_Table");
		var jobs = response.value.Jobs;
		var tr, td, but;
		
		// remove existing rows
		ClearTable(jobListTable);
		// for each job, add a row to the table
		for(i=0; i<jobs.length; i++) {
			tr = AddRow(jobListTable);
			td = AddCell(tr, jobs[i].JobName);
			td.style.width = "25em";
			td.title = jobs[i].CreatorComment;
			if (jobs[i].JobStatusID == Target.Abacus.Library.Core.JobStatus.InProgress) {
				// progress info
				var start = jobs[i].StartDateTime;
				var now = new Date();
				var hours = start.diff("h", now);
				var mins = start.diff("n", now) - (hours * 60);
				var secs = start.diff("s", now) - (hours * 60 * 60) - (mins * 60);
				td = AddCell(tr, "Step " + jobs[i].CurrentStep + " of " + jobs[i].TotalSteps);
				td.title = "Step " + jobs[i].CurrentStep + " of " + jobs[i].TotalSteps + " is currently being processed. " +
					"Total running time " + lPadStr(hours, 2, "0") + ":" + lPadStr(mins, 2, "0") + ":" + lPadStr(secs, 2, "0");
			} else {
				td = AddCell(tr, jobs[i].JobStatusName);
				td.title = jobs[i].JobStatusDesc;						
			}
			td.className = jobs[i].JobStatusCssClass;
			AddCell(tr, jobs[i].CreatedBy);
			AddCell(tr, Date.strftime(DATE_FORMAT, jobs[i].ScheduledStartDateTime));
			td = AddCell(tr, " ");
			if(jobs[i].StartDateTime.getFullYear() > 1901) SetInnerText(td, Date.strftime(DATE_FORMAT, jobs[i].StartDateTime));
			td = AddCell(tr, " ");
			if(jobs[i].EndDateTime.getFullYear() > 1901) SetInnerText(td, Date.strftime(DATE_FORMAT, jobs[i].EndDateTime));
			td = AddCell(tr, "")
			
			but = AddImg(td, "../../Images/Jobs/steps.png", "View the steps for this job", "pointer", JobList_ViewSteps, 
				new Array(
					jobs[i].ID, 
					jobs[i].JobName, 
					jobs[i].CreatorComment)
				);
			
			if(canDeleteJobs) {
			    if(jobs[i].CanDelete) {
                    but.style.marginRight = "0.25em";
				    but = AddImg(td, "../../Images/Jobs/delete.png", "Delete this job", "pointer", JobList_DeleteJob, jobs[i].ID);
			    }
            }
            if(canCancelJobs) {
			    if(jobs[i].CanCancel) {
                    but.style.marginRight = "0.25em";
				    but = AddImg(td, "../../Images/Jobs/cancel.png", "Cancel this job", "pointer", JobList_CancelJob, jobs[i].ID);
			    }
            }
            if(canCopyJobs && jobs[i].AllowManualCreate) {
                    if ((jobs[i].JobStatusID == Target.Abacus.Library.Core.JobStatus.Complete) ||
			            (jobs[i].JobStatusID == Target.Abacus.Library.Core.JobStatus.Warnings) ||
			            (jobs[i].JobStatusID == Target.Abacus.Library.Core.JobStatus.Exceptions) ||
			            (jobs[i].JobStatusID == Target.Abacus.Library.Core.JobStatus.Cancelled) ||
			            (jobs[i].JobStatusID == Target.Abacus.Library.Core.JobStatus.Failed)) {
			        but.style.marginRight = "0.25em";
				    but = AddImg(td, "../../Images/Jobs/createnew.png", "Take a copy of this job and submit it for processing", "pointer", JobList_CopyJob, jobs[i].ID);
			    }
            }
		}
		// load the paging link HTML
		GetElement("JobList_PagingLinks").innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function RefreshJobList() {
	FetchJobList(jobListCreatedBy, jobListStatusID, jobTypeID, jobListPage);
}
function FilterJobList() {
	FetchJobList(GetElement("cboUser_cboDropDownList").value, GetElement("cboJobStatus_cboDropDownList").value, GetElement("cboJobName_cboDropDownList").value, 1);
}
function JobList_ViewSteps(evt, args) {
	var jobName = args[1];
	var jobComment = args[2];
	if(!jobComment) jobComment = "";
	// set the heading
	SetInnerText(GetElement("JobStepList_JobDetails"), jobName + (jobComment.length>0 ? " - " + jobComment : ""));
	// view the steps list
	ChangePageView("JobStepList");
	// fetch the steps
	currentJobID = args[0];
	FetchJobStepList(currentJobID);
}
function JobList_DeleteJob(evt, args) {
	var jobID = args;
	if(window.confirm("Are you sure you wish to delete this job?")) {
		DisplayLoading(true);
		jobSvc.DeleteJob(jobID, JobList_DeleteJob_Callback);
	}
}
function JobList_DeleteJob_Callback(response) {
	if(!response.value.Success) {
		DisplayError(response.value, jobSvc.url);
	} else {
		// refresh the job list after deleting
		// using setTimeout() as AJAX.NET has problems chaining callbacks
		window.setTimeout(RefreshJobList, 100);
	}
}
function JobList_CancelJob(evt, args) {
	var jobID = args;
	if(window.confirm("Are you sure you wish to cancel this job?")) {
		DisplayLoading(true);
		jobSvc.CancelJob(jobID, JobList_CancelJob_Callback);
	}
}
function JobList_CancelJob_Callback(response) {
	if(!response.value.Success) {
		DisplayError(response.value, jobSvc.url);
	} else {
		// refresh the job list after cancelling
		// using setTimeout() as AJAX.NET has problems chaining callbacks
		window.setTimeout(RefreshJobList, 100);
	}
}
function JobList_CopyJob(evt, args) {
	var jobID = args;
	if(window.confirm("Are you sure you wish to take a copy of this job and submit it for processing?")) {
		DisplayLoading(true);
		jobSvc.CopyJob(jobID, JobList_CopyJob_Callback);
	}
}
function JobList_CopyJob_Callback(response) {
	if(!response.value.Success) {
		DisplayError(response.value, jobSvc.url);
	} else {
		// refresh the job list after copying
		// using setTimeout() as AJAX.NET has problems chaining callbacks
		window.setTimeout(RefreshJobList, 100);
	}
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
		var jobStepListTable = GetElement("JobStepList_Table");
		steps = response.value.Steps;
		var tr, td, but, prog;
		
		// remove existing rows
		ClearTable(jobStepListTable);
		jobSteps = new Array(steps.length);
		maxJobStepNumber = steps.length;
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
			td = AddCell(tr, " ");
			if(steps[i].StartDateTime.getFullYear() > 1901) SetInnerText(td, Date.strftime(DATE_FORMAT, steps[i].StartDateTime));
			td.className = "nowrap";
			td = AddCell(tr, " ");
			if(steps[i].EndDateTime.getFullYear() > 1901) SetInnerText(td, Date.strftime(DATE_FORMAT, steps[i].EndDateTime));
			td.className = "nowrap";
			td = AddCell(tr, "")
			
			but = AddImg(td, "../../Images/Jobs/inputs.png", "View the inputs given to this step.", "pointer", JobStepList_ViewXml, 
				new Array(
					steps[i].ID, 
					steps[i].StepNumber, 
					steps[i].JobStepTypeName,
					Target.Abacus.Library.Core.JobStepXml.Input)
				);
			but.id = "btnInputs" + steps[i].ID;
				
			but = AddImg(td, "../../Images/Jobs/progress.png", "View the detailed progress of this step.", "pointer", JobStepList_ViewXml, 
				new Array(
					steps[i].ID, 
					steps[i].StepNumber, 
					steps[i].JobStepTypeName,
					Target.Abacus.Library.Core.JobStepXml.Progress)
				);
			but.id = "btnProgress" + steps[i].ID;
			but.style.marginLeft = "0.25em";
			but.style.marginRight = "0.25em";
			
			but = AddImg(td, "../../Images/Jobs/outputs.png", "View the results of this step.", "pointer", JobStepList_ViewXml, 
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
	GetElement(TAB_PANEL_PREFIX + "2").innerHTML = "Loading...";
	GetElement(TAB_PANEL_PREFIX + "3").innerHTML = "Loading...";
}

function PrevJobStepXml() {
    if(currentJobStepNumber == 1) return;
    var prevStep = steps[currentJobStepNumber - 2];
    var args = new Array(
					prevStep.ID, 
					prevStep.StepNumber, 
					prevStep.JobStepTypeName, 
					currentJobStepXml
				);
    JobStepList_ViewXml(null, args);
}
function NextJobStepXml(evt, args) {
    if(currentJobStepNumber == steps.length) return;
    var nextStep = steps[currentJobStepNumber];
    var args = new Array(
					nextStep.ID, 
					nextStep.StepNumber, 
					nextStep.JobStepTypeName, 
					currentJobStepXml
				);
    JobStepList_ViewXml(null, args);
}

function JobStepList_ViewXml(evt, args) {
	currentJobStepID = args[0];
	currentJobStepNumber = args[1];
	var jobStepTypeDesc = args[2];
	currentJobStepXml = args[3];
	var xmlTypeName;
	var tabStrip;
	
	// work out the type of Xml
	switch(currentJobStepXml) {
	    case Target.Abacus.Library.Core.JobStepXml.Input:
			xmlTypeName = "Input";
			break;
        case Target.Abacus.Library.Core.JobStepXml.Progress:
			xmlTypeName = "Progress";
			break;
        case Target.Abacus.Library.Core.JobStepXml.Output:
			xmlTypeName = "Results";
			break;		
	}
	DisplayLoading(true);
	// display the correct tab
	tabStrip = $find(MPCONTENT_PREFIX + "TabStrip");
	tabStrip.set_activeTabIndex(currentJobStepXml - 1);
	// clear the requested view
	GetElement(TAB_PANEL_PREFIX + currentJobStepXml).innerHTML = "Loading...";
	// set the title and overview for the type of Xml we are viewing
	var title = GetElement("JobStepXml_PageTitle");
	var overview = GetElement("JobStepXml_PageOverview");
	SetInnerText(title, "Job Step " + xmlTypeName);
	SetInnerText(overview, "Displayed below is the " + xmlTypeName.toLowerCase() + " information for the selected job step.");
	// set the heading
	SetInnerText(GetElement("JobStepXml_JobStepDetails"), "Step " + currentJobStepNumber + " " + jobStepTypeDesc);
	// change view
	ChangePageView("JobStepXml");
	// fetch step Xml
	FetchJobStepXml(currentJobStepID, currentJobStepXml);
}

/* JOB STEP XML METHODS */
function FetchJobStepXml(jobStepID, jobStepXml) {
	jobSvc.FetchJobStepXml(jobStepID, jobStepXml, FetchJobStepXml_Callback);
}
function FetchJobStepXml_Callback(response) {
    var tabStrip = $find(MPCONTENT_PREFIX + "TabStrip");
    var activeTab = GetElement(TAB_PANEL_PREFIX + currentJobStepXml);
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
function RefreshJobStepXml() {
	DisplayLoading(true);
	// clear the requested view
	GetElement(TAB_PANEL_PREFIX + currentJobStepXml).innerHTML = "Loading...";
	// get the data
	FetchJobStepXml(currentJobStepID, currentJobStepXml);
}

/* COPY TO CLIPBOARD */
var COPY_CLIPBOARD_SUCCESS = "The information was successfully copied to the clipboard.\nYou can now paste this information into your word processor (e.g. Microsoft Word) or an email window as normal.";

function CopyJobStepDetails() {
	var stepTitle, inputs, progress, results, text;
	// get the step title
	stepTitle = GetInnerText(GetElement("JobStepXml_JobStepDetails"));
	// get the inputs
	inputs = GetInnerText(GetElement(TAB_PANEL_PREFIX + "1"));
	// get the progress
	progress = GetInnerText(GetElement(TAB_PANEL_PREFIX + "2"));
	// get the results
	results = GetInnerText(GetElement(TAB_PANEL_PREFIX + "3"));
	text = stepTitle + "\n\nINPUTS:\n" + inputs + "\n\nPROGRESS:\n" + progress + "\n\nRESULTS:\n" + results;
	CopyToClipboard(text, COPY_CLIPBOARD_SUCCESS);
}
function CopyAboutDetails() {
	var text = GetInnerText(GetElement("About_Details"));
	CopyToClipboard(text, COPY_CLIPBOARD_SUCCESS);
}

/* ABOUT */
function FetchAbout() {
	DisplayLoading(true);
	// change view
	GetElement("About_Details").innerHTML = "Loading...";
	ChangePageView("About");
	jobSvc.FetchAbout(FetchAbout_Callback);
}
function FetchAbout_Callback(response) {
	if(!response.value.ErrMsg.Success) {
		DisplayError(response.value.ErrMsg, jobSvc.url);
	} else {
		var content = GetElement("About_Details");
		var html, section, elem;
		
		// clear the content
		content.innerHTML = "";
		
		// for each app info section
		for(sectionCounter=0; sectionCounter<response.value.AppInfo.Sections.length; sectionCounter++) {
			section = response.value.AppInfo.Sections[sectionCounter];
			// create section title
			elem = document.createElement("STRONG");
			elem.appendChild(document.createTextNode(section.Name));
			content.appendChild(elem);
			content.appendChild(document.createElement("BR"));
			
			// output each detail line
			for(detailCounter=0; detailCounter<section.Details.length; detailCounter++) {
				content.appendChild(document.createTextNode(section.Details[detailCounter]));
				content.appendChild(document.createElement("BR"));
			}			
			content.appendChild(document.createElement("BR"));
		}
		
	}
	DisplayLoading(false);
}

