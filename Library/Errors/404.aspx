<%@ Page Language="vb" AutoEventWireup="false" Codebehind="404.aspx.vb" Inherits="Target.Web.Library.Errors.Error404" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		The page you requested could not be found. It may have been moved 
		or deleted or the URL you have typed may be incorrect.
		<br /><br /> 
		Please <a href="javascript:history.go(-1)">go back to the previous page</a> or 
		<asp:HyperLink id=lnkHome runat="server" Text="return to the site home page"></asp:HyperLink>.
	</asp:Content>