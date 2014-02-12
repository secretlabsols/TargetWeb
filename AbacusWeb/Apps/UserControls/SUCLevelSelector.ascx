<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SUCLevelSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.SUCLevelSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%> 
<table class="listTable" id="SUCLevelSelector_tblSUCLevelSelectors" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of service user contribution level.">
<caption>List of service user contribution levels</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thDateFrom" width="12.5%">Date From</th>
		<th id="thDateTo" width="12.5%">Date To</th>
		<th id="thAssessmentType" width="25%">Assessment Type</th>
		<th id="thAssessedCharge" width="12.5%">Assessment</th>
		<th id="thChargeableCost" width="12.5%">Chargeable</th>
		<th id="thContributionLevel" width="12.5%">Level</th>
		<th id="thPlannedAdditionalCost" width="12.5%">Additional</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td></tr></tbody>
</table>
<div id="SUCLevelSelector_PagingLinks" style="float:left;"></div>
<%--<input type="button" id="SUCLevelSelector_btnView" style="float:right;width:5em;" value="View" onclick="SUCLevelSelector_btnView_Click();" />--%>
<input type="button" id="SUCLevelSelector_btnNew" runat="server" style="float:right;width:5em;" value="New" onclick="SUCLevelSelector_btnNew_Click();" />
<div class="clearer"></div>
