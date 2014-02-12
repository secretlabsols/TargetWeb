var SDSContributionMonitorResultsPanel, SDSContributionMonitorResultSettings;

$(function() {

    var SDSContributionMonitorResultsControl = 'Results.Controls.SDSContributionMonitorResults';

    Ext.require(SDSContributionMonitorResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var SDSContributionMonitorResultsPanel = Ext.create(SDSContributionMonitorResultsControl, {
            resultSettings: SDSContributionMonitorResultSettings
        });

        Ext.resumeLayouts(true);

    });

});