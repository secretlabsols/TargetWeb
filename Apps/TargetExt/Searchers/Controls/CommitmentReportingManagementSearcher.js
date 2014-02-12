Ext.define("Searchers.Controls.CommitmentReportingManagementSearcher", {
    extend: "Searchers.SearcherControl",
    requires: [
        "Target.ux.form.field.ClearButton",
        "Target.ux.menu.ViewMenuItem",
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = "CommitmentReportingManagementSearchItem";
        Ext.define("CommitmentReportingManagementSearchItem", {
            extend: "Ext.data.Model",
            fields: [
                { name: "BudgetYearId", mapping: "BudgetYearId" },
                { name: "BudgetPeriodId", mapping: "BudgetPeriodId" },
                { name: "Period", mapping: "Period" },
                { name: "ReportStatusId", mapping: "ReportStatusId" }
            ],
            idProperty: "ID"
        });
        // setup forms
        me.setupFrmGeneral(me.resultSettings);
        // add forms to panel
        me.items = [
            cfg.frmGeneral
        ];
        // load forms with data from model
        me.setSearch(me.resultSettings.SearcherSettings.Item);
        // call parent
        me.callParent(arguments);
    },
    getSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm(), searchRecord, returnRecord;
        if (frmGeneral.isValid()) {
            searchRecord = frmGeneral.getRecord();
            frmGeneral.updateRecord(searchRecord);
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {
                BudgetYearId: searchRecord.data.BudgetYearId,
                BudgetPeriodId: searchRecord.data.BudgetPeriodId,
                Period: searchRecord.data.Period == "" ? null : searchRecord.data.Period,
                ReportStatusId: searchRecord.data.ReportStatusId
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var me = this, cfg = me.config, frmGeneral = cfg.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord.data.BudgetYearId = null;
        searchRecord.data.BudgetPeriodId = null;
        searchRecord.data.Period = null;
        searchRecord.data.ReportStatusId = null;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            // Budget year 
            var budgetYearCollection = args.GetSearchParameterValue('BudgetYearsCollection', []); // get budget year collection 
            cfg.budgetYearData = Ext.create("Ext.data.Store", {
                fields: ["ID", "Description", "IsFinancialYear"],
                data: budgetYearCollection,
                remoteSort: false
            });
            cfg.budgetYearData.sort("Description", "ASC");

            // Budget period
            var budgetPeriodCollection = args.GetSearchParameterValue("BudgetPeriodsCollection", []); // get budget period collection
            cfg.budgetPeriodData = Ext.create('Ext.data.Store', {
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
                data: budgetPeriodCollection,
                remoteSort: false
            });
            cfg.budgetPeriodData.sort('BudgetYearID, DateFrom', 'ASC');

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
                    }
                },
                plugins: ['clearbutton']
            });
            cfg.ddlBudgetPeriod = Ext.create('Ext.form.ComboBox', {
                fieldLabel: "Budget Period",
                store: cfg.budgetPeriodData,
                multiSelect: false,
                queryMode: "local",
                displayField: "Description",
                valueField: "ID",
                labelWidth: 100,
                anchor: "100%",
                name: "BudgetPeriodId",
                plugins: ['clearbutton']
            });

            // Report status
            cfg.reportStatusData = Ext.create("Ext.data.Store", {
                fields: ["ID", "Description"],
                data: [
                        { "ID": "", "Description": "ALL" },
                        { "ID": 0, "Description": "Queued" },
                        { "ID": 1, "Description": "Processing" },
                        { "ID": 2, "Description": "Processed" },
                        { "ID": -1, "Description": "Error" },
                        { "ID": 3, "Description": "Cancelled" },
                        { "ID": 4, "Description": "Expired / purged" }
                      ]
            });

            cfg.ddlReportStatusId = Ext.create("Ext.form.ComboBox", {
                fieldLabel: "Status",
                store: cfg.reportStatusData,
                displayField: "Description",
                valueField: "ID",
                queryMode: "local",
                labelWidth: 100,
                anchor: "100%",
                name: "ReportStatusId",
                multiSelect: false,
                plugins: ['clearbutton'],
                defaultValue: 2,
                listeners: {
                    afterrender: function () {
                        this.setValue(this.defaultValue);
                    }
                }
            });

            // Period type
            cfg.periodTypeData = Ext.create("Ext.data.Store", {
                fields: ["Frequency", "Description"],
                data: [
                        { "Frequency": "", "Description": "ALL" },
                        { "Frequency": "WK", "Description": "Weekly" },
                        { "Frequency": "MM", "Description": "Monthly" },
                        { "Frequency": "BY", "Description": "Budget Period" }
                      ]
            });

            cfg.ddlPeriodType = Ext.create("Ext.form.ComboBox", {
                fieldLabel: "Frequency",
                store: cfg.periodTypeData,
                displayField: "Description",
                valueField: "Frequency",
                queryMode: "local",
                labelWidth: 100,
                anchor: "100%",
                name: "Period",
                multiSelect: false,
                plugins: ['clearbutton'],
                defaultValue: "",
                listeners: {
                    afterrender: function () {
                        this.setValue(this.defaultValue);
                    }
                }
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create("Ext.form.Panel", {
                title: "General",
                items: [
                    cfg.ddlBudgetYear,
                    cfg.ddlBudgetPeriod,
                    cfg.ddlPeriodType,
                    cfg.ddlReportStatusId
                ],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener("change", function () {
                                var meagain = me;
                                setTimeout(function () { meagain.raiseOnSearchChanged(); }, 10);
                            }, me);
                        }
                    }
                }
            });

            cfg.ddlReportStatusId.setValue("0");
        }
    },
    setSearch: function (item) {
        var me = this, cfg = me.config, mdl = Ext.create(cfg.modelName, item);
        cfg.frmGeneral.loadRecord(mdl);
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
    }
});


