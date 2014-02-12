var batchID, jobID;
var jobSvc, jobStepListTable;
var TAB_PANEL_PREFIX = "TabStrip_TabPanel";
var dateFormat = "%d/%m/%y %H:%M:%S";

function Init() {

    batchID = GetQSParam(document.location.search, "bid");
    jobID = GetQSParam(document.location.search, "jid");
    jobStepListTable = GetElement("JobStepList_Table");
    
    // Get the job step(s) for the passed job ID..
    jobSvc = new Target.Abacus.Web.Apps.Jobs.WebSvc.JobService_class();
    FetchJobStepList(jobID);

}

function FetchJobStepList(jobID) {

    DisplayLoading(true);

    if (jobID > 0) {

        jobSvc.FetchJobStepList(jobID, FetchJobStepList_Callback);

    } else {

        ClearTable(jobStepListTable);
        DisplayLoading(false);

    } 

}

function FetchJobStepList_Callback(response) {

    if (CheckAjaxResponse(response, jobSvc.url)) {

        var jobStepListTable = GetElement("JobStepList_Table");
        var steps = response.value.Steps;
        var stepsCount = steps.length;
        var tr, td, but, prog, currentStep;

        // remove existing rows
        ClearTable(jobStepListTable);
        
        // for each job, add a row to the table
        for (i = 0; i < stepsCount; i++) {

            currentStep = steps[i];
            
            tr = AddRow(jobStepListTable);
            AddCell(tr, currentStep.StepNumber);
            td = AddCell(tr, currentStep.JobStepTypeName);
            td.title = currentStep.JobStepTypeDesc;
            
            if (currentStep.JobStepStatusID == Target.Abacus.Library.Core.JobStepStatus.InProgress) {
                // display a progress bar and running time info
                
                td = AddCell(tr, "");
                var start = currentStep.StartDateTime;
                var now = new Date();
                var hours = start.diff("h", now);
                var mins = start.diff("n", now) - (hours * 60);
                var secs = start.diff("s", now) - (hours * 60 * 60) - (mins * 60);
                td.title = currentStep.PercentComplete + "% Complete. Running time " +
					lPadStr(hours, 2, "0") + ":" + lPadStr(mins, 2, "0") + ":" + lPadStr(secs, 2, "0");
                prog = new ProgressBar(td, 100, 12);
                prog.setPercent(currentStep.PercentComplete / 100);
                
            } else {
                // display status name
                
                td = AddCell(tr, currentStep.JobStepStatusName);
                td.title = currentStep.JobStepStatusDesc;
                td.className = currentStep.JobStepStatusCssClass;

            }
            
            td = AddCell(tr, Date.strftime(dateFormat, currentStep.StartDateTime));
            td.className = "nowrap";
            td = AddCell(tr, Date.strftime(dateFormat, currentStep.EndDateTime));
            td.className = "nowrap";
            td = AddCell(tr, "")

            but = AddImg(td, "../../../Images/Jobs/outputs.png", "View results", "pointer", JobStepList_ViewXml,
				new Array(
					currentStep.ID,
					currentStep.StepNumber,
					currentStep.JobStepTypeName,
					Target.Abacus.Library.Core.JobStepXml.Output)
				);
            but.id = "btnResults" + currentStep.ID;
            
        }

    }
    
    DisplayLoading(false);
}

function JobStepList_ViewXml(evt, args) {

    var index, stepID;
    var currentJobStepID = args[0];
    var jobStepNumber = args[1];
    var jobStepTypeDesc = args[2];
    var currentJobStepXml = args[3];
    var xmlTypeName;
    var tabStrip;

    // highlight the row containing the clicked on results icon
    for (index = 0; index < jobStepListTable.tBodies[0].rows.length; index++) {
        stepID = jobStepListTable.tBodies[0].rows[index].cells[0].innerHTML;
        if (jobStepNumber == stepID) {
            jobStepListTable.tBodies[0].rows[index].className = "highlightedRow"
        } else {
            jobStepListTable.tBodies[0].rows[index].className = ""
        }
    }

    // work out the type of Xml
    switch (currentJobStepXml) {
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

function FetchJobStepXml(jobStepID, jobStepXml) {
    jobSvc.FetchJobStepXml(jobStepID, jobStepXml, FetchJobStepXml_Callback);
}

function FetchJobStepXml_Callback(response) {
    var tabStrip = $find(MPCONTENT_PREFIX + "TabStrip");
    var activeTab = GetElement(TAB_PANEL_PREFIX + "1");
    if (!response.value.ErrMsg.Success) {
        DisplayError(response.value.ErrMsg, jobSvc.url);
    } else {
        if (response.value.Data.length == 0) {
            activeTab.innerHTML = "No further information is available.";
        } else {
            activeTab.innerHTML = response.value.Data;
        }
    }
    DisplayLoading(false);
}


addEvent(window, "load", Init);