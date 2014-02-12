<%@ Page Language="vb" AutoEventWireup="false" Codebehind="List.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.List" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for, view and maintain the domciliary contracts for a particular provider.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>