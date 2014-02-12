Ext.define('Results.Controls.RateCategoryResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.RateCategoryActions',
        'Searchers.Controls.RateCategorySearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add selector control
        cfg.selectorControl = new Selectors.RateCategorySelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function(args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function(args) {
            me.actionViewRateCategory();
        });
        cfg.webSvcProvxy = Target.Abacus.Web.Apps.WebSvc.DomContract;
        cfg.rateCategoryUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Admin/Dom/RateCategories.aspx';
        cfg.sortOrderUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Admin/Dom/RateOrdering.aspx';
        cfg.enhancedRatesUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Admin/Dom/RateEnhancedEquiv.aspx';
        cfg.rateFrameworkUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Admin/Dom/RateFrameworks.aspx';
        cfg.InclusionsUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Admin/Dom/RateInclusions.aspx';
        cfg.PreclusionsUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Admin/Dom/RatePreclusions.aspx';
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.RateCategoryActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onReportsButtonClicked: {
                    fn: function(cmp) {
                        cmp.viewReports(me.mapResultsToReports());
                    },
                    scope: me
                },
                onNewButtonClicked: {
                    fn: function() {
                        me.actionNewRateCategory();
                    },
                    scope: me
                },
                onSortOrderButtonClicked: {
                    fn: function() {
                        me.actionViewSortOrder();
                    },
                    scope: me
                },
                onEnhancedRatesButtonClicked: {
                    fn: function() {
                        me.actionViewEnhancedRates();
                    },
                    scope: me
                }
            }
        });
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.RateCategorySearcher', { resultSettings: me.resultSettings });
        // add can add new permission
        cfg.permissionCanAddNew = me.resultSettings.HasPermission('AddNew');
        cfg.permissionCanViewFramework = me.resultSettings.HasPermission('ViewFramework');
        cfg.searchPanel.on('onSearch', function(cmp, search) {
            if (search) {
                cfg.actionPanel.disableNewButton(!me.canEditFramework(search.RateFrameworkID))
                if (search.RateFrameworkID > 0) {
                    cfg.actionPanel.disableSortOrderButton((search.RateFrameworkID == 0));
                    cfg.actionPanel.disableEnhancedRatesButton(search.RateFramework.Item.Enhanced == 'No');
                } else {
                    Ext.Msg.alert('Rate Categories', 'Please select a Rate Framework.');
                    cfg.actionPanel.disableSortOrderButton(true);
                    cfg.actionPanel.disableEnhancedRatesButton(true);
                }
            }
        }, me);
        // call parents init
        this.callParent(arguments);
    },
    actionNewRateCategory: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        var msgAccessDenied = 'You do not have permissions to create a new Rate Category.';
        var msgLoading = 'Creating Rate Category...';
        var url = me.getUrlForNewRateCategory(item);

        if (!cfg.permissionCanAddNew) {
            Ext.Msg.alert('Access Denied', msgAccessDenied);
            return false;
        }
        me.redirectToUrl(url, msgLoading);
    },
    actionViewRateCategory: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        var msgLoading = 'Loading Rate Category...';
        var url = me.getUrlForViewRateCategory(item);

        me.redirectToUrl(url, msgLoading);
    },
    actionViewSortOrder: function() {
        var me = this, cfg = me.config;
        var msgLoading = 'Loading sort order screen...';
        var url = me.getUrlForViewSortOrder();

        me.redirectToUrl(url, msgLoading);
    },
    actionViewEnhancedRates: function() {
        var me = this, cfg = me.config;
        var msgLoading = 'Loading enhanced rates screen...';
        var url = me.getUrlForEnhancedRates();

        me.redirectToUrl(url, msgLoading);
    },
    redirectToUrl: function(url, msg) {
        var me = this, cfg = me.config;
        cfg.selectorControl.ShowMask(false);
        Ext.getBody().mask(msg);
        document.location.href = url;
    },
    createContextMenu: function(item, hidePreclusionInclusion) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu;
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuItemInclusions = Ext.create('Ext.menu.Item', {
                text: 'Inclusions',
                tooltip: 'View/Set the Inclusions for this Rate Category',
                href: me.getUrlForViewInclusions(item, true),
                cls: 'ActionPanels',
                iconCls: 'InclusionsImage',
                hidden: hidePreclusionInclusion
            });
            cfg.contextMenu.menuItemPreclusions = Ext.create('Ext.menu.Item', {
                text: 'Preclusions',
                tooltip: 'View/Set the Preclusions for this Rate Category',
                href: me.getUrlForViewPreclusions(item, true),
                cls: 'ActionPanels',
                iconCls: 'PreclusionsImage',
                hidden: hidePreclusionInclusion
            });
            cfg.contextMenu.menuView = Ext.create('Target.ux.menu.ViewMenuItem', {
                viewItems: me.getViewMenuItems(item)
            });
            cfg.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Reports for the Selected Rate Category?',
                reportsButtonConfig: {
                    reportIds: me.resultSettings.ReportIdsForContextMenu,
                    listeners: {
                        onReportsButtonClicked: {
                            fn: function(cmp) {
                                cmp.viewReports(me.mapResultsToReportsForContextMenu());
                            },
                            scope: me
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
                    cfg.contextMenu.menuView,
                    cfg.contextMenu.menuItemInclusions,
                    cfg.contextMenu.menuItemPreclusions,
                    cfg.reportsMenu
                ]
            });
        }
    },
    canEditFramework: function(frameworkID) {
        var me = this, cfg = me.config;
        var webSvcResponse = null;

        if (frameworkID > 0) {
            webSvcResponse = cfg.webSvcProvxy.CanEditRateFramework(frameworkID);

            if (!CheckAjaxResponse(webSvcResponse, cfg.webSvcProvxy.url)) {
                return false;
            }
            return webSvcResponse.value.Value;
        } else {
            return false;
        }
    },
    getViewMenuItems: function(item) {
        var me = this, cfg = me.config, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForViewRateCategory(item), key: 'RateCategory', text: 'Rate Category' });
        viewMenuItems.push({ href: me.getUrlForViewRateFramework(item), key: 'RateFramework', text: 'Rate Framework', disabled: ((cfg.permissionCanViewFramework == false) ? true : false) });
        return viewMenuItems;
    },
    getUrlForNewRateCategory: function(item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        if (cfg.permissionCanAddNew) {
            var selectedFrameworkID = search.RateFrameworkID;

            url = cfg.rateCategoryUrl + '?' + 'frameworkID=' + selectedFrameworkID.toString() + '&mode=2' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForViewRateCategory: function(item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        if (cfg.permissionCanAddNew) {
            var selectedFrameworkID = search.RateFrameworkID;

            url = cfg.rateCategoryUrl + '?' + 'frameworkID=' + selectedFrameworkID.toString() + '&id=' + item.ID + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForViewSortOrder: function() {
        var me = this, cfg = me.config, url = '#';
        var search = cfg.searchPanel.getSearch();

        url = cfg.sortOrderUrl + '?frameworkID=' + search.RateFrameworkID + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        return url;
    },
    getUrlForEnhancedRates: function() {
        var me = this, cfg = me.config, url = '#';
        var search = cfg.searchPanel.getSearch();

        url = cfg.enhancedRatesUrl + '?ID=' + search.RateFrameworkID + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        return url;
    },
    getUrlForViewRateFramework: function(item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        var selectedFrameworkID = search.RateFrameworkID;

        url = cfg.rateFrameworkUrl + '?' + 'id=' + selectedFrameworkID.toString() + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        return url;
    },
    getUrlForViewInclusions: function(item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        if (cfg.permissionCanAddNew) {
            var selectedFrameworkID = search.RateFrameworkID;

            url = cfg.InclusionsUrl + '?' + 'frameworkID=' + selectedFrameworkID.toString() + '&id=' + item.ID + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    getUrlForViewPreclusions: function(item) {
        var me = this, cfg = me.config, search = cfg.searchPanel.getSearch(), itemID = 0, url = '#';
        if (cfg.permissionCanAddNew) {
            var selectedFrameworkID = search.RateFrameworkID;

            url = cfg.PreclusionsUrl + '?' + 'frameworkID=' + selectedFrameworkID.toString() + '&id=' + item.ID + '&mode=1' + '&backUrl=' + escape(me.getBackUrl());
        }
        return url;
    },
    showContextMenu: function(args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem();
        var search = cfg.searchPanel.getSearch();
        var hidePreclusionInclusion = ((search.RateFramework.Item.FrameworkType != 'Service Register') ? true : false)
        if (!ctxMenu) {
            me.createContextMenu(selectedItem, hidePreclusionInclusion);
        } else {
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(selectedItem));
            cfg.contextMenu.menuItemInclusions.setHref(me.getUrlForViewRateCategory(selectedItem));
            cfg.contextMenu.menuItemPreclusions.setHref(me.getUrlForViewRateCategory(selectedItem));
            cfg.contextMenu.menuItemInclusions.setVisible(!hidePreclusionInclusion);
            cfg.contextMenu.menuItemPreclusions.setVisible(!hidePreclusionInclusion);
        }
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    }
});
