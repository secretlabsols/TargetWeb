<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ServiceOrderSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ServiceOrderSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc1" TagName="CareTypeSelector" Src="~/AbacusWeb/Apps/UserControls/CareTypeSelector.ascx" %>
<table class="listTable" id="ServiceOrderSelector_tblOrders" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of service orders.">
<caption>List of service orders.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th style="width:15%" id="thSvcUserName">Service User</th>
		<th style="width:14%" id="thSvcUserRef">S/U Ref</th>
		<th style="width:20%" id="thProvider">Provider</th>
		<th style="width:10%" id="thContract">Contract</th>
		<th style="width:10%" id="thOrderRef">Order Ref</th>
		<th style="width:10%">Date From</th>
		<th style="width:10%">Date To</th>
		<th style="width:10%" id="thServiceGrp">Service Grp</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="serviceOrder_PagingLinks" style="float:left;"></div>
<input type="button" id="ServiceOrderSelector_btnView" runat="server" style="float:right;width:5em;" value="View" onclick="ServiceOrderSelector_btnView_Click();" />
<input type="button" id="ServiceOrderSelector_btnNew" runat="server" style="float:right;width:5em;" value="New" onclick="ServiceOrderSelector_btnNew_Click();" />
<input type="button" id="ServiceOrderSelector_btnCopy" runat="server" style="float:right;width:5em;" value="Copy" onclick="ServiceOrderSelector_btnCopy_Click();" />
<div class="clearer"></div>
<uc1:CareTypeSelector id="cTypeSelector" runat="server"></uc1:CareTypeSelector>
