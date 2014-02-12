var RecalcActuals_dateToID, RecalcActuals_dateFromID;

function dteDateFrom_Changed(id) {
    var $dateFromCtrl = $('#' + id + '_txtTextBox');
    var $dateToCtrl = $('#' + RecalcActuals_dateToID + '_txtTextBox');
    $dateToCtrl.datepicker('option', 'minDate', $dateFromCtrl.val().toDate());
}

function dteDateTo_Changed(id) {
    var $dateToCtrl = $('#' + id + '_txtTextBox');
    var $dateFromCtrl = $('#' + RecalcActuals_dateFromID + '_txtTextBox');
    $dateFromCtrl.datepicker('option', 'maxDate', $dateToCtrl.val().toDate());
}