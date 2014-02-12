<%@ Page Language="vb" AutoEventWireup="false" Codebehind="InvoiceEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.InvoiceEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view the details of existing provider invoices, 
		optionally, filtered by provider, contract, service user, Invoice Number, Weekending date, 
		invoice status and/or date. 
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
    </asp:Content>
