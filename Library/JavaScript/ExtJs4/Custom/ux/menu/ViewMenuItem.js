Ext.define('Target.ux.menu.ViewMenuItem', {
    extend: 'Ext.menu.Item',
    config: {
        viewMenuClass: 'ViewMenu',
        showChildMenuAlways: false
    },
    statics: {
        instanceCount: 0
    },
    constructor: function(config) {
        var me = this, cfg = me.config;
        this.initConfig(config);
        me.self.instanceCount++;
        this.callParent([config]);
        return this;
    },
    initComponent: function() {
        var me = this, cfg = me.config, viewItemsLen, menuItems = [], firstMenuItem;
        cfg.viewItems = (cfg.viewItems || []), viewItemsLen = cfg.viewItems.length, me.items = [];
        for (i = 0; i < viewItemsLen; i++) {
            var viewItem = me.getViewItem(cfg.viewItems[i]);
            if (viewItem) {
                var menuItem = me.setMenuItemProperties({}, null, viewItem);
                me.setMenuItemProperties({}, null, viewItem);
                menuItems.push(menuItem);
            }
        }
        firstMenuItem = menuItems[0];
        if (menuItems.length > 1 || cfg.showChildMenuAlways == true) {
            me.menu = {
                xtype: 'menu',
                items: menuItems
            }
        }
        me.cls = 'ViewMenu', me.href = firstMenuItem.href, me.iconCls = 'ViewImage', me.id = firstMenuItem.id + 'Top', me.text = 'View';
        me.setMenuItemProperties(me, null, firstMenuItem);
        this.callParent(arguments);
    },
    clickMenuItemHandler: function(itm) {
        Ext.getBody().mask(itm.viewMenuItemData.clickMessage);
    },
    getMenuItem: function(key) {
        var me = this;
        if (me.menu) {
            return me.menu.down('#' + me.getMenuItemId(key));
        } else {
            return me;
        }
    },
    getMenuItemId: function(key) {
        return 'ViewMenuItem' + this.self.instanceCount.toString() + key;
    },
    getViewItem: function(item) {
        var viewItem = Ext.merge({
            clickMessage: null,
            disabled: false,
            hidden: false,
            href: '#',
            key: null,
            text: null,
            tooltip: null
        }, item);
        if (!viewItem.key) {
            alert('key property of view menu item must be set');
            return null;
        };
        if (!viewItem.text) {
            viewItem.text = 'Not Set'
        }
        if (!viewItem.tooltip) {
            viewItem.tooltip = 'View ' + viewItem.text + '?';
        }
        if (!viewItem.clickMessage) {
            viewItem.clickMessage = 'Opening ' + viewItem.text + '...';
        }
        if (viewItem.disabled) {
            viewItem.href = '#';
        }
        return viewItem;
    },
    setMenuItem: function(item) {
        var me = this, cfg = me.config, viewItem = me.getViewItem(item), menuItem;
        if (viewItem) {
            menuItem = me.getMenuItem(viewItem.key);
            if (menuItem) {
                var menuItemEl = menuItem.getEl();
                me.setMenuItemProperties(menuItem, menuItemEl, viewItem);
                if (menuItem.getId() + 'Top' == me.getId() && me.menu) {
                    var viewItemParent = Ext.merge({}, viewItem);
                    menuItemEl = me.getEl();
                    viewItemParent.text = 'View';
                    if (viewItemParent.disabled == true && cfg.viewItems.length > 1) {
                        viewItemParent.disabled = false;
                        viewItemParent.href = '#';
                    }
                    me.setMenuItemProperties(me, menuItemEl, viewItemParent);
                }
            }
        }
    },
    setMenuItemProperties: function(mi, miEl, item) {
        var me = this, cfg = me.config;
        if (!miEl) {
            mi.cls = 'ViewMenu', mi.hidden = item.hidden, mi.href = item.href, mi.id = me.getMenuItemId(item.key), mi.text = item.text, mi.tooltip = item.tooltip, mi.disabled = item.disabled;
            if (mi.id == me.getId() && cfg.viewItems.length > 1) {
                mi.text = 'View';
                if (mi.disabled == true) {
                    mi.disabled = false;
                    mi.href = '#';
                }
            }
            mi.listeners = {
                click: {
                    fn: function(itm, evt, scp) {
                        if (itm.href != '#') {
                            me.clickMenuItemHandler(itm, evt);
                        } else {
                            Ext.MessageBox.show({
                                title: 'Error',
                                msg: 'You do not have permissions to execute this action.',
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        }
                    },
                    scope: me
                }
            };
        } else {
            miEl.dom.children[0].href = item.href;
            if (mi.disabled != item.disabled) {
                mi.setDisabled(item.disabled);
            }
            if (mi.hidden != item.hidden) {
                mi.setVisible(!item.hidden);
            }
            if (mi.text != item.text) {
                mi.setText(item.text);
            }
            if (mi.tooltip != item.tooltip) {
                mi.setTooltip(item.tooltip);
            }
        }
        mi.viewMenuItemData = item;
        return mi;
    },
    setMenuItems: function(items) {
        var me = this, itemsLen;
        items = (items || []), itemsLen = items.length;
        for (i = 0; i < itemsLen; i++) {
            me.setMenuItem(items[i]);
        }
    },
    viewDefault: function() {
        var me = this;
        me.clickMenuItemHandler(me);
        document.location = me.viewMenuItemData.href;
    }
});
