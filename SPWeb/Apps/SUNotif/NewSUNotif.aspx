<%@ Page Language="vb" AutoEventWireup="false" Codebehind="NewSUNotif.aspx.vb" Inherits="Target.SP.Web.Apps.SUNotif.NewSUNotif"
	EnableViewState="True" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to submit the details of a new service user that has been entered into a particular service.
	</asp:Content>
    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>