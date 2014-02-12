var rsupResultsPanel, rsupResultSettings;

$(function() {

    var paymentResultsControl = 'Results.Controls.NonResidentialServiceUserPaymentResults';

    Ext.require(paymentResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var rsupResultsPanel = Ext.create(paymentResultsControl, {
            resultSettings: rsupResultSettings
        });

        //pass along browser window resize events to the panel
        Ext.EventManager.onWindowResize(rsupResultsPanel.doLayout, rsupResultsPanel);

        Ext.resumeLayouts(true);

    });

});