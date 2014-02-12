<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Default.Master" CodeBehind="Test.aspx.vb" Inherits="Target.Abacus.Web.Test" 
    title="Untitled Page" EnableViewState="True" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPPageTitle" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" runat="server">
        First Name: <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
        <br />
        Surname: <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
    </fieldset>
    <br />
</asp:Content>
