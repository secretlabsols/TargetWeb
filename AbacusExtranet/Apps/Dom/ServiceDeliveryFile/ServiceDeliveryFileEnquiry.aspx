<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ServiceDeliveryFileEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ServiceDeliveryFile.ServiceDeliveryFileEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view the details of uploaded service delivery files, optionally filtered by status and/or date.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
    </asp:Content>
