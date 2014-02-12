
var documentSvc, currentPage;
var documentPrintQueueBatchID, jobID, arrBatch;

// filter controls' IDs
var createdFromID = null;
var createdToID   = null;
var createdByID   = null;
var cboPrinterID  = null;

// filter values
var createdFrom = null;
var createdTo   = null;
var createdBy   = null;
var cboPrinter  = null;

var tblBatches, divPagingLinks;

function Init() {
    tblBatches = GetElement("tblPrintQueueBatches");
    divPagingLinks = GetElement("PrintQueueBatches_PagingLinks");

    documentSvc = new Target.Abacus.Web.Apps.WebSvc.Documents_class();

    currentPage = 1;

    FetchPrintQueueBatches();
}

function FetchPrintQueueBatches() {
    DisplayLoading(true);

    SetFilterValues();

    documentSvc.FetchPrintQueueBatches(currentPage, 10, null, createdFrom, createdTo,
	                                   createdBy, cboPrinter, FetchPrintQueueBatches_Callback)
}

function FetchPrintQueueBatches_Callback(response) {
    var i;
    var tr, td, radioButton;

    EnableButtons(false);

    if (CheckAjaxResponse(response, documentSvc.url)) {
        // populate the conversation table
        arrBatch = response.value.PrintQueueBatches;

        // remove existing rows
        ClearTable(tblBatches);

        for (i = 0; i < arrBatch.length; i++) {
            tr = AddRow(tblBatches);

            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "DocumentBatchSelect", arrBatch[i].DocumentPrintQueueBatchID, RadioButton_Click);

            AddCell(tr, Date.strftime("%d/%m/%Y %H:%M", arrBatch[i].CreatedDate, true));

            AddCellWithValue(tr, arrBatch[i].CreatedBy);

            AddCellWithValue(tr, arrBatch[i].Comment);

            AddCellWithValue(tr, arrBatch[i].DocumentCount);

            AddCellWithValue(tr, arrBatch[i].PrinterName);

            AddCellWithValueAndCssClass(tr, arrBatch[i].JobStatus, arrBatch[i].JobStatusCssClass);

            // select the batch?
            if (documentPrintQueueBatchID == arrBatch[i].DocumentPrintQueueBatchID || (currentPage == 1 && arrBatch.length == 1)) {
                radioButton.click();
            }
        }

        // load the paging link HTML
        divPagingLinks.innerHTML = response.value.PagingLinks;
    }

    DisplayLoading(false);
}

function SetFilterValues() {
    createdFrom = GetDateValue(createdFromID, false);
    createdTo   = GetDateValue(createdToID, true);
    createdBy   = GetControlValue(createdByID);
    cboPrinter  = GetControlValue(cboPrinterID);
}

function GetControlValue(controlID) {
    if (!document.getElementById(controlID)) return null;

    if (document.getElementById(controlID).value.length == 0) return null;

    return document.getElementById(controlID).value;
}

function GetDateValue(controlID, setTimeToMidnight) {
    if (!GetControlValue(controlID)) return null;

    var objDate = (GetControlValue(controlID)).toDate();

    if (setTimeToMidnight) objDate.setHours(23, 59, 0, 0);

    return objDate;
}

function EnableButtons(visibleFlag) {
    document.getElementById("btnViewJob").disabled = !visibleFlag;
    document.getElementById("btnViewDocuments").disabled = !visibleFlag;
}

function AddCellWithValue(tr, tdValue) {
    var td = null;

    if (!tdValue || tdValue == "")
        td = AddCell(tr, " ");
    else
        td = AddCell(tr, tdValue);

    td.style.textOverflow = "ellipsis";
    td.style.overflow = "hidden";

    return td;
}

function AddCellWithValueAndCssClass(tr, tdValue, cssClass) {
    var td = null;

    if (!tdValue || tdValue == "")  {
        td = AddCell(tr, " ");
        return td;
    }
    
    td = AddCell(tr, "");

    var span = document.createElement("span");        
    span.className = cssClass;
    span.innerHTML = tdValue;        

    td.appendChild(span);    

    return td;
}

function RadioButton_Click() {
    var index, rdo;

    for (index = 0; index < tblBatches.tBodies[0].rows.length; index++) {
        rdo = tblBatches.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            tblBatches.tBodies[0].rows[index].className = "highlightedRow"
            documentPrintQueueBatchID = rdo.value;
            jobID = arrBatch[index].JobID;
            EnableButtons(true);
        } else {
            tblBatches.tBodies[0].rows[index].className = ""
        }
    }
}

function PrintQueueBatches_btnViewJob_Click() {
    document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Jobs/Default.aspx?jobID=" + jobID;
}

function PrintQueueBatches_btnViewDocuments_Click() {
    var url = "AbacusWeb/Apps/Documents/PrintQueueBatchDocuments.aspx";
    url += "?documentPrintQueueBatchID=" + documentPrintQueueBatchID;
    document.location.href = SITE_VIRTUAL_ROOT + url;
}

addEvent(window, "load", Init);
