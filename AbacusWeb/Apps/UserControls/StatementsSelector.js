
var currentPage;
var objects;
var webSvc;
var tblResults;
var divPagingLinks;

var StatementsSelector_selectedStatementID;
var StatementsSelector_ClientID;
var StatementsSelector_viewStatementInNewWindow;
var StatementsSelector_DocumentID;

var btnNew, btnView, btnProperties;

var listFilterReference = "";
var listFilterDateFrom = "";
var listFilterDateTo = "";
var listFilterDateCreated = "";
var listFilterCreatedBy = "";

function Init() {
    webSvc = new Target.Abacus.Web.Apps.WebSvc.PersonalBudgetStatements_class();

    tblResults = GetElement("StatementsSelector_tblStatements");
    divPagingLinks = GetElement("StatementsSelector_PagingLinks");

    btnNew = GetElement("StatementsSelector_btnNew", true);
    btnView = GetElement("StatementsSelector_btnView", true);
    btnProperties = GetElement("StatementsSelector_btnProperties", true);

    if (btnView) btnView.disabled = true;
    if (btnProperties) btnProperties.disabled = true;

    // setup list filters
    listFilter = new Target.Web.ListFilter(ListFilter_Callback);
    listFilter.AddColumn("Reference", GetElement("thRef"));
    listFilter.AddColumn("Date From", GetElement("thDateFrom"));
    listFilter.AddColumn("Date To", GetElement("thDateTo"));
    listFilter.AddColumn("Date Created", GetElement("thDateCreated"));
    listFilter.AddColumn("Created By", GetElement("thCreatedBy"));

    // populate table
    FetchObjectList(currentPage, StatementsSelector_selectedStatementID);
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "Reference":
            listFilterReference = column.Filter;
            break;
        case "Date From":
            listFilterDateFrom = column.Filter;
            break;
        case "Date To":
            listFilterDateTo = column.Filter;
            break;
        case "Date Created":
            listFilterDateCreated = column.Filter;
            break;
        case "Created By":
            listFilterCreatedBy = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchObjectList(1, 0);
}

/* FETCH OBJECT LIST METHODS */
function FetchObjectList(page, selectedID) {
    currentPage = page;
    DisplayLoading(true);
    if (selectedID == undefined) selectedID = 0;

    webSvc.FetchPersonalBudgetStatementList(page, StatementsSelector_ClientID, selectedID, 
                                            listFilterReference, listFilterDateFrom,
                                            listFilterDateTo, listFilterDateCreated, 
                                            listFilterCreatedBy,
                                            FetchObjectList_Callback)
}

function FetchObjectList_Callback(response) {
    var objCounter;
    var tr, td, radioButton;
    var str;
    var link;

    if (StatementsSelector_selectedStatementID == 0) {
        if (btnView) btnView.disabled = true;
        if (btnProperties) btnProperties.disabled = true;
    }

    if (CheckAjaxResponse(response, webSvc.url)) {

        // populate the HTML table

        objects = response.value.PersonalBudgetStatements;

        // remove existing rows
        ClearTable(tblResults);

        for (objCounter = 0; objCounter < objects.length; objCounter++) {

            tr = AddRow(tblResults);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "PersonalBudgetStatementsSelect", objects[objCounter].ID, RadioButton_Click);

            if (btnView) {
                td = AddCell(tr, "");
                link = AddLink(td, objects[objCounter].Reference, StatementsSelector_GetViewURL(objects[objCounter].DocumentID), "Click here to view this statement.");
                link.className = "transBg";
            } else {
                td = AddCell(tr, objects[objCounter].Reference);
            }
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            AddCell(tr, Date.strftime("%d/%m/%Y", objects[objCounter].DateFrom, false));

            AddCell(tr, Date.strftime("%d/%m/%Y", objects[objCounter].DateTo, false));

            AddCell(tr, Date.strftime("%d/%m/%Y", objects[objCounter].CreatedDate, false));

            td = AddCell(tr, objects[objCounter].CreatedBy);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the statement?
            if (StatementsSelector_selectedStatementID == objects[objCounter].ID || ( currentPage == 1 && objects.length == 1)) {
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
    for (index = 0; index < tblResults.tBodies[0].rows.length; index++) {
        rdo = tblResults.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = tblResults.tBodies[0].rows[index];
            tblResults.tBodies[0].rows[index].className = "highlightedRow"
            StatementsSelector_selectedStatementID = rdo.value;
            StatementsSelector_DocumentID = objects[index].DocumentID;
            if (btnView) btnView.disabled = false;
            if (btnProperties) btnProperties.disabled = false;
        } else {
            tblResults.tBodies[0].rows[index].className = ""
        }
    }
}

function StatementsSelector_GetViewURL(id) {
    return SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Documents/DocumentDownloadHandler.axd?id=" + id + "&saveas=1";
}

function StatementsSelector_btnView_Click() {
    document.location.href = StatementsSelector_GetViewURL(StatementsSelector_DocumentID);
}

function StatementsSelector_btnNew_Click() {    
    if (StatementsSelector_viewStatementInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/Enquiry/NewStatement.aspx?clientid=" + StatementsSelector_ClientID + "&mode=2", 63, 25, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/Enquiry/NewStatement.aspx?clientid=" + StatementsSelector_ClientID + "&mode=2";
}

function StatementsSelector_btnProperties_Click() {
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Documents/DocumentProperties.aspx?autopopup=1&documentid=" + StatementsSelector_DocumentID;

    if (url) OpenDialog(url, 60, 30, window);
}

addEvent(window, "load", Init);
