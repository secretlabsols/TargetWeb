<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PlannedCarePackageEnquiry.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.PlannedCarePackageEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for, view and maintain planned care package values for a particular service user, 
		optionally filtered by period.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>