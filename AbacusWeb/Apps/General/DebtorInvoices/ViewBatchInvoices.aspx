<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewBatchInvoices.aspx.vb" Inherits="Target.Abacus.Web.Apps.General.DebtorInvoices.ViewBatchInvoices" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the invoices linked to the selected Debtor Invoice Batch.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <label class="label" for="lblCreatedDate">Created</label>
	    <asp:Label id="lblCreatedDate" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label class="label" for="lblCreatedBy">Created By</label>
	    <asp:Label id="lblCreatedBy" runat="server" CssClass="content"></asp:Label>
		<br />
        <label class="label" for="lblInvoiceCount">Invoice Count</label>
	    <asp:Label id="lblInvoiceCount" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblInvoiceValueGross">Total Value</label>
	    <asp:Label id="lblInvoiceValue" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <br />
        <table class="listTable sortable" id="tblInvoices" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of domiciliary contracts.">
            <caption>Inv Type key: RES=Residential  DOM=Domiciliary  LD=Learning Disability  STD=Standard  MAN=Manual  SDS=Self Directed Support<br /><br /></caption>
            <thead>
	            <tr>
		            <th id="thDebtor" style="width:26em;text-align:left;vertical-align:bottom;background-position:bottom;">Debtor</th>
		            <th id="thInvType" style="width:7.5em;text-align:left;vertical-align:bottom;background-position:bottom;">Inv Type</th>
		            <th id="thInvNum" style="width:6em;text-align:left;vertical-align:bottom;background-position:bottom;">Inv No.</th>
		            <th id="thDateCreated" style="width:7em;text-align:left;vertical-align:bottom;background-position:bottom;">Created</th>
		            <th id="thInvTotal" style="width:6.5em;text-align:left;vertical-align:bottom;background-position:bottom;">Inv Total</th>
		            <th id="thInvStatus" style="width:5em;text-align:left;vertical-align:bottom;background-position:bottom;">Status</th>
	            </tr>
            </thead>
            <tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
        </table>
        <br />
        <div id="DebtorInvoiceBatchInvoices_PagingLinks" style="float:left;"></div>
        <div style="float:right;">
            <input type="button" style="width:6em;" id="btnPrint" value="Print" title="Print the results displayed" onclick="btnPrint_Click();" />
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Go back to the previous screen" onclick="history.go(-1);" />
        </div>
        <div class="clearer"></div>
    </asp:Content>

