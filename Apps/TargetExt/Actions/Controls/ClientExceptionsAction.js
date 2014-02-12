Ext.define('Actions.Controls.ClientExceptionsAction', {
    extend: 'Actions.ActionControl',
    config: {
        approveButton: null
    },
    constructor: function (config) {
        var me = this, cfg = me.config;
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config;
        // add can edit permission
        cfg.permissionEdit = me.resultSettings.HasPermission('CanEdit');
        // add the add button
        cfg.approveButton = me.createActionButton(true, 'CreateBatchImage', 'Edit All', 'Edit All Service User Exceptions?', function () {
            me.fireEvent('onEditAllExceptionsButtonClicked', me);
        });
        cfg.approveButton.setDisabled(!cfg.permissionEdit);

        me.items = [
            cfg.approveButton,
        ];
        // init the parent
        me.callParent(arguments);
    }
});



