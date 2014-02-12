var InterfaceLogSelector_LookupService;
var InterfaceLogSelector_ResultsTable;
var InterfaceLogSelector_PagingLinks;
var InterfaceLogSelector_CurrentPage;
var InterfaceLogSelector_SelectedID;
var InterfaceLogSelector_ListFilter;
var InterfaceLogSelector_ListFilter_CreatedDate
var InterfaceLogSelector_ListFilter_CreatedBy;
var InterfaceLogSelector_InterfaceLogs;

function InterfaceLogSelector_Init() {

    InterfaceLogSelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.InterfaceLogs_class();
    InterfaceLogSelector_ResultsTable = GetElement("tblInterfaceLogs");
    InterfaceLogSelector_PagingLinks = GetElement("InterfaceLog_PagingLinks");

    // setup list filters
    InterfaceLogSelector_ListFilter = new Target.Web.ListFilter(InterfaceLogSelector_ListFilter_Callback);
    InterfaceLogSelector_ListFilter.AddColumn("CreatedBy", GetElement("thInterfaceLogCreatedBy"));
    InterfaceLogSelector_ListFilter.AddColumn("CreatedDate", GetElement("thInterfaceLogCreatedDate"));

    InterfaceLogSelector_FetchInterfaceLogs(InterfaceLogSelector_CurrentPage, InterfaceLogSelector_SelectedID);
}

function InterfaceLogSelector_ListFilter_Callback(column) {
    switch (column.Name) {
        case "Created":
            InterfaceLogSelector_ListFilter_CreatedDate = column.Filter;
            break;
        case "Created By":
            InterfaceLogSelector_ListFilter_CreatedBy = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    InterfaceLogSelector_FetchInterfaceLogs(1, 0);
}

function InterfaceLogSelector_FetchInterfaceLogs(page, selectedID) {  
  
    DisplayLoading(true);
    if (page == undefined) page = 0;
    if (selectedID == undefined) selectedID = 0;
    if (InterfaceLogSelector_InterfaceLogTypes == 0) InterfaceLogSelector_InterfaceLogTypes = 0;
    InterfaceLogSelector_LookupService.GetPagedInterfaceLogs(page, 10, selectedID, InterfaceLogSelector_ListFilter_CreatedDate, InterfaceLogSelector_ListFilter_CreatedBy, InterfaceLogSelector_InterfaceLogTypes, InterfaceLogSelector_FetchInterfaceLogs_Callback);
    
}

function InterfaceLogSelector_FetchInterfaceLogs_Callback(response) {

    InterfaceLogSelector_InterfaceLogs = null;
    
    if (CheckAjaxResponse(response, InterfaceLogSelector_LookupService.url)) {

        var index, tr, td, radioButton, str, link, sCount, currentInterfaceLog, currentInterfaceLogArgs, span, hasSelected;

        hasSelected = false;  
        InterfaceLogSelector_InterfaceLogs = response.value.Items;
        sCount = InterfaceLogSelector_InterfaceLogs.length;         
        ClearTable(InterfaceLogSelector_ResultsTable);

        if (sCount > 0) {

            for (index = 0; index < sCount; index++) {

                currentInterfaceLog = InterfaceLogSelector_InterfaceLogs[index];
                currentInterfaceLogArgs = new Array(currentInterfaceLog.ID, currentInterfaceLog.JobID);
                
                tr = AddRow(InterfaceLogSelector_ResultsTable);
                td = AddCell(tr, "");
                radioButton = AddRadio(td, "", "InterfaceLogSelect", currentInterfaceLog.ID, InterfaceLogSelector_RadioButton_Click);

                InterfaceLogSelector_AddCell(tr, currentInterfaceLog.DateCreated.strftime("%d %b %Y %T"));
                InterfaceLogSelector_AddCell(tr, currentInterfaceLog.CreatedBy);
                InterfaceLogSelector_AddCell(tr, currentInterfaceLog.BatchReference);
                InterfaceLogSelector_AddCell(tr, currentInterfaceLog.Entries.toString());
                InterfaceLogSelector_AddCell(tr, String(currentInterfaceLog.Value).formatCurrency(), 1);
                InterfaceLogSelector_AddCell(tr, String(currentInterfaceLog.Vat).formatCurrency(), 1);
                InterfaceLogSelector_AddCell(tr, String(currentInterfaceLog.TotalValue).formatCurrency(), 1);

                td = AddCell(tr, "");
                td.className = currentInterfaceLog.JobStatusCssClass;
                span = document.createElement("span");
                td.appendChild(span);
                SetInnerText(span, currentInterfaceLog.JobStatus);
                span.style.styleFloat = "left";
                span.style.marginTop = "0.5em";
                if (currentInterfaceLog.JobStatus != "Queued") {
                    // add link to job
                    but = AddImg(td, "../../../Images/Jobs/outputs.png", "View the results of the latest job", "pointer", InterfaceLogSelector_JobStatusClicked_Internal, currentInterfaceLogArgs);
                    but.style.styleFloat = "right";
                }              
                
                // select the item?
                if (InterfaceLogSelector_SelectedID == currentInterfaceLog.ID || ( InterfaceLogSelector_CurrentPage == 1 && InterfaceLogSelector_InterfaceLogs.length == 1)) {
                    radioButton.click();
                    hasSelected = true;
                }

            }

            if (hasSelected == false) {
                InterfaceLogSelector_SelectedItemChanged(null);
            }

        } else {

            if (typeof InterfaceLogSelector_SelectedItemChanged == "function") {

                InterfaceLogSelector_SelectedItemChanged(null);
                
            }       
        }
        
        // load the paging link HTML
        InterfaceLogSelector_PagingLinks.innerHTML = response.value.PagingLinks;
    }
    
    DisplayLoading(false);
}

function InterfaceLogSelector_AddCell(tr, str, isHTML) {

    var td;
    if (isHTML == undefined) {
        if (!str || str.length == 0) str = " ";
        td = AddCell(tr, str);
    } else {
        td = AddCell(tr, "");
        td.innerHTML = str;
    }
    td.style.textOverflow = "ellipsis";
    td.style.overflow = "hidden";
    return td;

}

function InterfaceLogSelector_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = InterfaceLogSelector_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = InterfaceLogSelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = InterfaceLogSelector_ResultsTable.tBodies[0].rows[index];
            InterfaceLogSelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            InterfaceLogSelector_SelectedID = rdo.value;
        } else {
            InterfaceLogSelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof InterfaceLogSelector_SelectedItemChanged == "function") {
        InterfaceLogSelector_SelectedItemChanged(InterfaceLogSelector_GetSelectedObject(InterfaceLogSelector_SelectedID));
    }
}

function InterfaceLogSelector_GetSelectedObject(id) {
    if (InterfaceLogSelector_InterfaceLogs != null) {
        var collectionLength = InterfaceLogSelector_InterfaceLogs.length;
        for (var j = 0; j < collectionLength; j++) {
            if (InterfaceLogSelector_InterfaceLogs[j].ID == id) {
                return InterfaceLogSelector_InterfaceLogs[j];
            }
        }
    }
}

function InterfaceLogSelector_JobStatusClicked_Internal(evt, args) {

    if (typeof InterfaceLogSelector_JobStatusClicked == "function") {
        InterfaceLogSelector_JobStatusClicked(InterfaceLogSelector_GetSelectedObject(args[0]));
    }
    
}

addEvent(window, "load", InterfaceLogSelector_Init);