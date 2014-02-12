Ext.define('Results.Controls.BudgetHolderResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.BudgetHolderSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function(args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function(args) {
            me.actionViewServiceUser();
        });
        cfg.svcUserUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx';
        cfg.bpUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Admin/ServiceUsers/ServiceUserBudgetPeriods.aspx';
        // call parents init
        this.callParent(arguments);
    },
    actionViewServiceUser: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item && item.ID > 0) {
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
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                    cfg.contextMenu.menuView
                ]
            });
        }
    },
    getViewMenuItems: function(item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForBudgetPeriod(item), key: 'BudgetPeriod', text: 'Budget Period' });
        viewMenuItems.push({ href: me.getUrlForServiceUser(item), key: 'ServiceUser', text: 'Service User' });
        return viewMenuItems;
    },
    getUrlForServiceUser: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.svcUserUrl + '?clientid=' + item.ID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForBudgetPeriod: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.bpUrl + '?clientid=' + item.ID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
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
