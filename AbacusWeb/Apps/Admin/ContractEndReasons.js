var dpContractCheckBoxID, dpContractCheckBox, dpContractPaymentCheckBoxID, dpContractPaymentCheckBox;
var isEditable = false;

function Init() {
    dpContractCheckBox = GetElement(dpContractCheckBoxID);
    dpContractPaymentCheckBox = GetElement(dpContractPaymentCheckBoxID);
    chkDPContracts_OnClick(dpContractCheckBox);
}

function chkDPContracts_OnClick(cb) {
    if (isEditable == true) {
        var autoCheckDpPayments = false;
        if (cb.checked == true) {
            autoCheckDpPayments = true;
        }
        dpContractPaymentCheckBox.checked = autoCheckDpPayments;
        dpContractPaymentCheckBox.disabled = autoCheckDpPayments;
    }    
}

addEvent(window, "load", Init);