<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SpendPlanSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.SpendPlanSelector" %> 

<table class="listTable" id="SpendPlanSelector_tblSpendPlans" style="table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%" summary="List of spend plans.">
<caption>List of spend plans.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<%If showServiceUserColumns Then%>
            <th id="SpendPlanSelector_thSvcUserName">Service User</th>
            <th id="SpendPlanSelector_thSvcUserRef">S/U Ref</th>
		<%End If%>
		<th id="SpendPlanSelector_thOrderRef">Reference</th>
		<th>Date From</th>
		<th>Date To</th>
		<th>Service Delivered Via</th>
		<th>Gross Cost</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="SpendPlan_PagingLinks" style="float:left;"></div>
<br />
<div style="margin-bottom : 30px;">
    <div runat="server" id="SpendPlanSelector_divLegendLatestCost" class="warningText" style="background-color:transparent;display:block;">
        The current cost for the Spend Plan is shown in the "Gross Cost" column
        <br />
    </div>
    <div id="SpendPlanSelector_divLegendGrossCostRequireRecalculation" class="errorText transBg" style="background-color:transparent;display:none;">
        Gross Costs highlighted in red text require recalculation
        <br />
    </div>
    <input type="button" id="SpendPlanSelector_btnView" style="float:right;width:5em;" value="View" onclick="SpendPlanSelector_btnView_Click();" />
    <input type="button" id="SpendPlanSelector_btnNew" runat="server" style="float:right;width:5em;" value="New" onclick="SpendPlanSelector_btnNew_Click();" />
    <div class="clearer"></div>
</div>
<div class="clearer"></div>
