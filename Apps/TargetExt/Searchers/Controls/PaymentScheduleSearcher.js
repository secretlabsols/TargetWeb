Ext.define('Searchers.Controls.PaymentScheduleSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.GenericContractInPlaceSelector',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector',
        'InPlaceSelectors.Controls.ServiceGroupInPlaceSelector',
        'InPlaceSelectors.Controls.ServiceUserInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'GenericPaymentScheduleSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DatePeriodType', mapping: 'DatePeriodType' },
                { name: 'Provider', mapping: 'Provider' },
                { name: 'GenericContract', mapping: 'GenericContract' },
                { name: 'NoProforma', mapping: 'NoProforma' },
                { name: 'AwaitingVerification', mapping: 'AwaitingVerification' },
                { name: 'Verified', mapping: 'Verified' },
                { name: 'UnpaidInv', mapping: 'UnpaidInv' },
                { name: 'SuspendedInv', mapping: 'SuspendedInv' },
                { name: 'AuthorisedInv', mapping: 'AuthorisedInv' },
                { name: 'PaidInv', mapping: 'PaidInv' },
                { name: 'AMAwaitingVerification', mapping: 'AMAwaitingVerification' },
                { name: 'ARVerified', mapping: 'ARVerified' },
                { name: 'ARDeclined', mapping: 'ARDeclined' }
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
        // call parent
        me.callParent(arguments);
    },
    getSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm(), searchRecord, returnRecord;
        if (frmGeneral.isValid()) {
            searchRecord = frmGeneral.getRecord();
            frmGeneral.updateRecord(searchRecord);
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {
                GenericContractID: searchRecord.data.GenericContract.ID,
                ProviderID: searchRecord.data.Provider.ID,
                DateFrom: searchRecord.data.DateFrom,
                DateTo: searchRecord.data.DateTo,
                NoProforma: searchRecord.data.NoProforma,
                AwaitingVerification: searchRecord.data.AwaitingVerification,
                Verified: searchRecord.data.Verified,
                UnpaidInv: searchRecord.data.UnpaidInv,
                SuspendedInv: searchRecord.data.SuspendedInv,
                AuthorisedInv: searchRecord.data.AuthorisedInv,
                PaidInv: searchRecord.data.PaidInv,
                AMAwaitingVerification: searchRecord.data.AMAwaitingVerification,
                ARVerified: searchRecord.data.ARVerified,
                ARDeclined: searchRecord.data.ARDeclined
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        //searchRecord.data.DateFrom = new Date();
        var oneWeekAgo = new Date();
        oneWeekAgo.setDate(oneWeekAgo.getDate() - 365);
        searchRecord.data.DateFrom = oneWeekAgo;
        searchRecord.data.DateTo = null;
        searchRecord.data.GenericContract = null;
        searchRecord.data.GenericContractID = 0;
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;

        searchRecord.data.NoProforma = true;
        searchRecord.data.AwaitingVerification = true;
        searchRecord.data.Verified = true;
        searchRecord.data.UnpaidInv = true;
        searchRecord.data.SuspendedInv = true;
        searchRecord.data.AuthorisedInv = true;
        searchRecord.data.PaidInv = true;
        searchRecord.data.AMAwaitingVerification = true;
        searchRecord.data.ARVerified = true;
        searchRecord.data.ARDeclined = true;

        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            // create provider in place selector
            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Provider',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            cfg.frmGeneralProviderSelector.getSelector().SetIncludeResidential(false);
            // create contract in place selector
            cfg.frmGeneralContractSelector = Ext.create('InPlaceSelectors.Controls.GenericContractInPlaceSelector', {
                fieldLabel: 'Contract',
                name: 'GenericContract',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            cfg.frmGeneralProviderSelector.linkToInPlaceContractSelector(cfg.frmGeneralContractSelector, args.SearcherSettings.Item.Provider.ID);   // link contracts selector to provider selector

            // create date range field            
            cfg.frmGeneralDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                dateRangeDateTimeFromControlConfig: {
                    dateControlConfig: {
                        name: 'DateFrom'
                    }
                },
                dateRangeDateTimeToControlConfig: {
                    dateControlConfig: {
                        name: 'DateTo'
                    }
                },
                dateRangeDropDownControlConfig: {
                    name: 'DatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Covering part or all of the period' }
                ]
            });

            cfg.frmGeneralUnprocessedProFormaInvoices = Ext.create('Ext.form.CheckboxGroup', {
                columns: 1,
                items: [
                    { boxLabel: 'No Pro forma Invoices', name: 'NoProforma' },
                    { boxLabel: 'Pro forma Invoices that are Awaiting Verification', name: 'AwaitingVerification' },
                    { boxLabel: 'Verified Pro forma Invoices', name: 'Verified' }
                ]
            });
            cfg.frmGeneralUnprocessedProFormaInvoicesFldSet = Ext.create('Ext.form.FieldSet', {
                title: 'Unprocessed Pro forma Invoices Having:',
                collapsible: true,
                items: [cfg.frmGeneralUnprocessedProFormaInvoices]//,
            });

            cfg.frmGeneralProviderInvoices = Ext.create('Ext.form.CheckboxGroup', {
                columns: 2,
                items: [
                    { boxLabel: 'Unpaid Invoices', name: 'UnpaidInv' },
                    { boxLabel: 'Suspended Invoices', name: 'SuspendedInv' },
                    { boxLabel: 'Authorised Invoices', name: 'AuthorisedInv' },
                    { boxLabel: 'Paid Invoices', name: 'PaidInv' }
                ]
            });
            cfg.frmGeneralProviderInvoicesFldSet = Ext.create('Ext.form.FieldSet', {
                title: 'Provider Invoices Having:',
                collapsible: true,
                items: [cfg.frmGeneralProviderInvoices]//,

            });

            cfg.frmGeneralAmendmentRequests = Ext.create('Ext.form.CheckboxGroup', {
                columns: 1,
                items: [
                    { boxLabel: 'Requests that are Awaiting Verification', name: 'AMAwaitingVerification' },
                    { boxLabel: 'Verified Requests', name: 'ARVerified' },
                    { boxLabel: 'Declined Requests', name: 'ARDeclined' }
                ]
            });
            cfg.frmGeneralAmendmentRequestsFldSet = Ext.create('Ext.form.FieldSet', {
                title: 'Unprocessed Visit Amendment Requests Having:',
                collapsible: true,
                items: [cfg.frmGeneralAmendmentRequests]//,
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralContractSelector,
                    cfg.frmGeneralDateRange,
                    cfg.frmGeneralUnprocessedProFormaInvoicesFldSet,
                    cfg.frmGeneralProviderInvoicesFldSet,
                    cfg.frmGeneralAmendmentRequestsFldSet
                ],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener('change', function () {
                                var meagain = me;
                                setTimeout(function () { meagain.raiseOnSearchChanged(); }, 10);
                            }, me);
                        }
                    }
                }
            });
        }
    },
    setSearch: function (item) {
        var me = this, cfg = me.config, mdl = Ext.create(cfg.modelName, item);
        cfg.frmGeneral.loadRecord(mdl);
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
        cfg.frmGeneralProviderSelector.resetValueFromWebService();
        cfg.frmGeneralContractSelector.resetValueFromWebService();
    }
});


