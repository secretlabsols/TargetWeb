Ext.define('TargetExt.DSOProviderUnitCostOverride.Editor', {
    extend: 'Ext.window.Window',
    title: 'Unit Cost Override',
    closeAction: 'hide',
    width: 500,
    height: 240,
    layout: 'fit',
    resizable: false,
    modal: true,
    config: {
        domServiceOrderID: null
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config;

        cfg.modelName = 'UnitCostOverrideItem'

        Ext.define(cfg.modelName, {
            extend: 'Ext.data.Model',
            fields: [
                { name: 'DomRateCategoryID', mapping: 'DomRateCategoryID' },
                { name: 'DateFrom', mapping: 'DateFrom', type: 'date' },
                { name: 'DateTo', mapping: 'DateTo', type: 'date' },
                { name: 'UnitCost', mapping: 'UnitCost' },
                { name: 'DomServiceOrderID', mapping: 'domServiceOrderID' }
            ],
            idProperty: 'ID'
        });
        var todayDate = new Date();
        var data = { DomRateCategoryID: 0, DateFrom: '', DateTo: '', UnitCost: '', ID: 0, DomServiceOrderID: cfg.domServiceOrderID };
        var mdl = Ext.create(cfg.modelName, data);
        me.setupFrm();
        cfg.overrideForm.loadRecord(mdl);

        me.callParent(arguments);

    },
    setupFrm: function (args) {
        var me = this, cfg = me.config;
        if (!cfg.overrideForm) {

            cfg.service = Target.Abacus.Library.ServiceOrder.ServiceOrderService;

            cfg.formRateCategoryStore = Ext.create('Ext.data.Store', {
                fields: [{ name: 'ID' },
                         { name: 'Description'}],
                data: []
            });

            cfg.cboRateCategory = Ext.create('Ext.form.ComboBox', {
                name: 'DomRateCategoryID',
                fieldLabel: 'Rate Category',
                labelWidth: 120,
                store: cfg.formRateCategoryStore,
                queryMode: 'local',
                displayField: 'Description',
                valueField: 'ID',
                width: 470,
                validator: function (val) {
                    if (!Ext.isEmpty(val)) {
                        return true;
                    } else {
                        return "Rate Category is required";
                    }
                }
            });

            cfg.dateFrom = Ext.create('Target.ux.form.field.DateTime', {
                dateControlConfig: {
                    name: 'DateFrom',
                    fieldLabel: 'Date From',
                    labelWidth: 120,
                    width: 250
                },
                fieldLabel: '',
                labelWidth: 0
            });

            cfg.dateTo = Ext.create('Target.ux.form.field.DateTime', {
                dateControlConfig: {
                    name: 'DateTo',
                    fieldLabel: 'Date To',
                    labelWidth: 120,
                    width: 250
                },
                fieldLabel: '',
                labelWidth: 0
            });

            cfg.dateFrom.addListener('dateChange', function (fld, newVal) {
                cfg.dateTo.setMinDateValue(newVal)
            }, me);
            cfg.dateTo.addListener('dateChange', function (fld, newVal) {
                cfg.dateFrom.setMaxDateValue(newVal)
            }, me);

            Ext.override(Ext.form.NumberField, {
                forcePrecision: false,

                valueToRaw: function (value) {
                    var me = this, decimalSeparator = me.decimalSeparator;
                    value = me.parseValue(value);
                    value = me.fixPrecision(value);
                    value = Ext.isNumber(value) ? value : parseFloat(String(value).replace(decimalSeparator, '.'));
                    if (isNaN(value)) {
                        value = '';
                    }
                    else {
                        value = me.forcePrecision ? value.toFixed(me.decimalPrecision) : parseFloat(value);
                        value = String(value).replace('.', decimalSeparator);
                    }
                    return value;
                }
            });

            cfg.unitCost = Ext.create('Ext.form.NumberField', {
                fieldLabel: 'Unit Cost',
                labelWidth: 120,
                name: 'UnitCost',
                hideTrigger: true,
                decimalPrecision: 2,
                forcePrecision: true,
                width: 200
            });

            cfg.overrideForm = Ext.create('Ext.form.Panel', {
                layout: {
                    type: 'vbox',
                    align: 'left'
                },
                border: false,
                bodyPadding: 10,
                items: [
                        cfg.cboRateCategory,
                        cfg.dateFrom,
                        cfg.dateTo,
                        cfg.unitCost
                   ],
                buttons: [{
                    text: 'Save',
                    handler: function () {
                        var me = this, cfg = me.config;
                        if (this.up('form').getForm().isValid()) {
                            this.up('window').saveOverride();
                        }
                    }
                },
                {
                    text: 'Cancel',
                    handler: function () {
                        this.up('window').hide();
                    }
                }
                ]
            });
        }


        me.items = [cfg.overrideForm];


    },
    show: function (item) {
        var me = this, cfg = me.config;
        me.callParent(arguments);

        cfg.selectedItemFromSelector = item;

        me.load();

    },
    getRecord: function () {
        var me = this, cfg = me.config, frm = cfg.overrideForm.getForm(), updater = frm.getRecord();
        frm.updateRecord(updater);
        return updater.data;
    },
    adviseChanged: function () {
        var me = this, cfg = me.config;
        // cfg.overrideForm.update();
        me.fireEvent('onChanged', me);
    },
    load: function (item) {
        var me = this, cfg = me.config;
        cfg.service.GetRateCategoriesForOrder(cfg.domServiceOrderID, me.GetRateCategoriesForOrderCallBack, me);

    },
    GetRateCategoriesForOrderCallBack: function (response) {
        var me = response.context, cfg = me.config;
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.formRateCategoryStore.loadData(response.value.Item);

        if (cfg.selectedItemFromSelector) {
            mdl = Ext.create(cfg.modelName, cfg.selectedItemFromSelector);
            cfg.overrideForm.loadRecord(mdl);
            cfg.cboRateCategory.setValue(cfg.selectedItemFromSelector.DomRateCategoryID);
            cfg.dateFrom.setMaxDateValue(cfg.selectedItemFromSelector.DateTo);
            cfg.dateTo.setMinDateValue(cfg.selectedItemFromSelector.DateFrom);
        }
    },
    saveOverride: function () {
        var me = this, cfg = me.config;
        var record = me.getRecord();
        cfg.service.SaveDSOUnitCostOverride(record, me.saveDSOUnitCostOverrideCallBack, me);
    },
    saveDSOUnitCostOverrideCallBack: function (response) {
        var me = response.context, cfg = me.config;
        var record = me.getRecord();
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }

        if (response.value.overLapCount > 1) {
            Ext.MessageBox.confirm({
                title: 'info',
                msg: "Either enter a date later than the current override or delete the current override before entering this older cost",
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.QUESTION,
                scope: me
            });

            return;
        }
         else {
            me.adviseChanged();
            me.hide();
        }
    }
});