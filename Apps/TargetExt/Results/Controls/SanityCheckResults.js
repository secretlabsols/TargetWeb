Ext.define('Results.Controls.SanityCheckResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Target.ux.menu.ViewMenuItem',
        'TargetExt.SanityChecks.Editor'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
       
        // add selector control
        cfg.selectorControl = new Selectors.SanityChecksSelector({
            request: {
                PageSize: 0
            },
            showFilters: true
        });
        me.setupPermissions();
        cfg.selectorControl.OnItemContextMenu(function(args) {
            me.showContextMenu(args);
        });
        this.callParent(arguments);
    },
    createContextMenu: function(item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuEdit = me.createBasicContextMenuItem('#', 'Edit', 'Edit', 'Edit Validation Check?',
                function () {
                    me.actionEdit();
                });
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                    cfg.contextMenu.menuEdit
                ]
            });
        }
    },
    setupPermissions: function () {
        var me = this, cfg = me.config;
        cfg.permissionEdit = me.resultSettings.HasPermission('CanEdit');
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu,
        selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.menuEdit.setDisabled(!cfg.permissionEdit);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    getValueInt : function(item){
        if (item.Type == 0)
        return item.Value;
        else
        return null;
    },
    getValueString: function(item){
        if (item.Type == 1)
        return item.Value;
        else
        return null;
    },
    actionEdit: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
            cfg.editorControl = Ext.create('TargetExt.SanityChecks.Editor', {
                checkType: item.Type,
                performCheck: item.PerformCheck,
                withholdUpdate: item.WithholdUpdate,
                valueString: me.getValueString(item),
                valueInt: me.getValueInt(item),
                sanityCheckID: item.ID,
                comments: item.Comment,
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
        cfg.editorControl.show({ item: item });
    }
});
