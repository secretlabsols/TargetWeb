Ext.define('TargetExt.MovementRequests.Editor', {
    extend: 'Ext.window.Window',
    requires: [
        'Target.ux.form.field.ClearButton'
    ],
    closeAction: 'hide',
    modal: true,
    resizable: false,
    title: 'Edit Movement Request',
    width: 500,
    config: {
        currentItem: null,
        hasUpdated: false,
        referenceData: null,
        webSecurityUserID: 0
    },
    constructor: function (config) {
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.Movements.Services.MovementsService;
        me.setupForm();
        me.bbar = [
            { xtype: 'tbfill' },
            cfg.formSave
        ];
        me.items = [
            cfg.form
        ];
        me.callParent(arguments);
    },
    createReferenceDataComboAndStore: function (comboFieldLabel, comboFieldName, comboFieldAllowBlank) {
        var me = this, cfg = me.config, returnData = {};
        // create store
        returnData.store = Ext.create('Ext.data.Store', {
            fields: ['ID', 'Description'],
            data: []
        });
        // create combo
        returnData.combo = Ext.create('Ext.form.ComboBox', {
            fieldLabel: comboFieldLabel,
            store: returnData.store,
            queryMode: 'local',
            displayField: 'Description',
            anchor: '100%',
            valueField: 'ID',
            multiSelect: false,
            name: comboFieldName,
            plugins: ['clearbutton'],
            allowBlank: comboFieldAllowBlank
        });
        return returnData;
    },
    ensureComboItem: function (id, store, availableList, nonAvailableList) {
        var me = this, cfg = me.config, itemIsFound = false, item;
        store.loadData(availableList, false);
        itemIsFound = (store.findExact('ID', id) >= 0);
        if (!itemIsFound) {
            Ext.Array.forEach(nonAvailableList, function (itmVal, itmIdx) {
                if (itmVal.ID == id) {
                    item = itmVal;
                    return false;
                }
            });
            if (item) {
                store.loadData([item], true);
            }
        }
    },
    getItem: function () {
        var me = this, cfg = me.config;
        return cfg.currentItem;
    },
    getHasUpdated: function () {
        var me = this, cfg = me.config;
        return cfg.hasUpdated;
    },
    isItemTransitionable: function () {
        var me = this, cfg = me.config, item = me.getItem();
        return (item.ProcessStatusType == Selectors.MovementProcessStatuses.Pending);
    },
    setupForm: function () {
        var me = this, cfg = me.config, comboAndStore;
        if (!cfg.form) {
            // define the model for the form
            cfg.modelName = 'MovementRequestEditorItem';
            Ext.define(cfg.modelName, {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'ProcessStatusID', mapping: 'ProcessStatusID', type: 'string' },
                    { name: 'ProcessSubStatusID', mapping: 'ProcessSubStatusID', type: 'string' },
                    { name: 'AssignedTo', mapping: 'AssignedTo' },
                    { name: 'RejectionReasonID', mapping: 'RejectionReasonID', type: 'string' }
                ],
                idProperty: 'ID'
            });
            // create the status combo   
            createReferenceDataComboAndStore = me.createReferenceDataComboAndStore('Status', 'ProcessStatusID', false);
            cfg.formProcessStatusesStore = createReferenceDataComboAndStore.store;
            cfg.formProcessStatusesCmb = createReferenceDataComboAndStore.combo;
            // create the rejection reason combo   
            createReferenceDataComboAndStore = me.createReferenceDataComboAndStore('Rejection Reason', 'RejectionReasonID', false);
            cfg.formRejectionReasonsStore = createReferenceDataComboAndStore.store;
            cfg.formRejectionReasonsCmb = createReferenceDataComboAndStore.combo;
            // create the sub status combo   
            createReferenceDataComboAndStore = me.createReferenceDataComboAndStore('Sub Status', 'ProcessSubStatusID', true);
            cfg.formProcessSubStatusesStore = createReferenceDataComboAndStore.store;
            cfg.formProcessSubStatusesCmb = createReferenceDataComboAndStore.combo;
            // create the assigned to selector
            cfg.formAssignedToSelector = Ext.create('InPlaceSelectors.Controls.WebSecurityUserInPlaceSelector', {
                fieldLabel: 'AssignedTo',
                name: 'AssignedTo',
                searchTextBoxConfig: {
                    allowBlank: true
                },
                webSecurityUserID: cfg.webSecurityUserID
            });
            // create the save button       
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
            cfg.form = Ext.create('Ext.form.Panel', {
                border: 0,
                bodyPadding: 5,
                items: [
                    cfg.formProcessStatusesCmb,
                    cfg.formRejectionReasonsCmb,
                    cfg.formProcessSubStatusesCmb,
                    cfg.formAssignedToSelector
                ],
                listeners: {
                    add: function (src, cmp) {
                        cmp.on('change', function () { me.resetFormVisibility(); });
                    }
                }
            });
        }
    },
    save: function () {
        var me = this, cfg = me.config, frm = cfg.form.getForm(), updater = frm.getRecord();
        frm.updateRecord(updater);
        updater = updater.data
        updater.ID = cfg.currentItem.ID;
        updater.AssignedToID = updater.AssignedTo.ID;
        updater.ProcessStatusID = Ext.Number.from(updater.ProcessStatusID, 0);
        updater.ProcessSubStatusID = Ext.Number.from(updater.ProcessSubStatusID, 0);
        updater.RejectionReasonID = Ext.Number.from(updater.RejectionReasonID, 0);
        me.setLoading('Updating Movement Request...');
        cfg.service.UpdateMovement(updater, me.saveCallBack, me);
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
    setModelZeroToNull: function (mdl, mdlName) {
        if (mdl.get(mdlName) == '0') {
            mdl.set(mdlName, null);
        }
    },
    show: function (args) {
        var me = this, cfg = me.config, mdl, statusItems = [];
        args = $.extend(true, {
            item: null
        }, args);
        if (!cfg.referenceData) {
            Ext.MessageBox.alert('Error', 'Reference data must be passed in the config.');
            return false;
        }
        if (!args.item) {
            Ext.MessageBox.alert('Error', 'Item must be passed to the show function.');
            return false;
        }
        cfg.currentItem = args.item;
        cfg.hasUpdated = false;
        // load statuses conditionally
        if (me.isItemTransitionable()) {
            Ext.Array.forEach(cfg.referenceData.ProcessStatusList, function (itmVal, itmIdx) {
                if (itmVal.Description == 'Pending' || itmVal.Description == 'Accepted' || itmVal.Description == 'Rejected') {
                    statusItems.push(itmVal);
                }
            });
        } else {
            statusItems = cfg.referenceData.ProcessStatusList;
        }
        cfg.formProcessStatusesStore.loadData(statusItems, false);
        // setup the model
        mdl = Ext.create(cfg.modelName, args.item);
        me.setModelZeroToNull(mdl, 'RejectionReasonID');
        me.setModelZeroToNull(mdl, 'ProcessSubStatusID');
        // setup combos so they include redundant items if required
        me.ensureComboItem(mdl.data.RejectionReasonID, cfg.formRejectionReasonsStore, cfg.referenceData.NonRedundantRejectionReasonList, cfg.referenceData.RejectionReasonList);
        me.ensureComboItem(mdl.data.ProcessSubStatusID, cfg.formProcessSubStatusesStore, cfg.referenceData.NonRedundantProcessSubStatusList, cfg.referenceData.ProcessSubStatusList);
        // load data onto form and set values
        cfg.form.loadRecord(mdl);
        cfg.formAssignedToSelector.setValueFromWebService(args.item.AssignedToID);
        me.resetFormVisibility();
        me.callParent();
    },
    resetFormVisibility: function () {
        var me = this, cfg = me.config, psMdl = cfg.formProcessStatusesStore.findRecord('ID', cfg.formProcessStatusesCmb.getValue()), item = me.getItem(),
        isRejected = false, isRejectedSentToBeds, frm = cfg.form.getForm(), isTransitionable = me.isItemTransitionable();
        cfg.formProcessStatusesCmb.setReadOnly(!isTransitionable);
        if (psMdl) {
            isRejected = (psMdl.data.Description == 'Rejected');
            isRejectedSentToBeds = (psMdl.data.Description == 'Rejected (Sent)');
        }
        cfg.formRejectionReasonsCmb.setDisabled(!isRejected && !isRejectedSentToBeds);
        cfg.formRejectionReasonsCmb.setReadOnly(!isTransitionable);
        if (!isRejected && !isRejectedSentToBeds) {
            cfg.formRejectionReasonsCmb.clearValue();
        }
        cfg.formSave.setDisabled(!frm.isValid());
    }
});