Ext.define('Results.Controls.ResidentialPaymentResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.ResidentialPaymentActions',
        'Searchers.Controls.ResidentialPaymentSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function () {
        var me = this, cfg = me.config;

        // add selector control
        cfg.selectorControl = new Selectors.ResidentialPaymentSelector({
            request: {
                PageSize: 0
            }
        });

        cfg.selectorControl.OnItemContextMenu(function (args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function (args) {
            me.actionViewResidentialPayment();
        });

        cfg.webSvcProxy = Target.Abacus.Extranet.Apps.WebSvc.ResPayments;
        cfg.resPaymentUrl = SITE_VIRTUAL_ROOT + 'AbacusExtranet/Apps/Res/Payments/ViewRemittance.aspx';
        cfg.documentUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Documents/UserControls/Documents/DocumentDownloadHandler.axd?saveas=1&id=';
        // add can add new permission
        cfg.permissionCanViewPrint = me.resultSettings.HasPermission('CanViewPrint');
        cfg.verificationText = me.resultSettings.GetSearchParameterValue('verificationText');
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.ResidentialPaymentSearcher', { resultSettings: me.resultSettings });

        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.ResidentialPaymentActions', {
            resultSettings: me.resultSettings
        });
        cfg.searchPanel.on('onSearch', function (cmp, search) {
            if (search) {
                if (search.ProviderID == 0) {
                    Ext.Msg.alert('Residential Payments', 'Please specify a Provider.');
                }
            }
        }, me);

        // call parent's init
        this.callParent(arguments);
    },
    actionViewResidentialPayment: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (item && item.ID > 0) {
            window.open(me.getUrlForResidentialPayment(item));
        } else {
            Ext.Msg.alert('No item to view', 'Please select an item to view.');
        }
    },
    showContextMenu: function (args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, item = cfg.selectorControl.GetSelectedItem();

        me.createContextMenu(item);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    },
    createContextMenu: function (item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;

        cfg.contextMenu = {};
        cfg.contextMenu.menuPreview = Ext.create('Ext.menu.Item', {
            text: 'Preview',
            tooltip: 'Preview payment',
            cls: 'ActionPanels',
            iconCls: 'PreviewImage',
            listeners: {
                click: {
                    scope: me,
                    fn: function () {
                        me.previewPayment();
                    }
                }
            }
        });
        cfg.contextMenu.menuAccept = Ext.create('Ext.menu.Item', {
            text: 'Accept',
            tooltip: 'Accept payment',
            cls: 'ActionPanels',
            hidden: (item.StatusDesc != 'Provisional Published'),
            iconCls: 'AcceptImage',
            listeners: {
                click: {
                    scope: me,
                    fn: function () {
                        me.acceptPaymentPrompt();
                    }
                }
            }
        });
        cfg.contextMenu.menuReject = Ext.create('Ext.menu.Item', {
            text: 'Reject',
            tooltip: 'Reject payment',
            cls: 'ActionPanels',
            hidden: (item.StatusDesc != 'Provisional Published'),
            iconCls: 'BlockedImage',
            listeners: {
                click: {
                    scope: me,
                    fn: function () {
                        me.rejectPaymentPrompt();
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
                cfg.contextMenu.menuPreview,
                cfg.contextMenu.menuAccept,
                cfg.contextMenu.menuReject
        ]
        });
        ctxMenu = cfg.contextMenu;

        return ctxMenu;
    },
    getUrlForResidentialPayment: function (item) {
        var me = this, cfg = me.config, url = '#';
        if (item) {
            return cfg.resPaymentUrl + '?id=' + item.ID.toString()// + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    previewPayment: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();

        if (item.DocumentID != 0) {
            document.location.href = cfg.documentUrl + item.DocumentID.toString();
        } else {
            me.actionViewResidentialPayment();
        }
    },
    acceptPaymentPrompt: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();

        Ext.MessageBox.confirm({
            title: 'Confirm Payment Acceptance',
            //msg: 'Do you accept payment of ' + item.AmountSubmitted.toString().formatCurrency() + ' for the period ' + item.DateFrom.format('dd/MM/yyyy') + ' to ' + item.DateTo.format('dd/MM/yyyy') + '?',
            msg: cfg.verificationText,
            buttons: Ext.MessageBox.YESNO,
            icon: Ext.MessageBox.QUESTION,
            fn: me.acceptPayment,
            scope: me
        });
    },
    rejectPaymentPrompt: function () {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();

        Ext.MessageBox.confirm({
            title: 'Confirm Payment Rejection',
            msg: 'Do you reject payment of ' + item.AmountSubmitted.toString().formatCurrency() + ' for the period ' + item.DateFrom.format('dd/MM/yyyy') + ' to ' + item.DateTo.format('dd/MM/yyyy') + '?',
            buttons: Ext.MessageBox.YESNO,
            icon: Ext.MessageBox.QUESTION,
            fn: me.rejectPayment,
            scope: me
        });
    },
    acceptPayment: function (btn) {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();

        if (btn === 'yes') {
            webSvcResponse = cfg.webSvcProxy.AuthoriseRemittance(item.ID.toString())
            if (webSvcResponse.value) {
                Ext.MessageBox.confirm({
                    title: 'Payment Acceptance',
                    msg: 'The payment has been successfully accepted.',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.INFO,
                    scope: me
                });

                cfg.searchPanel.search();
            } else {
                Ext.MessageBox.confirm({
                    title: 'Payment Acceptance',
                    msg: 'The payment has been not been accepted:<br/><br/>' + webSvcResponse.error.Message,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR,
                    scope: me
                });
            }
        }
    },
    rejectPayment: function (btn) {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();

        if (btn === 'yes') {
            Ext.MessageBox.prompt({
                title: 'Payment Rejection',
                minWidth: 400,
                msg: 'Please specify a reason for the rejection of this payment<br/>(max. 100 chars):',
                buttons: Ext.MessageBox.OKCANCEL,
                multiline: true,
                prompt: { maxlength: 10 },
                fn: me.getReasonForReject,
                scope: me
            });
        }
    },
    getReasonForReject: function (btn, reason) {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();

        if (btn === 'ok') {
            if (reason != '') {
                webSvcResponse = cfg.webSvcProxy.RejectRemittance(item.ID.toString(), reason)
                if (webSvcResponse.value) {
                    Ext.MessageBox.confirm({
                        title: 'Payment Rejection',
                        msg: 'The payment has been successfully rejected for the following reason:<br/><br/>' + reason,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.INFO,
                        scope: me
                    });

                    cfg.searchPanel.search();
                } else {
                    Ext.MessageBox.confirm({
                        title: 'Payment Rejection',
                        msg: 'The payment has been not been rejected:<br/><br/>' + webSvcResponse.error.Message,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR,
                        scope: me
                    });
                }
            } else {
                me.rejectPayment('yes');
            }
        }
    }

});
