
var btnSelect, id, number, title;

function Init() {
	btnSelect = GetElement("btnSelect");
	if(DomContractSelector_selectedContractID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.InPlaceDomContract_ItemSelected(id, number, title);
	window.parent.close();
}
function DomContractSelector_SelectedItemChange(args) {
	id = args[0];
	number = args[1];
	title = args[2];
	if(id != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);
