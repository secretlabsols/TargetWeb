Ext.define('Searchers.Controls.ServiceUserSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Target.ux.form.field.DateTime',
        'Target.ux.form.field.DateTimeRange',
        'Target.ux.form.field.TextFilter',
        'Target.ux.form.field.DateFilter'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'ServiceUserSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
            { name: 'NameSearchTypeID', mapping: 'NameSearchTypeID' },
            { name: 'NameSearchCriteria', mapping: 'NameSearchCriteria' },
            { name: 'ReferenceSearchTypeID', mapping: 'ReferenceSearchTypeID' },
            { name: 'ReferenceSearchCriteria', mapping: 'ReferenceSearchCriteria' },
            { name: 'NHSNumberSearchTypeID', mapping: 'NHSNumberSearchTypeID' },
            { name: 'NHSNumberSearchCriteria', mapping: 'NHSNumberSearchCriteria' },
            { name: 'AltRefSearchTypeID', mapping: 'AltRefSearchTypeID' },
            { name: 'AltRefSearchCriteria', mapping: 'AltRefSearchCriteria' },
            { name: 'CreditorRefSearchTypeID', mapping: 'CreditorRefSearchTypeID' },
            { name: 'CreditorRefSearchCriteria', mapping: 'CreditorRefSearchCriteria' },
            { name: 'DebtorRefSearchTypeID', mapping: 'DebtorRefSearchTypeID' },
            { name: 'DebtorRefSearchCriteria', mapping: 'DebtorRefSearchCriteria' },
            { name: 'BirthDateTo', mapping: 'BirthDateTo', type: 'date' },
            { name: 'BirthDateFrom', mapping: 'BirthDateFrom', type: 'date' },
            { name: 'BirthDateSearchTypeID', mapping: 'BirthDateSearchTypeID' },
            { name: 'DeathDateTo', mapping: 'DeathDateTo', type: 'date' },
            { name: 'DeathDateFrom', mapping: 'DeathDateFrom', type: 'date' },
            { name: 'DeathDateSearchTypeID', mapping: 'DeathDateSearchTypeID' },
            { name: 'AddressSearchTypeID', mapping: 'AddressSearchTypeID' },
            { name: 'AddressSearchCriteria', mapping: 'AddressSearchCriteria' },
            { name: 'PostcodeSearchTypeID', mapping: 'PostcodeSearchTypeID' },
            { name: 'PostcodeSearchCriteria', mapping: 'PostcodeSearchCriteria' }
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

            });
        }
        return returnRecord;
    },
    resetSearch: function () {
        var me = this, cfg = me.config;
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();

        cfg.nameFilter.setValue(4);
        cfg.nameFilter.resetSearchCriteria();
        cfg.referenceFilter.setValue(4);
        cfg.referenceFilter.resetSearchCriteria();
        cfg.nhsFilter.setValue(4);
        cfg.nhsFilter.resetSearchCriteria();
        cfg.altrefFilter.setValue(4);
        cfg.altrefFilter.resetSearchCriteria();
        cfg.creditorrefFilter.setValue(4);
        cfg.creditorrefFilter.resetSearchCriteria();
        cfg.debtorrefFilter.setValue(4);
        cfg.debtorrefFilter.resetSearchCriteria();
        cfg.birthDateFilter.setValue(2);
        cfg.birthDateFilter.setDateTo();
        cfg.birthDateFilter.setDateFrom();
        cfg.birthDateFilter.showDateTo(false);
        cfg.deathDateFilter.setValue(2);
        cfg.deathDateFilter.setDateTo();
        cfg.deathDateFilter.setDateFrom();
        cfg.deathDateFilter.showDateTo(false);
        cfg.addressFilter.setValue(1);
        cfg.addressFilter.resetSearchCriteria();
        cfg.postcodeFilter.setValue(4);
        cfg.postcodeFilter.resetSearchCriteria();

        cfg.addressFilter.setValue(1);
        cfg.postcodeFilter.setValue(3);

        this.search();

    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config, defaultLabelWidth = 100;
        if (!cfg.frmGeneral) {
            // creating data store
            cfg.nameFilter = Ext.create('Target.ux.form.field.TextFilter', {
                padding: '0 0 5 0',
                textFilterDropDownControlConfig: {
                    name: 'NameSearchTypeID',
                    fieldLabel: 'Name',
                    labelWidth: defaultLabelWidth
                },
                filterOptionStoreData: [
                     { value: 1, description: 'Contains' },
                     { value: 2, description: 'Ends With' },
                     { value: 3, description: 'Equals' },
                     { value: 4, description: 'Starts With' }
                ],
                textFilterTextboxControlConfig: {
                    name: 'NameSearchCriteria'
                }
            });

            // create combo box and attach search type data store
            cfg.referenceFilter = Ext.create('Target.ux.form.field.TextFilter', {
                padding: '0 0 5 0',
                textFilterDropDownControlConfig: {
                    name: 'ReferenceSearchTypeID',
                    fieldLabel: 'Reference',
                    labelWidth: defaultLabelWidth
                },
                filterOptionStoreData: [
                     { value: 1, description: 'Contains' },
                     { value: 2, description: 'Ends With' },
                     { value: 3, description: 'Equals' },
                     { value: 4, description: 'Starts With' }
                ],
                textFilterTextboxControlConfig: {
                    name: 'ReferenceSearchCriteria'
                }
            });

            // create combo box and attach search type data store
            cfg.nhsFilter = Ext.create('Target.ux.form.field.TextFilter', {
                padding: '0 0 5 0',
                textFilterDropDownControlConfig: {
                    name: 'NHSNumberSearchTypeID',
                    fieldLabel: 'NHS Number',
                    labelWidth: defaultLabelWidth
                },
                filterOptionStoreData: [
                     { value: 1, description: 'Contains' },
                     { value: 2, description: 'Ends With' },
                     { value: 3, description: 'Equals' },
                     { value: 4, description: 'Starts With' }
                ],
                textFilterTextboxControlConfig: {
                    name: 'NHSNumberSearchCriteria'
                }
            });

            // create combo box and attach search type data store
            cfg.altrefFilter = Ext.create('Target.ux.form.field.TextFilter', {
                padding: '0 0 5 0',
                textFilterDropDownControlConfig: {
                    name: 'AltRefSearchTypeID',
                    fieldLabel: 'Alternate Ref',
                    labelWidth: defaultLabelWidth
                },
                filterOptionStoreData: [
                     { value: 1, description: 'Contains' },
                     { value: 2, description: 'Ends With' },
                     { value: 3, description: 'Equals' },
                     { value: 4, description: 'Starts With' }
                ],
                textFilterTextboxControlConfig: {
                    name: 'AltRefSearchCriteria'
                }
            });
            // create combo box and attach search type data store
            cfg.creditorrefFilter = Ext.create('Target.ux.form.field.TextFilter', {
                padding: '0 0 5 0',
                textFilterDropDownControlConfig: {
                    name: 'CreditorRefSearchTypeID',
                    fieldLabel: 'Creditor Ref',
                    labelWidth: defaultLabelWidth
                },
                filterOptionStoreData: [
                     { value: 1, description: 'Contains' },
                     { value: 2, description: 'Ends With' },
                     { value: 3, description: 'Equals' },
                     { value: 4, description: 'Starts With' }
                ],
                textFilterTextboxControlConfig: {
                    name: 'CreditorRefSearchCriteria'
                }
            });

            // create combo box and attach search type data store
            cfg.debtorrefFilter = Ext.create('Target.ux.form.field.TextFilter', {
                padding: '0 0 5 0',
                textFilterDropDownControlConfig: {
                    name: 'DebtorRefSearchTypeID',
                    fieldLabel: 'Debtor Ref',
                    labelWidth: defaultLabelWidth
                },
                filterOptionStoreData: [
                     { value: 1, description: 'Contains' },
                     { value: 2, description: 'Ends With' },
                     { value: 3, description: 'Equals' },
                     { value: 4, description: 'Starts With' }
                ],
                textFilterTextboxControlConfig: {
                    name: 'DebtorRefSearchCriteria'
                }
            });

            // create combo box and attach search type data store
            cfg.birthDateFilter = Ext.create('Target.ux.form.field.DateFilter', {
                dateFilterDropDownControlConfig: {
                    name: 'BirthDateSearchTypeID',
                    fieldLabel: 'Birth Date',
                    labelWidth: defaultLabelWidth
                },
                dateFromControlConfig: {
                    name: 'BirthDateFrom'
                },
                dateToControlConfig: {
                    name: 'BirthDateTo'
                }
            });

            // create combo box and attach search type data store
            cfg.deathDateFilter = Ext.create('Target.ux.form.field.DateFilter', {
                dateFilterDropDownControlConfig: {
                    name: 'DeathDateSearchTypeID',
                    fieldLabel: 'Death date',
                    labelWidth: defaultLabelWidth
                },
                dateFromControlConfig: {
                    name: 'DeathDateFrom'
                },
                dateToControlConfig: {
                    name: 'DeathDateTo'
                }
            });

            cfg.addressFilter = Ext.create('Target.ux.form.field.TextFilter', {
                padding: '0 0 5 0',
                textFilterDropDownControlConfig: {
                    name: 'AddressSearchTypeID',
                    fieldLabel: 'Address',
                    labelWidth: defaultLabelWidth
                },
                filterOptionStoreData: [
                     { value: 1, description: 'Contains' },
                     { value: 2, description: 'Ends With' },
                     { value: 3, description: 'Equals' },
                     { value: 4, description: 'Starts With' }
                ],
                textFilterTextboxControlConfig: {
                    name: 'AddressSearchCriteria'
                }
            });

            // create combo box and attach search type data store              
            cfg.postcodeFilter = Ext.create('Target.ux.form.field.TextFilter', {
                padding: '0 0 5 0',
                textFilterDropDownControlConfig: {
                    name: 'PostcodeSearchTypeID',
                    fieldLabel: 'Postcode',
                    labelWidth: defaultLabelWidth
                },
                filterOptionStoreData: [
                     { value: 1, description: 'Contains' },
                     { value: 2, description: 'Ends With' },
                     { value: 3, description: 'Equals' },
                     { value: 4, description: 'Starts With' }
                ],
                textFilterTextboxControlConfig: {
                    name: 'PostcodeSearchCriteria'
                }
            });



            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                       cfg.nameFilter,
                       cfg.referenceFilter,
                       cfg.nhsFilter,
                       cfg.altrefFilter,
                       cfg.creditorrefFilter,
                       cfg.debtorrefFilter,
                       cfg.birthDateFilter,
                       cfg.deathDateFilter,
                       cfg.addressFilter,
                       cfg.postcodeFilter
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
        if (mdl.data.NameSearchTypeID == 0) {
            cfg.nameFilter.setValue(4);
        }
        if (mdl.data.ReferenceSearchTypeID == 0) {
            cfg.referenceFilter.setValue(4);
        }
        if (mdl.data.NHSNumberSearchTypeID == 0) {
            cfg.nhsFilter.setValue(4);
        }
        if (mdl.data.AltRefSearchTypeID == 0) {
            cfg.altrefFilter.setValue(4);
        }
        if (mdl.data.CreditorRefSearchTypeID == 0) {
            cfg.creditorrefFilter.setValue(4);
        }
        if (mdl.data.DebtorRefSearchTypeID == 0) {
            cfg.debtorrefFilter.setValue(4);
        }
        if (mdl.data.BirthDateSearchTypeID == 0) {
            cfg.birthDateFilter.setValue(2);
        }
        if (mdl.data.DeathDateSearchTypeID == 0) {
            cfg.deathDateFilter.setValue(2);
        }
        if (mdl.data.AddressSearchTypeID == 0) {
            cfg.addressFilter.setValue(1);
        }
        if (mdl.data.PostcodeSearchTypeID == 0) {
            cfg.postcodeFilter.setValue(4);
        }
    }
});


