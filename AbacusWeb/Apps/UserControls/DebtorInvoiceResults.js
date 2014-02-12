
var DebtorInvoiceResults_NULL_DATE_TO = "9999-12-31";
var DebtorInvoiceResults_INVTYPE_RES = "RES", DebtorInvoiceResults_INVTYPE_DOM = "DOM", DebtorInvoiceResults_INVTYPE_LD = "LD";
var DebtorInvoiceResults_INVTYPE_STD = "STD", DebtorInvoiceResults_INVTYPE_MANUAL = "MAN", DebtorInvoiceResults_INVTYPE_SDS = "SDS";

var DebtorInvoiceResults_contractSvc, DebtorInvoiceResults_currentPage, DebtorInvoiceResults_invoiceID, DebtorInvoiceResults_clientID, DebtorInvoiceResults_invoices;
var DebtorInvoiceResults_invRes, DebtorInvoiceResults_invDom, DebtorInvoiceResults_invLD, DebtorInvoiceResults_invStd, DebtorInvoiceResults_invManual, DebtorInvoiceResults_invSDS;
var DebtorInvoiceResults_invClient, DebtorInvoiceResults_invTP, DebtorInvoiceResults_invProp, DebtorInvoiceResults_invOLA, DebtorInvoiceResults_invPenColl, DebtorInvoiceResults_invHomeColl;
var DebtorInvoiceResults_invDateFrom, DebtorInvoiceResults_invDateTo;
var DebtorInvoiceResults_invActual, DebtorInvoiceResults_invProvisional, DebtorInvoiceResults_invRetracted, DebtorInvoiceResults_invViaRetract;
var DebtorInvoiceResults_invZeroValue, DebtorInvoiceResults_invExclude, DebtorInvoiceResults_invBatchSel;
var DomProviderInvoiceStep_required;
var DebtorInvoiceResults_tblInvoices, DebtorInvoiceResults_divPagingLinks, DebtorInvoiceResults_btnPrint, DebtorInvoiceResults_btnExcInc, DebtorInvoiceResults_btnCreateBatch, DebtorInvoiceResults_btnView;
var DebtorInvoiceResults_listFilter, DebtorInvoiceResults_listDebtorFilter = "", DebtorInvoiceResults_listInvoiceNumFilter = "";
var DebtorInvoiceResults_listClientRefFilter = "", DebtorInvoiceResults_listCommentFilter = "";
var DebtorInvoiceResults_btnViewID;
var DebtorInvoiceResults_viewCreateSDSV2InvoiceJobInNewWindow, DebtorInvoiceResults_CreateSDSV2InvoiceJobClientID;
var ReportViewerURL, DocumentDownloadHandler, divDownloadContainer_ID;

DocumentDownloadHandler = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Documents/DocumentDownloadHandler.axd?saveas=1";

function Init() {
    DebtorInvoiceResults_contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
    DebtorInvoiceResults_tblInvoices = GetElement("DebtorInvoiceResults_tblInvoices");
    DebtorInvoiceResults_divPagingLinks = GetElement("DebtorInvoice_PagingLinks");
    DebtorInvoiceResults_btnPrint = GetElement("DebtorInvoiceResults_btnPrint", true);
    DebtorInvoiceResults_btnExcInc = GetElement("DebtorInvoiceResults_btnExcInc", true);
    DebtorInvoiceResults_btnCreateBatch = GetElement("DebtorInvoiceResults_btnCreateBatch", true);
    DebtorInvoiceResults_btnView = GetElement(DebtorInvoiceResults_btnViewID + '_btnReports', true);
    ReportViewerURL = ReportsButton_GetMenuUrlByIndex(DebtorInvoiceResults_btnView.id, 0);

    var reportBtn = jQuery("#" + DebtorInvoiceResults_btnView.id);
    reportBtn.unbind('MenuItemClicked');
    reportBtn.bind('MenuItemClicked', DebtorInvoiceResults_ViewFromReportButton);

    if (DebtorInvoiceResults_btnExcInc) DebtorInvoiceResults_btnExcInc.disabled = true;
    if (DebtorInvoiceResults_btnView) DebtorInvoiceResults_btnView.disabled = true;

    // setup list filters
    DebtorInvoiceResults_listFilter = new Target.Web.ListFilter(DebtorInvoiceResults_ListDebtorInvoice_Callback);
    DebtorInvoiceResults_listFilter.AddColumn("Debtor", GetElement("thDebtor"));
    DebtorInvoiceResults_listFilter.AddColumn("Ref.", GetElement("thClientRef"));
    DebtorInvoiceResults_listFilter.AddColumn("Comment", GetElement("thComment"));
    DebtorInvoiceResults_listFilter.AddColumn("Inv No.", GetElement("thInvNum"));
    DebtorInvoiceResults_currentPage = 1;
    DebtorInvoiceResults_FetchDebtorInvoiceList(DebtorInvoiceResults_currentPage, DebtorInvoiceResults_invoiceID);
}

function DebtorInvoiceResults_ListDebtorInvoice_Callback(column) {
    switch (column.Name) {
        case "Debtor":
            DebtorInvoiceResults_listDebtorFilter = column.Filter;
            break;
        case "Ref.":
            DebtorInvoiceResults_listClientRefFilter = column.Filter;
            break;
        case "Comment":
            DebtorInvoiceResults_listCommentFilter = column.Filter;
            break;
        case "Inv No.":
            DebtorInvoiceResults_listInvoiceNumFilter = column.Filter;
            break;
        default:
            alert("Invalid column filter specified.");
            break;
    }

    DebtorInvoiceResults_FetchDebtorInvoiceList(1);
}

function DebtorInvoiceResults_FetchDebtorInvoiceList(page, selectedInvoiceID) {
    DebtorInvoiceResults_currentPage = page;

    DisplayLoading(true);

    if (selectedInvoiceID == undefined) selectedInvoiceID = "0";
    if (DebtorInvoiceResults_listDebtorFilter == undefined || DebtorInvoiceResults_listDebtorFilter == "") DebtorInvoiceResults_listDebtorFilter = "null";
    if (DebtorInvoiceResults_listInvoiceNumFilter == undefined || DebtorInvoiceResults_listInvoiceNumFilter == "") DebtorInvoiceResults_listInvoiceNumFilter = "null";
    if (DebtorInvoiceResults_listCommentFilter == undefined || DebtorInvoiceResults_listCommentFilter == "") DebtorInvoiceResults_listCommentFilter = "null";
    if (DebtorInvoiceResults_listClientRefFilter == undefined || DebtorInvoiceResults_listClientRefFilter == "") DebtorInvoiceResults_listClientRefFilter = "null";

    DebtorInvoiceResults_contractSvc.FetchDebtorInvoiceList(page, selectedInvoiceID, DebtorInvoiceResults_clientID,
	    DebtorInvoiceResults_invRes, DebtorInvoiceResults_invDom, DebtorInvoiceResults_invLD, DebtorInvoiceResults_invClient, DebtorInvoiceResults_invTP, DebtorInvoiceResults_invProp,
        DebtorInvoiceResults_invOLA, DebtorInvoiceResults_invPenColl, DebtorInvoiceResults_invHomeColl, DebtorInvoiceResults_invStd, DebtorInvoiceResults_invManual, DebtorInvoiceResults_invSDS,
        DebtorInvoiceResults_invActual, DebtorInvoiceResults_invProvisional, DebtorInvoiceResults_invRetracted, DebtorInvoiceResults_invViaRetract, DebtorInvoiceResults_invZeroValue,
        DebtorInvoiceResults_invDateFrom, DebtorInvoiceResults_invDateTo, DebtorInvoiceResults_invBatchSel, DebtorInvoiceResults_invExclude,
        DebtorInvoiceResults_listDebtorFilter, DebtorInvoiceResults_listInvoiceNumFilter, DebtorInvoiceResults_listClientRefFilter,
        DebtorInvoiceResults_listCommentFilter, "0",
        DebtorInvoiceResults_FetchDebtorInvoiceList_Callback);
}

function DebtorInvoiceResults_FetchDebtorInvoiceList_Callback(response) {
    var index, invTypes;
    var tr, td, radioButton;
    var str;
    var link;
    var documentID;

    if (DebtorInvoiceResults_btnExcInc) {
        DebtorInvoiceResults_btnExcInc.disabled = true;
        DebtorInvoiceResults_btnExcInc.value = "Exclude";
    }
    if (DebtorInvoiceResults_btnCreateBatch) DebtorInvoiceResults_btnCreateBatch.disabled = false;

    if (DebtorInvoiceResults_btnView) DebtorInvoiceResults_btnView.disabled = true;

    if (CheckAjaxResponse(response, DebtorInvoiceResults_contractSvc.url)) {

        // populate the table
        DebtorInvoiceResults_invoices = response.value.Invoices;

        // remove existing rows
        ClearTable(DebtorInvoiceResults_tblInvoices);
        for (index = 0; index < DebtorInvoiceResults_invoices.length; index++) {

            tr = AddRow(DebtorInvoiceResults_tblInvoices);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "DebtorInvoiceSelect", DebtorInvoiceResults_invoices[index].ID, DebtorInvoiceResults_RadioButton_Click);
            AddInput(td, "", "hidden", "", "", DebtorInvoiceResults_invoices[index].ID, null, null);

            td = AddCell(tr, DebtorInvoiceResults_invoices[index].ClientName);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";
            td = AddCell(tr, DebtorInvoiceResults_invoices[index].ClientRef);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";
            if ((DebtorInvoiceResults_invoices[index].IsTPInvoice == "Yes" || DebtorInvoiceResults_invoices[index].ThirdPartyID != 0) && DebtorInvoiceResults_invoices[index].ThirdPartyName != "") {
                td = AddCell(tr, "c/o " + DebtorInvoiceResults_invoices[index].ThirdPartyName);
            } else if (DebtorInvoiceResults_invoices[index].IsPropertyInvoice == "Yes") {
                td = AddCell(tr, "PROPERTY");
            } else if (DebtorInvoiceResults_invoices[index].IsOLAInvoice == "Yes") {
                td = AddCell(tr, "OLA");
            } else if (DebtorInvoiceResults_invoices[index].IsPenCollectInvoice == "Yes") {
                td = AddCell(tr, "PENSION");
            } else if (DebtorInvoiceResults_invoices[index].IsHomeCollectInvoice == "Yes") {
                td = AddCell(tr, "HOME COLLECT");
            } else {
                td = AddCell(tr, " ");
            }
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, "");
            documentID = DebtorInvoiceResults_invoices[index].DocumentID;
            link = AddLink(td, DebtorInvoiceResults_invoices[index].InvoiceNumber, DebtorInvoiceResults_ViewFromLink(DebtorInvoiceResults_invoices[index].ID, documentID), "Click here to view this invoice.");
            link.className = "transBg";
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            // Create a key to highlight all the invoice types met by the current invoice:
            invTypes = "";
            if (DebtorInvoiceResults_invoices[index].IsSDSInvoice == "Yes") {
                invTypes = invTypes + ", " + DebtorInvoiceResults_INVTYPE_SDS;
            } else {
                if (DebtorInvoiceResults_invoices[index].IsResidentialInvoice == "Yes") invTypes = invTypes + ", " + DebtorInvoiceResults_INVTYPE_RES;
                if (DebtorInvoiceResults_invoices[index].IsDomiciliaryInvoice == "Yes") invTypes = invTypes + ", " + DebtorInvoiceResults_INVTYPE_DOM;
            }
            if (DebtorInvoiceResults_invoices[index].IsLearnDisabInvoice == "Yes") invTypes = invTypes + ", " + DebtorInvoiceResults_INVTYPE_LD;
            if (DebtorInvoiceResults_invoices[index].IsStandardInvoice == "Yes") invTypes = invTypes + ", " + DebtorInvoiceResults_INVTYPE_STD;
            if (DebtorInvoiceResults_invoices[index].IsManualInvoice == "Yes") invTypes = invTypes + ", " + DebtorInvoiceResults_INVTYPE_MANUAL;
            if (invTypes != "") invTypes = invTypes.substring(2);
            td = AddCell(tr, invTypes);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, DebtorInvoiceResults_invoices[index].InvoiceTotal);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, DebtorInvoiceResults_invoices[index].DateCreated.strftime("%d/%m/%Y"));
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            if (DebtorInvoiceResults_invoices[index].IsRetractedInvoice == "Yes") {
                td = AddCell(tr, "Retracted");
            } else if (DebtorInvoiceResults_invoices[index].IsViaRetraction == "Yes") {
                td = AddCell(tr, "Retraction");
            } else if (DebtorInvoiceResults_invoices[index].IsProvisionalInvoice == "Yes") {
                td = AddCell(tr, "Provisional");
            } else {
                td = AddCell(tr, "Actual");
            }
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            if (DebtorInvoiceResults_invoices[index].BatchID > 0) {
                td = AddCell(tr, "Yes");
            } else {
                td = AddCell(tr, "No");
            }
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            td = AddCell(tr, DebtorInvoiceResults_invoices[index].ExcludeFromBatch);
            td.style.textOverflow = "ellipsis";
            td.style.overflow = "hidden";

            if (DebtorInvoiceResults_invoices[index].ID == DebtorInvoiceResults_invoiceID || ( DebtorInvoiceResults_currentPage == 1 && DebtorInvoiceResults_invoices.length == 1))
                radioButton.click();

        }
        // load the paging link HTML
        DebtorInvoiceResults_divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}
function DebtorInvoiceResults_RadioButton_Click() {
    var index, rdo, hid, documentID;

    DebtorInvoiceResults_invoiceID = "0";
    for (index = 0; index < DebtorInvoiceResults_tblInvoices.tBodies[0].rows.length; index++) {
        rdo = DebtorInvoiceResults_tblInvoices.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
        if (rdo.checked) {
            DebtorInvoiceResults_invoiceID = rdo.value;
            documentID = DebtorInvoiceResults_invoices[index].DocumentID;
            DebtorInvoiceResults_CreateSDSV2InvoiceJobClientID = DebtorInvoiceResults_invoices[index].ClientID;

            ReportsButton_AddParam(DebtorInvoiceResults_btnView.id, "invoiceid", rdo.value);
            ReportsButton_AddParam(DebtorInvoiceResults_btnView.id, "documentid", documentID);
            
            DebtorInvoiceResults_btnView.disabled = false;

            if (DebtorInvoiceResults_btnExcInc && (DebtorInvoiceResults_invoices[index].BatchID == 0)) {
                DebtorInvoiceResults_btnExcInc.disabled = false;
                if (DebtorInvoiceResults_invoices[index].ExcludeFromBatch == "Yes") {
                    DebtorInvoiceResults_btnExcInc.value = "Include";
                } else {
                    DebtorInvoiceResults_btnExcInc.value = "Exclude";
                }
            } else if (DebtorInvoiceResults_btnExcInc) {
                DebtorInvoiceResults_btnExcInc.disabled = true;
            }
            DebtorInvoiceResults_tblInvoices.tBodies[0].rows[index].className = "highlightedRow"
        } else {
            DebtorInvoiceResults_tblInvoices.tBodies[0].rows[index].className = ""
        }
    }
}

function DebtorInvoiceResults_btnPrint_Click() {
    var url = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/General/DebtorInvoices/PrintInvoice.aspx?clientID=" + DebtorInvoiceResults_clientID + "&invRes=" + DebtorInvoiceResults_invRes + "&invDom=" + DebtorInvoiceResults_invDom +
        "&invLD=" + DebtorInvoiceResults_invLD + "&invStd=" + DebtorInvoiceResults_invStd + "&invManual=" + DebtorInvoiceResults_invManual +
        "&invSDS=" + DebtorInvoiceResults_invSDS + "&invClient=" + DebtorInvoiceResults_invClient + "&invTP=" + DebtorInvoiceResults_invTP +
        "&invProp=" + DebtorInvoiceResults_invProp + "&invOLA=" + DebtorInvoiceResults_invOLA + "&invPenColl=" + DebtorInvoiceResults_invPenColl +
        "&invHomeColl=" + DebtorInvoiceResults_invHomeColl + "&invDateFrom=" + DebtorInvoiceResults_invDateFrom + "&invExclude=" + DebtorInvoiceResults_invExclude +
        "&invDateTo=" + DebtorInvoiceResults_invDateTo + "&invActual=" + DebtorInvoiceResults_invActual + "&invProvisional=" + DebtorInvoiceResults_invProvisional +
        "&invRetracted=" + DebtorInvoiceResults_invRetracted + "&invViaRetract=" + DebtorInvoiceResults_invViaRetract + "&invZeroValue=" + DebtorInvoiceResults_invZeroValue +
        "&invBatchSel=" + DebtorInvoiceResults_invBatchSel + "&filterDebtor=" + DebtorInvoiceResults_listDebtorFilter + "&filterInvNum=" + DebtorInvoiceResults_listInvoiceNumFilter +
        "&filterClientRef=" + DebtorInvoiceResults_listClientRefFilter + "&filterComment=" + DebtorInvoiceResults_listCommentFilter;
    OpenPopup(url, 75, 50, 1);
}

function DebtorInvoiceResults_btnExcInc_Click() {
    // Toggle the Exclude/Include value for the current invoice's Exclude setting
    // (based on the Invoice.ExcludeFromDebtors field)
    var response;
    var toggle;
    if (DebtorInvoiceResults_btnExcInc.value == "Include") {
        response = DebtorInvoiceResults_contractSvc.DisplayDebtorInvoiceIncludeWarningMessage(DebtorInvoiceResults_invoiceID)
        if (response.value) {
            toggle = window.confirm('This Invoice was automatically excluded from Debtors Interfaces by Abacus ' +
                                'because it has a net charge of zero. Are you sure you wish to reverse this decision and ' +
                                'make this Invoice available for collection by a subsequent Debtors Interface run?');
                             
        } else {
            toggle = true;
        }

    } else {
        toggle = true;
    }
    
    if (toggle==true){
        DebtorInvoiceResults_contractSvc.DebtorInvoiceToggleExclude(DebtorInvoiceResults_invoiceID, DebtorInvoiceResults_ExcInc_Callback);
    }
}

function DebtorInvoiceResults_ExcInc_Callback(response) {
    DebtorInvoiceResults_FetchDebtorInvoiceList(DebtorInvoiceResults_currentPage);
}

function DebtorInvoiceResults_btnCreateBatch_Click() {
    var message;

    if (DebtorInvoiceResults_invBatchSel == 1) {
        alert("There must be unbatched invoices available for a batch to be created.");
        return;
    } else {
        message = "Invoices matching any of the following criteria will not be" +
            "\npassed onto the batch creation process:\n\n" +
            "  - Provisional invoices\n" +
            "  - Invoices already linked to a batch\n" +
            "  - Invoices marked as 'excluded from debtors'."
        alert(message);
    }
    var url = "CreateBatch.aspx?invoiceID=" + DebtorInvoiceResults_invoiceID + "&clientID=" + DebtorInvoiceResults_clientID +
        "&invRes=" + DebtorInvoiceResults_invRes + "&invDom=" + DebtorInvoiceResults_invDom +
        "&invLD=" + DebtorInvoiceResults_invLD + "&invStd=" + DebtorInvoiceResults_invStd + "&invManual=" + DebtorInvoiceResults_invManual +
        "&invSDS=" + DebtorInvoiceResults_invSDS + "&invClient=" + DebtorInvoiceResults_invClient + "&invTP=" + DebtorInvoiceResults_invTP +
        "&invProp=" + DebtorInvoiceResults_invProp + "&invOLA=" + DebtorInvoiceResults_invOLA + "&invPenColl=" + DebtorInvoiceResults_invPenColl +
        "&invHomeColl=" + DebtorInvoiceResults_invHomeColl + "&invDateFrom=" + DebtorInvoiceResults_invDateFrom +
        "&invDateTo=" + DebtorInvoiceResults_invDateTo + "&invActual=" + DebtorInvoiceResults_invActual + "&invProvisional=false" +
        "&invRetracted=" + DebtorInvoiceResults_invRetracted + "&invViaRetract=" + DebtorInvoiceResults_invViaRetract +
        "&invZeroValue=" + DebtorInvoiceResults_invZeroValue + "&invBatchSel=2&invExclude=false" +
        "&filterDebtor=" + DebtorInvoiceResults_listDebtorFilter + "&filterInvNum=" + DebtorInvoiceResults_listInvoiceNumFilter +
        "&filterClientRef=" + DebtorInvoiceResults_listInvoiceNumFilter + "&filterComment=" + DebtorInvoiceResults_listCommentFilter;

    document.location.href = url + "&backUrl=" + DebtorInvoiceResults_GetBackUrl();
}

function DebtorInvoiceResults_GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function DebtorInvoiceResults_BeforeNavigate() {
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
    url = AddQSParam(RemoveQSParam(url, "invoiceID"), "invoiceID", DebtorInvoiceResults_invoiceID);
    SelectorWizard_newUrl = url;
    return true;
}

function DebtorInvoiceResults_ViewFromReportButton(src, menuItem) {
    var invoiceID = GetQSParam(menuItem.url, "invoiceid");
    var documentID = GetQSParam(menuItem.url, "documentid");
    var url = DebtorInvoiceResults_ViewFromLink(invoiceID, documentID);
    if (url.indexOf("javascript:", 0) >= 0) {
        eval(url.replace("javascript:", ""));
    } else {
        document.location.href = url;
    }
}

function DebtorInvoiceResults_ViewFromLink(invoiceID, documentID) {
    var url;

    if (documentID > 0) {
        url = DocumentDownloadHandler + "&id=" + documentID;
    }
    else {
        url = "javascript:OpenReport('" + ReportViewerURL + "&invoiceid=" + invoiceID + "', '" + divDownloadContainer_ID + "')";
    }

    return url;
}

function DebtorInvoiceResults_btnNew_Click() {
    var SDS_INVOICING_V2_JOB_TYPE_ID = 35;
    var url = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Jobs/CreateNew.aspx?jobTypeID=" +
              SDS_INVOICING_V2_JOB_TYPE_ID + "&clientID=" + DebtorInvoiceResults_clientID;

    if (DebtorInvoiceResults_viewCreateSDSV2InvoiceJobInNewWindow)
        OpenPopup(url + "&autopopup=1", 75, 50, 1);
    else
        document.location.href = url + "&backUrl=" + DebtorInvoiceResults_GetBackUrl();
}

addEvent(window, "load", Init);
