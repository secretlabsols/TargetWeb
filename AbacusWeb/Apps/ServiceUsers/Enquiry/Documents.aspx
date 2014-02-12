<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Documents.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.Documents" masterpagefile="~/Popup.Master" %>

<%@ Register TagPrefix="DS" TagName="DocumentSelector" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentSelector.ascx" %>

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
    
    <DS:DocumentSelector id="docSelector" runat="server"></DS:DocumentSelector>
    
</asp:Content>
