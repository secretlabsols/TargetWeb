<%@ Register TagPrefix="uc1" TagName="FileStoreTree" Src="../Controls/FileStoreTree.ascx" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Default.aspx.vb" Inherits="Target.Web.Apps.FileStore.Admin.DefaultPage" EnableViewState="True" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Please choose a file/folder below and the action you wish to perform.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:FileStoreTree id="tree" runat="server"></uc1:FileStoreTree>	
    </asp:Content>