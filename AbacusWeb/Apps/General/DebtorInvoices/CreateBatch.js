var NULL_DATE_FROM = "1900-01-01", NULL_DATE_TO = "9999-12-31";
var contractSvc, btnCreate, lblInvCount, lblInvTotalValue;
var rdoCreateNow, rdoDefer;
var dteStartDate, dteStartDateButton, tmeStartDateHours, tmeStartDateMinutes;
var dtePostingDate, dtePostingDateButton, cboPostingYear, cboPeriodNum;
var CreateBatch_dteStartDateID, CreateBatch_tmeStartDateID;
var CreateBatch_dtePostingDateID, CreateBatch_cboPostingYearID;
var CreateBatch_cboPeriodNumID;

var contractSvc, currentPage, invoiceID, clientID, invoices;
var invRes, invDom, invLD, invStd, invManual, invSDS;
var invClient, invTP, invProp, invOLA, invPenColl, invHomeColl;
var invDateFrom, invDateTo, invBatchSel, invExclude;
var invActual, invProvisional, invRetracted, invViaRetract, invZeroValue;
var listFilter, listInvoiceNumFilter = "", listDebtorFilter = "";
var listClientRefFilter = "", listCommentFilter = "";
var isBalanceRun, backURL = "";

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	
	btnCreate = GetElement("btnCreate");
	lblInvCount = GetElement("lblInvCount");
	lblInvTotalValue = GetElement("lblInvTotalValue");
	rdoCreateNow = GetElement("optCreateNow");
	rdoDefer = GetElement("optDefer");
	dteStartDate = GetElement(CreateBatch_dteStartDateID + "_txtTextBox");
	dteStartDateButton = GetElement(CreateBatch_dteStartDateID + "_btnDatePicker");
	tmeStartDateHours = GetElement(CreateBatch_tmeStartDateID + "_cboHours");
	tmeStartDateMinutes = GetElement(CreateBatch_tmeStartDateID + "_cboMinutes");
	dtePostingDate = GetElement(CreateBatch_dtePostingDateID + "_txtTextBox");
	dtePostingDateButton = GetElement(CreateBatch_dtePostingDateID + "_btnDatePicker");
	cboPostingYear = GetElement(CreateBatch_cboPostingYearID + "_cboDropDownList");
	cboPeriodNum = GetElement(CreateBatch_cboPeriodNumID + "_cboDropDownList");

    GetInitialURLParams();
	FetchDebtorInvoiceList(0);
}

function FetchDebtorInvoiceList(page) {
	DisplayLoading(true);

	contractSvc.FetchDebtorInvoiceListCount(page, invoiceID, clientID,
	    invRes, invDom, invLD, invClient, invTP, invProp,
        invOLA, invPenColl, invHomeColl, invStd, invManual, invSDS,
        invActual, invProvisional, invRetracted, invViaRetract, invZeroValue, 
        invDateFrom, invDateTo, invBatchSel, invExclude,
        listDebtorFilter, listInvoiceNumFilter, listClientRefFilter, listCommentFilter,
        "0", UpdateBatchInfo_Callback);
}

function UpdateBatchInfo_Callback(response) {
	var invoices, index;

	if (CheckAjaxResponse(response, contractSvc.url)) {
		
		// populate the table
		invoices = response.value.Invoices;

		if(invoices.length == 0) 
		{
		    btnCreate.disabled = true;

		    lblInvCount.innerHTML = "0";
		    lblInvTotalValue.innerHTML = '&pound;' + "0.00";
		}
		else
		{
		    btnCreate.disabled = false;

		    for(index=0; index<1; index++) {
			    lblInvCount.innerHTML = invoices[index].InvoiceCount;
			    if (isBalanceRun == "true")
			    {
			        lblInvTotalValue.innerHTML = invoices[index].InvoiceTotalUnpaid;
			    }
			    else
			    {
			        lblInvTotalValue.innerHTML = invoices[index].InvoiceTotalValue;
			    }
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
	FetchDebtorInvoiceList(0);
}

function btnBack_Click() {
    GetInitialURLParams();

    var url = backURL
//    "List.aspx?clientID=" + clientID + "&invRes=" + invRes + "&invDom=" + invDom +
//        "&invLD=" + invLD + "&invStd=" + invStd + "&invManual=" + invManual +
//        "&invSDS=" + invSDS + "&invClient=" + invClient + "&invTP=" + invTP +
//        "&invProp=" + invProp + "&invOLA=" + invOLA + "&invPenColl=" + invPenColl +
//        "&invHomeColl=" + invHomeColl + "&invDateFrom=" + invDateFrom +
//        "&invDateTo=" + invDateTo + "&invActual=" + invActual + "&invProvisional=" + invProvisional +
//        "&invRetracted=" + invRetracted + "&invViaRetract=" + invViaRetract + 
//        "&invZeroValue=" + invZeroValue + 
//        "&invBatchSel=" + invBatchSel + "&invExclude=" + invExclude + "&currentStep=3";
    
    document.location.href = url;
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

function GetInitialURLParams() {
	clientID = GetQSParam(document.location.search, "clientID");
	if (clientID == undefined) clientID = "0";
	invRes = GetQSParam(document.location.search, "invRes");
	if (invRes == undefined) invRes = "true";
	invDom = GetQSParam(document.location.search, "invDom");
	if (invDom == undefined) invDom = "true";
	invLD = GetQSParam(document.location.search, "invLD");
	if (invLD == undefined) invLD = "true";
	invClient = GetQSParam(document.location.search, "invClient");
	if (invClient == undefined) invClient = "true";
	invTP = GetQSParam(document.location.search, "invTP");
	if (invTP == undefined) invTP = "true";
	invProp = GetQSParam(document.location.search, "invProp");
	if (invProp == undefined) invProp = "true";
	invOLA = GetQSParam(document.location.search, "invOLA");
	if (invOLA == undefined) invOLA = "true";
	invPenColl = GetQSParam(document.location.search, "invPenColl");
	if (invPenColl == undefined) invPenColl = "true";
	invHomeColl = GetQSParam(document.location.search, "invHomeColl");
	if (invHomeColl == undefined) invHomeColl = "true";
	invStd = GetQSParam(document.location.search, "invStd");
	if (invStd == undefined) invStd = "true";
	invManual = GetQSParam(document.location.search, "invManual");
	if (invManual == undefined) invManual = "true";
	invSDS = GetQSParam(document.location.search, "invSDS");
	if (invSDS == undefined) invSDS = "true";
//	invDateFrom = GetQSParam(document.location.search, "invDateFrom");
//	if (invDateFrom == undefined) invDateFrom = "null";
//	invDateTo = GetQSParam(document.location.search, "invDateTo");
//	if (invDateTo == undefined) invDateTo = NULL_DATE_TO;
	invActual = GetQSParam(document.location.search, "invActual");
	if (invActual == undefined) invActual = "true";
	invProvisional = GetQSParam(document.location.search, "invProvisional");
	if (invProvisional == undefined) invProvisional = "true";
	invRetracted = GetQSParam(document.location.search, "invRetracted");
	if (invRetracted == undefined) invRetracted = "true";
	invViaRetract = GetQSParam(document.location.search, "invViaRetract");
	if (invViaRetract == undefined) invViaRetract = "true";
	invZeroValue = GetQSParam(document.location.search, "invZeroValue");
	if (invZeroValue == undefined) invZeroValue = "false";
	invBatchSel = GetQSParam(document.location.search, "invBatchSel");
	if (invBatchSel == undefined) invBatchSel = "0";
	invExclude = GetQSParam(document.location.search, "invExclude");
	if (invExclude == undefined) invExclude = "";
	listDebtorFilter = GetQSParam(document.location.search, "filterDebtor");
	if (listDebtorFilter == undefined || listDebtorFilter == "null") listDebtorFilter = "";
	listInvoiceNumFilter = GetQSParam(document.location.search, "filterInvNum");
	if (listInvoiceNumFilter == undefined || listInvoiceNumFilter == "null") listInvoiceNumFilter = "";
	listClientRefFilter = GetQSParam(document.location.search, "filterClientRef");
	if (listClientRefFilter == undefined || listClientRefFilter == "null") listClientRefFilter = "";
	listCommentFilter = GetQSParam(document.location.search, "filterComment");
	if (listCommentFilter == undefined || listCommentFilter == "null") listCommentFilter = "";
	isBalanceRun = GetQSParam(document.location.search, "OBR");
	if (invExclude == undefined) isBalanceRun = "false";
}

//addEvent(window, "load", Init);