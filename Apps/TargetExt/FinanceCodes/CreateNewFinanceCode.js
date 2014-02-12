Ext.define('TargetExt.FinanceCodes.CreateNewFinanceCode', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Actions.Buttons.AuditLogButton'
    ],
    title: 'Create a new Finance Code',
    closeAction: 'hide',
    width: 500,
    height: 180,
    layout: 'fit',
    resizable: false,
    modal: true,
    config: {
        financeCodeMaxLength: 35
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config;

        cfg.modelName = 'NewFinanceCode'

        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'ID', mapping: 'ID' },
                { name: 'Code', mapping: 'Code' },
                { name: 'Description', mapping: 'Description' },
                { name: 'Category', mapping: 'Category', type: 'int' }
            ],
            idProperty: 'ID'
        });

        var data = { FinanceCode: '', Description: ''};
        var mdl = Ext.create(cfg.modelName, data);
        me.setupFrm();
        cfg.financeCodeForm.loadRecord(mdl);

        me.callParent(arguments);

    },
    setupFrm: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.financeCodeForm) {

            cfg.service = Target.Abacus.Library.FinanceCodes.Services.FinanceCodeService;

            cfg.FinanceCode = Ext.create('Ext.form.field.Text', {
                name: 'Code',
                fieldLabel: 'Finance Code',
                width:300,
                enforceMaxLength: true,
                maxLength: cfg.financeCodeMaxLength,
                plugins: ['clearbutton']
            });

            cfg.Description = Ext.create('Ext.form.field.Text', {
                name: 'Description',
                fieldLabel: 'Description',
                width:470,
                enforceMaxLength: true,
                maxLength: 50,
                plugins: ['clearbutton']
            });

            cfg.financeCodeType = Ext.create('Ext.form.FieldSet', {
                defaultType: 'radiofield',
                width: '50%',
                border:'none',
                defaults: {
                    flex: 1
                },
                layout: 'hbox',

                items: [
                    {
                        boxLabel: 'Income',
                        name: 'Category',
                        inputValue: 2,
                        disabled: true,
                    }, {
                        boxLabel: 'Expenditure',
                        name: 'Category',
                        inputValue: 1,
                        checked:true
                    }
                ]
            });


            cfg.financeCodeForm = Ext.create('Ext.form.Panel', {
                layout: {
                    type: 'vbox',
                    align: 'left'
                },
                border: false,
                width:'100%',
                bodyPadding: 10,
                defaults: {
                    flex: 1
                },
                items: [
                        cfg.FinanceCode,
                        cfg.Description,
                        cfg.financeCodeType
                   ],
                buttons: [{
                    text: 'Cancel',
                    handler: function () {
                        this.up('window').hide();
                    }
                }, {
                    text: 'Save',
                    handler: function () {
                        var me = this, cfg = me.config;
                        if (this.up('form').getForm().isValid()) {
                            this.up('window').saveFinanceCode();
                        }
                    }
                }]
            });
        }


        me.items = [cfg.financeCodeForm];


    },
    show: function (item) {
        var me = this, cfg = me.config;
        me.callParent(arguments);

    },
    getRecord: function () {
        var me = this, cfg = me.config, frm = cfg.financeCodeForm.getForm(), updater = frm.getRecord();
        frm.updateRecord(updater);
        return updater.data;
    },
    saveFinanceCode: function () {
        var me = this, cfg = me.config;
        var record = me.getRecord();
        cfg.service.SaveNewFinanceCode(record, me.saveNewFinanceCodeCallBack, me);
    },
    saveNewFinanceCodeCallBack: function (response) {
        var me = response.context, cfg = me.config;
        var record = me.getRecord();
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        me.fireEvent('onChanged', me, response.value.Item);
        me.hide();
    }
});