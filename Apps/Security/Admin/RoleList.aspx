<%@ Page Language="vb" AutoEventWireup="false" Codebehind="RoleList.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.RoleList" %>
<%@ Register TagPrefix="uc1" TagName="RoleSelector" Src="../UserControls/RoleSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to search for, view and edit security roles that control access to areas of different functionality.
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:RoleSelector id="selector" runat="server"></uc1:RoleSelector>
    </asp:Content>