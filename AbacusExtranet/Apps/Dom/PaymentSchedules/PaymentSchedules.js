var InPlaceContractSelectorID, InPlaceProviderSelectorID, pnlUnProcessedVisitamendmentRequest, InPlaceProviderSelectorID_SelectedID;
var txtProviderName, txtProviderReference, txtContractName, txtContractReference, btnContractFind;
var EnableDisableContractSelector;
var SelectedProviderId, SelectedContractId;
var txtProviderId, txtContractId, txtReferenceId, hidPaymentScheduleId;
var txtProvider, txtContract, txtReference, hidPaymentSchedule;
var chkUnpaidId, chkSuspendedId, chkAuthorisedId, chkPaidId;
var chkUnpaid, chkSuspended, chkAuthorised, chkPaid, chkVisitBased, chkVisitBasedId, hidVisitBased;
var chkAmendUnVerifiedId, chkAmendVerifiedId;
var chkAmendUnVerified, chkAmendVerified;
var chkAwaitingId, chkVerifiedId;
var chkAwaiting, chkVerified;
var OriginalValueChangedId;
var OriginalValueChanged;
var RequestModeId;
var RequestMode;
var dtePaymentToId, dtePaymentTo, dtePaymentFromId, dtePaymentFrom;
var PaymetnScheduleTitle;
var ToolTipProvider, ToolTipContract, ToolTipPaymentFrom, ToolTipPaymentTo, ToolTipContractID;
var btnAddNonVisitBasedInvoices, btnAddProformaInvoice, paymentScheduleSvc, requestPaymentsSvc;
var $tblInvoices = null;
var $lblFilterCriteria = null;
var $selectedFilterCtrl; selectedFilterCtrl = null;
var InvoiceListRequestvc;
var pScheduleId, PermitEditReference;
var $paymentReferenceStatusFilterCtrl;
var paymentReferenceStatusFilterCtrlVal = null;
var isVisitBased = false;
var MaxNoWeeksAllowed = 0;
var DateDiffInWeeks = 0;
var UserHasVisitBasedAddCommand = false;
var UserHasNonVisitBasedAddCommand = false;


function Init() {
    InvoiceListRequestvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
    setControls();
    btnAddProformaInvoice.hide();
    btnAddNonVisitBasedInvoices.hide();
    chkVisitBased.attr('disabled', 'disabled');
    setControlVisibilityByVisitBased();
    paymentScheduleSvc = new Target.Abacus.Extranet.Apps.WebSvc.PaymentSchedule_class();
    requestPaymentsSvc = new Target.Abacus.Extranet.Apps.WebSvc.RequestPayments_class();
    var dlgDiv = $('#invoiceRef');
    dlgDiv.dialog({
        autoOpen: false,
        draggable: true,
        modal: true,
        resizable: false,
        closeOnEscape: true,
        zIndex: 9999,
        minHeight: 400,
        minWidth: 800,
        title: PaymetnScheduleTitle,
        buttons: {
            "Close": function() {
                $(this).dialog('close');
            }
        },
        beforeClose: function(event, ui) {
            return closeReferencesDialog($(this));
        }
    });

    // Adding Toolt ip information
    var tooltipItems = [];
    var tooltipInvoker = $('<span class=\'TooltipInformation\' title=\'View Further Information?\' />');
    tooltipItems.push({ name: 'Provider', value: ToolTipProvider });
    tooltipItems.push({ name: 'Contract', value: ToolTipContract });
    tooltipItems.push({ name: 'Period From', value: ToolTipPaymentFrom });
    tooltipItems.push({ name: 'Period To', value: ToolTipPaymentTo });
    dlgDiv.dialog('addLeftAlignedButtonPaneControl', { control: tooltipInvoker });
    tooltipInvoker.each(function() {
        $(this).qtip({
            content: {
                title: 'Payment Schedule Information',
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
    if (RequestMode.value != '1') {
        EnableDisableContract(!(parseInt(txtProvider.value) > 0));
    }
    chkVisitBased.click(function() {
        setControlVisibilityByVisitBased();
    });

    enableDisablePaymentFrom();
}



function ProformaInvoiceEntry(createNew) {
    if (createNew) {
        document.location.href = SITE_VIRTUAL_ROOT +
       "AbacusExtranet/Apps/Dom/PaymentSchedules/clientSelector.aspx?&estabid=" + txtProvider.value +
       "&contractid=" + txtContract.value +
       "&pScheduleId=" + hidPaymentSchedule.value +
       "&pScheduleRef=" + txtReference.value +
       "&pSWE=" + dtePaymentTo.value +
       "&backUrl=" + GetBackUrl();
    }
    else {
        document.location.href = SITE_VIRTUAL_ROOT +
       "AbacusExtranet/Apps/Dom/ProformaInvoice/ManualEnquiry.aspx?=null&estabid=" + txtProvider.value +
       "&contractid=" + txtContract.value +
       "&pScheduleId=" + hidPaymentSchedule.value +
       "&pScheduleRef=" + txtReference.value +
       "&pSWE=" + dtePaymentTo.value +
       "&backUrl=" + GetBackUrl();
    }
}

function btnProformaInvoiceList_Click() {
    var baseUrl = SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/Dom/ProformaInvoice/";
    var id = hidPaymentSchedule.value;
    chkAwaiting = GetElement(chkAwaitingId);
    chkVerified = GetElement(chkVerifiedId);

    document.location.href = SITE_VIRTUAL_ROOT +
    "AbacusExtranet/Apps/Dom/ProformaInvoice/ViewInvoices.aspx?=null&pScheduleId=" +
    hidPaymentSchedule.value +
    "&id=" + hidPaymentSchedule.value +
    "&await=" + chkAwaiting.checked +
    "&ver=" + chkVerified.checked +
    "&backUrl=" + GetBackUrl();
    if (chkVisitBased.is(':checked') === true) {
        baseUrl += "ViewInvoices.aspx"
    }
    else {
        if (chkAwaiting.checked === false && chkVerified.checked === false) {
            alert('Please select at least one checkbox from the \'Unprocessed Pro forma Invoices\' section.');
            return false;
        }
        baseUrl += "ViewNonVisitInvoices.aspx"
    }
    document.location.href = baseUrl +
                            "?=null&pScheduleId=" + id +
                            "&id=" + id +
                            "&await=" + chkAwaiting.checked +
                            "&ver=" + chkVerified.checked +
                            "&pSWE=" + dtePaymentTo.value +
                            "&backUrl=" + GetBackUrl();
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function btnBack_Click() {
    var change = "true";
    if (OriginalValueChanged.value == "true") {
        change = confirm("Changes have been made to the Payment Schedule during the current editing session. These changes will be lost if you continue with this action. Please confirm that you understand this and that you wish to continue regardless.")
    }
    if (change) {
        var url = GetQSParam(document.location.search, "backUrl");
        url = unescape(url);
        document.location.href = url;
    }
}

function btnBackResultStep_Click(url) {
    document.location.href = url;
}

function btnCancel_Click() {
    if (OriginalValueChanged.value == "true") {
        submit = confirm("Changes have been made to the Payment Schedule during the current editing session. These changes will be lost if you continue with this action. Please confirm that you understand this and that you wish to continue regardless.")
    }
    return submit;
}

function btnDelete_Click() {
    var submit = "true";
    submit = confirm("The Payment Schedule and all associated Pro forma Invoices will be deleted. Please confirm that you understand this and that you wish to continue regardless.")
    return submit;
}

function btnProviderInvoiceList_Click() {

    chkUnpaid = GetElement(chkUnpaidId);
    chkSuspended = GetElement(chkSuspendedId);
    chkAuthorised = GetElement(chkAuthorisedId);
    chkPaid = GetElement(chkPaidId);

    document.location.href = SITE_VIRTUAL_ROOT +
        "AbacusExtranet/Apps/Dom/ProviderInvoice/DomProviderInvoiceList.aspx?=null&pScheduleId=" +
        hidPaymentSchedule.value +
        "&id=" + hidPaymentSchedule.value +
        "&unpaid=" + chkUnpaid.checked +
        "&sus=" + chkSuspended.checked +
        "&auth=" + chkAuthorised.checked +
        "&paid=" + chkPaid.checked +
        "&dtfrom=" + null +
        "&dtto=" + null +
        "&backUrl=" + GetBackUrl();
}

function btnVisitAmendmentList_Click() {

    chkAmendUnVerified = GetElement(chkAmendUnVerifiedId);
    chkAmendVerified = GetElement(chkAmendVerifiedId);

    document.location.href = SITE_VIRTUAL_ROOT +
        "AbacusExtranet/Apps/Dom/ProviderInvoice/VisitAmendmentRequestList.aspx?=null&pScheduleId=" +
        hidPaymentSchedule.value +
        "&id=" + hidPaymentSchedule.value +
        "&await=" + chkAmendUnVerified.checked +
        "&ver=" + chkAmendVerified.checked +
        "&backUrl=" + GetBackUrl();
}

function txtReference_Changed() {
    OnOriginalValueChange();
}

function dtePaymentFrom_Changed(id) {
    var dateFromCtrl = $('#' + dtePaymentFromId + '_txtTextBox');
    var dateToCtrl = $('#' + dtePaymentToId + '_txtTextBox');
    var dateToDate = dateFromCtrl.val().toDate();
    dateToDate.setDate(dateToDate.getDate() + 6);
    //dateToCtrl.datepicker('option', { minDate: null, maxDate: null });
    dateToCtrl.datepicker('option', 'minDate', dateToDate);

    setVisitBasecheckBox();
    if (isVisitBased) {
        dateToCtrl.datepicker('option', 'maxDate', dateToDate);
        dateToCtrl.datepicker("setDate", new Date(dateFromCtrl.val().toDate()) + 6);
    } else {
        var dateToMax = dateFromCtrl.val().toDate();
        dateToMax.setDate(dateToMax.getDate() + (MaxNoWeeksAllowed * 7));
        dateToCtrl.datepicker("option", "maxDate", dateToMax);
        //dateToCtrl.datepicker({ defaultDate: dateToDate });
        if (dateToCtrl.datepicker("getDate") != null && DateDiffInWeeks > 0) {
            var resetToDate = dateFromCtrl.val().toDate();
            resetToDate.setDate(resetToDate.getDate() + DateDiffInWeeks);
            dateToCtrl.datepicker("setDate", resetToDate);
        }
        else {
            dateToCtrl.datepicker("setDate", dateToDate);
        }
    }

    enableDisablePaymentTo();
    OnOriginalValueChange();
}

function dtePaymentTo_Changed(id) {
    OnOriginalValueChange();
    var dateFromCtrl = $('#' + dtePaymentFromId + '_txtTextBox');
    var dateToCtrl = $('#' + dtePaymentToId + '_txtTextBox');
    if (dateToCtrl.datepicker("getDate") != null) {
        DateDiffInWeeks = new Date(dateToCtrl.val().toDate() - dateFromCtrl.val().toDate())
        DateDiffInWeeks = DateDiffInWeeks / 1000 / 60 / 60 / 24;
    }
}

function setVisitBasecheckBox() {
    var contractId, providerId, dateFromCtrl, dateToCtrl;
    var btnsave = GetElement("ctl00_MPContent_stdButtons1_btnSave");
    var chk = GetElement(chkVisitBasedId);

    contractId = SelectedContractId;
    providerId = SelectedProviderId;
    dateFromCtrl = $('#' + dtePaymentFromId + '_txtTextBox');
    dateToCtrl = $('#' + dtePaymentToId + '_txtTextBox');

    if (contractId != undefined && providerId != undefined && dateFromCtrl.val().length > 0) {

        var response = paymentScheduleSvc.IsVisitBased(providerId, contractId, dateFromCtrl.val().toDate());
        //alert(response);
        if (response.value.msg.Success) {
            if (response.value.VisitBased == -1) {
                chk.checked = true;
                btnsave.disabled = false;
            }
            else if (response.value.VisitBased == 0) {
                chk.checked = false;
                btnsave.disabled = false;
            } else if (response.value.VisitBased == -2) {
                chk.checked = false;
                alert("Creation of Payment Schedules for this Contract/Period is not permitted");
                btnsave.disabled = true;
            }

        }
        isVisitBased = chkVisitBased.is(':checked');
        hidVisitBased.val(chkVisitBased.is(':checked'));
    }
}

function enableDisablePaymentFrom() {

    var dateFromCtrl = $('#' + dtePaymentFromId + '_txtTextBox');
    var dateToCtrl = $('#' + dtePaymentToId + '_txtTextBox');

    if (!(dateFromCtrl.val().length > 0 && dateToCtrl.val().length > 0)) {
        if (SelectedContractId > 0) {
            dateFromCtrl.datepicker('enable');
            ShowImage();
            enableDisablePaymentTo();
        }
        else {
            dateFromCtrl.datepicker('disable');
            HideImage();
            enableDisablePaymentTo();
        }

        var dfrom = $('#' + dtePaymentFrom.id);
        dfrom[0].value = "";

        var dTo = $('#' + dtePaymentTo.id);
        dTo[0].value = "";
    }
}

function enableDisablePaymentTo() {
    var dateFromCtrl = $('#' + dtePaymentFromId + '_txtTextBox');
    var dateToCtrl = $('#' + dtePaymentToId + '_txtTextBox');

    if (dtePaymentFrom.value.length > 0) {
        dateToCtrl.datepicker('enable');
    }
    else {
        dateToCtrl.datepicker('disable');
    }
}

function HideImage() {
    var imglist;
    $('img.ui-datepicker-trigger').filter(function() {
        $(this).hide();
    });
}

function ShowImage() {
    var imglist;
    $('img.ui-datepicker-trigger').filter(function() {
        $(this).show();
    });
}

function EnableDisableContract(disabled) {
    if (parseInt(hidPaymentSchedule.value) > 0) {
        disabled = !InPlaceEstablishmentSelector_IsEnabled(InPlaceProviderSelectorID); ;
    }
    InPlaceDomContractSelector_Disable(InPlaceContractSelectorID, disabled);
}

function InPlaceEstablishment_Changed(providerId) {
    var hasChanged = true;
    if (providerId > 0) {
        hasChanged = (providerId != txtProvider.value);
        SelectedProviderId = providerId;
        txtProvider.value = providerId;
        OnOriginalValueChange();
        EnableDisableContract(false);
    } else {
        InPlaceDomContractSelector_Disable(InPlaceContractSelectorID, true);
    }
    if (hasChanged) {
        InPlaceDomContractSelector_ClearStoredID(InPlaceContractSelectorID);
    }
}

function InPlaceContract_Changed(contractId, reference, name, frameworkTypeAbbr) {
    setControls();
    var psId = parseInt(hidPaymentSchedule.value);
    if (isNaN(psId) || psId <= 0) {
        if (contractId > 0) {
            SelectedContractId = contractId;
            txtContract.value = contractId;
            OnOriginalValueChange();
            enableDisablePaymentFrom();
        }
        //        if (frameworkTypeAbbr && frameworkTypeAbbr === 'V') {
        //            chkVisitBased.attr({ checked: true, disabled: false });            
        //        } else {
        //            chkVisitBased.attr({ checked: false, disabled: true });
        //        }
        setControlVisibilityByVisitBased();
        setVisitBasecheckBox();
    }
}

function setControlVisibilityByVisitBased() {

    var visitBased = chkVisitBased.is(':checked');

//    alert("UserHasVisitBasedAddCommand  " + UserHasVisitBasedAddCommand + "\n" +
//           "UserHasNonVisitBasedAddCommand " + UserHasNonVisitBasedAddCommand + "\n" +
//           "check box " + visitBased);

    if (visitBased == true && UserHasVisitBasedAddCommand == "true") {
//        alert('visitbase dikha de');
        btnAddProformaInvoice.show();
        pnlUnProcessedVisitamendmentRequest.show();
    }
    else if (visitBased == false && UserHasNonVisitBasedAddCommand == "true") {
//    alert("non visit based dikha de");
        btnAddNonVisitBasedInvoices.show();
        btnAddNonVisitBasedInvoices.click(function() {
            AddNonVisitBasedInvoices();
        });
        pnlUnProcessedVisitamendmentRequest.hide();
    }
    hidVisitBased.val(chkVisitBased.is(':checked'));
}

function txtTotalValue_Changed() {
    OnOriginalValueChange();
}
function OnOriginalValueChange() {
    OriginalValueChanged.value = "true";
}

function btnEditInvoiceReferences_click() {
    DisplayInvoiceReference();
}

function lnkPaymentDue_click() {
    var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/Dom/PaymentSchedules/VoidPaymentDue.aspx?psid=" + hidPaymentSchedule.value;
    var dialog = OpenDialog(url, 35, 20, window);
}
function PopulateInvoiceReference() {
    InvoiceListRequestvc.FetchInvoiceReferenceList(hidPaymentSchedule.value, PopulateInvoiceReference_CallBack);
}

function PopulateInvoiceReference_CallBack(serviceResponse) {

    if (CheckAjaxResponse(serviceResponse, InvoiceListRequestvc.url)) {

        var dlgDiv = $('#invoiceRef');
        var $tblInvoices = $('#tblInvoices');
        var tbody = GetElement("tblBody");
        var row, cell, lbl, txtbox, img;

        //Create the Invoice list request First
        $lblFilterCriteria = $('#lblFilterCriteria');

        var $tblInvoicesOptions = {
            filterDelay: 0,
            customFilterControls: [{ index: 5, control: GetSelectedFilterCtrl()}],
            onMatchingCell: function(tbl, row, rowIdx, cell, cellIdx, cellText, isMatch) {
                if (cellIdx == 5) {
                    if (!paymentReferenceStatusFilterCtrlVal || paymentReferenceStatusFilterCtrlVal == '') {
                        return true;
                    } else {
                        if (paymentReferenceStatusFilterCtrlVal == 'Read') {
                            return (cell[0].childNodes[1].value == 'Read');
                        }
                        else if (paymentReferenceStatusFilterCtrlVal == 'Unread') {
                            return (cell[0].childNodes[1].value == 'Unread');
                        }
                        else if (paymentReferenceStatusFilterCtrlVal == 'Saved') {
                            return (cell[0].childNodes[1].value == 'Saved');
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

        $.each(serviceResponse.value.InvoiceReferences, function(key, item) {

            // create row
            row = document.createElement("tr");
            // append row to tbody
            tbody.appendChild(row);
            // cerate td / cell
            cell = document.createElement("td");
            cell.setAttribute("width", "20%");
            // append cellt to row
            row.appendChild(cell);
            // insert the id of invoice in hidden field
            txtbox = document.createElement("input");
            txtbox.setAttribute("value", item.InvoiceId);
            txtbox.setAttribute("type", "hidden");
            cell.appendChild(txtbox);
            // insert the type of invoice like "invoice" or Proforma "in hidden field
            txtbox = document.createElement("input");
            txtbox.setAttribute("value", item.InvoiceType);
            txtbox.setAttribute("type", "hidden");
            cell.appendChild(txtbox);
            // create label
            lbl = document.createElement("label");
            // assign attribute values
            SetInnerText(lbl, item.ServiceUserReference);
            // append label to cell
            cell.appendChild(lbl);

            // cerate td / cell
            cell = document.createElement("td");
            cell.setAttribute("width", "20%");
            // append cellt to row
            row.appendChild(cell);
            // create label
            lbl = document.createElement("label");
            // assign attribute values
            SetInnerText(lbl, item.ServiceUserName);
            // append label to cell
            cell.appendChild(lbl);

            // cerate td / cell
            cell = document.createElement("td");
            cell.setAttribute("width", "15%");
            // append cellt to row
            row.appendChild(cell);
            // create label
            lbl = document.createElement("label");
            // assign attribute values
            SetInnerText(lbl, item.TypeName);
            // append label to cell
            cell.appendChild(lbl);

            // cerate td / cell
            cell = document.createElement("td");
            cell.setAttribute("width", "22%");
            // append cellt to row
            row.appendChild(cell);
            // create label
            lbl = document.createElement("label");
            // assign attribute values
            SetInnerText(lbl, item.InvoiceNumber);
            // append label to cell
            cell.appendChild(lbl);

            // cerate td / cell
            cell = document.createElement("td");
            cell.setAttribute("width", "20%");
            // append cellt to row
            row.appendChild(cell);
            if (PermitEditReference) {
                // create textbox
                txtbox = document.createElement("input");
                txtbox.setAttribute("value", item.PaymentReference);
                txtbox.setAttribute("type", "text");
                addEvent(txtbox, "change", function(evt) { UpdateReference(item.InvoiceId, item.InvoiceType, GetSrcElementFromEvent(evt).value); });
                addEvent(txtbox, "blur", function(evt) { UpdateReferenceImage(item.InvoiceId, item.InvoiceType, GetSrcElementFromEvent(evt).value, item.PaymentReference); });
                cell.appendChild(txtbox);


            } else {
                // create label
                lbl = document.createElement("label");
                // assign attribute values
                SetInnerText(lbl, item.PaymentReference);
                // append label to cell
                cell.appendChild(lbl);
            }

            // cerate td / cell
            cell = document.createElement("td");
            cell.setAttribute("width", "3%");
            // append cellt to row
            row.appendChild(cell);
            // display icons 
            img = document.createElement("img");
            img.setAttribute("src", SITE_VIRTUAL_ROOT + "images/BookUnread.png");
            img.setAttribute("id", item.InvoiceId + "_" + item.InvoiceType);
            cell.appendChild(img);

            // insert text read, unread, saved used for filtering
            txtbox = document.createElement("input");
            txtbox.setAttribute("value", item.InvoiceId);
            txtbox.setAttribute("type", "hidden");
            txtbox.setAttribute("id", "txtbox_" + item.InvoiceId + "_" + item.InvoiceType);
            txtbox.setAttribute("value", "Unread");
            cell.appendChild(txtbox);

        });

        $tblInvoices.tableFilter($tblInvoicesOptions);
        $tblInvoices.tableScroll({ height: 300, width: null });
        dlgDiv.dialog('displayLoading', { Text: null, Display: false });

    }

}

function GetSelectedFilterCtrl() {
    var $tblInvoices = $('#tblInvoices');

    if (!$paymentReferenceStatusFilterCtrl) {
        var classFilterRemove = 'FilterRemove';
        var menuItems = [{ id: -1, description: 'Clear Filter', cssClass: 'MenuItemClearFilter', tooltip: 'Clear Filtering' },
                    { description: 'Unread', cssClass: 'MenuItemUnread', tooltip: 'Filter by Unread' },
                    { description: 'Read', cssClass: 'MenuItemRead', tooltip: 'Filter by Read' },
                    { description: 'Saved', cssClass: 'MenuItemSaved', tooltip: 'Filter by Saved'}];
        $paymentReferenceStatusFilterCtrl = $('<span />');
        $paymentReferenceStatusFilterCtrl.addClass('Filter').attr('title', 'Filter');
        $paymentReferenceStatusFilterCtrl.searchableMenu(menuItems, { 'cssClass': 'Menu', position: 'BottomRight', showSearchBox: false, width: 100 });
        $paymentReferenceStatusFilterCtrl.bind('MenuItemClicked', function(src, menuItem) {
            if (menuItem.id === -1) {
                $paymentReferenceStatusFilterCtrl.removeClass(classFilterRemove);
                paymentReferenceStatusFilterCtrlVal = '';
            } else {
                $paymentReferenceStatusFilterCtrl.addClass(classFilterRemove);
                paymentReferenceStatusFilterCtrlVal = menuItem.description;
            }
            $(this).trigger('filterTableCellChanged', { filterTable: $tblInvoices, filterValue: paymentReferenceStatusFilterCtrlVal });
        });
    }
    return $paymentReferenceStatusFilterCtrl;
}

function UpdateReference(invoiceId, isProformaInvoice, newReference) {

    if (PermitEditReference) {

        var serviceResponse = InvoiceListRequestvc.UpdateInvoiceReference(invoiceId, isProformaInvoice, newReference);

        if (CheckAjaxResponse(serviceResponse, InvoiceListRequestvc.url)) {

            var img = GetElement(invoiceId + "_" + isProformaInvoice);
            var txtbox = GetElement("txtbox_" + invoiceId + "_" + isProformaInvoice);

            img.setAttribute("src", SITE_VIRTUAL_ROOT + "images/saved.png");
            txtbox.setAttribute("value", "Saved");

        }

    }

}

function UpdateReferenceImage(invoiceId, isProformaInvoice, newReference, oldReference) {
    if (newReference == oldReference) {
        var img = GetElement(invoiceId + "_" + isProformaInvoice);
        var txtbox = GetElement("txtbox_" + invoiceId + "_" + isProformaInvoice);
        img.setAttribute("src", SITE_VIRTUAL_ROOT + "images/BookRead.png");
        txtbox.setAttribute("value", "Read");
    }
}


function DisplayInvoiceReference() {
    var dlgDiv = $('#invoiceRef');
    dlgDiv.dialog('open');
    SetToolTip();
    dlgDiv.dialog('displayLoading', { Text: null, Display: true });
    PopulateInvoiceReference();

}

function closeReferencesDialog(src) {

    setTimeout(function() { $("#tblInvoices tbody").empty(); }, 1);
    return true;

}

function setControls() {
    EnableDisableContractSelector = GetElement("EnableDisableContractSelector");
    txtProvider = GetElement(txtProviderId);
    txtContract = GetElement(txtContractId);
    txtReference = GetElement(txtReferenceId + "_txtTextBox");
    hidPaymentSchedule = GetElement(hidPaymentScheduleId);
    txtContractName = GetElement(InPlaceContractSelectorID + "_txtContractNumber");
    txtContractReference = GetElement(InPlaceContractSelectorID + "_txtContractTitle");
    btnContractFind = GetElement(InPlaceContractSelectorID + "_btnFind");
    txtProviderName = GetElement(InPlaceProviderSelectorID + "_txtReference");
    txtProviderReference = GetElement(InPlaceProviderSelectorID + "_txtName");
    OriginalValueChanged = GetElement(OriginalValueChangedId);
    RequestMode = GetElement(RequestModeId);
    dtePaymentTo = GetElement(dtePaymentToId + "_txtTextBox");
    dtePaymentFrom = GetElement(dtePaymentFromId + "_txtTextBox");
    var mode = GetQSParam(document.location.search, "mode");
    if (mode != 2) {
        // un processed proforma invoice
        chkAwaiting = GetElement(chkAwaitingId);
        chkVerified = GetElement(chkVerifiedId);
        //
        chkUnpaid = GetElement(chkUnpaidId);
        chkSuspended = GetElement(chkSuspendedId);
        chkAuthorised = GetElement(chkAuthorisedId);
        chkPaid = GetElement(chkPaidId);
        // un processed visit amendments
        chkAmendUnVerified = GetElement(chkAmendUnVerifiedId);
        chkAmendVerified = GetElement(chkAmendVerifiedId);
    }
    chkVisitBased = $('input[id$=\'chkVisitBased\']');
    hidVisitBased = $('input[id$=\'hidVisitBased\']');
    btnAddProformaInvoice = $('button[id$=\'btnAddProformaInvoice\']');
    btnAddNonVisitBasedInvoices = $('#btnAddNonVisitBasedInvoices');
    pnlUnProcessedVisitamendmentRequest = $('div[id$=\'pnlUnProcessedVisitamendmentRequest\']');
}

function SetToolTip() {

}

function AddNonVisitBasedInvoices() {
    var initSettings = {
        service: paymentScheduleSvc,
        serviceRequestPayments: requestPaymentsSvc
    };
    var showSettings = {
        contractId: ToolTipContractID,
        paymentScheduleId: parseInt(hidPaymentSchedule.value),
        toolTipData: {
            reference: $(txtReference).val(),
            provider: ToolTipProvider,
            contract: ToolTipContract,
            dateFrom: ToolTipPaymentFrom,
            dateTo: ToolTipPaymentTo
        }
    };
    $(document).addNonVisitBasedInvoicesDialog(initSettings);
    $(document).addNonVisitBasedInvoicesDialog('show', showSettings);
}

(function($) {

    var dlgDiv, divRecords, tblRecords, tblRecordsTbody;
    var settings = {
        service: null,
        serviceRequestPayments: null,
        currentSettings: null,
        lastPaymentScheduleId: 0,
        onOkd: null,
        onCancelled: null
    };
    var templateRow = '<tr><td>${Name}&nbsp;</td><td>${Reference}&nbsp;</td><td>${HomeAddress}&nbsp;</td></tr>';
    var templateRowName = 'addNonVisitBasedInvoicesDialog.Row';

    $.fn.addNonVisitBasedInvoicesDialog = function(method, options) {
        return this.each(function() {
            var $this = $(this);
            if (methods[method]) {
                return methods[method].apply($this, [options]);
            } else if (typeof method === 'object' || !method) {
                return methods.init.apply($this, [method]);
            } else {
                alert('Method ' + method + ' does not exist on jQuery.addNonVisitBasedInvoicesDialog');
            }
        });
    };

    function cancelClicked(src) {
        dlgDiv.dialog('close');
    }

    function confirmClose(src) {
        displayLoadingDiv(false);
        return true;
    }

    function disableButtons(disabled) {
        dlgDiv.dialog('disableAllButtons', { Disabled: disabled });
    }

    function displayLoadingDiv(display, message) {
        dlgDiv.dialog('displayLoading', { Text: message, Display: display });
    }

    function disableOkButton(disabled) {
        dlgDiv.dialog('disableButton', { Text: 'OK', Disabled: disabled });
    }

    function loadTable(serviceResponse) {
        if (!CheckAjaxResponse(serviceResponse, settings.service)) {
            $dlgDiv.dialog('close');
            return false;
        }
        var items = serviceResponse.value.Items;
        if (tblRecords) {
            tblRecords.remove();
        }
        if (divRecords) {
            divRecords.empty();
        } else {
            divRecords = $('<div />').appendTo(dlgDiv);
        }
        tblRecords = $('<table class=\'listTable\' cellspacing=\'0\' cellpadding=\'2\' width=\'775px\' style=\'float: right;\'>').appendTo(divRecords);
        tblRecords.append($('<thead><tr><th style=\'width: 250px;\'>Service User</th><th style=\'width: 225px;\'>S/U Reference</th><th style=\'width: 300px;\'>Address</th></tr></thead>'));
        tblRecordsTbody = tblRecords.append($('<tbody />'));
        $.each(items, function(idx, val) {
            $.tmpl(templateRowName, val).appendTo(tblRecordsTbody);
        });
        $('<div style=\'clear: both;\'></div>').appendTo(divRecords);
        var tooltipItems = [];
        var tooltipInvoker = $('<span class=\'TooltipInformation\' title=\'View Further Information?\' />');
        tooltipItems.push({ name: 'Provider', value: settings.currentSettings.toolTipData.provider });
        tooltipItems.push({ name: 'Contract', value: settings.currentSettings.toolTipData.contract });
        tooltipItems.push({ name: 'Payments From', value: settings.currentSettings.toolTipData.dateFrom });
        tooltipItems.push({ name: 'Payments To', value: settings.currentSettings.toolTipData.dateTo });
        dlgDiv.dialog('addLeftAlignedButtonPaneControl', { control: tooltipInvoker });
        tooltipInvoker.each(function() {
            $(this).qtip({
                content: {
                    title: 'Payment Schedule Information',
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
        if (!items || items.length == 0) {
            disableOkButton(true);
        }
        if (tblRecords && tblRecords.height() >= 400 && dlgDiv.find('.tablescroll').length === 0) {
            tblRecords.tableScroll({ height: 400, width: 750 });
        }
        dlgDiv.dialog("option", "position", "center");
        settings.lastPaymentScheduleId = settings.currentSettings.paymentScheduleId;
    }

    function saveChanges(src) {
        disableButtons(true);
        settings.serviceRequestPayments.CreatePaymentRequest(settings.currentSettings.toolTipData.dateTo.toDate(), settings.currentSettings.paymentScheduleId, saveChangesCreatePaymentRequestCallBack);
    }

    function saveChangesCreatePaymentRequestCallBack(serviceResponse) {
        if (CheckAjaxResponse(serviceResponse, settings.serviceRequestPayments)) {
            var paymentRequestID = serviceResponse.value.PaymentRequestID;
            serviceResponse = settings.serviceRequestPayments.CreatePaymentRequest_DomContract(paymentRequestID, settings.currentSettings.contractId);
            if (CheckAjaxResponse(serviceResponse, settings.serviceRequestPayments)) {
                serviceResponse = settings.serviceRequestPayments.CreateJob_ProcessPaymentRequest(paymentRequestID);
                if (CheckAjaxResponse(serviceResponse, settings.serviceRequestPayments)) {
                    var emailAddress = serviceResponse.value.EmailAddress;
                    var goodbyeMsg = 'Thank you. Your Payment Request has been received and will be processed shortly.';
                    if ($.isFunction(settings.onOkd)) {
                        settings.onOkd({ createdPaymentRequestID: paymentRequestID });
                    }
                    if (emailAddress) {
                        goodbyeMsg += '\n\nAn email will be sent to the following address once processing is complete:\n\n' + emailAddress + '.';
                    }
                    alert(goodbyeMsg);
                    dlgDiv.dialog('close');
                    disableButtons(false);
                }
            }
        }
        disableButtons(false);
        displayLoadingDiv(false);
    }

    var methods = {
        init: function(options) {
            if (!dlgDiv) {
                if (options) {
                    $.extend(settings, options);
                }
                if (!settings.service) {
                    alert('Please specify the option \'service\'');
                    return;
                }
                if (!settings.serviceRequestPayments) {
                    alert('Please specify the option \'serviceRequestPayments\'');
                    return;
                }
                dlgDiv = $('<div>');
                dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999,
                    title: '',
                    buttons: {
                        "Cancel": function() {
                            cancelClicked($(this));
                        }
                    }
                });
                $.template(templateRowName, templateRow);
            }
        },
        show: function(options) {
            if (dlgDiv) {
                var showSettings = {
                    contractId: 0,
                    paymentScheduleId: 0,
                    toolTipData: {
                        reference: '',
                        provider: '',
                        contract: '',
                        dateFrom: '',
                        dateTo: ''
                    }
                };
                if (options) {
                    $.extend(showSettings, options);
                }
                if (showSettings.paymentScheduleId <= 0) {
                    alert('Please specify parameter \'paymentScheduleId\'');
                    return false;
                }
                if (showSettings.contractId <= 0) {
                    alert('Please specify parameter \'contractId\'');
                    return false;
                }
                settings.currentSettings = showSettings;
                dlgDiv.dialog({
                    autoOpen: false,
                    draggable: true,
                    modal: true,
                    resizable: false,
                    closeOnEscape: true,
                    zIndex: 9999,
                    title: settings.currentSettings.toolTipData.reference + ' (Payments From ' + settings.currentSettings.toolTipData.dateFrom + ' to ' + settings.currentSettings.toolTipData.dateTo + ')',
                    width: 800,
                    maxHeight: 800,
                    buttons: {
                        "OK": function() {
                            saveChanges($(this));
                        },
                        "Cancel": function() {
                            $(this).dialog('close');
                        }
                    },
                    beforeClose: function(event, ui) {
                        return confirmClose($(this));
                    }
                });
                dlgDiv.dialog('open');
                if (settings.lastPaymentScheduleId !== showSettings.paymentScheduleId) {
                    disableButtons(true);
                    displayLoadingDiv(true);
                    settings.service.GetOutstandingServiceUsers(showSettings.paymentScheduleId, loadTable);
                } else {
                    displayLoadingDiv(false);
                }
            } else {
                alert('Please call \'init\' prior to calling \'show\'.');
            }
        }
    };

})(jQuery);



addEvent(window, "load", Init);