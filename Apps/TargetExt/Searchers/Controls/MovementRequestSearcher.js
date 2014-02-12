Ext.define('Searchers.Controls.MovementRequestSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.ServiceUserInPlaceSelector',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector',
        'InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'MovementRequestSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'MovementDateFrom', mapping: 'MovementDateFrom', type: 'date' },
                { name: 'MovementDateTo', mapping: 'MovementDateTo', type: 'date' },
                { name: 'MovementDatePeriodType', mapping: 'MovementDatePeriodType' },
                { name: 'ProcessStatuses', mapping: 'ProcessStatuses' },
                { name: 'ProcessStatusesBy', mapping: 'ProcessStatusesBy' },
                { name: 'ProcessSubStatuses', mapping: 'ProcessSubStatuses' },
                { name: 'Provider', mapping: 'Provider' },
                { name: 'Reasons', mapping: 'Reasons' },
                { name: 'ServiceUser', mapping: 'ServiceUser' },
                { name: 'AssignedTo', mapping: 'AssignedTo' },
                { name: 'ReceivedDateFrom', mapping: 'ReceivedDateFrom', type: 'date' },
                { name: 'ReceivedDateTo', mapping: 'ReceivedDateTo', type: 'date' },
                { name: 'ReceivedDatePeriodType', mapping: 'ReceivedDateFrom' },
                { name: 'ProcessStatusDateFrom', mapping: 'ProcessStatusDateFrom', type: 'date' },
                { name: 'ProcessStatusDateTo', mapping: 'ProcessStatusDateTo', type: 'date' },
                { name: 'ProcessStatusDatePeriodType', mapping: 'ProcessStatusDatePeriodType' }
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
        cfg.defaultProcessStatuses = me.resultSettings.SearcherSettings.Item.ProcessStatuses;
        // call parent
        me.callParent(arguments);
    },
    createReferenceDataComboAndStore: function (storeData, comboFieldLabel, comboFieldName, comboFieldValues) {
        var me = this, cfg = me.config, returnData = {};
        // create store
        returnData.store = Ext.create('Ext.data.Store', {
            fields: ['ID', 'Description'],
            data: storeData
        });
        // create combo
        returnData.combo = Ext.create('Ext.form.ComboBox', {
            fieldLabel: comboFieldLabel,
            store: returnData.store,
            queryMode: 'local',
            displayField: 'Description',
            anchor: '100%',
            valueField: 'ID',
            multiSelect: true,
            name: comboFieldName,
            plugins: ['clearbutton'],
            value: comboFieldValues
        });
        return returnData;
    },
    getSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm(), searchRecord, returnRecord;
        if (frmGeneral.isValid()) {
            searchRecord = frmGeneral.getRecord();
            frmGeneral.updateRecord(searchRecord);
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {
                AssignedToID: searchRecord.data.AssignedTo.ID,
                ProviderID: searchRecord.data.Provider.ID,
                ServiceUserID: searchRecord.data.ServiceUser.ID
            });
        }
        return returnRecord;
    },
    raiseOnSearchChange: function (meagain) {
        setTimeout(function () { meagain.raiseOnSearchChanged(); }, 10);
    },
    resetSearch: function () {
        var cfg = this.config, frmGeneral = cfg.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.AssignedTo = null;
        searchRecord.data.AssignedToID = 0;
        searchRecord.data.MovementDateFrom = null;
        searchRecord.data.MovementDatePeriodType = 0;
        searchRecord.data.MovementDateTo = null;
        searchRecord.data.ProcessStatuses = cfg.defaultProcessStatuses;
        searchRecord.data.ProcessStatusesBy = [];
        searchRecord.data.ProcessSubStatuses = [];
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;
        searchRecord.data.Reasons = [];
        searchRecord.data.ReceivedDateFrom = null;
        searchRecord.data.ReceivedDatePeriodType = 0;
        searchRecord.data.ReceivedDateTo = null;
        searchRecord.data.ServiceUser = null;
        searchRecord.data.ServiceUserID = 0;
        searchRecord.data.ProcessStatusDateFrom = null;
        searchRecord.data.ProcessStatusDateTo = null;
        searchRecord.data.ProcessStatusDatePeriodType = 0;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config, referenceData = me.resultSettings.GetSearchParameterValue('ReferenceData', null);
        if (!cfg.frmGeneral) {
            // create received date range field
            cfg.frmReceivedDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Received Date',
                dateRangeDateTimeFromControlConfig: {
                    name: 'ReceivedDateFrom'
                },
                dateRangeDateTimeToControlConfig: {
                    name: 'ReceivedDateTo'
                },
                dateRangeDropDownControlConfig: {
                    name: 'ReceivedDatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Falling within the period' }
                ],
                showTime: true
            });
            // create movement date range field
            cfg.frmMovementDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Effective Date',
                dateRangeDateTimeFromControlConfig: {
                    name: 'MovementDateFrom'
                },
                dateRangeDateTimeToControlConfig: {
                    name: 'MovementDateTo'
                },
                dateRangeDropDownControlConfig: {
                    name: 'MovementDatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Falling within the period' }
                ]
            });
            // create service user selector
            cfg.frmGeneralServiceUserSelector = Ext.create('InPlaceSelectors.Controls.ServiceUserInPlaceSelector', {
                fieldLabel: 'Service User',
                name: 'ServiceUser',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            // create provider in place selector
            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Home',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeBedsHomes(true);
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeResidential(true);
            // create reasons combo
            cfg.frmReasonsCmb = me.createReferenceDataComboAndStore(referenceData.ReasonDescriptionList, 'Reason', 'Reasons').combo;
            // create status combo
            cfg.frmStatus = me.createReferenceDataComboAndStore(referenceData.ProcessStatusList, 'Status', 'ProcessStatuses');
            cfg.frmStatusCmb = cfg.frmStatus.combo;
            cfg.frmStatusStore = cfg.frmStatus.store;
            cfg.frmStatusStore.filter([{
                filterFn: function (item) {
                    return item.get("Description") !== 'Failed Transmission';
                }
            }]);
            // create status date range
            cfg.frmStatusDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Status Date',
                dateRangeDateTimeFromControlConfig: {
                    name: 'ProcessStatusDateFrom'
                },
                dateRangeDateTimeToControlConfig: {
                    name: 'ProcessStatusDateTo'
                },
                dateRangeDropDownControlConfig: {
                    name: 'ProcessStatusDatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Falling within the period' }
                ],
                showTime: true
            });
            // create status by combo
            cfg.frmStatusByCmb = me.createReferenceDataComboAndStore(referenceData.ProcessStatusByList, 'Status By', 'ProcessStatusesBy').combo;
            // create sub status combo
            cfg.frmSubStatusCmb = me.createReferenceDataComboAndStore(referenceData.ProcessSubstatusListFiltered, 'Sub Status', 'ProcessSubStatuses').combo;
            // create user selector
            cfg.frmGeneralAssignedToSelector = Ext.create('InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector', {
                fieldLabel: 'AssignedTo',
                name: 'AssignedTo',
                searchTextBoxConfig: {
                    allowBlank: true
                },
                webSecurityUserID: me.resultSettings.WebSecurityUserID
            });
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmReceivedDateRange,
                    cfg.frmMovementDateRange,
                    cfg.frmGeneralServiceUserSelector,
                    cfg.frmGeneralProviderSelector,
                    cfg.frmReasonsCmb,
                    cfg.frmStatusCmb,
                    cfg.frmStatusDateRange,
                    cfg.frmStatusByCmb,
                    cfg.frmSubStatusCmb,
                    cfg.frmGeneralAssignedToSelector
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
    setSearch: function (item) {
        var me = this, cfg = me.config, mdl = Ext.create(cfg.modelName, item);
        cfg.frmGeneral.loadRecord(mdl);
    }
});


