var rpResultsPanel, rpResultSettings;

$(function() {

    var residentialPaymentResultsControl = 'Results.Controls.ResidentialPaymentResults';

    Ext.require(residentialPaymentResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var gsrResultsPanel = Ext.create(residentialPaymentResultsControl, {
            resultSettings: rpResultSettings
        });

        Ext.resumeLayouts(true);

    });

});