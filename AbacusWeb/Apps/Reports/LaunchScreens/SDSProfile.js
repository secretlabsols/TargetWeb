var DateRange_btnViewID, DateRange_btnView;
var optShowAllID, optPermResOnlyID, optExcludePermResID;
var optShowAll, optPermResOnly, optExcludePermRes;
var cboOutputID, cboOutput;
var divDownloadContainerID;

function Init() {
    DateRange_btnView = GetElement(DateRange_btnViewID + '_btnReports', true);
    optShowAll = GetElement(optShowAllID);
    optPermResOnly = GetElement(optPermResOnlyID);
    optExcludePermRes = GetElement(optExcludePermResID);
    cboOutput = GetElement(cboOutputID + "_cboDropDownList");
    DateRange_btnView.style.display = "none";
    optType_Click();
}



function SDSProfile_SDS() {
    ReportsButton_AddParam(DateRange_btnView.id, "ReportType", "2");
    OpenReport(ReportsButton_GetMenuUrlByIndex(DateRange_btnView.id, cboOutput.value), divDownloadContainerID);
}

function SDSProfile_NonSDS() {
    ReportsButton_AddParam(DateRange_btnView.id, "ReportType", "3");
    OpenReport(ReportsButton_GetMenuUrlByIndex(DateRange_btnView.id, cboOutput.value), divDownloadContainerID);
}

function SDSProfile_SDSOnly() {
    ReportsButton_AddParam(DateRange_btnView.id, "ReportType", "4");
    OpenReport(ReportsButton_GetMenuUrlByIndex(DateRange_btnView.id, cboOutput.value), divDownloadContainerID);
}

function SDSProfile_ConvertedSDS() {
    ReportsButton_AddParam(DateRange_btnView.id, "ReportType", "5");
    OpenReport(ReportsButton_GetMenuUrlByIndex(DateRange_btnView.id, cboOutput.value), divDownloadContainerID);
}

function SDSProfile_NonSDSOnly() {
    ReportsButton_AddParam(DateRange_btnView.id, "ReportType", "6");
    OpenReport(ReportsButton_GetMenuUrlByIndex(DateRange_btnView.id, cboOutput.value), divDownloadContainerID);
}

function SDSProfile_ConvertedNonSDS() {
    ReportsButton_AddParam(DateRange_btnView.id, "ReportType", "7");
    OpenReport(ReportsButton_GetMenuUrlByIndex(DateRange_btnView.id, cboOutput.value), divDownloadContainerID);
}

function SDSProfile_AllSvcUsers() {
    ReportsButton_AddParam(DateRange_btnView.id, "ReportType", "1");
    OpenReport(ReportsButton_GetMenuUrlByIndex(DateRange_btnView.id, cboOutput.value), divDownloadContainerID);
}

function optType_Click() {
    if (optShowAll.checked == true) {
        ReportsButton_RemoveParam(DateRange_btnView.id, "ShowPermRes");
    } else if (optPermResOnly.checked == true) {
        ReportsButton_AddParam(DateRange_btnView.id, "ShowPermRes", true);
    } else if (optExcludePermRes.checked == true) {
        ReportsButton_AddParam(DateRange_btnView.id, "ShowPermRes", false);
    }
}

addEvent(window, "load", Init);