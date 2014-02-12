<%@ Page Language="vb" AutoEventWireup="false" Codebehind="List.aspx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.Batches.List" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CreateJobPointInTime" Src="~/AbacusWeb/Apps/Jobs/UserControls/ucCreateJobPointInTime.ascx" %>
<%@ Register TagPrefix="uc1" TagName="InterfaceLogSelector" Src="../../UserControls/InterfaceLogSelector.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        Displayed below are the available Creditor Payment Batches. 
	</asp:Content>
	
	<asp:Content ID="conPageError" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    </asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server" />
	    <uc1:InterfaceLogSelector id="interfaceLogSelector1" runat="server" />	
        <div style="float:right;">
            <asp:Button ID="btnRemittances" runat="server" Text="Remittances" ToolTip="View remittances for the selected batch" OnClientClick="btnRemittances_Click(); return false;" />
            <asp:Button ID="btnRecreateFiles" runat="server" Text="Recreate Files" ToolTip="Re-create the interface files for the selected batch" OnClientClick="btnRecreateFiles_Click(); return false;" />
            <uc1:ReportsButton id="btnReportsList" runat="server" />
            <asp:Button ID="btnReports" runat="server" Text="Reports" ToolTip="View reports for the selected batch" OnClientClick="btnReports_Click(); return false;" />
        </div>
        <div class="clearer"></div>        
	</asp:Content>
	