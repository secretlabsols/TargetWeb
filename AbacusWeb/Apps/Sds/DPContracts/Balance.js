var btnCreatePayment, btnCreateLetter, btnMarkAsBalanced, txtBalancingAmount, dtePeriodFrom, dtePeriodTo, cbExcludeFromCreditors, errorNonZeroBalancingAmount = 'Please enter a balancing amount that is not equal to 0', errorNoPeriodFrom = 'Please select a Period From';

$(document).ready(function() {
    dpContractSvc = new Target.Abacus.Web.Apps.WebSvc.DPContract_class();
    btnCreatePayment = $('input[id$=\'btnCreatePayment\']');
    btnCreateLetter = $('input[id$=\'btnCreateLetter\']');
    btnMarkAsBalanced = $('input[id$=\'btnMarkAsBalanced\']');
    txtBalancingAmount = $('input[id$=\'txtBalancingAmount_txtTextBox\']').change(function() {
        resetControlVisibility();
    });
    dtePeriodFrom = $('input[id$=\'dtePeriodFrom_txtTextBox\']');
    dtePeriodTo = $('input[id$=\'dtePeriodTo_txtTextBox\']');
    cbExcludeFromCreditors = $('input[id$=\'cbExcludeFromCreditors\']');
    dtePeriodFrom_Changed();
    txtBalancingAmount.val(parseFloat(txtBalancingAmount.val()).toFixed(2));    
    resetControlVisibility();
    setTimeout(function() { txtBalancingAmount.focus().select(); FireEvent(txtBalancingAmount[0], 'change'); }, 1);
});

function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    if (!url) {
        window.close();
    }
    else {
        document.location.href = unescape(url);
    }
}

function btnCreateBalancingPayment_Click() {
    createBalancingPayment();
}

function btnMarkAsBalanced_Click(isBalanced) {
    markAsBalancedPayment(isBalanced);
}

function disableForm(disabled) {
    $(':button, :submit, tr, input, select').attr('disabled', disabled);
}

function displayLoadingWithDisabling(display, disabled) {
    DisplayLoading(display);
    disableForm(disabled);
}

function dtePeriodFrom_Changed() {
    dtePeriodTo.datepicker("option", "minDate", dtePeriodFrom.val().toDate());
    resetControlVisibility();
}

function getBalancingAmount() {
    return parseFloat(txtBalancingAmount.val());
}

function getExcludeFromCreditors() {
    return cbExcludeFromCreditors.is(':checked');
}

function getPeriodFrom() {    
    return dtePeriodFrom.datepicker("getDate");
}

function getPeriodTo() {
    return dtePeriodTo.datepicker("getDate");
}

function resetControlVisibility() {
    var disabled = false, btnCreatePaymentToolTip = 'Create Payment?', btnCreateLetterToolTip = 'Create Letter?', btnMarkAsBalancedToolTip = 'Mark As Balanced?';
    if (getBalancingAmount() === 0) {
        txtBalancingAmount.focus().select();
        disabled = true;
        btnCreatePaymentToolTip = errorNonZeroBalancingAmount;
        btnCreateLetterToolTip = errorNonZeroBalancingAmount;
        btnMarkAsBalancedToolTip = errorNonZeroBalancingAmount;
    }
    if (dtePeriodFrom.val() === '') {
        disabled = true;
        btnCreatePaymentToolTip = errorNoPeriodFrom;
        btnCreateLetterToolTip = errorNoPeriodFrom;
        btnMarkAsBalancedToolTip = errorNoPeriodFrom;
    }
    btnCreatePayment.attr({ disabled: disabled, title: btnCreatePaymentToolTip });
    btnCreateLetter.attr({ disabled: disabled, title: btnCreateLetterToolTip });
    btnMarkAsBalanced.attr({ disabled: disabled, title: btnMarkAsBalancedToolTip });
}

function createBalancingPayment() {
    displayLoadingWithDisabling(false, true);
    if (window.confirm('Are you sure you wish to create this payment?')) {
        displayLoadingWithDisabling(true, true);   
        dpContractSvc.CreateBalancingPaymentForDpContract(qsDpContractID, getBalancingAmount(), getPeriodFrom(), getPeriodTo(), getExcludeFromCreditors(), createBalancingPaymentCallBack);
    } else {
        displayLoadingWithDisabling(false, false);
        resetControlVisibility();
    }
}

function createBalancingPaymentCallBack(serviceResponse) {
    if (CheckAjaxResponse(serviceResponse, dpContractSvc.url)) {
        btnBack_Click();
        return false;
    }
    displayLoadingWithDisabling(false, false);
    resetControlVisibility();
}

function markAsBalancedPayment(isBalanced) {
    displayLoadingWithDisabling(false, true);
    if (window.confirm('Are you sure you wish to ' + ((isBalanced === false) ? 'un-' : '') + 'mark this payment as balanced?')) {
        displayLoadingWithDisabling(true, true);
        dpContractSvc.MarkDpContractAsBalanced(qsDpContractID, isBalanced, markAsBalancedPaymentCallBack);
    } else {
        displayLoadingWithDisabling(false, false);
        resetControlVisibility();
    }
}

function markAsBalancedPaymentCallBack(serviceResponse) {
    if (CheckAjaxResponse(serviceResponse, dpContractSvc.url)) {
        btnBack_Click();
        return false;
    }
    displayLoadingWithDisabling(false, false);
    resetControlVisibility();
}

function validateBalancingPayment() {
    if (getBalancingAmount() === 0) {
        alert(errorNonZeroBalancingAmount);
        txtBalancingAmount.focus().select();
        return false;
    }
    return true;
}





