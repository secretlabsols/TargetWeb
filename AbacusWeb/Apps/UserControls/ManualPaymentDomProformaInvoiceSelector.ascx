<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ManualPaymentDomProformaInvoiceSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ManualPaymentDomProformaInvoiceSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable" id="tblInvoices" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of domiciliary pro forma invoices.">
<caption>List of domiciliary pro forma invoices.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th style="width:20%">Provider</th>
		<th style="width:10%">Contract</th>
		<th style="width:25%;">System Account</th>
		<th style="width:15%;">Week Ending</th>
		<th style="width:15%;">Payment Ref</th>
		<th style="width:15%;">Net Payment</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="divPagingLinks" style="float:left;"></div>
<input type="button" id="btnView" runat="server" style="float:right;width:5em;" value="View" onclick="btnView_Click();" />
<input type="button" id="btnNew" runat="server" style="float:right;width:5em;" value="New" onclick="btnNew_Click();" />
<div class="clearer"></div>
