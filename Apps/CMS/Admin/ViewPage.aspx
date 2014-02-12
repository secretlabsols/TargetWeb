<%@ Register TagPrefix="uc1" TagName="PageWizardHeader" Src="../Controls/PageWizardHeader.ascx" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewPage.aspx.vb" Inherits="Target.Web.Apps.CMS.Admin.ViewPage" EnableViewState="True" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<asp:HyperLink id=lnkViewPage runat="server" rel="external">Your page</asp:HyperLink> is now complete.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:PageWizardHeader id=wizHeader runat="server"></uc1:PageWizardHeader>
	    If you wish to email this page, enter the the recipient email addresses below (one per line) and click on "Email Page".
	    <br />
	    <asp:Label id=lblError runat="server" CssClass="errorText"></asp:Label>
	    <asp:Label id=lblEmailsSent runat="server" CssClass="warningText"></asp:Label>
	    <br />
	    <asp:TextBox id=txtAddresses runat="server" Columns="80" Rows="5" TextMode="MultiLine"></asp:TextBox>
	    <br />
	    <asp:RequiredFieldValidator id=reqAddresses runat="server" ControlToValidate="txtAddresses" Display="Dynamic" ErrorMessage="Please enter at least one email address."></asp:RequiredFieldValidator>
	    <br />
	    <asp:Button id=btnEmailPage runat="server" Text="Email Page"></asp:Button>
    </asp:Content>