Ext.define('TargetExt.Contracts.ContractNumberMappingEditor', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton',
        'InPlaceSelectors.Controls.GenericContractInPlaceSelector',
        'Actions.Buttons.AuditLogButton'
    ],
    closeAction: 'hide',
    constrain: true,
    modal: true,
    resizable: false,
    title: 'Contract Number Mapping',
    width: 700,
    config: {
        currentItem: null,
        hasUpdated: false
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.Contracts.Services.ContractsService;
        me.setupForm();
        me.bbar = [
            cfg.auditLogButton,
            { xtype: 'tbfill' },
            cfg.formSave,
            cfg.formDelete
        ];
        me.items = [
            cfg.form
        ];
        me.callParent(arguments);
    },
    deleteItem: function () {
        var me = this, cfg = me.config, item = me.getItem();
        Ext.MessageBox.confirm('Confirm', 'Are you sure you wish to delete this record?', function (answer) {
            if (answer == 'yes') {
                me.setLoading(true);
                cfg.service.DeleteContractNumberMapping(item.ID, function (response) {
                    me.setLoading(false);
                    if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                        return false;
                    }
                    cfg.currentItem = null;
                    cfg.hasUpdated = true;
                    me.close();
                }, me);
            }
        });
    },
    getItem: function () {
        var me = this, cfg = me.config;
        return cfg.currentItem;
    },
    getHasUpdated: function () {
        var me = this, cfg = me.config;
        return cfg.hasUpdated;
    },
    resetFormVisibility: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), isValid = frm.isValid();
        cfg.formDelete.setDisabled(cfg.currentItem.ID <= 0);
        cfg.formSave.setDisabled(!isValid);
        cfg.auditLogButton.setVisible(cfg.currentItem.ID > 0);
    },
    setupForm: function () {
        var me = this, cfg = me.config, formFldsItems = [], labelWidth = 200;
        if (!cfg.form) {
            // define the model for the form
            cfg.modelName = 'ContractNumberMappingEditorItem';
            Ext.define(cfg.modelName, {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'GenericContract', mapping: 'GenericContract' },
                    { name: 'KeyFactor1', mapping: 'KeyFactor1', type: 'string' },
                    { name: 'KeyFactor2', mapping: 'KeyFactor2', type: 'string' },
                    { name: 'KeyFactor3', mapping: 'KeyFactor3', type: 'string' },
                    { name: 'KeyFactor4', mapping: 'KeyFactor4', type: 'string' },
                    { name: 'KeyFactor5', mapping: 'KeyFactor5', type: 'string' },
                    { name: 'KeyFactor6', mapping: 'KeyFactor6', type: 'string' },
                    { name: 'KeyFactor7', mapping: 'KeyFactor7', type: 'string' },
                    { name: 'KeyFactor8', mapping: 'KeyFactor8', type: 'string' },
                    { name: 'KeyFactor9', mapping: 'KeyFactor9', type: 'string' },
                    { name: 'KeyFactor10', mapping: 'KeyFactor10', type: 'string' }
                ],
                idProperty: 'ID'
            });
            // create the save button       
            cfg.formSave = Ext.create('Ext.Button', {
                text: 'Save',
                cls: 'Selectors',
                iconCls: 'imgSave',
                tooltip: 'Save?',
                handler: function () {
                    me.saveItem();
                }
            });
            // create the delete button       
            cfg.formDelete = Ext.create('Ext.Button', {
                text: 'Delete',
                cls: 'Selectors',
                iconCls: 'imgDelete',
                tooltip: 'Delete?',
                handler: function () {
                    me.deleteItem();
                }
            });
            // create the audit button
            cfg.auditLogButton = Ext.create('Actions.Buttons.AuditLogButton', {});
            // create the factor controls based on whats in the db
            cfg.service.GetContractNumberMappingConfiguration(function (response) {
                if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                    return false;
                }
                var mappingCfg = response.value.Item, currentKeyFactor, currentKeyFactorValue;
                cfg.frmContractSelector = Ext.create('InPlaceSelectors.Controls.GenericContractInPlaceSelector', {
                    fieldLabel: 'Contract',
                    labelWidth: labelWidth,
                    name: 'GenericContract',
                    searchTextBoxConfig: {
                        allowBlank: false
                    }
                });
                cfg.frmContractSelector.getSelector().SetTypes([Selectors.GenericContractSelectorTypes.NonResidential]);
                formFldsItems.push(cfg.frmContractSelector);
                for (var i = 1; i <= 10; i++) {
                    currentKeyFactor = 'KeyFactor' + i;
                    currentKeyFactorValue = Ext.String.trim(mappingCfg[currentKeyFactor]);
                    if (!Ext.isEmpty(currentKeyFactorValue, false)) {
                        ;
                        formFldsItems.push(
                            Ext.create('Ext.form.field.Text', {
                                name: currentKeyFactor,
                                fieldLabel: currentKeyFactorValue,
                                anchor: '100%',
                                maxLength: 50,
                                enforceMaxLength: true,
                                allowBlank: false,
                                labelWidth: labelWidth,
                                plugins: ['clearbutton']
                            })
                        )
                    }
                }
                cfg.formFlds = Ext.create('Ext.form.FieldSet', {
                    title: 'Criteria',
                    collapsible: false,
                    items: formFldsItems,
                    listeners: {
                        add: function (src, cmp) {
                            cmp.on('change', function () { me.resetFormVisibility(); });
                        }
                    }
                });
                // create the form with all items
                cfg.form = Ext.create('Ext.form.Panel', {
                    border: 0,
                    bodyPadding: 5,
                    items: [
                        cfg.formFlds
                    ]
                });
                me.add(cfg.form);
            }, me);
        }
    },
    saveItem: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), updater = frm.getRecord();
        me.setLoading(true);
        frm.updateRecord(updater);
        updater = updater.data;
        updater.GenericContractID = updater.GenericContract.ID;
        cfg.service.SaveContractNumberMapping(updater, function (response) {
            me.setLoading(false);
            if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                return false;
            }
            cfg.hasUpdated = true;
            me.close();
        }, me);
    },
    show: function (args) {
        var me = this, cfg = me.config, task;
        cfg.hasUpdated = false;
        me.setLoading(true);
        cfg.auditLogButton.setVisible(false);
        // due to the dynamic generation of the form we need to load the item once the 
        // form has been created which is dependent on calling a web service...wait until that has done first and then load
        task = Ext.TaskManager.start({
            run: function () {
                if (cfg.form) {
                    cfg.currentItem = (args.item || { ID: 0, ContractID: 0 });
                    mdl = Ext.create(cfg.modelName, cfg.currentItem);
                    cfg.form.loadRecord(mdl);
                    cfg.frmContractSelector.setValueFromWebService(cfg.currentItem.ContractID);
                    cfg.auditLogButton.setConfigSettings({
                        auditLogID: cfg.currentItem.ID,
                        auditLogTables: ['ContractNumberMapping'],
                        auditLogUseApplicationFilter: true,
                        auditLogWebNavMenuItemID: 183
                    });
                    me.resetFormVisibility();
                    me.center();
                    Ext.TaskManager.stop(task);
                    me.setLoading(false);
                    cfg.form.getForm().clearInvalid();
                }
            },
            interval: 250
        });
        me.callParent();
        me.center();
    }
});