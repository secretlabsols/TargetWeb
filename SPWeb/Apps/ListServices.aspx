<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ListServices.aspx.vb" Inherits="Target.SP.Web.Apps.ListServices"%>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view the details of services, optionally filtered by provider.
	</asp:Content>
    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Back" title="Navigates to the previous screen." runat="server" id="btnBack" onclick="document.location.href=unescape(GetQSParam(document.location.search, 'backUrl'));" />
    	
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>