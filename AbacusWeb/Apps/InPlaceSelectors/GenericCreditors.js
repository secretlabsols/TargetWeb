var btnSelect, id, name, parentControlID, selectedGenericCreditor;

function Init() {
    btnSelect = GetElement("btnSelect");
    btnSelect.disabled = true;
}

function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}

function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    parentWindow.InPlaceGenericCreditorSelector_SelectionChanged(parentControlID, selectedGenericCreditor);
    window.parent.close();
}

function GenericCreditorSelector_SelectedItemChanged(selectedItem) {
    selectedGenericCreditor = selectedItem;
    if (selectedItem) {
        if (selectedGenericCreditor.ID != 0) btnSelect.disabled = false;
    }
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);