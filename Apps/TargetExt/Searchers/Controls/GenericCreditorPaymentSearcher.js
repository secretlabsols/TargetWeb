Ext.define('Searchers.Controls.GenericCreditorPaymentSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTimeRange',
        'InPlaceSelectors.Controls.GenericCreditorInPlaceSelector',
        'InPlaceSelectors.Controls.GenericContractInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'GenericCreditorPaymentSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'DatePeriodType', mapping: 'DatePeriodType' },
                { name: 'Excluded', mapping: 'Excluded' },
                { name: 'GenericContract', mapping: 'GenericContract' },
                { name: 'GenericCreditor', mapping: 'GenericCreditor' },
                { name: 'StatusDateFrom', mapping: 'StatusDateFrom', type: 'date' },
                { name: 'StatusDateTo', mapping: 'StatusDateTo', type: 'date' },
                { name: 'StatusDatePeriodType', mapping: 'StatusDatePeriodType' },
                { name: 'StatusIncludeAuthorised', mapping: 'StatusIncludeAuthorised' },
                { name: 'StatusIncludePaid', mapping: 'StatusIncludePaid' },
                { name: 'StatusIncludeSuspendedAutomaticallyFlag', mapping: 'StatusIncludeSuspendedAutomaticallyFlag' },
                { name: 'StatusIncludeSuspendedManuallyFlag', mapping: 'StatusIncludeSuspendedManuallyFlag' },
                { name: 'StatusIncludeUnpaid', mapping: 'StatusIncludeUnpaid' },
                { name: 'SuspensionReasonFlags', mapping: 'SuspensionReasonFlags' },
                { name: 'Types', mapping: 'Types' }
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
    defaultCommitmentIndicatorsFieldValue: function (mdl) {
        var me = this, cfg = me.config;
        if (mdl.data.SuspensionReasonFlags == 0) {
            mdl.set('SuspensionReasonFlags', null);
        }
    },
    defaultExcludedModelFieldValue: function (mdl) {
        var me = this, cfg = me.config, excluded = [];
        cfg.paymentExclusionTypeDefaults = [1, 0];
        if (mdl.data.Excluded === false) {
            excluded.push(0);
        } else if (mdl.data.Excluded === true) {
            excluded.push(1);
        } else {
            excluded = cfg.paymentExclusionTypeDefaults;
        }
        mdl.set('Excluded', excluded);
    },
    getSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm(), searchRecord, returnRecord;
        if (frmGeneral.isValid()) {
            searchRecord = frmGeneral.getRecord();
            frmGeneral.updateRecord(searchRecord);
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {
                GenericContractID: searchRecord.data.GenericContract.ID,
                GenericCreditorID: searchRecord.data.GenericCreditor.ID
            });
            // default the exlude to a nullable boolean
            returnRecord.Excluded = (returnRecord.Excluded || []);
            if (returnRecord.Excluded.length == 2 || returnRecord.Excluded.length == 0) {
                returnRecord.Excluded = null;
            } else {
                returnRecord.Excluded = Ext.Array.contains(returnRecord.Excluded, 1);
            }
            // default the suspended status based on both flags
            returnRecord.StatusIncludeSuspended = (searchRecord.data.StatusIncludeSuspendedAutomaticallyFlag || searchRecord.data.StatusIncludeSuspendedManuallyFlag);
            if (returnRecord.StatusIncludeSuspendedAutomaticallyFlag && !returnRecord.StatusIncludeSuspendedManuallyFlag) {
                returnRecord.StatusIncludeSuspendedManually = false;
            } else if (!returnRecord.StatusIncludeSuspendedAutomaticallyFlag && returnRecord.StatusIncludeSuspendedManuallyFlag) {
                returnRecord.StatusIncludeSuspendedManually = true;
            } else {
                returnRecord.StatusIncludeSuspendedManually = null;
            }
            returnRecord.SuspensionReasons = Ext.Array.sum((returnRecord.SuspensionReasonFlags || []));
            returnRecord.TypesIncludeDirectPayments = Ext.Array.contains(returnRecord.Types, Selectors.GenericCreditorPaymentSelectorTypes.DirectPayment);
            returnRecord.TypesIncludeNonResidentialPayments = Ext.Array.contains(returnRecord.Types, Selectors.GenericCreditorPaymentSelectorTypes.NonResidential);
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
        searchRecord.data.DateFrom = Ext.Date.add(new Date(), Ext.Date.YEAR, -1);
        searchRecord.data.DatePeriodType = 0;
        searchRecord.data.DateTo = null;
        searchRecord.data.Excluded = cfg.paymentExclusionTypeDefaults;
        searchRecord.data.GenericContract = null;
        searchRecord.data.GenericContractID = 0;
        searchRecord.data.GenericCreditor = null;
        searchRecord.data.GenericCreditorID = 0;
        searchRecord.data.StatusDateFrom = null;
        searchRecord.data.StatusDatePeriodType = 0;
        searchRecord.data.StatusDateTo = null;
        searchRecord.data.StatusIncludeAuthorised = true;
        searchRecord.data.StatusIncludePaid = true;
        searchRecord.data.StatusIncludeSuspended = true;
        searchRecord.data.StatusIncludeSuspendedAutomaticallyFlag = true;
        searchRecord.data.StatusIncludeSuspendedManuallyFlag = true;
        searchRecord.data.StatusIncludeUnpaid = true;
        searchRecord.data.SuspensionReasonFlags = null;
        searchRecord.data.SuspensionReasons = null;
        searchRecord.data.Types = cfg.availablePaymentTypes;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            var availablePaymentTypeParam = me.resultSettings.GetSearchParameterValue('AvailablePaymentTypes', null), labelWidth = 120;
            var availablePaymentTypes = [], availablePaymentTypesNeither = (!availablePaymentTypeParam.HasDirectPayments && !availablePaymentTypeParam.HasNonResPayments);
            var paymentFieldSetItems = [];
            // determine the payment types to output
            if (availablePaymentTypeParam.HasDirectPayments || availablePaymentTypesNeither) {
                availablePaymentTypes.push({ ID: Selectors.GenericCreditorPaymentSelectorTypes.DirectPayment, Description: 'Direct Payment' });
            }
            if (availablePaymentTypeParam.HasNonResPayments || availablePaymentTypesNeither) {
                availablePaymentTypes.push({ ID: Selectors.GenericCreditorPaymentSelectorTypes.NonResidential, Description: 'Non-Residential' });
            }
            cfg.availablePaymentTypes = Ext.Array.pluck(availablePaymentTypes, 'ID');
            // create payment types
            cfg.frmPaymentTypesStore = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: availablePaymentTypes
            });
            cfg.frmPaymentTypesCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Type',
                store: cfg.frmPaymentTypesStore,
                queryMode: 'local',
                displayField: 'Description',
                labelWidth: labelWidth,
                anchor: '100%',
                valueField: 'ID',
                multiSelect: true,
                name: 'Types',
                value: cfg.availablePaymentTypes,
                listeners: {
                    boxready: {
                        fn: function (cmp) {
                            me.setCommitmentIndicatorsAvailability();
                        },
                        scope: me
                    },
                    change: {
                        fn: function (cmp) {
                            var types = (cmp.getValue() || []);
                            if (types.length == 0) {
                                cmp.setValue(cfg.availablePaymentTypes);
                            }
                            me.setCommitmentIndicatorsAvailability();
                        },
                        scope: me
                    }
                },
                editable: false
            });
            paymentFieldSetItems.push(cfg.frmPaymentTypesCmb);
            // create suspension reasons if required
            if (availablePaymentTypeParam.HasNonResPayments) {
                cfg.frmNonResSuspensionReasonsStore = Ext.create('Ext.data.Store', {
                    fields: ['ID', 'Description'],
                    data: [
                        { ID: 1, Description: 'Within Commitment' },
                        { ID: 2, Description: 'Different Service Delivered within Cost' },
                        { ID: 4, Description: 'Cost Exceeded' },
                        { ID: 8, Description: 'Excess Tolerated' },
                        { ID: 16, Description: 'No Service Order' }
                      ]
                });
                cfg.frmNonResSuspensionReasonsCmb = Ext.create('Ext.form.ComboBox', {
                    editable: false,
                    fieldLabel: 'Commitment Indicators',
                    store: cfg.frmNonResSuspensionReasonsStore,
                    queryMode: 'local',
                    displayField: 'Description',
                    labelWidth: labelWidth,
                    anchor: '100%',
                    valueField: 'ID',
                    multiSelect: true,
                    name: 'SuspensionReasonFlags',
                    plugins: ['clearbutton']
                });
                paymentFieldSetItems.push(cfg.frmNonResSuspensionReasonsCmb);
            }
            // create payment date range field
            cfg.frmDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                fieldLabel: 'Payment Period',
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
                labelWidth: labelWidth,
                rangeOptionStoreData: [
                    { value: 0, description: 'Covering part or all of the period' }
                ]
            });
            paymentFieldSetItems.push(cfg.frmDateRange);
            // create creditor in place selector
            cfg.frmCreditorSelector = Ext.create('InPlaceSelectors.Controls.GenericCreditorInPlaceSelector', {
                fieldLabel: 'Creditor',
                labelWidth: labelWidth,
                name: 'GenericCreditor',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            paymentFieldSetItems.push(cfg.frmCreditorSelector);
            // create contract in place selector
            cfg.frmContractSelector = Ext.create('InPlaceSelectors.Controls.GenericContractInPlaceSelector', {
                fieldLabel: 'Contract',
                labelWidth: labelWidth,
                name: 'GenericContract',
                searchTextBoxConfig: {
                    allowBlank: true
                }
            });
            paymentFieldSetItems.push(cfg.frmContractSelector);
            cfg.frmCreditorSelector.linkToInPlaceContractSelector(cfg.frmContractSelector, args.SearcherSettings.Item.GenericCreditor.ID);   // link contracts selector to creditor selector
            // create payment types
            cfg.frmPaymentExlusionTypesStore = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: [
                    { ID: 1, Description: 'Excluded' },
                    { ID: 0, Description: 'Not Excluded' }
                  ]
            });
            cfg.frmPaymentExclusionCmb = Ext.create('Ext.form.ComboBox', {
                editable: false,
                fieldLabel: 'Excluded',
                store: cfg.frmPaymentExlusionTypesStore,
                queryMode: 'local',
                displayField: 'Description',
                labelWidth: labelWidth,
                anchor: '100%',
                valueField: 'ID',
                multiSelect: true,
                name: 'Excluded',
                value: cfg.paymentExclusionTypeDefaults,
                listeners: {
                    change: {
                        fn: function (cmp) {
                            var exclusions = (cmp.getValue() || []);
                            if (exclusions.length == 0) {
                                cmp.setValue(cfg.paymentExclusionTypeDefaults);
                            }
                        },
                        scope: me
                    }
                }
            });
            paymentFieldSetItems.push(cfg.frmPaymentExclusionCmb);
            // create payment field set
            cfg.frmPaymentFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Payment',
                items: paymentFieldSetItems,
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
            // create payment statuses check box group and 
            cfg.frmPaymentStatusesBxGroup = Ext.create('Ext.form.CheckboxGroup', {
                columns: 2,
                items: [
                    { boxLabel: 'Unpaid', name: 'StatusIncludeUnpaid' },
                    { boxLabel: 'Suspended Manually', name: 'StatusIncludeSuspendedManuallyFlag' },
                    { boxLabel: 'Authorised', name: 'StatusIncludeAuthorised' },
                    { boxLabel: 'Suspended Automatically', name: 'StatusIncludeSuspendedAutomaticallyFlag' },
                    { boxLabel: 'Paid', name: 'StatusIncludePaid' }
                ],
                listeners: {
                    change: {
                        fn: function (cmp) {
                            if (cmp.getChecked().length == 0) {
                                cmp.setValue({
                                    StatusIncludeUnpaid: true,
                                    StatusIncludeSuspendedManuallyFlag: true,
                                    StatusIncludeAuthorised: true,
                                    StatusIncludeSuspendedAutomaticallyFlag: true,
                                    StatusIncludePaid: true
                                });
                            }
                        },
                        scope: me
                    }
                }
            });
            cfg.frmPaymentStatusesFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Status',
                items: [cfg.frmPaymentStatusesBxGroup],
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
            // create payment status date range field
            cfg.frmStatusDateRange = Ext.create('Target.ux.form.field.DateTimeRange', {
                showTime: true,
                dateRangeDateTimeFromControlConfig: {
                    name: 'StatusDateFrom'
                },
                dateRangeDateTimeToControlConfig: {
                    name: 'StatusDateTo'
                },
                dateRangeDropDownControlConfig: {
                    name: 'StatusDatePeriodType'
                },
                labelWidth: labelWidth,
                rangeOptionStoreData: [
                    { value: 0, description: 'Falling within the period' }
                ]
            });
            // create payment status field set
            cfg.frmPaymentStatusFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Payment Status',
                items: [
                    cfg.frmPaymentStatusesFlds,
                    cfg.frmStatusDateRange
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
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmPaymentFlds,
                    cfg.frmPaymentStatusFlds
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
    setCommitmentIndicatorsAvailability: function () {
        var me = this, cfg = me.config;
        if (cfg.frmNonResSuspensionReasonsCmb) {
            var selectedPaymentTypes = (cfg.frmPaymentTypesCmb.getValue() || []);
            var hasOnlyNonRes = (selectedPaymentTypes.length == 1 && Ext.Array.contains(selectedPaymentTypes, Selectors.GenericCreditorPaymentSelectorTypes.NonResidential));
            cfg.frmNonResSuspensionReasonsCmb.setDisabled(!hasOnlyNonRes);
            if (!hasOnlyNonRes) {
                cfg.frmNonResSuspensionReasonsCmb.setValue();
            }
        }
    },
    setSearch: function (item) {
        var me = this, cfg = me.config, mdl = Ext.create(cfg.modelName, item);
        me.defaultCommitmentIndicatorsFieldValue(mdl);
        me.defaultExcludedModelFieldValue(mdl);
        cfg.frmGeneral.loadRecord(mdl);
    },
    setSearchFromSavedSelections: function (item) {
        var me = this, cfg = me.config;
        cfg.frmCreditorSelector.resetValueFromWebService();
        cfg.frmContractSelector.resetValueFromWebService();
    }
});


