Ext.define('InPlaceSelectors.Controls.BudgetHolderInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Budget Holder',
    getSelector: function() {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('BudgetHolderSelector');
        }
        return me.config.searchSelector;
    }
});