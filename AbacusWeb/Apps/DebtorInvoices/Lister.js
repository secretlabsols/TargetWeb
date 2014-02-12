var gsoResultsPanel, gsoResultSettings;

$(function() {

    var debtorInvoiceControl = 'Results.Controls.DebtorInvoiceResults';

    Ext.require(debtorInvoiceControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();
        
        var gsoResultsPanel = Ext.create(debtorInvoiceControl, {
            resultSettings: gsoResultSettings
        });

        Ext.resumeLayouts(true);
        
    });

});