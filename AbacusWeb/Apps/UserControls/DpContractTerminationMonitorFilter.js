var DpContractTerminationMonitorFilter_QsFilterContractTypeKey, DpContractTerminationMonitorFilter_QsFilterIsBalancedKey, DpContractTerminationMonitorFilter_QsFilterTerminationDateFromKey, DpContractTerminationMonitorFilter_QsFilterTerminationDateToKey, DpContractTerminationMonitorFilter_QsFilterUnderOrOverPaymentsKey;
var DpContractTerminationMonitorFilter_TerminationPeriodFrom, DpContractTerminationMonitorFilter_TerminationPeriodTo;

$(function() {
    DpContractTerminationMonitorFilter_TerminationPeriodFrom = $('input[id$=\'dteTerminatedFrom_txtTextBox\']');
    DpContractTerminationMonitorFilter_TerminationPeriodTo = $('input[id$=\'dteTerminatedTo_txtTextBox\']');
});

function dteTerminatedFrom_Changed() {
    DpContractTerminationMonitorFilter_TerminationPeriodTo.datepicker("option", "minDate", DpContractTerminationMonitorFilter_TerminationPeriodFrom.val().toDate());
}

function DpContractTerminationMonitorFilter_BeforeNavigate() {
    var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
    url = AddQSParam(RemoveQSParam(url, DpContractTerminationMonitorFilter_QsFilterContractTypeKey), DpContractTerminationMonitorFilter_QsFilterContractTypeKey, getSelectedContractType().val());
    url = AddQSParam(RemoveQSParam(url, DpContractTerminationMonitorFilter_QsFilterUnderOrOverPaymentsKey), DpContractTerminationMonitorFilter_QsFilterUnderOrOverPaymentsKey, getSelectedUnderOverPayments().val());
    url = AddQSParam(RemoveQSParam(url, DpContractTerminationMonitorFilter_QsFilterIsBalancedKey), DpContractTerminationMonitorFilter_QsFilterIsBalancedKey, getSelectedBalanced().val());
    url = AddQSParam(RemoveQSParam(url, DpContractTerminationMonitorFilter_QsFilterTerminationDateFromKey), DpContractTerminationMonitorFilter_QsFilterTerminationDateFromKey, DpContractTerminationMonitorFilter_TerminationPeriodFrom.val());
    url = AddQSParam(RemoveQSParam(url, DpContractTerminationMonitorFilter_QsFilterTerminationDateToKey), DpContractTerminationMonitorFilter_QsFilterTerminationDateToKey, DpContractTerminationMonitorFilter_TerminationPeriodTo.val()); 
    SelectorWizard_newUrl = url;
    return true;
}

function getSelectedContractType() {
    return $('input[name$=\'rblContractTypes\']:checked');
}

function getSelectedUnderOverPayments() {
    return $('input[name$=\'rblUnderOrOverPayments\']:checked');
}

function getSelectedBalanced() {
    return $('input[name$=\'rblBalanced\']:checked');
}