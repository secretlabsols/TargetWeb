<%@ Page Language="vb" AutoEventWireup="false" AspCompat="true" CodeBehind="Addresses.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.Addresses" MasterPageFile="~/popup.master"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
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
    <div class="clearer"></div>
    <div><asp:PlaceHolder id="phAddresses" runat="server"></asp:PlaceHolder> </div>
</asp:Content>



