<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Lister.aspx.vb" 
Inherits="Target.Abacus.Web.Apps.Dom.SvcUsers.BudgetPeriods.Lister" %>

<asp:Content ID="cpOverview" ContentPlaceHolderID="MPPageOverview" runat="server" />

<asp:Content ID="cpError" ContentPlaceHolderID="MPPageError" runat="server">

    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server" />