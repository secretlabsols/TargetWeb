Ext.define('Results.Controls.ContractNumberMappingResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Buttons.ReportsButton',
        'Actions.Controls.ContractNumberMappingActions'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.service = Target.Abacus.Library.Contracts.Services.ContractsService;
        cfg.selectorControl = new Selectors.ContractNumberMappingSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.actionPanel = Ext.create('Actions.Controls.ContractNumberMappingActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onConfigButtonClicked: {
                    fn: function () {
                        me.actionConfigure();
                    },
                    scope: me
                },
                onNewButtonClicked: {
                    fn: function () {
                        me.actionNew();
                    },
                    scope: me
                }
            }
        });
        me.setupPermissions();
        cfg.selectorControl.OnItemContextMenu(function (args) { me.showContextMenu(args); });
        cfg.selectorControl.OnItemDoubleClick(function (args) { me.actionEdit(); });
        this.callParent(arguments);
    },
    actionConfigure: function () {
        var me = this, cfg = me.config;
        if (cfg.permissionEdit) {
            if (!cfg.configEditorControl) {
                cfg.configEditorControl = Ext.create('TargetExt.Contracts.ContractNumberMappingConfigurationEditor', {
                    listeners: {
                        close: {
                            fn: function (cmp) {
                                if (cmp.getHasUpdated()) {
                                    cfg.mappingEditorControl = null;
                                    Ext.MessageBox.confirm('Confirm', 'The configuration of this screen has been changed, in order to view these changes you must refresh the screen. Would you like to refresh the screen now?', function (answer) {
                                        if (answer == 'yes') {
                                            location.reload(true);
                                        }
                                    });
                                }
                            },
                            scope: me
                        }
                    }
                });
            }
            cfg.configEditorControl.show();
        }
    },
    actionCopy: function () {
        var me = this, cfg = me.config, ctrl = me.getMappingEditorControl(), item = cfg.selectorControl.GetSelectedItem();
        if (cfg.permissionAddNew) {
            item.ID = 0;
            ctrl.show({ item: item });
        }
    },
    actionDelete: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (cfg.permissionDelete) {
            Ext.MessageBox.confirm('Confirm', 'Are you sure you wish to delete this record?', function (answer) {
                if (answer == 'yes') {
                    Ext.getBody().mask('Deleting...');
                    cfg.service.DeleteContractNumberMapping(item.ID, function (response) {
                        Ext.getBody().unmask();
                        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                            return false;
                        }
                        cfg.selectorControl.Load();
                    }, me);
                }
            });
        }
    },
    actionEdit: function () {
        var me = this, cfg = me.config, ctrl = me.getMappingEditorControl(), item = cfg.selectorControl.GetSelectedItem();
        if (cfg.permissionEdit) {
            ctrl.show({ item: item });
        }
    },
    actionNew: function () {
        var me = this, cfg = me.config, ctrl = me.getMappingEditorControl();
        if (cfg.permissionAddNew) {
            ctrl.show({ Item: null });
        }
    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuEdit = me.createBasicContextMenuItem('#', 'Edit', 'Edit', 'Edit this Contract Number Mapping?', function () { me.actionEdit() });
            cfg.contextMenu.menuCopy = me.createBasicContextMenuItem('#', 'CopyImage', 'Copy', 'Copy this Contract Number Mapping?', function () { me.actionCopy() });
            cfg.contextMenu.menuDelete = me.createBasicContextMenuItem('#', 'Delete', 'Delete', 'Delete this Contract Number Mapping?', function () { me.actionDelete() });
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                    cfg.contextMenu.menuEdit,
                    cfg.contextMenu.menuCopy,
                    cfg.contextMenu.menuDelete
                ]
            });
            ctxMenu = cfg.contextMenu;
        }
        return ctxMenu;
    },
    getMappingEditorControl: function () {
        var me = this, cfg = me.config;
        if (!cfg.mappingEditorControl) {
            cfg.mappingEditorControl = Ext.create('TargetExt.Contracts.ContractNumberMappingEditor', {
                listeners: {
                    close: {
                        fn: function (cmp) {
                            if (cmp.getHasUpdated()) {
                                cfg.selectorControl.Load();
                            }
                        },
                        scope: me
                    }
                }
            });
        }
        return cfg.mappingEditorControl;
    },
    setupPermissions: function () {
        var me = this, cfg = me.config;
        cfg.permissionAddNew = me.resultSettings.HasPermission('ContractNumberMappings.AddNew');
        cfg.permissionConfigure = me.resultSettings.HasPermission('ContractNumberMappings.Configure');
        cfg.permissionDelete = me.resultSettings.HasPermission('ContractNumberMappings.Delete');
        cfg.permissionEdit = me.resultSettings.HasPermission('ContractNumberMappings.Edit');
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu,
        selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.menuEdit.setDisabled(!cfg.permissionEdit);
        cfg.contextMenu.menuCopy.setDisabled(!cfg.permissionAddNew);
        cfg.contextMenu.menuDelete.setDisabled(!cfg.permissionDelete);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    }
});
