
var Edit_domContractID, Edit_domProviderID, Edit_serviceUserID, systemCalcType;
var selectedProviderID = 0, selectedContractID = 0;
var dtefromID, dteToId;
var radioProviderId, radioServiceUserId;
var defaultWEDate, minDate;

function Init() {
    var rdprovider = GetElement(radioProviderId);
    $(rdprovider).attr('checked', true);
    selectionChanged(radioProviderId, 'provider');
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
    var rdprovider = GetElement(radioProviderId);
    var btnCreate = GetElement(btnCreateJob);
    var dateFromCtrl = $('#' + dtefromID + '_txtTextBox');
    var dateToCtrl = $('#' + dteToId + '_txtTextBox');
    if (dateFromCtrl.val().length > 0) {
        var dateToDate = dateFromCtrl.val().toDate();
        dateToDate.setDate(dateToDate.getDate());
        dateToCtrl.datepicker('option', 'minDate', dateToDate);

        btnCreate.disabled = false;
    }
    else {

        if ($(rdprovider).is(':checked')) {

            btnCreate.disabled = true;
        }
    }
}

function selectionChanged(val, name) {

    var dateFromCtrl = $('#' + dtefromID + '_txtTextBox');
    var dateToCtrl = $('#' + dteToId + '_txtTextBox');
    var clearer = dateFromCtrl.textboxClearer;
    var btnCreate = GetElement(btnCreateJob);
    btnCreate.disabled = false;

    if (name == 'provider') {
        var d = Date.toDateFromString(defaultWEDate);
        InPlaceClientSelector_ClearStoredID(Edit_serviceUserID);
        InPlaceClientSelector_Enabled(Edit_serviceUserID, false);
        InPlaceEstablishmentSelector_Enabled(Edit_domProviderID, true);
        InPlaceDomContractSelector_Enabled(Edit_domContractID, true);
        dateFromCtrl.datepicker('option', 'minDate', d);
        dateFromCtrl.datepicker("setDate", d);
        dateToCtrl.datepicker('option', 'minDate', d);

    } else if (name == 'serviceUser') {
        var d = Date.toDateFromString(minDate); 
        InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
        InPlaceEstablishmentSelector_ClearStoredID(Edit_domProviderID);
        InPlaceEstablishmentSelector_Enabled(Edit_domProviderID, false);
        InPlaceDomContractSelector_Enabled(Edit_domContractID, false);
        InPlaceClientSelector_Enabled(Edit_serviceUserID, true);
        dateFromCtrl.datepicker('option', 'minDate', d);
        dateToCtrl.datepicker('option', 'minDate', d);
        
    }

}

addEvent(window, "load", Init);
