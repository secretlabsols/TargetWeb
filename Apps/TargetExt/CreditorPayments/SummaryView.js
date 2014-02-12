Ext.define('TargetExt.CreditorPayments.SummaryView', {
    extend: 'Ext.panel.Panel',
    floating: true,
    title: 'Creditor Payment Information',
    layout: 'fit',
    initComponent: function() {
        var me = this, cfg = me.config;
        // create the model item
        Ext.define('CreditorPaymentsSummaryItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'StatusFullDescription', mapping: 'StatusFullDescription' },
                { name: 'PaymentNumber', mapping: 'PaymentNumber' },
                { name: 'DateFrom', mapping: 'DateFrom' },
                { name: 'Total', mapping: 'Total' },
                { name: 'ContractFullDescription', mapping: 'ContractFullDescription' },
                { name: 'CreditorFullDescription', mapping: 'CreditorFullDescription' },
                { name: 'ServiceUserFullDescription', mapping: 'ServiceUserFullDescription' }
            ],
            idProperty: 'ID'
        });
        // create a form panel
        cfg.form = Ext.create('Ext.form.Panel', {
            bodyPadding: 5,
            border: 0,
            defaults: {
                labelWidth: 200
            },
            defaultType: 'displayfield',
            items: [
                {
                    fieldLabel: 'Payment Status',
                    name: 'StatusFullDescription'
                },
                {
                    fieldLabel: 'Payment Number',
                    name: 'PaymentNumber'
                },
                {
                    fieldLabel: 'Payment Date',
                    format: 'date',
                    name: 'DateFrom'
                },
                {
                    fieldLabel: 'Payment Value',
                    format: 'currency',
                    name: 'Total'
                },
                {
                    fieldLabel: 'Contract',
                    name: 'ContractFullDescription'
                },
                {
                    fieldLabel: 'Creditor',
                    name: 'CreditorFullDescription'
                },
                {
                    fieldLabel: 'Service User',
                    name: 'ServiceUserFullDescription'
                }
            ],
            minWidth: 400,
            padding: '3 3 3 3'
        });
        me.items = [cfg.form];
        // call the parent
        this.callParent(arguments);
    },
    show: function(item) {
        var me = this, cfg = me.config;
        // call the parent
        me.callParent();
        item.ContractFullDescription = item.ContractNumber + ': ' + item.ContractTitle;
        item.CreditorFullDescription = item.CreditorRef + ': ' + item.CreditorName;
        item.ServiceUserFullDescription = item.ServiceUserRef + ': ' + item.ServiceUserName;
        item.StatusFullDescription = item.Status + ' on ' + Ext.util.Format.date(item.StatusDate, 'd/m/Y') + ' at ' + Ext.util.Format.date(item.StatusDate, 'H:i:s');
        cfg.form.loadRecord(Ext.create('CreditorPaymentsSummaryItem', item));
        me.doComponentLayout();
    }
});