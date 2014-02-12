var btnSelect, id, reference, name;

function Init() {
    btnSelect = GetElement("btnSelect");
    //if (selectedExternalAccountID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    if (selectedExternalAccountID != 0)
        parentWindow.InPlaceExternalAccount_ItemSelected(selectedExternalAccountID, selectedExternalAccount);
    window.parent.close();
}
function ExternalAccountSelector_SelectedItemChange(args) {
    id = args[0];
    reference = args[1];
    name = args[2];
    if (id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);