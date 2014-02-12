
function ListBoxEx_Init() {
	addEvent(document.forms[0], "submit", ListBoxEx_OnSubmit);
}
function ListBoxEx_OnSubmit() {
	if (typeof(GetElement) == "undefined" ) return;
	if (typeof(ListBoxExs) == "undefined") return;
	var i, listBoxEx;
	for (i = 0; i < ListBoxExs.length; i++) {
		listBoxEx = ListBoxExs[i];
		if (typeof(listBoxEx) == "string") {
			ListBoxEx_PersistSelectedItems(listBoxEx);
		}
	}
}
function ListBoxEx_PersistSelectedItems(listBoxExID) {
	var hiddenField = GetElement(listBoxExID + "_SelectedItems");
	var listBoxEx = GetElement(listBoxExID);
	var i;
	var items = "";
	for(i=0; i<listBoxEx.options.length; i++) {
		items += listBoxEx.options[i].text + ":" + listBoxEx.options[i].value;
		// don't add for the last item
		if(i<listBoxEx.options.length-1) items += ";";
	}
	hiddenField.value = items;
}