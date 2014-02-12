Ext.define("TargetExt.CommitmentReportingManagement.Editor", {
    extend: "Ext.window.Window",
    requires: [
        "Target.ux.form.field.ClearButton"
    ],
    closeAction: "hide",
    title: "New Commitment Report Data",
    resizable: false,
    width: 500,
    modal: true,
    config: {
        currentItem: null,
        hasUpdated: false,
        statusColl: null,
        subStatusColl: null,
        webSecurityUserID: 0
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config, controls = [];
        cfg.service = Target.Abacus.Library.WorkTrayCommitmentReportingManagement.Services.WorkTrayCommitmentReportingManagement;
        me.setupForm();
        me.bbar = [
            { xtype: "tbfill" },
            cfg.formSave,
            cfg.formCancel
        ];
        me.items = [
            cfg.frmGeneral
        ];
        me.callParent(arguments);
    },
    setValues: function () {
        var me = this, cfg = me.config;
        cfg.ddlBudgetPeriod.setValue();
        cfg.ddlBudgetYear.setValue();
    },
    setupForm: function () {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            // Budget year 
            cfg.budgetYearData = Ext.create("Ext.data.Store", {
                fields: ["ID", "Description", "IsFinancialYear"],
                data: cfg.budgetYearCollection,
                remoteSort: false
            });
            cfg.budgetYearData.sort("Description", "ASC");

            // Budget period
            cfg.budgetPeriodData = Ext.create("Ext.data.Store", {
                name: "BudgetPeriodData",
                fields: ["ID",
                         "BudgetYearID",
                         {
                             name: "Description",
                             convert: function (v, rec) {
                                 return Ext.util.Format.trim(
                                            Ext.util.Format.date(rec.get("DateFrom"), "d/m/y") + " - " +
                                            Ext.util.Format.date(rec.get("DateTo"), "d/m/y")
                                        );
                             }
                         },
                         "DateFrom",
                         "DateTo",
                         "PeriodNumber"],
                data: cfg.budgetPeriodCollection,
                remoteSort: false
            });
            cfg.budgetPeriodData.sort("BudgetYearID, DateFrom", "ASC");

            cfg.ddlBudgetYear = Ext.create("Ext.form.ComboBox", {
                fieldLabel: "Budget Year",
                store: cfg.budgetYearData,
                multiSelect: false,
                queryMode: "local",
                displayField: "Description",
                valueField: "ID",
                labelWidth: 100,
                anchor: "100%",
                name: "BudgetYearId",
                listeners: {
                    select: function (combo, records, index) {
                        var frm = cfg.frmGeneral.getForm();
                        var selectedValue = records[0].get("ID");

                        // Clear any existing filters
                        cfg.budgetPeriodData.clearFilter(false);

                        // Filter the store to only display those periods for the selected year
                        var filter = Ext.create("Ext.util.Filter", {
                            scope: this,
                            filterFn: function (item) {
                                return (item.data.BudgetYearID == selectedValue);
                            }
                        });

                        // Apply the filter to the store
                        cfg.budgetPeriodData.filter(filter);
                        cfg.budgetPeriodData.sort("ID", "ASC");

                        // Now we have a selected value enable the save button
                        cfg.formSave.setDisabled(!frm.isValid());
                    }
                },
                allowBlank: false,
                forceSelection: true
            });
            cfg.ddlBudgetPeriod = Ext.create("Ext.form.ComboBox", {
                fieldLabel: "Budget Period",
                store: cfg.budgetPeriodData,
                multiSelect: false,
                queryMode: "local",
                displayField: "Description",
                valueField: "ID",
                labelWidth: 100,
                anchor: "100%",
                name: "BudgetPeriodId",
                plugins: ["clearbutton"]
            });

            cfg.lblBudgetYear = Ext.create("Ext.form.Label", {
                text: "Select the budget year for which to create report data.",
                forId: "BudgetYearId",
                margin: "0 0 10 0"
            });

            cfg.lblBudgetPeriod = Ext.create("Ext.form.Label", {
                text: "Optionally, select the budget period within the selected year to further narrow down the data.",
                forId: "BudgetPeriodId",
                margin: "0 0 15 0"
            });

            cfg.formSave = Ext.create("Ext.Button", {
                text: "Save Request",
                cls: "Selectors",
                iconCls: "imgSave",
                tooltip: "Create a new report request",
                handler: function () {
                    me.saveConfirm();
                }
            });

            cfg.formCancel = Ext.create("Ext.Button", {
                text: "Cancel",
                cls: "Selectors",
                iconCls: "imgDelete",
                tooltip: "Cancel",
                handler: function () {
                    me.cancel();
                }
            });

            // create the form with all items
            cfg.frmGeneral = Ext.create("Ext.form.Panel", {
                border: 0,
                bodyPadding: 5,
                items: [
                   cfg.lblBudgetYear,
                   cfg.lblBudgetPeriod,
                   cfg.ddlBudgetYear,
                   cfg.ddlBudgetPeriod,
                ]
            });
        }
    },
    saveConfirm: function () {
        var me = this, cfg = me.config;
        cfg.service.HasDataForSelectedRange(cfg.ddlBudgetYear.getValue(),
                                            cfg.ddlBudgetPeriod.getValue(),
                                            me.saveConfirmCallback, me)

    },
    save: function () {
        var me = this, cfg = me.config;
        me.setLoading("Creating report request...");

        cfg.service.AddNewRequest(cfg.ddlBudgetYear.getValue(),
                                   cfg.ddlBudgetPeriod.getValue(),
                                   me.saveCallback, me);
    },
    saveCallback: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);

        var error = response.value.ErrMsg;
        if (error != null && !error.Success) {
            Ext.Msg.alert("Error creating request", error.Message);
            return false;
        }

        cfg.currentItem = response.value.Item;
        cfg.hasUpdated = true;
        me.close();
    },
    saveConfirmCallback: function (response) {
        var me = response.context, cfg = me.config;
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.currentItem = response.value.Item;

        if (cfg.currentItem == true) {
            Ext.Msg.show({
                title: "Overwrite existing data",
                msg: "Data for the selected period already exists. Do you wish to replace it?",
                buttons: Ext.Msg.YESNOCANCEL,
                fn: function (args) {
                    if (args.toUpperCase() == "YES") {
                        me.save();
                    }
                }
            });
        } else {
            me.save();
        }
    },
    cancel: function () {
        var me = this, cfg = me.config;
        me.hide();
    },
    getHasUpdated: function () {
        var me = this, cfg = me.config;
        return cfg.hasUpdated;
    },
    show: function (args) {
        var me = this, cfg = me.config, mdl, statusItems = [];
        args = $.extend(true, {
            item: null
        }, args);
        if (!cfg.budgetYearCollection) {
            Ext.MessageBox.alert("Error", "Budget Year Collection must be passed in the config.");
            return false;
        }
        if (!cfg.budgetPeriodCollection) {
            Ext.MessageBox.alert("Error", "Budget Period Collection must be passed in the config.");
            return false;
        }
        me.setValues();
        cfg.hasUpdated = false;
        me.resetFormVisibility();
        me.callParent();
    },
    resetFormVisibility: function () {
        var me = this, cfg = me.config, frm = cfg.frmGeneral.getForm();
        cfg.formSave.setDisabled(!frm.isValid());
    }

});