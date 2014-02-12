var piReturnsSvc, tblPISubmissionStatus, divPagingLinks, currentPage, providerID, serviceID 
var financialYearFrom, quarterFrom,financialYearTo, quarterTo, status, selectedSubQueueID;
var tblPISubStatusID = "ListSubmittedPIReturns";
var divPagingLinksID = "PIReturns_PagingLinks";
var btnDownloadFileID = "btnDownloadFile"; 
var btnViewDetailsID = "btnViewDetails";
var piReturns;
var btnDownloadFile, btnViewDetails;

function Init() {
	piReturnsSvc = new Target.SP.Web.Apps.WebSvc.PIReturns_class();
	tblPISubmissionStatus = GetElement(tblPISubStatusID);
	divPagingLinks = GetElement(divPagingLinksID);
	btnDownloadFile = GetElement(btnDownloadFileID, true);
	btnViewDetails = GetElement(btnViewDetailsID, true);

	// populate table
	FetchPISubmissionStatusList(currentPage);
}

/* FETCH PI Submission Status LIST METHODS */
function FetchPISubmissionStatusList(page) {
	currentPage = page;
	DisplayLoading(true);
	piReturnsSvc.FetchPISubmissionStatusList(page, providerID, serviceID, financialYearFrom, quarterFrom, financialYearTo, quarterTo, status, FetchPISubmissionStatusList_Callback);
}
function FetchPISubmissionStatusList_Callback(response) {
	var remCounter, tr, td, link, radioButton;
	if(CheckAjaxResponse(response, piReturnsSvc.url)) {
		piReturns = response.value.PIReturns;
		
		ClearTable(tblPISubmissionStatus);
		for(remCounter=0; remCounter<piReturns.length; remCounter++) {
			tr = AddRow(tblPISubmissionStatus);
			// reference
			AddCell(tr, piReturns[remCounter].ProviderReference);
			AddCell(tr, piReturns[remCounter].ProviderName);
			AddCell(tr, piReturns[remCounter].ServiceReference);
			AddCell(tr, piReturns[remCounter].ServiceName);
			AddCell(tr, piReturns[remCounter].FinancialYear + "/" + piReturns[remCounter].Quarter);
			AddCell(tr, piReturns[remCounter].Status);

		}		
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

