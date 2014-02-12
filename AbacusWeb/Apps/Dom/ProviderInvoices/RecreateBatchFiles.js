    
var contractSvc, currentDPIBatchID, createdBy, Recreate_postingDate;
var dteStartDate, tmeStartDateHours, tmeStartDateMinutes;
var dtePostingDate, cboPostingYear, cboPeriodNum, chkReread;
var lblPostingDate, lblPostingYear, lblPeriodNum;
var Recreate_dteStartDateID, Recreate_tmeStartDateID, Recreate_dtePostingDateID;
var Recreate_cboPostingYearID, Recreate_cboPeriodNumID, Recreate_chkRereadDataID;
var rdoRollbackNone, rdoRollbackAll, rdoRollbackPartial, rdoCreateNow, rdoDefer;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	dteStartDate = GetElement(Recreate_dteStartDateID + "_txtTextBox");
	tmeStartDateHours = GetElement(Recreate_tmeStartDateID + "_cboHours");
	tmeStartDateMinutes = GetElement(Recreate_tmeStartDateID + "_cboMinutes");
	dtePostingDate = GetElement(Recreate_dtePostingDateID + "_txtTextBox");
	cboPostingYear = GetElement(Recreate_cboPostingYearID + "_cboDropDownList");
	cboPeriodNum = GetElement(Recreate_cboPeriodNumID + "_cboDropDownList");
	chkReread = GetElement(Recreate_chkRereadDataID);
	rdoRollbackNone = GetElement("optDoNotRollback");
	rdoRollbackAll = GetElement("optFullRollback");
	rdoRollbackPartial = GetElement("optPartialRollback");
	rdoCreateNow = GetElement("optCreateNow");
	rdoDefer = GetElement("optDefer");
	lblPostingDate = GetElement("lblPostingDate");
	lblPostingYear = GetElement("lblPostingYear");
	lblPeriodNum = GetElement("lblPeriodNum");
	
	currentDPIBatchID = GetQSParam(document.location.search, "batchid");
	createdBy = GetQSParam(document.location.search, "createdby");

    // Default the variables to match the original values..
    dtePostingDate.value = Recreate_postingDate;
    cboPostingYear.value = lblPostingYear.innerHTML;
    cboPeriodNum.value = lblPeriodNum.innerHTML;
}

function RecreateFiles_Callback(response) {
	var isSuccess, url;

	DisplayLoading(false);

	if(CheckAjaxResponse(response, contractSvc.url)) {
		isSuccess = response.value.IsSuccess;

		if(isSuccess) 
		{
		    alert("Interface files re-created successfully.");
		    btnBack_Click();
		}
		else
		{
		    alert("Interface files NOT re-created:\n" + response.value.ErrMsg.ErrorMessage);
		}
	}
}

function btnBack_Click() {
    var url = "ListBatch.aspx?currentPage=1&id=" + currentDPIBatchID;
        
    document.location.href = url;
}

//addEvent(window, "load", Init);
