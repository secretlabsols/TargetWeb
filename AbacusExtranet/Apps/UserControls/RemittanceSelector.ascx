<%@ Control Language="vb" AutoEventWireup="false" Codebehind="RemittanceSelector.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.RemittanceSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListRemittances" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available remittances.">
<caption>List of available remittances.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th style="width:10%;">Reference</th>
		<th style="width:38%;">Home</th>
		<th>Date From</th>
		<th>Date To</th>
		<th>Amount</th>
		<th>Paid Date</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Remittance_PagingLinks" style="float:left;"></div>
<input type="button" id="btnViewRemittance" value="View/Print Remittance" style="float:right; margin-bottom:4px;" 
title="Display the selected remittance" onclick="btnViewRemittance_Click()" />
<div class="clearer"></div>
