<%@ Page Language="vb" AutoEventWireup="false" Codebehind="AccessDenied.aspx.vb" Inherits="Target.Web.Library.Errors.AccessDenied" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		You have attempted to access a resource that you do not have the required access rights to view.
		<br /><br />
		Please <a href="javascript:history.go(-1)">go back to the previous page</a> or 
		<asp:HyperLink id=lnkHome runat="server" Text="return to the site home page"></asp:HyperLink>.
		<br /><br />
		Alternatively, your login session may have timed out. If so, <asp:HyperLink id="lnkLogin" runat="server" Text="please login again"></asp:HyperLink>.
	</asp:Content>