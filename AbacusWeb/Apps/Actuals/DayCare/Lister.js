var gsrResultsPanel, gsrResultSettings;

$(function() {

    var genericServiceRegisterResultsControl = 'Results.Controls.ServiceRegisterResults';

    Ext.require(genericServiceRegisterResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var gsrResultsPanel = Ext.create(genericServiceRegisterResultsControl, {
            resultSettings: gsrResultSettings
        });

        Ext.resumeLayouts(true);

    });

});