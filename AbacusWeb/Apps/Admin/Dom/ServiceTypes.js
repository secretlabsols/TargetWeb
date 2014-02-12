var lookupSvc, cboDomService, selectedServiceTypeID;
var mode, tblLinkedServices, serviceTypes_services, serviceTypes_dum;
var careType;
var serviceGroupCollection = new Array();
var lblServiceCategory, lblServiceCategoryID;
var hidServiceGroup, hidServiceGroupID;
var serviceType_NonRes = 1;
var serviceType_Res = 2;
var serviceType_Cash = 3;
var inEditMode = false;
var isInitialising = false;
var domunitsofmeasure;
var tdBcDescriptionIndex = 0;
var tdDomMeasuredInIndex = 1;
var tdBcMeasuredInIndex = 3;
var tdBcConFactorIndex = 4;
var isVisitBased = false;
var selectedServiceGroup;

function ServiceGroup(id, serviceCategory, permanent, ftAbbreviation) {
    this.ID = id;
    this.ServiceCategory = serviceCategory;
    this.Permanent = permanent;
}

function Init() {
    isInitialising = true;
    lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
    lblServiceCategory = GetElement(lblServiceCategoryID);
    hidServiceGroup = GetElement(hidServiceGroupID);
    if (careType == "1" || careType == "3")
    {
         tblLinkedServices = GetElement("tblLinkedServices");
     }
     SetServiceCategory(hidServiceGroup.value);
     FetchDomUnitsOfMeasureList();       
}

function InPlaceServiceGroup_Changed(newID, newServiceGroupID) 
{
    SetServiceCategory(newServiceGroupID);    
}

function SetServiceCategory(id) {

    selectedServiceGroup = GetServiceGroupObject(id);
    
    var disableFields = (mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew || mode == Target.Library.Web.UserControls.StdButtonsMode.Edit);

    if (selectedServiceGroup != null) {
        if (selectedServiceGroup.ServiceCategory == serviceType_NonRes) {
            lblServiceCategory.innerText = '(Non-Residential)';
        }
        else if (selectedServiceGroup.ServiceCategory == serviceType_Res) {
            var serviceCategoryDesc = '(';
            if (selectedServiceGroup.Permanent == true) {
                serviceCategoryDesc += 'Permanent';
            } else {
                serviceCategoryDesc += 'Temporary';
            }
            serviceCategoryDesc += ' Residential)';
            lblServiceCategory.innerText = serviceCategoryDesc;
        }
        else if (selectedServiceGroup.ServiceCategory == serviceType_Cash) {
            lblServiceCategory.innerText = '(Cash)';
        } 
        else {
            lblServiceCategory.innerText = '(Unknown Service Category = ' + id + ')';
        }
    }
}

function FetchDomUnitsOfMeasureList() {
    if (careType != "0" && careType != "2")
    {
        DisplayLoading(true);
        var strIsVisitBased = 'UseDefault';
        lookupSvc.FetchDomUnitsOfMeasureList(strIsVisitBased, FetchDomUnitsOfMeasureList_Callback);
	}
}

function FetchDomUnitsOfMeasureList_Callback(response) {
    if (tblLinkedServices != undefined) {
        var rows = tblLinkedServices.tBodies[0].rows;
        var row;
        if (CheckAjaxResponse(response, lookupSvc.url)) {
            var currentService, titleTD, titleID;
            domunitsofmeasure = response.value.UnitsOfMeasure;
            for (index = 0; index < rows.length; index++) {
                row = rows[index];
                if (row.cells.length > 1) {
                    titleTD = row.cells[tdBcDescriptionIndex];
                    titleID = titleTD.getElementsByTagName("INPUT")[0].id;
                    titleID = titleID.substring(0, titleID.lastIndexOf("_"));
                    currentService = InPlaceServiceSelector_GetSelectedObject(titleID)
                    InPlaceServiceSelector_Changed_Parent(titleID, currentService);
                }
            }
        }
    }
    DisplayLoading(false);
    isInitialising = false;   
}

function tabStrip_ActiveTabChanged(sender, args) {
    var hidSelectedTab = GetElement("hidSelectedTab");
    hidSelectedTab.value = sender.get_activeTab().get_headerText();
}

function GetServiceGroupObject(id) {
    if (serviceGroupCollection != null) {
        for (var j = 0; j < serviceGroupCollection.length; j++) {
            if (serviceGroupCollection[j].ID == id) {
                return serviceGroupCollection[j];
            }
        }
    }
}

function InPlaceServiceSelector_GetQueryObject(srcControlID) {

    var cell, row, rows, rowsCount, serviceParentControlID, currentServiceID;
    var query = new InPlaceServiceSelector_Query();
    var selectedServiceID = InPlaceServiceSelector_GetSelectedID(srcControlID);
    
    query.ExcludeIds = new Array();
    query.IncludeIds = [selectedServiceID];
    query.IncludeServiceTypeIds = [0, selectedServiceTypeID];
    query.ServiceTypeVisitBased = 'UseDefault';
    query.Redundant = false;

    if (tblLinkedServices != undefined) {
        
        rows = tblLinkedServices.tBodies[0].rows;
        rowsCount = rows.length;

        for (index = 0; index < rowsCount; index++) {

            row = rows[index];
            cell = row.cells[tdBcDescriptionIndex];
            serviceParentControlID = cell.getElementsByTagName("INPUT")[0].id;
            serviceParentControlID = serviceParentControlID.substring(0, serviceParentControlID.lastIndexOf("_"));
            currentServiceID = InPlaceServiceSelector_GetSelectedID(serviceParentControlID);
            if ((selectedServiceID != currentServiceID) && currentServiceID > 0) {
                query.ExcludeIds[index] = InPlaceServiceSelector_GetSelectedID(serviceParentControlID);
            }
        }
        
    }
    
    return query;
}

function InPlaceServiceSelector_Changed_Parent(srcControlID, selectedObject) {

    var bcDescriptionTD;
    var bcDescriptionCtrlID;
    var currentUom = null;
    var measuredInSelectedValue = "";
    var measuredInDisabled = false;
    var opt;
    var rowCount = tblLinkedServices.tBodies[0].rows.length;
    var selectedObjectParentTD, selectedObjectMeasureInTD, selectedObjectMeasureIn, selectedObjectMeasureInVal, selectedObjectMeasureInSelectedVal;
    var uomsLength = 0;
    var useExistingUom = false;
    var serviceVisitBased = false;
    var uomVisitBased;

    setIsVisitBased(selectedObject);
    serviceVisitBased = isVisitBased;

    if (domunitsofmeasure != undefined) {
        uomsLength = domunitsofmeasure.length;
    }
       
    selectedObjectParentTD = GetElement(srcControlID + "_txtName").parentNode;
    selectedObjectMeasureInTD = selectedObjectParentTD.nextSibling;
    selectedObjectMeasureIn = selectedObjectMeasureInTD.getElementsByTagName("SELECT")[0];
    selectedObjectMeasureInSelectedVal = selectedObjectMeasureIn.value;
    selectedObjectMeasureIn.length = 0;
    selectedObjectMeasureInVal = selectedObjectMeasureInTD.getElementsByTagName("SPAN")[0];    

    bcDescriptionTD = selectedObjectMeasureInTD.nextSibling;
    bcDescriptionCtrlID = bcDescriptionTD.childNodes[0].id; 
    bcDescriptionCtrlID = bcDescriptionCtrlID.substring(0, bcDescriptionCtrlID.lastIndexOf("_"));
    
    opt = document.createElement("OPTION");
    selectedObjectMeasureIn.options.add(opt);
    SetInnerText(opt, "");
    opt.value = "";

    if (uomsLength > 0) {
        for (var uomIndex = 0; uomIndex < uomsLength; uomIndex++) {
            currentUom = domunitsofmeasure[uomIndex];
            if (currentUom != undefined) {
                uomVisitBased = (currentUom.MinutesPerUnit > 0) ? true : false;
                if (uomVisitBased == serviceVisitBased) {
                    opt = document.createElement("OPTION");
                    selectedObjectMeasureIn.options.add(opt);
                    SetInnerText(opt, currentUom.Description);
                    opt.value = currentUom.ID;
                }

            }
        }
    }

    if (isInitialising && selectedObjectMeasureInVal && !isNaN(parseInt(selectedObjectMeasureInSelectedVal))) {

        selectedObject.DomUnitsOfMeasureID = parseInt(selectedObjectMeasureInSelectedVal);
        
    }

    useExistingUom = ((selectedObject.ID > 0 && selectedObject.DomUnitsOfMeasureID > 0) &&(selectedObject.IsInUseByRateCategory == true || selectedObject.IsInUseBySdsInvoicedDomServiceActualWeek == true));

    if (selectedObject.ID == 0 || useExistingUom == false) {

        var allowableUomSystemTypesLength = 0;       
        var matchingUnitOfMeasures = new Array();
        var matchingUnitOfMeasuresLength = 0;        
        var uomWasAutoSet = false;

        if (serviceVisitBased) {

            selectedObjectMeasureIn.length = 0;
            
            if (selectedObject.ID > 0) {
                if (selectedObject.AllowableUomSystemTypes != undefined && selectedObject.AllowableUomSystemTypes.length > 0) {
                    var allowableUomSystemTypesIndex = 0;
                    allowableUomSystemTypesLength = selectedObject.AllowableUomSystemTypes.length;
                    for (var allowIndex = 0; allowIndex < allowableUomSystemTypesLength; allowIndex++) {
                        if (uomsLength > 0) {
                            for (var uomIndex = 0; uomIndex < uomsLength; uomIndex++) {
                                if (domunitsofmeasure[uomIndex].SystemType == selectedObject.AllowableUomSystemTypes[allowIndex]) {
                                    matchingUnitOfMeasures[allowableUomSystemTypesIndex] = domunitsofmeasure[uomIndex];
                                    allowableUomSystemTypesIndex++;
                                }
                            }
                        }
                    }
                    if (matchingUnitOfMeasures.length == 1) {
                        measuredInSelectedValue = matchingUnitOfMeasures[0].ID;
                        measuredInDisabled = true;
                        uomWasAutoSet = true;
                    }
                }
            }

            if (allowableUomSystemTypesLength == 0 && uomsLength > 0) {
                var allowableUomSystemTypesIndex = 0;
                for (var uomIndex = 0; uomIndex < uomsLength; uomIndex++) {
                    if (domunitsofmeasure[uomIndex].SystemType != 3) {
                        matchingUnitOfMeasures[allowableUomSystemTypesIndex] = domunitsofmeasure[uomIndex];
                        allowableUomSystemTypesIndex++;
                    }
                }
            }

            matchingUnitOfMeasuresLength = matchingUnitOfMeasures.length;

            opt = document.createElement("OPTION");
            selectedObjectMeasureIn.options.add(opt);
            SetInnerText(opt, "");
            opt.value = "";

            if (matchingUnitOfMeasuresLength > 0) {
                for (var uomIndex = 0; uomIndex < uomsLength; uomIndex++) {
                    currentUom = matchingUnitOfMeasures[uomIndex];
                    if (currentUom != undefined) {
                        uomVisitBased = (currentUom.MinutesPerUnit > 0) ? true : false;
                        if (uomVisitBased == serviceVisitBased) {
                            opt = document.createElement("OPTION");
                            selectedObjectMeasureIn.options.add(opt);
                            SetInnerText(opt, currentUom.Description);
                            opt.value = currentUom.ID;
                        }
                    }
                }
            }

        } else {
            measuredInDisabled = false;            
        }

        if (selectedObject.ID > 0 && uomWasAutoSet == false && isInitialising == false) {
            if (uomsLength > 0) {
                for (var uomIndex = 0; uomIndex < uomsLength; uomIndex++) {
                    currentUom = domunitsofmeasure[uomIndex];
                    if (currentUom != undefined) {
                        if (InPlaceServiceSelector_GetStrippedMeasuredIn(selectedObject.MeasuredIn) == InPlaceServiceSelector_GetStrippedMeasuredIn(currentUom.Description)) {
                            measuredInSelectedValue = currentUom.ID;
                            measuredInDisabled = false;
                            break;
                        }
                    }
                }
            }
        }
    }
    else if (selectedObject.ID > 0 && useExistingUom == true) {

        measuredInSelectedValue = selectedObject.DomUnitsOfMeasureID;
        measuredInDisabled = true;
    }

    if (isInitialising == false) {
        selectedObjectMeasureIn.value = measuredInSelectedValue;
    } else {
        selectedObjectMeasureIn.value = selectedObject.DomUnitsOfMeasureID;
    }
    if (inEditMode) {
        selectedObjectMeasureIn.disabled = measuredInDisabled;
        selectedObjectMeasureInVal.enabled = !measuredInDisabled;
    }
    
    cboDomUnitsOfMeasure_Change(selectedObjectMeasureIn.id);
    InPlaceBudgetCategorySelector_Changed_Parent(bcDescriptionCtrlID, InPlaceBudgetCategorySelector_GetSelectedObject(bcDescriptionCtrlID));
        
}

function cboDomUnitsOfMeasure_Change(id) {
    var cboDomUnitsOfMeasure = GetElement(id);
    var hdnDomUnitsOfMeasure = null;
    var hdnDomUnitsOfMeasureID = id.substring(0, id.lastIndexOf("_")).replace(/_dum/gi, "_dumHdn");
    var bcDesciptionTD = cboDomUnitsOfMeasure.parentNode.nextSibling;
    var bcDescriptionID = bcDesciptionTD.childNodes[0].id;

    bcDescriptionID = bcDescriptionID.substring(0, bcDescriptionID.lastIndexOf("_"));
    hdnDomUnitsOfMeasure = GetElement(hdnDomUnitsOfMeasureID);
    hdnDomUnitsOfMeasure.value = cboDomUnitsOfMeasure.value;
    InPlaceBudgetCategorySelector_Changed_Parent(bcDescriptionID, InPlaceBudgetCategorySelector_GetSelectedObject(bcDescriptionID));
}

function InPlaceBudgetCategorySelector_GetQueryObject(srcControlID) {

    var query = new InPlaceServiceSelector_Query();
    var selectedID = InPlaceBudgetCategorySelector_GetSelectedID(srcControlID);

    query.IncludeIds = [selectedID];
    query.IncludeServiceTypeIds = [0, selectedServiceTypeID];
    query.Redundant = false;

    return query;
}

function InPlaceBudgetCategorySelector_Changed_Parent(srcControlID, selectedObject) {
    var srcControl = GetElement(srcControlID + "_txtName");
    var parentRow = srcControl.parentNode.parentNode;
    var bcMeasuredInLbl = parentRow.cells[tdBcMeasuredInIndex].childNodes[0];
    var bcNewMeasuredInText = "&nbsp;";
    var dsTitleSrcControlID = parentRow.cells[tdBcDescriptionIndex].childNodes[0].id;
    var dsSelectedService = null;
    var bcConTD = parentRow.cells[tdBcConFactorIndex];
    var bcConReadOnly = true;
    var bcConReadOnlyText = "&nbsp;";
    var bcConReadOnlyDiv = bcConTD.childNodes[1];
    var bcConEditableDiv = bcConTD.childNodes[0];
    var visitBased = false;
    var bcNumeratorCtrl = bcConEditableDiv.childNodes[0].childNodes[0];
    var bcNumeratorValCtrl = bcConEditableDiv.childNodes[0].getElementsByTagName("SPAN")[1];
    var bcDenominatorCtrl = bcConEditableDiv.childNodes[2].childNodes[0];
    var bcDenominatorValCtrl = bcConEditableDiv.childNodes[2].getElementsByTagName("SPAN")[1];
    var bcNumeratorValue = 0;
    var bcDenominatorValue = 0;
    var domMeasuredInCbo = parentRow.cells[tdDomMeasuredInIndex].childNodes[0];
    var domMeasuredInValue = domMeasuredInCbo.value;

    dsTitleSrcControlID = dsTitleSrcControlID.substring(0, dsTitleSrcControlID.lastIndexOf("_"));
    dsSelectedService = InPlaceServiceSelector_GetSelectedObject(dsTitleSrcControlID);

    setIsVisitBased(dsSelectedService);
    visitBased = isVisitBased;

    if (domMeasuredInValue == "") {
        domMeasuredInValue = 0;
    }

    domMeasuredInValue = parseInt(domMeasuredInValue);

    if (selectedObject != undefined && selectedObject != null && selectedObject.ID > 0 && selectedObject.DomUnitsOfMeasureID > 0) {
        bcNewMeasuredInText = selectedObject.DomUnitsOfMeasureDescription;
    }

    if ((selectedObject != undefined && selectedObject != null && selectedObject.ID > 0) && (dsSelectedService != undefined && dsSelectedService != null && dsSelectedService.ID > 0)) {
        if (visitBased) {
            bcConReadOnly = true;
            bcConReadOnlyText = "Automatic";
            bcNumeratorValue = 0;
            bcDenominatorValue = 0;
        } else {
            if (selectedObject.DomUnitsOfMeasureID == domMeasuredInValue) {
                bcConReadOnly = true;
                bcConReadOnlyText = "Automatic";
                bcNumeratorValue = 1;
                bcDenominatorValue = 1;
            } else {
                bcConReadOnly = false;
            }
        }
    } 

    if (bcConReadOnly == true) {
        bcConReadOnlyDiv.style.display = "block";
        bcConEditableDiv.style.display = "none";
    } else {
        bcConReadOnlyDiv.style.display = "none";
        bcConEditableDiv.style.display = "block";
    }

    bcNumeratorValCtrl.enabled = !bcConReadOnly;
    bcNumeratorValCtrl.isvalid = true;
    ValidatorUpdateDisplay(bcNumeratorValCtrl);
    bcDenominatorValCtrl.enabled = !bcConReadOnly;
    bcDenominatorValCtrl.isvalid = true;
    ValidatorUpdateDisplay(bcDenominatorValCtrl);

    if (isInitialising == false) {
        bcNumeratorCtrl.value = bcNumeratorValue;
        bcDenominatorCtrl.value = bcDenominatorValue;
    }

    bcConReadOnlyDiv.innerHTML = bcConReadOnlyText;
    bcMeasuredInLbl.innerHTML = bcNewMeasuredInText;

}

function setIsVisitBased(selectedObject) {
    if (selectedObject.VisitBasedReturns == true || selectedObject.EnableVisits == true) {
        isVisitBased = true;
    }
    else {
        isVisitBased = false;
    }
}

addEvent(window, "load", Init);