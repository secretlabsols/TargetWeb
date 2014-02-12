<%@ Page Language="vb" AutoEventWireup="false" Codebehind="BrowserCheck.aspx.vb" Inherits="Target.Web.Library.Errors.BrowserCheck" EnableViewState="False" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		The web browser you are using is not compatible with this web site.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <p>The compatible browsers are:</p>
	    <asp:Literal id="litSupportedBrowsers" runat="server"></asp:Literal>
	    <p>
	        <asp:Literal id="litCurrentBrowser" runat="server"></asp:Literal>
		    <br /><br />
		    <small>User agent: <asp:Literal id="litUserAgent" runat="server"></asp:Literal></small>
		</p>
	    <p>
	        <br />
		    If you require more information, please <a id="lnkContact" runat="server">contact the system administrator</a>.
	    </p>
	    <div id="divCanContinue" runat="server">
		    <p>
		        <br />
			    You can <a id="lnkContinue" runat="server">continue to use this web site</a> but should do so with caution.
		    </p>
		    <br />
	    </div>
    </asp:Content>