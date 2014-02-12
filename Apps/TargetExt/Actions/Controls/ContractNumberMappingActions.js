Ext.define('Actions.Controls.ContractNumberMappingActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.NewButton'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('ContractNumberMappings.AddNew');
        cfg.permissionCanConfigure = me.resultSettings.HasPermission('ContractNumberMappings.Configure');
        // add the add button
        cfg.addButton = Ext.create('Actions.Buttons.NewButton', {
            autoRegisterEventsControls: [me]
        });
        // disable or enable the add button
        cfg.addButton.setDisabled(!cfg.permissionCanAddNew);
        // add the add button
        cfg.configButton = me.createActionButton(true, 'CogEdit', 'Configuration', 'Configure?', function () {
            me.fireEvent('onConfigButtonClicked', me);
        });
        // disable or enable the add button
        cfg.configButton.setDisabled(!cfg.permissionCanConfigure);
        me.items = [
            cfg.addButton,
            cfg.configButton,
        ];
        // init the parent
        me.callParent(arguments);
    }
});


