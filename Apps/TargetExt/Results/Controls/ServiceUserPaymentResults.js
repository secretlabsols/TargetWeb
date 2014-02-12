Ext.define('Results.Controls.ServiceUserPaymentResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.ServiceUserPaymentActions',
        'Searchers.Controls.ServiceUserPaymentSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;

        // add selector control
        cfg.selectorControl = new Selectors.ServiceUserPaymentSelector({
            request: {
                PageSize: 0
            }
        });

        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewServiceUserPayment();
        });

        cfg.viewStatementUrl = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Res/Payments/ViewStatement.aspx';
        // add can add new permission
        cfg.permissionCanViewPrint = me.resultSettings.HasPermission('CanViewPrint');
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.ServiceUserPaymentSearcher', { resultSettings: me.resultSettings });

        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.ServiceUserPaymentActions', {
            resultSettings: me.resultSettings
        });
        cfg.searchPanel.on('onSearch', function (cmp, search) {
            if (search) {
                if (search.ProviderID == 0) {
                    Ext.Msg.alert('Service User Payments', 'Please specify a Provider.');
                } else {
                    if (search.ServiceUserID == 0) {
                        Ext.Msg.alert('Service User Payments', 'Please specify a Service User.');
                    }
                }
            }
        }, me);
        // call parent's init
        this.callParent(arguments);
    },
    actionViewServiceUserPayment: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item && item.ID > 0) {
            window.open(me.getUrlForServiceUserPayment(item));
        } else {
            Ext.Msg.alert('No item to view', 'Please select an item to view.');
        }
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, item = cfg.selectorControl.GetSelectedItem();

        me.createContextMenu(item);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;

        cfg.contextMenu = {};
        cfg.contextMenu.menuView = me.createBasicContextMenuItem('#', 'View', 'View/Print Statement', 'Display and/or print this Statement?', function () { window.open(me.getUrlForServiceUserPayment(item)) });
        cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
            allowOtherMenus: true,
            defaults: {
                cls: 'ActionPanels'
            },
            items: [
                cfg.contextMenu.menuView
            ]
        });
        ctxMenu = cfg.contextMenu;

        return ctxMenu;
    },
    getUrlForServiceUserPayment: function (item) {
        var me = this, cfg = me.config, url = '#', search = cfg.searchPanel.getSearch();
        var dtFrom, dtTo;
        if (item) {
            if (search.DateFrom != null) {
                dtFrom = '&datefrom=' + search.DateFrom.format('dd/MM/yyyy');
            } else {
                dtFrom = '';
            }
            if (search.DateTo != null) {
                dtTo = '&dateto=' + search.DateTo.format('dd/MM/yyyy');
            } else {
                dtTo = '';
            }
            return cfg.viewStatementUrl + '?estabid=' + search.ProviderID.toString() + '&clientid=' + search.ServiceUserID.toString() + dtFrom + dtTo + '&currentstep=3';
        }
        return url;
    }

});
