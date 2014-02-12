<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RemittanceSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.Batches.RemittanceSelector" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<table class="listTable" id="RemittanceSelector_tblRemittances" style="table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%" summary="List of remittances.">
<caption>List of remittances.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
        <th id="RemittanceSelector_thCreditorRef" >Creditor Ref</th>
        <th id="RemittanceSelector_thCreditorName" >Creditor Name</th>
		<th>Date From</th>
		<th>Date To</th>
		<th>Total</th>
		<th>Type</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Remittance_PagingLinks" style="float:left;"></div>
<br />
<div style="float:right;" ><uc2:ReportsButton id="btnView" disabled="False" runat="server" ButtonText="View"></uc2:ReportsButton></div>
<div class="clearer"></div>
