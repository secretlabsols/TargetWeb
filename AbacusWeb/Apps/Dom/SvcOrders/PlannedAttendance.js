function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}

addEvent(window, "unload", DialogUnload);