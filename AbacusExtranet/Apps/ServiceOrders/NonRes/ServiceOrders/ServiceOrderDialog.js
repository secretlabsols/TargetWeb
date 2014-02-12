(function ($) {

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
        selectedTab: 0
    };
    var $classUiDisabled = 'ui-state-disabled';
   
    $.fn.serviceOrderDialog = function (method, options) {
        return this.each(function () {
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
        init: function (options) {

            if (!$dlgDiv) {
                if (options) {
                    $.extend($settings, options);
                }
                $dlgDiv = $('#' + $settings.dialogueDivID) //$('<div>');
                $dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    minWidth: 980,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999,
                    title: 'Service Order',
                    buttons: {
                        "OK": function () {
                            okClicked($(this));
                        }
                    }
                });
                $('#tabs').tabs();

            }

        },
        show: function (options) {
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
                rows.children("td:nth-child(5)").each(function () {
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


function FetchDocumentList(page, genericServiceOrderID) {
    var totalRecordCount;
    var serviceUserType = 4; //This is association to Service Order
    var $Documents;
    var documentSvc = new Target.Abacus.Extranet.Apps.WebSvc.Documents_class();

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

function GetWeekEndingDate(svc, date) {

    var $response = svc.GetWeekendingDate(date);
    if (!CheckAjaxResponse($response, svc.url)) {
        return false;
    }
    return $response.value.WeekEndingDate;
}
