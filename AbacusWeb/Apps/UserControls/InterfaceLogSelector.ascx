<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InterfaceLogSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.InterfaceLogSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<table class="listTable" id="tblInterfaceLogs" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="<% Response.Write(TableSummary) %>">
<caption id="tblInterfaceLogsCaption" runat="server">
    <% Response.Write(TableCaption)%>
</caption>
<thead>
	<tr valign="top">
		<th style="width:1.5em;"></th>
		<th id="thInterfaceLogCreatedDate" style="width:10%;">Created</th>
		<th id="thInterfaceLogCreatedBy" style="width:20%;">Created<br />By</th>
        <th id="thInterfaceLogBatchReference" style="width:10%">Batch<br />Ref.</th>
		<th id="thInterfaceLogPaymentCount" style="width:10%;">Payment<br />Count</th>
		<th id="thInterfaceLogNetValue" style="width:10%;">Net Value</th>
		<th id="thInterfaceLogVat" style="width:10%;">VAT</th>
		<th id="thInterfaceLogTotalValue" style="width:10%;">Total Value</th>
		<th id="thInterfaceLogJobStatus" style="width:20%;">Job Status</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="InterfaceLog_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>