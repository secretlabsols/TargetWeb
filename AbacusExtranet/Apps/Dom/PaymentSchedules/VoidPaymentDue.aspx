<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VoidPaymentDue.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.VoidPaymentDue" MasterPageFile="~/Popup.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
    <div style="padding-top: 30px; padding-left: 50px;">
        <table class="listTable" cellpadding="0" cellspacing="0" width="70%">
            <caption>
                Explanation.</caption>
            <thead>
                <tr>
                    <th colspan="2">
                        Void Payment Explanation
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        Pending Payments
                    </td>
                    <td>
                        <asp:Label ID="lblPendingPayments" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        Payments already made
                    </td>
                    <td>
                        <asp:Label ID="lblPaymentsMade" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        Void Payments already made
                    </td>
                    <td>
                        <asp:Label ID="lblVoidalreadyMade" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 2px;">
                    </td>
                </tr>
                <tr>
                    <td>
                        Sum of above
                    </td>
                    <td>
                        <asp:Label ID="lblProformaProviderSum" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                         Void Payment Due
                    </td>
                    <td>
                        <asp:Label ID="lblVoidPaymentDue" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 2px;">
                    </td>
                </tr>
                <tr style="font-weight:bold;">
                    <td>
                        Sum of above
                    </td>
                    <td>
                        <asp:Label ID="lblResult" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>
