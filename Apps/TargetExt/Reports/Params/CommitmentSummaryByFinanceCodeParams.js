Ext.define("Reports.Params.CommitmentSummaryByFinanceCodeParams", {
    extend: "Reports.ReportParameterControl",
    requires: [
        "InPlaceSelectors.Controls.FinanceCodeInPlaceSelector"
    ],
    initComponent: function () {
        var me = this, cfg = me.config, mdl, mdlName = "CommitmentsReportParamsModel";
        var frmData = me.getData();

        // setup search model
        Ext.define(mdlName, {
            extend: "Ext.data.Model",
            fields: [
                { name: "FinanceCode", mapping: "FinanceCode" }
            ],
            idProperty: "ID"
        });
        mdl = Ext.create(mdlName, { FinanceCode: null });

        // setup form, load and add to ctrl
        me.setupFrm();
        cfg.frmCommitmentsReport.loadRecord(mdl);
        me.items = [
            cfg.frmCommitmentsReport
        ];
        // call parent init
        me.callParent(arguments);
    },
    getParameters: function () {
        var me = this;
        cfg = me.config;
        frmCommitmentsReport = cfg.frmCommitmentsReport.getForm();
        rcd = frmCommitmentsReport.getRecord();

        frmCommitmentsReport.updateRecord(rcd);

        return {
            success: true,
            params: [
                {
                    key: "FinanceCode",
                    value: rcd.data.FinanceCode.ID == 0 ? "" : rcd.data.FinanceCode
                }
            ]
        };
    },
    setupFrm: function () {
        var me = this, cfg = me.config, response;
        var frmData = me.getData();

        if (!cfg.frmCommitmentsReport) {

            var frmData = me.getData();

            cfg.frmCommitmentsReportFinanceCodeSelector = Ext.create("InPlaceSelectors.Controls.FinanceCodeInPlaceSelector", {
                fieldLabel: "Finance Code",
                labelWidth: 100,
                anchor: "100%",
                name: "FinanceCode",
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            // create form containing controls
            cfg.frmCommitmentsReport = Ext.create("Ext.form.Panel", {
                border: 0,
                items:
                [
                    cfg.frmCommitmentsReportFinanceCodeSelector
                ]
            });
        }
    }
});
