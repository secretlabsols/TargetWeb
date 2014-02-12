var DpContractTerminationMonitorResults_FilterContractType, DpContractTerminationMonitorResults_FilterIsBalanced, DpContractTerminationMonitorResults_FilterUnderOrOverPayments, DpContractTerminationMonitorResults_FilterTerminationPeriodFrom, DpContractTerminationMonitorResults_FilterTerminationPeriodTo;
var DpContractTerminationMonitorResults_ResultsTable, DpContractTerminationMonitorResults_PagingLinks, DpContractTerminationMonitorResults_CurrentPage, DpContractTerminationMonitorResults_SelectedID, DpContractTerminationMonitorResults_Items, DpContractTerminationMonitorResults_BtnReports, DpContractTerminationMonitorResults_btnBalance, DpContractTerminationMonitorResults_btnView;
var DpContractTerminationMonitorResults_LookupService, DpContractTerminationMonitorResults_ListFilter;
var DpContractTerminationMonitorResults_ServiceUserName, DpContractTerminationMonitorResults_ListFilter_ServiceUserReference, DpContractTerminationMonitorResults_BudgetHolderName, DpContractTerminationMonitorResults_ListFilter_BudgetHolderReference, DpContractTerminationMonitorResults_ContractNumber;

$(function() {
    DpContractTerminationMonitorResults_LookupService = new Target.Abacus.Web.Apps.WebSvc.DPContract_class();
    DpContractTerminationMonitorResults_ResultsTable = $("#DpContractTerminationMonitorResults_List")[0];
    DpContractTerminationMonitorResults_PagingLinks = $("#DpContractTerminationMonitorResults_PagingLinks")[0];
    DpContractTerminationMonitorResults_BtnReports = $('button[id$="btnPrint_btnReports"]');
    DpContractTerminationMonitorResults_btnBalance = $("#DpContractTerminationMonitorResults_btnBalance");
    DpContractTerminationMonitorResults_btnView = $("#DpContractTerminationMonitorResults_btnView");
    DpContractTerminationMonitorResults_ResetControlVisibility();
    // setup list filters
    DpContractTerminationMonitorResults_ListFilter = new Target.Web.ListFilter(DpContractTerminationMonitorResults_ListFilter_Callback);
    DpContractTerminationMonitorResults_ListFilter.AddColumn("ServiceUser", $("#DpContractTerminationMonitorResults_thServiceUserName")[0]);
    DpContractTerminationMonitorResults_ListFilter.AddColumn("ServiceUserReference", $("#DpContractTerminationMonitorResults_thServiceUserRef")[0]);
    DpContractTerminationMonitorResults_ListFilter.AddColumn("BudgetHolder", $("#DpContractTerminationMonitorResults_thBudgetHolderName")[0]);
    DpContractTerminationMonitorResults_ListFilter.AddColumn("BudgetHolderReference", $("#DpContractTerminationMonitorResults_thBudgetHolderRef")[0]);
    DpContractTerminationMonitorResults_ListFilter.AddColumn("ContractNumber", $("#DpContractTerminationMonitorResults_thContractNumber")[0]);
    // fetch data
    DpContractTerminationMonitorResults_FetchTerminatedContracts(DpContractTerminationMonitorResults_CurrentPage, DpContractTerminationMonitorResults_SelectedID);
});

function DpContractTerminationMonitorResults_ListFilter_Callback(column) {
    var reportsButtonId = DpContractTerminationMonitorResults_BtnReports.attr('id');
    switch (column.Name) {
        case "Service User":
            DpContractTerminationMonitorResults_ServiceUserName = column.Filter;
            ReportsButton_AddParam(reportsButtonId, 'serviceUserName', DpContractTerminationMonitorResults_ServiceUserName);
            break;
        case "S/U Ref":
            DpContractTerminationMonitorResults_ListFilter_ServiceUserReference = column.Filter;
            ReportsButton_AddParam(reportsButtonId, 'serviceUserReference', DpContractTerminationMonitorResults_ListFilter_ServiceUserReference);
            break;
        case "Budget Holder":
            DpContractTerminationMonitorResults_BudgetHolderName = column.Filter;
            ReportsButton_AddParam(reportsButtonId, 'budgetHolderName', DpContractTerminationMonitorResults_BudgetHolderName);
            break;
        case "B/H Ref":
            DpContractTerminationMonitorResults_ListFilter_BudgetHolderReference = column.Filter;
            ReportsButton_AddParam(reportsButtonId, 'budgetHolderReference', DpContractTerminationMonitorResults_ListFilter_BudgetHolderReference);
            break;
        case "Contract":
            DpContractTerminationMonitorResults_ContractNumber = column.Filter;
            ReportsButton_AddParam(reportsButtonId, 'contractNumber', DpContractTerminationMonitorResults_ContractNumber);
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    DpContractTerminationMonitorResults_FetchTerminatedContracts(1, 0);
}

function DpContractTerminationMonitorResults_FetchTerminatedContracts(page, selectedID) {
    DisplayLoading(true);
    if (page == undefined) page = 1;
    if (selectedID == undefined) selectedID = 0;
    DpContractTerminationMonitorResults_CurrentPage = page;
    DpContractTerminationMonitorResults_LookupService.GetPagedTerminatedContracts(page, selectedID, DpContractTerminationMonitorResults_FilterContractType, DpContractTerminationMonitorResults_FilterUnderOrOverPayments, DpContractTerminationMonitorResults_FilterIsBalanced, DpContractTerminationMonitorResults_FilterTerminationPeriodFrom, DpContractTerminationMonitorResults_FilterTerminationPeriodTo, DpContractTerminationMonitorResults_ServiceUserName, DpContractTerminationMonitorResults_ListFilter_ServiceUserReference, DpContractTerminationMonitorResults_BudgetHolderName, DpContractTerminationMonitorResults_ListFilter_BudgetHolderReference, DpContractTerminationMonitorResults_ContractNumber, DpContractTerminationMonitorResults_FetchTerminatedContracts_CallBack);    
}

function DpContractTerminationMonitorResults_FetchTerminatedContracts_CallBack(serviceReponse) {

    var index, tr, td, radioButton, str, link, sCount, currentItem;

    DpContractTerminationMonitorResults_Items = null;

    if (CheckAjaxResponse(serviceReponse, DpContractTerminationMonitorResults_LookupService.url)) {

        DpContractTerminationMonitorResults_Items = serviceReponse.value.Items;
        sCount = DpContractTerminationMonitorResults_Items.length;
        ClearTable(DpContractTerminationMonitorResults_ResultsTable);

        for (index = 0; index < sCount; index++) {

            currentItem = DpContractTerminationMonitorResults_Items[index];

            tr = AddRow(DpContractTerminationMonitorResults_ResultsTable);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "ClientSelect", currentItem.DPContractID, DpContractTerminationMonitorResults_RadioButton_Click);

            str = currentItem.ServiceUserName;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = currentItem.ServiceUserReference;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = currentItem.BudgetHolderName;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = currentItem.BudgetHolderReference;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = currentItem.DPContractNumber;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, currentItem.DPContractDateFrom.strftime("%d/%m/%Y"));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, currentItem.DPContractRequiredTerminationDate.strftime("%d/%m/%Y"));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);

            currentItem.DPContractUnderOverPaymentAmount = parseFloat(currentItem.DPContractUnderOverPaymentAmount.toFixed(2));
            if (currentItem.DPContractUnderOverPaymentAmount === 0) {
                td.innerHTML = currentItem.DPContractUnderOverPaymentAmount.toFixed(2);
            }
            else if (currentItem.DPContractUnderOverPaymentAmount > 0) {
                td.innerHTML = '+' + currentItem.DPContractUnderOverPaymentAmount.toFixed(2);
            } else {
                td.innerHTML = currentItem.DPContractUnderOverPaymentAmount.toFixed(2);
            } 
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";           

            // select the item?
            if (DpContractTerminationMonitorResults_SelectedID == currentItem.DPContractID || (DpContractTerminationMonitorResults_CurrentPage == 1 && DpContractTerminationMonitorResults_Items.length == 1)) {
                radioButton.click();
            }

        }

        // load the paging link HTML
        DpContractTerminationMonitorResults_PagingLinks.innerHTML = serviceReponse.value.PagingLinks;

    }

    DisplayLoading(false);

}

function DpContractTerminationMonitorResults_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = DpContractTerminationMonitorResults_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = DpContractTerminationMonitorResults_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        selectedRow = DpContractTerminationMonitorResults_ResultsTable.tBodies[0].rows[index];
        if (rdo.checked) {
            selectedRow.className = "highlightedRow"
            DpContractTerminationMonitorResults_SelectedID = rdo.value;
        } else {
            selectedRow.className = ""
        }
    }
    DpContractTerminationMonitorResults_ResetControlVisibility();
}

function DpContractTerminationMonitorResults_GetSelectedId() {
    var returnVal = parseInt(DpContractTerminationMonitorResults_SelectedID);
    return (isNaN(returnVal)) ? 0 : returnVal;
}

function DpContractTerminationMonitorResults_ResetControlVisibility() {
    var disabled = (DpContractTerminationMonitorResults_GetSelectedId() <= 0);
    DpContractTerminationMonitorResults_btnBalance.attr('disabled', disabled);
    DpContractTerminationMonitorResults_btnView.attr('disabled', disabled);
}

function DpContractTerminationMonitorResults_View() {
    document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=" + DpContractTerminationMonitorResults_GetSelectedId() + "&backUrl=" + DpContractTerminationMonitorResults_GetBackUrl();
}

function DpContractTerminationMonitorResults_Balance() {
    document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Balance.aspx?id=" + DpContractTerminationMonitorResults_GetSelectedId() + "&backUrl=" + DpContractTerminationMonitorResults_GetBackUrl();
}

function DpContractTerminationMonitorResults_GetBackUrl() {
    var url = document.location.href;
    url = RemoveQSParam(url, "dpcID");
    url = AddQSParam(url, "dpcID", DpContractTerminationMonitorResults_GetSelectedId());    
    return escape(url);
}
