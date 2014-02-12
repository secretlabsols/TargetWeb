var InPlaceDcrDomContractSelector_HiddenIdKey = '_txtHidselectedDcrDomContractId';

function InPlaceDcrDomContractSelector_btnFind_Click(id) {

    InPlaceDcrDomContractSelector_currentID = id;

    if (typeof (InPlaceDcrDomContractSelector_GetQueryObject) == "function") {

        var query = InPlaceDcrDomContractSelector_GetQueryObject(id);

        if (query) {

            var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/InPlaceSelectors/DcrContractSelector.aspx?qs_externalAccount=" + query.externalAccountId + "&dcrId=" + query.dcrId + "&cId=" + InPlaceDcrDomContractSelector_GetSelectedID(id);
            var dialog = OpenDialog(url, 70, 32, window);
            
        }

    } else {

        alert('Function InPlaceDcrDomContractSelector_GetQueryObject does not exist, please define.');

    }

}

function InPlaceDcrDomContractSelector_ClearStoredID(ctrlID) {

    InPlaceDcrDomContractSelector_currentID = ctrlID;
    InPlaceDcrDomContractSelector_ItemSelected(0, "", "");

}

function InPlaceDcrDomContractSelector_GetSelectedID(ctrlID) {

    var idControl = GetElement(ctrlID + InPlaceDcrDomContractSelector_HiddenIdKey);
    return parseInt(idControl.value);

}

function InPlaceDcrDomContractSelector_ItemSelected(selectedDcrDomContractId, selectedDcrDomContractTitle, selectedDcrDomContractNumber) {

    var txtContractNumber, txtContractTitle, txtHidselectedDcrDomContractId;

    txtHidselectedDcrDomContractId = GetElement(InPlaceDcrDomContractSelector_currentID + InPlaceDcrDomContractSelector_HiddenIdKey);
    txtHidselectedDcrDomContractId.value = selectedDcrDomContractId;

    txtContractNumber = GetElement(InPlaceDcrDomContractSelector_currentID + "_txtContractNumber");
    txtContractNumber.value = selectedDcrDomContractNumber;

    txtContractTitle = GetElement(InPlaceDcrDomContractSelector_currentID + "_txtContractTitle");
    txtContractTitle.value = selectedDcrDomContractTitle;
}

