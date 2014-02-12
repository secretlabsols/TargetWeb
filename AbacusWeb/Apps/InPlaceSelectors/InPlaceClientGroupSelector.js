
var InPlaceClientGroupSelector_currentID;

function InPlaceClientGroupSelector_btnFind_Click(id) {
	var txtRef, txtName, hidID;
	
	InPlaceClientGroupSelector_currentID = id;
	
	txtRef = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/ClientGroups.aspx?&id=" + hidID.value;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceClientGroup_ItemSelected(id, name, ref) {
	var txtRef, txtName, hidID;
	txtRef = GetElement(InPlaceClientGroupSelector_currentID + "_txtReference");
	txtName = GetElement(InPlaceClientGroupSelector_currentID + "_txtName");
	hidID = GetElement(InPlaceClientGroupSelector_currentID + "_hidID");

	hidID.value = id;	
	txtName.value = name;
	txtRef.value = ref;

	if (typeof (InPlaceClientGroupSelector_Changed) == "function") {
	    InPlaceClientGroupSelector_Changed(hidID.value);
	}
}
function InPlaceClientGroupSelector_ClearStoredID(id) {
	var txtRef, txtName, hidID;
	
	txtRef = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtRef.value = "";
	txtName.value = "";
	hidID.value = "";

	if (typeof (InPlaceClientGroupSelector_Changed) == "function") {
	    InPlaceClientGroupSelector_Changed(hidID.value);
	}
}