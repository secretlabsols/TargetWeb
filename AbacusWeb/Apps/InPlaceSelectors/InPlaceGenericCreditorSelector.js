var InPlaceGenericCreditorSelector_ButtonKey = "_btnFind";
var InPlaceGenericCreditorSelector_ReferenceKey = "_txtReference";
var InPlaceGenericCreditorSelector_DescriptionKey = "_txtName";
var InPlaceGenericCreditorSelector_HiddenIdKey = "_hidID";
var InPlaceGenericCreditorSelector_ModalDialogUrl = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/GenericCreditors.aspx";
var InPlaceGenericCreditorSelector_Selections = new Array();

function InPlaceGenericCreditorSelector_ClearStoredID(ctrlID) {

    InPlaceGenericCreditorSelector_SelectionChanged(ctrlID, null);

}

function InPlaceGenericCreditorSelector_Enable(ctrlID, enabled, clearCurrentSelection) {

    var txtName, btnFind;

    enabled = InPlaceGenericCreditorSelector_GetBoolean(enabled);
    clearCurrentSelection = InPlaceGenericCreditorSelector_GetBoolean(clearCurrentSelection);

    txtName = GetElement(ctrlID + InPlaceGenericCreditorSelector_DescriptionKey);
    btnFind = GetElement(ctrlID + InPlaceGenericCreditorSelector_ButtonKey);

    txtName.disabled = !enabled;
    btnFind.disabled = !enabled;

    if (clearCurrentSelection == true) {

        InPlaceGenericCreditorSelector_ClearStoredID(ctrlID);
        
    }

}

function InPlaceGenericCreditorSelector_FindClicked(ctrlID) {

    var currentID = InPlaceGenericCreditorSelector_GetSelectedID(ctrlID);

    // enable the query code below to allow parent pages to supply filter criteria, change the InPlaceGenericCreditorSelector_Query() objec to add query properties

//    if (typeof (InPlaceGenericCreditorSelector_GetQueryObject) == "function") {

//        var query = InPlaceGenericCreditorSelector_GetQueryObject(ctrlID);

        //if (query != undefined && query != null) {

            var url = InPlaceGenericCreditorSelector_ModalDialogUrl + "?&id=" + currentID + "&ctrlid=" + ctrlID;
            dialog = OpenDialog(url, 60, 32, window);

        //}

//    } else {

//        alert('Function InPlaceGenericCreditorSelector_GetQueryObject does not exists, please define.');

//    }
    
}

function InPlaceGenericCreditorSelector_GetSelectedID(ctrlID) {

    var idControl = GetElement(ctrlID + InPlaceGenericCreditorSelector_HiddenIdKey);
    return idControl.value;
    
}

function InPlaceGenericCreditorSelector_GetSelectedObject(ctrlID) {

    return InPlaceGenericCreditorSelector_Selections[ctrlID];

}

function InPlaceGenericCreditorSelector_GetBoolean(value) {

    if (value != undefined && (value == "true" || value == "1")) {
        return true;
    } else {
        return false;
    }
    
}

function InPlaceGenericCreditorSelector_IsEnabled(ctrlID) {

    var txtName;

    txtName = GetElement(ctrlID + InPlaceGenericCreditorSelector_DescriptionKey);

    return !txtName.disabled;

}

function InPlaceGenericCreditorSelector_SelectionChanged(ctrlID, selectedObject) {

    var txtName, hidID;

    txtReference = GetElement(ctrlID + InPlaceGenericCreditorSelector_ReferenceKey);
    txtName = GetElement(ctrlID + InPlaceGenericCreditorSelector_DescriptionKey);
    hidID = GetElement(ctrlID + InPlaceGenericCreditorSelector_HiddenIdKey);

    if (selectedObject != null) {

        txtReference.value = selectedObject.CreditorReference;
        txtName.value = selectedObject.Name;
        hidID.value = selectedObject.ID;

    } else {

        txtReference.value = "";
        txtName.value = "";
        hidID.value = "";
        
    }

    InPlaceGenericCreditorSelector_Selections[ctrlID] = selectedObject;

    if (typeof (InPlaceGenericCreditorSelector_Changed_Parent) == "function") {

        InPlaceGenericCreditorSelector_Changed_Parent(ctrlID, InPlaceGenericCreditorSelector_GetSelectedObject(ctrlID));
        
    }

}

function InPlaceGenericCreditorSelector_Query() {

//    // bool, null = both true/false
//    this.Redundant = redundant;

}



