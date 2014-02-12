var totalChkBoxes, gvControlID, hidSelectedObjectIndexID, btnSelectedID;
var copyToCareWorker, careWorkerReference;

function Init() {
    var hidSelectedObjectIndex = GetElement(hidSelectedObjectIndexID);
    //alert(hidSelectedObjectIndex.value);
}

function checkAllBoxes() {

    var gvControl = GetElement(gvControlID);
    var hidSelectedObjectIndex = GetElement(hidSelectedObjectIndexID);
    hidSelectedObjectIndex.value = "";
    var btnSelected = GetElement(btnSelectedID);
    var gvChkBoxControl = "chkObjectIndex";
    var inputTypes = gvControl.getElementsByTagName("input");
    for (var i = 0; i < inputTypes.length; i++) {
        //if the input type is a checkbox and the id of it is what we set above
        //then check or uncheck according to the main checkbox in the header template
        if (inputTypes[i].type == 'checkbox' && inputTypes[i].id.indexOf(gvChkBoxControl, 0) >= 0) {
            inputTypes[i].checked = true;
            if (hidSelectedObjectIndex.value.length == 0) {
                hidSelectedObjectIndex.value = inputTypes[i - 1].value;
            }
            else {
                hidSelectedObjectIndex.value = hidSelectedObjectIndex.value + "|" + inputTypes[i - 1].value;
            }
        }
    }
    //alert(hidSelectedObjectIndex.value);
    btnSelected.disabled = false;
}

function UnCheckAllBoxes() {
    var btnSelected = GetElement(btnSelectedID);
    var gvControl = GetElement(gvControlID);
    var gvChkBoxControl = "chkObjectIndex";
    var inputTypes = gvControl.getElementsByTagName("input");
    var hidSelectedObjectIndex = GetElement(hidSelectedObjectIndexID);
    for (var i = 0; i < inputTypes.length; i++) {
        //if the input type is a checkbox and the id of it is what we set above
        //then check or uncheck according to the main checkbox in the header template
        if (inputTypes[i].type == 'checkbox' && inputTypes[i].id.indexOf(gvChkBoxControl, 0) >= 0) {
            inputTypes[i].checked = false;
        }
    }
    hidSelectedObjectIndex.value = "";
    btnSelected.disabled = true;
}

function CheckedChanged(chkid, hidid) {
    var chk = GetElement(chkid);
    var hid = GetElement(hidid);
    if (chk.checked == true) {
        AddRemoveVisitDetailIndex(hid.value, true);
    }
    else {
        AddRemoveVisitDetailIndex(hid.value, false);
    }
}

function AddRemoveVisitDetailIndex(value, addValue) {
    var btnSelected = GetElement(btnSelectedID);
    var hidSelectedObjectIndex = GetElement(hidSelectedObjectIndexID);
    if (addValue) {
        if (hidSelectedObjectIndex.value.length == 0) {
            hidSelectedObjectIndex.value = value;
        }
        else {
            hidSelectedObjectIndex.value = hidSelectedObjectIndex.value + "|" + value;
        }
    }
    else {
        var str = hidSelectedObjectIndex.value;
        var list = str.split("|");
        str = "";
        for (var i = 0; i < list.length; i++) {
            if (list[i] != value) {
                if (str.length == 0) {
                    str = list[i];
                }
                else {
                    str = str + "|" + list[i]
                }
            }
        }
        hidSelectedObjectIndex.value = str;
    }
    // enable disable Ok
    if (hidSelectedObjectIndex.value.length == 0) {
        btnSelected.disabled = true;
    }
    else {
        btnSelected.disabled = false;
    }
    
    //alert(hidSelectedObjectIndex.value);
}


function btnClose_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}

function btnSelected_Click() {
    var hidSelectedObjectIndex = GetElement(hidSelectedObjectIndexID);

    var parentWindow = GetParentWindow();
    parentWindow.HideModalDIV();
    //if (selectedDcrDomContractId != 0)
    parentWindow.InPlaceCopyVisit_ItemSelected(careWorkerReference, copyToCareWorker, hidSelectedObjectIndex.value);
    window.parent.close();
}

addEvent(window, "load", Init);