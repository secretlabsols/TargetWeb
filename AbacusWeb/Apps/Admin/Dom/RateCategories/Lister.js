var gsoResultsPanel, gsoResultSettings;

$(function() {

    var rateCategoriesResultsControl = 'Results.Controls.RateCategoryResults';

    Ext.require(rateCategoriesResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var gsoResultsPanel = Ext.create(rateCategoriesResultsControl, {
            resultSettings: gsoResultSettings
        });

        Ext.resumeLayouts(true);

    });

});