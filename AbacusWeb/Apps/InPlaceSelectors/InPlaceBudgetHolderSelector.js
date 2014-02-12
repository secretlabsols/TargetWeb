
var InPlaceBudgetHolderSelector_currentID;

function InPlaceBudgetHolderSelector_btnFind_Click(id) {
    var txtRef, txtName, hidID, hidRedundant, hidServiceUser;

    InPlaceBudgetHolderSelector_currentID = id;

    txtRef = GetElement(id + "_txtReference");
    txtName = GetElement(id + "_txtName");
    hidID = GetElement(id + "_hidID");
    hidRedundant = GetElement(id + "_hidRedundant");
    hidServiceUser = GetElement(id + "_hidServiceUser");

    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/BudgetHolders.aspx?ref=" +
		txtRef.value + "&name=" + txtName.value + "&id=" + hidID.value + "&redundant=" + hidRedundant.value + "&suid=" + hidServiceUser.value;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceBudgetHolder_ItemSelected(id, name, ref) {
    var txtRef, txtName, hidID;
    txtRef = GetElement(InPlaceBudgetHolderSelector_currentID + "_txtReference");
    txtName = GetElement(InPlaceBudgetHolderSelector_currentID + "_txtName");
    hidID = GetElement(InPlaceBudgetHolderSelector_currentID + "_hidID");

    hidID.value = id;
    txtName.value = name;
    txtRef.value = ref;
}
function InPlaceBudgetHolderSelector_ClearStoredID(id) {
    var txtRef, txtName, hidID;

    txtRef = GetElement(id + "_txtReference");
    txtName = GetElement(id + "_txtName");
    hidID = GetElement(id + "_hidID");

    txtRef.value = "";
    txtName.value = "";
    hidID.value = "";
}
function InPlaceBudgetHolderSelector_Enabled(id, enable) {
    var txtReference, txtName, btnFind, hidID;

    txtReference = GetElement(id + "_txtReference");
    txtName = GetElement(id + "_txtName");
    btnFind = GetElement(id + "_btnFind");
    hidID = GetElement(id + "_hidID");

    txtReference.disabled = !enable;
    txtName.disabled = !enable;
    btnFind.disabled = !enable;
    hidID.disabled = !enable;
}
function InPlaceBudgetHolderSelector_SetFindCriteria(id, clientID) {
    var hidServiceUser = GetElement(id + "_hidServiceUser");

    hidServiceUser.value = clientID;
}