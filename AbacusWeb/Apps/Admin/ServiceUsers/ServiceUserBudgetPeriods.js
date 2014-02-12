var SUBP_DefaultBudgetPeriod, SUBP_dteDateFromID, SUBP_ClientID, SUBP_sdsSvc;

function Init() {
    SUBP_sdsSvc = new Target.Abacus.Web.Apps.WebSvc.Sds_class();
}

function SUBP_CanChange(SUBP_dteDateFromID, originalDateFromValue, SUBP_dteDateToID) {
    var response;
    var returnValue;

    if (SUBP_DefaultBudgetPeriod == 3 && originalDateFromValue!="") {
        var txtdatefrom = GetElement(SUBP_dteDateFromID + "_txtTextBox");
        var txtdateto = GetElement(SUBP_dteDateToID + "_txtTextBox");
        
        response = SUBP_sdsSvc.DisplayOverlappingWarningMessageOnBudgetPeriod(SUBP_ClientID, originalDateFromValue, txtdatefrom.value, txtdateto.value)
        if (response.value) {
            returnValue = window.confirm('Spend Plans and/or Direct Payments exist that overlap with this Budget Period. ' +
                                'Changing the day of the week upon which the Budget Period starts may cause budget ' +
                                'allocation and/or payments to be apportioned across more than one Budget Period. ' +
                                '\r\rAre you sure you wish to continue?');            
            if (returnValue == false){
                txtdatefrom.value = originalDateFromValue;
            } 
            return returnValue;
        } else {
            return true;
        }
    } else {
        return true;
    }

}

addEvent(window, "load", Init);