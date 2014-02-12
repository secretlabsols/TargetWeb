
var contractSvc, contractHeader_InUse, cboRateFramework, chkUseEhancedRateDays, chkBankHolidayCover, cboContractType, chkDSOMaintainedElectronically;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	cboRateFramework = GetElement("cboRateFramework_cboDropDownList", true);
	chkUseEhancedRateDays = GetElement("chkUseEnhancedRateDays_chkCheckbox");
	chkBankHolidayCover = GetElement("chkBankHolidayCover_chkCheckbox");
	cboContractType = GetElement("cboContractType_cboDropDownList", true);
	chkDSOMaintainedElectronically = GetElement("chkDsoMaintExternal_chkCheckbox");
	setDSOMaintainedElectronically(chkDSOMaintainedElectronically);
}

function cboContractType_Change(clientSelectorID) {
	if (cboContractType) {
		var spot = (cboContractType.value == Target.Abacus.Library.DomContractType.Spot);
		var periodicBlock = (cboContractType.value == Target.Abacus.Library.DomContractType.BlockPeriodic); 

		InPlaceClientSelector_Enabled(clientSelectorID, (spot && !contractHeader_InUse && !cboContractType.disabled));
		if (!spot && !contractHeader_InUse)
			InPlaceClientSelector_ClearStoredID(clientSelectorID);

		    chkDSOMaintainedElectronically.checked = periodicBlock;
		    chkDSOMaintainedElectronically.disabled = periodicBlock;
	}
}

function cboRateFramework_Change() {
	if(cboRateFramework) {
		var rfID = cboRateFramework.value;
		if (rfID.trim().length == 0) rfID = 0;
		DisplayLoading(true);
		contractSvc.RateFrameworkUsesEnhancedRateDays(rfID, RateFrameworkUsesEnhancedRateDays_Callback);
	}
}
function RateFrameworkUsesEnhancedRateDays_Callback(response) {
	if(CheckAjaxResponse(response, contractSvc.url)) {
		chkUseEhancedRateDays.checked = response.value.Value;
		if(chkUseEhancedRateDays.checked) {
		   chkBankHolidayCover.disabled = false;
		   chkBankHolidayCover.checked = true;
		} else {
			chkBankHolidayCover.checked = false;
			chkBankHolidayCover.disabled = true;
		}
	}
	DisplayLoading(false);
}

function setDSOMaintainedElectronically(chkDSOMaintainedElectronically) {
    var periodicBlock;
    if (cboContractType != null) {
        periodicBlock = (cboContractType.value == Target.Abacus.Library.DomContractType.BlockPeriodic);
    } else {
        periodicBlock = false;
    }
    chkDSOMaintainedElectronically.checked = periodicBlock;
    chkDSOMaintainedElectronically.disabled = periodicBlock;
}
