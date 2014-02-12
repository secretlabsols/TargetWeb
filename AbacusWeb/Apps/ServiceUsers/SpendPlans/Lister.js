var spResultsPanel, spResultSettings;

$(function() {

    var spendPlanResultsControl = 'Results.Controls.SpendPlanResults';

    Ext.require(spendPlanResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var spResultsPanel = Ext.create(spendPlanResultsControl, {
            resultSettings: spResultSettings
        });

        Ext.resumeLayouts(true);

    });

});