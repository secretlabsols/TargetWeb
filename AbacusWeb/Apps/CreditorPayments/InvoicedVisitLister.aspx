<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="InvoicedVisitLister.aspx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.InvoicedVisitLister" %>

<asp:Content ID="cpOverview" ContentPlaceHolderID="MPPageOverview" runat="server" />

<asp:Content ID="cpError" ContentPlaceHolderID="MPPageError" runat="server">

    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server" />