<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ExpenditureAccountSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ExpenditureAccountSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<br />
<cc1:DropDownListEx ID="cboExpAccountType" runat="server" LabelText="Account Type" LabelWidth="8em" Width="11em" ></cc1:DropDownListEx>
<table class="listTable sortable" id="tblExpenditureAccounts" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of Expenditure Accounts.">
<caption>
    <div class="caption">List of expenditure accounts.</div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thDescription">Description</th>
		<th>Type</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="ExpAccount_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>
