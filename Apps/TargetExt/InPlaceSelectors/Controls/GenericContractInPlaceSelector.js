Ext.define('InPlaceSelectors.Controls.GenericContractInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Contract',
    getSelector: function() {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('GenericContractSelector');
        }
        return me.config.searchSelector;
    },
    linkToInPlaceServiceUserSelector: function (serviceUserIpSelector, id) {
        var me = this, cfg = me.config;
        if (id) {
            serviceUserIpSelector.getSelector().SetContractID(id);
        }
        me.addListener('itemSelected', function (isevt, isargs) {
            var selector = serviceUserIpSelector.getSelector(), ServiceUserCurrentItem = (serviceUserIpSelector.getValue().Item || { ContractID: 0 });
            if (isargs.ID != ServiceUserCurrentItem.ContractID) {
                serviceUserIpSelector.clearValue();
                serviceUserIpSelector.setReloadOnNextShow();
            }
            selector.SetContractID(isargs.ID);
            if (!isargs.ID) {
                serviceUserIpSelector.clearValue();
                serviceUserIpSelector.setReloadOnNextShow();
            }
        }, me);
    }
});