<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ProviderSelector.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.ProviderSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListProvider" style="table-layout:fixed; float:left;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available providers.">
<caption>List of available providers.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRef" style="width:12%;">Reference</th>
		<th id="thName" style="width:38%;">Name</th>
		<th style="width:40%;">Address</th>
		<th style="width:10%;">Postcode</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Provider_PagingLinks" style="float:left;"></div>
<input type="button" id="btnViewProvider" value="View Provider" style="float:right;margin-bottom:0.5em;" title="Display the selected provider" runat="server" onclick="viewProviderButton_Click()" />
<div class="clearer"></div>
