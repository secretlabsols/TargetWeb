var btnSelect, id, name, parentControlID, selectedService;

function Init() {
    btnSelect = GetElement("btnSelect");
    if (ServiceSelector_SelectedID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    parentWindow.InPlaceServiceSelector_SelectionChanged(parentControlID, selectedService);
    window.parent.close();
}
function ServiceSelector_SelectedItemChanged(selectedItem) {
    selectedService = selectedItem;
    if (selectedService.ID != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);