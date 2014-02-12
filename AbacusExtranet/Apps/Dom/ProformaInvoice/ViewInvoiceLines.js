
var contractSvc, currentPage, invoiceID;
var selectedInvoiceLineID, commentDesc;
var tblInvoiceLines, divPagingLinks;

function Init() {
	contractSvc = new Target.Abacus.Extranet.Apps.WebSvc.DomContract_class();
	tblInvoiceLines = GetElement("tblInvoiceLines");
	divPagingLinks = GetElement("Invoice_PagingLinks");
	
    FetchInvoiceLineList(currentPage);
}
function FetchInvoiceLineList(page) {
    currentPage = page;
	DisplayLoading(true);
	
	contractSvc.FetchDomProformaInvoiceLines(currentPage, invoiceID, FetchInvoiceLineList_Callback);
}
function FetchInvoiceLineList_Callback(response) {
    var index, invoices, str;
    
    if(CheckAjaxResponse(response, contractSvc.url)) {
        invoices = response.value.InvoiceLines;
        ClearTable(tblInvoiceLines);     
        for(index=0; index<invoices.length; index++) {
        
            tr = AddRow(tblInvoiceLines);

            AddCell(tr, Date.strftime("%d/%m/%Y", invoices[index].WETo));
			AddCell(tr, invoices[index].RateCode);
			AddCell(tr, invoices[index].RateCategory);
			
			td = AddCell(tr, "");
			td.innerHTML = invoices[index].UnitCost.toString().formatCurrency();
			
			AddCell(tr, invoices[index].NetUnitsPaid);
			
			td = AddCell(tr, "");
			td.innerHTML = invoices[index].NetPayment.toString().formatCurrency();
		
			if (invoices[index].Comment != null) {
			    commentDesc = invoices[index].Comment
			    td = AddCell(tr, "");
			    link = AddLink(td, "View Comment", "javascript:ShowComment();", "Click here to view this comment.");
				link.className = "transBg";
			} else {
			    AddCell(tr, " ");
			}

        }
        
        // load the paging link HTML
		divPagingLinks.innerHTML = response.value.PagingLinks;
    }
    DisplayLoading(false);
}

//function GetBackUrl() {
//    var url = document.location.href;
//    url = AddQSParam(RemoveQSParam(url, "invoiceID"), "invoiceID", selectedInvoiceID);
//    return escape(url);
//}


function ShowComment() {
    var d = new Apps.Dom.ProformaInvoice.Comment.Dialog("Comment", "Comment:", commentDesc);
    d.SetCallback(ShowQuery_Callback)
    d.SetType(1);
    d.Show();
}
function ShowQuery_Callback(evt, args) {
    var d = args[0];
    d.Hide();
}

function btnBack_Click() {
    var url = GetQSParam(document.location.search, "backUrl");
    url = unescape(url);
    document.location.href = url;
}

addEvent(window, "load", Init);
