Ext.define('Results.Controls.GenericCreditorPaymentResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.GenericCreditorPaymentActions',
        'Searchers.Controls.GenericCreditorPaymentSearcher',
        'Target.ux.menu.ViewMenuItem',
        'Ext.menu.Item',
        'Actions.Buttons.FindWithSelectorButton'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // add service
        cfg.service = Target.Abacus.Library.CreditorPayments.Services.CreditorPaymentsService;
        cfg.FilterGenericCreditorPaymentID = 0;
        // add selector control
        cfg.selectorControl = new Selectors.GenericCreditorPaymentSelector({
            request: {
                PageSize: 0
            }
        });
        // adding find with slector control button 
        cfg.addfindWithSelector = me.setFindwithSelector();
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.GenericCreditorPaymentActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onAuthoriseManyClicked: {
                    fn: function () {
                        me.actionAuthoriseMany();
                    },
                    scope: me
                },
                onCreateBatchClicked: {
                    fn: function () {
                        me.actionCreateBatch();
                    },
                    scope: me
                },
                onNewButtonClicked: {
                    fn: function () {
                        me.actionNew();
                    },
                    scope: me
                },
                onReportsButtonClicked: {
                    fn: function (cmp) {
                        cmp.viewReports(me.mapResultsToReports());
                    },
                    scope: me
                },
                onSuspensionsManyClicked: {
                    fn: function (cmp) {
                        me.actionSuspensionsMany();
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.GenericCreditorPaymentSearcher', { resultSettings: me.resultSettings });
        // setup permissions
        me.setupPermissions();
        // handle any selector control events
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewCreditorPayment();
        });
        cfg.selectorControl.OnLoaded(function (args) {
            me.setupActionsPanel();
        });

        cfg.serviceOrderUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/svcOrders/Edit.aspx';
        // call parents init
        this.callParent(arguments);
    },
    actionAuthorise: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        Ext.MessageBox.confirm({
            title: 'Authorise Creditor Payment',
            msg: 'Are you sure you wish to authorise this payment of £' + item.Total.toFixed(2) + '?',
            buttons: Ext.MessageBox.YESNO,
            icon: Ext.MessageBox.INFO,
            fn: me.actionAuthoriseConfirm,
            scope: me
        });
    },
    actionAuthoriseConfirm: function (answer) {
        if (answer == 'yes') {
            var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
            Ext.getBody().mask('Authorising...');
            cfg.service.AuthoriseGenericCreditorPayment(item.ID, me.actionAuthoriseConfirmCallBack, me);
        }
    },
    actionAuthoriseConfirmCallBack: function (response) {
        var me = response.context, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        Ext.getBody().unmask();
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.selectorControl.Load();
    },
    actionAuthoriseMany: function () {
        var me = this, cfg = me.config, confirmTitle = 'Information', confirmText = 'Only \'Unpaid\' payments that are not excluded will be passed onto the authorisation process.',
        params = me.getGenericCreditorPaymentParameters();
        if (!cfg.authoriseControl) {
            cfg.authoriseControl = Ext.create('TargetExt.CreditorPayments.CreateAuthorisePaymentsJob', {});
        }
        if (cfg.selectorControl.HasAuthorisedPayments() || cfg.selectorControl.HasPaidPayments() || cfg.selectorControl.HasSuspendedPayments()) {
            me.confirmAction(confirmTitle, confirmText, Ext.Msg.OK, cfg.actionPanel.config.authoriseButton, function (answer) {
                if (answer === 'ok') {
                    cfg.authoriseControl.show(params);
                }
            });
        } else {
            cfg.authoriseControl.show(params);
        }
    },
    actionCreateBatch: function () {
        var me = this, cfg = me.config, confirmTitle = 'Information', confirmText = 'Only \'Authorised\' payments that are not excluded will be passed onto the batch creation process.',
        params = me.getGenericCreditorPaymentParameters();
        if (!cfg.createBatchControl) {
            cfg.createBatchControl = Ext.create('TargetExt.CreditorPayments.CreateBatchJob', {
                listeners: {
                    close: {
                        fn: function () {
                            cfg.selectorControl.Load();
                        },
                        scope: me
                    }
                }
            });
        }
        if (cfg.selectorControl.HasUnpaidPayments() || cfg.selectorControl.HasPaidPayments() || cfg.selectorControl.HasSuspendedPayments()) {
            me.confirmAction(confirmTitle, confirmText, Ext.Msg.OK, cfg.actionPanel.config.createBatchButton, function (answer) {
                if (answer === 'ok') {
                    cfg.createBatchControl.show(params);
                }
            });
        } else {
            cfg.createBatchControl.show(params);
        }
    },
    actionExcludeInclude: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        Ext.getBody().mask((item.ExcludeFromCreditors ? 'Including...' : 'Excluding...'));
        cfg.service.ToggleExcludeFromCreditorsFlag(item.ID, me.actionExcludeIncludeCallBack, me);
    },
    actionExcludeIncludeCallBack: function (response) {
        var me = response.context, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        Ext.getBody().unmask();
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        if (Ext.isEmpty(cfg.selectorControl.GetExcludeFromCreditors())) {
            item.ExcludeFromCreditors = !item.ExcludeFromCreditors;
            cfg.selectorControl.UpdateItem(item);
        } else {
            cfg.searchPanel.search();
        }
    },
    actionNew: function () {
        var me = this, cfg = me.config, msgLoading = 'Creating Creditor Payment...', url = me.getUrlForNew();
        cfg.selectorControl.ShowMask(false);
        Ext.getBody().mask(msgLoading);
        document.location.href = url;
    },
    actionNotes: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (!cfg.notesControl) {
            cfg.notesControl = Ext.create('TargetExt.ProviderInvoices.NotesEditor', {
                permissionCanAdd: cfg.permissionNotesAdd,
                permissionCanDelete: cfg.permissionNotesDelete,
                permissionCanEdit: cfg.permissionNotesEdit
            });
        }
        cfg.notesControl.show({ item: item });
    },
    actionSuspensions: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (!cfg.suspensionsControl) {
            cfg.suspensionsControl = Ext.create('TargetExt.CreditorPayments.SuspensionEditor', {
                permissionCanAddComment: cfg.permissionSuspensionsAddComments,
                permissionCanSuspend: cfg.permissionSuspensionsSuspend,
                permissionCanUnsuspend: cfg.permissionSuspensionsUnSuspend,
                listeners: {
                    close: {
                        fn: function (cmp) {
                            if (cmp.hasSuspended()) {
                                cfg.selectorControl.Load();
                            }
                        },
                        scope: me
                    }
                }
            });
        }
        cfg.suspensionsControl.show({ item: item });
    },
    actionSuspensionsMany: function () {
        var me = this, cfg = me.config, params = me.getGenericCreditorPaymentParameters();
        if (!cfg.suspensionsManyControl) {
            cfg.suspensionsManyControl = Ext.create('TargetExt.CreditorPayments.CreateSuspensionsJob', {
                permissionCanAddComment: cfg.permissionSuspensionsAddComments,
                permissionCanSuspend: cfg.permissionSuspensionsSuspend,
                permissionCanUnsuspend: cfg.permissionSuspensionsUnSuspend
            });
        }
        cfg.suspensionsManyControl.show({
            hasSuspended: (cfg.selectorControl.HasSuspendedPayments()),
            hasAuthorised: (cfg.selectorControl.HasAuthorisedPayments()),
            hasUnpaid: hasUnpaid = (cfg.selectorControl.HasUnpaidPayments()),
            params: params
        });
    },
    actionViewCreditorPayment: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item && item.ChildID > 0) {
            me.createContextMenu(item);
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(item));
            cfg.contextMenu.menuView.viewDefault();
        } else {
            Ext.Msg.alert('No Item to View', 'Please select an item to View.');
        }
    },
    confirmAction: function (title, msg, buttons, animateTarget, func) {
        var me = this;
        Ext.Msg.show({
            title: title,
            msg: msg,
            buttons: (buttons || Ext.Msg.YESNO),
            icon: Ext.Msg.INFO,
            animateTarget: animateTarget,
            fn: func,
            scope: me
        });
    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        cfg.contextMenu = {};
        cfg.contextMenu.menuExludeInclude = me.createBasicContextMenuItem('#', '', '', '', function () { me.actionExcludeInclude() });
        cfg.contextMenu.menuAuthorise = me.createBasicContextMenuItem('#', 'AuthoriseCreditorPayment', 'Authorise', 'Authorise Creditor Payment?', function () { me.actionAuthorise() });
        cfg.contextMenu.menuSuspensions = me.createBasicContextMenuItem('#', 'SuspendCreditorPayment', 'Suspensions', 'View Suspensions or Suspend this Creditor Payment?', function () { me.actionSuspensions() });
        cfg.contextMenu.menuNotes = me.createBasicContextMenuItem('#', 'NotesCreditorPayment', 'Notes', 'View or Add Notes to this Creditor Payment?', function () { me.actionNotes() });
        cfg.reportsMenuNonRes = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
            tooltip: 'View Reports for the Creditor Payment?',
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
        cfg.reportsMenuDirectPayments = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
            tooltip: 'View Reports for the Creditor Payment?',
            reportsButtonConfig: {
                reportIds: me.resultSettings.ReportIdsForContextMenuDirectPayments,
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
            items: [{
                text: 'View',
                menu: {
                    items: [
        	            me.getViewMenuItemForCreditorPayment(item),
        	            me.getViewMenuItemForCreditor(item),
        	            me.getViewMenuItemForBudgetHolder(item),
        	            me.getViewMenuItemForServiceUser(item),
        	            me.getViewMenuItemForContract(item),
        	            {
        	                text: 'Service Order',
        	                listeners: {
        	                    click: {
        	                        fn: function () {
        	                            me.viewServiceOrers();
        	                        }
        	                    }
        	                }
        	            }
                    ]
               }
            },
        	cfg.contextMenu.menuExludeInclude,
        	cfg.contextMenu.menuAuthorise,
        	cfg.contextMenu.menuSuspensions,
        	cfg.contextMenu.menuNotes,
        	cfg.reportsMenuNonRes,
        	cfg.reportsMenuDirectPayments,
            ]
        });
    },
    getGenericCreditorPaymentParameters: function () {
        var me = this, cfg = me.config;
        return cfg.selectorControl.GetWebServiceParameters();
    },
    getUrlForNew: function () {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), providerId = 0, contractId = 0;
        if (search.GenericCreditor.Item)
            providerId = search.GenericCreditor.Item.ChildID;
        if (search.GenericContract.Item)
            contractId = search.GenericContract.Item.ChildID;
        return SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/ProviderInvoices/Edit.aspx?&mode=2&gcd=' + search.GenericCreditorID + '&estabId=' + providerId + '&contractid=' + contractId + '&gcon=' + cfg.selectorControl.GetGenericContractID() + '&backUrl=' + escape(me.getBackUrl());
    },
    getViewMenuItems: function (item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push(me.getViewMenuItemForCreditorPayment(item));
        viewMenuItems.push(me.getViewMenuItemForCreditor(item));
        viewMenuItems.push(me.getViewMenuItemForBudgetHolder(item));
        viewMenuItems.push(me.getViewMenuItemForServiceUser(item));
        viewMenuItems.push(me.getViewMenuItemForContract(item));
        viewMenuItems.push(
            Ext.create('Ext.menu.Item', {
                text: 'ServiceOrder',
                key: 'ServiceOrder'
            })
        );

        return viewMenuItems;
    },
    getViewMenuItemForBudgetHolder: function (item) {
        var me = this, cfg = me.config, menuItem = {};
        menuItem.disabled = true;
        menuItem.key = 'BudgetHolder';
        menuItem.text = 'Budget Holder';
        menuItem.hidden = true;
        menuItem.href = '#';
        if (item) {
            if (item.Type === Selectors.GenericCreditorPaymentSelectorTypes.DirectPayment) {
                menuItem.href = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Sds/BudgetHolders.aspx?id=' + item.ThirdPartyBudgetHolderID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
                menuItem.disabled = !cfg.permissionBudgetHolders;
                menuItem.hidden = (item.ThirdPartyBudgetHolderID == 0);
            }
        }
        return menuItem;
    },
    getViewMenuItemForContract: function (item) {
        var me = this, cfg = me.config, menuItem = {};
        menuItem.disabled = true;
        menuItem.key = 'Contract';
        menuItem.text = 'Contract';
        menuItem.href = '#';
        if (item) {
            if (item.Type === Selectors.GenericCreditorPaymentSelectorTypes.NonResidential) {
                menuItem.href = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/Contracts/Edit.aspx?id=' + item.ChildContractID.toString() + '&backUrl=' + escape(me.getBackUrl());
                menuItem.disabled = !cfg.permissionProviderContracts;
            } else {
                menuItem.href = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Sds/DPContracts/Edit.aspx?id=' + item.ChildContractID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
                menuItem.disabled = !cfg.permissionDirectPaymentContracts;
            }
        }
        return menuItem;
    },
    getViewMenuItemForCreditor: function (item) {
        var me = this, cfg = me.config, menuItem = {};
        menuItem.key = 'Creditor';
        menuItem.text = 'Creditor';
        menuItem.href = '#';
        if (item) {
            menuItem.href = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/CreditorPayments/ViewCreditor.aspx?id=' + item.GenericCreditorID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return menuItem;
    },
    getViewMenuItemForServiceOrer: function (item) {
        //var me = this, cfg = me.config;
        //var menuItem = me.createBasicContextMenuItem('#', 'ServiceOrder', 'ServiceOrder', 'ServiceOrder', function () { me.viewServiceOrers() });
        var me = this, cfg = me.config;
        var me = this;
        var menuItem = Ext.create('Ext.menu.Item', {
            href: '#',
            cls: 'ActionPanels',
            text: 'Service Order(s)',
            tooltip: 'View Service Order(s)',
            key: 'ServiceOrder',
            disabled: !cfg.permissionServiceUsers,
            handler: function () {
                viewServiceOrers();
            }
        });
        return menuItem;
    },
    getViewMenuItemForCreditorPayment: function (item) {
        var me = this, cfg = me.config, menuItem = {};
        menuItem.disabled = true;
        menuItem.key = 'CreditorPayment';
        menuItem.text = 'Creditor Payment';
        menuItem.href = '#';
        if (item) {
            if (item.Type === Selectors.GenericCreditorPaymentSelectorTypes.NonResidential) {
                menuItem.href = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Dom/ProviderInvoices/Edit.aspx?id=' + item.ChildID.toString() + '&estabid=' + item.ProviderID + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
                menuItem.disabled = !cfg.permissionProviderInvoices;
            } else {
                menuItem.href = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Sds/DPPayment/View.aspx?id=' + item.ChildID.toString() + '&backUrl=' + escape(me.getBackUrl());
                menuItem.disabled = !cfg.permissionDirectPayments;
            }
        }
        return menuItem;
    },
    getViewMenuItemForServiceUser: function (item) {
        var me = this, cfg = me.config, menuItem = {};
        menuItem.disabled = true;
        menuItem.key = 'ServiceUser';
        menuItem.text = 'Service User';
        menuItem.disabled = !cfg.permissionServiceUsers;
        menuItem.href = '#';
        if (item) {
            menuItem.href = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx?clientid=' + item.ServiceUserID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return menuItem;
    },
    setupActionsPanel: function () {
        var me = this, cfg = me.config,
        hasUnpaid = cfg.selectorControl.HasUnpaidPayments(), hasAuthorised = cfg.selectorControl.HasAuthorisedPayments(),
        hasSuspended = cfg.selectorControl.HasSuspendedPayments(), hasPaid = cfg.selectorControl.HasPaidPayments(),
        authoriseButton = cfg.actionPanel.config.authoriseButton, createBatchButton = cfg.actionPanel.config.createBatchButton,
        newButton = cfg.actionPanel.config.addButton, suspensionsButton = cfg.actionPanel.config.suspensionsButton, canSuspend = true;
        if (authoriseButton) {
            authoriseButton.setDisabled(!(hasUnpaid && cfg.permissionAuthorise));
        }
        if (createBatchButton) {
            createBatchButton.setDisabled(!(hasAuthorised && cfg.permissionCreateBatch));
        }
        if (newButton) {
            newButton.setDisabled(!(cfg.permissionNew));
        }
        if (suspensionsButton) {
            if (!cfg.permissionSuspensions) {
                canSuspend = false;
            } else if (hasPaid) {
                canSuspend = false;
            } else if (!hasSuspended && !hasAuthorised && !hasUnpaid) {
                canSuspend = false;
            } else if (hasSuspended && (hasAuthorised || hasUnpaid)) {
                canSuspend = false;
            }
            suspensionsButton.setDisabled(!(canSuspend));
        }
    },
    setupAuthoriseMenuItem: function (item) {
        var me = this, cfg = me.config;
        cfg.contextMenu.menuAuthorise.setDisabled(!cfg.permissionAuthorise);
        cfg.contextMenu.menuAuthorise.setVisible((item.Status == 'Unpaid' && !item.ExcludeFromCreditors));
    },
    setupIncludeExcludeMenuItem: function (item) {
        var me = this, cfg = me.config, itemText = 'Exclude', itemTooltip = 'Exclude Creditor Payment?', itemCssClass = 'ExcludeCreditorPayment',
        canIncludeExclude = (item.IsInBatch || !cfg.permissionExcludeInclude);
        if (item.ExcludeFromCreditors) {
            itemText = 'Include';
            itemTooltip = 'Include Creditor Payment?';
            itemCssClass = 'IncludeCreditorPayment';
        }
        cfg.contextMenu.menuExludeInclude.setDisabled(canIncludeExclude);
        cfg.contextMenu.menuExludeInclude.setIconCls(itemCssClass);
        cfg.contextMenu.menuExludeInclude.setText(itemText);
        cfg.contextMenu.menuExludeInclude.setTooltip(itemTooltip);
    },
    setupNotesMenuItem: function (item) {
        var me = this, cfg = me.config;
        cfg.contextMenu.menuNotes.setDisabled(!cfg.permissionNotes);
        cfg.contextMenu.menuNotes.setVisible(item.Type == Selectors.GenericCreditorPaymentSelectorTypes.NonResidential);
    },
    setupPermissions: function () {
        var me = this, cfg = me.config;
        cfg.permissionBudgetHolders = me.resultSettings.HasPermission('BudgetHolders.View');
        cfg.permissionAuthorise = me.resultSettings.HasPermission('CreditorPayments.Authorise');
        cfg.permissionCreateBatch = me.resultSettings.HasPermission('CreditorPayments.CreateBatch');
        cfg.permissionNew = me.resultSettings.HasPermission('CreditorPayments.New');
        cfg.permissionExcludeInclude = me.resultSettings.HasPermission('CreditorPayments.ExcludeInclude');
        cfg.permissionNotes = me.resultSettings.HasPermission('CreditorPayments.Notes');
        cfg.permissionNotesAdd = me.resultSettings.HasPermission('CreditorPayments.Notes.Add');
        cfg.permissionNotesDelete = me.resultSettings.HasPermission('CreditorPayments.Notes.Delete');
        cfg.permissionNotesEdit = me.resultSettings.HasPermission('CreditorPayments.Notes.Edit');
        cfg.permissionSuspensions = me.resultSettings.HasPermission('CreditorPayments.Suspensions');
        cfg.permissionSuspensionsAddComments = me.resultSettings.HasPermission('CreditorPayments.Suspensions.AddComments');
        cfg.permissionSuspensionsSuspend = me.resultSettings.HasPermission('CreditorPayments.Suspensions.Suspend');
        cfg.permissionSuspensionsUnSuspend = me.resultSettings.HasPermission('CreditorPayments.Suspensions.UnSuspend');
        cfg.permissionDirectPaymentContracts = me.resultSettings.HasPermission('DirectPaymentContracts.View');
        cfg.permissionDirectPayments = me.resultSettings.HasPermission('DirectPayments.View');
        cfg.permissionProviderContracts = me.resultSettings.HasPermission('ProviderContracts.View');
        cfg.permissionProviderInvoices = me.resultSettings.HasPermission('ProviderInvoices.View');
        cfg.permissionServiceUsers = me.resultSettings.HasPermission('ServiceUsers.View');
        cfg.permissionServiceOrder = me.resultSettings.HasPermission('ServiceOrder.View');
    },
    setupReportsMenuItem: function (item) {
        var me = this, cfg = me.config;
        cfg.reportsMenuNonRes.setVisible(item.Type == Selectors.GenericCreditorPaymentSelectorTypes.NonResidential);
        cfg.reportsMenuDirectPayments.setVisible(item.Type == Selectors.GenericCreditorPaymentSelectorTypes.DirectPayment);
    },
    setupSuspensionsMenuItem: function (item) {
        var me = this, cfg = me.config;
        cfg.contextMenu.menuSuspensions.setDisabled(!cfg.permissionSuspensions);
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem();
        me.createContextMenu(selectedItem);
        me.setupAuthoriseMenuItem(selectedItem);
        me.setupIncludeExcludeMenuItem(selectedItem);
        me.setupNotesMenuItem(selectedItem);
        me.setupReportsMenuItem(selectedItem);
        me.setupSuspensionsMenuItem(selectedItem);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    viewServiceOrers: function () {
        var me = this;
        var cfg = me.config;
        var item = cfg.selectorControl.GetSelectedItem();
        cfg.service.ServiceOrderCountInPayment(item.ID, me.viewServiceOrersCallBack, me);
    },
    viewServiceOrersCallBack: function (response) {
        var me = response.context, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (response.value.Item.OrdersCount == 0) {
            Ext.Msg.alert('Information', 'No Service Order exists for this payment.');
        } else if (response.value.Item.OrdersCount == 1) {
            var url = me.getUrlForServiceOrder(response.value.Item.ServiceOrderId);
            document.location.href = url;
        } else if (response.value.Item.OrdersCount > 1) {
            me.setFindwithSelector();
            cfg.FilterGenericCreditorPaymentID = item.ID;
            cfg.addfindWithSelector.showSelectorWindow();
        }
    },
    getUrlForServiceOrder: function (svcOrderId) {
        var me = this, cfg = me.config, url = '#', search = cfg.searchPanel.getSearch();
        url = cfg.serviceOrderUrl;
        url = url + '?id=' + svcOrderId.toString() + '&mode=1&backUrl=' + escape(me.getBackUrl());
        return url;
    },
    setFindwithSelector: function () {
        var me = this, cfg = me.config;
        cfg.addfindWithSelector = Ext.create('Actions.Buttons.FindWithSelectorButton', {
            autoRegisterEventsControls: [me],
            getSelector: function () {
                // setting up the selector to open
                var selector = new Selectors.GenericServiceOrderSelector({});
                // set filter to show by user
                selector.SetGenericCreditorPaymentID(cfg.FilterGenericCreditorPaymentID);
                return selector;
            },
            // adding a listener selected item in selector
            listeners: {
                onFindWithSelectorItemSelected: {
                    fn: function (cmp, dso) {
                        var me = this;
                        var cfg = me.config;
                        var url = me.getUrlForServiceOrder(dso.ChildID);
                        document.location.href = url;
                    },
                    scope: me
                }
            }
        });
    }
});
