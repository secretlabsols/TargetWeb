Ext.define('Actions.Controls.DebtorInvoiceActions', {
    extend: 'Actions.ActionControl',
    requires: [
        'Actions.Buttons.ReportsButton'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add the create Batch button
        cfg.createBatch = Ext.create('Actions.Buttons.CreateBatchButton', {
            autoRegisterEventsControls: [me],
            disabled: true
        });
        //cfg.createBatch.disableCreateBatchButton(true);
        // add the reports button
        cfg.reportsButton = Ext.create('Actions.Buttons.ReportsButton', {
            autoRegisterEventsControls: [me],
            reportIds: me.resultSettings.ReportIdsForActionPanel
        });
        me.items = [
            cfg.createBatch,
            '->',
            cfg.reportsButton
        ];
        // init the parent
        me.callParent(arguments);
    },
    disableCreateBatchButton: function(disable) {
       var me = this, cfg = me.config;
       cfg.createBatch.setDisabled(disable);
    }
});

