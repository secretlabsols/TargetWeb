Ext.define('Target.ux.form.field.MultiSelectEditor', {
    extend: 'Ext.form.FieldContainer',
    requires: ['Ext.ux.form.MultiSelect'],
    fieldLabel: 'fieldLabel',
    constructor: function(config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        // setup globals
        cfg.eventOnAddClicked = 'onAddClicked';
        cfg.modelName = 'MultiSelectEditorItem';
        cfg.modelIdField = 'ID';
        cfg.modelValueField = 'Value';
        // create a model to represent items in the editor
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: cfg.modelIdField, type: 'string' },
                { name: cfg.modelValueField, type: 'string' }
            ],
            idProperty: cfg.modelIdField
        });
        // create the store
        cfg.store = Ext.create('Ext.data.Store',
            Ext.apply(
                {
                    model: cfg.modelName,
                    data: [],
                    listeners: {
                        // reset button vis after clear of store
                        clear: function() {
                            me.resetButtonVisibility();
                        },
                        // reset button vis after data changed in store
                        datachanged: function() {
                            me.resetButtonVisibility();
                        },
                        scope: me
                    }
                },
                // apply any conifg settings passed in for store
                (cfg.cfgStore || {})
            )
        );
        // create the editor control
        cfg.editor = Ext.create('Ext.ux.form.MultiSelect',
            Ext.apply(
                {
                    displayField: cfg.modelValueField,
                    flex: 1,
                    height: 105,
                    listeners: {
                        boundList: {
                            // on double click delete items
                            itemdblclick: function() {
                                me.deleteSelectedItems();
                            },
                            // on selection chnage reset btn visibility
                            selectionchange: function() {
                                me.resetButtonVisibility();
                            },
                            scope: me
                        }
                    },
                    maxHeight: 150,
                    queryMode: 'local',
                    store: cfg.store,
                    valueField: cfg.modelIdField
                },
                // apply any conifg settings passed in for editor
                (cfg.cfgEditor || {})
            )
        );
        // create an add button
        cfg.btnAdd = Ext.create('Ext.Button', {
            handler: function() {
                me.raiseOnAddClicked();
            },
            iconCls: 'imgArrowLeft',
            tooltip: 'Add an Item'
        });
        // create a clear selected button
        cfg.btnClearSelected = Ext.create('Ext.Button', {
            handler: function() {
                me.deleteSelectedItems();
            },
            iconCls: 'imgArrowRight',
            tooltip: 'Clear Selected Items'
        });
        // create a clear all button
        cfg.btnClearAll = Ext.create('Ext.Button', {
            handler: function() {
                me.deleteAllItems();
            },
            iconCls: 'imgArrowRightAll',
            tooltip: 'Clear All Items'
        });
        // create an undo button
        cfg.btnUndo = Ext.create('Ext.Button', {
            handler: function() {
                me.undoChanges();
            },
            iconCls: 'imgArrowUndo',
            tooltip: 'Undo All Changes'
        });
        // create a panel to store the buttons vertically in
        cfg.btnPanel = Ext.create('Ext.panel.Panel', {
            border: 0,
            // set defaults for all buttons(items)
            defaults: {
                cls: 'MultiSelectEditor',
                margin: '0 0 2 0'
            },
            height: 100,
            // add all buttons
            items: [
		        cfg.btnAdd,
		        cfg.btnClearSelected,
		        cfg.btnClearAll,
                cfg.btnUndo
	        ],
            layout: 'vbox',
            padding: '0 0 0 5'
        });
        // add items into control
        me.items = [
	        cfg.editor,
	        cfg.btnPanel
        ];
        // layout should be horizontal
        me.layout = 'hbox';
        // listen to disable evts
        me.addListener('disable', function(cmp) {
            me.resetButtonVisibility();
        }, me);
        // listen to enable evts
        me.addListener('enable', function(cmp) {
            me.resetButtonVisibility();
        }, me);
        // reset button visibility
        me.resetButtonVisibility();
        // init parent
        me.callParent(arguments);
    },
    // adds an item into the list
    addItem: function(args) {
        var me = this, cfg = me.config, store = cfg.store;
        // ensure properties on args
        args = Ext.applyIf(args, {
            Data: null,
            SuspendSort: false
        });
        if (args.Data) {
            var dtItmId = args.Data[cfg.lastLoadValueField], dtItmIdVal = args.Data[cfg.lastLoadDisplayField], mdl = store.getById(dtItmId);
            if (!mdl) {
                // if not found a model then create one
                mdl = Ext.create(cfg.modelName, {
                    ID: dtItmId,
                    Value: dtItmIdVal
                });
                // load into the store
                store.loadData([mdl], true);
            } else {
                // else found already so update
                mdl.data.ID = dtItmId;
                mdl.data.Value = dtItmIdVal;
                // commit so val is updated in editor
                mdl.commit();
            }
            if (!args.SuspendSort) { me.sort(); }
        }
    },
    // clear all items from the list
    deleteAllItems: function() {
        var me = this, cfg = me.config, store = cfg.store;
        store.removeAll();
    },
    // clear all selected items from the list
    deleteSelectedItems: function() {
        var me = this, cfg = me.config, selectedItm, selectedItms = cfg.editor.getValue(), store = cfg.store;
        // if we have any selected items
        if (selectedItms) {
            // loop em 
            Ext.Array.each(selectedItms, function(itmVal, itmIdx) {
                selectedItm = store.getById(itmVal);
                if (selectedItm) { store.remove(selectedItm); }
            });
        }
    },
    // get all the items
    getAllItems: function() {
        var me = this, cfg = me.config, store = cfg.store, itms = [];
        // loop em 
        Ext.Array.each(store.data.items, function(itmVal, itmIdx) {
            itms.push({
                ID: itmVal.data[cfg.lastLoadValueField],
                Value: itmVal.data[cfg.lastLoadDisplayField]
            });
        });
        return itms;
    },
    // get the selected items
    getSelectedItems: function() {
        var me = this, cfg = me.config, selectedItm, selectedItms = cfg.editor.getValue(), store = cfg.store, itms = [];
        // if we have any selected items
        if (selectedItms) {
            // loop em 
            Ext.Array.each(selectedItms, function(itmVal, itmIdx) {
                selectedItm = store.getById(itmVal);
                if (selectedItm) {
                    itms.push({
                        ID: selectedItm.data[cfg.lastLoadValueField],
                        Value: selectedItm.data[cfg.lastLoadDisplayField]
                    });
                }
            });
        }
        return itms;
    },
    // load items into the list
    load: function(args) {
        var me = this, cfg = me.config, store = cfg.store;
        // ensure properties on args
        args = Ext.applyIf(args, {
            Data: [],
            DisplayField: cfg.modelValueField,
            ValueField: cfg.modelIdField
        });
        // store some data for use later
        cfg.lastLoadDisplayField = args.DisplayField;
        cfg.lastLoadValueField = args.ValueField;
        // remove all existing items
        me.deleteAllItems();
        // add each item into editor
        Ext.Array.each(args.Data, function(dtItm, dtIdx) {
            me.addItem({ Data: dtItm, SuspendSort: true });
        });
        // sort by value field
        me.sort();
        // store values for undo
        cfg.lastLoadItems = [];
        // loop each items and add a copy to an array
        Ext.Array.each(store.data.items, function(dtItm, dtIdx) {
            cfg.lastLoadItems.push(Ext.applyIf({}, dtItm.data));
        });
        me.resetButtonVisibility();
    },
    // raise on clicked event i.e. instruct listeners to display ui
    raiseOnAddClicked: function() {
        var me = this, cfg = me.config;
        me.fireEvent(cfg.eventOnAddClicked);
    },
    // reset the button visibility based on list items etc
    resetButtonVisibility: function() {
        var me = this, cfg = me.config, store = cfg.store, selectedItems = [];
        if (!cfg.editor) {
            return false;
        }
        selectedItems = me.getSelectedItems();
        cfg.btnAdd.setDisabled(!(!me.disabled));
        cfg.btnClearSelected.setDisabled(!(!me.disabled && selectedItems && selectedItems.length > 0));
        cfg.btnClearAll.setDisabled(!(!me.disabled && store && store.count() > 0));
        cfg.btnUndo.setDisabled(!(!me.disabled && cfg.lastLoadItems));
    },
    // sort the items in the list
    sort: function() {
        var me = this, cfg = me.config, store = cfg.store;
        // sort the store
        store.sort(cfg.modelValueField, 'ASC');
    },
    // undo any changes since the last call to load
    undoChanges: function() {
        var me = this, cfg = me.config, args;
        // ensure properties on args
        args = {
            Data: cfg.lastLoadItems,
            DisplayField: cfg.lastLoadDisplayField,
            ValueField: cfg.lastLoadValueField
        }
        me.load(args);
    }
});
