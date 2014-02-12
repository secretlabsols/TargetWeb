
var Edit_domContractID, Edit_NewRegID, Edit_AmendedRegID;
var batchTypeIDs, selectedProviderID = 0, selectedContractID = 0;

function Init() {
    var chkNew = GetElement(Edit_NewRegID + "_chkCheckbox");
    var chkAmended = GetElement(Edit_AmendedRegID + "_chkCheckbox");
    chkNew.checked = true;
    chkAmended.checked = true;
}

function InPlaceEstablishment_Changed(newID) {
	InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
	InPlaceDomContractSelector_providerID = newID;
	selectedProviderID = newID;
}

function InPlaceDomContract_Changed(newID) {
    selectedContractID = newID;
}
function btnPreviewAttendanceRegisters_Click() {
    
    var chkNew = GetElement(Edit_NewRegID + "_chkCheckbox");
    var chkAmended = GetElement(Edit_AmendedRegID + "_chkCheckbox");
    var providerID = selectedProviderID;
    var contractID = selectedContractID;
    
    if(chkNew.checked != true && chkAmended.checked !=true) {
        alert("Please select at least one Batch Type.");
        return;
    }
    
    var url = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Jobs/PreviewAttendanceRegisters.aspx" +
        "?nRegisters=" + chkNew.checked +
        "&aRegisters=" + chkAmended.checked +
        "&providerID=" + providerID +
        "&contractID=" + contractID;
        
    window.open(url);
}

addEvent(window, "load", Init);