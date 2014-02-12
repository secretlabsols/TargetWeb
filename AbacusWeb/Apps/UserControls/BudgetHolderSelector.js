
var budholderSvc, currentPage, BudgetHolderStep_required, budholderSelector_Redundant, budholderSelector_ServiceUserID, budgetHolderSelector_selectedbhID;
var listFilter, listFilterReference = "", listFilterName = "";
var listFilterCreditorRef = "", listFilterOrg = "";
var tblBudgetHolders, divPagingLinks, btnNext, btnFinish;

function Init() {
    budholderSvc = new Target.Abacus.Web.Apps.WebSvc.BudgetHolders_class();
    tblBudgetHolders = GetElement("tblBudgetHolders");
    divPagingLinks = GetElement("BudgetHolder_PagingLinks");
    btnNext = GetElement("SelectorWizard1_btnNext", true);
    btnFinish = GetElement("SelectorWizard1_btnFinish", true);

    // setup list filters
    listFilter = new Target.Web.ListFilter(ListFilter_Callback);
    listFilter.AddColumn("Reference", GetElement("thRef"));
    listFilter.AddColumn("Creditor Ref.", GetElement("thCreditorRef"));
    listFilter.AddColumn("Name", GetElement("thName"));
    listFilter.AddColumn("Organisation", GetElement("thOrganisation"));

    // populate table
    FetchBudHolderList(currentPage, budgetHolderSelector_selectedbhID);
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "Reference":
            listFilterReference = column.Filter;
            break;
        case "Creditor Ref.":
            listFilterCreditorRef = column.Filter;
            break;
        case "Name":
            listFilterName = column.Filter;
            break;
        case "Organisation":
            listFilterOrg = column.Filter;
            break;

        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchBudHolderList(1, 0);
}

/* FETCH BUDGET HOLDER LIST METHODS */
function FetchBudHolderList(page, budHolderID) {
    currentPage = page;
    DisplayLoading(true);
    if (budHolderID == undefined) budHolderID = 0;

    budholderSvc.FetchBudgetHolderList(page, budHolderID, listFilterReference, listFilterName, listFilterCreditorRef, listFilterOrg, budholderSelector_Redundant, budholderSelector_ServiceUserID, FetchBudHolderList_Callback);
}

function FetchBudHolderList_Callback(response) {
    var budholders, budHolderCounter;
    var tr, td, radioButton;
    var str;
    var link;

    if (CheckAjaxResponse(response, budholderSvc.url)) {
        // disable next/finish buttons by default if the step is mandatory
        if (BudgetHolderStep_required) {
            if (btnNext) btnNext.disabled = true;
            if (btnFinish) btnFinish.disabled = true;
        }

        // populate the conversation table
        budholders = response.value.BudgetHolders;

        // remove existing rows
        ClearTable(tblBudgetHolders);
        for (budHolderCounter = 0; budHolderCounter < budholders.length; budHolderCounter++) {

            tr = AddRow(tblBudgetHolders);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "BudgetHolderSelect", budholders[budHolderCounter].ID, RadioButton_Click);

            str = budholders[budHolderCounter].Reference;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = budholders[budHolderCounter].CreditorReference;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = budholders[budHolderCounter].BudgetHolderName;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = budholders[budHolderCounter].Organisation;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the budget holder?
            if (budgetHolderSelector_selectedbhID == budholders[budHolderCounter].ID || (currentPage == 1 && budholders.length == 1)) {
                radioButton.click();
            }
        }
        // load the paging link HTML
        divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}

function RadioButton_Click() {
    var index, rdo, selectedRow;

    for (index = 0; index < tblBudgetHolders.tBodies[0].rows.length; index++) {
        rdo = tblBudgetHolders.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = tblBudgetHolders.tBodies[0].rows[index];
            tblBudgetHolders.tBodies[0].rows[index].className = "highlightedRow"
            budgetHolderSelector_selectedbhID = rdo.value;
        } else {
            tblBudgetHolders.tBodies[0].rows[index].className = ""
        }
    }
    if (BudgetHolderStep_required) {
        if (btnNext) btnNext.disabled = false;
        if (btnFinish) btnFinish.disabled = false;
    }
    if (typeof BudgetHolderSelector_SelectedItemChange == "function") {
        var args = new Array(4);
        args[0] = budgetHolderSelector_selectedbhID;
        args[1] = GetInnerText(selectedRow.cells[1]);
        args[2] = GetInnerText(selectedRow.cells[2]);
        args[3] = GetInnerText(selectedRow.cells[3]);
        args[4] = GetInnerText(selectedRow.cells[4]);
        BudgetHolderSelector_SelectedItemChange(args);
    }
}

function GetSelectedBudgetHolder() {
    var index, rdo;

    for (index = 0; index < tblBudgetHolders.tBodies[0].rows.length; index++) {
        rdo = tblBudgetHolders.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo)
            if (rdo.checked) {
                return rdo.value;
            }
    }
    return 0;
}

function BudgetHolderStep_BeforeNavigate() {
    var originalBudgetHolderID = GetQSParam(document.location.search, "bhID");
    var budgetHolderID = GetSelectedBudgetHolder();
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    url = AddQSParam(RemoveQSParam(url, "bhID"), "bhID", budgetHolderID);
    SelectorWizard_newUrl = url;
    return true;
}

if (typeof BudgetHolderSelector_GetBackUrl != "function") {
    BudgetHolderSelector_GetBackUrl = function () {
        // not used but still needs to be defined
        return "";
    }
}

addEvent(window, "load", Init);
