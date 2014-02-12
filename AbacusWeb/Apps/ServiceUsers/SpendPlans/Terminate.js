var dteDateToID, dteDateTo;
var cboEndReasonID, cboEndReason;
var isTerminated = false;

function Init() {
    dteDateTo = GetElement(dteDateToID);
    cboEndReason = GetElement(cboEndReasonID);
    if (isTerminated == true) {
        Terminated();
    }
}

function btnCancel_Click() {
    GetParentWindow().HideModalDIV();
    window.parent.close();
}

function Terminated() {
    var parentWindow = GetParentWindow();
    var selectedDateTo = dteDateTo.value;
    var selectedEndReason = cboEndReason.options[cboEndReason.selectedIndex].text;
    parentWindow.HideModalDIV();
    parentWindow.SpendPlan_Terminated(selectedDateTo, selectedEndReason);
    window.parent.close();
}

addEvent(window, "load", Init);
addEvent(window, "unload", DialogUnload);