Ext.define('Searchers.Controls.ProviderContractSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector',
        'InPlaceSelectors.Controls.ServiceGroupInPlaceSelector'
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
                { name: 'ServiceGroup', mapping: 'ServiceGroup' },
                { name: 'ServiceGroupID', mapping: 'ServiceGroupID' },
                { name: 'ServiceGroup', mapping: 'ServiceGroup' },
                { name: 'ContractTypeStr', mapping: 'ContractTypeStr' },
                { name: 'ContractGroupID', mapping: 'ContractGroupID' },
                { name: 'EndReasonID', mapping: 'EndReasonID' },
                { name: 'AdministrativeSectorID', mapping: 'AdministrativeSectorID' }
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
                ServiceGroupID: searchRecord.data.ServiceGroup.ID
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var me = this, cfg = me.config;
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;
        searchRecord.data.ServiceGroup = null;
        searchRecord.data.ServiceGroup = 0;
        searchRecord.data.ContractTypeStr = '';
        searchRecord.data.ContractGroupID = 0;
        searchRecord.data.EndReasonID = 0;
        searchRecord.data.AdministrativeSectorID = 0;
        searchRecord.data.DateFrom = new Date();
        searchRecord.data.DateTo = null;
        cfg.frmGeneralContractTypesCmb.setValue();
        cfg.frmGeneralContractGroupsCmb.setValue();
        cfg.frmGeneralContractEndReasonCmb.setValue();
        cfg.frmAdministrativeSectorCmb.setValue();
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            var contractTypes = args.GetSearchParameterValue('ContractTypes', []);  // get contract types from params
            var contractGroups = args.GetSearchParameterValue('ContractGroups', []);  // get contract groups from params
            var contractEndReasons = args.GetSearchParameterValue('ContractEndReasons', []);  // get contract end reasons from params
            var administrativeSectors = args.GetSearchParameterValue('AdministrativeSectors', []);  // get contract end reasons from params

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
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeRedundant(true);

            cfg.frmGeneralServiceGroupSelector = Ext.create('InPlaceSelectors.Controls.ServiceGroupInPlaceSelector', {
                fieldLabel: 'Service Group',
                labelWidth: 100,
                anchor: '100%',
                name: 'ServiceGroup',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            cfg.frmGeneralServiceGroupSelector.getSelector().SetFilterbyUser(true);

            cfg.frmGeneralContractTypesStore = Ext.create('Ext.data.Store', {
                fields: ['Value', 'Key'],
                data: contractTypes,
                remoteSort: false
            });

            cfg.frmGeneralContractTypesCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Contract Type',
                store: cfg.frmGeneralContractTypesStore,
                queryMode: 'local',
                displayField: 'Value',
                valueField: 'Value',
                labelWidth: 100,
                anchor: '100%',
                name: 'ContractTypeStr',
                plugins: ['clearbutton']
            });

            cfg.frmGeneralContractGroupsStore = Ext.create('Ext.data.Store', {
                fields: ['Key', 'Value'],
                data: contractGroups,
                remoteSort: false
            });

            cfg.frmGeneralContractGroupsCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Contract Group',
                store: cfg.frmGeneralContractGroupsStore,
                queryMode: 'local',
                displayField: 'Value',
                valueField: 'Key',
                labelWidth: 100,
                anchor: '100%',
                name: 'ContractGroupID',
                plugins: ['clearbutton']
            });

            cfg.frmGeneralContractEndReasonStore = Ext.create('Ext.data.Store', {
                fields: ['Key', 'Value'],
                data: contractEndReasons,
                remoteSort: false
            });

            cfg.frmGeneralContractEndReasonCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'End Reason',
                store: cfg.frmGeneralContractEndReasonStore,
                queryMode: 'local',
                displayField: 'Value',
                valueField: 'Key',
                labelWidth: 100,
                anchor: '100%',
                name: 'EndReasonID',
                plugins: ['clearbutton']
            });

            cfg.frmAdministrativeSectorStore = Ext.create('Ext.data.Store', {
                fields: ['Key', 'Value'],
                data: administrativeSectors,
                remoteSort: false
            });

            cfg.frmAdministrativeSectorCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Administrative Areas',
                store: cfg.frmAdministrativeSectorStore,
                queryMode: 'local',
                displayField: 'Value',
                valueField: 'Key',
                labelWidth: 100,
                anchor: '100%',
                name: 'AdministrativeSectorID',
                plugins: ['clearbutton']
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
                    { value: 0, description: 'Covering any part of the period' }
                ]
            });

            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralServiceGroupSelector,
                    cfg.frmGeneralContractTypesCmb,
                    cfg.frmGeneralContractGroupsCmb,
                    cfg.frmGeneralContractEndReasonCmb,
                    cfg.frmAdministrativeSectorCmb,
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
        // load forms with data from model
        cfg.frmGeneral.loadRecord(mdl);
        // default values
        if (mdl.data.ContractTypeID == 0) {
            cfg.frmGeneralContractTypesCmb.setValue();
        }
        if (mdl.data.ContractGroupID == 0) {
            cfg.frmGeneralContractGroupsCmb.setValue();
        }
        if (mdl.data.EndReasonID == 0) {
            cfg.frmGeneralContractEndReasonCmb.setValue();
        }
        if (mdl.data.AdministrativeSectorID == 0) {
            cfg.frmAdministrativeSectorCmb.setValue();
        }
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
        cfg.frmGeneralProviderSelector.resetValueFromWebService();
        cfg.frmGeneralServiceGroupSelector.resetValueFromWebService();
    }
});


