var tblProformas, lblFilterCriteria, contractSvc, proformaSvc;
var cellIdxNotes = 7, notesFilterCtrl, notesFilterCtrlVal, notesFilterCtrlCellClass;
var cellIdxEditStatus = 8, invoiceStatusFilterCtrl, invoiceStatusFilterCtrlVal, invoiceStatusFilterCellClass;
var cellIdxChkBoxStatus = 0, chkBoxFilterCtrl, chkBoxFilterCtrlVal, chkBoxFilterCellClass;
var cellIdxVerifyStatus = 5;
var btnDeleteProforma, btnVerifyProforma, btnUnVerifyProforma, btnReports;

var classStatus = 'st', classStatusVerified = 'stv', classStatusAwaitingVerification = 'stav', classEditStatus = 'es', serviceUserName = 'sn';
var classEditStatusRead = 'esRead', classEditStatusSaved = 'esSaved', classEditStatusUnread = 'esUnread', classWeeks = 'w';
var classNote = 'q', classNoteNoNote = 'qNoNote', classNotePrivateNote = 'qPrivateNote', classNoteInvoiceNote = 'qInvoiceNote', classPayment = 'p';
var selectedItem = { id: 0, row: null };
var statusAwaitingVerification = 'Awaiting Verification', statusVerified = 'Verified';
var filterServiceUserReference, filterServiceUserName, filterPaymentReference, filterWeeks, filterNoteType, filterBatchStatus, filterEditStatus, filterChkBoxStatus;
var hidContractBlockGuarantee, inlineStatusChangeCall;

$(function () {

    tblProformas = $('#tblProformas');
    lblFilterCriteria = $('#lblFilterCriteria');
    btnDeleteProforma = $('input[name$="btnDeleteProforma"]');
    btnVerifyProforma = $('input[name$="btnVerifyProforma"]');
    btnUnVerifyProforma = $('input[name$="btnUnVerifyProforma"]');
    btnRecalculate = $('input[name$="btnRecalculate"]');
    hidContractBlockGuarantee = $('input[name$="hidContractBlockGuarantee"]');
    hidContractBlockGuarantee.val(currentContractIsBlockGuarantee);
    inlineStatusChangeCall = false;

    btnReports = $('button[id$="rptPrint_btnReports"]');
    proformaSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomProfomaInvoice_class();
    contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
    var tblProformasOptions = {
        filterDelay: 0,
        customFilterControls: [{ index: cellIdxChkBoxStatus, control: GetCheckboxFilterCtrl() }, { index: cellIdxNotes, control: GetNotesFilterCtrl() }, { index: cellIdxEditStatus, control: GetStatusFilterCtrl()}],
        onMatchingCell: function (tbl, row, rowIdx, cell, cellIdx, cellText, isMatch) {
            if (rowIdx == 0) {
                return true;
            }
            if (cellIdx == cellIdxChkBoxStatus) {
                if (!chkBoxFilterCtrlVal || chkBoxFilterCtrlVal == '') {
                    return true;
                } else {
                    if (chkBoxFilterCtrlVal == 'Unselected') {
                        return (cell[0].childNodes[1].checked == false);
                    } else {
                        return (cell[0].childNodes[1].checked == true);
                    }
                }


            }
            if (cellIdx == cellIdxNotes) {
                return IsCustomFilterCellMatching(cell, notesFilterCtrlCellClass);
            }
            else if (cellIdx == cellIdxEditStatus) {
                return IsCustomFilterCellMatching(cell, invoiceStatusFilterCellClass);
            } else {
                return isMatch;
            }
            return isMatch;
        },
        onFilterCompleted: function (tbl, filterCells, filterCount) {
            var filterStr = '<b>Filtered By:</b> ';
            var filterParamsStr = '';
            var filterCellHeading = '';
            var filterCellValue = '';
            var btnReportsId = btnReports.attr('id');
            $.each(filterCells, function (cellKey, cellValue) {
                if (cellValue.isFilterable) {
                    if ($.trim(cellValue.cellHeading).length > 1) {
                        filterCellHeading = cellValue.cellHeading;
                        filterCellValue = cellValue.currentValue;
                    } else {
                        if (cellKey == 0) {
                            filterCellHeading = 'Invoices';
                            filterCellValue = cellValue.currentValue;
                        } else {
                            filterCellHeading = 'Edit Status';
                            filterCellValue = cellValue.currentValue;
                        }
                    }
                    if (filterCellValue === undefined) {
                        filterCellValue = '';
                    }
                    filterCellValue = filterCellValue.replace('^', '');
                    filterCellValue = filterCellValue.replace('$', '');
                    if ($.trim(filterCellValue) != '') {
                        filterParamsStr += filterCellHeading + ' = <i>' + filterCellValue + '</i>; ';
                    }
                    switch (cellValue.cellHeading) {
                        case 'Service User Ref':
                            ReportsButton_AddParam(btnReportsId, 'strServiceUserRef', filterCellValue);
                            filterServiceUserReference = filterCellValue;
                            break;
                        case 'Service User Name':
                            ReportsButton_AddParam(btnReportsId, 'strServiceUserName', filterCellValue);
                            filterServiceUserName = filterCellValue;
                            break;
                        case 'Payment Ref':
                            ReportsButton_AddParam(btnReportsId, 'strPaymentRef', filterCellValue);
                            filterPaymentReference = filterCellValue;
                            break;
                        case 'Weeks':
                            ReportsButton_AddParam(btnReportsId, 'intWeeks', (filterCellValue) ? parseInt(filterCellValue) : null);
                            filterWeeks = (filterCellValue) ? parseInt(filterCellValue) : null;
                            break;
                        case 'Status':
                            var batchStatusId = null;
                            filterBatchStatus = null;
                            if (filterCellValue) {
                                if (filterCellValue === statusAwaitingVerification) {
                                    batchStatusId = 1;
                                } else {
                                    batchStatusId = 2;
                                }
                            }
                            ReportsButton_AddParam(btnReportsId, 'intBatchStatus', batchStatusId);
                            filterBatchStatus = filterCellValue;
                            break;
                        case 'Notes':
                            var notesId = null;
                            if (filterCellValue) {
                                if (filterCellValue === 'No Note') {
                                    notesId = 0;
                                }
                                else if (filterCellValue === 'Private Note') {
                                    notesId = 1;
                                }
                                else if (filterCellValue === 'Invoice Note') {
                                    notesId = 2;
                                }
                            }
                            ReportsButton_AddParam(btnReportsId, 'intQueryType', notesId);
                            filterNoteType = notesId;
                            break;
                        default:
                            var editStatusId = null;
                            if (filterCellValue) {
                                if (filterCellValue === 'Unread') {
                                    editStatusId = 0;
                                }
                                else if (filterCellValue === 'Read') {
                                    editStatusId = 1;
                                }
                                else if (filterCellValue === 'Saved') {
                                    editStatusId = 2;
                                }
                            }
                            ReportsButton_AddParam(btnReportsId, 'intEditStatus', editStatusId);
                            filterEditStatus = editStatusId;
                            break;
                    }
                }
            });
            if ($.trim(filterParamsStr) != '') {
                filterStr += filterParamsStr + ' <b>(' + (filterCount - 1).toString() + ' records)</b>';
                lblFilterCriteria.show();
            } else {
                filterStr = '';
                lblFilterCriteria.hide();
            }
            lblFilterCriteria.html(filterStr);
            ResetButtonVisibility();
            invoicesSelected();
        },
        onPopulatingDropDown: function (args) {
            if (args.tableHeaderIdx === cellIdxVerifyStatus) {
                var hasAwaiting = false;
                var hasVerified = false;
                $.each(args.items, function (itemIdx, itemVal) {
                    if (itemVal === statusAwaitingVerification) {
                        hasAwaiting = true;
                    }
                    if (itemVal === statusVerified) {
                        hasVerified = true;
                    }
                });
                if (!hasAwaiting) {
                    args.items.push(statusAwaitingVerification);
                }
                if (!hasVerified) {
                    args.items.push(statusVerified);
                }
            }
        }
    }
    var verifyFilter = '';
    if (qsProformaInvoiceStatusShowAwaitingVerification === true && qsProformaInvoiceStatusShowVerified === false) {
        verifyFilter = statusAwaitingVerification;
    } else if (qsProformaInvoiceStatusShowAwaitingVerification === false && qsProformaInvoiceStatusShowVerified === true) {
        verifyFilter = statusVerified;
    }
    tblProformas.tableFilter(tblProformasOptions);
    tblProformas.tableScroll({ height: 200, width: null });
    if (verifyFilter !== '') {
        tblProformas.tableFilter('setFilterValue', { cellIndex: cellIdxVerifyStatus, cellValue: verifyFilter });
    }
    setTimeout(SetProformasTableHeight, 1);
    $(window).bindWithDelay('resize', function () { SetProformasTableHeight(); }, 125);
    ProformaSelected(null, 0);

    ResetButtonVisibility();
    invoicesSelected();

    changeUIForPeriodicBlock(currentContractIsPeriodicBlock);

});

function DeleteProforma() {
    if (IsConfirmed("The selected Pro forma Invoice will be deleted." +
                    "\n\nPlease confirm that you understand this and that you wish to continue.",
                    "All \'Awaiting Verification\' Pro forma Invoices that are selected in the table above " +
                    "(i.e. that meet filtering criteria) will be deleted. " +
                    "\n\nPlease confirm that you understand this and that you wish to continue.")) {

        ChangeProformaInvoiceStatus('Delete');       
           
       
    }    
}

function DisableButton(btn, disabled) {
    if ($(btn) && $(btn).length > 0) {
        $(btn).attr('disabled', disabled);
    }
}

function GetNotesFilterCtrl() {
    if (!notesFilterCtrl) {
        var classFilterRemove = 'FilterRemove';
        var menuItems = [{ id: -1, description: 'Clear Filter', cssClass: 'MenuItemClearFilter', tooltip: 'Clear Filtering', cellClass: '' },
                        { description: 'Private Note', cssClass: 'MenuItemPrivateNote', tooltip: 'Filter by Private Note', cellClass: 'qPrivateNote' },
                        { description: 'Invoice Note', cssClass: 'MenuItemInvoiceNote', tooltip: 'Filter by Invoice Note', cellClass: 'qInvoiceNote' },
                        { description: 'No Note', cssClass: 'MenuItemNoNote', tooltip: 'Filter by No Note', cellClass: 'qNoNote'}];
        notesFilterCtrl = $('<span />');
        notesFilterCtrl.addClass('Filter').attr('title', 'Filter');
        notesFilterCtrl.searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'BottomRight', showSearchBox: false, width: 100 });
        notesFilterCtrl.bind('MenuItemClicked', function(src, menuItem) {
            if (menuItem.id === -1) {
                notesFilterCtrl.removeClass(classFilterRemove);
                notesFilterCtrlVal = '';
                notesFilterCtrlCellClass = '';
            } else {
                notesFilterCtrl.addClass(classFilterRemove);
                notesFilterCtrlVal = menuItem.description;
                notesFilterCtrlCellClass = menuItem.cellClass;
            }
            $(this).trigger('filterTableCellChanged', { filterTable: tblProformas, filterValue: notesFilterCtrlVal });
        });
    }
    return notesFilterCtrl;
}

function GetRowById(id) {
    return $('#tblBody #tr_' + id.toString());
}

function GetEditStatusCell(id) {
    return GetRowById(id).find('td.' + classEditStatus);
}

function GetPaymentCell(id) {
    return GetRowById(id).find('td.' + classPayment);
}

function GetStatusCell(id) {
    return GetStatusCellByRow(GetRowById(id));
}

function GetStatusCellByRow(row) {
    return row.find('td.' + classStatus);
}

function IsVoidPaymentInvoice(row) {
    if (row) {
        return row.find('#voidPayment').val();
    } else {
        return false;
    }
}

function IsBlockGuaranteeContract(row) {
    if (row) {
        return row.find('#IsBlockGuranteeContract').val();
    } else {
        return false;
    }
}

function GetWeeksCell(id) {
    return GetRowById(id).find('td.' + classWeeks);
}

function GetNoteCell(id) {
    return GetRowById(id).find('td.' + classNote);
}

function GetVisibleIds() {
    var id = 0, ids = [];
    $.each(tblProformas.find('tbody tr td:first-child input:first-child'), function(inputIdx, inputVal) {
        var parentTR = $(inputVal).closest('tr')
        if (parentTR.is(':visible')) {
            id = parseInt($(inputVal).val())
            if (id > 0) {
                ids.push(id);
            } 
        }
    });
    return ids;
}

function GetSelectedIds() {
    var id = 0, ids = [];
    $.each(getVisibleCheckedRows(), function(rowIdx, rowVal) {
        id = parseInt(rowVal.attr('id').replace('tr_', ''));
        ids.push(id);
    });
    return ids;
}

function GetCheckboxFilterCtrl() {
    if (!chkBoxFilterCtrl) {
        var classFilterRemove = 'FilterRemove';
        var menuItems = [{ id: -1, description: 'Clear Filter', cssClass: 'MenuItemClearFilter', tooltip: 'Clear Filtering' },
                        { description: 'Show selected items only', cssClass: 'MenuItemSelected', tooltip: 'Show selected items only' },
                        { description: 'Show un-selected items only', cssClass: 'MenuItemUnselected', tooltip: 'Show un-selected items only'}];
            chkBoxFilterCtrl = $('<span />');
            chkBoxFilterCtrl.addClass('Filter').attr('title', 'Filter');
            chkBoxFilterCtrl.searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'BottomLeft', showSearchBox: false, width: 200 });
            chkBoxFilterCtrl.bind('MenuItemClicked', function(src, menuItem) {
            if (menuItem.id == -1) {
                chkBoxFilterCtrl.removeClass(classFilterRemove);
                chkBoxFilterCtrlVal = '';
                chkBoxFilterCellClass = '';
            } else {
                chkBoxFilterCtrl.removeClass(classFilterRemove);
                if (menuItem.description == 'Show un-selected items only') {
                    chkBoxFilterCtrlVal = 'Unselected';
                } else {
                    chkBoxFilterCtrlVal = 'Selected';
                }

            }
            $(this).trigger('filterTableCellChanged', { filterTable: tblProformas, filterValue: chkBoxFilterCtrlVal });
            
            
        });
    }
    return chkBoxFilterCtrl;
}

function GetStatusFilterCtrl() {
    if (!invoiceStatusFilterCtrl) {
        var classFilterRemove = 'FilterRemove';
        var menuItems = [{ id: -1, description: 'Clear Filter', cssClass: 'MenuItemClearFilter', tooltip: 'Clear Filtering', cellClass: '' },
                        { description: 'Unread', cssClass: 'MenuItemUnread', tooltip: 'Filter by Unread', cellClass: 'esUnread' },
                        { description: 'Read', cssClass: 'MenuItemRead', tooltip: 'Filter by Read', cellClass: 'esRead' },
                        { description: 'Saved', cssClass: 'MenuItemSaved', tooltip: 'Filter by Saved', cellClass: 'esSaved'}];
        invoiceStatusFilterCtrl = $('<span />');
        invoiceStatusFilterCtrl.addClass('Filter').attr('title', 'Filter');
        invoiceStatusFilterCtrl.searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'BottomRight', showSearchBox: false, width: 100 });
        invoiceStatusFilterCtrl.bind('MenuItemClicked', function(src, menuItem) {
            if (menuItem.id === -1) {
                invoiceStatusFilterCtrl.removeClass(classFilterRemove);
                invoiceStatusFilterCtrlVal = '';
                invoiceStatusFilterCellClass = '';
            } else {
                invoiceStatusFilterCtrl.addClass(classFilterRemove);
                invoiceStatusFilterCtrlVal = menuItem.description;
                invoiceStatusFilterCellClass = menuItem.cellClass;
            }
            $(this).trigger('filterTableCellChanged', { filterTable: tblProformas, filterValue: invoiceStatusFilterCtrlVal });
        });
    }
    return invoiceStatusFilterCtrl;
}

function IsCustomFilterCellMatching(cell, cellClass) {
    if (!cellClass || cellClass == '') {
        return true;
    } else {
        return (cell.hasClass(cellClass));
    }
}

function ProformaSelected(src, id) {
    var srcRow = $(src).closest('tr');
    ResetButtonVisibility();
    selectedItem.id = id;
    selectedItem.row = srcRow;

    invoicesSelected();
    
}

function GetIsVerifiedByRow(row) {
    var srcCellStatus = GetStatusCellByRow(row);
    return (srcCellStatus.hasClass(classStatusVerified));
}

function GetIsAwaitingVerifiedByRow(row) {
    var srcCellStatus = GetStatusCellByRow(row);
    return (srcCellStatus.hasClass(classStatusAwaitingVerification));
}

function ResetButtonVisibility() {
    var enableRecalcButton;    
    var noVerifiedInvoicesSelected = 0;
    var noAwaitingVerifiedInvoicesSelected = 0;
    var voidPayment = false;
    var visibleRows = [], currentRow;

    $('#tblBody tr td:first-child :checkbox:checked').each(function () {

        currentRow = $(this).closest('tr');
        if (GetIsVerifiedByRow(currentRow)) {
            noVerifiedInvoicesSelected = noVerifiedInvoicesSelected + 1
        }
        if (GetIsAwaitingVerifiedByRow(currentRow)) {
            noAwaitingVerifiedInvoicesSelected = noAwaitingVerifiedInvoicesSelected + 1
        }

        voidPayment = IsVoidPaymentInvoice(currentRow);

    });

    //only enable the recalculate button if there is only one row selected
    //and this row is verified.
    if (noAwaitingVerifiedInvoicesSelected == 1 && userHasRecalculateCommand == true) {
        DisableButton(btnRecalculate, !userHasRecalculateCommand);
    } else {
        DisableButton(btnRecalculate, true);
    }
    //Only enable the following buttons if there is one or more rows selected with a status of awaiting verification.
    if (noAwaitingVerifiedInvoicesSelected >= 1) {
        DisableButton(btnDeleteProforma, !userHasDeleteInvoiceCommand);
        DisableButton(btnVerifyProforma, !userHasVerifyUnverifyInvoiceCommand);
    } else {
        DisableButton(btnDeleteProforma, true);
        DisableButton(btnVerifyProforma, true);
    }
    //Only enable the following buttons if there is one or more rows selected with a status of verified.
    if (noVerifiedInvoicesSelected >= 1) {
        DisableButton(btnUnVerifyProforma, !userHasVerifyUnverifyInvoiceCommand);
    } else {
        DisableButton(btnUnVerifyProforma, true);
    }

    // last check to disable all features if it is a void invoice
    if (voidPayment == "true") {
        DisableButton(btnDeleteProforma, true);
        DisableButton(btnRecalculate, true);
    }
}

function SetProformasTableHeight() {    
    var footerContainer = $('#footerContainer');
    var bottom = footerContainer.position().top + footerContainer.outerHeight();
    var heightOfBody = document.documentElement.clientHeight;
    var remainingSpace = heightOfBody - bottom;
    var tableScrollWrapper = $('.tablescroll_wrapper');
    tableScrollWrapper.height(tableScrollWrapper.height() + (remainingSpace - 2));
}

function ShowNote(id) {
    var initSettings = {
        proformasService: proformaSvc,
        contractService: contractSvc,
        onSaved: function(args) { ShowNoteOnSaved(args); },
        onCancelled: null
    };
    var showSettings = {
        paymentScheduleId: qsPaymentScheduleID,
        proformaInvoiceId: id
    };
    $(document).nonVisitBasedProformaInvoiceNotesDialog(initSettings);
    $(document).nonVisitBasedProformaInvoiceNotesDialog('show', showSettings);

}

function ShowNoteOnSaved(args) {
    var className = classNoteNoNote;
    switch (args.note.noteType) {
        case 1:
            className = classNotePrivateNote;
            break;
        case 2:
            className = classNoteInvoiceNote;
            break;
        default:
            className = classNoteNoNote;
    }
    SetNoteCellClass(args.currentSettings.proformaInvoiceId, className);
}

function SetNoteCellClass(id, className) {
    var cell = GetNoteCell(id);
    if (cell.length) {
        cell.removeClass().addClass(classNote).addClass(className);
    }
    tblProformas.tableFilter('refresh');
}

function ShowProforma(id, suRef, suName, suId) {
    var statusCell = GetStatusCell(id);
    var initSettings = {
        serviceDomProformas: proformaSvc,
        paymentSchedule: paymentSchedule,  
        paymentScheduleId: qsPaymentScheduleID,
        rateCategories: proformaInvoicesRateCats,
        weekEndingDay: weekEndingDay,
        onSaved: function(settings) { ShowProformaOnSaved(settings); },
        onClosed: function(settings) { ShowProformaOnClosed(settings); }     
    };
    var showSettings = {
        id: id,
        serviceUserId: suId,
        serviceUserName: suName,
        serviceUserReference: suRef,
        isEditable: (userHasEditProformaInvoiceCommand && !statusCell.hasClass(classStatusVerified))    
    };
    $(document).nonVisitBasedProfomaInvoiceDialog(initSettings);
    $(document).nonVisitBasedProfomaInvoiceDialog('show', showSettings);
}

function SetEditStatusCellClass(id, className) {
    var cell = GetEditStatusCell(id);
    if (cell.length) {
        if (!cell.hasClass(classEditStatusSaved)) {
            var editStatusTitle = className.substring(2);
            cell.attr('title', editStatusTitle);
            cell.removeClass().addClass(classEditStatus).addClass(className);
        }
    }
    tblProformas.tableFilter('refresh');
}

function SetPaymentCellText(id, txt) {
    var cell = GetPaymentCell(id);
    if (cell.length) {
        cell.text('£' + parseFloat(txt).toFixed(2).toString());
    }
    tblProformas.tableFilter('refresh');
}

function SetWeekCellText(id, txt) {
    var cell = GetWeeksCell(id);
    if (cell.length) {
        cell.text(txt);
    }
    tblProformas.tableFilter('refresh');
}

function ShowProformaOnSaved(settings) {
    SetEditStatusCellClass(settings.id, classEditStatusSaved);
    SetWeekCellText(settings.id, settings.savedItem.Weeks);
    SetPaymentCellText(settings.id, settings.savedItem.CalculatedPayment.toString());
    tblProformas.tableFilter('repopulateDropDowns');
}

function ShowProformaOnClosed(settings) {
    SetEditStatusCellClass(settings.id, classEditStatusRead);
}

function Recalculate_click() {
    if (window.confirm("Are you sure you wish to recalculate this invoice?")) {
        proformaSvc.RecalculateDomProformaInvoice(selectedItem.id, RecalculateInvoice_Callback);
    }
}

function RecalculateInvoice_Callback(response) {
    if (CheckAjaxResponse(response, contractSvc.url)) {
        document.location.href = document.location.href;
    }
}

var lastChangeProformaInvoiceStatus = '';

function ChangeProformaInvoiceStatus(status) {
    var statusAwaitingTmp = false, statusVerifiedTmp = false;
    var serviceUserRef = filterServiceUserReference, serviceUserName = filterServiceUserName;
    DisplayLoading(true);
    if (filterBatchStatus === statusAwaitingVerification) {
        statusAwaitingTmp = true;
    }
    else if (filterBatchStatus === statusVerified) {
        statusVerifiedTmp = true;
    } else {
        statusAwaitingTmp = true;
        statusVerifiedTmp = true;
    }
    if (serviceUserRef) {
        serviceUserRef = '%' + filterServiceUserReference + '%';
    }
    if (serviceUserName) {
        serviceUserName = '%' + filterServiceUserName + '%';
    }
    lastChangeProformaInvoiceStatus = status;

    var processIDs = [], processID;

    $.each(getVisibleCheckedRows(), function(idx, row) {
        processID = parseInt(row.attr('id').replace('tr_', ''));
        if (status === 'UnVerify') {
            if (GetIsVerifiedByRow(row) == true) {
                processIDs.push(processID);
            }
        } else if (status === 'Delete' || status === 'Verify') {
            if (GetIsAwaitingVerifiedByRow(row) == true) {
                processIDs.push(processID);
            }
        }
    });
    
    proformaSvc.ChangeDomProformaInvoiceStatusFromIDs(qsPaymentScheduleID, processIDs, status, ChangeProformaInvoiceStatusCallBack);
}

function ChangeProformaInvoiceStatusCallBack(serviceResponse) {
    if (CheckAjaxResponse(serviceResponse, proformaSvc.url)) {
        var isDelete = false, statusRow, statusCell, statusCellText, statusCellClass;
        $.each(serviceResponse.value.List, function (statusIdx, statusVal) {
            isDelete = false;
            statusRow = GetRowById(statusVal.InvoiceID);
            statusCell = GetStatusCellByRow(statusRow);
            if (statusVal.StatusDesc === 'Deleted') {
                isDelete = true;
            }
            else {
                statusCellText = statusVal.StatusDesc
                statusCellClass = '';
                if (statusVal.StatusDesc === statusVerified) {
                    statusCellClass = classStatusVerified;
                } else if (statusVal.StatusDesc === statusAwaitingVerification) {
                    statusCellClass = classStatusAwaitingVerification;
                }
            }
            // if row is deleted then just remove that other wise change status.                
            if (!isDelete) {
                statusCell.removeClass().addClass(classStatus).addClass(statusCellClass);
                ReflectInlineStatusChange(statusVal.InvoiceID, statusCellText);
                if (inlineStatusChangeCall === true) {
                    UnCheckAutoCheckedCheckBox(statusVal.InvoiceID);
                 }
                statusCell.text(statusCellText);
            } else {
                statusRow.remove();
            }
        });
    }
    // in end must set to false.
    inlineStatusChangeCall = false;
    ResetButtonVisibility();
    tblProformas.tableFilter('refresh');
    ProformaSelected(null, 0);
    DisplayLoading(false);
    ReloadPage();

}

function RemoveDeletedRows(serviceResponse) {
    var tblProforma = GetElement('tblProformas');
    var idToRemove;
    var statusRow;
    var RowsToRemove = new Array();
    var count = -1;
    // no item was selected but some have been deleted.
    if (selectedItem.id == 0) {
        for (index = 1; index < tblProforma.tBodies[0].rows.length; index++) {
            var rowid = tblProforma.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0].value;
            var RowPresentInNewList = false;
            for (i = 0; i < serviceResponse.value.List.length; i++) {
                //serviceResponse.value.List[0].InvoiceID
                if (rowid == serviceResponse.value.List[i].InvoiceID) {
                    RowPresentInNewList = true;
                }
            }
            // if row does not exists in the new list then remove this.
            if (!RowPresentInNewList) {
                count++;
                RowsToRemove[count] = rowid;
            }
        }
    }

    for (index = 0; index < RowsToRemove.length; index++) {
        statusRow = GetRowById(RowsToRemove[index]);
        statusRow.remove();
    }

}

(function($) {

    var $classRowCost = 'cost';
    var $classRowDelivered = 'delivered';
    var $classRowNotDelivered = 'notDelivered';
    var $dlgDiv = null;
    var $dlgDivLoading = null;
    var $dropDownDayOfWeek = null;
    var $dropDownServiceOutcome = null;
    var $keyDay = 'day';
    var $rowDataKey = 'rowData';
    var $settings = {
        rateCategories: null,
        serviceDomProformas: null,
        currentSettings: null,
        paymentScheduleId: 0,
        paymentSchedule: null,
        onSaved: null,
        onClosed: null,
        weekEndingDay: 0
    };
    var $divInvoices = null;
    var $tblInvoices = null;
    var $tblInvoicesTbody = null;
    var recordStatus = { Deleted: 1, Inserted: 2, Updated: 3, NotChanged: 0 };
    var attrInvalid = 'isInvalid', atLeastOneChanged = true;

    $.fn.nonVisitBasedProfomaInvoiceDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.nonVisitBasedProfomaInvoiceDialog');
            }
        });
    };

    function addLine(item) {
        displayLoadingDiv(true, null);
        $settings.serviceDomProformas.GetDomProformaInvoiceDetailForNew(item.currentSettings.id, item.rateCategoryID, item.weekEnding.toDate(), item.unitCost, addLineCallBack);
    }

    function addLineCallBack(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
            var item = serviceResponse.value.Item;
            if (item) {
                var matchingLineFound = false;
                $.each($tblInvoicesTbody.find('tr'), function(trIdx, trVal) {
                    trVal = $(trVal);
                    var rowData = getRowData(trVal);
                    if (rowData) {
                        var weekEndingDatesMatch = (Date.strftime("%d/%m/%Y", rowData.WeekEnding) === Date.strftime("%d/%m/%Y", item.WeekEnding));
                        var rateCategoriesMatch = (rowData.DomRateCategoryID == item.DomRateCategoryID);
                        var unitCostsMatch = (rowData.UnitCost == item.UnitCost);
                        if (weekEndingDatesMatch && rateCategoriesMatch && unitCostsMatch) {
                            trVal.show();
                            trVal.find('td.' + $classRowDelivered + ' > input').focus().select();
                            matchingLineFound = true;
                            return false;
                        }
                    }
                });
                if (!matchingLineFound) {
                    var insertBefore = null;
                    $.each($tblInvoicesTbody.find('tr td:first-child'), function(tdIdx, tdVal) {
                        tdVal = $(tdVal);
                        if (tdVal.text()) {
                            var tdValDate = tdVal.text().toDate();
                            if (item.WeekEnding < tdValDate) {
                                insertBefore = tdVal.closest('tr');
                                return false;
                            }
                        }
                    });
                    var insertedRow = addRow(item, insertBefore);
                    if (insertedRow) {
                        setRowStatus(insertedRow, recordStatus.Inserted);
                    }
                }
            }
        }
        displayLoadingDiv(false, null);
    }

    function onUpdatedNonDeliveryVisitBasedRecords(args) {
        var rowStatusSet = false, totalDurationPaid = 0;
        var numOfNonDeliveredMsg = ' ' + ((args.currentSettings.proformaInvoiceDetail.NumberOfNonDeliveryItems > 0) ? '(' + args.currentSettings.proformaInvoiceDetail.NumberOfNonDeliveryItems.toString() + ')' : '');
        rowData = getRowData(args.currentSettings.sourceSettings.row);
        rowData.DomProformaInvoiceNonDeliveryVisitBasedItems = args.rows;
        $.each(rowData.DomProformaInvoiceNonDeliveryVisitBasedItems, function(idx, val) {
            if (val.CrudStatus != recordStatus.NotChanged && rowStatusSet == false) {
                setRowStatus(args.currentSettings.sourceSettings.row, recordStatus.Updated);
                rowStatusSet = true;
            }
            if (val.CrudStatus != recordStatus.Deleted) {
                var durationPaidHoursUnits = val.DurationPaid.getHours();
                var durationPaidMinsUnits = (val.DurationPaid.getMinutes() / 60) * 100;
                totalDurationPaid += parseFloat(durationPaidHoursUnits.toString() + '.' + durationPaidMinsUnits.toString()) * val.Occurrences;
            }
        });
        args.currentSettings.sourceSettings.control.text(((totalDurationPaid * 60) / rowData.MinutesPerUnit).toFixed(2).toString() + numOfNonDeliveredMsg);
        recalculateCost(args.currentSettings.sourceSettings.row);
    }

    function onUpdatedNonDeliveryUnitBasedRecords(args) {
        var rowStatusSet = false, totalDurationPaid = 0;
        var numOfNonDeliveredMsg = ' ' + ((args.currentSettings.proformaInvoiceDetail.NumberOfNonDeliveryItems > 0) ? '(' + args.currentSettings.proformaInvoiceDetail.NumberOfNonDeliveryItems.toString() + ')' : '');
        rowData = getRowData(args.currentSettings.sourceSettings.row);
        rowData.DomProformaInvoiceNonDeliveryUnitBasedItems = args.rows;
        $.each(rowData.DomProformaInvoiceNonDeliveryUnitBasedItems, function(idx, val) {
            if (val.CrudStatus != recordStatus.NotChanged && rowStatusSet == false) {
                setRowStatus(args.currentSettings.sourceSettings.row, recordStatus.Updated);
                rowStatusSet = true;
            }
            if (val.CrudStatus != recordStatus.Deleted) {
                if (val.UnitsPaid === undefined) {
                    val.UnitsPaid = val.Units;
                }
                totalDurationPaid += val.UnitsPaid;
            }
        });
        args.currentSettings.sourceSettings.control.text(totalDurationPaid.toFixed(2) + numOfNonDeliveredMsg);
        recalculateCost(args.currentSettings.sourceSettings.row);
    }

    function addRow(invoiceValue, insertBefore, suppressStatus) {
        if (invoiceValue) {
            insertBefore = $(insertBefore);
            var currentProforma = $settings.currentSettings.proformaInvoice;
            var currentRow = $('<tr />');
            var outputNonDeliveryCell = (currentProforma.FrameworkTypeAbbreviation === 'C' && currentProforma.ServiceOutcomeGroupCount === 0 && currentProforma.VisitCodeGroupCount === 0) ? false : true;
            if (insertBefore.length > 0) {
                currentRow.insertBefore(insertBefore);
            } else {
                currentRow.appendTo($tblInvoicesTbody);
            }
            currentRow.append($('<td>' + Date.strftime("%d/%m/%Y", invoiceValue.WeekEnding) + '</td>'));
            currentRow.append($('<td>' + invoiceValue.DomRateCategoryDescription + '</td>'));
            currentRow.append($('<td>' + invoiceValue.PlannedUnits.toString() + ' ' + invoiceValue.DomUnitsOfMeasureDescription + '</td>'));
            var tdDelivered = $('<td />').addClass($classRowDelivered);
            if ($settings.currentSettings.isEditable === false) {
                tdDelivered.html(invoiceValue.DeliveredUnits.toFixed(2));
            } else {
                var txtDelivered = $('<input type=\'text\' value=\'' + invoiceValue.DeliveredUnits.toFixed(2) + '\' title=\'Set Delivered - Valid Number\' />').css('width', '7.5em').change(function(e) {
                    var src = $(e.currentTarget), row = $(src).closest('tr'), rowData = getRowData(row);
                    var txtDeliveredVal = parseFloat(src.val());
                    if (isNaN(txtDeliveredVal) === false) {
                        rowData.DeliveredUnits = txtDeliveredVal;
                        src.val(txtDeliveredVal.toFixed(2));
                        recalculateCost(src.closest('tr'));
                        src.attr(attrInvalid, 'false');
                    } else {
                        disableSaveButton(true);
                        alert('Please enter a valid number.');
                        setTimeout(function() { src.focus().select(); }, 1);
                        src.attr(attrInvalid, 'true');
                    }
                });
                tdDelivered.append(txtDelivered);
                txtDelivered.attr({ name: invoiceValue.ID + '_txtDelivered', id: invoiceValue.ID + '_txtDelivered' });
            }
            currentRow.append(tdDelivered);
            if (outputNonDeliveryCell) {
                var outputNonDeliveryCellLink = ((currentProforma.FrameworkTypeAbbreviation != 'C') || (currentProforma.FrameworkTypeAbbreviation === 'C' && (invoiceValue.MinutesPerUnit > 0 || (invoiceValue.MinutesPerUnit == 0 && invoiceValue.HasServiceOutcomeGroupAndNotTimeBased))));
                var tdNonDelivered = $('<td title=\'View\\Edit Non Delivery of Service\' />');
                var numOfNonDeliveredMsg = ' ' + ((invoiceValue.NumberOfNonDeliveryItems > 0) ? '(' + invoiceValue.NumberOfNonDeliveryItems.toString() + ')' : '');
                if (outputNonDeliveryCellLink) {
                    tdNonDelivered.click(function(e) {
                        var src = $(e.currentTarget), row = $(src).closest('tr'), rowData = getRowData(row), isVisitBased = (rowData.MinutesPerUnit > 0);
                        if (isVisitBased) {
                            var initSettings = {
                                serviceDomProformas: $settings.serviceDomProformas,
                                paymentSchedule: $settings.paymentSchedule,
                                proformaInvoice: $settings.currentSettings.proformaInvoice,
                                onOkd: function(args) {
                                    onUpdatedNonDeliveryVisitBasedRecords(args);
                                },
                                onCancelled: null
                            };
                            var showSettings = {
                                id: rowData.ID,
                                weekEnding: rowData.WeekEnding,
                                isEditable: $settings.currentSettings.isEditable,
                                overriddenItems: rowData.DomProformaInvoiceNonDeliveryVisitBasedItems,
                                sourceSettings: { control: src, row: row },
                                proformaInvoiceDetail: rowData,
                                heading: $dlgDiv.dialog('option', 'title')
                            };
                            $(document).nonDeliveryVisitBasedProformaInvoiceLineDialog(initSettings);
                            $(document).nonDeliveryVisitBasedProformaInvoiceLineDialog('show', showSettings);
                        } else {
                            var initSettings = {
                                serviceDomProformas: $settings.serviceDomProformas,
                                paymentSchedule: $settings.paymentSchedule,
                                proformaInvoice: $settings.currentSettings.proformaInvoice,
                                onOkd: function(args) {
                                    onUpdatedNonDeliveryUnitBasedRecords(args);
                                },
                                onCancelled: null
                            };
                            var showSettings = {
                                id: rowData.ID,
                                weekEnding: rowData.WeekEnding,
                                isEditable: $settings.currentSettings.isEditable,
                                overriddenItems: rowData.DomProformaInvoiceNonDeliveryUnitBasedItems,
                                sourceSettings: { control: src, row: row },
                                proformaInvoiceDetail: rowData,
                                heading: $dlgDiv.dialog('option', 'title')
                            };
                            $(document).nonDeliveryUnitBasedProformaInvoiceLineDialog(initSettings);
                            $(document).nonDeliveryUnitBasedProformaInvoiceLineDialog('show', showSettings);
                        }
                    });
                    tdNonDelivered.addClass($classRowNotDelivered)
                }
                tdNonDelivered.text(invoiceValue.NonDeliveryUnits.toFixed(2) + numOfNonDeliveredMsg);
                currentRow.append(tdNonDelivered);
            }
            currentRow.append($('<td>&pound;' + invoiceValue.UnitCost.toFixed(2) + '</td>'));
            var tdCost = $('<td>&pound;</td>').addClass($classRowCost);
            currentRow.append(tdCost);
            if ($settings.currentSettings.isEditable === false || invoiceValue.IsRateCategoryOverridable === false) {
                tdCost.text(invoiceValue.Cost.toFixed(2));
            } else {
                var txtCost = $('<input type=\'text\' value=\'' + invoiceValue.Cost + '\' title=\'Set Cost - Valid Number\' />').css('width', '7.5em').change(function(e) {
                    var src = $(e.currentTarget);
                    var txtCostVal = src.val();
                    if (isNaN(parseFloat(txtCostVal)) === false) {
                        setRowCost(src.closest('tr'), txtCostVal);
                        src.attr(attrInvalid, 'false');
                    } else {
                        disableSaveButton(true);
                        alert('Please enter a valid number.');
                        setTimeout(function() { src.focus().select(); }, 1);
                        src.attr(attrInvalid, 'true');
                    }
                });
                tdCost.append(txtCost);
            }
            if ($settings.currentSettings.isEditable) {
                var tdDelete = $('<td class=\'deleteInvoiceLine\' title=\'Delete?\'>&nbsp;</td>').click(function(e) {
                    var src = $(e.currentTarget);
                    deleteRow(src.closest('tr'));
                });
                currentRow.append(tdDelete);
            }
            setRowData(currentRow, invoiceValue);
            setRowCost(currentRow, invoiceValue.Cost, suppressStatus);
            if ($tblInvoicesTbody.find('tr').size() == 2) {
                var widthRow = currentRow.clone().height('0px');
                widthRow.prependTo($tblInvoicesTbody);
                widthRow.find('td, th').html('');
            }
            if ($tblInvoices && $tblInvoices.height() >= 200 && $dlgDiv.find('.tablescroll').length === 0) {
                $tblInvoices.tableScroll({ height: 200, width: 855 });
            }
            return currentRow;
        }
        return null;
    }

    function addVoidRow(invoiceValue, insertBefore, suppressStatus) {
        if (invoiceValue) {
            insertBefore = $(insertBefore);
            var currentProforma = $settings.currentSettings.proformaInvoice;
            var currentRow = $('<tr />');
            var outputNonDeliveryCell = (currentProforma.FrameworkTypeAbbreviation === 'C' && currentProforma.ServiceOutcomeGroupCount === 0 && currentProforma.VisitCodeGroupCount === 0) ? false : true;
            if (insertBefore.length > 0) {
                currentRow.insertBefore(insertBefore);
            } else {
                currentRow.appendTo($tblInvoicesTbody);
            }
            currentRow.append($('<td>' + Date.strftime("%d/%m/%Y", invoiceValue.WeekEnding) + '</td>'));
            currentRow.append($('<td>' + invoiceValue.DomRateCategoryDescription + '</td>'));
            currentRow.append($('<td>' + invoiceValue.Cost.toFixed(2) + '</td>'));
           
            setRowData(currentRow, invoiceValue);
            setRowCost(currentRow, invoiceValue.Cost, suppressStatus);
            if ($tblInvoicesTbody.find('tr').size() == 2) {
                var widthRow = currentRow.clone().height('0px');
                widthRow.prependTo($tblInvoicesTbody);
                widthRow.find('td, th').html('');
            }
            if ($tblInvoices && $tblInvoices.height() >= 200 && $dlgDiv.find('.tablescroll').length === 0) {
                $tblInvoices.tableScroll({ height: 200, width: 855 });
            }
            return currentRow;
        }
        return null;
    }

    function confirmClose(src) {
        var shouldClose = true;
        var rowsToSave = getRowsForSave();
        if (rowsToSave && rowsToSave.length > 0 && atLeastOneChanged === true) {
            shouldClose = confirm('Changes made to the current proforma invoice will be lost if you close this window, do you wish to continue?') == true
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
            if (rowData.ID && rowData.ID > 0) {
                setRowStatus(row, recordStatus.Deleted);
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

    function disableSaveButton(disabled) {
        $dlgDiv.dialog('disableButton', { Text: 'Save', Disabled: disabled });
    }

    function saveChanges(src) {

        if (getInvoiceLineCount() === 0) {
            alert("Cannot save invoice until at least one detail line has been entered");
            return;
        }
        
        disableButtons(true);
        displayLoadingDiv(true, 'Saving.....');
        var rowsToSave = getRowsForSave();
        var rowsToSaveMassaged = [];
        $.each(rowsToSave, function(rowToSaveIdx, rowToSave) {
            if (rowToSave.CrudStatus !== recordStatus.NotChanged) {
                var currentRowToSave = {
                    Cost: rowToSave.Cost,
                    CrudStatus: rowToSave.CrudStatus,
                    DeliveredUnits: rowToSave.DeliveredUnits,
                    DomProformaInvoiceID: rowToSave.DomProformaInvoiceID,
                    DomRateCategoryID: rowToSave.DomRateCategoryID,
                    ID: rowToSave.ID,
                    NonDeliveryUnits: rowToSave.NonDeliveryUnits,
                    UnitCost: rowToSave.UnitCost,
                    WeekEnding: rowToSave.WeekEnding,
                    DomProformaInvoiceNonDeliveryUnitBasedItems: [],
                    DomProformaInvoiceNonDeliveryVisitBasedItems: []
                };
                if (rowToSave.DomProformaInvoiceNonDeliveryVisitBasedItems) {
                    $.each(rowToSave.DomProformaInvoiceNonDeliveryVisitBasedItems, function(visitBasedToSaveIdx, visitBasedToSave) {
                        if (visitBasedToSave.CrudStatus !== recordStatus.NotChanged) {
                            currentRowToSave.DomProformaInvoiceNonDeliveryVisitBasedItems.push({
                                CrudStatus: visitBasedToSave.CrudStatus,
                                DomProformaInvoiceID: visitBasedToSave.DomProformaInvoiceID,
                                DomProformaInvoiceDetailID: visitBasedToSave.DomProformaInvoiceDetailID,
                                DomVisitCodeID: visitBasedToSave.DomVisitCodeID,
                                ID: visitBasedToSave.ID,
                                RosteredDuration: visitBasedToSave.RosteredDuration,
                                Occurrences: visitBasedToSave.Occurrences,
                                DurationPaid: visitBasedToSave.DurationPaid,
                                StandardTimePaid: visitBasedToSave.StandardTimePaid,
                                MaximumTimePaid: visitBasedToSave.MaximumTimePaid,
                                ProviderPaid: visitBasedToSave.ProviderPaid,
                                ClientCharged: visitBasedToSave.ClientCharged,
                                Recalculate: visitBasedToSave.Recalculate
                            });
                        }
                    });
                }
                if (rowToSave.DomProformaInvoiceNonDeliveryUnitBasedItems) {
                    $.each(rowToSave.DomProformaInvoiceNonDeliveryUnitBasedItems, function(unitBasedToSaveIdx, unitBasedToSave) {
                        if (unitBasedToSave.CrudStatus !== recordStatus.NotChanged) {
                            currentRowToSave.DomProformaInvoiceNonDeliveryUnitBasedItems.push({
                                CrudStatus: unitBasedToSave.CrudStatus,
                                DomProformaInvoiceID: unitBasedToSave.DomProformaInvoiceID,
                                DomProformaInvoiceDetailID: unitBasedToSave.DomProformaInvoiceDetailID,
                                ID: unitBasedToSave.ID,
                                ServiceOutcomeID: unitBasedToSave.ServiceOutcomeID,
                                Units: unitBasedToSave.Units,
                                Paid: unitBasedToSave.Paid,
                                Recalculate: unitBasedToSave.Recalculate
                            });
                        }
                    });
                }
                rowsToSaveMassaged.push(currentRowToSave);
            }
        });
        $settings.serviceDomProformas.SaveDomProformaInvoiceDetails(rowsToSaveMassaged, saveChangesCallback);
    }

    function saveChangesCallback(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas.url)) {
            if ($.isFunction($settings.onSaved)) {
                $settings.currentSettings.savedItem = serviceResponse.value.Item;
                $settings.onSaved($settings.currentSettings);
            }
            atLeastOneChanged = false;
            $dlgDiv.dialog('close');
        }
        disableButtons(false);
        displayLoadingDiv(false, null);

       ReloadPage();
    }

    function getDomProformaInvoiceDetailsCallback(serviceResponse)
    {
        if (CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas))
        {
            if (serviceResponse.value.VoidPayment === true){
            // show void payment popup
                getVoidProformaInvoiceDetailsByDomProfomaInvoiceCallback(serviceResponse);
            }
            else{
                // show non void popup
                getDomProformaInvoiceDetailsByDomProfomaInvoiceCallback(serviceResponse);
            }
        }
    }

    function getDomProformaInvoiceDetailsByDomProfomaInvoiceCallback(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
            serviceReponse = $settings.serviceDomProformas.GetDomProformaInvoicesByPaymentSchedule($settings.paymentScheduleId, $settings.currentSettings.id);
            if (!CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
                $dlgDiv.dialog('close');
                return false;
            }
            $settings.currentSettings.proformaInvoice = serviceReponse.value.List[0];
            var currentProforma = $settings.currentSettings.proformaInvoice;
            var currentRow = null;
            var outputNonDeliveryCell = (currentProforma.FrameworkTypeAbbreviation === 'C' && currentProforma.ServiceOutcomeGroupCount === 0 && currentProforma.VisitCodeGroupCount === 0) ? false : true;
            if ($tblInvoices) {
                $tblInvoices.remove();
            }
            if ($divInvoices) {
                $divInvoices.empty();
            } else {
                $divInvoices = $('<div />').appendTo($dlgDiv);
            }
            $tblInvoices = $('<table class=\'listTable\' cellspacing=\'0\' cellpadding=\'2\' width=\'875px\' style=\'float: right;\'>').appendTo($divInvoices);
            $tblInvoices.append($('<thead><tr><th>Week Ending</th><th>Rate Category</th><th>Planned</th><th>Delivered</th>' + ((outputNonDeliveryCell) ? '<th>Non-Delivery</th>' : '') + '<th>Unit Cost</th><th>Cost</th>' + (($settings.currentSettings.isEditable) ? '<th>&nbsp;</th>' : '') + '</tr></thead>'));
            $tblInvoicesTbody = $tblInvoices.append($('<tbody />'));
            $.each(serviceResponse.value.List, function(invoiceIdx, invoiceValue) {
                addRow(invoiceValue, null, true);
            });
            if ($settings.currentSettings.isEditable) {
                $('<div style=\'clear: both;\'></div>').appendTo($divInvoices);
                $('<br />').appendTo($divInvoices);
                var $btnAdd = $('<input type=\'button\' value=\'Add\' title=\'Add New Record?\' />').css('float', 'right').appendTo($divInvoices).click(function(e) {
                    var $initSettings = {
                        rateCategories: $settings.rateCategories,
                        weekEndingDay: $settings.weekEndingDay,
                        serviceDomProformas: $settings.serviceDomProformas,
                        paymentSchedule: $settings.paymentSchedule,
                        minDate: currentProforma.PaymentScheduleDateFrom,
                        maxDate: currentProforma.PaymentScheduleDateTo,
                        onOkd: function(item) {
                            addLine(item);
                        }
                    };
                    var $showSettings = {
                        id: $settings.currentSettings.id,
                        serviceUserId: $settings.currentSettings.serviceUserId
                    };
                    $(document).nonVisitBasedProformaInvoiceLineDialog($initSettings);
                    $(document).nonVisitBasedProformaInvoiceLineDialog('show', $showSettings);
                });
            }
            var tooltipItems = [];
            var tooltipInvoker = $('<span class=\'TooltipInformation\' title=\'View Further Information?\' />');
            tooltipItems.push({ name: 'Payment Schedule', value: currentProforma.PaymentScheduleReference });
            tooltipItems.push({ name: 'Payments From', value: Date.strftime("%d/%m/%Y", currentProforma.PaymentScheduleDateFrom) });
            tooltipItems.push({ name: 'Payments To', value: Date.strftime("%d/%m/%Y", currentProforma.PaymentScheduleDateTo) });
            tooltipItems.push({ name: 'Provider', value: currentProforma.ProviderReference + ': ' + currentProforma.ProviderName });
            tooltipItems.push({ name: 'Contract', value: currentProforma.DomContractNumber + ': ' + currentProforma.DomContractTitle });
            tooltipItems.push({ name: 'Payment Ref', value: currentProforma.OurReference });
            $dlgDiv.dialog('addLeftAlignedButtonPaneControl', { control: tooltipInvoker });
            tooltipInvoker.each(function() {
                $(this).qtip({
                    content: {
                        title: 'Proforma Invoice Information',
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
        } else {
            $dlgDiv.dialog('close');
            return false;
        }
        displayLoadingDiv(false, null);
        disableSaveButton(true);
    }

    function getVoidProformaInvoiceDetailsByDomProfomaInvoiceCallback(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
            serviceReponse = $settings.serviceDomProformas.GetDomProformaInvoicesByPaymentSchedule($settings.paymentScheduleId, $settings.currentSettings.id);
            if (!CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
                $dlgDiv.dialog('close');
                return false;
            }
            $settings.currentSettings.proformaInvoice = serviceReponse.value.List[0];
            var currentProforma = $settings.currentSettings.proformaInvoice;
            var currentRow = null;
            var outputNonDeliveryCell = (currentProforma.FrameworkTypeAbbreviation === 'C' && currentProforma.ServiceOutcomeGroupCount === 0 && currentProforma.VisitCodeGroupCount === 0) ? false : true;
            if ($tblInvoices) {
                $tblInvoices.remove();
            }
            if ($divInvoices) {
                $divInvoices.empty();
            } else {
                $divInvoices = $('<div />').appendTo($dlgDiv);
            }
            $tblInvoices = $('<table class=\'listTable\' cellspacing=\'0\' cellpadding=\'2\' width=\'875px\' style=\'float: right;\'>').appendTo($divInvoices);
            $tblInvoices.append($('<thead><tr><th>Week Ending</th><th>Rate Category</th><th>Cost</th>' + (($settings.currentSettings.isEditable) ? '<th>&nbsp;</th>' : '') + '</tr></thead>'));
            $tblInvoicesTbody = $tblInvoices.append($('<tbody />'));
            $.each(serviceResponse.value.List, function(invoiceIdx, invoiceValue) {
                addVoidRow(invoiceValue, null, true);
            });
            var tooltipItems = [];
            var tooltipInvoker = $('<span class=\'TooltipInformation\' title=\'View Further Information?\' />');
            tooltipItems.push({ name: 'Payment Schedule', value: currentProforma.PaymentScheduleReference });
            tooltipItems.push({ name: 'Payments From', value: Date.strftime("%d/%m/%Y", currentProforma.PaymentScheduleDateFrom) });
            tooltipItems.push({ name: 'Payments To', value: Date.strftime("%d/%m/%Y", currentProforma.PaymentScheduleDateTo) });
            tooltipItems.push({ name: 'Provider', value: currentProforma.ProviderReference + ': ' + currentProforma.ProviderName });
            tooltipItems.push({ name: 'Contract', value: currentProforma.DomContractNumber + ': ' + currentProforma.DomContractTitle });
            tooltipItems.push({ name: 'Payment Ref', value: currentProforma.OurReference });
            $dlgDiv.dialog('addLeftAlignedButtonPaneControl', { control: tooltipInvoker });
            tooltipInvoker.each(function() {
                $(this).qtip({
                    content: {
                        title: 'Proforma Invoice Information',
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
        } else {
            $dlgDiv.dialog('close');
            return false;
        }
        displayLoadingDiv(false, null);
        disableSaveButton(true);
    }

    function getRowData(row) {
        return $(row).data($rowDataKey);
    }

    function getRowsForSave() {
        var rowsForSave = [];
        if ($tblInvoicesTbody && $tblInvoicesTbody.length > 0) {
            $.each($tblInvoicesTbody.find('tr'), function(rowIdx, rowVal) {
                rowVal = $(rowVal);
                var rowData = getRowData(rowVal);
                if (rowData) {
                    if (rowData.CrudStatus === recordStatus.Deleted || rowData.CrudStatus === recordStatus.Updated || rowData.CrudStatus === recordStatus.Inserted) {
                        rowsForSave.push(rowData);
                    }
                }
            });
        }
        return rowsForSave;
    }

    function getInvoiceLineCount() {
        var invoiceLinecount = 0;
        if ($tblInvoicesTbody && $tblInvoicesTbody.length > 0) {
            $.each($tblInvoicesTbody.find('tr'), function(rowIdx, rowVal) {
                rowVal = $(rowVal);
                var rowData = getRowData(rowVal);
                if (rowData) {
                    if (rowData.CrudStatus != recordStatus.Deleted) {
                        invoiceLinecount++;
                    }
                }
            });
        }
        return invoiceLinecount;
    }

    function recalculateCost(row) {
        var rowData = getRowData(row);
        row = $(row);
        var numDelivered = parseFloat(row.find('.' + $classRowDelivered + ' > input').val());
        var numNotDelivered = parseFloat(row.find('.' + $classRowNotDelivered).text());
        var cost = parseFloat((numDelivered + (isNaN(numNotDelivered) ? 0 : numNotDelivered)) * rowData.UnitCost);
        rowData.NonDeliveryUnits = numNotDelivered;
        setRowCost(row, RoundSpecial(cost));
    }

    function setRowData(row, data) {
        $(row).data($rowDataKey, data);
    }

    function setRowCost(row, cost, suppressStatus) {
        var rowData = getRowData(row);
        row = $(row);
        if (isNaN(cost)) {
            cost = 0;
        }
        cost = parseFloat(cost).toFixed(2);
        var cell = row.find('.' + $classRowCost);
        var cellCost = 0;
        var costShould =  RoundSpecial(rowData.PlannedUnits * rowData.PlannedUnitCost);
        var costExceeded = (toComparableCurrency(cost) > toComparableCurrency(costShould));
        var styleBold = { 'color': 'red', 'font-weight': 'bold' };
        var styleNormal = { 'color': 'black', 'font-weight': 'normal' };
        var styleToApply = (costExceeded) ? styleBold : styleNormal;
        if (cell.children().size() === 0) {
            cell.html('&pound;' + cost);
            cell.css(styleToApply);
        } else {
            cell.find('input').val(cost);
            cell.children().css(styleToApply);
        }
        if (!suppressStatus || suppressStatus === false) {
            setRowStatus(row, recordStatus.Updated);
        }
        rowData.Cost = parseFloat(cost);
    }

    function setRowStatus(row, status) {
        row = $(row);
        var rowData = getRowData(row);
        var rowStatusChanged = false;
        if (rowData) {
            if (rowData.ID === 0) {
                status = recordStatus.Inserted;
            }
            switch (status) {
                case recordStatus.Inserted:
                    if (rowData.CrudStatus != recordStatus.Deleted && rowData.CrudStatus != recordStatus.Updated && rowData.CrudStatus != recordStatus.Inserted) {
                        rowData.CrudStatus = status;
                        rowStatusChanged = true;
                    }
                    break;
                case recordStatus.Updated:
                    if (rowData.CrudStatus != recordStatus.Deleted && rowData.CrudStatus != recordStatus.Inserted && rowData.CrudStatus != recordStatus.Updated) {
                        rowData.CrudStatus = status;
                        rowStatusChanged = true;
                    }
                    break;
                case recordStatus.Deleted:
                    if (rowData.CrudStatus != recordStatus.Deleted && rowData.ID && rowData.ID > 0) {
                        rowData.CrudStatus = status;
                        rowStatusChanged = true;
                    }
                    break;
            }
        }
        if (rowStatusChanged) {
            atLeastOneChanged = true;
        }
        disableSaveButton(!(atLeastOneChanged && $tblInvoicesTbody.find('[' + attrInvalid + '=\'true\']').length == 0));
    }

    function toComparableCurrency(val) {
        if (isNaN(val)) {
            val = 0;
        }
        return parseFloat(parseFloat(val).toFixed(2));
    }

    var methods = {
        init: function(options) {
            if (!$dlgDiv) {
                if (options) {
                    $.extend($settings, options);
                }
                if (!$settings.serviceDomProformas) {
                    alert('Please specify the option \'serviceDomProformas\'');
                    return;
                }
                if (!$settings.paymentScheduleId || isNaN($settings.paymentScheduleId)) {
                    alert('Please specify the option \'paymentScheduleId\'');
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
                    title: 'Invoice Lines',
                    buttons: {
                        "Cancel": function() {
                            cancelClicked($(this));
                        }
                    },
                    onSaved: null,
                    onClosed: null
                });
            }
        },
        show: function(options) {
            if ($dlgDiv) {
                var buttons = null;
                var $showSettings = {
                    id: 0,
                    serviceUserId: 0,
                    serviceUserName: '',
                    serviceUserReference: '',
                    isEditable: 0
                };
                if (options) {
                    $.extend($showSettings, options);
                }
                if (parseInt($showSettings.serviceUserID) <= 0) {
                    alert('Please specify the option \'serviceUserID\'');
                    return;
                }
                $settings.currentSettings = $showSettings;

                if ($showSettings.isEditable == true) {
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
                    width: 900,
                    maxHeight: 315,
                    title: $showSettings.serviceUserReference + ': ' + $showSettings.serviceUserName,
                    buttons: buttons,
                    beforeClose: function(event, ui) {
                        return confirmClose($(this));
                    }
                });
                $dlgDiv.dialog('open');
                atLeastOneChanged = false;
                disableButtons(true);
                displayLoadingDiv(true);
                $settings.serviceDomProformas.GetDomProformaInvoiceDetailsByDomProfomaInvoice($showSettings.id, getDomProformaInvoiceDetailsCallback);
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);

(function($) {

    var $dlgDiv = null;
    var $settings = {
        rateCategories: null,
        weekEndingDay: 0,
        serviceDomProformas: null,
        currentSettings: null,
        paymentSchedule: null,
        onOkd: null,
        onCancelled: null,
        minDate: null,
        maxDate: null
    };
    var txtWeekEndingDate = null;
    var dropDownRateCategory = null;
    var dropDownUnitCost = null;

    $.fn.nonVisitBasedProformaInvoiceLineDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.nonVisitBasedProformaInvoiceLineDialog');
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
        if ($.isFunction($settings.onOkd)) {
            $settings.onOkd({ currentSettings: $settings.currentSettings, weekEnding: txtWeekEndingDate.val(), rateCategoryID: parseInt(dropDownRateCategory.val()), unitCost: parseFloat(dropDownUnitCost.val()) });
        }
        $dlgDiv.dialog('close');
    }

    function cancelClicked(src) {
        $dlgDiv.dialog('close');
    }

    function clearSelections() {
        txtWeekEndingDate.val('');
        dropDownRateCategory.val('');
        dropDownUnitCost.val('');
    }

    function isRateCategorySet() {
        return (dropDownRateCategory.attr("selectedIndex") > 0);
    }

    function isUnitCostSet() {
        return (dropDownUnitCost.attr("selectedIndex") > 0);
    }

    function isWeekEndingDateSet() {
        return (txtWeekEndingDate.val() !== '');
    }

    function populateUnitCosts() {
        var hasWeekEndingDate = isWeekEndingDateSet();
        var hasRateCategory = isRateCategorySet();
        dropDownUnitCost[0].options.length = 0;
        if (hasWeekEndingDate && hasRateCategory) {
            displayLoadingDiv(true, null);
            $settings.serviceDomProformas.FetchDomProviderInvoiceDetailRates($settings.paymentSchedule.DomContractID, $settings.currentSettings.serviceUserId, txtWeekEndingDate.val().toDate(), parseInt(dropDownRateCategory.val()), '', populateUnitCostsCallback);
        } else {
            setControlAvailability();
        }
    }

    function populateUnitCostsCallback(serviceResponse) {
        $('<option>').text('').val('').appendTo(dropDownUnitCost);
        if (CheckAjaxResponse(serviceResponse, $settings.serviceDomProformas)) {
            $.each(serviceResponse.value.Values, function(costKey, costValue) {
                $('<option>').text(costValue).val(costValue).appendTo(dropDownUnitCost);
            });
        }
        if (dropDownUnitCost.find('option').length === 2) {
            dropDownUnitCost.find('option:eq(1)').attr('selected', true);
        }
        displayLoadingDiv(false, null);
        setControlAvailability();
    }

    function setControlAvailability() {
        var disabled = false;
        var hasWeekEndingDate = isWeekEndingDateSet();
        var hasRateCategory = isRateCategorySet();
        var hasUnitCost = isUnitCostSet();
        dropDownRateCategory.attr('disabled', !hasWeekEndingDate);
        dropDownUnitCost.attr('disabled', (!hasWeekEndingDate || !hasRateCategory));
        if (!hasWeekEndingDate || !hasRateCategory || !hasUnitCost) {
            disabled = true;
        }
        $dlgDiv.dialog('disableButton', { Text: 'OK', Disabled: disabled });
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
                    title: 'Add Detail',
                    open: function() { 
                        txtWeekEndingDate.datepicker('enable')
                        },
                    buttons: {
                        "OK": function() {
                            okClicked($(this));
                        },
                        "Cancel": function() {
                            cancelClicked($(this));
                        }
                    }
                });
                $('<label>').text('Week Ending:').appendTo($dlgDiv);
                $('<br>').appendTo($dlgDiv);
                txtWeekEndingDate = $('<input style=\'width: 7.5em;\' type=\'text\' />').attr({ readonly: true, title: 'Select a date...' }).appendTo($dlgDiv);
                txtWeekEndingDate.datepicker({
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    showOn: 'both',
                    buttonImage: '../../../../Images/Calendar.png',
                    buttonImageOnly: true,
                    buttonText: 'Select a date...',
                    beforeShowDay: function(d) { return datePickerRestrictDays(d, [$settings.weekEndingDay]); },
                    onSelect: function() { populateUnitCosts(); },
                    minDate: $settings.minDate,
                    maxDate: $settings.maxDate
                });
                $('<br>').appendTo($dlgDiv);
                $('<br>').appendTo($dlgDiv);
                $('<label>').text('Rate Category:').appendTo($dlgDiv);
                $('<br>').appendTo($dlgDiv);
                dropDownRateCategory = $('<select style=\'width: 100%;\'>').appendTo($dlgDiv).change(function() {
                    populateUnitCosts();
                });
                $('<option>').text('').val('').appendTo(dropDownRateCategory);
                if ($settings.rateCategories) {
                    $.each($settings.rateCategories, function(rcKey, rcValue) {
                        $('<option>').text(rcValue.Description).val(rcValue.ID).appendTo(dropDownRateCategory);
                    });
                }
                $('<br>').appendTo($dlgDiv);
                $('<br>').appendTo($dlgDiv);
                $('<label>').text('Unit Cost:').appendTo($dlgDiv);
                $('<br>').appendTo($dlgDiv);
                dropDownUnitCost = $('<select style=\'width: 100%;\'>').appendTo($dlgDiv).change(function() {
                    setControlAvailability();
                });
                $('<option>').text('').val('').appendTo(dropDownUnitCost);
            }
        },
        show: function(options) {
            if ($dlgDiv) {
                displayLoadingDiv(false, '');
                disableButtons(false);
                var showSettings = {
                    id: 0,
                    serviceUserId: 0
                };
                if (options) {
                    $.extend(showSettings, options);
                }
                $settings.currentSettings = showSettings;
                clearSelections();
                setControlAvailability();
                txtWeekEndingDate.datepicker('disable');
                $dlgDiv.dialog('open');
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);

(function($) {

    var dlgDiv = null;
    var settings = {
        proformasService: null,
        contractService: null,
        currentSettings: null,
        onSaved: null,
        onCancelled: null
    };
    var txtInvoiceNote, chkTypePrivate, chkTypeViewableProviderInvoice, isCancelled = false;

    $.fn.nonVisitBasedProformaInvoiceNotesDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.nonVisitBasedProformaInvoiceNotesDialog');
            }
        });
    };

    function disableButtons(disabled) {
        dlgDiv.dialog('disableAllButtons', { Disabled: disabled });
    }

    function displayLoadingDiv(display, message) {
        dlgDiv.dialog('displayLoading', { Text: message, Display: display });
    }

    function disableSaveButton(disabled) {
        dlgDiv.dialog('disableButton', { Text: 'Save', Disabled: disabled });
    }

    function getDomProformaInvoicesByPaymentScheduleCallBack(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, settings.proformasService.url)) {
            var currentProforma = serviceResponse.value.List[0];
            txtInvoiceNote.val(currentProforma.QueryDescription);
            switch (currentProforma.QueryType) {
                case 2:
                    chkTypeViewableProviderInvoice.attr('checked', true);
                    break;
                default:
                    chkTypePrivate.attr('checked', true);
            }
        } else {
            dlgDiv.dialog('close');
            return false;
        }
        displayLoadingDiv(false, null);
        disableButtons(false);
        setControlAvailability();
    }

    function getNoteType() {
        return parseInt(dlgDiv.find("input:radio[name=noteType]:checked").val());
    }

    function saveClicked(src) {
        disableButtons(true);
        displayLoadingDiv(true, 'Saving.....');
        var noteType = 0;
        if (isCancelled === false) {
            noteType = getNoteType();
        }
        settings.contractService.ChangeProformaInvoiceQuery(settings.currentSettings.proformaInvoiceId, txtInvoiceNote.val(), noteType, saveClickedCallBack);
    }

    function saveClickedCallBack(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, settings.contractService.url)) {
            var noteType = 0;
            if (isCancelled === false) {
                noteType = getNoteType();
            }
            if ($.isFunction(settings.onSaved)) {
                settings.onSaved({ currentSettings: settings.currentSettings, note: { noteDescription: txtInvoiceNote.val(), noteType: noteType} });
            }
            dlgDiv.dialog('close');
        }
        isCancelled = false;
        disableButtons(false);
        displayLoadingDiv(false, null);
    }

    function setControlAvailability() {
        var noteDescriptionLen = txtInvoiceNote.val().replace(/\s*/g, "").length;
        var noteDescriptionLenMax = txtInvoiceNote.val().length;
        if (noteDescriptionLen === 0 || noteDescriptionLenMax > 500) {
            disableSaveButton(true);
        } else {
            disableSaveButton(false);
        }
    }

    function clearClicked(src) {
        clearSelections();
        isCancelled = true;
        saveClicked(src);
    }

    function cancelClicked(src) {
        dlgDiv.dialog('close');
    }

    function clearSelections() {
        txtInvoiceNote.val('');
        chkTypePrivate.attr('checked', true);
    }

    var methods = {
        init: function(options) {
            if (!dlgDiv) {
                if (options) {
                    $.extend(settings, options);
                }
                dlgDiv = $('<div>');
                dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999,
                    title: 'Notes',
                    width: 600,
                    buttons: {
                        "Save": function() {
                            saveClicked($(this));
                        },
                        "Clear": function() {
                            clearClicked($(this));
                        },
                        "Cancel": function() {
                            cancelClicked($(this));
                        }
                    }
                });
                $('<label>').text('Invoice Note:').appendTo(dlgDiv);
                $('<br>').appendTo(dlgDiv);
                txtInvoiceNote = $('<textarea rows=\'5\' style=\'width: 98%\' />').bind('paste keyup cut', function(src) {
                    setTimeout(setControlAvailability, 1);
                }).appendTo(dlgDiv);
                $('<br>').appendTo(dlgDiv);
                $('<br>').appendTo(dlgDiv);
                chkTypePrivate = $('<input type=\'radio\' name=\'noteType\' checked=\'true\' value=\'1\' id=\'chkNoteType1\' />').appendTo(dlgDiv);
                $('<label for=\'chkNoteType1\'>Private Note</label>').appendTo(dlgDiv);
                dlgDiv.append('  ');
                chkTypeViewableProviderInvoice = $('<input type=\'radio\' name=\'noteType\' value=\'2\' id=\'chkNoteType2\' />').appendTo(dlgDiv);
                $('<label for=\'chkNoteType2\'>Note viewable by the Council on the Provider Invoice</label>').appendTo(dlgDiv);
                setControlAvailability();
            }
        },
        show: function(options) {
            if (dlgDiv) {
                displayLoadingDiv(false, '');
                disableButtons(false);
                var showSettings = {
                    paymentScheduleId: 0,
                    proformaInvoiceId: 0
                };
                if (options) {
                    $.extend(showSettings, options);
                }
                if (showSettings.paymentScheduleId <= 0) {
                    alert('Please specify parameter \'paymentScheduleId\'');
                }
                if (showSettings.proformaInvoiceId <= 0) {
                    alert('Please specify parameter \'proformaInvoiceId\'');
                }
                settings.currentSettings = showSettings;
                clearSelections();
                isCancelled = false;
                dlgDiv.dialog('open');
                disableButtons(true);
                displayLoadingDiv(true);
                settings.proformasService.GetDomProformaInvoicesByPaymentSchedule(showSettings.paymentScheduleId, showSettings.proformaInvoiceId, getDomProformaInvoicesByPaymentScheduleCallBack);
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);

function getVisibleRows() {
    var visibleRow, visibleRows = [], visibleRowIdx = 0;
    $('#tblBody tr').each(function() {
        if (visibleRowIdx > 0) {
            visibleRow = $((this));
            if (visibleRow.css('display') != 'none') {
                visibleRows.push(visibleRow);
            }
        }
        visibleRowIdx++;
    });
    return visibleRows;
}

function getVisibleCheckedRows() {
    var visibleRow, visibleRows = [], visibleRowIdx = 0;
    $('#tblBody tr td:first-child :checkbox:checked').each(function() {
        visibleRow = $($(this).closest('tr'));
        if (visibleRow.css('display') != 'none') {
            visibleRows.push(visibleRow);
        }
        visibleRowIdx++;
    });
    return visibleRows;
}

function toggleChecked(status) {
    var visibleRows = getVisibleRows(), visibleRowsLen = visibleRows.length;
    for (visibleRowIdx = 0; visibleRowIdx < visibleRowsLen; visibleRowIdx++) {
        var cb = $(visibleRows[visibleRowIdx]).find('td:first-child :checkbox');
        if (cb.length > 0) {
            cb.attr("checked", status);
        }
    }
    tblProformas.tableFilter("refresh");
    invoicesSelected();
    // need to set this variable so that we can stop the infinit loop
    ResetButtonVisibility();
}

function invoicesSelected() {
    var visibleCheckboxes = getVisibleRows();
    var checkedCheckboxes = getVisibleCheckedRows();
    $('#lblInvoiceCountSummary').text(checkedCheckboxes.length + " Pro forma Invoices Selected")
    $('input[id*="chkAll"]').attr("checked", (visibleCheckboxes.length == checkedCheckboxes.length));
}


function changeUIForPeriodicBlock(currentContractIsPeriodicBlock) {
    if (currentContractIsPeriodicBlock === true) {
        var columnHeading = $("#periodicContract");
        columnHeading[0].innerText = "Service Dates";
        columnHeading = $("#periodicSURef");
        columnHeading[0].innerText = "System Ref";
    }

}

function ReloadPage() {
    if (hidContractBlockGuarantee.val() == "true") {
        document.location.href = document.location.href;
    }
}

///java script rounding issues. 10.5 * 9.69 gives wrong results like  .7449999999 which should be rounded to 3 decimals and then to 2 decimal places. A8258
function RoundSpecial(val){
    return parseFloat(parseFloat(val).toFixed(3)).toFixed(2);
}

function PaymentRefChanged(id, newVal) {
    UpdateReference(id,newVal);
}

function dtePaymentRefDate_Changed(id , newVal , tagid) {
    UpdateReferenceDate(tagid, newVal);
    
}

function InLineStatusChanged(id, newstatus, butt) {
    inlineStatusChangeCall = true;
    var visibleRows = getVisibleRows(), visibleRowsLen = visibleRows.length;
    for (visibleRowIdx = 0; visibleRowIdx < visibleRowsLen; visibleRowIdx++) {
        var cb = $(visibleRows[visibleRowIdx]).find('td:first-child :checkbox');
        var invoiceid = $(visibleRows[visibleRowIdx]).find('td:first-child :input[id=InvoiceId]').val();
        if (cb.length > 0 && invoiceid == id) {
            cb.attr("checked", true);
        }
    }

    if (newstatus == "UnVerify") {
        UnVerifyProforma();
    }
    else {
        VerifyProforma();
    }
}

function UnCheckAutoCheckedCheckBox(id) {
    var visibleRows = getVisibleRows(), visibleRowsLen = visibleRows.length;
        for (visibleRowIdx = 0; visibleRowIdx < visibleRowsLen; visibleRowIdx++) {
            var cb = $(visibleRows[visibleRowIdx]).find('td:first-child :checkbox');
            var invoiceid = $(visibleRows[visibleRowIdx]).find('td:first-child :input[id=InvoiceId]').val();
            if (cb.length > 0 && invoiceid == id) {
                cb.attr("checked", false);
            }
        }
 }

 function ReflectInlineStatusChange(id, newStatus) {
    var visibleRows = getVisibleRows(), visibleRowsLen = visibleRows.length;
    for (visibleRowIdx = 0; visibleRowIdx < visibleRowsLen; visibleRowIdx++) {
        var button = $(visibleRows[visibleRowIdx]).find("#InlineStatusChange");
        var invoiceid = $(visibleRows[visibleRowIdx]).find('td:first-child :input[id=InvoiceId]').val();
        if (invoiceid == id) {
            if (newStatus === "Verified") {
                button[0].value = "UnVerify";
            } else {
                button[0].value = "Verify";
             }
        }
    }
}

function UnVerifyProforma() {

    var blockGuranteemsg = "All pro forma invoices including any void payment due will be set as awaiting verification";

    if (hidContractBlockGuarantee.val() == "true") {
        toggleChecked(true);

        Ext.MessageBox.confirm('Block Guarantee contract', blockGuranteemsg, function (btn) {
            if (btn === 'yes') {
                ConfirmVerifyUnverifyAndProceed('UnVerify', verificationText, 'UnVerify');
                toggleChecked(false);
            }
        });
    }
    else {
        ConfirmVerifyUnverifyAndProceed('UnVerify', verificationText, 'UnVerify');
    }
}

function ConfirmVerifyUnverifyAndProceed(Caption, message, newstatus) {
    Ext.MessageBox.confirm(Caption, message, function (btn) {
        if (btn === 'yes') {
            ChangeProformaInvoiceStatus(newstatus);
        }
    });
}

function VerifyProforma() {

    var blockGuranteemsg = "All pro forma invoices including any void payment due will be verified";

    if (hidContractBlockGuarantee.val() == "true") {
        toggleChecked(true);

        Ext.MessageBox.confirm('Block Guarantee contract', blockGuranteemsg, function (btn) {
            if (btn === 'yes') {
                ConfirmVerifyUnverifyAndProceed('Verify', verificationText, 'Verify');
                toggleChecked(false);
            }
        });
    }
    else {
        ConfirmVerifyUnverifyAndProceed('Verify', verificationText, 'Verify');
    }

}

function UpdateReference(invoiceId, newReference) {

        var serviceResponse = contractSvc.UpdateInvoiceReference(invoiceId, 1, newReference);

        if (CheckAjaxResponse(serviceResponse, contractSvc.url)) {
        } 
}


function UpdateReferenceDate(invoiceId,  newdate) {

    var serviceResponse = contractSvc.UpdateInvoiceDate(invoiceId, 1, newdate);

            if (CheckAjaxResponse(serviceResponse, contractSvc.url)) {
        } 
}
