<%@ Control Language="vb" AutoEventWireup="false" Codebehind="InvoicedVisitsSelector.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.InvoicedVisitsSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table class="listTable" id="tblVisits" style="width:100%;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available domiciliary.">
<caption>List of Invoiced visits.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th>Visit Date</th>
		<th>Start Time Claimed</th>
		<th>Duration Claimed</th>
		<th>Actual Duration</th>
		<%If DisplayCareWorkerColumn() = True Then%><th>Care Worker</th><%End if %>
		<%If DisplayServiceuserColumn() = True Then%><th>Service User</th><%End if %>
	</tr>
</thead>
<tbody>
    <tr>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
    </tr>
</tbody>
</table>
<div id="Visits_PagingLinks" style="float:left;"></div>
<input type="button" id="btnView" runat="server" style="float:right;width:10em;" value="View/Amend Visit" onclick="btnView_Click();" />
<div class="clearer"></div>
