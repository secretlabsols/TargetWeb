
var ServiceOrderSelector_serviceSvc, ServiceOrderSelector_currentPage, ServiceOrderSelector_selectedServiceOrderID, 
ServiceOrderSelector_providerID, ServiceOrderSelector_contractID, ServiceOrderSelector_clientID, ServiceOrderSelector_dateFrom, 
ServiceOrderSelector_dateTo, ServiceOrderSelector_serviceGroupID;
var ServiceOrderSelector_serviceOrderStep_required, ServiceOrderSelector_viewServiceOrderInNewWindow;
var ServiceOrderSelector_tblOrders, ServiceOrderSelector_divPagingLinks, ServiceOrderSelector_btnView, ServiceOrderSelector_btnNew, ServiceOrderSelector_btnCopy;
var ServiceOrderSelector_listFilter, ServiceOrderSelector_listFilterSvcUserName = "", ServiceOrderSelector_listFilterSvcUserRef = "", ServiceOrderSelector_listFilterProvider = "";
var ServiceOrderSelector_listFilterOrderRef = "", ServiceOrderSelector_listFilterContract = "", ServiceOrderSelector_listFilterServiceGrp = "";
var ServiceOrderSelector_btnNewID, ServiceOrderSelector_btnCopyID;
var ServiceOrderSelector_selectedGenericServiceOrderID;

function Init() {
    ServiceOrderSelector_serviceSvc = new Target.Abacus.Web.Apps.WebSvc.ServiceOrder_class();
    ServiceOrderSelector_tblOrders = GetElement("ServiceOrderSelector_tblOrders");
    ServiceOrderSelector_divPagingLinks = GetElement("serviceOrder_PagingLinks");
    ServiceOrderSelector_btnView = GetElement("ServiceOrderSelector_btnView", true);
    ServiceOrderSelector_btnNew = GetElement(ServiceOrderSelector_btnNewID, true);
    ServiceOrderSelector_btnCopy = GetElement(ServiceOrderSelector_btnCopyID, true);

    if (ServiceOrderSelector_btnView) ServiceOrderSelector_btnView.disabled = true;
    if (ServiceOrderSelector_btnCopy) ServiceOrderSelector_btnCopy.disabled = true;

    // setup list filters
    ServiceOrderSelector_listFilter = new Target.Web.ListFilter(ServiceOrderSelector_listFilter_Callback);
    ServiceOrderSelector_listFilter.AddColumn("Service User", GetElement("thSvcUserName"));
    ServiceOrderSelector_listFilter.AddColumn("S/U Ref", GetElement("thSvcUserRef"));
    ServiceOrderSelector_listFilter.AddColumn("Provider", GetElement("thProvider"));
    ServiceOrderSelector_listFilter.AddColumn("Contract", GetElement("thContract"));
    ServiceOrderSelector_listFilter.AddColumn("Order Ref", GetElement("thOrderRef"));
    ServiceOrderSelector_listFilter.AddColumn("Service Grp", GetElement("thServiceGrp"));

    // populate table
    ServiceOrderSelector_FetchServiceOrderList(ServiceOrderSelector_currentPage, ServiceOrderSelector_selectedServiceOrderID);
}

function ServiceOrderSelector_Refresh() {
    ServiceOrderSelector_FetchServiceOrderList(ServiceOrderSelector_currentPage, ServiceOrderSelector_selectedServiceOrderID);
}

function ServiceOrderSelector_FetchServiceOrderList(page, selectedID) {
    ServiceOrderSelector_currentPage = page;
    if (selectedID == undefined) selectedID = 0;
    DisplayLoading(true);
    ServiceOrderSelector_serviceSvc.FetchServiceOrderList(page, selectedID, ServiceOrderSelector_providerID, ServiceOrderSelector_contractID, ServiceOrderSelector_clientID, ServiceOrderSelector_dateFrom, ServiceOrderSelector_dateTo, ServiceOrderSelector_serviceGroupID, ServiceOrderSelector_listFilterSvcUserName, ServiceOrderSelector_listFilterSvcUserRef, ServiceOrderSelector_listFilterProvider, ServiceOrderSelector_listFilterOrderRef, ServiceOrderSelector_listFilterContract, ServiceOrderSelector_listFilterServiceGrp, ServiceOrderSelector_FetchServiceOrderList_Callback)
}

function ServiceOrderSelector_listFilter_Callback(column) {
    switch (column.Name) {
        case "Service User":
            ServiceOrderSelector_listFilterSvcUserName = column.Filter;
            break;
        case "S/U Ref":
            ServiceOrderSelector_listFilterSvcUserRef = column.Filter;
            break;
        case "Provider":
            ServiceOrderSelector_listFilterProvider = column.Filter;
            break;
        case "Contract":
            ServiceOrderSelector_listFilterContract = column.Filter;
            break;
        case "Order Ref":
            ServiceOrderSelector_listFilterOrderRef = column.Filter;
            break;
        case "Service Grp":
            ServiceOrderSelector_listFilterServiceGrp = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    ServiceOrderSelector_FetchServiceOrderList(1, 0);
}


function ServiceOrderSelector_FetchServiceOrderList_Callback(response) {
    var orders, index;
    var tr, td, radioButton;
    var str;
    var link;

    if (ServiceOrderSelector_btnView) ServiceOrderSelector_btnView.disabled = true;
    if (ServiceOrderSelector_btnCopy) ServiceOrderSelector_btnCopy.disabled = true;

    if (CheckAjaxResponse(response, ServiceOrderSelector_serviceSvc.url)) {

        // populate the table
        orders = response.value.Orders;

        // remove existing rows
        ClearTable(ServiceOrderSelector_tblOrders);
        for (index = 0; index < orders.length; index++) {

            tr = AddRow(ServiceOrderSelector_tblOrders);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "ServiceOrderSelect", orders[index].ChildID, ServiceOrderSelector_RadioButton_Click);
            // add hidden field with DSOs Maint Via Electronic Interface
            AddInput(td, "hidFlag", "hidden", "", "", orders[index].DSOsMaintainedViaElectronicInterface);
            AddInput(td, "hidGenericDSOID", "hidden", "", "", orders[index].ID);

            td = AddCell(tr, orders[index].ClientName);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, orders[index].ClientRef);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, orders[index].ProviderName);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, orders[index].ContractNo);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, orders[index].OrderReference);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            AddCell(tr, Date.strftime("%d/%m/%Y", orders[index].DateFrom));

            AddCell(tr, Date.strftime("%d/%m/%Y", orders[index].DateTo));

            td = AddCell(tr, orders[index].ServiceGroup);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            if (orders[index].ChildID == ServiceOrderSelector_selectedServiceOrderID || ( ServiceOrderSelector_currentPage == 1 && orders.length == 1))
                radioButton.click();

            if (orders[index].ID == ServiceOrderSelector_selectedGenericServiceOrderID || (ServiceOrderSelector_currentPage == 1 && orders.length == 1))
                radioButton.click();

        }
        // load the paging link HTML
        ServiceOrderSelector_divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}
function ServiceOrderSelector_RadioButton_Click() {
    var index, rdo, hid, selectedRow;

    for (index = 0; index < ServiceOrderSelector_tblOrders.tBodies[0].rows.length; index++) {
        rdo = ServiceOrderSelector_tblOrders.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            ServiceOrderSelector_tblOrders.tBodies[0].rows[index].className = "highlightedRow"
            ServiceOrderSelector_selectedServiceOrderID = rdo.value;
            selectedRow = ServiceOrderSelector_tblOrders.tBodies[0].rows[index];
            if (ServiceOrderSelector_btnView) ServiceOrderSelector_btnView.disabled = false;
            if (ServiceOrderSelector_btnCopy) ServiceOrderSelector_btnCopy.disabled = false;

            hid = ServiceOrderSelector_tblOrders.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[1]
            ServiceOrderSelector_selectedGenericServiceOrderID = ServiceOrderSelector_tblOrders.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[2].value;
            if (ServiceOrderSelector_btnNew) ServiceOrderSelector_btnNew.disabled = (hid.value == "true");

        } else {
            ServiceOrderSelector_tblOrders.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof GenericServiceOrderSelector_SelectedItemChange == "function") {
        var args = new Array(2);
        args[0] = ServiceOrderSelector_selectedServiceOrderID;
        args[1] = GetInnerText(selectedRow.cells[5]);
        args[2] = ServiceOrderSelector_selectedGenericServiceOrderID;
        GenericServiceOrderSelector_SelectedItemChange(args);
    }
}
function ServiceOrderSelector_btnNew_Click() {
    CareTypeSelector_Show(ServiceOrderSelector_btnNew_CallBack);
}
function ServiceOrderSelector_btnNew_CallBack(evt, args) {
    var type;
    var answer = args[1];
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");
    qs = RemoveQSParam(qs, "dsoID");

    //ok Clicked
    if (answer == 1) {
        //get the value off the dialogue
        type = CareTypeSelector_GetSelectedCareType();
        if (type == 1) {
            //Residential
            
        } else if (type == 2) {
            //Non Residential
            // mode=2 means AddNew
            
             if (ServiceOrderSelector_viewServiceOrderInNewWindow)
                 OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/svcOrders/Edit.aspx" + qs + "&autopopup=1&mode=2", 75, 50, 1);
             else
                document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/svcOrders/Edit.aspx" + qs + "&mode=2&backUrl=" + ServiceOrderSelector_GetBackUrl();
        } else if (type = 3) {
            //Direct Payment
        }

    }
    
    CareTypeSelector_Hide(args);
    
    
    
}
function ServiceOrderSelector_btnView_Click() {
    // mode=1 means Fetched
     if (ServiceOrderSelector_viewServiceOrderInNewWindow)
         OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/svcOrders/Edit.aspx?id=" + ServiceOrderSelector_selectedServiceOrderID + "&autopopup=1&mode=1", 75, 50, 1);
     else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/svcOrders/Edit.aspx?id=" + ServiceOrderSelector_selectedServiceOrderID + "&mode=1&backUrl=" + ServiceOrderSelector_GetBackUrl();
}
function ServiceOrderSelector_btnCopy_Click() {
    // mode=2 means AddNew
    if (ServiceOrderSelector_viewServiceOrderInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/svcOrders/Edit.aspx?cid=" + ServiceOrderSelector_selectedServiceOrderID + "&autopopup=1&mode=2", 75, 50, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/svcOrders/Edit.aspx?cid=" + ServiceOrderSelector_selectedServiceOrderID + "&mode=2&backUrl=" + ServiceOrderSelector_GetBackUrl();
}
function ServiceOrderSelector_GetBackUrl() {
    var url = document.location.href;

    if (ServiceOrderSelector_selectedServiceOrderID != 0)
        url = AddQSParam(RemoveQSParam(url, "dsoID"), "dsoID", ServiceOrderSelector_selectedServiceOrderID);
    return escape(url);
}

function serviceOrderStep_BeforeNavigate() {
    var originalDsoID = GetQSParam(document.location.search, "dsoID");
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

    // contract is required?
    if (ServiceOrderSelector_serviceOrderStep_required && ServiceOrderSelector_selectedServiceOrderID == 0) {
        alert("Please select a service order.");
        return false;
    }

    url = AddQSParam(RemoveQSParam(url, "dsoID"), "dsoID", ServiceOrderSelector_selectedServiceOrderID);
    SelectorWizard_newUrl = url;
    return true;
}

addEvent(window, "load", Init);
