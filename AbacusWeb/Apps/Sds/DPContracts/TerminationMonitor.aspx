<%@ Page Language="vb" AutoEventWireup="false" Codebehind="TerminationMonitor.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPContracts.TerminationMonitor" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
    
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This wizard allows you to search for and view terminated Direct Payment Contracts.
</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">
    <uc1:SelectorWizard id="SelectorWizard1" runat="server" BackButtonWidth="6em" NewButtonWidth ="10.1em" />
</asp:Content>
