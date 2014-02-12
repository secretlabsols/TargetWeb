<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DomContractReOpenedWeekSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DomContractReOpenedWeekSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable" id="tblWeeks" cellpadding="2" cellspacing="0" width="100%" summary="List of re-opened weeks.">
<caption>List of re-opened weeks.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th>Provider</th>
		<th>Contract Number</th>
		<th>Week Ending</th>
		<th id="thReOpenedBy">Re-Opened By</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="divPagingLinks" style="float:left;"></div>
<input type="button" id="btnView" style="float:right;width:5em;" value="View" onclick="btnView_Click();" />
<input type="button" id="btnNew" runat="server" style="float:right;width:5em;" value="New" onclick="btnNew_Click();" />
<div class="clearer"></div>