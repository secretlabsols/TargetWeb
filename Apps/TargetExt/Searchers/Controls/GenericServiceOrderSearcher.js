Ext.define('Searchers.Controls.GenericServiceOrderSearcher', {
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
        cfg.modelName = 'GenericServiceOrderSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DatePeriodType', mapping: 'DatePeriodType' },
                { name: 'Provider', mapping: 'Provider' },
                { name: 'GenericContract', mapping: 'GenericContract' },
                { name: 'ServiceGroup', mapping: 'ServiceGroup' },
                { name: 'ServiceUser', mapping: 'ServiceUser' }
            ],
            idProperty: 'ID'
        });

        cfg.currentApplication = me.resultSettings.GetSearchParameterValue('CurrentApplication');

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
                ServiceGroupID: searchRecord.data.ServiceGroup.ID,
                ServiceUserID: searchRecord.data.ServiceUser.ID
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.DateFrom = new Date();
        searchRecord.data.DatePeriodType = 0;
        searchRecord.data.DateTo = null;
        searchRecord.data.GenericContract = null;
        searchRecord.data.GenericContractID = 0;
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;
        searchRecord.data.ServiceGroup = null;
        searchRecord.data.ServiceGroupID = 0;
        searchRecord.data.ServiceUser = null;
        searchRecord.data.ServiceUserID = 0;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            var svcGroups = args.GetSearchParameterValue('ServiceGroups', []);  // get service groups from params
            // create provider in place selector
            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Provider',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeRedundant(true);
            if (cfg.currentApplication = 3) {
                cfg.frmGeneralProviderSelector.getSelector().SetIncludeResidential(false);
            }

            // create contract in place selector
            cfg.frmGeneralContractSelector = Ext.create('InPlaceSelectors.Controls.GenericContractInPlaceSelector', {
                fieldLabel: 'Contract',
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
                name: 'ServiceUser',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            if (cfg.currentApplication != 3) {
                // create service group selector
                cfg.frmGeneralServiceGroupSelector = Ext.create('InPlaceSelectors.Controls.ServiceGroupInPlaceSelector', {
                    fieldLabel: 'Service Group',
                    name: 'ServiceGroup',
                    searchTextBoxConfig: {
                        allowBlank: true
                    }
                });
            }
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
                    { value: 0, description: 'Covering part or all of the period' },
                    { value: 1, description: 'Starts in the period' },
                    { value: 2, description: 'Ends in the period' },
                    { value: 3, description: 'Starts or ends in the period' }
                ]
            });
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralContractSelector,
                    cfg.frmGeneralServiceUserSelector,
                    cfg.frmGeneralServiceGroupSelector,
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
        cfg.frmGeneralServiceGroupSelector.resetValueFromWebService();
    }
});


