
var ReportViewer_ClientID, ReportViewer_svc;

function Init() {
    ReportViewer_svc = new Target.Web.Apps.Reports.WebSvc.Reports_class();
    RemoveScrollbars();
}

function ResizeReport() {
    var viewer = GetElement(ReportViewer_ClientID);
    viewer.PreRender = PreRenderResize()
}

function RemoveScrollbars() {
    window.st
    var viewer = GetElement(ReportViewer_ClientID);
    viewer.PreRender = PreRenderResize()
}

function PreRenderResize()
{
    var viewer = GetElement(ReportViewer_ClientID);
    var htmlheight = document.documentElement.clientHeight;
    viewer.style.height = (htmlheight - 30) + "px";
}

function ClearSession() {
    ReportViewer_svc.ClearSession();
}

function ReportViewer_NotifyRowLimit(rowLimit) {
    var msg = "This report is limited to " + rowLimit + " rows and the parameters you have chosen returns data that exceeds this limit.<br /><br />" +
        "Not all of the available data will be displayed by this report.<br /><br />" +
        "Please restrict your parameters further to ensure all of the data is reported.";
    var d = new Target.Web.Dialog.Msg();
    d.SetWidth(40);
	d.SetTitle("Warning");
	d.SetContentText(msg);
	d.Show();
}

addEvent(window, "load", Init);
addEvent(window, "resize", ResizeReport);
addEvent(window, "unload", ClearSession);