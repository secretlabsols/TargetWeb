
Ext.define('Results.Controls.CareHomePaymentResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Searchers.Controls.CareHomePaymentSearcher',
        'Target.ux.menu.ViewMenuItem',
        'Actions.Buttons.ReportsButton',
        'TargetExt.Remittances.CareHomePaymentEditor',
        'Actions.Controls.CareHomePaymentAction'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.Remittances.Services.RemittancesService;
        // add selector control
        cfg.selectorControl = new Selectors.CareHomePaymentSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.actionPanel = Ext.create('Actions.Controls.CareHomePaymentAction', {
            resultSettings: me.resultSettings,
            listeners: {
                onApproveCareHomePaymentsButtonClicked: {
                    fn: function () {
                        me.actionConfirmApprove();
                    },
                    scope: me
                }
            }
        });
        cfg.selectorControl.OnLoaded(function (args) {
            me.setupActionsPanel();
        });
        me.setupPermissions();
        cfg.selectorControl.OnItemContextMenu(function (args) { me.showContextMenu(args); });
        cfg.selectorControl.OnItemDoubleClick(function (args) { me.actionView(args); });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.CareHomePaymentSearcher', { resultSettings: me.resultSettings });
        // call parents init
        this.callParent(arguments);
    },
    actionEdit: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (!cfg.editorControl) {
            cfg.editorControl = Ext.create('TargetExt.Remittances.CareHomePaymentEditor', {
                statusColl: cfg.searchPanel.resultSettings.GetSearchParameterValue('ProcessStatusColl', []),
                subStatusColl: cfg.searchPanel.resultSettings.GetSearchParameterValue('ProcessSubStatusColl', []),
                currentItem: item,                
                webSecurityUserID: me.resultSettings.WebSecurityUserID,
                listeners: {
                    close: {
                        fn: function (cmp) {
                            if (cmp.getHasUpdated()) {
                                cfg.selectorControl.Load();
                            }
                        },
                        scope: me
                    }
                }
            });
        }
        cfg.editorControl.show({ item: item });
    },
    actionView: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem(), ctxMenu, rptsButton;
        ctxMenu = me.createContextMenu(item);
        ctxMenu.reportsMenu.initControls();
        if (!cfg.isCtxRptsMenuIntd) {
            rptsButton = cfg.contextMenu.reportsMenu.config.reportsButton;
            rptsButton.initControls(false, function () {
                cfg.isCtxRptsMenuIntd = true;
                me.actionViewCallBack();
            });
        } else {
            me.actionViewCallBack();
        }
    },
    actionViewCallBack: function () {
        var me = this, cfg = me.config, rptParams, rptsButton = cfg.contextMenu.reportsMenu.config.reportsButton;
        rptParams = rptsButton.getReportParametersForDirectView(me.resultSettings.ReportIdsForContextMenu[0], '', 'intSelectedID', cfg.selectorControl.GetSelectedID());
        rptsButton.viewReport(rptParams);
    },
    actionConfirmApprove: function () {
        var me = this, cfg = me.config, confirmTitle = 'Information', confirmText = 'Only Care Home Payments with a Status of ‘Provisional Created’ will be approved';
        me.confirmAction(confirmTitle, confirmText, function (answer) {
            if (answer === 'ok') {
                me.actionApprove();
            }
        });
    },
    actionApprove: function () {
        var me = this, cfg = this.config, item = cfg.selectorControl.GetWebServiceParameters();
        cfg.service.ApproveCarehomePayments(item, me.actionCallBack, me);
    },
    actionCallBack: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuEdit = me.createBasicContextMenuItem('#', 'Edit', 'Edit', 'Edit this Payment?',
                function () {
                    me.actionEdit();
            });
            cfg.contextMenu.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Payment?',
                iconCls: 'ViewImage',
                text: 'View',
                reportsButtonConfig: {
                    reportIds: me.resultSettings.ReportIdsForContextMenu,
                    listeners: {
                        onReportsButtonClicked: {
                            fn: function (cmp) {
                                me.actionView();
                            },
                            scope: me
                        }
                    }
                }
            });
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                    cfg.contextMenu.reportsMenu,
                    cfg.contextMenu.menuEdit
                ]
            });

            ctxMenu = cfg.contextMenu;
        }
        return ctxMenu;
    },
    setupPermissions: function () {
        var me = this, cfg = me.config;
        cfg.permissionEdit = me.resultSettings.HasPermission('CanEdit');
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu,
        selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.menuEdit.setDisabled(!cfg.permissionEdit);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    confirmAction: function (title, msg, func) {
        var me = this;
        Ext.Msg.show({
            title: title,
            msg: msg,
            buttons: Ext.Msg.OK,
            icon: Ext.Msg.INFO,
            fn: func,
            scope: me
        });
    },
    setupActionsPanel: function (args) {
        var me = this, cfg = this.config, item = me.resultSettings.SearcherSettings.Item;
        cfg.actionPanel.config.approveButton.setDisabled(cfg.selectorControl.GetProvisionalCreatedCount() <= 0)
    }
});

