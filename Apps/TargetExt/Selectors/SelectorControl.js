if (typeof Selectors == "undefined") Selectors = {};

Selectors.InstanceCount = 0;

Selectors.Helpers = {
    ToDateString: function (value) {
        if (!value || !Ext.isDate(value)) {
            value = '';
        }
        return Ext.Date.format(value, 'd/m/Y H:i:s A');
    },
    ToInt: function (value) {
        if (!value || isNaN(parseInt(value))) {
            value = 0;
        }
        return parseInt(value);
    },
    ToXml: function (list, rootName, elementName) {
        var xml = '<' + rootName + '>';
        if (list) {
            $.each(list, function (liIdx, liVal) {
                xml += '<' + elementName + '>' + liVal + '</' + elementName + '>';
            });
        };
        xml += '</' + rootName + '>';
        return xml;
    },
    GetKeysValue: function (list, key, returnProp, defaultValue) {
        var val = defaultValue;
        if (list && key && returnProp) {
            $.each(list, function (liIdx, liVal) {
                if (key.toUpperCase() == liVal.Key.toUpperCase()) {
                    val = liVal[returnProp];
                    return false;
                }
            });
        };
        return val;
    },
    GetParameterValue: function (list, key, defaultValue) {
        return Selectors.Helpers.GetKeysValue(list, key, 'Value', null);
    },
    GetSelectorControlInstance: function (className, args) {
        if (Selectors[className] === undefined) {
            $.ajax({ url: SITE_VIRTUAL_ROOT + 'Apps/TargetExt/Selectors/Controls/' + className + '.js', async: false, dataType: 'script' });
        }
        return new Selectors[className](args);
    },
    HasPermission: function (list, key) {
        return Selectors.Helpers.GetKeysValue(list, key, 'IsGranted', false);
    }
};

Selectors.SelectorControl = function (initSettings) {

    var self = this;
    var settings = $.extend(true, {
        control: null,
        events: {
            onItemContextMenuEvts: [],
            onItemMouseOverEvts: [],
            onItemMouseOutEvts: [],
            onItemDeletedEvts: [],
            onItemSelectedEvts: [],
            onItemDoubleClickEvts: [],
            onItemUpdatedEvts: [],
            onLoadingEvts: [],
            onLoadedEvts: [],
            onMruItemSelectedEvts: []
        },
        instanceIndex: 0,
        request: {
            IsMetaDataRequired: true,
            Page: 1,
            PageSize: 0,
            Parameters: [],
            SelectedID: 0,
            Type: null
        },
        service: null,
        showFilters: true,
        showLoading: true
    }, initSettings);
    var extSettings = {
        currentView: null,
        isSystemViewPresent: false,
        maxViewItemWidth: 800,
        modelName: null,
        mruSelected: false,
        parentPanel: null,
        pagingVisible: true,
        response: null,
        responseApplicationID: 1,
        responseColumns: null,
        responseCanMaintainDefaultView: false,
        responseCanMaintainUserViews: false,
        responseMruTemplate: '{ID}',
        responseSelectorTypeDescription: '',
        responseViews: null,
        selectedItem: null,
        store: null,
        toolbar: null,
        toolbarMruCombo: null,
        toolbarSplitButton: null,
        toolbarSplitButtonAddViewButton: null,
        toolbarSplitButtonEditViewButton: null,
        toolbarSplitButtonMakeDefaultViewButton: null,
        toolbarClearButton: null,
        toolbarExportButton: null,
        views: [],
        viewEditor: null
    };
    var filterFormats = { TemplateColumn: 0, StringColumn: 1, DateTimeHoursMinsColumn: 2, NumberColumn: 3, BooleanColumn: 4, CurrencyColumn: 5, DateColumn: 6, DateTimeHoursMinsSecsColumn: 7, TimeHoursMinsColumn: 8, TimeHoursMinsSecsColumn: 9 };
    var alignments = { Default: 0, Left: 1, Right: 2, Center: 3 };
    var viewTypes = { System: 0, User: 1 };

    $(function () {

        Ext.require('Ext.grid.Panel');
        Ext.require('Ext.toolbar.Paging');
        Ext.require('Ext.ux.grid.FiltersFeature');
        Ext.require('Target.ux.grid.filter.AdvancedStringFilter');
        Ext.require('Target.ux.file.Downloader');

        Selectors.InstanceCount++;
        settings.instanceIndex = Selectors.InstanceCount;

    });

    function adviseItemContextMenu(args) {
        adviseEvents(settings.events.onItemContextMenuEvts, args);
    }

    function adviseItemCleared() {
        adviseItemSelected();
    }

    function adviseOnItemDoubleClick(args) {
        adviseEvents(settings.events.onItemDoubleClickEvts, args);
    }

    function adviseOnItemMouseOver(args) {
        adviseEvents(settings.events.onItemMouseOverEvts, args);
    }

    function adviseOnItemMouseOut(args) {
        adviseEvents(settings.events.onItemMouseOutEvts, args);
    }

    function adviseItemRemoved(item) {
        adviseEvents(settings.events.onItemDeletedEvts, item);
    }

    function adviseItemSelected() {
        adviseEvents(settings.events.onItemSelectedEvts, extSettings.selectedItem);
    }

    function adviseItemUpdated(item) {
        adviseEvents(settings.events.onItemUpdatedEvts, item);
    }

    function adviseLoaded() {
        adviseEvents(settings.events.onLoadedEvts);
    }

    function adviseLoading() {
        adviseEvents(settings.events.onLoadingEvts);
    }

    function adviseMruItemSelected() {
        adviseEvents(settings.events.onMruItemSelectedEvts);
    }

    function adviseEvents(evts, args) {
        $.each(evts, function (evtIdx, evtVal) {
            if ($.isFunction(evtVal)) {
                evtVal(args);
            }
        });
    }

    function addParameter(key, value) {
        if (key) {
            var existingParam = getParameter(key);
            if (existingParam) {
                existingParam.Value = value;
                return existingParam;
            } else {
                var newParam = createParameter(key, value);
                settings.request.Parameters.push(newParam);
            }
        }
    }

    function clearParameters() {
        settings.request.Parameters = [];
    }

    function createModel() {
        if (!extSettings.modelName) {
            var fields = [];
            extSettings.modelName = settings.request.Type.toString();
            $.each(extSettings.responseColumns, function (idx, val) {
                fields.push({
                    name: val.Item.Source
                });
            });
            Ext.define(extSettings.modelName, {
                extend: 'Ext.data.Model',
                fields: fields,
                idProperty: 'ID'
            });
        }
    }

    function createParameter(key, value) {
        return {
            Key: key,
            Value: value
        }
    }

    function createStore() {
        if (!extSettings.store) {
            var toolbarItems = [], toolbarSplitButtonMenuItems = [], viewsMenuItems = [];
            extSettings.parentPanel.setTitle(extSettings.response.value.Title);
            $.each(extSettings.responseViews, function (vwIdx, vwVal) {
                var createdMenuItem = createExtViewMenuItem(vwVal, false);
                if (extSettings.response.value.DefaultViewName === vwVal.Name) {
                    createdMenuItem.checked = true;
                }
                if (vwIdx == 1) {
                    viewsMenuItems.push('-');
                }
                viewsMenuItems.push(createdMenuItem);
            });
            toolbarSplitButtonMenuItems.push({
                cls: 'Selectors',
                iconCls: 'imgDataTable',
                text: 'Views',
                menu: {
                    items: viewsMenuItems,
                    listeners: {
                        show: function (cmp) {
                            if (cmp.items.items.length > 1) {
                                var longestWidth = cmp.items.items[1].getWidth() + 5;
                                $.each(cmp.items.items, function (cmpIdx, cmpVal) {
                                    if (cmpVal.getWidth() > longestWidth) {
                                        longestWidth = cmpVal.getWidth();
                                    }
                                });
                                if (longestWidth > extSettings.maxViewItemWidth) {
                                    longestWidth = extSettings.maxViewItemWidth;
                                }
                                cmp.setWidth(longestWidth);
                            }
                            cmp.alignTo(cmp.parentItem, 'tr-tl');
                        }
                    },
                    maxWidth: extSettings.maxViewItemWidth
                }
            });
            if (extSettings.responseCanMaintainUserViews) {
                extSettings.toolbarSplitButtonAddViewButton = Ext.create('Ext.menu.Item', {
                    handler: function () {
                        var viewForCrud = $.extend(true, {}, getCurrentViewForCrud());
                        extSettings.toolbar.disable();
                        viewForCrud.ID = 0;
                        viewForCrud.IsDefault = false;
                        viewForCrud.Name = '';
                        viewForCrud.Type = viewTypes.User;
                        createViewEditor();
                        extSettings.viewEditor.Show({
                            Item: viewForCrud,
                            Type: settings.request.Type
                        });
                    },
                    cls: 'Selectors',
                    iconCls: 'imgDataTableCreate',
                    tooltip: 'Create new view based on selected view?',
                    text: 'Add View'
                });
                extSettings.toolbarSplitButtonEditViewButton = Ext.create('Ext.menu.Item', {
                    handler: function () {
                        var viewForCrud = getCurrentViewForCrud();
                        extSettings.toolbar.disable();
                        createViewEditor();
                        editViewCallBack(null);
                    },
                    cls: 'Selectors',
                    iconCls: 'imgDataTableEdit',
                    tooltip: 'Edit currently selected view?',
                    text: 'Edit View'
                });
                toolbarSplitButtonMenuItems.push('-');
                toolbarSplitButtonMenuItems.push(extSettings.toolbarSplitButtonAddViewButton);
                toolbarSplitButtonMenuItems.push(extSettings.toolbarSplitButtonEditViewButton);
            };
            extSettings.toolbarSplitButtonMakeDefaultViewButton = Ext.create('Ext.menu.Item', {
                handler: function () {
                    var viewForCrud = getCurrentViewForCrud();
                    if (!viewForCrud.IsDefault) {
                        if (viewForCrud.ID > 0) {
                            settings.service.MakeSelectorViewDefault(viewForCrud.ID, makeDefaultViewCallBack);
                        } else {
                            settings.service.ClearDefaultSelectorViewForUser(settings.request.Type, makeDefaultViewCallBack);
                        }
                    }
                },
                cls: 'Selectors',
                iconCls: 'imgDataTableDefault',
                text: 'Default View?'
            });
            toolbarSplitButtonMenuItems.push(extSettings.toolbarSplitButtonMakeDefaultViewButton);
            extSettings.toolbarSplitButton = Ext.create('Ext.button.Split', {
                handler: function (cmp) {
                    cmp.showMenu();
                },
                text: extSettings.response.value.DefaultViewName,
                tooltip: {
                    text: 'Select a view of this data?'
                },
                menuAlign: 'tr-br',
                menu: new Ext.menu.Menu({
                    items: toolbarSplitButtonMenuItems
                }),
                allowOtherMenus: true,
                cls: 'Selectors',
                maxWidth: extSettings.maxViewItemWidth
            });
            extSettings.toolbarClearButton = Ext.create('Ext.Button', {
                disabled: true,
                text: 'Clear Filters',
                tooltip: 'Clear all current column filters?',
                iconCls: 'imgFilterRemove',
                handler: function () {
                    self.ClearFilters();
                }
            });
            extSettings.toolbarExportButton = Ext.create('Ext.button.Button', {
                cls: 'Selectors',
                handler: function () {
                    var request = Ext.applyIf({}, settings.request);
                    request.Page = 1;
                    request.PageSize = 10000;
                    exportSelector(request);
                },
                iconCls: 'imgExport',
                maxWidth: extSettings.maxViewItemWidth,
                text: 'Export',
                tooltip: {
                    text: 'Export up to the first 10000 records?'
                }
            });
            extSettings.downloader = Ext.create('Target.ux.file.Downloader', {
                downloaderTarget: extSettings.toolbarExportButton
            });
            toolbarItems = [
                extSettings.toolbarClearButton,
                extSettings.toolbarExportButton,
                { xtype: 'tbfill' },
                'Views: ',
                '-',
                extSettings.toolbarSplitButton
            ];
            if (!extSettings.toolbarMruCombo && extSettings.responseMruTemplate) {
                extSettings.toolbarMruCombo = Ext.create('Ext.form.ComboBox', {
                    queryMode: 'local',
                    store: new Ext.data.ArrayStore({
                        id: 0,
                        fields: [
                              { name: 'ID', mapping: 'ID' },
                              { name: 'Value', mapping: 'Value' }
                        ],
                        data: getMrus()
                    }),
                    valueField: 'ID',
                    displayField: 'Value',
                    listeners: {
                        select: function (fld, newVal, oldVal) {
                            newVal = parseInt(newVal[0].data.ID);
                            if (newVal > 0) {
                                var item = self.GetItemById(newVal);
                                extSettings.mruSelected = true;
                                if (item) {
                                    self.SetSelectedItem(item.ID, true);
                                } else {
                                    settings.request.SelectedID = newVal;
                                    extSettings.selectedItem = { ID: newVal };
                                    extSettings.store.load();
                                }
                            } else {
                                self.ClearSelectedItem();
                            }
                        }
                    },
                    width: 250
                });
                toolbarItems.splice(0, 0, 'Recent: ', extSettings.toolbarMruCombo, '-');
            };
            extSettings.toolbar = Ext.create('Ext.toolbar.Toolbar', {
                items: toolbarItems,
                width: '100%',
                height: 37
            });
            extSettings.parentPanel.add(extSettings.toolbar);
            extSettings.store = Ext.create('Ext.data.Store', {
                autoLoad: true,
                data: extSettings.response.value,
                model: extSettings.modelName,
                pageSize: settings.request.PageSize,
                proxy: {
                    type: 'memory',
                    reader: {
                        type: 'json',
                        root: 'Items',
                        totalProperty: 'TotalRecords'
                    }
                }
            });
            extSettings.store.on('beforeload', function (store, records, successful, operation) {
                adviseLoading();
                if (extSettings.currentView) {
                    extSettings.currentView.View.hide();
                }
                showLoadingMask(true);
            }, extSettings.store);
            extSettings.store.on('load', function (store, records, successful, operation) {
                extSettings.response = settings.service.GetSelectorResponse(settings.request, loadCallBack);
            }, extSettings.store);
            extSettings.store.on('update', function (store, record, operation, opts) {
                adviseItemUpdated(record.data);
            }, extSettings.store);
            extSettings.store.on('remove', function (store, record, operation, opts) {
                adviseItemRemoved(record.data);
            }, extSettings.store);
        }
    }

    function createViewEditor() {
        extSettings.viewEditor = new Selectors.SelectorViewEditorWin({
            applicationId: extSettings.responseApplicationID,
            events: {
                onHide: function (args) {
                    if (args.Reason == 1) {
                        handleViewSaved(args.Item);
                    } else if (args.Reason == 2) {
                        handleViewDeleted(args.Item);
                    }
                    extSettings.toolbar.enable();
                }
            }
        });
    }

    function createView(id) {
        var view = getView(id);
        if (!view) {
            var viewColumns = [], notViewColumns = [], allViewColumns, dbView = null, viewColumnRenderer;
            var viewFilters = {
                ftype: 'filters',
                autoReload: true,
                encode: true,
                local: false,
                filters: []
            };
            if (settings.showFilters) {
                $.each(extSettings.responseColumns, function (colIdx, colVal) {
                    if ((colVal.Header.FilterParameterName)) {
                        var viewFilter = {
                            dataIndex: colVal.Item.Source,
                            disabled: ((colVal.Header.FilterParameterName) ? false : true)
                        };
                        if (colVal.Header.FilterItems.length == 0) {
                            viewFilter.type = 'advancedString';
                            viewFilter.emptyText = 'Enter Filter Criteria...';
                            viewFilter.updateBuffer = 0;
                        } else {
                            var viewFilterItems = [];
                            $.each(colVal.Header.FilterItems, function (fiIdx, fiVal) {
                                viewFilterItems.push({
                                    id: fiVal.Value,
                                    text: fiVal.Name
                                });
                            });
                            viewFilter.type = 'list';
                            viewFilter.options = viewFilterItems;
                            viewFilter.single = colVal.Header.FilterItemsSingleOnly;
                        }
                        viewFilters.filters.push(viewFilter);
                    }
                });
            }
            extSettings.toolbarClearButton.setVisible(settings.showFilters);
            $.each(extSettings.responseViews, function (vwIdx, vwVal) {
                if (vwVal.ID === id) {
                    dbView = vwVal;
                    $.each(vwVal.Columns, function (vwColIdx, vwColVal) {
                        var viewColumn = {
                            dataIndex: vwColVal.Source,
                            flex: vwColVal.Flex,
                            width: vwColVal.Width,
                            hidden: vwColVal.Hidden
                        };
                        $.each(extSettings.responseColumns, function (colIdx, colVal) {
                            if (colVal.Item.Source === vwColVal.Source) {
                                viewColumn.text = colVal.Header.Text;

                                switch (colVal.Header.Alignment) {
                                    case alignments.Default:
                                        break;
                                    case alignments.Left:
                                        viewColumn.align = "left";
                                        break;
                                    case alignments.Right:
                                        viewColumn.align = "right";
                                        break;
                                    case alignments.Center:
                                        viewColumn.align = "center";
                                        break;
                                }

                                return false;
                            }
                        });
                        viewColumns.push(viewColumn);
                    });
                    return false;
                }
                if (dbView) {
                    return false;
                }
            });
            $.each(extSettings.responseColumns, function (colIdx, colVal) {
                var found = false;
                $.each(viewColumns, function (vwColIdx, vwColVal) {
                    if (colVal.Item.Source === vwColVal.dataIndex) {
                        found = true;
                        return false;
                    }
                });
                if (!found) {
                    notViewColumns.push({
                        text: colVal.Header.Text,
                        dataIndex: colVal.Item.Source,
                        hidden: true,
                        flex: 1
                    });
                }
            });
            allViewColumns = viewColumns.concat(notViewColumns);
            $.each(allViewColumns, function (vwColIdx, vwColVal) {
                vwColVal.sortable = false;
                viewColumnRenderer = self['Renderer' + vwColVal.dataIndex];
                if (Ext.isFunction(viewColumnRenderer)) {
                    vwColVal.renderer = viewColumnRenderer;
                }
                vwColVal.tdCls = extSettings.responseSelectorTypeDescription + '-' + vwColVal.dataIndex;
                $.each(extSettings.responseColumns, function (colIdx, colVal) {
                    if (colVal.Item.Source === vwColVal.dataIndex) {
                        if (settings.showFilters && colVal.Header.FilterParameterName) {
                            vwColVal.cls = 'Filterable';
                            vwColVal.listeners = {
                                headerclick: function (ct, col, e) {
                                    var exeFunc = function () {
                                        var ctMenu = ct.getMenu(), filterMenuItem;
                                        ct.showMenuBy(col.el.dom, col);
                                        filterMenuItem = ctMenu.items.items[2];
                                        ctMenu.setActiveItem(filterMenuItem);
                                        filterMenuItem.expandMenu();
                                    };
                                    if (Ext.ieVersion == 0 || Ext.ieVersion > 8) {
                                        exeFunc();
                                    } else {
                                        setTimeout(exeFunc, 0);
                                    }
                                }
                            };
                        }
                        switch (colVal.Item.Format) {
                            case filterFormats.TemplateColumn:
                                vwColVal.xtype = 'templatecolumn';
                                vwColVal.tpl = colVal.Item.FormatString;
                                break;
                            case filterFormats.DateColumn:
                            case filterFormats.DateTimeHoursMinsColumn:
                            case filterFormats.DateTimeHoursMinsSecsColumn:
                            case filterFormats.TimeHoursMinsColumn:
                            case filterFormats.TimeHoursMinsSecsColumn:
                                vwColVal.xtype = 'datecolumn';
                                vwColVal.format = colVal.Item.FormatString;
                                break;
                            case filterFormats.NumberColumn:
                            case filterFormats.CurrencyColumn:
                                vwColVal.xtype = 'numbercolumn';
                                vwColVal.format = colVal.Item.FormatString;
                                vwColVal.align = "right";
                                break;
                            case filterFormats.BooleanColumn:
                                vwColVal.xtype = 'booleancolumn';
                                vwColVal.trueText = 'Yes';
                                vwColVal.falseText = 'No';
                                break;
                        }

                        switch (colVal.Item.Alignment) {
                            case alignments.Default:
                                break;
                            case alignments.Left:
                                vwColVal.align = "left";
                                break;
                            case alignments.Right:
                                vwColVal.align = "right";
                                break;
                            case alignments.Center:
                                vwColVal.align = "center";
                                break;
                        };
                        return false;
                    }
                });
            });
            var pagingBar = Ext.create('Ext.toolbar.Paging', {
                xtype: 'pagingtoolbar',
                store: extSettings.store,
                dock: 'bottom',
                displayInfo: true,
                items: [
                    '-',
                    'Page Size: ',
                    {
                        id: getPagingComboName(id),
                        name: getPagingComboName(id),
                        xtype: 'combo',
                        width: 70,
                        value: 0,
                        displayField: 'text',
                        queryMode: 'local',
                        triggerAction: 'all',
                        valueField: 'value',
                        editable: false,
                        store: new Ext.data.SimpleStore({
                            fields: ['text', 'value'],
                            data: [
                                ['Auto', 0], ['10', 10], ['20', 20], ['30', 30], ['40', 40], ['50', 50],
                                ['60', 60], ['70', 70], ['80', 80], ['90', 90], ['100', 100]
                            ]
                        }),
                        submitValue: true,
                        listeners: {
                            select: {
                                fn: function (item, record, index) {
                                    settings.request.Page = 1;
                                    extSettings.store.currentPage = 1;
                                    self.SetPageSize(this.getValue());
                                    getPagingToolbar().doRefresh();
                                }
                            }
                        }
                    }
                ],
                listeners: {
                    beforechange: {
                        fn: function (cmp, page) {
                            settings.request.Page = page;
                        }
                    }
                }
            });
            var uviewFilters = Ext.create('Ext.ux.grid.FiltersFeature', viewFilters);
            var uview = Ext.create('Ext.grid.Panel', {
                store: extSettings.store,
                cls: 'Selectors',
                columns: allViewColumns,
                dockedItems: [
                    pagingBar
                ],
                features: [uviewFilters],
                loadMask: true,
                flex: 10,
                viewConfig: {
                    stripeRows: true
                },
                layout: {
                    type: 'fit'
                },
                padding: '2 0 0 0',
                stateful: false,
                columnLines: true,
                sortableColumns: false
            });
            uview.on('select', function (mdl, rcd, idx, opts) {
                extSettings.selectedItem = getMergedSelectedItem(rcd.data);
                ensureItemSelected(true);
            });
            uview.on('filterupdate', function (filters, filter) {
                var disableFilter = true;
                $.each(extSettings.responseColumns, function (idx, val) {
                    if (filter.dataIndex === val.Item.Source) {
                        self.AddParameter(val.Header.FilterParameterName, ((filter.active) ? filter.getValue() : null));
                        return false;
                    }
                });
                $.each(filters.filters.items, function (idx, val) {
                    if (val.active) {
                        disableFilter = false;
                        return false;
                    }
                });
                extSettings.toolbarClearButton.setDisabled(disableFilter);
                settings.request.Page = 1;
            });
            uview.on('itemmouseenter', function (vw, rcd, itm, idx, e, opt) {
                adviseOnItemMouseOver({
                    Item: rcd.data,
                    Row: itm,
                    Index: idx
                });
            });
            uview.on('itemmouseleave', function (vw, rcd, itm, idx, e, opt) {
                adviseOnItemMouseOut({
                    Item: rcd.data,
                    Row: itm,
                    Index: idx
                });
            });
            uview.on('itemcontextmenu', function (vw, rcd, itm, idx, e, opt) {
                e.stopEvent();
                self.SetSelectedItem(rcd.data.ID, true);
                adviseItemContextMenu({
                    item: rcd.data,
                    Item: rcd.data,
                    itemObject: e
                });
            });
            uview.on('itemdblclick', function (vw, rcd, itm, idx, e, opt) {
                adviseOnItemDoubleClick({
                    Item: rcd.data,
                    Row: itm,
                    Index: idx
                });
            });
            extSettings.parentPanel.add(uview);
            view = {
                ID: dbView.ID,
                DbView: dbView,
                View: uview
            };
            extSettings.views.push(view);
        }
        return view;
    }

    function createViewMenuItem(view, findMenuItem) {
        var viewMenuItem = ((findMenuItem) ? getViewMenuItem(view.ID) : null);
        var viewMenu = extSettings.toolbarSplitButton.menu.items.items[0].menu;
        if (!viewMenuItem) {
            viewMenuItem = createExtViewMenuItem(view);
            if (view.Type == viewTypes.User) {
                var viewMenuItemIdx = 2;
                while (viewMenu.items.items[viewMenuItemIdx] && viewMenuItem.text > viewMenu.items.items[viewMenuItemIdx].text) {
                    viewMenuItemIdx++;
                }
                viewMenuItem = viewMenu.insert(viewMenuItemIdx, viewMenuItem);
            } else {
                if (!extSettings.isSystemViewPresent) {
                    viewMenuItem = viewMenu.insert(0, viewMenuItem);
                    viewMenu.insert(1, '-');
                    extSettings.isSystemViewPresent = true;
                }
            }
        }
        return viewMenuItem;
    }

    function createExtViewMenuItem(view) {
        var viewMenuItem = {
            checked: false,
            group: 'views',
            handler: function (src, evt) {
                setView(parseInt(src.id.replace(getViewNameStart(), '')));
            },
            id: getViewName(view.ID),
            itemId: getViewName(view.ID),
            maxWidth: extSettings.maxViewItemWidth,
            text: view.Name
        };
        return viewMenuItem;
    }

    function deleteParameter(key) {
        if (key) {
            var idx = 0;
            $.each(settings.request.Parameters, function (paramIdx, paramValue) {
                if (paramValue.Key == key) {
                    idx = paramIdx;
                    return;
                }
            });
            if (idx > 0) {
                settings.request.Parameters.splice(idx, 1);
            }
        }
    }

    function editViewCallBack(response) {
        var viewForCrud = getCurrentViewForCrud();
        extSettings.viewEditor.Show({
            Item: viewForCrud,
            Type: settings.request.Type
        });
        showLoadingMask(false);
    }

    function ensureItemSelected(raiseSelected) {
        if (extSettings.selectedItem) { self.SetSelectedItem(extSettings.selectedItem.ID, raiseSelected); }
    }

    function exportSelector(request) {
        var columns = [];
        Ext.Array.each(getCurrentViewForCrud().Columns, function (obj) {
            if (!obj.Hidden) {
                columns.push(obj.Source);
            }
        });
        extSettings.downloader.download({
            url: SITE_VIRTUAL_ROOT + 'Selectors/SelectorExportHandler.axd',
            data: [{
                key: 'SelectorRequest',
                value: escape(Ext.JSON.encode(request))
            }, {
                key: 'SelectorColumns',
                value: escape(Ext.JSON.encode(columns))
            }]
        });
    }

    function extLoadCallbackWs(response) {
        if (!CheckAjaxResponse(response, settings.service.url, true)) {
            return false;
        }
        extLoadCallback(response.value, false);
    }

    function extLoadCallback(response, initOnly) {
        extSettings.response = { value: response };
        extSettings.responseAdditionalItems = extSettings.response.value.AdditionalItems;
        extSettings.responseApplicationID = extSettings.response.value.ApplicationID;
        extSettings.responseColumns = extSettings.response.value.Columns;
        extSettings.responseViews = extSettings.response.value.Views;
        extSettings.responseCanMaintainDefaultView = extSettings.response.value.CanMaintainDefaultView;
        extSettings.responseCanMaintainUserViews = extSettings.response.value.CanMaintainUserViews;
        extSettings.responseMruTemplate = extSettings.response.value.MruTemplate;
        extSettings.responseSelectorTypeDescription = extSettings.response.value.SelectorTypeDescription;
        settings.request.IsMetaDataRequired = false;
        createModel();
        createStore();
        setView(extSettings.response.value.DefaultViewID, true);
        if (!initOnly) {
            extSettings.store.loadPage(settings.request.Page);
        } else {
            adviseLoaded();
            extSettings.toolbarExportButton.setDisabled(true);
        }
    }

    function getAdditionalItem(key) {
        if (key) {
            var add = null;
            if (extSettings.responseAdditionalItems) {
                $.each(extSettings.responseAdditionalItems, function (addIdx, addValue) {
                    if (addValue.Key == key) {
                        add = addValue;
                        return;
                    }
                });
            }
            return add;
        }
    }

    function getCurrentViewForCrud() {
        var states = [], vw = extSettings.currentView.DbView;
        if (extSettings.currentView) {
            $.each(extSettings.currentView.View.columns, function (idx, val) {
                var state = val.getColumnState();
                if (val.flex) {
                    state.flex = val.flex;
                    state.width = null;
                } else {
                    state.flex = null;
                    state.width = val.width;
                }
                if (val.hidden) {
                    state.hidden = val.hidden;
                } else {
                    state.hidden = false;
                }
                state.dataIndex = val.dataIndex;
                if (!state.hidden) {
                    states.push({
                        Flex: state.flex,
                        Hidden: state.hidden,
                        Source: state.dataIndex,
                        Width: state.width
                    });
                }
            });
        }
        vw.Columns = states;
        return vw;
    }

    function getMergedSelectedItem(item) {
        var itm = null;
        $.each(extSettings.response.value.Items, function (itmIdx, itmVal) {
            if (itmVal.ID === item.ID) {
                itm = Ext.Object.merge(itmVal, item);
                return false;
            }
        });
        if (!itm) {
            itm = item;
        }
        return itm;
    }

    function getModelItemById(id) {
        var item = null;
        if (extSettings.store) {
            item = extSettings.store.getById(id);
        }
        return item;
    }

    function getMrusKey() {
        // Ensure that the key is unique to the authenticated user
        return 'SelectorMru' + settings.request.Type.toString() + "_" + extSettings.response.value.WebSecurityUserID;
    }

    function getMrus() {
        var mrus = [];
        try {
            var mrus = Ext.JSON.decode(Ext.util.Cookies.get(getMrusKey()));
            if (!mrus) {
                mrus = [];
            }
        }
        catch (err) {

        }
        return mrus;
    }

    function getViewMenuItem(id) {
        var viewMenuItems = extSettings.toolbar.query('#' + getViewName(id));
        if (viewMenuItems.length > 0) {
            return viewMenuItems[0];
        } else {
            return null;
        }
    }

    function getViewNameStart() {
        return 'view_' + settings.instanceIndex.toString() + '_';
    }

    function getPagingComboName(id) {
        return 'pagingCombo_' + settings.instanceIndex.toString() + '_' + id.toString();
    }

    function getViewName(id) {
        return getViewNameStart() + id.toString();
    }

    function getParameter(key) {
        if (key) {
            var param = null;
            $.each(settings.request.Parameters, function (paramIdx, paramValue) {
                if (paramValue.Key == key) {
                    param = paramValue;
                    return;
                }
            });
            return param;
        }
    }

    function getPagingToolbar() {
        return extSettings.currentView.View.dockedItems.items[1];
    }

    function getResponseView(id) {
        var view;
        if (extSettings.responseViews) {
            $.each(extSettings.responseViews, function (vwIdx, vwVal) {
                if (vwVal.ID == id) {
                    view = vwVal;
                    return false;
                }
            });
        }
        return view;
    }

    function getView(id) {
        var view;
        if (extSettings.views) {
            $.each(extSettings.views, function (vwIdx, vwVal) {
                if (vwVal.ID == id) {
                    view = vwVal;
                    return false;
                }
            });
        }
        return view;
    }

    function handleViewChanged(id) {
        var viewMenuItem = getViewMenuItem(id);
        var viewDataItem = getResponseView(id);
        if (viewDataItem) {
            createViewMenuItem(viewDataItem, true);
            extSettings.toolbarSplitButton.setText(viewDataItem.Name);
            setView(viewDataItem.ID);
        }
        if (viewMenuItem) {
            viewMenuItem.suspendEvents(false);
            viewMenuItem.setChecked(true);
            viewMenuItem.resumeEvents(true);
        }
    }

    function handleViewDeleted(view) {
        var viewMenuItem = getViewMenuItem(view.ID);
        var viewDataItem = getView(view.ID);
        if (viewMenuItem) {
            var viewMenu = extSettings.toolbarSplitButton.menu.items.items[0].menu;
            viewMenu.remove(viewMenuItem, true);
        }
        handleViewChanged(0);
    }

    function handleViewSaved(view) {
        var viewMenuItem = getViewMenuItem(view.ID);
        var viewDataItem = getResponseView(view.ID);
        if (!viewMenuItem) {
            viewMenuItem = createViewMenuItem(view);
            viewMenuItem.setChecked(true);
            extSettings.responseViews.push(view);
            createView(view.ID);
        }
        var tmpView = getView(view.ID);
        tmpView.DbView = view;
        viewMenuItem.setText(view.Name);
        viewMenuItem.setWidth();
        setView(view.ID);
    }

    function loadCallBack(response) {
        if (response) {
            if (!CheckAjaxResponse(response, settings.service.url, true)) {
                return false;
            }
            extSettings.response = response;
            extSettings.responseAdditionalItems = response.value.AdditionalItems;
        }
        if (!extSettings.response.value.Items) {
            extSettings.response.value.Items = [];
        }
        extSettings.store.loadData(extSettings.response.value.Items, false);
        if (extSettings.currentView) {
            extSettings.currentView.View.show();
            showLoadingMask(false);
        }
        ensureItemSelected();
        settings.request.SelectedID = 0;
        updatePagingBar();
        adviseLoaded();
        extSettings.toolbarExportButton.setDisabled((extSettings.store.totalCount == 0));
    }

    function makeDefaultViewCallBack(response) {
        if (!CheckAjaxResponse(response, settings.service.url, true)) {
            return false;
        }
        var viewForCrud = getCurrentViewForCrud();
        viewForCrud.IsDefault = true;
    }

    function setView(id, ignoreSyncToColumnHeadings) {
        var view = createView(id);
        if (view) {
            if (extSettings.currentView) {
                extSettings.currentView.View.hide();
            }
            var extView = view.View;
            extSettings.currentView = view;
            extView.show();
            ensureItemSelected(false);
            extSettings.toolbarSplitButton.setText(view.DbView.Name);
            if (extSettings.toolbarSplitButtonAddViewButton) {
                extSettings.toolbarSplitButtonAddViewButton.setDisabled(!(extSettings.responseCanMaintainUserViews));
            }
            if (extSettings.toolbarSplitButtonEditViewButton) {
                var editBtnDisabled = false;
                switch (view.DbView.Type) {
                    case viewTypes.User:
                        editBtnDisabled = (!extSettings.responseCanMaintainUserViews);
                        break;
                    default:
                        editBtnDisabled = (!extSettings.responseCanMaintainDefaultView);
                        break;
                }
                extSettings.toolbarSplitButtonEditViewButton.setDisabled(editBtnDisabled);
            }
            if (extLoadCallback.isIE) {
                extView.headerCt.items.items[0].show();
            }
            if (!ignoreSyncToColumnHeadings) {
                self.SyncRequestParametersToColumnHeadings();
            }
            updatePagingBar();
        }
    }

    function setMrus() {
        if (extSettings.responseMruTemplate) {
            var item = self.GetSelectedItem();
            if (item && item.ID) {
                var items = getMrus();
                var itemTemplate = new Ext.Template(extSettings.responseMruTemplate);
                var expiryDate = new Date();
                expiryDate.setDate(expiryDate.getDate() + 7);
                if (!items) {
                    items = [];
                }
                $.each(items, function (itmIdx, itmVal) {
                    if (itmVal && itmVal.ID == item.ID) {
                        items.splice(itmIdx, 1);
                        return false;
                    }
                });
                items.unshift({ ID: item.ID, Value: itemTemplate.applyTemplate(item) });
                if (items.length >= 9) {
                    items = items.slice(0, 10);
                }
                Ext.util.Cookies.set(getMrusKey(), Ext.JSON.encode(items), expiryDate, '/');
            }
        }
    }

    function showLoadingMask(mask, maskStr) {
        if (extSettings.parentPanel) {
            if (!settings.showLoading || !mask) {
                extSettings.parentPanel.setLoading(false);
            } else {
                extSettings.parentPanel.setLoading((maskStr || 'Loading...'));
            }
        }
    }

    function updatePagingBar() {
        var pager = getPagingToolbar();
        var pageSizeCombo = pager.getComponent(getPagingComboName(extSettings.currentView.ID));
        var pageSizeComboItem = pageSizeCombo.getStore().findExact('value', settings.request.PageSize);
        if (pageSizeComboItem != -1) {
            pageSizeCombo.setValue(settings.request.PageSize);
        } else {
            pageSizeCombo.setValue(0);
        }
        extSettings.store.currentPage = ((extSettings.response.value.Page > 0) ? extSettings.response.value.Page : 1);
        extSettings.store.pageSize = settings.request.PageSize;
        extSettings.store.totalCount = extSettings.response.value.TotalRecords;
        pager.onLoad();
        getPagingToolbar().setVisible(extSettings.pagingVisible);
    }

    this.AddParameter = function (key, value) {
        addParameter(key, value);
    };

    this.ClearFilters = function () {
        if (extSettings.parentPanel) {
            extSettings.toolbarClearButton.setDisabled(true);
            $.each(extSettings.responseColumns, function (colIdx, colVal) {
                if (colVal.Header.FilterParameterName) {
                    self.AddParameter(colVal.Header.FilterParameterName, null);
                }
            });
            if (extSettings.currentView) {
                extSettings.currentView.View.filters.clearFilters()
            }
        }
    }

    this.ClearSelectedItem = function () {
        if (extSettings.currentView) {
            extSettings.selectedItem = null;
            extSettings.currentView.View.getSelectionModel().deselectAll();
            adviseItemSelected();
        }
    }

    this.ClearParameters = function () {
        clearParameters();
    }

    this.DeleteItem = function (id) {
        var mdlItem = getModelItemById(id);
        if (mdlItem) {
            extSettings.store.remove(mdlItem);
        }
    }

    this.DeleteParameter = function (key) {
        deleteParameter(key);
    };

    this.GetAdditionalItem = function (key) {
        return getAdditionalItem(key);
    }

    this.GetEvents = function () {
        return settings.events;
    }

    this.GetItems = function () {
        return extSettings.response.value.Items;
    }

    this.HasItems = function () {
        if (extSettings.response.value.Items.length > 0)
            return true;
        else
            return false;
    }

    this.GetItemById = function (id) {
        var item = null, mdlItem = getModelItemById(id);
        if (mdlItem && mdlItem.data) {
            item = mdlItem.data;
        }
        return item;
    }

    this.GetParameter = function (key) {
        return getParameter(key);
    }

    this.GetParameterValue = function (key) {
        if (!key) {
            alert('Cannot get parameter for a null or blank key (Selector.GetParameterValue).');
        }
        var param = getParameter(key);
        if (!param) {
            return null;
        } else {
            return param.Value;
        }
    }

    this.GetParameters = function () {
        return settings.request.Parameters;
    }

    this.GetSelectedItem = function () {
        var item = extSettings.selectedItem;
        if (item && item.ID) {
            var modelItem = getModelItemById(item.ID);
            if (modelItem) {
                return getMergedSelectedItem(modelItem.data);
            }
        }
        return null;
    }

    this.GetModelItem = function (id) {
        if (!id) {
            var item = extSettings.selectedItem;
            if (item && item.ID) {
                return getModelItemById(item.ID);
            }
            return null;
        } else {
            return getModelItemById(id);
        }
    }

    this.GetSelectedID = function () {
        var item = extSettings.selectedItem;
        if (item) {
            return item.ID;
        } else {
            return 0;
        }
    }

    this.GetType = function () {
        return settings.request.Type;
    }

    this.GetWebSecurityUserID = function () {
        return extSettings.response.value.WebSecurityUserID;
    }

    this.GetExternalUserID = function () {
        return extSettings.response.value.ExternalUserID;
    }

    this.GetApplicationID = function () {
        return extSettings.response.value.ApplicationID;
    }

    this.GetWebSecurityExternalUserID = function () {
        return extSettings.response.value.WebSecurityExternalUserID;
    }

    this.InsertItem = function (item, idx) {
        extSettings.store.insert(idx, this.CreateModelItem(item));
    }

    this.CreateModelItem = function (data) {
        return Ext.create(extSettings.modelName, data);
    }

    this.Load = function (toCtrl, initOnly) {
        adviseLoading();
        if (!extSettings.parentPanel) {
            var me = this, isFirstLayout = true;
            if (toCtrl && !settings.control) {
                settings.control = toCtrl;
            }
            if (initOnly == undefined) {
                initOnly = false;
            }
            $(window).unload(function () {
                setMrus();
            });
            if (settings.request.SelectedID > 0) {
                extSettings.selectedItem = { ID: settings.request.SelectedID };
            }
            if (!settings.service) {
                settings.service = new Target.Abacus.Library.Selectors.Services.SelectorService_class();
            }
            extSettings.parentPanel = Ext.create('Ext.panel.Panel', {
                cls: 'Selectors',
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                anchor: '100% 100%',
                title: 'Loading...',
                bodyPadding: 1,
                border: 0,
                listeners: {
                    afterlayout: function (cmp) {
                        if (isFirstLayout) {
                            isFirstLayout = false;
                            me.SetPageSize(settings.request.PageSize);
                            if (!initOnly) {
                                showLoadingMask(true);
                            }
                            if (window['selectorType' + settings.request.Type] == undefined) {
                                settings.service.GetSelectorResponse(settings.request, extLoadCallbackWs);
                            } else {
                                extLoadCallback(eval('selectorType' + settings.request.Type), initOnly);
                            }
                        }
                    }
                }
            });
            settings.control.add(extSettings.parentPanel);
        } else {
            settings.request.Page = 1;
            extSettings.store.load();
        }
    }

    this.OnItemContextMenu = function (func) { settings.events.onItemContextMenuEvts.push(func); }

    this.OnItemDeleted = function (func) { settings.events.onItemDeletedEvts.push(func); }

    this.OnItemDoubleClick = function (func) { settings.events.onItemDoubleClickEvts.push(func); }

    this.OnItemUpdated = function (func) { settings.events.onItemUpdatedEvts.push(func); }

    this.OnItemMouseOver = function (func) { settings.events.onItemMouseOverEvts.push(func); }

    this.OnItemMouseOut = function (func) { settings.events.onItemMouseOutEvts.push(func); }

    this.OnItemSelected = function (func) { settings.events.onItemSelectedEvts.push(func); }

    this.OnLoaded = function (func) { settings.events.onLoadedEvts.push(func); }

    this.OnLoading = function (func) { settings.events.onLoadingEvts.push(func); }

    this.OnMruItemSelected = function (func) { settings.events.onMruItemSelectedEvts.push(func); }

    this.SaveMrus = function (refreshUi) {
        setMrus();
        if (refreshUi && extSettings.toolbarMruCombo) {
            extSettings.toolbarMruCombo.getStore().loadData(getMrus());
        }
    }

    this.SetDisabled = function (disabled) {
        if (extSettings.parentPanel) {
            extSettings.parentPanel.setDisabled(disabled);
        }
    }

    this.SetPageSize = function (size) {
        if (size == 0) {
            var availableHeight = (extSettings.parentPanel.getHeight() - 140);
            size = parseInt(availableHeight / 27);
        }
        settings.request.PageSize = size;
    }

    this.SetPagingDisabled = function (disabled) {
        getPagingToolbar().setDisabled(disabled);
    }

    this.SetPagingVisible = function (visible) {
        extSettings.pagingVisible = visible;
        getPagingToolbar().setVisible(visible);
    }

    this.SetRequestOptions = function (options) {
        settings.request = $.extend(true, settings.request, options);
    }

    this.SetRequestSelectedID = function (id) {
        settings.request.SelectedID = id;
    }

    this.SetShowFilters = function (show) {
        settings.showFilters = show;
        extSettings.toolbarClearButton.setVisible(settings.showFilters);
    }

    this.SetShowLoading = function (show) {
        settings.showLoading = show;
    }

    this.SetSelectedItem = function (id, raiseSelected) {
        var selectedItem = getModelItemById(id);
        if (selectedItem) {
            extSettings.currentView.View.getSelectionModel().select(selectedItem);
            extSettings.selectedItem = getMergedSelectedItem(selectedItem.data);
            if (raiseSelected) {
                adviseItemSelected();
                if (extSettings.mruSelected) {
                    extSettings.mruSelected = false;
                    adviseMruItemSelected();
                }
            }
        } else {
            if (id == 0) {
                if (extSettings.currentView) {
                    extSettings.currentView.View.getSelectionModel().deselectAll();
                }
                extSettings.selectedItem = null;
            }
            if (raiseSelected) {
                adviseItemCleared();
            }
        }
    }

    this.SetSelectedItemByIndex = function (index, raiseSelected) {
        var items = extSettings.response.value.Items;
        if (items.length && index <= items.length) {
            this.SetSelectedItem(items[index].ID, raiseSelected);
        }
    }

    this.ShowMask = function (mask, maskStr) {
        showLoadingMask(mask, maskStr);
    }

    this.SyncRequestParametersToColumnHeadings = function () {
        var extView = extSettings.currentView.View, filtersFeature = extView.features[0], filterItems;
        if (filtersFeature.filters.length == 0 && filtersFeature.filterConfigs.length > 0) {
            filtersFeature.addFilters(filtersFeature.filterConfigs);
        }
        filterItems = filtersFeature.filters.items;
        if (filterItems) {
            extView.filters.applyingState = true;
            $.each(filterItems, function (fiIdx, fiVal) {
                if (!fiVal.disabled) {
                    $.each(extSettings.responseColumns, function (rcIdx, rcVal) {
                        if (fiVal.dataIndex === rcVal.Item.Source) {
                            var filterParam = getParameter(rcVal.Header.FilterParameterName);
                            if (filterParam && filterParam.Value && filterParam.Value != '') {
                                fiVal.setValue(filterParam.Value);
                                fiVal.setActive(true);
                            } else {
                                fiVal.setValue(null);
                                fiVal.setActive(false);
                            }
                            return false;
                        }
                    });
                }
            });
            extView.filters.deferredUpdate.cancel();
            delete extView.filters.applyingState;
        }
    }

    this.UpdateItem = function (item) {
        var mdlItem = getModelItemById(item.ID);
        if (mdlItem) {
            mdlItem.suspendEvents(false);
            mdlItem.set(item);
            mdlItem.resumeEvents();
            mdlItem.commit();
        }
    }

}