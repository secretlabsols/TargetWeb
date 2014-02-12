var InPlaceServiceTypeSelector_ButtonKey = "_btnFind";
var InPlaceServiceTypeSelector_DescriptionKey = "_txtName";
var InPlaceServiceTypeSelector_HiddenIdKey = "_hidID";
var InPlaceServiceTypeSelector_ModalDialogUrl = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/ServiceTypes.aspx";
var InPlaceServiceTypeSelector_Selections = new Array();

function InPlaceServiceTypeSelector_ClearStoredID(ctrlID) {

    InPlaceServiceTypeSelector_SelectionChanged(ctrlID, null);

}

function InPlaceServiceTypeSelector_Enable(ctrlID, enabled, clearCurrentSelection) {

    var txtName, btnFind;

    enabled = InPlaceServiceTypeSelector_GetBoolean(enabled);
    clearCurrentSelection = InPlaceServiceTypeSelector_GetBoolean(clearCurrentSelection);

    txtName = GetElement(ctrlID + InPlaceServiceTypeSelector_DescriptionKey);
    btnFind = GetElement(ctrlID + InPlaceServiceTypeSelector_ButtonKey);

    txtName.disabled = !enabled;
    btnFind.disabled = !enabled;

    if (clearCurrentSelection == true) {

        InPlaceServiceTypeSelector_ClearStoredID(ctrlID);
        
    }

}

function InPlaceServiceTypeSelector_FindClicked(ctrlID) {
    
    var currentID = InPlaceServiceTypeSelector_GetSelectedID(ctrlID);

    if (typeof (InPlaceServiceTypeSelector_GetQueryObject) == "function") {

        var query = InPlaceServiceTypeSelector_GetQueryObject(ctrlID);

        if (query != undefined && query != null) {

            var dialog;
            var excludeIds = "";
            var excludeServiceCategories = "";
            var includeIds = "";
            var redundant = "";
            var splitChar = ",";
            var url = "";

            excludeIds = InPlaceServiceTypeSelector_GetTokenSeparatedStringFromArray(query.ExcludeIds, splitChar);
            excludeServiceCategories = InPlaceServiceTypeSelector_GetTokenSeparatedStringFromArray(query.ExcludeServiceCategories, splitChar);
            includeIds = InPlaceServiceTypeSelector_GetTokenSeparatedStringFromArray(query.IncludeIds, splitChar);

            if (query.Redundant != undefined && query.Redundant != null) {
                redundant = query.Redundant;
            }

            url = InPlaceServiceTypeSelector_ModalDialogUrl + "?&id=" + currentID + "&ctrlid=" + ctrlID + "&redundant=" + redundant + "&incIds=" + includeIds + "&exIds=" + excludeIds + "&exSvcCats=" + excludeServiceCategories;
            dialog = OpenDialog(url, 60, 32, window);

        }

    } else {

        alert('Function InPlaceServiceTypeSelector_GetQueryObject does not exists, please define.');

    }
    
}

function InPlaceServiceTypeSelector_GetDefaultObject(ctrlID) {

    var defaultObject = new InPlaceServiceTypeSelector_ServiceType();
    defaultObject.ID = 0;
    defaultObject.ServiceCategoryDescription = "";
    defaultObject.ServiceCategoryID = 0;
    defaultObject.ServiceClassificationGroupDescription = "";
    defaultObject.ServiceClassificationGroupID = 0;
    defaultObject.ServiceGroupDescription = "";
    defaultObject.ServiceGroupID = 0;
    defaultObject.ServiceTypeDescription = "";
    defaultObject.Redundant = false;
    defaultObject.Permanent = false;
    //defaultObject.VisitBased = false;
    return defaultObject;
    
}

function InPlaceServiceTypeSelector_GetSelectedID(ctrlID) {

    var idControl = GetElement(ctrlID + InPlaceServiceTypeSelector_HiddenIdKey);
    return idControl.value;
    
}

function InPlaceServiceTypeSelector_GetSelectedObject(ctrlID) {

    var selectedObject = InPlaceServiceTypeSelector_Selections[ctrlID];

    if (selectedObject == undefined || selectedObject == null) {

        selectedObject = InPlaceServiceTypeSelector_GetDefaultObject(ctrlID);
        
    }

    return selectedObject;

}

function InPlaceServiceTypeSelector_GetBoolean(value) {

    if (value != undefined && (value == "true" || value == "1")) {
        return true;
    } else {
        return false;
    }
    
}

function InPlaceServiceTypeSelector_GetInteger(value) {

    if (value != undefined && value.length > 0 && isNaN(value) == false) {
        return parseInt(value);
    } else {
        return 0;
    }

}

function InPlaceServiceTypeSelector_GetTokenSeparatedStringFromArray(tokenizedArray, token) {

    var tokenResult = "";

    if (tokenizedArray != undefined && tokenizedArray != null && tokenizedArray.length > 0) {
        var tokenizedArrayLength = tokenizedArray.length;
        for (var i = 0; i < tokenizedArrayLength; i++) {
            if (tokenResult.length > 0) {
                tokenResult += token;
            }
            tokenResult += tokenizedArray[i];
        }
    }

    return tokenResult;
}

function InPlaceServiceTypeSelector_IsEnabled(ctrlID) {

    var txtName;

    txtName = GetElement(ctrlID + InPlaceServiceTypeSelector_DescriptionKey);

    return !txtName.disabled;

}

function InPlaceServiceTypeSelector_SelectionChanged(ctrlID, selectedObject) {

    var storedObject = new InPlaceServiceTypeSelector_ServiceType();
    var txtName, hidID;

    txtName = GetElement(ctrlID + InPlaceServiceTypeSelector_DescriptionKey);
    hidID = GetElement(ctrlID + InPlaceServiceTypeSelector_HiddenIdKey);

    if (selectedObject != null) {
    
        storedObject.ID = selectedObject.ID;
        storedObject.ServiceCategoryDescription = selectedObject.ServiceCategoryDescription;
        storedObject.ServiceCategoryID = selectedObject.ServiceCategoryID;
        storedObject.ServiceClassificationGroupDescription = selectedObject.ServiceClassificationGroupDescription;
        storedObject.ServiceClassificationGroupID = selectedObject.ServiceClassificationGroupID;
        storedObject.ServiceGroupDescription = selectedObject.ServiceGroupDescription;
        storedObject.ServiceGroupID = selectedObject.ServiceGroupID;
        storedObject.ServiceTypeDescription = selectedObject.ServiceTypeDescription;
        storedObject.Redundant = selectedObject.Redundant;
        storedObject.Permanent = selectedObject.Permanent;
        
        txtName.value = selectedObject.ServiceTypeDescription;
        hidID.value = selectedObject.ID;

    } else {
    
        storedObject = InPlaceServiceTypeSelector_GetDefaultObject(ctrlID);
        txtName.value = "";
        hidID.value = "";
        
    }

    InPlaceServiceTypeSelector_Selections[ctrlID] = storedObject;

    if (typeof (InPlaceServiceTypeSelector_Changed_Parent) == "function") {

        InPlaceServiceTypeSelector_Changed_Parent(ctrlID, InPlaceServiceTypeSelector_GetSelectedObject(ctrlID));
        
    }

}

function InPlaceServiceTypeSelector_Query(redundant, includeIds, excludeIds, excludeServiceCategories, selectedID) {

    // bool, null = both true/false
    this.Redundant = redundant;
    // array of int, null = include all
    this.IncludeIds = includeIds;
    // array of int, null = exclude none
    this.ExcludeIds = excludeIds;
    // array of int, null = exclude none
    this.ExcludeServiceCategories = excludeServiceCategories;
    
}

function InPlaceServiceTypeSelector_ServiceType(id, serviceCategoryDescription, serviceCategoryID, serviceClassificationGroupDescription, serviceClassificationGroupID, serviceGroupDescription, serviceGroupID, serviceTypeDescription, redundant, permanent) {
    
    this.ID = InPlaceServiceTypeSelector_GetInteger(id); 
    this.ServiceCategoryDescription = serviceCategoryDescription;
    this.ServiceCategoryID = InPlaceServiceTypeSelector_GetInteger(serviceCategoryID);
    this.ServiceClassificationGroupDescription = serviceClassificationGroupDescription;
    this.ServiceClassificationGroupID = InPlaceServiceTypeSelector_GetInteger(serviceClassificationGroupID);
    this.ServiceGroupDescription = serviceGroupDescription;
    this.ServiceGroupID = InPlaceServiceTypeSelector_GetInteger(serviceGroupID);
    this.ServiceTypeDescription = serviceTypeDescription;
    this.Redundant = InPlaceServiceTypeSelector_GetBoolean(redundant);
    this.Permanent = InPlaceServiceTypeSelector_GetBoolean(permanent);
    
}

