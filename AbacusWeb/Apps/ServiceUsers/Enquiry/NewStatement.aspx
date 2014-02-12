<%@ Page Language="vb" AutoEventWireup="false" Codebehind="NewStatement.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.NewStatement" masterpagefile="~/Popup.Master"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc2" TagName="ServiceUserHeader" Src="~/AbacusWeb/Apps/UserControls/ServiceUserHeader.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

	<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
	<asp:Content ID="Content1" ContentPlaceHolderID="MPPageOverview" runat="server">
		<div style="padding:0.75em 0.75em 0em 0.75em;">
			This screen allows you to view statements.
		</div>
	</asp:Content>
	<asp:Content ID="Content2" ContentPlaceHolderID="MPContent" runat="server">
		<div style="padding:0em 0.75em 0.75em 0.75em;">
			<fieldset>
			   <legend>Service User Details</legend>
			<div>
				<uc2:ServiceUserHeader id="serviceUserHeader1" runat="server"></uc2:ServiceUserHeader>
			</div>
			<div class="clearer"></div>
			<br />
			</fieldset>
			<br />
			<fieldset>
				<legend>View Statement Selection</legend>
				<cc1:DropDownListEx ID="cboBudgetPeriods" runat="server" LabelText="Budget Periods" LabelBold="true" LabelWidth="12em" Width="20em"></cc1:DropDownListEx>
				<br />
				<cc1:DropDownListEx ID="cboStatementLayout" runat="server" LabelText="Statement Layout" LabelBold="true" LabelWidth="12em"></cc1:DropDownListEx>
				<br />
			</fieldset>
			<br />
			<div class="clearer"></div>
			<div style="float:left;"><uc1:ReportsButton id="btnPreview" runat="server" ButtonText="Preview" ButtonWidth="70px"></uc1:ReportsButton></div>
            <asp:Button ID="btnGenerate" runat="server" Text="Generate" OnClientClick="btnGenerate_Click();" />
			<div class="clearer"></div>
		</div>
	</asp:Content>
