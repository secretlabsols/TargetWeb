//var txtExternalAccount, txtHidExternalAccountID, InPlaceExternalAccount_currentID;
var providerID, txtHidReferenceID, txtHidNameID, txtHidID, tabStripID, txtCopyToNameID, txtCopyToReferenceID, txtCopyVisitsID, OriginalValueChanged, EnableAddTab, backUrl;
var txtHidReference, txtHidName, txtHid;
var isPopUpOpen = false;
var pSWE, dteWeekEndingId, dteWeekEnding, txtNoOfVisitsId, txtNoOfVisits, txtNoOfHoursId, txtNoOfHours;
var pScheduleId;
var btnstd;
var txtEditCareProviderId_ClientId,  txtEditCareProviderName_ClientId,  txtEditCareProviderRef_ClientId ;
var txtEditCareProviderId, txtEditCareProviderName, txtEditCareProviderRef;
var txtExistingCareWorkerList_ClientId, txtExistingCareWorkerList;

function Init() {
    
    var tabcount = GetElement("txthidtabCount");

    dteWeekEnding = GetElement(dteWeekEndingId + "_txtTextBox");
    txtNoOfVisits = GetElement(txtNoOfVisitsId + "_txtTextBox");
    txtNoOfHours = GetElement(txtNoOfHoursId + "_txtTextBox");

    txtEditCareProviderRef = GetElement(txtEditCareProviderRef_ClientId);
    txtEditCareProviderName = GetElement(txtEditCareProviderName_ClientId);
    txtEditCareProviderId = GetElement(txtEditCareProviderId_ClientId);
    txtExistingCareWorkerList = GetElement(txtExistingCareWorkerList_ClientId);
    
    
    pSWE = GetQSParam(document.location.search, "pSWE");
    if (pSWE != null) {
        if (pSWE.length > 1) {
            if (dteWeekEnding.value.length == 0) {
                dteWeekEnding.value = pSWE;
            }
        } 
    }

    pScheduleId = GetQSParam(document.location.search, "pScheduleId");
    //$find("collapsiblePanelInfo").collapsePanel(true);


    // disable weekending.
    dteWeekEnding.disabled = true;
    txtNoOfVisits.disabled = true;
    txtNoOfHours.disabled = true;

    // Recalculate hours to update the No. of Hours
    ReCalculateHoursForAll();
    //alert(btnstd + "_btnEdit");
    if ($('#' + btnstd + '_btnEdit').length || ($('#' + btnstd + '_btnSave').length == 0 && $('#' + btnstd + '_btnEdit').length == 0)) {
        hidebuttonsInViewMode();
    }

}

function __doPostBack(eventTarget, eventArgument) {
    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
        theForm.__EVENTTARGET.value = eventTarget;
        theForm.__EVENTARGUMENT.value = eventArgument;
        theForm.submit();
    }
    valuechanged();
  }

function InPlaceCareWorkerSelector_ItemCancelled(tabindex) {
      // display the correct tab
      tabStrip = $find(tabStripID);
      tabStrip.set_activeTabIndex(tabindex-1);
      isPopUpOpen = false;
}
function InPlaceCareWorkerSelector_ItemSelected(careWorkerID, careWorkerReference, careWorkerName, editMode) {

    txtHidReference = GetElement(txtHidReferenceID);
    txtHidCWId = GetElement(txtHidID);
    txtHidName = GetElement(txtHidNameID);

    txtHidCWId.value = careWorkerID;
    txtHidName.value = careWorkerName;
    txtHidReference.value = careWorkerReference;
    
    if (editMode === "true") {
        __doPostBack('__Page', "editCareWorker");   
    }
    else {

        var tabIndex = CareWorkerAlreadyExists(txtHidName.value);
        var tabStrip = $find(tabStripID);
        var TabCount = tabStrip.get_tabs().length;
        if (tabIndex < 0) { // which means that the selected care worker was not in any tab panel
            var tabContainer = GetElement(tabStripID);
            if ((TabCount == 2)
                &&
             (TabCount == 0)
             ) {
                tabStrip = $find(tabStripID);
                tabStrip.set_activeTabIndex(0);
            }
            __doPostBack('__Page', "customPostBack");
        }
        else {
            //alert("already there");
            // display the correct tab
            tabStrip = $find(tabStripID);
            tabStrip.set_activeTabIndex(tabIndex);
            isPopUpOpen = false;
        }
        
    }
    
}

function InPlaceCopyVisit_ItemSelected(careWorkerReference, copyToCareWorker, visits) {
    txtCopyToName = GetElement(txtCopyToNameID);
    txtCopyToName.value = copyToCareWorker;
    txtCopyToReference = GetElement(txtCopyToReferenceID);
    txtCopyToReference.value = careWorkerReference;
    txtCopyVisits = GetElement(txtCopyVisitsID);
    txtCopyVisits.value = visits;

    __doPostBack('__Page', "copyVisits");
}

function CareWorkerAlreadyExists(cwName) {
    var tabIndex = -1;
    var tabStrip = $find(tabStripID);
    var TabCount = tabStrip.get_tabs().length;
    var tabContainer = GetElement(tabStripID);

    if (TabCount > 2) {
        for (i = 0; i < TabCount; i++) {

            if ($find(tabStripID)._tabs[0]._header.children[0].innerText.trim() == txtHidName.value) {
                tabIndex = i;
            }
        }
    }
    return tabIndex;
}

String.prototype.trim = function() {
    return this.replace(/^\s+|\s+$/g, "");
};

function ChangeSelectedTab(tabIndex) {
    
}

function clientClicked(sender, e) {
      var hidAddValue = GetElement(EnableAddTab);
      var tabIndex = sender.get_activeTabIndex();
      var tabContainer = GetElement(tabStripID);


      
      var tabStrip = $find(tabStripID);
      var lastTabIndex = tabStrip.get_tabs().length - 1;

//      var lastTabIndex = tabContainer.childNodes[0].children.length - 1;
      if (lastTabIndex != tabIndex) {
          return;
      }
      // if page is not in edit mode donto open the pop up
      if (hidAddValue.value == "1") {
          isPopUpOpen = true;
          var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT +
                    "AbacusExtranet/Apps/InPlaceSelectors/CareWorkerSelectorManualInvoice.aspx?estabid=" + providerID +
                    "&editMode=" + false +
                    "&existingIds=" + txtExistingCareWorkerList.value +
                    "&tabindex=" + tabIndex;
          var dialog = OpenDialog(url, 45, 32, window);
      }
      else {
          //alert("Select edit mode to add a new care worker.");
          sender.set_activeTabIndex(0)
      }
     
}

function btnCopy_Click(copyToCareWorker, copyToCareWorkerReference, copyToDayOfWeek, vsCollapsablePanel) {
    var txtCopyToWeekDay = GetElement("txtCopyToWeekDay");
    txtCopyToWeekDay.value = copyToDayOfWeek;
    var txtCollapsablePanel = GetElement("txtCollapsablePanel");
    txtCollapsablePanel.value = vsCollapsablePanel
    //alert(txtCollapsablePanel.value);
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/InPlaceSelectors/InPlaceInvoiceVisitsCopier.aspx?copyToDayOfWeek=" + copyToDayOfWeek + "&copyToCareWorker=" + copyToCareWorker + "&copyToCareWorkerReference=" + copyToCareWorkerReference;
    var dialog = OpenDialog(url, 70, 32, window);
    return false;
}

function hidebuttonsInViewMode() {
    var btnCopylist = $("[id$='btnCopy']");
    for (var i = 0; i <= btnCopylist.length - 1; i++) {
        $('#' + btnCopylist[i].id).hide();
    }

    var btnAddlist = $("[id$='btnAdd']");
    for (var i = 0; i <= btnAddlist.length - 1; i++) {
        $('#' + btnAddlist[i].id).hide();
    }

    var btnDellist = $("[id$='btnDel']");
    for (var i = 0; i <= btnDellist.length - 1; i++) {
        $('#' + btnDellist[i].id).hide();
    }
    
}


function btnCancel_Click() {
    var change = "true";
    var url = "";
    var copyUrl = "";
    if (GetQSParam(document.location.search, "copyUrl")) {
        copyUrl = GetQSParam(document.location.search, "copyUrl");
    }
    var hidChangeValue = GetElement(OriginalValueChanged);
    if (hidChangeValue.value) {
        change = confirm("Changes have been made to the set of visits during the current editing session. These changes will be lost if you continue with this action. Please confirm that you understand this and that you wish to continue regardless.")
    }
    if (change) {
        // call from Add (new & copy)
        if (backUrl.length == 0 && copyUrl.length != 0) {
            url = GetQSParam(document.location.search, "copyUrl");
            url = unescape(url);
        }
        // from view incoices
        else if (backUrl.length == 0 && copyUrl.length == 0) {
            url = unescape(GetQSParam(document.location.href, "backUrl"));
        }
        else {
            url = SITE_VIRTUAL_ROOT + backUrl; //GetQSParam(document.location.search, "backUrl");
        }
        document.location.href = url;
    }  
    
}

function btnDelete_Click() {

    return confirm("This set of manually entered visits will be deleted. Please click OK to indicate that this is your intention, otherwise click Cancel");
}

function btnBack_Click(isBack) {
    var change = "true";
    var url = "";
    var copyUrl = "";
    if (GetQSParam(document.location.search, "copyUrl")) {
        copyUrl = GetQSParam(document.location.search, "copyUrl");
    }
    var hidChangeValue = GetElement(OriginalValueChanged);
    if (hidChangeValue.value) {
        change = confirm("Changes have been made to the set of visits during the current editing session. These changes will be lost if you continue with this action. Please confirm that you understand this and that you wish to continue regardless.")
    }

    if (!isBack) {
        return change;
    }
    else {
        if (change) {
            // call from Add (new & copy)
            if (backUrl.length == 0 && copyUrl.length != 0) {
                url = GetQSParam(document.location.search, "copyUrl");
                url = unescape(url);
            }
            // from view incoices
            else if (backUrl.length == 0 && copyUrl.length == 0) {
                url = unescape(GetQSParam(document.location.href, "backUrl"));
            }
            else {
                url = SITE_VIRTUAL_ROOT + backUrl; //GetQSParam(document.location.search, "backUrl");
            }
            document.location.href = url;
        }    
    }

}


function txtNoOfHours_Changed() {
    valuechanged();
}
function txtNoOfVisits_Changed() {
    valuechanged();
}
function txtDirectIncome_Changed() {
    valuechanged();
}
function txtPaymentClaimed_Changed() {
    valuechanged();
}
function txtReference_Changed() {
    valuechanged();
}
function dteWeekEnding_Changed() {
    valuechanged();
}

function valuechanged() {
    //alert("value changed");
    var hidChangeValue = GetElement(OriginalValueChanged);
    hidChangeValue.value = "true";
}


function calculateTime(ddlHoursST_ClientID, 
                       ddlMinutesST_ClientID, 
                       ddlHoursET_ClientID, 
                       ddlMinutesET_ClientID, 
                       ddlHoursDC_ClientID,
                       ddlMinutesDC_ClientID,
                       ddlHoursAD_ClientID,
                       ddlMinutesAD_ClientID) {
                       
    var currentCpPrefix = ddlHoursDC_ClientID.split("_")[0] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[1] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[2] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[3] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[4];


    var startTimeHours, startTimeMinutes, endTimeHours, endTimeMinutes;
    startTimeHours = GetElement(ddlHoursST_ClientID).value;
    startTimeMinutes = GetElement(ddlMinutesST_ClientID).value;
    endTimeHours = GetElement(ddlHoursET_ClientID).value;
    endTimeMinutes = GetElement(ddlMinutesET_ClientID).value;

    var startTime = new Date("September 5, 2005 " + startTimeHours + ":" + startTimeMinutes + ":00");
    var endTime = new Date("September 5, 2005 " + endTimeHours + ":" + endTimeMinutes + ":00")

    var diff = new Date();
    if (endTime < startTime) {
        endTime.setDate(endTime.getDate() + 1);
    }

    diff.setTime(endTime - startTime);
    //alert(diff.getHours() + ":" + diff.getMinutes());
    GetElement(ddlHoursDC_ClientID).value = diff.getHours();
    GetElement(ddlMinutesDC_ClientID).value = diff.getMinutes();
    GetElement(ddlHoursAD_ClientID).value = diff.getHours();
    GetElement(ddlMinutesAD_ClientID).value = diff.getMinutes();


    ReCalculateHoursForAll();
    CalculateHours(ddlHoursDC_ClientID);

}


function CalculateHours(ddlHoursDC_ClientID) {
    var gridId = ddlHoursDC_ClientID.split("_")[0] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[1] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[2] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[3] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[4] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[5] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[6] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[7];


    var currentGridRows = $("#" + gridId + " tr").length;
   
    var currentCpPrefix = ddlHoursDC_ClientID.split("_")[0] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[1] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[2] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[3] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[4];

    var currentDayName = ddlHoursDC_ClientID.split("_")[6].replace("VisitGrid", "");

    var currentDayMinutes = 0;
    var currentDayHours = 0;
    var currentDayvisits = 0;
    
    for (i = 2; i <= currentGridRows; i++) {
        var dcHours = GetElement(gridId + "_ctl" + zeroPad(i, 2) + "_ddlDurationClaimedHours");
        var dcMinutes = GetElement(gridId + "_ctl" + zeroPad(i, 2) + "_ddlDurationClaimedMinutes");
       
        currentDayHours = currentDayHours + parseInt(dcHours.value);
        currentDayMinutes = currentDayMinutes + parseInt(dcMinutes.value);
        currentDayvisits++;
        
    }
    currentDayMinutes = currentDayMinutes + (currentDayHours * 60);
    
    
    // get count for each day
    var hoursMonday = GetElement(currentCpPrefix + "_txtCpNumberOfHours_Monday").value;
    var hoursTuesday = GetElement(currentCpPrefix + "_txtCpNumberOfHours_Tuesday").value;
    var hoursWednesday = GetElement(currentCpPrefix + "_txtCpNumberOfHours_Wednesday").value;
    var hoursThursday = GetElement(currentCpPrefix + "_txtCpNumberOfHours_Thursday").value;
    var hoursFriday = GetElement(currentCpPrefix + "_txtCpNumberOfHours_Friday").value;
    var hoursSaturday = GetElement(currentCpPrefix + "_txtCpNumberOfHours_Saturday").value;
    var hoursSunday = GetElement(currentCpPrefix + "_txtCpNumberOfHours_Sunday").value;

   
    if (currentDayName == "Monday") {
        hoursMonday = currentDayMinutes
        GetElement(currentCpPrefix + "_txtCpNumberOfHours_Monday").value = GetHoursAndMinutesToDisplay(hoursMonday);
    }
    else {
        hoursMonday = parseInt(hoursMonday.split(":")[0]) * 60 + parseInt(hoursMonday.split(":")[1]);
    }

    if (currentDayName == "Tuesday") {
        hoursTuesday = currentDayMinutes
        GetElement(currentCpPrefix + "_txtCpNumberOfHours_Tuesday").value = GetHoursAndMinutesToDisplay(hoursTuesday);
    }
    else {
        hoursTuesday = parseInt(hoursTuesday.split(":")[0]) * 60 + parseInt(hoursTuesday.split(":")[1]);
    }

    if (currentDayName == "Wednesday") {
        hoursWednesday = currentDayMinutes
        GetElement(currentCpPrefix + "_txtCpNumberOfHours_Wednesday").value = GetHoursAndMinutesToDisplay(hoursWednesday);
    }
    else {
        hoursWednesday = parseInt(hoursWednesday.split(":")[0]) * 60 + parseInt(hoursWednesday.split(":")[1]);
    }

    if (currentDayName == "Thursday") {
        hoursThursday = currentDayMinutes
        GetElement(currentCpPrefix + "_txtCpNumberOfHours_Thursday").value = GetHoursAndMinutesToDisplay(hoursThursday);
    }
    else {
        hoursThursday = parseInt(hoursThursday.split(":")[0]) * 60 + parseInt(hoursThursday.split(":")[1]);
    }

    if (currentDayName == "Friday") {
        hoursFriday = currentDayMinutes
        GetElement(currentCpPrefix + "_txtCpNumberOfHours_Friday").value = GetHoursAndMinutesToDisplay(hoursFriday);
    }
    else {
        hoursFriday = parseInt(hoursFriday.split(":")[0]) * 60 + parseInt(hoursFriday.split(":")[1]);
    }

    if (currentDayName == "Saturday") {
        hoursSaturday = currentDayMinutes
        GetElement(currentCpPrefix + "_txtCpNumberOfHours_Saturday").value = GetHoursAndMinutesToDisplay(hoursSaturday);
    }
    else {
        hoursSaturday = parseInt(hoursSaturday.split(":")[0]) * 60 + parseInt(hoursSaturday.split(":")[1]);
    }

    if (currentDayName == "Sunday") {
        hoursSunday = currentDayMinutes
        GetElement(currentCpPrefix + "_txtCpNumberOfHours_Sunday").value = GetHoursAndMinutesToDisplay(hoursSunday);
    }
    else {
        hoursSunday = parseInt(hoursSunday.split(":")[0]) * 60 + parseInt(hoursSunday.split(":")[1]);
    }

    

    var x = hoursMonday + hoursTuesday + hoursWednesday + hoursThursday + hoursFriday + hoursSaturday + hoursSunday;
    var totalHoursId = currentCpPrefix + "_txtCpNumberOfHours";
    var totalHours = GetElement(totalHoursId);
    
    totalHours.value = GetHoursAndMinutesToDisplay(x); 

    UpdateCollapsiblePanel(ddlHoursDC_ClientID, currentDayMinutes, currentDayvisits, currentDayName);
}

function UpdateCollapsiblePanel(ddlHoursDC_ClientID, currentDayMinutes, currentDayvisits, currentDayName) {

    var currentDayHoursAndMonutesDisplay = GetHoursAndMinutesToDisplay(currentDayMinutes);
    
    var cPanelId = ddlHoursDC_ClientID.split("_")[0] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[1] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[2] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[3] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[4] +
                            "_" +
                            ddlHoursDC_ClientID.split("_")[5];

    var cpnl = GetElement(cPanelId);
    cpnl.childNodes[0].lastChild.nodeValue = currentDayName +
                                             " - " +
                                             currentDayvisits +
                                             " visits totalling " +
                                             currentDayHoursAndMonutesDisplay + 
                                             " hours/mins";
}

function GetHoursAndMinutesToDisplay(minutes) {
    var x = minutes;
    var hours = Math.floor(x / 60);
    var minutes = x % 60;

    return zeroPad(hours, 2) + ":" + zeroPad(minutes, 2);
}

function zeroPad(num, count) {
    var numZeropad = num + '';
    while (numZeropad.length < count) {

        numZeropad = "0" + numZeropad;
    }
    return numZeropad;
}

function ReCalculateHoursForAll() {
    var ddlDCHourslist = $("[id$='ddlDurationClaimedHours']");
    var ddlDCMinuteslist = $("[id$='ddlDurationClaimedMinutes']");
    
    var i = 0;
    var totalHours = 0;
    var totalMinutes = 0;
    for (i = 0; i <= ddlDCHourslist.length - 1; i++) {
        totalHours = totalHours + parseInt(ddlDCHourslist[i].value);
        totalMinutes = totalMinutes + parseInt(ddlDCMinuteslist[i].value);       
    }
    totalMinutes = totalMinutes + (totalHours * 60);
    var hours = Math.floor(totalMinutes / 60);
    var minutes = totalMinutes % 60;
    txtNoOfHours.value = zeroPad(hours, 2) + ":" + zeroPad(minutes, 2);
}


function UpdateNoOfHours(dch, dcm, txtHours) {
// discuss with angus
//    alert(dch + " " + dcm + " "  + txtHours);
//    var ddldch = GetElement(dch)
//    var ddldcm = GetElement(dcm)
//    var hours = GetElement(txtHours);
//    alert(ddldch.value);
//    alert(ddldcm.value);
//    alert(hours.value);
//    var hoursplit = hours.value.split(":");
//    var TotalMinutes = (hoursplit[0] * 60) + (hoursplit[1]*1);
//    var CurrentVisitMinutes = (ddldch.value * 60) + (ddldcm.value * 1);
//    alert(CurrentVisitMinutes);
}

function checkNum(x) {

    var s_len = x.value.length;
    var s_charcode = 0;
    for (var s_i = 0; s_i < s_len; s_i++) {
        s_charcode = x.value.charCodeAt(s_i);
        if (!((s_charcode >= 48 && s_charcode <= 57))) {
            alert("Only Numeric Values Allowed");
            x.value = '1';
            x.focus();
            return false;
        }
    }
    return true;
}

// do not delete this function
function Collapse(id, isExpanded, panelid, addBtnId) {
    var cpId = GetElement('CollapsablePanel')
    var AddNewRow = GetElement('AddNewRow')
    var Expanded = isExpanded;
    if (isExpanded == "False") {
        cpId.value = panelid;
        AddNewRow.value = "False";
        var btn = GetElement(addBtnId);
        btn.click();
    }
    else {
        cpId.value = "0";
    }
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function btnInvoiceLines_click() {
    var url = SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/Dom/ProformaInvoice/ViewInvoiceLines.aspx?" +
    "&id=" + GetQSParam(document.location.href, "invoiceid") +
    "&pScheduleId=" + GetQSParam(document.location.href, "pscheduleid") +
    "&await=" + GetQSParam(document.location.href, "await") +
    "&ver=" + GetQSParam(document.location.href, "ver") +
    "&backUrl=" + GetBackUrl();

    document.location.href = url;
}

function EditCareProvider(existingIds, tabIndex, cpId, cpName, cpRef) {
    txtEditCareProviderId.value = cpId;
    txtEditCareProviderName.value = cpName;
    txtEditCareProviderRef.value = cpRef;
    
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT +
                "AbacusExtranet/Apps/InPlaceSelectors/CareWorkerSelectorManualInvoice.aspx?estabid=" + providerID +
                "&tabindex=" + tabIndex +
                "&existingIds=" + existingIds +
                "&editMode=" + true;
        var dialog = OpenDialog(url, 45, 32, window);
}



addEvent(window, "load", Init);
//addEvent(window, "unload", DialogUnload);

