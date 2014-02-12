
var cboBudgetCategoryType, cboUnitOfMeasure, cboBudgetCategoryGroup, lblBudgetCategoryGroupInfo;
var tabDetailPrefix = "tabStrip_tabDetails_";
var buttonsMode;
var tabRatesVisible;
var hidUnitOfMeasureID;
var hidUnitOfMeasure;
var lblServiceCategory, lblServiceCategoryID;
var serviceType_NonRes = 1;
var serviceType_Res = 2;
var serviceType_Cash = 3;
var isInitialising = false;
var currentBudgetCategoryGroupValue;
// The following store the full contents of the Unit Of Measure dropdown from the code-behind..
var uomDescs = new Array();
var uomValues = new Array();
var budgetCategoryGroupCollection = new Array();
var txtServiceTypeID;
var lblServiceType;
var isInEntryMode = false;
var bcSystemType = 0;
var disableActualCostIndicator = true;

$(document).ready(function () {
    $("[id*='rateUseActualCost']").attr("disabled", disableActualCostIndicator)

});

function Init() {
    isInEntryMode = ((buttonsMode == Target.Library.Web.UserControls.StdButtonsMode.AddNew || buttonsMode == Target.Library.Web.UserControls.StdButtonsMode.Edit) && (bcSystemType <= 0));
    isInitialising = true;
    SetupControls();
    cboBudgetCategoryType_Change();
    PopulateBudgetCategoryGroups("");
    InPlaceServiceTypeSelector_Changed_Parent(txtServiceTypeID, InPlaceServiceTypeSelector_GetSelectedObject(txtServiceTypeID));
    cboBudgetCategoryGroup_Change();

    if (tabRatesVisible == 'Y') SetupRates();
    isInitialising = false;
}

function SetupControls() {

    cboBudgetCategoryType = GetElement(tabDetailPrefix + "cboBudgetCategoryType_cboDropDownList");
    cboUnitOfMeasure = GetElement(tabDetailPrefix + "cboUnitOfMeasure_cboDropDownList");
    cboBudgetCategoryGroup = GetElement(tabDetailPrefix + "cboBudgetCategoryGroup_cboDropDownList");
    lblBudgetCategoryGroupInfo = GetElement(tabDetailPrefix + "lblBudgetCategoryGroupInfo");
    hidUnitOfMeasure = GetElement(hidUnitOfMeasureID);
    lblServiceCategory = GetElement(lblServiceCategoryID);
    lblServiceType = GetElement(tabDetailPrefix + "lblServiceType");
}

function tabStrip_ActiveTabChanged(sender, args) {
    var hidSelectedTab = GetElement("hidSelectedTab");
    hidSelectedTab.value = sender.get_activeTab().get_headerText();
}

function btnRemoveAdditionalCost_Click() {
    return window.confirm("Are you sure you wish to remove this Additional Cost?");
}

function btnRemoveRate_Click() {
    return window.confirm("Are you sure you wish to remove this Rate?");
}

function cboBudgetCategoryType_Change() {

}

function cboBudgetCategoryGroup_Change() {
    var selectedBudgetCategoryGroupID = cboBudgetCategoryGroup.value
    var selectedBudgetCategoryGroup = GetBudgetCategoryGroupObject(selectedBudgetCategoryGroupID);
    var selectedBudgetCategoryGroupInfo = '';
    if (selectedBudgetCategoryGroup) {
        if (selectedBudgetCategoryGroup.GroupUnitsOfServiceOnServiceUserStatement == true) {
            selectedBudgetCategoryGroupInfo = '(units of service grouped on service user invoice)'
        }
    }
    SetInnerText(lblBudgetCategoryGroupInfo, selectedBudgetCategoryGroupInfo);
}

function cboUnitOfMeasure_Change() {
    var unitOfMeasureValue = cboUnitOfMeasure.value;
    hidUnitOfMeasure.value = unitOfMeasureValue;
    PopulateBudgetCategoryGroups(unitOfMeasureValue.substring(0, unitOfMeasureValue.indexOf('?')));
    cboUnitOfMeasure.previousSibling.disabled = cboUnitOfMeasure.disabled;
}

function txtRateDateFrom_Change(id) {
    
    //var idRep = id.replace(/_/g, "$");
    var txtRateDateFrom = GetElement(id + "_txtTextBox");
    var hidRateDateFrom = GetElement(id.replace("rateDateFrom", "rateDateFromH"));
    hidRateDateFrom.value = txtRateDateFrom.value;
}

function txtRateDateTo_Change(id) {
    //var idRep = id.replace(/_/g, "$");
    var txtRateDateTo = GetElement(id + "_txtTextBox");
    var hidRateDateTo = GetElement(id.replace("rateDateTo", "rateDateToH"));
    txtRateDateTo.disabled = false;
    hidRateDateTo.value = txtRateDateTo.value;
    txtRateDateTo.disabled = true;
}

function SetupRates() {
    var tbl, tBody, row, cell;
    var txtDateTo, butDateTo, hidDateTo;

    tbl = GetElement("tblRates");
    tBody = tbl.tBodies[0];

    // Prime the Rates table..
    for (index = 0; index < tBody.rows.length; index++) {
        row = tBody.rows[index];

        // Disable (the components of) the Date To field..
        cell = row.cells[1];
        txtDateTo = cell.getElementsByTagName("input")[0];
        txtDateTo.disabled = true;
        butDateTo = cell.getElementsByTagName("input")[1];
        butDateTo.disabled = true;
        hidDateTo = cell.getElementsByTagName("input")[2];
        
        // Setup additional cost checkbox and drop down
        cell = row.cells[6];
        chkAdditionalCostClicked(cell.getElementsByTagName("input")[0], cell.getElementsByTagName("select")[0].id);
        
    }
}

function SetUnitOfMeasureBySystemType(value, disabled) {    
    
    if (value != null && value.length > 0) {
        for (var i = 0; i <= cboUnitOfMeasure.options.length - 1; i++) {
            lPos = cboUnitOfMeasure.options[i].value.indexOf(":" + value);
            if (lPos != -1) {
                if (isInitialising == false) {
                    cboUnitOfMeasure.value = cboUnitOfMeasure.options[i].value;
                    break;
                }                
            }
        }
        cboUnitOfMeasure.value = cboUnitOfMeasure.value;
    } else {
        if (isInitialising == false) {
            cboUnitOfMeasure.value = '';
        }
    }
    
    if (isInEntryMode == false) {
        disabled = true;
    }
    
    cboUnitOfMeasure.disabled = disabled;
    cboUnitOfMeasure_Change();
}

function PopulateBudgetCategoryGroups(uomId) {
    cboBudgetCategoryGroup.disabled = true;
    cboBudgetCategoryGroup.length = 0;
    if (uomId != "") {        
        var option;

        if (isInEntryMode == true) {
            cboBudgetCategoryGroup.disabled = false;
        } else {
            cboBudgetCategoryGroup.disabled = true;
        }        

        option = document.createElement("option");
        cboBudgetCategoryGroup.options.add(option);
        option.text = " ";
        option.value = "";       
        
        if (budgetCategoryGroupCollection != null) {
            var currentBudgetCategoryGroup;
            var budgetCategoryGroupCollectionLength = budgetCategoryGroupCollection.length;
            for (var i = 0; i < budgetCategoryGroupCollectionLength; i++) {
                currentBudgetCategoryGroup = budgetCategoryGroupCollection[i];
                if (currentBudgetCategoryGroup.ID == currentBudgetCategoryGroupValue || (currentBudgetCategoryGroup.Redundant == false && (currentBudgetCategoryGroup.UnitOfMeasureID == uomId || currentBudgetCategoryGroup.UnitOfMeasureID == 0))) {
                    option = document.createElement("option");
                    cboBudgetCategoryGroup.options.add(option);
                    option.text = currentBudgetCategoryGroup.Description;
                    option.value = currentBudgetCategoryGroup.ID;
                    if (currentBudgetCategoryGroup.ID == currentBudgetCategoryGroupValue) {
                        cboBudgetCategoryGroup.value = currentBudgetCategoryGroupValue;
                    }
                }
            }
        }
    }
    cboBudgetCategoryGroup_Change();   
}

function GetBudgetCategoryGroupObject(id) {
    if (budgetCategoryGroupCollection != null) {
        var budgetCategoryGroupCollectionLength = budgetCategoryGroupCollection.length;
        for (var j = 0; j < budgetCategoryGroupCollection.length; j++) {
            if (budgetCategoryGroupCollection[j].ID == id) {
                return budgetCategoryGroupCollection[j];
            }
        }
    }
}

function InPlaceServiceTypeSelector_GetQueryObject(parentControlID) {
    var query = new InPlaceServiceTypeSelector_Query();
    query.Redundant = false;
    query.ExcludeServiceCategories = [serviceType_NonRes];
    query.IncludeIds = [InPlaceServiceTypeSelector_GetSelectedID(parentControlID)];
    return query;
}

function InPlaceServiceTypeSelector_Changed_Parent(parentControlID, selectedObject) {
    
    var lPos, servTypeValue, servTypeSel, servTypeVisitBased;
    var searchStr, opt, curValue;
    var selectedServiceType;
    var unitOfMeasureBySystemType = "";
    var serviceCategoryDesc = "";
    var unitOfMeasureDisabled = false;
    var selectorEnabled = InPlaceServiceTypeSelector_IsEnabled(parentControlID);
    
    SetupControls();
    lblServiceCategory.innerText = "";

    if (selectedObject.ID != '' && selectedObject.ID > 0) {
        servTypeSel = selectedObject.ID;
        if (Boolean(selectedObject.VisitBased) == false) {
            servTypeVisitBased = "0";
        } else {
            servTypeVisitBased = "1";
        }        
    } else {
        servTypeSel = 0;
        servTypeVisitBased = "0";
    }

    // Refresh the Unit Of Measure dropdown with items based on the selected
    // service type's VisitBased setting..
    cboUnitOfMeasure.disabled = false;
    curValue = cboUnitOfMeasure.value;
    searchStr = "?" + servTypeVisitBased;
    cboUnitOfMeasure.options.length = 0;
    for (var i = 0; i <= uomDescs.length - 1; i++) {
        lPos = uomValues[i].indexOf(searchStr);
        if (lPos != -1 || uomValues[i] == "" || searchStr == "?0") {
            opt = document.createElement("option");
            cboUnitOfMeasure.options.add(opt);
            opt.text = uomDescs[i];
            opt.value = uomValues[i];

            if (uomValues[i] == curValue) {
                cboUnitOfMeasure.value = curValue;
            }
        }
    }

    selectedServiceType = selectedObject;

    if (servTypeSel > 0) {
        if (selectedServiceType != null) {
            serviceCategoryDesc = "(" + selectedServiceType.ServiceCategoryDescription + ")"
            if (parseInt(selectedServiceType.ServiceCategoryID) == serviceType_Res) {
                unitOfMeasureBySystemType = "6";
                unitOfMeasureDisabled = true;
            }
            else if (parseInt(selectedServiceType.ServiceCategoryID) == serviceType_Cash) {
                unitOfMeasureBySystemType = "2";
            }
        }
    }

    SetUnitOfMeasureBySystemType(unitOfMeasureBySystemType, unitOfMeasureDisabled);
    lblServiceCategory.innerText = serviceCategoryDesc;

    if (servTypeSel == 0) {    // none
        hidUnitOfMeasure.value = cboUnitOfMeasure.value;
    } else {
        if (servTypeVisitBased == "1") {
            // Visit-based service type, so set Unit of Measure to "Hours" and disable..
            for (var i = 0; i <= cboUnitOfMeasure.options.length - 1; i++) {
                lPos = cboUnitOfMeasure.options[i].value.indexOf(":1");
                if (lPos != -1) {
                    cboUnitOfMeasure.value = cboUnitOfMeasure.options[i].value;
                    break;
                }
            }
            hidUnitOfMeasure.value = cboUnitOfMeasure.value;
            cboUnitOfMeasure.disabled = true;
        } else {
            hidUnitOfMeasure.value = cboUnitOfMeasure.value;
        }
    }

    cboUnitOfMeasure_Change();
    cboBudgetCategoryGroup_Change();
    lblServiceType.disabled = !selectorEnabled;
    lblServiceCategory.disabled = !selectorEnabled;

}

function chkAdditionalCostClicked(sourceCheckBox, additionalCostCapID) {

    var additionalCostCapDropDown = GetElement(additionalCostCapID);

    if (additionalCostCapDropDown) {

        additionalCostCapDropDown.disabled = !sourceCheckBox.checked;

        if (sourceCheckBox.checked == false) {

            additionalCostCapDropDown.value = 0;

        }

    }

}

function chkUseActualCostClicked(sourceCheckBox, txtUnitRateID) {
    var txtUnitRate = GetElement(txtUnitRateID);

    if (txtUnitRate) {

        txtUnitRate.disabled = sourceCheckBox.checked;

        if (sourceCheckBox.checked == true) {

            txtUnitRate.value = 0;

        }

    }

}

addEvent(window, "load", Init);
