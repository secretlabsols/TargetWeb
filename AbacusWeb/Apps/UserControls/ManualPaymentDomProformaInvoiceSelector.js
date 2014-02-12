
var contractSvc, currentPage, providerID, contractID, systemAccountID, selectedInvoiceID;
var btnViewID;
var ManualPaymentDomProformaInvoiceStep_required;
var tblInvoices, divPagingLinks, btnView, btnNext, btnFinish;

function Init() {
	contractSvc = new Target.Abacus.Web.Apps.WebSvc.DomContract_class();
	tblInvoices = GetElement("tblInvoices");
	divPagingLinks = GetElement("divPagingLinks");
	btnView = GetElement(btnViewID, true);
	btnNext = GetElement("SelectorWizard1_btnNext", true);
	btnFinish = GetElement("SelectorWizard1_btnFinish", true);
	
	if(btnView) btnView.disabled = true;
			
	// populate table
	FetchInvoiceList(currentPage);
}

function FetchInvoiceList(page) {
	currentPage = page;
	DisplayLoading(true);
	contractSvc.FetchManualPaymentDomProformaList(page, providerID, contractID, systemAccountID, selectedInvoiceID, FetchInvoiceList_Callback)
}

function FetchInvoiceList_Callback(response) {
	var invoices, index;
	var tr, td, radioButton;
	var str;
	var link;
	
	if(selectedInvoiceID == 0) {
		if(btnView) btnView.disabled = true;
	}
	// disable next/finish buttons by default if the step is mandatory
	if(ManualPaymentDomProformaInvoiceStep_required) {
		if(btnNext) btnNext.disabled = true;
		if(btnFinish) btnFinish.disabled = true;
	}
	
	if(CheckAjaxResponse(response, contractSvc.url)) {
		
		// populate the table
		invoices = response.value.Invoices;
		
		// remove existing rows
		ClearTable(tblInvoices);
		for(index=0; index<invoices.length; index++) {

			tr = AddRow(tblInvoices);
			td = AddCell(tr, "");			
			radioButton = AddRadio(td, "", "DomProformaInvoiceSelect", invoices[index].InvoiceID, RadioButton_Click);
			
			str = invoices[index].Provider;
			if(!str || str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			str = invoices[index].ContractNo;
			if(!str || str.length == 0) str = " ";
			td = AddCell(tr, str);	
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
					
			str = invoices[index].SystemAccount;
			if(!str || str.length == 0) str = " ";
			td = AddCell(tr, str);
			td.style.textOverflow = "ellipsis";
			td.style.overflow = "hidden";
			
			AddCell(tr, Date.strftime("%d/%m/%Y", invoices[index].WETo));
			
			str = invoices[index].Reference;
			if(!str || str.length == 0) str = " ";
			AddCell(tr, str);
			
			td = AddCell(tr, " ");
			td.innerHTML = invoices[index].CalculatedPayment.toString().formatCurrency();
						
			if(invoices[index].ID == selectedInvoiceID || (currentPage == 1 && invoices.length == 1))
			    radioButton.click();
			
		}
		// load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
	}
	DisplayLoading(false);
}
function RadioButton_Click() {
	var index, rdo, selectedRow;
	
	for (index = 0; index < tblInvoices.tBodies[0].rows.length; index++){
		rdo = tblInvoices.tBodies[0].rows[index].cells[0].getElementsByTagName("INPUT")[0];
		if (rdo.checked) {
			selectedRow = tblInvoices.tBodies[0].rows[index];
			tblInvoices.tBodies[0].rows[index].className = "highlightedRow"
			selectedInvoiceID = rdo.value;
			if(btnView) btnView.disabled = false;
		} else {
			tblInvoices.tBodies[0].rows[index].className = ""
		}
	}
	if(ManualPaymentDomProformaInvoiceStep_required) {
		if(btnNext) btnNext.disabled = false;
		if(btnFinish) btnFinish.disabled = false;
	}
}
function btnNew_Click() {
	var qs = document.location.search;
	qs = RemoveQSParam(qs, "currentStep");
	document.location.href = "EnterManualPayment.aspx" + qs + "&mode=2&backUrl=" + GetBackUrl();
}
function btnView_Click() {
	document.location.href = "EnterManualPayment.aspx?id=" + selectedInvoiceID + "&mode=1&backUrl=" + GetBackUrl();
}
function GetBackUrl() {
	var url = document.location.href;
	return escape(url);
}

function ManualPaymentDomProformaInvoiceStep_BeforeNavigate() {

	var url = SelectorWizard_GetNewUrl(document.location.href, SelectorWizard_currentStep + 1);
	
	url = AddQSParam(RemoveQSParam(url, "id"), "id", selectedInvoiceID);
	SelectorWizard_newUrl = url;
	return true;
}

addEvent(window, "load", Init);
