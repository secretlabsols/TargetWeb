Ext.define('Searchers.Controls.RateCategorySearcher', {
    extend: 'Searchers.SearcherControl',
    requires: [
        'Target.ux.form.field.ClearButton',
        'InPlaceSelectors.Controls.RateFrameworkInPlaceSelector'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        // setup search model
        cfg.modelName = 'RateCategoriesSearchItem';
        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'RateFramework', mapping: 'RateFramework' }
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
                RateFrameworkID: searchRecord.data.RateFramework.ID
            });
        }
        if (!returnRecord || returnRecord.RateFrameworkID == 0) {
            returnRecord = (returnRecord || {});
            returnRecord.RateFrameworkID = -1;
        }
        return returnRecord;
    },
    resetSearch: function () {
        var frmGeneral = this.config.frmGeneral.getForm();
        var searchRecord = frmGeneral.getRecord();
        searchRecord = frmGeneral.getRecord();
        searchRecord.data.RateFramework = null;
        searchRecord.data.RateFrameworkID = 0;
        frmGeneral.loadRecord(searchRecord);
        this.search();
    },
    setupFrmGeneral: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {
            // create contract in place selector
            cfg.frmGeneralRateFrameworkSelector = Ext.create('InPlaceSelectors.Controls.RateFrameworkInPlaceSelector', {
                fieldLabel: 'Rate Framework',
                labelWidth: 100,
                anchor: '100%',
                name: 'RateFramework',
                searchTextBoxConfig: {
                    allowBlank: false
                },
                listeners: {
                    change: {
                        scope: me,
                        fn: function () {
                            me.search();
                        }
                    }
                },
                allowBlank: false
            });
            cfg.frmGeneralRateFrameworkSelector.getSelector().SetIncludeRedundant(false);
            // create form containing controls
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                title: 'General',
                items: [
                    cfg.frmGeneralRateFrameworkSelector,
                    cfg.frmGeneralRateCatSelector
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
        cfg.frmGeneralRateFrameworkSelector.resetValueFromWebService();
    }
});


