var InPlaceContractSelector_currentID, InPlaceContractSelector_SelectedContractID;

function InPlaceDomContractSelector_Init(id) {
    if (id && id.toString && id.toString().indexOf("ctl00") === 0) {
        var txtContractReference, txtContractName, hidContractID, txtHidFrameworkTypeAbbr;
        txtContractReference = GetElement(id + "_txtContractNumber");
        txtContractName = GetElement(id + "_txtContractTitle");
        hidContractID = GetElement(id + "_txtHidselectedDomContractId");
        txtHidFrameworkTypeAbbr = GetElement(id + "_txtHidFrameworkTypeAbbr");
        InPlaceContractSelector_currentID = id;
        InPlaceDomContractSelector_ItemSelected(hidContractID.value, txtContractReference.value, txtContractName.value, txtHidFrameworkTypeAbbr.value);
    }
}

function InPlaceDomContractSelector_btnFind_Click(id) {
    var txtReference, txtExternalAccount, hidID;
    InPlaceContractSelector_currentID = id;

    txtContractNumber = GetElement(id + "_txtContractNumber");
    txtContractTitle = GetElement(id + "_txtContractTitle");

    InPlaceContractSelector_currentID = id;
    // need to apply filters on DcrContractSelector nased on the External Account.
    //externalAccountClientId is being populated in the page load TargetWeb\AbacusExtranet\Apps\Dom\ReferenceData\DurationClaimedRoundingRules.aspx.vb
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/InPlaceSelectors/Contracts.aspx?estabid=" + SelectedProviderId + "&cid=" + InPlaceContractSelector_SelectedContractID;
    var dialog = OpenDialog(url, 70, 32, window);
}

function InPlaceDomContractSelector_ItemSelected(id, reference, name, frameworkTypeAbbr) {
    var txtContractReference, txtContractName, hidContractID, txtHidFrameworkTypeAbbr;
    txtContractReference = GetElement(InPlaceContractSelector_currentID + "_txtContractNumber");
    txtContractName = GetElement(InPlaceContractSelector_currentID + "_txtContractTitle");
    hidContractID = GetElement(InPlaceContractSelector_currentID + "_txtHidselectedDomContractId");
    txtHidFrameworkTypeAbbr = GetElement(InPlaceContractSelector_currentID + "_txtHidFrameworkTypeAbbr");

    hidContractID.value = id;
    txtContractReference.value = reference;
    txtContractName.value = name;
    txtHidFrameworkTypeAbbr = frameworkTypeAbbr;
    InPlaceContractSelector_SelectedContractID = id;

    if (typeof (InPlaceContract_Changed) == "function") {
        InPlaceContract_Changed(id, reference, name, frameworkTypeAbbr);
    }
}

function InPlaceDomContractSelector_ClearStoredID(id) {

    var txtContractReference, txtContractName, hidContractID, txtHidFrameworkTypeAbbr;

    txtContractReference = GetElement(id + "_txtContractNumber");
    txtContractName = GetElement(id + "_txtContractTitle");
    hidContractID = GetElement(id + "_txtHidselectedDomContractId");
    txtHidFrameworkTypeAbbr = GetElement(id + "_txtHidFrameworkTypeAbbr");

    hidContractID.value = 0;
    InPlaceContractSelector_SelectedContractID = 0;
    txtContractReference.value = '';
    txtContractName.value = '';
    txtHidFrameworkTypeAbbr.value = '';

    if (typeof (InPlaceContract_Changed) == "function") {
        InPlaceContract_Changed(0, '', '', '');
    }
}

function InPlaceDomContractSelector_Disable(id, disabled) {

    var txtContractReference, txtContractName, btnFind;

    txtContractReference = GetElement(id + "_txtContractNumber");
    txtContractName = GetElement(id + "_txtContractTitle");
    btnFind = GetElement(id + "_btnFind");
    
    txtContractReference.disabled = disabled;
    txtContractName.disabled = disabled;
    btnFind.disabled = disabled;

}

addEvent(window, "load", InPlaceDomContractSelector_Init);