<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReOpenWeek.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.ReOpenWeek" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for, view and maintain re-opened weeks, 
		optionally filtered by provider, contract, week-ending date and/or expected closure date.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>