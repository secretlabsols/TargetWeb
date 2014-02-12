<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Finance.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.Finance" masterpagefile="~/Popup.Master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc4" TagName="GenericCreditorPayment" Src="~/AbacusWeb/Apps/UserControls/GenericCreditorPaymentSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="DebtorInvoice" Src="~/AbacusWeb/Apps/UserControls/DebtorInvoiceResults.ascx" %>
<%@ Register TagPrefix="uc6" TagName="Statements" Src="~/AbacusWeb/Apps/UserControls/StatementsSelector.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPPageOverview" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
<style type="text/css">
    .pnl{
    font-weight: bold;
    background-color: #eeeeee;
    padding: 5px;
    cursor: pointer; 
    border: solid 1px #c0c0c0
    }
</style>
    <cc1:CollapsiblePanel id="cpPayments" runat="server" HeaderLinkText="Payments" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrFinance')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrFinance')">
        <ContentTemplate>
            <uc4:GenericCreditorPayment id="genericCreditorPayment1" runat="server"></uc4:GenericCreditorPayment>
        </ContentTemplate>
    </cc1:CollapsiblePanel>  
    <cc1:CollapsiblePanel id="cpInvoices" runat="server" HeaderLinkText="Invoices" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrFinance')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrFinance')">
        <ContentTemplate>
            <uc5:DebtorInvoice id="debtorInvoice1" runat="server"></uc5:DebtorInvoice>
        </ContentTemplate>
    </cc1:CollapsiblePanel>
    <cc1:CollapsiblePanel id="cpStatements" runat="server" HeaderLinkText="Statements" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrFinance')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrFinance')">
        <ContentTemplate>
            <uc6:Statements id="ucStatementSelector" runat="server"></uc6:Statements>
        </ContentTemplate>
    </cc1:CollapsiblePanel>
</asp:Content>
