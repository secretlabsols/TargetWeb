
var Edit_domContractID, cboContractTypeID, cboContractGroupID, chkCreateProformaID, chkCreateProviderID;
var selectedProviderID = 0, selectedContractID = 0;
var chkCreateProforma, chkCreateProvider;

function Init() {
    chkCreateProforma = GetElement(chkCreateProformaID + "_chkCheckbox");
    chkCreateProvider = GetElement(chkCreateProviderID + "_chkCheckbox");
    chkCreateProforma_Click();
}

function InPlaceEstablishment_Changed(newID) {
	InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
	InPlaceDomContractSelector_providerID = newID;
	selectedProviderID = newID;
}
function cboContractType_Change() {
    InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
    InPlaceDomContractSelector_contractTypeID = GetElement(cboContractTypeID + "_cboDropDownList").value;
}
function cboContractGroup_Change() {
    InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
    InPlaceDomContractSelector_contractGroupID = GetElement(cboContractGroupID + "_cboDropDownList").value;
}
function InPlaceDomContract_Changed(newID) {
    selectedContractID = newID;
}
function chkCreateProforma_Click() {
    if(chkCreateProforma.checked) {
        chkCreateProvider.disabled = false;
    } else {
        chkCreateProvider.disabled = true;
        chkCreateProvider.checked = false;
    }
    // Disable create provider invoice option
    // see http://sharepoint:18085/personal/mikevo/Lists/Electronic%20Monitoring%20Issues/DispForm.aspx?ID=71
    chkCreateProvider.checked = false;
    chkCreateProvider.disabled = true;
}

addEvent(window, "load", Init);
