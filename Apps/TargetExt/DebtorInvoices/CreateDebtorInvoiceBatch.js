Ext.define('TargetExt.DebtorInvoices.CreateDebtorInvoiceBatch', {
    extend: 'Ext.window.Window',
    requires: [
        'TargetExt.DebtorInvoices.CreateInterfaceFilesControl'
    ],
    title: 'Create Debtor Invoice Batch',
    width: 400,
    resizable: false,
    modal: true,
    closeAction: 'hide',
    show: function (itm) {
        var me = this, cfg = me.config, webSvcResponse, panelData = cfg.createFiles.getPanelData();
        cfg.SearchCriteriaItem = itm;
        me.updateBatchCounts();
        me.callParent();
    },
    initComponent: function () {
        var me = this, cfg = me.config, controls = [];
        cfg.webSvcProxy = Target.Abacus.Web.Apps.WebSvc.DebtorInvoice;
        cfg.SearchCriteriaItem = null;
        cfg.blanker1 = Ext.create('Ext.form.field.Display', {
            fieldLabel: '',
            labelWidth: 115,
            name: 'blanker',
            value: ''
        });
        cfg.blanker2 = Ext.create('Ext.form.field.Display', {
            fieldLabel: '',
            labelWidth: 115,
            name: 'blanker2',
            value: ''
        });
        cfg.blanker3 = Ext.create('Ext.form.field.Display', {
            fieldLabel: '',
            labelWidth: 115,
            name: 'blanker3',
            value: ''
        });
        cfg.invoiceCount = Ext.create('Ext.form.field.Display', {
            fieldLabel: 'Invoice Count',
            labelWidth: 115,
            name: 'InvoiceCount',
            value: ''
        });
        cfg.invoiceTotalValue = Ext.create('Ext.form.field.Display', {
            fieldLabel: 'Total Value',
            labelWidth: 115,
            name: 'TotalValue',
            value: ''
        });
        cfg.formSectorCodeStore = Ext.create('Ext.data.Store', {
            fields: [{ name: 'SectorID' },
                     { name: 'SectorName'}],
            data: []
        });

        cfg.webSvcProxy.FetchSectorCodesList(me.getSectorCodesCallBack, me);
        cfg.sectorCodeCmb = Ext.create('Ext.form.ComboBox', {
            name: 'SectorCode',
            fieldLabel: 'Sector Code',
            labelWidth: 115,
            store: cfg.formSectorCodeStore,
            queryMode: 'local',
            displayField: 'SectorName',
            valueField: 'SectorID',
            editable: false,
            width: 205,
            listeners:  {
                change: function(field, newValue, oldValue)
                {
                    me.updateBatchCounts();
                }
            }
        });
        cfg.refreshBtn = Ext.create('Ext.button.Button', {
            text: 'Refresh',
            scope: this,
            handler: function () {
                var me = this
                me.updateBatchCounts();
            }
        });
        cfg.anticipatedBatchContents = Ext.create('Ext.form.FieldSet', {
            title: 'Anticipated Batch Contents',
            margin: '5 5 0 5',
            items: [{ xtype: 'container',
                anchor: '100%',
                layout: { type: 'table', columns: 2, tableAttrs: { style: { width: '107%'}} },
                items: [cfg.invoiceCount, cfg.refreshBtn, cfg.invoiceTotalValue, cfg.blanker1, cfg.blanker2, cfg.blanker3, cfg.sectorCodeCmb]
            }]
        });
        controls.push(cfg.anticipatedBatchContents);
        cfg.createFiles = Ext.create('TargetExt.DebtorInvoices.CreateInterfaceFilesControl', {
            listeners: {
                fieldvaliditychange: {
                    fn: function (cmp, fld, vld) {
                        me.setSaveAvailibility();
                    },
                    scope: this
                }
            }
        })
        controls.push(cfg.createFiles);
        cfg.createBtn = Ext.create('Ext.button.Button', {
            text: 'Create',
            iconCls: 'SaveImage',
            scope: this,
            handler: function () {
                var me = this
                me.createBatch();
            }
        });
        cfg.toolbar = Ext.create('Ext.toolbar.Toolbar', {
            defaults: {
                cls: 'ActionPanels'
            },
            border: '2 0 0 0',
            items: ['->', cfg.createBtn]
        });
        controls.push(cfg.toolbar);
        cfg.controlsPanel = Ext.create('Ext.panel.Panel', {
            items: controls
        });
        me.items = cfg.controlsPanel;
        me.callParent(arguments);
    },
    updateBatchCounts: function () {
        var me = this, cfg = me.config, panelData = cfg.createFiles.getPanelData();
        if (cfg.SearchCriteriaItem != null) {
            me.setLoading('Updating...');
            cfg.webSvcProxy.FetchDebtorInvoiceListCount(cfg.SearchCriteriaItem.data, "0", cfg.sectorCodeCmb.getValue().toString(), me.updateBatchCountsCallBack, me);
        }
    },
    updateBatchCountsCallBack: function (response) {
        var me = response.context, cfg = me.config, firstInvoice;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.webSvcProxy.url)) {
            return false;
        }
        if (response.value.Invoices.length > 0) {
            firstInvoice = response.value.Invoices[0];
        } else {
            firstInvoice = {
                InvoiceCount: 0,
                InvoiceTotalValue: 0
            };
        }
        cfg.invoiceCount.setValue(firstInvoice.InvoiceCount);
        cfg.invoiceTotalValue.setValue(firstInvoice.InvoiceTotalValue);
        me.setSaveAvailibility();
    },
    createBatch: function () {
        var me = this, cfg = me.config, panelData = cfg.createFiles.getPanelData(), rollBackOption;
        if (panelData.Rollback == 'All') {
            rollBackOption = 2
        } else {
            rollBackOption = 1
        }
        cfg.controlsPanel.setLoading(('Creating Debtor Invoice Batch...' || 'Loading...'));

        cfg.webSvcProxy.CreateDebtorInvoiceBatch(cfg.SearchCriteriaItem.data,
                                             panelData.CreateDate, //StartDateTime
                                             panelData.PostingDateFrom, //PostingDate
                                             panelData.PostingYear, //postingYear
                                             panelData.PeriodNumber, //periodNumber
                                             cfg.sectorCodeCmb.getValue().toString(), //sectorCode
                                             rollBackOption, //rollback
                                             me.createBatch_CallBack,
                                             me);
    },
    createBatch_CallBack: function (webSvcResponse) {
        var me = webSvcResponse.context, cfg = me.config;
        cfg.controlsPanel.setLoading(false);
        if (!CheckAjaxResponse(webSvcResponse, cfg.webSvcProxy.url)) {
            return false;
        }
        me.fireEvent('onBatchCreated');
    },
    setSaveAvailibility: function () {
        var me = this, cfg = me.config, isValid = cfg.createFiles.isValid();
        cfg.createBtn.setDisabled((cfg.invoiceCount.getValue() == 0 || !isValid));
    },
    getSectorCodesCallBack: function (response) {
        var me = response.context, cfg = me.config;

        if (!CheckAjaxResponse(response, cfg.webSvcProxy.url)) {
            return false;
        }

        cfg.formSectorCodeStore.loadData(response.value.SectorCodes);
        cfg.sectorCodeCmb.setValue(response.value.SectorCodes[0].CurrentUserSectorID);
        if (cfg.formSectorCodeStore.getCount() == 0 || cfg.formSectorCodeStore.getCount() == 1) {
            cfg.sectorCodeCmb.hide();
        } else {
            cfg.sectorCodeCmb.show();
        }
    }
});