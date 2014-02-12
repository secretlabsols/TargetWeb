var sosResultsPanel, sosResultSettings;

$(function() {

    var serviceOrderSuspensionResultsControl = 'Results.Controls.ServiceOrderSuspensionResults';

    Ext.require(serviceOrderSuspensionResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();
        
        var sosResultsPanel = Ext.create(serviceOrderSuspensionResultsControl, {
            resultSettings: sosResultSettings
        });

        Ext.resumeLayouts(true);
        
    });

});