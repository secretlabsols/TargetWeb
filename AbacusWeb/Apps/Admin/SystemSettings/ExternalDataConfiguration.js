var externalFieldSvc, canToggleExternalIsMaster, canToggleExceptionIfBlank, canToggleAllowEditInAbacus;

$(function () {
    var externalFieldSelectorControl = 'Selectors.ExternalFieldSelector';

    Ext.require([externalFieldSelectorControl, 'Ext.window.MessageBox'])

    Ext.onReady(function () {

        var efSelectorControl = Ext.create(externalFieldSelectorControl, {
            request: {
                PageSize: 0
            }
        });

        externalFieldSvc = new Target.Abacus.Web.Apps.WebSvc.ExternalFields_class();

        var footerContainer = $('#footerContainer'), bottom = footerContainer.position().top + footerContainer.outerHeight();
        var selectorHeight = ((document.documentElement.clientHeight - bottom) - 1);

        var extPanel = Ext.create('Ext.panel.Panel', {
            layout: 'fit',
            border: 1,
            height: selectorHeight,
            renderTo: 'extContent'
        });

        efSelectorControl.Load(extPanel);
        efSelectorControl.OnItemContextMenu(function (args) {
            showContextMenu(args);
        });

        var contextMenu;

        function createContextMenu(item) {
            var ctxMenu = contextMenu;
            var showEditInAbacusMenu;

            showEditInAbacusMenu = (item.UIFriendlyTableName != 'Service Order - Funding' && item.UIFriendlyTableName != 'Service Order - Order');


            if (!ctxMenu) {
                contextMenu = {};
                contextMenu.menuInclude = Ext.create('Ext.menu.Item', {
                    text: 'Toggle "External Is Master" Flag',
                    tooltip: 'Mark this field to be mastered by an external system or not.',
                    cls: 'ActionPanels',
                    disabled: !canToggleExternalIsMaster,
                    iconCls: 'ToggleImage',
                    listeners: {
                        click: {
                            scope: this,
                            fn: function () {
                                toggleExternalIsMaster();
                            }
                        }
                    }
                });

                contextMenu.menuException = Ext.create('Ext.menu.Item', {
                    text: 'Toggle "Exception If Blank" Flag',
                    tooltip: 'Raise an exception if this field is blank when imported from an external system, and optionally withhold update if an exception is encountered.',
                    cls: 'ActionPanels',
                    disabled: !canToggleExceptionIfBlank,
                    iconCls: 'ToggleImage',
                    listeners: {
                        click: {
                            scope: this,
                            fn: function () {
                                toggleExceptionIfBlank();
                            }
                        }
                    }
                });

                contextMenu.menuEditableAbacus = Ext.create('Ext.menu.Item', {
                    text: 'Toggle "Allow Edit In ABACUS" Flag',
                    tooltip: 'Is this field editable in ABACUS.',
                    cls: 'ActionPanels',
                    disabled: !canToggleAllowEditInAbacus,
                    hidden: showEditInAbacusMenu,
                    iconCls: 'ToggleImage',
                    listeners: {
                        click: {
                            scope: this,
                            fn: function () {
                                toggleAllowEditInAbacus();
                            }
                        }
                    }
                });


                contextMenu.menu = Ext.create('Ext.menu.Menu', {
                    allowOtherMenus: true,
                    defaults: {
                        cls: 'ActionPanels'
                    },
                    items: [
                    contextMenu.menuInclude,
                    contextMenu.menuException,
                    contextMenu.menuEditableAbacus
                ]
                });
            }
            return contextMenu;
        };

        function toggleExternalIsMaster() {
            var item = efSelectorControl.GetSelectedItem(), webSvcResponse;
            webSvcResponse = externalFieldSvc.ToggleExternalIsMaster(item.ID.toString());
            if (!CheckAjaxResponse(webSvcResponse, externalFieldSvc.url)) {
                return false;
            }
            item.ExternalIsMaster = !item.ExternalIsMaster;
            efSelectorControl.UpdateItem(item);

        }

        function toggleExceptionIfBlank() {
            var item = efSelectorControl.GetSelectedItem();
            var showWithholdMessage;

            showWithholdMessage = (item.UIFriendlyTableName == 'Service Order - Funding' || item.UIFriendlyTableName == 'Service Order - Order');

            if (item.ExceptionIfBlank == false && showWithholdMessage) {
                Ext.MessageBox.confirm('Confirm', 'Would you like to withhold update when an exception is encountered on importing data?', confirmResult);
            } else {
                confirmResult('no');
            }


        }
        function confirmResult(btn) {
            var item = efSelectorControl.GetSelectedItem(), webSvcResponse;
            if (btn == 'yes') {
                webSvcResponse = externalFieldSvc.ToggleExceptionIfBlank(item.ID.toString(), true);
                if (!CheckAjaxResponse(webSvcResponse, externalFieldSvc.url)) {
                    return false;
                }
                item.WithholdUpdate = !item.WithholdUpdate;

            } else {
                webSvcResponse = externalFieldSvc.ToggleExceptionIfBlank(item.ID.toString(), false);
                if (!CheckAjaxResponse(webSvcResponse, externalFieldSvc.url)) {
                    return false;
                }
            }
            item.ExceptionIfBlank = !item.ExceptionIfBlank;
            if (item.ExceptionIfBlank == false) {
                item.WithholdUpdate = false;
            }
            efSelectorControl.UpdateItem(item);

        }

        function toggleAllowEditInAbacus() {
            var item = efSelectorControl.GetSelectedItem(), webSvcResponse;
            webSvcResponse = externalFieldSvc.ToggleAllowEditInAbacus(item.ID.toString());
            if (!CheckAjaxResponse(webSvcResponse, externalFieldSvc.url)) {
                return false;
            }
            item.AllowEditInAbacus = !item.AllowEditInAbacus;
            efSelectorControl.UpdateItem(item);

        }

        function showContextMenu(args) {
            var ctxMenu = contextMenu, selectedItem = efSelectorControl.GetSelectedItem();
            if (!ctxMenu) {
                createContextMenu(selectedItem);
            } else {
                var showEditInAbacusMenu;

                showEditInAbacusMenu = (selectedItem.UIFriendlyTableName == 'Service Order - Funding' || selectedItem.UIFriendlyTableName == 'Service Order - Order');

                contextMenu.menuEditableAbacus.setVisible(showEditInAbacusMenu);
            }
            contextMenu.menu.showAt(args.itemObject.xy);
        };

    });

});