Ext.define('InPlaceSelectors.Controls.SpendPlanInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Spend Plan',
    getSelector: function() {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('SpendPlanSelector');
        }
        return me.config.searchSelector;
    }
});