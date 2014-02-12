Ext.define('Actions.Controls.CommitmentReportingManagementAction', {
    extend: 'Actions.ActionControl',
    config: {
        addNewButton: null
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
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNewCommitmentReport');

        // add the add button
        cfg.addNewButton = Ext.create('Actions.Buttons.NewButton', {
            autoRegisterEventsControls: [me]
        });

        // disable or enable the add button
        cfg.addNewButton.setDisabled(!cfg.permissionCanAddNew);

        me.items = [
            cfg.addNewButton
        ];
        // init the parent
        me.callParent(arguments);
    }
});



