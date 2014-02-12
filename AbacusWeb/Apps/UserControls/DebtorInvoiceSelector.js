
var lookupSvc, currentPage;
var DebtorInvoiceSelector_selectedDebtorInvoiceID;
var tblDebtorInvoices, divPagingLinks;
var listFilter, listFilterReference = "", listFilterDebtor = "", listFilterInvNo = "";

function Init() {
    lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
    tblDebtorInvoices = GetElement("tblDebtorInvoices");
    divPagingLinks = GetElement("DebtorInvoice_PagingLinks");

    // setup list filters
    listFilter = new Target.Web.ListFilter(ListFilter_Callback);
    listFilter.AddColumn("Debtor", GetElement("thDebtor"));
    listFilter.AddColumn("Ref", GetElement("thRef"));
    listFilter.AddColumn("Inv No", GetElement("thInvNo"));

    // populate table
    FetchDebtorInvoiceList(currentPage, DebtorInvoiceSelector_selectedDebtorInvoiceID);
}

function ListFilter_Callback(column) {
    switch (column.Name) {
        case "Debtor":
            listFilterDebtor = column.Filter;
            break;
        case "Ref":
            listFilterReference = column.Filter;
            break;
        case "Inv No":
            listFilterInvNo = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }
    FetchDebtorInvoiceList(1, 0);
}

function FetchDebtorInvoiceList(page, selectedID) {
    currentPage = page;
    DisplayLoading(true);
    if (selectedID == undefined) selectedID = 0;

    lookupSvc.FetchDebtorInvoiceListInternal(page, selectedID, listFilterDebtor, listFilterReference, listFilterInvNo, FetchDebtorInvoiceList_Callback);
}

function FetchDebtorInvoiceList_Callback(response) {
    var dis, index;
    var tr, td, radioButton;
    var str;
    var link;

    if (CheckAjaxResponse(response, lookupSvc.url)) {
        // populate the table
        dis = response.value.Invoices;

        // remove existing rows
        ClearTable(tblDebtorInvoices);
        for (index = 0; index < dis.length; index++) {

            tr = AddRow(tblDebtorInvoices);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "DebtorInvoiceSelect", dis[index].ID, RadioButton_Click);
           // str = dis[index].ThirdPartyName == '' ? dis[index].ClientName : dis[index].ThirdPartyName;
            str = dis[index].Debtor;
            if (str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = dis[index].ClientRef;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = dis[index].InvoiceNumber;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = getInvoiceType(dis[index]);
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = dis[index].InvoiceTotal;
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = Date.strftime("%d/%m/%Y", dis[index].DateCreated);
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = getInvoiceStatus(dis[index]);
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = dis[index].BatchID > 0 ? 'Yes' : 'No';
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            str = dis[index].ExcludeFromBatch == '1' ? 'Yes' : 'No';
            if (!str || str.length == 0) str = " ";
            td = AddCell(tr, str);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // select the sg?
            if (DebtorInvoiceSelector_selectedDebtorInvoiceID == dis[index].ID || (currentPage == 1 && dis.length == 1)) {
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
    for (index = 0; index < tblDebtorInvoices.tBodies[0].rows.length; index++) {
        rdo = tblDebtorInvoices.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            selectedRow = tblDebtorInvoices.tBodies[0].rows[index];
            tblDebtorInvoices.tBodies[0].rows[index].className = "highlightedRow"
            DebtorInvoiceSelector_selectedDebtorInvoiceID = rdo.value;
        } else {
            tblDebtorInvoices.tBodies[0].rows[index].className = ""
        }
    }
    if (typeof DebtorInvoiceSelector_SelectedItemChange == "function") {
        var args = new Array(3);
        args[0] = DebtorInvoiceSelector_selectedDebtorInvoiceID;
        args[1] = GetInnerText(selectedRow.cells[1]);
        args[2] = GetInnerText(selectedRow.cells[3]);
        DebtorInvoiceSelector_SelectedItemChange(args);
    }
}

function getInvoiceType(invoice) {

    var invType, srcType;

    if (invoice.IsSDSInvoice == 'Yes') {
        invType = 'SDS, ';
    }
    else if (invoice.IsDomiciliaryInvoice == 'No') {
        invType = 'RES';
        
        if (invoice.IsClientInvoice == 'Yes') {
            srcType =  ' (CLIENT), ';
        }
        if (invoice.IsTPInvoice == 'Yes') {
            srcType = ' (TP), ';
        }
        if (invoice.IsPropertyInvoice == 'Yes') {
            srcType =  ' (PROP), ';
        }
        if (invoice.IsOLAInvoice == 'Yes') {
            srcType = ' (OLA), ';
        }
        if (invoice.IsPenCollectInvoice == 'Yes') {
            srcType = ' (PEN), ';
        }
        if (invoice.IsHomeCollectInvoice == 'Yes') {
            srcType = ' (HOME), ';
        }
        if (srcType == undefined) {
            srcType = ', ';
        }

        invType = invType + srcType;
    }
    else if (invoice.IsDomiciliaryInvoice == 'Yes') {
        invType = 'DOM, ';
    }

    if (invoice.IsManualInvoice == 'Yes') {
        invType = invType + 'MAN';
    }
    else if (invoice.IsManualInvoice == 'No') {
        invType = invType + 'STD';
    }

    return invType;

}

function getInvoiceStatus(invoice) {
    var invStatus;

    if (invoice.IsRetractedInvoice == 'Yes') {
        invStatus = 'Retracted';
    }

    if (invoice.IsViaRetraction == 'Yes') {
        invStatus = 'Retraction';
    }

    if (invoice.IsProvisionalInvoice == 'Yes') {
        invStatus = 'Provisional';
    }

    if (invoice.IsActualInvoice == 'Yes') {
        invStatus = 'Actual';
    }

    return invStatus;

}

addEvent(window, "load", Init);
