Ext.define("Results.Controls.CommitmentReportingManagementResults", {
    extend: "Results.ResultControl",
    requires: [
        "Target.ux.menu.ViewMenuItem",
        "TargetExt.CommitmentReportingManagement.Editor",
        "Actions.Buttons.ReportsButton",
        "Searchers.Controls.CommitmentReportingManagementSearcher"
    ],
    initComponent: function () {
        var me = this, cfg = me.config;

        cfg.service = Target.Abacus.Library.WorkTrayCommitmentReportingManagement.Services.WorkTrayCommitmentReportingManagement;
        // add selector control
        cfg.selectorControl = new Selectors.CommitmentReportingManagementSelector({
            request: {
                PageSize: 0
            },
            showFilters: true
        });
        cfg.actionPanel = Ext.create("Actions.Controls.CommitmentReportingManagementAction", {
            resultSettings: me.resultSettings,
            listeners: {
                onNewButtonClicked: {
                    fn: function () {
                        me.actionAddNew();
                    }
                }
            }
        });

        cfg.searchPanel = Ext.create("Searchers.Controls.CommitmentReportingManagementSearcher", { resultSettings: me.resultSettings });

        me.setupPermissions();
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) { me.actionViewEx(args); });

        this.callParent(arguments);
    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu; selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuItemCancel = Ext.create("Ext.menu.Item", {
                text: "Cancel",
                tooltip: "Cancel queued report request",
                cls: "ActionPanels",
                iconCls: "TerminateContract",
                listeners: {
                    click: {
                        fn: function () {
                            me.actionCancel();
                        }
                    }
                }
            });
            cfg.contextMenu.menuItemExpire = Ext.create("Ext.menu.Item", {
                text: "Expire",
                tooltip: "Expire old reports",
                cls: "ActionPanels",
                iconCls: "TerminateContract",
                listeners: {
                    click: {
                        fn: function () {
                            me.actionExpire();
                        }
                    }
                }
            });
            cfg.contextMenu.menuItemMarkForRetention = Ext.create("Ext.menu.Item", {
                text: "Mark for Retention",
                tooltip: "Mark report data for retention",
                cls: "ActionPanels",
                iconCls: "ReinstateContract",
                listeners: {
                    click: {
                        fn: function () {
                            me.actionMarkForRetention();
                        }
                    }
                }
            });
            cfg.contextMenu.reportsMenu = Ext.create("Actions.Buttons.ReportsButtonMenuItem", {
                tooltip: "View reports for the selected period",
                reportsButtonConfig: {
                    reportIds: me.resultSettings.ReportIdsForContextMenu,
                    listeners: {
                        onReportsButtonClicked: {
                            fn: function (cmp) {
                                cmp.viewReports(me.mapResultsToReportsForContextMenu());
                            },
                            scope: me
                        }
                    }
                }
            });
            cfg.contextMenu.menu = Ext.create("Ext.menu.Menu", {
                allowOtherMenus: true,
                defaults: {
                    cls: "ActionPanels"
                },
                items: [
                    cfg.contextMenu.reportsMenu,
                    cfg.contextMenu.menuItemCancel,
                    cfg.contextMenu.menuItemExpire,
                    cfg.contextMenu.menuItemMarkForRetention
                ]
            });

            ctxMenu = cfg.contextMenu;
        }

        return ctxMenu;
    },
    setupPermissions: function () {
        var me = this, cfg = me.config;
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.reportsMenu.setDisabled(selectedItem.Status.toUpperCase() != "PROCESSED");
        cfg.contextMenu.menuItemCancel.setDisabled(selectedItem.Status.toUpperCase() != "QUEUED");
        cfg.contextMenu.menuItemExpire.setDisabled(selectedItem.Status.toUpperCase() != "PROCESSED" || selectedItem.IsLockedForRetention);
        cfg.contextMenu.menuItemMarkForRetention.setDisabled(selectedItem.Status.toUpperCase() != "PROCESSED" || selectedItem.IsLockedForRetention);

        // Ensure we only display the relevant reports
        if (selectedItem.Period.toUpperCase() == "WEEKLY") {
            var reportIds = cfg.searchPanel.resultSettings.GetSearchParameterValue("WeeklyReportCollection", []);
            cfg.contextMenu.reportsMenu.config.reportsButtonConfig.isInited = false;
            cfg.contextMenu.reportsMenu.config.reportsButtonConfig.reportIds = reportIds;
        }
        else if (selectedItem.Period.toUpperCase() == "MONTHLY") {
            var reportIds = cfg.searchPanel.resultSettings.GetSearchParameterValue("MonthlyReportCollection", []);
            cfg.contextMenu.reportsMenu.config.reportsButtonConfig.isInited = false;
            cfg.contextMenu.reportsMenu.config.reportsButtonConfig.reportIds = reportIds;
        }
        else if (selectedItem.Period.toUpperCase() == "BUDGET PERIOD") {
            var reportIds = cfg.searchPanel.resultSettings.GetSearchParameterValue("PeriodReportCollection", []);
            cfg.contextMenu.reportsMenu.config.reportsButtonConfig.isInited = false;
            cfg.contextMenu.reportsMenu.config.reportsButtonConfig.reportIds = reportIds;
        }


        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    getValueInt: function (item) {
        if (item.Type == 0)
            return item.Value;
        else
            return null;
    },
    getValueString: function (item) {
        if (item.Type == 1)
            return item.Value;
        else
            return null;
    },
    actionAddNew: function () {
        var me = this, cfg = me.config, params = cfg.selectorControl.GetWebServiceParameters();
        cfg.editorControl = Ext.create("TargetExt.CommitmentReportingManagement.Editor", {
            budgetYearCollection: cfg.searchPanel.resultSettings.GetSearchParameterValue("BudgetYearsCollection", []),
            budgetPeriodCollection: cfg.searchPanel.resultSettings.GetSearchParameterValue("BudgetPeriodsCollection", []),
            params: params,
            listeners: {
                close: {
                    fn: function (cmp) {
                        if (cmp.getHasUpdated()) {
                            Ext.MessageBox.alert("Information", "A job has been launched to process your new report request(s).<br/><br/>" +
                                                         "Please note that due to the length of time taken to process this data, results will only be available the next working day.")
                            cfg.selectorControl.Load();
                        }
                    },
                    scope: me
                }
            }
        });
        cfg.editorControl.show({
            params: params
        });
    },
    actionCancel: function () {
        var me = this, cfg = me.config;
        Ext.Msg.show({
            title: "Cancel outstanding request",
            msg: "Are you sure you wish to cancel the outstanding request?",
            buttons: Ext.Msg.YESNOCANCEL,
            fn: function (args) {
                if (args.toUpperCase() == "YES") {
                    me.actionCancelConfirm();
                }
            }
        });
    },
    actionExpire: function () {
        var me = this, cfg = me.config;
        Ext.Msg.show({
            title: "Expire report",
            msg: "Are you sure you wish to mark the report as expired? All data will be erased.",
            buttons: Ext.Msg.YESNOCANCEL,
            fn: function (args) {
                if (args.toUpperCase() == "YES") {
                    me.actionExpireConfirm();
                }
            }
        });
    },
    actionMarkForRetention: function () {
        var me = this, cfg = me.config;
        Ext.Msg.show({
            title: "Mark report for retention",
            msg: "Are you sure you wish to mark this report for retention? The data for this report will not be automatically expired.",
            buttons: Ext.Msg.YESNOCANCEL,
            fn: function (args) {
                if (args.toUpperCase() == "YES") {
                    me.actionMarkForRetentionConfirm();
                }
            }
        });
    },
    actionCancelConfirm: function () {
        var me = this, cfg = me.config, selectedItem = cfg.selectorControl.GetSelectedItem();
        me.setLoading("Cancelling report request...");

        cfg.service.CancelRequest(selectedItem.CommitmentReportId, me.cancelCallback, me);
    },
    actionExpireConfirm: function () {
        var me = this, cfg = me.config, selectedItem = cfg.selectorControl.GetSelectedItem();
        me.setLoading("Expiring report...");

        cfg.service.ExpireReport(selectedItem.CommitmentReportId, me.expireCallback, me);
    },
    actionMarkForRetentionConfirm: function () {
        var me = this, cfg = me.config, selectedItem = cfg.selectorControl.GetSelectedItem();
        me.setLoading("Marking report for retention...");

        cfg.service.MarkReportForRetention(selectedItem.CommitmentReportId, me.markForRetentionCallback, me);
    },
    cancelCallback: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);

        var error = response.value.ErrMsg;
        if (error != null && !error.Success) {
            Ext.Msg.alert("Error cancelling request", error.Message);
            return false;
        }

        Ext.Msg.alert("Request cancelled", "The outstanding request has been cancelled successfully.");
        cfg.selectorControl.Load();
    },
    expireCallback: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);

        var error = response.value.ErrMsg;
        if (error != null && !error.Success) {
            Ext.Msg.alert("Error expiring report", error.Message);
            return false;
        }

        Ext.Msg.alert("Report expired", "The report was successfully marked as expired. All data will be erased.");
        cfg.selectorControl.Load();
    },
    markForRetentionCallback: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);

        var error = response.value.ErrMsg;
        if (error != null && !error.Success) {
            Ext.Msg.alert("Error marking report for retention", error.Message);
            return false;
        }

        Ext.Msg.alert("Report marked for retention", "The report was successfully marked for retention.");
        cfg.selectorControl.Load();
    }
});
