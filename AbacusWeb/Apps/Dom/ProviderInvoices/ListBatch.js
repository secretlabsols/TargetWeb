
var contractSvc, currentPage, currentDPIBatchID, jobID;
var listFilter, listCreatedBy = "", listCreated = "";
var tblBatches, divPagingLinks, batches;
var btnReports, btnRecreateFiles;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	tblBatches = GetElement("tblBatches");
	divPagingLinks = GetElement("DomProviderInvoiceBatch_PagingLinks");
	btnRecreateFiles = GetElement("btnRecreateFiles", true);
	btnReports = GetElement("btnReports")
	
	if(btnRecreateFiles) btnRecreateFiles.disabled = true;
    btnReports.disabled = true;
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListDPIBatchFilter_Callback);
	listFilter.AddColumn("Created By", GetElement("thCreatedBy"));
	listFilter.AddColumn("Created", GetElement("thCreated"));
			
	// populate table
	listCreatedBy = GetQSParam(document.location.search, "createdBy");
	currentDPIBatchID = GetQSParam(document.location.search, "id");
	jobID = GetQSParam(document.location.search, "jobid");
	FetchDomProviderInvoiceBatchList(currentPage, currentDPIBatchID);
}

function ListDPIBatchFilter_Callback(column) {
	switch(column.Name) {
		case "Created By":
			listCreatedBy = column.Filter;
			break;
		case "Created":
			listCreated = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchDomProviderInvoiceBatchList(1, 0);
}

function FetchDomProviderInvoiceBatchList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;

	contractSvc.FetchDomProviderInvoiceBatchList(page, selectedID, 
	    listCreatedBy, listCreated, FetchDomProviderInvoiceBatchList_Callback)
}

function FetchDomProviderInvoiceBatchList_Callback(response) {
	var index;
	var tr, td, radioButton, span, but;
	var str;
	var link;
	if(currentDPIBatchID == 0) {
		if(btnRecreateFiles) btnRecreateFiles.disabled = true;
		btnReports.disabled = true;
	}
	
	if(CheckAjaxResponse(response, contractSvc.url)) {
		
		// populate the table
		batches = response.value.Batches;
		
		// remove existing rows
		ClearTable(tblBatches);
		for(index=0; index<batches.length; index++) {
		
			tr = AddRow(tblBatches);
			td = AddCell(tr, "");
			radioButton = AddRadio(td, "", "DomProviderInvoiceBatchSelect", batches[index].ID, RadioButton_Click);
			
			td = AddCell(tr, batches[index].CreatedDate);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, batches[index].CreatedBy);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, batches[index].InvoiceCount);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			td = AddCell(tr, batches[index].InvoiceValueNet);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, batches[index].InvoiceValueVAT);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";		
			
			td = AddCell(tr, batches[index].InvoiceValueGross);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			td = AddCell(tr, "");
			td.className = batches[index].LastJobStatusCssClass;
			span = document.createElement("span");
			td.appendChild(span);
			SetInnerText(span, batches[index].LastJobStatus);
			span.style.styleFloat = "left";
            span.style.marginTop = "0.5em";
			if (batches[index].LastJobStatus != "Queued") {
		        // add link to job
		        but = AddImg(td, "../../../Images/Jobs/outputs.png", "View the results of the latest job", "pointer", btnViewJobResults_Click, 
		            new Array(batches[index].ID, batches[index].LastJobID)
		        );
		        but.style.styleFloat = "right";
			}

			if (batches[index].ID == currentDPIBatchID || (currentPage == 1 && batches.length == 1)) {
			    radioButton.click(); 
			}
			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function RadioButton_Click() {
	var index, rdo, hid;
	
	for (index = 0; index < tblBatches.tBodies[0].rows.length; index++){
		rdo = tblBatches.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			tblBatches.tBodies[0].rows[index].className = "highlightedRow"
			currentDPIBatchID = rdo.value;
			jobID = batches[index].LastJobID;
			if (batches[index].InvoiceCount != "0") {
			    btnReports.disabled = false;
			} else {
			    if (btnRecreateFiles) btnRecreateFiles.disabled = true;
			    btnReports.disabled = true;
			}
			if (batches[index].LastJobStatus != "Queued") {
    			if (batches[index].InvoiceCount != "0") {
		            if (btnRecreateFiles) btnRecreateFiles.disabled = false;
			    }
			} else {
		        if (btnRecreateFiles) btnRecreateFiles.disabled = true;
			}
			
		} else {
			tblBatches.tBodies[0].rows[index].className = ""
		}
	}
}

function btnRecreateFiles_Click() {
	document.location.href = "RecreateBatchFiles.aspx?batchid=" + currentDPIBatchID + "&backUrl=" + GetBackUrl();
}

function btnViewJobResults_Click(evt, args) {
	document.location.href = "ViewBatchReports.aspx?batchid=" + args[0] + "&jobid=" + args[1] + "&backUrl=" + GetBackUrl();
}

function btnReports_Click() {
    document.location.href = "BatchReports.aspx?batchid=" + currentDPIBatchID + "&backUrl=" + GetBackUrl();
}

function GetBackUrl() {
	var url = document.location.href;
	if(currentDPIBatchID != 0)
	    url = AddQSParam(RemoveQSParam(url, "id"), "id", currentDPIBatchID);
	return escape(url);
}

function DomProviderInvoiceBatchStep_BeforeNavigate() {
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	// contract is required?
	if(DomProviderInvoiceBatchStep_required && currentDPIBatchID == 0) {
		alert("Please select a domiciliary provider invoice batch.");
		return false;
	}
	
	url = AddQSParam(RemoveQSParam(url, "id"), "id", currentDPIBatchID);
	SelectorWizard_newUrl = url;
	return true;
}

addEvent(window, "load", Init);
