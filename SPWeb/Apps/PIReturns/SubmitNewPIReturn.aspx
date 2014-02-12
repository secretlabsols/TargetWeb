<%@ Page Language="vb" AutoEventWireup="false" Codebehind="SubmitNewPIReturn.aspx.vb" Inherits="Target.SP.Web.Apps.PIReturns.SubmitNewPIReturn"
	EnableViewState="True" AspCompat="True" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to submit a PI Return for a particular service for a given Financial Year/Quarter.
	</asp:Content>
    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>