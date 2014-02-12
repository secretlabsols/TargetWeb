/// <reference path="http://localhost:82/TargetWeb/Library/JavaScript/jQuery/UI/jquery-ui/js/jquery-1.5.1-vsdoc.js" />


var $tblDSOs = null;
var $lblFilterCriteria = null;
var selectedFilterCtrl = null; selectedFilterCtrlVal = null;
var dsoSvc, documentSvc;
var domContractID = null, establishmentID = null, dateFrom = null, dateTo = null, movement = null, showDocumentTab=false;
var $trPerfix = "dso_tr_";
var $genericServiceOrderID;
var hasSetTableAsScrollable = false;
var dtpDetailId, dtpDetails;
var dtpCostId, dtpCosts;
var $dtpDetail;
var $dtpCost;


$(document).ready(function () {
   
    dsoSvc = new Target.Abacus.Extranet.Apps.WebSvc.GenericServiceOrder_class();
    documentSvc = new Target.Abacus.Extranet.Apps.WebSvc.Documents_class();
    $tblDSOs = $('#tblDSOs');
    $lblFilterCriteria = $('#lblFilterCriteria');
    var $genericServiceOrderID = 0;
    var $genericServiceCurrentDate;
    var $currentDate = new Date();
    $dtpDetail = $("[id$='detailWeekEndingDate_txtTextBox']");
    $dtpCost = $("[id$='costWeekEndingDate_txtTextBox']");
    dtpDetails = GetElement(dtpDetailId + '_txtTextBox');
    dtpCosts = GetElement(dtpCostId + '_txtTextBox');
    var $minDate, $maxDate;

    var $tblContractOptions = {
        filterDelay: 0,
        customFilterControls: [{ index: 5, control: GetSelectedFilterCtrl()}],
        onMatchingCell: function (tbl, row, rowIdx, cell, cellIdx, cellText, isMatch) {
            if (cellIdx == 5) {
                if (!selectedFilterCtrlVal || selectedFilterCtrlVal == '') {
                    return true;
                } else {
                    return (cell.hasClass(selectedFilterCtrlVal));
                }
            } else {
                return isMatch;
            }
        },
        onFilterCompleted: function (tbl, filterCells, filterCount) {
            var filterStr = '<b>Filtered By:</b> ';
            var filterParamsStr = '';
            var filterCellHeading = '';
            var filterCellValue = '';
            $.each(filterCells, function (cellKey, cellValue) {
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
    $tblDSOs.tableFilter($tblContractOptions);

    PopulateResults();

    $('tbody tr').click(function () {

        $genericServiceOrderID = $(this).attr('id').replace($trPerfix, '');
        var $response;

        //Mark Service Order as being read
        $response = dsoSvc.MarkServiceOrderAsRead($genericServiceOrderID);
        if ($response.value.Success == true) {
            //Successfully updated read flag
            $(this).children("td:nth-child(6)").attr('class', 'Read');

        }
        // service order date 
        var $genericServiceOrderDateTo = Date.toDateFromString($(this).children("td:nth-child(5)").html());
        var $genericServiceOrderDateFrom = Date.toDateFromString($(this).children("td:nth-child(4)").html());

        $currentDate = new Date();
        // closed service order
        if ($currentDate > $genericServiceOrderDateFrom && $currentDate > $genericServiceOrderDateTo)
            $genericServiceCurrentDate = $genericServiceOrderDateTo;

        // currently open service order
        if ($currentDate > $genericServiceOrderDateFrom && $currentDate < $genericServiceOrderDateTo)
            $genericServiceCurrentDate = new Date();

        // future service Order
        if ($currentDate < $genericServiceOrderDateFrom && $currentDate < $genericServiceOrderDateTo)
            $currentDate = $genericServiceOrderDateFrom;


        // min Max Date 
        $minDate = GetWeekEndingDate(dsoSvc, $genericServiceOrderDateFrom);
        $maxDate = GetWeekEndingDate(dsoSvc, $genericServiceOrderDateTo);
        // to select weekending date
        var Weendending = GetWeekEndingDate(dsoSvc, $currentDate);

        $dtpDetail.datepicker('option', 'minDate', $minDate);
        $dtpDetail.datepicker('option', 'maxDate', $maxDate);
        $dtpCost.datepicker('option', 'minDate', $minDate);
        $dtpCost.datepicker('option', 'maxDate', $maxDate);
        // the Service Order ends before the current system date the filter date is, by default, 
        // set equal to the end of the week in which the Service Order ends
        if ($maxDate < Weendending)
            $dtpDetail.datepicker('setDate', $maxDate);
        else
            $dtpDetail.datepicker('setDate', Weendending);
        //If the Service Order starts after the current system date the filter date is, 
        // by default, set equal to the end of the week in which the Service Order starts
        if ($minDate > Weendending)
            $dtpDetail.datepicker('setDate', $minDate);
        $dtpCost.datepicker('setDate', Weendending);
        // by default , pass current date and set 0 index tab to display
        ShowDso($genericServiceOrderID, $currentDate, 0);
    });

    $('input[id*="btnAdd"]').button();

});

// catch change event of detail weekendign date
function detailWeekEndingDate_Changed() {
    var selectedDate = $dtpDetail.datepicker('getDate');
    $dtpCost.datepicker('setDate', selectedDate);
    ShowDso($genericServiceOrderID, selectedDate, 1);

}

function costWeekEndingDate_Changed() {
    var selectedDate = $dtpCost.datepicker('getDate');
    $dtpDetail.datepicker('setDate', selectedDate);
    ShowDso($genericServiceOrderID, selectedDate, 3);
}

function GetSelectedFilterCtrl() {
   
    if (!selectedFilterCtrl) {
        var classFilterRemove = 'FilterRemove';
        var menuItems = [{ id: -1, description: 'Clear Filter', cssClass: 'MenuItemClearFilter', tooltip: 'Clear Filtering' },
                        { description: 'Unread', cssClass: 'MenuItemUnread', tooltip: 'Filter by Unread' },
                        { description: 'Read', cssClass: 'MenuItemRead', tooltip: 'Filter by Read' }];
        selectedFilterCtrl = $('<span />');
        selectedFilterCtrl.addClass('Filter').attr('title', 'Filter');
        selectedFilterCtrl.searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'BottomRight', showSearchBox: false, width: 100 });
        selectedFilterCtrl.bind('MenuItemClicked', function(src, menuItem) {
            if (menuItem.id === -1) {
                selectedFilterCtrl.removeClass(classFilterRemove);
                selectedFilterCtrlVal = '';
            } else {
            selectedFilterCtrl.addClass(classFilterRemove);
                selectedFilterCtrlVal = menuItem.description;
            }
            $(this).trigger('filterTableCellChanged', { filterTable: $tblDSOs, filterValue: selectedFilterCtrlVal });
        });
    }
    return selectedFilterCtrl;
}

function PopulateResults() {

    $('#tblDSOs tr:gt(1)').remove();

    var $response = dsoSvc.FetchServiceOrderList(establishmentID, domContractID, dateFrom.toDate(), dateTo.toDate(), movement);
    if (!CheckAjaxResponse($response, dsoSvc.url)) {
        return false;
    }
    $.each($response.value.dsoList, function(key, item) {

        var row = document.createElement("tr");
        row.setAttribute("id", $trPerfix + item.ID);

        var cell = document.createElement("td");
        $tblDSOs.children('tbody:first').append(row);

        createTableData(row, item.ServiceUserRef, "10%");

        $(row).append($('<td width=\57%\'><a>' + item.ServiceUserName + '</a></td>'));

        createTableData(row, item.OrderRef, "10%");

        createTableData(row, Date.strftime("%d/%m/%Y", item.DateFrom), "10%");

        createTableData(row, Date.strftime("%d/%m/%Y", item.DateTo), "10%");

        var cell = document.createElement('td');
        cell.setAttribute("width", "3%");
        var textNode = document.createTextNode(' ');
        cell.appendChild(textNode);

        if (item.ExtranetStatus == 1) {
            cell.className = 'Read';
        } else {
            cell.className = 'Unread';
        }

        row.appendChild(cell);

    });
    $tblDSOs.tableFilter("refresh");
    if ($response.value.dsoList.length > 0 && !hasSetTableAsScrollable) {
        var $footerContainer = $('#footerContainer');
        var $bottom = $footerContainer.position().top + $footerContainer.outerHeight();
        var $heightOfBody = document.documentElement.clientHeight;
        var $remainingSpace = ($tblDSOs.outerHeight() - $footerContainer.outerHeight()) + ($heightOfBody - $bottom);
        $tblDSOs.tableScroll({ height: $remainingSpace, width: null });
        hasSetTableAsScrollable = true;
    }
}

function createTableData(row, data) {
    createRowContent(row, "td", data);
}

function createTableData(row, data, widthPercent) {
    createRowContent(row, "td", data,widthPercent);
}

function createRowContent(rowObject, cellType, data) {
    var cell = document.createElement(cellType);
    var textNode = document.createTextNode(data);
    cell.appendChild(textNode);
    rowObject.appendChild(cell);
    
}

function createRowContent(rowObject, cellType, data, widthPercent) {
    var cell = document.createElement(cellType);
    cell.setAttribute("width", widthPercent);
    var textNode = document.createTextNode(data);
    cell.appendChild(textNode);
    rowObject.appendChild(cell);
}

function ShowDso(id, filterEeekEndingDate, selectedTab) {
    $genericServiceOrderID = id.replace($trPerfix, '');
    var $response;
    var $initSettings = {
        genericServiceOrderID: $genericServiceOrderID,
        dialogueDivID: 'dso_dialog',
        genericServiceOrderSvc: dsoSvc,
        showDocuments: showDocumentTab,
        filterEeekEndingDate: filterEeekEndingDate,
        selectedTab: selectedTab
    }

    //Show dialogue
    $(document).serviceOrderDialog($initSettings);
    $(document).serviceOrderDialog('show', $initSettings);
    
}


(function($) {

    var $dlgDiv = null;
    var $domServiceOrderID = null;
    var $dlgDivLoading = null;
    var $dropDownDayOfWeek = null;
    var $dropDownServiceOutcome = null;
    var $keyDay = 'day';
    var $settings = {
        genericServiceOrderID: 0,
        dialogueDivID: null,
        genericServiceOrderSvc: null,
        showDocuments: false,
        filterEeekEndingDate: null,
        selectedTab :0
    };
    var $classUiDisabled = 'ui-state-disabled';

    $.fn.serviceOrderDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.serviceOrderDialog');
            }
        });
    };

    function displayLoadingDiv(display, message) {
        $dlgDiv.dialog('displayLoading', { Text: message, Display: display });
    }

    function okClicked(src) {
        $dlgDiv.dialog('close');
        $dlgDiv = null
    }


    var methods = {
        init: function(options) {

            if (!$dlgDiv) {
                if (options) {
                    $.extend($settings, options);
                }
                $dlgDiv = $('#' + $settings.dialogueDivID) //$('<div>');
                $dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    minWidth: 950,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999,
                    title: 'Service Order',
                    buttons: {
                        "OK": function() {
                            okClicked($(this));
                        }
                    }
                });
                $('#tabs').tabs();

            }

        },
        show: function(options) {
            var $orderHeader = null;
            var $orderDetail = null;
            var $orderSuspensions = null;
            var $orderCosts = null;
            var $response = null;
            var $tabs = null;

            if ($dlgDiv) {
                displayLoadingDiv(false, '');


                //Clear Contents
                $('#tab-Order').empty();
                $('#tab-Details-Content').empty();
                $('#tblDSOSuspensionBody').empty();
                $('#tblDSOCostBody').empty();



                //Select the first tab.
                $tabs = $('#tabs').tabs();
                $tabs.tabs('select', options.selectedTab);

                //If the user doesnt have permission to view documents remove the documents tab
                if (options.showDocuments == false) {
                    $tabs.tabs("remove", 4);
                }

                $response = $settings.genericServiceOrderSvc.FetchServiceOrder(options.genericServiceOrderID)
                if ($response.value.ErrMsg.Success) {
                    $orderHeader = $response.value.dso;
                    $domServiceOrderID = $orderHeader.ChildID;
                    dsoDetail_PopulateControl($domServiceOrderID, true);
                    $("#orderTemplate").tmpl($orderHeader).appendTo("#tab-Order");

                }
                $response = $settings.genericServiceOrderSvc.FetchServiceOrderDetailList(options.genericServiceOrderID, options.filterEeekEndingDate);
                if (!CheckAjaxResponse($response, $settings.genericServiceOrderSvc.url)) {
                    return false;
                }
                $orderDetail = $response.value.dsodList
                if ($orderHeader.FrameworkType != "Service Register") {
                    $("#OrderDetailTemplate").tmpl($orderHeader).appendTo("#tab-Details-Content");
                    $("#OrderDetailTemplateBody").tmpl($orderDetail).appendTo("#tblDSODetailBody");
                }
                if ($orderHeader.FrameworkType == "Service Register") {
                    $("#OrderDetailSvcRegistersTemplate").tmpl($orderHeader).appendTo("#tab-Details-Content");
                    $("#OrderDetailSvcRegistersTemplateBody").tmpl($orderDetail).appendTo("#tblDSODetailBody");
                }

                $response = $settings.genericServiceOrderSvc.FetchServiceOrderSuspensionList($domServiceOrderID);
                if (!CheckAjaxResponse($response, $settings.genericServiceOrderSvc.url)) {
                    return false;
                }
                $orderSuspensions = $response.value.suspensionList;
                $("#suspensionRowsTemplate").tmpl($orderSuspensions).appendTo("#tblDSOSuspensionBody");

                $response = $settings.genericServiceOrderSvc.FetchServiceOrderCostList($domServiceOrderID, options.filterEeekEndingDate)
                if (!CheckAjaxResponse($response, $settings.genericServiceOrderSvc.url)) {
                    return false;
                }
                $orderCosts = $response.value.costList;
                $("#costRowsTemplate").tmpl($orderCosts).appendTo("#tblDSOCostBody");

                var $overallCost = 0;
                var rows = $("#tblDSOCosts tr:gt(0)");
                rows.children("td:nth-child(5)").each(function() {
                    $overallCost += parseFloat($(this).html().replace('£', ''));
                });

                $overallCost = $overallCost.toString();
                $("#lblOverallCostValue").empty();
                $("#lblOverallCostValue").append($overallCost.formatCurrency());

                var today = new Date()
               // $("#lblRatesInfo").empty();
                //$("#lblRatesInfo").append("Standard rates shown above are those in effect on " + formatDate(today));


                if (options.showDocuments == true) {
                    FetchDocumentList(1, options.genericServiceOrderID);
                };


                $dlgDiv.dialog('open');
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);

function formatDate(dtDate) {
    if (Date.strftime("%d/%m/%Y", dtDate) == '31/12/9999') {
        return '(open ended)'
    } else {
        return Date.strftime("%d/%m/%Y", dtDate)
    }
}

function formatCurrency(strValue) {
    var value = strValue.toString();
    return value.formatCurrency()
}


function FetchDocumentList(page,  genericServiceOrderID) {
    var totalRecordCount;
    var serviceUserType = 4; //This is association to Service Order
    var $Documents;

    $("#tblDSODocumentBody").empty();

    $response = documentSvc.FetchDocumentList(page, 10, totalRecordCount, serviceUserType, genericServiceOrderID)
    if (!CheckAjaxResponse($response, documentSvc.url)) {
        return false;
    }
    $documents = $response.value.Documents;
    $("#documentRowsTemplate").tmpl($documents).appendTo("#tblDSODocumentBody");
    $("#Document_PagingLinks").html($response.value.PagingLinks);
}

function FetchDocumentListFromAdd() {
    FetchDocumentList(1, $genericServiceOrderID);
}


function GenericServiceOrderSelector_btnAdd_Click() {

    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/Documents/DocumentAdd.aspx?autopopup=1&gsoID=" + $genericServiceOrderID;

    if (url) OpenDialog(url, 47, 20, window);
    //if (url) OpenPopup(url, 47, 30, window);
}

function GetWeekEndingDate(svc,date){
    
   var $response = svc.GetWeekendingDate(date);
                if (!CheckAjaxResponse($response, svc.url)) {
                    return false;
                }
    return $response.value.WeekEndingDate;
}

function Init() {
   
}


addEvent(window, "load", Init);
