<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DpContractTerminationMonitorResults.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DpContractTerminationMonitorResults" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<label id="DpContractTerminationMonitorResults_Error" class="errorText" style="background-color: Transparent"
    runat="server" />

<table class="listTable" id="DpContractTerminationMonitorResults_List" style="table-layout:fixed;margin-top:7px;"  cellpadding="2" cellspacing="0" width="100%">
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="DpContractTerminationMonitorResults_thServiceUserName">Service User</th>		
		<th id="DpContractTerminationMonitorResults_thServiceUserRef">S/U Ref</th>	
		<th id="DpContractTerminationMonitorResults_thBudgetHolderName">Budget Holder</th>	
		<th id="DpContractTerminationMonitorResults_thBudgetHolderRef">B/H Ref</th>	
		<th id="DpContractTerminationMonitorResults_thContractNumber">Contract</th>	
		<th id="DpContractTerminationMonitorResults_thDateFrom">Date From</th>	
		<th id="DpContractTerminationMonitorResults_thReqEndDate">Required End Date</th>	
		<th id="DpContractTerminationMonitorResults_thUnderOver">Under/Over (£)</th>	
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="DpContractTerminationMonitorResults_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>
<div style="float : right; margin-bottom:5px;" >
    <uc1:ReportsButton id="DpContractTerminationMonitorResults_btnPrint" ButtonText="Print" runat="server" ButtonWidth="5em" ButtonHeight="24px" />
    <input type="button" onclick="DpContractTerminationMonitorResults_Balance();" value="Balance" id="DpContractTerminationMonitorResults_btnBalance" style="width:6em;" />
    <input type="button" onclick="DpContractTerminationMonitorResults_View();" value="View" id="DpContractTerminationMonitorResults_btnView" style="width:6em;" />
</div>
<div class="clearer"></div>
