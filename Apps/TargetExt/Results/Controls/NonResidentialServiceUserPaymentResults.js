Ext.define('Results.Controls.NonResidentialServiceUserPaymentResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.NonResidentialServiceUserPaymentActions',
        'Target.ux.menu.ViewMenuItem',
        'Searchers.Controls.NonResidentialServiceUserPaymentSearcher'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.isFirstLoad = true;
        // add service
        cfg.service = Target.Abacus.Library.DomProviderInvoice.Services.DomProviderInvoicesService;

        // add selector control
        cfg.selectorControl = new Selectors.NonResidentialServiceUserPaymentSelector({
            request: {
                PageSize: 0
            },
            showFilters: true
        });
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewNonResidentialServiceUserPayment();
        });
        cfg.selectorControl.OnLoaded(function (args) {
            me.actionOnSingleResultItem();
            cfg.isFirstLoad = false;
        });
        cfg.selectorControl.OnMruItemSelected(function (args) {
            me.actionViewNonResidentialServiceUserPayment();
        });
        cfg.nonResidentialServiceUserInvoiceLinesUrl = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Dom/ProviderInvoice/ViewInvoiceLines.aspx';
        cfg.nonResidentialServiceUserCostedVisitsUrl = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Dom/ProviderInvoice/ViewInvoiceCostedVisits.aspx';

        // Set any user-based permissions
        me.setupPermissions();

        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.NonResidentialServiceUserPaymentActions', {
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
        cfg.searchPanel = Ext.create('Searchers.Controls.NonResidentialServiceUserPaymentSearcher', { resultSettings: me.resultSettings });

        cfg.selectorControl.OnLoaded(function (args) {
            me.setupActionsPanel();
        });

        me.resizable = true;

        // call parents init
        this.callParent(arguments);
    },
    actionOnSingleResultItem: function () {
        var me = this, cfg = me.config, itemCount = cfg.selectorControl.GetItems().length;
        if (itemCount == 1 && cfg.isFirstLoad == false) {
            // Do nothing
        }
    },
    actionViewNonResidentialServiceUserPayment: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item && item.ID > 0) {
            me.createContextMenu(item);
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(item));
            cfg.contextMenu.menuView.viewDefault();
        } else {
            Ext.Msg.alert('No Item to View', 'Please select an item to View.');
        }
    },
    actionRetract: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        var serviceUserName = item.SUName;
        var serviceUserRef = item.SUReference;
        var invNumber = item.InvoiceNumber;
        var msgPrompt = "";

        msgPrompt = msgPrompt + "Upon confirmation, Abacus will attempt to mark the selected Provider Invoice ";
        msgPrompt = msgPrompt + "as retracted and create a contra copy of the Provider Invoice; ";
        msgPrompt = msgPrompt + "it is not possible to undo this action.\r\n\r\n";
        msgPrompt = msgPrompt + "Are you sure you wish to retract the Provider Invoice ";
        msgPrompt = msgPrompt + invNumber + " for " + serviceUserName + " (" + serviceUserRef + ") ?";

        Ext.MessageBox.confirm({
            title: 'Retract Invoice',
            msg: msgPrompt,
            buttons: Ext.MessageBox.YESNO,
            icon: Ext.MessageBox.INFO,
            fn: me.actionRetractConfirm,
            scope: me
        });
    },
    actionRetractConfirm: function (result) {
        if (result == "yes") {
            var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
            cfg.service.RetractInvoice(item.ID, item.PaymentScheduleID, me.actionRetractConfirmCallback, me);
            Ext.getBody().mask('Retracting...');
        }
    },
    actionRetractConfirmCallback: function (response) {
        var me = response.context, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        Ext.getBody().unmask();
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        Ext.Msg.alert('Retract Invoice', 'Invoice retracted successfully.');
        cfg.selectorControl.Load();
    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuView = Ext.create('Target.ux.menu.ViewMenuItem', {
                viewItems: me.getViewMenuItems(item)
            });
            cfg.contextMenu.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Reports for the Payment',
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
            cfg.contextMenu.menuItemRetract = me.createBasicContextMenuItem('#', 'RetractInvoice', 'Retract', 'Retract Invoice?', function () { me.actionRetract() });
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                    cfg.contextMenu.menuView,
                    cfg.contextMenu.reportsMenu,
                    cfg.contextMenu.menuItemRetract
                ]
            });
        }
    },
    getViewMenuItems: function (item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push(
            { href: me.getUrlForNonResidentialServiceUserInvoiceLines(item), key: 'NonResidentialServiceUserInvoiceLines', text: 'Invoice Lines', clickMessage: 'Opening Invoice Lines...', tooltip: 'View invoice lines?' }
        );
        viewMenuItems.push(
            {
                href: me.getUrlForNonResidentialServiceUserCostedVisits(item),
                key: 'NonResidentialServiceUserCostedVisits',
                text: 'Costed Visits',
                clickMessage: 'Opening Costed Visits...',
                tooltip: 'View costed visits',
                disabled: !item.InvoiceStyleID == 3
            }
        );
        return viewMenuItems;
    },
    getUrlForNonResidentialServiceUserInvoiceLines: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.nonResidentialServiceUserInvoiceLinesUrl + '?id=' + item.ID.toString() + '&psview=false&pScheduleId=0&estabID=' + item.ProviderID;
        }
        return url;
    },
    getUrlForNonResidentialServiceUserCostedVisits: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.nonResidentialServiceUserCostedVisitsUrl + '?id=' + item.ID.toString() + '&psview=false&pScheduleId=' + item.PaymentScheduleID;
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
        me.setRetractAvailability(selectedItem)
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    setRetractAvailability: function (invoice) {
        var me = this;
        var cfg = me.config;
        var response = cfg.service.IsRetractProviderInvoiceVisible(invoice.ID, me.setRetractAvailabilityCallback, me);
    },
    setRetractAvailabilityCallback: function (response) {
        var isAvailable = false;
        var me = response.context, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        Ext.getBody().unmask();

        if (response.value.ErrMsg.Success &&
            response.value.Visibility &&
            cfg.permissionCanRetract) {
            isAvailable = true;
        }

        cfg.contextMenu.menuItemRetract.disabled = !isAvailable;
    },
    setupActionsPanel: function () {
        var me = this, cfg = me.config;
    },
    setupPermissions: function () {
        var me = this;
        var cfg = me.config;
        cfg.permissionCanRetract = me.resultSettings.HasPermission('Action.CanRetract');
    }
});
