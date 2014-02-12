var btnSelect, id, name, parentControlID, selectedBudgetCategory;

function Init() {
    btnSelect = GetElement("btnSelect");
    if (BudgetCategorySelector_SelectedID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    parentWindow.InPlaceBudgetCategorySelector_SelectionChanged(parentControlID, selectedBudgetCategory);
    window.parent.close();
}
function BudgetCategorySelector_SelectedItemChanged(selectedItem) {
    selectedBudgetCategory = selectedItem;
    if (selectedBudgetCategory.ID != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);