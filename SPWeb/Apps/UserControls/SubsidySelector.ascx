<%@ Control Language="vb" AutoEventWireup="false" Codebehind="SubsidySelector.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.SubsidySelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListSubsidies" style="table-layout:fixed; float:left;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available subsidies.">
<caption>List of Subsidies.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRef" style="width:14%;">Svc User Ref</th>
		<th id="thName" style="width:26%;">Name</th>
		<th id="thDateFrom" style="width:12%;">Date From</th>
		<th style="width:12%;">Date To</th>
		<th style="width:10%;">Subsidy</th>
		<th style="width:12%;">Provider Ref</th>
		<th style="width:6%;">Level</th>
		<th style="width:8%;">Status</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Subsidy_PagingLinks" style="float:left;"></div>
<input type="button" id="btnViewSubsidy" value="View Subsidy" style="float:right;" title="Display the selected subsidy" onclick="btnViewSubsidy_Click()" />
<br />
<div id="clearer"></div>
<br />