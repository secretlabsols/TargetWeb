
var btnSelect, id, reference, name;

function Init() {
	btnSelect = GetElement("btnSelect");
	if(ClientSelector_selectedClientID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.InPlaceClient_ItemSelected(id, reference, name);
	window.parent.close();
}
function ClientSelector_SelectedItemChange(args) {
	id = args[0];
	reference = args[1];
	name = args[2];
	if(id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);
