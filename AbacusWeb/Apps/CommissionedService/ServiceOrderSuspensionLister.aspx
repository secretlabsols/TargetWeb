<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ServiceOrderSuspensionLister.aspx.vb" Inherits="Target.Abacus.Web.Apps.CommissionedService.ServiceOrderSuspensionLister" %>

<asp:Content ID="cpOverview" ContentPlaceHolderID="MPPageOverview" runat="server" />

<asp:Content ID="cpError" ContentPlaceHolderID="MPPageError" runat="server">

    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server" />