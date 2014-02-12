var pppdResultSettings, periodPaymentPanelDetailHeight, hidTotal, lblError, userID,
    domContractPeriodID, periodFrom, periodTo, lblTotal, txtContractValue, stdButtonMode, dteEndDate, lastPaymentToDate;

Ext.define('TargetExt.PeriodPaymentPlanDetail.PeriodPaymentPlanDetailControl', {
    requires: [
        'Actions.Controls.PeriodPaymentDetailActions',
        'Selectors.PeriodPaymentPlanDetailSelector',
        'TargetExt.PeriodPaymentPlanDetail.PeriodPaymentPlanDetailEditor'
    ],
    config: {
        actionPanel: null,
        selectorControl: null,
        selectorHeight: 0,
        resultSettings: null,
        pppDID: 0,
        stdButtonModeEdit: Target.Library.Web.UserControls.StdButtonsMode.Edit,
        stdButtonModeNew: Target.Library.Web.UserControls.StdButtonsMode.AddNew,
        stdButtonMode: stdButtonMode
    },
    initComponent: function () {
        var me = this, cfg = me.config;
        cfg.pppDID = domContractPeriodID;
        cfg.pDateFrom = periodFrom;
        cfg.pDateTo = periodTo;
        cfg.lblRunningTotal = lblTotal;
        cfg.lblErrorMsg = lblError;
        cfg.hidRunningTotal = hidTotal;
        cfg.txtContractValue = txtContractValue;
        cfg.resultSettings = pppdResultSettings;
        cfg.selectorHeight = periodPaymentPanelDetailHeight;
        cfg.dteEndDate = dteEndDate;
        cfg.userID = userID;
        cfg.lastPaymentToDate = lastPaymentToDate;
        //std button mode can edit?
        //cfg.stdBtnPermissionEdit = (cfg.stdButtonMode == cfg.stdButtonModeEdit || cfg.stdButtonMode == cfg.stdButtonModeNew);
        cfg.stdBtnPermissionEdit = (cfg.stdButtonMode == cfg.stdButtonModeEdit);
        // add the service
        cfg.service = Target.Abacus.Library.ContractPeriodPaymentPlan.Services.PeriodPaymentPlanService;
        // add selector control
        cfg.selectorControl = Ext.create('Selectors.PeriodPaymentPlanDetailSelector', {
            request: {
                PageSize: 0
            }
        });
        //set the selector to filter by domcontract id
        cfg.selectorControl.SetDomContractPeriodID(cfg.pppDID);
        //set the selector to filter by WebSecurityUserID
        cfg.selectorControl.SetWebSecurityUserID(cfg.userID);

        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function () {
            me.actionEdit();
        });
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.PeriodPaymentDetailActions', {
            resultSettings: cfg.resultSettings,
            stdBtnPermissionEdit: cfg.stdBtnPermissionEdit,
            listeners: {
                onNewButtonClicked: {
                    fn: function () {
                        me.actionNew();
                    },
                    scope: me
                }
            }
        });
        // add can add new permission
        cfg.permissionAddNew = cfg.resultSettings.HasPermission('CanAddNew');
        // add can edit permission
        cfg.permissionEdit = cfg.resultSettings.HasPermission('CanEdit');
        // add can delete permission
        cfg.permissionDelete = cfg.resultSettings.HasPermission('CanDelete');

        //create the panel
        var extPanel = Ext.create('Ext.panel.Panel', {
            layout: 'fit',
            border: 1,
            height: cfg.selectorHeight,
            renderTo: 'extContent',
            bbar: cfg.actionPanel
        });

        cfg.selectorControl.Load(extPanel);

    },
    actionNew: function () {
        var me = this, cfg = me.config, ctrl = me.getEditorControl();
        if (cfg.permissionEdit) {
            ctrl.show({ Item: null });
        }
    },
    actionDelete: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (cfg.permissionDelete) {
            me.actionDeleteItem(item);
        }
    },
    actionDeleteItem: function (item) {
        var me = this, cfg = me.config, runningTotal;

        Ext.MessageBox.confirm('Confirm', 'Are you sure you wish to delete this record?', function (answer) {
            if (answer == 'yes') {
                Ext.getBody().mask('Deleting...');
                cfg.service.DeletePeriodPaymentPlanDetail(item.ID, function (response) {
                    Ext.getBody().unmask();
                    if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                        return false;
                    }
                    //this needs to be done using a web service.
                    runningTotal = response.value.Item.CurrentPaymentDetailTotal;
                    cfg.lblRunningTotal.innerHTML = runningTotal.toFixed(2);
                    if (Number(txtContractValue.value).toFixed(2) != runningTotal.toFixed(2)) {
                        cfg.lblRunningTotal.className = "errorText";
                        cfg.lblErrorMsg.innerHTML = "Contract Value and Total sum of Period Payment Plan Detail lines must be the same";
                    }
                    else {
                        cfg.lblRunningTotal.className = "";
                        cfg.lblErrorMsg.innerHTML = "";
                    }
                    cfg.hidRunningTotal.value = runningTotal.toFixed(2);
                    if (response.value.Item.LatestPaymentToDate) {
                        $(cfg.dteEndDate).datepicker("option", "minDate", response.value.Item.LatestPaymentToDate);
                    }
                    cfg.selectorControl.SetRequestSelectedID(response.value.Item.PaymentPlanDetailIDBeforeDeletedID);
                    cfg.selectorControl.Load();
                }, me);
            }
        });

    },
    createBasicContextMenuItem: function (href, iconCls, text, tooltip, clickFunc) {
        return Ext.create('Ext.menu.Item', {
            cls: 'ActionPanels',
            href: href,
            iconCls: iconCls,
            text: text,
            tooltip: tooltip,
            listeners: {
                click: {
                    fn: function () {
                        clickFunc();
                    }
                }
            }
        });
    },
    actionEdit: function () {
        var me = this, cfg = me.config, ctrl = me.getEditorControl(), item = cfg.selectorControl.GetSelectedItem();
        if (cfg.permissionEdit && cfg.stdBtnPermissionEdit) {
            ctrl.show({ item: item });
        }
    },
    createContextMenu: function () {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuEdit = me.createBasicContextMenuItem('#', 'Edit', 'Edit', 'Edit this Payment Detail Record?', function () { me.actionEdit(); });
            cfg.contextMenu.menuDelete = me.createBasicContextMenuItem('#', 'Delete', 'Delete', 'Delete this Payment Detail Record?', function () { me.actionDelete(); });
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
    getEditorControl: function () {
        var me = this, cfg = me.config;
        if (!cfg.editorControl) {
            cfg.editorControl = Ext.create('TargetExt.PeriodPaymentPlanDetail.PeriodPaymentPlanDetailEditor', {
                resultSettings: cfg.resultSettings,
                domContractPeriodID: cfg.pppDID,
                periodFrom: cfg.pDateFrom,
                periodTo: cfg.pDateTo,
                txtContractValue: cfg.txtContractValue,
                lblRunningTotal: cfg.lblRunningTotal,
                hidRunningTotal: cfg.hidRunningTotal,
                lblErrorMsg: cfg.lblErrorMsg,
                dteEndDate: cfg.dteEndDate,
                userID: cfg.userID,
                lastPaymentToDate: cfg.lastPaymentToDate,
                listeners: {
                    close: {
                        fn: function (cmp) {
                            if (cmp.getHasUpdated()) {
                                cfg.selectorControl.SetRequestSelectedID(cmp.getItemID());
                                cfg.selectorControl.Load();
                            }
                        },
                        scope: me
                    }
                }
            });
        }
        return cfg.editorControl;
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu,
            selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        }
        cfg.contextMenu.menuEdit.setDisabled(!(Ext.String.trim(selectedItem.PaidStatus) == "" && cfg.permissionEdit && cfg.stdBtnPermissionEdit));
        cfg.contextMenu.menuDelete.setDisabled(!(Ext.String.trim(selectedItem.PaidStatus) == "" && cfg.permissionDelete && cfg.stdBtnPermissionEdit));
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    }
});