var providerSvc, currentPage, selectedExternalAccountID = 0, selectedExternalAccount;
var tblEA, divPagingLinks;
var populating = false;
var listFilterExternalAccount = "", listFilterEmailAddress = "", listFilterFullName = "";

function Init() {
    providerSvc = new Target.Abacus.Extranet.Apps.WebSvc.DurationClaimedRounding_class();
    tblEA = GetElement("tblEA");
    divPagingLinks = GetElement("EA_PagingLinks");



    // setup list filters
    listFilter = new Target.Web.ListFilter(ListFilter_Callback);
    listFilter.AddColumn("External Account", GetElement("thExternalAccount"), ((listFilterExternalAccount.length > 0) ? listFilterExternalAccount : null));
    listFilter.AddColumn("Email Address", GetElement("thEmailAddress"), ((listFilterEmailAddress.length > 0) ? listFilterEmailAddress : null));
    listFilter.AddColumn("Full Name", GetElement("thFullName"), ((listFilterFullName.length > 0) ? listFilterFullName : null));
    FetchEAItems(currentPage, selectedExternalAccountID);
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "External Account":
            listFilterExternalAccount = column.Filter;
            break;
        case "Email Address":
            listFilterEmailAddress = column.Filter;
            break;
        case "Full Name":
            listFilterFullName = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchEAItems(1);
}

function FetchEAItems(page, selectedId) {
    currentPage = page;
    DisplayLoading(true);
    providerSvc.FetchInPlaceExternalAccountEnquiryResults(
    	    currentPage,
    	    listFilterExternalAccount,
    	    listFilterEmailAddress,
    	    listFilterFullName,
    	    selectedId,
    	    FetchEAItems_Callback);
}

function FetchEAItems_Callback(response) {

    var index, EAItems, str, currentItem;

    if (CheckAjaxResponse(response, providerSvc.url)) {
    
        EAItems = response.value.EAItems;
        ClearTable(tblEA);
        
        for (index = 0; index < EAItems.length; index++) {
            populating = true;
            currentItem = EAItems[index];
            tr = AddRow(tblEA);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "EASelect", currentItem.EAID, RadioButton_Click);

            AddCell(tr, currentItem.ExternalAccount);
            str = currentItem.EmailAddress;
            if (!str || str.length == 0) str = " ";
            AddCell(tr, str);
            str = currentItem.FullName;
            if (!str || str.length == 0) str = " ";
            AddCell(tr, str);

            // select the current item?
            if (selectedExternalAccountID == currentItem.EAID || (currentPage == 1 && EAItems.length == 1)) {
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

    for (index = 0; index < tblEA.tBodies[0].rows.length; index++) {
        rdo = tblEA.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = tblEA.tBodies[0].rows[index];
            tblEA.tBodies[0].rows[index].className = "highlightedRow"
            selectedExternalAccountID = rdo.value;
            selectedExternalAccount = tblEA.tBodies[0].rows[index].cells[1].innerHTML;
        } else {
            tblEA.tBodies[0].rows[index].className = ""
        }
    }
}

addEvent(window, "load", Init);
