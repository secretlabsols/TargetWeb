<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewInvoiceBatch.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.ViewInvoiceBatch" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details for the selected Domiciliary Pro forma Invoice Batch.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" value="Back" onclick="history.go(-1);" />
	    <input type="button" id="btnDelete" runat="server" value="Delete" onclick="btnDelete_Click();" />
	    <input type="button" id="btnVerify" runat="server" value="Verify" onclick="btnVerify_Click();" />
	    <input type="button" id="btnViewInvoices" runat="server" value="View Invoices" onclick="btnViewInvoices_Click();" />
	    <br />
	    <br />
	    <label class="label" for="lblProvider">Provider</label>
	    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label class="label" for="lblContract">Contract</label>
	    <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
		<br />
        <label class="label" for="lblServiceUser">Service User</label>
	    <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblDateCreated">Date Created</label>
	    <asp:Label id="lblDateCreated" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblCreatedBy">Created By</label>
	    <asp:Label id="lblCreatedBy" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblStatus">Status</label>
	    <asp:Label id="lblStatus" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblStatusDate">Status Date</label>
	    <asp:Label id="lblStatusDate" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblLastActionBy">Last Action By</label>
	    <asp:Label id="lblLastActionBy" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblInvoiceCount">Invoice Count</label>
	    <asp:Label id="lblInvoiceCount" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblNetPayment">Net Payment</label>
	    <asp:Label id="lblNetPayment" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblItemsQueried">Items Queried</label>
	    <asp:Label id="lblItemsQueried" runat="server" CssClass="content"></asp:Label>
	    <br />
    </asp:Content>
