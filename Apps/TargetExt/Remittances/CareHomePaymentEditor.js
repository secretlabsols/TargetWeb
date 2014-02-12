﻿Ext.define("TargetExt.Remittances.CareHomePaymentEditor", {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton'
    ],
    closeAction: 'hide',
    title: 'Edit Care Home Payment',
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
        cfg.service = Target.Abacus.Library.Remittances.Services.RemittancesService;
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
        if (cfg.currentItem.ProcessStatusID != 0) {
            cfg.frmGeneralprocessStatusCollCmb.setValue(cfg.currentItem.ProcessStatusID);
        }
        if (cfg.currentItem.ProcessSubStatusID != 0) {
            cfg.frmGeneralprocessSubStatusCollCmb.setValue(cfg.currentItem.ProcessSubStatusID);
        }
        if (cfg.currentItem.AssignedToWebSecurityUserID != 0) {
            cfg.frmGeneralWebSecurityUser.setValueFromWebService(cfg.currentItem.AssignedToWebSecurityUserID);
        }
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
        me.setLoading('Updating payment....');
        cfg.service.EditRemittance(cfg.frmGeneralprocessStatusCollCmb.getValue(),
                                    cfg.frmGeneralprocessSubStatusCollCmb.getValue(),
                                    cfg.currentItem.ID, cfg.frmGeneralWebSecurityUser.getValue().ID,
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
    getProcessStatusByType: function (type) {
        var me = this, cfg = me.config;
        return cfg.frmGeneralprocessStatusColl.findRecord('Type', type, false, false, false, true).data;
    },
    show: function (args) {
        var me = this, cfg = me.config, mdl, statusItems = [], currentStatus, isPubMedBedsPaySch, isBedsHome, isStatusProvisionalCreated, isStatusProvisionalPublished, isStatusProvisionalRejected;
        args = $.extend(true, {
            item: null
        }, args);
        if (!cfg.statusColl) {
            Ext.MessageBox.alert('Error', 'Status Collection must be passed in the config.');
            return false;
        }
        if (!cfg.subStatusColl) {
            Ext.MessageBox.alert('Error', 'Sub Status Collection must be passed in the config.');
            return false;
        }
        if (!args.item) {
            Ext.MessageBox.alert('Error', 'Item must be passed to the show function.');
            return false;
        }
        cfg.currentItem = args.item;
        cfg.hasUpdated = false;
        me.resetFormVisibility();
        cfg.frmGeneralprocessStatusColl.clearFilter();
        cfg.frmGeneralprocessStatusCollCmb.setReadOnly(true);
        me.setValues();
        currentStatus = cfg.frmGeneralprocessStatusCollCmb.getValue();
        isPubMedBedsPaySch = args.item = (args.item.PublicationMedium == 'BEDS Payment Schedule');
        isBedsHome = args.item.BEDSHome;
        isStatusProvisionalCreated = (currentStatus == me.getProcessStatusByType(Selectors.CareHomePaymentSelectorProcessStatuses.ProvisionalCreated).ID);
        isStatusProvisionalPublished = (currentStatus == me.getProcessStatusByType(Selectors.CareHomePaymentSelectorProcessStatuses.ProvisionalPublished).ID);
        isStatusProvisionalRejected = (currentStatus == me.getProcessStatusByType(Selectors.CareHomePaymentSelectorProcessStatuses.ProvisionalRejected).ID);
        if (isStatusProvisionalCreated || ((isPubMedBedsPaySch || isBedsHome) && (isStatusProvisionalPublished || isStatusProvisionalRejected))) {
            cfg.frmGeneralprocessStatusColl.filter([{
                filterFn: function (item) {
                    var filterType = item.get("Type");
                    if (isStatusProvisionalCreated) {
                        return filterType < Selectors.CareHomePaymentSelectorProcessStatuses.ProvisionalPublished;
                    } else {
                        return (item.get("ID") == currentStatus || filterType == Selectors.CareHomePaymentSelectorProcessStatuses.ProvisionalApproved);
                    }
                }
            }]);
            cfg.frmGeneralprocessStatusCollCmb.setReadOnly(false);
        }
        me.callParent();
    },
    resetFormVisibility: function () {
        var me = this, cfg = me.config, frm = cfg.frmGeneral.getForm();
        cfg.formSave.setDisabled(!frm.isValid());
    }
});