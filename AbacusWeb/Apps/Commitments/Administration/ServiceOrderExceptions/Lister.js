var soeResultsPanel, soeResultSettings;

$(function() {

    var serviceOrderExceptionsResultsControl = 'Results.Controls.ServiceOrderExceptionsResults';
    Ext.require(serviceOrderExceptionsResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var soeResultsPanel = Ext.create(serviceOrderExceptionsResultsControl, {
            resultSettings: soeResultSettings
        });

        Ext.resumeLayouts(true);

    });

});