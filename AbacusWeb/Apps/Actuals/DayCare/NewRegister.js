
var Edit_domContractID, dteDatesEffectiveDateID, contractSvc, sgClassificationID;

function Init() {
    contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
    ServiceGroupClassificationID = sgClassificationID;
}

function InPlaceEstablishment_Changed(newID) {
	var enabled = false;
	if(newID.trim().length > 0) enabled = true;
	InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
	if(enabled) {
		InPlaceDomContractSelector_Enabled(Edit_domContractID, true);
	} else {
		InPlaceDomContractSelector_Enabled(Edit_domContractID, false);
	}
	InPlaceDomContractSelector_providerID = newID;
	ServiceGroupClassificationID = sgClassificationID;
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

addEvent(window, "load", Init);


