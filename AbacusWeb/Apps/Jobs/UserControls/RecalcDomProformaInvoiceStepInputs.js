
var Edit_domContractID;
//, chkCreateProformaID, chkCreateProviderID;
var selectedProviderID = 0, selectedContractID = 0;
var dtefromID, dteToId;
//var chkCreateProforma, chkCreateProvider;

function Init() {
    //chkCreateProforma = GetElement(chkCreateProformaID + "_chkCheckbox");
    //chkCreateProvider = GetElement(chkCreateProviderID + "_chkCheckbox");
    //chkCreateProforma_Click();
  
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
//function chkCreateProforma_Click() {
//    if (chkCreateProforma.checked) {
//        chkCreateProvider.disabled = false;
//    } else {
//        chkCreateProvider.disabled = true;
//        chkCreateProvider.checked = false;
//    }
//    // Disable create provider invoice option
//    // see http://sharepoint:18085/personal/mikevo/Lists/Electronic%20Monitoring%20Issues/DispForm.aspx?ID=71
//    chkCreateProvider.checked = false;
//    chkCreateProvider.disabled = true;
//}

addEvent(window, "load", Init);
