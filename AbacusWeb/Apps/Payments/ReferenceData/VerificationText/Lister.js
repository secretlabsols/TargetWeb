var vtResultsPanel, vtResultSettings;

$(function() {

    var verificationTextResultsControl = 'Results.Controls.VerificationTextResults';
    Ext.require(verificationTextResultsControl)

    Ext.onReady(function() {

        Ext.suspendLayouts();

        var vtResultsPanel = Ext.create(verificationTextResultsControl, {
            resultSettings: vtResultSettings
        });

        Ext.resumeLayouts(true);

    });

});