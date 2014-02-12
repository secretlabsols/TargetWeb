Ext.define('Results.Controls.ServiceDeliveryFileResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.ServiceDeliveryFileActions',
        'Searchers.Controls.ServiceDeliveryFileSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.ServiceDeliveryFileSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewPaymentSchedule();
        });
        cfg.newServiceDeliveryFileUrl = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Dom/ServiceDeliveryFile/UploadServiceDeliveryFile.aspx';
        cfg.viewServiceDeliveryFileUrl = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Dom/ServiceDeliveryFile/ViewServiceDeliveryFile.aspx';

        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.ServiceDeliveryFileActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onNewButtonClicked: {
                    fn: function () {
                        me.actionNewServiceDeliveryFile();
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.ServiceDeliveryFileSearcher', { resultSettings: me.resultSettings });
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');

        // call parents init
        this.callParent(arguments);
    },
    actionNewServiceDeliveryFile: function () {
        var me = this, cfg = me.config;
        var item = cfg.selectorControl.GetSelectedItem();
        var msgAccessDenied = 'You do not have permissions to upload a new Service Delivery File.';
        var msgLoading = 'Loading...';
        var url = me.getUrlForNewServiceDeliveryFile(item);

        if (!cfg.permissionCanAddNew) {
            Ext.Msg.alert('Access Denied', msgAccessDenied);
            return false;
        }

        cfg.selectorControl.ShowMask(false);
        Ext.getBody().mask(msgLoading);
        document.location.href = url;
    },
    actionViewServiceDeliveryFile: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item && item.ID > 0) {
            me.createContextMenu(item);
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(item));
            cfg.contextMenu.menuView.viewDefault();
        } else {
            Ext.Msg.alert('No Item to View', 'Please select an item to View.');
        }
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
                    cfg.contextMenu.menuView,
                    cfg.contextMenu.menuItemCopy,
                    cfg.reportsMenu
                ]
            });
        }
    },
    getViewMenuItems: function (item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForViewServiceDeliveryFile(item), key: 'ServiceDeliveryFile', text: 'View' });
        return viewMenuItems;
    },
    getUrlForNewServiceDeliveryFile: function (item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        if (cfg.permissionCanAddNew) {

            url = cfg.newServiceDeliveryFileUrl + "?backUrl=" + escape(me.getBackUrl());

        }
        return url;
    },
    getUrlForViewServiceDeliveryFile: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.viewServiceDeliveryFileUrl + '?id=' + item.ID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        } else {
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(selectedItem));
            cfg.contextMenu.menuItemCopy.setHref(me.getUrlForNewServiceDeliveryFile(selectedItem, true));
        }
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    GetBackUrl: function (item) {
        var backUrl = document.location.href;
        if (item) {
            backUrl = AddQSParam(RemoveQSParam(backUrl, 'psid'), 'psid', item.ID);
        }
        return escape(backUrl);
    }
});
