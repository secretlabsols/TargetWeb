<%@ Page Language="vb" AutoEventWireup="false" Codebehind="InvoicedVisitsEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.InvoicedVisitsEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view Invoiced Visits filtered by Provider, by Contract, 
	    by one or both of Service User and Care Worker and having a visit date that falls within a certain period
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <uc1:SelectorWizard id="SelectorWizard1" runat="server" NextButtonWidth="4.1em"></uc1:SelectorWizard>
    </asp:Content>

