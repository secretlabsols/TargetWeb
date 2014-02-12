var CommProfile_btnViewID, CommProfile_btnView;
var CommProfile_btnReportID, CommProfile_btnReport;
var optCommTypeID, optServClassID, optServTypeID;
var optCommType, optServClass, optServType;
var cboOutputID, cboSvcClassID, cboOutput, cboSvcClass;
var divDownloadContainerID;
var dateFromValue, dateToValue;
var dateFromChanged, dateToChanged;

function Init() {
    CommProfile_btnView = GetElement(CommProfile_btnViewID + '_btnReports', true);
    CommProfile_btnReport = GetElement(CommProfile_btnReportID, true);
    optCommType = GetElement(optCommTypeID);
    optServClass = GetElement(optServClassID);
    optServType = GetElement(optServTypeID);
    cboOutput = GetElement(cboOutputID + "_cboDropDownList");
    cboSvcClass = GetElement(cboSvcClassID + "_cboDropDownList");

    CommProfile_btnView.style.display = "none";
    ReportsButton_AddParam(CommProfile_btnView.id, "DateFrom", dateFromValue);
    ReportsButton_AddParam(CommProfile_btnView.id, "DateTo", dateToValue);

    optType_Click();
    cboSvcClassification_Changed();
}

function cboSvcClassification_Changed() {
    ReportsButton_AddParam(CommProfile_btnView.id, "svcClassificationID", cboSvcClass.value);
}

function dteDateFrom_Changed(id) {
    var dteDateFrom = GetElement(id + "_txtTextBox");

    dateFromChanged = true;

    ReportsButton_AddParam(CommProfile_btnView.id, "DateFrom", dteDateFrom.value);
    dateFromValue = dteDateFrom.value;

    CommProfile_btnReport.disabled = true;
    if (dateToValue.toDate() >= dateFromValue.toDate()) {
        CommProfile_btnReport.disabled = false;
    }
}

function dteDateTo_Changed(id) {
    var dteDateTo = GetElement(id + "_txtTextBox");

    dateToChanged = true;

    ReportsButton_AddParam(CommProfile_btnView.id, "DateTo", dteDateTo.value);
    dateToValue = dteDateTo.value;

    CommProfile_btnReport.disabled = true;

    if (dateToValue.toDate() >= dateFromValue.toDate()) {
        CommProfile_btnReport.disabled = false;
    }
}

function optType_Click() {
    if (optCommType.checked == true) {
        cboSvcClass.disabled = true;
    } else if (optServClass.checked == true) {
        cboSvcClass.disabled = true;
    } else if (optServType.checked == true) {
        cboSvcClass.disabled = false;
    }
}

// ***************  Code for the Report Breakdowns ***********************

function CommitmentProfile_All() {
    ReportsButton_AddParam(CommProfile_btnView.id, "ReportType", "1");
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommitmentProfile_CommSvc() {
    ReportsButton_AddParam(CommProfile_btnView.id, "ReportType", "2");
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}
         
function CommitmentProfile_DirectPayment() {
    ReportsButton_AddParam(CommProfile_btnView.id, "ReportType", "3");
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommitmentProfile_ActualCommSvc() {
    ReportsButton_AddParam(CommProfile_btnView.id, "ReportType", "4");
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommitmentProfile_ForecastedCommSvc() {
    ReportsButton_AddParam(CommProfile_btnView.id, "ReportType", "5");
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommitmentProfile_ActualDirectPayment() {
    ReportsButton_AddParam(CommProfile_btnView.id, "ReportType", "6");
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommitmentProfile_ForecastedDirectPayment() {
    ReportsButton_AddParam(CommProfile_btnView.id, "ReportType", "7");
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommSvcClassSvcType_All() {
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterType", "1");
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterValue", '');
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommSvcClassSvcType_ServiceClassification(SvcClassification) {
     ReportsButton_AddParam(CommProfile_btnView.id, "FilterType", "2");
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterValue", SvcClassification);
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommSvcClassSvcType_ServiceType(SvcType) {
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterType", "3");
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterValue", SvcType);
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommSvcTypeBudgCat_All() {
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterType", "1");
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterValue", '');
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommSvcTypeBudgCat_ServiceType(svcType) {
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterType", "2");
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterValue", svcType);
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

function CommSvcTypeBudgCat_BudgetCategory(BudgetCat) {
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterType", "3");
    ReportsButton_AddParam(CommProfile_btnView.id, "FilterValue", BudgetCat);
    OpenReport(ReportsButton_GetMenuUrlByIndex(CommProfile_btnView.id, cboOutput.value), divDownloadContainerID);
}

// ***************  END breakdown Code ***********************

addEvent(window, "load", Init);