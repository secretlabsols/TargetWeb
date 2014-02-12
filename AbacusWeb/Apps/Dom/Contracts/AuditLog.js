
function btnShow_Click() {
	var contractID = GetQSParam(document.location.search, "contractID");
	var cboTableName = GetElement("cboTableName_cboDropDownList");
	var dteDateFrom = GetElement("dteDateFrom_txtTextBox");
	var dteDateTo = GetElement("dteDateTo_txtTextBox");
    var url = SITE_VIRTUAL_ROOT + "Apps/AuditLog/Popup.aspx?id=" + contractID + 
		"&tbl=" + cboTableName.value + "&dateFrom=" + dteDateFrom.value + "&dateTo=" + dteDateTo.value;
	OpenPopup(url, 60, 46, 1);
}