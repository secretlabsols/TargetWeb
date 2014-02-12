var gcpResultsPanel, gcpResultSettings;

$(function() {

    var genericCreditorPaymentResultsControl = 'Results.Controls.GenericCreditorPaymentResults';

    Ext.require(genericCreditorPaymentResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();
        
        var gcpResultsPanel = Ext.create(genericCreditorPaymentResultsControl, {
            resultSettings: gcpResultSettings
        });

        Ext.resumeLayouts(true);
        
    });

});