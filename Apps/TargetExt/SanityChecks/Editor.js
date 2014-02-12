Ext.define("TargetExt.SanityChecks.Editor", {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton'
    ],
    closeAction: 'hide',
    title: 'Edit Validation Check',
    resizable: false,
    width: 550,
    modal: true,
    config: {
        sanityCheckID: 0,
        checkType: null,
        performCheck: null,
        withholdUpdate: null,
        valueInt: null,
        valueString: null,
        comments: null,
        hasUpdated: false
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config, controls = [];
        cfg.service = Target.Abacus.Library.WorkTraySanityChecks.Services.SanityChecks;
        me.setupForm();
        me.setValueVisibility();
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
        cfg.txtcomments.setValue(cfg.comments);
        cfg.chkPerformCheck.setValue(cfg.performCheck);
        cfg.chkWithHoldUpdate.setValue(cfg.withholdUpdate);

        if (cfg.checkType == 0) {
            cfg.txtValueInt.setValue(cfg.valueInt);
        }
        if (cfg.checkType == 1) {
            cfg.txtValueString.setValue(cfg.valueString);
        }

        me.setPerformCheck(cfg.performCheck);

    },
    setPerformCheck: function (checked) {
        var me = this, cfg = me.config;

        if (cfg.txtValueInt && cfg.txtValueString) {
            me.setDisabled(cfg.txtValueInt, !checked && (cfg.checkType == 0));
            me.setDisabled(cfg.txtValueString, !checked && (cfg.checkType == 1));
            me.setDisabled(cfg.chkWithHoldUpdate, !checked);

            // if with hold is checked and user unchecks the perform check option then uncheck withhold check as well
            if (cfg.chkWithHoldUpdate.checked && !checked)
                cfg.chkWithHoldUpdate.setValue(checked);

            if (cfg.checkType == 0 && checked) {
                cfg.txtValueInt.allowBlank = false;
            }
            else {
                cfg.txtValueInt.allowBlank = true;
                cfg.txtValueInt.setValue();
            }
            if (cfg.checkType == 1 && checked) {
                cfg.txtValueString.allowBlank = false;
            }
            else {
                cfg.txtValueString.allowBlank = true;
                cfg.txtValueString.setValue();
            }
        }
    },
    setDisabled: function (obj, checked) {
        obj.setDisabled(checked);
    },
    setupForm: function () {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            cfg.txtcomments = Ext.create('Ext.form.TextArea', {
                fieldLabel: 'Note',
                height: 120,
                anchor: '95%',
                padding: '0 0 0 0',
                listeners: {
                    change: {
                        fn: function () {
                            me.resetFormVisibility();
                        }
                    }
                }
            });

            cfg.txtValueInt = Ext.create('Ext.form.Number', {
                fieldLabel: 'Value',
                labelWidth: 170,
                anchor: '80%',
                padding: '0 0 0 0',
                listeners: {
                    change: {
                        fn: function () {
                            me.resetFormVisibility();
                        }
                    }
                }
            });

            cfg.txtValueString = Ext.create('Ext.form.TextField', {
                fieldLabel: 'Value',
                labelWidth: 170,
                anchor: '80%',
                padding: '0 0 0 0',
                listeners: {
                    change: {
                        fn: function () {
                            me.resetFormVisibility();
                        }
                    }
                }
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

            cfg.chkPerformCheck = Ext.create('Ext.form.Checkbox', {
                fieldLabel: 'Perform this check',
                labelWidth: 170,
                anchor: '80%',
                padding: '0 0 0 0',
                listeners: {
                    change: {
                        fn: function () {
                            me.setPerformCheck(this.checked);
                            me.resetFormVisibility();
                        }
                    }
                }
            });

            cfg.chkWithHoldUpdate = Ext.create('Ext.form.Checkbox', {
                fieldLabel: 'Withhold update if check fails',
                labelWidth: 170,
                anchor: '80%',
                padding: '0 0 0 0'
            });

            // create the form with all items
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                    cfg.txtcomments,
                    cfg.chkPerformCheck,
                    cfg.chkWithHoldUpdate,
                    cfg.txtValueInt,
                    cfg.txtValueString
                ]
            });

        }
    },
    setValueVisibility: function () {
        var me = this, cfg = me.config;
        cfg.txtValueInt.setVisible(cfg.checkType == 0);
        cfg.txtValueString.setVisible(cfg.checkType == 1);
    },
    save: function () {
        var me = this, cfg = me.config;
        var test = cfg.chkPerformCheck.getValue();
        me.setLoading('Updating ....');
        cfg.service.UpdateSanityCheck(cfg.sanityCheckID,
                                  cfg.checkType,
                                  cfg.chkPerformCheck.getValue(),
                                  cfg.txtValueInt.getValue(),
                                  cfg.txtValueString.getValue(),
                                  cfg.chkWithHoldUpdate.getValue(),
                                  me.saveCallBack, me);
    },
    saveCallBack: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.hasUpdated = true;
        me.close();
    },
    getHasUpdated: function () {
        var me = this, cfg = me.config;
        return cfg.hasUpdated;
    },
    show: function (args) {
        var me = this, cfg = me.config, mdl, statusItems = [];
        me.setValues();
        cfg.hasUpdated = false;
        me.resetFormVisibility();
        me.callParent();
    },
    resetFormVisibility: function () {
        var me = this, cfg = me.config, frm = cfg.frmGeneral.getForm();
        cfg.formSave.setDisabled(!frm.isValid());
    }

});