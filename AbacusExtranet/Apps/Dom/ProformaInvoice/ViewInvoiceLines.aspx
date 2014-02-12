<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ViewInvoiceLines.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.ViewInvoiceLines" %>

<%@ Register TagPrefix="uc1" TagName="BreadCrumb" Src="~/AbacusExtranet/Apps/Dom/ProformaInvoice/InvoiceBatchBreadcrumb.ascx" %>

<%@ Register TagPrefix="uc2" TagName="InvHeader" Src="~/AbacusExtranet/Apps/UserControls/InvoiceHeader.ascx" %>

<asp:content contentplaceholderid="MPPageOverview" runat="server">
		Displayed below are the invoice details for the selected  Pro forma Invoice.
	</asp:content>
<asp:content contentplaceholderid="MPContent" runat="server">
    
    <div>
	    <%--<uc1:BreadCrumb id="breadCrumb" runat="server"></uc1:BreadCrumb>--%>
	    <%--<input type="button" id="btnBack" value="Back" style="width:5em;" onclick="btnBack_Click();" />--%>
	    <input type="button" id="btnBack" runat="server" value="Back" style="width:5em;" />
	</div>  
	
	<div style="margin-top:5px;">
	        <uc2:InvHeader id="invoiceHeader" runat="server"></uc2:InvHeader>
    </div>        
	<div style="margin-top:5px;">   
        <table class="listTable" id="tblInvoiceLines" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available pro forma invoices.">
            <caption>List of invoice lines.</caption>
            <thead>
	            <tr>
	                <th>Week Ending</th>
		            <th>Rate Code</th>
		            <th>Rate Category</th>
		            <th style="width:6em;">Unit Cost</th>
		            <th style="width:9em;">Net Units Paid</th>
		            <th style="width:8em;">Net Payment</th>
		            <th>Comment</th>
	            </tr>
            </thead>
            <tbody>
                <tr>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                 </tr>
            </tbody>
        </table>
        <div id="Invoice_PagingLinks" style="float:left;"></div>
        <div class="clearer"></div>
    </div>
</asp:content>
