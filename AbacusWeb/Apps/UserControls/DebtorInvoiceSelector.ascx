<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DebtorInvoiceSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DebtorInvoiceSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table class="listTable" id="tblDebtorInvoices" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of debtor invoices.">
<caption>List of debtor invoices.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thDebtor" style="width:16%;">Debtor</th>
		<th id="thRef" style="width:8%;">Ref</th>
		<th id="thInvNo" style="width:10%;">Inv No</th>
        <th style="width:18%;">Inv Type</th>
        <th style="width:10%;">Inv Total</th>
        <th style="width:11%;">Inv Date</th>
        <th style="width:9%;">Status</th>
        <th style="width:9%;">Batched</th>
        <th style="width:9%;">Exclude</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="DebtorInvoice_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>
