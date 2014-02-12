Ext.define('Actions.Controls.SDSContributionMonitorActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.NotifyAllButton',
        'Actions.Buttons.ReportsButton'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // 'defer making changes to contribution levels until a notification letter has been produced' setting value
        cfg.deferChangesToContributionLevels = me.resultSettings.HasPermission('DeferChangesToContributionLevels');
        // add the notify button
        cfg.notifyAllButton = Ext.create('Actions.Buttons.NotifyAllButton', {
            autoRegisterEventsControls: [me]
        });
        // set the visibility on the notify button
        cfg.notifyAllButton.setVisible(cfg.deferChangesToContributionLevels);
        // add the reports button
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me],  // enables the results control to listen for this event
            reportIds: me.resultSettings.ReportIdsForActionPanel 
        });
        me.items = [
            '->',
            cfg.notifyAllButton,
            cfg.reportsButton
        ];
        // init the parent
        me.callParent(arguments);
    },
    setNotifyAllButtonDisabled: function(disabled) {
        var me = this, cfg = me.config;
        cfg.notifyAllButton.setDisabled(disabled);
    }
});