<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ListSubsidies.aspx.vb" Inherits="Target.SP.Web.Apps.ListSubsidies.ListSubsidies" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view subsidies, optionally filtered by provider, service, service user date and/or status.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>