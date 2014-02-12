<%@ Page Language="vb" AutoEventWireup="false" Codebehind="VisitAmendReqEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.VisitAmendReqEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view the details of Visit Amendment Requests, filtered by provider, 
		and optionally, Contract, Service User, Care Worker, and Weekending date. 
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
    </asp:Content>