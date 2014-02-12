
var InPlaceCareManagerSelector_currentID;

function InPlaceCareManagerSelector_btnFind_Click(id) {
	var txtRef, txtName, hidID;
	
	InPlaceCareManagerSelector_currentID = id;
	
	txtRef = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/CareManagers.aspx?ref=" +
		txtRef.value + "&name=" + txtName.value + "&id=" + hidID.value;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceCareManager_ItemSelected(id, name, ref) {
	var txtRef, txtName, hidID;
	txtRef = GetElement(InPlaceCareManagerSelector_currentID + "_txtReference");
	txtName = GetElement(InPlaceCareManagerSelector_currentID + "_txtName");
	hidID = GetElement(InPlaceCareManagerSelector_currentID + "_hidID");

	hidID.value = id;	
	txtName.value = name;
	txtRef.value = ref;
}
function InPlaceCareManagerSelector_ClearStoredID(id) {
	var txtRef, txtName, hidID;
	
	txtRef = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtRef.value = "";
	txtName.value = "";
	hidID.value = "";
}