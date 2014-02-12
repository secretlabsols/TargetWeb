Ext.define('Searchers.Controls.CareHomePaymentSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector',
        'InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'CareHomePaymentSearchItem';
        Ext.define('CareHomePaymentSearchItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DatePeriodType', mapping: 'DatePeriodType' },
                { name: 'ProcessStatusDateFrom', mapping: 'ProcessStatusDateFrom', type: 'date' },
                { name: 'ProcessStatusDateTo', mapping: 'ProcessStatusDateTo', type: 'date' },
                { name: 'ProcessStatusDatePeriodType', mapping: 'ProcessStatusDatePeriodType' },
                { name: 'Provider', mapping: 'Provider' },
                { name: 'WebSecurityUser', mapping: 'WebSecurityUser' },
                { name: 'ProcessSubStatuses', mapping: 'ProcessSubStatuses' },
                { name: 'ProcessStatusesBy', mapping: 'ProcessStatusesBy' },
                { name: 'ProcessStatuses', mapping: 'ProcessStatuses' },
                { name: 'PublicationMedium', mapping: 'PublicationMedium' }
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
        this.callParent(arguments);
    },
    getSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm(), searchRecord, returnRecord;
        if (frmGeneral.isValid()) {
            searchRecord = frmGeneral.getRecord();
            frmGeneral.updateRecord(searchRecord);
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {
                WebSecurityUserID: searchRecord.data.WebSecurityUser.ID,
                ProviderID: searchRecord.data.Provider.ID
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var me = this, cfg = me.config, frmGeneral = cfg.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;
        searchRecord.data.DateFrom = null;
        searchRecord.data.DatePeriodType = 0;
        searchRecord.data.DateTo = null;
        searchRecord.data.ProcessStatusDateFrom = null;
        searchRecord.data.ProcessStatusDatePeriodType = 1;
        searchRecord.data.ProcessStatusDateTo = null;
        searchRecord.data.WebSecurityUser = null;
        searchRecord.data.WebSecurityUserID = 0;
        searchRecord.data.ProcessStatuses = cfg.defaultProcessStatuses;
        searchRecord.data.ProcessSubStatuses = [];
        searchRecord.data.ProcessStatusesBy = [];
        searchRecord.data.PublicationMedium = [];
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Home',
                labelWidth: 100,
                anchor: '100%',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeResidential(true);
            var processStatusColl = args.GetSearchParameterValue('ProcessStatusColl', []); // get process status collection 
            cfg.frmGeneralprocessStatusColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: processStatusColl,
                remoteSort: false
            });
            cfg.frmGeneralprocessStatusColl.sort('Description', 'ASC');
            cfg.frmGeneralprocessStatusColl.filter([{
                filterFn: function (item) {
                    return item.get("Description") !== 'Failed Transmission';
                }
            }]);
            cfg.frmGeneralprocessStatusCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Status',
                store: cfg.frmGeneralprocessStatusColl,
                multiSelect: true,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'ProcessStatuses',
                plugins: ['clearbutton']
            });
            var processStatusByColl = args.GetSearchParameterValue('ProcessStatusByColl', []); // get process status collection 
            cfg.frmGeneralprocessStatusByCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Status By',
                store: processStatusByColl,
                queryMode: 'local',
                labelWidth: 100,
                anchor: '100%',
                name: 'ProcessStatusesBy',
                plugins: ['clearbutton'],
                multiSelect: true
            });
            var publicationMediumColl = args.GetSearchParameterValue('PublicationMediumColl', []); // get process status collection 
            cfg.frmGeneralPublicationMediumCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Pub Medium',
                store: publicationMediumColl,
                queryMode: 'local',
                labelWidth: 100,
                anchor: '100%',
                name: 'PublicationMedium',
                plugins: ['clearbutton'],
                multiSelect: true
            });
            var processSubStatusColl = args.GetSearchParameterValue('ProcessSubStatusColl', []); // get process sub status collection 
            cfg.frmGeneralprocessSubStatusColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: processSubStatusColl,
                remoteSort: false
            });
            cfg.frmGeneralprocessSubStatusColl.sort('Description', 'ASC');
            cfg.frmGeneralprocessSubStatusCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Sub Status',
                store: cfg.frmGeneralprocessSubStatusColl,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'ProcessSubStatuses',
                plugins: ['clearbutton'],
                multiSelect: true
            });
            cfg.frmGeneralWebSecurityUser = Ext.create('InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector', {
                fieldLabel: 'Assigned To',
                labelWidth: 100,
                anchor: '100%',
                name: 'WebSecurityUser',
                searchTextBoxConfig: {
                    allowBlank: true
                },
                webSecurityUserID: me.resultSettings.WebSecurityUserID
            });
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
                    { value: 0, description: 'Covers part or all of the period' }
                ]
            });
            // create status date range
            cfg.frmGeneralStatusDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
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
                    { value: 1, description: 'Falling within the period' }
                ],
                showTime: true
            });
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralprocessStatusCollCmb,
                    cfg.frmGeneralStatusDateRange,
                    cfg.frmGeneralprocessStatusByCollCmb,
                    cfg.frmGeneralprocessSubStatusCollCmb,
                    cfg.frmGeneralWebSecurityUser,
                    cfg.frmGeneralDateRange,
                    cfg.frmGeneralPublicationMediumCollCmb
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
        if (!Ext.isArray(mdl.data.ProcessStatuses)) {
            me.setModelPropertyAsArrayOfInt(mdl, 'ProcessStatuses');
        }
        if (!cfg.hasSetDefaults) {
            cfg.defaultProcessStatuses = mdl.data.ProcessStatuses;
            cfg.hasSetDefaults = true;
        }
        cfg.frmGeneral.loadRecord(mdl);
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
        cfg.frmGeneralProviderSelector.resetValueFromWebService();
        cfg.frmGeneralWebSecurityUser.resetValueFromWebService();
    }
});


