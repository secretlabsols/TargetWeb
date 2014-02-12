Ext.define('Results.Controls.DebtorInvoiceResults', {
    extend: 'Results.ResultControl',
    requires: [
        'TargetExt.DebtorInvoices.CreateDebtorInvoiceBatch',
        'Actions.Controls.DebtorInvoiceActions',
        'Searchers.Controls.DebtorInvoiceSearcher',
        'Target.ux.menu.ViewMenuItem',
        'Actions.Buttons.ReportsButton'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.DebtorInvoiceSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.openReport();
        });
        Ext.define('DebtorInvoiceSearchCriteriaItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'ID', mapping: 'ID', type: 'number' },
                { name: 'ClientID', mapping: 'ClientID', type: 'number' },
                { name: 'Res', mapping: 'Res' },
                { name: 'Dom', mapping: 'Dom' },
                { name: 'Client', mapping: 'Client' },
                { name: 'TP', mapping: 'TP' },
                { name: 'Prop', mapping: 'Prop' },
                { name: 'OLA', mapping: 'OLA' },
                { name: 'PenColl', mapping: 'PenColl' },
                { name: 'HomeColl', mapping: 'HomeColl' },
                { name: 'Std', mapping: 'Std' },
                { name: 'Manual', mapping: 'Manual' },
                { name: 'SDS', mapping: 'SDS' },
                { name: 'Actual', mapping: 'Actual' },
                { name: 'Provisional', mapping: 'Provisional' },
                { name: 'Retracted', mapping: 'Retracted' },
                { name: 'Retract', mapping: 'Retract' },
                { name: 'ZeroValue', mapping: 'ZeroValue' },
                { name: 'DateFrom', mapping: 'DateFrom' },
                { name: 'DateTo', mapping: 'DateTo' },
                { name: 'BatchSel', mapping: 'BatchSel' },
                { name: 'Exclude', mapping: 'Exclude' },
                { name: 'DebtorFilter', mapping: 'DebtorFilter' },
                { name: 'InvoiceNumFilter', mapping: 'InvoiceNumFilter' },
                { name: 'ClientRefFilter', mapping: 'ClientRefFilter' }
            ],
            idProperty: 'ID'
        });
        cfg.webSvcProxy = Target.Abacus.Web.Apps.WebSvc.DebtorInvoice;
        cfg.svcUserUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx';
        cfg.documentUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Documents/UserControls/Documents/DocumentDownloadHandler.axd?saveas=1&id=';
        cfg.debtorInvoiceUrl = SITE_VIRTUAL_ROOT + "Apps/Reports/Viewer.aspx?rID=" + 1 + "&rc:Command=Render";
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.DebtorInvoiceActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onReportsButtonClicked: {
                    fn: function (cmp) {
                        cmp.viewReports(me.mapResultsToReports());
                    },
                    scope: me
                },
                onCreateBatchButtonClicked: {
                    fn: function () {
                        me.actionCreatebatch();
                    },
                    scope: me
                }
            }
        });
        cfg.createBatchPanel = Ext.create('TargetExt.DebtorInvoices.CreateDebtorInvoiceBatch', {
            listeners: {
                onBatchCreated: {
                    fn: function () {
                        cfg.searchPanel.search();
                        cfg.createBatchPanel.hide();
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.DebtorInvoiceSearcher', { resultSettings: me.resultSettings });
        cfg.selectorControl.OnLoaded(function () {
            if (cfg.selectorControl.GetItems().length > 0) {
                cfg.actionPanel.disableCreateBatchButton(!me.resultSettings.HasPermission('CanCreateBatch'));
            } else {
                cfg.actionPanel.disableCreateBatchButton(true);
            }
        });

        // call parents init
        this.callParent(arguments);
    },
    actionCreatebatch: function () {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), mdl = me.getData(), clientID, ID;
        if (mdl.data.ID == 'undefined') {
            ID = '0'
        } else {
            ID = mdl.data.ID.toString();
        }
        if (mdl.data.ClientID == 'undefined') {
            clientID = '0'
        } else {
            clientID = mdl.data.ClientID.toString();
        }
        me.setLoading(true);
        cfg.webSvcProxy.DisplayCreateDebtorInvoiceBatchWarning(mdl.data, "0", me.InvoiceBatchWarning_Callback, me);
    },
    InvoiceBatchWarning_Callback: function (webSvcResponse) {
        var me = webSvcResponse.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(webSvcResponse, cfg.webSvcProxy.url)) {
            return false;
        }
        if (webSvcResponse.value.Value == true) {
            Ext.MessageBox.confirm({
                title: 'Info',
                msg: "Invoices matching any of the following criteria will not be" +
                            "<br/>passed onto the batch creation process:<br/><br/>" +
                            "  - Provisional invoices<br/>" +
                            "  - Invoices already linked to a batch<br/>" +
                            "  - Invoices marked as 'excluded from debtors'.",
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.INFO,
                fn: me.displayCreatebatch,
                scope: me
            });
        } else {
            me.displayCreatebatch();
        }
    },
    getData: function () {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), mdl,
        invActual = Ext.Array.contains(search.InvoiceStatus, Selectors.DebtorInvoiceSelectorStatuses.Actual),
        invProvisional = Ext.Array.contains(search.InvoiceStatus, Selectors.DebtorInvoiceSelectorStatuses.Provisional),
        invRetracted = Ext.Array.contains(search.InvoiceStatus, Selectors.DebtorInvoiceSelectorStatuses.Retracted),
        invViaRetract = Ext.Array.contains(search.InvoiceStatus, Selectors.DebtorInvoiceSelectorStatuses.Retract),
        invBatchSel, invInclude;
        if ((Ext.Array.contains(search.InvoiceBatching, Selectors.DebtorInvoiceSelectorBatchStatuses.Batched) && Ext.Array.contains(search.InvoiceBatching, Selectors.DebtorInvoiceSelectorBatchStatuses.NotBatched)) ||
                (Ext.Array.contains(search.InvoiceBatching, Selectors.DebtorInvoiceSelectorBatchStatuses.Batched) == false && Ext.Array.contains(search.InvoiceBatching, Selectors.DebtorInvoiceSelectorBatchStatuses.NotBatched) == false)) {
            invBatchSel = 0;
        } else {
            invBatchSel = search.InvoiceBatching[0];
        }
        if ((Ext.Array.contains(search.InvoiceExclusion, Selectors.DebtorInvoiceSelectorExcludedStatuses.NotExcluded) && Ext.Array.contains(search.InvoiceExclusion, Selectors.DebtorInvoiceSelectorExcludedStatuses.Excluded)) ||
                (Ext.Array.contains(search.InvoiceExclusion, Selectors.DebtorInvoiceSelectorExcludedStatuses.NotExcluded) == false && Ext.Array.contains(search.InvoiceExclusion, Selectors.DebtorInvoiceSelectorExcludedStatuses.Excluded) == false)) {
            invInclude = '';
        } else {
            invInclude = search.InvoiceExclusion[0];
        }
        mdl = Ext.create('DebtorInvoiceSearchCriteriaItem', { ID: null,
            client: search.ServiceUser.ID,
            ClientID: search.ServiceUser.ID,
            Res: search.Residential.toString(),
            Dom: search.Domiciliary.toString(),
            Client: search.ClientInv.toString(),
            TP: search.ThirdPartyInv.toString(),
            Prop: search.PropertyInv.toString(),
            OLA: search.OLAInv.toString(),
            PenColl: search.PenCollectInv.toString(),
            HomeColl: search.HomeCollectInv.toString(),
            Std: search.Standard.toString(),
            Manual: search.Manual.toString(),
            SDS: search.SDS.toString(),
            Actual: invActual.toString(),
            Provisional: invProvisional.toString(),
            Retracted: invRetracted.toString(),
            Retract: invViaRetract.toString(),
            ZeroValue: search.IncludeZeroValue.toString(),
            DateFrom: search.DateFrom,
            DateTo: search.DateTo,
            BatchSel: invBatchSel,
            Exclude: invInclude,
            DebtorFilter: cfg.selectorControl.GetListFilterDebtor(),
            InvoiceNumFilter: cfg.selectorControl.GetListFilterInvoiceNo(),
            ClientRefFilter: cfg.selectorControl.GetListFilterDebtorRef()
        });
        return mdl;
    },
    displayCreatebatch: function () {
        var me = this, cfg = me.config, mdl = me.getData();
        cfg.createBatchPanel.show(mdl);
    },
    getUrlForServiceUser: function (item, backUrl) {
        var me = this, cfg = me.config, url = '#';
        backUrl = (backUrl || me.getBackUrl());
        if (item) {
            return cfg.svcUserUrl + '?clientid=' + item.ServiceUserID.toString() + '&mode=1' + '&backUrl=' + escape(backUrl);
        }
        return url;
    },
    createContextMenu: function (item, backUrl) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            backUrl = (backUrl || me.getBackUrl());
            cfg.contextMenu = {};
            cfg.contextMenu.menuInclude = Ext.create('Ext.menu.Item', {
                text: 'Include',
                tooltip: 'Mark this invoice to be included in a batch',
                cls: 'ActionPanels',
                hidden: (item.Exclude == false),
                disabled: (!me.resultSettings.HasPermission('ExcludeInclude')),
                iconCls: 'InclusionsImage',
                listeners: {
                    click: {
                        scope: me,
                        fn: function () {
                            me.toggleIncludeExclude();
                        }
                    }
                }
            });
            cfg.contextMenu.menuExclude = Ext.create('Ext.menu.Item', {
                text: 'Exclude',
                tooltip: 'Mark this invoice to be excluded from a batch',
                cls: 'ActionPanels',
                hidden: (item.Exclude == true),
                disabled: (!me.resultSettings.HasPermission('ExcludeInclude')),
                iconCls: 'PreclusionsImage',
                listeners: {
                    click: {
                        scope: me,
                        fn: function () {
                            me.toggleIncludeExclude();
                        }
                    }
                }
            });
            cfg.contextMenu.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                iconCls: 'PreviewImage',
                text: 'Preview',
                reportsButtonConfig: {
                    reportIds: me.resultSettings.ReportIdsForContextMenu,
                    listeners: {
                        onReportsButtonClicked: {
                            fn: function (cmp) {
                                me.openReport();
                            },
                            scope: me
                        }
                    }
                }
            });
            cfg.contextMenu.menuView = Ext.create('Target.ux.menu.ViewMenuItem', {
                showChildMenuAlways: true,
                viewItems: me.getViewMenuItems(item, backUrl)
            });
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                    cfg.contextMenu.menuView,
                    cfg.contextMenu.menuInclude,
                    cfg.contextMenu.menuExclude,
                    cfg.contextMenu.reportsMenu,
                    cfg.reportsMenu
                ]
            });
        }
        return cfg.contextMenu;
    },
    getViewMenuItems: function (item, backUrl) {
        var me = this, viewMenuItems = [];
        backUrl = (backUrl || me.getBackUrl());
        viewMenuItems.push({ href: me.getUrlForServiceUser(item, backUrl), key: 'ServiceUser', text: 'Service User' });
        return viewMenuItems;
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem(), backUrl;
        backUrl = (backUrl || me.getBackUrl());
        if (!ctxMenu) {
            me.createContextMenu(selectedItem, backUrl);
        } else {
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(selectedItem, backUrl));
            cfg.contextMenu.menuInclude.setVisible(selectedItem.Exclude == true);
            cfg.contextMenu.menuExclude.setVisible(selectedItem.Exclude == false);
        }
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    toggleIncludeExclude: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        // Toggle the Exclude/Include value for the current invoice's Exclude setting
        // (based on the Invoice.ExcludeFromDebtors field)
        var webSvcResponse;

        if (item.Exclude == "Yes") {
            webSvcResponse = cfg.webSvcProxy.DisplayDebtorInvoiceIncludeWarningMessage(item.ID)
            if (!CheckAjaxResponse(webSvcResponse, cfg.webSvcProxy.url)) {
                return false;
            }
            if (webSvcResponse.value) {
                Ext.MessageBox.confirm({
                    title: 'Confirm',
                    msg: 'This Invoice was automatically excluded from Debtors Interfaces by Abacus ' +
                         'because it has a net charge of zero. <br/><br/>Are you sure you wish to reverse this decision and ' +
                         'make this Invoice available for collection by a subsequent Debtors Interface run?',
                    buttons: Ext.MessageBox.YESNO,
                    icon: Ext.MessageBox.QUESTION,
                    fn: me.setIncludeExclude,
                    scope: me
                });
            } else {
                me.setIncludeExclude('yes');
            }
        } else {
            me.setIncludeExclude('yes');
        }
    },
    setIncludeExclude: function (btn) {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (btn === 'yes') {
            webSvcResponse = cfg.webSvcProxy.DebtorInvoiceToggleExclude(item.ID.toString());
            if (!CheckAjaxResponse(webSvcResponse, cfg.webSvcProxy.url)) {
                return false;
            }
            item.Exclude = !item.Exclude;
            cfg.selectorControl.UpdateItem(item);
        }
    },
    openReport: function () {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, rptsButton;
        if (!ctxMenu) {
            ctxMenu = me.createContextMenu(cfg.selectorControl.GetSelectedItem(), me.getBackUrl());
        }
        ctxMenu.reportsMenu.initControls();
        if (!cfg.isCtxRptsMenuIntd) {
            rptsButton = cfg.contextMenu.reportsMenu.config.reportsButton;
            rptsButton.initControls(false, function () {
                cfg.isCtxRptsMenuIntd = true;
                me.openReportCallBack();
            });
        } else {
            me.openReportCallBack();
        }
    },
    openReportCallBack: function () {
        var me = this, cfg = me.config, rptParams, rptsButton = cfg.contextMenu.reportsMenu.config.reportsButton;
        var selectedItem = cfg.selectorControl.GetSelectedItem();
        if (selectedItem.DocumentID != 0)
           document.location.href =  cfg.documentUrl + selectedItem.DocumentID;
        else {
            rptParams = rptsButton.getReportParametersForDirectView(me.resultSettings.ReportIdsForContextMenu[0], 'PDF', 'invoiceID', cfg.selectorControl.GetSelectedID());
            rptParams.params[0].params.push({ key: 'documentid', value: selectedItem.DocumentID });
            rptsButton.viewReport(rptParams);
        }
    }
});
