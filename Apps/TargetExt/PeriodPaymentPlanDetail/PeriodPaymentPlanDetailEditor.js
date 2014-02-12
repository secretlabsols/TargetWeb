/// <reference path="../../../Library/JavaScript/ExtJs4/Source/ext-all-debug-w-comments.js" />
Ext.define('TargetExt.PeriodPaymentPlanDetail.PeriodPaymentPlanDetailEditor', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton',
        'Actions.Buttons.AuditLogButton',
        'Target.ux.form.field.DateTime',
        'Target.ux.form.field.Number'
    ],
    closeAction: 'hide',
    modal: true,
    resizable: false,
    title: 'Period Payment Detail',
    width: 400,
    constrain: true,
    config: {
        domContractPeriodID: null,
        currentItem: null,
        hasUpdated: false,
        periodPaymentPlanDetailID: null,
        paidStatus: "",
        paymentFrom: null,
        paymentTo: null,
        minDate: null,
        maxDate: null,
        periodFrom: null,
        periodTo: null,
        lblRunningTotal: null,
        txtContractValue: null,
        hidRunningTotal: null,
        domServiceOrderID: null,
        lblErrorMsg: null,
        webSecurityUserID: null,
        rowVersion: null,
        userID: null,
        lastPaymentToDate: null,
        paymentDateInUseErrorCode: 'E3137'
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.ContractPeriodPaymentPlan.Services.PeriodPaymentPlanService;
        // add can add new permission
        cfg.permissionAddNew = me.resultSettings.HasPermission('CanAddNew');
        me.setupForm();
        me.bbar = [
            cfg.auditLogButton,
            { xtype: 'tbfill' },
            cfg.formSave
        ];
        me.items = [
            cfg.form
        ];

        me.callParent(arguments);
    },
    getItem: function () {
        var me = this, cfg = me.config;
        return cfg.currentItem;
    },
    getItemID: function () {
        var me = this, cfg = me.config;
        return cfg.currentItem.ID;
    },
    getHasUpdated: function () {
        var me = this, cfg = me.config;
        return cfg.hasUpdated;
    },
    resetFormVisibility: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), msgDateVal;
        cfg.formSave.setDisabled(!(frm.isValid() && cfg.permissionAddNew && me.validateFormDates() === true && Ext.util.Format.trim(cfg.currentItem.PaidStatus) === ''));
        //cfg.auditLogButton.setDisabled(!(frm.isValid()));
    },
    validateFormDates: function () {
        var me = this, cfg = me.config, msgDateVal;
        msgDateVal = me.validateFormDateControls();
        return msgDateVal;
    },
    setupForm: function () {
        var me = this, cfg = me.config, formFldsItems = [];
        if (!cfg.form) {
            // define the model for the form
            cfg.modelName = 'PeriodPaymentPlanDetailItem';
            Ext.define(cfg.modelName, {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'PaymentDate', mapping: 'PaymentDate', type: 'date' },
                    { name: 'PaymentFrom', mapping: 'paymentFrom', type: 'date' },
                    { name: 'PaymentTo', mapping: 'paymentTo', type: 'date' },
                    { name: 'Value', mapping: 'Value' },
                    { name: 'PaidStatus', mapping: 'paidStatus', type: 'string' },
                    { name: 'DomContractPeriodID', mapping: 'domContractPeriodID' },
                    { name: 'DomServiceOrderID', mapping: 'domServiceOrderID' },
                    { name: 'PeriodPaymentPlanDetailID', mapping: 'periodPaymentPlanDetailID' },
                    { name: 'WebSecurityUserID', mapping: 'webSecurityUserID' },
                    { name: 'Rowversion', mapping: 'rowVersion' }
                ],
                idProperty: 'ID'
            });
            formFldsItems.push(
                    Ext.create('Target.ux.form.field.DateTime', {
                        dateControlConfig: {
                            name: 'PaymentDate',
                            fieldLabel: 'Payment Date',
                            labelWidth: 120,
                            width: 250,
                            allowBlank: false
                        },
                        fieldLabel: '',
                        labelWidth: 0
                    }),
                    Ext.create('Target.ux.form.field.DateTime', {
                        dateControlConfig: {
                            name: 'PaymentFrom',
                            fieldLabel: 'Payment From',
                            labelWidth: 120,
                            width: 250,
                            allowBlank: false
                        },
                        fieldLabel: '',
                        labelWidth: 0
                    }),
                    Ext.create('Target.ux.form.field.DateTime', {
                        dateControlConfig: {
                            name: 'PaymentTo',
                            fieldLabel: 'Payment To',
                            labelWidth: 120,
                            width: 250,
                            allowBlank: false
                        },
                        fieldLabel: '',
                        labelWidth: 0
                    }),
                    cfg.unitCost = Ext.create('Target.ux.form.field.Number', {
                        fieldLabel: 'Value',
                        labelWidth: 120,
                        name: 'Value',
                        hideTrigger: true,
                        decimalPrecision: 2,
                        forcePrecision: true,
                        width: 200,
                        allowBlank: false,
                        listeners: {
                            change: function (src, cmp) {
                                me.resetFormVisibility();
                            }
                        }
                    }),
                    Ext.create('Ext.form.Label', {
                        cls: 'errorText'
                    })
            );
            cfg.formFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Add Payment Plan Detail',
                collapsible: false,
                items: formFldsItems,
                listeners: {
                    add: function (src, cmp) {
                        cmp.on('dateChange', function () { me.resetFormVisibility(); });
                    }
                }
            });
            // create the save button       
            cfg.formSave = Ext.create('Ext.Button', {
                text: 'Save',
                cls: 'Selectors',
                iconCls: 'imgSave',
                tooltip: 'Save?',
                handler: function () {
                    me.save();
                }
            });
            // create the audit button
            cfg.auditLogButton = Ext.create('Actions.Buttons.AuditLogButton', {});
            // create the form with all items
            cfg.form = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                    cfg.formFlds
                ]
            });
        }
    },
    validateFormDateControls: function () {
        var me = this, cfg = me.config, result = "";

        if ((cfg.formFlds.items.items[1].getValue() > cfg.formFlds.items.items[2].getValue())) {
            result = result.concat("Payment From must be earlier or equal to Payment To.\n");
        }

        if ((cfg.formFlds.items.items[2].getValue() < cfg.formFlds.items.items[1].getValue())) {
            result = result.concat("Payment To must be later than or equal to Payment From.");
        }

        cfg.formFlds.items.items[4].setText(result);

        if (result === "") {
            return true;
        }
        else {
            return result;
        }
    },
    nullDate: function () {
        return new Date('01/01/1901');
    },
    formatControls: function () {
        var me = this, cfg = me.config, numberItem, dateItem;

        //format date controls
        dateItem = new Date(cfg.formFlds.items.items[0].getValue());
        if (Number(me.nullDate()) == Number(dateItem)) {
            cfg.formFlds.items.items[0].setValue();
        }
        dateItem = new Date(cfg.formFlds.items.items[1].getValue());
        if (Number(me.nullDate()) == Number(dateItem)) {
            cfg.formFlds.items.items[1].setValue();
        }
        dateItem = new Date(cfg.formFlds.items.items[2].getValue());
        if (Number(me.nullDate()) == Number(dateItem)) {
            cfg.formFlds.items.items[2].setValue();
        }

        // format number fields
        numberItem = cfg.formFlds.items.items[3].getValue();
        if (numberItem == 0) {
            cfg.formFlds.items.items[3].setValue();
        }

    },
    saveItem: function (itemToSave) {
        var me = this, cfg = me.config, runningTotal, errorOccured = false, msgDatesValid;
        msgDatesValid = me.validateFormDates();
        if (me.hasItemChanged(cfg.currentItem, itemToSave)) {
            if (msgDatesValid) {
                me.setLoading(true);
                cfg.service.SavePeriodPaymentPlanDetail(itemToSave, function (response) {
                    me.setLoading(false);
                    if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                        return false;
                    }
                    if (response.value.ErrMsg.Number == cfg.paymentDateInUseErrorCode) {
                        errorOccured = true;
                        Ext.Msg.alert('Info', response.value.ErrMsg.Message);
                    }
                    else {
                        //this needs to be done using a web service.
                        runningTotal = response.value.Item.CurrentPaymentDetailTotal;
                        cfg.lblRunningTotal.innerHTML = runningTotal.toFixed(2);
                        if (Number(cfg.txtContractValue.value).toFixed(2) !== Number(runningTotal).toFixed(2)) {
                            cfg.lblRunningTotal.className = "errorText";
                            cfg.lblErrorMsg.innerHTML = "Contract Value and Total sum of Period Payment Plan Detail lines must be the same";
                        }
                        else {
                            cfg.lblRunningTotal.className = "";
                            cfg.lblErrorMsg.innerHTML = "";
                        }
                        cfg.hidRunningTotal.value = runningTotal.toFixed(2);
                        if (response.value.Item.LatestPaymentToDate) {
                            $(cfg.dteEndDate).datepicker("option", "minDate", response.value.Item.LatestPaymentToDate);
                        }
                        itemToSave.ID = response.value.Item.NewPaymentPlanDetailID;
                        cfg.currentItem = itemToSave;
                        cfg.hasUpdated = true;
                        me.close();
                    }
                }, me);
            }
            else {
                if (!msgDatesValid) {
                    cfg.formSave.setDisabled(true);
                    cfg.auditLogButton.setDisabled(true);
                    Ext.MessageBox.confirm({
                        title: 'Info',
                        msg: msgDatesValid,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.INFO,
                        scope: me
                    });
                }
            }
        }
        else {

            cfg.hasUpdated = false;
            me.close();
        }
    },
    save: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), updater = frm.getRecord();
        frm.updateRecord(updater);
        updater = updater.data;
        me.saveItem(updater);
    },
    show: function (args) {
        var me = this, cfg = me.config;
        cfg.hasUpdated = false;

        if (cfg.form) {
            cfg.currentItem = (args.item || { PaymentDate: '', PaymentFrom: '', PaymentTo: '', Value: '', ID: 0,
                DomContractPeriodID: cfg.domContractPeriodID, PaidStatus: cfg.paidStatus, DomServiceOrderID: cfg.domServiceOrderID,
                PeriodPaymentPlanDetailID: null, WebSecurityUserID: cfg.userID, Rowversion: null
            });
            mdl = Ext.create(cfg.modelName, cfg.currentItem);
            cfg.form.loadRecord(mdl);
            //cfg.formFlds.items.items[0].focus(true, true);
            me.formatControls();
            cfg.auditLogButton.setConfigSettings({
                auditLogID: cfg.currentItem.PeriodPaymentPlanDetailID,
                auditLogTables: ['PeriodPaymentPlanDetail'],
                auditLogUseApplicationFilter: true,
                auditLogWebNavMenuItemID: 118
            });
        }

        me.resetFormVisibility();
        me.callParent();
        me.center();

        //set date constraints
        if (cfg.lastPaymentToDate == "" || Ext.util.Format.trim(cfg.currentItem.PaidStatus) !== '') {
            cfg.formFlds.items.items[0].setMinDateValue(new Date(periodFrom));
            cfg.formFlds.items.items[1].setMinDateValue(new Date(periodFrom));
            cfg.formFlds.items.items[2].setMinDateValue(new Date(periodFrom));
        }
        else {
            cfg.formFlds.items.items[0].setMinDateValue(new Date(cfg.lastPaymentToDate));
            cfg.formFlds.items.items[1].setMinDateValue(new Date(cfg.lastPaymentToDate));
            cfg.formFlds.items.items[2].setMinDateValue(new Date(cfg.lastPaymentToDate));
        }
        cfg.formFlds.items.items[0].setMaxDateValue(cfg.dteEndDate.value.toDate());
        cfg.formFlds.items.items[1].setMaxDateValue(cfg.dteEndDate.value.toDate());
        cfg.formFlds.items.items[2].setMaxDateValue(cfg.dteEndDate.value.toDate());


    },
    hasItemChanged: function (currentItem, itemToSave) {
        if (currentItem.PaymentDate !== itemToSave.PaymentDate || Number(currentItem.Value).toFixed(2) !== Number(itemToSave.Value).toFixed(2) || currentItem.PaymentFrom !== itemToSave.PaymentFrom || currentItem.PaymentTo !== itemToSave.PaymentTo) {
            return true;
        }
        return false;
    }
});