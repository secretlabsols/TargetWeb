
var stdButtons_SelectedItemID, stdButtons_hidSelectedItemID, stdButtons_auditLogTableName;

function StdButtons_OnDeleteClicked() {
    return window.confirm('Are you sure you wish to delete this record?');
}
function StdButtons_OnFindClicked(cboSearchByID, txtSearchForID, hidSelectedItemID, enmGenericFinderTypeID, extraParams) {
        
    var cboSearchBy = document.getElementById(cboSearchByID);
    var txtSearchFor = document.getElementById(txtSearchForID);
    stdButtons_hidSelectedItemID = hidSelectedItemID;
    
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "Apps/GenericFinder/GenericFinder.aspx?gftID=" + enmGenericFinderTypeID +
        "&searchBy=" + cboSearchBy.value + "&searchFor=" + txtSearchFor.value;
    // add extra params
    for(index=0; index<extraParams.length; index++) {
		url += "&param=" + extraParams[index];
    }
    var dialog = OpenDialog(url, 75, 32, window);
	return false;
}
function StdButtons_ItemSelected(selectedItemID) {
	stdButtons_SelectedItemID = selectedItemID;
    // save selected ID in hidden field for submission
    if(stdButtons_SelectedItemID > 0) {
		document.getElementById(stdButtons_hidSelectedItemID).value = stdButtons_SelectedItemID;
		// manual postback
		StdButtons_DoFindPostBack();
    }
}
function StdButtons_GetSelectedItemID(stdButtonsID) {
	return GetElement(stdButtonsID + "_hidSelectedItemID").value;
}
function StdButtons_btnAudit_Click(stdButtonsID, UseApplicationFilter, auditLogTableName) {
    var url = SITE_VIRTUAL_ROOT +
	"Apps/AuditLog/Popup.aspx?tbl=" + auditLogTableName + // + stdButtons_auditLogTableName +
	"&id=" + StdButtons_GetSelectedItemID(stdButtonsID) +
	"&UseApplicationFilter=" + UseApplicationFilter;

    OpenPopup(url, 60, 46, 1);
}