var ServiceSelector_FilterRedundant, ServiceSelector_FilterExcludeIds, ServiceSelector_FilterIncludeIds, ServiceSelector_FilterServiceTypeVisitBased, ServiceSelector_FilterExcludeIfNotAssociatedWithServiceType, ServiceSelector_FilterIncludeServiceTypeIds, ServiceSelector_FilterIncludeUomIds, ServiceSelector_FilterForUseWithDomiciliaryContracts;
var ServiceSelector_LookupService;
var ServiceSelector_ResultsTable;
var ServiceSelector_PagingLinks;
var ServiceSelector_CurrentPage;
var ServiceSelector_SelectedID;
var ServiceSelector_ListFilter;
var ServiceSelector_ListFilter_ServiceTitle;
var ServiceSelector_ListFilter_ServiceDescription;
var ServiceSelector_Services;
var ServiceSelector_FilterSavedId;

function ServiceSelector_Init() {

    ServiceSelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
    ServiceSelector_ResultsTable = GetElement("tblServices");
    ServiceSelector_PagingLinks = GetElement("Service_PagingLinks");

    // setup list filters
    ServiceSelector_ListFilter = new Target.Web.ListFilter(ServiceSelector_ListFilter_Callback);
    ServiceSelector_ListFilter.AddColumn("ServiceTitle", GetElement("thServiceTitle"));
    ServiceSelector_ListFilter.AddColumn("ServiceDescriptionn", GetElement("thServiceDescription"));

    ServiceSelector_FetchServiceList(ServiceSelector_CurrentPage, ServiceSelector_SelectedID);
}

function ServiceSelector_ListFilter_Callback(column) {
    switch (column.Name) {
        case "Domiciliary Service":
            ServiceSelector_ListFilter_ServiceTitle = column.Filter;
            break;
        case "Description":
            ServiceSelector_ListFilter_ServiceDescription = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    ServiceSelector_FetchServiceList(1, 0);
}

function ServiceSelector_FetchServiceList(page, selectedID) {   
 
    DisplayLoading(true);
    if (page == undefined) page = 0;
    if (selectedID == undefined) selectedID = 0;
    ServiceSelector_CurrentPage = page;

    ServiceSelector_LookupService.FetchServiceList(page, selectedID, ServiceSelector_ListFilter_ServiceTitle, ServiceSelector_ListFilter_ServiceDescription, ServiceSelector_FilterRedundant, ServiceSelector_FilterServiceTypeVisitBased, ServiceSelector_FilterExcludeIds, ServiceSelector_FilterIncludeIds, ServiceSelector_FilterExcludeIfNotAssociatedWithServiceType, ServiceSelector_FilterForUseWithDomiciliaryContracts, ServiceSelector_FilterIncludeUomIds, ServiceSelector_FilterIncludeServiceTypeIds, ServiceSelector_FilterSavedId, ServiceSelector_FetchServiceList_Callback);
    
}

function ServiceSelector_FetchServiceList_Callback(response) {

    var index, tr, td, radioButton, str, link, sCount;

    ServiceSelector_Services = null;
    
    if (CheckAjaxResponse(response, ServiceSelector_LookupService.url)) {

        ServiceSelector_Services = response.value.Services; 
        sCount = response.value.Services.length;         
        ClearTable(ServiceSelector_ResultsTable);

        for (index = 0; index < sCount; index++) {

            tr = AddRow(ServiceSelector_ResultsTable);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "ServiceSelect", ServiceSelector_Services[index].ID, ServiceSelector_RadioButton_Click);

            str = ServiceSelector_Services[index].Title;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = ServiceSelector_Services[index].Description;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = ServiceSelector_Services[index].BudgetCategoryDescription;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the service?
            if (ServiceSelector_SelectedID == ServiceSelector_Services[index].ID || ( ServiceSelector_CurrentPage == 1 && ServiceSelector_Services.length == 1)) {
                radioButton.click();
            }

        }
        
        // load the paging link HTML
        ServiceSelector_PagingLinks.innerHTML = response.value.PagingLinks;
    }

    DisplayLoading(false);
    
}

function ServiceSelector_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = ServiceSelector_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = ServiceSelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = ServiceSelector_ResultsTable.tBodies[0].rows[index];
            ServiceSelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            ServiceSelector_SelectedID = rdo.value;
        } else {
            ServiceSelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof ServiceSelector_SelectedItemChanged == "function") {
        ServiceSelector_SelectedItemChanged(ServiceSelector_GetSelectedObject(ServiceSelector_SelectedID));
    }
}

function ServiceSelector_GetSelectedObject(id) {
    if (ServiceSelector_Services != null) {
        var collectionLength = ServiceSelector_Services.length;
        for (var j = 0; j < collectionLength; j++) {
            if (ServiceSelector_Services[j].ID == id) {
                return ServiceSelector_Services[j];
            }
        }
    }
}

addEvent(window, "load", ServiceSelector_Init);