Ext.define('TargetExt.ProviderInvoices.NotesEditor', {
    extend: 'Ext.window.Window',
    requires: [
        'TargetExt.CreditorPayments.SummaryView'
    ],
    config: {
        permissionCanAdd: false,
        permissionCanDelete: false,
        permissionCanEdit: false
    },
    closeAction: 'hide',
    modal: true,
    resizable: false,
    title: 'Non-Residential Provider Invoice Notes',
    width: 750,
    constructor: function(config) {
        var me = this, cfg = me.config;
        this.initConfig(config);
        this.callParent([config]);
        return this;
    },
    initComponent: function() {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.DomProviderInvoice.Services.DomProviderInvoicesService;
        // create a container for the selector
        cfg.selectorContainer = Ext.create('Ext.panel.Panel', {
            border: 1,
            padding: '3 3 0 3',
            height: 300,
            layout: {
                type: 'fit',
                align: 'stretch'
            }
        });
        cfg.providerInvoiceSummary = Ext.create('TargetExt.CreditorPayments.SummaryView', {});
        cfg.buttonInfo = Ext.create('Ext.Button', {
            cls: 'Selectors',
            iconCls: 'imgInfo',
            listeners: {
                mouseover: {
                    fn: function(cmp, e) {
                        cfg.providerInvoiceSummary.show(cfg.showArgs.item);
                        cfg.providerInvoiceSummary.alignTo(cmp);
                    },
                    scope: me
                },
                mouseout: {
                    fn: function(cmp, e) {
                        cfg.providerInvoiceSummary.hide();
                    },
                    scope: me
                }
            },
            text: 'Information'
        });
        cfg.buttonNew = me.createActionButton('New', 'imgAdd', 'Create a New Note?', function() { me.actionNew(); });
        cfg.buttonDelete = me.createActionButton('Delete', 'imgDelete', 'Delete this Note?', function() { me.actionDeleteConfirm(); });
        cfg.buttonCancel = me.createActionButton('Cancel', 'imgUndo', 'Cancel Changes this Note?', function() { me.actionUndo(); });
        cfg.buttonSave = me.createActionButton('Save', 'imgSave', 'Save this Note?', function() { me.actionSave(); });
        // create a notes control
        cfg.notesControl = Ext.create('Ext.form.field.TextArea', {
            xtype: 'textareafield',
            allowBlank: true,
            anchor: '100%',
            emptyText: 'Please enter a note...',
            name: 'Notes',
            listeners: {
                change: {
                    fn: function(cmp, newVal) {
                        var isChanged = false, isNew = true, isBlank = true, note;
                        if (cfg.selectedNote) {
                            note = cfg.selectedNote.data;
                            isBlank = (Ext.String.trim(newVal).length == 0);
                            isChanged = (note.Notes != newVal);
                            isNew = ((note.ID || 0) == 0);
                        }
                        me.setButtonsDisabled(null, null, !(isChanged || isNew), (isBlank || !(!isBlank && isChanged)));
                    },
                    scope: me
                }
            }
        });
        // create a container for the notes
        cfg.notesForm = Ext.create('Ext.form.Panel', {
            bbar: [
                cfg.buttonInfo,
                { xtype: 'tbfill' },
                cfg.buttonNew,
                cfg.buttonDelete,
                cfg.buttonCancel,
                cfg.buttonSave
            ],
            border: 1,
            defaultType: 'textfield',
            height: 200,
            layout: 'fit',
            items: [
                cfg.notesControl
            ],
            padding: '3 3 3 3',
            title: 'Note'
        });
        // create the selector control
        cfg.selector = Selectors.Helpers.GetSelectorControlInstance('ProviderInvoiceNoteSelector', { request: { PageSize: 10000 }, showFilters: false, showLoading: false });
        // hook up to selector control events
        cfg.selector.OnItemSelected(function(item) {
            me.actionNoteSelected(item);
        });
        cfg.selector.OnLoaded(function() {
            cfg.selector.SetPagingVisible(false);
            me.actionProviderInvoiceChanged();
            me.setLoading(false);
        });
        // add any items
        me.items = [
            cfg.selectorContainer,
            cfg.notesForm
        ];
        // call parent component
        me.callParent(arguments);
    },
    actionDelete: function() {
        var me = this, cfg = me.config, note = me.getCurrentNote();
        me.setLoading('Deleting...');
        cfg.service.DeleteDomProviderInvoiceNote(note.data.ID, me.actionDeleteCallBack, me);
    },
    actionDeleteCallBack: function(response) {
        var me = response.context, cfg = me.config, note = me.getCurrentNote();
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.selector.DeleteItem(note.data.ID);
        me.actionNoteSelected(cfg.selector.GetSelectedItem(), function() {
            cfg.selector.ClearSelectedItem();
            me.actionProviderInvoiceChanged();
            me.setButtonsDisabled(false, true, true, true);
        });
    },
    actionDeleteConfirm: function() {
        var me = this, cfg = me.config;
        me.confirmAction('Delete?', 'Are you sure you wish to delete this record?.', cfg.buttonDelete, function(answer) {
            if (answer === 'yes') {
                me.actionDelete();
            }
        });
    },
    actionNew: function() {
        var me = this, cfg = me.config;
        me.actionNoteSelected(cfg.selector.GetSelectedItem(), function() {
            cfg.selector.ClearSelectedItem();
            me.actionProviderInvoiceChanged();
            me.setButtonsDisabled(true, null, false, null);
            cfg.notesControl.setReadOnly(false);
            cfg.notesControl.focus(true);
        });
    },
    actionNoteSelected: function(item, callBackFunc) {
        var me = this, cfg = me.config, frm = cfg.notesForm.getForm(), mdl, note;
        if (item) {
            mdl = cfg.selector.GetModelItem(item.ID);
            note = me.getCurrentNote();
            if (note && note.isModified('Notes')) {
                me.confirmAction('Save?', 'You\'re about to lose your changes, would you like to save?', cfg.buttonSave, function(answer) {
                    if (answer === 'yes') {
                        me.actionSave(function() {
                            me.actionNoteSelectedConfirmed(item, mdl, true, callBackFunc);
                            cfg.selector.SetSelectedItem(item.ID, false);
                        });
                    } else {
                        me.actionNoteSelectedConfirmed(item, mdl, false, callBackFunc);
                    }
                });
            } else {
                me.actionNoteSelectedConfirmed(item, mdl, true, callBackFunc);
            }
        } else {
            mdl = cfg.selector.CreateModelItem();
            me.actionNoteSelectedConfirmed(item, mdl, true, callBackFunc);
        }

    },
    actionNoteSelectedConfirmed: function(item, mdl, commit, callBackFunc) {
        var me = this, cfg = me.config, frm = cfg.notesForm.getForm(), note = me.getCurrentNote(), isNew = true;
        if (note) {
            if (commit) {
                note.commit();
            } else {
                note.reject();
            }
        }
        frm.reset();
        frm.loadRecord(mdl);
        note = me.getCurrentNote();
        if (note) {
            note.commit();
            isNew = ((note.data.ID || 0) == 0);
        }
        cfg.notesControl.setReadOnly(!(cfg.permissionCanEdit && !isNew));
        me.setButtonsDisabled(!cfg.permissionCanAdd, !(cfg.permissionCanDelete && !isNew), true, true);
        cfg.selectedNote = note;
        if (callBackFunc) {
            callBackFunc();
        }
    },
    actionProviderInvoiceChanged: function() {
        var me = this, cfg = me.config, frm = cfg.notesForm.getForm();
        frm.reset();
        cfg.notesControl.setReadOnly(true);
        me.setButtonsDisabled(!cfg.permissionCanAdd, true, true, true);
    },
    actionSave: function(callBackFunc) {
        var me = this, cfg = me.config, note = me.getCurrentNote();
        me.setLoading('Saving...');
        cfg.service.SaveDomProviderInvoiceNote(cfg.showArgs.item.ChildID, (note.data.ID || 0), note.data.Notes, me.actionSaveCallBack, { context: me, callback: callBackFunc })
    },
    actionSaveCallBack: function(response) {
        var me = response.context.context, cfg = me.config, note = me.getCurrentNote(), item, frm = cfg.notesForm.getForm();
        me.setLoading(false);
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        item = response.value.Item.Item;
        frm = cfg.notesForm.getForm();
        if (!note.data.ID) {
            cfg.selector.InsertItem(item, 0);
            frm.updateRecord(cfg.selector.GetModelItem(item));
            note.commit();
            cfg.selector.SetSelectedItem(item.ID, false);
        } else {
            cfg.selector.UpdateItem(item);
        }
        if (response.context.callback) {
            response.context.callback();
        } else if (note) {
            note.commit();
        }
        me.setButtonsDisabled(!cfg.permissionCanAdd, !cfg.permissionCanDelete, true, true);
    },
    actionUndo: function() {
        var me = this, cfg = me.config;
        me.actionNoteSelectedConfirmed(cfg.selector.GetSelectedItem(), cfg.selectedNote, false);
    },
    close: function() {
        var me = this, cfg = me.config;
        me.actionNoteSelected(cfg.selector.GetSelectedItem(), function() {
            me.actionNew();
            me.hide();
        });
    },
    confirmAction: function(title, msg, animateTarget, func) {
        var me = this;
        Ext.Msg.show({
            title: title,
            msg: msg,
            buttons: Ext.Msg.YESNO,
            icon: Ext.Msg.INFO,
            animateTarget: animateTarget,
            fn: func,
            scope: me
        });
    },
    createActionButton: function(text, iconCls, tooltip, handler) {
        var btn;
        btn = Ext.create('Ext.Button', {
            text: text,
            cls: 'Selectors',
            disabled: true,
            iconCls: iconCls,
            tooltip: tooltip,
            handler: function() {
                handler();
            }
        });
        return btn;
    },
    getCurrentNote: function() {
        var me = this, cfg = me.config,
        frm = cfg.notesForm.getForm(),
        frmNote = frm.getRecord();
        if (frmNote) {
            frm.updateRecord(frmNote);
        } else {
            frmNote = null;
        }
        return frmNote;
    },
    setButtonsDisabled: function(newBtnIsDisabled, delBtnIsDisabled, cancelBtnIsDisabled, saveBtnIsDisabled) {
        var me = this, cfg = me.config;
        if (!Ext.isEmpty(newBtnIsDisabled)) { cfg.buttonNew.setDisabled(newBtnIsDisabled); }
        if (!Ext.isEmpty(delBtnIsDisabled)) { cfg.buttonDelete.setDisabled(delBtnIsDisabled); }
        if (!Ext.isEmpty(cancelBtnIsDisabled)) { cfg.buttonCancel.setDisabled(cancelBtnIsDisabled); }
        if (!Ext.isEmpty(saveBtnIsDisabled)) { cfg.buttonSave.setDisabled(saveBtnIsDisabled); }
    },
    show: function(args) {
        var me = this, cfg = me.config;
        me.callParent();
        args = $.extend(true, {
            item: {}
        }, args);
        cfg.showArgs = args;
        me.setLoading('Loading...');
        cfg.selector.SetProviderInvoiceID(args.item.ChildID);
        cfg.selector.Load(cfg.selectorContainer);
    }
});