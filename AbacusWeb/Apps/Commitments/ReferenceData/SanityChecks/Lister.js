var scResultsPanel, scResultSettings;

$(function() {

    var sanityCheckResultsControl = 'Results.Controls.SanityCheckResults';
    Ext.require(sanityCheckResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var scResultsPanel = Ext.create(sanityCheckResultsControl, {
            resultSettings: scResultSettings
        });

        Ext.resumeLayouts(true);

    });

});