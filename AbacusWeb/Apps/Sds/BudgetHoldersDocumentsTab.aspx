<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BudgetHoldersDocumentsTab.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.BudgetHoldersDocumentsTab" masterpagefile="~/Popup.Master" %>

<%@ Register TagPrefix="DS" TagName="DocumentSelector" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentSelector.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPPageOverview" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    
    <DS:DocumentSelector id="docSelector" runat="server"></DS:DocumentSelector>
    
</asp:Content>