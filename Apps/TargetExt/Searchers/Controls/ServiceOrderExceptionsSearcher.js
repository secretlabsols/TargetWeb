Ext.define('Searchers.Controls.ServiceOrderExceptionsSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'Target.ux.menu.ViewMenuItem',
        'InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'ServiceOrderExceptionsSearchItem';
        Ext.define('ServiceOrderExceptionsSearchItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'Provider', mapping: 'Provider' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DatePeriodType', mapping: 'DatePeriodType' },
                { name: 'ExceptionType', mapping: 'ExceptionType' },
                { name: 'ProcessStatuses', mapping: 'ProcessStatuses' },
                { name: 'ProcessStatusesBy', mapping: 'ProcessStatusesBy' },
                { name: 'ProcessSubStatuses', mapping: 'ProcessSubStatuses' },
                { name: 'WebSecurityUser', mapping: 'WebSecurityUser' },
                { name: 'WebSecurityUserID', mapping: 'WebSecurityUserID' },
                { name: 'WithholdUpdate', mapping: 'WithholdUpdate' }
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
        searchRecord.data.DateTo = null;
        searchRecord.data.WebSecurityUser = null;
        searchRecord.data.WebSecurityUserID = 0;
        searchRecord.data.ProcessStatuses = cfg.defaultProcessStatuses;
        searchRecord.data.ProcessSubStatuses = [];
        searchRecord.data.ProcessStatusesBy = [];
        searchRecord.data.ExceptionType = [];
        searchRecord.data.WithholdUpdate = [];
        frmGeneral.loadRecord(searchRecord);       
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Provider',
                labelWidth: 100,
                anchor: '100%',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            cfg.frmGeneralDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Exception Date',
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

            // with held
            cfg.frmGeneralexceptionTypeColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: [
                        { "ID": 1, "Description": "Critical " },
                        { "ID": 2, "Description": "Missing Information" },
                        { "ID": 3, "Description": "Failed Validation Check" }
                      ]
            });
            cfg.frmGeneralexceptionTypeColl.sort('Description', 'ASC');
            cfg.frmGeneralexceptionTypeCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Exception Type',
                store: cfg.frmGeneralexceptionTypeColl,
                displayField: 'Description',
                valueField: 'ID',
                queryMode: 'local',
                labelWidth: 100,
                anchor: '100%',
                name: 'ExceptionType',
                plugins: ['clearbutton'],
                multiSelect: true
            });

            // process status 
            var processStatusColl = args.GetSearchParameterValue('ProcessStatusColl', []); // get process status collection 
            cfg.frmGeneralprocessStatusColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: processStatusColl,
                remoteSort: false
            });
            cfg.frmGeneralprocessStatusColl.sort('Description', 'ASC');
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

            //process status by
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

            // process status 
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

            // assigned to inplace selector
            cfg.frmGeneralWebSecurityUser = Ext.create('InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector', {
                fieldLabel: 'Assigned To',
                labelWidth: 100,
                anchor: '100%',
                name: 'WebSecurityUser',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            // with held
            cfg.frmGeneralWithholdUpdateColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: [
                        { "ID": 1, "Description": "Withheld" },
                        { "ID": 0, "Description": "Not Withheld" }
                      ]
            });
            cfg.frmGeneralWithholdUpdateColl.sort('Description', 'ASC');
            cfg.frmGeneralwithholdUpdateCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Withhold Update',
                store: cfg.frmGeneralWithholdUpdateColl,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'WithholdUpdate',
                plugins: ['clearbutton'],
                multiSelect: true
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralDateRange,
                    cfg.frmGeneralexceptionTypeCollCmb,
                    cfg.frmGeneralprocessStatusCollCmb,
                    cfg.frmGeneralprocessStatusByCollCmb,
                    cfg.frmGeneralprocessSubStatusCollCmb,
                    cfg.frmGeneralWebSecurityUser,
                    cfg.frmGeneralwithholdUpdateCollCmb
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
        cfg.frmGeneralWebSecurityUser.resetValueFromWebService();
    }
});


