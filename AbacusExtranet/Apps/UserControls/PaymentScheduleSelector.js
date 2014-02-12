 var currentPage ;
 var seletedPaymentScheduleID;
 var providerID;
 var contractID; 
 var reference;; 
 var periodFrom; 
 var periodTo; 
 var visitBasedYes; 
 var visitBasedNo; 
 var pfInvoiceNo; 
 var pfInvoiceAwait; 
 var pfInvoiceVerified; 
 var invUnpaid; 
 var invSuspend; 
 var invAuthorised; 
 var invPaid; 
 var varAwait; 
 var varVerified;
 var varDeclined;
 var serviceDeliveryFile;
 var serviceDeliveryFileId;
 var disableNew;
 var btnNewVisible;
 
var PaymentScheduleSvc, clientID, dateFrom, dateTo, userHasRetractCommand;
var paid, unPaid, authorised, suspended;
var invoiceNumber, weekendingDateFrom, weekendingDateTo;
var tblPSchedules, divPagingLinks, btnViewPrint, btnView, btnViewInvoiceLines, btnRetract;
var populating = false;
var listFilter, listFilterSUReference = "", listFilterSUName = "";

function Init() {
    PaymentScheduleSvc = new Target.Abacus.Extranet.Apps.WebSvc.PaymentSchedule_class();
    tblPSchedules = GetElement("tblPSchedules");
    divPagingLinks = GetElement("Invoice_PagingLinks");
    btnView = GetElement("btnView");
    btnView.disabled = true;
    btnNew = GetElement("btnNew");

    FetchPaymentScheduleList(currentPage, seletedPaymentScheduleID);

    if (!btnNewVisible) {
        btnNew.style.display = 'none';
    }
}

function ListFilter_Callback(column) {

    FetchPaymentScheduleList(1, 0);
}

function FetchPaymentScheduleList(page, selectedID) {

    if (currentPage == undefined) currentPage = 0;
    if (selectedID == undefined) selectedID = 0;

    DisplayLoading(true);

    if (serviceDeliveryFile == "true") {
        PaymentScheduleSvc.FetchServiceFilePaymentScheduleList(page, 
                                                        serviceDeliveryFileId, 
                                                        FetchPaymentScheduleList_Callback);
    }
    else {
        PaymentScheduleSvc.FetchPaymenstScheduleEnquiryResults(page,
                                selectedID,
                                providerID,
                                contractID,
                                reference,
                                periodFrom,
                                periodTo,
                                visitBasedYes,
                                visitBasedNo,
                                pfInvoiceNo,
                                pfInvoiceAwait,
                                pfInvoiceVerified,
                                invUnpaid,
                                invSuspend,
                                invAuthorised,
                                invPaid,
                                varAwait,
                                varVerified,
                                varDeclined,
                                FetchPaymentScheduleList_Callback);
    }
  
}

function FetchPaymentScheduleList_Callback(response) {
    var index, pSchedules, str, link;
    var retractVisible;

    btnView.disabled = true;
    
    if (CheckAjaxResponse(response, PaymentScheduleSvc.url)) {
        pSchedules = response.value.PaymentSchedules;
        ClearTable(tblPSchedules);
        for (index = 0; index < pSchedules.length; index++) {
            populating = true;
            //btnViewPrint.disabled = false;

            tr = AddRow(tblPSchedules);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "PaymentScheduleSelect", pSchedules[index].PaymentScheduleId, RadioButton_Click);
            AddInput(td, "PaymentScheduleId", "hidden", "", "", pSchedules[index].PaymentScheduleId, null, null);

            td = AddCell(tr, "");
            link = AddLink(td, pSchedules[index].Reference, "javascript:viewPaymentSchedule(" + pSchedules[index].PaymentScheduleId + ");", "View this Payment Schedule.");
            link.className = "transBg";

            AddCell(tr, pSchedules[index].Provider);
            AddCell(tr, pSchedules[index].Contract);

            AddCell(tr, Date.strftime("%d/%m/%Y", pSchedules[index].DateFrom));
            AddCell(tr, Date.strftime("%d/%m/%Y", pSchedules[index].DateTo));


            AddCell(tr, pSchedules[index].VisitBased);

            if (seletedPaymentScheduleID == pSchedules[index].PaymentScheduleId || (currentPage == 1 && pSchedules.length == 1)) {
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
    for (x = 0; x < tblPSchedules.tBodies[0].rows.length; x++) {
        cell = tblPSchedules.tBodies[0].rows[x].childNodes[0];
        Radio = cell.getElementsByTagName("INPUT")[0];
        input = cell.getElementsByTagName("INPUT")[1];
        if (Radio.checked) {
            tblPSchedules.tBodies[0].rows[x].className = "highlightedRow"
            seletedPaymentScheduleID =  Radio.value;
            btnView.disabled = false;
        } else {
            tblPSchedules.tBodies[0].rows[x].className = ""
        }
    }
}

function ReplaceUrl() {
    var url = document.location.href;
    url = AddQSParam(RemoveQSParam(url, "invoiceID"), "invoiceID", selectedInvoiceID);
    url = AddQSParam(RemoveQSParam(url, "estabID"), "estabID", selectedProviderID);
    url = AddQSParam(RemoveQSParam(url, "suRef"), "suRef", listFilterSUReference);
    url = AddQSParam(RemoveQSParam(url, "suName"), "suName", listFilterSUName);
    document.location.replace(url);
}

function viewPaymentSchedule(id) {
    seletedPaymentScheduleID = id;
    var url = SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/Dom/PaymentSchedules/PaymentSchedules.aspx?mode=1&id=" + id + "&backUrl=" + GetBackUrl();
    document.location.href = url;
}

function btnView_Click() {
    viewPaymentSchedule(seletedPaymentScheduleID);
}

function btnNew_Click() {
    var url = "PaymentSchedules.aspx?mode=2&estabid=" + providerID + "&contractid=" + contractID + "&backUrl=" + GetBackUrl();
    document.location.href = url;
}
  
function btnViewInvoiceLines_Click() {
    RemoveFilter();
    document.location.href = "ViewInvoiceLines.aspx?id=" + seletedPaymentScheduleID + "&estabID=" + seletedPaymentScheduleID;
}

function btnRetract_Click() {
    if (confirm("During the invoice retraction process a ‘verified’ visit amendment request with a ‘duration claimed’ value of zero will be created for each visit that is associated with the selected Provider Invoice; once these have been processed by the Council one additional Provider Invoice will be created being a contra to the invoice that you are retracting.Are you sure that you wish to retract this Provider Invoice?")) {
        FetchPaymentScheduleList(currentPage, seletedPaymentScheduleID, true);
    }

}

function RemoveFilter() {
    var url = document.location.href;
    url = RemoveQSParam(url, "suRef");
    url = RemoveQSParam(url, "suName");
    document.location.replace(url);
}

function GetBackUrl() {
    var backUrl = document.location.href;
    backUrl = AddQSParam(RemoveQSParam(backUrl, 'psid'), 'psid', seletedPaymentScheduleID);
    return escape(backUrl);
}

addEvent(window, "load", Init);
