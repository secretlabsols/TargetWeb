Ext.define('InPlaceSelectors.Controls.GenericCreditorInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Creditor',
    getSelector: function () {
        var me = this;
        if (!me.config.searchSelector) {
            me.config.searchSelector = me.getSelectorControlInstance('GenericCreditorSelector');
        }
        return me.config.searchSelector;
    },
    linkToInPlaceContractSelector: function (contractIpSelector, id) {
        var me = this, cfg = me.config;
        if (id) {
            contractIpSelector.getSelector().SetCreditorID(id);
        }
        me.addListener('itemSelected', function (isevt, isargs) {
            var selector = contractIpSelector.getSelector(), contractsCurrentItem = (contractIpSelector.getValue().Item || { CreditorID: 0 });
            if (isargs.ID != contractsCurrentItem.CreditorID) {
                contractIpSelector.clearValue();
                contractIpSelector.setReloadOnNextShow();
            }
            selector.SetCreditorID(isargs.ID);
        }, me);
    }
});