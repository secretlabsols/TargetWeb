var piReturnsSvc, tblPIReturns, divPagingLinks, currentPage, providerID, serviceID, financialYear, quarter, status, selectedSubQueueID;
var tblPIReturnsID = "ListSubmittedPIReturns";
var divPagingLinksID = "PIReturns_PagingLinks";
var btnDownloadFileID = "btnDownloadFile"; 
var btnViewDetailsID = "btnViewDetails";
var piReturns;
var btnDownloadFile, btnViewDetails;

function Init() {
	piReturnsSvc = new Target.SP.Web.Apps.WebSvc.PIReturns_class();
	tblPIReturns = GetElement(tblPIReturnsID);
	divPagingLinks = GetElement(divPagingLinksID);
	btnDownloadFile = GetElement(btnDownloadFileID);
	btnViewDetails = GetElement(btnViewDetailsID);

	// populate table
	FetchPIReturnsList(currentPage);
}

/* FETCH Submitted PI LIST METHODS */
function FetchPIReturnsList(page) {
	currentPage = page;
	DisplayLoading(true);
	piReturnsSvc.FetchSubmittedReturnsList(page, providerID, serviceID, financialYear, quarter, status, FetchPIReturnsList_Callback);
}
function FetchPIReturnsList_Callback(response) {
	var remCounter, tr, td, link, radioButton;
	
	if(CheckAjaxResponse(response, piReturnsSvc.url)) {
		piReturns = response.value.PIReturns;
		
		ClearTable(tblPIReturns);
		for(remCounter=0; remCounter<piReturns.length; remCounter++) {
			tr = AddRow(tblPIReturns);
			// selector
			td = AddCell(tr, "");
			radioButton = AddRadio(td, "", "PIReturnSelect", remCounter, rdoViewPIReturn_OnClick);
			// reference
			AddCell(tr, piReturns[remCounter].ProviderReference);
			AddCell(tr, piReturns[remCounter].ProviderName);
			AddCell(tr, piReturns[remCounter].ServiceReference);
			AddCell(tr, piReturns[remCounter].ServiceName);
			AddCell(tr, piReturns[remCounter].FinancialYear + "/" + piReturns[remCounter].Quarter);
			AddCell(tr, piReturns[remCounter].Status);
		
			
			// select the return?
			if(piReturns.length == 1) {
				radioButton.click();
			}
		}		
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function rdoViewPIReturn_OnClick() {
	var x
	var Radio
	for (x = 0; x < tblPIReturns.tBodies[0].rows.length; x++){
		Radio = tblPIReturns.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblPIReturns.tBodies[0].rows[x].className = "highlightedRow"
			selectedSubQueueID = Radio.value;
		} else {
			tblPIReturns.tBodies[0].rows[x].className = ""
		}
	}
	btnDownloadFile.disabled = false;
	btnViewDetails.disabled = false;
}



function btnViewDetails_Click() {
	var d = new Target.SP.Web.UserControls.PIReturnDetails.Dialog();
	d.SetType(1);
	d.Show();
}

function btnDownloadFile_Click() {
	var fileID = piReturns[selectedSubQueueID].WebFileStoreDataID;
	document.location.href="../../../Apps/FileStore/FileStoreGetFile.axd?fileDataID=" + fileID + "&saveas=1"
}

//*********************************************************************************************************
// PI RETURN DETAILS DIALOG
//*********************************************************************************************************

addNamespace("Target.SP.Web.UserControls.PIReturnDetails.Dialog");

Target.SP.Web.UserControls.PIReturnDetails.Dialog = function() { 
	var strSubDate, strProcDate;
	this.SetTitle("PI Return Details");
	
	strSubDate = Date.strftime("%d/%m/%Y %H:%M:%S", piReturns[selectedSubQueueID].SubmissionDate);
	if (strSubDate.length == 0) strSubDate = " ";
	strProcDate = Date.strftime("%d/%m/%Y %H:%M:%S", piReturns[selectedSubQueueID].ProcessedDate);
	if (strProcDate.length == 0) strProcDate = " ";
	
	// descriptive text
	this.SetContentText("<fieldset> <legend>Comments</legend><div style=\"float:left;height:20em; width:100%; overflow-y:scroll\">" + piReturns[selectedSubQueueID].Comments.replace(/\r\n/g, "<br />\r\n") + "</div></fieldset><br />" +
						"<span style=\"width:10em; float:left; font-weight:bold;\">Submitted by </span><span>" + piReturns[selectedSubQueueID].SubmittedByUser  + "</span><br />" +
						"<span style=\"width:10em; float:left; font-weight:bold;\">Date Submitted </span><span>" + strSubDate + "</span><br />" +
						"<span style=\"width:10em; float:left; font-weight:bold;\">Processed by </span><span>" + piReturns[selectedSubQueueID].ProcessedByUser  + "</span><br />" +
						"<span style=\"width:10em; float:left; font-weight:bold;\">Date Processed </span><span>" + strProcDate + "</span><br />");
	this.SetWidth("39");
	// buttons
	this.ClearButtons();
	this.AddButton("OK", "", this._callback, new Array(this, 1));
}

// inherit from base
Target.SP.Web.UserControls.PIReturnDetails.Dialog.prototype = new Target.Web.Dialog();