<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PendingSuclChangesSelector.ascx.vb"
    Inherits="Target.Abacus.Web.Apps.UserControls.PendingSuclChangesSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<label id="PendingSuclChangesSelector_Error" class="errorText" style="background-color: Transparent"
    runat="server" />
<table class="listTable" id="PendingSuclChangesSelector_Results" style="table-layout: fixed;"
    cellpadding="2" cellspacing="0" width="100%" summary="List of pending service user contribution level changes.">
    <caption>
        List of pending service user contribution level changes.</caption>
    <thead>
        <tr>
            <th style="width: 1.5em;">
            </th>
            <%--<th width="20px">&nbsp;</th>--%>
            <th id="thDateFrom">
                Date From
            </th>
            <th id="thDateTo">
                Date To
            </th>
            <th id="thAssessmentType">
                Assessment Type
            </th>
            <th id="thAssessedCharge">
                Assessment
            </th>
            <th id="thChargeableCost">
                Chargeable
            </th>
            <th id="thContributionLevel">
                Level
            </th>
            <th id="thPlannedAdditionalCost">
                Additional
            </th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
             <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
    </tbody>
</table>
<div id="PendingSuclChangesSelector_PagingLinks" style="float: left;">
</div>
<div class="clearer">
</div>
<div id="PendingSuclChangesSelector_Information" style="float: left;">
</div>
<div class="clearer">
</div>
<div class="PendingSuclChangesSelector_Buttons" style="float: right;">
    <asp:Button ID="btnPreview" runat="server" Text="Preview" ToolTip="Preview Contribution Notification letter" />
    <asp:Button ID="btnNotify" runat="server" Text="Notify" ToolTip="Generate Contribution Notification" />
</div>
