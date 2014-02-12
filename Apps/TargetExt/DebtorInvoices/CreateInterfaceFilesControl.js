Ext.define('TargetExt.DebtorInvoices.CreateInterfaceFilesControl', {
    extend: 'Ext.panel.Panel',
    border: false,
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTime'
    ],
    initComponent: function () {
        var me = this, cfg = me.config, controls = [];
        Ext.define('DebtorInvoiceBatchItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'CreateDate', mapping: 'CreateDate', type: 'date' },
                { name: 'PostingDateFrom', mapping: 'PostingDateFrom' },
                { name: 'PostingYear', mapping: 'PostingYear' },
                { name: 'PeriodNumber', mapping: 'PeriodNumber' },
                { name: 'Rollback', mapping: 'Rollback' }
            ],
            idProperty: 'ID'
        });
        this.setupFrmCreateInterface();

        var todayDate = new Date();
        var data = { CreateDate: todayDate, PostingDateFrom: '', PostingYear: '', PeriodNumber: '', Rollback: 'Error' };
        var mdl = Ext.create('DebtorInvoiceBatchItem', data);
        cfg.frmCreateInterface.loadRecord(mdl);
        this.callParent(arguments);
    },
    setupFrmCreateInterface: function (args) {
        var me = this, cfg = me.config, controls = [];
        var years = [], periods = [];

        for (var y = 2008; y <= 2030; y++) {
            years.push({
                value: y,
                description: y.toString()
            });
        };
        for (var x = 1; x <= 13; x++) {
            periods.push({
                value: x,
                description: x.toString()
            });
        };

        //Create models
        cfg.yearsModel = Ext.define('Years', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'value', type: 'number' },
                'description']
        });
        cfg.periodsModel = Ext.define('Periods', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'value', type: 'number' },
                'description']
        });

        cfg.yearStore = Ext.create('Ext.data.Store', {
            // destroy the store if the grid is destroyed
            autoDestroy: true,
            model: cfg.yearsModel,
            proxy: {
                type: 'memory'
            },
            data: years,
            sorters: [{
                property: 'value',
                direction: 'ASC'
            }]
        });

        cfg.periodStore = Ext.create('Ext.data.Store', {
            // destroy the store if the grid is destroyed
            autoDestroy: true,
            model: cfg.periodsModel,
            proxy: {
                type: 'memory'
            },
            data: periods,
            sorters: [{
                property: 'value',
                direction: 'ASC'
            }]
        });

        cfg.startDateTimeControl = Ext.create('Target.ux.form.field.DateTime', {
            name: 'CreateDate',
            showTime: true,
            dateControlConfig: {
                fieldLabel: 'Start Date/Time',
                labelWidth: 115,
                width: 235,
                allowBlank: false
            },
            timeControlConfig: {
                allowBlank: false
            },
            fieldLabel: '',
            labelWidth: 0
        });
        cfg.postingDateControl = Ext.create('Target.ux.form.field.DateTime', {
            name: 'PostingDateFrom',
            dateControlConfig: {
                fieldLabel: 'Posting Date',
                labelWidth: 115,
                width: 235
            },
            fieldLabel: '',
            labelWidth: 0
        });
        cfg.postingYearCmb = Ext.create('Ext.form.ComboBox', {
            name: 'PostingYear',
            fieldLabel: 'Posting Year',
            store: cfg.yearStore,
            queryMode: 'local',
            displayField: 'description',
            valueField: 'value',
            labelWidth: 115,
            width: 205,
            multiSelect: false,
            plugins: ['clearbutton']
        });
        cfg.periodNoCmb = Ext.create('Ext.form.ComboBox', {
            name: 'PeriodNumber',
            fieldLabel: 'Period Number',
            store: cfg.periodStore,
            queryMode: 'local',
            displayField: 'description',
            valueField: 'value',
            labelWidth: 115,
            width: 205,
            plugins: ['clearbutton']
        });

        cfg.rollbackOptionsRadios = Ext.create('Ext.form.FieldContainer', {
            defaultType: 'radiofield',
            defaults: {
                name: 'Rollback',
                flex: 1,
            },
            items: [
                {
                    boxLabel: 'Rollback entire batch upon encountering an error',
                    inputValue: 'All'
                }, {
                    boxLabel: 'Remove erroneous items from the batch',
                    inputValue: 'Error'
                }
            ]
        });
        cfg.fieldset = Ext.create('Ext.form.FieldSet', {
            title: 'Create Interface File(s)',
            margin: '5 5 5 5',
            collapsible: false,
            items: [//cfg.deferRadios,
            cfg.startDateTimeControl,
            cfg.postingDateControl,
            cfg.postingYearCmb,
            cfg.periodNoCmb,
            cfg.rollbackOptionsRadios]
        });
        cfg.frmCreateInterface = Ext.create('Ext.form.Panel', {
            border: false,
            items: cfg.fieldset,
            listeners: {
                fieldvaliditychange: {
                    fn: function (cmp, fld, vld) {
                        me.fireEvent('fieldvaliditychange', me, vld);
                    },
                    scope: this
                }
            }
        });
        controls.push(cfg.frmCreateInterface);
        me.items = controls;
    },
    getPanelData: function () {
        var me = this, cfg = me.config, frm = cfg.frmCreateInterface.getForm();

        var data = { CreateDate: cfg.startDateTimeControl.getValue(), PostingDateFrom: cfg.postingDateControl.getValue(), PostingYear: cfg.postingYearCmb.getValue(), PeriodNumber: cfg.periodNoCmb.getValue(), Rollback: frm.getValues()['Rollback']};
        return data;
    },
    isValid: function () {
        var me = this, cfg = me.config, frm = cfg.frmCreateInterface.getForm();
        return frm.isValid();
    },
});