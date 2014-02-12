Ext.define('Results.Controls.InvoicedVisitResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.InvoicedVisitActions',
        'Searchers.Controls.InvoicedVisitSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;

        //Get current application 
        cfg.currentApplication = me.resultSettings.GetSearchParameterValue('CurrentApplication');

        
        // add selector control
        cfg.selectorControl = new Selectors.InvoicedVisitSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewInvoicedVisit();
        });
        cfg.invoicedVisitUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/ProviderInvoices/ViewInvoiceVisitDetails.aspx';
        cfg.invoicedVisitUrlExtranet = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Dom/ProviderInvoice/ViewInvoicedVisit.aspx';
        cfg.svcUserUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx';
        cfg.contractUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/Contracts/Edit.aspx';

        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.InvoicedVisitActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onNewButtonClicked: {
                    fn: function () {
                        alert('New button not available.');
                    },
                    scope: me
                },
                onReportsButtonClicked: {
                    fn: function (cmp) {
                        cmp.viewReports(me.mapResultsToReports());
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.InvoicedVisitSearcher', { resultSettings: me.resultSettings });


        // add can add new permission
        cfg.permissionCanAddNew = false;
        // call parents init
        this.callParent(arguments);
    },
    actionViewInvoicedVisit: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item) {
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
            cfg.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Reports for Invoiced Visits?',
                reportsButtonConfig: {
                    reportIds: me.resultSettings.ReportIdsForContextMenu,
                    listeners: {
                        onReportsButtonClicked: {
                            fn: function (cmp) {
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
    getViewMenuItems: function (item) {
        var me = this, cfg = me.config, viewMenuItems = [];
        if (cfg.currentApplication != 3) {
            viewMenuItems.push({ href: me.getUrlForInvoicedVisit(item), key: 'InvoicedVisit', text: 'Invoiced Visit' });
            viewMenuItems.push({ href: me.getUrlForServiceUser(item), key: 'ServiceUser', text: 'Service User' });
            viewMenuItems.push({ href: me.getUrlForContract(item), key: 'Contract', text: 'Provider Contract' });
        } else {
            viewMenuItems.push({ href: me.getUrlForInvoicedVisit(item), key: 'InvoicedVisit', text: 'View/Amend Invoiced Visit' });
        }
        return viewMenuItems;
    },
    getUrlForContract: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.contractUrl + '?id=' + item.DomContractID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForInvoicedVisit: function (item) {
        var me = this, cfg = me.config, url = '#', webSvcResponse;
        if (item) {
            if (cfg.currentApplication != 3) {
                url = cfg.invoicedVisitUrl + '?id=' + item.ID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
            } else {
                url = cfg.invoicedVisitUrlExtranet + '?id=' + item.ID.toString() + '&backUrl=' + escape(me.getBackUrl());
            }
        }


        return url;
    },
    getUrlForServiceUser: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.svcUserUrl + '?clientid=' + item.ServiceUserID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
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
    }
});
