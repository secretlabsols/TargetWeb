Ext.define('Results.Controls.MovementRequestResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Searchers.Controls.MovementRequestSearcher',
        'Actions.Buttons.ReportsButton',
        'TargetExt.MovementRequests.Editor'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.Movements.Services.MovementsService;
        cfg.selectorControl = new Selectors.MovementRequestSelector({
            request: {
                PageSize: 0
            }
        });
        me.setupPermissions();
        cfg.selectorControl.OnItemContextMenu(function (args) { me.showContextMenu(args); });
        cfg.selectorControl.OnItemDoubleClick(function (args) { me.actionView(); });
        cfg.searchPanel = Ext.create('Searchers.Controls.MovementRequestSearcher', { resultSettings: me.resultSettings });
        this.callParent(arguments);
    },
    actionEdit: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (!cfg.editorControl) {
            cfg.editorControl = Ext.create('TargetExt.MovementRequests.Editor', {
                referenceData: me.resultSettings.GetSearchParameterValue('ReferenceData', null),
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
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuEdit = me.createBasicContextMenuItem('#', 'Edit', 'Edit', 'Edit this Movement Request?', function () { me.actionEdit() });
            cfg.contextMenu.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View this Movement Request?',
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
        cfg.permissionEdit = me.resultSettings.HasPermission('MovementRequests.Edit');
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu,
        selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.menuEdit.setDisabled(!cfg.permissionEdit);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    }
});
