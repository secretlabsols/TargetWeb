Ext.define("TargetExt.DeceasedWorktray.MultiRecordEditor", {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton'
    ],
    closeAction: 'hide',
    title: 'Edit All Deceased Case(s)',
    resizable: false,
    width: 500,
    modal: true,
    config: {
        currentItem: null,
        hasUpdated: false,
        statusColl: null,
        subStatusColl: null,
        webSecurityUserID: 0
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config, controls = [];
        cfg.service = Target.Abacus.Library.WorkTrayDeceased.Services.WorkTrayDeceased;
        me.setupForm();
        me.bbar = [
            { xtype: 'tbfill' },
            cfg.formSave
        ];
        me.items = [
            cfg.frmGeneral
        ];
        me.callParent(arguments);
    },
    setValues: function () {
        var me = this, cfg = me.config;
        cfg.frmGeneralprocessStatusCollCmb.setValue();
        cfg.frmGeneralprocessSubStatusCollCmb.setValue();
        cfg.frmGeneralWebSecurityUser.setValue();
    },
    setupForm: function () {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            cfg.frmGeneralWebSecurityUser = Ext.create('InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector',
            {
                fieldLabel: 'Assigned To',
                labelWidth: 100,
                anchor: '100%',
                name: 'WebSecurityUser',
                searchTextBoxConfig: {
                    allowBlank: true
                },
                webSecurityUserID: cfg.webSecurityUserID
            });

            cfg.frmGeneralprocessStatusColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description', 'Type'],
                data: cfg.statusColl,
                remoteSort: false
            });
            cfg.frmGeneralprocessStatusColl.sort('Description', 'ASC');
            cfg.frmGeneralprocessStatusCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Status',
                store: cfg.frmGeneralprocessStatusColl,
                multiSelect: false,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'Status',
                emptyText: 'Select an item ....',
                plugins: ['clearbutton'],
                allowBlank: false,
                listeners: {
                    change: {
                        fn: function () {
                            me.resetFormVisibility();
                        }
                    }
                }
            });

            cfg.frmGeneralprocessSubStatusColl = Ext.create('Ext.data.Store', {
                fields: ['ID', 'Description'],
                data: cfg.subStatusColl,
                remoteSort: false
            });
            cfg.frmGeneralprocessSubStatusColl.sort('Description', 'ASC');
            cfg.frmGeneralprocessSubStatusCollCmb = Ext.create('Ext.form.ComboBox', {
                fieldLabel: 'Sub Status',
                store: cfg.frmGeneralprocessSubStatusColl,
                multiSelect: false,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                labelWidth: 100,
                anchor: '100%',
                name: 'SubStatus',
                emptyText: 'Select an item ....',
                plugins: ['clearbutton']
            });

            cfg.formSave = Ext.create('Ext.Button', {
                text: 'Update',
                cls: 'Selectors',
                iconCls: 'imgSave',
                tooltip: 'Update?',
                handler: function () {
                    me.save();
                }
            });

            // create the form with all items
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                   cfg.frmGeneralprocessStatusCollCmb,
                   cfg.frmGeneralprocessSubStatusCollCmb,
                   cfg.frmGeneralWebSecurityUser
                ]
            });
        }
    },
    save: function () {
        var me = this, cfg = me.config;
        me.setLoading('Updating Status....');
        cfg.service.UpdateAllExceptions(me.params,
                                               cfg.frmGeneralprocessStatusCollCmb.getValue(),
                                               cfg.frmGeneralprocessSubStatusCollCmb.getValue(),
                                               cfg.frmGeneralWebSecurityUser.getValue().ID,
                                               me.saveCallBack, me);
    },
    saveCallBack: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.currentItem = response.value.Item;
        cfg.hasUpdated = true;
        me.close();
    },
    getHasUpdated: function () {
        var me = this, cfg = me.config;
        return cfg.hasUpdated;
    },
    show: function (args) {
        var me = this, cfg = me.config, mdl, statusItems = [];
        args = $.extend(true, {
            params: null
        }, args);
        if (!cfg.statusColl) {
            Ext.MessageBox.alert('Error', 'Status Collection must be passed in the config.');
            return false;
        }
        if (!cfg.subStatusColl) {
            Ext.MessageBox.alert('Error', 'Sub Status Collection must be passed in the config.');
            return false;
        }
        if (!args.params) {
            Ext.MessageBox.alert('Error', 'Filter criteria must be passed to the show function.');
            return false;
        }
        cfg.currentItem = args.item;
        me.setValues();
        cfg.hasUpdated = false;
        me.resetFormVisibility();
        if (cfg.frmGeneralprocessStatusCollCmb.getValue() == cfg.frmGeneralprocessStatusColl.findRecord('Type', 2).data.ID ||
            cfg.frmGeneralprocessStatusCollCmb.getValue() == cfg.frmGeneralprocessStatusColl.findRecord('Type', 4).data.ID) {
            cfg.frmGeneralprocessStatusCollCmb.setReadOnly(true);
        }
        else {
            cfg.frmGeneralprocessStatusColl.filter([{
                filterFn: function (item) {
                    return item.get("Type") < 3;
                }
            }]);

        }
        me.callParent();
    },
    resetFormVisibility: function () {
        var me = this, cfg = me.config, frm = cfg.frmGeneral.getForm();
        cfg.formSave.setDisabled(!frm.isValid());
    }

});