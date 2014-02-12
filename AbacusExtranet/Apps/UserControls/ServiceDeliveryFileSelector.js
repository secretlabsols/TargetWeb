var serviceDeliveryFileSvc, currentPage, fileID, submittedByUserID, dateFrom, dateTo, deleted, processed, workInProgress, awaitingProcessing, failed, userHasServiceFileUploadCommand;
var tblServiceDeliveryFile, divPagingLinks, btnViewFile, btnNewFile;
var listFilter, listFilterReference = "", listFilterDescription = "";

function Init() {
	serviceDeliveryFileSvc = new Target.Abacus.Extranet.Apps.WebSvc.ServiceDeliveryFile_class();
	tblServiceDeliveryFile = GetElement("ListServiceDeliveryFiles");
	divPagingLinks = GetElement("Detail_PagingLinks");
	btnViewFile = GetElement("btnViewServiceDeliveryFile");
	btnNewFile = GetElement("btnNewServiceDeliveryFile");

	if (userHasServiceFileUploadCommand) {
	    btnNewFile.style.display = "block";
	 }
	else {
	    btnNewFile.style.display = "none";
	 }
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Reference", GetElement("thRef"));
	listFilter.AddColumn("File Description", GetElement("thDesc"));
	
	// populate table
	FetchServiceDeliveryFileList(currentPage);
}

function ListFilter_Callback(column) {
	switch(column.Name) {
		case "Reference":
			listFilterReference = column.Filter;
			break;
		case "File Description":
			listFilterDescription = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchServiceDeliveryFileList(1);
}

/* FETCH OCCUPANCY LIST METHODS */
function FetchServiceDeliveryFileList(page) {
	currentPage = page;
	DisplayLoading(true);
	btnViewFile.disabled = true;
	serviceDeliveryFileSvc.FetchServiceDeliveryFileList(
	    page, 
	    submittedByUserID, 
	    dateFrom, 
	    dateTo, 
	    deleted, 
	    workInProgress, 
	    processed, 
	    awaitingProcessing, 
	    listFilterReference,
	    listFilterDescription,
	    failed,
	    FetchServiceDeliveryFileList_Callback)	    
}

function FetchServiceDeliveryFileList_Callback(response) {
	var files, remCounter, tr, td, link, radioButton;
	var str;
	var viewUrl = "ViewServiceDeliveryFile.aspx?backUrl=" + GetBackUrl() + "&id=";

	btnViewFile.disabled = true;
	
	if(CheckAjaxResponse(response, serviceDeliveryFileSvc.url)) {
		files = response.value.ServiceDeliveryFile;
		ClearTable(tblServiceDeliveryFile);
		for(remCounter=0; remCounter<files.length; remCounter++) {
			tr = AddRow(tblServiceDeliveryFile);
			
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "FileSelect", files[remCounter].ID, RadioButton_Click);
			
			td = AddCell(tr, "");
			link = AddLink(td, files[remCounter].Reference, viewUrl + files[remCounter].ID, "Click here to view this files details.");
			link.className = "transBg";
			
			td = AddCell(tr, "");
			link = AddLink(td, files[remCounter].FileDescription, viewUrl + files[remCounter].ID, "Click here to view this files details.");
			link.className = "transBg";
			
			AddCell(tr, files[remCounter].SubmittedBy); 
			AddCell(tr, Date.strftime("%d/%m/%Y", files[remCounter].UploadedOn));
			AddCell(tr, files[remCounter].Status);

			if (fileID == files[remCounter].ID || (currentPage == 1 && files.length == 1))
			    radioButton.click();
			
		}		
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var x
	var Radio
	for (x = 0; x < tblServiceDeliveryFile.tBodies[0].rows.length; x++){
		Radio = tblServiceDeliveryFile.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblServiceDeliveryFile.tBodies[0].rows[x].className = "highlightedRow"
			fileID = Radio.value;
			btnViewFile.disabled = false;
		} else {
			tblServiceDeliveryFile.tBodies[0].rows[x].className = ""
		}
	}
}

function btnViewServiceDeliveryFile_Click() {
	if(fileID == 0)
		alert("Please select a File.");
	else
		document.location.href = "ViewServiceDeliveryFile.aspx?id=" + fileID + "&backUrl=" + GetBackUrl();
}

function btnNewServiceDeliveryFile_Click() {
    document.location.href = "UploadServiceDeliveryFile.aspx?backUrl=" + GetBackUrl();
}

function GetBackUrl() {
    var url = document.location.href;
    url = escape(url);
    return url
}