Ext.define('Results.Controls.SpendPlanResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.SpendPlanActions',
        'Searchers.Controls.SpendPlanSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.SpendPlanSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function(args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function(args) {
            me.actionViewSpendPlan();
        });
        cfg.spendPlanUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/SpendPlans/Edit.aspx';
        cfg.svcUserUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx';
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.SpendPlanActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onReportsButtonClicked: {
                    fn: function(cmp) {
                        cmp.viewReports(me.mapResultsToReports());
                    },
                    scope: me
                },
                onNewButtonClicked: {
                    fn: function() {
                        me.actionNewSpendPlan();
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.SpendPlanSearcher', { resultSettings: me.resultSettings });
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        // call parents init
        this.callParent(arguments);
    },
    actionNewSpendPlan: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem(), msgAccessDenied = 'You do not have permissions to create a new Spend Plan.', msgLoading = 'Creating Spend Plan...', url = me.getUrlForNewSpendPlan(item);
        if (!cfg.permissionCanAddNew) {
            Ext.Msg.alert('Access Denied', msgAccessDenied);
            return false;
        }
        cfg.selectorControl.ShowMask(false);
        Ext.getBody().mask(msgLoading);
        document.location.href = url;
    },
    actionViewSpendPlan: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item) {
            me.createContextMenu(item);
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(item));
            cfg.contextMenu.menuView.viewDefault();
        } else {
            Ext.Msg.alert('No Item to View', 'Please select an item to View.');
        }
    },
    createContextMenu: function(item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuView = Ext.create('Target.ux.menu.ViewMenuItem', {
                viewItems: me.getViewMenuItems(item)
            });
            cfg.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Reports for the Selected Spend Plan?',
                reportsButtonConfig: {
                    reportIds: me.resultSettings.ReportIdsForContextMenu,
                    listeners: {
                        onReportsButtonClicked: {
                            fn: function(cmp) {
                                cmp.viewReports(me.mapResultsToReportsForContextMenu());
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
                    cfg.contextMenu.menuView,
                    cfg.reportsMenu
                ]
            });
        }
    },
    getViewMenuItems: function(item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForSpendPlan(item), key: 'SpendPlan', text: 'Spend Plan' });
        viewMenuItems.push({ href: me.getUrlForServiceUser(item), key: 'ServiceUser', text: 'Service User' });
        return viewMenuItems;
    },
    getUrlForNewSpendPlan: function(item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        if (cfg.permissionCanAddNew) {
            var selectedServiceUserID = search.ServiceUserID;
            var itemID = '?id=0';
            if (item) {
                itemID = '?id=' + item.ID.toString()
            }
            url = cfg.spendPlanUrl + itemID + '&clientid=' + selectedServiceUserID + '&mode=2' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForSpendPlan: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.spendPlanUrl + '?id=' + item.ID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForServiceUser: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.svcUserUrl + '?clientid=' + item.ServiceUserID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    showContextMenu: function(args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        } else {
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(selectedItem));
        }
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    }
});
