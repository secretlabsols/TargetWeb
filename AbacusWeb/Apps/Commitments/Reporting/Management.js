var commitmentReportingManagementSettings;

$(function () {

    var commitmentReportingManagementControl = "Results.Controls.CommitmentReportingManagementResults";
    Ext.require(commitmentReportingManagementControl);
    Ext.require('Actions.Controls.CommitmentReportingManagementAction');
    Ext.require('Actions.Buttons.NewButton');

    Ext.onReady(function () {

        Ext.suspendLayouts();

        var commitmentReportingManagementPanel = Ext.create(commitmentReportingManagementControl, {
            resultSettings: commitmentReportingManagementSettings
        });

        Ext.resumeLayouts(true);

    });

});