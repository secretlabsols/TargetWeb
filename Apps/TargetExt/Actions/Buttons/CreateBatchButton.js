Ext.define('Actions.Buttons.CreateBatchButton', {
    extend: 'Actions.ActionButton',
    config: {
        autoRegisterEventsControls: [],
        eventNameOnCreateBatchButtonClicked: 'onCreateBatchButtonClicked'
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
        me.iconCls = 'CreateBatchImage';
        me.handler = function() {
            me.raiseOnCreateBatchButtonClicked();
        };
        me.text = 'Create Batch';
        me.tooltip = 'Create a New Batch';
        // init the parent
        me.callParent(arguments);
    },
    // advise observers that the button has been clicked
    raiseOnCreateBatchButtonClicked: function() {
        var me = this, cfg = me.config;
        me.raiseEventsToObservers(cfg.eventNameOnCreateBatchButtonClicked, cfg.autoRegisterEventsControls);
    }
});