var tblServiceUsers, tblServiceUsersDaily, tblDayIdStarter = 'tblDay', divDayStarter = 'divDay', divDays, divAll, divPrint, selDays;
var classTotalPlanned, classTotalAttended, classPlanned, classAttended, classAttendedExceeded;
var mode, modeBlank, modeComplete, modeBlankAdditionalLines;
var daysAll, daysAllDayByDay, htmSpace = '&nbsp;', hasService;
var imgCheckBoxUnChecked;

$(document).ready(function() {
    tblServiceUsers = $('#tblServiceUsers');
    tblServiceUsersDaily = $('#tblServiceUsersDaily');
    divDays = $('#divDays');
    divAll = $('#divAll');
    divPrint = $('#divPrint');
    selDays = $('select[id$=\'selDays\']');
    if (hasService) {
        tblServiceUsersDaily.hide();
        divDays.hide();
    } else {
        selDays.attr('disabled', true);
        divPrint.hide();
    }
});

function FilterReportByDay(src) {
    if (!hasService) {
        alert('The current Service Register contains no records to be filtered.');
        return false;
    }
    $('div[id^=\'' + divDayStarter + '\']').hide();
    var selectedDay = $(src).val();
    if (selectedDay === daysAll) {
        divAll.show();
        divDays.hide();
    } else if (selectedDay === daysAllDayByDay) {
        divAll.hide();
        divDays.show();
        LoadAllDayTables(true);
    } else {
        var selectedDayDesc = $(src).find('option:selected').text();
        var selectedDayTbl = GetDayDiv(selectedDay, selectedDayDesc);
        divAll.hide();
        divDays.show();
        selectedDayTbl.show();
    }
}

function LoadAllDayTables(display, days) {
    if (!days || days.length == 0) {
        days = [2, 3, 4, 5, 6, 7, 8];
    }
    $.eachAsync(days, {
        delay: 1,
        bulk: 0,
        loop: function(index, value) {
            ReportLoading(true);
            var selectedDayOpt = selDays.find('option:eq(' + value + ')');            
            var selectedDayTbl = GetDayDiv(selectedDayOpt.val(), selectedDayOpt.text());
            if (display) {
                divDays.show();
                selectedDayTbl.show();
            }
        },
        end: function() {
            ReportLoading(false);
        }
    });
}

function GetPlannedActualColumns(curTotalPlanned, curTotalAttended) {
    if (mode === modeComplete) {
        return '<td class=\'' + classTotalPlanned + '\'>' + curTotalPlanned + '</td><td class=\'' + classTotalAttended + ((curTotalAttended > curTotalPlanned) ? ' ' + classAttendedExceeded : '').toString() + '\'>' + curTotalAttended + '</td><tr/>';
    } else {
        return '</tr>';
    }
}

function GetDayDiv(selectedDay, selectedDayDesc) {
    var selectedDayDiv;
    if (selectedDay != '' && selectedDay != daysAll && selectedDay != daysAllDayByDay) {
        selectedDayDiv = $('#' + divDayStarter + selectedDay);
        if (selectedDayDiv.length > 0) {
            var selectedDayTbl = $('#' + tblDayIdStarter + selectedDay);
            if (selectedDayTbl.length == 0) {
                selectedDayDiv.children(0).text(selectedDayDesc);
                selectedDayTbl = $(tblServiceUsersDaily.clone(true));
                selectedDayTbl.attr('id', tblDayIdStarter + selectedDay);
                var lastSuRef = '', curSuRef = '', curSuName = '', curRateCategoryCell, curRows = '', curRow = '', curCellIsPlanned = false;
                var curTotalPlanned = 0, curTotalAttended = 0;
                var totalPlanned = 0, totalAttended = 0;
                var selectedDayCellIdx = tblServiceUsers.find('thead > tr > th:contains(\'' + selectedDay + '\')').attr('cellIndex');
                var rateCategoryTotals = [], rateCategoryTotalsIdx = 0, curRateCategoryTotal;
                $.each(tblServiceUsers.find('tbody > tr'), function(rowKey, rowValue) {
                    curSuRef = $(rowValue).children('td:eq(0)').html();
                    curSuName = $(rowValue).children('td:eq(1)').html();
                    curRateCategoryCell = $(rowValue).children('td:eq(' + selectedDayCellIdx.toString() + ')');
                    curCellIsPlanned = false;
                    if (curSuRef !== lastSuRef) {
                        if (lastSuRef) {
                            curRow += GetPlannedActualColumns(curTotalPlanned, curTotalAttended);
                            if (mode === modeComplete || curTotalPlanned > 0) {
                                curRows += curRow;
                            }
                        }
                        curRow = '';
                        curRow += '<tr><td>' + curSuRef + '</td><td>' + curSuName + '</td>';
                        lastSuRef = curSuRef;
                        curTotalPlanned = 0;
                        curTotalAttended = (mode == modeComplete) ? 0 : curTotalAttended = htmSpace;
                        rateCategoryTotalsIdx = 0;
                    }
                    curRateCategoryTotal = rateCategoryTotals[rateCategoryTotalsIdx];
                    if (!curRateCategoryTotal) {
                        curRateCategoryTotal = { TotalPlanned: 0, TotalAttended: 0 };
                        rateCategoryTotals[rateCategoryTotalsIdx] = curRateCategoryTotal;
                    }
                    if (curRateCategoryCell.hasClass(classPlanned)) {
                        curCellIsPlanned = true;
                        curTotalPlanned++;
                        curRateCategoryTotal.TotalPlanned = ++curRateCategoryTotal.TotalPlanned;
                        totalPlanned++;
                    }
                    if (curRateCategoryCell.hasClass(classAttended)) {
                        if (mode == modeComplete) {
                            curTotalAttended++;
                            curRateCategoryTotal.TotalAttended = ++curRateCategoryTotal.TotalAttended;
                            totalAttended++;
                        } else {
                            curTotalAttended = htmSpace
                        }
                    }
                    if (mode === modeComplete) {
                        curRow += '<td>' + curRateCategoryCell.html() + '</td>';
                    } else {
                        if (curCellIsPlanned === true) {
                            curRow += '<td>' + curRateCategoryCell.html() + '</td>';
                        } else {
                            curRow += '<td>' + $(curRateCategoryCell.html())[0].outerHTML + '</td>';
                        }
                    }
                    rateCategoryTotalsIdx++;
                });
                curRow += GetPlannedActualColumns(curTotalPlanned, curTotalAttended);
                if (mode === modeComplete || curTotalPlanned > 0) {
                    curRows += curRow;
                }
                selectedDayTbl.find('tbody').html(curRows);
                curRows = '<tr><td class=\'totalPlanned\' colspan=\'2\'>Total Planned:</td>';
                $.each(rateCategoryTotals, function(rcKey, rcValue) {
                    curRows += '<td>' + rcValue.TotalPlanned + '</td>';
                });
                if (mode === modeComplete) {
                    curRows += '<td class=\'' + classTotalPlanned + '\'>' + totalPlanned + '</td><td class=\'' + classTotalAttended + '\'>N/A</td>';
                }
                curRows += '</tr>';
                curRows += '<tr><td class=\'totalActual\' colspan=\'2\'>Total Actual:</td>';
                $.each(rateCategoryTotals, function(rcKey, rcValue) {
                    if (mode !== modeComplete) {
                        rcValue.TotalAttended = htmSpace
                    }
                    curRows += '<td>' + rcValue.TotalAttended + '</td>';
                });
                if (mode !== modeComplete) {
                    totalAttended = htmSpace
                }
                if (mode === modeComplete) {
                    curRows += '<td class=\'' + classTotalPlanned + '\'>N/A</td><td class=\'' + classTotalAttended + '\'>' + totalAttended + '</td>';
                }
                curRows += '</tr>';
                selectedDayTbl.find('tfoot').html(curRows);
                selectedDayTbl.appendTo(selectedDayDiv);
                selectedDayTbl.show();
                curRows = '';
                var remainingWidth = 100, remainingCells = 0, headWidth = 0, numOfCells = selectedDayTbl.find('thead > tr > th').size();
                if (mode === modeBlank) {
                    var blankRow = '<tr>';                    
                    for (i = 0; i < numOfCells; i++) {
                        if (i < 2) {
                            blankRow += '<td>' + htmSpace + '</td>';
                        } else {
                            blankRow += '<td><img src=\'' + imgCheckBoxUnChecked + '\' /></td>';
                        }
                    }
                    blankRow += '</tr>';
                    curRows = '';
                    for (i = 0; i < modeBlankAdditionalLines; i++) {
                        curRows += blankRow;
                    }
                    if (curRows) {
                        selectedDayTbl.find('tbody').html(selectedDayTbl.find('tbody').html() + curRows);
                    }
                    remainingWidth = 70;
                    remainingCells = numOfCells - 2;
                    headWidth = remainingWidth / remainingCells;
                } else {
                    remainingWidth = 60;
                    remainingCells = numOfCells - 4;
                    headWidth = remainingWidth / remainingCells;
                }
                $.each(selectedDayTbl.find('thead > tr > th'), function(headKey, headValue) {
                    if (headKey >= 2 && (mode === modeBlank || (mode === modeComplete && headKey < (numOfCells - 2)))) {
                        $(headValue).width(headWidth.toString() + '%');
                    }
                });
            }
        }   
    }
    return selectedDayDiv;
}

function ReportLoading(isLoading) {
    DisplayLoading(isLoading);
    if (isLoading) {
        divPrint.hide();        
    } else {
        divPrint.show();        
    }
    selDays.attr('disabled', isLoading);
}






