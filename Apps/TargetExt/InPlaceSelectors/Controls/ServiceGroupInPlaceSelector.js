Ext.define('InPlaceSelectors.Controls.ServiceGroupInPlaceSelector', {
    extend: 'InPlaceSelectors.InPlaceSelectorControl',
    fieldLabel: 'Service Group',
    getSelector: function() {
        var me = this;
        if (!me.config.searchSelector) {            
            me.config.searchSelector = me.getSelectorControlInstance('ServiceGroupSelector');
        }
        return me.config.searchSelector;
    }    
});