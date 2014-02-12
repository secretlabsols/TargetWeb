
var Edit_domContractID, cboContractTypeID, cboContractGroupID;
var batchTypeIDs, selectedProviderID = 0, selectedContractID = 0;
var dataPartitioned, intWebSecurityUserID;

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
function btnProformaInvoice_Click() {
    var selectedTypes = GetSelectedTotal(batchTypeIDs, "chkBatchType");
    var providerID = selectedProviderID;
    var contractTypeID = InPlaceDomContractSelector_contractTypeID;
    var contractGroupID = InPlaceDomContractSelector_contractGroupID;
    var contractID = selectedContractID;
    
    if(selectedTypes == 0) {
        alert("Please select at least one Batch Type.");
        return;
    }
    
    var url = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Jobs/PreviewDomProformaInvoices.aspx" +
        "?batchTypes=" + selectedTypes +
        "&providerID=" + providerID +
        "&ctID=" + contractTypeID +
        "&cgID=" + contractGroupID +
        "&contractID=" + contractID +
        "&dataPartitioned=" + dataPartitioned +
        "&intWebSecurityUserID=" + intWebSecurityUserID;
  
    OpenPopup(url, 75, 50, 1);
}
function GetSelectedTotal(idArray, idPrefix) {
    var result = 0;
    for(index=0; index<idArray.length; index++) {
        var id = idArray[index] + "_chkCheckbox";
        var chk = GetElement(id);
        if(chk.checked) {
            id = id.substr(id.indexOf(idPrefix, 0));
            id = id.replace(idPrefix, "");
            id = id.replace("_chkCheckbox", "");
            id = id.replace("_", "");
            var value = parseInt(id, 10);
            result += value;
        }
    }
    return result;
}