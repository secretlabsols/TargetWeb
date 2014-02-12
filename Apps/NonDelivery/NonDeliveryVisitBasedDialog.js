(function($) {

    var $dlgDiv = null;
    var $settings = {
        serviceDomProformas: null,
        currentSettings: null,
        paymentSchedule: null,
        proformaInvoice: null,
        onOkd: null,
        onCancelled: null
    };
    var divRecords, tblRecords, tblRecordsTbody;
    var classRosteredDuration = 'rd', classOccurrences = 'oc', classVisitCodes = 'vc', classDurationPaid = 'dp', classValidator = 'val';
    var rowDataKey = 'rowData';
    var recordStatus = { Deleted: 1, Inserted: 2, Updated: 3, NotChanged: 0 };
    var visitCodes = [], atLeastOneChange = false, attrInvalid = 'isInvalid';

    $.fn.nonDeliveryVisitBasedProformaInvoiceLineDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.nonDeliveryVisitBasedProformaInvoiceLineDialog');
            }
        });
    };

    function addRow(item, suppressStatus) {
        if (item) {
            item.OpeningCrudStatus = item.CrudStatus;
        }
        if (item) {
            var currentRow = $('<tr valign=\'top\' />').appendTo(tblRecordsTbody);
            var tdVisitCodes = $('<td />').addClass(classVisitCodes).appendTo(currentRow);
            var tdRosteredDuration = $('<td />').addClass(classRosteredDuration).appendTo(currentRow);
            var tdRosteredDurationValHours = getTimeBasedString(item.RosteredDuration.getHours());
            var tdRosteredDurationValMins = getTimeBasedString(((item.RosteredDuration.getMinutes() < 10) ? '0' + item.RosteredDuration.getMinutes() : item.RosteredDuration.getMinutes()));
            var tdRosteredDurationVal = getRosteredDurationAsString(parseFloat(item.RosteredDuration.getHours() + '.' + ((item.RosteredDuration.getMinutes() < 10) ? '0' + item.RosteredDuration.getMinutes() : item.RosteredDuration.getMinutes())).toFixed(2));
            var tdDurationPaid = $('<td>' + getDurationPaidAsString(parseFloat(item.DurationPaid.getHours() + '.' + ((item.DurationPaid.getMinutes() < 10) ? '0' + item.DurationPaid.getMinutes() : item.DurationPaid.getMinutes())).toFixed(2)) + '</td>').addClass(classDurationPaid).appendTo(currentRow);
            var tdOccurrences = $('<td />').addClass(classOccurrences).appendTo(currentRow);
            if ($settings.currentSettings.isEditable) {
                var ddVisitCodes = $('<select style=\'width: 100%;\' title=\'Select Visit Code\'>').appendTo(tdVisitCodes).change(function(e) {
                    var src = $(e.currentTarget), row = src.closest('tr'), rowData = getRowData(row);
                    rowData.Recalculate = true;
                    recalculateDurationPaid(row);
                });
                $.each(visitCodes, function(vstKey, vstValue) {
                    if (vstValue.DefaultCode !== -1) {
                        var visitCodeOpt = $('<option>').text(vstValue.Description).attr('title', vstValue.Description).val(vstValue.ID).appendTo(ddVisitCodes);
                        if (vstValue.ID == item.DomVisitCodeID) {
                            visitCodeOpt.attr('selected', true);
                        }
                    }
                });
                var ddRosteredDurationHours = $('<select style=\'width: 45%;\' title=\'Select Hours\'>').appendTo(tdRosteredDuration).change(function(e) {
                    var src = $(e.currentTarget), row = src.closest('tr'), rowData = getRowData(row);
                    rowData.Recalculate = true;
                    recalculateDurationPaid(row);
                });
                for (i = 0; i <= 23; i++) {
                    var expandedVal = getTimeBasedString(i);
                    var ddRosteredDurationHoursOption = $('<option>').text(expandedVal).val(expandedVal).appendTo(ddRosteredDurationHours);
                    if (tdRosteredDurationValHours === expandedVal) {
                        ddRosteredDurationHoursOption.attr('selected', true);
                    }
                }
                var ddRosteredDurationMins = $('<select style=\'width: 45%;\' title=\'Select Minutes\'>').appendTo(tdRosteredDuration).change(function(e) {
                    var src = $(e.currentTarget), row = src.closest('tr'), rowData = getRowData(row);
                    rowData.Recalculate = true;
                    recalculateDurationPaid(row);
                });
                for (i = 0; i <= 59; i++) {
                    var expandedVal = getTimeBasedString(i);
                    var ddRosteredDurationMinsOption = $('<option>').text(expandedVal).val(expandedVal).appendTo(ddRosteredDurationMins);
                    if (tdRosteredDurationValMins === expandedVal) {
                        ddRosteredDurationMinsOption.attr('selected', true);
                    }
                }
                $('<div class=\'' + classValidator + '\'>* Required</div>').appendTo(tdRosteredDuration).hide();
                var txtOccurrences = $('<input type=\'text\' value=\'' + (item.Occurrences === 0 ? 1 : item.Occurrences) + '\' title=\'Set Occurrences - Number Greater Than or Equal to 1\' />').css('width', '95%').change(function(e) {
                    var src = $(e.currentTarget), row = src.closest('tr');
                    recalculateDurationPaid(row);
                }).appendTo(tdOccurrences);
                $('<div class=\'' + classValidator + '\'>* Required</div>').appendTo(tdOccurrences).hide();
                var tdDelete = $('<td class=\'deleteInvoiceLine\' title=\'Delete?\'>&nbsp;</td>').click(function(e) {
                    var src = $(e.currentTarget), row = src.closest('tr');
                    deleteRow(row);
                });
                currentRow.append(tdDelete);
                item.Recalculate = true;
                setRowData(currentRow, item);
            } else {
                $.each(visitCodes, function(vstKey, vstValue) {
                    if (vstValue.ID == item.DomVisitCodeID) {
                        tdVisitCodes.text(vstValue.Description);
                        return false;
                    }
                });
                tdRosteredDuration.text(tdRosteredDurationVal);
                tdOccurrences.text(item.Occurrences);
            }
            if (tblRecordsTbody.find('tr').size() == 2) {
                var widthRow = currentRow.clone().height('0px');
                widthRow.prependTo(tblRecordsTbody);
                widthRow.find('td, th').html('');
            }
            if (tblRecords && tblRecords.height() >= 200 && $dlgDiv.find('.tablescroll').length === 0) {
                tblRecords.tableScroll({ height: 200, width: 655 });
            }
            if (item.CrudStatus === recordStatus.Deleted) {
                currentRow.hide();
            }
            return currentRow;
        }
        return null;
    }

    function cancelClicked(src) {
        $dlgDiv.dialog('close');
    }

    function confirmClose(src) {
        var shouldClose = true;
        if (atLeastOneChange) {
            shouldClose = confirm('Changes made to the current proforma invoice will be lost if you close this window, do you wish to continue?') == true
            if (shouldClose) {
                var rowsToSave = getRowsForSave(false);
                if (rowsToSave && rowsToSave.length > 0) {
                    $.each(rowsToSave, function(rowToSaveIdx, rowToSaveVal) {
                        if (rowToSaveVal.OpeningCrudStatus !== undefined) {
                            rowToSaveVal.CrudStatus = rowToSaveVal.OpeningCrudStatus;
                        } else {
                            rowToSaveVal.CrudStatus = rowToSaveVal.CrudStatus;
                        }
                    });
                }
            }
        }
        if (shouldClose == true) {
            if ($.isFunction($settings.onClosed)) {
                $settings.onClosed($settings.currentSettings);
            }
        }
        displayLoadingDiv(false);
        return shouldClose;
    }

    function deleteRow(row) {
        if (confirm('Are you sure you want to delete this record?')) {
            var rowData = getRowData(row);
            row = $(row);
            row.find('[' + attrInvalid + '=\'true\']').attr(attrInvalid, 'false');
            setRowStatus(row, recordStatus.Deleted);
            if (rowData.ID && rowData.ID > 0) {
                row.hide();
            } else {
                row.remove();
            }
        }
    }

    function disableButtons(disabled) {
        $dlgDiv.dialog('disableAllButtons', { Disabled: disabled });
    }

    function displayLoadingDiv(display, message) {
        $dlgDiv.dialog('displayLoading', { Text: message, Display: display });
    }

    function disableOkButton(disabled) {
        $dlgDiv.dialog('disableButton', { Text: 'OK', Disabled: disabled });
    }

    function getDomProformaInvoiceNonDeliveryVisitBasedCallBack(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
            loadTable(serviceResponse.value.List);
        } else {
            $dlgDiv.dialog('close');
        }
    }

    function getRowData(row) {
        return $(row).data(rowDataKey);
    }

    function getTimeBasedString(val) {
        val = parseInt(val);
        if (!isNaN(val)) {
            return ((val < 10) ? '0' + val.toString() : val.toString())
        } else {
            return '00';
        }
    }

    function getRowsForSave(setRowData) {
        var rowsForSave = [], rowsForSaveCnt = 0;
        $.each(tblRecords.find('tr'), function(rowIdx, rowVal) {
            rowVal = $(rowVal);
            var rowData = getRowData(rowVal);
            if (rowData) {
                if (setRowData) {
                    rowData.DomVisitCodeID = getVisitCodeCellValue(rowVal);
                    rowData.RosteredDuration = getRosteredDurationCellValueAsDate(rowVal);
                    rowData.Occurrences = getOccurencesCellValue(rowVal);
                    if (rowData.DurationPaidTemp) {
                        rowData.DurationPaid = getValueAsDate(rowData.DurationPaidTemp);
                    } else if (!rowData.DurationPaid) {
                        rowData.DurationPaid = new Date(1987, 05, 16, 0, 0, 0, 0);
                    }
                    if (rowData.Recalculate === true) {
                        var tdVisitCodeVal = getVisitCodeCellValue(rowVal), selectedVisitCode;
                        $.each(visitCodes, function(vstKey, vstValue) {
                            if (vstValue.ID == tdVisitCodeVal) {
                                selectedVisitCode = vstValue;
                            }
                        });
                        rowData.StandardTimePaid = selectedVisitCode.StandardTimePaid;
                        rowData.MaximumTimePaid = selectedVisitCode.MaximumTimePaid;
                        rowData.ProviderPaid = selectedVisitCode.ProviderPaid;
                        rowData.ClientCharged = selectedVisitCode.ClientCharged;
                    }
                }
                rowsForSave.push(rowData);
                if (rowData.CrudStatus != recordStatus.Deleted) {
                    rowsForSaveCnt += 1;
                }
            }
        });
        $settings.currentSettings.proformaInvoiceDetail.NumberOfNonDeliveryItems = rowsForSaveCnt;
        return rowsForSave;
    }

    function getVisitCodeCell(row) {
        return $(row).find('td.' + classVisitCodes);
    }

    function getVisitCodeCellValue(row) {
        return parseInt(getVisitCodeCell($(row)).find('option:selected').val());
    }

    function getRosteredDurationCell(row) {
        return $(row).find('td.' + classRosteredDuration);
    }

    function getRosteredDurationCellValue(row) {
        var cell = getRosteredDurationCell($(row))
        return parseFloat(cell.find('select:eq(0)').val() + '.' + cell.find('select:eq(1)').val());
    }

    function getRosteredDurationCellValidator(row) {
        var cell = getRosteredDurationCell($(row))
        return $(cell.find('.' + classValidator)[0]);
    }

    function getRosteredDurationCellValueAsDate(row) {
        var value = getRosteredDurationCellValue(row).toString().split('.');
        var hours = parseInt(value[0]);
        var mins = value[1];
        if (mins) {
            if (mins.charAt(0) === '0') {
                mins = mins.charAt(1);
            } else if (mins.length == 1) {
                mins = mins.charAt(0) + '0';
            }
        } else {
            mins = 0;
        }
        mins = parseInt(mins);
        return new Date(1987, 05, 16, hours, mins, 0, 0);
    }

    function getValueAsDate(num) {
        var value = parseFloat(num).toFixed(2).toString().split('.');
        var hours = parseInt(value[0]);
        var mins = value[1];
        if (mins) {
            if (mins.charAt(0) === '0') {
                mins = mins.charAt(1);
            } else if (mins.length == 1) {
                mins = mins.charAt(0) + '0';
            }
        } else {
            mins = 0;
        }
        mins = parseInt(mins);
        return new Date(1987, 05, 16, hours, mins, 0, 0);
    }

    function getOccurencesCell(row) {
        return $(row).find('td.' + classOccurrences);
    }

    function getOccurencesCellValue(row) {
        return parseInt(getOccurencesCell($(row)).find('input').val());
    }

    function getOccurencesCellValidator(row) {
        var cell = getOccurencesCell($(row))
        return $(cell.find('.' + classValidator)[0]);
    }

    function getDurationPaidCell(row) {
        return $(row).find('td.' + classDurationPaid);
    }

    function getDurationPaidCellValue(row) {
        return parseFloat(getDurationPaidCell(row).text()).toFixed(2);
    }

    function getDurationPaidAsString(durationPaid) {
        return getHoursMinsString(durationPaid);
    }

    function getRosteredDurationAsString(rosteredDurationPaid) {
        return getHoursMinsString(rosteredDurationPaid);
    }

    function getHoursMinsString(hoursMins) {
        var hoursMinsTokens = hoursMins.toString().split('.');
        var hoursMinsHours = parseInt(hoursMinsTokens[0]);
        var hoursMinsMins = hoursMinsTokens[1];
        if (hoursMinsMins) {
            if (hoursMinsMins.charAt(0) === '0') {
                hoursMinsMins = hoursMinsMins.charAt(1);
            }
        } else {
            hoursMinsMins = '0';
        }
        hoursMinsMins = parseInt(hoursMinsMins);
        return hoursMinsHours.toString() + ' hour(s) ' + hoursMinsMins.toString() + ' min(s)';
    }

    function loadTable(items) {
        var serviceResponse = $settings.serviceDomProformas.GetDomProformaInvoiceContractVisitCodes($settings.paymentSchedule.DomContractID, $settings.currentSettings.weekEnding);
        if (!CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
            $dlgDiv.dialog('close');
            return false;
        }
        visitCodes = serviceResponse.value.List;
        if (tblRecords) {
            tblRecords.remove();
        }
        if (divRecords) {
            divRecords.empty();
        } else {
            divRecords = $('<div />').appendTo($dlgDiv);
        }
        tblRecords = $('<table class=\'listTable\' cellspacing=\'0\' cellpadding=\'2\' width=\'675px\' style=\'float: right;\'>').appendTo(divRecords);
        tblRecords.append($('<thead><tr><th style=\'width: 300px;\'>Visit Code</th><th>Rostered Duration</th><th>Duration Paid</th><th>Occurrences</th>' + (($settings.currentSettings.isEditable) ? '<th>&nbsp;</th>' : '') + '</tr></thead>'));
        tblRecordsTbody = tblRecords.append($('<tbody />'));
        $.each(items, function(idx, val) {
            addRow(val, true);
        });
        $('<div style=\'clear: both;\'></div>').appendTo(divRecords);
        if ($settings.currentSettings.isEditable) {
            $('<br />').appendTo(divRecords);
            $('<input type=\'button\' value=\'Add\' />').css('float', 'right').attr('title', 'Add New Record?').appendTo(divRecords).click(function(e) {
                var blankDate = new Date();
                blankDate.setHours(0, 0, 0, 0);
                var newItem = { ID: 0, DomProformaInvoiceID: 0, DomProformaInvoiceDetailID: $settings.currentSettings.id, DomVisitCodeID: 0, RosteredDuration: blankDate, Occurrences: 0, DurationPaid: blankDate };
                var insertedRow = addRow(newItem, true);
                if (insertedRow) {
                    setRowStatus(insertedRow, recordStatus.Inserted);
                }
                disableOkButton(true);
            });
        }
        var currentProforma = $settings.proformaInvoice;
        var currentProformaDetail = $settings.currentSettings.proformaInvoiceDetail;
        var tooltipItems = [];
        var tooltipInvoker = $('<span class=\'TooltipInformation\' title=\'View Further Information?\' />');
        tooltipItems.push({ name: 'Payment Schedule', value: currentProforma.PaymentScheduleReference });
        tooltipItems.push({ name: 'Payments From', value: Date.strftime("%d/%m/%Y", currentProforma.PaymentScheduleDateFrom) });
        tooltipItems.push({ name: 'Payments To', value: Date.strftime("%d/%m/%Y", currentProforma.PaymentScheduleDateTo) });
        tooltipItems.push({ name: 'Provider', value: currentProforma.ProviderReference + ': ' + currentProforma.ProviderName });
        tooltipItems.push({ name: 'Contract', value: currentProforma.DomContractNumber + ': ' + currentProforma.DomContractTitle });
        tooltipItems.push({ name: 'Week Ending', value: Date.strftime("%d/%m/%Y", $settings.currentSettings.weekEnding) });
        tooltipItems.push({ name: 'Payment Ref', value: currentProforma.OurReference });
        tooltipItems.push({ name: 'Rate Category', value: currentProformaDetail.DomRateCategoryDescription });
        tooltipItems.push({ name: 'Unit Cost', value: currentProformaDetail.UnitCost.toFixed(2) });
        tooltipItems.push({ name: 'Planned Units', value: currentProformaDetail.PlannedUnits.toFixed(2) });
        tooltipItems.push({ name: 'Delivered Units', value: currentProformaDetail.DeliveredUnits.toFixed(2) });
        $dlgDiv.dialog('addLeftAlignedButtonPaneControl', { control: tooltipInvoker });
        tooltipInvoker.each(function() {
            $(this).qtip({
                content: {
                    title: 'Proforma Invoice Detail Information',
                    text: function(api) {
                        return getToolTipNameValueContent({ items: tooltipItems });
                    }
                },
                position: {
                    my: 'left top',
                    at: 'right top',
                    viewport: $(window)
                }
            });
        });
        displayLoadingDiv(false, null);
        disableOkButton(true);
    }

    function recalculateDurationPaid(row) {
        var rowData = getRowData(row);
        row = $(row);
        var tdVisitCodeVal = getVisitCodeCellValue(row);
        var tdRosteredDurationVal = getRosteredDurationCellValue(row);
        var tdRosteredDurationInput = getRosteredDurationCell($(row)).find('input');
        var tdRosteredDurationValidator = getRosteredDurationCellValidator($(row));
        var tdOccurrencesVal = getOccurencesCellValue(row);
        var tdOccurrencesValInput = getOccurencesCell($(row)).find('input');
        var tdOccurrencesValidator = getOccurencesCellValidator($(row));
        var tdDurationPaid = getDurationPaidCell(row);
        var selectedVisitCode = null;
        disableOkButton(true);
        if (isNaN(tdRosteredDurationVal) || (tdRosteredDurationVal <= 0 || tdRosteredDurationVal > 23.59)) {
            tdRosteredDurationValidator.show();
            setTimeout(function() { tdRosteredDurationInput.focus().select(); }, 1);
            tdRosteredDurationInput.attr(attrInvalid, 'true');
            return false;
        } else {
            tdRosteredDurationValidator.hide();
            tdRosteredDurationInput.val(tdRosteredDurationVal);
            tdRosteredDurationInput.attr(attrInvalid, 'false');
        }
        if (isNaN(tdOccurrencesVal) || tdOccurrencesVal < 1) {
            tdOccurrencesValidator.show();
            setTimeout(function() { tdOccurrencesValInput.focus().select(); }, 1);
            tdOccurrencesValInput.attr(attrInvalid, 'true');
            return false;
        } else {
            tdOccurrencesValidator.hide();
            tdOccurrencesValInput.val(tdOccurrencesVal);
            tdOccurrencesValInput.attr(attrInvalid, 'false');
        }
        $.each(visitCodes, function(vstKey, vstValue) {
            if (vstValue.ID == tdVisitCodeVal) {
                selectedVisitCode = vstValue;
            }
        });
        var durationPaid = 0.00;
        if (selectedVisitCode) {
            var actualDuration = 0.00;
            var standardTimePaid = selectedVisitCode.StandardTimePaid;
            var maximumTimePaid = selectedVisitCode.MaximumTimePaid;
            var providerPaid = selectedVisitCode.ProviderPaid;
            if ((rowData.Recalculate === undefined || rowData.Recalculate === false) && rowData.ID > 0) {
                standardTimePaid = rowData.StandardTimePaid;
                maximumTimePaid = rowData.MaximumTimePaid;
                providerPaid = rowData.ProviderPaid;
            }
            var rosteredDurationHoursAndMins = getValueAsDate(tdRosteredDurationVal);
            var rosteredDurationHoursAndMinsAsMins = (rosteredDurationHoursAndMins.getHours() * 60) + rosteredDurationHoursAndMins.getMinutes();
            if (providerPaid === -1) {
                if (standardTimePaid === 0 && maximumTimePaid === 0) {
                    actualDuration = tdRosteredDurationVal;
                } else if (standardTimePaid != 0) {
                    actualDuration = parseFloat(parseInt((standardTimePaid / 60)).toString() + '.' + (standardTimePaid % 60).toString());
                } else if (maximumTimePaid != 0 && rosteredDurationHoursAndMinsAsMins > maximumTimePaid) {
                    actualDuration = parseFloat(parseInt((maximumTimePaid / 60)).toString() + '.' + (maximumTimePaid % 60).toString());
                } else {
                    actualDuration = tdRosteredDurationVal;
                }
            }
            durationPaid = parseFloat(actualDuration).toFixed(2);
            setRowStatus(row, recordStatus.Updated);
        }
        tdDurationPaid.text(getDurationPaidAsString(durationPaid));
        rowData.DurationPaidTemp = durationPaid;
        return true;
    }

    function saveChanges(src) {
        disableButtons(true);
        var rowsToSave = getRowsForSave(atLeastOneChange);
        if ($.isFunction($settings.onOkd)) {
            $settings.onOkd({ currentSettings: $settings.currentSettings, rows: rowsToSave });
        }
        atLeastOneChange = false;
        $dlgDiv.dialog('close');
        disableButtons(false);
    }

    function setRowData(row, data) {
        $(row).data(rowDataKey, data);
    }

    function setRowStatus(row, status) {
        row = $(row);
        var rowData = getRowData(row);
        var rowStatusChanged = false;
        if (rowData) {
            switch (status) {
                case recordStatus.Inserted:
                    if (rowData.CrudStatus != recordStatus.Deleted && rowData.CrudStatus != recordStatus.Updated) {
                        rowData.CrudStatus = status;
                        rowStatusChanged = true;
                        atLeastOneChange = true; ;
                    }
                    break;
                case recordStatus.Updated:
                    if (rowData.CrudStatus != recordStatus.Deleted && rowData.CrudStatus != recordStatus.Inserted) {
                        rowData.CrudStatus = status;
                        rowStatusChanged = true;
                        atLeastOneChange = true; ;
                    }
                    if (!rowData.ID || rowData.ID === 0) {
                        atLeastOneChange = true;
                    }
                    break;
                case recordStatus.Deleted:
                    if (rowData.CrudStatus != recordStatus.Deleted) {
                        rowData.CrudStatus = status;
                        rowStatusChanged = true;
                        atLeastOneChange = true; ;
                    }
                    break;
            }
        }
        if (atLeastOneChange && tblRecordsTbody.find('[' + attrInvalid + '=\'true\']').length == 0) {
            disableOkButton(false);
        }
    }

    var methods = {
        init: function(options) {
            if (!$dlgDiv) {
                if (options) {
                    $.extend($settings, options);
                }
                if (!$settings.paymentSchedule) {
                    alert('Please specify the option \'paymentSchedule\'');
                    return;
                }
                if (!$settings.proformaInvoice) {
                    alert('Please specify the option \'proformaInvoice\'');
                    return;
                }
                $dlgDiv = $('<div>');
                $dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999,
                    title: 'Non Delivery of Service',
                    buttons: {
                        "Cancel": function() {
                            cancelClicked($(this));
                        }
                    }
                });
            }
        },
        show: function(options) {
            if ($dlgDiv) {
                var buttons = null;
                var $showSettings = {
                    id: 0,
                    isEditable: 0,
                    weekEnding: null,
                    overriddenItems: [],
                    proformaInvoiceDetail: null,
                    heading: 'Non Delivery of Service'
                };
                if (options) {
                    $.extend($showSettings, options);
                }
                if (!$showSettings.proformaInvoiceDetail) {
                    alert('Please specify the option \'proformaInvoiceDetail\'');
                    return;
                }
                $settings.currentSettings = $showSettings;
                atLeastOneChange = false;
                if ($showSettings.isEditable == true) {
                    buttons = {
                        "OK": function() {
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
                    width: 700,
                    maxHeight: 315,
                    buttons: buttons,
                    beforeClose: function(event, ui) {
                        return confirmClose($(this));
                    },
                    title: $showSettings.heading
                });
                $dlgDiv.dialog('open');
                disableButtons(true);
                displayLoadingDiv(true);
                if (!$showSettings.overriddenItems || $showSettings.overriddenItems.length == 0) {
                    $settings.serviceDomProformas.GetDomProformaInvoiceNonDeliveryVisitBased(0, $showSettings.id, getDomProformaInvoiceNonDeliveryVisitBasedCallBack);
                } else {
                    loadTable($showSettings.overriddenItems);
                }
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);
