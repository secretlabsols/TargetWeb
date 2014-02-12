Ext.define("TargetExt.Editors.ValidationTextEditor", {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton'
    ],
    closeAction: 'hide',
    title: 'Edit Verification Text',
    resizable: false,
    width: 600,
    modal: true,
    config: {
        TextID: 0,
        CurrentItem: null,
        hasUpdated: false
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config, controls = [];
        cfg.service = Target.Abacus.Library.ReferenceDataCommon.Services.VerificationTextSvc;
        me.setupForm();
        me.setValueVisibility();
        me.bbar = [
            { xtype: 'tbfill' },
            cfg.formSave,
            cfg.formDelete
        ];
        me.items = [
            cfg.frmGeneral
        ];
        me.callParent(arguments);
    },
    setValues: function () {
        var me = this, cfg = me.config;
        cfg.service.FetchVerificationText(cfg.TextID, me.FetchCallBack, me)
    },
    FetchCallBack: function (response) {
        var me = response.context, cfg = me.config;
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }

        cfg.CurrentItem = response.value.Item;
        cfg.chkResidential.setValue(cfg.CurrentItem.IsForResidential);
        cfg.chkResidential.setValue(cfg.CurrentItem.IsForResidential);
        cfg.chkNonResidential.setValue(cfg.CurrentItem.IsForNonResidential);
        cfg.chkDefault.setValue(cfg.CurrentItem.IsDefault);
        cfg.chkRedundant.setValue(cfg.CurrentItem.IsRedundant);
        cfg.txtHeading.setValue(cfg.CurrentItem.Heading)
        cfg.txtDescription.setValue(cfg.CurrentItem.Description)
    },
    setPerformCheck: function (checked) {
        var me = this, cfg = me.config;

    },
    setDisabled: function (obj, checked) {
        obj.setDisabled(checked);
    },
    setupForm: function () {
        var me = this, cfg = me.config;
        if (!cfg.frmGeneral) {

            cfg.formSave = Ext.create('Ext.Button', {
                text: 'Save',
                cls: 'Selectors',
                iconCls: 'imgSave',
                tooltip: 'Save?',
                handler: function () {
                    me.save();
                }
            });

            cfg.formDelete = Ext.create('Ext.Button', {
                text: 'Delete',
                cls: 'Selectors',
                iconCls: 'imgDelete',
                tooltip: 'Delete?',
                handler: function () {
                    me.Delete();
                }
            });

            cfg.txtHeading = Ext.create('Ext.form.Text', {
                fieldLabel: 'Heading',
                width: 400,
                allowBlank: false,
                listeners: {
                    change: {
                        fn: function () {
                            me.resetFormVisibility();
                        }
                    }
                }
            });


            cfg.txtDescription = Ext.create('Ext.form.HtmlEditor', {
                width: 580,
                height: 250,

            });

            cfg.chkResidential = Ext.create('Ext.form.Checkbox', {
                fieldLabel: 'Residential',
                labelWidth: 170,
                anchor: '80%',
                padding: '0 0 0 0',
                listeners: {
                    change: {
                        fn: function (me, newValue, oldValue) {
                            if (newValue === true)
                            {
                               cfg.chkNonResidential.setValue(false); 
                            }
                        }
                    }
                }
            });

            cfg.chkNonResidential = Ext.create('Ext.form.Checkbox', {
                fieldLabel: 'Non-Residential',
                labelWidth: 170,
                anchor: '80%',
                padding: '0 0 0 0',
                listeners: {
                    change: {
                        fn: function (me, newValue, oldValue) {
                            if (newValue === true)
                            {
                               cfg.chkResidential.setValue(false); 
                            }
                       }
                   }
                }
            });

            cfg.chkDefault = Ext.create('Ext.form.Checkbox', {
                fieldLabel: 'Default',
                labelWidth: 170,
                anchor: '80%',
                padding: '0 0 0 0'
            });

            cfg.chkRedundant = Ext.create('Ext.form.Checkbox', {
                fieldLabel: 'Redundant',
                labelWidth: 170,
                anchor: '80%',
                padding: '0 0 0 0'
            });

            // create the form with all items
            cfg.frmGeneral = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                    cfg.txtHeading,
                    cfg.txtDescription,
                    cfg.chkResidential,
                    cfg.chkNonResidential,
                    cfg.chkDefault,
                    cfg.chkRedundant
                ]
            });

        }
    },
    setValueVisibility: function () {
        var me = this, cfg = me.config;
        if (cfg.TextID == 0) {
            cfg.formDelete.hide();
        } else {
            cfg.formSave.text = "Update";
            cfg.formSave.tooltip = "Update?";
        }
    },
    save: function () {
        var me = this, cfg = me.config;

        me.setLoading('Updating ....');
        // load object 
        cfg.CurrentItem.Heading = cfg.txtHeading.value;
        cfg.CurrentItem.Description = cfg.txtDescription.getValue();
        cfg.CurrentItem.IsForResidential = cfg.chkResidential.value;
        cfg.CurrentItem.IsForNonResidential = cfg.chkNonResidential.value;
        cfg.CurrentItem.IsDefault = cfg.chkDefault.value;
        cfg.CurrentItem.IsRedundant = cfg.chkRedundant.value;

        if (cfg.TextID > 0) {
            cfg.service.UpdateVerificationText(cfg.CurrentItem, me.saveCallBack, me);
        } else {
            cfg.service.CreateVerificationText(cfg.CurrentItem, me.saveCallBack, me);
        }
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
    Delete: function () {
        var me = this, cfg = me.config;

        me.setLoading('Updating ....');
        if (cfg.TextID > 0) {
            cfg.service.DeleteVerificationText(cfg.TextID, me.saveCallBack, me);
        }

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
        cfg.formDelete.setDisabled(!frm.isValid());
    }

});