<%@ Page Language="vb" AutoEventWireup="false" AspCompat="true" CodeBehind="Notes.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.Notes" MasterPageFile="~/popup.master"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc3" TagName="Notes" Src="~/AbacusWeb/Apps/UserControls/NotesSelector.ascx" %>

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
    <div id="notesdiv" runat="server"><uc3:Notes id="Notes1" runat="server"></uc3:Notes></div>
</asp:Content>



