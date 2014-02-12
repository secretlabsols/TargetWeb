Ext.define('Actions.Controls.FinanceCodeMatrixActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.NewButton'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        // add can configure permission
        //cfg.permissionCanConfigure = me.resultSettings.HasPermission('CanConfigure');
        // add the add button
        cfg.addButton = Ext.create('Actions.Buttons.NewButton', {
            autoRegisterEventsControls: [me]
        });
        // add the configure button
        cfg.configureButton = me.createActionButton(true, 'CogEdit', 'Configuration', 'Configure?', function () {
            me.fireEvent('onConfigureButtonClicked', me);
        });
        // disable or enable the add button
        cfg.addButton.setDisabled(!cfg.permissionCanAddNew);
        // disable or enable the configure button
        cfg.configureButton.setDisabled(!cfg.permissionCanAddNew);

        me.items = [
            cfg.addButton,
            cfg.configureButton
        ];
        // init the parent
        me.callParent(arguments);
    }
}); 
