var $serviceUserStatusFilterCtrl, serviceUserStatusFilterCtrlVal = null;
var serviceRegisterSvc, serviceRegisterID = null;
var $tblServiceUsers = null, $tblServiceUsersRowId = 'tsu_tr_';
var $btnDelete, $btnBack;
var $classServiceUserRead = 'Read', $classServiceUserSaved = 'Saved', $classServiceUserUnread = 'Unread';
var $lblFilterCriteria = null;
var serviceRegisterInProgress = 1, serviceRegisterSubmitted = 2, serviceRegisterReportBlank = 0, serviceRegisterReportComplete = 1;

function DisplayLoadingWithDisabling(display, disabled) {
    DisplayLoading(display);
    $(':button, :submit, tr').attr('disabled', disabled);
}

function GetUrl() {
    return document.location.toString().split('#')[0];
}

function ShowPrintableRegisterReport(mode) {
    OpenPopup('RegisterReport.aspx?rmode=' + mode.toString() + '&id=' + serviceRegisterID.toString(), 75, 50, 1);
}

function ShowPrintableBlankRegisterReport() {
    ShowPrintableRegisterReport(serviceRegisterReportBlank);
}

function ShowPrintableCompleteRegisterReport() {
    ShowPrintableRegisterReport(serviceRegisterReportComplete);
}

function SubmitRegisterToService(confirmation, status) {
    DisplayLoadingWithDisabling(true, true);
    if (confirm(confirmation) == true) {
        DisplayLoading(true);
        var serviceResponse = serviceRegisterSvc.SubmitServiceRegister(serviceRegisterID, status);
        if (CheckAjaxResponse(serviceResponse, serviceRegisterSvc.url, false)) {
            document.location.href = GetUrl();
            return false;
        }
    }
    DisplayLoadingWithDisabling(false, false);
    return false;
}

function SubmitRegister() {
    return SubmitRegisterToService('Are you sure you want to submit this service register?', serviceRegisterSubmitted);
}

function UnSubmitRegister() {
    return SubmitRegisterToService('This register has already been submitted. Are you sure you want to unsubmit this service register?', serviceRegisterInProgress);
}

function AmendRegister() {
    return SubmitRegisterToService('This register has already been processed. Are you sure you want to amend this service register?', serviceRegisterInProgress);
}

function ShowClearAttendance() {
    var $initSettings = {
        serviceRegisterID: serviceRegisterID,
        serviceRegisterSvc: serviceRegisterSvc,
        serviceRegisterDays: serviceRegisterDays,
        serviceRegisterServiceOutcomes: serviceRegisterServiceOutcomes
    }
    $(document).serviceRegisterClearAttendanceDialog($initSettings);
    $(document).serviceRegisterClearAttendanceDialog('show');
}

function AddServiceUser() {
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusWeb/Apps/InPlaceSelectors/Clients.aspx?ref=&name=&clientID=&mode=&hideDebtorRef=1&hideCreditorRef=1";
    var dialog = OpenDialog(url, 60, 32, window);
}

function AddServiceUserRow(id, name, reference) {
    var $serviceUserRow = GetServiceUserRow(id);
    if ($serviceUserRow.length == 0) {
        var $insertBeforeRow = null;
        $.each($tblServiceUsers.find('tbody > tr'), function(key, cell) {
            var $cellName = $(cell).find('td:nth-child(2)').text();
            if (name < $cellName) {
                $insertBeforeRow = $(cell).closest('tr');
                return false;
            }
        });
        $serviceUserRow = $('<tr>').attr('id', $tblServiceUsersRowId + id).click(function() {
            ShowServiceUser(id, name, reference, false);
        });
        $('<td style=\'width: 25%\'>').html(reference).appendTo($serviceUserRow);
        $('<td style=\'width: 72%\'>').html('<a>' + name + '</a>').appendTo($serviceUserRow);
        $('<td style=\'width: 3%\'>').html('&nbsp;').addClass($classServiceUserUnread).appendTo($serviceUserRow);
        if ($insertBeforeRow) {
            $serviceUserRow.insertBefore($insertBeforeRow);
        } else {
            $serviceUserRow.appendTo($tblServiceUsers);
        }
    }
}

function InPlaceClient_ItemSelected(id, reference, name) {
    var $serviceUserRow = GetServiceUserRow(id);
    var $serviceUserIsNew = false;
    if ($serviceUserRow.length == 0) {
        var serviceResponse = serviceRegisterSvc.PrimeServiceRegister(serviceRegisterID, id);
        var $insertBeforeRow = null;
        if (CheckAjaxResponse(serviceResponse, serviceRegisterSvc.url) == false) {
            return false;
        }
        $serviceUserIsNew = true;
    }
    ShowServiceUser(id, name, reference, $serviceUserIsNew);
}

function BackToServiceRegisterList() {
    if ($btnBack) {
        $btnBack.trigger('click');
    }
}

function DeleteServiceRegister() {
    DisplayLoadingWithDisabling(true, true);
    if (serviceRegisterID && serviceRegisterID > 0) {
        if (confirm('Are you sure you want to delete this register?') == true) {
            serviceRegisterSvc.DeleteServiceRegister(serviceRegisterID, DeleteServiceRegister_Callback);
        } else {
            DisplayLoadingWithDisabling(false, false);
        }
    } else {
        alert('Cannot complete delete operation as no Service Register exists to delete.');
        DisplayLoadingWithDisabling(false, false);
    }    
}

function DeleteServiceRegister_Callback(response) {
    if (CheckAjaxResponse(response, serviceRegisterSvc.url)) {
        BackToServiceRegisterList();
    }
    DisplayLoading(false);
}

function GetServiceUserStatusFilterCtrl() {
    if (!$serviceUserStatusFilterCtrl) {
        var classFilterRemove = 'FilterRemove';
        var menuItems = [{ id: -1, description: 'Clear Filter', cssClass: 'MenuItemClearFilter', tooltip: 'Clear Filtering' },
                        { description: 'Unread', cssClass: 'MenuItemUnread', tooltip: 'Filter by Unread' },
                        { description: 'Read', cssClass: 'MenuItemRead', tooltip: 'Filter by Read' },
                        { description: 'Saved', cssClass: 'MenuItemSaved', tooltip: 'Filter by Saved'}];
        $serviceUserStatusFilterCtrl = $('<span />');
        $serviceUserStatusFilterCtrl.addClass('Filter').attr('title', 'Filter');
        $serviceUserStatusFilterCtrl.searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'BottomRight', showSearchBox: false, width: 100 });
        $serviceUserStatusFilterCtrl.bind('MenuItemClicked', function(src, menuItem) {
            if (menuItem.id === -1) {
                $serviceUserStatusFilterCtrl.removeClass(classFilterRemove);
                serviceUserStatusFilterCtrlVal = '';
            } else {
                $serviceUserStatusFilterCtrl.addClass(classFilterRemove);
                serviceUserStatusFilterCtrlVal = menuItem.description;
            }
            $(this).trigger('filterTableCellChanged', { filterTable: $tblServiceUsers, filterValue: serviceUserStatusFilterCtrlVal });
        });
    }
    return $serviceUserStatusFilterCtrl;
}

function ShowServiceUser(id, name, reference, isNew) {

    var $initSettings = {
        serviceRegisterID: serviceRegisterID,
        serviceRegisterDays: serviceRegisterDays,
        serviceRegisterRateCats: serviceRegisterRateCats,
        serviceRegisterServiceOutcomes: serviceRegisterServiceOutcomes,
        serviceRegisterRateCategoryInclusions: serviceRegisterRateCategoryInclusions,
        serviceRegisterRateCategoryPreclusions: serviceRegisterRateCategoryPreclusions,
        serviceRegisterSvc: serviceRegisterSvc,
        onSaved: function(settings) { ShowServiceUserOnSaved(settings); },
        onClosed: function(settings) { ShowServiceUserOnClosed(settings); },
        serviceRegisterEditable: serviceRegisterEditable
    };

    var $showSettings = {
        serviceUserID: id,
        serviceUserName: name,
        serviceUserReference: reference,
        serviceUserIsNew: isNew
    };

    $(document).serviceRegisterServiceUserDialog($initSettings);
    $(document).serviceRegisterServiceUserDialog('show', $showSettings);

}

function GetServiceUserRow(id) {
    return $tblServiceUsers.find('#' + $tblServiceUsersRowId + id);
}

function GetServiceUserRowStatusCell(id) {
    var $serviceUserRow = GetServiceUserRow(id);
    var $serviceUserStatusCell = $serviceUserRow.children(':last-child');
    return $serviceUserStatusCell;
}

function focusServiceUserRow(id) {
    window.location.href = '#' + $tblServiceUsersRowId + id;
}

function ShowServiceUserOnClosed(settings) {    
    var $serviceUserStatusCell = GetServiceUserRowStatusCell(settings.serviceUserID);
    if ($serviceUserStatusCell.length && $serviceUserStatusCell.hasClass($classServiceUserUnread)) {
        $serviceUserStatusCell.removeClass($classServiceUserUnread).addClass($classServiceUserRead);
    }
    focusServiceUserRow(settings.serviceUserID);
    $tblServiceUsers.tableFilter('refresh');
}

function ShowServiceUserOnSaved(settings) {    
    if (settings.serviceUserIsNew === true && (settings.serviceUserTotalActual > 0 || settings.serviceUserTotalPlanned > 0)) {
        AddServiceUserRow(settings.serviceUserID, settings.serviceUserName, settings.serviceUserReference);
    }
    var $serviceUserStatusCell = GetServiceUserRowStatusCell(settings.serviceUserID);
    if ($serviceUserStatusCell.length) {
        $serviceUserStatusCell.removeClass($classServiceUserUnread).removeClass($classServiceUserRead).addClass($classServiceUserSaved);
    }
    focusServiceUserRow(settings.serviceUserID);
    $tblServiceUsers.tableFilter('refresh');
}

$(document).ready(function() {
    serviceRegisterSvc = new Target.Abacus.Web.Apps.WebSvc.ServiceRegisters_class();
    $tblServiceUsers = $('#tblServiceUsers');
    $lblFilterCriteria = $('#lblFilterCriteria');
    var $tblServiceUsersOptions = {
        filterDelay: 0,
        customFilterControls: [{ index: 2, control: GetServiceUserStatusFilterCtrl()}],
        onMatchingCell: function(tbl, row, rowIdx, cell, cellIdx, cellText, isMatch) {
            if (cellIdx == 2) {
                if (!serviceUserStatusFilterCtrlVal || serviceUserStatusFilterCtrlVal == '') {
                    return true;
                } else {
                    return (cell.hasClass(serviceUserStatusFilterCtrlVal));
                }
            } else {
                return isMatch;
            }
        },
        onFilterCompleted: function(tbl, filterCells, filterCount) {
            var filterStr = '<b>Filtered By:</b> ';
            var filterParamsStr = '';
            var filterCellHeading = '';
            var filterCellValue = '';
            $.each(filterCells, function(cellKey, cellValue) {
                if ($.trim(cellValue.cellHeading).length != 1) {
                    filterCellHeading = cellValue.cellHeading;
                    filterCellValue = cellValue.currentValue;
                } else {
                    filterCellHeading = 'Edit Status';
                    filterCellValue = cellValue.currentValue;
                }
                if ($.trim(filterCellValue) != '') {
                    filterParamsStr += filterCellHeading + ' = <i>' + filterCellValue + '</i>; ';
                }
            });
            if ($.trim(filterParamsStr) != '') {
                filterStr += filterParamsStr + ' <b>(' + filterCount.toString() + ' records)</b>';
                $lblFilterCriteria.show();
            } else {
                filterStr = '';
                $lblFilterCriteria.hide();
            }
            $lblFilterCriteria.html(filterStr);
        }
    }
    $tblServiceUsers.tableFilter($tblServiceUsersOptions);
    $tblServiceUsers.tableScroll({ height: 200, width: null });
    $btnDelete = $('input[id$="stdButtons1_btnDelete"]');
    $btnBack = $('input[id$="stdButtons1_btnBack"]');
    if ($btnDelete && $btnDelete.length > 0) {
        $btnDelete[0].onclick = function() { DeleteServiceRegister(); return false; };
    }
    setTimeout(SetServiceUsersTableHeight, 1);
    $(window).bindWithDelay('resize', function() { SetServiceUsersTableHeight(); }, 125);
});

function SetServiceUsersTableHeight() {
    var $footerContainer = $('#footerContainer');
    var $bottom = $footerContainer.position().top + $footerContainer.outerHeight();
    var $heightOfBody = document.documentElement.clientHeight;
    var $remainingSpace = $heightOfBody - $bottom;
    var $tableScrollWrapper = $('.tablescroll_wrapper');
    $tableScrollWrapper.height($tableScrollWrapper.height() + ($remainingSpace - 2));
}

(function($) {

    var $blankSpace = '&nbsp;';
    var $classAttended = 'Attended', $classNotAttended = 'NotAttended', $classPlanned = 'Planned', $classPlannedExceeded = 'PlannedExceeded', $classNotPlanned = 'NotPlanned', $classPlannedAttended = 'PlannedAttended', $classPlannedNotAttended = 'PlannedNotAttended', $classNotPlannedAttended = 'NotPlannedAttended', $classNotPlannedNotAttended = 'NotPlannedNotAttended', $classRateCategory = 'RateCategory', $classTotal = 'Total';
    var $classMenuItemIsAttended = 'MenuItemIsAttended', $classMenuItemIsNotAttended = 'MenuItemIsNotAttended', $classMenuItemIsPlannedAttended = 'MenuItemIsPlannedAttended', $classMenuItemIsPlannedNotAttended = 'MenuItemIsPlannedNotAttended', $classUiDisabled = 'ui-state-disabled';
    var $clearAllButton = null;
    var $dlgDiv = null, $dlgDivLoading = null;
    var $evtMenuItemClicked = 'MenuItemClicked';
    var $idTotalActual = 'TotalActual', $idTotalPlanned = 'TotalPlanned';
    var $isSaving = false;
    var $keyDay = 'day';
    var $tbl = null, $tblHead = null, $tblHeadRow = null, $tblBody = null;
    var $settings = {
        serviceRegisterID: 0,
        serviceRegisterDays: [],
        serviceRegisterRateCats: [],
        serviceRegisterServiceOutcomes: [],
        serviceRegisterRateCategoryInclusions: [],
        serviceRegisterRateCategoryPreclusions: [],
        serviceRegisterSvc: null,
        serviceCurrentClientID: 0,
        serviceRegisterCurrentServiceUser: null,
        serviceRegisterEditable: true,
        onSaved: null,
        onClosed: null
    };
    var $serviceUserChanges = [];

    $.fn.serviceRegisterServiceUserDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.serviceRegisterServiceUserDialog');
            }
        });
    };

    function adviseNotEditable() {
        alert('Current service user cannot be edited as the service register is not editable.');
        displayLoadingDiv(false);
    }

    function displayLoadingDiv(display, message) {
        $dlgDiv.dialog('displayLoading', { Text: message, Display: display });
        disableButtons(display);
    }

    function disableButtons(disabled) {
        $dlgDiv.dialog('disableAllButtons', { Disabled: disabled });
        revaluateControlVisibility(null);
    }

    function revaluateControlVisibility(src) {
        var disabled = false;
        if ($serviceUserChanges.length == 0 && ($settings.serviceRegisterCurrentServiceUser && $settings.serviceRegisterCurrentServiceUser.serviceUserIsNew == false)) {
            disabled = true;
        }
        if ($isSaving === false) {
            $dlgDiv.dialog('disableButton', { Text: 'Save', Disabled: disabled });
        }
    }

    function serviceOutcomeChanged(src, menuItem) {
        revaluateControlVisibility(src);
        if ($settings.serviceRegisterEditable == false) {
            adviseNotEditable();
            return false;
        }
        var $cellData = $(src).data($keyDay);
        var $cellRow = $(src).closest('tr');
        var $service = $cellData.service;
        var $rateCategory = $cellData.rateCategory;
        var $day = $cellData.day;
        var $isAttended = $(src).hasClass($classAttended);
        var $isPlanned = $(src).hasClass($classPlanned);
        var $changedItem = null;
        if (menuItem.serviceOutcome && menuItem.serviceOutcome.ServiceOutcomeType == true) {
            var isPrecluded = false;
            if ($settings.serviceRegisterRateCategoryPreclusions && $settings.serviceRegisterRateCategoryPreclusions.length > 0) {
                $.each($settings.serviceRegisterRateCategoryPreclusions, function(key, rateCategoryPreclusion) {
                    if (rateCategoryPreclusion.PrecludedDomRateCategoryID == $rateCategory.ID && $isAttended == false) {
                        var precludedRateCategory = $tblBody.find('#' + rateCategoryPreclusion.DomRateCategoryID + '_' + $day.ID + '.' + $classAttended);
                        if (precludedRateCategory.length > 0) {
                            alert("You cannot tick this cell because " + rateCategoryPreclusion.rDescription + " is already ticked for this service user on " + $day.DayOfWeekName + ", and is precluded.");
                            isPrecluded = true;
                            return false;
                        }
                    }
                });
                if (isPrecluded == false) {
                    $.each($settings.serviceRegisterRateCategoryPreclusions, function(key, rateCategoryPreclusion) {
                        if (rateCategoryPreclusion.DomRateCategoryID == $rateCategory.ID && $isAttended == false) {
                            $.each($settings.serviceRegisterRateCategoryPreclusions, function(key, subRateCategoryPreclusion) {
                                var precludedRateCategory = $tblBody.find('#' + rateCategoryPreclusion.PrecludedDomRateCategoryID + '_' + $day.ID + '.' + $classAttended);
                                if (precludedRateCategory.length > 0) {
                                    alert("You cannot tick this cell because " + rateCategoryPreclusion.pDescription + " is already ticked for this service user on " + $day.DayOfWeekName + ", and is precluded.");
                                    isPrecluded = true;
                                    return false;
                                }
                            });
                        }
                        if (isPrecluded) {
                            return false;
                        }
                    });
                }
            }
            if (isPrecluded) {
                return false;
            }
            $(src).html(menuItem.serviceOutcome.ServiceOutcomeTypeCode);
            $(src).removeClass($classNotAttended).removeClass($classNotPlannedNotAttended).removeClass($classPlannedNotAttended).addClass($classAttended);
            if ($isPlanned) {
                $(src).addClass($classPlannedAttended);
            } else {
                $(src).addClass($classNotPlannedAttended);
            }
        } else {
            if (menuItem.serviceOutcome && menuItem.serviceOutcome.ServiceOutcomeType == false && $service && $service.IsPlanned == true) {
                $(src).html(menuItem.serviceOutcome.ServiceOutcomeTypeCode);
            } else {
                $(src).html($blankSpace);
            }
            $(src).removeClass($classAttended).removeClass($classPlannedAttended).addClass($classNotAttended);
            if ($isPlanned) {
                $(src).addClass($classPlannedNotAttended);
            } else {
                $(src).addClass($classNotPlannedNotAttended);
            }
        }
        $isAttended = $(src).hasClass($classAttended);
        $isPlanned = $(src).hasClass($classPlanned);
        $.each($serviceUserChanges, function(key, changedItem) {
            if (changedItem && $day.ID == changedItem.RegisterDayID && $rateCategory.ID == changedItem.DomRateCategoryID) {
                $changedItem = changedItem;
                return false;
            }
        });
        if (!$changedItem) {
            if ($service) {
                $changedItem = $service;
            } else {
                $changedItem = {
                    RegisterDayID: $day.ID,
                    RegisterRowID: 0,
                    RegisterColumnID: 0,
                    ServiceOutcomeID: 0,
                    ServiceOutcomeCode: '',
                    IsPlanned: false,
                    IsAttended: false,
                    DomRateCategoryID: $rateCategory.ID,
                    DomRateFrameworkID: 0
                }
            }
            $serviceUserChanges.push($changedItem);
        }
        $changedItem.IsAttended = $isAttended;
        if (menuItem.serviceOutcome) {
            if ($isAttended === false && $isPlanned === false) {
                //if the service was not planned and not attended then we cant store a service outcome.
                $changedItem.ServiceOutcomeID = 0;
            } else {
                $changedItem.ServiceOutcomeID = menuItem.serviceOutcome.ServiceOutcomeID;
            }
        } else {
            $changedItem.ServiceOutcomeID = 0;
        }
        $(src).attr('title', 'Change the' + (($isPlanned) ? ' planned ' : ' ') + 'attendance for \'' + $rateCategory.Description + '\' on \'' + $day.DayOfWeekName + '\'' + (($changedItem.ServiceOutcomeID > 0) ? ' from \'' + menuItem.serviceOutcome.ServiceOutcomeDescription + '\'' : ''));
        if ($changedItem.ServiceOutcomeID === $cellData.OriginalServiceOutcomeID) {
            $.each($serviceUserChanges, function(key, changedItem) {
                if (changedItem && $changedItem.RegisterDayID == changedItem.RegisterDayID && $changedItem.DomRateCategoryID == changedItem.DomRateCategoryID) {
                    $serviceUserChanges.splice(key, 1);
                }
            });
        }
        setRowTotals($cellRow);
        revaluateControlVisibility(src);
    }

    function setRowTotals(row) {
        var totalActual = $(row).find('.' + $classAttended).size();
        var totalPlanned = $(row).find('.' + $classPlanned).size();
        $(row).children('#' + $idTotalActual).html($(row).find('.' + $classAttended).size());
        $(row).children('#' + $idTotalPlanned).html($(row).find('.' + $classPlanned).size());
        var $totalActualCell = $(row).children('#' + $idTotalActual);
        if (totalActual > totalPlanned) {
            $totalActualCell.addClass($classPlannedExceeded);
            $totalActualCell.attr('title', 'Actual service exceeds that of the plan.');
        } else {
            $totalActualCell.removeClass($classPlannedExceeded);
            $totalActualCell.attr('title', '');
        }
    }

    function getTotalActual() {
        var $total = 0;
        $.each($tblBody.find('#' + $idTotalActual), function(key, cell) {
            $total += parseInt($(cell).html());
        });
        return parseInt($total);
    }

    function getTotalPlanned() {
        var $total = 0;
        $.each($tblBody.find('#' + $idTotalPlanned), function(key, cell) {
            $total += parseInt($(cell).html());
        });
        return parseInt($total);
    }

    function clearAll(src, menuItem) {
        if ($settings.serviceRegisterEditable == false) {
            adviseNotEditable();
            return false;
        }
        $.each($tblBody.find('td.' + $classPlanned + ', ' + 'td.' + $classAttended), function(key, cell) {
            serviceOutcomeChanged($(cell), menuItem);
        });
    }

    function close(src, raiseClosed) {
        var shouldClose = true;
        if ($serviceUserChanges && $serviceUserChanges.length > 0) {
            shouldClose = confirm('Changes made to the current service user will be lost if you close this window, do you wish to continue?') == true
        }
        if (shouldClose == true) {
            if ($.isFunction($settings.onClosed) && raiseClosed == true) {
                $settings.onClosed($settings.serviceRegisterCurrentServiceUser);
            }
            $(src).dialog('close');
        }
    }

    function confirmClose(src) {
        var shouldClose = true;
        if ($serviceUserChanges && $serviceUserChanges.length > 0) {
            shouldClose = confirm('Changes made to the current service user will be lost if you close this window, do you wish to continue?') == true
        }
        if (shouldClose == true) {
            if ($.isFunction($settings.onClosed)) {
                $settings.onClosed($settings.serviceRegisterCurrentServiceUser);
            }
        }
        displayLoadingDiv(false);
        $(document).trigger('click');
        return shouldClose;
    }

    function saveChanges(src) {
        $isSaving = true;
        displayLoadingDiv(true, 'Saving.....');
        saveChangesToService(src);
    }

    function saveChangesToService(src) {
        var $isIncluded = true;
        if ($settings.serviceRegisterEditable == false) {
            adviseNotEditable();
            displayLoadingDiv(false);
            return false;
        }
        var actualServiceUserChanges = [];
        $.each($serviceUserChanges, function(key, change) {
            if (change) {
                actualServiceUserChanges.push(change);
            }
        });
        if (actualServiceUserChanges && actualServiceUserChanges.length > 0) {
            if ($settings.serviceRegisterRateCategoryInclusions && $settings.serviceRegisterRateCategoryInclusions.length > 0) {
                var $inclusions = [];
                var $currentrateInclusion = null;
                $.each($settings.serviceRegisterRateCategoryInclusions, function(key, rateCategoryInclusion) {
                    if ((!$currentrateInclusion) || ($currentrateInclusion && $currentrateInclusion.rateCategoryID != rateCategoryInclusion.DomRateCategoryID)) {
                        $currentrateInclusion = { rateCategoryID: rateCategoryInclusion.DomRateCategoryID, inclusions: [] };
                        $inclusions.push($currentrateInclusion);
                    }
                    $currentrateInclusion.inclusions.push(rateCategoryInclusion);
                });
                $.each($inclusions, function(key, rateCategoryInclusion) {
                    $.each($tblBody.find('td[id^=\'' + rateCategoryInclusion.rateCategoryID + '\'].' + $classAttended), function(key, includedCell) {
                        var $includedCellData = $(includedCell).data($keyDay);
                        $isIncluded = false;
                        $.each($tblBody.find('tr > td:nth-child(' + ($(includedCell).attr('cellIndex') + 1) + ')'), function(key, includeeCell) {
                            var $includeeCellData = $(includeeCell).data($keyDay);
                            if ($(includeeCell).hasClass($classAttended)) {
                                $.each(rateCategoryInclusion.inclusions, function(key, subRateCategoryInclusion) {
                                    if (subRateCategoryInclusion.IncludedDomRateCategoryId == $includeeCellData.rateCategory.ID) {
                                        $isIncluded = true;
                                        return false;
                                    }
                                });
                            }
                        });
                        if ($isIncluded == false) {
                            var inclusionsList = '';
                            $.each(rateCategoryInclusion.inclusions, function(key, subRateCategoryInclusion) {
                                if (inclusionsList && inclusionsList != '') {
                                    inclusionsList += ', ';
                                }
                                inclusionsList += subRateCategoryInclusion.pDescription;
                            });
                            alert('The rate category \'' + rateCategoryInclusion.inclusions[0].rDescription + '\' can not be added to the plan on a \'' + $includedCellData.day.DayOfWeekName + '\', unless one of \'' + inclusionsList + '\' is also included on this day.');
                            return false;
                        }
                    });
                    if ($isIncluded == true) {
                        return false;
                    }
                });
            }
        }
        if ($isIncluded == false) {
            $isSaving = false;
            displayLoadingDiv(false);
            return false;
        }
        $settings.serviceRegisterSvc.UpdateRegisterPlannedOrAttendedService($settings.serviceRegisterID, $settings.serviceCurrentClientID, actualServiceUserChanges, saveChangesToServiceCallback);
    }

    function saveChangesToServiceCallback(serviceResponse) {
        $isSaving = false;
        if (CheckAjaxResponse(serviceResponse, serviceRegisterSvc.url)) {
            $settings.serviceRegisterCurrentServiceUser.serviceUserTotalActual = getTotalActual();
            $settings.serviceRegisterCurrentServiceUser.serviceUserTotalPlanned = getTotalPlanned();
            if ($.isFunction($settings.onSaved)) {
                $settings.onSaved($settings.serviceRegisterCurrentServiceUser);
            }
            $serviceUserChanges = [];
            $dlgDiv.dialog('close');
        }
        revaluateControlVisibility($dlgDiv);
    }

    function showServiceUserCallBack(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, serviceRegisterSvc.url)) {
            $.each($tblBody.children('tr'), function(key, row) {
                $.each($(row).children('td'), function(key, cell) {
                    var $data = $(cell).data($keyDay);
                    if ($data) {
                        var attended = false;
                        var planned = false;
                        var serviceOutcome = $blankSpace;
                        var serviceOutcomeDesc = '';
                        var serviceOutcomes = [];
                        $data.service = null;
                        $data.OriginalServiceOutcomeID = 0;
                        $.each(serviceResponse.value.List, function(key, service) {
                            if ($data.day.ID == service.RegisterDayID && $data.rateCategory.ID == service.DomRateCategoryID) {
                                if (service.IsPlanned) {
                                    planned = true;
                                }
                                if (service.IsAttended) {
                                    attended = true;
                                }
                                $data.service = service;
                                $data.OriginalServiceOutcomeID = service.ServiceOutcomeID;
                                return;
                            }
                        });
                        $(cell).removeClass($classPlanned + ' ' + $classNotPlanned + ' ' + $classAttended + ' ' + $classNotAttended + ' ' + $classPlannedAttended + ' ' + $classPlannedNotAttended + ' ' + $classNotPlannedAttended + ' ' + $classNotPlannedNotAttended);
                        if (planned) {
                            $(cell).addClass($classPlanned);
                        } else {
                            $(cell).addClass($classNotPlanned);
                        }
                        if (attended) {
                            $(cell).addClass($classAttended);
                            if (planned) {
                                $(cell).addClass($classPlannedAttended);
                            } else {
                                $(cell).addClass($classNotPlannedAttended);
                            }
                        } else {
                            $(cell).addClass($classNotAttended);
                            if (planned) {
                                $(cell).addClass($classPlannedNotAttended);
                            } else {
                                $(cell).addClass($classNotPlannedNotAttended);
                            }
                        }
                        if ($data.service && $data.service.ServiceOutcomeCode) {
                            serviceOutcome = $data.service.ServiceOutcomeCode;
                            serviceOutcomeDesc = $data.service.ServiceOutcomeDescription;
                        }
                        $(cell).html(serviceOutcome);
                        $(cell).attr('title', 'Change the' + ((planned) ? ' planned ' : ' ') + 'attendance for \'' + $data.rateCategory.Description + '\' on \'' + $data.day.DayOfWeekName + '\'' + ((serviceOutcomeDesc != '') ? ' from \'' + serviceOutcomeDesc + '\'' : ''));
                        if ($settings.serviceRegisterEditable == true) {
                            $.each($settings.serviceRegisterServiceOutcomes, function(key, outcome) {
                                var dayWeekDate = $data.day.DayOfWeekDate;
                                if (dayWeekDate >= outcome.DateFrom && dayWeekDate <= outcome.DateTo) {
                                    if ((planned == true) || (planned == false && outcome.ServiceOutcomeType == true)) {
                                        var $attendedClass = (planned) ? $classMenuItemIsPlannedAttended : $classMenuItemIsAttended;
                                        var $nonAttendedClass = (planned) ? $classMenuItemIsPlannedNotAttended : $classMenuItemIsNotAttended;
                                        serviceOutcomes.push({ description: outcome.ServiceOutcomeDescription, cssClass: (outcome.ServiceOutcomeType) ? $attendedClass : $nonAttendedClass, serviceOutcome: outcome });
                                    }
                                }
                            });
                            if (planned == false) {
                                serviceOutcomes.push({ description: 'Not Attended', cssClass: $classMenuItemIsNotAttended });
                            }
                            $(cell).searchableMenu(serviceOutcomes, { cssClass: 'Menu', shouldStopPropagation: false });
                            $(cell).unbind($evtMenuItemClicked);
                            $(cell).bind($evtMenuItemClicked, function(src, menuItem) {
                                serviceOutcomeChanged($(this), menuItem);
                            });
                        } else {
                            $(cell).unbind('click');
                            $(cell).click(function() {
                                adviseNotEditable();
                            });
                        }
                    }
                });
                setRowTotals($(row));
            });
        }
        displayLoadingDiv(false);
    }

    var methods = {
        init: function(options) {

            if (!$dlgDiv) {
                if (options) {
                    $.extend($settings, options);
                }
                if (parseInt($settings.serviceRegisterID) <= 0) {
                    alert('Please specify the option \'serviceRegisterID\'');
                    return false;
                }
                if (!$settings.serviceRegisterDays || $settings.serviceRegisterDays.length != 7) {
                    alert('Please specify exactly 7 \'serviceRegisterDays\'');
                    return false;
                }
                if (!$settings.serviceRegisterRateCats || $settings.serviceRegisterRateCats.length <= 0) {
                    alert('Please specify at least 1 \'serviceRegisterRateCats\'');
                    return false;
                }
                $dlgDiv = $('<div>');
                $clearAllButton = $('<span>').html('Clear All?').attr('title', 'Clear all service for this service user?').addClass('RateCategoryImage').appendTo($dlgDiv);
                $tbl = $('<table class=\'listTable ServiceUserDialogTable\' cellspacing=\'0\' cellpadding=\'2\' width=\'100%\'>').appendTo($dlgDiv);
                $tblHead = $('<thead>').appendTo($tbl);
                $tblHeadRow = $('<tr>').appendTo($tblHead);
                $tblBody = $('<tbody>').appendTo($tbl);
                $keyDay = 'day';
                $('<th>').html('Rate Category').addClass($classRateCategory).appendTo($tblHeadRow);
                $.each($settings.serviceRegisterDays, function(key, value) {
                    $('<th>').html(value.DayOfWeekNameShort).appendTo($tblHeadRow);
                });
                $('<th>').html('Planned').appendTo($tblHeadRow);
                $('<th>').html('Actual').appendTo($tblHeadRow);
                $.each($settings.serviceRegisterRateCats, function(key, rateCategory) {
                    var $tblBodyRow = $('<tr>').appendTo($tblBody);
                    $('<td>').html(rateCategory.Description).addClass($classRateCategory).appendTo($tblBodyRow);
                    $.each($settings.serviceRegisterDays, function(key, day) {
                        $('<td>').html($blankSpace).attr('title', 'Change the attendance for \'' + rateCategory.Description + '\' on \'' + day.DayOfWeekName + '\'').attr('id', rateCategory.ID + '_' + day.ID).addClass('Day').data($keyDay, { rateCategory: rateCategory, day: day }).appendTo($tblBodyRow);
                    });
                    $('<td id=\'TotalPlanned\'>').html('0').addClass($classTotal).appendTo($tblBodyRow);
                    $('<td id=\'TotalActual\'>').html('0').addClass($classTotal).appendTo($tblBodyRow);
                });
                if ($settings.serviceRegisterEditable == true) {
                    var $clearServiceOutcomes = [];
                    var $lastDateFrom = null;
                    var $showClearServiceOutcomes = true;
                    $.each($settings.serviceRegisterServiceOutcomes, function(key, outcome) {
                        if (!$lastDateFrom) {
                            $lastDateFrom = outcome.DateFrom;
                        }
                        if ($lastDateFrom != outcome.DateFrom) {
                            $showClearServiceOutcomes = false;
                            return false;
                        }
                        if (outcome.ServiceOutcomeType == false) {
                            $clearServiceOutcomes.push({ description: outcome.ServiceOutcomeDescription, cssClass: $classMenuItemIsNotAttended, serviceOutcome: outcome });
                        }
                        $lastDateFrom = outcome.DateFrom;
                    });
                    if ($clearServiceOutcomes && $clearServiceOutcomes.length > 0) {
                        $clearAllButton.searchableMenu($clearServiceOutcomes, { 'cssClass': 'Menu', shouldStopPropagation: false });
                        $clearAllButton.unbind($evtMenuItemClicked);
                        $clearAllButton.bind($evtMenuItemClicked, function(src, menuItem) {
                            if ($showClearServiceOutcomes == true) {
                                clearAll(src, menuItem);
                            } else {
                                alert('Cannot clear all service as the week spans multiple contract periods.');
                            }
                        });
                    } else {
                        $clearAllButton.click(function() {
                            alert('Cannot clear all service as the week contains no \'Not Attended\' service outcomes.');
                        });
                    }
                } else {
                    $clearAllButton.unbind('click');
                    $clearAllButton.click(function() {
                        adviseNotEditable();
                    });
                }
            }

        },
        show: function(options) {

            if ($dlgDiv) {

                var buttons = null;
                var $showSettings = {
                    serviceUserID: 0,
                    serviceUserName: '',
                    serviceUserReference: '',
                    serviceUserIsNew: false
                };
                if (options) {
                    $.extend($showSettings, options);
                }
                if (parseInt($showSettings.serviceUserID) <= 0) {
                    alert('Please specify the option \'serviceUserID\'');
                    return;
                }

                $settings.serviceCurrentClientID = $showSettings.serviceUserID;
                $serviceUserChanges = [];
                $settings.serviceRegisterCurrentServiceUser = null;

                if ($settings.serviceRegisterEditable == true) {
                    buttons = {
                        "Save": function() {
                            saveChanges($(this));
                        },
                        "Cancel": function() {
                            $(this).dialog('close');
                        }
                    }
                }
                else {
                    buttons = {
                        "Cancel": function() {
                            $(this).dialog('close');
                        }
                    }
                }

                $dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999,
                    width: 1000,
                    minWidth: 800,
                    title: $showSettings.serviceUserReference + ': ' + $showSettings.serviceUserName,
                    buttons: buttons,
                    beforeClose: function(event, ui) {
                        return confirmClose($(this));
                    }
                });
                $dlgDiv.dialog('open');
                displayLoadingDiv(true);
                revaluateControlVisibility($(this));
                $settings.serviceRegisterCurrentServiceUser = $showSettings;
                $settings.serviceRegisterSvc.GetRegisterPlannedOrAttendedService($settings.serviceRegisterID, $showSettings.serviceUserID, 0, null, null, showServiceUserCallBack);

            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }

        }
    };

})(jQuery);

(function($) {

    var $dlgDiv = null;
    var $dlgDivLoading = null;
    var $dropDownDayOfWeek = null;
    var $dropDownServiceOutcome = null;
    var $keyDay = 'day';
    var $settings = {
        serviceRegisterDays: [],
        serviceRegisterServiceOutcomes: [],
        serviceRegisterID: 0,
        serviceRegisterSvc: null
    };
    var $classUiDisabled = 'ui-state-disabled';

    $.fn.serviceRegisterClearAttendanceDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.serviceRegisterClearAttendanceDialog');
            }
        });
    };

    function disableButtons(disabled) {
        $dlgDiv.dialog('disableAllButtons', { Disabled: disabled });
    }

    function displayLoadingDiv(display, message) {
        $dlgDiv.dialog('displayLoading', { Text: message, Display: display });
    }

    function okClicked(src) {
        disableButtons(true);
        displayLoadingDiv(true, 'Saving.....');
        var $selectedDay = $dropDownDayOfWeek.val();
        var $selectedServiceOutcome = $dropDownServiceOutcome.val();
        if ($selectedDay != '' && $selectedServiceOutcome != '') {
            $settings.serviceRegisterSvc.ClearRegisterAttendanceByDay($settings.serviceRegisterID, 0, $selectedDay, $selectedServiceOutcome, clearRegisterAttendanceByDayServiceCallback);
        } else {
            alert('Please select a \'Day of Week\' and \'Service Outcome\'.');
            displayLoadingDiv(false, '');
        }
    }

    function clearRegisterAttendanceByDayServiceCallback(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, serviceRegisterSvc.url)) {
            displayLoadingDiv(false, '');
            $dlgDiv.dialog('close');
            return false;
        } else {
            displayLoadingDiv(false, '');
        }
    }

    function cancelClicked(src) {
        $dlgDiv.dialog('close');
    }

    function optionChanged(src) {
        var okButton = $(":button:contains('OK')");
        if ($dropDownServiceOutcome.find('option').size() == 1 || $dropDownServiceOutcome.attr("selectedIndex") == 0) {
            okButton.attr('disabled', true).addClass($classUiDisabled);
            okButton.attr('title', 'Please select a Service Outcome');
        } else {
            okButton.attr('disabled', false).removeClass($classUiDisabled);
            okButton.attr('title', 'Clear Attendance?');
        }
    }

    var methods = {
        init: function(options) {

            if (!$dlgDiv) {
                if (options) {
                    $.extend($settings, options);
                }
                $dlgDiv = $('<div>');
                $dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999,
                    title: 'Clear Attendance',
                    buttons: {
                        "OK": function() {
                            okClicked($(this));
                        },
                        "Cancel": function() {
                            cancelClicked($(this));
                        }
                    }
                });
                $('<label>').text('Day of Week:').appendTo($dlgDiv);
                $dropDownDayOfWeek = $('<select style=\'width: 100%;\'>').appendTo($dlgDiv).change(function() {
                    var $dataDay = $dropDownDayOfWeek.find('option:nth-child(' + (this.selectedIndex + 1) + ')').data($keyDay);
                    $dropDownServiceOutcome[0].options.length = 0;
                    $('<option>').text('Please select...').val('').appendTo($dropDownServiceOutcome);
                    $.each($settings.serviceRegisterServiceOutcomes, function(soKey, soValue) {
                        if ($dataDay.DayOfWeekDate >= soValue.DateFrom && $dataDay.DayOfWeekDate <= soValue.DateTo) {
                            if (soValue.ServiceOutcomeType == false) {
                                $('<option>').text(soValue.ServiceOutcomeDescription).val(soValue.ServiceOutcomeID).appendTo($dropDownServiceOutcome);
                            }
                        }
                    });
                    optionChanged($(this));
                });
                $.each($settings.serviceRegisterDays, function(dayKey, dayValue) {
                    var $dropDownDayOfWeekItem = $('<option>').text(dayValue.DayOfWeekName).val(dayValue.ID).data($keyDay, dayValue).appendTo($dropDownDayOfWeek);
                    if (dayKey == 0) {
                        $dropDownDayOfWeekItem.attr('selected', true);
                    }
                });
                $('<br>').appendTo($dlgDiv);
                $('<br>').appendTo($dlgDiv);
                $('<label>').text('Service Outcome:').appendTo($dlgDiv);
                $dropDownServiceOutcome = $('<select style=\'width: 100%;\'>').appendTo($dlgDiv).change(function() {
                    optionChanged($(this));
                });
                $dropDownDayOfWeek.trigger('change');
            }

        },
        show: function(options) {
            if ($dlgDiv) {
                displayLoadingDiv(false, '');
                disableButtons(false);
                optionChanged($(this));
                $dlgDiv.dialog('open');
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);


