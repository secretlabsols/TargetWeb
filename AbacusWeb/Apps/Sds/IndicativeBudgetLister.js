var ibResultsPanel, ibResultSettings;

$(function() {

    var indicativeBudgetResultsControl = 'Results.Controls.IndicativeBudgetResults';

    Ext.require(indicativeBudgetResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();
        
        var ibResultsPanel = Ext.create(indicativeBudgetResultsControl, {
            resultSettings: ibResultSettings
        });

        Ext.resumeLayouts(true);
        
    });

});