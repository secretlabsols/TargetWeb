
var btnSelect, id, reference, name;

function Init() {
	btnSelect = GetElement("btnSelect");
	if(selectedEstablishmentID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.InPlaceEstablishment_ItemSelected(id, reference, name);
	window.parent.close();
}
function EstablishmentSelector_SelectedItemChange(args) {
	id = args[0];
	reference = args[1];
	name = args[2];
	if(id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);
