Ext.define('Results.Controls.ResidentialOccupancyResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.ResidentialOccupancyActions',
        'Searchers.Controls.ResidentialOccupancySearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;

        // add selector control
        cfg.selectorControl = new Selectors.ResidentialOccupancySelector({
            request: {
                PageSize: 0
            }
        });

        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewServiceUser();
        });

        cfg.resOccupancyUrl = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Res/ViewServiceUser.aspx';
        // add can add new permission
        cfg.permissionCanViewSU = me.resultSettings.HasPermission('CanViewSU');
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.ResidentialOccupancySearcher', { resultSettings: me.resultSettings });

        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.ResidentialOccupancyActions', {
            resultSettings: me.resultSettings
        });
        cfg.searchPanel.on('onSearch', function (cmp, search) {
            if (search) {
                if (search.ProviderID == 0) {
                    Ext.Msg.alert('Residential Occupancy', 'Please specify a Provider.');
                }
            }
        }, me);
        // call parent's init
        this.callParent(arguments);
    },
    actionViewServiceUser: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item) {
            me.createContextMenu(item);
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(item));
            cfg.contextMenu.menuView.viewDefault();
        } else {
            Ext.Msg.alert('No Item to View', 'Please select an item to View.');
        }
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        } else {
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(selectedItem));
        }
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    createContextMenu: function (item) {
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
    getUrlForResidentialOccupancy: function (item) {
        var me = this, cfg = me.config, url = '#';

        if (item) {
            return cfg.resOccupancyUrl + '?id=' + item.ID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getViewMenuItems: function(item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForResidentialOccupancy(item), key: 'ServiceUser', text: 'Service User' });
        return viewMenuItems;
    }

});
