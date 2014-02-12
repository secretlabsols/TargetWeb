
var DPContractSelector_MAX_DATE = "31/12/9999", DPContractSelector_dateTemp;
var DPContractSelector_OPEN_ENDED = "(open-ended)";
var DPContractSelector_contractSvc, DPContractSelector_currentPage, DPContractSelector_clientID, DPContractSelector_budgetHolderID, DPContractSelector_dateFrom, DPContractSelector_dateTo;
var DPContractSelector_selectedContractID, DPContractSelector_btnViewID, DPContractSelector_btnCopyID;
var DPContractSelector_btnTerminateID, DPContractSelector_btnReinstateID, DPContractSelector_btnCreatePaymentsID;
var DPContractStep_required, DPContractSelector_showServiceUserColumn;
var DPContractSelector_listFilter, DPContractSelector_listFilterNumber = "", DPContractSelector_listFilterSURef = "", DPContractSelector_listFilterSUName = "";
var DPContractSelector_listFilterBHRef = "", DPContractSelector_listFilterBHName = "";
var DPContractSelector_listFilterGroup = "";
var DPContractSelector_tblContracts, DPContractSelector_divPagingLinks, DPContractSelector_btnView, btnNext, btnFinish, DPContractSelector_btnTerminate, DPContractSelector_btnReinstate, DPContractSelector_btnCreatePayments;
var DPContractSelector_canCreate, DPContractSelector_viewContractInNewWindow, DPContractSelector_canView, DPContractSelector_listIsSDS;
var DPContractSelector_CurrentContracts = [];
var DPContractSelector_btnCopy, DPContractSelector_enableCopyButton;
var DPContractSelector_CurrentContractOpenEnded;

function Init() {
    DPContractSelector_contractSvc = new Target.Abacus.Web.Apps.WebSvc.DPContract_class();
    DPContractSelector_tblContracts = GetElement("DPContractSelector_tblContracts");
    DPContractSelector_divPagingLinks = GetElement("DPContract_PagingLinks");
    DPContractSelector_btnView = GetElement(DPContractSelector_btnViewID, true);
    DPContractSelector_btnTerminate = GetElement(DPContractSelector_btnTerminateID, true);
    DPContractSelector_btnReinstate = GetElement(DPContractSelector_btnReinstateID, true);
    DPContractSelector_btnCreatePayments = GetElement(DPContractSelector_btnCreatePaymentsID, true);
    DPContractSelector_btnCopy = GetElement(DPContractSelector_btnCopyID, true);

    if (DPContractSelector_btnView) DPContractSelector_btnView.disabled = true;
    if (DPContractSelector_btnTerminate) DPContractSelector_btnTerminate.disabled = true;
    if (DPContractSelector_btnReinstate) DPContractSelector_btnReinstate.disabled = true;
    if (DPContractSelector_btnCreatePayments) DPContractSelector_btnCreatePayments.disabled = true;
    if (DPContractSelector_btnCopy) DPContractSelector_btnCopy.disabled = true;

    // setup list filters
    DPContractSelector_listFilter = new Target.Web.ListFilter(DPContractSelector_listFilter_Callback);
    DPContractSelector_listFilter.AddColumn("Number", GetElement("thNumber"));
    DPContractSelector_listFilter.AddColumn("SURef", GetElement("thSURef"));
    DPContractSelector_listFilter.AddColumn("SUName", GetElement("thSUName"));
    DPContractSelector_listFilter.AddColumn("BHRef", GetElement("thBHRef"));
    DPContractSelector_listFilter.AddColumn("BHName", GetElement("thBHName"));

    // populate table
    DPContractSelector_selectedContractID = GetQSParam(document.location.search, "dpcID");
    DPContractSelector_FetchDPContractList(DPContractSelector_currentPage, DPContractSelector_selectedContractID);
}

function DirectPaymentContractSelector_Refresh() {
    DPContractSelector_FetchDPContractList(DPContractSelector_currentPage, DPContractSelector_selectedContractID);
}

function DPContractSelector_listFilter_Callback(column) {
    switch (column.Name) {
        case "Service User":
            DPContractSelector_listFilterSUName = column.Filter;
            break;
        case "S/U Ref":
            DPContractSelector_listFilterSURef = column.Filter;
            break;
        case "Budget Holder":
            DPContractSelector_listFilterBHName = column.Filter;
            break;
        case "B/H Ref":
            DPContractSelector_listFilterBHRef = column.Filter;
            break;
        case "Contract":
            DPContractSelector_listFilterNumber = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    DPContractSelector_FetchDPContractList(1);
}

function DPContractSelector_FetchDPContractList(page, selectedID) {
    DPContractSelector_currentPage = page;
    DisplayLoading(true);
    if (selectedID == undefined) selectedID = 0;
    DPContractSelector_contractSvc.FetchDPContractList(page, selectedID, DPContractSelector_clientID, DPContractSelector_budgetHolderID, DPContractSelector_dateFrom, DPContractSelector_dateTo, DPContractSelector_listFilterNumber, DPContractSelector_listFilterSURef, DPContractSelector_listFilterSUName, DPContractSelector_listFilterBHRef, DPContractSelector_listFilterBHName, DPContractSelector_listIsSDS, DPContractSelector_FetchDPContractList_Callback)
}

function DPContractSelector_FetchDPContractList_Callback(response) {
    var contract, index;
    var tr, td, radioButton;
    var str;
    var link;

    if (DPContractSelector_btnView) DPContractSelector_btnView.disabled = true;
    if (DPContractSelector_btnTerminate) DPContractSelector_btnTerminate.disabled = true;
    if (DPContractSelector_btnReinstate) DPContractSelector_btnReinstate.disabled = true;
    if (DPContractSelector_btnCopy) DPContractSelector_btnCopy.disabled = true;

    if (DPContractSelector_btnCreatePayments) DPContractSelector_btnCreatePayments.disabled = true;

    if (CheckAjaxResponse(response, DPContractSelector_contractSvc.url)) {

        // populate the table
        contracts = response.value.Contracts;
        DPContractSelector_CurrentContracts = contracts;

        // remove existing rows
        ClearTable(DPContractSelector_tblContracts);
        for (index = 0; index < contracts.length; index++) {

            tr = AddRow(DPContractSelector_tblContracts);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "DPContractSelect", contracts[index].ID, DPContractSelector_RadioButton_Click);

            td = AddCell(tr, contracts[index].ClientName);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, contracts[index].ClientRef);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = contracts[index].BudgetHolderName;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = contracts[index].BudgetHolderRef;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            if (DPContractSelector_canView == true) {
                // The Contract Number is a hyperlink to the View Contract screen..
                td = AddCell(tr, "");
                link = AddLink(td, contracts[index].ContractNum, DPContractSelector_GetContractURL(contracts[index].ID), "Click here to view this contract.");
                link.className = "transBg";
            } else {
                td = AddCell(tr, contracts[index].ContractNum);
                td.style.textOverflow = "ellipsis";
                td.style.overflow = "hidden";
            }

            AddCell(tr, Date.strftime("%d/%m/%Y", contracts[index].DateFrom));



            DPContractSelector_dateTemp = Date.strftime("%d/%m/%Y", contracts[index].DateTo);
            if (DPContractSelector_dateTemp == DPContractSelector_MAX_DATE) {
                AddCell(tr, DPContractSelector_OPEN_ENDED);
            } else {
                AddCell(tr, DPContractSelector_dateTemp);
            }

            if (contracts[index].ID == DPContractSelector_selectedContractID || (DPContractSelector_currentPage == 1 && contracts.length == 1)) {
                radioButton.click();
            }
            if (DPContractSelector_btnCreatePayments) DPContractSelector_btnCreatePayments.disabled = false;

            AddCell(tr, contracts[index].IsSDS);
        }

        // load the paging link HTML
        DPContractSelector_divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}
function DPContractSelector_RadioButton_Click() {
    var index, rdo, selectedRow, selectedItem;

    for (index = 0; index < DPContractSelector_tblContracts.tBodies[0].rows.length; index++) {
        rdo = DPContractSelector_tblContracts.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = DPContractSelector_tblContracts.tBodies[0].rows[index];
            selectedItem = DPContractSelector_CurrentContracts[index];
            DPContractSelector_tblContracts.tBodies[0].rows[index].className = "highlightedRow"
            DPContractSelector_selectedContractID = rdo.value;
            DPContractSelector_CurrentContractOpenEnded = Date.strftime("%d/%m/%Y", DPContractSelector_CurrentContracts[index].DateTo) == DPContractSelector_MAX_DATE
            if (DPContractSelector_btnView) DPContractSelector_btnView.disabled = false;
            if (DPContractSelector_btnCopy && DPContractSelector_enableCopyButton) DPContractSelector_btnCopy.disabled = false;
            if (GetInnerText(selectedRow.cells[7]) == DPContractSelector_OPEN_ENDED) {
                if (DPContractSelector_btnTerminate) DPContractSelector_btnTerminate.disabled = false;
                if (DPContractSelector_btnReinstate) DPContractSelector_btnReinstate.disabled = true;
            } else {
                if (DPContractSelector_btnTerminate) DPContractSelector_btnTerminate.disabled = true;
                if (DPContractSelector_btnReinstate) DPContractSelector_btnReinstate.disabled = false;
            }
            if (DPContractSelector_btnCreatePayments) DPContractSelector_btnCreatePayments.disabled = false;
            if (DPContractSelector_btnTerminate) DPContractSelector_btnTerminate.title = 'Terminate?';
            if (DPContractSelector_btnReinstate) DPContractSelector_btnReinstate.title = 'Re-instate?';
            if (selectedItem.IsMaintainedExternally) {
                if (DPContractSelector_btnTerminate) {
                    DPContractSelector_btnTerminate.disabled = true;
                    DPContractSelector_btnTerminate.title = 'The selected Direct Payment Contract is maintained via an Electronic Interface and, as a result, may not be Re-instated or Terminated manually';
                }
                if (DPContractSelector_btnReinstate) {
                    DPContractSelector_btnReinstate.disabled = true;
                    DPContractSelector_btnReinstate.title = 'The selected Direct Payment Contract is maintained via an Electronic Interface and, as a result, may not be Re-instated or Terminated manually';
                }
            }
        } else {
            DPContractSelector_tblContracts.tBodies[0].rows[index].className = ""
        }
    }
}
function DPContractSelector_btnNew_Click() {
    var selClientID = GetQSParam(document.location.search, "clientID");
    var selBHID = GetQSParam(document.location.search, "bhID");
    if (selClientID == undefined) selClientID = 0;
    if (selBHID == undefined) selBHID = 0;

    // Pass on the selected client and 3rd party budget holder (if selected)..
    if (DPContractSelector_viewContractInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=0&autopopup=1&clientid=" + selClientID + "&bhID=" + selBHID, 75, 50, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=0&clientid=" + selClientID + "&bhID=" + selBHID + "&backUrl=" + DPContractSelector_GetBackUrl();
}

function DPContractSelector_GetContractURL(id) {
    if (DPContractSelector_viewContractInNewWindow)
        return "javascript:OpenPopup('" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=" + id + "&autopopup=1', 75, 50, 1);void(0);";
    else
        return "javascript:document.location.href = '" + escape(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=" + id + "&backUrl=" + DPContractSelector_GetBackUrl(1)) + "';";
}

function DPContractSelector_btnView_Click() {
    if (DPContractSelector_viewContractInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=" + DPContractSelector_selectedContractID + "&autopopup=1", 75, 50, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=" + DPContractSelector_selectedContractID + "&backUrl=" + DPContractSelector_GetBackUrl(1);
}

function DPContractSelector_btnTerminate_Click() {
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");

    if (DPContractSelector_viewContractInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Terminate.aspx?id=" + DPContractSelector_selectedContractID + "&autopopup=1", 75, 50, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Terminate.aspx?id=" + DPContractSelector_selectedContractID + "&backUrl=" + DPContractSelector_GetBackUrl(1);
}

function DPContractSelector_btnReinstate_Click() {
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");
    if (DPContractSelector_viewContractInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Reinstate.aspx?id=" + DPContractSelector_selectedContractID + "&autopopup=1", 75, 50, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Reinstate.aspx?id=" + DPContractSelector_selectedContractID + "&backUrl=" + DPContractSelector_GetBackUrl(1);
}

function DPContractSelector_GetBackUrl(addContractID) {
    var url = document.location.href;

    if (addContractID == undefined) addContractID = 0;
    if (addContractID == 1) {
        // Add the current contract to the back URL so that the contract
        // is selected when Back is pressed..
        url = RemoveQSParam(url, "dpcID");
        url = AddQSParam(url, "dpcID", DPContractSelector_selectedContractID);
    }

    return escape(url);
}

function DPContractSelector_btnCreatePayments_Click() {
    if (DPContractSelector_viewContractInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPPayment/Create.aspx?clientid=" + DPContractSelector_clientID + "&bhid=" + DPContractSelector_budgetHolderID + "&autopopup=1" + "&issds=" + DPContractSelector_listIsSDS, 75, 50, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPPayment/Create.aspx?clientid=" + DPContractSelector_clientID + "&bhid=" + DPContractSelector_budgetHolderID + "&issds=" + DPContractSelector_listIsSDS + "&backUrl=" + DPContractSelector_GetBackUrl(1);
}

function DPContractSelector_btnCopy_Click() {
    var selClientID = GetQSParam(document.location.search, "clientID");
    var selBHID = GetQSParam(document.location.search, "bhID");
    if (selClientID == undefined) selClientID = 0;
    if (selBHID == undefined) selBHID = 0;

    if (DPContractSelector_CurrentContractOpenEnded) {
        if (!confirm('This action will create an overlapping contract. Note that only 1 contract can be paid on a net basis'))
            return;
    }
    
     // Pass on the selected client and 3rd party budget holder (if selected)..
    if (DPContractSelector_viewContractInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=0&autopopup=1&clientid=" + selClientID + "&bhID=" + selBHID, 75, 50, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=0&clientid=" + selClientID + "&isCopy=true" + "&copyID=" + DPContractSelector_selectedContractID + "&bhID=" + selBHID + "&backUrl=" + DPContractSelector_GetBackUrl(1);
 
}

addEvent(window, "load", Init);
