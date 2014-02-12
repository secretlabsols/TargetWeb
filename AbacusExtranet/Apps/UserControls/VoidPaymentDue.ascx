<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="VoidPaymentDue.ascx.vb" 
    Inherits="Target.Abacus.Extranet.Apps.UserControls.VoidPaymentDue" %>


    <asp:Panel runat="server" ID="pnlVoidPaymentdue">
                <asp:Label ID="lblVoidPaymentDue" runat="server" Text="Void Payment Due: "></asp:Label>
                <asp:HyperLink ID="lnkPaymentDue" runat="server" onclick="javascript:lnkPaymentDue_click();">0.00</asp:HyperLink>
    </asp:Panel>


    <script type="text/javascript">
        var pScheduleId;

        function lnkPaymentDue_click() {
            var url = "ModalDialogWrapper.axd?" + SITE_VIRTUAL_ROOT + "AbacusExtranet/Apps/Dom/PaymentSchedules/VoidPaymentDue.aspx?psid=" + pScheduleId;
            var dialog = OpenDialog(url, 30, 20, window);
        }
    
    </script>