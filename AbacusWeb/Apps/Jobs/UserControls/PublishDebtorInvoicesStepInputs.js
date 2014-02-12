var optInvoiceUnprinted, optInvoiceSingle;
var chkProduceStatements, chkReplaceText;
var lblBillingType, cboBillingType, cboBillingTypeID;
var invoicePickerID, invoicePickerRef, invoicePickerName, invoicePickerCmd;
var invoicePickerValidation, transFromValidation, transToValidation;
var dteFromID, dteToID;
var hidCurrInvoice, hidInvoiceChoice, hidProduceStatements, hidReplaceText;

$(document).ready(function () {
    cboBillingType = GetElement(cboBillingTypeID + "_cboDropDownList");
    invoicePickerRef = GetElement(invoicePickerID + "_txtRef");
    invoicePickerName = GetElement(invoicePickerID + "_txtName");
    invoicePickerCmd = GetElement(invoicePickerID + "_btnFind");
    invoicePickerValidation = document.getElementById(invoicePickerID + "_valRequired");
    transFromValidation = document.getElementById(dteFromID + "_valRequired");
    transToValidation = document.getElementById(dteToID + "_valRequired");

    optInvoiceUnprinted.onclick = function () {
        optInvoices_Click();
    }
    optInvoiceSingle.onclick = function () {
        optInvoices_Click();
    }
    optInvoices_Click();

    chkProduceStatements.onclick = function () {
        chkProduceStatements_Click();
    }
    chkProduceStatements_Click();
    chkReplaceText.onclick = function () {
        chkReplaceText_Click();
    }
    chkReplaceText_Click();

    dteTransFrom_Changed(dteFromID);
    dteTransTo_Changed(dteToID);
});

function optInvoices_Click() {
    invoicePickerValidation = document.getElementById(invoicePickerID + "_valRequired");

    // Enable/disable the Billing Type dropdown and invoice selector as appropriate..
    if (optInvoiceUnprinted.checked) {
        lblBillingType.disabled = false;
        cboBillingType.disabled = false;

        if (typeof (invoicePickerValidation) != undefined && invoicePickerValidation != null) {
            ValidatorEnable(invoicePickerValidation, false);
        }
        invoicePickerRef.disabled = true;
        invoicePickerName.disabled = true;
        invoicePickerCmd.disabled = true;

        hidInvoiceChoice.value = "1";
    } else {
        lblBillingType.disabled = true;
        cboBillingType.disabled = true;
        invoicePickerRef.disabled = false;
        invoicePickerName.disabled = false;
        invoicePickerCmd.disabled = false;

        hidInvoiceChoice.value = "2";
        if (typeof (invoicePickerValidation) != undefined && invoicePickerValidation != null) {
            ValidatorEnable(invoicePickerValidation, true);
        }
    }
}

function chkProduceStatements_Click() {
    transFromValidation = document.getElementById(dteFromID + "_valRequired");
    transToValidation = document.getElementById(dteToID + "_valRequired");

    if (chkProduceStatements.checked) {
        hidProduceStatements.value = '1';

        if (typeof (transFromValidation) != undefined && transFromValidation != null) {
            ValidatorEnable(transFromValidation, true);
        }

        if (typeof (transToValidation) != undefined && transToValidation != null) {
            ValidatorEnable(transToValidation, true);
        }
    } else {
        hidProduceStatements.value = '0';

        if (typeof (transFromValidation) != undefined && transFromValidation != null) {
            ValidatorEnable(transFromValidation, false);
        }

        if (typeof (transToValidation) != undefined && transToValidation != null) {
            ValidatorEnable(transToValidation, false);
        }
    }
}

function chkReplaceText_Click() {
    if (chkReplaceText.checked) {
        hidReplaceText.value = '1';
    } else {
        hidReplaceText.value = '0';
    }
}

function dteTransFrom_Changed(id) {
    var dateFromCtrl = $('#' + dteFromID + '_txtTextBox');
    var dateToCtrl = $('#' + dteToID + '_txtTextBox');

    if (dateFromCtrl.val().length > 0) {
        var dateToDate = dateFromCtrl.val().toDate();
        dateToDate.setDate(dateToDate.getDate());
        dateToCtrl.datepicker('option', 'minDate', dateToDate);
    }
}

function dteTransTo_Changed(id) {
    var dateFromCtrl = $('#' + dteFromID + '_txtTextBox');
    var dateToCtrl = $('#' + dteToID + '_txtTextBox');

    if (dateToCtrl.val().length > 0) {
        var dateFromDate = dateToCtrl.val().toDate();
        dateFromDate.setDate(dateFromDate.getDate());
        dateFromCtrl.datepicker('option', 'maxDate', dateFromDate);
    }
}
