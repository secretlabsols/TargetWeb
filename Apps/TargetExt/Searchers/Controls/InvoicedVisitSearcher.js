Ext.define('Searchers.Controls.InvoicedVisitSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.GenericContractInPlaceSelector',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector',
        'InPlaceSelectors.Controls.ServiceUserInPlaceSelector',
        'InPlaceSelectors.Controls.CareWorkerInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'InvoicedVisitSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DatePeriodType', mapping: 'DatePeriodType' },
                { name: 'Provider', mapping: 'Provider' },
                { name: 'GenericContract', mapping: 'GenericContract' },
                { name: 'ServiceUser', mapping: 'ServiceUser' },
                { name: 'CareWorker', mapping: 'CareWorker' }
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
        var frmGeneral = this.config.frmGeneral.getForm(), me = this, cfg = me.config, searchRecord;
        var returnRecord, dateFrom = new Date(), dateTo = new Date();
        if (frmGeneral.isValid()) {
            searchRecord = frmGeneral.getRecord();
            frmGeneral.updateRecord(searchRecord);
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {
                GenericContractID: searchRecord.data.GenericContract.ID,
                ProviderID: searchRecord.data.Provider.ID,
                ServiceUserID: searchRecord.data.ServiceUser.ID,
                CareWorkerID: searchRecord.data.CareWorker.ID
            });
        }

        if ((!returnRecord || returnRecord.ProviderID == 0) && cfg.currentApplication == 3) {
            returnRecord = (returnRecord || {});
            returnRecord.ProviderID = -1;
            returnRecord.DateFrom = dateFrom;
            returnRecord.DatePeriodType = 0;
            returnRecord.DateTo = dateTo;
            returnRecord.GenericContract = null;
            returnRecord.GenericContractID = 0;
            returnRecord.Provider = null;
            returnRecord.ProviderID = 0;
            returnRecord.ServiceUser = null;
            returnRecord.ServiceUserID = 0;
            returnRecord.CareWorker = null;
            returnRecord.CareWorkerID = 0;
        }

        return returnRecord;
    },
    resetSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm(), searchRecord = frmGeneral.getRecord(), dateFrom = new Date(), dateTo = new Date();
        dateFrom.setHours(0, 0, 0, 0);
        dateTo.setHours(23, 59, 0, 0);
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.DateFrom = dateFrom;
        searchRecord.data.DatePeriodType = 0;
        searchRecord.data.DateTo = dateTo;
        searchRecord.data.GenericContract = null;
        searchRecord.data.GenericContractID = 0;
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;
        searchRecord.data.ServiceUser = null;
        searchRecord.data.ServiceUserID = 0;
        searchRecord.data.CareWorker = null;
        searchRecord.data.CareWorkerID = 0;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            //Get current application 
            cfg.currentApplication = me.resultSettings.GetSearchParameterValue('CurrentApplication');

            // create provider in place selector
            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Provider',
                labelWidth: 100,
                anchor: '100%',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: (cfg.currentApplication == 2)
                }
            });
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeRedundant(true);
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeResidential(false);
            //cfg.frmGeneralProviderSelector.getSelector().SetIncludeNonResidential(true);
            // create contract in place selector
            cfg.frmGeneralContractSelector = Ext.create('InPlaceSelectors.Controls.GenericContractInPlaceSelector', {
                fieldLabel: 'Contract',
                labelWidth: 100,
                anchor: '100%',
                name: 'GenericContract',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            // show only non res contracts
            cfg.frmGeneralContractSelector.getSelector().SetTypes([Selectors.GenericContractSelectorTypes.NonResidential]);
            cfg.frmGeneralProviderSelector.linkToInPlaceContractSelector(cfg.frmGeneralContractSelector, args.SearcherSettings.Item.Provider.ID);   // link contracts selector to provider selector
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
            // create Care Worker selector
            cfg.frmGeneralCareWorkerSelector = Ext.create('InPlaceSelectors.Controls.CareWorkerInPlaceSelector', {
                fieldLabel: 'Care Worker',
                labelWidth: 100,
                anchor: '100%',
                name: 'CareWorker',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
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
                    { value: 0, description: 'Claimed visit covers all or part of the period' },
                    { value: 1, description: 'Claimed start time falls within the period' },
                    { value: 2, description: 'Claimed end time falls within the period' },
                    { value: 3, description: 'Actual visit covers all or part of the period' },
                    { value: 4, description: 'Actual start time falls within the period' },
                    { value: 5, description: 'Actual end time falls within the period' }
                ]
            });
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralContractSelector,
                    cfg.frmGeneralServiceUserSelector,
                    cfg.frmGeneralCareWorkerSelector,
                    cfg.frmGeneralDateRange
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
        cfg.frmGeneralServiceUserSelector.resetValueFromWebService();
        cfg.frmGeneralCareWorkerSelector.resetValueFromWebService();
    }
});


