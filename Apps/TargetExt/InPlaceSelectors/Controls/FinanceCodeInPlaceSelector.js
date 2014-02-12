Ext.define('InPlaceSelectors.Controls.FinanceCodeInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Finance Code',
    getSelector: function () {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('FinanceCodeSelector');
        }
        return me.config.searchSelector;
    }
});