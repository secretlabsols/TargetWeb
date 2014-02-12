
var InPlaceClientSubGroupSelector_currentID;

function InPlaceClientSubGroupSelector_btnFind_Click(id) {
	var txtRef, txtName, hidID;
	
	InPlaceClientSubGroupSelector_currentID = id;
	
	txtRef = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/ClientSubGroups.aspx?&id=" + hidID.value;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceClientSubGroup_ItemSelected(id, name, ref) {
	var txtRef, txtName, hidID;
	txtRef = GetElement(InPlaceClientSubGroupSelector_currentID + "_txtReference");
	txtName = GetElement(InPlaceClientSubGroupSelector_currentID + "_txtName");
	hidID = GetElement(InPlaceClientSubGroupSelector_currentID + "_hidID");

	hidID.value = id;	
	txtName.value = name;
	txtRef.value = ref;

	if (typeof (InPlaceClientSubGroupSelector_Changed) == "function") {
	    InPlaceClientSubGroupSelector_Changed(hidID.value);
	}

}
function InPlaceClientSubGroupSelector_ClearStoredID(id) {
	var txtRef, txtName, hidID;
	
	txtRef = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtRef.value = "";
	txtName.value = "";
	hidID.value = "";

	if (typeof (InPlaceClientSubGroupSelector_Changed) == "function") {
	    InPlaceClientSubGroupSelector_Changed(hidID.value);
	}
}