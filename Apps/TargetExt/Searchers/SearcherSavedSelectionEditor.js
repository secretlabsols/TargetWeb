Ext.define('TargetExt.Searchers.SearcherSavedSelectionEditor', {
    extend: 'Ext.window.Window',
    closeAction: 'hide',
    modal: true,
    modelName: 'SearcherSavedSelectionEditorItem',
    title: 'Save Saved Selections',
    width: 340,
    resizable: false,
    config: {
        canMaintainGlobals: false
    },
    constructor: function(config) {
        var me = this;
        me.initConfig(config);
        me.callParent([config]);
        return me;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.SavedSelections.Services.SavedSelectionsService;
        // setup the form
        me.setupForm();
        // create and load model
        Ext.define(me.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'ID', mapping: 'ID' },
                { name: 'IsGlobal', mapping: 'IsGlobal' },
                { name: 'GlobalSelection', mapping: 'GlobalSelection' },
                { name: 'Name', mapping: 'Name' },
                { name: 'QueryString', mapping: 'QueryString' },
                { name: 'WebNavMenuItemID', mapping: 'WebNavMenuItemID' }
            ],
            idProperty: 'ID'
        });
        // add buttons bar
        me.bbar = [
            '->',
            cfg.formSave
        ];
        // add any items
        me.items = [
            cfg.form
        ];
        // call parent component
        me.callParent(arguments);
    },
    getRecord: function() {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), rcd = frm.getRecord();
        frm.updateRecord(rcd);
        return rcd;
    },
    setupForm: function() {
        var me = this, cfg = me.config;
        if (!cfg.form) {
            // create the form and items
            cfg.formNameTb = Ext.create('Ext.form.field.Text', {
                allowBlank: false,
                anchor: '100%',
                emptyText: 'Please enter a name...',
                fieldLabel: 'Name',
                maxLength: 50,
                name: 'Name'
            });
            cfg.formIsGlobal = Ext.create('Ext.form.field.Checkbox', {
                disabled: !cfg.canMaintainGlobals,
                fieldLabel: 'Is Global?',
                maxLength: 50,
                name: 'IsGlobal'
            });
            cfg.formSave = Ext.create('Ext.Button', {
                text: 'Save',
                cls: 'Selectors',
                iconCls: 'imgSave',
                tooltip: 'Save?',
                handler: function() {
                    me.saveSavedSelection();
                }
            });
            cfg.form = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                    cfg.formNameTb,
                    cfg.formIsGlobal
                ],
                listeners: {
                    fieldvaliditychange: {
                        fn: function(cmp) {
                            cfg.formSave.setDisabled(!cmp.getForm().isValid());
                        },
                        scope: me
                    }
                }
            });
        }
    },
    saveSavedSelection: function() {
        var me = this, cfg = me.config, rcd = me.getRecord().data;
        me.setLoading('Saving...');
        cfg.service.SaveSavedSelection(rcd.ID, rcd.Name, rcd.WebNavMenuItemID, rcd.IsGlobal, rcd.QueryString, me.saveSavedSelectionCallBack, me);
    },
    saveSavedSelectionCallBack: function(response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        me.fireEvent('onSaved', me, response.value.Item);
        me.close();
    },
    show: function(item) {
        var me = this, cfg = me.config, mdl;
        me.callParent();
        item.IsGlobal = (item.GlobalSelection == -1 ? true : false);
        mdl = Ext.create(me.modelName, item);
        cfg.form.loadRecord(mdl);
    }
});