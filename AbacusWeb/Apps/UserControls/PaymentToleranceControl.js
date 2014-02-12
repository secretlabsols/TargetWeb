

function Init() {
    chkSuspendInvoiceWhenPlannedUnitsExceeded_OnClick();
    cboPaymentToleranceCombinationMethod_OnChange();
    setToleranceCombinationMethodState();
    //check percentage values
    jQuery('[id*=txtAcceptableAdditionalPaymentAsPercentage]').change(function() { checkPercentageValue(jQuery('[id*=txtAcceptableAdditionalPaymentAsPercentage]')); });
    jQuery('[id*=txtAcceptableAdditionalUnitsAsPercentage]').change(function() { checkPercentageValue(jQuery('[id*=txtAcceptableAdditionalUnitsAsPercentage]')); });
    //check single values
    jQuery('[id*=txtAcceptableAdditionalUnits]').change(function() { checkCurrencyValue(jQuery('[id*=txtAcceptableAdditionalUnits]')); });
    jQuery('[id*=txtUnitsOfServiceCappedAt]').change(function() { checkCurrencyValue(jQuery('[id*=txtUnitsOfServiceCappedAt]')); });
    //check currency values
    jQuery('[id*=txtAcceptableAdditionalPayment]').change(function() { checkCurrencyValue(jQuery('[id*=txtAcceptableAdditionalPayment]')); });
    jQuery('[id*=txtCostOfServiceCappedAt]').change(function() { checkCurrencyValue(jQuery('[id*=txtCostOfServiceCappedAt]')); });
}

//If user entered payment tolerance group then can not change
//tolerance combination method
function setToleranceCombinationMethodState() {

    if (PaymentToleranceControl_ptgSystemType == Target.Abacus.Library.PaymentToleranceGroupSystemTypes.UserEnteredPaymentToleranceGroup) {

        //disbale all drop downs with id containing cboPaymentToleranceCombinationMethod
        jQuery('[id*=cboPaymentToleranceCombinationMethod]').attr("disabled", "disabled");
        
    }

}

function cboPaymentToleranceCombinationMethod_OnChange() {

    if (PaymentToleranceControl_cboPaymentToleranceCombinationMethod) {
        if (PaymentToleranceControl_cboPaymentToleranceCombinationMethod.value == Target.Abacus.Library.ToleranceCombinationMethod.And) {

            PaymentToleranceControl_lblWarning.innerHTML = PaymentToleranceControl_AND_TOL_COMBI_MESSAGE;

        }
        else {

            PaymentToleranceControl_lblWarning.innerHTML = PaymentToleranceControl_OR_TOL_COMBI_MESSAGE;

        }
    }

}

function chkSuspendInvoiceWhenPlannedUnitsExceeded_OnClick() {

//    if (PaymentToleranceControl_chkSuspendInvoiceWhenPlannedUnitsExceeded) {
//        if (PaymentToleranceControl_chkSuspendInvoiceWhenPlannedUnitsExceeded.checked == true) {
//            //enable all textboxes
//            jQuery('input[type=text]').removeAttr("disabled");

//        }
//        else {
//            //reset all textbox values
//            jQuery('input[type=text]').val('');
//            //disable all textboxes 
//            jQuery('input[type=text]').attr("disabled", "disabled");

//        }
//    }

}

function checkPercentageValue(controlId) {
    if (controlId[1].value < 0) {
        controlId[1].value = 0;
    } else if (controlId[1].value > 100) {
        controlId[1].value = 100;
    }
    else if (isNaN(controlId[1].value)) {
        controlId[1].value = "0.00";
    }
    
}

function checkCurrencyValue(controlId) {
    if (controlId[1].value < 0.00) {
        controlId[1].value = 0.00;
    } else if (controlId[1].value > 9999.99) {
        controlId[1].value = 9999.99;
    }
    else if (isNaN(controlId[1].value)) {
        controlId[1].value = "0.00";
    }
}
addEvent(window, "load", Init);