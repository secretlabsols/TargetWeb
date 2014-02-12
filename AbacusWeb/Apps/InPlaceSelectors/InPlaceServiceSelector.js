var InPlaceServiceSelector_ButtonKey = "_btnFind";
var InPlaceServiceSelector_DescriptionKey = "_txtName";
var InPlaceServiceSelector_HiddenIdKey = "_hidID";
var InPlaceServiceSelector_InfoKey = "_lblInfo";
var InPlaceServiceSelector_ModalDialogUrl = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/Services.aspx";
var InPlaceServiceSelector_Selections = new Array();

function InPlaceServiceSelector_ClearStoredID(ctrlID) {

    InPlaceServiceSelector_SetInformationString(ctrlID, "");
    InPlaceServiceSelector_SelectionChanged(ctrlID, null);
    
}

function InPlaceServiceSelector_Enable(ctrlID, enabled, clearCurrentSelection) {

    var txtName, btnFind, lblInfo;

    enabled = InPlaceServiceSelector_GetBoolean(enabled);
    clearCurrentSelection = InPlaceServiceSelector_GetBoolean(clearCurrentSelection);

    lblInfo = GetElement(ctrlID + InPlaceServiceSelector_InfoKey);
    txtName = GetElement(ctrlID + InPlaceServiceSelector_DescriptionKey);
    btnFind = GetElement(ctrlID + InPlaceServiceSelector_ButtonKey);

    txtName.disabled = !enabled;
    btnFind.disabled = !enabled;
    lblInfo.disabled = !enabled;

    if (clearCurrentSelection == true) {

        InPlaceServiceSelector_ClearStoredID(ctrlID);
        
    }

}

function InPlaceServiceSelector_FindClicked(ctrlID) {

    var currentID = InPlaceServiceSelector_GetSelectedID(ctrlID);

    if (typeof (InPlaceServiceSelector_GetQueryObject) == "function") {

        var query = InPlaceServiceSelector_GetQueryObject(ctrlID);

        if (query != undefined && query != null) {

            var dialog;
            var excludeIds = "";
            var exNoServiceType = "";
            var forUseWithDomiciliaryContracts = "";
            var includeIds = "";
            var includeServiceTypeIds = "";
            var includeUomIds = "";
            var redundant = "";
            var savedId = 0;
            var splitChar = ",";
            var stVisitBased = "";
            var url = "";          

            if (query.ExcludeWhenNotAssociatedWithServiceType != null) {
                exNoServiceType = query.ExcludeWhenNotAssociatedWithServiceType;
            }

            if (query.ForUseWithDomiciliaryContracts != null) {
                forUseWithDomiciliaryContracts = query.ForUseWithDomiciliaryContracts;
            }

            excludeIds = InPlaceServiceSelector_GetTokenSeparatedStringFromArray(query.ExcludeIds, splitChar);
            includeIds = InPlaceServiceSelector_GetTokenSeparatedStringFromArray(query.IncludeIds, splitChar);
            includeServiceTypeIds = InPlaceServiceSelector_GetTokenSeparatedStringFromArray(query.IncludeServiceTypeIds, splitChar);
            includeUomIds = InPlaceServiceSelector_GetTokenSeparatedStringFromArray(query.IncludeUomIds, splitChar);
                        
            if (query.Redundant != null) {
                redundant = query.Redundant;
            }
            
            if (query.ServiceTypeVisitBased != null) {
                stVisitBased = query.ServiceTypeVisitBased;
            }

            query.SavedId = (query.SavedId) ? parseInt(query.SavedId) : 0;

            if (query.SavedId > 0) {
                savedId = query.SavedId;
            }

            url = InPlaceServiceSelector_ModalDialogUrl + "?&id=" + currentID + "&ctrlid=" + ctrlID + "&redundant=" + redundant + "&incIds=" + includeIds + "&exIds=" + excludeIds + "&stVisitBased=" + stVisitBased + "&exNoServiceType=" + exNoServiceType + "&fuwdc=" + forUseWithDomiciliaryContracts + "&incStis=" + includeServiceTypeIds + "&incUoms=" + includeUomIds + "&sid=" + savedId;
            dialog = OpenDialog(url, 60, 32, window);

        }

    } else {

        alert('Function InPlaceServiceSelector_GetQueryObject does not exists, please define.');

    }
}

function InPlaceServiceSelector_GetBoolean(value) {

    if (value != undefined && value != null && (value == "true" || value == "1")) {
        return true;
    } else {
        return false;
    }
    
}

function InPlaceServiceSelector_GetDefaultObject(ctrlID) {

    var defaultObject = new InPlaceServiceSelector_Service();
    
    defaultObject.ID = 0;
    defaultObject.BudgetCategoryDescription = "";
    defaultObject.BudgetCategoryID = 0;
    defaultObject.Description = "";
    defaultObject.DontTreatAsMinutes = false;
    defaultObject.EnableVisits = false;
    defaultObject.ForUseWithDomiciliaryContracts = false;
    defaultObject.MeasuredIn = "";
    defaultObject.MinuteConversion = 0;
    defaultObject.MinuteRounding = 0;
    defaultObject.Title = "";
    defaultObject.VisitBasedReturns = false;
    defaultObject.IsInUseByRateCategory = false;
    defaultObject.IsInUseBySdsInvoicedDomServiceActualWeek = false;
    defaultObject.DomUnitsOfMeasureID = false;
    
    return defaultObject;
    
}

function InPlaceServiceSelector_GetSelectedID(ctrlID) {

    var idControl = GetElement(ctrlID + InPlaceServiceSelector_HiddenIdKey);
    return idControl.value;
    
}

function InPlaceServiceSelector_GetSelectedObject(ctrlID) {

    var selectedObject = InPlaceServiceSelector_Selections[ctrlID];
    var hoursMinutesSystemType = 3;
    var hoursSystemType = 1;
    var minutesSystemType = 4;

    if (selectedObject == undefined || selectedObject == null) {
        selectedObject = InPlaceServiceSelector_GetDefaultObject(ctrlID);
    }

    if (selectedObject.ID > 0) {
        if (selectedObject.VisitBasedReturns == false && selectedObject.EnableVisits == true && selectedObject.ForUseWithDomiciliaryContracts == false && selectedObject.DontTreatAsMinutes == false) {
            selectedObject.AllowableUomSystemTypes = [hoursMinutesSystemType];
        } else if (selectedObject.VisitBasedReturns == false && selectedObject.EnableVisits == true && selectedObject.ForUseWithDomiciliaryContracts == false && selectedObject.DontTreatAsMinutes == true) {
            selectedObject.AllowableUomSystemTypes = [hoursSystemType];
        } else if (selectedObject.VisitBasedReturns == true && selectedObject.EnableVisits == false && selectedObject.ForUseWithDomiciliaryContracts == true && selectedObject.DontTreatAsMinutes == true) {
            if (selectedObject.MinuteConversion == 0) {
                selectedObject.AllowableUomSystemTypes = [minutesSystemType];
            } else if (selectedObject.MinuteConversion == 1) {
                selectedObject.AllowableUomSystemTypes = [hoursSystemType];
            }
        }
    }

    return selectedObject;

}

function InPlaceServiceSelector_GetStrippedMeasuredIn(value) {
    if (value != undefined) {
        return value.replace(/\(|\)|\s/gi, "").toUpperCase();
    } else {
        return "";
    }
}


function InPlaceServiceSelector_GetTokenSeparatedStringFromArray(tokenizedArray, token) {

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

function InPlaceServiceSelector_IsEnabled(ctrlID) {

    var txtName;

    txtName = GetElement(ctrlID + InPlaceServiceSelector_DescriptionKey);

    return !txtName.disabled;

}

function InPlaceServiceSelector_Query(redundant, includeIds, excludeIds, serviceTypeVisitBased, excludeWhenNotAssociatedWithServiceType, forUseWithDomiciliaryContracts, includeServiceTypeIds, includeUomIds, savedId) {

    // bool, null = both true/false
    this.Redundant = redundant;
    // array of int, null = include all
    this.IncludeIds = includeIds;
    // array of int, null = exclude none
    this.ExcludeIds = excludeIds;
    // bool, null = both true/false
    this.ServiceTypeVisitBased = serviceTypeVisitBased;
    // bool, null = both true/false
    this.ExcludeWhenNotAssociatedWithServiceType = excludeWhenNotAssociatedWithServiceType;
    // bool, null = both true/false
    this.ForUseWithDomiciliaryContracts;
    // array of int, null = include all
    this.IncludeServiceTypeIds = includeServiceTypeIds;
    // array of int, null = include all
    this.IncludeUomIds = includeUomIds;
    // int, overidden selected id
    this.SavedId = savedId;
}

function InPlaceServiceSelector_SelectionChanged(ctrlID, selectedObject) {

    var storedObject = new InPlaceServiceSelector_Service();
    var txtName, hidID;

    txtName = GetElement(ctrlID + InPlaceServiceSelector_DescriptionKey);
    hidID = GetElement(ctrlID + InPlaceServiceSelector_HiddenIdKey);

    if (selectedObject != null) {
    
        storedObject.ID = selectedObject.ID;
        storedObject.BudgetCategoryDescription = selectedObject.BudgetCategoryDescription;
        storedObject.BudgetCategoryID = selectedObject.BudgetCategoryID;
        storedObject.Description = selectedObject.Description;
        storedObject.DontTreatAsMinutes = InPlaceServiceSelector_GetBoolean(selectedObject.DontTreatAsMinutes);
        storedObject.EnableVisits = InPlaceServiceSelector_GetBoolean(selectedObject.EnableVisits);
        storedObject.ForUseWithDomiciliaryContracts = InPlaceServiceSelector_GetBoolean(selectedObject.ForUseWithDomiciliaryContracts);
        storedObject.MeasuredIn = selectedObject.MeasuredIn;
        storedObject.MinuteConversion = selectedObject.MinuteConversion;
        storedObject.MinuteRounding = selectedObject.MinuteRounding;
        storedObject.Title = selectedObject.Title;
        storedObject.VisitBasedReturns = InPlaceServiceSelector_GetBoolean(selectedObject.VisitBasedReturns);
        storedObject.IsInUseByRateCategory = InPlaceServiceSelector_GetBoolean(selectedObject.IsInUseByRateCategory);
        storedObject.IsInUseBySdsInvoicedDomServiceActualWeek = InPlaceServiceSelector_GetBoolean(selectedObject.IsInUseBySdsInvoicedDomServiceActualWeek);
        storedObject.DomUnitsOfMeasureID = selectedObject.DomUnitsOfMeasureID;
        txtName.value = selectedObject.Title;
        hidID.value = selectedObject.ID;

    } else {
    
        storedObject = InPlaceServiceSelector_GetDefaultObject(ctrlID);
        txtName.value = "";
        hidID.value = "";
        
    }

    InPlaceServiceSelector_Selections[ctrlID] = storedObject;

	if (typeof (InPlaceServiceSelector_Changed_Parent) == "function") {
	    InPlaceServiceSelector_Changed_Parent(ctrlID, InPlaceServiceSelector_GetSelectedObject(ctrlID));
	}
}

function InPlaceServiceSelector_Service(id, budgetCategoryDescription, budgetCategoryID, description, enableVisits, forUseWithDomiciliaryContracts, minuteConversion, minuteRounding, title, visitBasedReturns, allowableUomSystemTypes, dontTreatAsMinutes, measuredIn, isInUseByRateCategory, isInUseBySdsInvoicedDomServiceActualWeek, domUnitsOfMeasureID) {
    
    this.ID = id;
    this.BudgetCategoryDescription = budgetCategoryDescription;
    this.BudgetCategoryID = budgetCategoryID;
    this.Description = description;
    this.EnableVisits = enableVisits;
    this.ForUseWithDomiciliaryContracts = forUseWithDomiciliaryContracts;
    this.MinuteConversion = minuteConversion;
    this.MinuteRounding = minuteRounding;
    this.Title = title;
    this.VisitBasedReturns = visitBasedReturns;
    this.AllowableUomSystemTypes = allowableUomSystemTypes;
    this.DontTreatAsMinutes = dontTreatAsMinutes;
    this.MeasuredIn = measuredIn;
    this.IsInUseByRateCategory = isInUseByRateCategory;
    this.IsInUseBySdsInvoicedDomServiceActualWeek = isInUseBySdsInvoicedDomServiceActualWeek;
    this.DomUnitsOfMeasureID = domUnitsOfMeasureID;
    
}

function InPlaceServiceSelector_SetInformationString(ctrlID, value) {

    var lblInfo = GetElement(ctrlID + InPlaceServiceSelector_InfoKey);
    SetInnerText(lblInfo, value);

}


