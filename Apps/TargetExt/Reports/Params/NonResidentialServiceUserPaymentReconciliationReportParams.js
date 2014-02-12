Ext.define('Reports.Params.NonResidentialServiceUserPaymentReconciliationReportParams', {
    extend: 'Reports.ReportParameterControl',
    requires: [
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config, mdl, mdlName = "NonResidentialServiceUserPaymentReconciliationReportParamsModel";
        var frmData = me.getData();

        // setup search model
        Ext.define(mdlName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'Provider', mapping: 'Provider' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'IsActualPaymentDate', mapping: 'IsActualPaymentDate', type: 'boolean' }
            ]
        });
        mdl = Ext.create(mdlName, { ProviderID: null, DateFrom: null, DateTo: null, IsActualPaymentDate: frmData.HasActualPaymentDates });

        // setup form, load and add to ctrl
        me.setupFrm();
        cfg.frmPaymentReconciliation.loadRecord(mdl);
        me.items = [
            cfg.frmPaymentReconciliation
        ];
        // call parent init
        me.callParent(arguments);

        cfg.frmPaymentReconciliationPaymentType.items.getAt(0).setValue(!frmData.HasActualPaymentDates);
        cfg.frmPaymentReconciliationPaymentType.items.getAt(1).setValue(frmData.HasActualPaymentDates);
        cfg.frmPaymentReconciliationPaymentType.items.getAt(1).disabled = !frmData.HasActualPaymentDates;
    },
    getParameters: function () {
        var me = this;
        cfg = me.config;
        frmPaymentReconciliation = cfg.frmPaymentReconciliation.getForm();
        rcd = frmPaymentReconciliation.getRecord();

        frmPaymentReconciliation.updateRecord(rcd);

        return {
            success: true,
            params: [
                {
                    key: 'pid',
                    value: rcd.data.Provider.ID
                },
                {
                    key: 'df',
                    value: Ext.Date.format(rcd.data.DateFrom, 'Y-m-d')
                },
                {
                    key: 'dt',
                    value: Ext.Date.format(rcd.data.DateTo, 'Y-m-d')
                },
                {
                    key: 'bapd',
                    value: rcd.data.IsActualPaymentDate
                }
            ]
        };
    },
    setupFrm: function () {
        var me = this, cfg = me.config, response;
        if (!cfg.frmPaymentReconciliation) {

            var frmData = me.getData();

            cfg.frmPaymentReconciliationProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Provider',
                labelWidth: 100,
                anchor: '100%',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            cfg.frmPaymentReconciliationPaidBetweenDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                showTime: false,
                fieldLabel: 'Paid Between',
                dateRangeDateTimeFromControlConfig: {
                    name: 'DateFrom'
                },
                dateRangeDateTimeToControlConfig: {
                    name: 'DateTo'
                },
                rangeOptionStoreData:
                [
                    { value: 0, description: 'Falling within the period' }
                ]
            });

            cfg.frmPaymentReconciliationPaymentType = Ext.create("Ext.form.RadioGroup",
            {
                vertical: true,
                padding: 10,
                items:
                [
                    {
                        boxLabel: 'Date submitted for payment',
                        name: 'IsActualPaymentDate',
                        inputValue: false
                    },
                    {
                        boxLabel: 'Actual payment date',
                        name: 'IsActualPaymentDate',
                        inputValue: true
                    }
                ]
            });

            // create form containing controls
            cfg.frmPaymentReconciliation = Ext.create('Ext.form.Panel', {
                border: 0,
                items:
                [
                    cfg.frmPaymentReconciliationProviderSelector,
                    cfg.frmPaymentReconciliationPaidBetweenDateRange
                ]
            });

            if (!frmData.HasActualPaymentDates) {
                cfg.frmActualPaymentWarning = Ext.create("Ext.form.Label",
                {
                    text: "You cannot report on 'actual payment date', as the council does not record this detail in abacus.",
                    border: false,
                    padding: 5
                });
                cfg.frmPaymentReconciliation.items.add(cfg.frmActualPaymentWarning);
            }
            else {
                if (!cfg.lblActualPaymentWarning1) {
                    cfg.lblActualPaymentWarning1 = Ext.create("Ext.form.Label",
                    {
                        text: "You cannot report before '" + Ext.Date.format(frmData.FirstActualPaymentDate, "d/m/y") + "', as the council did not previously record payment dates in abacus.",
                        border: false,
                        padding: 5
                    });
                    cfg.frmPaymentReconciliation.items.add(cfg.lblActualPaymentWarning1);
                }
                if (!cfg.lblActualPaymentWarning2) {
                    cfg.lblActualPaymentWarning2 = Ext.create("Ext.form.Label",
                    {
                        text: "The latest confirmed payment was made on '" + Ext.Date.format(frmData.LastActualPaymentDate, "d/m/y") + "'.",
                        border: false,
                        padding: 5
                    });
                    cfg.frmPaymentReconciliation.items.add(cfg.lblActualPaymentWarning2);
                }
                if (!cfg.lblActualPaymentWarning3) {
                    cfg.lblActualPaymentWarning3 = Ext.create("Ext.form.Label",
                    {
                        text: "If your report based on 'actual payment date' does not tally with your receipts, report again using the criteria of 'date submitted for payment'.",
                        border: false,
                        padding: 5
                    });
                    cfg.frmPaymentReconciliation.items.add(cfg.lblActualPaymentWarning3);
                }
            }

            cfg.frmPaymentReconciliation.items.add(cfg.frmPaymentReconciliationPaymentType);
        }
    }
});
