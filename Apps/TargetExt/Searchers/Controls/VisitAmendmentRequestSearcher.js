Ext.define('Searchers.Controls.VisitAmendmentRequestSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.GenericContractInPlaceSelector',
        'InPlaceSelectors.Controls.ProviderInPlaceSelector',
        'InPlaceSelectors.Controls.ServiceUserInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'VisitAmendmentRequestSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                            { name: 'Provider', mapping: 'Provider' },
                            { name: 'GenericContract', mapping: 'GenericContract' },
                            { name: 'Client', mapping: 'Client' },
                            { name: 'OrigCouncil', mapping: 'OrigCouncil' },
                            { name: 'OrigProvider', mapping: 'OrigProvider' },
                            { name: 'RequestDateFrom', mapping: 'RequestDateFrom', type: 'date' },
                            { name: 'RequestDateTo', mapping: 'RequestDateTo', type: 'date' },
                            { name: 'RequestDatePeriodType', mapping: 'RequestDatePeriodType' },
                            { name: 'StatusDateFrom', mapping: 'StatusDateFrom', type: 'date' },
                            { name: 'StatusDateTo', mapping: 'StatusDateTo', type: 'date' },
                            { name: 'RequestByID', mapping: 'RequestByID' },
                            { name: 'StatusAwaiting', mapping: 'StatusAwaiting' },
                            { name: 'StatusDeclined', mapping: 'StatusDeclined' },
                            { name: 'StatusVerified', mapping: 'StatusVerified' },
                            { name: 'StatusProcessed', mapping: 'StatusProcessed' },
                            { name: 'StatusInvoiced', mapping: 'StatusInvoiced' }
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
                ProviderID: searchRecord.data.Provider.ID,
                RequestDateFrom: searchRecord.data.RequestDateFrom,
                RequestDateTo: searchRecord.data.RequestDateTo,
                Status: searchRecord.data.Status,
                StatusDateFrom: searchRecord.data.StatusDateFrom,
                StatusDateTo: searchRecord.data.StatusDateTo,
                RequestbyCompanyID: searchRecord.data.RequestbyCompanyID,
                RequestByUserID: searchRecord.data.RequestByUserID,
                ClientID: searchRecord.data.Client.ID
            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.GenericContract = null;
        searchRecord.data.GenericContractID = 0;
        searchRecord.data.Provider = null;
        searchRecord.data.ProviderID = 0;
        searchRecord.data.Client = null;
        searchRecord.data.ClientID = 0;
        searchRecord.data.OrigCouncil = true;
        searchRecord.data.OrigProvider = true;
        searchRecord.data.RequestDateFrom = null;
        searchRecord.data.RequestDateTo = null;
        searchRecord.data.StatusDateFrom = null;
        searchRecord.data.StatusDateTo = null;
        searchRecord.data.PaymentScheduleID = 0;
        searchRecord.data.StatusAwaiting = true;
        searchRecord.data.StatusDeclined = true;
        searchRecord.data.StatusVerified = true;
        searchRecord.data.StatusProcessed = true;
        searchRecord.data.StatusInvoiced = true;
        searchRecord.data.RequestByID = 0;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            var CouncilUsers = args.GetSearchParameterValue('CouncilUsers', []);  // get contract types from params
            var AccessibleProviders = args.GetSearchParameterValue('AccessibleProviders', []);  // get contract groups from params

            cfg.frmGeneralOrigCouncilUserStore = Ext.create('Ext.data.Store', {
                fields: ['Value', 'Key'],
                data: CouncilUsers,
                remoteSort: false
            });

            cfg.frmGeneralOrigProviderStore = Ext.create('Ext.data.Store', {
                fields: ['Value', 'Key'],
                data: AccessibleProviders,
                remoteSort: false
            });

            cfg.frmGeneralRequestedByCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Requested By',
                displayField: 'Value',
                store: cfg.frmGeneralOrigCouncilUserStore,
                valueField: 'Key',
                queryMode: 'local',
                labelWidth: 100,
                anchor: '100%',
                name: 'RequestByID',
                plugins: ['clearbutton'],
                multiSelect: false,
                disabled: true
            });

            // create provider in place selector
            cfg.frmGeneralProviderSelector = Ext.create('InPlaceSelectors.Controls.ProviderInPlaceSelector', {
                fieldLabel: 'Provider',
                name: 'Provider',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            cfg.frmGeneralProviderSelector.getSelector().SetIncludeRedundant(true);
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

            // create contract in place selector
            cfg.frmGeneralServiceUserSelector = Ext.create('InPlaceSelectors.Controls.ServiceUserInPlaceSelector', {
                fieldLabel: 'Service User',
                name: 'Client',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });

            cfg.frmGeneralServiceUserSelector.getSelector().SetShowNonResidential(true);
            // show only service users that are linked with this Provider
            cfg.frmGeneralProviderSelector.linkToInPlaceServiceUserSelector(cfg.frmGeneralServiceUserSelector, args.SearcherSettings.Item.Provider.ID);   // link provider selector to Service user selector
            cfg.frmGeneralContractSelector.linkToInPlaceServiceUserSelector(cfg.frmGeneralServiceUserSelector, args.SearcherSettings.Item.GenericContract.ID);   // link contracts selector to Service user selector

            cfg.frmGeneralVisitDetailsOriginator = Ext.create('Ext.form.CheckboxGroup', {
                columns: 2,
                margin: '0 0 0 27',
                items: [
                    { boxLabel: 'Council', name: 'OrigCouncil', checked: true, width: 93 },
                    { boxLabel: 'Provider Organisation', name: 'OrigProvider', checked: true, width: 165 }
                ],
                listeners: {
                    change: function (field, newValue, oldValue, eOpts) {
                        me.setRequestedBy(newValue, oldValue);
                    }
                }
            });

            // create date range field            
            cfg.frmGeneralRequestDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Requested Date Period',
                dateRangeDateTimeFromControlConfig: {
                    dateControlConfig: {
                        name: 'RequestDateFrom'
                    }
                },
                dateRangeDateTimeToControlConfig: {
                    dateControlConfig: {
                        name: 'RequestDateTo'
                    }
                },
                dateRangeDropDownControlConfig: {
                    name: 'DatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Covering part or all of the period' }
                ]
            });
            cfg.pnl = Ext.create('Ext.container.Container', {
                layout: {
                    type: 'hbox',
                    pack: 'start',
                    align: 'stretch'
                },
                items: [
                    {
                        xtype: 'label',
                        text: 'Originator',
                        cls: 'x-form-item SenchaBold'
                    },
                    cfg.frmGeneralVisitDetailsOriginator
                ]
            })

            cfg.frmGeneralvisitDetailsFldSet = Ext.create('Ext.form.FieldSet', {
                title: 'Visit Details:',
                collapsible: true,
                items: [
                        cfg.pnl,
                        cfg.frmGeneralRequestedByCollCmb,
                        cfg.frmGeneralRequestDateRange
                ]//,
            });

            cfg.frmGeneralVisitAmendmentstatus = Ext.create('Ext.form.CheckboxGroup', {
                columns: 2,
                items: [
                    { boxLabel: 'Awaiting Verification', name: 'StatusAwaiting', checked: true },
                    { boxLabel: 'Declined', name: 'StatusDeclined', checked: true },
                    { boxLabel: 'Verified', name: 'StatusVerified', checked: true },
                    { boxLabel: 'Processed', name: 'StatusProcessed', checked: true },
                    { boxLabel: 'Invoiced', name: 'StatusInvoiced', checked: true }
                ]
            });

            cfg.frmGeneralStatusDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Status Date Period',
                dateRangeDateTimeFromControlConfig: {
                    dateControlConfig: {
                        name: 'StatusDateFrom'
                    }
                },
                dateRangeDateTimeToControlConfig: {
                    dateControlConfig: {
                        name: 'StatusDateTo'
                    }
                },
                dateRangeDropDownControlConfig: {
                    name: 'DatePeriodType'
                },
                rangeOptionStoreData: [
                    { value: 0, description: 'Covering part or all of the period' }
                ]
            });

            cfg.frmGeneralvisiVisitamendmentStatusFldSet = Ext.create('Ext.form.FieldSet', {
                title: 'Visit Amendment Status:',
                collapsible: true,
                items: [
                         cfg.frmGeneralStatusDateRange,
                         cfg.frmGeneralVisitAmendmentstatus
                ]//,
            });
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralProviderSelector,
                    cfg.frmGeneralContractSelector,
                    cfg.frmGeneralServiceUserSelector,
                    cfg.frmGeneralvisitDetailsFldSet,
                    cfg.frmGeneralvisiVisitamendmentStatusFldSet
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
    },
    setRequestedBy: function (newValue, oldValue) {
        var me = this, cfg = me.config, frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();

        if ((newValue.OrigCouncil && newValue.OrigProvider) || (!newValue.OrigCouncil && !newValue.OrigProvider)) {
            searchRecord.data.RequestByID = 0;
            cfg.frmGeneralRequestedByCollCmb.setValue()
            me.setEnableDisableRequestedBy(true);
        } else {
            if (!newValue.OrigCouncil && newValue.OrigProvider) {
                //alert('OrigProvider checked');
                me.setEnableDisableRequestedBy(false);
                cfg.frmGeneralRequestedByCollCmb.bindStore(cfg.frmGeneralOrigCouncilUserStore);
            }
            if (newValue.OrigCouncil && !newValue.OrigProvider) {
                //alert('OrigCouncil checked');
                me.setEnableDisableRequestedBy(false);
                cfg.frmGeneralRequestedByCollCmb.bindStore(cfg.frmGeneralOrigProviderStore);
            }

        }
    },
    setEnableDisableRequestedBy: function (disabled) {
        var me = this, cfg = me.config;
        cfg.frmGeneralRequestedByCollCmb.setDisabled(disabled);
    }
});


