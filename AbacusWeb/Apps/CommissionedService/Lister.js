var gsoResultsPanel, gsoResultSettings;

$(function() {

    var genericServiceOrderResultsControl = 'Results.Controls.GenericServiceOrderResults';

    Ext.require(genericServiceOrderResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();
        
        var gsoResultsPanel = Ext.create(genericServiceOrderResultsControl, {
            resultSettings: gsoResultSettings
        });

        Ext.resumeLayouts(true);
        
    });

});