/// <reference path="../../../../Library/JavaScript/ExtJs4/Source/ext-all-debug-w-comments.js" />

Ext.define('Searchers.Controls.ClientExceptionWorkTraySearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'Target.ux.menu.ViewMenuItem',
        'InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector',
        'InPlaceSelectors.Controls.ServiceUserInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'ClientExceptionsSearchItem';
        Ext.define('ClientExceptionsSearchItem', {
            extend: 'Ext.data.Model',
            fields: [
                        { name: 'ServiceUser', mapping: 'ServiceUser' },
                        { name: 'ServiceUserID', mapping: 'ServiceUserID' },
                        { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                        { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                        { name: 'DateType', mapping: 'DateType' },
                        { name: 'ProcessStatuses', mapping: 'ProcessStatuses' },
                        { name: 'ProcessStatusesBy', mapping: 'ProcessStatusesBy' },
                        { name: 'ProcessSubStatuses', mapping: 'ProcessSubStatuses' },
                        { name: 'WebSecurityUser', mapping: 'WebSecurityUser' },
                        { name: 'WebSecurityUserID', mapping: 'WebSecurityUserID' },
                        { name: 'DIDateFrom', mapping: 'DIDateFrom', type: 'date' },
                        { name: 'DIDateTo', mapping: 'DIDateTo', type: 'date' },
                        { name: 'DIDateType', mapping: 'DIDateType' },
                        { name: 'Deceased', mapping: 'Deceased' },
                        { name: 'OnAbacus', mapping: 'OnAbacus' },
                        { name: 'WithholdAcceptence', mapping: 'WithholdAcceptence' }
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
                ServiceUserID: searchRecord.data.ServiceUser.ID
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var me = this, cfg = me.config, frmGeneral = cfg.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord.data.ServiceUser = null;
        searchRecord.data.ServiceUserID = 0;
        searchRecord.data.DateFrom = null;
        searchRecord.data.DateTo = null;
        searchRecord.data.DIDateFrom = null;
        searchRecord.data.DIDateTo = null;
        searchRecord.data.WebSecurityUser = null;
        searchRecord.data.WebSecurityUserID = 0;
        searchRecord.data.ProcessStatuses = cfg.defaultProcessStatuses;
        searchRecord.data.ProcessSubStatuses = [];
        searchRecord.data.ProcessStatusesBy = [];
        searchRecord.data.Deceased = [];
        searchRecord.data.OnAbacus = [];
        searchRecord.data.WithholdAcceptence = [];

        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            cfg.frmGeneralClientSelector = Ext.create('InPlaceSelectors.Controls.ServiceUserInPlaceSelector', {
                fieldLabel: 'Service User',
                labelWidth: 100,
                anchor: '100%',
                name: 'ServiceUser',
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
            // // process status 
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
            //            //process status by
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
            cfg.frmGeneralDateImported = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Date Imported',
                dateRangeDateTimeFromControlConfig: {
                    dateControlConfig: {
                        name: 'DIDateFrom'
                    }
                },
                dateRangeDateTimeToControlConfig: {
                    dateControlConfig: {
                        name: 'DIDateTo'
                    }
                },
                dateRangeDropDownControlConfig: {
                    name: 'DIDatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Covering part or all of the period' }
                ]
            });
            // deceased
            cfg.frmGeneralDeceasedColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: [
                        { "ID": 1, "Description": "Deceased" },
                        { "ID": 0, "Description": "Not Deceased" }
                      ]
            });
            cfg.frmGeneralDeceasedColl.sort('Description', 'ASC');
            cfg.frmGeneralDeceasedCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Deceased',
                store: cfg.frmGeneralDeceasedColl,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'Deceased',
                plugins: ['clearbutton'],
                multiSelect: true
            });
            // deceased
            cfg.frmGeneralOnAbacusColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: [
                        { "ID": 1, "Description": "On Abacus" },
                        { "ID": 0, "Description": "Not On Abacus" }
                      ]
            });
            cfg.frmGeneralOnAbacusColl.sort('Description', 'ASC');
            cfg.frmGeneralOnAbacusCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'On Abacus',
                store: cfg.frmGeneralOnAbacusColl,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'OnAbacus',
                plugins: ['clearbutton'],
                multiSelect: true
            });
            // with held
            cfg.frmGeneralWithholdAcceptenceColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: [
                        { "ID": 1, "Description": "Withheld" },
                        { "ID": 0, "Description": "Not Withheld" }
                      ]
            });
            cfg.frmGeneralWithholdAcceptenceColl.sort('Description', 'ASC');
            cfg.frmGeneralWithholdAcceptenceCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Withhold Acceptance',
                store: cfg.frmGeneralWithholdAcceptenceColl,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'WithholdAcceptence',
                plugins: ['clearbutton'],
                multiSelect: true
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralClientSelector,
                    cfg.frmGeneralDateRange,
                    cfg.frmGeneralprocessStatusCollCmb,
                    cfg.frmGeneralprocessStatusByCollCmb,
                    cfg.frmGeneralprocessSubStatusCollCmb,
                    cfg.frmGeneralWebSecurityUser,
                    cfg.frmGeneralDateImported,
                    cfg.frmGeneralDeceasedCollCmb,
                    cfg.frmGeneralOnAbacusCollCmb,
                    cfg.frmGeneralWithholdAcceptenceCollCmb
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
        cfg.frmGeneralServiceUserSelector.resetValueFromWebService();
    }
});


