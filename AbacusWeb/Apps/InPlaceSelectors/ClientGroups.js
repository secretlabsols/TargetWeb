
var btnSelect, id, name, ref;

function Init() {
	btnSelect = GetElement("btnSelect");
	if(ClientGroupSelector_selectedClientGroupID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.InPlaceClientGroup_ItemSelected(id, name, ref);
	window.parent.close();
}
function ClientGroupSelector_SelectedItemChange(args) {
	id = args[0];
	name = args[1];
	ref = args[2];
	if(id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);
