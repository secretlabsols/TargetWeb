<%@ Control Language="vb" AutoEventWireup="false" Codebehind="RemittanceDetailSelector.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.RemittanceDetailSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListDetailLines" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available remittance detail lines.">
<caption>List of available remittance detail lines.</caption>
<thead>
	<tr>
		<th>Reference</th>
		<th>Date From</th>
		<th>Date To</th>
		<th style="text-align:center">Weeks/Days</th>
		<th>Gross Rate</th>
		<th style="width:14%;">Direct Income</th>
		<th>NET Rate</th>
		<th>Amount</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Detail_PagingLinks" style="float:left;"></div>
<input type="button" id="btnStatement" style="float:right;width:11.6em; margin-bottom:4px;"
 value="View/Print Statement" title="Click here to view/print a statement of these payments." onclick="btnStatement_OnClick();" />
<div class="clearer"></div>
