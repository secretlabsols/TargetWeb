var rpResultsPanel, rpResultSettings;

$(function() {

    var residentialOccupancyResultsControl = 'Results.Controls.ResidentialOccupancyResults';

    Ext.require(residentialOccupancyResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var gsrResultsPanel = Ext.create(residentialOccupancyResultsControl, {
            resultSettings: rpResultSettings
        });

        Ext.resumeLayouts(true);

    });

});