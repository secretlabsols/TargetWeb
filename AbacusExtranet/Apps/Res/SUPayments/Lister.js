var rpResultsPanel, rpResultSettings;

$(function() {

    var ServiceUserPaymentResultsControl = 'Results.Controls.ServiceUserPaymentResults';

    Ext.require(ServiceUserPaymentResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var gsrResultsPanel = Ext.create(ServiceUserPaymentResultsControl, {
            resultSettings: rpResultSettings
        });

        Ext.resumeLayouts(true);

    });

});