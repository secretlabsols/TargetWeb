var ServiceTypeSelector_FilterRedundant, ServiceTypeSelector_FilterExcludeIds, ServiceTypeSelector_FilterExcludeServiceCategories, ServiceTypeSelector_FilterIncludeIds;
var ServiceTypeSelector_LookupService;
var ServiceTypeSelector_ResultsTable;
var ServiceTypeSelector_PagingLinks;
var ServiceTypeSelector_CurrentPage;
var ServiceTypeSelector_SelectedID;
var ServiceTypeSelector_ListFilter;
var ServiceTypeSelector_ListFilter_ServiceCategory;
var ServiceTypeSelector_ListFilter_ServiceGroupClassification;
var ServiceTypeSelector_ListFilter_ServiceGroup;
var ServiceTypeSelector_ServiceTypes;

function ServiceTypeSelector_Init() {

    ServiceTypeSelector_LookupService = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
    ServiceTypeSelector_ResultsTable = GetElement("tblServiceTypes");
    ServiceTypeSelector_PagingLinks = GetElement("ServiceType_PagingLinks");

    // setup list filters
    ServiceTypeSelector_ListFilter = new Target.Web.ListFilter(ServiceTypeSelector_ListFilter_Callback);
    ServiceTypeSelector_ListFilter.AddColumn("ServiceCategory", GetElement("thServiceCategory"));
    ServiceTypeSelector_ListFilter.AddColumn("ServiceGroupClassification", GetElement("thServiceGroupClassification"));
    ServiceTypeSelector_ListFilter.AddColumn("ServiceGroup", GetElement("thServiceGroup"));

    ServiceTypeSelector_FetchServiceTypeList(ServiceTypeSelector_CurrentPage, ServiceTypeSelector_SelectedID);
}

function ServiceTypeSelector_ListFilter_Callback(column) {
    switch (column.Name) {
        case "Category":
            ServiceTypeSelector_ListFilter_ServiceCategory = column.Filter;
            break;
        case "Classification":
            ServiceTypeSelector_ListFilter_ServiceGroupClassification = column.Filter;
            break;
        case "Service Group":
            ServiceTypeSelector_ListFilter_ServiceGroup = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    ServiceTypeSelector_FetchServiceTypeList(1, 0);
}

function ServiceTypeSelector_FetchServiceTypeList(page, selectedID) {    
    DisplayLoading(true);
    if (page == undefined) page = 0;
    if (selectedID == undefined) selectedID = 0;
    ServiceTypeSelector_CurrentPage = page;
    ServiceTypeSelector_LookupService.FetchServiceTypeList(page, selectedID, ServiceTypeSelector_ListFilter_ServiceCategory, ServiceTypeSelector_ListFilter_ServiceGroupClassification, ServiceTypeSelector_ListFilter_ServiceGroup, ServiceTypeSelector_FilterRedundant, ServiceTypeSelector_FilterExcludeIds, ServiceTypeSelector_FilterExcludeServiceCategories, ServiceTypeSelector_FilterIncludeIds, ServiceTypeSelector_FetchServiceTypeList_Callback);
}

function ServiceTypeSelector_FetchServiceTypeList_Callback(response) {
    var sts, index, tr, td, radioButton, str, link, stsCount;
    ServiceTypeSelector_ServiceTypes = null;
    if (CheckAjaxResponse(response, ServiceTypeSelector_LookupService.url)) {        
        
        sts = response.value.ServiceTypes;
        stsCount = sts.length;
        ServiceTypeSelector_ServiceTypes = sts;  
        ClearTable(ServiceTypeSelector_ResultsTable);

        for (index = 0; index < stsCount; index++) {

            tr = AddRow(ServiceTypeSelector_ResultsTable);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "ServiceTypeSelect", sts[index].ID, ServiceTypeSelector_RadioButton_Click);

            str = sts[index].ServiceCategoryDescription;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = sts[index].ServiceClassificationGroupDescription;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = sts[index].ServiceGroupDescription;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = sts[index].ServiceTypeDescription;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the st?
            if (ServiceTypeSelector_SelectedID == sts[index].ID || ( ServiceTypeSelector_CurrentPage ==  1 && sts.length == 1)) {
                radioButton.click();
            }

        }
        
        // load the paging link HTML
        ServiceTypeSelector_PagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}

function ServiceTypeSelector_RadioButton_Click() {
    var index, rdo, selectedRow, rowsLength;
    rowsLength = ServiceTypeSelector_ResultsTable.tBodies[0].rows.length;
    for (index = 0; index < rowsLength; index++) {
        rdo = ServiceTypeSelector_ResultsTable.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = ServiceTypeSelector_ResultsTable.tBodies[0].rows[index];
            ServiceTypeSelector_ResultsTable.tBodies[0].rows[index].className = "highlightedRow"
            ServiceTypeSelector_SelectedID = rdo.value;
        } else {
            ServiceTypeSelector_ResultsTable.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof ServiceTypeSelector_SelectedItemChanged == "function") {
        ServiceTypeSelector_SelectedItemChanged(ServiceTypeSelector_GetSelectedObject(ServiceTypeSelector_SelectedID));
    }
}

function ServiceTypeSelector_GetSelectedObject(id) {
    if (ServiceTypeSelector_ServiceTypes != null) {
        var collectionLength = ServiceTypeSelector_ServiceTypes.length;
        for (var j = 0; j < collectionLength; j++) {
            if (ServiceTypeSelector_ServiceTypes[j].ID == id) {
                return ServiceTypeSelector_ServiceTypes[j];
            }
        }
    }
}

addEvent(window, "load", ServiceTypeSelector_Init);