Ext.define('Actions.Buttons.NewButton', {
    extend: 'Actions.ActionButton',
    config: {
        autoRegisterEventsControls: [],
        eventNameOnNewButtonClicked: 'onNewButtonClicked'
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
            me.raiseOnNewButtonClicked();
        };
        me.text = 'New';
        me.tooltip = 'Create a New Record';
        // init the parent
        me.callParent(arguments);
    },
    // advise observers that the button has been clicked
    raiseOnNewButtonClicked: function() {
        var me = this, cfg = me.config;
        me.raiseEventsToObservers(cfg.eventNameOnNewButtonClicked, cfg.autoRegisterEventsControls);
    }
});
