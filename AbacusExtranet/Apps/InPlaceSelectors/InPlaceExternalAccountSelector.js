var txtExternalAccount, txtHidExternalAccountID, InPlaceExternalAccount_currentID, InPlaceExternalAccount_HiddenIdKey = "_txtHidExternalAccountID";

function InPlaceExternalAccountSelector_btnFind_Click(id) {
    var txtReference, txtExternalAccount, hidID;
    InPlaceExternalAccount_currentID = id;

    txtExternalAccount = GetElement(id + "_txtExternalAccount");

    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/InPlaceSelectors/ExternalAccounts.aspx?id=" + InPlaceExternalAccountSelector_GetSelectedID(id);
    var dialog = OpenDialog(url, 70, 32, window);
}

function InPlaceExternalAccountSelector_GetSelectedID(ctrlID) {

    var idControl = GetElement(ctrlID + InPlaceExternalAccount_HiddenIdKey);
    return parseInt(idControl.value);

}

function InPlaceExternalAccount_ItemSelected(id, ExternalAccount) {
  
    //InPlaceExpenditureAccountSelector_currentID
    txtExternalAccount = GetElement(InPlaceExternalAccount_currentID + "_txtExternalAccount");
    txtExternalAccount.value = ExternalAccount;
//    txtName = GetElement(InPlaceEstablishmentSelector_currentID + "_txtName");
    txtHidExternalAccountID = GetElement(InPlaceExternalAccount_currentID + InPlaceExternalAccount_HiddenIdKey);
    txtHidExternalAccountID.value = id;
}

function InPlaceExternalAccountSelector_ClearStoredID(id) {
    InPlaceExternalAccount_currentID = id;
    InPlaceExternalAccount_ItemSelected("", "");
}