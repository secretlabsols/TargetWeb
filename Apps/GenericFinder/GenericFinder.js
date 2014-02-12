
var genericFinder_SelectedID = 0;

function rdoSelect_Click(value) {
	var index, rdo, tblGenericFinder;
	tblGenericFinder = GetElement("tblGenericFinder");
	for (index=0;index<tblGenericFinder.tBodies[0].rows.length;index++){
		rdo = tblGenericFinder.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo) {
			if (rdo.checked)
				tblGenericFinder.tBodies[0].rows[index].className = "highlightedRow";
			else
				tblGenericFinder.tBodies[0].rows[index].className = "";
		}
	}
	genericFinder_SelectedID = value;
	GetElement("btnSelect").disabled = false;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
	window.parent.close();
}
function btnSelect_Click() {
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
	parentWindow.StdButtons_ItemSelected(genericFinder_SelectedID);
	window.parent.close();
}
function GenericFinder_FetchPage(page) {
    var url = document.location.href;
    url = AddQSParam(RemoveQSParam(url, "page"), "page", page);
    removeEvent(window, "unload", DialogUnload);
    document.location.href = url;
}
addEvent(window, "unload", DialogUnload);