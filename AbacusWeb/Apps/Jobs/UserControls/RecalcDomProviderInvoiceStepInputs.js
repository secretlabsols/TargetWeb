
var Edit_domContractID, chkCreateProformaID, chkCreateProviderID;
var selectedProviderID = 0, selectedContractID = 0;
var dtefromID, dteToId;

function Init() {

}
function InPlaceEstablishment_Changed(newID) {
	InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
	InPlaceDomContractSelector_providerID = newID;
	selectedProviderID = newID;
}
function InPlaceDomContract_Changed(newID) {
    selectedContractID = newID;
}
function dteDateFrom_Changed(id) {

    var dateFromCtrl = $('#' + dtefromID + '_txtTextBox');
    var dateToCtrl = $('#' + dteToId + '_txtTextBox');
    if (dateFromCtrl.val().length > 0) {
        var dateToDate = dateFromCtrl.val().toDate();
        dateToDate.setDate(dateToDate.getDate());
        dateToCtrl.datepicker('option', 'minDate', dateToDate);
    } 
}


addEvent(window, "load", Init);
