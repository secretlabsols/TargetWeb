Ext.define('Actions.Buttons.FindWithSelectorButton', {
    extend: 'Actions.ActionButton',
    iconCls: 'FindWithSelectorImage',
    text: 'Find',
    tooltip: 'Find a Record',
    config: {
        autoRegisterEventsControls: [],
        eventNameOnFindWithSelectorItemSelected: 'onFindWithSelectorItemSelected',
        isInited: false,
        selectorControl: null,
        selectorReloadOnNextShow: false,
        selectorWindow: null,
        selectorWindowAutoHide: true,
        selectorWindowLoaded: false
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
        me.handler = function() {
            me.showSelectorWindow();
        };
        // init the parent
        me.callParent(arguments);
    },
    initControls: function() {
        var me = this, cfg = me.config;
        if (!cfg.isInited) {
            // setup window object to display the selector control in
            cfg.selectorWindow = Ext.create('Ext.window.Window', {
                border: 0,
                closeAction: 'hide',
                constrain: true,
                height: 375,
                layout: {
                    type: 'fit',
                    align: 'stretch'
                },
                width: ($(document).width() - 100),
                resizable: false,
                modal: true,
                listeners: {
                    show: {
                        fn: function() {
                            var thisObj = this;
                            if (!cfg.selectorWindowLoaded) {
                                // if not loaded selector window then do so
                                cfg.selectorControl = cfg.selectorControl;
                                // observe the item selected event and hide the window
                                cfg.selectorControl.OnItemSelected(function(item) {
                                    // advise observers that item has been selected
                                    thisObj.raiseFindWithSelectorItemSelected(item);
                                    if (cfg.selectorWindowAutoHide) {
                                        // hide if configured to do so
                                        thisObj.hideSelectorWindow();
                                    }
                                });
                                cfg.selectorControl.Load(cfg.selectorWindow);
                            } else {
                                cfg.selectorControl.SetSelectedItem(0, false);
                                if (cfg.selectorReloadOnNextShow) {
                                    cfg.selectorReloadOnNextShow = false;
                                    cfg.selectorControl.Load();
                                }
                            }
                            cfg.selectorWindowLoaded = true;
                        },
                        scope: me
                    }
                }
            });
            cfg.isInited = true;
        }
    },
    // get the selector control to use with this control, must be overriden
    getSelector: function() {
        Ext.Msg.alert('getSelector Required', 'Please specify the getSelector function.');
        return null;
    },
    // hide the selector window
    hideSelectorWindow: function() {
        var me = this, cfg = me.config;
        cfg.selectorWindow.hide();
    },
    // advise observers that an item has been selected
    raiseFindWithSelectorItemSelected: function(item) {
        var me = this, cfg = me.config;
        me.raiseEventsToObservers(cfg.eventNameOnFindWithSelectorItemSelected, cfg.autoRegisterEventsControls, item);
    },
    setReloadOnNextShow: function(reload) {
        var me = this, cfg = me.config;
        cfg.selectorReloadOnNextShow = (reload || true);
    },
    // show the selector control in a window
    showSelectorWindow: function() {
        var me = this, cfg = me.config, selector = (cfg.selectorControl || me.getSelector());
        // validate the selector control exists
        if (!selector) {
            Ext.Msg.alert('Selector Control Required', 'Please specify the Selector Control.');
            return false;
        }
        // init the controls ie window etc
        me.initControls();
        // set the selector control
        cfg.selectorControl = selector;
        // show the window
        cfg.selectorWindow.show();
    }
});

Ext.define('Actions.Buttons.FindWithSelectorButtonMenuItem', {
    extend: 'Ext.menu.Item',
    cls: 'ActionPanels',
    iconCls: 'FindWithSelectorImage',
    text: 'Find',
    tooltip: 'Find a Record?',
    requires: [
        'Actions.Buttons.FindWithSelectorButton'
    ],
    config: {
        findButton: null,
        findButtonConfig: null
    },
    constructor: function(config) {
        var me = this, cfg = me.config;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        me.handler = function() {
            me.initControls();
            cfg.findButton.showSelectorWindow();
        };
        this.callParent(arguments);
    },
    initControls: function() {
        var me = this, cfg = me.config;
        if (!cfg.findButton) {
            cfg.findButtonConfig.renderTo = Ext.getBody();
            cfg.findButtonConfig.hidden = true;
            cfg.findButton = Ext.create('Actions.Buttons.FindWithSelectorButton', cfg.findButtonConfig);
        }
    },
    setReloadOnNextShow: function(reload) {
        var me = this, cfg = me.config;
        me.initControls();
        cfg.findButton.setReloadOnNextShow(reload);
    }
});