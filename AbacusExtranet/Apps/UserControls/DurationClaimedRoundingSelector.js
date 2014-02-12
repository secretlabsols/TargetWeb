var providerSvc, currentPage, clientID, isCouncilUser, userHasAddNewCommand;
var tblDCR, divPagingLinks, btnView, btnCopy, btnNew;
var populating = false;
var listFilterDescription = "", listFilterReference = "", listFilterExternalAccount = "";
var qs_DcrID, qs_Reference, qs_DcrDesc, qs_DcrEcternalAccount;



function Init() {
    providerSvc = new Target.Abacus.Extranet.Apps.WebSvc.DurationClaimedRounding_class();
    tblDCR = GetElement("tblDCR");
    divPagingLinks = GetElement("DCR_PagingLinks");
    btnView = GetElement("btnView");
    btnNew = GetElement("btnNew");
    btnCopy = GetElement("btnCopy");

    // disable if user donot have access
    if (!userHasAddNewCommand) {
        btnNew.style.display = 'none';
        btnCopy.style.display = 'none';
    }
    btnView.disabled = true;
    btnCopy.disabled = true;

    // setup list filters
    listFilter = new Target.Web.ListFilter(ListFilter_Callback);
    listFilter.AddColumn("Reference", GetElement("thRef"), ((listFilterReference.length > 0) ? listFilterReference : null));
    listFilter.AddColumn("Description", GetElement("thDescription"), ((listFilterExternalAccount.length > 0) ? listFilterExternalAccount : null));
    listFilter.AddColumn("External Account", GetElement("thExternalAccount"), ((listFilterDescription.length > 0) ? listFilterDescription : null));

    FetchDCRItems(currentPage, qs_DcrID);
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "Reference":
            listFilterReference = column.Filter;
            break;
        case "External Account":
            listFilterExternalAccount = column.Filter;
            break;
        case "Description":
            listFilterDescription = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchDCRItems(1);
}

function FetchDCRItems(page, selectedId) {
    currentPage = page;
    DisplayLoading(true);

    providerSvc.FetchDurationClaimedRoundingEnquiryResult(
	    currentPage,
	    listFilterReference,
	    listFilterDescription,
	    listFilterExternalAccount,
	    selectedId,
	    FetchDCRItems_Callback);
}
function FetchDCRItems_Callback(response) {
    var index, dcrItems, str;

    btnView.disabled = true;
    btnCopy.disabled = true;

    if (CheckAjaxResponse(response, providerSvc.url)) {
        dcrItems = response.value.DcrItems;
        ClearTable(tblDCR);
        for (index = 0; index < dcrItems.length; index++) {
            populating = true;
            tr = AddRow(tblDCR);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "DCRSelect", dcrItems[index].dcrID, RadioButton_Click);
            AddCell(tr, dcrItems[index].Reference);
            AddCell(tr, dcrItems[index].Description);
            AddCell(tr, dcrItems[index].ExternalAccount);

            if (qs_DcrID == dcrItems[index].dcrID || (currentPage == 1 && dcrItems.length == 1)) {
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
    var x
    var cell, Radio, input;
    for (x = 0; x < tblDCR.tBodies[0].rows.length; x++) {
        cell = tblDCR.tBodies[0].rows[x].childNodes[0];
        Radio = cell.getElementsByTagName("INPUT")[0];
        input = cell.getElementsByTagName("INPUT")[1];
        if (Radio.checked) {
            tblDCR.tBodies[0].rows[x].className = "highlightedRow"

            qs_DcrID = Radio.value;
            qs_Reference = tblDCR.tBodies[0].rows[x].cells[1].innerHTML;
            qs_DcrDesc = tblDCR.tBodies[0].rows[x].cells[2].innerHTML;
            qs_DcrEcternalAccount = tblDCR.tBodies[0].rows[x].cells[3].innerHTML;
            
            btnCopy.disabled = false;
            btnView.disabled = false;

        } else {
            tblDCR.tBodies[0].rows[x].className = ""
        }
    }
    if (!populating) ReplaceUrl();
}

function ReplaceUrl() {
    var url = document.location.href;
    url = AddQSParam(RemoveQSParam(url, "invoiceID"), "invoiceID", selectedInvoiceID);
    url = AddQSParam(RemoveQSParam(url, "estabID"), "estabID", selectedProviderID);
    url = AddQSParam(RemoveQSParam(url, "suRef"), "suRef", listFilterSUReference);
    url = AddQSParam(RemoveQSParam(url, "suName"), "suName", listFilterSUName);
    document.location.replace(url);
}

function btnNew_Click() {
    var url = document.location.href;
    document.location.href = "DurationClaimedRoundingRules.aspx?mode=2&backurl=" + GetBackUrl(true);
}

function btnView_Click() {
    var url = document.location.href;
    document.location.href = "DurationClaimedRoundingRules.aspx?mode=1&id=" + qs_DcrID + "&backurl=" + GetBackUrl(false);
}

function btnCopy_Click() {
    var url = document.location.href;
    document.location.href = "DurationClaimedRoundingRules.aspx?mode=3&cid=" + qs_DcrID + "&backurl=" + GetBackUrl(true);
}

function GetBackUrl(stripId) {

    var url = document.location.href;

    if (stripId) {
        qs_DcrID = 0;
    }
    
    url = AddQSParam(RemoveQSParam(url, "id"), "id", qs_DcrID);

    return escape(url);

}

addEvent(window, "load", Init);
