Ext.define('Actions.Controls.VerificationTextActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.NewButton'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        // add the add button
        cfg.addButton = Ext.create('Actions.Buttons.NewButton', {
            autoRegisterEventsControls: [me]
        });
        // disable or enable the add button
        cfg.addButton.setDisabled(!cfg.permissionCanAddNew);

        me.items = [
            cfg.addButton
        ];
        // init the parent
        me.callParent(arguments);
    }
});

