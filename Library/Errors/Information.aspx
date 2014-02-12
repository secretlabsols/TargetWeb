<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Information.aspx.vb" Inherits="Target.Web.Library.Errors.Information" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		
		<br /><br />
        <asp:Panel runat="server" ID="infoPanel" ></asp:Panel>
        <br /><br />
		Please <asp:HyperLink id=lnkBack runat="server" Text="go back to the previous page" ></asp:HyperLink> or 
		<asp:HyperLink id=lnkHome runat="server" Text="return to the site home page"></asp:HyperLink>.
	</asp:Content>