<%@ Page Language="vb" AutoEventWireup="false" Codebehind="LicenceError.aspx.vb" Inherits="Target.Web.Library.Errors.LicenceError" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	    <span class="errorText">
		    Your action has been blocked because the application licence has been exceeded for the following reason(s):
		    <br /><br />
		    <asp:literal id="litErrorMsg" runat="server"></asp:literal>
        </span>
		<br /><br />
		Please <asp:HyperLink id=lnkHome runat="server" Text="return to the site home page"></asp:HyperLink> or 
		<asp:HyperLink id="lnkLogin" runat="server" Text="login again"></asp:HyperLink>.
		<br /><br />
        <asp:HyperLink id="lnkForceLogin" runat="server" Text="Click here to log out of all sessions"></asp:HyperLink>.
        
	</asp:Content>