<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ThirdPartySelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ThirdPartySelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<table class="listTable sortable" id="tblTPs" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of client third parties.">
<caption>
    <div class="caption">List of third parties.</div>

    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thTitle" style="width:12%">Title</th>
		<th id="thSurname" style="width:40%">Surname</th>
		<th>Address</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="TP_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>
