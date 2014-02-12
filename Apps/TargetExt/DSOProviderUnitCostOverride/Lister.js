Ext.define('TargetExt.DSOProviderUnitCostOverride.Lister', {
    extend: 'Ext.window.Window',
    requires: [
        'TargetExt.DSOProviderUnitCostOverride.Editor', 'Actions.Buttons.NewButton', 'Target.ux.form.field.DateTime'
    ],
    closeAction: 'hide',
    modal: true,
    title: 'Service Order',
    width: 1000,
    resizable: false,
    initComponent: function () {
        var me = this, cfg = me.config;
        //initialise WebService
        cfg.service = Target.Abacus.Library.ServiceOrder.ServiceOrderService;
        // create a container for the selector
        cfg.selectorContainer = Ext.create('Ext.panel.Panel', {
            border: 1,
            padding: '3 3 3 3',
            height: 300,
            layout: {
                type: 'fit',
                align: 'stretch'
            }
        });

        // create the selector control
        cfg.selector = Selectors.Helpers.GetSelectorControlInstance('DomServiceOrderProviderUnitCostOverrideSelector', { request: { PageSize: 10000 }, showFilters: true, showLoading: false });
        cfg.selector.OnLoaded(function () {
            cfg.selector.SetPagingVisible(false);
            me.setLoading(false);
        });
        cfg.selector.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });

        cfg.addButton = Ext.create('Actions.Buttons.NewButton', {
            listeners: {
                onNewButtonClicked: {
                    fn: function () {
                        me.createNewItem();
                    },
                    scope: me
                }
            }
        });

        cfg.toolBar = Ext.create('Ext.toolbar.Toolbar', {
            items: [cfg.addButton]
        });


        // add any items
        me.items = [
            cfg.selectorContainer,
            cfg.toolBar
        ];
        // call parent component
        me.callParent(arguments);


    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuItemEdit = Ext.create('Ext.menu.Item', {
                text: 'Edit',
                tooltip: 'Edit this Unit Cost Override?',
                cls: 'ActionPanels',
                iconCls: 'Edit',
                listeners: {
                    click: {
                        scope: me,
                        fn: function () {
                            me.showContactForm();
                        }
                    }
                }
            });
            cfg.contextMenu.menuItemDelete = Ext.create('Ext.menu.Item', {
                text: 'Delete',
                tooltip: 'Delete this Unit Cost Override?',
                cls: 'ActionPanels',
                iconCls: 'Delete',
                listeners: {
                    click: {
                        scope: me,
                        fn: function () {
                            me.deleteOverride();
                        }
                    }
                }
            });
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                    cfg.contextMenu.menuItemEdit,
                    cfg.contextMenu.menuItemDelete
                ]
            });
        }
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selector.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    show: function (args) {
        var me = this, cfg = me.config, paidStr = 'Paid', suspendedStr = 'Suspended';
        me.callParent();
        args = $.extend(true, {
            item: null
        }, args);
        cfg.hasSuspended = false;
        cfg.showArgs = args;

        me.setLoading('Loading...');
        cfg.DomServiceOrderID = args.DomServiceOrderID
        cfg.selector.SetDomServiceOrderID(args.DomServiceOrderID);
        cfg.selector.Load(cfg.selectorContainer);

    },
    showContactForm: function () {

        var me = this, cfg = me.config;
        var item = cfg.selector.GetSelectedItem();

        cfg.overrideFormContainer = Ext.create('TargetExt.DSOProviderUnitCostOverride.Editor', {
            domServiceOrderID: cfg.DomServiceOrderID,
            listeners: {
                onChanged: {
                    fn: function () {
                        me.reloadSelector();
                    },
                    scope: me
                }
            }
        });
        cfg.overrideFormContainer.show(item);
    },
    createNewItem: function () {
        var me = this, cfg = me.config, item;
        cfg.overrideFormContainer = Ext.create('TargetExt.DSOProviderUnitCostOverride.Editor', {
            domServiceOrderID: cfg.DomServiceOrderID,
            listeners: {
                onChanged: {
                    fn: function () {
                        me.reloadSelector();
                    },
                    scope: me
                }
            }
        });
        cfg.overrideFormContainer.show(item);
    },
    reloadSelector: function () {
        var me = this, cfg = me.config;
        cfg.selector.Load();
    },
    deleteOverride: function () {
        var me = this, cfg = me.config;
        Ext.MessageBox.confirm({
            title: 'Confirm',
            msg: 'Are you sure you wish to delete this record?.',
            buttons: Ext.MessageBox.YESNO,
            icon: Ext.MessageBox.QUESTION,
            fn: me.deleteConfirm,
            scope: me
        });


    },
    deleteConfirm: function (btn) {
        var me = this, cfg = me.config;
        var item = cfg.selector.GetSelectedItem();
        if (btn === 'yes') {
            cfg.service.DeleteDSOUnitCostOverride(item.ID, me.deleteDSOUnitCostOverrideCallBack, me);
        }
    },
    deleteDSOUnitCostOverrideCallBack: function (response) {
        var me = response.context, cfg = me.config;
        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
            return false;
        }
        cfg.selector.Load();
    }

});