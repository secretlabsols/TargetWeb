
var InPlaceTeamSelector_currentID;

function InPlaceTeamSelector_btnFind_Click(id, availableToRes, availableToDom) {
	var txtRef, txtName, hidID;
	
	InPlaceTeamSelector_currentID = id;
	
	txtRef = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/Teams.aspx?&id=" + 
		hidID.value + "&availableToRes=" + availableToRes + "&availableToDom=" + availableToDom;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceTeam_ItemSelected(id, name, ref) {
	var txtRef, txtName, hidID;
	txtRef = GetElement(InPlaceTeamSelector_currentID + "_txtReference");
	txtName = GetElement(InPlaceTeamSelector_currentID + "_txtName");
	hidID = GetElement(InPlaceTeamSelector_currentID + "_hidID");

	hidID.value = id;	
	txtName.value = name;
	txtRef.value = ref;
}
function InPlaceTeamSelector_ClearStoredID(id) {
	var txtRef, txtName, hidID;
	
	txtRef = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtRef.value = "";
	txtName.value = "";
	hidID.value = "";
}