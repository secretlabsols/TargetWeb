var GenericCreditorSelector_LookupService;
var GenericCreditorSelector_ResultsTable;
var GenericCreditorSelector_PagingLinks;
var GenericCreditorSelector_CurrentPage;
var GenericCreditorSelector_SelectedID;
var GenericCreditorSelector_ListFilter;
var GenericCreditorSelector_ListFilter_GenericCreditorName
var GenericCreditorSelector_ListFilter_GenericCreditorReference;
var GenericCreditorSelector_GenericCreditors;
var GenericCreditorSelector_QsGenericCreditorID = "gcID";
var GenericCreditorSelector_btnView;

function GenericCreditorSelector_Init() {

    GenericCreditorSelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.CreditorPayments_class();
    GenericCreditorSelector_ResultsTable = GetElement("tblGenericCreditors");
    GenericCreditorSelector_PagingLinks = GetElement("GenericCreditors_PagingLinks");
    GenericCreditorSelector_btnView = GetElement("GenericCreditors_btnView", true);

    if (GenericCreditorSelector_btnView) GenericCreditorSelector_btnView.disabled = true;

    // setup list filters
    GenericCreditorSelector_ListFilter = new Target.Web.ListFilter(GenericCreditorSelector_ListFilter_Callback);
    GenericCreditorSelector_ListFilter.AddColumn("GenericCreditorReference", GetElement("thGenericCreditorReference"));
    GenericCreditorSelector_ListFilter.AddColumn("GenericCreditorName", GetElement("thGenericCreditorName"));

    GenericCreditorSelector_FetchGenericCreditors(GenericCreditorSelector_CurrentPage, GenericCreditorSelector_SelectedID);
}

function GenericCreditorSelector_ListFilter_Callback(column) {
    switch (column.Name) {
        case "Reference":
            GenericCreditorSelector_ListFilter_GenericCreditorReference = column.Filter;
            break;
        case "Name":
            GenericCreditorSelector_ListFilter_GenericCreditorName = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    GenericCreditorSelector_FetchGenericCreditors(1, 0);
}

function GenericCreditorSelector_FetchGenericCreditors(page, selectedID) {    
    DisplayLoading(true);
    if (page == undefined) page = 0;
    if (selectedID == undefined) selectedID = 0;
    GenericCreditorSelector_CurrentPage = page;
    GenericCreditorSelector_LookupService.GetPagedGenericCreditors(page, 10, GenericCreditorSelector_ListFilter_GenericCreditorName, GenericCreditorSelector_ListFilter_GenericCreditorReference, selectedID, GenericCreditorSelector_FetchGenericCreditors_Callback);
}

function GenericCreditorSelector_FetchGenericCreditors_Callback(response) {

    var itemSelected = false;

    GenericCreditorSelector_GenericCreditors = null;

    if (GenericCreditorSelector_btnView) GenericCreditorSelector_btnView.disabled = true;
    
    if (CheckAjaxResponse(response, GenericCreditorSelector_LookupService.url)) {

        var index, tr, td, radioButton, str, link, sCount, currentCreditor;

        GenericCreditorSelector_GenericCreditors = response.value.Items;

        if (GenericCreditorSelector_GenericCreditors) {
            sCount = GenericCreditorSelector_GenericCreditors.length;
        } else {
            sCount = 0;   
        }

        ClearTable(GenericCreditorSelector_ResultsTable);

        if (sCount > 0) {

            for (index = 0; index < sCount; index++) {

                // set the current record
                currentCreditor = GenericCreditorSelector_GenericCreditors[index];

                // create row and add radio button
                tr = AddRow(GenericCreditorSelector_ResultsTable);
                td = AddCell(tr, "");
                radioButton = AddRadio(td, "", "GenericCreditorSelect", currentCreditor.ID, GenericCreditorSelector_RadioButton_Click);

                // add other items
                GenericCreditorSelector_AddLink(tr, currentCreditor.CreditorReference, currentCreditor.ID);
                GenericCreditorSelector_AddLink(tr, currentCreditor.Name, currentCreditor.ID);
                GenericCreditorSelector_AddCell(tr, currentCreditor.TypeDescription);
                GenericCreditorSelector_AddCell(tr, currentCreditor.Address);

                // select the item?
                if (GenericCreditorSelector_SelectedID == currentCreditor.ID || (GenericCreditorSelector_CurrentPage == 1 && GenericCreditorSelector_GenericCreditors.length == 1)) {
                    radioButton.click();
                    itemSelected = true;
                }

            }

        }       
        
        // load the paging link HTML
        GenericCreditorSelector_PagingLinks.innerHTML = response.value.PagingLinks;
    }

    if (itemSelected == false) {

        if (typeof GenericCreditorSelector_SelectedItemChanged == "function") {
            GenericCreditorSelector_SelectedItemChanged(null);
        }  

    }
    
    DisplayLoading(false);
}

function GenericCreditorSelector_AddLink(tr, str, creditorID) {
    var td; 
    
    td = AddCell(tr, "");
    link = AddLink(td, str, GenericCreditorSelector_GetPlanURL(creditorID), "Click here to view this creditor.");
    link.className = "transBg";

    return td;
}

function GenericCreditorSelector_AddCell(tr, str) {

    var td;    
    if (str.length == 0) str = " ";
    td = AddCell(tr, str);
    td.style.textOverflow = "ellipsis";
    td.style.overflow = "hidden";
    return td;

}

function GenericCreditorSelector_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = GenericCreditorSelector_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = GenericCreditorSelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = GenericCreditorSelector_ResultsTable.tBodies[0].rows[index];
            GenericCreditorSelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            GenericCreditorSelector_SelectedID = rdo.value;
            if (GenericCreditorSelector_btnView) GenericCreditorSelector_btnView.disabled = false;
        } else {
            GenericCreditorSelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof GenericCreditorSelector_SelectedItemChanged == "function") {
        GenericCreditorSelector_SelectedItemChanged(GenericCreditorSelector_GetSelectedObject(GenericCreditorSelector_SelectedID));
    }
}

function GenericCreditorSelector_GetSelectedObject(id) {
    if (GenericCreditorSelector_GenericCreditors != null) {
        var collectionLength = GenericCreditorSelector_GenericCreditors.length;
        for (var j = 0; j < collectionLength; j++) {
            if (GenericCreditorSelector_GenericCreditors[j].ID == id) {
                return GenericCreditorSelector_GenericCreditors[j];
            }
        }
    }
}

function GenericCreditorSelector_BeforeNavigate() {

    var qsGenericCreditorToken = GenericCreditorSelector_QsGenericCreditorID;
    var originalID = GetQSParam(document.location.search, qsGenericCreditorToken);
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    url = AddQSParam(RemoveQSParam(url, qsGenericCreditorToken), qsGenericCreditorToken, GenericCreditorSelector_SelectedID);
    SelectorWizard_newUrl = url;

    return true;

}

function GenericCreditorSelector_MruOnChange(mruListKey, selectedValue) {

    if (selectedValue.length > 0) {

        var url = document.location.href;
        
        url = RemoveQSParam(url, GenericCreditorSelector_QsGenericCreditorID);
        url = AddQSParam(url, GenericCreditorSelector_QsGenericCreditorID, selectedValue);
        document.location.href = url;
        
    }

}

function GenericCreditorSelector_btnView_Click() {
    document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/CreditorPayments/ViewCreditor.aspx?id=" + GenericCreditorSelector_SelectedID + "&mode=1&backUrl=" + GenericCreditorSelector_GetBackUrl();
}

function GenericCreditorSelector_GetBackUrl() {
    var url = document.location.href;

    if (GenericCreditorSelector_SelectedID != 0)
        url = AddQSParam(RemoveQSParam(url, "id"), "id", GenericCreditorSelector_SelectedID);
    return escape(url);
}

addEvent(window, "load", GenericCreditorSelector_Init);


function GenericCreditorSelector_GetPlanURL(id) {
    return "javascript:GenericCreditorSelector_ShowCreditor(" + id + ");";
}

function GenericCreditorSelector_ShowCreditor(id) {
    document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/CreditorPayments/ViewCreditor.aspx?id=" + id + "&mode=1&backUrl=" + GenericCreditorSelector_GetBackUrl();
}