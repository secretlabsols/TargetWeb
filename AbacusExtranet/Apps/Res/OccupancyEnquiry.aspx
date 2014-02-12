<%@ Page Language="vb" AutoEventWireup="false" Codebehind="OccupancyEnquiry.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Res.OccupancyEnquiry" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This wizard allows you to search for and view the occupancy details of a particular home, optionally filtered by date and/or movement.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
	</asp:Content>