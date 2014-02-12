<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GenericCreditorSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.GenericCreditorSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<table class="listTable" id="tblGenericCreditors" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of creditors.">
<caption>
    <div class="caption">List of creditors.</div>
    <div class="mruList">
        <cc1:MruList ID="mru" runat="server" MruListKey="GENERIC_CREDITORS" BlankItemText="[Recent Creditors]" ClientOnChange="GenericCreditorSelector_MruOnChange" />
    </div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thGenericCreditorReference" style="width:10%;">Reference</th>
		<th id="thGenericCreditorName" style="width:25%;">Name</th>
		<th id="thGenericCreditorType" style="width:15%;">Type</th>
		<th id="thGenericCreditorAddress" style="width:50%;">Address</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="GenericCreditors_PagingLinks" style="float:left;"></div>
<input type="button" id="GenericCreditors_btnView" runat="server" style="float:right;width:5em;" value="View" onclick="GenericCreditorSelector_btnView_Click();" />
<div class="clearer"></div>