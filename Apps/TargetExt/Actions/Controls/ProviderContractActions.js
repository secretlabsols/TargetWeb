Ext.define('Actions.Controls.ProviderContractActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.ReportsButton',
        'Actions.Buttons.NewButton'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        // add the add button
        cfg.newButton = Ext.create('Actions.Buttons.NewButton', {
            autoRegisterEventsControls: [me]
        });
        cfg.newButton.setDisabled(!cfg.permissionCanAddNew);
        // add the reports button
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me],
            reportIds: me.resultSettings.ReportIdsForActionPanel
        });
        me.items = [
            cfg.newButton,
            '->',
            cfg.reportsButton
        ];
        // init the parent
        me.callParent(arguments);
    }
});
