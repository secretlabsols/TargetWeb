<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ManualPayments.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.ManualPayments" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for, view and enter manual provider payments, 
		optionally filtered by provider, contract, period and/or system account.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>