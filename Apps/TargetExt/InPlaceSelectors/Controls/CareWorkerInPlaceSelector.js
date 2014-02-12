Ext.define('InPlaceSelectors.Controls.CareWorkerInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Care Worker',
    getSelector: function() {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('CareWorkerSelector');
        }
        return me.config.searchSelector;
    }
});