var hidEndReasonID, hidContractGroupID, budgetholderID, selectedClientID, selectedID;

function Init() {
    if (selectedID <= 0) {
        InPlaceClient_Changed(selectedClientID);
    }
}

function tabStrip_ActiveTabChanged(sender, args) {
	var selectedTabIndex = sender.get_activeTabIndex();
	var hidSelectedTabIndex = GetElement("hidSelectedTabIndex");
	hidSelectedTabIndex.value = selectedTabIndex;
}

function btnBack_Click() {
}

function RemoveIFrame(frameUID) {
    var iFrame = document.getElementById(frameUID);
    var parentDiv, hr, btnAdd;

    if (iFrame) {
        parentDiv = iFrame.parentNode;
        hr = iFrame.nextSibling;
    }

    if (parentDiv) btnAdd = parentDiv.getElementsByTagName("input")[0];
    if (hr) parentDiv.removeChild(hr);
    if (parentDiv) parentDiv.removeChild(iFrame);
    if (btnAdd) btnAdd.disabled = false;
}

function EnableAddButton(frameUID) {
    var iFrame = document.getElementById(frameUID);
    var parentDiv, btnAdd;

    if (iFrame) {
        parentDiv = iFrame.parentNode;
    }

    if (parentDiv) btnAdd = parentDiv.getElementsByTagName("input")[0];
    if (btnAdd) btnAdd.disabled = false;
}

function EditPeriod_SaveClicked(frameUID) {
    EnableAddButton(frameUID);
}

function EditPeriod_AfterSuccessfulSave(warning) {
    var newUrl = document.location.href;
    newUrl = AddQSParam(RemoveQSParam(newUrl, "tabid"), "tabid", 1);
    if (warning) {
        alert(warning);
    }
    document.location.href = newUrl;
}

function EditPeriod_CancelClicked(frameUID) {
    RemoveIFrame(frameUID);
}

function EditPeriod_DeleteClicked(frameUID) {
    RemoveIFrame(frameUID);
}

function EditPayment_SaveClicked(frameUID) {
    EnableAddButton(frameUID);
}

function EditPayment_CancelClicked(frameUID) {
    RemoveIFrame(frameUID);
}

function EditPayment_DeleteClicked(frameUID) {
    RemoveIFrame(frameUID);
}

function EditPayment_ReconsiderClicked() {
    var newUrl = document.location.href;
    newUrl = AddQSParam(RemoveQSParam(newUrl, "tabid"), "tabid", 2);

    document.location.href = newUrl;
}

function cboEndReason_Change(cboID, hID) {
    var cboEndReason = GetElement(cboID + "_cboDropDownList");
    var hidEndReason = GetElement(hID);
    hidEndReason.value = cboEndReason.value;
}

function cboContractGroup_Change(cboID, hID) {
    var cboContractGroup = GetElement(cboID + "_cboDropDownList");
    var hidContractGroup = GetElement(hID);
    hidContractGroup.value = cboContractGroup.value;
}

function InPlaceClient_Changed(id) {
    var hasId;
    id = parseInt((id || 0));
    hasId = (id > 0);
    InPlaceBudgetHolderSelector_ClearStoredID(budgetholderID);
    InPlaceBudgetHolderSelector_Enabled(budgetholderID, hasId);
    InPlaceBudgetHolderSelector_SetFindCriteria(budgetholderID, id);
}

addEvent(window, "load", Init);