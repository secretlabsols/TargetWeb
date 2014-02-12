var InPlaceClientThirdPartySelector_currentID;


function InPlaceClientThirdPartySelector_btnFindTP_Click(id, mode) {
	var txtName, hidID, clientSelector, selectedID; 

	InPlaceClientThirdPartySelector_currentID = id;
	txtName = GetElement(id + "_txtTPName");
	hidID = GetElement(id + "_hidID");
	clientSelector = GetElement(id + "_clientSelector_hidID");

	if(clientSelector.value == undefined || clientSelector.value == '' ) selectedID = 0;
	if (selectedID == 0){
	    alert("Please select a service user first.")
	}else{
	    selectedID = clientSelector.value
        var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/ThirdPartys.aspx?clientID=" +
            selectedID + "&thirdPartyID=" + hidID.value + "&name=" + txtName.value + "&mode=" + mode;
        var dialog = OpenDialog(url, 60, 32, window);
    }
}
function InPlaceClientThirdParty_ItemSelected(selectedID, title, surname) {
	var  txtName, hidID;

	txtName = GetElement(InPlaceClientThirdPartySelector_currentID + "_txtTPName");
	hidID = GetElement(InPlaceClientThirdPartySelector_currentID + "_hidID");

	hidID.value = selectedID;	
	txtName.value = title + ' ' + surname;
	
	//MruList_ItemSelected("SERVICE_USERS", id, reference, name);
}
function InPlaceClientThirdPartySelector_ClearStoredID(id) {
	var txtName, hidID;

	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtName.value = "";
	hidID.value = "";
}
function InPlaceClientThirdPartySelector_Enabled(id, enable) {
	var txtName, hidID, btnFind, lblTP;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	btnFind = GetElement(id + "_btnFind");
    lblTP = getElement(id + "_lblThirdParty");
	
	
	txtName.disabled = !enable;
	hidID.disabled = !enable;
	btnFind.disabled = !enable;
	lblTP.Enabled = enable;
}