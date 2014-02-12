
var InPlaceEstablishmentSelector_currentID;

function InPlaceEstablishmentSelector_btnFind_Click(id, mode) {
	var txtReference, txtName, hidID;
	
	InPlaceEstablishmentSelector_currentID = id;
	
	txtReference = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/Establishments.aspx?ref=" + 
		txtReference.value + "&name=" + txtName.value + "&estabID=" + hidID.value + "&mode=" + mode + "&redundant=false";
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceEstablishment_ItemSelected(id, reference, name) {
	var txtReference, txtName, hidID;
	txtReference = GetElement(InPlaceEstablishmentSelector_currentID + "_txtReference");
	txtName = GetElement(InPlaceEstablishmentSelector_currentID + "_txtName");
	hidID = GetElement(InPlaceEstablishmentSelector_currentID + "_hidID");

	hidID.value = id;	
	txtReference.value = reference;
	txtName.value = name;
	
	if(typeof(InPlaceEstablishment_Changed) == "function") {
		InPlaceEstablishment_Changed(hidID.value);
	}
	
	MruList_ItemSelected("PROVIDERS", id, reference, name);
}
function InPlaceEstablishmentSelector_ClearStoredID(id) {
	var txtReference, txtName, hidID;
	
	txtReference = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtReference.value = "";
	txtName.value = "";
	hidID.value = "";
	
	if(typeof(InPlaceEstablishment_Changed) == "function") {
		InPlaceEstablishment_Changed(hidID.value);
	}
}
function InPlaceEstablishmentSelector_Enabled(id, enable) {
	var txtNumber, txtTitle, btnFind, hidID;
	
	txtReference = GetElement(id + "_txtReference");
	txtName = GetElement(id + "_txtName");
	btnFind = GetElement(id + "_btnFind");
	hidID = GetElement(id + "_hidID");
	
	txtReference.disabled = !enable;
	txtName.disabled = !enable;
	btnFind.disabled = !enable;
	hidID.disabled = !enable;
}