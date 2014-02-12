CreditorPaymentsSuspensionTypes = {
    AddComment: '0',
    Suspend: '1',
    UnSuspend: '2'
};

Ext.define('TargetExt.CreditorPayments.SuspensionCriteria', {
    extend: 'Ext.form.Panel',
    border: 1,
    bodyPadding: 5,
    title: 'Update Status',
    initComponent: function() {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.CreditorPayments.Services.CreditorPaymentsService;
        // create the form
        cfg.formCommentTypes = Ext.create('Ext.form.RadioGroup', {
            fieldLabel: 'Type',
            defaults: {
                disabled: true
            },
            items: [
                { boxLabel: 'Suspend', name: 'SuspensionCommentType', inputValue: CreditorPaymentsSuspensionTypes.Suspend },
                { boxLabel: 'Add Comment', name: 'SuspensionCommentType', inputValue: CreditorPaymentsSuspensionTypes.AddComment },
                { boxLabel: 'Un-suspend', name: 'SuspensionCommentType', inputValue: CreditorPaymentsSuspensionTypes.UnSuspend }
            ],
            name: 'SuspensionCommentTypes',
            listeners: {
                change: {
                    fn: function() {
                        me.updateCommentTypes();
                        me.adviseChanged();
                    },
                    scope: me
                }
            }
        });
        cfg.formCommentsStore = Ext.create('Ext.data.Store', {
            fields: ['ID', 'Description'],
            data: []
        });
        cfg.formCommentsCmb = Ext.create('Ext.form.ComboBox', {
            fieldLabel: 'Comment',
            store: cfg.formCommentsStore,
            queryMode: 'local',
            disabled: true,
            displayField: 'Description',
            anchor: '100%',
            valueField: 'ID',
            name: 'SuspensionComment',
            listeners: {
                change: {
                    fn: function(cmp, newVal) {
                        me.adviseChanged();
                    },
                    scope: me
                }
            },
            plugins: ['clearbutton']
        });
        me.items = [
            cfg.formCommentTypes,
            cfg.formCommentsCmb
        ];
        // call parent component
        me.callParent(arguments);
    },
    adviseChanged: function() {
        var me = this, cfg = me.config;
        me.fireEvent('onChanged', me, me.getRecord());
    },
    cacheCommentTypes: function(commentType, commentTypes) {
        var me = this, cfg = me.config;
        if (!cfg.cachedCommentTypes) {
            cfg.cachedCommentTypes = [];
        }
        cfg.cachedCommentTypes.push({ commentType: commentType, commentTypes: commentTypes });
    },
    getCachedCommentTypes: function(commentType) {
        var me = this, cfg = me.config, commentTypes;
        if (!cfg.cachedCommentTypes) {
            cfg.cachedCommentTypes = [];
        }
        Ext.Array.each(cfg.cachedCommentTypes, function(val, idx) {
            if (val.commentType === commentType) {
                commentTypes = val.commentTypes;
                return false;
            }
        });
        return commentTypes;
    },
    getRecord: function() {
        var me = this, record = me.getForm().getValues();
        if (record.SuspensionCommentType == CreditorPaymentsSuspensionTypes.AddComment) {
            record.SuspensionCommentTypeWs = CreditorPaymentsSuspensionTypes.Suspend;
        } else {
            record.SuspensionCommentTypeWs = record.SuspensionCommentType;
        }
        if (!record.SuspensionComment) {
            record.SuspensionComment = 0;
        }
        return record;
    },
    load: function(canSuspend, canComment, canUnsuspend, commentType) {
        var me = this, cfg = me.config, commentTypes = cfg.formCommentTypes.items.items;
        cfg.formCommentTypes.suspendEvents(false);
        cfg.formCommentTypes.setValue();
        cfg.formCommentsCmb.setValue();
        commentTypes[0].setDisabled(!canSuspend);
        commentTypes[1].setDisabled(!canComment);
        commentTypes[2].setDisabled(!canUnsuspend);
        cfg.formCommentTypes.setValue({ SuspensionCommentType: commentType });
        cfg.formCommentTypes.resumeEvents();
        cfg.formCommentsCmb.setDisabled((!canSuspend && !canComment && !canUnsuspend));
        me.updateCommentTypes();
    },
    update: function(item) {
        alert('Update must be implemented by derived classes');
    },
    updateCommentTypes: function() {
        var me = this, cfg = me.config, selectedCommentType = me.getRecord().SuspensionCommentTypeWs, cachedCommentTypes;
        cfg.formCommentsCmb.setValue();
        if (selectedCommentType >= 0) {
            cachedCommentTypes = me.getCachedCommentTypes(selectedCommentType);
            if (!cachedCommentTypes) {
                cfg.service.GetGenericCreditorPaymentSuspensionComments(selectedCommentType, 99, me.updateCommentTypesCallBack, me);
            } else {
                cfg.formCommentsStore.loadData(cachedCommentTypes);
            }
        }
    },
    updateCommentTypesCallBack: function(response) {
        var me = response.context, cfg = me.config;
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        me.cacheCommentTypes(response.request.args.type, response.value.Item);
        cfg.formCommentsStore.loadData(response.value.Item);
    }
});