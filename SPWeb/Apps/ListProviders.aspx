<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ListProviders.aspx.vb" Inherits="Target.SP.Web.Apps.ListProviders" %>
<%@ Register TagPrefix="uc1" TagName="ProviderSelector" Src="UserControls/ProviderSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below is the list of available providers.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:ProviderSelector id=ProviderSelector1 runat="server"></uc1:ProviderSelector>
	</asp:Content>