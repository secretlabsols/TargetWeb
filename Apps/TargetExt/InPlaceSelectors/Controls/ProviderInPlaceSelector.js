Ext.define('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Provider',
    getSelector: function () {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('ProviderSelector');
        }
        return me.config.searchSelector;
    },
    linkToInPlaceContractSelector: function (contractIpSelector, id) {
        var me = this, cfg = me.config;
        if (id) {
            contractIpSelector.getSelector().SetProviderID(id);
        }
        me.addListener('itemSelected', function (isevt, isargs) {
            var selector = contractIpSelector.getSelector(), contractsCurrentItem = (contractIpSelector.getValue().Item || { ProviderID: 0 });
            if (isargs.ID != contractsCurrentItem.ProviderID) {
                contractIpSelector.clearValue();
                contractIpSelector.setReloadOnNextShow();
            }
            selector.SetProviderID(isargs.ID);
            if (!isargs.ID) {
                contractIpSelector.clearValue();
                contractIpSelector.setReloadOnNextShow();
            }
        }, me);
    },
    linkToInPlaceServiceUserSelector: function (serviceUserIpSelector, id) {
        var me = this, cfg = me.config;
        if (id) {
            serviceUserIpSelector.getSelector().SetProviderID(id);
        }
        me.addListener('itemSelected', function (isevt, isargs) {
            var selector = serviceUserIpSelector.getSelector(), ServiceUserCurrentItem = (serviceUserIpSelector.getValue().Item || { ProviderID: 0 });
            if (isargs.ID != ServiceUserCurrentItem.ProviderID) {
                serviceUserIpSelector.clearValue();
                serviceUserIpSelector.setReloadOnNextShow();
            }
            selector.SetProviderID(isargs.ID);
            if (!isargs.ID) {
                serviceUserIpSelector.clearValue();
                serviceUserIpSelector.setReloadOnNextShow();
            }
        }, me);
    }
});