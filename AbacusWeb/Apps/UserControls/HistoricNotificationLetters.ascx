<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="HistoricNotificationLetters.ascx.vb" 
Inherits="Target.Abacus.Web.Apps.UserControls.HistoricNotificationLetters"  
TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>


<table class="listTable" id="HistoricNotificationLetters_ResultsTable" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List historic notification documents.">
<caption>List of historic notification letters.</caption>
<thead>
	<tr>	    
		<th style="width:1.5em;"></th>
		<%--<th width="20px">&nbsp;</th>--%>
		<th id="thDateFrom">Date From</th>
		<th id="thDateTo">Date To</th>
		<th id="thAssessmentType">Assessment Type</th>
		<th id="thAssessedCharge">Assessed Charge</th>
		<th id="thChargeableCost">Chargeable Cost</th>
		<th id="thContributionLevel">Contribution Level</th>
		<th id="thPlannedAdditionalCost">Additional Cost</th>		
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="HistoricNotificationLetters_PagingLinks" style="float:left;"></div>
<div class="clearer"></div>

<div class="HistoricNotificationLetters_Buttons" style="float:right;">
    <%--<asp:Button ID="btnPreview" runat="server" Text="Preview" ToolTip="Preview Contribution Notification letter" />--%>
    <input type="button" id="btnView" value="View" onclick="HistoricNotificationLetters_btnView_Click();" runat="server" />
    <%--<asp:Button ID="btnView" runat="server" Text="View" ToolTip="View Contribution Notification" OnClientClick="HistoricNotificationLetters_btnView_Click();" />--%>
</div>