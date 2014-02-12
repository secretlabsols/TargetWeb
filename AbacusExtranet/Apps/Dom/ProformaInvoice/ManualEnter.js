
var CTRL_PREFIX_VISIT_DOW, CTRL_PREFIX_VISIT_CODE, CTRL_PREFIX_DURATION, CTRL_PREFIX_ACTUAL_DURATION, OriginalValueChanged;
var contractSvc, contractID, dteWeekEnding, tblVisits;
var selectedInvoiceBatchID;

function Init() {
    contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
    dteWeekEnding = GetElement("dteWeekEnding_txtTextBox");
    tblVisits = GetElement("tblVisits");
}
function FetchVisitCodeList(uniqueID) {
    var visitDate, cboDoW, dow;
    
    visitDate = dteWeekEnding.value.toDate();
    cboDoW = GetElement(CTRL_PREFIX_VISIT_DOW + uniqueID + "_cboDropDownList");
    // adjust the visit date back from the week ending date, if we have a dow
    if(cboDoW.value.length > 0) {
        dow = parseInt(cboDoW.value, 10);
        while(visitDate.getDay() != dow) {
            visitDate.addDays(-1);
        }
    }    
    
    DisplayLoading(true);
    contractSvc.FetchVisitCodesAvailableForVisit(contractID, visitDate, uniqueID, FetchVisitCodeList_Callback);
}
function FetchVisitCodeList_Callback(response) {
    var visitCodes, index, uniqueID, cboVisitCode, opt, selectedVisitCode, selectedVisitCodeAvailable;

    if(CheckAjaxResponse(response, contractSvc.url)) {
        visitCodes = response.value.List;
        uniqueID = response.value.UniqueID;
        cboVisitCode = GetElement(CTRL_PREFIX_VISIT_CODE + uniqueID + "_cboDropDownList");
        selectedVisitCode = cboVisitCode.value;
        
        // clear
	    cboVisitCode.options.length = 0;
	    // add blank		
	    opt = document.createElement("OPTION");
	    cboVisitCode.options.add(opt);
	    SetInnerText(opt, "");
	    opt.value = "";
		
		for(index=0; index<visitCodes.length; index++) {
		    if(visitCodes[index].Value == selectedVisitCode)
		        selectedVisitCodeAvailable = true;
		    opt = document.createElement("OPTION");
		    cboVisitCode.options.add(opt);
		    SetInnerText(opt, visitCodes[index].Text);
		    opt.value = visitCodes[index].Value;
		}
		
		// select
		if(selectedVisitCodeAvailable)
		    cboVisitCode.value = selectedVisitCode;
		else
	        cboVisitCode.value = response.value.DefaultCodeID;
    }
    DisplayLoading(false);
}

function btnDelete_Click() {
    return confirm("Once an invoice batch is deleted it cannot be re-instated.\n\nAre you sure you wish to delete this invoice batch?");
}

function btnCancel_Click() {
    var submit = "true";
    var hidChangeValue = GetElement(OriginalValueChanged);
    if (hidChangeValue.value) {
        submit = confirm("Changes have been made to the set of visits during the current editing session. These changes will be lost if you continue with this action. Please confirm that you understand this and that you wish to continue regardless.")
    }
    return submit;
}

function btnBack_Click() {
    var change = "true";
    var hidChangeValue = GetElement(OriginalValueChanged);
    if (hidChangeValue.value) {
       change  = confirm("Changes have been made to the set of visits during the current editing session. These changes will be lost if you continue with this action. Please confirm that you understand this and that you wish to continue regardless.")
   }
   if (change) {
       var url = GetQSParam(document.location.search, "backUrl");
       url = unescape(url);
        document.location.href = url;
   }
}

function dteWeekEnding_Changed(id) {
    var index, uniqueID;
    for(index=0; index<tblVisits.tBodies[0].rows.length; index++) {
        uniqueID = tblVisits.tBodies[0].rows[index].id;
        FetchVisitCodeList(uniqueID.replace(MPCONTENT_PREFIX, ""));
    }
}

function ctlDuration_Change(uniqueID) {

    var srcHourControl = GetElement(CTRL_PREFIX_DURATION + uniqueID + "_cboHours");
    var srcMinuteControl = GetElement(CTRL_PREFIX_DURATION + uniqueID + "_cboMinutes");
    var dstHourControl = GetElement(CTRL_PREFIX_ACTUAL_DURATION + uniqueID + "_cboHours");
    var dstMinuteControl = GetElement(CTRL_PREFIX_ACTUAL_DURATION + uniqueID + "_cboMinutes");

    dstHourControl.value = srcHourControl.value;
    dstMinuteControl.value = srcMinuteControl.value;
}

function btnRemoveVisit_Click() {
    return window.confirm("Are you sure you wish to remove this Rounding detail?");
}

function txtPaymentClaimed_Changed() {
    valuechanged();
}
function txtReference_Changed() {
    valuechanged();
}
function dteWeekEnding_Changed() {
    valuechanged();
}
function valuechanged() {
    var hidChangeValue = GetElement(OriginalValueChanged);
    hidChangeValue.value = "true";
}

addEvent(window, "load", Init);
