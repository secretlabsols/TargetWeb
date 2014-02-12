Ext.define('InPlaceSelectors.Controls.ServiceUserInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Service User',
    getSelector: function() {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('ServiceUserSelector');
        }
        return me.config.searchSelector;
    }
});
