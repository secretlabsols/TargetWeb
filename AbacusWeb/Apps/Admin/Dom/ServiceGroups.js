var cboServiceGroupClassificationID, cboServiceGroupClassification;
var rblServiceCategoryID, rblServiceCategory;
var hidServiceCategoryID, hidServiceCategory;
var lblServiceCategoryID, lblServiceCategory;
var rblTemporaryOrPermanentID, rblTemporaryOrPermanent;
var serviceGroupClassificationCollection = new Array();
var careType_Cash = 0;
var careType_NonResidential = 1;
var careType_Residential = 2;
var careType_EarlyIntervention = 3;
var serviceCategory_Cash = 3;
var serviceCategory_NonResidential = 1;
var serviceCategory_Residential = 2;
var inEditMode = false;
var isInitialising = false;
var isServiceGroupInUse = false;

function ServiceGroupClassification(id, careType) {
    this.ID = id;
    this.CareType = careType;
}

function SetServiceCategoryEnabled(enabled) {
    if (rblServiceCategory) {
        if (inEditMode == true) {
            var radios = rblServiceCategory.getElementsByTagName("input");
            for (var j = 0; j < radios.length; j++) {
                radios[j].disabled = !enabled;
            }
            var labels = rblServiceCategory.getElementsByTagName("label");
            for (var j = 0; j < labels.length; j++) {
                labels[j].disabled = !enabled;
            }
            lblServiceCategory.disabled = !enabled;
        }        
    }
}

function SetServiceCategoryEnabledByValue(enabled, value) {
    if (rblServiceCategory) {
        if (inEditMode == true) {
            var radios = rblServiceCategory.getElementsByTagName("input");
            for (var j = 0; j < radios.length; j++) {
                if (radios[j].value == value) {
                    radios[j].disabled = !enabled;
                }
            }
        }        
    }
}

function GetServiceCategoryValue() {
    if (rblServiceCategory) {
        var radios = rblServiceCategory.getElementsByTagName("input");
        for (var j = 0; j < radios.length; j++) {
            if (radios[j].checked == true) {
                return radios[j].value;
            }
        }
    }
    return '';
}

function SetServiceCategoryValue(value) {
    if (rblServiceCategory) {
        var radios = rblServiceCategory.getElementsByTagName("input");
        var radioValueFound = false;
        if (value != null) {
            for (var j = 0; j < radios.length; j++) {
                if (radios[j].value == value) {
                    radios[j].checked = true;
                    rblServiceCategory_Changed(radios[j]);
                    radioValueFound = true;
                }
                else {
                    radios[j].checked = false;
                }
            }
            hidServiceCategory.value = value;            
        }
        if (radioValueFound == false) {
            rblServiceCategory_Changed(null);
        }       
    }
}

function SetTemporaryOrPermanentValue(value) {
    var radios = rblTemporaryOrPermanent.getElementsByTagName("input");
    var radioValueFound = false;
    if (value != null) {
        for (var j = 0; j < radios.length; j++) {
            if (radios[j].value == value) {
                radios[j].checked = true;
            }
        }
    }
}

function GetServiceGroupClassificationObject(id) {
    if (serviceGroupClassificationCollection != null) {
        for (var j = 0; j < serviceGroupClassificationCollection.length; j++) {
            if (serviceGroupClassificationCollection[j].ID == id) {
                return serviceGroupClassificationCollection[j];
            }
        }
    }
}

function cboServiceGroupClassification_Changed() {
    var selectedServiceGroupClassificationID = cboServiceGroupClassification.value;
    var selectedServiceGroupClassification = GetServiceGroupClassificationObject(selectedServiceGroupClassificationID);
    var serviceCategoryEnabled = false;
    var serviceCategoryValue = '';
 
    if (isInitialising == true) {
        serviceCategoryValue = GetServiceCategoryValue();
    } else {
        if (selectedServiceGroupClassification != null) {
            if (selectedServiceGroupClassification.CareType == careType_Cash) {
                serviceCategoryValue = serviceCategory_Cash;
            }
            else if (selectedServiceGroupClassification.CareType == careType_NonResidential) {
                serviceCategoryValue = serviceCategory_NonResidential;
            }
            else if (selectedServiceGroupClassification.CareType == careType_Residential) {
                serviceCategoryValue = serviceCategory_Residential;
            }
            else if (selectedServiceGroupClassification.CareType == careType_EarlyIntervention) {
                serviceCategoryEnabled = true;
            }
        }
    }
    SetServiceCategoryEnabled(serviceCategoryEnabled);
    SetServiceCategoryValue(serviceCategoryValue);
    if (selectedServiceGroupClassification != null) {
        if (selectedServiceGroupClassification.CareType == careType_EarlyIntervention) {
            SetServiceCategoryEnabled(true);
            SetServiceCategoryEnabledByValue(false, serviceCategory_Cash);
        }
    }
}

function rblServiceCategory_Changed(cb) {
    var disableTemporaryOrPermanent = true;
    if (cb != null) {
        if (cb.value == serviceCategory_Cash || cb.value == serviceCategory_Residential) {
            if (cb.value == serviceCategory_Residential) {
                disableTemporaryOrPermanent = false;
            }
        }
        hidServiceCategory.value = cb.value;
    } else {
        hidServiceCategory.value = '';
        disableTemporaryOrPermanent = true;
    } 
    if (disableTemporaryOrPermanent == true) {
        rblTemporaryOrPermanent.style.display = 'none';
    } else {
        rblTemporaryOrPermanent.style.display = 'block';
    }
    if (isInitialising == false) {
        SetTemporaryOrPermanentValue('0');
    } 
}

function Init() {
    isInitialising = true;
    rblServiceCategory = GetElement(rblServiceCategoryID);
    cboServiceGroupClassification = GetElement(cboServiceGroupClassificationID);
    hidServiceCategory = GetElement(hidServiceCategoryID);
    lblServiceCategory = GetElement(lblServiceCategoryID);
    rblTemporaryOrPermanent = GetElement(rblTemporaryOrPermanentID);
    cboServiceGroupClassification_Changed();
    if (isServiceGroupInUse == true) {
        cboServiceGroupClassification.disabled = true;
        cboServiceGroupClassification.previousSibling.disabled = true;        
        lblServiceCategory.disabled = true;
        rblServiceCategory.disabled = true;
        rblTemporaryOrPermanent.disabled = true; 
    }
    isInitialising = false;
}

addEvent(window, "load", Init);


