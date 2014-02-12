Ext.define('TargetExt.CreditorPayments.SuspensionEditor', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton',
        'TargetExt.CreditorPayments.SuspensionCriteria',
        'TargetExt.CreditorPayments.SummaryView'
    ],
    config: {
        permissionCanSuspend: false,
        permissionCanAddComment: false,
        permissionCanUnsuspend: false
    },
    closeAction: 'hide',
    modal: true,
    title: 'Suspension Comments',
    width: 600,
    resizable: false,
    constructor: function(config) {
        var me = this, cfg = me.config;
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.CreditorPayments.Services.CreditorPaymentsService;
        // create a container for the selector
        cfg.selectorContainer = Ext.create('Ext.panel.Panel', {
            border: 1,
            padding: '3 3 3 3',
            height: 300,
            layout: {
                type: 'fit',
                align: 'stretch'
            }
        });
        cfg.Summary = Ext.create('TargetExt.CreditorPayments.SummaryView', {});
        cfg.buttonInfo = Ext.create('Ext.Button', {
            cls: 'Selectors',
            iconCls: 'imgInfo',
            listeners: {
                mouseover: {
                    fn: function(cmp, e) {
                        cfg.Summary.show(cfg.showArgs.item);
                        cfg.Summary.alignTo(cmp);
                    },
                    scope: me
                },
                mouseout: {
                    fn: function(cmp, e) {
                        cfg.Summary.hide();
                    },
                    scope: me
                }
            },
            text: 'Information'
        });
        // create the selector control
        cfg.selector = Selectors.Helpers.GetSelectorControlInstance('GenericCreditorPaymentSuspensionHistorySelector', { request: { PageSize: 10000 }, showFilters: false, showLoading: false });
        cfg.selector.OnLoaded(function() {
            cfg.selector.SetPagingVisible(false);
            me.setLoading(false);
        });
        // add buttons bar
        me.bbar = [
            cfg.buttonInfo,
            { xtype: 'tbfill' }
        ];
        // create the form
        cfg.formSave = Ext.create('Ext.Button', {
            text: 'Update',
            cls: 'Selectors',
            disabled: true,
            iconCls: 'imgSave',
            tooltip: 'Update?',
            handler: function() {
                me.update(cfg.form.getRecord());
            }
        });
        cfg.form = Ext.create('TargetExt.CreditorPayments.SuspensionCriteria', {
            bbar: [
                { xtype: 'tbfill' },
                cfg.formSave
            ],
            padding: '3 3 0 3',
            update: function(item) {
                me.update(item);
            },
            listeners: {
                onChanged: {
                    fn: function(cmp, item) {
                        cfg.formSave.setDisabled(item.SuspensionComment <= 0);
                    },
                    scope: me
                }
            }
        });
        // add any items
        me.items = [
            cfg.form,
            cfg.selectorContainer
        ];
        // call parent component
        me.callParent(arguments);
    },
    getGenericCreditorPayment: function() {
        var me = this, cfg = me.config;
        return cfg.showArgs.item;
    },
    hasSuspended: function() {
        var me = this, cfg = me.config;
        return cfg.hasSuspended;
    },
    show: function(args) {
        var me = this, cfg = me.config, paidStr = 'Paid', suspendedStr = 'Suspended';
        me.callParent();
        args = $.extend(true, {
            item: null
        }, args);
        cfg.hasSuspended = false;
        cfg.showArgs = args;
        cfg.form.setVisible(false);
        me.setLoading('Loading...');
        cfg.selector.SetGenericCreditorPaymentID(args.item.ID);
        cfg.selector.Load(cfg.selectorContainer);
        if (args.item.Status != paidStr && !args.item.IsInBatch) {
            var commentType = '', canSuspend = false, canComment = false, canUnsuspend = false;
            if (args.item.Status != suspendedStr && cfg.permissionCanSuspend) {
                canSuspend = true;
                commentType = CreditorPaymentsSuspensionTypes.Suspend;
            } else if (args.item.Status == suspendedStr) {
                canComment = cfg.permissionCanAddComment;
                canUnsuspend = cfg.permissionCanUnsuspend;
                if (cfg.permissionCanAddComment) {
                    commentType = CreditorPaymentsSuspensionTypes.AddComment;
                } else if (cfg.permissionCanUnsuspend) {
                    commentType = CreditorPaymentsSuspensionTypes.UnSuspend;
                }
            }
            cfg.form.load(canSuspend, canComment, canUnsuspend, commentType);
            cfg.form.setVisible(true);
        }
    },
    update: function(item) {
        var me = this, cfg = me.config;
        me.setLoading('Updating...');
        cfg.service.UpdateStatus(cfg.showArgs.item.ID, item.SuspensionComment, item.SuspensionCommentTypeWs, true, me.updateCallBack, me);
    },
    updateCallBack: function(response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.showArgs.item = response.value.Item.Item;
        me.show(cfg.showArgs);
        cfg.hasSuspended = true;
    }
});