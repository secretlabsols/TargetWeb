var externalAccountClientId, dcrId, externalAccountId;

function btnRemoveRUB_Click() {
    return window.confirm("Are you sure you wish to remove this contract?");
}

function btnRemove_Click() {
    return window.confirm("Are you sure you wish to remove this Rounding detail?");
}

function InPlaceDcrDomContractSelector_GetQueryObject(id) {

    return {
        externalAccountId: externalAccountId,
        dcrId: dcrId
    }   
    
}