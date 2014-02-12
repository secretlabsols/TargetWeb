var contractSvc, btnCreate, lblInvCount, lblInvTotalValue, lblInvTotalVAT;
var rdoCreateNow, rdoDefer;
var dteStartDate, dteStartDateButton, tmeStartDateHours, tmeStartDateMinutes, tmeStartDateSeconds, lblStartDate;
var CreateBatch_dteStartDateID, CreateBatch_tmeStartDateID;
var CreateBatch_dtePostingDateID, CreateBatch_cboPostingYearID;
var CreateBatch_cboPeriodNumID, CreateBatch_chkRollbackID;
var batchID, invoiceID, providerID;
var contractID, clientID, weFrom, weTo;
var invoiceNumber, invoiceRef, statusDateFrom, statusDateTo;
var statusIsUnpaid, statusIsAuthorised, statusIsPaid, statusIsSuspended;
var exclude;
var listInvoiceNum = "", listInvoiceRef = "";
var invNumFilter = "", invRefFilter = "";

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	btnCreate = GetElement("btnCreate");
	lblInvCount = GetElement("lblInvCount");
	lblInvTotalValue = GetElement("lblInvTotalValue");
	lblInvTotalVAT = GetElement("lblInvTotalVAT");
	rdoCreateNow = GetElement("optCreateNow");
	rdoDefer = GetElement("optDefer");
	dteStartDate = GetElement(CreateBatch_dteStartDateID + "_txtTextBox");
	lblStartDate = GetElement(CreateBatch_dteStartDateID + "_lblLabel");
	dteStartDateButton = GetElement(CreateBatch_dteStartDateID + "_btnDatePicker");
	tmeStartDateHours = GetElement(CreateBatch_tmeStartDateID + "_cboHours");
	tmeStartDateMinutes = GetElement(CreateBatch_tmeStartDateID + "_cboMinutes");
	
    GetInitialURLParams();
	FetchDomProviderInvoiceList(0);
}


function FetchDomProviderInvoiceList(page) {
	DisplayLoading(true);

    PrepareValuesForVBCall();
	    
    contractSvc.FetchDomProviderInvoiceListCount(page, invoiceID, 
	    batchID, providerID, "",
	    contractID, "", clientID, "", 
	    weFrom, weTo, listInvoiceNum, listInvoiceRef, 
	    statusDateFrom, statusDateTo, true, false, 
	    false, false, exclude, invNumFilter, invRefFilter, UpdateBatchInfo_Callback)
}

function UpdateBatchInfo_Callback(response) {
	var invoices, index;

	if(CheckAjaxResponse(response, contractSvc.url)) {
		
		// populate the table
		invoices = response.value.Invoices;

		if(invoices.length == 0) 
		{
		    btnCreate.disabled = true;

		    lblInvCount.innerHTML = "0";
		    lblInvTotalValue.innerHTML = '&pound;' + "0.00";
		    lblInvTotalVAT.innerHTML = '&pound;' + "0.00";
		}
		else
		{
		    btnCreate.disabled = false;

		    for(index=0; index<1; index++) {
			    lblInvCount.innerHTML = invoices[index].InvoiceCount;
			    lblInvTotalValue.innerHTML = invoices[index].InvoiceTotalNet;
			    lblInvTotalVAT.innerHTML = invoices[index].InvoiceTotalVAT;
		    }
		}
	}
	DisplayLoading(false);
}

function CreateBatch_Callback(response) {
	var interfaceLog, url;

	DisplayLoading(false);

	if(CheckAjaxResponse(response, contractSvc.url)) {
		interfaceLog = response.value.InterfaceID;

		if(interfaceLog > -1) 
		{
		    alert("Invoice batch created successfully.");
            url = "ListBatch.aspx?currentPage=0&id=" + interfaceLog;
            document.location.href = url;
		}
		else
		{
		    alert("Invoice batch NOT created:\n" + response.value.ErrMsg.ErrorMessage);
		}
	}
}

function btnRefresh_Click() {
	FetchDomProviderInvoiceList(0);
}

function btnBack_Click() {
    GetInitialURLParams();

    var url = "List.aspx?currentStep=4&invID=" + invoiceID + "&batchID=" + batchID + 
        "&estabID=" + providerID + "&contractID=" + contractID + 
        "&clientID=" + clientID +
        "&weFrom=" + weFrom + "&weTo=" + weTo + 
        "&invoiceNumber=" + listInvoiceNum + "&invoiceRef=" + listInvoiceRef + 
        "&invoiceNumberFilter=" + invNumFilter + "&invoiceRefFilter=" + invRefFilter + 
        "&dateFrom=" + statusDateFrom + "&dateTo=" + statusDateTo + 
        "&unPaid=" + statusIsUnpaid + "&authorised=" + statusIsAuthorised + 
        "&paid=" + statusIsPaid + "&suspended=" + statusIsSuspended + "&exclude=" + exclude;
        
    document.location.href = url;
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

function GetInitialURLParams() {
	providerID = GetQSParam(document.location.search, "providerID");
	contractID = GetQSParam(document.location.search, "contractID");
	clientID = GetQSParam(document.location.search, "clientID");
	weFrom = GetQSParam(document.location.search, "weFrom");
	weTo = GetQSParam(document.location.search, "weTo");
	listInvoiceNum = GetQSParam(document.location.search, "invoiceNumber");
	listInvoiceRef = GetQSParam(document.location.search, "invoiceRef");
	invNumFilter = GetQSParam(document.location.search, "invoiceNumberFilter");
	invRefFilter = GetQSParam(document.location.search, "invoiceRefFilter");
	statusDateFrom = GetQSParam(document.location.search, "statusDateFrom");
	statusDateTo = GetQSParam(document.location.search, "statusDateTo");
	statusIsAuthorised = GetQSParam(document.location.search, "statusIsAuthorised");
	statusIsPaid = GetQSParam(document.location.search, "statusIsPaid");
	statusIsSuspended = GetQSParam(document.location.search, "statusIsSuspended");
	statusIsUnpaid = GetQSParam(document.location.search, "statusIsUnpaid");
	exclude = GetQSParam(document.location.search, "exclude");
}

function PrepareValuesForVBCall() {
	if(providerID == undefined || providerID == "null") providerID = 0;
	if(contractID == undefined || contractID == "null") contractID = 0;
	if(clientID == undefined || clientID == "null") clientID = 0;
	if(weFrom == "" || weFrom == undefined || weFrom == "null") weFrom=null;
	if(weTo == "" || weTo == undefined || weTo == "null") weTo=null;
	if(listInvoiceNum == "" || listInvoiceNum == undefined) listInvoiceNum="null";
	if(listInvoiceRef== "" || listInvoiceRef == undefined) listInvoiceRef="null";
	if(statusDateFrom == "" || statusDateFrom == undefined || statusDateFrom == "null") statusDateFrom=null;
	if(statusDateTo == "" || statusDateTo == undefined || statusDateTo == "null") statusDateTo=null;
	if(invNumFilter == "" || invNumFilter == undefined) invNumFilter="null";
	if(invRefFilter== "" || invRefFilter == undefined) invRefFilter="null";
	if(exclude == undefined || exclude == "null") exclude = "null";
}

function optCreation_Click() {
    var thedate = new Date( );
    if (rdoCreateNow.checked == true) {
        lblStartDate.style.color='lightgrey';
        dteStartDate.disabled = true;
        dteStartDate.value = "";
	    dteStartDateButton.disabled = true; 
	    tmeStartDateHours.disabled = true;
	    tmeStartDateHours.value = "00";
	    tmeStartDateMinutes.disabled = true;
	    tmeStartDateMinutes.value = "00";
	    
    } else if (rdoDefer.checked == true) {
        lblStartDate.style.color='black';
        dteStartDate.disabled = false;
	    dteStartDateButton.disabled = false; 
	    dteStartDate.value = lPadStr(thedate.getDate(), 2, '0') + "/" + lPadStr((thedate.getMonth()+ 1), 2, '0') + "/" + thedate.getFullYear();
	    tmeStartDateHours.disabled = false;
	    tmeStartDateHours.value = PadDigits(thedate.getHours(), 2);
	    tmeStartDateMinutes.disabled = false;
	    tmeStartDateMinutes.value = PadDigits(thedate.getMinutes(), 2);
    }
}

function PadDigits(n, totalDigits) 
    { 
        n = n.toString(); 
        var pd = ''; 
        if (totalDigits > n.length) 
        { 
            for (i=0; i < (totalDigits-n.length); i++) 
            { 
                pd += '0'; 
            } 
        } 
        return pd + n.toString(); 
    } 


addEvent(window, "load", Init);
