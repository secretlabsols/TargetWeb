
var InPlaceClientSelector_currentID, InPlaceClientSelector_hideDebtorRef, InPlaceClientSelector_hideCreditorRef;

function InPlaceClientSelector_btnFind_Click(id, mode) {
    var txtReference, txtName, hidID;

    InPlaceClientSelector_currentID = id;

    txtReference = GetElement(id + "_txtReference");
    txtName = GetElement(id + "_txtName");
    hidID = GetElement(id + "_hidID");

    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/Clients.aspx?ref=" +
		txtReference.value + "&name=" + txtName.value + "&clientID=" + hidID.value + "&mode=" + mode + "&hideDebtorRef=" + InPlaceClientSelector_hideDebtorRef + "&hideCreditorRef=" + InPlaceClientSelector_hideCreditorRef;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceClient_ItemSelected(id, reference, name) {
    var txtReference, txtName, hidID;
    txtReference = GetElement(InPlaceClientSelector_currentID + "_txtReference");
    txtName = GetElement(InPlaceClientSelector_currentID + "_txtName");
    hidID = GetElement(InPlaceClientSelector_currentID + "_hidID");

    hidID.value = id;
    txtReference.value = reference;
    txtName.value = name;

    if (typeof (InPlaceClient_Changed) == "function") {
        InPlaceClient_Changed(hidID.value);
    }

    MruList_ItemSelected("SERVICE_USERS", id, reference, name);
}
function InPlaceClientSelector_ClearStoredID(id) {
    var txtReference, txtName, hidID;

    txtReference = GetElement(id + "_txtReference");
    txtName = GetElement(id + "_txtName");
    hidID = GetElement(id + "_hidID");

    txtReference.value = "";
    txtName.value = "";
    hidID.value = "";

    if (typeof (InPlaceClient_Changed) == "function") {
        InPlaceClient_Changed(hidID.value);
    }
}
function InPlaceClientSelector_Enabled(id, enable) {
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