Ext.define('Results.Controls.GenericServiceOrderResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.GenericServiceOrderActions',
        'Searchers.Controls.GenericServiceOrderSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.GenericServiceOrderSelector({
            request: {
                PageSize: 0
            }
        });

        cfg.currentApplication = me.resultSettings.GetSearchParameterValue('CurrentApplication');

        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewServiceOrder();
        });
        cfg.svcOrderUrl_Intranet = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/svcOrders/Edit.aspx';
        cfg.svcUserUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx';
        cfg.contractUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/Contracts/Edit.aspx';
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.GenericServiceOrderActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onNewButtonClicked: {
                    fn: function () {
                        me.actionNewServiceOrder(false);
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
        cfg.searchPanel = Ext.create('Searchers.Controls.GenericServiceOrderSearcher', { resultSettings: me.resultSettings });
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');

        // call parents init
        this.callParent(arguments);
    },
    actionNewServiceOrder: function (isCopy) {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem(), msgAccessDenied = 'You do not have permissions to create a new Service Order.', msgLoading = 'Creating Service Order...', url = me.getUrlForNewServiceOrder(item, isCopy);
        if (!cfg.permissionCanAddNew) {
            if (isCopy) {
                msgAccessDenied = 'You do not have permissions to copy a Service Order.';
            }
            Ext.Msg.alert('Access Denied', msgAccessDenied);
            return false;
        }
        if (isCopy) {
            msgLoading = 'Copying Service Order...';
        }
        cfg.selectorControl.ShowMask(false);
        Ext.getBody().mask(msgLoading);
        document.location.href = url;
    },
    actionViewServiceOrder: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item && item.ChildID > 0) {
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
            cfg.contextMenu.menuItemCopy = Ext.create('Ext.menu.Item', {
                text: 'Copy',
                tooltip: 'Copy this Service Order?',
                href: me.getUrlForNewServiceOrder(item, true),
                cls: 'ActionPanels',
                disabled: (!cfg.permissionCanAddNew),
                iconCls: 'CopyImage'
            });

            if (cfg.currentApplication != 3) {
                cfg.contextMenu.menuView = Ext.create('Target.ux.menu.ViewMenuItem', {
                    viewItems: me.getViewMenuItems(item)
                });
            } else {
                cfg.contextMenu.menuItemViewDSOExtranet = me.createBasicContextMenuItem('#', 'ViewImage', 'Service Order', 'View Service Order?', function () { me.ShowServiceOrderExtranet(item.ID) });
            };

            cfg.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Reports for the Selected Service Order?',
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

            if (cfg.currentApplication != 3) {
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
            } else {
                cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                    allowOtherMenus: true,
                    defaults: {
                        cls: 'ActionPanels'
                    },
                    items: [
                    cfg.contextMenu.menuItemViewDSOExtranet,
                    cfg.reportsMenu
                ]
                });
            }

        }
    },
    getViewMenuItems: function (item) {
        var me = this, viewMenuItems = [], cfg = me.config;

        viewMenuItems.push({ href: me.getUrlForServiceOrder(item), key: 'ServiceOrder', text: 'Service Order' });
        viewMenuItems.push({ href: me.getUrlForContract(item), key: 'Contract', text: 'Provider Contract' });
        viewMenuItems.push({ href: me.getUrlForServiceUser(item), key: 'ServiceUser', text: 'Service User' });

        return viewMenuItems;
    },
    getUrlForContract: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.contractUrl + '?id=' + item.ProviderContractID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForNewServiceOrder: function (item, isCopy) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        if (cfg.permissionCanAddNew) {
            var selectedProviderID = search.ProviderID, selectedServiceUserID = search.ServiceUserID, selectedGenericContractID = search.GenericContractID;
            if (item && item.ChildID > 0 && isCopy) {
                itemID = item.ChildID;
            }
            url = cfg.svcOrderUrl_Intranet + '?' + ((isCopy) ? 'c' : '') + 'id=' + itemID.toString() + '&estabid=' + selectedProviderID + '&clientid=' + selectedServiceUserID + '&contractid=' + selectedGenericContractID + '&mode=2' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForServiceOrder: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.svcOrderUrl_Intranet + '?id=' + item.ChildID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
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
            if (cfg.currentApplication != 3) {
                cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(selectedItem));
                cfg.contextMenu.menuItemCopy.setHref(me.getUrlForNewServiceOrder(selectedItem, true));
            } else {

                cfg.contextMenu.menuItemViewDSOExtranet = me.createBasicContextMenuItem('#', 'ViewImage', 'Service Order', 'View Service Order?', function () { me.ShowServiceOrderExtranet(selectedItem.ID) });
                
                cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                    allowOtherMenus: true,
                    defaults: {
                        cls: 'ActionPanels'
                    },
                    items: [
                    //cfg.contextMenu.menuView,
                    cfg.contextMenu.menuItemViewDSOExtranet,
                    cfg.reportsMenu
                ]
                });
            };
            
        }
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    ShowServiceOrderExtranet: function (genericServiceOrderID) {
        //alert("Show dso")

        var dsoSvc = new Target.Abacus.Extranet.Apps.WebSvc.GenericServiceOrder_class();
        $currentDate = new Date();

        //$genericServiceOrderID = id.replace($trPerfix, '');
        var $response;
        var $initSettings = {
            genericServiceOrderID: genericServiceOrderID,
            dialogueDivID: 'dso_dialog',
            genericServiceOrderSvc: dsoSvc,
            showDocuments: true,
            filterEeekEndingDate: $currentDate,
            selectedTab: 0
        }

        //Show dialogue
        $(document).serviceOrderDialog($initSettings);
        $(document).serviceOrderDialog('show', $initSettings);

    }
});
