Ext.define('TargetExt.CreditorPayments.AnticipatedBatchContentsView', {
    extend: 'Ext.form.FieldSet',
    title: 'Anticipated Batch Contents',
    collapsible: false,
    initComponent: function() {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.CreditorPayments.Services.CreditorPaymentsService;
        cfg.itmPaymentCount = Ext.create('Ext.form.field.Display', {
            fieldLabel: 'Payment Count',
            name: 'PaymentCount',
            value: '0'
        });
        cfg.itmTotalValue = Ext.create('Ext.form.field.Display', {
            fieldLabel: 'Total Value',
            name: 'TotalValue',
            format: 'currency',
            value: 0
        });
        me.items = [
            cfg.itmPaymentCount,
            {
                xtype: 'fieldcontainer',
                fieldLabel: '',
                labelWidth: 100,
                layout: 'hbox',
                defaultType: 'displayfield',
                items: [
                    cfg.itmTotalValue,
                    { xtype: 'tbfill' },
                    {
                        xtype: 'button',
                        text: 'Refresh',
                        tooltip: 'Refresh?',
                        handler: function() {
                            me.refresh(cfg.lastArgs);
                        }
                    }
                ]
            }

        ];
        // call the parent
        this.callParent(arguments);
    },
    refresh: function(args) {
        var me = this, cfg = me.config;
        me.fireEvent('onRefreshing', me);
        cfg.lastArgs = args;
        cfg.service.GetGenericCreditorPaymentsSummary(cfg.lastArgs, me.refreshCallBack, me);
    },
    refreshCallBack: function(response) {
        var me = response.context, cfg = me.config, summary;
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            me.fireEvent('onRefreshed', me, null);
            return false;
        }
        summary = response.value.Item;
        cfg.itmPaymentCount.setValue(summary.TotalPayments);
        cfg.itmTotalValue.setValue(summary.TotalPaymentsValue);
        me.fireEvent('onRefreshed', me, summary);
    }

});