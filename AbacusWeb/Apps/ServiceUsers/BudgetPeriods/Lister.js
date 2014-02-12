var suResultsPanel, suResultSettings;

$(function() {

    var budgetPeriodsResultsControl = 'Results.Controls.BudgetPeriodResults';
    Ext.require(budgetPeriodsResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var suResultsPanel = Ext.create(budgetPeriodsResultsControl, {
            resultSettings: suResultSettings
        });

        Ext.resumeLayouts(true);

    });

});