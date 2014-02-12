var dwtResultsPanel, dwtResultSettings;

$(function() {

    var deceasedWorkTrayResultsControl = 'Results.Controls.DeceasedWorkTrayResults';
    Ext.require(deceasedWorkTrayResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var dwtResultsPanel = Ext.create(deceasedWorkTrayResultsControl, {
            resultSettings: dwtResultSettings
        });

        Ext.resumeLayouts(true);

    });

});