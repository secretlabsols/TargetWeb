var btnSelectContract, id, reference, name;

function Init() {
    btnSelectContract = GetElement("btnSelectContract");
    if (selectedDcrDomContractId == 0) btnSelectContract.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelectContract_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    if (selectedDcrDomContractId != 0)
        parentWindow.InPlaceDcrDomContractSelector_ItemSelected(selectedDcrDomContractId, selectedDcrDomContractTitle, selectedDcrDomContractNumber);
    window.parent.close();
}
//function ExternalAccountSelector_SelectedItemChange(args) {
//    id = args[0];
//    reference = args[1];
//    name = args[2];
//    if (id != 0) btnSelectContract.disabled = false;
//}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);