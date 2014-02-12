
var reportSvc, currentPage, ReportsSelector_selectedID, ReportsSelector_selectedLaunchScreenUrl;
var listFilter, listFilterDesc = "", listFilterPath = "", listFilterCategories = "";
var tblReports, divPagingLinks, btnView;
var ReportsSelector_launchScreenHeight = 0, ReportsSelector_launchScreenWidth = 0;

function Init() {
	reportSvc = new Target.Web.Apps.Reports.WebSvc.Reports_class();
	tblReports = GetElement("tblReports");
	divPagingLinks = GetElement("divPagingLinks");
	btnView = GetElement("btnView");
	
	btnView.disabled = true;
		
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Description", GetElement("thDesc"));
	listFilter.AddColumn("Path", GetElement("thPath"));
	listFilter.AddColumn("Categories", GetElement("thCategories"));

	// populate table
	FetchReportsList(currentPage);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Description":
			listFilterDesc = column.Filter;
			break;
		case "Path":
			listFilterPath = column.Filter;
			break;
		case "Categories":
			listFilterCategories = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchReportsList(1);
}

/* FETCH REPORT LIST METHODS */
function FetchReportsList(page) {
	currentPage = page;
	DisplayLoading(true);
			
    reportSvc.FetchReportsList(page, 
                            listFilterDesc,
                            listFilterPath,
                            listFilterCategories,
                            FetchReportsList_Callback);
}

function FetchReportsList_Callback(response) {
	var reports, reportCounter;
	var tr, td, rdo;
	var str;
	var link;

	btnView.disabled = true;
		
	if(CheckAjaxResponse(response, reportSvc.url)) {
	   
		reports = response.value.Reports;

		// remove existing rows
		ClearTable(tblReports);
		for(reportCounter=0; reportCounter<reports.length; reportCounter++) {
		
			tr = AddRow(tblReports);
			
			td = AddCell(tr, "");			
			rdo = AddRadio(td, "", "ReportSelect", reports[reportCounter].ID, RadioButton_Click);
			AddHidden(td, "", "", reports[reportCounter].LaunchScreenUrl);
			AddHidden(td, "", "", reports[reportCounter].LaunchScreenHeight);
			AddHidden(td, "", "", reports[reportCounter].LaunchScreenWidth);
									
			// desc
			td = AddCell(tr, " ");
			link = AddLink(td, reports[reportCounter].Description, "javascript:ViewReport(" + reports[reportCounter].ID + ", '" + reports[reportCounter].LaunchScreenUrl + "', '" + reports[reportCounter].LaunchScreenHeight + "', '" + reports[reportCounter].LaunchScreenWidth + "');", "");
		    link.className = "transBg";
		    td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
            
            // path
			td = AddCell(tr, reports[reportCounter].Path);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
            
            // categories
            td = AddCell(tr, reports[reportCounter].Categories);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the report?
            if (ReportsSelector_selectedID == reports[reportCounter].ID || (currentPage == 1 && reports.length == 1)) {
                rdo.click();
            }

		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
    var index, rdo, selectedRow, hid1, hid2, hid3;
	for (index = 0; index < tblReports.tBodies[0].rows.length; index++){
		rdo = tblReports.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		hid1 = tblReports.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[1];
		hid2 = tblReports.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[2];
		hid3 = tblReports.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[3];
		if (rdo.checked) {
			selectedRow = tblReports.tBodies[0].rows[index];
			tblReports.tBodies[0].rows[index].className = "highlightedRow"
			ReportsSelector_selectedID = rdo.value;
			ReportsSelector_selectedLaunchScreenUrl = hid1.value;
			ReportsSelector_launchScreenHeight = hid2.value;
			ReportsSelector_launchScreenWidth = hid3.value;
			btnView.disabled = false;
		} else {
			tblReports.tBodies[0].rows[index].className = ""
		}
	}
}

function ViewReport(reportID, launchScreenUrl, launchScreenHeight, launchScreenWidth) {
    var url;
    if(launchScreenUrl && launchScreenUrl.length > 0) {
        url = AddQSParam(launchScreenUrl, "rID", reportID);
        url = SITE_VIRTUAL_ROOT + url;
        OpenPopup(url, launchScreenWidth, launchScreenHeight, 1);
    } else {
        url = SITE_VIRTUAL_ROOT + "Apps/Reports/Viewer.aspx?rID=" + reportID;
        OpenReport(url);
    }
    
}

function btnView_Click() {
    ViewReport(ReportsSelector_selectedID, ReportsSelector_selectedLaunchScreenUrl, ReportsSelector_launchScreenHeight, ReportsSelector_launchScreenWidth);
}

addEvent(window, "load", Init);
