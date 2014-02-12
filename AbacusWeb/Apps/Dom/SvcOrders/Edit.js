var Edit_domContractID, Edit_endReasonID, Edit_dateToID, Edit_fc2ID, DSO_ID, Edit_frmAttenanceID, dteDatesEffectiveDateID;
var Edit_rateCategories, Edit_uoms, Edit_endReasons, Edit_AttendanceEffectiveDates;
var Edit_showDayOfWeekColumn, Edit_showSvcUserMinutesColumn, Edit_showVisitColumn;
var Edit_attPlanID;
var attendanceFrame;
var plannedAttendance, dsoID, edit_EffectiveDateID;
var contractSvc;
var documentsTabLoaded, paymentToleranceTabLoaded, Edit_clientID, contractID, Edit_dateFromID;
var dateFrom, dateTo;
var Edit_InEditMode;


var dwtResultsPanel, dwtResultSettings, ibResultsPanel, overrideUnitCostDialog;
var overrideUnitCostResultsControl;

$(document).ready(function () {
    //initialiseControl();
    //The following function is located in \TargetWeb\Library\UserControls\DSOBasicDetails.JS
    dsoDetail_PopulateControl(DSO_ID);
    $(".trig").click(function () {
        $(".fundingpanel").toggle("fast");
        $(".panel").toggle("fast");
        $(this).toggleClass("active");
        return false;
    });

    $("[id*='sumLineDateFrom']").change(function () {
        var $dateFromCtrl = $('#' + this.id);
        var $dateToCtrl = $('#' + this.id.replace("DateFrom", "DateTo"));
        $dateToCtrl.datepicker('option', 'minDate', $dateFromCtrl.val().toDate());
    });

    $("[id*='sumLineDateTo']").change(function () {
        var $dateToCtrl = $('#' + this.id);
        var $dateFromCtrl = $('#' + this.id.replace("DateTo", "DateFrom"));
        $dateFromCtrl.datepicker('option', 'maxDate', $dateToCtrl.val().toDate());
    });

    $("[id*='visitLineDateFrom']").change(function () {
        var $dateFromCtrl = $('#' + this.id);
        var $dateToCtrl = $('#' + this.id.replace("DateFrom", "DateTo"));
        $dateToCtrl.datepicker('option', 'minDate', $dateFromCtrl.val().toDate());
    });

    $("[id*='visitLineDateTo']").change(function () {
        var $dateToCtrl = $('#' + this.id);
        var $dateFromCtrl = $('#' + this.id.replace("DateTo", "DateFrom"));
        $dateFromCtrl.datepicker('option', 'maxDate', $dateToCtrl.val().toDate());
    });

    $("[id*='chkDontFilterCommitmentSummary']").click(function () {
        $("[id*='txtDummyFilterDate']").val(Date()).change();
    });

    $("[id*='chkDontFilterCommitmentVisit']").click(function () {
        $("[id*='txtDummyFilterDate']").val(Date()).change();
    });

    Ext.onReady(function () {
        Ext.require('Ext.layout.container.Border')
        Ext.require('TargetExt.DSOProviderUnitCostOverride.Lister');
    });

});


function showSvcOrderFundingPanel() {
    $(".panel").toggle("fast");
}


function Init() {
    contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();

    //get the date from and date to of the service order
    dateFrom = GetElement(Edit_dateFromID + "_txtTextBox");
    dateTo = GetElement(Edit_dateToID + "_txtTextBox");

}

function txtUnits_Change(id, cboRateCategoryID, txtSuMinutesID) {

    var txtUnits = GetElement(id);
    var serviceUserMinutesControl = GetElement(txtSuMinutesID, true);
    var rateCategoryCbo = GetElement(cboRateCategoryID + "_cboDropDownList");
    var rateCategory = Edit_rateCategories.item(rateCategoryCbo.value);
    var minutesPerUnit = 0;
    var units = txtUnits.value;

    if (rateCategory) {

        minutesPerUnit = Number(Edit_uoms.item(rateCategory[1])[3]);
        serviceUserMinutes = Math.round(units * minutesPerUnit);

        if (serviceUserMinutesControl) {
            serviceUserMinutesControl.value = serviceUserMinutes;
        }
//        else {
//            alert('Error : Cannot locate the service user minutes control for the current row.');
//        }
    }
    else {

        alert('Cannot determine the rate category for the current row.');

    }
         
}

function ctlUnitsHoursMins_Change(src, txtSuMinutesID, txtUnitsHoursID, txtUnitsMinsID) {

    var $txtSuMinutes = $('#' + txtSuMinutesID);
    var $txtUnitsHours = $('#' + txtUnitsHoursID);
    var $txtUnitsMins = $('#' + txtUnitsMinsID);
    var totalMinutes = 0;

    if ($txtUnitsHours) {
        var hours = parseInt($txtUnitsHours.val());
        if (isNaN(hours)) {
            hours = 0;
        }
        totalMinutes += (hours * 60);
    }
    else {
        alert('Error : Cannot locate the unit hours control for the current row.');
    }

    if ($txtUnitsMins) {
        var mins = parseInt($txtUnitsMins.val());
        if (isNaN(mins)) {
            mins = 0;
        }
        totalMinutes += mins;
    }
    else {
        alert('Error : Cannot locate the unit minutes control for the current row.');
    }

    if ($txtSuMinutes) {
        $txtSuMinutes.val(totalMinutes.toString());
    }
//    else {
//        alert('Error : Cannot locate the service user minutes control for the current row.');
//    }
 
    FireEvent($txtSuMinutes[0], 'change');
    
}

function InPlaceEstablishment_Changed(newID) {
	var enabled = false;
	if(newID.trim().length > 0) enabled = true;
	InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
	if(enabled) {
		InPlaceDomContractSelector_Enabled(Edit_domContractID, true);
	} else {
		InPlaceDomContractSelector_Enabled(Edit_domContractID, false);
	}
	InPlaceDomContractSelector_providerID = newID;
}

function dteDateFrom_Changed(id) {
    var $dateFromCtrl = $('#' + id + '_txtTextBox');
    var $dateToCtrl = $('#' + Edit_dateToID + '_txtTextBox');
    $dateToCtrl.datepicker('option', 'minDate', $dateFromCtrl.val().toDate());    
}

function dteDateTo_Changed(id) {
	var cboEndReason, valEndReason, dteDateTo, valDateTo;
	
	cboEndReason = GetElement(Edit_endReasonID + "_cboDropDownList");
	valEndReason = GetElement(Edit_endReasonID + "_valRequired");
	dteDateTo = GetElement(id + "_txtTextBox");
	valDateTo = GetElement(id + "_valRequired", true);
	
	if(dteDateTo.value.length == 0) {
	    cboEndReason.value = "";
	    cboEndReason.disabled = true;
		ValidatorEnable(valEndReason, false);
		if (valDateTo)
		    ValidatorEnable(valDateTo, false);
	} else {
	    ValidatorEnable(valEndReason, true);
	    cboEndReason.disabled = false;
	}
}

function cboEndReason_Changed() {
	var cboEndReason, txtDateTo, valDateTo;
	var endReason, endReasonType, enableDateTo;
	
	cboEndReason = GetElement(Edit_endReasonID + "_cboDropDownList");
	
	if(!cboEndReason.disabled) {
	
	    txtDateTo = GetElement(Edit_dateToID + "_txtTextBox");
	    valDateTo = GetElement(Edit_dateToID + "_valRequired", true);
    	
	    endReason = Edit_endReasons.item(cboEndReason.value);
	    enableDateTo = true;
    	
	    if(endReason != null) {
            endReasonType = endReason;
            if(endReasonType == 1) {
                // a "system" end reason
                enableDateTo = false;
            }
	    }

	    if (enableDateTo) {
            $(txtDateTo).datepicker('enable');
	    } else {
	        $(txtDateTo).datepicker('disable');
	    }

	    if (valDateTo) {
	        ValidatorEnable(valDateTo, enableDateTo);
	        if (cboEndReason.value.length == 0)
	            ValidatorEnable(valDateTo, false);
	    }
    }
}
function txtFinanceCode2_Changed(id) {
	var txtFinanceCode2, valFinanceCode2, dteFinanceCode2StartDate, valFinanceCode2StartDate;

	txtFinanceCode2 = GetElement(id + "_txtTextBox");
	valFinanceCode2 = GetElement(id + "_valRequired");
		
	if(txtFinanceCode2.value.length == 0) {

		ValidatorEnable(valFinanceCode2, false);
	} else {

	}
}
function dteFinanceCode2StartDate_Changed(id) {
	var txtFinanceCode2, valFinanceCode2, dteFinanceCode2StartDate, valFinanceCode2StartDate;

	valFinanceCode2 = GetElement(Edit_fc2ID + "_valRequired");

}

function tabStrip_ActiveTabChanged(sender, args) {

    var hidSelectedTab = GetElement("hidSelectedTab");
    hidSelectedTab.value = sender.get_activeTab().get_headerText();

    switch (hidSelectedTab.value) {
        case "Documents":
            if (!documentsTabLoaded) {
                if (document.getElementById('ifrDocuments')) {
                    document.getElementById('ifrDocuments').src = "DocumentsTab.aspx?clientid=" + Edit_clientID + "&iframeid=ifrDocuments";
                    documentsTabLoaded = true;
                }
            }
            break;
        case "Payment Tolerances":
            if (!paymentToleranceTabLoaded) {
                if (document.getElementById('ifrPaymentTolerances')) {
                    document.getElementById('ifrPaymentTolerances').src = SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/Contracts/PaymentTolerances.aspx?contractID=" + contractID + "&serviceOrderID=" + dsoID + "&dsoDateFrom=" + dateFrom.value + "&dsoDateTo=" + dateTo.value + "&clientID=" + Edit_clientID + "&mode=1";
                    paymentToleranceTabLoaded = true;
                }
            }
            break;
    }  
}

function cboRateCatAtt_Change(cboRateCategoryID) {

    PrimeAttMeasuredIn(cboRateCategoryID);
    set_PlannedCounts()
}

function PrimeAttMeasuredIn(cboRateCategoryID) {

    var tdMeasuredInIndex = 3;
    //if(!Edit_showDayOfWeekColumn) tdMeasuredInIndex--;
    
    var cbo = GetElement(cboRateCategoryID + "_cboDropDownList");
    var row = cbo.parentNode.parentNode;
    var tdMeasuredIn = row.cells[tdMeasuredInIndex];
    var spanMeasuredIn = tdMeasuredIn.getElementsByTagName("SPAN")[0];
    
    var rateCategory = Edit_rateCategories.item(cbo.value);
    var uomID = 0;
    var uomDesc = " ";
    var uomComment = "";

    if(rateCategory) {
        uomID = rateCategory[1];
        uomDesc = Edit_uoms.item(uomID)[0];
        uomComment = Edit_uoms.item(uomID)[1];
    }
    SetInnerText(spanMeasuredIn, uomDesc);
    spanMeasuredIn.setAttribute("title", uomComment);
}



function cboRateCategory_Change(cboRateCategoryID) {
    
    PrimeMeasuredIn(cboRateCategoryID);
    PrimeUnits(cboRateCategoryID);
    if (Edit_showSvcUserMinutesColumn) {
        ServiceUserMinutesEnable(cboRateCategoryID);
    }
        
    if(Edit_showVisitColumn) {
        var tdVisitsIndex = 7;
        if(!Edit_showDayOfWeekColumn) tdVisitsIndex--;
        if(!Edit_showSvcUserMinutesColumn) tdVisitsIndex--;        
        
        var cbo = GetElement(cboRateCategoryID + "_cboDropDownList");
        var row = cbo.parentNode.parentNode;
        var tdVisits = row.cells[tdVisitsIndex];
        var txtVisits = tdVisits.getElementsByTagName("INPUT")[0];
        var valVisitsID = txtVisits.id.replace("txtTextBox", "valRequired");
        var valVisits = GetElement(valVisitsID, true);
        var rateCategory = Edit_rateCategories.item(cbo.value);

        var timeBased = false;
        var uomID = 0;

        //if(rateCategory) visitBasedReturns = rateCategory[0];
        if (rateCategory) {
            uomID = rateCategory[1];
            timeBased = (Edit_uoms.item(uomID)[3] > 0);
        }
        
        if(timeBased) {
            txtVisits.disabled = !Edit_InEditMode;
            if(valVisits) ValidatorEnable(valVisits, true);
        } else {
            txtVisits.value = "0";
            txtVisits.disabled = true;
            if(valVisits) ValidatorEnable(valVisits, false);
        }
    }
}

function PrimeMeasuredIn(cboRateCategoryID) {

    var tdMeasuredInIndex = 5;
    if(!Edit_showDayOfWeekColumn) tdMeasuredInIndex--;
    
    var cbo = GetElement(cboRateCategoryID + "_cboDropDownList");
    var row = cbo.parentNode.parentNode;
    var tdMeasuredIn = row.cells[tdMeasuredInIndex];
    var spanMeasuredIn = tdMeasuredIn.getElementsByTagName("SPAN")[0];
    
    var rateCategory = Edit_rateCategories.item(cbo.value);
    var uomID = 0;
    var uomDesc = " ";
    var uomComment = "";

    if(rateCategory) {
        uomID = rateCategory[1];
        uomDesc = Edit_uoms.item(uomID)[0];
        uomComment = Edit_uoms.item(uomID)[1];
    }
    SetInnerText(spanMeasuredIn, uomDesc);
    spanMeasuredIn.setAttribute("title", uomComment);
}

function PrimeUnits(cboRateCategoryID) {

    var tdUnitsIndex = 4;
    if(!Edit_showDayOfWeekColumn) tdUnitsIndex--;

    var cbo = GetElement(cboRateCategoryID + "_cboDropDownList");
    var row = cbo.parentNode.parentNode;
    var tdUnits = row.cells[tdUnitsIndex];
    var spanUnits = tdUnits.childNodes[0];
    var spanUnitsHoursMins = tdUnits.childNodes[1];
    var txtUnits = spanUnits.getElementsByTagName("INPUT")[0];
    var valUnits = spanUnits.getElementsByTagName("SPAN")[0];

    var txtUnitsHours = spanUnitsHoursMins.getElementsByTagName("INPUT")[0];  
    var txtUnitsMins = spanUnitsHoursMins.getElementsByTagName("INPUT")[1];
    var valUnitsHours = spanUnitsHoursMins.getElementsByTagName("SPAN")[0]; 
    var valUnitsMins = spanUnitsHoursMins.getElementsByTagName("SPAN")[4];
    
    var rateCategory = Edit_rateCategories.item(cbo.value);
//    var visitBased = false;
    var displayHoursMins = false;
    var uomID = 0;
    var serviceTypeID = 0;
    var oneUnitPerOrder = false;
    
    if(rateCategory) {
//        visitBased = rateCategory[0];
        uomID = rateCategory[1];
        serviceTypeID = rateCategory[2];
        displayHoursMins = Edit_uoms.item(uomID)[2];
        oneUnitPerOrder = rateCategory[3];
    }
    
    if(displayHoursMins) {
        spanUnits.style.display = "none";
        txtUnits.disabled = true;
        ValidatorEnable(valUnits, false);
        spanUnitsHoursMins.style.display = "block";

        txtUnitsHours.disabled = !Edit_InEditMode;
        //ValidatorEnable(valUnitsHours, true);
        txtUnitsMins.disabled = !Edit_InEditMode;
        //ValidatorEnable(valUnitsMins, true);
        
    } else {
        spanUnits.style.display = "block";
        txtUnits.disabled = !Edit_InEditMode;
        ValidatorEnable(valUnits, true);
        spanUnitsHoursMins.style.display = "none";
        if(oneUnitPerOrder) {
            txtUnits.value = "1.00";
            txtUnits.disabled = true;
            ValidatorEnable(valUnits, false);
        } else {
            txtUnits.disabled = !Edit_InEditMode;
            ValidatorEnable(valUnits, true);
        }
        txtUnitsHours.disabled = true;
        if (valUnitsHours) ValidatorEnable(valUnitsHours, false);
        txtUnitsMins.disabled = true;
        if (valUnitsMins) ValidatorEnable(valUnitsMins, false);
    }        
}

function ServiceUserMinutesEnable(cboRateCategoryID) {
    var tdSUMinutesIndex = 4;
    if (!Edit_showDayOfWeekColumn) tdSUMinutesIndex--;

    var cbo = GetElement(cboRateCategoryID + "_cboDropDownList");
    var row = cbo.parentNode.parentNode;
    var rateCategory = Edit_rateCategories.item(cbo.value);
    var visitBased = false;

    if (rateCategory) {
        visitBased = rateCategory[0];
    }

    var tdSUMinutes = row.cells[tdSUMinutesIndex];

    var txtSUMins;
    if (visitBased) {
        var txtSUMins = tdSUMinutes.getElementsByTagName("INPUT")[2];
    }
    else {
        var txtSUMins = tdSUMinutes.getElementsByTagName("INPUT")[0];
    }

    var valSUMinsID = txtSUMins.id.replace("txtTextBox", "valRequired");
    var valSUMins = GetElement(valSUMinsID, true);
    
    
    if (txtSUMins != undefined) {
        txtSUMins.disabled = !visitBased
        if (valSUMins != null){
            ValidatorEnable(valSUMins, visitBased);
        }
    }
}

//function btnOverride_Click(btnID) {

//    var btn, dialog, cell, row, hidFields, hidUnitCost, hidOverridden, hidOverriddenUnitCost;
//    var rowIndex, rateCategory, stdUnitCost, overriddenUnitCost, overridden;
//    
//    btn = GetElement(btnID);
//    cell = btn.parentNode;
//    hidFields = cell.getElementsByTagName("input");
//    hidUnitCost = hidFields[0];
//    hidOverridden = hidFields[1];
//    hidOverriddenUnitCost = hidFields[2];
//    row = cell.parentNode;    
//    
//    rowIndex = row.rowIndex;
//    rateCategory = GetInnerText(row.cells[0]);
//    stdUnitCost = hidUnitCost.value;
//    overriddenUnitCost = hidOverriddenUnitCost.value;
//    if (hidOverridden.value == 'True') {
//        overridden = true;
//    } else {
//        overridden = false;
//    }
//    //overridden = new Boolean(parseInt(hidOverridden.value, 10)).valueOf();
//    
//    dialog = new Target.Abacus.Web.DomSvcOrder.OverrideUnitCostDialog(rowIndex, rateCategory, stdUnitCost, overriddenUnitCost, overridden, btnOverride_Callback);
//    dialog.Show();
//    
//    return false;
//}

//function btnOverride_Callback(evt, args) {

//    var dialog = args[0];
//    var rowIndex = args[1];
//    var result = args[2];
//    var tbl, row, btnCell, hidFields, hidUnitCost, hidOverridden, hidOverriddenUnitCost;
//    var units, tdUnitCost, tdOverridden, tdTotalCost, tdOverallCost, oldTotalCost, overallCost;
//    
//    if (result == 1) {
//        // OK was clicked
//        
//        // get the bits of the grid we wish to update
//        tbl = GetElement("tblExpenditure");
//        row = tbl.rows[rowIndex];
//        tdUnitCost = row.cells[3];
//        tdOverridden = row.cells[4];
//        tdTotalCost =  row.cells[5];
//        oldTotalCost = parseFloat(GetInnerText(tdTotalCost).replace("£", "").replace(",", ""));
//        btnCell = row.cells[7];
//        hidFields = btnCell.getElementsByTagName("input");
//        hidUnitCost = hidFields[0];
//        hidOverridden = hidFields[1];
//        hidOverriddenUnitCost = hidFields[2];
//        units = parseFloat(hidFields[3].value);
//        tdOverallCost = tbl.rows[tbl.rows.length - 1].cells[1];
//        overallCost = parseFloat(GetInnerText(tdOverallCost).replace("£", "").replace(",", ""));
//        
//        if(dialog.UseStandard.checked) {
//            tdUnitCost.innerHTML = hidUnitCost.value.formatCurrency();
//            SetInnerText(tdOverridden, "No");
//            tdTotalCost.innerHTML = (hidUnitCost.value * units).toString().formatCurrency();
//            hidOverridden.value = "False";
//        } else {
//            tdUnitCost.innerHTML = dialog.OverriddenUnitCost.value.formatCurrency();
//            SetInnerText(tdOverridden, "Yes");
//            tdTotalCost.innerHTML = (dialog.OverriddenUnitCost.value.replace(",", "") * units).toString().formatCurrency();
//            hidOverridden.value = "True";
//            hidOverriddenUnitCost.value = dialog.OverriddenUnitCost.value;
//        }
//        // new overall cost
//        overallCost = overallCost - oldTotalCost + parseFloat(GetInnerText(tdTotalCost).replace("£", "").replace(",", ""));
//        tdOverallCost.innerHTML = overallCost.toString().formatCurrency();
//        
//        
//    }
//    
//    dialog.Hide();
//}

///************************************/
///* OVERRIDE DIALOG                  */
///************************************/

//addNamespace("Target.Abacus.Web.DomSvcOrder.OverrideUnitCostDialog");

//Target.Abacus.Web.DomSvcOrder.OverrideUnitCostDialog = function(rowIndex, rateCategory, stdUnitCost, overriddenUnitCost, overridden, callback) {
// 
//	this.SetTitle("Override Provider Unit Cost");
//	this.SetWidth("39");
//	
//	this.ClearContent();
//    this.AddContent(document.createTextNode("Rate Category: " + rateCategory));
//    this.AddContent(document.createElement("br"));
//    this.AddContent(document.createElement("br"));
//    
//    this.UseStandard = AddRadio(this._content, 
//                            "", 
//                            "rdoUnitCost", 
//                            1, 
//                            Target.Abacus.Web.DomSvcOrder.OverrideUnitCostDialog.ChangeUnitCost, 
//                            new Array(this, 1));
//    this.AddContent(document.createTextNode("Use standard unit cost: £" + stdUnitCost.toString().formatCurrency(true)));
//    
//    this.AddContent(document.createElement("br"));
//    this.AddContent(document.createElement("br"));
//    
//    args = new Array(1);
//    args[0] = 2;
//    this.UseOverridden = AddRadio(this._content, 
//                            "", 
//                            "rdoUnitCost", 
//                            2, 
//                            Target.Abacus.Web.DomSvcOrder.OverrideUnitCostDialog.ChangeUnitCost, 
//                            new Array(this, 2));
//    this.AddContent(document.createTextNode("Use overridden unit cost: "));    
//    this.OverriddenUnitCost = AddInput(this._content, "txtOverriddenUnitCost", "text", "", "", overriddenUnitCost.toString().formatCurrency(true));
//    // filter key presses to only allow "0123456789.-" keys from normal keyboard, the same from numeric key pad, delete or backspace keys
//    addEvent(this.OverriddenUnitCost, 
//            "keydown", 
//            function go(evt) { FilterKeyPress(evt, new Array(48,49,50,51,52,53,54,55,56,57,189,190,96,97,98,99,100,101,102,103,104,105,109,110,8,46)); }
//    );
//    // format result
//    addEvent(this.OverriddenUnitCost,
//            "change",
//            function go(evt) { var target=GetSrcElementFromEvent(evt);if(target.value.trim().length>0) target.value=target.value.formatCurrency(true, true); }
//    );
//    
//    if(overridden) {
//        this.UseOverridden.checked = true;
//        // defaultChecked is IE workaround
//        this.UseOverridden.setAttribute("defaultChecked", true, 0);
//        fireEvent(this.UseOverridden, "click");
//    } else {
//        this.UseStandard.checked = true;
//        // defaultChecked is IE workaround
//        this.UseStandard.setAttribute("defaultChecked", true, 0);
//        fireEvent(this.UseStandard, "click");
//    }
//        
//    this.AddContent(document.createElement("br"));
//    this.AddContent(document.createElement("br"));
//    
//    var elem = document.createElement("em");
//    elem.appendChild(document.createTextNode("Standard rate shown is that in effect on " + new Date().strftime("%d/%m/%Y")));
//    this.AddContent(elem);
//	
//	// buttons
//	this.ClearButtons();
//	this.AddButton("OK", "", callback, new Array(this, rowIndex, 1));     // OK = 1
//	this.AddButton("Cancel", "", callback, new Array(this, rowIndex, 2)); // Cancel = 2
//}

//Target.Abacus.Web.DomSvcOrder.OverrideUnitCostDialog.ChangeUnitCost = function (evt, args) {
//    var dialog = args[0];
//    var option = args[1];
//    if(option == 1) {
//        // use std
//        dialog.OverriddenUnitCost.disabled = true;
//    } else {
//        // use overridden
//        dialog.OverriddenUnitCost.disabled = false;
//    }
//}

function btnSvcOrderFunding_Click() {

    var autoPopup = GetQSParam(document.location.search, "autopopup");
    
    if (!autoPopup)
        autoPopup = 0;
        
    document.location.href = "Funding.aspx?id=" + DSO_ID + "&autopopup=" + autoPopup + "&backURL=" + GetBackUrl();
}

function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

function cboFrequency_Change(cboFrequencyID) {
    var cboFreq = GetElement(cboFrequencyID + "_cboDropDownList");
    var tdFreq = cboFreq.parentNode;
    cleanWhitespace(tdFreq.parentNode);
}

function days_ChangeSummary(txtUnitsID) {
    var txtUnits = GetElement(txtUnitsID, true);
    var unitCount = 0;
    if (txtUnits) {
        var tdUnits = txtUnits.parentNode;
        cleanWhitespace(tdUnits.parentNode);
        var tdDays = tdUnits.previousSibling;
        var chkMon = tdDays.getElementsByTagName("span")[0].lastChild;
        var chkTue = tdDays.getElementsByTagName("span")[1].lastChild;
        var chkWed = tdDays.getElementsByTagName("span")[2].lastChild;
        var chkThu = tdDays.getElementsByTagName("span")[3].lastChild;
        var chkFri = tdDays.getElementsByTagName("span")[4].lastChild;
        var chkSat = tdDays.getElementsByTagName("span")[5].lastChild;
        var chkSun = tdDays.getElementsByTagName("span")[6].lastChild;

        if (chkMon.checked == true) unitCount++;
        if (chkTue.checked == true) unitCount++;
        if (chkWed.checked == true) unitCount++;
        if (chkThu.checked == true) unitCount++;
        if (chkFri.checked == true) unitCount++;
        if (chkSat.checked == true) unitCount++;
        if (chkSun.checked == true) unitCount++;

        SetInnerText(txtUnits, unitCount);
    }
}

function days_Change(txtUnitsID) {
    var txtUnits = GetElement(txtUnitsID, true);
    var unitCount = 0;
    if (txtUnits) {
        var tdUnits = txtUnits.parentNode;
        cleanWhitespace(tdUnits.parentNode);
        var tdDays = tdUnits.previousSibling;
        var chkMon = tdDays.getElementsByTagName("span")[0].lastChild;
        var chkTue = tdDays.getElementsByTagName("span")[1].lastChild;
        var chkWed = tdDays.getElementsByTagName("span")[2].lastChild;
        var chkThu = tdDays.getElementsByTagName("span")[3].lastChild;
        var chkFri = tdDays.getElementsByTagName("span")[4].lastChild;
        var chkSat = tdDays.getElementsByTagName("span")[5].lastChild;
        var chkSun = tdDays.getElementsByTagName("span")[6].lastChild;

        if (chkMon.checked==true) unitCount++;
        if (chkTue.checked==true) unitCount++;
        if (chkWed.checked==true) unitCount++;
        if (chkThu.checked==true) unitCount++;
        if (chkFri.checked==true) unitCount++;
        if (chkSat.checked==true) unitCount++;
        if (chkSun.checked==true) unitCount++;
        
        SetInnerText(txtUnits, unitCount);
        set_PlannedCounts();
    }
}

function set_PlannedCounts() {
    var tbl, tBody, row, cell;
    var value, key;
    var plan = GetElement("divRevisedPlan", true);
    var data = new Collection();
    var unitIndex, uomIndex;
    
    if (!plan) {
        plan = GetElement(Edit_attPlanID, true);
    }
    tbl = GetElement("tblAttendance", true);
    if (plan && tbl) {
        tBody = tbl.tBodies[0];

        plan.innerHTML = "";

        unitIndex = 2;
        uomIndex = 3;

        linesTotal = 0;
        for (index = 0; index < tBody.rows.length; index++) {
            row = tBody.rows[index];
            //Get the number of Units
            cell = row.cells[unitIndex];
//            if (Edit_showDayOfWeekColumn) {
                value = GetInnerText(cell.firstChild);
//            } else {
//                value = cell.getElementsByTagName('input')[0].value;
//                if (value == '') value = 0;
//            }
            //Get the Unit Of measure
            cell = row.cells[uomIndex];
            key = GetInnerText(cell.firstChild);

            //Update the collection or add to it.
            if (data.exists(key)) {
                data.update(key, parseFloat(data.item(key)) + parseFloat(value));
            } else {
                if (key != " ") data.add(key, value);
            }

        }

        var keys = data.getKeys();
        keys.sort();
        for (i = 0; i < keys.length; i++) {
            var invalidatesPlan;
            var newLabel = document.createElement('label');

            if (plannedAttendance != null) {
                invalidatesPlan = (data.item(keys[i]) != plannedAttendance.item(keys[i]));
            }

            newLabel.style.paddingRight = "0.5em";
            if (invalidatesPlan == true) {
                newLabel.style.color = "Red"
            }
            SetInnerText(newLabel, keys[i]);
            plan.appendChild(newLabel);

            var newLabel = document.createElement('label');
            newLabel.setAttribute("className", "warningText");
            newLabel.style.paddingRight = "2em";
            if (invalidatesPlan == true) {
                newLabel.style.color = "Red"
            }
            SetInnerText(newLabel, data.item(keys[i]));
            plan.appendChild(newLabel);

        }
    }
}

function btnAttendanceViewPlan_click() {
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/Dom/SvcOrders/PlannedAttendance.aspx?id=" + DSO_ID;
    var dialog = OpenDialog(url, 60, 32, window);
}

function btnDates_Click() {
    var d = new Target.Web.Dialog.Msg();
    var emptyDialogContent, datesDialogContentContainer, datesDialogContent, effectiveDate, newEffectiveDate;
    
    effectiveDate = GetElement(edit_EffectiveDateID + "_txtTextBox");
    newEffectiveDate = GetElement(dteDatesEffectiveDateID + "_txtTextBox");
    
    d.SetType(4);   // OK/Cancel
    d.SetCallback(DatesDialog_Callback);
    d.SetTitle("Change effective date");

    d.ClearContent();    
    emptyDialogContent = document.createElement("DIV");
    d.AddContent(emptyDialogContent);
    
    datesDialogContentContainer = GetElement("divDatesDialogContentContainer");
    datesDialogContent = datesDialogContentContainer.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = d._content.removeChild(emptyDialogContent);
    datesDialogContent = datesDialogContentContainer.removeChild(datesDialogContent);
    datesDialogContentContainer.appendChild(emptyDialogContent);
    d.AddContent(datesDialogContent);
    
    newEffectiveDate.value = effectiveDate.value;
    
    
    d.ShowCloseLink(false);
    d.Show();
}


function DatesDialog_Callback(evt, args) {
    var d = args[0];
    var answer = args[1];
    var emptyDialogContent, datesDialogContentContainer, datesDialogContent;
    var valRequired, effectiveDate, newEffectiveDate, response, hidCurrentEffectiveDate;
    
    // answer == 1 means OK
    if(answer == 1) {
        if(Page_ClientValidate("Copy")) {
            effectiveDate = GetElement(edit_EffectiveDateID + "_txtTextBox");
            newEffectiveDate = GetElement(dteDatesEffectiveDateID + "_txtTextBox").value;
            valRequired = GetElement(dteDatesEffectiveDateID + "_valRequired");
            hidCurrentEffectiveDate = GetElement('ctl00_MPContent_tabStrip_tabAttendance_hidCurrentEffectiveDate');
            
            response = contractSvc.UpdateAttendanceEffectiveDate(dsoID, effectiveDate.value.toDate(), newEffectiveDate.toDate()).value;
            if(response.Success == false) {
                alert(response.Message);
                return;
            } else {
                effectiveDate.value = newEffectiveDate;   
                hidCurrentEffectiveDate.value = newEffectiveDate;
            }
        } else {
            return;
        }
    }
    datesDialogContentContainer = GetElement("divDatesDialogContentContainer");
    emptyDialogContent = datesDialogContentContainer.getElementsByTagName("DIV")[0];
    datesDialogContent = d._content.getElementsByTagName("DIV")[0];
    
    // swap nodes
    emptyDialogContent = datesDialogContentContainer.removeChild(emptyDialogContent);
    datesDialogContent = d._content.removeChild(datesDialogContent);
    datesDialogContentContainer.appendChild(datesDialogContent);
    d.AddContent(emptyDialogContent);
    
    d.Hide();
}

function btnDeleteAttendance_Click() {
    if (Edit_AttendanceEffectiveDates.item(2) == null) {
        return window.confirm('This is the last remaining attendance pattern for this order' +
                                '\rA new pattern will be created from the existing plan' +
                                '\rAre you sure you wish to delete this revision?')
    } else {
       return window.confirm('Are you sure you wish to delete this revision?')
        
    }

}

function cboRateCatAttSummary_Change(cboRateCategoryID) {

    PrimeAttSumMeasuredIn(cboRateCategoryID);
}

function PrimeAttSumMeasuredIn(cboRateCategoryID) {

    var tdMeasuredInIndex = 3;
    if (!Edit_showDayOfWeekColumn) tdMeasuredInIndex--;

    var cbo = GetElement(cboRateCategoryID + "_cboDropDownList");
    var row = cbo.parentNode.parentNode;
    var tdMeasuredIn = row.cells[tdMeasuredInIndex];
    var spanMeasuredIn = tdMeasuredIn.getElementsByTagName("SPAN")[0];

    var rateCategory = Edit_rateCategories.item(cbo.value);
    var uomID = 0;
    var uomDesc = " ";
    var uomComment = "";

    if (rateCategory) {
        uomID = rateCategory[1];
        uomDesc = Edit_uoms.item(uomID)[0];
        uomComment = Edit_uoms.item(uomID)[1];
    }
    SetInnerText(spanMeasuredIn, uomDesc);
    spanMeasuredIn.setAttribute("title", uomComment);
}

function resizeIframe(newHeight, iFrameName) {
    document.getElementById(iFrameName).style.height = parseInt(newHeight) + 15 + 'px';
    currentFrame = iFrameName;
}


function txtVisitsFilterDate_Changed(clientID) {

    $("[id*='txtDummyFilterDate']").val(GetElement(clientID + "_txtTextBox").value).change();
}

function btnOverrideUnitCosts_Click() {

    overrideUnitCostDialog = Ext.create('TargetExt.DSOProviderUnitCostOverride.Lister', {});

    overrideUnitCostDialog.show({ DomServiceOrderID: DSO_ID });
 
}

function days_TickUntickAll(chkAllClientID) {
    //Get the all days check box
    var chkAll = GetElement(chkAllClientID + '_chkCheckbox', true);
    var NewValue = chkAll.checked;

    //Locate the Mon-Sun Check boxes
    var chkMon = GetElement(chkAllClientID.replace('All', 'Mon') + '_chkCheckbox', true);
    var chkTue = GetElement(chkAllClientID.replace('All', 'Tue') + '_chkCheckbox', true);
    var chkWed = GetElement(chkAllClientID.replace('All', 'Wed') + '_chkCheckbox', true);
    var chkThu = GetElement(chkAllClientID.replace('All', 'Thu') + '_chkCheckbox', true);
    var chkFri = GetElement(chkAllClientID.replace('All', 'Fri') + '_chkCheckbox', true);
    var chkSat = GetElement(chkAllClientID.replace('All', 'Sat') + '_chkCheckbox', true);
    var chkSun = GetElement(chkAllClientID.replace('All', 'Sun') + '_chkCheckbox', true);

    //Set the Mon-Sun Check boxes equal to that of the All Days checkbox
    chkMon.checked = NewValue;
    chkTue.checked = NewValue;
    chkWed.checked = NewValue;
    chkThu.checked = NewValue;
    chkFri.checked = NewValue;
    chkSat.checked = NewValue;
    chkSun.checked = NewValue;
}



addEvent(window, "load", Init);

