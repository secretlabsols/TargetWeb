var rdoReportOnly, rdoGeneratePayments, allowGenerate, allowReport;

function Init() {
    // conditionally disable radios
    rdoGeneratePayments.disabled = !allowGenerate;
    rdoReportOnly.disabled = !allowReport;
    // conditionally set radios based on permissions
    rdoGeneratePayments.checked = false;
    rdoReportOnly.checked = false;
    if (!allowGenerate && allowReport) {
        rdoReportOnly.checked = true;
    } else {
        rdoGeneratePayments.checked = true;
    }
    optMode_Click();
}

function optMode_Click() {

    if (rdoReportOnly.checked == true) {
        rdoGeneratePayments.checked = false;
    }
    else if (rdoGeneratePayments.checked == true) {
        rdoReportOnly.checked = false;
    }

}

function CustomValidatorPaymentPreviewOptions_ClientValidate(source, args) {
    if (rdoReportOnly.checked || rdoGeneratePayments.checked) {
        args.IsValid = true;
    }
    else {
        args.IsValid = false;
    }

}

addEvent(window, "load", Init);