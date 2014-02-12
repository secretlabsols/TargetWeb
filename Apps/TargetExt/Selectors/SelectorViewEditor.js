if (typeof Selectors == "undefined") Selectors = {};

Selectors.SelectorViewEditor = function(initSettings) {

    var self = this;
    var settings = $.extend(true, {
        applicationId: 0,
        service: null,
        type: 0,
        events: {
            onDeleted: null,
            onDeleting: null,
            onSaving: null,
            onSaved: null
        }
    }, initSettings);
    var extSettings = {
        isModelCreated: false,
        parentPanel: null,
        form: null,
        store: null,
        item: null
    }

    function adviseDeleted() {
        if ($.isFunction(settings.events.onDeleted)) { settings.events.onDeleted({ Item: extSettings.item.data }); }
    }

    function adviseDeleting() {
        if ($.isFunction(settings.events.onDeleting)) { settings.events.onDeleting({ Item: extSettings.item.data }); }
    }

    function adviseSaved(succeeded) {
        if ($.isFunction(settings.events.onSaved)) { settings.events.onSaved({ Item: ((succeeded) ? extSettings.item.data : null), Succeeded: succeeded }); }
    }

    function adviseSaving() {
        if ($.isFunction(settings.events.onSaving)) { settings.events.onSaving({ Item: extSettings.item.data }); }
    }

    function createParentPanel() {
        if (!extSettings.parentPanel) {
            extSettings.form = Ext.create('Ext.form.Panel', {
                bodyPadding: 5,
                border: 1,
                padding: '5 5 0 5',
                defaultType: 'textfield',
                fieldDefaults: {
                    msgTarget: 'side',
                    labelWidth: 75
                },
                items: [
                    {
                        allowBlank: false,
                        anchor: '100%',
                        emptyText: 'Please enter a name...',
                        fieldLabel: 'Name',
                        maxLength: 50,
                        name: 'Name'
                    }
                ]
            });
            extSettings.parentPanel = Ext.create('Ext.Panel', {
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                items: [extSettings.form]
            });
        }
    }

    function createModel() {
        if (!extSettings.isModelCreated) {
            Ext.define('SelectorView', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'ID', mapping: 'ID' },
                    { name: 'IsDefault', mapping: 'IsDefault' },
                    { name: 'Name', mapping: 'Name' },
                    { name: 'Columns', mapping: 'Columns' },
                    { name: 'Type', mapping: 'Type' }
                ],
                idProperty: 'ID'
            });
        }
    }

    function createStore() {
        if (!extSettings.store) {
            extSettings.store = Ext.create('Ext.data.ArrayStore', {
                data: [],
                model: 'SelectorView'
            });
        }
    }

    function deleteRecordCallBack(response) {
        extSettings.parentPanel.enable();
        if (!CheckAjaxResponse(response, settings.service.url, true)) {
            return false;
        }
        adviseDeleted();
    }

    function saveRecordCallBack(response) {
        var succeeded = true;
        extSettings.parentPanel.enable();
        if (!CheckAjaxResponse(response, settings.service.url, true)) {
            succeeded = false;
        }
        if (succeeded) {
            extSettings.item.data = response.value.Item;
        }
        adviseSaved(succeeded);
    }

    this.CreateRecord = function(args) {
        args = $.extend(true, {
            Item: []
        }, args);
        if (!args.Item) {
            alert('Please specify args.Item');
            return;
        }
        return Ext.create('SelectorView', args.Item);
    }

    this.DeleteRecord = function() {
        adviseDeleting();
        extSettings.parentPanel.disable();
        settings.service.DeleteSelectorView(extSettings.item.data.ID, deleteRecordCallBack);
    }

    this.GetView = function() {
        return extSettings.parentPanel;
    }

    this.LoadRecord = function(args) {
        var isNameDisabled = true;
        if (args) {
            settings.type = args.Type;
        }
        if (!settings.service) {
            settings.service = new Target.Abacus.Library.Selectors.Services.SelectorService_class();
        }
        createModel();
        createStore();
        createParentPanel();
        extSettings.item = self.CreateRecord(args);
        extSettings.store.load(extSettings.item);
        extSettings.form.loadRecord(extSettings.item);
        isNameDisabled = (extSettings.item.data.Type == 0);
        extSettings.form.items.items[0].setDisabled(isNameDisabled);
    }

    this.SaveRecord = function() {
        var form = extSettings.form.getForm();
        var record = form.getRecord();
        if (!form.isValid()) {
            Ext.Msg.alert('Validation Errors', 'Please ensure that all validation errors are corrected and try again.');
            return false;
        }
        if (!extSettings.item.data.Columns || extSettings.item.data.Columns.length == 0) {
            Ext.Msg.alert('Validation Errors', 'Please select at least one column.');
            return false;
        }
        adviseSaving();
        form.updateRecord(record);
        extSettings.parentPanel.disable();
        extSettings.item.data.SelectorType = settings.type;
        settings.service.SaveSelectorView(extSettings.item.data, saveRecordCallBack);
    }

};

Selectors.SelectorViewEditorWin = function(initSettings) {

    var self = this;
    var settings = $.extend(true, {
        events: {
            onHide: null
        },
        applicationId: 0,
        type: 0,
        width: 450
    }, initSettings);
    var extSettings = {
        editor: null,
        loadMask: null,
        window: null,
        lastSavedItem: null,
        hideReason: 0,
        saveButton: null,
        deleteButton: null
    }

    function adviseOnHide() {
        if ($.isFunction(settings.events.onHide)) { settings.events.onHide({ Reason: extSettings.hideReason, Item: extSettings.lastSavedItem }); }
    }

    function createWindow() {
        if (!extSettings.window) {
            extSettings.editor = new Selectors.SelectorViewEditor({
                applicationId: settings.applicationId,
                events: {
                    onDeleted: function(args) {
                        extSettings.lastSavedItem = args.Item;
                        extSettings.hideReason = 2;
                        extSettings.window.close();
                    },
                    onSaving: function(args) {
                        preventInput(true);
                    },
                    onSaved: function(args) {
                        if (args.Succeeded) {
                            extSettings.lastSavedItem = args.Item;
                            extSettings.hideReason = 1;
                            extSettings.window.close();
                        } else {
                            preventInput(false);
                        }
                    }
                }
            });
            extSettings.editor.LoadRecord();
            extSettings.deleteButton = Ext.create('Ext.Button', {
                text: 'Delete',
                cls: 'Selectors',
                iconCls: 'imgDelete',
                handler: function() {
                    preventInput(true);
                    extSettings.editor.DeleteRecord();
                },
                tooltip: 'Delete View Configuration?'
            });
            extSettings.saveButton = Ext.create('Ext.Button', {
                text: 'Save',
                cls: 'Selectors',
                iconCls: 'imgSave',
                handler: function() {
                    extSettings.editor.SaveRecord();
                },
                tooltip: 'Save View Configuration?'
            });
            extSettings.window = Ext.create('Ext.window.Window', {
                border: 0,
                bbar: [
                    { xtype: 'tbfill' },
                    extSettings.saveButton,
                    extSettings.deleteButton
                ],
                closeAction: 'hide',
                constrain: true,
                items: [extSettings.editor.GetView()],
                layout: {
                    type: 'fit',
                    align: 'stretch'
                },
                listeners: {
                    hide: function() {
                        preventInput(false);
                        adviseOnHide();
                    }
                },
                loadMask: true,
                height: 135,
                resizable: false,
                title: 'View Configuration',
                width: settings.width,
                modal: true
            });
        }
    }

    function preventInput(prevent) {
        extSettings.deleteButton.setDisabled(prevent);
        extSettings.saveButton.setDisabled(prevent);
    }

    this.Show = function(args) {
        args = $.extend(true, {
            Item: null,
            Type: 0
        }, args);
        settings.type = args.Type;
        createWindow();
        extSettings.hideReason = 0;
        extSettings.lastSavedItem = args.Item;
        extSettings.deleteButton.setDisabled((extSettings.lastSavedItem.ID <= 0));
        extSettings.window.show();
        extSettings.editor.LoadRecord(args);
    }

};