
var Edit_domContractID, Edit_cboSuspensionReasonID, Edit_chkSuspendID;
var Edit_chkSuspend, Edit_cboSuspensionReason, Edit_valSuspensionReason;
var Edit_domContractID, Edit_clientID;
var Edit_dteAddDetailWeekEndingID, Edit_cboAddDetailRateCategoryID, Edit_cboAddDetailRateID;
var Edit_txtActualCostID, Edit_chkVatID, Edit_vatID, Edit_penaltyPaymentID, Edit_clientContribID, Edit_txtNetCostID, Edit_txtInvoiceTotalID;
var Edit_canEditLineCost;
var Edit_AutoSuspendClient;
var contractSvc, Edit_invoiceStyle, Edit_mode, invID, invVATRate;
var Edit_txtWeekEndingFromID, Edit_txtWeekEndingToID;
var NaturalPeriodOfProviderInvoiceAdditionalDays;
var Edit_preventEntryOfFutureDates;
var Edit_rvInvoiceDate, Edit_invoiceDate, Edit_HeadersEditable;
var InvoiceHasNotes;

var InvoiceHasNotes;
var proformainvoiceSvc;
var NonDeliveryOfService;
var psService;


$(document).ready(function() {

    var $tabStripBody = $('div[id$="_tabStrip_tabSummary"]');
    var $tabSummaryContainer = $('#tabSummaryContainer');
    $tabStripBody.height($tabStripBody.height() + $tabSummaryContainer.height());

    if (InvoiceHasNotes == 'true') {
        AddNotesLink();
    }


    proformainvoiceSvc = new Target.Abacus.Web.Apps.WebSvc.DomProfomaInvoice_class();
    NonDeliveryOfService = new Target.Abacus.Web.Apps.WebSvc.DomProviderInvoice_class();
    psService = new Target.Abacus.Web.Apps.WebSvc.PaymentSchedule_class();

});

function AddNotesLink() {
    $('#imgNotes').prepend('<img alt="View provider-entered note" style="cursor:pointer;" onclick="DisplayNotes();" id="theImg" src="../../../../images/Notes.png" />');
}

//Start D11766
//helper function
//var paymentTolerance_WeekEndings;
var FormatHelpers = {
    formatCurrency: function(val) {
        val = parseFloat(val);
        return '£' + (isNaN(val) ? 0.00 : val).toFixed(2);
    },
    formatDate: function(val) {
        return Date.strftime("%d/%m/%Y", val);
    }
};
//service order payment tolerances dictionary
//var dictmDomServiceOrderPaymentToleranceAndGroupIDs = new Dictionary();
//contract period payment tolerances dictionary
//var dictmContractPeriodPaymentToleranceAndGroupIDs = new Dictionary();
var costExceeded, unitsExceeded;
var costExceededText = "Contracted Cost Exceeded";
var unitsExceededText = "Contracted Units Exceeded";
//detailLine Object
function oDetailLine() {
    this.row = undefined;
    this.weekEnding = undefined;
    this.toleranceID = undefined;
    this.toleranceType = undefined;
    this.plannedUnits = undefined;
    this.plannedCost = undefined;
    this.otherInvoicesUnits = undefined;
    this.otherInvoicesCosts = undefined;
    this.thisInvoiceUnits = undefined;
    this.thisInvoiceCost = undefined;
    this.totalUnits = undefined;
    this.totalCost = undefined;
}
//detailLine Collection
var detailLines = [];
//End D11766
function Init() {
    var txtVat;
    var vat;

    contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
    Edit_chkSuspend = GetElement(Edit_chkSuspendID);
    Edit_cboSuspensionReason = GetElement(Edit_cboSuspensionReasonID + "_cboDropDownList");
    Edit_valSuspensionReason = GetElement(Edit_cboSuspensionReasonID + "_valRequired");

    invID = "0";
    if (Edit_mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew || Edit_mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
        chkSuspend_Click();
        if (Edit_mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
            invID = GetQSParam(document.location.search, "id");
        }
        if (Edit_vatID != undefined) {
            txtVat = GetElement(Edit_vatID + "_txtTextBox");
            vat = parseFloat(txtVat.value);
            if (vat != 0.00) {
                if (Edit_invoiceStyle == Target.Abacus.Library.DomProviderInvoiceStyle.SummaryLevel) {
                    txtVat.disabled = false;
                }
                RecalcSummary();
            } else {
                chkSummaryVat_Click();
            }
        }
    }

    var invDate = GetElement(Edit_invoiceDate);

    contractSvc.FetchDomProviderInvoiceVATRate(invDate.value, VATRate_Callback);

//    if (Edit_HeadersEditable == true) {
//        InPlaceEstablishment_Changed(InPlaceDomContractSelector_providerID.toString());
//    }
    // invoice notes
    var dlgDiv = $('#invNotesPopup');
    dlgDiv.dialog({
        autoOpen: false,
        draggable: true,
        modal: true,
        resizable: false,
        closeOnEscape: true,
        zIndex: 9999,
        minHeight: 400,
        minWidth: 800,
        title: "Provider-entered Invoice Note",
        buttons: {
            "Close": function() {
                cancelClicked($(this));
            }
        }
    });
    
}

// view invoice notes
function DisplayNotes() {
    var dlgDiv = $('#invNotesPopup');
    dlgDiv.dialog('open');
}

function cancelClicked() {
    var dlgDiv = $('#invNotesPopup');
    dlgDiv.dialog('close');
}
// view invoice notes

function InPlaceEstablishment_Changed(newID) {
    var enabled = false;
    if (newID.trim().length > 0 && newID != '0') enabled = true;
    InPlaceDomContractSelector_ClearStoredID(Edit_domContractID);
    if (enabled) {
        InPlaceDomContractSelector_Enabled(Edit_domContractID, true);
    } else {
        InPlaceDomContractSelector_Enabled(Edit_domContractID, false);
    }
    InPlaceDomContractSelector_providerID = newID;
}

function tabStrip_ActiveTabChanged(sender, args) {
    var hidSelectedTab = GetElement("hidSelectedTab");
    hidSelectedTab.value = sender.get_activeTab().get_headerText();
}

function chkSuspend_Click() {
    if (Edit_chkSuspend.checked) {
        Edit_cboSuspensionReason.disabled = false;
        ValidatorEnable(Edit_valSuspensionReason, true);
    } else {
        Edit_cboSuspensionReason.value = "";
        Edit_cboSuspensionReason.disabled = true;
        ValidatorEnable(Edit_valSuspensionReason, false);
    }
}

function cboAddDetailRateCategory_Change() {
    FetchRateList();
}

function dteAddDetailWeekEnding_Changed(txtID) {
    FetchRateList();
}

function txtThisInvoiceUnits_Change(id) {

    var txtUnits = GetElement(id + "_txtTextBox");
    var txtRate = GetElement(id.replace("_detailThisUnits", "_detailThisRate"));
    var lineCost = (parseFloat(txtUnits.value) * parseFloat(txtRate.value)).toFixed(2);

    if (Edit_canEditLineCost) {
        var txtCost = GetElement(id.replace("_detailThisUnits", "_detailThisCost") + "_txtTextBox");
        txtCost.value = lineCost;
    } else {
        var hidCost = GetElement(id.replace("_detailThisUnits", "_detailThisCost"));
        var spnCost = hidCost.previousSibling;
        SetInnerText(spnCost, lineCost);
        hidCost.value = lineCost;
    }

    // Ensure that the SU Units field matches the entered value in the Units field
    if (Edit_mode == Target.Library.Web.UserControls.StdButtonsMode.AddNew || Edit_mode == Target.Library.Web.UserControls.StdButtonsMode.Edit) {
        var txtSUUnits = GetElement(id.replace("_detailThisUnits", "_detailThisSUUnits") + "_txtTextBox");
        txtSUUnits.value = txtUnits.value;
    }

    RecalcSummary();
    ReEvaluateDetailLinesForSuspensionReason();

}

function txtThisInvoiceSUUnits_Change(id) {
    RecalcSummary();
}

function txtThisInvoiceCost_Change(id) {
    RecalcSummary();
    ReEvaluateDetailLinesForSuspensionReason();
}

function chkSummaryVat_Click() {
    var chkVat = GetElement(Edit_chkVatID + "_chkCheckbox");
    var txtVat = GetElement(Edit_vatID + "_txtTextBox");
    var txtActualCost = GetElement(Edit_txtActualCostID + "_txtTextBox");
    var actualCost, vat;

    if (chkVat.checked) {
        actualCost = parseFloat(txtActualCost.value);
        txtVat.value = (actualCost * parseFloat(parseFloat(invVATRate) / parseFloat(100.00))).toFixed(2);
        txtVat.disabled = false;
    } else {
        txtVat.value = "0.00";
        txtVat.disabled = true;
    }
    RecalcSummary();
}

function txtSummaryVat_Changed(id) {
    var chkVat = GetElement(Edit_chkVatID + "_chkCheckbox");
    var txtVat = GetElement(Edit_vatID + "_txtTextBox");
    var txtActualCost = GetElement(Edit_txtActualCostID + "_txtTextBox");
    var actualCost, expectedVat, enteredVat, tempValue;

    if (chkVat.checked) {
        actualCost = parseFloat(txtActualCost.value);
        tempValue = (actualCost * parseFloat(parseFloat(invVATRate) / parseFloat(100.00))).toFixed(2);
        expectedVat = parseFloat(tempValue);
        enteredVat = parseFloat(txtVat.value);
        if (expectedVat >= 0) {
            if (enteredVat > (expectedVat + parseFloat(0.05))) {
                alert("VAT amount entered cannot be greater than " + invVATRate + "% plus 5p");
                txtVat.value = (expectedVat).toFixed(2);
            }
        }
        else {
            if (enteredVat < (expectedVat - parseFloat(0.05))) {
                alert("VAT amount entered cannot be less than " + invVATRate + "% minus 5p");
                txtVat.value = (expectedVat).toFixed(2);
            }
        }
    }
    RecalcSummary();
}
function txtSummaryPenalty_Changed(id) {
    RecalcSummary();
}
function txtSummaryClientContrib_Changed(id) {
    RecalcSummary();
}

function RecalcSummary() {
    // for summary level invoices

    var tbl, tBody, row, cell;
    var lineCost, linesTotal;
    var txtActualCost, txtVat, txtPenaltyPayment, txtClientContrib, txtNetCost, txtInvoiceTotal;
    var vat, penaltyPayment, clientContrib;

    tbl = GetElement("tblDetailsSummary");
    tBody = tbl.tBodies[0];
    txtActualCost = GetElement(Edit_txtActualCostID + "_txtTextBox");
    txtVat = GetElement(Edit_vatID + "_txtTextBox");
    txtPenaltyPayment = GetElement(Edit_penaltyPaymentID + "_txtTextBox");
    txtClientContrib = GetElement(Edit_clientContribID + "_txtTextBox");
    txtNetCost = GetElement(Edit_txtNetCostID + "_txtTextBox");
    txtInvoiceTotal = GetElement(Edit_txtInvoiceTotalID + "_txtTextBox");

    // sum up line costs
    linesTotal = 0;
    for (index = 0; index < tBody.rows.length; index++) {
        row = tBody.rows[index];
        cell = row.cells[10];
        lineCost = parseFloat(cell.getElementsByTagName("input")[0].value);
        if (isNaN(lineCost)) lineCost = 0.00;
        linesTotal += lineCost;
    }

    // actual cost
    txtActualCost.value = linesTotal.toFixed(2);
    vat = parseFloat(txtVat.value);
    if (isNaN(vat)) vat = 0.00;
    penaltyPayment = parseFloat(txtPenaltyPayment.value);
    if (isNaN(penaltyPayment)) penaltyPayment = 0.00;
    clientContrib = parseFloat(txtClientContrib.value);
    if (isNaN(clientContrib)) clientContrib = 0.00;

    // net cost
    txtNetCost.value = (parseFloat(txtActualCost.value) + penaltyPayment - clientContrib).toFixed(2);

    // invoice total
    txtInvoiceTotal.value = (parseFloat(txtNetCost.value) + vat).toFixed(2);

}

function btnRemoveDetail_Click() {
    return window.confirm("Are you sure you wish to remove this invoice line?");
}

function FetchRateList() {

    var domContractID = 0
    var clientID = 0
    var weekEnding = new Date();
    var rateCategoryID = 0;

    var cboRate, txtWeekEnding, cboRateCategory;

    cboRate = GetElement(Edit_cboAddDetailRateID + "_cboDropDownList");
    txtWeekEnding = GetElement(Edit_dteAddDetailWeekEndingID + "_txtTextBox");
    cboRateCategory = GetElement(Edit_cboAddDetailRateCategoryID + "_cboDropDownList");

    domContractID = GetElement(Edit_domContractID + "_hidID").value;
    clientID = GetElement(Edit_clientID + "_hidID").value;
    if (txtWeekEnding.value.length > 0 && IsDate(txtWeekEnding.value)) weekEnding = txtWeekEnding.value.toDate();
    if (cboRateCategory.value.length > 0) rateCategoryID = parseInt(cboRateCategory.value, 10);

    DisplayLoading(true);
    contractSvc.FetchDomProviderInvoiceDetailRates(domContractID, clientID, weekEnding, rateCategoryID, cboRate.id, FetchRateList_Callback);
}
function FetchRateList_Callback(response) {

    var cboRate, rates, opt;

    if (CheckAjaxResponse(response, contractSvc.url)) {

        cboRate = GetElement(response.value.Tag);
        rates = response.value.Values;

        // clear
        cboRate.options.length = 0;

        // add blank
        if (rates.length > 1) {
            opt = document.createElement("OPTION");
            cboRate.options.add(opt);
            SetInnerText(opt, "");
            opt.value = "";
        }

        for (index = 0; index < rates.length; index++) {
            opt = document.createElement("OPTION");
            cboRate.options.add(opt);
            SetInnerText(opt, rates[index]);
            opt.value = rates[index];
        }

    }
    DisplayLoading(false);

}

function VATRate_Callback(response) {

    if (CheckAjaxResponse(response, contractSvc.url)) {
        invVATRate = response.value.Values[0];
    } else {
        invVATRate = "17.5";
    }
    DisplayLoading(false);

}

function btnAddDetail_Click() {

    var allowAdd = true;

    if (Edit_preventEntryOfFutureDates == true) {
        if (Edit_rvInvoiceDate != '') {
            var valControl = GetElement(Edit_rvInvoiceDate);
            if (valControl) {
                ValidatorEnable(valControl);
                if (valControl.isvalid == false) {
                    allowAdd = false;
                }
            }
        }
    }

    if (allowAdd == true) {

        var d = new Target.Web.Dialog.Msg();
        var bdyFontSize = parseFloat($("body").css("font-size"));
        var dlgWidth = $('#divAddDetailDialogContentContainer').outerWidth() + 10;
        var emptyDialogContent, addDetailDialogContentContainer, addDetailDialogContent;

        d.SetType(4);   // OK/Cancel
        d.SetCallback(AddDetailDialog_Callback);
        d.SetTitle("Add Detail");
        d.SetWidth(dlgWidth / bdyFontSize);

        d.ClearContent();
        emptyDialogContent = document.createElement("DIV");
        d.AddContent(emptyDialogContent);

        addDetailDialogContentContainer = GetElement("divAddDetailDialogContentContainer");
        addDetailDialogContent = addDetailDialogContentContainer.getElementsByTagName("DIV")[0];

        // swap nodes
        emptyDialogContent = d._content.removeChild(emptyDialogContent);
        addDetailDialogContent = addDetailDialogContentContainer.removeChild(addDetailDialogContent);
        addDetailDialogContentContainer.appendChild(emptyDialogContent);
        d.AddContent(addDetailDialogContent);

        d.ShowCloseLink(false);
        d.Show();

        FetchRateList();

        return false;

    }
    else {

        alert('The \'Invoice Date\' may not be in the future, please ensure the invoice date on the \'Header\' tab is not in the future prior to adding a new detail line.');
        return false;

    }
}
function AddDetailDialog_Callback(evt, args) {
    var d = args[0];
    var answer = args[1];
    var emptyDialogContent, addDetailDialogContentContainer, addDetailDialogContent;
    var newWeekEnding, newRateCategoryID, newRate, newKey, response;

    // answer == 1 means OK
    if (answer == 1) {
        if (Page_ClientValidate("AddDetail")) {
            newWeekEnding = GetElement(Edit_dteAddDetailWeekEndingID + "_txtTextBox").value;
            newRateCategoryID = GetElement(Edit_cboAddDetailRateCategoryID + "_cboDropDownList").value;
            newRate = GetElement(Edit_cboAddDetailRateID + "_cboDropDownList").value;
            newKey = newWeekEnding + "-" + newRateCategoryID + "-" + newRate;
            response = contractSvc.WeekendingDateValid(newWeekEnding.toDate()).value;
            if (response.Success == false) {
                alert(response.Message);
                return;
            } else {
                // loop through existing lines and check for unique key (w/e - rate category - rate)
                var tbl = GetElement("tblDetailsSummary");
                var tBody = tbl.tBodies[0];
                var row, weekEnding, rateCategoryID, rate, key;
                for (index = 0; index < tBody.rows.length; index++) {
                    row = tBody.rows[index];
                    weekEnding = row.cells[0].getElementsByTagName("input")[0].value;
                    rateCategoryID = row.cells[1].getElementsByTagName("input")[0].value;
                    rate = row.cells[9].getElementsByTagName("input")[0].value;
                    key = weekEnding + "-" + rateCategoryID + "-" + rate;
                    if (key == newKey) {
                        alert("A similar detail line is already present on this invoice. Please add further units to this detail line.");
                        return;
                    }
                }
                AddDetail_DoPostBack();
            }
        } else {
            return;
        }
    }
    addDetailDialogContentContainer = GetElement("divAddDetailDialogContentContainer");
    emptyDialogContent = addDetailDialogContentContainer.getElementsByTagName("DIV")[0];
    addDetailDialogContent = d._content.getElementsByTagName("DIV")[0];

    // swap nodes
    emptyDialogContent = addDetailDialogContentContainer.removeChild(emptyDialogContent);
    addDetailDialogContent = d._content.removeChild(addDetailDialogContent);
    addDetailDialogContentContainer.appendChild(addDetailDialogContent);
    d.AddContent(emptyDialogContent);

    d.Hide();
}
function txtWeekEndingFrom_Changed() {
    var txtWeekEndingFrom = GetElement(Edit_txtWeekEndingFromID + "_txtTextBox");
    var txtWeekEndingTo = GetElement(Edit_txtWeekEndingToID + "_txtTextBox");

    if (txtWeekEndingFrom.value.length > 0 && IsDate(txtWeekEndingFrom.value)) {

        var toDate = txtWeekEndingFrom.value.toDate();
        if (!NaturalPeriodOfProviderInvoiceAdditionalDays || NaturalPeriodOfProviderInvoiceAdditionalDays == '' || NaturalPeriodOfProviderInvoiceAdditionalDays == 0) {
            NaturalPeriodOfProviderInvoiceAdditionalDays = 27;
        }

        toDate.setDate(toDate.getDate() + NaturalPeriodOfProviderInvoiceAdditionalDays);
        txtWeekEndingTo.value = Date.strftime("%d/%m/%Y", toDate);
        FireEvent(txtWeekEndingTo, "change");
        setTxtWeekEndingToMinDate();
    }
}
function setTxtWeekEndingToMinDate() {
    var txtWeekEndingFrom = GetElement(Edit_txtWeekEndingFromID + "_txtTextBox");
    var txtWeekEndingTo = GetElement(Edit_txtWeekEndingToID + "_txtTextBox");
    if (txtWeekEndingFrom.value.length > 0 && IsDate(txtWeekEndingFrom.value)) {
        $(txtWeekEndingTo).datepicker("option", "minDate", txtWeekEndingFrom.value.toDate());
    }
}
function ReEvaluateDetailLinesForSuspensionReason() {

    if (Edit_AutoSuspendClient) {

        var tbl, tBody, row;
        var plannedUnits, plannedCost;
        var otherInvoicesUnits, otherInvoicesCosts;
        var thisInvoiceUnits, thisInvoiceCost;
        var totalUnits, totalCost;
        var suspensionReasonCheckBoxControl, suspensionReasonDropDownControl;
        var suspensionDescription, suspensionValue;
        var toleranceResult;
        //Start D11766
        //set references to checkbox and combo box controls
        suspensionReasonCheckBoxControl = Edit_chkSuspend;
        suspensionReasonDropDownControl = Edit_cboSuspensionReason;
        //check this invoice for paymentTolerances
        if (paymentTolerances.length != 0) {
            //build detail line collection required for Tolerance Calculator
            buildPaymentToleranceCollections();
            //create ToleranceCalculator object
            var olToleranceCalculator = new ToleranceCalculator();
            //process tolerances
            toleranceResult = olToleranceCalculator.processProviderInvoiceTolerances(detailLines, paymentTolerances, paymentToleranceGroups);
            if (toleranceResult == Target.Abacus.Library.DomContractedExceeded.Yes_Units) {
                unitsExceeded = true;
            }
            else if (toleranceResult == Target.Abacus.Library.DomContractedExceeded.Yes_Cost) {
                costExceeded = true;
            }
            else if (toleranceResult == Target.Abacus.Library.DomContractedExceeded.No) {
                costExceeded = false;
                unitsExceeded = false;
            }
            //clear ToleranceCalculator Object
            olToleranceCalculator = undefined;
        }
        //if these are undefined it means no payment tolerances have been processed because they don't exist
        //so process under the old tolerance rules pre D11766
        if (costExceeded == undefined && unitsExceeded == undefined) {
            //End D11766
            tbl = GetElement("tblDetailsSummary");
            tBody = tbl.tBodies[0];
            for (index = 0; index < tBody.rows.length; index++) {
                row = tBody.rows[index];
                plannedUnits = Number(row.cells[2].getElementsByTagName("input")[0].value);
                plannedCost = Number(row.cells[4].getElementsByTagName("input")[0].value);
                otherInvoicesUnits = Number(row.cells[5].getElementsByTagName("input")[0].value);
                otherInvoicesCosts = Number(row.cells[6].getElementsByTagName("input")[0].value);
                thisInvoiceUnits = Number(row.cells[7].getElementsByTagName("input")[0].value);
                thisInvoiceCost = Number(row.cells[10].getElementsByTagName("input")[0].value);
                totalUnits = Number(otherInvoicesUnits) + Number(thisInvoiceUnits);
                totalCost = Number(otherInvoicesCosts) + Number(thisInvoiceCost);
                if (totalCost > plannedCost) {
                    costExceeded = true;
                    break;
                }
                else if (totalUnits > plannedUnits) {
                    unitsExceeded = true;
                }
            }
            //Start D11766
        }
        //End D11766

        if (costExceeded == true) {
            suspensionDescription = costExceededText;
        }
        else if (unitsExceeded == true) {
            suspensionDescription = unitsExceededText;
        }

        if (costExceeded == true || unitsExceeded == true) {

            var foundSuspensionReason = false;
            suspensionReasonCheckBoxControl.checked = true;

            for (i = 0; i <= suspensionReasonDropDownControl.options.length - 1; i++) {
                if (suspensionReasonDropDownControl.options[i].text == suspensionDescription) {
                    suspensionReasonDropDownControl.options[i].selected = true;
                    foundSuspensionReason = true;
                }
            }

            if (foundSuspensionReason == false) {
                var suspensionOption = document.createElement("option");
                suspensionReasonDropDownControl.options.add(suspensionOption);
                suspensionOption.text = suspensionDescription;
                suspensionOption.selected = true;
            }

            suspensionReasonDropDownControl.disabled = true;
            suspensionReasonCheckBoxControl.disabled = true;
            ValidatorEnable(Edit_valSuspensionReason, false);
        }
        else {

            for (i = 0; i <= suspensionReasonDropDownControl.options.length - 1; i++) {
                if (suspensionReasonDropDownControl.options[i].text == costExceededText || suspensionReasonDropDownControl.options[i].text == unitsExceededText) {
                    suspensionReasonDropDownControl.remove(i);
                }
            }

            suspensionReasonDropDownControl.disabled = true;
            suspensionReasonDropDownControl.value = "";
            suspensionReasonCheckBoxControl.disabled = false;
            suspensionReasonCheckBoxControl.checked = false;
            ValidatorEnable(Edit_valSuspensionReason, false);

        }

    }

    //reset variables
    costExceeded = undefined
    unitsExceeded = undefined
}

//start D11766

function buildPaymentToleranceCollections() {
    //variables 
    var tbl, tBody, row;
    var plannedUnits, plannedCost;
    var otherInvoicesUnits, otherInvoicesCosts;
    var thisInvoiceUnits, thisInvoiceCost;
    var totalUnits, totalCost;
    var toleranceID
    var toleranceType
    var weekEnding
    tbl = GetElement("tblDetailsSummary");
    tBody = tbl.tBodies[0];
    //reset the collection
    detailLines = [];
    //iterate through detail lines on screen to collect information required for calculating payment tolerances
    for (index = 0; index < tBody.rows.length; index++) {
        //create new detail line object
        var olDetailLine = new oDetailLine();
        row = tBody.rows[index];
        olDetailLine.weekEnding = String(row.cells[0].getElementsByTagName("input")[0].value);
        olDetailLine.toleranceID = Number(row.cells[1].getElementsByTagName("input")[1].value);
        olDetailLine.toleranceType = Number(row.cells[1].getElementsByTagName("input")[2].value);
        olDetailLine.plannedUnits = Number(row.cells[2].getElementsByTagName("input")[0].value);
        olDetailLine.plannedCost = Number(row.cells[4].getElementsByTagName("input")[0].value);
        olDetailLine.otherInvoicesUnits = Number(row.cells[5].getElementsByTagName("input")[0].value);
        olDetailLine.otherInvoicesCosts = Number(row.cells[6].getElementsByTagName("input")[0].value);
        olDetailLine.thisInvoiceUnits = Number(row.cells[7].getElementsByTagName("input")[0].value);
        olDetailLine.thisInvoiceCost = Number(row.cells[10].getElementsByTagName("input")[0].value);
        olDetailLine.totalUnits = Number(olDetailLine.otherInvoicesUnits) + Number(olDetailLine.thisInvoiceUnits);
        olDetailLine.totalCost = Number(olDetailLine.otherInvoicesCosts) + Number(olDetailLine.thisInvoiceCost);
        //add the object to the detail lines collection
        detailLines.push(olDetailLine);
    }
}
//end D11766

function ShowDeliverydetail(paymentScheduleId, invoiceId, invoiceDetailId, plannedUnits, UnitCost, timebased) {
    if (timebased == "true") {
        loadTimeBased(paymentScheduleId, invoiceId, invoiceDetailId, plannedUnits, UnitCost);
    }
    else {

        loadNonTimeBased(paymentScheduleId, invoiceId, invoiceDetailId, plannedUnits, UnitCost);
    }
}

function loadTimeBased(paymentScheduleId, invoiceid, invoiceDetailId, plannedUnits, UnitCost) {
    //alert("time based");
    var $invoice = NonDeliveryOfService.FetchNonDeliveryCurrentProviderInvoice(invoiceid);
    var $invoiceDetail = NonDeliveryOfService.FetchNonDeliveryCurrentProviderInvoiceDetail(invoiceDetailId);
    var $ps = psService.FetchPaymenstSchedule(paymentScheduleId);

    if ($invoiceDetail.value.ErrMsg.Success) {
        $invoiceDetail.value.CurrentProformaDetail.PlannedUnits = plannedUnits;
        $invoiceDetail.value.CurrentProformaDetail.UnitCost = UnitCost;
    }

    var $response = NonDeliveryOfService.FetchDomProviderInvoiceDetailNonDeliveryVisitBasedEnquiryResults(invoiceDetailId);

    var initSettings = {
        serviceDomProformas: proformainvoiceSvc,
        paymentSchedule: $ps.value.PaymentSchedule,
        proformaInvoice: $invoice.value.CurrentProforma,
        onOkd: function(args) {
            onUpdatedNonDeliveryVisitBasedRecords(args);
        },
        onCancelled: null
    };
    var showSettings = {
        id: invoiceDetailId,
        weekEnding: $invoiceDetail.value.CurrentProformaDetail.WeekEnding,//"10/04/2011".toDate(),
        isEditable: false,
        overriddenItems: $response.value.NonDeliveryVisits,
        //sourceSettings: { control: src, row: row },
        proformaInvoiceDetail: $invoiceDetail.value.CurrentProformaDetail
    };
    $(document).nonDeliveryVisitBasedProformaInvoiceLineDialog(initSettings);
    $(document).nonDeliveryVisitBasedProformaInvoiceLineDialog('show', showSettings);

}

function loadNonTimeBased(paymentScheduleId, invoiceid, invoiceDetailId, plannedUnits, UnitCost) {
    //alert("time based");

    var $invoice = NonDeliveryOfService.FetchNonDeliveryCurrentProviderInvoice(invoiceid);
    var $invoiceDetail = NonDeliveryOfService.FetchNonDeliveryCurrentProviderInvoiceDetail(invoiceDetailId);
    var $ps = psService.FetchPaymenstSchedule(paymentScheduleId);

    if ($invoiceDetail.value.ErrMsg.Success) {
        $invoiceDetail.value.CurrentProformaDetail.PlannedUnits = plannedUnits;
        $invoiceDetail.value.CurrentProformaDetail.UnitCost = UnitCost;
    }

    var $response = NonDeliveryOfService.FetchDomProviderInvoiceDetailNonDeliveryUnitBasedEnquiryResults(invoiceDetailId);

    var initSettings = {
        serviceDomProformas: proformainvoiceSvc,
        paymentSchedule: $ps.value.PaymentSchedule,
        proformaInvoice: $invoice.value.CurrentProforma,
        onOkd: function(args) {
            onUpdatedNonDeliveryVisitBasedRecords(args);
        },
        onCancelled: null
    };
    var showSettings = {
        id: invoiceDetailId,
        weekEnding: $invoiceDetail.value.CurrentProformaDetail.WeekEnding, //"10/04/2011".toDate(),
        isEditable: false,
        overriddenItems: $response.value.NonDeliveryUnits,
        //sourceSettings: { control: src, row: row },
        proformaInvoiceDetail: $invoiceDetail.value.CurrentProformaDetail
    };
    $(document).nonDeliveryUnitBasedProformaInvoiceLineDialog(initSettings);
    $(document).nonDeliveryUnitBasedProformaInvoiceLineDialog('show', showSettings);

}

    function btnBack_Click() {
        var url = GetQSParam(document.location.search, "backUrl");
        url = unescape(url);
        //alert(url);
        document.location.href = url;
    }
