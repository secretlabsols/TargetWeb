<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PaymentSchedulesEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.PaymentSchedulesEnquiry" %>

<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		The wizard allows you to search for and view existing Payment Schedules, optionally, filtered by 
		Provider, Contract and a variety of other attributes.
	</asp:Content>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
    </asp:Content>
