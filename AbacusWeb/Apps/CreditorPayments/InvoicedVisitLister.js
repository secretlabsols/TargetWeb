var ivResultsPanel, ivResultSettings;

$(function() {

    var invoicedVisitResultsControl = 'Results.Controls.InvoicedVisitResults';

    Ext.require(invoicedVisitResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var gsrResultsPanel = Ext.create(invoicedVisitResultsControl, {
            resultSettings: ivResultSettings
        });

        Ext.resumeLayouts(true);

    });

});