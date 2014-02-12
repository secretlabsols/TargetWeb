var CareTypeSelector_showRes, CareTypeSelector_enableRes, CareTypeSelector_showNonRes;
var CareTypeSelector_enableNonRes, CareTypeSelector_showDP, CareTypeSelector_enableDP;
var CareTypeSelector_optNonResID, CareTypeSelector_optResID, CareTypeSelector_optDPID;
var optRes, optNonRes, optDP;
var divResID, divNonResID, divDPID, defaultValue;

function Init() {
    var divRes, divNonRes, divDP;
    optRes = GetElement(CareTypeSelector_optResID);
    optNonRes = GetElement(CareTypeSelector_optNonResID);
    optDP = GetElement(CareTypeSelector_optDPID);
    divRes = GetElement(divResID);
    divNonRes = GetElement(divNonResID);
    divDP = GetElement(divDPID);

    optRes.disabled = !CareTypeSelector_enableRes;
    optNonRes.disabled = !CareTypeSelector_enableNonRes;
    optDP.disabled = !CareTypeSelector_enableDP;
    if (CareTypeSelector_showRes == false) {
        divRes.style.display = "none";
    } else {
        divRes.style.display = "block";
    }
    if (CareTypeSelector_showNonRes == false) {
        divNonRes.style.display = "none";
    } else {
        divNonRes.style.display = "block";
    }
    if (CareTypeSelector_showDP == false) {
        divDP.style.display = "none";
    } else {
        divDP.style.display = "block";
    } 
    
    if (defaultValue == 1) {
        optRes.checked = true;
        optRes.defaultChecked = true;
    } else if (defaultValue == 2) {
        optNonRes.checked = true;
        optNonRes.defaultChecked = true;
    } else if (defaultValue == 3) {
        optDP.checked = true;
        optDP.defaultChecked = true;
    }
    
}

function CareTypeSelector_Show(CallBackFunctionName) {
    var d = new Target.Web.Dialog.Msg();
    var addDetailDialogContentContainer, addDetailDialogContent;

    d.SetType(4);   // OK/Cancel
    d.SetCallback(CallBackFunctionName);
    d.SetTitle("Choose a Care Type");
    
    addDetailDialogContentContainer = GetElement("divDetailContainer");
    addDetailDialogContent = addDetailDialogContentContainer.getElementsByTagName("DIV")[0];

    d.ClearContent();
    d._content.innerHTML = addDetailDialogContent.innerHTML;
    
    d.ShowCloseLink(false);
    d.Show();

}

function CareTypeSelector_GetSelectedCareType(args) {

    if (optRes.checked == true) {
        return 1;
    } else if (optNonRes.checked == true) {
        return 2;
    } else if (optDP.checked == true) {
        return 3;
    }
}

function CareTypeSelector_Hide(args) {
    var d = args[0];
    d.Hide();
}

addEvent(window, "load", Init);