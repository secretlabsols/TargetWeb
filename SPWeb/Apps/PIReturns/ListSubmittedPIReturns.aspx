<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ListSubmittedPIReturns.aspx.vb" Inherits="Target.SP.Web.Apps.PIReturns.ListSubmittedPIReturns"
	EnableViewState="True" AspCompat="True" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to list submitted PI Returns for a particular service for a given Financial Year, Quarter and Status.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>