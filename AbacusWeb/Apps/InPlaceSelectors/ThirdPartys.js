var btnSelect, id, title, surname;

function Init() {
	btnSelect = GetElement("btnSelect");
	if(ThirdPartySelector_selectedThirdPartyID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.InPlaceClientThirdParty_ItemSelected(id, title, surname);
	window.parent.close();
}
function ThirdPartySelector_SelectedItemChange(args) {
	id = args[0];
	title = args[1];
	surname = args[2];
	if(id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);