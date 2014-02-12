
var btnSelect, id, name, ref;

function Init() {
	btnSelect = GetElement("btnSelect");
	if(FinanceCodeSelector_selectedFinanceCodeID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.InPlaceFinanceCode_ItemSelected(id, name);
	window.parent.close();
}
function FinanceCodeSelector_SelectedItemChange(args) {
	id = args[0];
	name = args[1];
	if(id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);
