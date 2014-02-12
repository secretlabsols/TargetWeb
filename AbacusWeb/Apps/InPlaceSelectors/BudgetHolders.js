
var btnSelect, id, ref, name;

function Init() {
    btnSelect = GetElement("btnSelect");
    if (budgetHolderSelector_selectedbhID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    parentWindow.InPlaceBudgetHolder_ItemSelected(id, name, ref);
    window.parent.close();
}
function BudgetHolderSelector_SelectedItemChange(args) {
    id = args[0];
    ref = args[1];
    name = args[4];
    if (id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);
