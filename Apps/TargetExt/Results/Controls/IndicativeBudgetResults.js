Ext.define('Results.Controls.IndicativeBudgetResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.IndicativeBudgetActions',
        'Searchers.Controls.IndicativeBudgetSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.IndicativeBudgetSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function(args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function(args) {
            me.actionViewIndicativeBudget();
        });
        cfg.indicativeBudgetUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Sds/IndicativeBudget.aspx';
        cfg.svcUserUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx';
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.IndicativeBudgetActions', {
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
                        me.actionNewIndicativeBudget();
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.IndicativeBudgetSearcher', { resultSettings: me.resultSettings });
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        // call parents init
        this.callParent(arguments);
    },
    actionNewIndicativeBudget: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem(), msgAccessDenied = 'You do not have permissions to create a new Indicative Budget.', msgLoading = 'Creating Indicative Budget...', url = me.getUrlForNewIndicativeBudget(item);
        if (!cfg.permissionCanAddNew) {
            Ext.Msg.alert('Access Denied', msgAccessDenied);
            return false;
        }
        cfg.selectorControl.ShowMask(false);
        Ext.getBody().mask(msgLoading);
        document.location.href = url;
    },
    actionViewIndicativeBudget: function() {
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
                tooltip: 'View Reports for the Selected Indicative Budget?',
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
        viewMenuItems.push({ href: me.getUrlForIndicativeBudget(item), key: 'IndicativeBudget', text: 'Indicative Budget' });
        viewMenuItems.push({ href: me.getUrlForServiceUser(item), key: 'ServiceUser', text: 'Service User' });
        return viewMenuItems;
    },
    getUrlForNewIndicativeBudget: function(item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        if (cfg.permissionCanAddNew) {
            var selectedServiceUserID = search.ServiceUserID;
            url = cfg.indicativeBudgetUrl + '?' + 'id=' + itemID.toString() + '&clientid=' + selectedServiceUserID + '&mode=2' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForIndicativeBudget: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.indicativeBudgetUrl + '?id=' + item.ID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
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
