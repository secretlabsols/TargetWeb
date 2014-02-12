Ext.define('Results.Controls.FinanceCodeMatrixResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.FinanceCodeMatrixActions',
        'TargetExt.FinanceCodes.FinanceCodeMatrixConfigurationEditor'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;
        //declare finance code configuration item
        cfg.currentFCMCItem = null;
        // add the service
        cfg.service = Target.Abacus.Library.FinanceCodes.Services.FinanceCodeMatrixService;
        // add selector control
        cfg.selectorControl = new Selectors.FinanceCodeMatrixSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionEdit(false);
        });
        // the finance code matrix url
        cfg.fcmUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/FinanceCodeMatrix/Enquiry/Edit.aspx';
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.FinanceCodeMatrixActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onNewButtonClicked: {
                    fn: function () {
                        me.actionNew();
                    },
                    scope: me
                },
                onConfigureButtonClicked: {
                    fn: function () {
                        me.actionConfigure();
                    },
                    scope: me
                }
            }
        });
        // add can add new permission
        cfg.permissionAddNew = me.resultSettings.HasPermission('CanAddNew');
        // add can edit permission
        cfg.permissionEdit = me.resultSettings.HasPermission('CanEdit');
        // add can delete permission
        cfg.permissionDelete = me.resultSettings.HasPermission('CanDelete');
        // get the current effective date
        cfg.service.GetFinanceCodeMatrixConfiguration(function (response) {
            if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                return false;
            }
            cfg.currentFCMCItem = response.value.Item;
        }, me);
        // call parents init
        this.callParent(arguments);
    },
    getFCMCItem: function () {
        var me = this, cfg = me.config;
        return cfg.currentFCMCItem;
    },
    actionConfigure: function () {
        var me = this, cfg = me.config;
        if (cfg.permissionEdit) {
            if (!cfg.configEditorControl) {
                cfg.configEditorControl = Ext.create('TargetExt.FinanceCodes.FinanceCodeMatrixConfigurationEditor', {
                    resultSettings: me.resultSettings,
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
    getUrlForFinanceCodeMatrix: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (cfg.permissionAddNew || cfg.permissionEdit) {
            if (item) {
                return cfg.fcmUrl + '?id=' + item.ID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
            }
        }
        return url;
    },
    actionDelete: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem(), webSvcresponse, continueDelete;
        if (cfg.permissionDelete) {
            continueDelete = true;

            Ext.getBody().mask('Checking for associated service orders...');
            webSvcResponse = cfg.service.GetNumberOfServiceOrdersAssociatedToFinanceCodeMatrix(item.ID);
            Ext.getBody().unmask();
            if (!CheckAjaxResponse(webSvcResponse, cfg.service.url)) {
                return false;
            }
            if (webSvcResponse.value.Item > 0) {
                Ext.MessageBox.confirm('Confirm', webSvcResponse.value.Item.toString() + ' service orders have been allocated to this finance code from this matrix entry.\n\Deleting this entry will cause them to be reconsidered which may result in them being recoded differently.\n This will not cause automatic adjustments to the ledger. If adjustments are required they must be done manually.\n\nService orders will be reconsidered when the service order import job is next run', function (answer) {
                    if (answer == 'yes') {
                        me.actionDeleteItem(item.ID);
                    }
                });
            }
            else {
                me.actionDeleteItem(item.ID);
            }
        }
    },
    actionEdit: function (isEditMode) {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem(), mode, itemFCMC = me.getFCMCItem();
        if (isEditMode) {
            if ((item.DateTo != null) && (Number(item.DateTo) < (itemFCMC.EffectiveDate))) {
                mode = 1;
            }
            else {
                mode = 3;
            }

        }
        else {
            mode = 1;
        }
        if (cfg.permissionEdit) {
            document.location.href = cfg.fcmUrl + '?id=' + item.ID.toString() + '&mode=' + mode.toString() + '&backUrl=' + escape(me.getBackUrl());
        }
    },
    actionNew: function () {
        var me = this, cfg = me.config;
        if (cfg.permissionAddNew) {
            document.location.href = cfg.fcmUrl + '?' + 'mode=2' + '&backUrl=' + escape(me.getBackUrl());
        }
    },
    actionDeleteItem: function (itemID) {
        var me = this, cfg = me.config;

        Ext.MessageBox.confirm('Confirm', 'Are you sure you wish to delete this record?', function (answer) {
            if (answer == 'yes') {
                Ext.getBody().mask('Deleting...');
                cfg.service.DeleteFinanceCodeMatrix(itemID, function (response) {
                    Ext.getBody().unmask();
                    if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                        return false;
                    }
                    cfg.selectorControl.Load();
                }, me);
            }
        });

    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuEdit = me.createBasicContextMenuItem('#', 'Edit', 'Edit', 'Edit this Contract Number Mapping?', function () { me.actionEdit(true) });
            cfg.contextMenu.menuDelete = me.createBasicContextMenuItem('#', 'Delete', 'Delete', 'Delete this Contract Number Mapping?', function () { me.actionDelete() });
            cfg.contextMenu.menu = Ext.create('Ext.menu.Menu', {
                allowOtherMenus: true,
                defaults: {
                    cls: 'ActionPanels'
                },
                items: [
                    cfg.contextMenu.menuEdit,
                    cfg.contextMenu.menuDelete
                ]
            });
            ctxMenu = cfg.contextMenu;
        }
        return ctxMenu;
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu,
        selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.menuEdit.setDisabled(!cfg.permissionEdit);
        cfg.contextMenu.menuDelete.setDisabled(!cfg.permissionDelete);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    }
});
