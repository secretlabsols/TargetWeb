<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DPContractReports.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DPContractReports" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<br /><br />

<fieldset class="availableReports">
    <legend>Available Reports</legend>
    <asp:ListBox ID="lstReports" runat="server"></asp:ListBox>
</fieldset>

<fieldset id="fsSelectedReport" class="selectedReport">
    <legend>Selected Report</legend>
    <div id="divDefault">Please select a report from the list</div>
    
    <!-- Direct Payment Contracts -->
    <div id="divContractList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlContractList" runat="server"></uc1:ReportsButton>
    </div>
    
    <!-- Direct Payment Periods -->
    <div id="divPeriodList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlPeriodList" runat="server"></uc1:ReportsButton>
    </div>
    
    <!-- Direct Payment Details -->
    <div id="divDetailList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlDetailList" runat="server"></uc1:ReportsButton>
    </div>
    
    <!-- Direct Payment Breakdown Details -->
    <div id="divBreakdownList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlBreakdownList" runat="server"></uc1:ReportsButton>
    </div>
        
</fieldset>
<div class="clearer"></div>
