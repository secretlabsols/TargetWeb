
var SUCLevelSelectorSvc, SUCLevelSelector_currentPage, SUCLevelSelector_selectedSUCLevelID, SUCLevelSelector_clientID, SUCLevelSelector_isSDS;
var SUCLevelSelector_tblSUCLevelSelectors, SUCLevelSelector_divPagingLinks, SUCLevelSelector_btnView, SUCLevelSelector_btnNew;

function Init() {
    SUCLevelSelectorSvc = new Target.Abacus.Web.Apps.WebSvc.SUCLevels_class();
    SUCLevelSelector_tblSUCLevelSelectors = GetElement("SUCLevelSelector_tblSUCLevelSelectors");
    SUCLevelSelector_divPagingLinks = GetElement("SUCLevelSelector_PagingLinks");
    SUCLevelSelector_btnView = GetElement("SUCLevelSelector_btnView", true);
    SUCLevelSelector_btnNew = GetElement("SUCLevelSelector_btnNew", true);

    if (SUCLevelSelector_btnView) SUCLevelSelector_btnView.disabled = true;

    // populate table
    SUCLevelSelector_FetchSUCLevelList(SUCLevelSelector_currentPage, SUCLevelSelector_selectedSUCLevelID);
}

function SUCLevelSelector_FetchSUCLevelList(page, selectedID) {
    SUCLevelSelector_currentPage = page;
    DisplayLoading(true);
    SUCLevelSelectorSvc.FetchSUCLevelList(page, selectedID, SUCLevelSelector_clientID, SUCLevelSelector_isSDS, SUCLevelSelector_FetchSUCLevelList_Callback)
}

function SUCLevelSelector_FetchSUCLevelList_Callback(response) {
    var sucLevels, index;
    var tr, td, radioButton;
    var str;
    var link;

    if (SUCLevelSelector_selectedSUCLevelID == 0) {
        if (SUCLevelSelector_btnView) SUCLevelSelector_btnView.disabled = true;
    }

    if (CheckAjaxResponse(response, SUCLevelSelectorSvc.url)) {
        // populate the table
        sucLevels = response.value.SUCLists;
        
        // remove existing rows
        ClearTable(SUCLevelSelector_tblSUCLevelSelectors);
        
        for (index = 0; index < sucLevels.length; index++) {
            
            tr = AddRow(SUCLevelSelector_tblSUCLevelSelectors);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "SUCLevelSelect", sucLevels[index].ID, SUCLevelSelector_RadioButton_Click);

            td =  AddCell(tr, Date.strftime("%d/%m/%Y", sucLevels[index].DateFrom));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td =  AddCell(tr, Date.strftime("%d/%m/%Y", sucLevels[index].DateTo));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, sucLevels[index].AssessmentTypeDesc);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(sucLevels[index].AssessedCharge).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(sucLevels[index].ChargeableSpendPlanCost).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(sucLevels[index].ContributionLevel).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr);
            td.innerHTML = String(sucLevels[index].PlannedAdditionalCost).formatCurrency();
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";            

            if (sucLevels[index].ID == SUCLevelSelector_selectedSUCLevelID || ( SUCLevelSelector_currentPage == 1 && sucLevels.length == 1))
                radioButton.click();

        }
        
        // load the paging link HTML
        SUCLevelSelector_divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    
    DisplayLoading(false);
}
function SUCLevelSelector_RadioButton_Click() {
    var index, rdo, hid;
    
    for (index = 0; index < SUCLevelSelector_tblSUCLevelSelectors.tBodies[0].rows.length; index++) {
        rdo = SUCLevelSelector_tblSUCLevelSelectors.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            SUCLevelSelector_tblSUCLevelSelectors.tBodies[0].rows[index].className = "highlightedRow"
            SUCLevelSelector_selectedSUCLevelID = rdo.value;
            if (SUCLevelSelector_btnView) SUCLevelSelector_btnView.disabled = false;
            
            hid = SUCLevelSelector_tblSUCLevelSelectors.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[1]
            if (SUCLevelSelector_btnNew) SUCLevelSelector_btnNew.disabled = (hid.value == "true");

        } else {
            SUCLevelSelector_tblSUCLevelSelectors.tBodies[0].rows[index].className = ""
        }
    }
}
function SUCLevelSelector_btnNew_Click() {
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");
    document.location.href = "../SucLevels/Edit.aspx" + qs + "&mode=2&backUrl=" + SUCLevelSelector_GetBackUrl();
}

function SUCLevelSelector_btnView_Click() {
    // mode=1 means Fetched
    document.location.href = "../SucLevels/Edit.aspx?id=" + SUCLevelSelector_selectedSUCLevelID + "&mode=1&backUrl=" + SUCLevelSelector_GetBackUrl();
}

function SUCLevelSelector_GetBackUrl() {
    var url = document.location.href;

    if (SUCLevelSelector_selectedSUCLevelID != 0)
        url = AddQSParam(RemoveQSParam(url, "sucLevelID"), "sucLevelID", SUCLevelSelector_selectedSUCLevelID);
    return escape(url);
}

//function spendPlanStep_BeforeNavigate() {
//    var originalSpID = GetQSParam(document.location.search, "spID");
//    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

//    url = AddQSParam(RemoveQSParam(url, "spID"), "spID", SUCLevelSelector_selectedSUCLevelID);
//    SelectorWizard_newUrl = url;
//    return true;
//}

addEvent(window, "load", Init);
