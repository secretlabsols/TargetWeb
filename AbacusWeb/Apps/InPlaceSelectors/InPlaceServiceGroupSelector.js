
var InPlaceServiceGroupSelector_currentID;

function InPlaceServiceGroupSelector_btnFind_Click(id) {
	var txtName, hidID, hidFrameworkID;
	
	InPlaceServiceGroupSelector_currentID = id;
	
	txtName = GetElement(id + "_txtName");
	hidID = GetElement(id + "_hidID");
	hidFrameworkID = GetElement(id + "_hidFrameworkID");
	
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/ServiceGroups.aspx?&id=" + hidID.value  + "&fUser=1";
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceServiceGroup_ItemSelected(id, name, frameworkID) {
	var txtName, hidID, hidFrameworkID;
	txtName = GetElement(InPlaceServiceGroupSelector_currentID + "_txtName");
	hidID = GetElement(InPlaceServiceGroupSelector_currentID + "_hidID");
	hidFrameworkID =  GetElement(InPlaceServiceGroupSelector_currentID + "_hidFrameworkID");

	hidID.value = id;	
	txtName.value = name;
	hidFrameworkID.value = frameworkID;
	
	if(typeof(InPlaceServiceGroup_Changed) == "function") 
	{
	    InPlaceServiceGroup_Changed(hidFrameworkID.value, hidID.value);
    }
}
function InPlaceServiceGroupSelector_ClearStoredID(id) {
	var hidID,hidFrameworkID;
	hidID = GetElement(id + "_hidID");
	hidID.value = "";
	hidFrameworkID = GetElement(id + "_hidFrameworkID");
	hidFrameworkID.value = "";
	
	if(typeof(InPlaceServiceGroup_Changed) == "function") 
	{
	    InPlaceServiceGroup_Changed(hidFrameworkID.value, hidID.value);
    }	
}