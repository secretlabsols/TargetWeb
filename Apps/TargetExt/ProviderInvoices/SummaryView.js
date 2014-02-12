Ext.define('TargetExt.ProviderInvoices.SummaryView', {
    extend: 'Ext.panel.Panel',
    floating: true,
    title: 'Non Residential Provider Invoice Information',
    layout: 'fit',
    initComponent: function() {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.DomProviderInvoice.Services.DomProviderInvoicesService;
        // create the model item
        Ext.define('ProviderInvoiceSummaryItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'ProviderFullDescription', mapping: 'ProviderFullDescription' },
                { name: 'ContractFullDescription', mapping: 'ContractFullDescription' },
                { name: 'ClientFullDescription', mapping: 'ClientFullDescription' },
                { name: 'PeriodFrom', mapping: 'PeriodFrom', type: 'date' },
                { name: 'WETo', mapping: 'WETo', type: 'date' },
                { name: 'InvoiceNumber', mapping: 'InvoiceNumber' },
                { name: 'PaymentRef', mapping: 'PaymentRef' },
                { name: 'InvoiceTotal', mapping: 'InvoiceTotal' },
                { name: 'StatusFullDescription', mapping: 'StatusFullDescription' }
            ],
            idProperty: 'ID'
        });
        // create a form panel
        cfg.form = Ext.create('Ext.form.Panel', {
            border: 0,
            defaults: {
                labelWidth: 200
            },
            defaultType: 'displayfield',
            items: [
                {
                    fieldLabel: 'Provider',
                    name: 'ProviderFullDescription'
                },
                {
                    fieldLabel: 'Contract',
                    name: 'ContractFullDescription'
                },
                {
                    fieldLabel: 'Service User',
                    name: 'ClientFullDescription'
                },
                {
                    fieldLabel: 'Period From',
                    format: 'date',
                    name: 'PeriodFrom'
                },
                {
                    fieldLabel: 'Period To',
                    format: 'date',
                    name: 'WETo'
                },
                {
                    fieldLabel: 'Invoice Number',
                    name: 'InvoiceNumber'
                },
                {
                    fieldLabel: 'Invoice Reference',
                    name: 'PaymentRef'
                },
                {
                    fieldLabel: 'Invoice Total',
                    format: 'currency',
                    name: 'InvoiceTotal'
                },
                {
                    fieldLabel: 'Invoice Status',
                    name: 'StatusFullDescription'
                }
            ],
            minWidth: 400,
            padding: '3 3 3 3'
        });
        me.items = [cfg.form];
        // call the parent
        this.callParent(arguments);
    },
    show: function(id) {
        var me = this, cfg = me.config;
        // call the parent
        me.callParent();
        // fetch invoice if not already displayed
        if (id != cfg.lastID) {
            cfg.lastID = id;
            me.setLoading('Loading...');
            cfg.service.FetchDomProviderInvoiceSummary(id, me.showCallBack, me);
        }
    },
    showCallBack: function(response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.form.loadRecord(Ext.create('ProviderInvoiceSummaryItem', response.value.Item));
        me.doComponentLayout();
    }
});