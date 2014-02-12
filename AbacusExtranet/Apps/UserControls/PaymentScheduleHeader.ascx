<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PaymentScheduleHeader.ascx.vb"
    Inherits="Target.Abacus.Extranet.Apps.UserControls.PaymentScheduleHeader" 
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:Panel ID="pnlsingleLine" runat="server">
    <asp:Label ID="lblReferenceSingleLine" runat="server" Text="Payment Schedule" Width="11em"></asp:Label>
    <asp:HyperLink ID="lnksingleLineLink" runat="server"></asp:HyperLink>
    <asp:Label ID="lnksingleLineLabel" runat="server"></asp:Label>
</asp:Panel>
<asp:Panel ID="pnlPaymentSchedule" runat="server">
    <asp:Panel ID="pnlfieldset" runat="server" GroupingText="Payment Schedule">
        <div style="margin-bottom: 5px;">
            <asp:Label ID="lblReference" runat="server" Text="Reference" Width="8em"></asp:Label>
            <asp:HyperLink ID="lnkPaymentSchedile" runat="server"></asp:HyperLink>
        </div>
        <div style="margin-bottom: 5px;">
            <asp:Label ID="lblProvider" runat="server" Text="Provider" Width="8em"></asp:Label>
            <asp:Label ID="lblProviderRef" runat="server" Text=""></asp:Label>:
            <asp:Label ID="lblProviderName" runat="server" Text=""></asp:Label>
        </div>
        <div style="margin-bottom: 5px;">
            <asp:Label ID="lblcontract" runat="server" Text="Contract" Width="8em"></asp:Label>
            <asp:Label ID="lblContractNumber" runat="server" Text=""></asp:Label>:
            <asp:Label ID="lblContractTitle" runat="server" Text=""></asp:Label>
        </div>
        <asp:Panel ID="pnlPaymentFromTo" runat="server">
            <div style="margin-bottom: 5px;">
                <asp:Label ID="lblPaymentFrom" runat="server" Text="Period From" Width="8em"></asp:Label>
                <asp:Label ID="lblDateFrom" runat="server" Text=""></asp:Label>
                &nbsp;
                <asp:Label ID="lblPaymentTo" runat="server" Text="To"></asp:Label>
                &nbsp;
                <asp:Label ID="lblDateTo" runat="server" Text=""></asp:Label>
            </div>
        </asp:Panel>
        <asp:Panel ID="ExtraControls" runat="server">
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
