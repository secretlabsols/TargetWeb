<%@ Control Language="vb" AutoEventWireup="false" Codebehind="EstablishmentSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.EstablishmentSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<table class="listTable sortable" id="tblEstablishments" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available providers.">
<caption>
    <div class="caption">List of available providers.</div>
    <div class="mruList">
        <cc1:MruList ID="mru" runat="server" MruListKey="PROVIDERS" BlankItemText="[Recent Providers]" ClientOnChange="EstablishmentSelector_MruOnChange"></cc1:MruList>
    </div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRef" style="width:12%;">Reference</th>
		<th id="thName" style="width:30%;">Name</th>
		<th style="width:58%;">Address</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Establishment_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>
