<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ServiceSelector.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.ServiceSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListService" style="table-layout:fixed; float:left;"  cellpadding="2" cellspacing="0" width="100%" >
<caption>List of Services.</caption> 
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRef" style="width:12%;">Reference</th>
		<th id="thName" style="width:25%;">Name</th>
		<th style="width:38%;">Description</th>
		<th style="width:25%;">Type</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Service_PagingLinks" style="float:left;"></div>
<input type="button" id="btnViewService" value="View Service" style="float:right;margin-bottom:0.5em;" title="Display the selected Service" runat="server" onclick="viewServiceButton_Click()" />
<div class="clearer"></div>
