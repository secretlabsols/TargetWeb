Ext.define('Results.Controls.VisitAmendmentRequestResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.VisitAmendmentRequestActions',
        'Searchers.Controls.VisitAmendmentRequestSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.VisitAmendmentRequestSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewVisitAmendment();
        });
        cfg.visitAmendmentRequestUrl = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Dom/ProviderInvoice/ViewInvoicedVisit.aspx?sencha=1';

        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.VisitAmendmentRequestActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onReportsButtonClicked: {
                    fn: function (cmp) {
                        cmp.viewReports(me.mapResultsToReports());
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.VisitAmendmentRequestSearcher', { resultSettings: me.resultSettings });
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');

        // call parents init
        this.callParent(arguments);
    },
    actionViewVisitAmendment: function () {
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
        viewMenuItems.push({ href: me.getUrlVisitamendment(item), key: 'VisitAmendmentRequest', text: 'View' });
        return viewMenuItems;
    },
    getUrlVisitamendment: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.visitAmendmentRequestUrl + '&id=' + item.ID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
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
    GetBackUrl: function (item) {
        var backUrl = document.location.href;
        if (item) {
            backUrl = AddQSParam(RemoveQSParam(backUrl, 'psid'), 'psid', item.ID);
        }
        return escape(backUrl);
    }
});
