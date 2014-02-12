var btnSelect, id, name, parentControlID, selectedServiceType;

function Init() {
    btnSelect = GetElement("btnSelect");
    if (ServiceTypeSelector_SelectedID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    parentWindow.InPlaceServiceTypeSelector_SelectionChanged(parentControlID, selectedServiceType);
    window.parent.close();
}
function ServiceTypeSelector_SelectedItemChanged(selectedItem) {
    selectedServiceType = selectedItem;
    if (selectedServiceType.ID != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);