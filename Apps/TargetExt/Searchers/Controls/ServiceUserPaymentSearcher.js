Ext.define('Searchers.Controls.ServiceUserPaymentSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector',
        'InPlaceSelectors.Controls.ServiceUserInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'ServiceUserPaymentSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'Provider', mapping: 'Provider' },
                { name: 'ServiceUser', mapping: 'ServiceUser' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DatePeriodType', mapping: 'DatePeriodType' },
                { name: 'ProviderCount', mapping: 'ProviderCount' }
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
                ProviderID: searchRecord.data.Provider.ID,
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
        searchRecord.data.DateFrom.setDate(searchRecord.data.DateFrom.getDate() - 365);
        searchRecord.data.DatePeriodType = 0;
        searchRecord.data.DateTo = new Date();
        if (searchRecord.data.ProviderCount != 1) {
            searchRecord.data.Provider = null;
            searchRecord.data.ProviderID = 0;
        }
        searchRecord.data.ServiceUser = null;
        searchRecord.data.ServiceUserID = 0;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            // create provider in place selector
            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Provider',
                labelWidth: 100,
                anchor: '100%',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: false
                }
            });
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeRedundant(false);
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeResidential(true);

            // create service user selector
            cfg.frmGeneralServiceUserSelector = Ext.create('InPlaceSelectors.Controls.ServiceUserInPlaceSelector', {
                fieldLabel: 'Service User',
                labelWidth: 100,
                anchor: '100%',
                name: 'ServiceUser',
                searchTextBoxConfig: {
                    allowBlank: false
                }
            });
            cfg.frmGeneralServiceUserSelector.getSelector().SetShowResidential(true);
            cfg.frmGeneralServiceUserSelector.getSelector().SetShowNonResidential(false);
            // show only service users that are linked with this Provider
            cfg.frmGeneralProviderSelector.linkToInPlaceServiceUserSelector(cfg.frmGeneralServiceUserSelector, args.SearcherSettings.Item.Provider.ID);   // link provider selector to Service user selector

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
                    { value: 0, description: 'Falling within the period' }
                ]
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralServiceUserSelector,
                    cfg.frmGeneralDateRange
                ],
                listeners: {
                    add: {
                        scope: me,
                        fn: function (src, cmp, index) {
                            cmp.addListener('change', function () {
                                var meagain = me;
                                setTimeout(function () { meagain.raiseOnSearchChanged(); }, 10);

                                if (cfg.frmGeneralServiceUserSelector.getSelector().GetProviderID() == 0 || cfg.frmGeneralServiceUserSelector.getSelector().GetProviderID() == null) {
                                    cfg.frmGeneralServiceUserSelector.setDisabled(true);
                                    cfg.frmGeneralDateRange.setDisabled(true);
                                } else {
                                    cfg.frmGeneralServiceUserSelector.setDisabled(false);
                                    cfg.frmGeneralDateRange.setDisabled(false);
                                }
                            }, me);
                        }
                    }
                }
            });
        }
    },
    setSearch: function (item) {
        var me = this, cfg = me.config, mdl = Ext.create(cfg.modelName, item);
        // load forms with data from model
        cfg.frmGeneral.loadRecord(mdl);
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
        cfg.frmGeneralProviderSelector.resetValueFromWebService();
        cfg.frmGeneralServiceUserSelector.resetValueFromWebService();
    }
});


