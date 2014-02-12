Ext.define('InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'User',
    getAdditionalMenuItems: function () {
        var me = this, cfg = me.config, items = [];
        if ((cfg.webSecurityUserID || 0) > 0) {
            items.push({
                cls: 'Selectors',
                handler: function () {
                    me.setAsMe();
                },
                iconCls: 'imgMe',
                scope: me,
                text: 'Me',
                tooltip: 'Set to Me?'
            });
        }
        return items;
    },
    getSelector: function () {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('WebSecurityUserSelector');
        }
        return me.config.searchSelector;
    },
    setAsMe: function () {
        var me = this, cfg = me.config;
        me.setValueFromWebService(cfg.webSecurityUserID);
    }
});