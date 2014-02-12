<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ClientSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ClientSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<style type="text/css">
    .style1
    {
        width: 106px;
    }
    .style2
    {
        width: 33%;
    }
</style>

<table class="listTable sortable" id="tblClients" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of service users.">
<caption>
    <div class="caption">List of service users.</div>
    <div class="mruList">
        <cc1:MruList ID="mru" runat="server" MruListKey="SERVICE_USERS" BlankItemText="[Recent Service Users]" ClientOnChange="ClientSelector_MruOnChange"></cc1:MruList>
    </div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRef" style="width:15%">Reference</th>
		<% If  HideDebtorRef() Or HideCreditorRef() Then%>
		<th id="thName" style="width:25%">Name</th>
		<% Else %>
		<th id="thName" style="width:15%">Name</th>
		<% End If %>
		<th class="style1">Date of Birth</th>
		<th>Address</th>
		<%If Not HideDebtorRef() Then %>
		<th id="thDebtorRef" style="width:15%">Debtor Ref</th>
		<%End if %>
		<%If Not HideCreditorRef() Then %>
		<th id="thCreditorRef" style="width:15%">Creditor Ref</th>    
		<%End if %>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td class="style2"></td><td class="style1"></td><td></td></tr></tbody>
</table>
<div id="Client_PagingLinks" style="float:left;"></div>
<input type="button" id="btnView" style="float:right;width:5em;" runat="server" value="View" onclick="btnView_Click();" />
<input type="button" id="btnBudgetPeriods" style="float:right;width:8em;" runat="server" value="Budget Periods" onclick="btnViewBudgetPeriods_Click();" />
<div class="clearer"></div>
