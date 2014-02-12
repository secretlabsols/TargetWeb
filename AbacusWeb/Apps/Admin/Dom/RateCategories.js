
var contractSvc, lookupSvc, cboServiceType, cboServiceTypeLastVal, cboDayCategory, cboTimeBand, txtConstructedAbbreviation, cboUom, mode, lblService;
var rateFrameworkID, currentTimeBandID, currentDomServiceID, currentUomID, rateFrameWorkTypeAbvr, uomHoursId, uomHoursMinsId, inplaceServiceSelectorID, isRateCategoryManualPayment;
var valServiceType, valDayCategory, valTimeBand;
var rateFrameWorkAttendanceAbvr = "A";
var rateFrameWorkVisitsAbvr = "V";
var rateFrameWorkCommunityGeneralAbvr = "C";
var shouldRaiseUomOnChangeEvent = false;
var cboPaymentToleranceGroup, currentPTG_ID, ptgSystemType, paymentToleranceGroupSystemTypes;

function Init() {
    contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
    lookupSvc = new Target.Abacus.Web.Apps.WebSvc.Lookups_class();
    rateFrameworkID = GetQSParam(document.location.search, "frameworkID");
    cboServiceType = GetElement("cboServiceType_cboDropDownList", true);
    cboDayCategory = GetElement("cboDayCategory_cboDropDownList", true);
    cboTimeBand = GetElement("cboTimeBand_cboDropDownList", true);
    valServiceType = GetElement("cboServiceType_valRequired", true);
    valDayCategory = GetElement("cboDayCategory_valRequired", true);
    valTimeBand = GetElement("cboTimeBand_valRequired", true);
    txtConstructedAbbreviation = GetElement("txtConstructedAbbreviation_txtTextBox", true);
    cboUom = GetElement("cboUom_cboDropDownList", true);
    lblService = GetElement("lblService", true);
    cboServiceTypeLastVal = cboServiceType.value;
    cboPaymentToleranceGroup = GetElement("cboPaymentToleranceGroup_cboDropDownList", true);

    SetSystemTypeBasedOnSelectedUOM(cboUom.value);

    Populate_cboPaymentToleranceGroup();

    if (cboDayCategory)
        FetchTimeBands();

    if (cboServiceType) {
        GetAbbreviation();
    }
    cboServiceType_OnChange();

    if (mode > 1) {
        lblService.disabled = false;
        if (currentUomID) {
            cboPaymentToleranceGroup.disabled = false;
        }
        else {
            cboPaymentToleranceGroup.disabled = true;
        }
    } else {
        lblService.disabled = true;
    }

    //if manual payment rate category then clear and disable the tolerance group drop down
    if (isRateCategoryManualPayment) {
        jQuery('[id*=cboPaymentToleranceGroup]').attr("disabled", "disabled");
    }
}

function FetchTimeBands() {
    var dayCategoryID = cboDayCategory.value;
    if (dayCategoryID.length == 0) {
        if (cboTimeBand) {
            cboTimeBand.options.length = 0;
            cboTimeBand.disabled = true;
        }
        dayCategoryID = 0
    } else {
        DisplayLoading(true);
        contractSvc.FetchTimeBandsAvailableToDayCategory(dayCategoryID, FetchTimeBands_Callback);
    }
}
function FetchTimeBands_Callback(response) {
    var timeBands, opt;
    if (CheckAjaxResponse(response, contractSvc.url)) {
        timeBands = response.value.List;

        // clear
        if (cboTimeBand) {
            cboTimeBand.options.length = 0;
        }
        // add blank		
        opt = document.createElement("OPTION");
        if (cboTimeBand)
            cboTimeBand.options.add(opt);
        SetInnerText(opt, "");
        opt.value = "";

        for (index = 0; index < timeBands.length; index++) {
            opt = document.createElement("OPTION");
            if (cboTimeBand)
                cboTimeBand.options.add(opt);
            SetInnerText(opt, timeBands[index].Text);
            opt.value = timeBands[index].Value;
        }

        // select existing value
        if (cboTimeBand) {
            cboTimeBand.value = currentTimeBandID;
            if (cboDayCategory.disabled == false) {
                cboTimeBand.disabled = false;
            }
        }

    }
    DisplayLoading(false);
}
function GetAbbreviation() {
    var svcTypeID, dayCategoryID, timeBandID;

    svcTypeID = cboServiceType.value;
    if (svcTypeID.length == 0) svcTypeID = 0;
    if (cboDayCategory) {
        dayCategoryID = cboDayCategory.value;
    }
    else {
        dayCategoryID = 0;
    }
    if (dayCategoryID.length == 0) dayCategoryID = 0;

    if (cboTimeBand) {
        timeBandID = cboTimeBand.value;
    }
    else {
        timeBandID = 0
    }
    if (timeBandID == null) timeBandID
    if (timeBandID.length == 0) timeBandID = 0;

    DisplayLoading(true);
    contractSvc.GetRateCategoryAbbreviation(rateFrameworkID, svcTypeID, dayCategoryID, timeBandID, GetAbbreviation_Callback);

    DisplayLoading(true);

}
function GetAbbreviation_Callback(response) {
    if (CheckAjaxResponse(response, contractSvc.url)) {
        txtConstructedAbbreviation.value = response.value.Value;
    }
    DisplayLoading(false);
}

function FetchUomList() {
    var strChecked = 'UseDefault';
    DisplayLoading(true);
    if (rateFrameWorkTypeAbvr == rateFrameWorkVisitsAbvr) {
        strChecked = 'True';
    }
    lookupSvc.FetchDomUnitsOfMeasureList(strChecked, FetchUomList_Callback);
}

function FetchUomList_Callback(response) {
    var uoms, opt, currentUom;
    if (CheckAjaxResponse(response, contractSvc.url)) {
        uoms = response.value.List;

        // clear
        cboUom.options.length = 0;
        // add blank		
        opt = document.createElement("OPTION");
        cboUom.options.add(opt);
        SetInnerText(opt, "");
        opt.value = "";

        for (index = 0; index < uoms.length; index++) {
            currentUom = uoms[index];
            opt = document.createElement("OPTION");
            cboUom.options.add(opt);
            SetInnerText(opt, currentUom.Description);
            opt.value = currentUom.ID;
        }

        // select existing value
        cboUom.value = currentUomID;

    }
    if (shouldRaiseUomOnChangeEvent == true) {
        cboUom_OnChange();
    }
    DisplayLoading(false);
}

function PrimeValidators() {
    if (mode > 1) {
        // do not require any by default
        if (valServiceType) {
            ValidatorEnable(valServiceType, false);
        }
        if (valDayCategory) {
            ValidatorEnable(valDayCategory, false);
        }
        if (valTimeBand) {
            ValidatorEnable(valTimeBand, false);
        }

        if (valServiceType && isRateCategoryManualPayment === false) {
            ValidatorEnable(valServiceType, true);
        }

        if (rateFrameWorkTypeAbvr == rateFrameWorkVisitsAbvr) {
            // for visit based returns, require all
            if (valDayCategory && isRateCategoryManualPayment === false) {
                ValidatorEnable(valDayCategory, true);
            }
            if (valTimeBand && isRateCategoryManualPayment === false) {
                ValidatorEnable(valTimeBand, true);
            }
        } else {
            // for non visit based returns
            // if we have a time band then require a day category
            if (cboTimeBand && cboTimeBand.value.length > 0 && isRateCategoryManualPayment === false) {
                ValidatorEnable(valDayCategory, true);
            }
            if (cboDayCategory && cboDayCategory.value.length > 0 && isRateCategoryManualPayment === false) {
                ValidatorEnable(valServiceType, true);
            }
        }
    }
}

function cboUom_OnChange() {

    var selectedUom = cboUom.value;

    if (selectedUom == "") {
        selectedUom = 0;
    } else {
        selectedUom = parseInt(selectedUom);
    }

    if (mode > 1 && isRateCategoryManualPayment === false) {
        if (selectedUom == 0) {
            InPlaceServiceSelector_Enable(inplaceServiceSelectorID, false, true);

            cboPaymentToleranceGroup.value = 0;
            cboPaymentToleranceGroup.disabled = true;

        } else {

            InPlaceServiceSelector_Enable(inplaceServiceSelectorID, true, true);

            cboPaymentToleranceGroup.disabled = false;

            //set payment tolerance group system type
            SetSystemTypeBasedOnSelectedUOM(selectedUom);

            //populate payment tolerance groups
            Populate_cboPaymentToleranceGroup();

        }
    }

    cboServiceType_OnChange();
}

function cboServiceType_OnChange() {
    GetAbbreviation();
    PrimeValidators();
    if (mode > 1 && isRateCategoryManualPayment === false) {
        if (cboServiceTypeLastVal != cboServiceType.value || cboServiceTypeLastVal == '') {
            var isServiceSelectorEnabled = (cboServiceType.value !== '' || isRateCategoryManualPayment === true);
            InPlaceServiceSelector_Enable(inplaceServiceSelectorID, isServiceSelectorEnabled, true);
            cboServiceTypeLastVal = cboServiceType.value;
        }
    }

}

function InPlaceServiceSelector_GetQueryObject(srcControlID) {

    var allServiceTypesToken = 0;
    var query = new InPlaceServiceSelector_Query();
    var selectedID = InPlaceServiceSelector_GetSelectedID(srcControlID);
    var selectedServiceType = cboServiceType.value;
    var selectedUom = cboUom.value;

    if (selectedServiceType == "") {
        selectedServiceType = 0;
    } else {
        selectedServiceType = parseInt(selectedServiceType);
    }

    if (selectedUom == "") {
        selectedUom = 0;
    } else {
        selectedUom = parseInt(selectedUom);
    }

    query.ForUseWithDomiciliaryContracts = true;
    query.SavedId = currentDomServiceID;

    if (rateFrameWorkTypeAbvr.toUpperCase() == rateFrameWorkAttendanceAbvr.toUpperCase() || rateFrameWorkTypeAbvr.toUpperCase() == rateFrameWorkCommunityGeneralAbvr.toUpperCase()) {
        if (ValidateSelectedUom() && ValidateSelectedServiceType()) {
            query.Redundant = false;
            if (isRateCategoryManualPayment === false) {
                query.IncludeServiceTypeIds = [selectedServiceType];
            }
            query.IncludeUomIds = [selectedUom];
        } else {
            query = null;
        }
    } else if (rateFrameWorkTypeAbvr.toUpperCase() == rateFrameWorkVisitsAbvr.toUpperCase()) {
        if (isRateCategoryManualPayment === false) {
            query.IncludeServiceTypeIds = [selectedServiceType];
        }
        query.Redundant = false;
    } else {
        alert('Framework type selected is currently unconfigured for the selection of service.');
        query = null;
    }

    return query;
}

function InPlaceServiceSelector_Changed_Parent(srcControlID, selectedObject) {

    if (selectedObject != undefined && selectedObject != null && selectedObject.ID > 0 && selectedObject.BudgetCategoryID) {
        InPlaceServiceSelector_SetInformationString(inplaceServiceSelectorID, "(" + selectedObject.BudgetCategoryDescription + ")");
    } else {
        InPlaceServiceSelector_SetInformationString(inplaceServiceSelectorID, "");
    }

}

function ValidateSelectedServiceType() {
    var selectedServiceType = cboServiceType.value;
    if (selectedServiceType == "") {
        selectedServiceType = 0;
    } else {
        selectedServiceType = parseInt(selectedServiceType);
    }
    if (selectedServiceType <= 0 && isRateCategoryManualPayment === false) {
        alert('Please select a value from the \'Service Type\' drop down list.');
        return false;
    }
    return true;
}

function SetSystemTypeBasedOnSelectedUOM(selectedUom) {

    //get the uom object based on selectedUOM
    if (selectedUom > 0) {
        var uomArray = $.grep(domUnitOfMeasureCollection, function(n) {
            return n.ID == selectedUom;
        });

        //check if the current UOM is time based to set the correct paymentToleranceGroupSystemType
        if (uomArray.length == 1) {
            if (IsDomUOM_TimeBased(uomArray[0])) {
                ptgSystemType = Target.Abacus.Library.PaymentToleranceGroupSystemTypes.VisitBasedServiceToleranceGroup;
            }
            else {
                ptgSystemType = Target.Abacus.Library.PaymentToleranceGroupSystemTypes.UserEnteredPaymentToleranceGroup;
            }
        }
    }

}

function ValidateSelectedUom() {
    var selectedUom = cboUom.value;
    if (selectedUom == "") {
        selectedUom = 0;
    } else {
        selectedUom = parseInt(selectedUom);
    }
    if (selectedUom <= 0) {
        alert('Please select a value from the \'Measured In\' drop down list.');
        return false;
    }
    return true;
}

function Populate_cboPaymentToleranceGroup() {
    // clear
    cboPaymentToleranceGroup.options.length = 0;
    // add blank		
    opt = document.createElement("OPTION");
    cboPaymentToleranceGroup.options.add(opt);
    SetInnerText(opt, "");
    cboPaymentToleranceGroup.value = "";

    for (index = 0; index < paymentToleranceGroupCollection.length; index++) {

        //add the items matching the payment tolerance group system type
        if (paymentToleranceGroupCollection[index].SystemType == ptgSystemType) {
            opt = document.createElement("OPTION");
            cboPaymentToleranceGroup.options.add(opt);
            SetInnerText(opt, paymentToleranceGroupCollection[index].Description);
            opt.value = paymentToleranceGroupCollection[index].ID;
        }
    }

    // select existing value
    if (currentPTG_ID) {
        cboPaymentToleranceGroup.value = currentPTG_ID;
        // if current payment tolerance group id will invalidate contract or service order
        // prevent user from changing it.
        if (canContractOrServiceOrderBeInvalidated) {
            cboPaymentToleranceGroup.disabled = true
        }
    }
    else {
        cboPaymentToleranceGroup.value = ""
    }

}

function IsDomUOM_TimeBased(uom) {
    if (uom.MinutesPerUnit > 0) {
        return true
    }
    else {
        return false
    }
}