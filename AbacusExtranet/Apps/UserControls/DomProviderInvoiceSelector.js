var providerSvc, currentPage, providerID, contractID, clientID, dateFrom, dateTo, userHasRetractCommand, pScheduleId;
var paid, unPaid, authorised, suspended;
var invoiceNumber, weekendingDateFrom, weekendingDateTo;
var selectedInvoiceID, selectedProviderID;
var tblInvoices, divPagingLinks, btnViewPrint, btnViewCostedVisits, btnViewInvoiceLines, btnRetract;
var populating = false;
var listFilter, listFilterSUReference = "", listFilterSUName = "", listFilterInvNumber = "", listFilterPaymentRef = "";
var hideContractNumber, hideProviderReference, hideSUReference;
var invoices;
var btnCostedVisits;
var hideRetraction;
var showPaymentSchedule;
var currentContractIsPeriodicBlock;


function Init() {
    providerSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomProviderInvoice_class();
	tblInvoices = GetElement("tblInvoices");
	divPagingLinks = GetElement("Invoice_PagingLinks");
	//btnViewPrint = GetElement("btnViewPrint");
	btnViewCostedVisits = GetElement("btnViewCostedVisits");
	btnViewInvoiceLines = GetElement("btnViewInvoiceLines");
	btnRetract = GetElement("btnRetract");
	//btnViewPrint.disabled = true;
	btnViewCostedVisits.disabled = true;
	btnViewInvoiceLines.disabled = true;

	
	
	selectedInvoiceID = GetQSParam(document.location.search, "invoiceID");
	if(!selectedInvoiceID) selectedInvoiceID = 0;
	listFilterSUReference = GetQSParam(document.location.search, "suRef");
	if(!listFilterSUReference) listFilterSUReference = "";
	listFilterSUName = GetQSParam(document.location.search, "suName");
	if(!listFilterSUName) listFilterSUName = "";
	
	// setup list filters
	listFilter = new Target.Web.ListFilter(ListFilter_Callback);
	listFilter.AddColumn("Invoice Number", GetElement("thInvNumber"), ((listFilterInvNumber.length > 0) ? listFilterInvNumber : null));
	listFilter.AddColumn("Payment Ref", GetElement("thPaymentRef"), ((listFilterPaymentRef.length > 0) ? listFilterPaymentRef : null));
	//listFilter.AddColumn("S/U Reference", GetElement("thSURef"), ((listFilterSUReference.length > 0) ? listFilterSUReference : null));
	listFilter.AddColumn("Service User", GetElement("thSUName"), ((listFilterSUName.length > 0) ? listFilterSUName : null));

	btnRetract.disabled = true;// first disable Retract then populate
	FetchInvoiceList(currentPage, selectedInvoiceID);

	changeUIForPeriodicBlock(currentContractIsPeriodicBlock);
	
}

function ListFilter_Callback(column) {
    var columnName = column.Name
    switch (columnName) {
        case "Invoice Number":
            listFilterInvNumber = column.Filter;
            break;
        case "Payment Ref":
            listFilterPaymentRef = column.Filter;
            break;    
//	    case "S/U Reference":
//			listFilterSUReference = column.Filter;
//			break;
        case "Service User":
			listFilterSUName = column.Filter;
			break;
		default:
			alert("Invalid column filter specified.");
			break;
	}
	FetchInvoiceList(1, 0);
}

function FetchInvoiceList(page, invoiceID) {
    currentPage = page;
	DisplayLoading(true);
	
	providerSvc.FetchDomProviderInvoiceEnquiryResults(
	    currentPage, 
	    providerID, 
	    clientID, 
	    contractID, 
	    invoiceNumber, 
	    weekendingDateFrom, 
	    weekendingDateTo, 
	    unPaid, 
	    paid, 
	    authorised, 
	    suspended, 
	    dateFrom, 
	    dateTo, 
	    invoiceID, 
	    listFilterSUReference,
	    listFilterSUName,
	    listFilterInvNumber,
	    listFilterPaymentRef,
	    false,
	    pScheduleId,
	    hideRetraction,
	    FetchInvoiceList_Callback);
}
// Overload for reTract invoice
function FetchInvoiceList(page, invoiceID, reTractInvoice) {
    currentPage = page;
    DisplayLoading(true);

    providerSvc.FetchDomProviderInvoiceEnquiryResults(
	    currentPage,
	    providerID,
	    clientID,
	    contractID,
	    invoiceNumber,
	    weekendingDateFrom,
	    weekendingDateTo,
	    unPaid,
	    paid,
	    authorised,
	    suspended,
	    dateFrom,
	    dateTo,
	    invoiceID,
	    listFilterSUReference,
	    listFilterSUName,
	    listFilterInvNumber,
	    listFilterPaymentRef,
	    reTractInvoice,
	    pScheduleId,
	    hideRetraction,
	    FetchInvoiceList_Callback);
}

function FetchInvoiceList_Callback(response) {
    var index, str;
    var retractVisible;

    btnViewCostedVisits.disabled = true;
    btnViewInvoiceLines.disabled = true;
//    if (userHasRetractCommand) {
//        btnRetract.disabled = true;
//    }
    
    if (CheckAjaxResponse(response, providerSvc.url)) {
        invoices = response.value.Invoices;
        ClearTable(tblInvoices);
        for (index = 0; index < invoices.length; index++) {
            populating = true;
            //btnViewPrint.disabled = false;

            tr = AddRow(tblInvoices);
            td = AddCell(tr, "");
            radioButton = AddRadio(td, "", "InvoiceSelect", invoices[index].InvoiceID, RadioButton_Click);
            AddInput(td, "hidProviderID", "hidden", "", "", invoices[index].ProviderID, null, null);
            AddInput(td, "hidInvoiceStyleID", "hidden", "", "", invoices[index].InvoiceStyleID, null, null);
            AddInput(td, "hidPaymentScheduleID", "hidden", "", "", invoices[index].PaymentScheduleID, null, null);
            AddInput(td, "hidVoidPayment", "hidden", "", "", invoices[index].VoidPayment, null, null);
            

            AddCell(tr, invoices[index].InvoiceNumber);
            if (!hideContractNumber) {
                AddCell(tr, invoices[index].DomContractNumber);
            }
            if (!hideProviderReference) {
                AddCell(tr, invoices[index].ProviderRefrence);
            }

            td = AddCell(tr, invoices[index].PaymentRef);
            if (GetInnerText(td) == "") SetInnerText(td, " ");

            AddCell(tr, invoices[index].ServiceUserName);

            if (!hideSUReference) {
                AddCell(tr, invoices[index].ServiceUserReference);
            }

            //AddCell(tr, Date.strftime("%d/%m/%Y", invoices[index].Weekending));
            AddCell(tr, Date.strftime("%d/%m/%Y", invoices[index].PeriodFrom));
            AddCell(tr, Date.strftime("%d/%m/%Y", invoices[index].PeriodTo));

            str = invoices[index].DcrRounding;
            if (str == "True") {
                str = "The Duration Claimed value of one or more visit was altered during application of rounding rules"
                imgSrc = SITE_VIRTUAL_ROOT + "Images/rounding.png";
                rounding = "<img align='middle' src='" + imgSrc + "' alt='" + str + "' style='margin-left:5px;'  />";
            }
            else {
                str = "";
                rounding = "";
            }   

            td = AddCell(tr, "");
            td.innerHTML = "<div style='float:left;'>" + invoices[index].NetPayment.toString().formatCurrency() + "</div>";
            td.innerHTML += rounding;
            
//            //str = invoices[index].PaymentRef;
//            if (!str || str.length == 0) str = " ";
//            AddCell(tr, str);
            
            AddCell(tr, invoices[index].InvoiceStatus);

            if (selectedInvoiceID == invoices[index].InvoiceID || ( currentPage == 1 && invoices.length == 1)) {
                radioButton.click();
            }
        }

        populating = false;

        // load the paging link HTML
        divPagingLinks.innerHTML = response.value.PagingLinks;
    }

    if (userHasRetractCommand && selectedInvoiceID > 0) {
        if (response.value.RetractSuccessful) {
            alert("The Provider invoice has been retracted.");
        }
    }
    
    DisplayLoading(false);
}


function RadioButton_Click() {
	var x
	var cell, Radio, input, style, voidPayment;
	for (x = 0; x < tblInvoices.tBodies[0].rows.length; x++){
	    cell = tblInvoices.tBodies[0].rows[x].childNodes[0];
		Radio = cell.getElementsByTagName("INPUT")[0];
		input = cell.getElementsByTagName("INPUT")[1];
		style = cell.getElementsByTagName("INPUT")[2];
		if (Radio.checked) {
			tblInvoices.tBodies[0].rows[x].className = "highlightedRow"
			selectedInvoiceID = Radio.value;
			selectedProviderID = input.value;
			pScheduleId = tblInvoices.tBodies[0].rows[x].cells[0].children[3].value;
			voidPayment = tblInvoices.tBodies[0].rows[x].cells[0].children[4].value;


            if (style.value == "3") {
			    btnViewCostedVisits.disabled = false;
			}
			btnViewInvoiceLines.disabled = false;
			if (userHasRetractCommand) {
			    SetRetractVisibility()
			}
			// if invoice is void payment the disable Retract
			if (voidPayment == "true") {
			    btnRetract.disabled = true;
            }
		} else {
			tblInvoices.tBodies[0].rows[x].className = ""
		}
	}
}

function SetRetractVisibility() {
    if (selectedInvoiceID > 0) {
        var response = providerSvc.FetchRetractProviderInvoiceVisibility(selectedInvoiceID);
        if (response.value.ErrMsg.Success = true) {
            if (!response.value.Visibility) {
                btnRetract.disabled = true;
            }
            else {
                btnRetract.disabled = false;
            }
        }
    }
}

function GetBackUrl() {
    var url = document.location.href;
    return escape(url);
}

function btnViewVisits_Click() {
    RemoveFilter();
    document.location.href = "ViewInvoiceCostedVisits.aspx?psview=" + showPaymentSchedule + "&pScheduleId=" + pScheduleId + "&id=" + selectedInvoiceID;
}

function btnViewInvoiceLines_Click() {
    RemoveFilter();
    document.location.href = "ViewInvoiceLines.aspx?psview=" + showPaymentSchedule + "&pScheduleId=" + pScheduleId + "&id=" + selectedInvoiceID + "&estabID=" + selectedProviderID;
}

function btnRetract_Click() {
    var shouldRetract = true;
    var msgPrompt = "";
    var index, invNumber = "", serviceUserName = "", serviceUserRef = "";

    for (index = 0; index < invoices.length; index++) {
        if (selectedInvoiceID == invoices[index].InvoiceID) {
            invNumber = invoices[index].InvoiceNumber;
            serviceUserName = invoices[index].ServiceUserName;
            serviceUserRef = invoices[index].ServiceUserReference;

            break;
        }
    }
    
    msgPrompt = "Upon confirmation, Abacus will attempt to mark the selected Provider Invoice ";
    msgPrompt = msgPrompt + "as retracted and create a contra copy of the Provider Invoice; ";
    msgPrompt = msgPrompt + "it is not possible to undo this action.\r\n\r\n";
    msgPrompt = msgPrompt + "Are you sure you wish to retract the Provider Invoice ";
    msgPrompt = msgPrompt + invNumber + " for " + serviceUserName + " (" + serviceUserRef + ") ?";

    shouldRetract = confirm(msgPrompt) == true;
    if (shouldRetract) {
        FetchInvoiceList(currentPage, selectedInvoiceID, true);
    }
}

function RemoveFilter() {
    var url = document.location.href;
    url = RemoveQSParam(url, "suRef");
    url = RemoveQSParam(url, "suName");
    document.location.replace(url);
}


function changeUIForPeriodicBlock(currentContractIsPeriodicBlock) {
    if (currentContractIsPeriodicBlock === true) {
        var columnHeading = $("#periodicSU");
        if (columnHeading[0]) {
            columnHeading[0].innerText = "System Account";
        }

        columnHeading = $("#periodicSURef");
        if (columnHeading[0]) {
            columnHeading[0].innerText = "System Ref";
        }

        columnHeading = $("#periodicServiceUser");
        if (columnHeading[0]) {
            columnHeading[0].innerText = "System Account";
        }

        columnHeading = $("#periodicSUReference");
        if (columnHeading[0]) {
            columnHeading[0].innerText = "System Ref";
        }
    }

}

addEvent(window, "load", Init);
