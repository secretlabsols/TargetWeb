<%@ Page Language="vb" AutoEventWireup="false" Codebehind="SUPaymentEnquiry.aspx.vb" Inherits="Target.SP.Web.Apps.Payments.SUPaymentEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view the details of payments made for a specific service user, optionally filtered by date.
	</asp:Content>
    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>