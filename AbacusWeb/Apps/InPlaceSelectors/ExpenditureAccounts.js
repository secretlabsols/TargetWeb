
var btnSelect, id, desc, type;

function Init() {
	btnSelect = GetElement("btnSelect");
	if(ExpenditureAccountSelector_selectedAccountID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.InPlaceExpenditureAccountSelector_ItemSelected(id, desc, type);
	window.parent.close();
}
function InPlaceExpenditureAccountSelector_SelectedItemChange(args) {
	id = args[0];
	desc = args[1];
	type = args[2];
	if(id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);
