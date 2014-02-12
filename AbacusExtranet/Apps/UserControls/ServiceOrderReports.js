/// <reference path="http://localhost:82/TargetWeb/Library/JavaScript/jQuery/UI/jquery-ui/js/jquery-1.5.1-vsdoc.js" />


var reportButtonID;


$(document).ready(function () {

    var $dtp = $("[id$='detailWeekEndingDate_txtTextBox']");
    $dtp.change(function () {
        ReportsButton_AddParam(reportButtonID + "_btnReports", "WED", formatDate($dtp.datepicker('getDate')));
    });

    $("[id$='chkDonotfilter']").change(function () {
        if ($(this).is(':checked')) {
            ReportsButton_AddParam(reportButtonID + "_btnReports", "Filter", false);
        } else {
            ReportsButton_AddParam(reportButtonID + "_btnReports", "Filter", true);
        }
    });

});


function formatDate(dtDate) {
    if (Date.strftime("%d/%m/%Y", dtDate) == '31/12/9999') {
        return '(open ended)'
    } else {
        return Date.strftime("%d/%m/%Y", dtDate)
    }
}
