var pcResultsPanel, pcResultSettings;

$(function() {

    var genericProvidercontractResultsControl = 'Results.Controls.ProviderContractResults';

    Ext.require(genericProvidercontractResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var gsrResultsPanel = Ext.create(genericProvidercontractResultsControl, {
            resultSettings: pcResultSettings
        });

        Ext.resumeLayouts(true);

    });

});