Results = {};

Ext.define('Results.ResultControl', {
    extend: 'Ext.panel.Panel',
    renderTo: 'contentContainer',
    config: {
        actionPanel: null,
        isAutoSearchDone: false,
        isLaid: false,
        lastMapResultsToQs: null,
        lastMapResultsToReports: null,
        lastSearch: null,
        selectorControl: null,
        searchPanel: null,
        searchToSelectorMap: [],
        qsMap: [],
        reportsMapActionPanel: [],
        reportsMapContextMenu: []
    },
    initComponent: function () {
        var me = this, cfg = this.config, footerContainer = $('#footerContainer'), bottom = footerContainer.position().top + footerContainer.outerHeight();
        me.height = ((document.documentElement.clientHeight - bottom) - 1);
        me.items = [];
        me.layout = 'border';
        cfg.selectorPanel = Ext.create('Ext.panel.Panel', {
            region: 'center',
            margins: ((me.resultSettings.ShowSearchPanel) ? '5 5 5 0' : '5 5 5 5'),
            id: 'centrePanel',
            bbar: ((me.resultSettings.ShowActionPanel) ? me.config.actionPanel : null),
            layout: 'anchor'
        });
        if (me.resultSettings.ShowSearchPanel) {
            me.items.push(cfg.searchPanel);
        }
        me.items.push(cfg.selectorPanel);
        me.on('boxready', function (cmp) {
            var cfg = me.config;
            if (!cfg.isLaid) {
                cfg.isLaid = true;
                cfg.selectorControl.OnLoaded(function () {
                    me.resetCache();
                    if (me.resultSettings.ShowSearchPanel) {
                        cfg.searchPanel.blockInput(false);
                    }
                    Ext.getBody().unmask();
                    if (me.resultSettings.ShowActionPanel) {
                        cfg.actionPanel.setDisabled(false);
                    }
                    if (!cfg.lastSearch && me.resultSettings.ShowSearchPanel) {
                        me.showSearchCriteriaInvalid();
                    }
                });
                cfg.selectorControl.OnLoading(function () {
                    if (me.resultSettings.ShowSearchPanel) {
                        cfg.searchPanel.blockInput(true);
                    }
                    Ext.getBody().mask('Loading');
                });
                cfg.selectorControl.OnItemSelected(function () {
                    me.resetCache();
                });
                if (me.resultSettings.ShowActionPanel) {
                    cfg.actionPanel.setDisabled(false);
                }
                cfg.selectorControl.SetRequestSelectedID(me.resultSettings.SelectedID);
                cfg.selectorControl.SetShowLoading(false);
                if (me.resultSettings.ShowSearchPanel) {
                    cfg.searchPanel.search();
                } else {
                    me.handleSearch(null, null);
                }
            }
        }, me);
        if (me.resultSettings.ShowSearchPanel) {
            cfg.searchPanel.config.resultSettings = me.resultSettings;
            cfg.searchPanel.on('onSearch', me.handleSearch, me);
            cfg.searchPanel.on('onSearchChanged', me.handleSearchChanged, me);
            cfg.searchPanel.setSavedSelections();
        }
        if (me.resultSettings.ShowActionPanel) {
            cfg.actionPanel.on('onDeleteButtonIsDeleted', function (src, args) {
                Ext.getBody().unmask();
                if (args.success) {
                    me.onDeleted();
                }
            }, me);
            cfg.actionPanel.on('onDeleteButtonIsDeleting', function () {
                Ext.getBody().mask('Deleting');
            }, me);
        }
        // set maps
        cfg.searchToSelectorMap = me.resultSettings.SearcherToSelectorMap;
        cfg.qsMap = me.resultSettings.QueryStringMap;
        cfg.reportsMapActionPanel = me.resultSettings.ReportsMapForActionPanel;
        cfg.reportsMapContextMenu = me.resultSettings.ReportsMapForContextMenu;
        // call parent init
        me.callParent(arguments);
    },
    createBasicContextMenuItem: function (href, iconCls, text, tooltip, clickFunc) {
        var me = this;
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
    getBackUrl: function () {
        var me = this, qs;
        qs = me.mapResultsToQs();
        return document.location.pathname + '?' + qs;
    },
    getEncodedSearch: function (srch) {
        var tmpSrch = $.extend(true, {}, srch);
        for (key in tmpSrch) {
            var itm = tmpSrch[key];
            if (!Ext.isPrimitive(itm) && !Ext.isDate(itm) && !Ext.isArray(itm)) {
                tmpSrch[key] = null;
            }
        }
        return Ext.encode(tmpSrch);
    },
    getMapItemValue: function (mapItem, searcher, selector, destinationDesc) {
        var me = this, cfg = me.config, mapResult = { value: null, success: true };
        if (mapItem.Type == 1) {   // searcher
            if (searcher[mapItem.Source] === undefined) {
                me.outputErrorMessage('Cannot map Search Property \'' + mapItem.Source + '\' to \'' + destinationDesc + '\' as the Property does not exist.');
                mapResult.success = false;
                mapResult.value = null;
                return mapResult;
            }
            mapResult.value = searcher[mapItem.Source];
        } else {    // selector
            selectorFunc = selector[mapItem.Source];
            if (!selectorFunc) {
                me.outputErrorMessage('Cannot map Selector Function \'' + mapItem.Source + '\' to \'' + destinationDesc + '\' as the Function does not exist.');
                mapResult.success = false;
                mapResult.value = null;
                return mapResult;
            }
            mapResult.value = selectorFunc.call(searcher);
        }
        if (Ext.isString(mapResult.value)) {
            mapResult.value = escape(mapResult.value);
        }
        return mapResult;
    },
    getUrlFormattedString: function (value) {
        var formattedStr = '';
        if (!value && value !== false && value !== 0) {
            formattedStr = '';
        } else {
            formattedStr = value;
            if (Ext.isDate(formattedStr)) {
                formattedStr = Selectors.Helpers.ToDateString(formattedStr);
            }
        }
        return escape(formattedStr.toString());
    },
    handleSearch: function (args, srch) {
        var me = this, cfg = me.config;
        cfg.selectorControl.SetShowLoading(false);
        cfg.selectorControl.ShowMask(false);
        Ext.getBody().mask('Loading');
        if (srch && me.resultSettings.ShowSearchPanel) {
            cfg.lastSearch = me.getEncodedSearch(srch);
            if (me.mapSearchToSelector(srch)) {
                cfg.selectorControl.Load(cfg.selectorPanel, !me.resultSettings.SearcherSettings.AutoSearch);
            }
        } else {
            cfg.lastSearch = null;
            cfg.selectorControl.Load(cfg.selectorPanel, false);
        }
    },
    handleSearchChanged: function (args, srch) {
        var me = this, cfg = me.config;
        cfg.selectorControl.SetShowLoading(true);
        if (srch) {
            cfg.searchPanel.config.searchButton.setDisabled(false);
        } else {
            me.showSearchCriteriaInvalid();
        }
        cfg.selectorControl.SetShowLoading(false);
    },
    mapSearchToSelector: function (args) {
        var me = this, cfg = me.config;
        if (cfg.searchToSelectorMap && args) {
            var map = cfg.searchToSelectorMap, mappedAllItems = true, mapItem, mappedItem, selectorFunc;
            $.each(map, function (mapItemIdx, mapItem) {
                mappedItem = false;
                for (prop in args) {
                    if (mapItem.Source == prop) {
                        selectorFunc = cfg.selectorControl[mapItem.Destination];
                        if (!selectorFunc) {
                            me.outputErrorMessage('Cannot map Searcher Property \'' + prop + '\' to Selector Control as the function \'' + mapItem.Destination + '\' does not exist.');
                            mappedAllItems = false;
                            mappedItem = true;
                            break;
                        }
                        selectorFunc.call(cfg.selectorControl, args[prop]);
                        mappedItem = true;
                        break;
                    }
                }
                if (!mappedItem) {
                    me.outputErrorMessage('Cannot map Searcher Property \'' + mapItem.Source + '\' to Selector Control as the Search Property does not exist.');
                    mappedAllItems = false;
                }
            });
            if (!mappedAllItems) {
                var failedMsg = 'Failed to map all items from Searcher Control to Selector Control.';
                me.outputErrorMessage(failedMsg + ' Please refer to the previous messages for further detail.', { search: args, map: map });
                alert(failedMsg + ' Please refer to the console for further information.');
            }
            return mappedAllItems;
        } else {
            return false;
        }
    },
    mapResultsToQs: function () {
        var me = this, cfg = me.config, qs = '';
        if (cfg.lastMapResultsToQs) {
            return cfg.lastMapResultsToQs;
        }
        if (cfg.qsMap) {
            var map = cfg.qsMap, mappedAllItems = true, mapResult, selectorVal;
            var selectedItem = cfg.selectorControl.GetSelectedItem();
            var search = ((me.resultSettings.ShowSearchPanel) ? cfg.searchPanel.getSearch() : {});
            if (selectedItem) {
                qs = 'id=' + selectedItem.ID.toString();
            }
            $.each(map, function (mIdx, mVal) {
                mapResult = me.getMapItemValue(mVal, search, cfg.selectorControl, 'Query String');
                if (mapResult.success) {
                    selectorVal = me.getUrlFormattedString(mapResult.value);
                } else {
                    selectorVal = '';
                    mappedAllItems = false;
                }
                if (qs) {
                    qs += '&';
                }
                qs += mVal.Destination + '=' + selectorVal;
            });
            if (!mappedAllItems) {
                var failedMsg = 'Failed to map all items from the Results Control to the Query String.';
                me.outputErrorMessage(failedMsg + ' Please refer to the previous messages for further detail.');
                alert(failedMsg + ' Please refer to the console for further information.');
            }
            cfg.lastMapResultsToQs = qs;
            return qs;
        } else {
            return '';
        }
    },
    mapResultsToReports: function () {
        var me = this, cfg = me.config, mappedAllItems = true, reportParamMap = [];
        if (cfg.lastMapResultsToReports) {
            return cfg.lastMapResultsToReports;
        }
        reportParamMap = me.mapResultsToReportsParameters(cfg.reportsMapActionPanel);
        cfg.lastMapResultsToReports = reportParamMap;
        return reportParamMap;
    },
    mapResultsToReportsForContextMenu: function (map) {
        var me = this, cfg = me.config;
        map = (map || cfg.reportsMapContextMenu);
        return me.mapResultsToReportsParameters(map);
    },
    mapResultsToReportsParameters: function (map) {
        var me = this, cfg = me.config, mappedAllItems = true, reportParamMap = [];
        if (map) {
            var pmap = map, map, mapResult, selectorVal, reportParamMapItem;
            var search = ((me.resultSettings.ShowSearchPanel) ? cfg.searchPanel.getSearch() : {});
            $.each(pmap, function (pmIdx, pmVal) {
                reportParamMapItem = { ids: [], params: [] };
                reportParamMapItem.ids = pmVal.IDS;
                map = pmVal.Items;
                $.each(map, function (mIdx, mVal) {
                    mapResult = me.getMapItemValue(mVal, search, cfg.selectorControl, 'Report');
                    if (mapResult.success) {
                        selectorVal = me.getUrlFormattedString(mapResult.value);
                        reportParamMapItem.params.push({ key: mVal.Destination, value: selectorVal });
                    } else {
                        mappedAllItems = false;
                    }
                });
                reportParamMap.push(reportParamMapItem);
            });
        }
        if (!mappedAllItems) {
            me.outputErrorMessage('Failed to map all items from Results to Reports, please refer to the previous messages for further detail.');
        }
        return reportParamMap;
    },
    onDeleted: function () {
        var me = this, cfg = me.config;
        if (me.resultSettings.ShowSearchPanel) {
            cfg.searchPanel.setDisabled(true);
        }
        cfg.selectorControl.SetDisabled(true);
    },
    outputErrorMessage: function (msg, args) {
        if (console) {
            if (args) {
                console.error(msg, args);
            } else {
                console.error(msg);
            }
        } else {
            alert(msg);
        }
    },
    resetCache: function () {
        var me = this, cfg = me.config;
        cfg.lastMapResultsToReports = null;
        cfg.lastMapResultsToQs = null;
    },
    showSearchCriteriaInvalid: function () {
        var me = this, cfg = me.config;
        cfg.searchPanel.config.searchButton.setDisabled(true);
    }
});


