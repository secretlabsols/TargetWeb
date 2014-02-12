
function rdoSetting_OnClick(userID, userFullname, defaultSettings) {
	var hidDefaultSettings = GetElement("hidDefaultSettings" + userID);
	var rdoDefault = GetElement("rdoDefault" + userID);
	var rdoCustom = GetElement("rdoCustom" + userID);
	var result;
	
	// changing to custom
	if(hidDefaultSettings.value == "True" && !defaultSettings) {
		result = window.confirm("This action will create a copy of the default settings for this user that can then be customised using the 'Edit' button. Are you sure you wish to continue?");
		if(result) {
			btnEdit_OnClick(userID, userFullname);
		} else {
			rdoDefault.checked = true;
		}
	}
	// changing to default
	if(hidDefaultSettings.value == "False" && defaultSettings) {
		result = window.confirm("This action will delete the custom settings for this user so that they will now use the default settings. Are you sure you wish to continue?");
		if(result) {
			document.location.href = "ManageEditableFields.aspx?userID=" + userID + "&forceDefault=1";
		} else {
			rdoCustom.checked = true;
		}
	}
}
function btnEdit_OnClick(userID, userFullname) {
	document.location.href = "ChangeEditableFieldSettings.aspx?userID=" + userID + "&fullName=" + escape(userFullname);
}