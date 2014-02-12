var InPlaceBudgetCategorySelector_DescriptionKey = "_txtName";
var InPlaceBudgetCategorySelector_HiddenIdKey = "_hidID";
var InPlaceBudgetCategorySelector_ModalDialogUrl = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/BudgetCategories.aspx";
var InPlaceBudgetCategorySelector_Selections = new Array();

function InPlaceBudgetCategorySelector_BudgetCategory(budgetCategoryGroupDescription, budgetCategoryGroupID, description, domUnitsOfMeasureDescription, domUnitsOfMeasureID, id, reference) {
    
    this.BudgetCategoryGroupDescription = budgetCategoryGroupDescription;
    this.BudgetCategoryGroupID = budgetCategoryGroupID;
    this.Description = description;
    this.DomUnitsOfMeasureDescription = domUnitsOfMeasureDescription;
    this.DomUnitsOfMeasureID = domUnitsOfMeasureID;
    this.ID = id;
    this.Reference = reference;
    
}

function InPlaceBudgetCategorySelector_ClearStoredID(ctrlID) {

    InPlaceBudgetCategorySelector_SelectionChanged(ctrlID, null);
    
}

function InPlaceBudgetCategorySelector_FindClicked(ctrlID) {
    
    var currentID = InPlaceBudgetCategorySelector_GetSelectedID(ctrlID);

    if (typeof (InPlaceBudgetCategorySelector_GetQueryObject) == "function") {    
        
        var query = InPlaceBudgetCategorySelector_GetQueryObject(ctrlID);

        if (query != undefined && query != null) {

            var dialog;
            var excludeIds = "";            
            var includeIds = "";
            var includeServiceTypeIds = "";
            var redundant = "";
            var splitChar = ",";
            var url = "";            

            excludeIds = InPlaceBudgetCategorySelector_GetTokenSeparatedStringFromArray(query.ExcludeIds, splitChar);
            includeIds = InPlaceBudgetCategorySelector_GetTokenSeparatedStringFromArray(query.IncludeIds, splitChar);            
            includeServiceTypeIds = InPlaceBudgetCategorySelector_GetTokenSeparatedStringFromArray(query.IncludeServiceTypeIds, splitChar);

            if (query.Redundant != null) {
                redundant = query.Redundant;
            }
            
            url = InPlaceBudgetCategorySelector_ModalDialogUrl + "?&id=" + currentID + "&ctrlid=" + ctrlID + "&redundant=" + redundant + "&incIds=" + includeIds + "&exIds=" + excludeIds + "&incSvcTypIds=" + includeServiceTypeIds;
            dialog = OpenDialog(url, 60, 32, window);

        }

    } else {

        alert('Function InPlaceBudgetCategorySelector_GetQueryObject does not exists, please define.');
        
    }
}

function InPlaceBudgetCategorySelector_GetBoolean(value) {

    if (value != undefined && (value == "true" || value == "1")) {

        return true;

    } else {

        return false;
        
    }
    
}

function InPlaceBudgetCategorySelector_GetDefaultObject(ctrlID) {

    var defaultObject = new InPlaceBudgetCategorySelector_BudgetCategory();
    
    defaultObject.BudgetCategoryGroupDescription = "";
    defaultObject.BudgetCategoryGroupID = 0;
    defaultObject.Description = "";
    defaultObject.DomUnitsOfMeasureDescription = "";
    defaultObject.DomUnitsOfMeasureID = 0;
    defaultObject.ID = 0;
    defaultObject.Reference = "";
    
    return defaultObject;
    
}

function InPlaceBudgetCategorySelector_GetSelectedID(ctrlID) {

    var idControl = GetElement(ctrlID + InPlaceBudgetCategorySelector_HiddenIdKey);
    return idControl.value;
    
}

function InPlaceBudgetCategorySelector_GetSelectedObject(ctrlID) {

    var selectedObject = InPlaceBudgetCategorySelector_Selections[ctrlID];

    if (selectedObject == undefined || selectedObject == null) {

        selectedObject = InPlaceBudgetCategorySelector_GetDefaultObject(ctrlID);
        
    }

    return selectedObject;

}

function InPlaceBudgetCategorySelector_GetTokenSeparatedStringFromArray(tokenizedArray, token) {

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

function InPlaceBudgetCategorySelector_IsEnabled(ctrlID) {

    var txtName;

    txtName = GetElement(ctrlID + InPlaceBudgetCategorySelector_DescriptionKey);

    return !txtName.disabled;

}

function InPlaceBudgetCategorySelector_Query(redundant, includeIds, excludeIds, includeServiceTypeIds) {

    // bool, null = both true/false
    this.Redundant = redundant;
    // array of int, null = include all
    this.IncludeIds = includeIds;
    // array of int, null = include all
    this.IncludeServiceTypeIds = includeIds;
    // array of int, null = exclude none
    this.ExcludeIds = excludeIds;
    
}

function InPlaceBudgetCategorySelector_SelectionChanged(ctrlID, selectedObject) {

    var storedObject = new InPlaceBudgetCategorySelector_BudgetCategory();
    var txtName, hidID;

    txtName = GetElement(ctrlID + InPlaceBudgetCategorySelector_DescriptionKey);
    hidID = GetElement(ctrlID + InPlaceBudgetCategorySelector_HiddenIdKey);

    if (selectedObject != null) {
    
        storedObject.BudgetCategoryGroupDescription = selectedObject.BudgetCategoryGroupDescription;
        storedObject.BudgetCategoryGroupID = selectedObject.BudgetCategoryGroupID;
        storedObject.Description = selectedObject.Description;
        storedObject.DomUnitsOfMeasureDescription = selectedObject.DomUnitsOfMeasureDescription;
        storedObject.DomUnitsOfMeasureID = selectedObject.DomUnitsOfMeasureID;
        storedObject.ID = selectedObject.ID;
        storedObject.Reference = selectedObject.Reference;
        txtName.value = selectedObject.Description;
        hidID.value = selectedObject.ID;

    } else {
    
        storedObject = InPlaceBudgetCategorySelector_GetDefaultObject(ctrlID);
        txtName.value = "";
        hidID.value = "";
        
    }

    InPlaceBudgetCategorySelector_Selections[ctrlID] = storedObject;

    if (typeof (InPlaceBudgetCategorySelector_Changed_Parent) == "function") {

        InPlaceBudgetCategorySelector_Changed_Parent(ctrlID, InPlaceBudgetCategorySelector_GetSelectedObject(ctrlID));
	    
	}
}










