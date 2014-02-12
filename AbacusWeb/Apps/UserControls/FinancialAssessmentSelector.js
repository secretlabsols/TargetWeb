var FinancialAssessmentSelector_FilterClientID;
var FinancialAssessmentSelector_LookupService;
var FinancialAssessmentSelector_ResultsTable;
var FinancialAssessmentSelector_PagingLinks;
var FinancialAssessmentSelector_CurrentPage;
var FinancialAssessmentSelector_SelectedID;
var FinancialAssessmentSelector_FinancialAssessments;

function FinancialAssessmentSelector_Init() {

    FinancialAssessmentSelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
    FinancialAssessmentSelector_ResultsTable = GetElement("FinancialAssessmentSelector_Results");
    FinancialAssessmentSelector_PagingLinks = GetElement("FinancialAssessmentSelector_PagingLinks");
    FinancialAssessmentSelector_FetchFinancialAssessmentList(FinancialAssessmentSelector_CurrentPage, FinancialAssessmentSelector_SelectedID);
}

function FinancialAssessmentSelector_FetchFinancialAssessmentList(page, selectedID) {    
    DisplayLoading(true);
    if (page == undefined) page = 1;
    if (selectedID == undefined) selectedID = 0;
    FinancialAssessmentSelector_CurrentPage = page;
    FinancialAssessmentSelector_LookupService.FetchFinancialAssessmentList(page, selectedID, FinancialAssessmentSelector_FilterClientID, FinancialAssessmentSelector_FetchFinancialAssessmentList_Callback);
}

function FinancialAssessmentSelector_FetchFinancialAssessmentList_Callback(response) {
    
    var index, tr, td, radioButton, str, link, sCount;

    FinancialAssessmentSelector_FinancialAssessments = null;
    
    if (CheckAjaxResponse(response, FinancialAssessmentSelector_LookupService.url)) {   
               
        FinancialAssessmentSelector_FinancialAssessments = response.value.Results;
        sCount = FinancialAssessmentSelector_FinancialAssessments.length;         
        ClearTable(FinancialAssessmentSelector_ResultsTable);

        for (index = 0; index < sCount; index++) {

            tr = AddRow(FinancialAssessmentSelector_ResultsTable);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "FinancialAssessmentSelect", FinancialAssessmentSelector_FinancialAssessments[index].ID, FinancialAssessmentSelector_RadioButton_Click);

            td = AddCell(tr, Date.strftime("%d/%m/%Y", FinancialAssessmentSelector_FinancialAssessments[index].DateFrom));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, Date.strftime("%d/%m/%Y", FinancialAssessmentSelector_FinancialAssessments[index].DateTo));
            if (td.innerHTML = "31/12/2009") {
                td.innerHTML = "&nbsp;";
            }
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, "");
            if (FinancialAssessmentSelector_FinancialAssessments[index].Provisional == true) {
                td.innerHTML = "Yes";
            } else {
                td.innerHTML = "No";
            }
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, FinancialAssessmentSelector_FinancialAssessments[index].AssessmentType);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(FinancialAssessmentSelector_FinancialAssessments[index].AssessedCharge).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the item?
            if (FinancialAssessmentSelector_SelectedID == FinancialAssessmentSelector_FinancialAssessments[index].ID || ( FinancialAssessmentSelector_CurrentPage == 1 && FinancialAssessmentSelector_FinancialAssessments.length == 1)) {
                radioButton.click();
            }

        }
        
        // load the paging link HTML
        FinancialAssessmentSelector_PagingLinks.innerHTML = response.value.PagingLinks;

    }
    
    DisplayLoading(false);
    
}

function FinancialAssessmentSelector_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = FinancialAssessmentSelector_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = FinancialAssessmentSelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = FinancialAssessmentSelector_ResultsTable.tBodies[0].rows[index];
            FinancialAssessmentSelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            FinancialAssessmentSelector_SelectedID = rdo.value;
        } else {
            FinancialAssessmentSelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof FinancialAssessmentSelector_SelectedItemChanged == "function") {
        FinancialAssessmentSelector_SelectedItemChanged(FinancialAssessmentSelector_GetSelectedObject(FinancialAssessmentSelector_SelectedID));
    }
}

function FinancialAssessmentSelector_GetSelectedObject(id) {
    if (FinancialAssessmentSelector_FinancialAssessments != null) {
        var collectionLength = FinancialAssessmentSelector_FinancialAssessments.length;
        for (var j = 0; j < collectionLength; j++) {
            if (FinancialAssessmentSelector_FinancialAssessments[j].ID == id) {
                return FinancialAssessmentSelector_FinancialAssessments[j];
            }
        }
    }
}

addEvent(window, "load", FinancialAssessmentSelector_Init);