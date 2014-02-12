<%@ Control Language="vb" AutoEventWireup="false" Codebehind="RemittanceDetailSelector.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.RemittanceDetailSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListDetailLines" style="table-layout:fixed; float:left;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available remittance detail lines.">
<caption>List of available remittance detail lines.</caption>
<thead>
	<tr>
		<th style="width:13%;">Reference</th>
		<th style="width:52%;">Description</th>
		<th style="width:8%;">Amount</th>
		<th style="width:12%;">Our Ref</th>
		<th style="width:15%;">Your Ref</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Detail_PagingLinks" style="float:left;"></div>
<!--
<input type="button" id="btnDownload" style="float:right;width:10em;" value="Download Custom" title="Click here to download the selected interface file in the custom Comma Separated Values (CSV) format." onclick="btnDownload_OnClick();" />
-->
<input type="button" id="btnStatement" style="float:right;width:12em;" value="View/Print Statement" title="Click here to view/print a statement of these payments." onclick="btnStatement_OnClick();" />
<div class="clearer"></div>
<br />