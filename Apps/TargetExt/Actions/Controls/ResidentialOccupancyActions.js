Ext.define('Actions.Controls.ResidentialOccupancyActions', {
    extend: 'Actions.ActionControl',
    requires: [],
    initComponent: function() {
        var me = this, cfg = me.config;
        me.items = [];
        // init the parent
        me.callParent(arguments);
    }
});
