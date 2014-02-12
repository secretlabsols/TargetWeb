Ext.define('Searchers.Controls.ServiceRegisterSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTime',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector',
        'InPlaceSelectors.Controls.GenericContractInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'GenericCreditorPaymentSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DatePeriodType', mapping: 'DatePeriodType' },
                { name: 'Provider', mapping: 'Provider' },
                { name: 'GenericContract', mapping: 'GenericContract' },
                { name: 'Status', mapping: 'Status' }
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
                ProviderID: searchRecord.data.Provider.ID
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var me = this, cfg = me.config;
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.DateTo = me.getWeekEndingDate();
        searchRecord.data.DateFrom = Ext.Date.add(searchRecord.data.DateTo, Ext.Date.DAY, -27);
        searchRecord.data.GenericContract = null;
        searchRecord.data.GenericContractID = 0;
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;
        searchRecord.data.Status = ["1", "2", "3", "4"];
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            //var weDate = args.GetSearchParameterValue('WeekEndingDate', null);
            // create provider in place selector
            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Provider',
                labelWidth: 100,
                anchor: '100%',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
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
            cfg.frmGeneralContractSelector.getSelector().SetFrameWorkTypes([2]);
            cfg.frmGeneralProviderSelector.linkToInPlaceContractSelector(cfg.frmGeneralContractSelector, args.SearcherSettings.Item.Provider.ID);   // link contracts selector to provider selector

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
                    { value: 0, description: 'Covering any part of the period' }
                ]
            });

            cfg.frmGeneralDateRange.restrictSelectionToWeekRange(me.getWeekEndingDay());
            cfg.statuses = Ext.create('Ext.data.Store', {
                fields: ['ID', 'name'],
                data: [
                        { "ID": "1", "name": "In Progress" },
                        { "ID": "2", "name": "Submitted" },
                        { "ID": "3", "name": "Amended" },
                        { "ID": "4", "name": "Processed" }
                      ]
            });
            // Create the combo box, attached to the states data store
            cfg.frmRegisterstatusCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Register Status',
                store: cfg.statuses,
                queryMode: 'local',
                displayField: 'name',
                labelWidth: 100,
                anchor: '100%',
                valueField: 'ID',
                multiSelect: true,
                name: 'Status',
                id: 'Status'
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralContractSelector,
                    cfg.frmGeneralDateRange,
                    cfg.frmRegisterstatusCmb
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
    getWeekEndingDate: function () {
        var me = this, cfg = me.config;
        var weDate = Date.fromIsoDate(me.resultSettings.GetSearchParameterValue('WeekEndingDate', null));
        return weDate;
    },
    getWeekEndingDay: function () {
        var d = this.getWeekEndingDate();
        return d.getDay();
    },
    setSearch: function (item) {
        var me = this, cfg = me.config, mdl = Ext.create(cfg.modelName, item);
        if (!Ext.isArray(mdl.data.Status)) {
            me.setModelPropertyAsArray(mdl, 'Status');
        }
        cfg.frmGeneral.loadRecord(mdl);
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
        cfg.frmGeneralProviderSelector.resetValueFromWebService();
        cfg.frmGeneralContractSelector.resetValueFromWebService();
    }
});


