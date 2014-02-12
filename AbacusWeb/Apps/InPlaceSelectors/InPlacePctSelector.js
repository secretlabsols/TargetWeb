
var InPlacePctSelector_currentID;

function InPlacePctSelector_btnFind_Click(id) {
	var txtName, hidID;
	
	InPlacePctSelector_currentID = id;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/Pcts.aspx?name=" + 
		txtName.value + "&id=" + hidID.value;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlacePct_ItemSelected(id, name) {
	var txtName, hidID;
	txtName = GetElement(InPlacePctSelector_currentID + "_txtName");
	hidID = GetElement(InPlacePctSelector_currentID + "_hidID");

	hidID.value = id;	
	txtName.value = name;
}
function InPlacePctSelector_ClearStoredID(id) {
	var txtName, hidID;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtName.value = "";
	hidID.value = "";
}