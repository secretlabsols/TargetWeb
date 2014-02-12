
var SelectorWizard_id, SelectorWizard_dateFromID, SelectorWizard_dateToID;
var SelectorWizard_dateFromID, SelectorWizard_dateToID, SelectorWizard_reqDateFromID, SelectorWizard_reqDateToID;
var SelectorWizard_requestedByID, SelectorWizard_optCouncilID, SelectorWizard_optProviderID, SelectorWizard_optBothID;
var optCouncil, optProvider, optBoth, reqByCompanyID, reqByUserID, originator;
var SelectorWizard_requestedByID, cboRequestedBy, reqByID, webSecuritySvc;
var CouncilUser, optCouncil, optProvider, optBoth, statusValue;
var WebCompanyID, dateFrom, dateTo, statusFrom, statusTo;
var VisitAmendmentRequestEnquiryFilterStep_StatusIDs, VisitAmendmentRequestEnquiryFilterStep_StatusPrefix;
var useLastRequestedBy = true;

function VisitAmendmentRequestEnquiryFilterStep_BeforeNavigate() {
	var dateFrom = GetElement(SelectorWizard_dateFromID + "_txtTextBox").value;
	var dateTo = GetElement(SelectorWizard_dateToID + "_txtTextBox").value;
	var reqDateFrom = GetElement(SelectorWizard_reqDateFromID + "_txtTextBox").value;
	var reqDateTo = GetElement(SelectorWizard_reqDateToID + "_txtTextBox").value;
	var selectedStatus = VisitAmendmentRequestEnquiryFilterStep_GetSelectedTotal(VisitAmendmentRequestEnquiryFilterStep_StatusIDs, VisitAmendmentRequestEnquiryFilterStep_StatusPrefix);
	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
    
    cboRequestedBy = GetElement(SelectorWizard_requestedByID + "_cboDropDownList");
    optCouncil  = GetElement(SelectorWizard_optCouncilID);
    optProvider = GetElement(SelectorWizard_optProviderID);
    optBoth = GetElement(SelectorWizard_optBothID);
    

	url = RemoveQSParam(url, "dateFrom");	
	url = RemoveQSParam(url, "dateTo");	
	url = RemoveQSParam(url, "reqDateFrom");	
	url = RemoveQSParam(url, "reqDateTo");
	url = RemoveQSParam(url, "originator");	
	url = RemoveQSParam(url, "requestedby");
	url = RemoveQSParam(url, "status");
	url = RemoveQSParam(url, "reqByID");
	url = RemoveQSParam(url, "reqByCompanyID");
	
	if(dateFrom.length > 0) url = AddQSParam(url, "dateFrom", dateFrom);
	if(dateTo.length > 0) url = AddQSParam(url, "dateTo", dateTo);
	if(reqDateFrom.length > 0) url = AddQSParam(url, "reqDateFrom", reqDateFrom);
	if(reqDateTo.length > 0) url = AddQSParam(url, "reqDateTo", reqDateTo);
	url = AddQSParam(url, "status", selectedStatus);
    
       
    reqByCompanyID = 0;
    reqByUserID = 0;
    if (CouncilUser == true) {
        if (optCouncil.checked == true) {

            reqByUserID = cboRequestedBy.value;
            reqByCompanyID = 0
            originator = 1;
        }
        if (optProvider.checked == true) {

            reqByCompanyID = cboRequestedBy.value;
            reqByUserID = 0;
            originator = 2
        }
        if (optBoth.checked == true) {

            reqByCompanyID = 0;
            reqByUserID = 0;
            originator = 3;
        }
    
    } else {
        if (optProvider.checked == true) {
            reqByUserID = cboRequestedBy.value;
            reqByCompanyID = 0;
            originator = 2;
        } else {

            reqByUserID = 0;
            reqByCompanyID = 0;
            if (optCouncil.checked == true) {
                originator = 1;
            } else {
                originator = 3;
            }
        }
    }

    url = AddQSParam(url, "originator", originator);
    if (reqByUserID != 0) {
        url = AddQSParam(url, "reqByID", reqByUserID);
    } else if (reqByCompanyID != 0) {
        url = AddQSParam(url, "reqByCompanyID", reqByCompanyID);
    }
    SelectorWizard_newUrl = url;

	return true;
}

function PopulateRequestedBy() {
    webSecuritySvc = new Target.Web.Apps.Security.WebSvc.Security_class();
    cboRequestedBy = GetElement(SelectorWizard_requestedByID+ "_cboDropDownList")
    cboRequestedBy.options.length = 0;
    optCouncil  = GetElement(SelectorWizard_optCouncilID)
    optProvider = GetElement(SelectorWizard_optProviderID)
    optBoth = GetElement(SelectorWizard_optBothID)
    DisplayLoading(true);
    if (CouncilUser == true) {
        if (optCouncil.checked == true) {
            cboRequestedBy.disabled = false;
            webSecuritySvc.FetchSimpleUserList(WebCompanyID, FetchRequestedByList_Callback)
        }
        if (optProvider.checked == true) {
            cboRequestedBy.disabled = false;
            webSecuritySvc.FetchSimpleCompanyList(FetchRequestedByList_Callback)
        }
        if (optBoth.checked == true) {
            cboRequestedBy.disabled = true;
            DisplayLoading(false);
        }
    
    } else {
        if (optProvider.checked == true) {
            cboRequestedBy.disabled = false;
            webSecuritySvc.FetchSimpleUserList(WebCompanyID, FetchRequestedByList_Callback)
        } else {
            cboRequestedBy.disabled = true;
            DisplayLoading(false);
        }

    }
}

function FetchRequestedByList_Callback(response) {
	var keys, keyCounter, items;
	if(CheckAjaxResponse(response, webSecuritySvc.url)) {
		// populate the To filter dropdown
		cboRequestedBy.options.add(new Option("", 0));
		items = response.value.List;
		keys = items.getKeys();
		for(keyCounter=0; keyCounter<keys.length; keyCounter++) {
			cboRequestedBy.options.add(new Option(keys[keyCounter], items.getValue(keys[keyCounter])));
		}
	}
	if (reqByID != 0 && useLastRequestedBy == true) {
        cboRequestedBy.value = reqByID;
    }
    if (reqByCompanyID != 0 && useLastRequestedBy == true) {
        cboRequestedBy.value = reqByCompanyID;
    }
    useLastRequestedBy = false
	DisplayLoading(false);
}

function VisitAmendmentRequestEnquiryFilterStep_GetSelectedTotal(idArray, idPrefix) {
    var result = 0;
    var statusValid = false;
    for(index=0; index<idArray.length; index++) {
        var id = idArray[index];
        var chk = GetElement(id);
        if(chk.checked) {
            id = id.replace(SelectorWizard_id, "");
            id = id.replace(idPrefix, "");
            id = id.replace("_chkCheckbox", "");
            id = id.replace("_", "");
            var value = parseInt(id, 10);
            result += value;
            statusValid = true; //at least ont status has been selected
        }
    }
    
    if (statusValid == false) {
        result = 31; //Default the status with all status's selected
    }
    
    return result;
}

function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}