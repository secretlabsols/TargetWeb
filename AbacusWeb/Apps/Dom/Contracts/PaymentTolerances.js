function Init() {

    optPaymentToleranceSetting_Click();

    if (typeof(iFrameName) != "undefined") {
        parent.resizeIframe(document.body.scrollHeight, iFrameName);
    }
}

function optPaymentToleranceSetting_Click() {

    if (rdoSuspendAsIndicatedInSystemSetting.checked == true) {
        rdoTolerateRules.checked = false;

        if (paymentToleranceDisplayMode == Target.Abacus.Library.PaymentTolerance.PaymentToleranceDisplayMode.Contract) {

            //uncheck checkbox
            jQuery('input[type=checkbox]').attr('checked', false);

            //reset all textbox values
            jQuery('input[type=text]').val('');
            //disable all textboxes
            jQuery('input[type=text]').attr("disabled", "disabled");
        
            divPlaceHolderPaymentTolerances.disabled = true;
        }
        else if (paymentToleranceDisplayMode == Target.Abacus.Library.PaymentTolerance.PaymentToleranceDisplayMode.ServiceOrder) {
            divPlaceHolderPaymentTolerances.style.visibility = "hidden";
        }
                
    }
    else if (rdoTolerateRules.checked == true) {
    
        rdoSuspendAsIndicatedInSystemSetting.checked = false;
        divPlaceHolderPaymentTolerances.style.visibility = "visible";
        divPlaceHolderPaymentTolerances.disabled = false;
        //chkSuspendInvoiceWhenPlannedUnitsExceeded_OnClick();
        //enable all textboxes
        jQuery('input[type=text]').removeAttr("disabled");
        
    }

}

addEvent(window, "load", Init);