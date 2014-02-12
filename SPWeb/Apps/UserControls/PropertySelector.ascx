<%@ Control Language="vb" AutoEventWireup="false" Codebehind="PropertySelector.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.PropertySelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable sortable" id="ListProperty" style="table-layout:fixed; float:left;"  cellpadding="2" cellspacing="0" width="100%" summary="List of property.">
<caption>List of Properties.</caption> 
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th style="width:10%;">Reference</th>
		<th style="width:10%;">Alt Reference</th>
		<th style="width:35%;">Name</th>
		<th style="width:35%;">Address</th>
		<th style="width:10%;">Postcode</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Property_PagingLinks" style=""></div>
<input type="button" value="View Property" style="float:right;" title="Display the selected property" onclick="viewPropertyButton_Click()" />
<br />
<br />