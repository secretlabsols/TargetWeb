Ext.define('Searchers.SearcherControl', {
    extend: 'Ext.panel.Panel',
    requires: ['Target.ux.form.field.ClearButton'],
    width: 422,
    config: {
        clearButton: null,
        lastSearch: null,
        searchButton: null,
        resultSettings: null
    },
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.selectionsService = Target.Abacus.Library.SavedSelections.Services.SavedSelectionsService;
        // create saved selections items
        cfg.savedSelectionsStore = Ext.create('Ext.data.Store', {
            fields: ['ID', 'GlobalSelection', 'Name', 'QueryString', 'WebNavMenuItemID'],
            data: [],
            listeners: {
                datachanged: {
                    fn: function (cmp, rcds) {
                        if (cfg.savedSelectionsCmb) {
                            cfg.savedSelectionsCmb.setReadOnly((cmp.count() <= 0));
                        }
                    },
                    scope: me
                }
            }
        });
        cfg.savedSelectionsCmb = Ext.create('Ext.form.ComboBox', {
            store: cfg.savedSelectionsStore,
            queryMode: 'local',
            displayField: 'Name',
            fieldLabel: 'Saved Selections:',
            valueField: 'ID',
            name: 'SavedSelections',
            readonly: true,
            editable: false,
            flex: 1,
            padding: '0 0 0 5',
            plugins: ['clearbutton'],
            listeners: {
                change: {
                    fn: function (cmp, id) {
                        me.handleSavedSelectionChanged((id || -1));
                    },
                    scope: me
                }
            },
            groupingFunction: function (item) {
                return (item.GlobalSelection == -1 ? 'Global' : 'Personal');
            }
        });
        cfg.addSavedSelectionButton = Ext.create('Ext.Button', {
            cls: 'Searchers',
            iconCls: 'AddSelectionImage',
            tooltip: 'Add the Current Search Criteria as a Saved Selection?',
            handler: function () {
                me.handleAddSelection();
            }
        });
        cfg.saveSavedSelectionButton = Ext.create('Ext.Button', {
            cls: 'Searchers',
            disabled: true,
            iconCls: 'SaveSelectionImage',
            tooltip: 'Save the Current Search Criteria to this Saved Selection?',
            handler: function () {
                me.handleSaveSelection();
            }
        });
        cfg.deleteSavedSelectionButton = Ext.create('Ext.Button', {
            cls: 'Searchers',
            disabled: true,
            iconCls: 'DeleteSelectionImage',
            tooltip: 'Delete this Saved Selection?',
            handler: function () {
                me.handleDeleteSelection();
            }
        });
        // create search button that calls search function
        cfg.searchButton = Ext.create('Ext.Button', {
            iconCls: 'FindImage',
            cls: 'Searchers',
            text: 'Search',
            tooltip: 'Search?',
            handler: function () {
                me.search();
            }
        });
        // create reset button that calls reset search function
        cfg.clearButton = Ext.create('Ext.Button', {
            iconCls: 'FindClearImage',
            cls: 'Searchers',
            text: 'Reset',
            tooltip: 'Reset Search?',
            handler: function () {
                me.handleReset();
            }
        });
        // add toolbars
        me.dockedItems = [
            {
                xtype: 'toolbar',
                dock: 'top',
                items: [
                    cfg.savedSelectionsCmb,
                    cfg.addSavedSelectionButton,
                    cfg.saveSavedSelectionButton,
                    cfg.deleteSavedSelectionButton
                ]
            },
            {
                xtype: 'toolbar',
                dock: 'top',
                items: [
                    '->',
                    cfg.searchButton,
                    cfg.clearButton
                ]
            }
        ];
        // If this is an Extranet Application, remove the saved selections control
        if (me.isExtranet()) {
            var removedItem = me.dockedItems.reverse().pop();
        }
        // setup defaults for searcher control
        me.animCollapse = true;
        me.cls = 'Searchers';
        me.collapsed = me.resultSettings.SearcherSettings.Collapsed;
        me.collapsible = true;
        me.defaults = {
            autoScroll: true,
            bodyPadding: 5,
            cls: 'Searchers'
        };
        if (!me.layout) me.layout = 'accordion';
        me.margins = '5 0 5 5';
        me.maxWidth = 500;
        me.minWidth = 275;
        me.region = 'west';
        me.split = true;
        me.title = 'Search Criteria';
        me.titleCollapse = true;
        // call parent component
        me.callParent(arguments);
    },
    blockInput: function (block) {
        this.config.searchButton.setDisabled(block);
    },
    createSavedSelectionsWindow: function () {
        var me = this, cfg = me.config;
        if (!cfg.saveSelectionsWin) {
            cfg.saveSelectionsWin = Ext.create('TargetExt.Searchers.SearcherSavedSelectionEditor', {
                canMaintainGlobals: cfg.canMaintainGlobalSavedSelection,
                listeners: {
                    onDeleted: {
                        fn: function (cmp, item) {
                            me.handleSavedSelectionDeleted(item);
                        },
                        scope: me
                    },
                    onSaved: {
                        fn: function (cmp, item) {
                            me.handleSavedSelectionSaved(item);
                        },
                        scope: me
                    }
                }
            });
        }
    },
    getModelPropertyAsArray: function (mdl, prop) {
        return mdl.data[prop].split(',');
    },
    getModelPropertyAsArrayOfInt: function (mdl, prop) {
        var arr = this.getModelPropertyAsArray(mdl, prop);
        for (var i = 0; i < arr.length; i++) {
            arr[i] = parseInt(arr[i]);
        }
        return arr;
    },
    getSearch: function () {
        alert('getSearch not implemented.');
        return null;
    },
    getSelectedSavedSelection: function () {
        var me = this, cfg = me.config, id = parseInt(cfg.savedSelectionsCmb.getValue());
        return me.getSelectedSavedSelectionByID(id);
    },
    getSelectedSavedSelectionByID: function (id) {
        var me = this, cfg = me.config, savedSelection;
        id = parseInt((id || 0));
        if (id > 0) {
            savedSelection = cfg.savedSelectionsStore.findRecord('ID', id);
        }
        return savedSelection;
    },
    handleAddSelection: function () {
        var me = this, cfg = me.config, savedSelection;
        me.createSavedSelectionsWindow();
        savedSelection = {
            ID: 0,
            GlobalSelection: 0,
            Name: '',
            QueryString: Ext.JSON.encode(me.getSearch()),
            WebNavMenuItemID: cfg.resultSettings.WebNavMenuItemID
        };
        cfg.saveSelectionsWin.show(savedSelection);
    },
    handleDeleteSelection: function () {
        var me = this, cfg = me.config, savedSelection = me.getSelectedSavedSelection().data;
        Ext.getBody().mask('Deleting...');
        cfg.selectionsService.DeleteSavedSelection(savedSelection.ID, me.handleDeleteSelectionCallBack, me);
    },
    handleDeleteSelectionCallBack: function (response) {
        var me = response.context, cfg = me.config, savedSelection = me.getSelectedSavedSelection().data;
        Ext.getBody().unmask();
        if (!CheckAjaxResponse(response, cfg.selectionsService.url, true)) {
            return false;
        }
        me.handleSavedSelectionDeleted(savedSelection);
    },
    handleReset: function () {
        var me = this, cfg = me.config;
        cfg.savedSelectionsCmb.setValue();
        me.handleSavedSelectionChanged(0);
        me.resetSearch();
    },
    handleSaveSelection: function () {
        var me = this, cfg = me.config, savedSelection = me.getSelectedSavedSelection().data;
        me.createSavedSelectionsWindow();
        savedSelection.QueryString = Ext.JSON.encode(me.getSearch());
        cfg.saveSelectionsWin.show(savedSelection);
    },
    handleSavedSelectionChanged: function (id) {
        var me = this, cfg = me.config, savedSelection = me.getSelectedSavedSelection(id), canSave = false, canDelete = false, canGlobal = false, search;
        if (savedSelection) {
            canGlobal = (savedSelection.data.GlobalSelection == 0 || (savedSelection.data.GlobalSelection == -1 && cfg.canMaintainGlobalSavedSelection));
            canSave = (canGlobal && cfg.canEditSavedSelection);
            canDelete = (canGlobal && cfg.canDeleteSavedSelection);
            search = Ext.JSON.decode(savedSelection.data.QueryString);
            me.setSearch(search);
            if (me.setSearchFromSavedSelections) {
                me.setSearchFromSavedSelections(search);
            }
        } else if (id < 0) {
            me.resetSearch();
        }
        cfg.saveSavedSelectionButton.setDisabled(!canSave);
        cfg.deleteSavedSelectionButton.setDisabled(!canDelete);
    },
    handleSavedSelectionDeleted: function (item) {
        var me = this, cfg = me.config, deletedSelection;
        deletedSelection = me.getSelectedSavedSelectionByID(item.ID);
        if (deletedSelection) {
            cfg.savedSelectionsStore.remove([deletedSelection]);
        }
        cfg.savedSelectionsCmb.setValue();
    },
    handleSavedSelectionSaved: function (item) {
        var me = this, cfg = me.config, savedSelection;
        savedSelection = me.getSelectedSavedSelectionByID(item.ID);
        if (savedSelection) {
            savedSelection.set(item);
        } else {
            cfg.savedSelectionsStore.loadData([item], true);
        }
        me.sortSavedSelections();
        cfg.savedSelectionsCmb.setValue(item.ID);
    },
    isExtranet: function () {
        var me = this;
        return me.resultSettings.ApplicationID == 3;
    },
    raiseOnSearch: function (search) {
        var me = this, cfg = me.config;
        if (!search) {
            search = me.getSearch();
        }
        me.fireEvent('onSearch', me, search);
    },
    raiseOnSearchChanged: function (search) {
        var me = this, cfg = me.config;
        if (!search) {
            search = me.getSearch();
        }
        me.fireEvent('onSearchChanged', me, search);
    },
    resetSearch: function () {
        alert('resetSearch not implemented.');
        return null;
    },
    search: function () {
        var me = this, cfg = me.config, changed = false;
        me.blockInput(true);
        me.raiseOnSearch();
    },
    setModelPropertyAsArray: function (mdl, prop) {
        var me = this;
        mdl.data[prop] = me.getModelPropertyAsArray(mdl, prop);
    },
    setModelPropertyAsArrayOfInt: function (mdl, prop) {
        var me = this;
        mdl.data[prop] = me.getModelPropertyAsArrayOfInt(mdl, prop);
    },
    setSavedSelections: function () {
        var me = this, cfg = me.config;
        cfg.canAddSavedSelection = me.resultSettings.HasPermission('SavedSelections.Add');
        cfg.canDeleteSavedSelection = me.resultSettings.HasPermission('SavedSelections.Delete');
        cfg.canEditSavedSelection = me.resultSettings.HasPermission('SavedSelections.Edit');
        cfg.canMaintainGlobalSavedSelection = me.resultSettings.HasPermission('SavedSelections.Globals');
        cfg.addSavedSelectionButton.setDisabled(!cfg.canAddSavedSelection);
        cfg.deleteSavedSelectionButton.setDisabled(!cfg.canDeleteSavedSelection);
        cfg.saveSavedSelectionButton.setDisabled(!cfg.canEditSavedSelection);
        cfg.savedSelectionsStore.loadData(cfg.resultSettings.SavedSelections || []);
        me.sortSavedSelections();
        cfg.savedSelectionsCmb.setValue();
        me.handleSavedSelectionChanged();
    },
    sortSavedSelections: function () {
        var me = this, cfg = me.config;
        cfg.savedSelectionsStore.sort([
            {
                property: 'GlobalSelection',
                direction: 'ASC'
            },
            {
                property: 'Name',
                direction: 'ASC'
            }
        ]);
    }
});
