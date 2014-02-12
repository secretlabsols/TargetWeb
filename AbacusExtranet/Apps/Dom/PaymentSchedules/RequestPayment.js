var $tblContracts = null;
var $lblFilterCriteria = null;
var $selectedFilterCtrl; selectedFilterCtrl = null;
var payReqSvc;
var hasSetTableAsScrollable = false;

function GetSelectedFilterCtrl() {

    if (!$selectedFilterCtrl) {
        var menuItems = [{ id: -1, description: 'Clear Filter', cssClass: 'MenuItemClearFilter', tooltip: 'Clear Filtering' },
                        { description: 'Show selected items only', cssClass: 'MenuItemSelected', tooltip: 'Show selected items only' },
                        { description: 'Show un-selected items only', cssClass: 'MenuItemUnselected', tooltip: 'Show un-selected items only'}];
        $selectedFilterCtrl = $('<span />');
        $selectedFilterCtrl.addClass('Filter').attr('title', 'Filter');
        $selectedFilterCtrl.searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'BottomLeft' });
        $selectedFilterCtrl.bind('MenuItemClicked', function(src, menuItem) {
            if (menuItem.id == -1) {
                $selectedFilterCtrl.removeClass('FilterRemove');
                selectedFilterCtrl = '';
            } else {
                $selectedFilterCtrl.addClass('FilterRemove');
                if (menuItem.description == 'Show un-selected items only') {
                    selectedFilterCtrl = 'Unselected';
                } else {
                    selectedFilterCtrl = 'Selected';
                }

            }
            $(this).trigger('filterTableCellChanged', { filterTable: $tblContracts, filterValue: selectedFilterCtrl });
        });
    }
    return $selectedFilterCtrl;
}


$(document).ready(function() {
    payReqSvc = new Target.Abacus.Extranet.Apps.WebSvc.RequestPayments_class();
    $tblContracts = $('#tblContracts');
    $lblFilterCriteria = $('#lblFilterCriteria');
    var $tblContractOptions = {
        customFilterControls: [{ index: 0, control: GetSelectedFilterCtrl()}],
        onMatchingCell: function(tbl, row, rowIdx, cell, cellIdx, cellText, isMatch) {
            if (rowIdx == 0) {
                return true;
            }
            else if (cellIdx == 0) {
                if (!selectedFilterCtrl || selectedFilterCtrl == '') {
                    return true;
                } else {
                    if (selectedFilterCtrl == 'Unselected') {
                        return (cell[0].childNodes[0].checked == false);
                    } else {
                        return (cell[0].childNodes[0].checked == true);
                    }
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
                if ($.trim(cellValue.cellHeading) != '') {
                    filterCellHeading = cellValue.cellHeading;
                    filterCellValue = cellValue.currentValue;
                } else {
                    filterCellHeading = 'Contracts';
                    filterCellValue = selectedFilterCtrl;
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
            if ($("#tblContracts td:visible").find(':checkbox:checked').length == $("#tblContracts td:visible").find(':checkbox').length) {
                $('input[id*="chkAll"]').attr("checked", true);
            } else {
                $('input[id*="chkAll"]').attr("checked", false);
            }
            contractsSelected();
        }
    }
    $tblContracts.tableFilter($tblContractOptions);
    txtPayUpTo_Changed();

});

function SetContractsTableHeight() {
    var $tableScrollWrapper = $('.tablescroll_wrapper');
    if ($tableScrollWrapper.length > 0) {
        var $footerContainer = $('#footerContainer');
        var $bottom = $footerContainer.position().top + $footerContainer.outerHeight();
        var $heightOfBody = document.documentElement.clientHeight;
        var $remainingSpace = $heightOfBody - $bottom;
        var $tableFooter = $('#divFooter');
        var $lastRow = $('#tblBody').find('tr:last');
        if ($lastRow.length > 0) {
            $tableFooter.css({
                width: $tableScrollWrapper.outerWidth()
            });
        }
        $tableScrollWrapper.height($tableScrollWrapper.height() + ($remainingSpace - 2));
    }
}

function toggleChecked(status) {
    $("#tblContracts td:visible").find(':checkbox').each(function() {
        $(this).attr("checked", status);
    });

    $tblContracts.tableFilter("refresh");
    contractsSelected();
}

function btnSubmit_Click() {

    var checkedCheckboxes = $('#tblBody tr:visible td:visible :checkbox:checked');
    //Create the Payment Request First
    var $response;
    $response = payReqSvc.CreatePaymentRequest(getPayUpToDate(), 0);
    if ($response.value.ErrMsg.Success) {
        var $paymentRequestID = $response.value.PaymentRequestID;
        //loop around each visible checkbox in the table
        $.each(checkedCheckboxes, function(checkboxIdx, checkbox) {
            var parentRow = $(checkbox).closest('tr');
            var parentRowDataItem = parentRow.data('dataItem');
            $response = payReqSvc.CreatePaymentRequest_DomContract($paymentRequestID, parentRowDataItem.ContractID);
        });
    }
    // Create Job
    $response = payReqSvc.CreateJob_ProcessPaymentRequest($paymentRequestID);
    $(window.location).attr('href', '~/../RequestPaymentComplete.aspx');
}

function txtPayUpTo_Changed() {
    getContracts();
}

function getContracts() {
    DisplayLoading(true);
    payReqSvc.GetContractList(getPayUpToDate(), getContractsCallback);    
}

function getContractsCallback($response) {
    var $tblContracts = $('#tblContracts');
    $('#tblBody tr:gt(0)').remove();
    if (CheckAjaxResponse($response, payReqSvc.url)) {
        $.each($response.value.ContractList, function(key, item) {


            var row = document.createElement("tr");

            var cell = document.createElement("td");
            var cb = document.createElement('input');
            cb.type = "checkbox";
            cb.id = "checkbox";
            cell.appendChild(cb);
            row.appendChild(cell);
            $(cb).click(function() {
                contractsSelected();
            });

            //Add Hidden field
            var hid = document.createElement("input");
            hid.setAttribute("type", "hidden");
            hid.setAttribute("name", "hid_contractID_" + item.ContractID);
            hid.setAttribute("value", item.ContractID);
            cell.appendChild(hid);
            row.appendChild(cell);

            $tblContracts.children('tbody:first').append(row);

            createTableData(row, item.ProviderReference);
            $tblContracts.children('tbody:first').append(row);

            createTableData(row, item.ProviderName);
            $tblContracts.children('tbody:first').append(row);

            createTableData(row, item.ContractNumber);
            $tblContracts.children('tbody:first').append(row);

            createTableData(row, item.ContractTitle);
            $tblContracts.children('tbody:first').append(row);

            $(row).data('dataItem', item);

        });
        $tblContracts.tableFilter("refresh");
        if ($response.value.ContractList.length > 0 && !hasSetTableAsScrollable) {
            $tblContracts.tableScroll({ height: 200, width: null });
            $(window).bindWithDelay('resize', function() { SetContractsTableHeight(); }, 125);
            hasSetTableAsScrollable = true;
        }
        contractsSelected();
        SetContractsTableHeight();
    }
    DisplayLoading(false);
}

function getPayUpToDate() {
    return getPayUpToDateCtrl().val().toDate();
}

function getPayUpToDateCtrl() {
    return $('input[id*="txtPayUpTo"]');
}

function createRowContent(rowObject, cellType, data) {
    var cell = document.createElement(cellType);
    var textNode = document.createTextNode(data);
    cell.appendChild(textNode);
    rowObject.appendChild(cell);
}

function createTableData(row, data) {
    createRowContent(row, "td", data);
}
function createTableHeader(row, data) {
    createTableRow(row, data, "th");
}

function contractsSelected() {
    var visibleCheckboxes = $('#tblBody tr:visible td:visible :checkbox');
    var checkedCheckboxes = $('#tblBody tr:visible td:visible :checkbox:checked');
    $('input[id*="_btnSubmit"]').attr("disabled", (checkedCheckboxes.length <= 0));
    $('#lblContractCountSummary').text(checkedCheckboxes.length + " Contracts Selected")
    $('input[id*="chkAll"]').attr("checked", (visibleCheckboxes.length == checkedCheckboxes.length));
}










