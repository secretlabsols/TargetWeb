Ext.define('Actions.Controls.CareHomePaymentAction', {
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
    initComponent: function() {
        var me = this, cfg = me.config;
        // add the add button
        cfg.approveButton = me.createActionButton(true, 'CreateBatchImage', 'Approve', 'Approve all Care Home Payments?', function () {
            me.fireEvent('onApproveCareHomePaymentsButtonClicked', me);
        });
        me.items = [
            cfg.approveButton,
        ];
        // init the parent
        me.callParent(arguments);
    }
});


