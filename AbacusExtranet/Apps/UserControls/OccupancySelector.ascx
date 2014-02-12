<%@ Control Language="vb" AutoEventWireup="false" Codebehind="OccupancySelector.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.OccupancySelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListOccupancy" style="table-layout:fixed; width:100%"  cellpadding="2" cellspacing="0" width="100%" summary="Occupancy list for selected home.">
<caption>Occupancy list for the selected home.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th style="width:15%;">Reference</th>
		<th>Name</th>
		<th style="width:20%;">Date From</th>
		<th style="width:20%;">Date To</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Detail_PagingLinks" style="float:left;"></div>
<input type="button" id="btnViewServiceUser" value="View Service User" 
style="float:right; width:11.6em; margin-bottom: 4px;" title="View details of the selected service user" onclick="btnViewServiceUser_Click()" />
<div class="clearer"></div>
