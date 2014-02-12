Ext.define('InPlaceSelectors.Controls.DebtorInvoiceInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Debtor Invoice',
    getSelector: function () {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('DebtorInvoiceSelector');
        }
        return me.config.searchSelector;
    }
});