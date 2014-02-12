var btnSelect, id, name, ref;

function Init() {
	btnSelect = GetElement("btnSelect");
	if(ServiceGroupSelector_selectedServiceGroupID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.InPlaceServiceGroup_ItemSelected(id, name);
	window.parent.close();
}
function ServiceGroupSelector_SelectedItemChange(args) {
	id = args[0];
	name = args[1];
	if(id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);