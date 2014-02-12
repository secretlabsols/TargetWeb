
var contractSvc;
var DomContractPeriodSysAccFilterStep_contractPeriodID, DomContractPeriodSysAccFilterStep_systemAccountID;
var DomContractPeriodSysAccFilterStep_providerID, DomContractPeriodSysAccFilterStep_contractID;
var DomContractPeriodSysAccFilterStep_dateFrom, DomContractPeriodSysAccFilterStep_dateTo;
var DomContractPeriodSysAccFilterStep_selectedPeriod = 0, DomContractPeriodSysAccFilterStep_selectedSystemAccount = 0;
var cboPeriod, cboSysAcc;

function Init() {
    contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();

    cboSysAcc = GetElement(DomContractPeriodSysAccFilterStep_systemAccountID + "_cboDropDownList");
    
    FetchSystemAccounts();
}

function FetchSystemAccounts() {
	DisplayLoading(true);
	contractSvc.FetchSystemAccountList(DomContractPeriodSysAccFilterStep_providerID,DomContractPeriodSysAccFilterStep_contractID, DomContractPeriodSysAccFilterStep_dateFrom, DomContractPeriodSysAccFilterStep_dateTo, FetchSystemAccounts_Callback)
}
function FetchSystemAccounts_Callback(response) {
    var sysAccs, opt;
    if(CheckAjaxResponse(response, contractSvc.url)) {
        sysAccs = response.value.List;
    
		// clear
	    cboSysAcc.options.length = 0;
	    // add blank		
	    opt = document.createElement("OPTION");
	    cboSysAcc.options.add(opt);
	    SetInnerText(opt, "");
	    opt.value = 0;
		
		for(index=0; index<sysAccs.length; index++) {
		    opt = document.createElement("OPTION");
		    cboSysAcc.options.add(opt);
		    SetInnerText(opt, sysAccs[index].Text);
		    opt.value = sysAccs[index].Value;
		}
		
		// select existing value
	    cboSysAcc.value = DomContractPeriodSysAccFilterStep_selectedSystemAccount;
	    	    
	}
	DisplayLoading(false);
}

function DomContractPeriodSysAccFilterStep_BeforeNavigate() {
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	url = AddQSParam(RemoveQSParam(url, "sysAccID"), "sysAccID", cboSysAcc.value);
	
	SelectorWizard_newUrl = url;
	return true;
}

function dteDateTo_Changed(dteDateToID) {
    var dteDate = GetElement(dteDateToID + "_txtTextBox", true);
    if (dteDate.value == ""){
        var maxDate = "31/12/9999";
        DomContractPeriodSysAccFilterStep_dateTo = maxDate.toDate();
    } else {
        DomContractPeriodSysAccFilterStep_dateTo = dteDate.value.toDate();
    }

    FetchSystemAccounts();
}

function dteDateFrom_Changed(dteDateFromID) {
    var dteDate = GetElement(dteDateFromID + "_txtTextBox", true);
    DomContractPeriodSysAccFilterStep_dateFrom = dteDate.value.toDate();
    FetchSystemAccounts();

}

addEvent(window, "load", Init);
