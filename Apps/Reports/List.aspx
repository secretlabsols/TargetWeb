<%@ Page Language="vb" AutoEventWireup="false" Codebehind="List.aspx.vb" Inherits="Target.Web.Apps.Reports.List" %>
<%@ Register TagPrefix="uc1" TagName="ReportSelector" Src="UserControls/ReportSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to view reports.
	</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:ReportSelector id="selector" runat="server"></uc1:ReportSelector>
    </asp:Content>
