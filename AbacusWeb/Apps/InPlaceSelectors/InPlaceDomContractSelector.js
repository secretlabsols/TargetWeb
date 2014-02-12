
var InPlaceDomContractSelector_currentID, InPlaceDomContractSelector_providerID = 0;
var InPlaceDomContractSelector_contractTypeID = 0, InPlaceDomContractSelector_contractGroupID = 0;
var ServiceGroupClassificationID, FrameworkTypeID;

function InPlaceDomContractSelector_btnFind_Click(id) {
	var txtNumber, txtTitle, hidID;
	
	InPlaceDomContractSelector_currentID = id;
	
	txtNumber = GetElement(id + "_txtNumber");
	txtTitle = GetElement(id + "_txtTitle");
	hidID = GetElement(id + "_hidID");
	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + 
        "AbacusWeb/Apps/InPlaceSelectors/DomContracts.aspx?&contractID=" + hidID.value + 
        "&estabID=" + InPlaceDomContractSelector_providerID +
        "&ctID=" + InPlaceDomContractSelector_contractTypeID +
        "&cgID=" + InPlaceDomContractSelector_contractGroupID +
        "&serviceGroupClassificationID=" + ServiceGroupClassificationID +
        "&ftid=" + FrameworkTypeID;
		
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceDomContract_ItemSelected(id, number, title) {
	var txtNumber, txtTitle, hidID;
	txtNumber = GetElement(InPlaceDomContractSelector_currentID + "_txtNumber");
	txtTitle = GetElement(InPlaceDomContractSelector_currentID + "_txtTitle");
	hidID = GetElement(InPlaceDomContractSelector_currentID + "_hidID");

	hidID.value = id;	
	txtNumber.value = number;
	txtTitle.value = title;
	
	if(typeof(InPlaceDomContract_Changed) == "function") {
		InPlaceDomContract_Changed(hidID.value);
	}
	
	MruList_ItemSelected("DOM_CONTRACTS", id, number, title);
}
function InPlaceDomContractSelector_ClearStoredID(id) {
    var txtNumber, txtTitle, hidID;
	
	txtNumber = GetElement(id + "_txtNumber");
	txtTitle = GetElement(id + "_txtTitle");
	hidID = GetElement(id + "_hidID");
	
	txtNumber.value = "";
	txtTitle.value = "";
	hidID.value = "";
	
	if(typeof(InPlaceDomContract_Changed) == "function") {
		InPlaceDomContract_Changed(hidID.value);
	}
}
function InPlaceDomContractSelector_Enabled(id, enable) {
    var txtNumber, txtTitle, btnFind, hidID, hidIDVal;
	
	txtNumber = GetElement(id + "_txtNumber");
	txtTitle = GetElement(id + "_txtTitle");
	btnFind = GetElement(id + "_btnFind");
	hidID = GetElement(id + "_hidID");
	hidIDVal = GetElement(id + "_valRequired", true);
	
	txtNumber.disabled = !enable;
	txtTitle.disabled = !enable;
	btnFind.disabled = !enable;
	hidID.disabled = !enable;
	if (hidIDVal) {
	    hidIDVal.enabled = true;
	    ValidatorUpdateDisplay(hidIDVal);
	}	

}