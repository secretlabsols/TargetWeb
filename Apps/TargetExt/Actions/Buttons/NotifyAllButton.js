Ext.define('Actions.Buttons.NotifyAllButton', {
    extend: 'Actions.ActionButton',
    config: {
        autoRegisterEventsControls: [],
        eventNameOnNotifyAllButtonClicked: 'onNotifyAllButtonClicked'
    },
    constructor: function(config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        // setup button
        me.iconCls = 'AddImage';
        me.handler = function() {
            me.raiseOnNotifyAllButtonClicked();
        };
        me.text = 'Notify All';
        me.tooltip = 'Creates a SDS Contribution Notifications Job';
        // init the parent
        me.callParent(arguments);
    },
    // advise observers that the button has been clicked
    raiseOnNotifyAllButtonClicked: function() {
        var me = this, cfg = me.config;
        me.raiseEventsToObservers(cfg.eventNameOnNotifyAllButtonClicked, cfg.autoRegisterEventsControls);
    }
});
