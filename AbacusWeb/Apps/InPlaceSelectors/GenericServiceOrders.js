var btnSelect, id, reference, genericServiceOrderID;

function Init() {
    btnSelect = GetElement("btnSelect");
    if (ServiceOrderSelector_selectedServiceOrderID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    parentWindow.InPlaceGenericServiceOrderSelector_ItemSelected(genericServiceOrderID, id, reference);
    window.parent.close();
}
function GenericServiceOrderSelector_SelectedItemChange(args) {
    id = args[0];
    reference = args[1];
    genericServiceOrderID = args[2];
    if (id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);