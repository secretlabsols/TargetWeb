<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DomServiceOrderReports.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DomServiceOrderReports" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<br />

<fieldset class="availableReports">
    <legend>Available Reports</legend>
    <asp:ListBox ID="lstReports" runat="server"></asp:ListBox>
</fieldset>

<fieldset id="fsSelectedReport" class="selectedReport">
    <legend>Selected Report</legend>
    <div id="divDefault">Please select a report from the list</div>
    
    <!-- DSO list -->
    <div id="divDsoList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlDsoList" runat="server"></uc1:ReportsButton>
    </div>
    
    <!-- DSO detail list -->
    <div id="divDsoDetailList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlDsoDetailList" runat="server"></uc1:ReportsButton>
    </div>
    
    <!-- DSO visit list -->
    <div id="divDsoVisitList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlDsoVisitList" runat="server"></uc1:ReportsButton>
    </div>
    
    <!-- unpaid DSO list -->
    <div id="divUnpaidDsoList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlUnpaidDsoList" runat="server"></uc1:ReportsButton>
    </div>
        
</fieldset>
<div class="clearer"></div>
<br />
