Ext.define('InPlaceSelectors.Controls.RateFrameworkInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Rate Framework',
    getSelector: function() {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('RateFrameworkSelector');
        }
        return me.config.searchSelector;
    },
    linkToInPlaceContractSelector: function(ipselector, id) {
        var me = this, cfg = me.config;
        if (id) {
            ipselector.getSelector().SetRateFrameworkID(id);
        }
        me.addListener('itemSelected', function(isevt, isargs) {
            var selector = ipselector.getSelector();
            ipselector.clearValue();
            ipselector.setReloadOnNextShow();
            selector.SetRateFrameworkID(isargs.ID);
        }, me);
    }
});