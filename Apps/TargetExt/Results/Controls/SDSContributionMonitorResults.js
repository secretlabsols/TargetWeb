Ext.define('Results.Controls.SDSContributionMonitorResults', {
    extend: 'Results.ResultControl',
    requires: [
        'Actions.Controls.SDSContributionMonitorActions',
        'Searchers.Controls.SDSContributionMonitorSearcher',
        'Target.ux.menu.ViewMenuItem'
    ],
    initComponent: function() {
        var me = this, cfg = me.config;
        // add can view service users setting value
        cfg.canViewServiceUsers = me.resultSettings.HasPermission('CanViewServiceUsers');
        //the SDS Contribution Monitor Service
        cfg.service = Target.Abacus.Library.SDS.ServiceUserContributionLevel.Services.SDSContributionMonitorService;
        // add selector control
        cfg.selectorControl = new Selectors.SDSContributionMonitorSelector({
            request: {
                PageSize: 0
            }
        });
        cfg.selectorControl.OnItemContextMenu(function(args) {
            me.showContextMenu(args);
        });
        cfg.selectorControl.OnItemDoubleClick(function(args) {
            me.actionViewServiceUser();
        });
        // the service user url
        cfg.svcUserUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/ServiceUsers/Enquiry/Edit.aspx';
        // the Create SDS contribution notifications url
        cfg.jbSvcUrl = SITE_VIRTUAL_ROOT + 'AbacusWeb/Apps/Jobs/Default.aspx';
        // add action panel
        cfg.actionPanel = Ext.create('Actions.Controls.SDSContributionMonitorActions', {
            resultSettings: me.resultSettings,
            listeners: {
                onReportsButtonClicked: {
                    fn: function(cmp) {
                        cmp.viewReports(me.mapResultsToReports());
                    },
                    scope: me
                },
                onNotifyAllButtonClicked: {
                    fn: function() {
                        me.actionNewNotificationLetterAll();
                    },
                    scope: me
                }
            }
        });
        //disbable Notify All button by default
        cfg.actionPanel.setNotifyAllButtonDisabled(true);
        // add search panel
        cfg.searchPanel = Ext.create('Searchers.Controls.SDSContributionMonitorSearcher', {
            resultSettings: me.resultSettings,
            width: 460
        });
        cfg.searchPanel.on('onSearch', me.handleSearchEvent, me);
        // call parents init
        this.callParent(arguments);
    },
    actionViewServiceUser: function() {
        var me = this, cfg = me.config, item = cfg.selectorControl.GetSelectedItem();
        if (cfg.canViewServiceUsers) {
            if (item) {
                me.createContextMenu(item);
                cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(item));
                cfg.contextMenu.menuView.viewDefault();
            } else {
                Ext.Msg.alert('No Item to View', 'Please select an item to View.');
            }
        }
        else {
            Ext.Msg.alert('Authorisation Required', 'You do not have permission to view Service Users.');
        }
    },
    createContextMenu: function(item) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, item = cfg.selectorControl.GetSelectedItem(), msgLoading = 'Creating New Contribution Notification Letter...';
        if (!ctxMenu) {
            cfg.contextMenu = {};
            cfg.contextMenu.menuItemNotify = Ext.create('Ext.menu.Item', {
                text: 'Notify',
                tooltip: 'Generate a new Contribution Notification letter?',
                cls: 'ActionPanels',
                iconCls: 'NotifyImage',
                handler: function() {
                    me.setDisabled(true);
                    Ext.getBody().mask(msgLoading);
                    cfg.service.GenerateNewContributionNotificationLetter(item.ID, function(response) {
                        me.setDisabled(false);
                        Ext.getBody().unmask();
                        if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                            return false;
                        }
                        item.LetterDue = false;
                        cfg.selectorControl.UpdateItem(item);
                    });
                }
            });
            cfg.contextMenu.menuView = Ext.create('Target.ux.menu.ViewMenuItem', {
                viewItems: me.getViewMenuItems(item)
            });
            cfg.reportsMenu = Ext.create('Actions.Buttons.ReportsButtonMenuItem', {
                tooltip: 'View Reports for the Selected SDS Contribution?',
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
                    cfg.contextMenu.menuItemNotify,
                    cfg.reportsMenu
                ]
            });
        }
    },
    getViewMenuItems: function(item) {
        var me = this, viewMenuItems = [];
        viewMenuItems.push({ href: me.getUrlForServiceUser(item), key: 'ServiceUser', text: 'View', tooltip: 'View Service User' });
        return viewMenuItems;
    },
    getUrlForServiceUser: function(item) {
        var me = this, cfg = me.config, url = '#';
        if (cfg.canViewServiceUsers) {
            if (item) {
                return cfg.svcUserUrl + '?clientid=' + item.ID.toString() + '&mode=1' + '&showSdsContribs=1' + '&expandChildren=1' + '&backUrl=' + escape(me.getBackUrl());
            }
        }
        return url;
    },
    actionNewNotificationLetterAll: function() {
        var me = this, jobID = 0, cfg = me.config, url = '#', msgLoading = 'Creating SDS Contribution Notifications...';

        me.setDisabled(true);
        Ext.getBody().mask(msgLoading);
        cfg.service.CreateSDSNotificationJob(function(response) {
            me.setDisabled(false);
            Ext.getBody().unmask();
            if (!CheckAjaxResponse(response, cfg.service.url, true)) {
                return false;
            }
            else {
                //get the new jobID returned from the response object
                jobID = response.value.Item;
                document.location.href = cfg.jbSvcUrl + '?' + 'jobID=' + jobID.toString();
            }

        });
    },
    handleSearchEvent: function(args, srch) {
        var me = this, cfg = me.config;
        //set notify all button enable
        cfg.actionPanel.setNotifyAllButtonDisabled(!(srch.ServiceUsersAreBeingCollected === 1 && srch.ServiceUsersRequiringContributionNotificationLetter === 1 && srch.ServiceUsersHavingOneOrMoreIncompleteContributionLevel === 2));
    },
    showContextMenu: function(args) {
        var me = this, cfg = me.config, ctxMenu = cfg.contextMenu, selectedItem = cfg.selectorControl.GetSelectedItem();
        if (!ctxMenu) {
            me.createContextMenu(selectedItem);
        } else {
            cfg.contextMenu.menuView.setMenuItems(me.getViewMenuItems(selectedItem));
        }
        cfg.contextMenu.menuItemNotify.setDisabled(!(selectedItem.LetterDue == true && selectedItem.Incomplete == 'No'));
        cfg.contextMenu.menuView.setDisabled(!cfg.canViewServiceUsers);
        cfg.contextMenu.menu.showAt(args.itemObject.xy);
    }
});
