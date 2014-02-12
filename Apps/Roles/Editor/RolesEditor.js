var Roles = {};

Roles.RolesEditor = function(initSettings) {

    var self = this;
    var settings = $.extend(true, {
        events: {
            onUpdated: null
        },
        applicationId: 0,
        border: 0,
        padding: 0
    }, initSettings);
    var extSettings = {
        buttonPermissionGrant: null,
        buttonPermissionDeny: null,
        groupingFeature: null,
        store: null,
        toolbar: null,
        view: null
    }

    function adviseUpdated() {
        if ($.isFunction(settings.events.onUpdated)) { settings.events.onUpdated({ items: self.GetChangedRoles() }); }
    }

    function createStore(roles) {
        if (!extSettings.store) {
            Ext.QuickTips.init();
            Ext.define('Reports', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'ID', mapping: 'ID' },
                    { name: 'IsGranted', mapping: 'IsGranted' },
                    { name: 'Name', mapping: 'Name' },
                    { name: 'System', mapping: 'System' }
                ],
                idProperty: 'ID',
                setIsGranted: function(val) {
                    this.set('IsGranted', val);
                }
            });
            extSettings.store = Ext.create('Ext.data.ArrayStore', {
                data: roles,
                groupField: ((settings.applicationId == 0) ? 'System' : null),
                model: 'Reports'
            });
            extSettings.store.on('update', function(store, rcd, idx, opts) {
                adviseUpdated();
            });
            extSettings.store.sort('Name', 'ASC');
            if (settings.applicationId == 0) {
                extSettings.groupingFeature = Ext.create('Ext.grid.feature.Grouping', {
                    groupHeaderTpl: '{name} ({rows.length} Item{[values.rows.length > 1 ? "s" : ""]})'
                });
            }
        }
    }

    function createView() {
        if (!extSettings.view) {
            extSettings.buttonPermissionGrant = Ext.create('Ext.button.Split', {
                iconCls: 'imgAccept',
                text: 'Grant',
                tooltip: 'Grant permission to all selected roles?',
                menu: new Ext.menu.Menu(
                    { items: [
                            {
                                text: 'All',
                                handler: function() {
                                    setAllIsGranted(true);
                                }
                            },
                            {
                                text: 'Selected',
                                handler: function() {
                                    setSelectedIsGranted(true);
                                }
                            }
                        ]
                    }
                ),
                handler: function() {
                    setSelectedIsGranted(true);
                }
            });
            extSettings.buttonPermissionDeny = Ext.create('Ext.button.Split', {
                iconCls: 'imgDelete',
                text: 'Deny',
                tooltip: 'Deny permission to all selected roles?',
                menu: new Ext.menu.Menu(
                    { items: [
                            {
                                text: 'All',
                                handler: function() {
                                    setAllIsGranted(false);
                                }
                            },
                            {
                                text: 'Selected',
                                handler: function() {
                                    setSelectedIsGranted(false);
                                }
                            }
                        ]
                    }
                ),
                handler: function() {
                    setSelectedIsGranted(false);
                }
            });
            extSettings.toolbar = Ext.create('Ext.toolbar.Toolbar', {
                border: 0,
                dock: 'top',
                items: [
                    extSettings.buttonPermissionGrant,
                    extSettings.buttonPermissionDeny,
                    { xtype: 'tbfill' },
                    {
                        tooltip: {
                            text: 'To grant or deny permissions to a single item click a row below. This will toggle the items grant/deny status<br /><br />'
                                + 'To grant or deny permissions to multiple items use the \'Grant\' and \'Deny\' toolbar menus. Multiple items can be selected using the Ctrl and Shift keys on your keyboard.'
                        },
                        handler: function() {
                            setSelectedIsGranted(true);
                        },
                        iconCls: 'imgInformation'
                    }

                ]
            });
            extSettings.view = Ext.create('Ext.grid.Panel', {
                autoWidth: true,
                border: settings.border,
                cls: 'RolesEditor',
                columns: [
                    {
                        dataIndex: 'Name',
                        flex: 1,
                        menuDisabled: true,
                        resizable: false,
                        sortable: true,
                        text: 'Name'
                    }, {
                        dataIndex: 'IsGranted',
                        menuDisabled: true,
                        renderer: roleIsGrantedRenderer,
                        resizable: false,
                        sortable: true,
                        width: 30
                    }
                ],
                enableColumnHide: false,
                enableColumnMove: false,
                features: ((extSettings.groupingFeature) ? extSettings.groupingFeature : []),
                flex: 10,
                multiSelect: true,
                padding: settings.padding,
                store: extSettings.store,
                tbar: extSettings.toolbar
            });
            extSettings.view.on('itemclick', function(vw, rcd, itm, idx, e, opts) {
                if (!e.browserEvent.ctrlKey) {
                    rcd.setIsGranted(!rcd.data.IsGranted);
                }
            });
        }
    }

    function roleIsGrantedRenderer(val) {
        if (val) {
            return '<div class="imgAccept" style="width: 16px; height: 16px;"></div>';
        } else {
            return '<div class="imgDelete" style="width: 16px; height: 16px;"></div>';
        }
        return val;
    }

    function setAllIsGranted(isGranted) {
        extSettings.store.each(function(mdl) {
            mdl.setIsGranted(isGranted);
        });
    }

    function setSelectedIsGranted(isGranted) {
        $.each(extSettings.view.getSelectionModel().selected.items, function(mdlIdx, mdl) {
            mdl.setIsGranted(isGranted);
        });
    }

    this.GetChangedRoles = function() {
        var changedRoles = [];
        if (extSettings.store) {
            var tmpChangedRoles = extSettings.store.getUpdatedRecords();
            $.each(tmpChangedRoles, function(crIdx, crVal) {
                changedRoles.push(crVal.data);
            });
        }
        return changedRoles;
    }

    this.GetGrantedRoles = function() {
        var grantedRoles = [];
        if (extSettings.store) {
            var tmpGrantedRoles = extSettings.store.getRange(0, (extSettings.store.getCount() - 1));
            $.each(tmpGrantedRoles, function(idx, val) {
                if (val.data.IsGranted) {
                    grantedRoles.push(val.data);
                }
            });
        }
        return grantedRoles;
    }

    this.GetGrantedRolesIdsOnly = function() {
        var grantedRoles = self.GetGrantedRoles(), grantedRolesRet = [];
        $.each(grantedRoles, function(idx, val) {
            grantedRolesRet.push(val.ID);
        });
        return grantedRolesRet;
    }

    this.GetView = function() {
        return extSettings.view;
    }

    this.LoadRoles = function(args) {
        args = $.extend(true, {
            application: null,
            roles: []
        }, args);
        createStore(args.roles);
        createView();
    }

    this.SetRolePermissions = function(args) {
        args = $.extend(true, {
            rolePermissions: []
        }, args);
        extSettings.store.each(function(mdl) {
            extSettings.store.suspendEvents(false);
            mdl.suspendEvents(false);
            if (args.rolePermissions) {
                var pValFound = false;
                $.each(args.rolePermissions, function(pIdx, pVal) {
                    if (pVal.ID == mdl.data.ID) {
                        mdl.setIsGranted(pVal.IsGranted);
                        pValFound = true;
                        return false;
                    }
                });
                if (!pValFound) {
                    mdl.setIsGranted(false);
                }
            }
            mdl.commit();
            mdl.resumeEvents();
            extSettings.store.resumeEvents();
        });
    }

    this.SetRolePermissionsByIdArray = function(args) {
        var tmpRolePermissions = [];
        args = $.extend(true, {
            rolePermissions: []
        }, args);
        $.each(args.rolePermissions, function(idIdx, idVal) {
            tmpRolePermissions.push({ ID: idVal, IsGranted: true });
        });
        args.rolePermissions = tmpRolePermissions;
        self.SetRolePermissions(args);
    }

};