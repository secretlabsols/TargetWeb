var mrResultsPanel, mrResultSettings;

$(function() {

    var movementRequestResultsControl = 'Results.Controls.MovementRequestResults';

    Ext.require(movementRequestResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();
        
        var mrResultsPanel = Ext.create(movementRequestResultsControl, {
            resultSettings: mrResultSettings
        });

        Ext.resumeLayouts(true);
        
    });

});