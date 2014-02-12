
var Report_lstAvailableReports, Report_fsSelectedReport, Report_fsSelectedReportLegend;
var Report_selectedReport;
var Report_lstReportlistId;

function Init() {
    Report_lstAvailableReports = GetElement(Report_lstReportlistId, true);
    Report_fsSelectedReport = GetElement("fsSelectedReport", true);
    if(Report_fsSelectedReport) Report_fsSelectedReportLegend = Report_fsSelectedReport.getElementsByTagName("legend")[0];
}

function lstReports_Change() {
    var selectedIndex = Report_lstAvailableReports.options.selectedIndex;
    Report_selectedReport = Report_lstAvailableReports.options[selectedIndex];
    var divReport = GetElement(Report_selectedReport.value);
    var allReportDivs = Report_fsSelectedReport.getElementsByTagName("div");
       
    for(index=0; index<allReportDivs.length; index++) {
        allReportDivs[index].style.display = "none";
    }
        
    SetInnerText(Report_fsSelectedReportLegend, Report_selectedReport.text);
    divReport.style.display = "block";
}

addEvent(window, "load", Init);