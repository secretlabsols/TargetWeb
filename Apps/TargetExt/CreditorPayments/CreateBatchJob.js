Ext.define('TargetExt.CreditorPayments.CreateBatchJob', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.DateTime',
        'Target.ux.form.field.ClearButton',
        'TargetExt.CreditorPayments.AnticipatedBatchContentsView'
    ],
    closeAction: 'hide',
    modal: true,
    title: 'Create Creditor Payments Batch',
    width: 420,
    resizable: false,
    initComponent: function() {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.CreditorPayments.Services.CreditorPaymentsService;
        // setup the form
        me.setupForm();
        // create and load model
        Ext.define('CreditorPaymentsBatchItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'StartDateTime', mapping: 'StartDateTime', type: 'date' },
                { name: 'PostingDate', mapping: 'PostingDate' },
                { name: 'PostingYear', mapping: 'PostingYear' },
                { name: 'PeriodNumber', mapping: 'PeriodNumber' },
                { name: 'Rollback', mapping: 'Rollback' }
            ],
            idProperty: 'ID'
        });
        me.defaultFormData();
        // add buttons bar
        me.bbar = [
            { xtype: 'tbfill' },
            cfg.formSave
        ];
        // add any items
        me.items = [
            cfg.form
        ];
        // call parent component
        me.callParent(arguments);
    },
    createJob: function() {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), info = frm.getRecord();
        frm.updateRecord(info);
        info = info.data;
        me.setLoading('Creating Job...');
        cfg.service.CreateGenericCreditorPaymentBatchJob(info.StartDateTime, info.PostingDate, info.PostingYear, info.PeriodNumber, info.Rollback, cfg.lastArgs, me.createJobCallBack, me);
    },
    createJobCallBack: function(response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        me.close();
    },
    defaultFormData: function() {
        var me = this, cfg = me.config;
        cfg.form.loadRecord(Ext.create('CreditorPaymentsBatchItem', {
            StartDateTime: new Date(),
            PostingDate: null,
            PostingYear: null,
            PeriodNumber: null,
            Rollback: 1
        }));
    },
    setupForm: function() {
        var me = this, cfg = me.config;
        if (!cfg.form) {
            // create the form and items
            cfg.formAnticipated = Ext.create('TargetExt.CreditorPayments.AnticipatedBatchContentsView', {
                listeners: {
                    onRefreshing: {
                        fn: function(cmp) {
                            me.refreshingSummary();
                        },
                        scope: me
                    },
                    onRefreshed: {
                        fn: function(cmp, summary) {
                            me.refreshedSummary(summary);
                        },
                        scope: me
                    }
                }
            });
            cfg.formJobDateTime = Ext.create('Target.ux.form.field.DateTime', {
                showTime: true,
                name: 'StartDateTime',
                dateControlConfig: {
                    fieldLabel: 'Start Date/Time',
                    labelWidth: 120,
                    width: 250,
                    allowBlank: false
                },
                timeControlConfig: {
                    allowBlank: false
                },
                fieldLabel: '',
                labelWidth: 0
            });
            cfg.formJobDateTime.setMinDateValue(new Date());
            cfg.formPostingDate = Ext.create('Target.ux.form.field.DateTime', {
            name: 'PostingDate',
                dateControlConfig: {
                    fieldLabel: 'Posting Date',
                    labelWidth: 120,
                    width: 250,
                    allowBlank: true
                },
                fieldLabel: '',
                labelWidth: 0
            });
            cfg.formPostingYearData = [];
            for (var i = 2008; i <= 2030; i++) {
                cfg.formPostingYearData.push({ Value: i, Description: i });
            }
            cfg.formPostingYearStore = Ext.create('Ext.data.Store', {
                fields: ['Value', 'Description'],
                data: cfg.formPostingYearData
            });
            cfg.formPostingYearCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Posting Year',
                store: cfg.formPostingYearStore,
                queryMode: 'local',
                displayField: 'Description',
                labelWidth: 120,
                width: 250,
                valueField: 'Value',
                name: 'PostingYear',
                id: 'PostingYear',
                plugins: ['clearbutton']
            });
            cfg.formPeriodNumberData = [];
            for (var i = 1; i <= 13; i++) {
                cfg.formPeriodNumberData.push({ Value: i, Description: i });
            }
            cfg.formPeriodNumberStore = Ext.create('Ext.data.Store', {
                fields: ['Value', 'Description'],
                data: cfg.formPeriodNumberData
            });
            cfg.formPeriodNumberCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Period Number',
                store: cfg.formPeriodNumberStore,
                queryMode: 'local',
                displayField: 'Description',
                labelWidth: 120,
                width: 250,
                valueField: 'Value',
                name: 'PeriodNumber',
                id: 'PeriodNumber',
                plugins: ['clearbutton']
            });
            cfg.formRollBackRdos = Ext.create('Ext.form.FieldContainer', {
                defaultType: 'radiofield',
                defaults: {
                    flex: 1,
                    name: 'Rollback'
                },
                items: [
                    {
                        boxLabel: 'Rollback entire batch upon encountering an error',
                        inputValue: 2
                    }, {
                        boxLabel: 'Remove erroneous items from the batch',
                        inputValue: 1
                    }
                ]
            });
            cfg.formJobFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Create Interface File(s)',
                collapsible: false,
                items: [
                    cfg.formJobDateTime,
                    cfg.formPostingDate,
                    cfg.formPostingYearCmb,
                    cfg.formPeriodNumberCmb,
                    cfg.formRollBackRdos
                ]
            });
            cfg.formSave = Ext.create('Ext.Button', {
                text: 'Create',
                cls: 'Selectors',
                disabled: true,
                iconCls: 'imgSave',
                tooltip: 'Create Job?',
                handler: function() {
                    me.createJob();
                },
                items: [
                    cfg.formInformationFlds,
                    cfg.formJobFlds
                ]
            });
            cfg.form = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                    cfg.formAnticipated,
                    cfg.formJobFlds
                ],
                listeners: {
                    fieldvaliditychange: {
                        fn: function () {
                            me.setSaveAvailibility();
                        },
                        scope: this
                    }
                }
            });
        }
    },
    show: function(args) {
        var me = this, cfg = me.config, dateNow = new Date();
        me.callParent();
        me.defaultFormData();
        // default any items that arent applicable for creating batches
        args.Excluded = false;
        args.StatusIncludeAuthorised = true;
        args.StatusIncludePaid = false;
        args.StatusIncludeSuspended = false;
        args.StatusIncludeUnpaid = false;
        cfg.lastArgs = args;
        me.refreshSummary();
    },
    refreshSummary: function() {
        var me = this, cfg = me.config;
        cfg.formAnticipated.refresh(cfg.lastArgs);
    },
    refreshedSummary: function(summary) {
        var me = this, cfg = me.config;
        me.setLoading(false);
        cfg.totalPayments = summary.TotalPayments;
        me.setSaveAvailibility();
    },
    refreshingSummary: function() {
        var me = this, cfg = me.config;
        me.setLoading('Loading...');
    },
    setSaveAvailibility: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm();
        cfg.formSave.setDisabled((cfg.totalPayments == 0 || !frm.isValid()));
    }
});