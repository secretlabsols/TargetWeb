
var contractSvc, currentPage, establishmentID, contractType, contractGroupID, dateFrom, dateTo, contractEndReasonID, serviceGroupID, serviceGroupClassificationID, ContractSelector_showProviderColumn, ContractSelector_GenericCreditorID;
var ContractSelector_selectedContractID, ContractSelector_btnViewID, ContractSelector_btnCopyID, ContractSelector_Types;
var ContractSelector_btnTerminateID, ContractSelector_btnReinstateID;
var ContractStep_required, ContractSelector_showCreditorColumn;
var listFilter, listFilterNumber = "", listFilterTitle = "", listFilterSU = "", listFilterCreditor = "";
var listFilterGroup = "", listFilterSvcGroup = "";
var tblContracts, divPagingLinks, btnView, btnCopy, btnNext, btnFinish, btnTerminate, btnReinstate;

function Init() {
    contractSvc = new Target.Abacus.Web.Apps.WebSvc.CreditorPayments_class();
    tblContracts = GetElement("tblContracts");
    divPagingLinks = GetElement("Contract_PagingLinks");
    btnView = GetElement(ContractSelector_btnViewID, true);
    btnCopy = GetElement(ContractSelector_btnCopyID, true);
    btnNext = GetElement("SelectorWizard1_btnNext", true);
    btnFinish = GetElement("SelectorWizard1_btnFinish", true);
    btnTerminate = GetElement(ContractSelector_btnTerminateID, true);
    btnReinstate = GetElement(ContractSelector_btnReinstateID, true);

    if (btnView) btnView.disabled = true;
    if (btnCopy) btnCopy.disabled = true;
    if (btnTerminate) btnTerminate.disabled = true;
    if (btnReinstate) btnReinstate.disabled = true;

    // setup list filters
    listFilter = new Target.Web.ListFilter(ListFilter_Callback);
    listFilter.AddColumn("Number", GetElement("thNumber"));
    listFilter.AddColumn("Title", GetElement("thTitle"));
    if (ContractSelector_showCreditorColumn) listFilter.AddColumn("Creditor", GetElement("thCreditor"));
    listFilter.AddColumn("Contract Group", GetElement("thGroup"));
    listFilter.AddColumn("Service Group", GetElement("thSvcGroup"));

    // populate table
    FetchContractList(currentPage, ContractSelector_selectedContractID);
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "Number":
            listFilterNumber = column.Filter;
            break;
        case "Title":
            listFilterTitle = column.Filter;
            break;
        case "Creditor":
            listFilterCreditor = column.Filter;
            break;
        case "Contract Grp":
            listFilterGroup = column.Filter;
            break;
        case "Service Grp":
            listFilterSvcGroup = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchContractList(1);
}

function FetchContractList(page, selectedID) {
    currentPage = page;
    DisplayLoading(true);
    if (selectedID == undefined) selectedID = 0;
    contractSvc.GetPagedGenericContracts(page, selectedID, establishmentID, contractType, contractGroupID, dateFrom, dateTo, listFilterNumber, listFilterTitle, listFilterSU, listFilterGroup, listFilterSvcGroup, contractEndReasonID, serviceGroupID, serviceGroupClassificationID, listFilterCreditor, ContractSelector_GenericCreditorID, ContractSelector_Types, FetchContractList_Callback)
}

function FetchContractList_Callback(response) {
    var contract, index;
    var tr, td, radioButton;
    var str;
    var link;

    if (ContractSelector_selectedContractID == 0) {
        if (btnView) btnView.disabled = true;
        if (btnCopy) btnCopy.disabled = true;
        if (btnTerminate) btnTerminate.disabled = true;
        if (btnReinstate) btnReinstate.disabled = true;
    }
    // disable next/finish buttons by default if the step is mandatory
    if (ContractStep_required) {
        if (btnNext) btnNext.disabled = true;
        if (btnFinish) btnFinish.disabled = true;
        if (btnTerminate) btnTerminate.disabled = true;
        if (btnReinstate) btnReinstate.disabled = true;
    }

    if (CheckAjaxResponse(response, contractSvc.url)) {

        // populate the table
        contracts = response.value.Contracts;

        // remove existing rows
        ClearTable(tblContracts);
        for (index = 0; index < contracts.length; index++) {

            tr = AddRow(tblContracts);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "ContractSelect", contracts[index].ID, RadioButton_Click);

            td = AddCell(tr, contracts[index].Number);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, contracts[index].Title);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            if (ContractSelector_showProviderColumn) {
                str = contracts[index].ProviderName;
                if (!str || str.length == 0) str = " ";
                td = AddCell(tr, str);
                td.style.textOverflow = "ellipsis";
                td.style.overflow = "hidden";
            }

            if (ContractSelector_showCreditorColumn) {
                str = contracts[index].CreditorName;
                if (!str || str.length == 0) str = " ";
                td = AddCell(tr, str);
                td.style.textOverflow = "ellipsis";
                td.style.overflow = "hidden";
            }

            AddCell(tr, Date.strftime("%d/%m/%Y", contracts[index].StartDate));

            AddCell(tr, Date.strftime("%d/%m/%Y", contracts[index].EndDate));

            td = AddCell(tr, " ");
            if (contracts[index].ContractGroup) SetInnerText(td, contracts[index].ContractGroup);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, " ");
            if (contracts[index].ServiceGroup) SetInnerText(td, contracts[index].ServiceGroup);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            //			str = contracts[index].EndReason;
            //			if(!str || str.length == 0) str = " ";
            //			td = AddCell(tr, str);
            //			td.style.textOverflow = "ellipsis";
            //			td.style.overflow = "hidden";

            if (contracts[index].ID == ContractSelector_selectedContractID || ( currentPage == 1 && contracts.length == 1))
                radioButton.click();

        }
        // load the paging link HTML
        divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}
function RadioButton_Click() {
    var index, rdo, selectedRow;

    for (index = 0; index < tblContracts.tBodies[0].rows.length; index++) {
        rdo = tblContracts.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = tblContracts.tBodies[0].rows[index];
            tblContracts.tBodies[0].rows[index].className = "highlightedRow"
            ContractSelector_selectedContractID = rdo.value;
            if (btnView) btnView.disabled = false;
            if (btnCopy) btnCopy.disabled = false;
            if (GetInnerText(selectedRow.cells[5]) == " ") {
                if (btnTerminate) btnTerminate.disabled = false;
                if (btnReinstate) btnReinstate.disabled = true;
            } else {
                if (btnTerminate) btnTerminate.disabled = true;
                if (btnReinstate) btnReinstate.disabled = false;
            }
        } else {
            tblContracts.tBodies[0].rows[index].className = ""
        }
    }
    if (ContractStep_required) {
        if (btnNext) btnNext.disabled = false;
        if (btnFinish) btnFinish.disabled = false;
        if (GetInnerText(selectedRow.cells[5]) == " ") {
            if (btnTerminate) btnTerminate.disabled = false;
            if (btnReinstate) btnReinstate.disabled = true;
        } else {
            if (btnTerminate) btnTerminate.disabled = true;
            if (btnReinstate) btnReinstate.disabled = false;
        }
    }
    if (typeof ContractSelector_SelectedItemChange == "function") {
        var args = new Array(2);
        args[0] = ContractSelector_selectedContractID;
        args[1] = GetInnerText(selectedRow.cells[1]);
        args[2] = GetInnerText(selectedRow.cells[2]);
        ContractSelector_SelectedItemChange(args);
    }
}
function btnNew_Click() {
    var response, serviceGroupID;

    response = contractSvc.NoServiceGroupsAvailableToUser(true).value;
    if (response == 0) {
        alert("You do not have permission to set up a contract for a Service Group.");
    } else if (response == 1) {
        response = contractSvc.FetchOnlyAvailableServiceGroupToUser().value;
        if (response > 0) {
            InPlaceServiceGroup_ItemSelected(response, "");
        }
    } else {
        var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/ServiceGroups.aspx?&fUser=true";
        var dialog = OpenDialog(url, 60, 32, window);
    }

}

function InPlaceServiceGroup_ItemSelected(id, name) {
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");
    qs = RemoveQSParam(qs, "svcGroupID");

    document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/Contracts/Edit.aspx" + qs + "&svcGroupID=" + id + "&backUrl=" + GetBackUrl();
}

function btnTerminate_Click() {
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");
    document.location.href = "Terminate.aspx?id=" + ContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
}
function btnReinstate_Click() {
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");
    document.location.href = "Reinstate.aspx?id=" + ContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
}
function btnView_Click() {
    document.location.href = "Edit.aspx?id=" + ContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
}
function btnCopy_Click() {
    document.location.href = "Edit.aspx?copyFromID=" + ContractSelector_selectedContractID + "&backUrl=" + GetBackUrl();
}
function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function ContractStep_BeforeNavigate() {
    var originalContractID = GetQSParam(document.location.search, "contractID");
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    // contract is required?
    if (ContractStep_required && ContractSelector_selectedContractID == 0) {
        alert("Please select a contract.");
        return false;
    }

    url = AddQSParam(RemoveQSParam(url, "contractID"), "contractID", ContractSelector_selectedContractID);
    SelectorWizard_newUrl = url;
    return true;
}

function ContractSelector_MruOnChange(mruListKey, selectedValue) {
    if (selectedValue.length > 0) {
        var url = document.location.href;
        url = RemoveQSParam(url, "contractID");
        url = AddQSParam(url, "contractID", selectedValue);
        document.location.href = url;
    }
}

addEvent(window, "load", Init);
