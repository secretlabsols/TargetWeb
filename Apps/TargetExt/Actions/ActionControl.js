Ext.define('Actions.ActionControl', {
    extend: 'Ext.toolbar.Toolbar',
    defaults: {
        cls: 'ActionPanels'
    },
    createActionButton: function(disabled, iconCls, text, tooltip, callBackFunc) {
        var me = this;
        return Ext.create('Ext.Button', {
            cls: 'ActionPanels',
            disabled: disabled,
            iconCls: iconCls,
            text: text,
            tooltip: tooltip,
            handler: function() {
                callBackFunc();
            }
        });
    }
});
