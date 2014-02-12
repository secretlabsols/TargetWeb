
var InPlaceOtherFundingOrganizationSelector_currentID;
//var orgType;

function InPlaceOtherFundingOrganizationSelector_btnFind_Click(id, orgType) {
	var txtReference, txtName, hidID;
	InPlaceOtherFundingOrganizationSelector_currentID = id;
	if (orgType != 4 && orgType != 5) {
	    //Should never get this message, more a message to the user.
	    alert("This control does not support this Organisation type");
	}else{
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/OtherFundingOrganisations.aspx?name=" + 
		txtName.value + "&orgID=" + hidID.value + "&orgType=" + orgType ;
    var dialog = OpenDialog(url, 60, 32, window);
    }
}
function InPlaceOtherFundingOrganizationSelector_ItemSelected(id, name) {
	var txtReference, txtName, hidID;
	txtName = GetElement(InPlaceOtherFundingOrganizationSelector_currentID + "_txtName");
	hidID = GetElement(InPlaceOtherFundingOrganizationSelector_currentID + "_hidID");

	hidID.value = id;	
	txtName.value = name;
}
function InPlaceOtherFundingOrganizationSelector_ClearStoredID(id) {
	var  txtName, hidID;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	
	txtName.value = "";
	hidID.value = "";
}
function InPlaceOtherFundingOrganizationSelector_Enabled(id, enable) {
	var txtReference, txtName, hidID, btnFind;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	btnFind = GetElement(id + "_btnFind");
	
	txtName.disabled = !enable;
	hidID.disabled = !enable;
	btnFind.disabled = !enable;
}