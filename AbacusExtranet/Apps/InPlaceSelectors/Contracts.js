var btnSelect, contractId, contractReference, contractName, frameworkTypeAbbr;

function Init() {
    btnSelect = GetElement("btnSelect");
    if (DomContractSelector_selectedContractID == 0) btnSelect.disabled = true;
}
function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}
function btnSelect_Click() {
    //alert(contractId + "/" + contractReference + "/" + contractName)
    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    parentWindow.InPlaceDomContractSelector_ItemSelected(contractId, contractReference, contractName, frameworkTypeAbbr);
    window.parent.close();
}
function DomContractSelector_SelectedItemChange(args) {
    contractId = args[0];
    contractReference = args[1];
    contractName = args[2];
    frameworkTypeAbbr = args[3].FrameworkTypeAbbreviation;
    if (contractId != 0) btnSelect.disabled = false;
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);