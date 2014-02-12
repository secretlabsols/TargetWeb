var recurrencePatternID, RecurrencePattern_mode;
var txtDailyNoDaysID, txtMonthlyDayNoID, txtMonthlyDayMonthsID, cboMonthlyTypeID, cboMonthlyDayID, txtMonthlyDayMonths2ID, txtMonthlyPlusMinusID;
var cboYearlyEveryMonthID, txtYearlyEveryMonthNoID, cboYearlyTypeID, cboYearlyType2ID, cboYearlyMonthID, txtYearlyPlusMinusID;
var optDailyEveryXDayID, optDailyEveryWeekDayID, optMonthlyDayofMonthID, optMonthlyPatternID, optYearlyEveryID, optEndOnID;
var dteDateToID, txtOccurrencesID;
var optDailyEveryDay, optDailyEveryWeekDay, optMonthlyDayofMonth, optMonthlyPattern, optYearlyEvery, optEndOn;

function Init() {
    optDailyEveryDay = GetElement(optDailyEveryXDayID);
	optDailyEveryWeekDay = GetElement(optDailyEveryWeekDayID);
	optMonthlyDayofMonth = GetElement(optMonthlyDayofMonthID);
	optYearlyEvery  = GetElement(optYearlyEveryID);
	optEndOn  = GetElement(optEndOnID);
	
	if(RecurrencePattern_mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew || RecurrencePattern_mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
        optDaily_Click();
	    optMonthly_Click();
	    optYearly_Click();
	    optEnd_Click();
    }
	
}


function tabStrip_ActiveTabChanged(sender, args) {
    var hidSelectedTab = GetElement(recurrencePatternID + "_hidSelectedTab");
    hidSelectedTab.value = sender.get_activeTab().get_headerText();
}

function optDaily_Click() {
    var txtNoDays;
    txtNoDays = GetElement(txtDailyNoDaysID + '_txtTextBox');
    if (optDailyEveryDay.checked == true) {
        txtNoDays.disabled = false;
    } else {
        txtNoDays.disabled = true;
    }
}


function optMonthly_Click() {
    var txtMonthDay, txtMonthNo, cboPattern1, cboPattern2, txtMonthNo2, txtPlusMinus;
    txtMonthDay = GetElement(txtMonthlyDayNoID + '_txtTextBox');
    txtMonthNo = GetElement(txtMonthlyDayMonthsID + '_txtTextBox');
    cboPattern1 = GetElement(cboMonthlyDayID + '_cboDropDownList');
    cboPattern2 = GetElement(cboMonthlyTypeID + '_cboDropDownList');
    txtMonthNo2 = GetElement(txtMonthlyDayMonths2ID + '_txtTextBox');
    txtPlusMinus = GetElement(txtMonthlyPlusMinusID + '_txtTextBox');
    
    if (optMonthlyDayofMonth.checked == true) {
        txtMonthDay.disabled = false;
        txtMonthNo.disabled = false;
        cboPattern1.disabled = true;
        cboPattern2.disabled = true;
        txtMonthNo2.disabled = true;
        txtPlusMinus.disabled = true;
    } else {
        txtMonthDay.disabled = true;
        txtMonthNo.disabled = true;
        cboPattern1.disabled = false;
        cboPattern2.disabled = false;
        txtMonthNo2.disabled = false;
        txtPlusMinus.disabled = false;
    }
}

function optYearly_Click() {
    var cboYearlyMonth, txtMonthNo, cboPattern1, cboPattern2, cboPattern3, txtPlusMinus;
    cboYearlyMonth = GetElement(cboYearlyEveryMonthID + '_cboDropDownList');
    txtMonthNo = GetElement(txtYearlyEveryMonthNoID + '_txtTextBox');
    cboPattern1 = GetElement(cboYearlyTypeID + '_cboDropDownList');
    cboPattern2 = GetElement(cboYearlyType2ID + '_cboDropDownList');
    cboPattern3 = GetElement(cboYearlyMonthID + '_cboDropDownList');
    txtPlusMinus = GetElement(txtYearlyPlusMinusID + '_txtTextBox');
    
    if (optYearlyEvery.checked == true) {
        cboYearlyMonth.disabled = false;
        txtMonthNo.disabled = false;
        cboPattern1.disabled = true;
        cboPattern2.disabled = true;
        cboPattern3.disabled = true;
        txtPlusMinus.disabled = true;
    } else {
        cboPattern1.disabled = false;
        cboPattern2.disabled = false;
        cboPattern3.disabled = false;
        txtPlusMinus.disabled = false;
        cboYearlyMonth.disabled = true;
        txtMonthNo.disabled = true;
    }

}

function optEnd_Click() {
    var dteEndDate, dteOccurrences, btnEndDate;
    dteEndDate = GetElement(dteDateToID + '_txtTextBox');
    btnEndDate = GetElement(dteDateToID + '_btnDatePicker');
    dteOccurrences = GetElement(txtOccurrencesID + '_txtTextBox');

    if (optEndOn.checked == true) {
        dteEndDate.disabled = false;
        btnEndDate.disabled = false;
        dteOccurrences.disabled = true;
    } else {
        dteOccurrences.disabled = false;
        dteEndDate.disabled = true;
        btnEndDate.disabled = true;
    }
}

addEvent(window, "load", Init);