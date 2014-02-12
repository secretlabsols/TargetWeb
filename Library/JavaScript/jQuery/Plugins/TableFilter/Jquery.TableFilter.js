(function($) {

    var evtFilterTableCellChanged = 'filterTableCellChanged';
    var tableFilterOptionsKey = 'tableFilterOptions';

    $.fn.tableFilter = function(method, options) {

        return this.each(function() {

            var $this = $(this);

            if ($this.is('table')) {
                if (methods[method]) {
                    return methods[method].apply($this, [options]);
                } else if (typeof method === 'object' || !method) {
                    return methods.init.apply($this, [method]);
                } else {
                    alert('Method ' + method + ' does not exist on jQuery.tableFilter');
                }
            } else {
                alert('jQuery.tableFilter can only used on table elements/nodes.');
            }
        });

    };

    Array.prototype.unique = function() {
        var arrVal = this;
        var uniqueArr = [];
        for (var i = arrVal.length; i--; ) {
            var val = arrVal[i];
            if ($.inArray(val, uniqueArr) === -1) {
                uniqueArr.unshift(val);
            }
        }
        return uniqueArr;
    }

    var controls = {
        textbox: function(tbl, tblCfg, cellCfg, tblHeaderIdx) {
            var $tblFilterCtrl = $('<input type=\'text\' />').css('width', '96%').css('margin', '0').css('padding', '0');
            $tblFilterCtrl.bindWithDelay('keyup paste', function() {
                $(this).trigger(evtFilterTableCellChanged, { filterTable: tbl, filterValue: $(this).val() });
            }, tblCfg.filterDelay);
            cellCfg.isFilterable = true;
            return $tblFilterCtrl;
        },
        dropdown: function(tbl, tblCfg, cellCfg, tblHeaderIdx) {
            var $tblFilterCtrl = $('<select />').css('width', '96%').css('margin', '0').css('padding', '0');
            var $tblFilterCtrlTextArr = tbl.children('tbody:first').children('tr').children('td:nth-child(' + (tblHeaderIdx + 1).toString() + ')').map(function() {
                return $(this).text();
            }).get();
            var tblFilterCtrlTextArrUnique = getDropdownItems(tbl, tblCfg, tblHeaderIdx);
            populateDropDownItems($tblFilterCtrl, tblFilterCtrlTextArrUnique);
            $tblFilterCtrl.bindWithDelay('keyup change', function() {
                $(this).trigger(evtFilterTableCellChanged, { filterTable: tbl, filterValue: $(this).val() });
            }, tblCfg.filterDelay);
            cellCfg.isFilterable = true;
            return $tblFilterCtrl;
        },
        custom: function(tbl, tblCfg, cellCfg, tblHeaderIdx) {
            var $tblFilterCtrl = null;
            $.each(tblCfg.customFilterControls,
                function(idx, control) {
                    if (control.index == tblHeaderIdx) {
                        cellCfg.isFilterable = true;
                        $tblFilterCtrl = control.control;
                    }
                });
            return $tblFilterCtrl;
        },
        none: function() {
            return null;
        }
    }

    function populateDropDownItems(dropDown, items) {
        dropDown = $(dropDown);
        var selectedValue = dropDown.find('option:selected').val();
        selectedValue = ((selectedValue) ? selectedValue.replace('^', '').replace('$', '') : '');
        dropDown[0].options.length = 0;
        dropDown.append($('<option />'));
        $.each(items,
            function(itemIdx, itemText) {
                if (itemText) {
                    var dropDownOption = dropDown.append($('<option />').val('^' + itemText + '$').html(itemText));
                    if (itemText === selectedValue) {
                        dropDownOption.attr('selected', true);
                    }
                }
            }
        );
    }

    function getDropdownItems(tbl, tblCfg, tblHeaderIdx) {
        var $tblFilterOptions = getTableFilterOptions(tbl);
        var $tblFilterCtrlTextArr = tbl.children('tbody:first').children('tr').children('td:nth-child(' + (tblHeaderIdx + 1).toString() + ')').map(function() {
            return $(this).text();
        }).get();
        var tblFilterCtrlTextArrUnique = $.makeArray($tblFilterCtrlTextArr);
        tblFilterCtrlTextArrUnique = tblFilterCtrlTextArrUnique.unique();
        tblFilterCtrlTextArrUnique.sort();
        if ($.isFunction($tblFilterOptions.onPopulatingDropDown)) {
            $tblFilterOptions.onPopulatingDropDown({ table: tbl, tableHeaderIdx: tblHeaderIdx, items: tblFilterCtrlTextArrUnique });
        }
        tblFilterCtrlTextArrUnique = tblFilterCtrlTextArrUnique.unique();
        tblFilterCtrlTextArrUnique.sort();
        return tblFilterCtrlTextArrUnique;
    }

    function getTableFilterOptions(tbl) {
        return tbl.data(tableFilterOptionsKey);
    }

    function filter(src, options, force) {
        var $tbl = $(options.filterTable);
        var $tblFilterCellIdx = (src) ? $(src.currentTarget).closest('td').attr('cellIndex') : 0;
        var $tblFilterOptions = getTableFilterOptions($tbl);
        var $tblFilterCellSrc = (src) ? $tblFilterOptions.filterCells[$tblFilterCellIdx] : { currentValue: '' };
        if ($tblFilterCellSrc.currentValue != options.filterValue || force == true) {
            var $rows = $tbl.children('tbody:first').children('tr');
            var $shouldOnMatchingCell = ($.isFunction($tblFilterOptions.onMatchingCell));
            var $shouldOnMatchingRow = ($.isFunction($tblFilterOptions.onMatchingRow));
            $tblFilterOptions.filterCells[$tblFilterCellIdx].currentValue = options.filterValue;
            if ($.isFunction($tblFilterOptions.onFilterStarted)) {
                $tblFilterOptions.onFilterStarted($tbl, $rows.length);
            }
            $.each($rows,
            function(rowIdx, row) {
                var showRow = false;
                var $cells = null;
                row = $(row);
                $cells = $(row).children('td');
                $.each($cells,
                    function(cellIdx, cell) {
                        var filterCellConfig = $tblFilterOptions.filterCells[cellIdx];
                        var filterCellCurrentValue = filterCellConfig.currentValue;
                        cell = $(cell);
                        if ((filterCellConfig.isFilterable == false)
                                || (!filterCellCurrentValue || filterCellCurrentValue == '')
                                || (cell.text().search(new RegExp(filterCellCurrentValue, "i")) >= 0)) {
                            showRow = true;
                        } else {
                            showRow = false;
                        }
                        if ($shouldOnMatchingCell) {
                            showRow = $tblFilterOptions.onMatchingCell($tbl, row, rowIdx, cell, cellIdx, cell.text(), showRow);
                            if (!showRow || showRow != true) {
                                showRow = false;
                            }
                        }
                        if (showRow == false) {
                            return false;
                        }
                    });
                if ($shouldOnMatchingRow) {
                    showRow = $tblFilterOptions.onMatchingRow($tbl, row, rowIdx, showRow);
                    if (!showRow || showRow != true) {
                        showRow = false;
                    }
                }
                if (showRow == true) {
                    row.show();
                } else {
                    row.hide();
                }
            });
            if ($.isFunction($tblFilterOptions.onFilterCompleted)) {
                $rows = $tbl.children('tbody:first').children('tr:not(:hidden)');
                $tblFilterOptions.onFilterCompleted($tbl, $tblFilterOptions.filterCells, $rows.length);
            }
        }
    }

    var methods = {
        init: function(options) {
            var $tbl = $(this);
            var $tblOptions = getTableFilterOptions($tbl);
            if (!$tblOptions) {
                var $tblHead = $tbl.children('thead:first');
                var $tblFilterRow = $('<tr></tr>');
                var $tblFilterOptions = {
                    filterDelay: 0,
                    filterRow: null,
                    filterCells: [],
                    customFilterControls: [],
                    onMatchingCell: null,
                    onMatchingRow: null,
                    onFilterCompleted: null,
                    onFilterStarted: null,
                    onPopulatingDropDown: null
                };
                if (options) {
                    $.extend($tblFilterOptions, options);
                }
                $tblFilterOptions.filterCells = [];
                setTableFilterOptions($tbl, $tblFilterOptions);
                $.each($tbl.children('thead:first tr:first th'),
                    function(tblHeaderIdx, tblHeader) {
                        var filterType = $(tblHeader).attr('filterTableType');
                        var $tblFilterCell = $('<td></td>');
                        var $tblFilterCtrl = null;
                        var $tblFilterCellConfig = {
                            cell: $tblFilterCell,
                            cellFilterControl: null,
                            cellHeading: $(tblHeader).text(),
                            currentValue: '',
                            isFilterable: false
                        }
                        $tblFilterOptions.filterCells[tblHeaderIdx] = $tblFilterCellConfig;
                        if (filterType && filterType != '') {
                            if (controls[filterType.toLowerCase()]) {
                                $tblFilterCtrl = controls[filterType.toLowerCase()].apply($tbl, [$tbl, $tblFilterOptions, $tblFilterCellConfig, tblHeaderIdx]);
                            } else {
                                $tblFilterCtrl = controls.none.apply($tbl, [$tbl, $tblFilterOptions, $tblFilterCellConfig, tblHeaderIdx]);
                            }
                            if ($tblFilterCtrl) {
                                $tblFilterCtrl.bind(evtFilterTableCellChanged, function(src, options) {
                                    filter(src, options);
                                });
                                $tblFilterCell.append($tblFilterCtrl);
                            }
                        }
                        if (!$tblFilterCtrl) {
                            $tblFilterCell.html('&nbsp;');
                        } else {
                            if ($tblFilterCtrl.is('input:text')) {
                                $tblFilterCtrl.textboxClearer();
                            }
                            $tblFilterCellConfig.cellFilterControl = $tblFilterCtrl;
                        }
                        $tblFilterRow.append($tblFilterCell);
                        $tblFilterOptions.filterRow = $tblFilterRow;
                    }
                );
                $tblHead.append($tblFilterRow);
                setTableFilterOptions($tbl, $tblFilterOptions);
            }
        },
        clear: function() {
            var $tbl = $(this);
            var $tblFilterOptions = getTableFilterOptions($tbl);
            if ($tblFilterOptions) {
                var $tblFilterOptionsCells = $tblFilterOptions.filterCells;
                var $tblFilterCtrl = null;
                $.each($tblFilterOptionsCells,
                    function(cellIdx, cell) {
                        if (cell.isFilterable) {
                            cell.currentValue = '';
                            $tblFilterCtrl = $(cell.cell).children(0);
                            if ($tblFilterCtrl.is('input:text') || $tblFilterCtrl.is('select')) {
                                $tblFilterCtrl.val('');
                            }
                        }
                    }
                );
                if ($tblFilterCtrl) {
                    $tblFilterCtrl.currentTarget = $tblFilterCtrl;
                    filter($tblFilterCtrl, { filterTable: $tbl, filterValue: '' });
                }
            }
        },
        refresh: function() {
            var $tbl = $(this);
            var $tblOptions = getTableFilterOptions($tbl);
            if ($tblOptions.filterCells.length > 0) {
                $tblOptions.filterTable = $tbl;
                filter(null, $tblOptions, true);
            }
        },
        setFilterValue: function(options) {
            var $tbl = $(this);
            var $tblOptions = getTableFilterOptions($tbl);
            if ($tblOptions.filterCells.length > 0) {
                var $setFilterValueOptions = {
                    cellIndex: null,
                    cellValue: ''
                };
                if (options) {
                    $.extend($setFilterValueOptions, options);
                }
                if (!isNaN(parseInt($setFilterValueOptions.cellIndex))) {
                    var $filterCell = $tblOptions.filterCells[$setFilterValueOptions.cellIndex];
                    if ($filterCell && $filterCell.isFilterable) {
                        var $filterCellCtrl = $filterCell.cellFilterControl;
                        if ($filterCellCtrl.is('input:text')) {
                            $filterCellCtrl.val($setFilterValueOptions.cellValue);
                            $filterCellCtrl.trigger(evtFilterTableCellChanged, { filterTable: $tbl, filterValue: $filterCellCtrl.val() });
                        } else if ($filterCellCtrl.is('select')) {
                            $filterCellCtrl.find('option:selected').attr('selected', false);
                            var $filterCellCtrlOption = $filterCellCtrl.find('option:contains(\'' + $setFilterValueOptions.cellValue + '\')');
                            if ($filterCellCtrlOption.length === 0) {
                                $filterCellCtrlOption = $('<option />').val('^' + $setFilterValueOptions.cellValue + '$').html($setFilterValueOptions.cellValue);
                                $filterCellCtrlOption.appendTo($filterCellCtrl);
                            }
                            $filterCellCtrlOption.attr('selected', true);
                            $filterCellCtrl.trigger(evtFilterTableCellChanged, { filterTable: $tbl, filterValue: $filterCellCtrl.val() });
                        }
                    }
                }
            }
        },
        repopulateDropDowns: function() {
            var $tbl = $(this);
            var $tblOptions = getTableFilterOptions($tbl);
            if ($tblOptions.filterCells.length > 0) {
                $.each($tblOptions.filterCells, function(filterIdx, filterVal) {
                    var $filterCell = $tblOptions.filterCells[filterIdx];
                    if ($filterCell.isFilterable) {
                        var $filterCellCtrl = $filterCell.cellFilterControl;
                        if ($filterCellCtrl.is('select')) {
                            populateDropDownItems($filterCellCtrl, getDropdownItems($tbl, $tblOptions, filterIdx));
                        }
                    }
                });
            }
        }
    };

    function setTableFilterOptions(tbl, options) {
        tbl.data(tableFilterOptionsKey, options);
    }

})(jQuery);

