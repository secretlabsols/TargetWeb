<%@ Page Language="vb" AutoEventWireup="false" Codebehind="SUEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Res.SUEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceholderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view a Residential Service User for a particular home, optionally filtered by date.
	</asp:Content>
	<asp:Content ContentPlaceholderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>