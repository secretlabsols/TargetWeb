var txtMinsFrom1, txtMinsTo1, cboCalcMethod1;
var txtMinsFrom2, txtMinsTo2, cboCalcMethod2;
var txtMinsFrom3, txtMinsTo3, cboCalcMethod3;
var txtMinsFrom1ID, txtMinsTo1ID, cboCalcMethod1ID;
var txtMinsFrom2ID, txtMinsTo2ID, cboCalcMethod2ID;
var txtMinsFrom3ID, txtMinsTo3ID, cboCalcMethod3ID;
var CALCMETHOD_NOTSET = "0";
var MAX_BYTE_VALUE = 255;

function Init() {
    SetFieldHooks();
}
    
function txtMinsTo1_OnChange(id) {
    var newFromValue, newToValue;

    SetFieldHooks();
    cboCalcMethod1.disabled = false;

    if (txtMinsTo1.value == "")
    {
        // Blank out and disable all fields in rows 2 and 3..
        txtMinsFrom2.value = "";
        txtMinsTo2.value = "";
        txtMinsTo2.setAttribute('readOnly', 'readOnly');
        cboCalcMethod2.value = CALCMETHOD_NOTSET;
        cboCalcMethod2.disabled = true;

        txtMinsFrom3.value = "";
        txtMinsTo3.value = "";
        cboCalcMethod3.value = CALCMETHOD_NOTSET;
        cboCalcMethod3.disabled = true;

    } else
    {
        txtMinsTo1.value = Minimum(Maximum(parseInt(txtMinsTo1.value), 0), MAX_BYTE_VALUE);
        newFromValue = parseInt(txtMinsFrom1.value);
        newToValue = parseInt(txtMinsTo1.value);

        if (newFromValue > newToValue) 
        {
            alert('The \'Minutes To\' value specified is less than the corresponding Minutes From.');
            txtMinsTo1.focus();

        } else 
        {
            //txtMinsTo2.setAttribute('readOnly', false);
            cboCalcMethod2.disabled = false;

            // Ensure that rows 2 and row 3 are populated correctly..
            newFromValue = parseInt(txtMinsTo1.value) + 1;
            txtMinsFrom2.value = newFromValue.toString();
            if (txtMinsTo2.value != "")
            {
                newToValue = Maximum(newFromValue, parseInt(txtMinsTo2.value));
                txtMinsTo2.value = newToValue.toString();

                txtMinsTo3.setAttribute('readOnly', false);
                cboCalcMethod3.disabled = false;
                newFromValue = parseInt(txtMinsTo2.value) + 1;
                txtMinsFrom3.value = newFromValue.toString();
                txtMinsTo3.value = "";
            }
        }
    }
}

function txtMinsTo2_OnChange(id) {
    var newFromValue, newToValue;

    SetFieldHooks();
    cboCalcMethod2.disabled = false;

    if (txtMinsTo2.value == "") 
    {
        // Blank out and disable all fields in row 3..
        txtMinsFrom3.value = "";
        txtMinsTo3.value = "";
        cboCalcMethod3.value = CALCMETHOD_NOTSET;
        cboCalcMethod3.disabled = true;

    } else 
    {
        txtMinsTo2.value = Minimum(Maximum(parseInt(txtMinsTo2.value), 0), MAX_BYTE_VALUE);
        newFromValue = parseInt(txtMinsFrom2.value);
        newToValue = parseInt(txtMinsTo2.value);

        if (newFromValue > newToValue) 
        {
            alert('The \'Minutes To\' value specified is less than the corresponding Minutes From.');
            txtMinsTo2.focus();

        } else 
        {
            cboCalcMethod3.disabled = false;
            newFromValue = parseInt(txtMinsTo2.value) + 1;
            txtMinsFrom3.value = newFromValue.toString();
            txtMinsTo3.value = "";
        }
    }
}

function cboCalcMethod2_OnChange(id) {
    SetFieldHooks();
    //if (cboCalcMethod2.value == CALCMETHOD_NOTSET) 
    //{
    //    txtMinsTo1.setAttribute('readOnly', false);

    //} else 
    //{
    //    txtMinsTo1.setAttribute('readOnly', 'readOnly');
    //}    
}

function cboCalcMethod3_OnChange(id) {
    SetFieldHooks();
    //if (cboCalcMethod3.value == CALCMETHOD_NOTSET) 
    //{
    //    txtMinsTo2.setAttribute('readOnly', false);

    //} else 
    //{
    //    txtMinsTo2.setAttribute('readOnly', 'readOnly');
    //}
}

function Maximum(value1, value2) {
    if (value1 > value2) {
        return value1;
    } else {
        return value2;
    }
}

function Minimum(value1, value2) {
    if (value2 > value1) {
        return value1;
    } else {
        return value2;
    }
}

function validatorServiceUserMinutesCalc_ClientValidate(source, args) {
    var intMinsFrom, intMinsTo;

    SetFieldHooks();

    args.IsValid = true;

    // No validation needed if this field is disabled..
    if (cboCalcMethod1.disabled == true) {
        return;
    }
    
    // Any one of the following conditions within the control is invalid..
    if (txtMinsTo1.value == "" && (cboCalcMethod1.value == CALCMETHOD_NOTSET)) {
        args.IsValid = false;
        SetInnerText(source, "A Calculation Method is required for the first range.");
        return;
    }

    if (cboCalcMethod2.disabled == false) {
        if (txtMinsFrom2.value != "" && (cboCalcMethod2.value == CALCMETHOD_NOTSET)) {
            args.IsValid = false;
            SetInnerText(source, "A Calculation Method is required for the second range.");
            return;
        }

        if ((txtMinsTo1.value == "") && (cboCalcMethod2.value != CALCMETHOD_NOTSET)) {
            args.IsValid = false;
            SetInnerText(source, "A Minutes To value is required for the first range.");
            return;
        }
    }

    if (cboCalcMethod3.disabled == false) {
        if (txtMinsFrom3.value != "" && (cboCalcMethod3.value == CALCMETHOD_NOTSET)) {
            args.IsValid = false;
            SetInnerText(source, "A Calculation Method is required for the third range.");
            return;
        }

        if ((txtMinsTo1.value == "" || txtMinsTo2.value == "") && (cboCalcMethod3.value != CALCMETHOD_NOTSET)) {
            args.IsValid = false;
            SetInnerText(source, "A Minutes To value is required for both the first two ranges.");
            return;
        }
    }

    intMinsFrom = convertToInt(txtMinsFrom2.value);
    if (intMinsFrom > MAX_BYTE_VALUE) {
        args.IsValid = false;
        SetInnerText(source, "Minutes values exceeding 255 are not permitted.");
        return;
    }

    intMinsFrom = convertToInt(txtMinsFrom3.value);
    if (intMinsFrom > MAX_BYTE_VALUE) {
        args.IsValid = false;
        SetInnerText(source, "Minutes values exceeding 255 are not permitted.");
        return;
    }

    intMinsFrom = convertToInt(txtMinsFrom1.value);
    intMinsTo = convertToInt(txtMinsTo1.value);
    if (intMinsFrom > intMinsTo) {
        args.IsValid = false;
        SetInnerText(source, "The Minutes To is less than the Minutes From for the first range.");
        return;
    }
    
    intMinsFrom = convertToInt(txtMinsFrom2.value);
    intMinsTo = convertToInt(txtMinsTo2.value);
    if (intMinsFrom > intMinsTo) {
        args.IsValid = false;
        SetInnerText(source, "The Minutes To is less than the Minutes From for the second range.");
        return;
    }
}

function SetFieldHooks() {
    txtMinsFrom1 = GetElement(txtMinsFrom1ID + "_txtTextBox");
    txtMinsFrom2 = GetElement(txtMinsFrom2ID + "_txtTextBox");
    txtMinsFrom3 = GetElement(txtMinsFrom3ID + "_txtTextBox");
    txtMinsTo1 = GetElement(txtMinsTo1ID + "_txtTextBox");
    txtMinsTo2 = GetElement(txtMinsTo2ID + "_txtTextBox");
    txtMinsTo3 = GetElement(txtMinsTo3ID + "_txtTextBox");
    cboCalcMethod1 = GetElement(cboCalcMethod1ID + "_cboDropDownList");
    cboCalcMethod2 = GetElement(cboCalcMethod2ID + "_cboDropDownList");
    cboCalcMethod3 = GetElement(cboCalcMethod3ID + "_cboDropDownList");
}

function convertToInt(stringValue) {
    if (stringValue == "") {
        return MAX_BYTE_VALUE;
    } else {
        return parseInt(stringValue);
    }
}

addEvent(window, "load", Init);