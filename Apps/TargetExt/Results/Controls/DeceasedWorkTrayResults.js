﻿Ext.define('Results.Controls.DeceasedWorkTrayResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Target.ux.menu.ViewMenuItem',
        'TargetExt.DeceasedWorktray.Editor',
        'Actions.Buttons.ReportsButton',
        'Searchers.Controls.DeceasedWorkTraySearcher',
        'Actions.Controls.DeceasedWorkTrayAction'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;

        cfg.service = Target.Abacus.Library.WorkTrayDeceased.Services.WorkTrayDeceased;
        // add selector control
        cfg.selectorControl = new Selectors.DeceasedWorkTraySelector({
            request: {
                PageSize: 0
            },
            showFilters: true
        });
        cfg.actionPanel = Ext.create('Actions.Controls.DeceasedWorkTrayAction', {
            resultSettings: me.resultSettings,
            listeners: {
                onEditAllExceptionsButtonClicked: {
                    fn: function () {
                        me.actionEditAll();
                    },
                    scope: me
                }
            }
        });
        cfg.searchPanel = Ext.create('Searchers.Controls.DeceasedWorkTraySearcher', { resultSettings: me.resultSettings });

        me.setupPermissions();
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) { me.actionViewEx(args); });

        cfg.svcUserUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx';

        this.callParent(arguments);
    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuView = Ext.create('Target.ux.menu.ViewMenuItem', {
                viewItems: me.getViewMenuItems(item)
            });
            cfg.contextMenu.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Case?',
                iconCls: 'ViewImage',
                text: 'View',
                reportsButtonConfig: {
                    reportIds: me.resultSettings.ReportIdsForContextMenu,
                    listeners: {
                        onReportsButtonClicked: {
                            fn: function (cmp) {
                                me.actionViewEx();
                            },
                            scope: me
                        }
                    }
                }
            });
            cfg.contextMenu.menuEdit = me.createBasicContextMenuItem('#', 'Edit', 'Edit', 'Edit Case?',
                function () {
                    me.actionEdit();
                });
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                     cfg.contextMenu.menuView,
                     cfg.contextMenu.reportsMenu,
                     cfg.contextMenu.menuEdit
                ]
            });

            ctxMenu = cfg.contextMenu;
        }

        return ctxMenu;
    },
    getViewMenuItems: function (item) {
        var me = this, cfg = me.config, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForServiceUser(item), key: 'ServiceUser', text: 'Service User', disabled: (!cfg.permissionServiceUser) });
        return viewMenuItems;
    },

    getUrlForServiceUser: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.svcUserUrl + '?clientid=' + item.ID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    setupPermissions: function () {
        var me = this, cfg = me.config;
        cfg.permissionEdit = me.resultSettings.HasPermission('CanEdit');
        cfg.permissionServiceUser = me.resultSettings.HasPermission('CanViewServiceUser');
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.menuEdit.setDisabled(!cfg.permissionEdit);
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
    actionViewEx: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem(), ctxMenu, rptsButton;
        ctxMenu = me.createContextMenu(item);
        ctxMenu.reportsMenu.initControls();
        if (!cfg.isCtxRptsMenuIntd) {
            rptsButton = cfg.contextMenu.reportsMenu.config.reportsButton;
            rptsButton.initControls(false, function () {
                cfg.isCtxRptsMenuIntd = true;
                me.actionViewExCallBack();
            });
        } else {
            me.actionViewExCallBack();
        }
    },
    actionViewExCallBack: function () {
        var me = this, cfg = me.config, rptParams, rptsButton = cfg.contextMenu.reportsMenu.config.reportsButton;
        rptParams = rptsButton.getReportParametersForDirectView(me.resultSettings.ReportIdsForContextMenu[0], '', 'intSelectedID', cfg.selectorControl.GetSelectedID());
        rptsButton.viewReport(rptParams);
    },
    actionEdit: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        cfg.editorControl = Ext.create('TargetExt.DeceasedWorktray.Editor', {
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
        cfg.editorControl.show({ item: item });
    },
    actionEditAll: function () {
        var me = this, cfg = me.config, params = cfg.selectorControl.GetWebServiceParameters();
        cfg.editorControl = Ext.create('TargetExt.DeceasedWorktray.MultiRecordEditor', {
            statusColl: cfg.searchPanel.resultSettings.GetSearchParameterValue('ProcessStatusColl', []),
            subStatusColl: cfg.searchPanel.resultSettings.GetSearchParameterValue('ProcessSubStatusColl', []),
            params: params,
            webSecurityUserID: me.resultSettings.WebSecurityUserID,
            listeners: {
                close: {
                    fn: function (cmp) {
                        if (cmp.getHasUpdated()) {
                            Ext.MessageBox.alert('Information', 'A job has been launched to process your ‘Edit All’ update(s)');
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
    }
});