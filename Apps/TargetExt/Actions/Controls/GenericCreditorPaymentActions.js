Ext.define('Actions.Controls.GenericCreditorPaymentActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.NewButton',
        'Actions.Buttons.ReportsButton'
    ],
    config: {
        authoriseButton: null,
        createBatchButton: null,
        suspensionsButton: null
    },
    constructor: function(config) {
        var me = this, cfg = me.config;
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        // create buttons
        cfg.authoriseButton = me.createActionButton(true, 'AuthoriseCreditorPayment', 'Authorise', 'Authorise Creditor Payments?', function() {
            me.fireEvent('onAuthoriseManyClicked', me); 
        });
        cfg.createBatchButton = me.createActionButton(true, 'CreateBatchImage', 'Create Batch', 'Create Batch of Creditor Payments?', function() {
            me.fireEvent('onCreateBatchClicked', me); 
        });
        cfg.suspensionsButton = me.createActionButton(true, 'SuspendCreditorPayment', 'Suspensions', 'Suspend Creditor Payments?', function() {
            me.fireEvent('onSuspensionsManyClicked', me); 
        });
        cfg.addButton = Ext.create('Actions.Buttons.NewButton', {
            autoRegisterEventsControls: [me]
        });
        cfg.addButton.setDisabled(!cfg.permissionCanAddNew);
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me],
            reportIds: me.resultSettings.ReportIdsForActionPanel
        });
        // add buttons
        me.items = [
            cfg.authoriseButton,
            cfg.createBatchButton,
            cfg.suspensionsButton,
            cfg.addButton,
            '->',
            cfg.reportsButton
        ];
        // init the parent
        me.callParent(arguments);
    }
});

