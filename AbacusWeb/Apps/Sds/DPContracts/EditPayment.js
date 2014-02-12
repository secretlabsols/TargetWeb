var dteDateFromID, cboFreqID, txtNumPaymentsID, chkForceGrossID, btnAddBreakdownID, isBalancingPayment = false, firstWeekEndDate, firstWeekStartDate;
var txtNumPayments, btnAddEnabled;
var spnAmount, spnAmountID, spnDateTo, spnDateToID;
var txtUnits, txtRate, hidCost, spnCost;
var hidNumPaymentsID, hidNumPayments;
var hidAmountID, hidAmount, hidFrequencyID, hidFrequency;
var hidPaidUpToID, hidPaidUpTo;
var hidForceGrossID, hidForceGross;
var hidDateFromID, hidDateFrom;
var hidDateToID, hidDateTo;
var hidEndReasonID, hidEndReason;
var cboEndReason, cboEndReasonID;
var dteDateFrom, cboFreq, chkForceGross, btnAddBreakdown;
var editSuspendForbidden;
var optStatusActiveID, optStatusActive, origStatusActive;
var optStatusProvID, optStatusProv;
var inEditMode;
var freqWEEKLY, freqTWOWEEKLY, freqFOURWEEKLY, freqMONTHLY;
var freqQUARTERLY, freqHALFYEARLY, freqANNUALLY;
var freqQUARTERLY13weeks, freqHALFYEARLY26weeks, freqANNUALLY52weeks;
var freqWEEKLYid, freqTWOWEEKLYid, freqFOURWEEKLYid, freqMONTHLYid;
var freqQUARTERLYid, freqHALFYEARLYid, freqANNUALLYid;
var freqQUARTERLY13weeksid, freqHALFYEARLY26weeksid, freqANNUALLY52weeksid;
var freqFIRSTWEEKid;
var spendPlanID;
var budgetCategoryValidationMethod;
var pnlLegendID, pnlLegendInSpendPlanNotDPID, pnlLegendNotInSpendPlanID;
var budgetCategoryNotInSpendPlanOptionGroup, budgetCategoryInSpendPlanNotDpOptionGroup;
var lblErrorID;
var initComplete = false;
var saveButtonID;
var editPayment_BudgetPeriod;
var editPayment_isSDS, editPayment_clientID;
var editPayment_dpContractSvc;
var editPayment_DPContractID;
var editPayment_CurrentWillBePaidID, editPayment_CurrentHasBeenPaidID, editPayment_NextWillBePaidID, editPayment_NextHasBeenPaidID;
var editPayment_CurrentPeriodID, editPayment_NextPeriodID, editPayment_overlappsID;

function Init() {
    var msg;
    var lblError = GetElement(lblErrorID);
    editPayment_dpContractSvc = new Target.Abacus.Web.Apps.WebSvc.DPContract_class();
    hidPaidUpTo = GetElement(hidPaidUpToID);
    cboFreq = GetElement(cboFreqID + "_cboDropDownList");
    hidNumPayments = GetElement(hidNumPaymentsID);
    initComplete = false;
    spnAmount = GetElement(spnAmountID + "_lblReadOnlyContent");
    spnDateTo = GetElement(spnDateToID + "_lblReadOnlyContent");
    btnAddBreakdown = GetElement(btnAddBreakdownID);
    btnAddBreakdown.disabled = true;
    PrimeDateTo();
    PopulateBreakdownFrequencyCombos(cboFreq.options[cboFreq.selectedIndex].text);
    RecalcSummary();
    initComplete = true;
    chkForceGross = GetElement(chkForceGrossID);
    
    if (editSuspendForbidden == "Y") {
        alert("You do not have permission to amend a suspended Payment.");
    }

    optStatusActive = GetElement(optStatusActiveID, true);
    optStatusProv = GetElement(optStatusProvID, true);
    origStatusActive = false;
    
    if (optStatusActive) {
        if (inEditMode == "Y" && optStatusActive.checked) {
            origStatusActive = true;
            msg = "With the exception of the End Reason and Status fields,";
            msg = msg + " amending any field value will cause Abacus to set";
            msg = msg + " the status of the Payment to 'Provisional'.";
            alert(msg);
        }
    }
        
    
    if (hidPaidUpTo.value != '') {
        tbl = GetElement("tblBreakdown");
        for (var cnt = 0; cnt < tbl.all.length; cnt++) {
            if (tbl.all[cnt].tagName == 'INPUT' || tbl.all[cnt].tagName == 'SELECT') {
                tbl.all[cnt].disabled = true;
            }
        }
        btnAddBreakdown.disabled = true;
        tbl.disabled = true;
    }

    lblError.style.backgroundColor = 'transparent';

    $(chkForceGross).click(function() {
        if (origStatusActive == true) {
            optStatusProv.checked = true;
            optStatusActive.checked = false;
        }
    });

    $('input[id$="txtFinCode1_txtName"]').change(function() {
        if (origStatusActive == true) {
            optStatusProv.checked = true;
            optStatusActive.checked = false;
        }
    });

    autoSetDateFrom(true);
}

function autoSetDateFrom(enabledOnly) {
    if (inEditMode == "Y") {
        switch (hidFrequency.value) {
            case freqFIRSTWEEKid:
                if (!enabledOnly) {
                    dteDateFrom.value = Date.strftime("%d/%m/%Y", firstWeekStartDate);
                    hidDateFrom.value = dteDateFrom.value;
                }
                dteDateFrom.disabled = true;
                break;
            default:
                dteDateFrom.disabled = false;
                break;
        }
    }
}

function InPlaceFinanceCode_Changed(args) {
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }    
}

function dteDateFrom_Changed(id) {
    hidDateFrom.value = dteDateFrom.value;
    btnAddBreakdown.disabled = ((hidDateFrom.value == "" || hidFrequency.value == "-1") || isBalancingPayment === true);
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }
    PrimeDateTo();
}

function cboFrequency_OnChange() {
    // If the previous frequency was "ONCE", clear the number of payments and end reason fields..
    var txtNumPayments = GetElement(txtNumPaymentsID + "_txtTextBox");
    cboEndReason = GetElement(cboEndReasonID + "_cboDropDownList");
    hidEndReason = GetElement(hidEndReasonID);
    if (hidFrequency.value == "0") {
        txtNumPayments.value = "0";
        cboEndReason.value = "0<0";
        hidEndReason.value = cboEndReason.value;
        cboEndReason.disabled = true;
    }
        
    cboFreq = GetElement(cboFreqID + "_cboDropDownList");

    if (hidFrequency.value == cboFreq.value) return;
    hidFrequency.value = cboFreq.value;
    autoSetDateFrom(false);
    btnAddBreakdown.disabled = ((hidDateFrom.value == "" || hidFrequency.value == "-1") || isBalancingPayment === true);
    
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }

    PrimeDateTo();
    PopulateBreakdownFrequencyCombos(cboFreq.options[cboFreq.selectedIndex].text);
}

function PopulateBreakdownFrequencyCombos(mainFreqText) {
    var tbl, freqCombo, hidFreq, opt, freqComboValue, freqComboOriginalValue;
    tbl = GetElement("tblBreakdown");
    tBody = tbl.tBodies[0];
    for (index = 0; index < tBody.rows.length; index++) {
        row = tBody.rows[index];

        // Find the Frequency combo on this breakdown row..
        cell = row.cells[3];
        freqCombo = cell.getElementsByTagName("select")[0]; //
        hidFreq = cell.getElementsByTagName("input")[1];
        freqComboOriginalValue = cell.getElementsByTagName("input")[0].value;

        // Clear the content of the combo..
        freqCombo.options.length = 0;
        // Populate the combo based on the passed frequency string..
        switch (mainFreqText) {
            case "ONCE":
                AddComboItem(freqCombo, "ONCE", 0);
                freqComboValue = 0;
                break;
            case "WEEKLY":
            case "FIRST-WEEK":
                AddComboItem(freqCombo, "WEEKLY", freqWEEKLYid);
                freqComboValue = freqWEEKLYid;
                break;
            case "TWO-WEEKLY":
                freqComboValue = SetupMultiWeekBreakdownFrequency(freqCombo, freqWEEKLYid, "TWO-WEEKLY", freqTWOWEEKLYid);
                break;
            case "FOUR-WEEKLY":
                freqComboValue = SetupMultiWeekBreakdownFrequency(freqCombo, freqWEEKLYid, "FOUR-WEEKLY", freqFOURWEEKLYid);
                break;
            case "QUARTERLY (13 weeks)":
                freqComboValue = SetupMultiWeekBreakdownFrequency(freqCombo, freqWEEKLYid, "QUARTERLY (13 weeks)", freqQUARTERLY13weeksid);
                break;
            case "HALF-YEARLY (26 weeks)":
                freqComboValue = SetupMultiWeekBreakdownFrequency(freqCombo, freqWEEKLYid, "HALF-YEARLY (26 weeks)", freqHALFYEARLY26weeksid);
                break;
            case "ANNUALLY (52 weeks)":
                freqComboValue = SetupMultiWeekBreakdownFrequency(freqCombo, freqWEEKLYid, "ANNUALLY (52 weeks)", freqANNUALLY52weeksid);
                break;
        }
        if (initComplete == true) {
            freqCombo.value = freqComboValue;
            hidFreq.value = freqComboValue;
        } else {
            freqCombo.value = freqComboOriginalValue;
            hidFreq.value = freqComboOriginalValue;
        }
    }
}

function SetupMultiWeekBreakdownFrequency(combo, weeklyId, multiWeeklyDesc, multiWeeklyId) {
    combo.options.length = 0;
    AddComboItem(combo, multiWeeklyDesc, multiWeeklyId);
    AddComboItem(combo, "WEEKLY", freqWEEKLYid);
    return multiWeeklyId;
}

function AddComboItem(cbo, itemText, itemVal) {
    var opt = document.createElement("OPTION");
    cbo.options.add(opt);
    SetInnerText(opt, itemText);
    opt.value = itemVal;
}

function cboBreakdownFrequency_OnChange(id) {
    var cboFrequency = GetElement(id + "_cboDropDownList");
    var hdnFrequency = GetElement(id.replace("brkdwnThisFreq", "brkdwnThisFreqH"));

    // Force a change of status value on a change on field content..
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }

    if (hdnFrequency.value == cboFrequency.value) return;
    hdnFrequency.value = cboFrequency.value;
    RecalcSummary();
}

function cboEndReason_OnChange() {
    cboEndReason = GetElement(cboEndReasonID + "_cboDropDownList");
    hidEndReason = GetElement(hidEndReasonID);

    if (hidEndReason.value == cboEndReason.value) return;
    hidEndReason.value = cboEndReason.value;

    PrimeDateTo();
}

function txtNumOfPayments_OnChange(id) {
    var txtNumPayments = GetElement(id + "_txtTextBox");
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }
    if (txtNumPayments.value == "" || txtNumPayments.value == 0) {
        cboEndReason.value = "0<0";
        hidEndReason.value = cboEndReason.value;
    }
    PrimeDateTo();
}

function PrimeDateTo() {
    var txtNumPayments = GetElement(txtNumPaymentsID + "_txtTextBox");
    if (txtNumPayments.value == "") txtNumPayments.value = "0";
    var oldDate, newDate, dtTemp, daysCount;
    var count, increment;

    hidDateFrom = GetElement(hidDateFromID);
    dteDateFrom = GetElement(dteDateFromID + "_txtTextBox");
    
    hidFrequency = GetElement(hidFrequencyID);
    cboEndReason = GetElement(cboEndReasonID + "_cboDropDownList");
    hidEndReason = GetElement(hidEndReasonID);
    hidEndReason.value = cboEndReason.value;

    // Get the current values held in the Date From and Frequency fields
    // and combine with the NumOfPayments value in order to determine
    // the next Date To value..
    txtNumPayments.disabled = false;
    if (isNaN(txtNumPayments.value)) txtNumPayments.value = "0";

    count = parseInt(txtNumPayments.value);
    if (count < 0) count = 0;

    if (inEditMode == "Y") {
        txtNumPayments.disabled = false;
    } else {
        txtNumPayments.disabled = true;
    }

    if (hidFrequency.value == "0" || hidFrequency.value == freqFIRSTWEEKid) count = 1; // ONCE..
    if (hidFrequency.value == "-1") count = 0; // No selection..
    if (count == 0 || dteDateFrom.value == "") {
        SetInnerText(spnDateTo, "(open-ended)");
        cboEndReason.disabled = false;
        cboEndReason.selectedValue = "0<0";
        hidEndReason.value = cboEndReason.value;
        cboEndReason.disabled = true;
        hidDateTo = GetElement(hidDateToID);
        hidDateTo.value = GetInnerText(spnDateTo);
    } else {
        cboEndReason.disabled = txtNumPayments.disabled;

        // Ensure the date is in MM/DD/YYYY format before storing..
        dtTemp = dteDateFrom.value.substr(3, 3) + dteDateFrom.value.substr(0, 3) + dteDateFrom.value.substr(6);
        oldDate = new Date(dtTemp);

        switch (hidFrequency.value) {
            case freqWEEKLYid:
                increment = parseInt(freqWEEKLY) * count;                
                newDate = DateAdd("d", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqTWOWEEKLYid:
                increment = parseInt(freqTWOWEEKLY) * count;
                newDate = DateAdd("d", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqFOURWEEKLYid:
                increment = parseInt(freqFOURWEEKLY) * count;
                newDate = DateAdd("d", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqMONTHLYid:
                newDate = DateAdd("m", oldDate, count);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqQUARTERLYid:
                increment = 3 * count;
                newDate = DateAdd("m", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqHALFYEARLYid:
                increment = 6 * count;
                newDate = DateAdd("m", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqANNUALLYid:
                increment = 12 * count;
                newDate = DateAdd("m", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqQUARTERLY13weeksid:
                increment = parseInt(freqQUARTERLY13weeks) * count;
                newDate = DateAdd("d", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqHALFYEARLY26weeksid:
                increment = parseInt(freqHALFYEARLY26weeks) * count;
                newDate = DateAdd("d", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqANNUALLY52weeksid:
                increment = parseInt(freqANNUALLY52weeks) * count;
                newDate = DateAdd("d", oldDate, increment);
                newDate = DateAdd("d", newDate, -1);
                break;
            case freqFIRSTWEEKid:
                newDate = firstWeekEndDate;
                txtNumPayments.value = "1";
                txtNumPayments.disabled = true;
                SetEndReasonBySystemType(3, true);
                break;
            default:
                // ONCE selected..
                txtNumPayments.value = "1";
                txtNumPayments.disabled = true;
                newDate = oldDate;
                // Force the End Reason dropdown to 'One-Off Payment'..
                SetEndReasonBySystemType(2, true);
        }
        if (newDate) {
            SetInnerText(spnDateTo, Date.strftime("%d/%m/%Y", newDate));

        }
        hidDateTo = GetElement(hidDateToID);
        hidDateTo.value = GetInnerText(spnDateTo);
    }
    $("#" + cboEndReasonID + "_cboDropDownList").msDropDown();


    if (editPayment_isSDS == true) {
        var lblCurrentWillBePaid = GetElement(editPayment_CurrentWillBePaidID);
        var lblCurrentHasBeenPaid = GetElement(editPayment_CurrentHasBeenPaidID);
        var lblNextWillBePaid = GetElement(editPayment_NextWillBePaidID);
        var lblNextHasBeenPaid = GetElement(editPayment_NextHasBeenPaidID);
        var lblCurrentPeriod = GetElement(editPayment_CurrentPeriodID);
        var lblNextPeriod = GetElement(editPayment_NextPeriodID);
        var lbloverlapps = GetElement(editPayment_overlappsID);
        var cboFreq = GetElement(cboFreqID + "_cboDropDownList");
        var txtdatefrom = GetElement(dteDateFromID + "_txtTextBox");
        response = editPayment_dpContractSvc.GetPaymentsInBudgetPeriods(editPayment_clientID, txtdatefrom.value, hidDateTo.value,
                                        cboFreq.value, editPayment_DPContractID, hidPaidUpTo.value);
        if (response.value.ErrMsg.Success == true) {
            lblCurrentPeriod.innerText = response.value.CurrentPeriod
            lblNextPeriod.innerText = response.value.NextPeriod
            lblCurrentWillBePaid.innerText = response.value.currentPaymentsToBePaid
            lblCurrentHasBeenPaid.innerText = response.value.CurrentPaymentsMade
            lblNextWillBePaid.innerText = response.value.NextPaymentsToBePaid
            lblNextHasBeenPaid.innerText = response.value.NextPaymentsMade
            lbloverlapps.innerText = response.value.OvelappingPayments
        }

    }


}

function SetEndReasonBySystemType(systemType, disable) {
    var items = cboEndReason.options, itemsCount = items.length;
    cboEndReason.disabled = false;
    for (var ind = 0; ind < itemsCount; ind++) {
        var item = items[ind];
        if (item.value.indexOf("<" + systemType) != -1) {
            cboEndReason.value = item.value;
            hidEndReason.value = cboEndReason.value;
            break;
        }
    }
    cboEndReason.disabled = disable;
}

function Toggle(frameUID) {
    var heightAdjust = 10;  // add adjustment to counter padding issues
    window.parent.document.getElementById(frameUID).style.height = document.body.scrollHeight + heightAdjust;
    SetupFancyDropDowns();
}

function DateAdd(ItemType, DateToWorkOn, ValueToBeAdded) {
    var origDate = DateToWorkOn.getDate();
    var tempDate;
    var counter = 0;

    switch (ItemType) {
        //date portion 
        case 'd': //add days
            DateToWorkOn.setDate(DateToWorkOn.getDate() + ValueToBeAdded);
            break;
        case 'm': //add months
            DateToWorkOn.setMonth(DateToWorkOn.getMonth() + ValueToBeAdded);
            // Ensure that the new date is the last day of the previous month
            // if the new month has fewer days than the starting date month..
            if (DateToWorkOn.getDate() < origDate) {
                while (DateToWorkOn.getDate() < 5) {
                    DateToWorkOn.setDate(DateToWorkOn.getDate() - 1);
                    // Force out of interminable loop - should never be needed..
                    counter = counter + 1;
                    if (counter == 5) break;
                }
            }
            break;
        case 'y': //add years
            DateToWorkOn.setYear(DateToWorkOn.getFullYear() + ValueToBeAdded);
            break;
        //time portion 
        case 'h': //add days
            DateToWorkOn.setHours(DateToWorkOn.getHours() + ValueToBeAdded);
            break;
        case 'n': //add minutes
            DateToWorkOn.setMinutes(DateToWorkOn.getMinutes() + ValueToBeAdded);
            break;
        case 's': //add seconds
            DateToWorkOn.setSeconds(DateToWorkOn.getSeconds() + ValueToBeAdded);
            break;
    }
    return DateToWorkOn;
}

function ClientSaveClicked() {
    var response;
    txtNumPayments = GetElement(txtNumPaymentsID + "_txtTextBox");
    chkForceGross = GetElement(chkForceGrossID);

    hidNumPayments = GetElement(hidNumPaymentsID);
    hidNumPayments.value = txtNumPayments.value;
    hidForceGross = GetElement(hidForceGrossID);

    if (chkForceGross.checked) {
        hidForceGross.value = "1";
    } else {
        hidForceGross.value = "0";
    }

    hidAmount = GetElement(hidAmountID);
    hidAmount.value = GetInnerText(spnAmount);
    hidDateTo = GetElement(hidDateToID);
    hidDateTo.value = GetInnerText(spnDateTo);


    cboFreq = GetElement(cboFreqID + "_cboDropDownList");

    if (editPayment_BudgetPeriod == 3 && editPayment_isSDS == true) {
        var cboFreq = GetElement(cboFreqID + "_cboDropDownList");
        var txtdatefrom = GetElement(dteDateFromID + "_txtTextBox");
        response = editPayment_dpContractSvc.DisplaySpansMultipleFrequenciesMessage(editPayment_clientID, txtdatefrom.value, hidDateTo.value, cboFreq.value, hidNumPayments.value)
        if (response.value==true) {
            return window.confirm('The information you have entered for this payment will cause it to be apportioned across more than one Budget Period. ' +
                                '\r\rAre you sure you wish to continue?'); 
        } else {
            return true;
        }
    } else {
        return true;
   }
    
}

function cboBudgetCategory_Change(id) {
    var cboBudCat = GetElement(id + "_cboDropDownList");
    var hidBC = GetElement(id.replace("brkdwnBudgetCat", "brkdwnBudgetCatH"));

    // Do nothing if the selection hasn't changed..
    if (hidBC.value == cboBudCat.value) return;
    hidBC.value = cboBudCat.value;

    // Reset the Units on change of budget category..
    var hidUnits = GetElement(id.replace("brkdwnBudgetCat", "brkdwnThisUnitsH"));
    var idRep = id;  //id.replace(/_/g, "$");
    idRep = idRep.replace("brkdwnBudgetCat", "brkdwnThisUnits");
    txtUnits = GetElement(idRep + "_txtTextBox");
    txtUnits.value = "0.00";    
    hidUnits.value = txtUnits.value;

    // Force a change of status value on a change on field content..
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }

    RecalcSummary();
}

function txtBreakdownUnits_Change(id) {
    var idRep = id //id.replace(/_/g, "$");
    txtUnits = GetElement(idRep + "_txtTextBox");

    var hidUnits = GetElement(id.replace("brkdwnThisUnits", "brkdwnThisUnitsH"));
    hidUnits.value = txtUnits.value;

    // Force a change of status value on a change on field content..
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }

    RecalcSummary();
}

function txtBreakdownAmount_Change(id) {
    // Force a change of status value on a change on field content..
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }

    RecalcSummary();
}

function cboBreakdownFrequency_Change(id) {

    var cboFrequency = GetElement(id + "_cboDropDownList");
    var hdnFrequency = GetElement(id.replace("brkdwnThisFreq", "brkdwnThisFreqH"));

    // Force a change of status value on a change on field content..
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }

    if (hdnFrequency.value == cboFrequency.value) return;
    hdnFrequency.value = cboFrequency.value;
    RecalcSummary();

}

function RecalcSummary() {
    var tbl, tBody, row, cell, bc, units, rate, imgRemove, unitRate;
    var hidFreq, hidUnitsPaid, unitsPaid, cboBrkFreq;
    var hidBC, hidUnits, hidRate;
    var lineCost, linesTotal;
    var uom, totValue, spnCost, spnUOM, uomText;
    var selectedID, selectedType, remainder, lPos, lPosQM;
    var pnlLegend = GetElement(pnlLegendID);
    var pnlLegendNotInSpendPlan = GetElement(pnlLegendNotInSpendPlanID);
    var pnlLegendInSpendPlanNotDp = GetElement(pnlLegendInSpendPlanNotDPID);
    var showLegend = false;
    var showNotInSpendPlanLegend = false;
    var showInSpendPlanNotDpLegend = false;
    var lblError = GetElement(lblErrorID);
    var saveButton = GetElement(saveButtonID, true);
    var existingErrorText = '';
    var itemValArray;
    
    tbl = GetElement("tblBreakdown");
    tBody = tbl.tBodies[0];

    // Prevent the Frequency being amended if there are breakdowns present..
    cboFreq = GetElement(cboFreqID + "_cboDropDownList");
    if (tBody.rows.length > 0) {
        cboFreq.disabled = true;
    } else {
        cboFreq.disabled = false;
    }
    $("#" + cboFreqID + "_cboDropDownList").msDropDown();
    if (hidFrequency.value != "-1" && isBalancingPayment === false) {
        btnAddBreakdown.disabled = false;
    } else {
        btnAddBreakdown.disabled = true;
    }
    
    // Sum up the breakdown amounts..
    linesTotal = 0;
    for (index = 0; index < tBody.rows.length; index++) {
        row = tBody.rows[index];
        // Budget Category..
        cell = row.cells[0];
        bc = cell.getElementsByTagName("select")[0];
        hidBC = cell.getElementsByTagName("input")[0];
        hidBC.value = bc.value;
        // Units..
        cell = row.cells[1];
        units = cell.getElementsByTagName("input")[0];
        hidUnits = cell.getElementsByTagName("input")[1];
        // Measured In..
        cell = row.cells[2];
        uom = cell.getElementsByTagName("input")[0];
        // Frequency..
        cell = row.cells[3];
        cboBrkFreq = cell.getElementsByTagName("select")[0];
        hidFreq = cell.getElementsByTagName("input")[1];
        // Units Paid..
        cell = row.cells[4];
        hidUnitsPaid = cell.getElementsByTagName("input")[0];
        // Unit Cost..
        cell = row.cells[5];
        hidRate = cell.getElementsByTagName("input")[0];
        // Line Amount..
        cell = row.cells[6];
        totValue = cell.getElementsByTagName("input")[0];
        // Remove icon..
        cell = row.cells[7];
        imgRemove = cell.getElementsByTagName("input")[0];

        if (budgetCategoryNotInSpendPlanOptionGroup == bc.options[bc.selectedIndex].parentNode.label) {
            showLegend = true;
            showNotInSpendPlanLegend = true;
        } else if (budgetCategoryInSpendPlanNotDpOptionGroup == bc.options[bc.selectedIndex].parentNode.label) {
            showLegend = true;
            showInSpendPlanNotDpLegend = true;
        }

        // Split the (compound) Value into ID, Type and UnitRate sections..
        selectedID = bc.value;
        // itemValArray contains four spliced bit of info, arranged thus --> "a$b$c$d", where
        // a = budget category ID (equates to split item 0)
        // b = budget category type (1)
        // c = expenditure unit rate (2)
        // d = unit of measure (3)
        var itemValArray = selectedID.split("$");

        selectedID = itemValArray[0];
        if (itemValArray[1] != "") {
            unitRate = itemValArray[2];
            selectedType = itemValArray[1];
            uomText = itemValArray[3];
            if (uomText == "(blank)") uomText = "";
        } else {
            selectedType = itemValArray[1];
            unitRate = -1;
            uomText = "";
        }

        units.disabled = false;
        if (selectedType == "1") {
            // Monetary Amount, so fix the Units at "1" and enable the Unit Cost..
            if (!isBalancingPayment) {
                units.value = "1";
                hidUnits.value = units.value;
            }
            units.disabled = true;
        } else {
            // Units of Service, so enable the Units, and prime/disable the Unit Cost if a rate was passed (!= -1)..
            if (unitRate != -1) {
                hidRate.value = parseFloat(unitRate).toFixed(2);
            } else {
                hidRate.value = "0.00";
            }
            rate = hidRate.previousSibling;
            SetInnerText(rate, hidRate.value);
        }

        var unitsVal = hidUnits.value.replace(",", "");
        var weeks = GetWeeks(hidFreq.value);
        
        unitsPaid = hidUnitsPaid.previousSibling;
        hidUnitsPaid.value = (parseFloat(unitsVal) * weeks).toFixed(2);
        SetInnerText(unitsPaid, hidUnitsPaid.value);

        var rateVal = hidRate.value.replace(",", "")
        totValue.value = ((parseFloat(hidUnitsPaid.value) * parseFloat(rateVal))).toFixed(2);
        lineCost = parseFloat(totValue.value);
        if (isNaN(lineCost)) lineCost = 0.00;
        linesTotal += lineCost;

        // Update the Unit of Measure on-screen..
        spnUOM = uom.previousSibling;
        if (uomText != "") {
            SetInnerText(spnUOM, uomText);
        } else {
            SetInnerText(spnUOM, "(unspecified)");
        }

        // update frequency availability
        cboBrkFreq.disabled = isBalancingPayment;

        // Update the line total on-screen..
        spnCost = totValue.previousSibling;
        SetInnerText(spnCost, totValue.value);

        // Disable the field if the payment has a Paid up To date assigned..
        if (btnAddEnabled == "N") {
            bc.disabled = true;
            units.disabled = true;
            rate.disabled = true;
            imgRemove.disabled = true;
        }
    }    
    
    if (initComplete == false) {
        existingErrorText = GetInnerText(lblError);
    }

    if (showLegend == true) {
        pnlLegend.style.display = 'block';
        lblError.style.display = 'block';
        lblError.style.color = 'red';
        pnlLegendInSpendPlanNotDp.style.display = 'none';
        pnlLegendNotInSpendPlan.style.display = 'none';
        if (saveButton) {
            saveButton.disabled = false;
        }
        if (showInSpendPlanNotDpLegend) {
            pnlLegendInSpendPlanNotDp.style.display = 'block';
        }
        if (showNotInSpendPlanLegend) {
            pnlLegendNotInSpendPlan.style.display = 'block';
        }      
        if (budgetCategoryValidationMethod == 2) {
            if (existingErrorText.length == 0) {
                if (showNotInSpendPlanLegend == true && showInSpendPlanNotDpLegend == true) {
                    SetInnerText(lblError, 'WARNING: One or more budget category in the payment breakdown is not contained within the spend plan for the period AND One or more budget category in the payment breakdown is contained within the spend plan for the period but is not expected to be delivered via direct payments.'); 
                } else if (showNotInSpendPlanLegend == true) {
                    SetInnerText(lblError, 'WARNING: One or more budget category in the payment breakdown is not contained within the spend plan for the period.'); 
                } else if (showInSpendPlanNotDpLegend == true) {
                    SetInnerText(lblError, 'WARNING: One or more budget category in the payment breakdown is contained within the spend plan for the period but is not expected to be delivered via direct payments.'); 
                }                                
                lblError.style.color = 'orange';
            }
        } else if (budgetCategoryValidationMethod == 1) {
            if (showNotInSpendPlanLegend == true) {
                SetInnerText(lblError, 'ERROR: Payment line cannot be saved as at least one budget category from the payment break down grid is not is not contained within the spend plan for the period. ' + existingErrorText);
            } else if (showInSpendPlanNotDpLegend == true) {
                SetInnerText(lblError, 'ERROR: Payment line cannot be saved as at least one budget category from the payment break down grid is contained within the spend plan for the period but is not expected to be delivered via direct payments. ' + existingErrorText);
            }  
            if (saveButton) {
                saveButton.disabled = true;
            }
        } else if (budgetCategoryValidationMethod == 3) {
            if (showNotInSpendPlanLegend == true) {
                SetInnerText(lblError, 'ERROR: Payment line cannot be saved as at least one budget category from the payment break down grid is not is not contained within the spend plan for the period. ' + existingErrorText);
            } else if (showInSpendPlanNotDpLegend == true) {
                if (existingErrorText.length == 0) {
                    SetInnerText(lblError, 'WARNING: One or more budget category in the payment breakdown is contained within the spend plan for the period but is not expected to be delivered via direct payments. ' + existingErrorText);
                    lblError.style.color = 'orange';
                }              
            }                        
            if (saveButton && showNotInSpendPlanLegend == true) {
                saveButton.disabled = true;
            }
        }
    } else {
        pnlLegend.style.display = 'none';
        if (existingErrorText.length == 0) {
            lblError.style.display = 'none';
            lblError.style.color = 'red';
        }         
        if (saveButton) {
            saveButton.disabled = false;
        }  
    }

    // Update the overall cost field..
    if (spnAmount) spnAmount.innerHTML = (linesTotal).toString().formatCurrency();
    if (hidAmount && spnAmount) hidAmount.value = GetInnerText(spnAmount);
}

function btnRemoveBreakdown_Click() {
    var resp = window.confirm("Are you sure you wish to remove this payment breakdown?");

    // Force a change of status value on a change of breakdown content..
    if (resp && (origStatusActive == true)) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }

    if (resp) {
        cboFreq = GetElement(cboFreqID + "_cboDropDownList");
        cboFreq.disabled = false;
        $("#" + cboFreqID + "_cboDropDownList").msDropDown();
    }
    
    return resp;
}

function EditPayment_CancelClicked() {
    hidDateFrom = GetElement(hidDateFromID);    
    hidDateFrom.value = "";
}

function btnAddBreakdown_Click() {
    if (spendPlanID) {
    } else {
        if (spendPlanID > 0) {
            if (budgetCategoryValidationMethod == 1 || budgetCategoryValidationMethod == 3) {
                alert('A spend plan could not be located for the payment line. You will not be able to save the current record.');
            }
            else if (budgetCategoryValidationMethod == 2) {
                alert('A spend plan could not be located for the payment line. You will be able to save the current record but warnings will be given.');
            }
        }
    }

    // Force a change of status value on a change of breakdown content..
    if (origStatusActive == true) {
        optStatusProv.checked = true;
        optStatusActive.checked = false;
    }

    cboFreq = GetElement(cboFreqID + "_cboDropDownList");
    cboFreq.disabled = false;
    $("#" + cboFreqID + "_cboDropDownList").msDropDown();
    
    return true;
}

function SetupFancyDropDowns() {
    $("body select").msDropDown();
    if (isBalancingPayment) {
        var bcDropDownData = $("select[id*=\'brkdwnBudgetCatbrkdwn\']").msDropDown().data("dd");
        if (bcDropDownData) {
            bcDropDownData.disabled(true)
        }
    }
}

$(document).ready(function(e) {
    try {
        SetupFancyDropDowns();
        if (isBalancingPayment) {
            $("input[id*=\'brkdwnRemove\']").attr({ 'disabled': true, 'title': '' });
        }
    } catch (e) {
        alert(e.message);
    }
});


function editPayment_CanSave() {
    var response;
    if (editPayment_BudgetPeriod == 3 && editPayment_isSDS == true) {
        var txtdatefrom = GetElement(dteDateFromID + "_txtTextBox");
        response = Edit_spendPlanSvc.DisplayOverlappingWarningMessage(Edit_spendPlanID, Edit_ClientID, txtdatefrom.value)
        if (response.value) {
            return window.confirm('Direct Payments exist that overlap with this Spend Plan. ' +
                                'Changing the day of the week upon which the Spend Plan starts ' +
                                'may cause an inconsistency between when budget is allocated and when payments are made.' +
                                '\r\rAre you sure you wish to continue?');
        } else {
            return true;
        }
    } else {
        return true;
    }

}

function GetWeeks(breakdownFrequency) {
    var isWeekly = (breakdownFrequency == freqWEEKLYid);
    switch (hidFrequency.value) {
        case freqWEEKLYid:
            return parseInt(freqWEEKLY) / 7;
            break;
        case freqTWOWEEKLYid:
            return (isWeekly ? (parseInt(freqTWOWEEKLY) / 7) : 1);
            break;
        case freqFOURWEEKLYid:
            return (isWeekly ? (parseInt(freqFOURWEEKLY) / 7) : 1);
            break;
        case freqQUARTERLY13weeksid:
            return (isWeekly ? (parseInt(freqQUARTERLY13weeks) / 7) : 1);
            break;
        case freqHALFYEARLY26weeksid:
            return (isWeekly ? (parseInt(freqHALFYEARLY26weeks) / 7) : 1);
            break;
        case freqANNUALLY52weeksid:
            return (isWeekly ? (parseInt(freqANNUALLY52weeks) / 7) : 1);
            break;
        case freqFIRSTWEEKid:
            var fromDate = Date.toDateFromString(hidDateFrom.value);
            var toDate = Date.toDateFromString(hidDateTo.value);
            return ((1 / 7) * (fromDate.diff('d', toDate) + 1));
            break;
        default:
            return 1;
    }
}

addEvent(window, "load", Init);
