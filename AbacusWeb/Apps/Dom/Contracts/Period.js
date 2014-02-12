
var Period_useEnhancedRateDays, Period_inUseByNonSpecifyDoWDso, Period_FrameworkTypeId, Period_FrameworkTypeId_CG = 4;
var Period_chkVisitBasedPlans, Period_chkSpecifyPlannedDoW, Period_spnDayOfWeekWarning;

function Init() {
    Period_chkVisitBasedPlans = GetElement("chkVisitBasedPlans_chkCheckbox", true);
    Period_chkSpecifyPlannedDoW = GetElement("chkSpecifyPlannedDoW_chkCheckbox");
    Period_spnDayOfWeekWarning = GetElement("spnDayOfWeekWarning");
    if (Period_FrameworkTypeId_CG) {
        var pnlServiceOutcomeGroup = $('div[id$=\'pnlServiceOutcomeGroup\']');
        var pnlVisitCodeGroup = $('div[id$=\'pnlVisitCodeGroup\']');
        pnlServiceOutcomeGroup.insertBefore(pnlVisitCodeGroup);
        
    }
    ShowPlannedDoWWarning(false);
    DisplayLoading(false);
}

function chkSpecifyPlannedDoW_Click() {
    ShowPlannedDoWWarning(true);
}

function chkVisitBasedPlans_Click() {
	if(Period_inUseByNonSpecifyDoWDso) {
	    Period_chkSpecifyPlannedDoW.checked = false;
	    Period_chkSpecifyPlannedDoW.disabled = true;
    } else if (Period_chkVisitBasedPlans.checked) {
        Period_chkSpecifyPlannedDoW.checked = true;
        Period_chkSpecifyPlannedDoW.disabled = true;
    } else if(Period_useEnhancedRateDays) {
        Period_chkSpecifyPlannedDoW.checked = true;
        Period_chkSpecifyPlannedDoW.disabled = false;
	} else {
	    Period_chkSpecifyPlannedDoW.disabled = false;
	}
}

function stdbutEdit_Click() {
    DisplayLoading(true);
}

function ShowPlannedDoWWarning(promptUser) {
    var msg = 
        "WARNING - CONFIGURATION NOT RECOMMENDED\n\n" +
        "This contract uses enhanced rate days and you have chosen not to record the day of week that service is to be provided in the care plan.\n\n" +
        "This means it is not possible to correctly determine if/when enhanced rates should be used for the care plan and standard rates will be used instead.\n\n" +
        "This configuration has the following consequences:\n\n" +
        "  - Commitment reporting will be understated for weeks that contain enhanced rate days.\n\n" +
        "  - When actual service is entered, provider invoices may automatically suspend for weeks that contain enhanced rate days " +
             "(as actual service cost may exceed the planned cost) and, as a result, will require manual processing.\n\n";

    if (Period_useEnhancedRateDays && !Period_chkSpecifyPlannedDoW.checked) {
        if(!promptUser || window.confirm(msg + "Are you sure you wish to proceed with this configuration?")) {
            SetInnerText(Period_spnDayOfWeekWarning, msg);
            Period_spnDayOfWeekWarning.style.display = "inline";
        } else {
            Period_chkSpecifyPlannedDoW.checked = true;
            Period_spnDayOfWeekWarning.style.display = "none";
        }
    } else {
        Period_spnDayOfWeekWarning.style.display = "none";
    }
}

$(document).ready(function() {
    //get the button mode, and disable controls if not editing or adding new
    var $DisableControls = (parseInt($('input[id$="hidMode"]').val()) < 2);
    if ($('input[id$="rbInvCreatedSummaryProForma"]').is(':checked')) {
        //Option summary-level pro-forma invoices selected
        //Enable the Payment Request controls
        $('input[id*="rbPr"]').attr("disabled", $DisableControls);
        var $disableDropDown = (!$('input[id$="rbPrCouncilStaffRequestPayments"]').is(':checked') || $DisableControls);
        $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').attr("disabled", $disableDropDown);
        $('input[id*="cbPrProviderEmail"]').attr("disabled", $DisableControls);
        var $disableEmailTxtBox = ($('input[id*="cbPrProviderEmail"]').is(':checked') || $DisableControls);
        $('input[id*="tbPrProviderEmail"]').attr("disabled", $disableEmailTxtBox);

    } else {
        //Option summary-level pro-forma invoices was not selected
        //Disable all Payment request Controls
        $('input[id*="rbPr"]').attr("disabled", true);
        $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').attr("disabled", true);
        $('input[id*="PrProviderEmail"]').attr("disabled", true);
        $('input[id*="rbPr"]').attr("checked", false);
        $('input[id*="cbPrProviderEmail"]').attr("checked", false);
        $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').val("");
    };

    //Handle the click event of the Provider Invoice Input Method option buttons
    $('input[id*="rbInv"]').click(function() {

        if ($('input[id$="rbInvCreatedVisitProForma"]').is(':checked')) {
            $('input[id$="chkAutoSetSecondryVisit"]').attr("disabled", false);
        } else {
            $('input[id$="chkAutoSetSecondryVisit"]').attr("disabled", true);
            $('input[id$="chkAutoSetSecondryVisit"]').attr("checked", false);
        }
        if ($('input[id$="rbInvCreatedSummaryProForma"]').is(':checked')) {
            //Option summary-level pro-forma invoices selected
            //Enable the Payment Request controls
            $('input[id*="rbPr"]').attr("disabled", false);
            var $enableDropDown = $('input[id$="rbPrCouncilStaffRequestPayments"]').is(':checked');
            $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').attr("disabled", !$enableDropDown);
            $('input[id*="cbPrProviderEmail"]').attr("disabled", true);
            $('input[id*="tbPrProviderEmail"]').attr("disabled", true);

        } else {
            //Option summary-level pro-forma invoices was not selected
            //Disable all Payment request Controls
            $('input[id*="rbPr"]').attr("disabled", true);
            $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').attr("disabled", true);
            $('input[id*="PrProviderEmail"]').attr("disabled", true);
            $('input[id*="rbPr"]').attr("checked", false);
            $('input[id*="cbPrProviderEmail"]').attr("checked", false);
            $('input[id*="tbPrProviderEmail_txtTextBox"]').val("")
            $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').val("0");
        };
    });
    //Handle the click even ot the Payment Request Type Option Buttons
    $('input[id*="rbPr"]').click(function() {
        if ($('input[id$="rbPrCouncilStaffRequestPayments"]').is(':checked')) {
            $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').attr("disabled", false);
        } else {
            $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').attr("disabled", true);
            $('select[id$="ddPrMinPayPeriod_cboDropDownList"]').val("0");

        };
        $('input[id*="cbPrProviderEmail"]').attr("disabled", false);
        $('input[id*="cbPrProviderEmail"]').attr("checked", true);
        $('input[id*="tbPrProviderEmail_txtTextBox"]').val($('input[id$="hidProviderEmail"]').val());
        $('input[id*="tbPrProviderEmail"]').attr("disabled", true);
        

    });
    //Handle the email checkbox click event
    $('input[id$="cbPrProviderEmail_chkCheckbox"]').click(function() {
        if ($('input[id$="cbPrProviderEmail_chkCheckbox"]').is(':checked')) {
            $('input[id*="tbPrProviderEmail"]').attr("disabled", true);
            $('input[id*="tbPrProviderEmail_txtTextBox"]').val($('input[id$="hidProviderEmail"]').val());
        } else {
            $('input[id*="tbPrProviderEmail"]').attr("disabled", false);
        };
    });
    //Hook into the save button click event
    $('input[id$="btnSave"]').click(function() {
        //if one of the payment request options are selected
        if ($('input[id$="rbPrCouncilStaffRequestPayments"]').is(':checked') || $('input[id$="rbPrCareProvRequestPayments"]').is(':checked')) {
            //if an email address is not entered
            if ($('input[id*="tbPrProviderEmail_txtTextBox"]').val() == "") {
                return window.confirm("Payment requests for this contract period may be made via Abacus Extranet but a notification email address has not been entered. Are you sure you wish to continue?")
            }
        }
    });
});

addEvent(window, "load", Init);