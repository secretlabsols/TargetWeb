Ext.define("Reports.Params.CommitmentSummaryByProviderAndContractParams", {
    extend: "Reports.ReportParameterControl",
    requires: [
        "InPlaceSelectors.Controls.ProviderInPlaceSelector",
        "InPlaceSelectors.Controls.GenericContractInPlaceSelector"
    ],
    initComponent: function () {
        var me = this, cfg = me.config, mdl, mdlName = "CommitmentSummaryByProviderAndContractParamsModel";
        var frmData = me.getData();

        // setup search model
        Ext.define(mdlName, {
            extend: "Ext.data.Model",
            fields: [
                { name: "Provider", mapping: "Provider" },
                { name: "Contract", mapping: "Contract" }
            ]
        });
        mdl = Ext.create(mdlName, { ProviderId: null, ContractId: null });

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
                    key: "ProviderId",
                    value: rcd.data.Provider.ID
                },
                {
                    key: "ContractId",
                    value: rcd.data.Contract.ID
                }
            ]
        };
    },
    setupFrm: function () {
        var me = this, cfg = me.config, response;
        var frmData = me.getData();

        if (!cfg.frmCommitmentsReport) {

            var frmData = me.getData();

            cfg.frmCommitmentsReportProviderSelector = Ext.create("InPlaceSelectors.Controls.ProviderInPlaceSelector", {
                fieldLabel: "Provider",
                labelWidth: 100,
                anchor: "100%",
                name: "Provider",
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            cfg.frmCommitmentsReportContractSelector = Ext.create("InPlaceSelectors.Controls.GenericContractInPlaceSelector", {
                fieldLabel: "Contract",
                labelWidth: 100,
                anchor: "100%",
                name: "Contract",
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            // create form containing controls
            cfg.frmCommitmentsReport = Ext.create("Ext.form.Panel", {
                border: 0,
                items:
                [
                    cfg.frmCommitmentsReportProviderSelector,
                    cfg.frmCommitmentsReportContractSelector
                ]
            });

            //cfg.frmCommitmentsReport.linkToInPlaceContractSelector(cfg.frmCommitmentsReportContractSelector, args.SearcherSettings.Item.Provider.ID);   // link contracts selector to provider selector
        }
    }
});
