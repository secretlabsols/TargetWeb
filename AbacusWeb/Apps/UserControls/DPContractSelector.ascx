<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DPContractSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DPContractSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<table class="listTable" id="DPContractSelector_tblContracts" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available contracts.">
<caption>
    <div class="caption">List of available Direct Payment contracts.</div>
    <div class="clearer"></div>
</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thSUName" style="width:19%">Service User</th>
		<th id="thSURef" style="width:10%" >S/U Ref</th>
		<th id="thBHName" style="width:24%">Budget Holder</th>
		<th id="thBHRef" style="width:10%" >B/H Ref</th>
		<th id="thNumber" style="width:10%" >Contract</th>
		<th style="width:10%">Date From</th>
		<th style="width:10%">Date To</th>
		<th style="width:5%">SDS</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="DPContract_PagingLinks" style="float:left;"></div>
<input type="button" id="DPContractSelector_btnCreatePayments" runat="server" style="float:right;width:9em;" value="Create Payments" onclick="DPContractSelector_btnCreatePayments_Click();" />
<input type="button" id="DPContractSelector_btnTerminate" runat="server" style="float:right;width:7em;" value="Terminate" onclick="DPContractSelector_btnTerminate_Click();" />
<input type="button" id="DPContractSelector_btnReinstate" runat="server" style="float:right;width:7em;" value="Re-instate" onclick="DPContractSelector_btnReinstate_Click();" />
<input type="button" id="DPContractSelector_btnCopy" runat="server" style="float:right;width:7em;" value="Copy" onclick="DPContractSelector_btnCopy_Click();" />
<input type="button" id="DPContractSelector_btnView" runat="server" style="float:right;width:5em;" value="View" onclick="DPContractSelector_btnView_Click();" />
<input type="button" id="DPContractSelector_btnNew" runat="server" style="float:right;width:5em;" value="New" onclick="DPContractSelector_btnNew_Click();" />
<div class="clearer"></div>
