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
    var classServiceOutcome = 'so', classUnits = 'un', classPaid = 'pd';
    var rowDataKey = 'rowData';
    var recordStatus = { Deleted: 1, Inserted: 2, Updated: 3, NotChanged: 0 };
    var serviceOutcomes = [], atLeastOneChange = false, attrInvalid = 'isInvalid';

    $.fn.nonDeliveryUnitBasedProformaInvoiceLineDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.nonDeliveryUnitBasedProformaInvoiceLineDialog');
            }
        });
    };

    function addRow(item, suppressStatus, isNew) {
        if (item) {
            item.OpeningCrudStatus = item.CrudStatus;
        }
        if (item) {
            var currentRow = $('<tr />').appendTo(tblRecordsTbody);
            var tdServiceOutcomes = $('<td />').addClass(classServiceOutcome).appendTo(currentRow);
            var tdUnits = $('<td />').addClass(classUnits).appendTo(currentRow);
            var tdPaid = $('<td />').addClass(classPaid).appendTo(currentRow);
            if ($settings.currentSettings.isEditable) {
                var ddServiceOutcomes = $('<select style=\'width: 100%;\' title=\'Select Service Outcome\'>').appendTo(tdServiceOutcomes).change(function(e) {
                    var src = $(e.currentTarget), row = src.closest('tr'), rowData = getRowData(row);
                    var paidCell = getPaidCell(row), serviceOutcome = getServiceOutcome(getServiceOutcomeCellValue(row));
                    if (serviceOutcome.ProviderPaid === -1) {
                        paidCell.text('Yes');
                    } else {
                        paidCell.text('No');
                    }
                    rowData.Recalculate = true;
                    setRowStatus(row, recordStatus.Updated);
                });
                $.each(serviceOutcomes, function(soKey, soValue) {
                    var serviceOutcomeOpt = $('<option>').text(soValue.Description).attr('title', soValue.Description).val(soValue.ID).appendTo(ddServiceOutcomes);
                    if (soValue.ID == item.ServiceOutcomeID) {
                        serviceOutcomeOpt.attr('selected', true);
                    }
                });
                var txtUnits = $('<input type=\'text\' value=\'' + item.Units + '\' title=\'Set Units - Number Greater Than 0\' />').css('width', '95%').change(function(e) {
                    var src = $(e.currentTarget), row = src.closest('tr'), tdUnitsVal = getUnitsCellValue(row);
                    if (isNaN(tdUnitsVal) || tdUnitsVal <= 0) {
                        alert('Please enter a number greater than 0.');
                        setTimeout(function() { src.focus().select(); }, 1);
                        src.attr(attrInvalid, 'true');
                    } else {
                        src.attr(attrInvalid, 'false').val(tdUnitsVal);
                    }
                    setRowStatus(src.closest('tr'), recordStatus.Updated);
                }).appendTo(tdUnits);
                tdPaid.text(((item.Paid) ? 'Yes' : 'No'));
                var tdDelete = $('<td class=\'deleteInvoiceLine\' title=\'Delete?\'>&nbsp;</td>').click(function(e) {
                    var src = $(e.currentTarget), row = src.closest('tr');
                    deleteRow(row);
                });
                currentRow.append(tdDelete);
                setRowData(currentRow, item);
            } else {
                $.each(serviceOutcomes, function(soKey, soValue) {
                    if (soValue.ID == item.ServiceOutcomeID) {
                        tdServiceOutcomes.text(soValue.Description);
                        return false;
                    }
                });
                tdUnits.text(item.Units);
                tdPaid.text(((item.Paid) ? 'Yes' : 'No'));
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

    function getDomProformaInvoiceNonDeliveryUnitBasedCallBack(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
            loadTable(serviceResponse.value.List);
        } else {
            $dlgDiv.dialog('close');
        }
    }

    function getRowData(row) {
        return $(row).data(rowDataKey);
    }

    function getRowsForSave(setRowData) {
        var rowsForSave = [], rowsForSaveCnt = 0;
        $.each(tblRecords.find('tr'), function(rowIdx, rowVal) {
            rowVal = $(rowVal);
            var rowData = getRowData(rowVal);
            var serviceOutcome;
            if (rowData) {
                if (setRowData) {
                    rowData.ServiceOutcomeID = getServiceOutcomeCellValue(rowVal);
                    rowData.Units = getUnitsCellValue(rowVal);
                    rowData.Paid = getPaidCellValue(rowVal);
                    rowData.UnitsPaid = 0;
                    serviceOutcome = getServiceOutcome(rowData.ServiceOutcomeID);
                    if (serviceOutcome) {
                        if (serviceOutcome.ProviderPaid === -1) {
                            rowData.UnitsPaid = rowData.Units;
                        }
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

    function getServiceOutcome(id) {
        var serviceOutcome;
        id = parseInt(id);
        $.each(serviceOutcomes, function(soKey, soValue) {
            if (soValue.ID == id) {
                serviceOutcome = soValue;
                return false;
            }
        });
        return serviceOutcome;
    }

    function getServiceOutcomeCell(row) {
        return $(row).find('td.' + classServiceOutcome);
    }

    function getServiceOutcomeCellValue(row) {
        return parseInt(getServiceOutcomeCell($(row)).find('option:selected').val());
    }

    function getUnitsCell(row) {
        return $(row).find('td.' + classUnits);
    }

    function getUnitsCellValue(row) {
        return parseInt(getUnitsCell($(row)).find('input').val());
    }

    function getPaidCell(row) {
        return $(row).find('td.' + classPaid);
    }

    function getPaidCellValue(row) {
        if (getPaidCell($(row)).text('Yes')) {
            return -1;
        } else {
            return 0;
        }
    }

    function loadTable(items) {
        var serviceResponse = $settings.serviceDomProformas.GetDomProformaInvoiceContractServiceOutcomes($settings.paymentSchedule.DomContractID, $settings.currentSettings.weekEnding);
        if (!CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
            $dlgDiv.dialog('close');
            return false;
        }
        serviceOutcomes = serviceResponse.value.List;
        if (!serviceOutcomes || serviceOutcomes.length == 0) {
            alert('Service Outcomes cannot be determined, please specify using the Abacus Intranet product.');
            $dlgDiv.dialog('close');
            return false;
        }
        if (tblRecords) {
            tblRecords.remove();
        }
        if (divRecords) {
            divRecords.empty();
        } else {
            divRecords = $('<div />').appendTo($dlgDiv);
        }
        tblRecords = $('<table class=\'listTable\' cellspacing=\'0\' cellpadding=\'2\' width=\'675px\' style=\'float: right;\'>').appendTo(divRecords);
        tblRecords.append($('<thead><tr><th style=\'width: 300px;\'>Service Outcome</th><th>Units</th><th>Paid</th>' + (($settings.currentSettings.isEditable) ? '<th>&nbsp;</th>' : '') + '</tr></thead>'));
        tblRecordsTbody = tblRecords.append($('<tbody />'));
        $.each(items, function(idx, val) {
            addRow(val, true, false);
        });
        $('<div style=\'clear: both;\'></div>').appendTo(divRecords);
        if ($settings.currentSettings.isEditable) {
            $('<br />').appendTo(divRecords);
            $('<input type=\'button\' value=\'Add\' title=\'Add New Record?\' />').css('float', 'right').appendTo(divRecords).click(function(e) {
                var blankDate = new Date();
                blankDate.setHours(0, 0, 0, 0);
                var newItem = { ID: 0, DomProformaInvoiceID: 0, DomProformaInvoiceDetailID: $settings.currentSettings.id, ServiceOutcomeID: 0, Units: 1, Paid: 0 };
                var insertedRow = addRow(newItem, true, true);
                if (insertedRow) {
                    setRowStatus(insertedRow, recordStatus.Inserted);
                }
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
        //tooltipItems.push({ name: 'Period From', value: Date.strftime("%d/%m/%Y", currentProforma.WEFrom) });
        //tooltipItems.push({ name: 'Period To', value: Date.strftime("%d/%m/%Y", currentProforma.WETo) });
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
                        atLeastOneChange = true;
                    }
                    break;
                case recordStatus.Updated:
                    if (rowData.CrudStatus != recordStatus.Deleted && rowData.CrudStatus != recordStatus.Inserted) {
                        rowData.CrudStatus = status;
                        rowStatusChanged = true;
                        atLeastOneChange = true;
                    }
                    if (!rowData.ID || rowData.ID === 0) {
                        atLeastOneChange = true;
                    }
                    break;
                case recordStatus.Deleted:
                    if (rowData.CrudStatus != recordStatus.Deleted) {
                        rowData.CrudStatus = status;
                        rowStatusChanged = true;
                        atLeastOneChange = true;
                    }
                    break;
            }
        }
        disableOkButton(!(atLeastOneChange && tblRecordsTbody.find('[' + attrInvalid + '=\'true\']').length == 0));
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
                    $settings.serviceDomProformas.GetDomProformaInvoiceNonDeliveryUnitBased(0, $showSettings.id, getDomProformaInvoiceNonDeliveryUnitBasedCallBack);
                } else {
                    loadTable($showSettings.overriddenItems);
                }
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);
