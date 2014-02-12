Ext.define('Actions.Controls.ServiceOrderSuspensionActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.NewButton',
        'Actions.Buttons.ReportsButton'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        // add the add button
        cfg.addButton = Ext.create('Actions.Buttons.NewButton', {
            autoRegisterEventsControls: [me]
        });
        // disable or enable the add button
        cfg.addButton.setDisabled(!cfg.permissionCanAddNew);
        // add the reports button
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me],
            reportIds: me.resultSettings.ReportIdsForActionPanel
        });
        me.items = [
            cfg.addButton,
            '->',
            cfg.reportsButton
        ];
        // init the parent
        me.callParent(arguments);
    }
});

