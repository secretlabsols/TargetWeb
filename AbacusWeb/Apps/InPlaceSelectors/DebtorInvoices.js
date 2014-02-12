var btnSelect, id, name, ref;

function Init() {
    btnSelect = GetElement("btnSelect");
    if (DebtorInvoiceSelector_selectedDebtorInvoiceID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    parentWindow.InPlaceDebtorInvoice_ItemSelected(id, name, ref);
    window.parent.close();
}
function DebtorInvoiceSelector_SelectedItemChange(args) {
    id = args[0];
    name = args[1];
    ref = args[2]
    if (id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);
