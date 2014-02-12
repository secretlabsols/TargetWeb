<%@ Page Language="vb" AutoEventWireup="false" Codebehind="SelectItem.aspx.vb" Inherits="Target.Web.Apps.FileStore.Admin.SelectItem" EnableViewState="True" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="FileStoreTree" Src="../Controls/FileStoreTree.ascx" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:FileStoreTree id="tree" runat="server"></uc1:FileStoreTree>	
    </asp:Content>