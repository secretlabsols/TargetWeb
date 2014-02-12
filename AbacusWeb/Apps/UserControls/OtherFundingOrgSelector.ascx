<%@ Control Language="vb" AutoEventWireup="false" Codebehind="OtherFundingOrgSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.OtherFundingOrgSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<table class="listTable sortable" id="tblOrganizations" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of Organizations.">
<caption>
    <div class="caption">List of organizations.</div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thName" style="width:40%">Name</th>
		<th>Address</th>
		<th>Postcode</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Organization_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>
