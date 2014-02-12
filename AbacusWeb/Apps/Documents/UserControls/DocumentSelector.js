
var documentSvc, currentPage;
var DocumentSelector_selectedDocumentID = 0;

var serviceUserType = 0;
var documentPrintQueueBatchID = null;
var isPrintQueueScreen = false;

// documentAssociatorID is ID of Service User, Generic Creditor or 
// Third Party Budget Holder
var documentAssociatorID = 0;

var IframeID = null;
var cpFiltersID = null;

var GENERIC_FILE_ICON_IMAGE = "image.gif";
var ALLOWED_FILE_ICON_HEIGHT = 20;

// filter variables
var listFilterDocumentTypes = null;
var listFilterOrigin = null;
var listFilterDescription = null;
var listFilterCreatedFromDate = null;
var listFilterCreatedToDate = null;
var listFilterCreatedBy = null;
var listFilterRecipientReference = null;
var listFilterRecipientName = null;
var listFilterNeverQueued = null;
var listFilterQueued = null;
var listFilterBatched = null;
var listFilterSentToPrinter = null;
var listFilterRemovedFromQueue = null;
var listFilterPrintStatusFromDate = null;
var listFilterPrintStatusToDate = null;
var listFilterPrintStatusBy = null;

// filter controls' IDs
var listFilterDocumentTypesID = null;
var listFilterOriginID = null;
var listFilterDescriptionID = null;
var listFilterCreatedFromDateID = null;
var listFilterCreatedToDateID = null;
var listFilterCreatedByID = null;
var listFilterRecipientReferenceID = null;
var listFilterRecipientNameID = null;
var listFilterNeverQueuedID = null;
var listFilterQueuedID = null;
var listFilterBatchedID = null;
var listFilterSentToPrinterID = null;
var listFilterRemovedFromQueueID = null;
var listFilterPrintStatusFromDateID = null;
var listFilterPrintStatusToDateID = null;
var listFilterPrintStatusByID = null;

var tblDocuments, divPagingLinks;
var btnNew,   btnView,   btnProperties,   btnPrint,   btnRemove;
var btnNewID, btnViewID, btnPropertiesID, btnPrintID, btnRemoveID;

function Init() {
	documentSvc = new Target.Abacus.Web.Apps.WebSvc.Documents_class();

	tblDocuments = GetElement("tblDocuments");
	divPagingLinks = GetElement("Document_PagingLinks");

	btnNew = GetElement(btnNewID, true);
	btnView = GetElement(btnViewID, true);
	btnProperties = GetElement(btnPropertiesID, true);
	btnPrint = GetElement(btnPrintID, true);
	btnRemove = GetElement(btnRemoveID, true);

	EnableControls(false);

	// populate table
	FetchDocumentList(currentPage, DocumentSelector_selectedDocumentID);
}

/* FETCH DOCUMENT LIST METHODS */

function FetchDocumentList(page, selectedID) {
	currentPage = page;
	DisplayLoading(true);
	if(selectedID == undefined) selectedID = 0;

	PopulateFilterVars();

	documentSvc.FetchDocumentList(currentPage, 10, null, serviceUserType,
	                              documentAssociatorID, documentPrintQueueBatchID,
	                              listFilterDocumentTypes, listFilterOrigin,
	                              listFilterDescription, listFilterCreatedFromDate,
	                              listFilterCreatedToDate, listFilterCreatedBy,
	                              listFilterRecipientReference, listFilterRecipientName,
	                              listFilterNeverQueued, listFilterQueued,
	                              listFilterBatched, listFilterSentToPrinter, listFilterRemovedFromQueue,
	                              listFilterPrintStatusFromDate, listFilterPrintStatusToDate,
	                              listFilterPrintStatusBy, FetchDocumentList_Callback)
}

function PopulateFilterVars() {

    listFilterDocumentTypes = GetDocumentTypesXML();

    listFilterOrigin = null;
    if (document.getElementById(listFilterOriginID + '_0')) {
        var originGroup = jQuery('#' + listFilterOriginID + '_0')[0].name;

        listFilterOrigin = jQuery('input:radio[name=' + originGroup + ']:checked').val();
        if (listFilterOrigin == '0') listFilterOrigin = null;
    }

    listFilterDescription = GetFilterValue(listFilterDescriptionID);

    listFilterCreatedFromDate = GetDateFilterValue(listFilterCreatedFromDateID, false);
    listFilterCreatedToDate = GetDateFilterValue(listFilterCreatedToDateID, true);
    listFilterCreatedBy = GetFilterValue(listFilterCreatedByID);

    listFilterRecipientReference = GetFilterValue(listFilterRecipientReferenceID);
    listFilterRecipientName = GetFilterValue(listFilterRecipientNameID);

    listFilterNeverQueued = GetCheckboxFilterValue(listFilterNeverQueuedID);
    listFilterQueued = GetCheckboxFilterValue(listFilterQueuedID) || isPrintQueueScreen;
    listFilterBatched = GetCheckboxFilterValue(listFilterBatchedID);
    listFilterSentToPrinter = GetCheckboxFilterValue(listFilterSentToPrinterID);
    listFilterRemovedFromQueue = GetCheckboxFilterValue(listFilterRemovedFromQueueID);

    listFilterPrintStatusFromDate = GetDateFilterValue(listFilterPrintStatusFromDateID, false);
    listFilterPrintStatusToDate = GetDateFilterValue(listFilterPrintStatusToDateID, true);
    listFilterPrintStatusBy = GetFilterValue(listFilterPrintStatusByID);
}

function GetFilterValue(listFilterControlID){
    if (!document.getElementById(listFilterControlID)) return null;

    if (document.getElementById(listFilterControlID).value.length == 0) return null;

    return document.getElementById(listFilterControlID).value;
}

function GetCheckboxFilterValue(listFilterControlID) {
    if (!document.getElementById(listFilterControlID)) return null;

    return document.getElementById(listFilterControlID).checked;
}

function GetDateFilterValue(listFilterControlID, timeToMidnight) {
    var strDate = GetFilterValue(listFilterControlID);

    if (strDate && !timeToMidnight) return strDate.toDate();

    if (strDate && timeToMidnight) {
        var objDate = strDate.toDate();
        objDate.setHours(23, 59, 0, 0);
        return objDate;
    }

    return null;
}

/*
Selects document types from CheckBoxList and converts them into XML:
	
@documentTypes format as XML:

<documentType>
    <documentTypeID>1</documentTypeID>
    <documentTypeID>2</documentTypeID>
</documentType>	
*/
function GetDocumentTypesXML() {
    var result = "";
    // if no document types then return empty parent node
    if (!document.getElementById(listFilterDocumentTypesID)) return '<documentType></documentType>';
    
    var tbl = document.getElementById(listFilterDocumentTypesID).childNodes[0];
    for (var i = 0; i < tbl.childNodes.length; i++) {
        for (var k = 0; k < tbl.childNodes[i].childNodes.length; k++) {
            if (tbl.childNodes[i].childNodes[k].nodeName == "TD") {
                var currentTD = tbl.childNodes[i].childNodes[k];
                for (var j = 0; j < currentTD.childNodes.length; j++) {
                    if (currentTD.childNodes[j].nodeName == "SPAN") {
                        var currentSpan = currentTD.childNodes[j];
                        for (var l = 0; l < currentSpan.childNodes.length; l++) {
                            if (currentSpan.childNodes[l].nodeName == "INPUT" && currentSpan.childNodes[l].type == "checkbox") {
                                var currentChkBox = currentSpan.childNodes[l];
                                if (currentChkBox.checked) {
                                    result += '<documentTypeID>' + currentSpan.alt + '</documentTypeID>';
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    return '<documentType>' + result + '</documentType>';        


}

function FetchDocumentList_Callback(response) {
	var arrDocs, i;
	var tr, td, radioButton;
	var str;
	var link, strLinkHoverMsg;
	var imgFileIcon;

	EnableControls(false);

	if (CheckAjaxResponse(response, documentSvc.url)) 
	{
	    // get current page number (in case it was updated by "spxDocument_FetchListWithPaging")
	    currentPage = response.value.CurrPage;

		// populate the conversation table
		arrDocs = response.value.Documents;

		// remove existing rows
		ClearTable(tblDocuments);

		for (i = 0; i < arrDocs.length; i++) 
		{
			tr = AddRow(tblDocuments);
			td = AddCell(tr, "");

			radioButton = AddRadio(td, "", "DocumentsSelect", arrDocs[i].DocumentID, RadioButton_Click);

			if (arrDocs[i].RecipentRef == "")
			    td = AddCell(tr, " ");
			else
			    td = AddCell(tr, arrDocs[i].RecipentRef);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			if (arrDocs[i].RecipentName == "")
			    td = AddCell(tr, " ");
			else
			    td = AddCell(tr, arrDocs[i].RecipentName);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			td = AddCell(tr, arrDocs[i].DocumentType);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

            if(arrDocs[i].Description == "")
                td = AddCell(tr, " ");
            else
                td = AddCell(tr, arrDocs[i].Description);
            td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			td = AddCell(tr, "");
			strLinkHoverMsg = "Click to download \"" + arrDocs[i].Filename + "\"";
			imgFileIcon = AddImg(td, GetFileIconPath(arrDocs[i].IconFile), strLinkHoverMsg, "pointer", FileIcon_Click, arrDocs[i].DocumentID);
			ResizeFileIcon(imgFileIcon);
			link = AddLink(td, "&nbsp;" + arrDocs[i].Filename, DocumentSelector_GetViewURL(arrDocs[i].DocumentID), strLinkHoverMsg);
			link.className = "transBg";

			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			td = AddCell(tr, arrDocs[i].PrintStatus);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";

			AddCell(tr, Date.strftime("%d/%m/%Y %H:%M", arrDocs[i].StatusDatetime, true));
			
			// select the document?
			if (DocumentSelector_selectedDocumentID == arrDocs[i].DocumentID || (currentPage == 1 && arrDocs.length == 1)) 
			{
				radioButton.click();
			}			
		}
		
          // load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
    }

    PanelResize();
	DisplayLoading(false);
}

function RadioButton_Click() {
	var index, rdo, selectedRow;
	for (index = 0; index < tblDocuments.tBodies[0].rows.length; index++){
		rdo = tblDocuments.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblDocuments.tBodies[0].rows[index];
			tblDocuments.tBodies[0].rows[index].className = "highlightedRow"
			DocumentSelector_selectedDocumentID = rdo.value;
			EnableControls(true);
		} else {
			tblDocuments.tBodies[0].rows[index].className = ""
		}
	}

	if(typeof DocumentSelector_SelectedItemChange == "function") {
		var args = new Array(2);
		args[0] = DocumentSelector_selectedDocumentID;
		args[1] = GetInnerText(selectedRow.cells[1]);
		args[2] = GetInnerText(selectedRow.cells[2]);;
		DocumentSelector_SelectedItemChange(args);
	}
}

function DocumentSelector_MruOnChange(mruListKey, selectedValue) {
    if(selectedValue.length > 0) {
        var url = document.location.href;
        url = RemoveQSParam(url, "documentID");
        url = RemoveQSParam(url, "bhID");
        url = AddQSParam(url, "documentID", selectedValue);
        document.location.href = url;
    }
}

function DocumentSelector_btnNew_Click() {
    var id = DocumentSelector_selectedDocumentID;

    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Documents/DocumentAdd.aspx?autopopup=1&serviceusertype=" + serviceUserType + "&documentassociatorid=" + documentAssociatorID;

    if (url) OpenDialog(url, 47, 32, window);
    //if (url) OpenPopup(url, 47, 30, window);
}

function DocumentSelector_GetViewURL(id) {
    return SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Documents/DocumentDownloadHandler.axd?id=" + id + "&saveas=1";
}

function DocumentSelector_btnView_Click() {
    document.location.href = DocumentSelector_GetViewURL(DocumentSelector_selectedDocumentID);
}

function FileIcon_Click(evt, args) {
    document.location.href = DocumentSelector_GetViewURL(args);
}

function DocumentSelector_btnProperties_Click() {
    var id = DocumentSelector_selectedDocumentID;

    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Documents/DocumentProperties.aspx?autopopup=1&documentid=" + id;

    if (url) OpenDialog(url, 63, 35, window);
}

function DocumentSelector_btnCreateBatch_Click() {
    var url = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Documents/CreatePrintQueueBatch.aspx?" + GetFiltersForQueryString();

    alert(GetFiltersForQueryString());
    //document.location.href = url;
}

function GetFiltersForQueryString() {
    // This function is only tested when re-directed to CreatePrintQueueBatch.aspx
    // CreatePrintQueueBatch.aspx does not filter by any date parameters, hence
    // the date filters e.g. From Date, To Date etc. may not work!

    var qStr = null;

    PopulateFilterVars();

    listFilterDocumentTypes = listFilterDocumentTypes.replace('<documentType>', '');
    listFilterDocumentTypes = listFilterDocumentTypes.replace('</documentType>', '');
    listFilterDocumentTypes = listFilterDocumentTypes.replace(/\<documentTypeID\>/g, 'f1=');
    listFilterDocumentTypes = listFilterDocumentTypes.replace(/\<\/documentTypeID\>/g, '&');
    // remove '&' at end
    listFilterDocumentTypes = listFilterDocumentTypes.slice(0, -1);

    // if no document type selected then append blank document type
    if (listFilterDocumentTypes.length == 0) listFilterDocumentTypes = 'f1=-1';

    qStr = listFilterDocumentTypes;
    if(listFilterOrigin) qStr += "&f2="  + listFilterOrigin;
    if(listFilterDescription) qStr += "&f3=" + listFilterDescription;
    if(listFilterCreatedFromDate) qStr += "&f4=" + listFilterCreatedFromDate;
    if(listFilterCreatedToDate) qStr += "&f5="  + listFilterCreatedToDate;
    if(listFilterCreatedBy) qStr += "&f6="  + listFilterCreatedBy;
    if(listFilterRecipientReference) qStr += "&f7="  + listFilterRecipientReference;
    if(listFilterRecipientName) qStr += "&f8="  + listFilterRecipientName;
    if(listFilterNeverQueued) qStr += "&f9="  + listFilterNeverQueued;
    if(listFilterQueued) qStr += "&f10=" + listFilterQueued;
    if(listFilterBatched) qStr += "&f11=" + listFilterBatched;
    if(listFilterSentToPrinter) qStr += "&f12=" + listFilterSentToPrinter;
    if(listFilterRemovedFromQueue) qStr += "&f13=" + listFilterRemovedFromQueue;
    if(listFilterPrintStatusFromDate) qStr += "&f14=" + listFilterPrintStatusFromDate;
    if(listFilterPrintStatusToDate) qStr += "&f15=" + listFilterPrintStatusToDate;
    if (listFilterPrintStatusBy) qStr += "&f16=" + listFilterPrintStatusBy;

    return qStr;
}

// enables/disables btnProperties, btnPrint, btnView, btnRemove buttons
function EnableControls(enableStatus) {
    if (btnProperties) btnProperties.disabled = !enableStatus;
    if (btnPrint) btnPrint.disabled = !enableStatus;
    if (btnView) btnView.disabled = !enableStatus;
    if (btnRemove) btnRemove.disabled = !enableStatus;
}

function GetFileIconPath(fileIcon) {
    if (fileIcon == null) {
        fileIcon = GENERIC_FILE_ICON_IMAGE;
    }
    return SITE_VIRTUAL_ROOT + "Images/FileTypes/" + fileIcon;
}

function ResizeFileIcon(imgFileIcon) {
    var img = new Image();
    img.src = imgFileIcon.src;

    if (img.height <= ALLOWED_FILE_ICON_HEIGHT) return;

    var aspectRatio = img.width / img.height;

    imgFileIcon.height = ALLOWED_FILE_ICON_HEIGHT;
    imgFileIcon.width = aspectRatio * ALLOWED_FILE_ICON_HEIGHT;
}

function GetBackUrl(documentID) {
    if (!documentID) documentID = DocumentSelector_selectedDocumentID;
    var url = document.location.href;
    url = RemoveQSParam(url, "documentid");
    url = AddQSParam(url, "documentid", documentID);

    return escape(url);
}

function PanelResize() {
    if (IframeID) {
        var heightToExpandTo = document.getElementById('tblDocuments').offsetHeight + 50;
        heightToExpandTo += document.getElementById(cpFiltersID).offsetHeight;
        parent.resizeIframe(heightToExpandTo, IframeID);
    }
}

function PrintDocuments(printAll, printNow) {
    
    if (!printAll && printNow)  { PrintDocument(); return; }

    if (!printAll && !printNow) { QueueDocument(); return; }

    if (printAll && printNow) { PrintAllDocuments(); return; }

    if (printAll && !printNow) { QueueAllDocuments(); return; }
}

function PrintDocument() {
    DisplayLoading(true);
    documentSvc.PrintDocument(DocumentSelector_selectedDocumentID, PrintDocument_Callback)
}

function PrintDocument_Callback(response) {
    if (CheckAjaxResponse(response, documentSvc.url)) {

        alert("Your document has been added to the Print Queue\n and will be printed shortly.");

        // refresh documents' table
        FetchDocumentList(currentPage, DocumentSelector_selectedDocumentID);

    }
    DisplayLoading(false);
}

function QueueDocument() {
    DisplayLoading(true);
    documentSvc.QueueDocument(DocumentSelector_selectedDocumentID, QueueDocument_Callback)
}

function QueueDocument_Callback(response) {
    if (CheckAjaxResponse(response, documentSvc.url)) {

        alert("Your document has been added to the Print Queue. \nPlease use the \"Print Queue\" screen to batch \nand print this document.");

        // refresh documents' table
        FetchDocumentList(currentPage, DocumentSelector_selectedDocumentID);

    }
    DisplayLoading(false);
}

function PrintAllDocuments() {
    
    DisplayLoading(true);
    
    PopulateFilterVars();

    documentSvc.PrintAllDocuments(/* currentPage */1, /* pageSize */999, null, serviceUserType, documentAssociatorID,
                                  documentPrintQueueBatchID,
	                              listFilterDocumentTypes, listFilterOrigin,
	                              listFilterDescription, listFilterCreatedFromDate,
	                              listFilterCreatedToDate, listFilterCreatedBy,
	                              listFilterNeverQueued, listFilterQueued,
	                              listFilterBatched, listFilterSentToPrinter, listFilterRemovedFromQueue,
	                              listFilterPrintStatusFromDate, listFilterPrintStatusToDate,
	                              listFilterPrintStatusBy, PrintAllDocuments_Callback);
}

function PrintAllDocuments_Callback(response) {
    if (CheckAjaxResponse(response, documentSvc.url)) {

        alert("Your document(s) have been added to the Print Queue\n and will be printed shortly.");

        // refresh documents' table
        FetchDocumentList(1, DocumentSelector_selectedDocumentID);

    }
    DisplayLoading(false);
}

function QueueAllDocuments() {

    DisplayLoading(true);
    
    PopulateFilterVars();

    documentSvc.QueueAllDocuments(/* currentPage */1, /* pageSize */999, null, serviceUserType, documentAssociatorID,
                                  documentPrintQueueBatchID,
	                              listFilterDocumentTypes, listFilterOrigin,
	                              listFilterDescription, listFilterCreatedFromDate,
	                              listFilterCreatedToDate, listFilterCreatedBy,
	                              listFilterNeverQueued, listFilterQueued,
	                              listFilterBatched, listFilterSentToPrinter, listFilterRemovedFromQueue,
	                              listFilterPrintStatusFromDate, listFilterPrintStatusToDate,
	                              listFilterPrintStatusBy, QueueAllDocuments_Callback);
}

function QueueAllDocuments_Callback(response) {
    if (CheckAjaxResponse(response, documentSvc.url)) {

        alert("Your document(s) have been added to the Print Queue. \nPlease use the \"Print Queue\" screen to batch \nand print these document(s).");

        // refresh documents' table
        FetchDocumentList(currentPage, DocumentSelector_selectedDocumentID);

    }
    DisplayLoading(false);
}

function DocumentSelector_btnRemove_Click() {
    DisplayLoading(true);
    documentSvc.DequeueDocument(DocumentSelector_selectedDocumentID, DequeueDocument_Callback);
}

function DequeueDocument_Callback(response) {
    if (CheckAjaxResponse(response, documentSvc.url)) {

        alert("The selected document has been removed from the Print Queue.");

        // refresh documents' table
        FetchDocumentList(currentPage, DocumentSelector_selectedDocumentID);

    }
    DisplayLoading(false);
}

addEvent(window, "load", Init);
