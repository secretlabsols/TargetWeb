/// <reference path="../../../Library/JavaScript/ExtJs4/Source/ext-all-debug-w-comments.js" />
Ext.define('TargetExt.FinanceCodes.FinanceCodeMatrixConfigurationEditor', {
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
    title: 'Finance Code Matrix Configuration',
    width: 800,
    constrain: true,
    config: {
        currentItem: null,
        hasUpdated: false,
        effectiveDate: null,
        financeCodeLength: 0
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.FinanceCodes.Services.FinanceCodeMatrixService;
        // add can add new permission
        cfg.permissionConfigure = me.resultSettings.HasPermission('CanConfigure');
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
    getHasUpdated: function () {
        var me = this, cfg = me.config;
        return cfg.hasUpdated;
    },
    resetFormVisibility: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm();

        cfg.formSave.setDisabled(!(frm.isValid() && cfg.permissionConfigure));
    },
    setupForm: function () {
        var me = this, cfg = me.config, formFldsItems = [], formFldsOtherItems = [], formFldsDescriptionItems = [];
        if (!cfg.form) {
            // define the model for the form
            cfg.modelName = 'FinanceCodeMatrixConfigurationEditorItem';
            Ext.define(cfg.modelName, {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'EffectiveDate', mapping: 'EffectiveDate', type: 'date' },
                    { name: 'FinanceCodeLength', mapping: 'FinanceCodeLength' },
                    { name: 'StartOfServiceTypeElement', mapping: 'StartOfServiceTypeElement' },
                    { name: 'EndOfServiceTypeElement', mapping: 'EndOfServiceTypeElement' },
                    { name: 'ClientGroupVisible', mapping: 'ClientGroupVisible' },
                    { name: 'ClientSubGroupVisible', mapping: 'ClientSubGroupVisible' },
                    { name: 'AgeRangeVisible', mapping: 'AgeRangeVisible' },
                    { name: 'TeamVisible', mapping: 'TeamVisible' },
                    { name: 'ProviderVisible', mapping: 'ProviderVisible' },
                    { name: 'PrivateOrLAVisible', mapping: 'PrivateOrLAVisible' },
                    { name: 'ServiceTypeVisible', mapping: 'ServiceTypeVisible' },
                    { name: 'ContractNumberVisible', mapping: 'ContractNumberVisible' },
                    { name: 'ServiceOrderVisible', mapping: 'ServiceOrderVisible' }
                ],
                idProperty: 'ID'
            });
            formFldsDescriptionItems.push(
                        Ext.create('Ext.form.Label', {
                            name: 'Description',
                            text: 'If the new coding structure is to be implemented, enter the date it wil be effective from and ABACUS\n\
will automatically terminate all existing finance code derivations on the preceding day. \n You will then need to enter new finance codes manually to be effective from the date you enter.'
                        })
            );
            formFldsItems.push(
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'ClientGroupVisible',
                            boxLabel: 'Client Group',
                            inputValue: -1
                        }),
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'ClientSubGroupVisible',
                            boxLabel: 'Client Sub Group',
                            inputValue: -1
                        }),
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'AgeRangeVisible',
                            boxLabel: 'Age Range',
                            inputValue: -1
                        }),
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'TeamVisible',
                            boxLabel: 'Team',
                            inputValue: -1
                        }),
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'ProviderVisible',
                            boxLabel: 'Provider',
                            inputValue: -1
                        }),
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'PrivateOrLAVisible',
                            boxLabel: 'Private or LA',
                            inputValue: -1
                        }),
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'ServiceTypeVisible',
                            boxLabel: 'Service Type',
                            inputValue: -1
                        }),
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'ContractNumberVisible',
                            boxLabel: 'Contract Number',
                            inputValue: -1
                        }),
                        Ext.create('Ext.form.field.Checkbox', {
                            name: 'ServiceOrderVisible',
                            boxLabel: 'Service Order',
                            inputValue: -1
                        })
            );
            formFldsOtherItems.push(
                    Ext.create('Target.ux.form.field.DateTime', {
                        dateControlConfig: {
                            name: 'EffectiveDate',
                            fieldLabel: 'Effective Date',
                            labelWidth: 400,
                            width: 520
                        },
                        fieldLabel: '',
                        labelWidth: 0
                    }),
                    Ext.create('Target.ux.form.field.Number', {
                        name: 'FinanceCodeLength',
                        fieldLabel: 'Number of characters in finance codes',
                        maxValue: 35,
                        minValue: 0,
                        labelWidth: 400,
                        decimalPrecision: 0,
                        validator: function (val) {
                            if (!Ext.isEmpty(val)) {
                                return true;
                            } else {
                                return "Value is required";
                            }
                        }
                    }),
                    Ext.create('Target.ux.form.field.Number', {
                        name: 'StartOfServiceTypeElement',
                        fieldLabel: 'Start position of service type element in Finance Code',
                        maxValue: 99,
                        minValue: 0,
                        labelWidth: 400,
                        decimalPrecision: 0,
                        validator: function (val) {
                            return me.validateStartPos(val);
                        }
                    }),
                    Ext.create('Target.ux.form.field.Number', {
                        name: 'EndOfServiceTypeElement',
                        fieldLabel: 'End position of service type element in Finance Code',
                        labelWidth: 400,
                        maxValue: 99,
                        minValue: 0,
                        decimalPrecision: 0,
                        validator: function (val) {
                            return me.validateEndPos(val);
                        }
                    })
            );
            cfg.formFldsDescription = Ext.create('Ext.form.FieldSet', {
                title: 'Description',
                collapsible: false,
                items: formFldsDescriptionItems
            });
            cfg.formFlds = Ext.create('Ext.form.FieldSet', {
                title: 'Choose factors from which finance codes are derived',
                collapsible: false,
                items: formFldsItems,
                listeners: {
                    add: function (src, cmp) {
                        cmp.on('change', function () { me.resetFormVisibility(); });
                    }
                }
            });
            cfg.formFldsOther = Ext.create('Ext.form.FieldSet', {
                title: 'Other Finance Code Matrix Elements',
                collapsible: false,
                items: formFldsOtherItems,
                listeners: {
                    add: function (src, cmp) {
                        cmp.on('change', function () { me.resetFormVisibility(); });
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
                    cfg.formFldsDescription,
                    cfg.formFldsOther,
                    cfg.formFlds
                ]
            });
        }
    },
    convertBooleanToTriState: function (item) {
        if (item == true) {
            return -1;
        }
        else {
            return 0;
        }
    },
    getFormattedData: function (item) {
        var me = this;
        item.ClientGroupVisible = me.convertBooleanToTriState(item.ClientGroupVisible);
        item.ClientSubGroupVisible = me.convertBooleanToTriState(item.ClientSubGroupVisible);
        item.AgeRangeVisible = me.convertBooleanToTriState(item.AgeRangeVisible);
        item.TeamVisible = me.convertBooleanToTriState(item.TeamVisible);
        item.ProviderVisible = me.convertBooleanToTriState(item.ProviderVisible);
        item.PrivateOrLAVisible = me.convertBooleanToTriState(item.PrivateOrLAVisible);
        item.ServiceTypeVisible = me.convertBooleanToTriState(item.ServiceTypeVisible);
        item.ContractNumberVisible = me.convertBooleanToTriState(item.ContractNumberVisible);
        item.ServiceOrderVisible = me.convertBooleanToTriState(item.ServiceOrderVisible);

        return item;
    },
    nullDate: function () {
        return new Date('01/01/1901');
    },
    formatControls: function () {
        var me = this, cfg = me.config, currentItem, effectiveDate;

        //format date control
        effectiveDate = new Date(cfg.formFldsOther.items.items[0].getValue());
        if (Number(me.nullDate()) == Number(effectiveDate)) {
            cfg.formFldsOther.items.items[0].setValue();
        }

        //set the current effectiveDate
        cfg.effectiveDate = effectiveDate;

        //set the current finance code length
        cfg.financeCodeLength = cfg.formFldsOther.items.items[1].getValue();

        // format number fields
        for (var i = 1; i <= (cfg.formFldsOther.items.items.length - 1); i++) {
            currentItem = cfg.formFldsOther.items.items[i].getValue();
            if (currentItem == 0) {
                cfg.formFldsOther.items.items[i].setValue()
            }
        }
    },
    validateStartPos: function (val) {
        var me = this, cfg = me.config;

        if (Number(cfg.formFldsOther.items.items[3].getValue()) < Number(val)) {
            return "Start position cannot be greater than end position.";
        }

        if (Number(cfg.formFldsOther.items.items[1].getValue()) < Number(val)) {
            return "Start position cannot be greater than finance code length.";
        }

        return true;
    },
    validateEndPos: function (val) {
        var me = this, cfg = me.config;

        if (Number(cfg.formFldsOther.items.items[2].getValue()) > Number(val)) {
            return "End position cannot be less than start position.";
        }

        if (Number(cfg.formFldsOther.items.items[1].getValue()) < Number(val)) {
            return "End position cannot be greater than finance code length.";
        }

        return true;
    },
    saveItem: function (itemToSave) {
        var me = this, cfg = me.config;
        me.setLoading(true);
        cfg.service.SaveFinanceCodeMatrixConfiguration(itemToSave, function (response) {
            me.setLoading(false);
            if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                return false;
            }
            cfg.hasUpdated = true;
            me.close();
        }, me);
    },
    doesItemHaveFactors: function (item) {
        var me = this, cfg = me.config;
        if (item.ClientGroupVisible && cfg.formFlds.items.items[0].getValue() == false || item.ClientSubGroupVisible && cfg.formFlds.items.items[1].getValue() == false ||
            item.AgeRangeVisible && cfg.formFlds.items.items[2].getValue() == false || item.TeamVisible && cfg.formFlds.items.items[3].getValue() == false ||
            item.ProviderVisible && cfg.formFlds.items.items[4].getValue() == false || item.PrivateOrLAVisible && cfg.formFlds.items.items[5].getValue() == false ||
            item.ServiceTypeVisible && cfg.formFlds.items.items[6].getValue() == false || item.ContractVisible && cfg.formFlds.items.items[7].getValue() == false ||
            item.ServiceOrderVisible && cfg.formFlds.items.items[8].getValue() == false) {
            return true;
        }
        else {
            return false;
        }
    },
    formatFactorsInfo: function (item) {
        var msgString, me = this, cfg = me.config;

        msgString = 'You will need to change the effective date ending all existing finance code matrices because you are trying to remove a factor from the configuration\n\
that is already in use on existing finance code matrices.  The following factors are currently in use, ';

        if (item.ClientGroupVisible && cfg.formFlds.items.items[0].getValue() == false) {
            msgString += 'Client Group';
        }
        if (item.ClientSubGroupVisible && cfg.formFlds.items.items[1].getValue() == false) {
            msgString += ', Client Sub Group';
        }
        if (item.AgeRangeVisible && cfg.formFlds.items.items[2].getValue() == false) {
            msgString += ', Age Range';
        }
        if (item.TeamVisible && cfg.formFlds.items.items[3].getValue() == false) {
            msgString += ', Team';
        }
        if (item.ProviderVisible && cfg.formFlds.items.items[4].getValue() == false) {
            msgString += ', Provider';
        }
        if (item.PrivateOrLAVisible && cfg.formFlds.items.items[5].getValue() == false) {
            msgString += ', Private Or LA';
        }
        if (item.ServiceTypeVisible && cfg.formFlds.items.items[6].getValue() == false) {
            msgString += ', Service Type';
        }
        if (item.ContractVisible && cfg.formFlds.items.items[7].getValue() == false) {
            msgString += ', Contract';
        }
        if (item.ServiceOrderVisible && cfg.formFlds.items.items[8].getValue() == false) {
            msgString += ', Service Order';
        }

        return msgString;
    },
    save: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), updater = frm.getRecord(), effectiveDate, webSvcResponse;
        var msgBoxStr = 'Confirm you wish to terminate all existing finance code entries and that you will manually key in new finance codes together with their related components.<br>Note subsequently, you will not be able to enter finance code entries for an earlier period.<br><br>Once you have recorded the new set of finance codes, you should run the job "Finance Code Matrix – Recode Service Orders" to update service orders with these new codes. If you do not run the job, the old codes will still prevail.';
        frm.updateRecord(updater);
        updater = updater.data;
        updater = me.getFormattedData(updater);

        effectiveDate = updater.EffectiveDate;

        if (effectiveDate == null) {
            updater.EffectiveDate = me.nullDate();
            effectiveDate = me.nullDate();
        }
        if (Number(cfg.effectiveDate) != Number(effectiveDate)) {
            if (Number(effectiveDate) < Number(cfg.effectiveDate)) {
                Ext.MessageBox.confirm({
                    title: 'Info',
                    msg: "Effective Date can not be blank or earlier than previously entered Effective Date",
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.INFO,
                    scope: me
                });
                cfg.formFldsOther.items.items[0].setValue(cfg.effectiveDate);
            }
            else {
                if (Number(effectiveDate) > Number(cfg.effectiveDate)) {
                    Ext.MessageBox.confirm('Confirm', msgBoxStr,
                        function (answer) {
                            if (answer == 'yes') {
                                me.saveItem(updater);
                            }
                        }
                    )
                }
            }
        }
        else {
            Ext.getBody().mask('Checking for associated finance code matrix factors...');
            webSvcResponse = cfg.service.GetFinanceCodeMatrixFactorsInUse(updater.EffectiveDate);
            Ext.getBody().unmask();
            if (!CheckAjaxResponse(webSvcResponse, cfg.service.url, true)) {
                return false;
            }
            if (me.doesItemHaveFactors(webSvcResponse.value.Item)) {
                Ext.Msg.alert('Info', me.formatFactorsInfo(webSvcResponse.value.Item));
            }
            else {
                if (updater.FinanceCodeLength != cfg.financeCodeLength) {
                    Ext.getBody().mask('Checking finance code length...');
                    webSvcResponse = cfg.service.GetNumberOfFCMAssociatedToFCMConfig(updater.EffectiveDate);
                    Ext.getBody().unmask();
                    if (!CheckAjaxResponse(webSvcResponse, cfg.service.url, true)) {
                        return false;
                    }
                    if (webSvcResponse.value.Item > 0) {
                        Ext.Msg.alert('Info', 'You will need to change the effective date ending all existing finance code matrices because you have changed the finance code length when there is currently ' + webSvcResponse.value.Item.toString() + ' associated matrices');
                    }
                    else {
                        me.saveItem(updater);
                    }
                }
                else {
                    me.saveItem(updater);
                }
            }
        }
    },
    show: function (args) {
        var me = this, cfg = me.config, mdl;
        cfg.hasUpdated = false;
        me.setLoading(true);
        cfg.service.GetFinanceCodeMatrixConfiguration(function (response) {
            me.setLoading(false);
            if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                return false;
            }
            cfg.currentItem = response.value.Item;
            mdl = Ext.create(cfg.modelName, response.value.Item);
            cfg.form.loadRecord(mdl);
            cfg.formFlds.items.items[0].focus(true, true);
            me.formatControls();
            cfg.auditLogButton.setConfigSettings({
                auditLogID: cfg.currentItem.ID,
                auditLogTables: ['FinanceCodeMatrixConfiguration'],
                auditLogUseApplicationFilter: true,
                auditLogWebNavMenuItemID: 190
            });
            me.resetFormVisibility();
            //cfg.form.getForm().clearInvalid();
        }, me);
        me.callParent();
        me.center();
    }
});