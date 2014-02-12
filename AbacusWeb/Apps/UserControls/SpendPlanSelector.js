
var SpendPlanSelector_spendPlanSvc, SpendPlanSelector_currentPage, SpendPlanSelector_selectedSpendPlanID, 
SpendPlanSelector_selectedClientID, SpendPlanSelector_dateFrom, SpendPlanSelector_dateTo, 
SpendPlanSelector_showServiceUserColumns, SpendPlanSelector_viewSpendplanInNewWindow, SpendPlanSelector_serviceUser;
var SpendPlanSelector_tblSpendPlans, SpendPlanSelector_divPagingLinks, SpendPlanSelector_btnView;
var SpendPlanSelector_listFilter, SpendPlanSelector_listFilterSvcUserName = "", SpendPlanSelector_listFilterSvcUserRef = "", SpendPlanSelector_listFilterSpendPlanRef = "";
var SpendPlanSelector_showNewButton;
var SpendPlanSelector_divLegendGrossCostRequireRecalculation;
var SpendPlanSelector_SpendPlans;

function Init() {
    SpendPlanSelector_spendPlanSvc = new Target.Abacus.Web.Apps.WebSvc.SpendPlans_class();
    SpendPlanSelector_tblSpendPlans = GetElement("SpendPlanSelector_tblSpendPlans");
    SpendPlanSelector_divPagingLinks = GetElement("SpendPlan_PagingLinks");
    SpendPlanSelector_btnView = GetElement("SpendPlanSelector_btnView", true);
    SpendPlanSelector_divLegendGrossCostRequireRecalculation = GetElement('SpendPlanSelector_divLegendGrossCostRequireRecalculation');

    if (SpendPlanSelector_btnView) SpendPlanSelector_btnView.disabled = true;

    // setup list filters
    SpendPlanSelector_listFilter = new Target.Web.ListFilter(SpendPlanSelector_listFilter_Callback);
    if (SpendPlanSelector_showServiceUserColumns)
    {
        SpendPlanSelector_listFilter.AddColumn("Service User", GetElement("SpendPlanSelector_thSvcUserName"));
        SpendPlanSelector_listFilter.AddColumn("S/U Ref", GetElement("SpendPlanSelector_thSvcUserRef"));
    }
    SpendPlanSelector_listFilter.AddColumn("Reference", GetElement("SpendPlanSelector_thOrderRef"));

    // populate table
    SpendPlanSelector_FetchSpendPlanList(SpendPlanSelector_currentPage, SpendPlanSelector_selectedSpendPlanID);
}

function SpendPlanSelector_FetchSpendPlanList(page, selectedID) {
    SpendPlanSelector_currentPage = page;
    DisplayLoading(true);
    SpendPlanSelector_spendPlanSvc.FetchSpendPlanList(page, selectedID,SpendPlanSelector_selectedClientID, SpendPlanSelector_dateFrom, SpendPlanSelector_dateTo, SpendPlanSelector_listFilterSvcUserName, SpendPlanSelector_listFilterSvcUserRef, SpendPlanSelector_listFilterSpendPlanRef, SpendPlanSelector_FetchSpendPlanList_Callback)
}

function SpendPlanSelector_listFilter_Callback(column) {
    switch (column.Name) {
        case "Service User":
            SpendPlanSelector_listFilterSvcUserName = column.Filter;
            break;
        case "S/U Ref":
            SpendPlanSelector_listFilterSvcUserRef = column.Filter;
            break;
        case "Reference":
            SpendPlanSelector_listFilterSpendPlanRef = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    SpendPlanSelector_FetchSpendPlanList(1);
}

function SpendPlanSelector_Refresh() {
    SpendPlanSelector_FetchSpendPlanList(SpendPlanSelector_currentPage, SpendPlanSelector_selectedSpendPlanID);
}

function SpendPlanSelector_FetchSpendPlanList_Callback(response) {
    var index, currentSpendPlan;
    var tr, td, radioButton;
    var str;
    var link;

    SpendPlanSelector_SpendPlans = null;

    if (SpendPlanSelector_btnView) SpendPlanSelector_btnView.disabled = true;

    SpendPlanSelector_divLegendGrossCostRequireRecalculation.style.display = 'none';

    if (CheckAjaxResponse(response, SpendPlanSelector_spendPlanSvc.url)) {
        // populate the table

        SpendPlanSelector_SpendPlans = response.value.SpendPlans;
        
        // remove existing rows
        ClearTable(SpendPlanSelector_tblSpendPlans);

        for (index = 0; index < SpendPlanSelector_SpendPlans.length; index++) {

            currentSpendPlan = SpendPlanSelector_SpendPlans[index];
            
            tr = AddRow(SpendPlanSelector_tblSpendPlans);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "SpendPlanSelect", currentSpendPlan.ID, SpendPlan_RadioButton_Click);

            if (SpendPlanSelector_showServiceUserColumns)
            {
                td = AddCell(tr, currentSpendPlan.ServiceUser);
                td.style.textOverflow = "ellipsis";
                td.style.overflow = "hidden";

                td = AddCell(tr, currentSpendPlan.ServiceUserReference);
                td.style.textOverflow = "ellipsis";
                td.style.overflow = "hidden";
            }

            td = AddCell(tr, "");
            link = AddLink(td,
                currentSpendPlan.Reference,
                "javascript:SpendPlanSelector_View(" + currentSpendPlan.ID + ");", "View this Spend Plan.");
            link.className = "transBg";

            AddCell(tr, Date.strftime("%d/%m/%Y", currentSpendPlan.DateFrom));

            AddCell(tr, Date.strftime("%d/%m/%Y", currentSpendPlan.DateTo));

            str = currentSpendPlan.ServiceDeliveredVia;
            if (str.length == 0) {
                str = " ";
            }
            AddCell(tr, str);

            td = AddCell(tr);
            td.innerHTML = String(currentSpendPlan.CurrentSpcGrossAnnualCost).formatCurrency();
            if (currentSpendPlan.CurrentSpcReconsider == true) {
                td.style.color = 'red';
                SpendPlanSelector_divLegendGrossCostRequireRecalculation.style.display = 'block';
            }

            if (currentSpendPlan.ID == SpendPlanSelector_selectedSpendPlanID || (SpendPlanSelector_currentPage == 1 && SpendPlanSelector_SpendPlans.length == 1))
                radioButton.click();

        }
        
        // load the paging link HTML
        SpendPlanSelector_divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    
    DisplayLoading(false);
}
function SpendPlan_RadioButton_Click() {
    var index, rdo, hid;
    
    for (index = 0; index < SpendPlanSelector_tblSpendPlans.tBodies[0].rows.length; index++) {
        rdo = SpendPlanSelector_tblSpendPlans.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            SpendPlanSelector_tblSpendPlans.tBodies[0].rows[index].className = "highlightedRow"
            SpendPlanSelector_selectedSpendPlanID = rdo.value;
            if (SpendPlanSelector_btnView) SpendPlanSelector_btnView.disabled = false;
            
            hid = SpendPlanSelector_tblSpendPlans.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[1]

        } else {
            SpendPlanSelector_tblSpendPlans.tBodies[0].rows[index].className = ""
        }
    }
}
function SpendPlanSelector_btnNew_Click() {
    var qs = document.location.search;
    qs = RemoveQSParam(qs, "currentStep");
    qs = RemoveQSParam(qs, "dateFrom");
    qs = RemoveQSParam(qs, "dateTo");

    if (SpendPlanSelector_viewSpendplanInNewWindow)
        OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/SpendPlans/Edit.aspx" + qs + "&autopopup=1&mode=2", 75, 50, 1);
    else
        document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/SpendPlans/Edit.aspx" + qs + "&mode=2&backUrl=" + SpendPlanSelector_GetBackUrl();
}

function SpendPlanSelector_btnView_Click() {
    SpendPlanSelector_View();
}

function SpendPlanSelector_GetBackUrl() {
    var url = document.location.href;

    if (SpendPlanSelector_selectedSpendPlanID != 0)
        url = AddQSParam(RemoveQSParam(url, "spID"), "spID", SpendPlanSelector_selectedSpendPlanID);
    return escape(url);
}

function spendPlanStep_BeforeNavigate() {
    var originalSpID = GetQSParam(document.location.search, "spID");
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);

//    // contract is required?
//    if (SpendPlanSelector_selectedSpendPlanID == 0) {
//        alert("Please select a spend plan.");
//        return false;
//    }

    url = AddQSParam(RemoveQSParam(url, "spID"), "spID", SpendPlanSelector_selectedSpendPlanID);
    SelectorWizard_newUrl = url;
    return true;
}

function SpendPlanSelector_View(id) {

    if (id == undefined || id <= 0) {
        id = SpendPlanSelector_selectedSpendPlanID;
    }

    if (id > 0) {
        if (SpendPlanSelector_viewSpendplanInNewWindow) {
            OpenPopup(SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/SpendPlans/Edit.aspx?id=" + id + "&autopopup=1&mode=1", 75, 50, 1);
        } else {
            document.location.href = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/ServiceUsers/SpendPlans/Edit.aspx?id=" + id + "&mode=1&backUrl=" + SpendPlanSelector_GetBackUrl();
        }
    } else {
        alert("Please select a Spend Plan");
    }
}

addEvent(window, "load", Init);
