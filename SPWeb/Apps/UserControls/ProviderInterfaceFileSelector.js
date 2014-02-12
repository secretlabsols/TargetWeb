var paymentsSvc, tblInterfaceFiles, divPagingLinks, btnCapFormat, btnTargetFormat, currentPage, providerID, dateFrom, dateTo, selectedLogID;
var tblInterfaceFilesID = "ListInterfaceFile";
var divPagingLinksID = "InterfaceFile_PagingLinks";
var btnCapFormatID = "btnCapFormat";
//var btnTargetFormatID = "btnTargetFormat";

function Init() {
	paymentsSvc = new Target.SP.Web.Apps.WebSvc.Payments_class();
	tblInterfaceFiles = GetElement(tblInterfaceFilesID);
	divPagingLinks = GetElement(divPagingLinksID);
	btnCapFormat = GetElement(btnCapFormatID);
	//btnTargetFormat = GetElement(btnTargetFormatID);
	// populate table
	FetchInterfaceFileList(currentPage);
}

/* FETCH INTERFACE FILE LIST METHODS */
function FetchInterfaceFileList(page) {
	currentPage = page;
	DisplayLoading(true);
	btnCapFormat.disabled = true;
	//btnTargetFormat.disabled = true;
	paymentsSvc.FetchProviderInterfaceFileList(page, providerID, dateFrom, dateTo, FetchInterfaceFileList_Callback)
}
function FetchInterfaceFileList_Callback(response) {
	var interfaceFiles, counter, tr, td, radioButton;
	
	if(CheckAjaxResponse(response, paymentsSvc.url)) {
		interfaceFiles = response.value.InterfaceFiles;
		
		btnCapFormat.disabled = (interfaceFiles == 0);
		//btnTargetFormat.disabled = (interfaceFiles == 0);

		ClearTable(tblInterfaceFiles);
		for(counter=0; counter<interfaceFiles.length; counter++) {
			tr = AddRow(tblInterfaceFiles);
			// selector
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "InterfaceFileSelect", interfaceFiles[counter].ID, RadioButton_Click);
			// file number
			AddCell(tr, interfaceFiles[counter].FileNumber);
			// dates
			AddCell(tr, Date.strftime("%d/%m/%Y", interfaceFiles[counter].DateFrom));
			AddCell(tr, Date.strftime("%d/%m/%Y", interfaceFiles[counter].DateTo));
			// record count
			AddCell(tr, interfaceFiles[counter].RemittanceCount);
			// total value
			td = AddCell(tr, "");
			td.innerHTML = interfaceFiles[counter].TotalValue.toString().formatCurrency();
			// select the file?
			if(interfaceFiles.length == 1) {
				radioButton.click();
			}
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}

function RadioButton_Click() {
	var x
	var Radio
	for (x = 0; x < tblInterfaceFiles.tBodies[0].rows.length; x++){
		Radio = tblInterfaceFiles.tBodies[0].rows[x].childNodes[0].getElementsByTagName("INPUT")[0];
		if (Radio.checked) {
			tblInterfaceFiles.tBodies[0].rows[x].className = "highlightedRow"
			selectedLogID = Radio.value;
		} else {
			tblInterfaceFiles.tBodies[0].rows[x].className = ""
		}
	}
}

function btnDownload_OnClick(format) {
	if(selectedLogID == undefined) {
		alert("Please select an interface file.");
		return;
	}
	var url = "GetProviderInterfaceFile.aspx?format=" + format + "&logID=" + selectedLogID;
	document.location.href = url;
}
