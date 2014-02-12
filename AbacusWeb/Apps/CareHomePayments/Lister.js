var chResultsPanel, chResultSettings;

$(function() {

    var careHomePaymentsResultsControl = 'Results.Controls.CareHomePaymentResults';
    Ext.require(careHomePaymentsResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var chResultsPanel = Ext.create(careHomePaymentsResultsControl, {
            resultSettings: chResultSettings
        });

        Ext.resumeLayouts(true);

    });

});