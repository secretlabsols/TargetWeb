Ext.define('TargetExt.CreditorPayments.CreateAuthorisePaymentsJob', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.DateTime',
        'Target.ux.form.field.ClearButton',
        'TargetExt.CreditorPayments.AnticipatedBatchContentsView'
    ],
    closeAction: 'hide',
    modal: true,
    title: 'Authorise Payments',
    width: 430,
    resizable: false,
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.CreditorPayments.Services.CreditorPaymentsService;
        // setup the form
        me.setupForm();
        cfg.formSave = Ext.create('Ext.Button', {
            text: 'Create',
            cls: 'Selectors',
            disabled: true,
            iconCls: 'imgSave',
            tooltip: 'Create Job?',
            handler: function () {
                me.createJob();
            }
        });
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
    createJob: function () {
        var me = this, cfg = me.config;
        me.setLoading('Creating Job...');
        cfg.service.CreateGenericCreditorPaymentAuthorisationJob(cfg.formJobDateTime.getDateTime(), cfg.lastArgs, me.createJobCallBack, me);
    },
    createJobCallBack: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        me.close();
    },
    setupForm: function () {
        var me = this, cfg = me.config;
        if (!cfg.form) {
            // create the form and items
            cfg.formAnticipated = Ext.create('TargetExt.CreditorPayments.AnticipatedBatchContentsView', {
                listeners: {
                    onRefreshing: {
                        fn: function (cmp) {
                            me.refreshingSummary();
                        },
                        scope: me
                    },
                    onRefreshed: {
                        fn: function (cmp, summary) {
                            me.refreshedSummary(summary);
                        },
                        scope: me
                    }
                }
            });
            cfg.formJobDateTime = Ext.create('Target.ux.form.field.DateTime', {
                name: 'CreateDate',
                showTime: true,
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
            cfg.formJobFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Authorisation Job Options',
                collapsible: false,
                items: [
                    cfg.formJobDateTime
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
    show: function (args) {
        var me = this, cfg = me.config, dateNow = new Date();
        me.callParent();
        cfg.formJobDateTime.setValue(dateNow);
        // default any items that arent applicable for authorisation
        args.Excluded = false;
        args.StatusIncludeAuthorised = false;
        args.StatusIncludePaid = false;
        args.StatusIncludeSuspended = false;
        args.StatusIncludeUnpaid = true;
        cfg.lastArgs = args;
        me.refreshSummary();
    },
    refreshSummary: function () {
        var me = this, cfg = me.config;
        cfg.formAnticipated.refresh(cfg.lastArgs);
    },
    refreshedSummary: function (summary) {
        var me = this, cfg = me.config;
        me.setLoading(false);
        cfg.totalPayments = summary.TotalPayments;
        me.setSaveAvailibility();
    },
    refreshingSummary: function () {
        var me = this, cfg = me.config;
        me.setLoading('Loading...');
    },
    setSaveAvailibility: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm();
        cfg.formSave.setDisabled((cfg.totalPayments == 0 || !frm.isValid()));
    }
});