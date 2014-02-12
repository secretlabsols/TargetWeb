<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DomProformaInvoiceSelector.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.DomProformaInvoiceSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>

<table class="listTable" id="tblBatches" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available proforma invoices.">
<caption>List of available pro forma invoices.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th style="width:12%;">Reference</th>
		<th style="width:9em;">Contract No.</th>
		<th>Contract Title</th>
		<th style="width:18%;">Status</th>
		<th style="width:12%;">Status Date</th>
		<th style="width:13%;">Net Payment</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Invoice_PagingLinks" style="float:left;"></div>
<div style="float:right;">
    <input type="button" style="width:7em;" id="btnViewContract" runat="server" value="View Contract" title="View the contract details of the selected invoice batch" onclick="btnViewContract_Click();" />
    <input type="button" style="width:7em;" id="btnViewBatch" runat="server" value="View Batch" title="View the pro forma batch details" onclick="btnViewBatch_Click();" />
    <input type="button" style="width:7em;" id="btnViewInvoices" runat="server" value="View Invoices" title="View the pro forma invoice details" onclick="btnViewInvoices_Click();" />
</div>
<div class="clearer"></div>
