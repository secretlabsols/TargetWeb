Ext.define('Actions.Controls.ServiceUserPaymentActions', {
    extend: 'Actions.ActionControl',
    requires: [],
    initComponent: function() {
        var me = this, cfg = me.config;
        me.items = [];
        // init the parent
        me.callParent(arguments);
    }
});
