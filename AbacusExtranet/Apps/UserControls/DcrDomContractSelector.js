var contractSvc, currentPage, establishmentID, isCouncilUser, qs_externalAccount, qs_dcrId, qs_contractId;
var DcrDomContractSelector_selectedContractID;
var listFilter, listFilterNumber = "", listFilterTitle = "", listFilterGroup = "";
var tblContracts, divPagingLinks;
var selectedDcrDomContractId=0, selectedDcrDomContractTitle="", selectedDcrDomContractNumber = "";

function Init() {

    var url = document.location.href
    qs_externalAccount = GetQSParam(url, "qs_externalAccount");
    qs_dcrId = GetQSParam(url, "dcrId");
    qs_contractId = GetQSParam(url, "cId");
    selectedDcrDomContractId = qs_contractId;
    contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
    tblContracts = GetElement("tblContracts");
    divPagingLinks = GetElement("DcrDomContract_PagingLinks");

    // setup list filters
    listFilter = new Target.Web.ListFilter(ListFilter_Callback);
    listFilter.AddColumn("Number", GetElement("thNumber"), ((listFilterNumber.length > 0) ? listFilterNumber : null));
    listFilter.AddColumn("Title", GetElement("thTitle"), ((listFilterTitle.length > 0) ? listFilterTitle : null));
    listFilter.AddColumn("Group", GetElement("thGroup"), ((listFilterGroup.length > 0) ? listFilterGroup : null));

    // populate table
    FetchDcrDomContractList(currentPage, selectedDcrDomContractId);
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "Number":
            listFilterNumber = column.Filter;
            break;
        case "Title":
            listFilterTitle = column.Filter;
            break;
        case "Group":
            listFilterGroup = column.Filter;
            break;            
        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchDcrDomContractList(1);
}

function FetchDcrDomContractList(page, selectedID) {
    currentPage = page;
    DisplayLoading(true);
    contractSvc.FetchDcrDomContractList(page,
                                        qs_externalAccount,
                                        listFilterNumber,
                                        listFilterTitle,
                                        listFilterGroup,
                                        selectedID,
                                        qs_dcrId,
                                        FetchDcrDomContractList_Callback)
}

function FetchDcrDomContractList_Callback(response) {
    
    var index, ContractItems, str, currentItem;

    btnSelectContract.disabled = true;

    if (CheckAjaxResponse(response, contractSvc.url)) {
        ContractItems = response.value.Contracts;
        ClearTable(tblContracts);
        for (index = 0; index < ContractItems.length; index++) {
            populating = true;
            currentItem = ContractItems[index];
            tr = AddRow(tblContracts);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "ContractSelect", currentItem.ID, RadioButton_Click);

            AddCell(tr, currentItem.Number);
            AddCell(tr, currentItem.Title);
            AddCell(tr, currentItem.ProviderName);
            AddCell(tr, Date.strftime("%d/%m/%Y", currentItem.StartDate));
            AddCell(tr, Date.strftime("%d/%m/%Y", currentItem.EndDate));
            AddCell(tr, currentItem.ContractGroup);

            // select the current item?
            if (selectedDcrDomContractId == currentItem.ID || (currentPage == 1 && ContractItems.length == 1)) {
                radioButton.click();
            }            
            
        }

        populating = false;

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
            selectedDcrDomContractId = rdo.value;
            selectedDcrDomContractNumber = tblContracts.tBodies[0].rows[index].cells[1].innerHTML;
            selectedDcrDomContractTitle = tblContracts.tBodies[0].rows[index].cells[2].innerHTML;
            btnSelectContract.disabled = false;
        } else {
           tblContracts.tBodies[0].rows[index].className = ""
        }
    }
}

addEvent(window, "load", Init);