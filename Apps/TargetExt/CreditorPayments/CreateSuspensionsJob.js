Ext.define('TargetExt.CreditorPayments.CreateSuspensionsJob', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.DateTime',
        'Target.ux.form.field.ClearButton',
        'TargetExt.CreditorPayments.AnticipatedBatchContentsView'
    ],
    config: {
        permissionCanSuspend: false,
        permissionCanAddComment: false,
        permissionCanUnsuspend: false,
        summary: { PaymentCount: 0, TotalValue: 0 }
    },
    closeAction: 'hide',
    modal: true,
    title: 'Suspension Comments',
    width: 500,
    resizable: false,
    constructor: function (config) {
        var me = this, cfg = me.config;
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
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
            },
            items: [
                cfg.formInformationFlds,
                cfg.formJobFlds
            ]
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
        var me = this, cfg = me.config, item = me.getRecord();
        me.setLoading('Creating Job...');
        cfg.service.CreateGenericCreditorPaymentSuspensionsJob(item.CreateDate, item.SuspensionCommentType, item.SuspensionComment, cfg.lastArgs.params, me.createJobCallBack, me);
    },
    createJobCallBack: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        me.close();
    },
    getRecord: function () {
        var me = this, cfg = me.config, record = cfg.formSuspensionCriteria.getRecord();
        record.CreateDate = cfg.formJobDateTime.getValue();
        return record;
    },
    handleFormChanged: function () {
        var me = this, cfg = me.config, record = me.getRecord(), frm = cfg.form.getForm();
        cfg.formSave.setDisabled((cfg.summary.PaymentCount == 0 || record.SuspensionComment <= 0 || !frm.isValid()));
    },
    setupForm: function () {
        var me = this, cfg = me.config;
        if (!cfg.form) {
            // create the form and items
            cfg.formAnticipated = Ext.create('TargetExt.CreditorPayments.AnticipatedBatchContentsView', {
                listeners: {
                    onRefreshing: {
                        fn: function () {
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
                    labelWidth: 100,
                    width: 200,
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
                title: 'Suspension Job Options',
                collapsible: false,
                items: [
                    cfg.formJobDateTime
                ]
            });
            cfg.formSuspensionCriteria = Ext.create('TargetExt.CreditorPayments.SuspensionCriteria', {
                border: 0,
                bodyPadding: 0,
                title: '',
                update: function (item) {
                    me.update(item);
                },
                listeners: {
                    onChanged: {
                        fn: function (cmp, item) {
                            me.handleFormChanged();
                        },
                        scope: me
                    }
                }
            });
            cfg.formSuspensionFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Suspension Criteria',
                collapsible: false,
                bodyPadding: 0,
                items: [
                    cfg.formSuspensionCriteria
                ]
            });
            cfg.form = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                    cfg.formAnticipated,
                    cfg.formSuspensionFlds,
                    cfg.formJobFlds
                ],
                listeners: {
                    fieldvaliditychange: {
                        fn: function () {
                            me.handleFormChanged();
                        },
                        scope: this
                    }
                }
            });
        }
    },
    show: function (args) {
        var me = this, cfg = me.config, dateNow = new Date(), commentType, canSuspend = false, canComment = false, canUnsuspend = false;
        me.callParent();
        args = $.extend(true, {
            hasSuspended: false,
            hasAuthorised: false,
            hasUnpaid: false,
            params: null
        }, args);
        cfg.formJobDateTime.setValue(dateNow);
        args.StatusIncludePaid = false;
        cfg.lastArgs = args;
        me.refreshSummary();
        if (args.hasSuspended || args.hasAuthorised || args.hasUnpaid) {
            canSuspend = (cfg.permissionCanSuspend && !args.hasSuspended);
            canComment = (cfg.permissionCanAddComment && args.hasSuspended);
            canUnsuspend = (cfg.permissionCanUnsuspend && args.hasSuspended);
            if (canSuspend) {
                commentType = CreditorPaymentsSuspensionTypes.Suspend;
            } else if (canComment) {
                commentType = CreditorPaymentsSuspensionTypes.AddComment;
            } else if (canUnsuspend) {
                commentType = CreditorPaymentsSuspensionTypes.UnSuspend;
            }
        }
        cfg.formSuspensionCriteria.load(canSuspend, canComment, canUnsuspend, commentType);
        cfg.formSave.setDisabled(true);
    },
    refreshSummary: function () {
        var me = this, cfg = me.config;
        cfg.formAnticipated.refresh(cfg.lastArgs.params);
    },
    refreshedSummary: function (summary) {
        var me = this, cfg = me.config, record = me.getRecord();
        me.setLoading(false);
        cfg.summary = summary;
        me.handleFormChanged();
    },
    refreshingSummary: function () {
        var me = this, cfg = me.config;
        me.setLoading('Loading...');
    }
});