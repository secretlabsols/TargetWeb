var cwtResultsPanel, cwtResultSettings;

$(function() {

    var clientExceptionWorkTrayResultsControl = 'Results.Controls.ClientExceptionWorkTrayResults';
    Ext.require(clientExceptionWorkTrayResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var dwtResultsPanel = Ext.create(clientExceptionWorkTrayResultsControl, {
            resultSettings: cwtResultSettings
        });

        Ext.resumeLayouts(true);

    });

});