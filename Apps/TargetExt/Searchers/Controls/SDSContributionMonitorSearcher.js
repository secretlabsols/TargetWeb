Ext.define('Searchers.Controls.SDSContributionMonitorSearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.RadioGroup',
        'Target.ux.form.CheckboxGroup',
        'Ext.form.Label'
    ],
    initComponent: function () {
        var me = this, cfg = this.config;
        // setup search model
        cfg.modelName = 'SDSContributionMonitorSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'ServiceUsersAreBeingCollected', mapping: 'ServiceUsersAreBeingCollected' },
                { name: 'ServiceUsersRequiringContributionNotificationLetter', mapping: 'ServiceUsersRequiringContributionNotificationLetter' },
                { name: 'ServiceUsersHavingOneOrMoreIncompleteContributionLevel', mapping: 'ServiceUsersHavingOneOrMoreIncompleteContributionLevel' },
                { name: 'ServiceUsersHavingOneOrMissingOrProvAssessment', mapping: 'ServiceUsersHavingOneOrMissingOrProvAssessment' },
                { name: 'ServiceUsersHavingNilValueCostAssessment', mapping: 'ServiceUsersHavingNilValueCostAssessment' }
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
            returnRecord = $.extend(true, $.extend(true, {}, searchRecord.data), {});
        }
        return returnRecord;
    },
    frmGeneralContributionLetterCheckBoxGroupSetValue: function (chkOneOrMoreProvAssessmentChecked, chkNilValueCostAssessmentChecked) {
        var me = this, cfg = me.config;
        cfg.frmGeneralContributionLetterCheckBoxGroup.items.getByKey('chkOneOrMoreProvAssessment').setValue(chkOneOrMoreProvAssessmentChecked);
        cfg.frmGeneralContributionLetterCheckBoxGroup.items.getByKey('chkNilValueCostAssessment').setValue(chkNilValueCostAssessmentChecked);
    },
    frmGeneralContributionLetterCheckBoxGroupSetReadOnly: function (chkOneOrMoreProvAssessmentReadOnly, chkNilValueCostAssessmentReadOnly) {
        var me = this, cfg = me.config;
        cfg.frmGeneralContributionLetterCheckBoxGroup.items.getByKey('chkOneOrMoreProvAssessment').setReadOnly(chkOneOrMoreProvAssessmentReadOnly);
        cfg.frmGeneralContributionLetterCheckBoxGroup.items.getByKey('chkNilValueCostAssessment').setReadOnly(chkNilValueCostAssessmentReadOnly);
    },
    resetSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.ServiceUsersAreBeingCollected = 0;
        searchRecord.data.ServiceUsersRequiringContributionNotificationLetter = 0;
        searchRecord.data.ServiceUsersHavingOneOrMoreIncompleteContributionLevel = 0;
        searchRecord.data.ServiceUsersHavingOneOrMissingOrProvAssessment = false;
        searchRecord.data.ServiceUsersHavingNilValueCostAssessment = false;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        // 'defer making changes to contribution levels until a notification letter has been produced' setting value
        cfg.deferChangesToContributionLevels = me.resultSettings.HasPermission('DeferChangesToContributionLevels');
        if (!cfg.frmGeneral) {

            cfg.frmGeneralContributionLetterCheckBoxGroup = Ext.create('Target.ux.form.CheckboxGroup', {
                columns: 1,
                vertical: true,
                items: [
                        {
                            boxLabel: 'Having one or more missing or Provisional Assessment; OR',
                            name: 'ServiceUsersHavingOneOrMissingOrProvAssessment',
                            id: 'chkOneOrMoreProvAssessment',
                            padding: '0 0 0 15'
                        },
                        {
                            boxLabel: 'Having a nil value cost',
                            name: 'ServiceUsersHavingNilValueCostAssessment',
                            id: 'chkNilValueCostAssessment',
                            padding: '0 0 0 15'
                        },
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
            })
            // create Collection of Contributions label
            cfg.frmGeneralCollectionofContributionsLabel = Ext.create('Ext.form.Label', {
                text: 'Collection of Contributions',
                width: 200,
                name: 'lblCollectionOfContributions',
                cls: 'HeadingLabel',
                padding: '10 0 0 0'
            });
            // create Collection of Contributions Radio botton group
            cfg.frmGeneralCollectionofContributionsRadioGroup = Ext.create('Target.ux.form.RadioGroup', {
                columns: 1,
                vertical: true,
                name: 'ServiceUsersAreBeingCollected',
                defaults: {
                    name: 'ServiceUsersAreBeingCollected'
                },
                items: [
                        { boxLabel: 'Do not filter by this item', inputValue: 0 },
                        { boxLabel: 'Show service users from which contributions are <b><u>not</u></b> being collected', inputValue: 2 },
                        { boxLabel: 'Show service users from which contributions are being collected', inputValue: 1 }
                    ]
            });
            // create Contribution Letter label
            cfg.frmGeneralContributionLetterLabel = Ext.create('Ext.form.Label', {
                text: 'Contribution Letter',
                width: 200,
                name: 'lblContributionLetter',
                cls: 'HeadingLabel'
            });
            // set visibility of frmGeneralContributionLetterRadioGroup based on the value of Defer Changes To Contributions Levels system setting
            cfg.frmGeneralContributionLetterLabel.setVisible(cfg.deferChangesToContributionLevels);
            // create Contribution Letter Radio botton group
            cfg.frmGeneralContributionLetterRadioGroup = Ext.create('Target.ux.form.RadioGroup', {
                columns: 1,
                vertical: true,
                name: 'ServiceUsersRequiringContributionNotificationLetter',
                height: 100,
                defaults: {
                    name: 'ServiceUsersRequiringContributionNotificationLetter'
                },
                items: [
                    { boxLabel: 'Do not filter by this item', inputValue: 0 },
                    { boxLabel: 'Show service users requiring a Contribution Notification letter', inputValue: 1 },
                    { boxLabel: 'Show service users who are <b><u>not</u></b> due a Contribution Notification letter', inputValue: 2 }
                ]
            });
            // set visibility of frmGeneralContributionLetterRadioGroup based on the value of Defer Changes To Contributions Levels system setting
            cfg.frmGeneralContributionLetterRadioGroup.setVisible(cfg.deferChangesToContributionLevels);
            // create Pending Contribution Levels label if Defer Changes To Contribution Levels System setting is true
            cfg.frmGeneralContributionLevelsLabel = Ext.create('Ext.form.Label', {
                // set label based on the value of Defer Changes To Contributions Levels system setting
                text: cfg.deferChangesToContributionLevels ? 'Pending Contribution Levels' : 'Active Contribution Levels',
                width: 200,
                name: 'lblContributionLevels',
                cls: 'HeadingLabel'
            });
            // create Pending Contribution Levels Radio botton group
            cfg.frmGeneralPendingContributionLevelsRadioGroup = Ext.create('Target.ux.form.RadioGroup', {
                columns: 1,
                vertical: true,
                height: 150,
                name: 'ServiceUsersHavingOneOrMoreIncompleteContributionLevel',
                items: [
                    {
                        boxLabel: 'Do not filter by this item',
                        name: 'ServiceUsersHavingOneOrMoreIncompleteContributionLevel',
                        inputValue: 0,
                        id: 'rdoDoNotFilter',
                        handler: function () {
                            if (this.getValue()) {
                                me.frmGeneralContributionLetterCheckBoxGroupSetValue(false, false);
                                me.frmGeneralContributionLetterCheckBoxGroupSetReadOnly(true, true);
                            }
                        }
                    },
                    {
                        boxLabel: 'Show service users having one or more incomplete Contribution Level',
                        name: 'ServiceUsersHavingOneOrMoreIncompleteContributionLevel',
                        inputValue: 1,
                        id: 'rdoOneOrMoreIncompleteContributionLevel',
                        handler: function () {
                            if (this.getValue()) {
                                me.frmGeneralContributionLetterCheckBoxGroupSetValue(true, true);
                                me.frmGeneralContributionLetterCheckBoxGroupSetReadOnly(false, false);
                            }
                        }
                    },
                // create Pending Contribution Levels check box group
                    cfg.frmGeneralContributionLetterCheckBoxGroup,
                    {
                        boxLabel: 'Show service users having \'pending\' Contribution Levels that are <u>all</u> complete',
                        name: 'ServiceUsersHavingOneOrMoreIncompleteContributionLevel',
                        inputValue: 2,
                        id: 'rdoPendingContributionLevel',
                        handler: function () {
                            if (this.getValue()) {
                                me.frmGeneralContributionLetterCheckBoxGroupSetValue(false, false);
                                me.frmGeneralContributionLetterCheckBoxGroupSetReadOnly(true, true);
                            }
                        }
                    }
                ]
            });
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                        cfg.frmGeneralCollectionofContributionsLabel,
                        cfg.frmGeneralCollectionofContributionsRadioGroup,
                        cfg.frmGeneralContributionLetterLabel,
                        cfg.frmGeneralContributionLetterRadioGroup,
                        cfg.frmGeneralContributionLevelsLabel,
                        cfg.frmGeneralPendingContributionLevelsRadioGroup
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
    }
});


