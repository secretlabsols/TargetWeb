Ext.define('TargetExt.Contracts.ContractNumberMappingConfigurationEditor', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Actions.Buttons.AuditLogButton'
    ],
    closeAction: 'hide',
    modal: true,
    resizable: false,
    title: 'Contract Number Mapping Configuration',
    width: 500,
    constrain: true,
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
            cfg.formSave
        ];
        me.items = [
            cfg.form
        ];
        me.callParent(arguments);
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
        var me = this, cfg = me.config, frm = cfg.form.getForm(), rcd = frm.getRecord(), rcdData,
        currentFactor, currentFactorIsEmpty, previousFactorIsEmpty = false, factorsValid = true;
        frm.updateRecord(rcd);
        rcdData = rcd.data;
        for (var i = 1; i <= 10; i++) {
            currentFactor = Ext.String.trim(rcdData['KeyFactor' + i]);
            currentFactorIsEmpty = Ext.isEmpty(currentFactor, false);
            if (i == 1 && currentFactorIsEmpty) {
                factorsValid = false;
                break;
            }
            if (previousFactorIsEmpty == true && currentFactorIsEmpty == false) {
                factorsValid = false;
                break;
            }
            previousFactorIsEmpty = currentFactorIsEmpty;
        }
        cfg.formSave.setDisabled(!(frm.isValid() && factorsValid));
    },
    setupForm: function () {
        var me = this, cfg = me.config, formFldsItems = [];
        if (!cfg.form) {
            // define the model for the form
            cfg.modelName = 'ContractNumberMappingConfigurationEditorItem';
            Ext.define(cfg.modelName, {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'Delimiter', mapping: 'Delimiter', type: 'string' },
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
            for (var i = 1; i <= 10; i++) {
                formFldsItems.push(
                    Ext.create('Ext.form.field.Text', {
                        name: 'KeyFactor' + i,
                        fieldLabel: 'Key Factor ' + i,
                        anchor: '100%',
                        maxLength: 50,
                        enforceMaxLength: true,
                        allowBlank: (i > 1),
                        plugins: ['clearbutton']
                    })
                )
            }
            cfg.formFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Key Factors',
                collapsible: false,
                items: formFldsItems,
                listeners: {
                    add: function (src, cmp) {
                        cmp.on('change', function () { me.resetFormVisibility(); });
                    }
                }
            });
            cfg.formFldsOtherDelimiter = Ext.create('Ext.form.field.Text', {
                name: 'Delimiter',
                fieldLabel: 'Delimiter',
                anchor: '100%',
                maxLength: 1,
                enforceMaxLength: true,
                maskRe: /[^0-9a-zA-Z\s\/]/,
                allowBlank: false,
                plugins: ['clearbutton']
            });
            cfg.formFldsOther = Ext.create('Ext.form.FieldSet', {
                title: 'Other',
                collapsible: false,
                items: [
                    cfg.formFldsOtherDelimiter
                ],
                listeners: {
                    add: function (src, cmp) {
                        cmp.on('change', function () { me.resetFormVisibility(); });
                    }
                }
            });
            // create the save button       
            cfg.formSave = Ext.create('Ext.Button', {
                text: 'Save',
                cls: 'Selectors',
                iconCls: 'imgSave',
                tooltip: 'Save?',
                handler: function () {
                    me.save();
                }
            });
            // create the audit button
            cfg.auditLogButton = Ext.create('Actions.Buttons.AuditLogButton', {});
            // create the form with all items
            cfg.form = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                    cfg.formFlds,
                    cfg.formFldsOther
                ]
            });
        }
    },
    save: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), updater = frm.getRecord();
        me.setLoading(true);
        frm.updateRecord(updater);
        updater = updater.data;
        cfg.service.SaveContractNumberMappingConfiguration(updater, function (response) {
            me.setLoading(false);
            if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                return false;
            }
            cfg.hasUpdated = true;
            me.close();
        }, me);
    },
    show: function (args) {
        var me = this, cfg = me.config, mdl;
        cfg.hasUpdated = false;
        me.setLoading(true);
        cfg.service.GetContractNumberMappingConfiguration(function (response) {
            me.setLoading(false);
            if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                return false;
            }
            cfg.currentItem = response.value.Item;
            mdl = Ext.create(cfg.modelName, response.value.Item);
            cfg.form.loadRecord(mdl);
            cfg.formFlds.items.items[0].focus(true, true);
            cfg.auditLogButton.setConfigSettings({
                auditLogID: cfg.currentItem.ID,
                auditLogTables: ['ContractNumberMappingConfiguration'],
                auditLogUseApplicationFilter: true,
                auditLogWebNavMenuItemID: 183
            });
            me.resetFormVisibility();
            cfg.form.getForm().clearInvalid();
        }, me);
        me.callParent();
        me.center();
    }
});