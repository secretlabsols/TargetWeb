var BudgetCategorySelector_FilterRedundant, BudgetCategorySelector_FilterExcludeIds, BudgetCategorySelector_FilterIncludeIds, BudgetCategorySelector_FilterIncludeServiceTypeIds;
var BudgetCategorySelector_LookupService;
var BudgetCategorySelector_ResultsTable;
var BudgetCategorySelector_PagingLinks;
var BudgetCategorySelector_CurrentPage;
var BudgetCategorySelector_SelectedID;
var BudgetCategorySelector_ListFilter;
var BudgetCategorySelector_ListFilter_BudgetCategoryDescription;
var BudgetCategorySelector_ListFilter_BudgetCategoryReference;
var BudgetCategorySelector_ListFilter_BudgetCategoryGroupDescription;
var BudgetCategorySelector_BudgetCategories;

function BudgetCategorySelector_Init() {

    BudgetCategorySelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
    BudgetCategorySelector_ResultsTable = GetElement("tblBudgetCategories");
    BudgetCategorySelector_PagingLinks = GetElement("BudgetCategory_PagingLinks");

    // setup list filters
    BudgetCategorySelector_ListFilter = new Target.Web.ListFilter(BudgetCategorySelector_ListFilter_Callback);
    BudgetCategorySelector_ListFilter.AddColumn("BudgetCategoryDescription", GetElement("thBudgetCategoryDescription"));
    BudgetCategorySelector_ListFilter.AddColumn("BudgetCategoryReference", GetElement("thBudgetCategoryReference"));
    BudgetCategorySelector_ListFilter.AddColumn("BudgetCategoryGroupDescription", GetElement("thBudgetCategoryGroupDescription"));

    BudgetCategorySelector_FetchBudgetCategoryList(BudgetCategorySelector_CurrentPage, BudgetCategorySelector_SelectedID);
}

function BudgetCategorySelector_ListFilter_Callback(column) {
    switch (column.Name) {
        case "Budget Category":
            BudgetCategorySelector_ListFilter_BudgetCategoryDescription = column.Filter;
            break;
        case "Reference":
            BudgetCategorySelector_ListFilter_BudgetCategoryReference = column.Filter;
            break;
        case "Budget Category Group":
            BudgetCategorySelector_ListFilter_BudgetCategoryGroupDescription = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    BudgetCategorySelector_FetchBudgetCategoryList(1, 0);
}

function BudgetCategorySelector_FetchBudgetCategoryList(page, selectedID) {    
    DisplayLoading(true);
    if (page == undefined) page = 0;
    if (selectedID == undefined) selectedID = 0;
    BudgetCategorySelector_CurrentPage = page;
    BudgetCategorySelector_LookupService.FetchBudgetCategoryList(page, selectedID, BudgetCategorySelector_ListFilter_BudgetCategoryDescription, BudgetCategorySelector_ListFilter_BudgetCategoryReference, BudgetCategorySelector_ListFilter_BudgetCategoryGroupDescription, BudgetCategorySelector_FilterRedundant, BudgetCategorySelector_FilterExcludeIds, BudgetCategorySelector_FilterIncludeIds, BudgetCategorySelector_FilterIncludeServiceTypeIds, BudgetCategorySelector_FetchBudgetCategoryList_Callback);
}

function BudgetCategorySelector_FetchBudgetCategoryList_Callback(response) {
    var index, tr, td, radioButton, str, link, sCount;
    BudgetCategorySelector_BudgetCategories = null;
    if (CheckAjaxResponse(response, BudgetCategorySelector_LookupService.url)) {

        BudgetCategorySelector_BudgetCategories = response.value.BudgetCategories;
        sCount = BudgetCategorySelector_BudgetCategories.length;         
        ClearTable(BudgetCategorySelector_ResultsTable);

        for (index = 0; index < sCount; index++) {

            tr = AddRow(BudgetCategorySelector_ResultsTable);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "BudgetCategorySelect", BudgetCategorySelector_BudgetCategories[index].ID, BudgetCategorySelector_RadioButton_Click);

            str = BudgetCategorySelector_BudgetCategories[index].Description;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = BudgetCategorySelector_BudgetCategories[index].Reference;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = BudgetCategorySelector_BudgetCategories[index].DomUnitsOfMeasureDescription;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = BudgetCategorySelector_BudgetCategories[index].BudgetCategoryGroupDescription;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the budget category?
            if (BudgetCategorySelector_SelectedID == BudgetCategorySelector_BudgetCategories[index].ID || ( BudgetCategorySelector_CurrentPage ==1 && BudgetCategorySelector_BudgetCategories.length == 1)) {
                radioButton.click();
            }

        }
        
        // load the paging link HTML
        BudgetCategorySelector_PagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}

function BudgetCategorySelector_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = BudgetCategorySelector_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = BudgetCategorySelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = BudgetCategorySelector_ResultsTable.tBodies[0].rows[index];
            BudgetCategorySelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            BudgetCategorySelector_SelectedID = rdo.value;
        } else {
            BudgetCategorySelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof BudgetCategorySelector_SelectedItemChanged == "function") {
        BudgetCategorySelector_SelectedItemChanged(BudgetCategorySelector_GetSelectedObject(BudgetCategorySelector_SelectedID));
    }
}

function BudgetCategorySelector_GetSelectedObject(id) {
    if (BudgetCategorySelector_BudgetCategories != null) {
        var collectionLength = BudgetCategorySelector_BudgetCategories.length;
        for (var j = 0; j < collectionLength; j++) {
            if (BudgetCategorySelector_BudgetCategories[j].ID == id) {
                return BudgetCategorySelector_BudgetCategories[j];
            }
        }
    }
}

addEvent(window, "load", BudgetCategorySelector_Init);