<%@ Page Language="vb" AutoEventWireup="false" Codebehind="SelectItem.aspx.vb" Inherits="Target.Web.Apps.CMS.Admin.SelectItem" EnableViewState="True" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="CMSTree" Src="../Controls/CMSTree.ascx" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:CMSTree id="tree" runat="server"></uc1:CMSTree>	
    </asp:Content>