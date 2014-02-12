Ext.define('Searchers.Controls.DebtorInvoiceSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.formLoaded = false;
        cfg.modelName = 'DebtorInvoiceSearchItem';
        Ext.define('DebtorInvoiceSearchItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'ServiceUser', mapping: 'ServiceUser' },
                { name: 'InvoiceStatus', mapping: 'InvoiceStatus' },
                { name: 'InvoiceExclusion', mapping: 'InvoiceExclusion' },
                { name: 'InvoiceBatching', mapping: 'InvoiceBatching' },
                { name: 'IncludeZeroValue', mapping: 'IncludeZeroValue' },
                { name: 'Standard', mapping: 'Standard' },
                { name: 'Manual', mapping: 'Manual' },
                { name: 'Residential', mapping: 'Residential' },
                { name: 'Domiciliary', mapping: 'Domiciliary' },
                { name: 'SDS', mapping: 'SDS' },
                { name: 'ClientInv', mapping: 'ClientInv' },
                { name: 'ThirdPartyInv', mapping: 'ThirdPartyInv' },
                { name: 'PropertyInv', mapping: 'PropertyInv' },
                { name: 'OLAInv', mapping: 'OLAInv' },
                { name: 'PenCollectInv', mapping: 'PenCollectInv' },
                { name: 'HomeCollectInv', mapping: 'HomeCollectInv' }
            ],
            idProperty: 'ID'
        });
        // setup forms
        me.setupFrmGeneral(me.resultSettings);
        // add forms to panel
        me.items = [
            cfg.frmGeneral
        ];
        // load forms with data from model
        me.setSearch(me.resultSettings.SearcherSettings.Item);
        cfg.formLoaded = true;
        // call parent
        me.callParent(arguments);
    },
    getSearch: function () {
        var returnRecord, searchRecord;
        return this.getModel(returnRecord);
    },
    getModel: function (returnRecord) {
        var frmGeneral = this.config.frmGeneral.getForm(), searchRecord, returnRecord;
        if (frmGeneral.isValid()) {
            searchRecord = frmGeneral.getRecord();
            frmGeneral.updateRecord(searchRecord);
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {
                ServiceUserID: searchRecord.data.ServiceUser.ID
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        this.config.formLoaded = false;
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.DateFrom = Ext.Date.add(new Date(), Ext.Date.YEAR, -1);
        searchRecord.data.DateTo = null;
        searchRecord.data.ServiceUser = null;
        searchRecord.data.ServiceUserID = 0;
        searchRecord.data.Domiciliary = true;
        searchRecord.data.SDS = true;
        searchRecord.data.InvoiceStatus = [
            Selectors.DebtorInvoiceSelectorStatuses.Actual,
            Selectors.DebtorInvoiceSelectorStatuses.Retracted,
            Selectors.DebtorInvoiceSelectorStatuses.Retract
        ];
        searchRecord.data.InvoiceExclusion = [
            Selectors.DebtorInvoiceSelectorExcludedStatuses.NotExcluded,
            Selectors.DebtorInvoiceSelectorExcludedStatuses.Excluded
        ];
        searchRecord.data.InvoiceBatching = [
            Selectors.DebtorInvoiceSelectorBatchStatuses.Batched,
            Selectors.DebtorInvoiceSelectorBatchStatuses.NotBatched
        ];
        searchRecord.data.IncludeZeroValue = false;
        searchRecord.data.Standard = true;
        searchRecord.data.Manual = true;
        searchRecord.data.ClientInv = true;
        searchRecord.data.ThirdPartyInv = true;
        searchRecord.data.PropertyInv = true;
        searchRecord.data.OLAInv = true;
        searchRecord.data.PenCollectInv = false;
        searchRecord.data.HomeCollectInv = false;
        searchRecord.data.Residential = true;
        this.config.frmGeneralResInvoiceBxGroup.enable();
        frmGeneral.loadRecord(searchRecord);
        this.config.formLoaded = true;
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            cfg.frmGeneralDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                showTime: true,
                dateRangeDateTimeFromControlConfig: {
                    name: 'DateFrom'
                },
                dateRangeDateTimeToControlConfig: {
                    name: 'DateTo'
                },
                dateRangeDropDownControlConfig: {
                    name: 'DatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Created during the period' }
                ]
            });

            // create service user selector
            cfg.frmGeneralServiceUserSelector = Ext.create('InPlaceSelectors.Controls.ServiceUserInPlaceSelector', {
                fieldLabel: 'Service User',
                labelWidth: 100,
                anchor: '100%',
                name: 'ServiceUser',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            // create InvoiceStatus combo and store
            cfg.frmGeneralInvoiceStatusStore = Ext.create('Ext.data.Store', {
                fields: ['value', 'description'],
                data: [
                    { value: Selectors.DebtorInvoiceSelectorStatuses.Actual, description: 'Actual' },
                    { value: Selectors.DebtorInvoiceSelectorStatuses.Provisional, description: 'Provisional' },
                    { value: Selectors.DebtorInvoiceSelectorStatuses.Retracted, description: 'Retracted' },
                    { value: Selectors.DebtorInvoiceSelectorStatuses.Retract, description: 'Retraction' }
                ]
            });
            cfg.frmGeneralInvoiceStatusCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Invoice Status',
                store: cfg.frmGeneralInvoiceStatusStore,
                queryMode: 'local',
                displayField: 'description',
                valueField: 'value',
                labelWidth: 100,
                anchor: '100%',
                multiSelect: true,
                name: 'InvoiceStatus',
                plugins: ['clearbutton']
            });
            cfg.frmGeneralInvoiceExclusionStore = Ext.create('Ext.data.Store', {
                fields: ['value', 'description'],
                data: [
                    { value: Selectors.DebtorInvoiceSelectorExcludedStatuses.NotExcluded, description: 'Not Excluded' },
                    { value: Selectors.DebtorInvoiceSelectorExcludedStatuses.Excluded, description: 'Excluded' }
                ]
            });
            cfg.frmGeneralInvoiceExclusionCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Invoice Exclusion',
                store: cfg.frmGeneralInvoiceExclusionStore,
                queryMode: 'local',
                displayField: 'description',
                valueField: 'value',
                labelWidth: 100,
                anchor: '100%',
                multiSelect: true,
                name: 'InvoiceExclusion',
                plugins: ['clearbutton']
            });
            cfg.frmGeneralInvoiceBatchingStore = Ext.create('Ext.data.Store', {
                fields: ['value', 'description'],
                data: [
                    { value: Selectors.DebtorInvoiceSelectorBatchStatuses.Batched, description: 'Batched' },
                    { value: Selectors.DebtorInvoiceSelectorBatchStatuses.NotBatched, description: 'Unbatched' }
                ]
            });
            cfg.frmGeneralInvoiceBatchingCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Invoice Batching',
                store: cfg.frmGeneralInvoiceBatchingStore,
                queryMode: 'local',
                displayField: 'description',
                valueField: 'value',
                labelWidth: 100,
                anchor: '100%',
                multiSelect: true,
                name: 'InvoiceBatching',
                plugins: ['clearbutton']
            });
            cfg.frmGeneralResInvoiceBxGroup = Ext.create('Ext.form.CheckboxGroup', {
                columns: 2,
                items: [
                    { boxLabel: 'Client', name: 'ClientInv' },
                    { boxLabel: 'Third Party', name: 'ThirdPartyInv' },
                    { boxLabel: 'Property', name: 'PropertyInv', disabled: !me.resultSettings.HasPermission('EnableProperty') },
                    { boxLabel: 'OLA', name: 'OLAInv' },
                    { boxLabel: 'Pension Collection', name: 'PenCollectInv', disabled: !me.resultSettings.HasPermission('EnablePensionCollect') },
                    { boxLabel: 'Home Collection', name: 'HomeCollectInv', disabled: !me.resultSettings.HasPermission('EnableHomeCollect') }
                ]
            });
            cfg.frmGeneralDebtorType = Ext.create('Ext.form.FieldSet', {
                title: 'Residential Invoice Type',
                collapsible: true,
                items: [cfg.frmGeneralResInvoiceBxGroup],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener('change', function () {
                                var meagain = me;
                                meagain.raiseOnSearchChange(meagain);
                            }, me);
                        }
                    }
                }
            });
            cfg.frmGeneralInvoiceTypeBxGroup = Ext.create('Ext.form.CheckboxGroup', {
                columns: 2,
                items: [
                    { boxLabel: 'Standard Invoices', name: 'Standard' },
                    { boxLabel: 'Manual Invoices', name: 'Manual', checked: true }
                ]
            });
            cfg.frmGeneralInvoiceType = Ext.create('Ext.form.FieldSet', {
                title: 'Invoice Type',
                collapsible: true,
                items: [cfg.frmGeneralInvoiceTypeBxGroup],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener('change', function () {
                                var meagain = me;
                                meagain.raiseOnSearchChange(meagain);
                            }, me);
                        }
                    }
                }
            });

            cfg.frmGeneralzeroValueChkBx = Ext.create('Ext.form.field.Checkbox', {
                boxLabel: 'Include zero value invoices',
                name: 'IncludeZeroValue'
            });
            cfg.frmGeneralZeroValue = Ext.create('Ext.form.FieldSet', {
                title: 'Zero value invoices',
                collapsible: true,
                items: [cfg.frmGeneralzeroValueChkBx],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener('change', function () {
                                var meagain = me;
                                meagain.raiseOnSearchChange(meagain);
                            }, me);
                        }
                    }
                }
            });
            cfg.frmGeneralServiceTypeBxGroup = Ext.create('Ext.form.CheckboxGroup', {
                columns: 2,
                items: [
                    { boxLabel: 'Residential Invoices',
                        name: 'Residential',
                        listeners: {
                            change: {
                                scope: me,
                                fn: function () {
                                    if (cfg.formLoaded == true) {
                                        var returnRecord = this.getModel(returnRecord);
                                        if (returnRecord.Residential === true) {
                                            cfg.frmGeneralResInvoiceBxGroup.enable();
                                        } else {
                                            cfg.frmGeneralResInvoiceBxGroup.disable();
                                        }
                                    }

                                }
                            }
                        }
                    },
                    { boxLabel: 'Domiciliary Invoices', name: 'Domiciliary' },
                    { boxLabel: 'Self Directed Support', name: 'SDS', disabled: !me.resultSettings.HasPermission('EnableSDS') }
                ]
            });
            cfg.frmGeneralServiceType = Ext.create('Ext.form.FieldSet', {
                title: 'Service Type',
                collapsible: true,
                items: [cfg.frmGeneralServiceTypeBxGroup],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener('change', function () {
                                var meagain = me;
                                meagain.raiseOnSearchChange(meagain);
                            }, me);
                        }
                    }
                }
            });
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                        cfg.frmGeneralDateRange,
                        cfg.frmGeneralServiceUserSelector,
                        cfg.frmGeneralInvoiceStatusCmb,
                        cfg.frmGeneralInvoiceExclusionCmb,
                        cfg.frmGeneralInvoiceBatchingCmb,
                        cfg.frmGeneralServiceType,
                        cfg.frmGeneralDebtorType,
                        cfg.frmGeneralInvoiceType,
                        cfg.frmGeneralZeroValue
                    ],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener('change', function () {
                                var meagain = me;
                                meagain.raiseOnSearchChange(meagain);
                            }, me);
                        }
                    }
                }
            });
        }
    },
    raiseOnSearchChange: function (meagain) {
        setTimeout(function () { meagain.raiseOnSearchChanged(); }, 10);
    },
    setSearch: function (item) {
        var me = this, cfg = me.config, mdl = Ext.create(cfg.modelName, item);
        me.conditionallySetAsArray('InvoiceStatus', mdl);
        me.conditionallySetAsArray('InvoiceExclusion', mdl);
        me.conditionallySetAsArray('InvoiceBatching', mdl);
        cfg.frmGeneral.loadRecord(mdl);
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
        cfg.frmGeneralServiceUserSelector.resetValueFromWebService();
    },
    conditionallySetAsArray: function (name, mdl) {
        var me = this, cfg = me.config;
        if (!Ext.isArray(mdl.data[name])) {
            me.setModelPropertyAsArray(mdl, name);
        }
    }
});


