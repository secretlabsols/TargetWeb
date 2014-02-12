var suResultsPanel, suResultSettings;

$(function() {

    var serviceUserResultsControl = 'Results.Controls.ServiceUserResults';

    Ext.require(serviceUserResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var suResultsPanel = Ext.create(serviceUserResultsControl, {
            resultSettings: suResultSettings
        });

        Ext.resumeLayouts(true);

    });

});