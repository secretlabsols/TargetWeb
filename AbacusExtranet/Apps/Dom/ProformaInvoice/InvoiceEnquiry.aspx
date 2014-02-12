<%@ Page Language="vb" AutoEventWireup="false" Codebehind="InvoiceEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.InvoiceEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view the details of existing domiciliary pro forma invoice batches, filtered by provider, 
		contract and optionally, invoice batch type, status and/or week ending date. 
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
    </asp:Content>
