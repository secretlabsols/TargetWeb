<%@ Page Language="vb" AutoEventWireup="false" Codebehind="UserList.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.UserList" %>
<%@ Register TagPrefix="uc1" TagName="UserSelector" Src="../UserControls/UserSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to search for, view and edit users.
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:UserSelector id="selector" runat="server"></uc1:UserSelector>
    </asp:Content>