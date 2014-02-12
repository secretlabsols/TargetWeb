/// <reference path="../../../../Library/JavaScript/ExtJs4/Source/ext-all-debug-w-comments.js" />

Ext.define('Searchers.Controls.DeceasedWorkTraySearcher', {
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
        cfg.modelName = 'DeceasedWorkTraySearchItem';
        Ext.define('DeceasedWorkTraySearchItem', {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DDateFrom', mapping: 'DDateFrom', type: 'date' },
                { name: 'DDateTo', mapping: 'DDateTo', type: 'date' },
                { name: 'DDateType', mapping: 'DDateType' },
                { name: 'DRDateFrom', mapping: 'DRDateFrom', type: 'date' },
                { name: 'DRDateTo', mapping: 'DRDateTo', type: 'date' },
                { name: 'DRDateType', mapping: 'DRDateType' },
                { name: 'CareType', mapping: 'CareType' },
                { name: 'ProcessStatuses', mapping: 'ProcessStatuses' },
                { name: 'ProcessStatusesBy', mapping: 'ProcessStatusesBy' },
                { name: 'ProcessSubStatuses', mapping: 'ProcessSubStatuses' },
                { name: 'WebSecurityUser', mapping: 'WebSecurityUser' },
                { name: 'WebSecurityUserID', mapping: 'WebSecurityUserID' },
                { name: 'BilledUpToDate', mapping: 'BilledUpToDate' },
                { name: 'AccountPaid', mapping: 'AccountPaid' },
                { name: 'PropertyWithCase', mapping: 'PropertyWithCase' },
                { name: 'Appointee', mapping: 'Appointee' },
                { name: 'OpenServiceOrders', mapping: 'OpenServiceOrders' },
                { name: 'OpenCareEpisodes', mapping: 'OpenCareEpisodes' }

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
                WebSecurityUserID: searchRecord.data.WebSecurityUser.ID
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var me = this, cfg = me.config, frmGeneral = cfg.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord.data.DDateFrom = me.getDefaultDate();
        searchRecord.data.DDateTo = null;
        searchRecord.data.DRDateFrom = null;
        searchRecord.data.DRDateTo = null;
        searchRecord.data.WebSecurityUser = null;
        searchRecord.data.WebSecurityUserID = 0;
        searchRecord.data.ProcessStatuses = cfg.defaultProcessStatuses;
        searchRecord.data.ProcessSubStatuses = [];
        searchRecord.data.ProcessStatusesBy = [];
        searchRecord.data.CareType = [];
        searchRecord.data.BilledUpToDate = false;
        searchRecord.data.AccountPaid = false;
        searchRecord.data.PropertyWithCase = false;
        searchRecord.data.Appointee = false;
        searchRecord.data.OpenServiceOrders = false;
        searchRecord.data.OpenCareEpisodes = false;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            cfg.frmGeneralDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Death Date',
                dateRangeDateTimeFromControlConfig: {
                    dateControlConfig: {
                        name: 'DDateFrom'
                    }
                },
                dateRangeDateTimeToControlConfig: {
                    dateControlConfig: {
                        name: 'DDateTo'
                    }
                },
                dateRangeDropDownControlConfig: {
                    name: 'DDatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Covering part or all of the period' }
                ]
            });

            cfg.frmGeneralDeathRecordedDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Death Date Recorded',
                dateRangeDateTimeFromControlConfig: {
                    dateControlConfig: {
                        name: 'DRDateFrom'
                    }
                },
                dateRangeDateTimeToControlConfig: {
                    dateControlConfig: {
                        name: 'DRDateTo'
                    }
                },
                dateRangeDropDownControlConfig: {
                    name: 'DRDatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Covering part or all of the period' }
                ]
            });

            //            // with held
            cfg.frmGeneralcareTypeColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: [
                        { "ID": 1, "Description": "Non-Residential" },
                        { "ID": 2, "Description": "Residential" },
                        { "ID": 3, "Description": "SDS" }
                      ]
            });
            cfg.frmGeneralcareTypeColl.sort('Description', 'ASC');
            cfg.frmGeneralcareTypeCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Care Type',
                store: cfg.frmGeneralcareTypeColl,
                displayField: 'Description',
                valueField: 'ID',
                queryMode: 'local',
                labelWidth: 100,
                anchor: '100%',
                name: 'CareType',
                plugins: ['clearbutton'],
                multiSelect: true
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

            cfg.frmGeneralBilledUpToDate = Ext.create('Ext.form.field.Checkbox', {
                boxLabel: 'Not Billed Up To Date',
                name: 'BilledUpToDate'
            });

            cfg.frmGeneralAccountPaid = Ext.create('Ext.form.field.Checkbox', {
                boxLabel: 'Account Not Paid',
                name: 'AccountPaid'
            });

            cfg.frmGeneralProperty = Ext.create('Ext.form.field.Checkbox', {
                boxLabel: 'Property',
                name: 'PropertyWithCase',
                labelAlign: 'right'
            });

            cfg.frmGeneralAppointee = Ext.create('Ext.form.field.Checkbox', {
                boxLabel: 'Appointee',
                name: 'Appointee'
            });

            cfg.frmGeneralOpenSO = Ext.create('Ext.form.field.Checkbox', {
                boxLabel: 'Open Service Order',
                name: 'OpenServiceOrders'
            });

            cfg.frmGeneralOpenCO = Ext.create('Ext.form.field.Checkbox', {
                boxLabel: 'Open Care Episode',
                name: 'OpenCareEpisodes'
            });

            cfg.frmGeneralSpacerL1 = Ext.create('Ext.form.Label', {

            });
            cfg.frmGeneralSpacerL2 = Ext.create('Ext.form.Label', {

            });
            cfg.frmGeneralSpacerL3 = Ext.create('Ext.form.Label', {
                name: 'text',
                text: 'OR',
                padding: '0 15 0 0',
                layout: {
                    type: 'hbox',
                    align: 'middle'
                }

            });

            cfg.TextFilterContainert = Ext.create('Ext.container.Container', {
                xtype: 'container',
                anchor: '100%',
                padding: '0 0 0 0',
                layout: { type: 'table', columns: 3, tableAttrs: { style: { width: '100%'}} },
                items: [
                    cfg.frmGeneralBilledUpToDate,
                    cfg.frmGeneralSpacerL1,
                    cfg.frmGeneralAccountPaid,
                    cfg.frmGeneralProperty,
                    cfg.frmGeneralSpacerL2,
                    cfg.frmGeneralAppointee,
                    cfg.frmGeneralOpenSO,
                    cfg.frmGeneralSpacerL3,
                    cfg.frmGeneralOpenCO
                ]
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralDateRange,
                    cfg.frmGeneralDeathRecordedDateRange,
                    cfg.frmGeneralprocessStatusCollCmb,
                    cfg.frmGeneralprocessStatusByCollCmb,
                    cfg.frmGeneralprocessSubStatusCollCmb,
                    cfg.frmGeneralWebSecurityUser,
                    cfg.frmGeneralcareTypeCollCmb,
                    cfg.TextFilterContainert
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
    },
    getDefaultDate: function () {
        var currDate = new Date();
        currDate.setFullYear(currDate.getFullYear() - 1);
        return currDate;
    }

});


