<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ManualPaymentDomProformaInvoiceReports.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ManualPaymentDomProformaInvoiceReports" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<br /><br />

<fieldset class="availableReports">
    <legend>Available Reports</legend>
    <asp:ListBox ID="lstReports" runat="server"></asp:ListBox>
</fieldset>

<fieldset id="fsSelectedReport" class="selectedReport">
    <legend>Selected Report</legend>
    <div id="divDefault">Please select a report from the list</div>
    
    <!-- contract list -->
    <div id="divManualInvoiceList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlContractList" runat="server"></uc1:ReportsButton>
    </div>
    
    <!-- unit costs -->
    <div id="divManualInvoiceLines" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlUnitCosts" runat="server"></uc1:ReportsButton>
    </div>
            
</fieldset>
<div class="clearer"></div>
