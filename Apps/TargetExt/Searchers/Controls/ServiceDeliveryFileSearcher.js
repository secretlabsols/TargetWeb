Ext.define('Searchers.Controls.ServiceDeliveryFileSearcher', {
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
        cfg.modelName = 'ServiceDeliveryFileSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'SubmittedByUserID', mapping: 'SubmittedByUserID' },
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'AwaitingProc', mapping: 'AwaitingProc' },
                { name: 'WorkProgress', mapping: 'WorkProgress' },
                { name: 'Processed', mapping: 'Processed' },
                { name: 'Deleted', mapping: 'Deleted' },
                { name: 'Failed', mapping: 'Failed' }
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
                SubmittedByUserID: searchRecord.data.SubmittedByUserID,
                DateFrom: searchRecord.data.DateFrom,
                DateTo: searchRecord.data.DateTo,
                AwaitingProc: searchRecord.data.AwaitingProc,
                WorkProgress: searchRecord.data.WorkProgress,
                Processed: searchRecord.data.Processed,
                Deleted: searchRecord.data.Deleted,
                Failed: searchRecord.data.Failed
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.SubmittedByUserID = null;
        searchRecord.data.DateFrom = null; 
        searchRecord.data.DateTo = null;
        searchRecord.data.AwaitingProc = true;
        searchRecord.data.WorkProgress = true;
        searchRecord.data.Processed = true;
        searchRecord.data.Deleted = false;
        searchRecord.data.Failed = true;


        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {


            var UploadedByColl = args.GetSearchParameterValue('UploadedByColl', []);

            cfg.frmUploadedByStore = Ext.create('Ext.data.Store', {
                fields: ['Value', 'Key'],
                data: UploadedByColl,
                remoteSort: false
            });

            // Create the combo box, attached to the states data store
            cfg.frmSubmittedBy = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Submitted By',
                store: cfg.frmUploadedByStore,
                queryMode: 'local',
                displayField: 'Value',
                labelWidth: 100,
                anchor: '100%',
                valueField: 'Key',
                name: 'SubmittedByUserID',
                id: 'SubmittedByUserID',
                plugins: ['clearbutton']
            });


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

            cfg.frmFileStatus = Ext.create('Ext.form.CheckboxGroup', {
                columns: 2,
                items: [
                    { boxLabel: 'Awaiting Processing', name: 'AwaitingProc' },
                    { boxLabel: 'Work in Progress', name: 'WorkProgress' },
                    { boxLabel: 'Processed', name: 'Processed' },
                    { boxLabel: 'Deleted', name: 'Deleted' },
                    { boxLabel: 'Failed', name: 'Failed' }
                ]
            });
            cfg.frmFileStatusFldSet = Ext.create('Ext.form.FieldSet', {
                title: 'Status',
                collapsible: true,
                items: [cfg.frmFileStatus]
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmSubmittedBy,
                    cfg.frmGeneralDateRange,
                    cfg.frmFileStatusFldSet
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
    }//,
//    setSearchFromSavedSelections: function (item) {
//        var me = this, cfg = me.config;
//        cfg.frmGeneralProviderSelector.resetValueFromWebService();
//        cfg.frmGeneralContractSelector.resetValueFromWebService();
//    }
});


