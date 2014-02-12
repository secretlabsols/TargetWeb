Ext.define('InPlaceSelectors.InPlaceSelectorControl', {
    extend: 'Ext.form.FieldContainer',
    requires: ['Ext.window.Window'],
    anchor: '100%',
    labelWidth: 100,
    config: {
        firstSearchID: -1,
        isInited: false,
        searchButton: null,
        searchID: 0,
        searchMenu: null,
        searchSelector: null,
        searchSelectorReload: false,
        searchTextBox: null,
        searchTextBoxCleared: false,
        searchValue: null,
        searchWindow: null,
        searchWindowLoaded: false,
        searchWindowSelectedItem: null,
        searchWindowSelectedItemIgnoreFirst: false,
        service: null
    },
    constructor: function (config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function () {
        var me = this, cfg = me.config, cfgSearchTextBox = cfg.searchTextBoxConfig || {},
        menuItems = [
            {
                cls: 'InPlaceSelector',
                handler: function () {
                    me.clearSelection();
                },
                iconCls: 'BtnClear',
                scope: me,
                text: 'Clear'
            },
            {
                cls: 'InPlaceSelector',
                handler: function () {
                    me.makeSelection();
                },
                iconCls: 'BtnSelect',
                scope: me,
                text: 'Select'
            }
        ], additionalMenuItems = (me.getAdditionalMenuItems ? me.getAdditionalMenuItems() : []);
        if (additionalMenuItems && additionalMenuItems.length > 0) {
            menuItems = menuItems.concat('-', additionalMenuItems);
        }
        cfg.searchMenu = new Ext.menu.Menu({
            items: menuItems
        });
        cfg.searchButton = Ext.create('Ext.button.Split', {
            handler: function () {
                me.makeSelection();
            },
            iconCls: 'BtnSelect',
            menu: cfg.searchMenu,
            menuAlign: 'tr-br',
            scope: me
        })
        cfg.searchTextBox = Ext.create('Ext.form.field.Text',
            Ext.apply(
                {
                    allowBlank: false,
                    emptyText: 'Select an item...',
                    flex: 1,
                    margin: '0 1 0 0',
                    listeners: {
                        click: {
                            element: 'el',
                            fn: function () {
                                if (!cfg.searchTextBoxCleared) {
                                    me.makeSelection();
                                }
                            },
                            scope: me
                        },
                        change: {
                            fn: function (cmp, newVal) {
                                if (!newVal) {
                                    cfg.searchTextBoxCleared = true;
                                    me.clearValue();
                                    setTimeout(function () { cfg.searchTextBoxCleared = false; }, 10);
                                }
                            },
                            scope: me
                        }
                    },
                    readOnly: true,
                    plugins: [new Target.ux.form.field.ClearButton({ hideClearButtonReadOnly: false })]
                },
                cfgSearchTextBox
            )
        );
        me.items = [
            cfg.searchTextBox,
            cfg.searchButton
        ];
        me.cls = 'InPlaceSelector';
        me.layout = 'hbox';
        // add custom events
        me.addEvents('itemSelected');
        me.on('boxready', function () {
            Ext.EventManager.addListener(me.labelCell.dom, 'click', function () {
                me.makeSelection();
            });
        });
        // init parent
        me.callParent(arguments);
    },
    initControls: function () {
        var me = this, cfg = me.config;
        if (!cfg.isInited) {
            cfg.searchWindow = Ext.create('Ext.window.Window', {
                border: 0,
                closeAction: 'hide',
                constrain: true,
                height: 425,
                layout: 'anchor',
                width: ($(document).width() - 100),
                resizable: false,
                modal: true,
                listeners: {
                    show: {
                        fn: function () {
                            var thisObj = this;
                            if (!cfg.searchWindowLoaded) {
                                cfg.searchSelector = this.getSelector();
                                cfg.searchSelector.OnItemSelected(function (item) {
                                    if (!thisObj.config.searchWindowSelectedItemIgnoreFirst) {
                                        if (item) {
                                            if (thisObj.config.searchID != item.ID) {
                                                thisObj.setValue({
                                                    ID: item.ID,
                                                    Item: item,
                                                    Value: item.Reference
                                                }, true);
                                                thisObj.config.searchSelector.SaveMrus(true);
                                                setTimeout(function () { thisObj.config.searchWindow.close(); }, 10);
                                            }
                                        } else {
                                            thisObj.setValue({
                                                ID: 0,
                                                Item: null,
                                                Value: ''
                                            }, true);
                                        }
                                    }
                                    thisObj.config.searchWindowSelectedItemIgnoreFirst = false;
                                });
                                cfg.searchSelector.OnLoaded(function (item) {
                                    me.autoSelectIfApplicable();
                                });
                                if (cfg.searchID && cfg.searchID > 0) {
                                    cfg.searchWindowSelectedItemIgnoreFirst = true;
                                    cfg.searchSelector.SetRequestSelectedID(cfg.searchID);
                                }
                                cfg.searchSelector.Load(cfg.searchWindow, false);
                            } else {
                                if (cfg.searchSelectorReload) {
                                    cfg.searchSelectorReload = false;
                                    cfg.searchSelector.Load();
                                }
                            }
                            cfg.searchWindowLoaded = true;
                        },
                        scope: me
                    }
                }
            });
            cfg.isInited = true;
        }
    },
    fieldLabel: 'fieldLabel',
    width: 300,
    clearSelection: function () {
        this.clearValue();
    },
    makeSelection: function () {
        this.initControls();
        this.config.searchWindow.show();
    },
    clearValue: function () {
        var me = this;
        if (me.config.searchSelector) {
            me.config.searchSelector.ClearSelectedItem();
            me.config.searchSelector.ClearFilters();
        }
        me.setValue({
            ID: 0,
            Item: null,
            Value: ''
        }, true);
    },
    getValue: function () {
        var me = this;
        var val = {
            ID: me.config.searchID,
            Item: me.config.searchWindowSelectedItem,
            Value: me.config.searchValue
        };
        return val;
    },
    getModelData: function () {
        var me = this, data = null, val;
        val = me.getValue();
        if (val !== null) {
            data = {};
            data[me.getName()] = val;
        }
        return data;
    },
    setReloadOnNextShow: function () {
        this.config.searchSelectorReload = true;
    },
    setValue: function (args, raiseEvt) {
        var me = this, cfg = me.config, clearBtn = cfg.searchMenu.items.items[0];
        args = $.extend(true, {
            ID: 0,
            Item: null,
            Value: ''
        }, args);
        cfg.searchID = args.ID;
        cfg.searchValue = args.Value;
        cfg.searchWindowSelectedItem = args.Item;
        if (!cfg.searchWindowSelectedItem || !cfg.searchWindowSelectedItem.ID || cfg.searchWindowSelectedItem.ID == 0) {
            cfg.searchTextBox.setValue(cfg.searchValue);
            if (cfg.searchSelector) {
                cfg.searchSelector.SetSelectedItem(0, false);
            }
        } else {
            cfg.searchTextBox.setValue(cfg.searchWindowSelectedItem.InPlaceSelectorValue);
        }
        if (cfg.firstSearchID == -1) {
            cfg.firstSearchID = cfg.searchID;
        }
        if (raiseEvt) {
            me.itemSelected();
        }
        clearBtn.setDisabled((cfg.searchID <= 0));
    },
    setValueFromWebService: function (id, force) {
        var me = this, cfg = me.config, svc = cfg.service, sel = me.getSelector(), item;
        if (me.getValue().ID !== id || force === true) {
            if (!svc) {
                svc = new Target.Abacus.Library.Selectors.Services.SelectorService_class();
            }
            me.setDisabled(true);
            svc.GetSelectorItem(id, sel.GetType(), function (response) {
                me.setDisabled(false);
                if (!CheckAjaxResponse(response, svc.url, true)) {
                    return false;
                }
                item = response.value.Item;
                me.setValue({
                    ID: item.ID,
                    Item: item,
                    Value: item.InPlaceSelectorValue
                }, true);
            });
        }
    },
    resetValueFromWebService: function () {
        var me = this, cfg = me.config, val = me.getValue();
        if (val.ID > 0) {
            me.setValueFromWebService(val.ID, true);
        }
    },
    getName: function () {
        return this.name;
    },
    isDirty: function () {
        return (this.config.firstSearchID != this.config.searchID);
    },
    isFormField: true,
    isValid: function () {
        var me = this, cfg = me.config;
        return cfg.searchTextBox.isValid();
    },
    itemSelected: function () {
        var me = this, cfg = me.config;
        me.fireEvent('itemSelected', me, {
            ID: cfg.searchID,
            Item: cfg.searchWindowSelectedItem,
            Value: cfg.searchValue
        });
        me.fireEvent('change', me);
    },
    validate: function () {
        var me = this, cfg = me.config;
        return cfg.searchTextBox.validate();
    },
    getSelectorControlInstance: function (className, args) {
        return Selectors.Helpers.GetSelectorControlInstance(className, args);
    },
    autoSelectIfApplicable: function () {
        var me = this, cfg = me.config, items = cfg.searchSelector.GetItems();
        if (items.length == 1) {
            cfg.searchSelector.SetSelectedItem(items[0].ID);
        }
    },
    clearInvalid: function () {
        var me = this, cfg = me.config;
        cfg.searchTextBox.clearInvalid();
    }
});