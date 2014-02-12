Ext.define('Actions.ActionButton', {
    extend: 'Ext.button.Button',
    constructor: function(config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        // setup button
        me.cls = 'ActionPanels';
        // init the parent
        me.callParent(arguments);
    },
    // advise observers that an item has been clicked
    raiseEventsToObservers: function(evtName, evtCtrls, evtArgs) {
        var me = this, cfg = me.config;
        me.fireEvent(evtName, me, evtArgs);
        if (evtCtrls) {
            Ext.Array.each(evtCtrls, function(ctrl, ctrlIdx) {
                ctrl.fireEvent(evtName, me, evtArgs);
            });
        }
    }
});
