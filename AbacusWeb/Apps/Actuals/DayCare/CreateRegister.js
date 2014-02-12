var backUrl = '', contractSelectorClientID, selectedContractID = 0, selectedEstablishmentID = 0, serviceRegisterSvc, weekEndingClientID = '', createButton;
var isIniting;

function CreateServiceRegister() {
    
    if (Page_ClientValidate() == true) {
        DisplayLoadingWithDisabling(true, true);
        var weekendingDate = GetElement(weekEndingClientID).value;
        var year = Number(weekendingDate.substring(6, 10));
        var month = Number(weekendingDate.substring(3, 5)) - 1;
        var day = Number(weekendingDate.substring(0, 2));
        var weekendingDateFormatted = new Date(year, month, day);
        serviceRegisterSvc.CreateNewRegister(selectedEstablishmentID, selectedContractID, weekendingDateFormatted, CreateServiceRegister_CallBack);       
    }
}

function CreateServiceRegister_CallBack(serviceResponse) {
    if (CheckAjaxResponse(serviceResponse, serviceRegisterSvc.url, false)) {
        document.location.href = 'Register.aspx?id=' + serviceResponse.value.RegisterID + '&backUrl=' + backUrl;
    } else {
        DisplayLoadingWithDisabling(false, false);
    }
}

function DisplayLoadingWithDisabling(display, disabled) {
    DisplayLoading(display);
    $('input').attr('disabled', disabled);
}

function InPlaceDomContract_Changed(newID) {
    selectedContractID = parseInt(newID);
    ResetFormVisibility();
}

function InPlaceEstablishment_Changed(newID) {
    InPlaceDomContractSelector_ClearStoredID(contractSelectorClientID);
    selectedContractID = 0;
    selectedEstablishmentID = parseInt(newID);
    InPlaceDomContractSelector_providerID = selectedEstablishmentID;
    ResetFormVisibility();
}

function ResetFormVisibility() {
    var isEstablishmentSelected = (selectedEstablishmentID && selectedEstablishmentID > 0);
    var isContractSelected = (selectedContractID && selectedContractID > 0);
    var isReadyForCreate = (isEstablishmentSelected && isContractSelected);
    InPlaceDomContractSelector_Enabled(contractSelectorClientID, isEstablishmentSelected);
    createButton.attr('disabled', !isReadyForCreate);
    if (isReadyForCreate) {
        createButton.attr('title', 'Create new Service Register?');
    } else {
        createButton.attr('title', 'Please select a Provider, Contract and Week Ending Date to Create a new Service Register.');
    }
    
    if (!isIniting) {
        Page_ClientValidate();
    }
}

$(document).ready(function() {
    isIniting = true;
    if (selectedEstablishmentID != 0 && selectedContractID == 0) {
        InPlaceDomContractSelector_providerID = selectedEstablishmentID;
    }
    createButton = $('input:button[value=\'Create\']');
    serviceRegisterSvc = new Target.Abacus.Web.Apps.WebSvc.ServiceRegisters_class();
    ResetFormVisibility();
    isIniting = false;
});