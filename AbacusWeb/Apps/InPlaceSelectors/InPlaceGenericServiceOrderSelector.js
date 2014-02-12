
var InPlaceGenericServiceOrderSelector_currentID;

function InPlaceGenericServiceOrderSelector_btnFind_Click(id, mode) {
    var txtReference, hidID, hidClientID;

    InPlaceGenericServiceOrderSelector_currentID = id;

    txtReference = GetElement(id + "_txtReference");
    hidID = GetElement(id + "_hidID");
    hidClientID = GetElement(id + "_hidClientID");

    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/GenericServiceOrders.aspx?ref=" +
		txtReference.value + "&genericServiceOrderID=" + hidID.value + "&clientID=" + hidClientID.value + "&mode=" + mode;
    var dialog = OpenDialog(url, 60, 32, window);
}
function InPlaceGenericServiceOrderSelector_ItemSelected(genericServiceOrderID, childID, reference) {
    var txtReference,  hidID;
    txtReference = GetElement(InPlaceGenericServiceOrderSelector_currentID + "_txtReference");
    hidID = GetElement(InPlaceGenericServiceOrderSelector_currentID + "_hidID");

    hidID.value = genericServiceOrderID;
    txtReference.value = reference;

    if (typeof (InPlaceGenericServiceOrderSelector_Changed) == "function") {
        InPlaceGenericServiceOrderSelector_Changed(hidID.value);
    }

    MruList_ItemSelected("SERVICE_ORDERS", childID, reference);
}
function InPlaceGenericServiceOrderSelector_ClearStoredID(id) {
    var txtReference, hidID;

    txtReference = GetElement(id + "_txtReference");
    hidID = GetElement(id + "_hidID");

    txtReference.value = "";
    hidID.value = "";

    if (typeof (InPlaceGenericServiceOrderSelector_Changed) == "function") {
        InPlaceGenericServiceOrderSelector_Changed(hidID.value);
    }
}
function InPlaceGenericServiceOrderSelector_Enabled(id, enable) {
    var txtReference, btnFind, hidID;

    txtReference = GetElement(id + "_txtReference");
    btnFind = GetElement(id + "_btnFind");
    hidID = GetElement(id + "_hidID");

    txtReference.disabled = !enable;
    btnFind.disabled = !enable;
    hidID.disabled = !enable;
}