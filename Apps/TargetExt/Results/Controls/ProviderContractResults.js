Ext.define('Results.Controls.ProviderContractResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.ProviderContractActions',
        'Actions.Buttons.FindWithSelectorButton',
        'Searchers.Controls.ProviderContractSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;

        // adding find with slector control button 
        cfg.addButton = Ext.create('Actions.Buttons.FindWithSelectorButton', {
            autoRegisterEventsControls: [me],
            getSelector: function() {
                // setting up the selector to open
                var selector = new Selectors.ServiceGroupSelector({});
                // set filter to show by user
                selector.SetFilterbyUser(true);
                return selector;
            },
            // adding a listener selected item in selector
            listeners: {
                onFindWithSelectorItemSelected: {
                    fn: function(cmp, svcGroup) {
                        var me = this;
                        var cfg = me.config;
                        var item = cfg.selectorControl.GetSelectedItem();
                        var url = me.getUrlForNewProviderContract(item, false, svcGroup.ID);
                        document.location.href = url;
                    },
                    scope: me
                }
            }
        });
        // add selector control
        cfg.selectorControl = new Selectors.GenericContractSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.SetTypes([Selectors.GenericContractSelectorTypes.NonResidential]);

        cfg.selectorControl.OnItemContextMenu(function(args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function(args) {
            me.actionViewProviderContract();
        });

        cfg.provContractUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/Contracts/Edit.aspx';
        cfg.provConTerminateUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/Contracts/Terminate.aspx';
        cfg.provConReinstateUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/Contracts/Reinstate.aspx';
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('CanAddNew');
        cfg.permissionCanCopy = me.resultSettings.HasPermission('CanCopy')
        cfg.permissionCanTerminate = me.resultSettings.HasPermission('CanTerminate'); ;
        cfg.permissionCanReinstate = me.resultSettings.HasPermission('CanReinstate'); ;
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.ProviderContractSearcher', { resultSettings: me.resultSettings });

        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.ProviderContractActions', {
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
                        me.actionNewProviderContract();
                    }
                }
            }
        });
        // call parents init
        this.callParent(arguments);
    },
    actionNewProviderContract: function() {
        var me = this;
        var cfg = me.config;
        var item = cfg.selectorControl.GetSelectedItem();
        var url = '#';
        var msgAccessDenied = 'You do not have permission to set up a contract for a Service Group..'
        var msgLoading = 'Creating Provider Contract...';

        var availableGroups = me.resultSettings.GetSearchParameterValue('AvailableGroups', null);
        var availableServiceGroupID = me.resultSettings.GetSearchParameterValue('AvailableServiceGroupID', null);

        if (availableGroups == 0) {
            Ext.Msg.alert('Access Denied', msgAccessDenied);
        } else if (availableGroups == 1 && availableServiceGroupID > 0) {
            url = me.getUrlForNewProviderContract(item, false, availableServiceGroupID);
            document.location.href = url;
        } else {
            cfg.addButton.showSelectorWindow();
        }
    },
    actionViewProviderContract: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item && item.ID > 0) {
            me.createContextMenu(item);
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(item));
            cfg.contextMenu.menuView.viewDefault();
        } else {
            Ext.Msg.alert('No Item to View', 'Please select an item to View.');
        }
    },
    showContextMenu: function(args) {
        var me = this;
        var cfg = me.config;
        var ctxMenu = cfg.contextMenu;
        var selectedItem = cfg.selectorControl.GetSelectedItem();
        var canTerminate = me.canTerminateItem(selectedItem);
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        } else {
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(selectedItem));

        }

        cfg.contextMenu.menuItemTerminate.setVisible(canTerminate);
        cfg.contextMenu.menuItemReinstate.setVisible(!canTerminate);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    createContextMenu: function(item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuItemCopy = Ext.create('Ext.menu.Item', {
                text: 'Copy',
                tooltip: 'Copy this Provider Contract?',
                href: me.getUrlForNewProviderContract(item, true),
                cls: 'ActionPanels',
                disabled: (!cfg.permissionCanCopy),
                iconCls: 'CopyImage'
            });
            cfg.contextMenu.menuView = Ext.create('Target.ux.menu.ViewMenuItem', {
                viewItems: me.getViewMenuItems(item)
            });
            cfg.contextMenu.menuItemTerminate = Ext.create('Ext.menu.Item', {
                text: 'Terminate',
                tooltip: 'Terminate this Provider Contract?',
                href: me.getUrlForTerminateContract(item),
                cls: 'ActionPanels',
                disabled: (!cfg.permissionCanTerminate),
                iconCls: 'TerminateContract'
            });
            cfg.contextMenu.menuItemReinstate = Ext.create('Ext.menu.Item', {
                text: 'Re-instate',
                tooltip: 'Reinstate this Provider Contract?',
                href: me.getUrlForReinstateContract(item),
                cls: 'ActionPanels',
                disabled: (!cfg.permissionCanReinstate),
                iconCls: 'ReinstateContract'
            });
            cfg.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Reports for the Selected Provider Contract?',
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
                        cfg.contextMenu.menuItemTerminate,
                        cfg.contextMenu.menuItemReinstate,
                        cfg.contextMenu.menuItemCopy,
                        cfg.reportsMenu
                    ]
            });
        }
    },
    getViewMenuItems: function(item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForProviderContract(item), key: 'ProviderContract', text: 'Provider Contract' });
        return viewMenuItems;
    },
    getUrlForTerminateContract: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (cfg.permissionCanTerminate && item) {
            return cfg.provConTerminateUrl + '?id=' + item.ChildID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForReinstateContract: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (cfg.permissionCanReinstate && item) {
            return cfg.provConReinstateUrl + '?id=' + item.ChildID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForNewProviderContract: function(item, isCopy, svcGroupID) {
        var me = this, cfg = me.config, url = '#', search = cfg.searchPanel.getSearch();
        if (cfg.permissionCanCopy && isCopy) {
            url = cfg.provContractUrl;
            url = url + '?copyFromID=' + item.ChildID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        else if (cfg.permissionCanAddNew && !isCopy) {
            var selectedProviderID = search.ProviderID;
            url = cfg.provContractUrl;
            url = url + '?estabid=' + selectedProviderID.toString() + '&svcGroupID=' + svcGroupID + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForProviderContract: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.provContractUrl + '?id=' + item.ChildID.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    canTerminateItem: function(item) {
        return !(item && item.DateTo);
    }

});
