Ext.define('InPlaceSelectors.Controls.RateCategoryInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Rate Category',
    getSelector: function() {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('RateCategorySelector');
        }
        return me.config.searchSelector;
    }
});