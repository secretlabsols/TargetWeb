Ext.define("Reports.Params.CommitmentSummaryByClientParams", {
    extend: "Reports.ReportParameterControl",
    initComponent: function () {
        var me = this, cfg = me.config, mdl, mdlName = "CommitmentSummaryByClientParamsModel";
        var frmData = me.getData();

        // setup search model
        Ext.define(mdlName, {
            extend: "Ext.data.Model",
            fields: [
                { name: "ServiceUserName", mapping: "ServiceUserName" },
                { name: "ServiceUserReference", mapping: "ServiceUserReference" }
            ]
        });
        mdl = Ext.create(mdlName, { ClientSurname: null });

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
                    key: "ServiceUserName",
                    value: rcd.data.ServiceUserName
                },
                {
                    key: "ServiceUserReference",
                    value: rcd.data.ServiceUserReference
                }
            ]
        };
    },
    setupFrm: function () {
        var me = this, cfg = me.config, response;
        var frmData = me.getData();

        if (!cfg.frmCommitmentsReport) {

            var frmData = me.getData();

            cfg.txtClientSurname = Ext.create("Ext.form.TextField", {
                fieldLabel: "Client Surname",
                labelWidth: 150,
                anchor: "100%",
                name: "ServiceUserName"
            });

            cfg.txtClientReference = Ext.create("Ext.form.TextField", {
                fieldLabel: "Client Reference",
                labelWidth: 150,
                anchor: "100%",
                name: "ServiceUserReference"
            });

            // create form containing controls
            cfg.frmCommitmentsReport = Ext.create("Ext.form.Panel", {
                border: 0,
                items:
                [
                    cfg.txtClientSurname,
                    cfg.txtClientReference
                ]
            });
        }
    }
});
