Ext.define('Results.Controls.ServiceRegisterResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.ServiceRegisterActions',
        'Searchers.Controls.ServiceRegisterSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.ServiceRegisterSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function(args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function(args) {
            me.actionViewServiceRegister();
        });
        cfg.svcRegisterUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Actuals/DayCare/Register.aspx';
        cfg.svcCreatRegisterUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Actuals/DayCare/CreateRegister.aspx';
        cfg.contractUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/Contracts/Edit.aspx';
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.ServiceRegisterActions', {
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
                        me.actionNewServiceRegister();
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.ServiceRegisterSearcher', { resultSettings: me.resultSettings });
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        // call parents init
        this.callParent(arguments);
    },
    actionNewServiceRegister: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem(), msgAccessDenied =
        'You do not have permissions to create a new Service Register.', msgLoading =
        'Creating Service Register...', url = me.getUrlForNewServiceRegister(item);

        cfg.selectorControl.ShowMask(false);
        Ext.getBody().mask(msgLoading);
        document.location.href = url;
    },
    actionViewServiceRegister: function() {
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
            cfg.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Reports for the Selected Service Register?',
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
                    cfg.contextMenu.menuItemCopy,
                    cfg.contextMenu.menuView,
                    cfg.reportsMenu
                ]
            });
        }
    },
    getViewMenuItems: function(item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForServiceRegister(item), key: 'Service Register', text: 'Service Register' });
        viewMenuItems.push({ href: me.getUrlForContract(item), key: 'Provider Contract', text: 'Provider Contract' });
        return viewMenuItems;
    },
    getUrlForContract: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.contractUrl + '?id=' + item.ContractID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForNewServiceRegister: function(item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        var wedate = this.getWeekEndingDate();
        if (cfg.permissionCanAddNew) {
            var selectedProviderID = search.ProviderID;
            var selectedContractID = 0;
            if (search.GenericContract.Item != null) {
                selectedContractID = search.GenericContract.Item.ChildID;
            }
            url = cfg.svcCreatRegisterUrl + '?estabid=' + selectedProviderID.toString() +
                    '&contractid=' + selectedContractID.toString() +
                    '&wedate=' + Date.strftime("%d/%m/%Y", wedate) +
                    '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForServiceRegister: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            url = cfg.svcRegisterUrl + '?id=' + item.ID.toString() + '&contractid=' + item.ContractID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
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
    },
    getWeekEndingDate: function() {
        var me = this, cfg = me.config;
        var weDate = me.resultSettings.GetSearchParameterValue('WeekEndingDate', null);
        weDate = Date.convertDateToUTC(weDate);
        return weDate;
    }
});
