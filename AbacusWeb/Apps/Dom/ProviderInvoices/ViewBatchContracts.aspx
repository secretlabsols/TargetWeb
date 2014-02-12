<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewBatchContracts.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.ViewBatchContracts" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the Domiciliary Contracts linked to the selected Domiciliary Invoice Batch.
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
	    <label class="label" for="lblInvoiceValueNet">Net Value</label>
	    <asp:Label id="lblInvoiceValueNet" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblInvoiceValueVAT">VAT</label>
	    <asp:Label id="lblInvoiceValueVAT" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblInvoiceValueGross">Total Value</label>
	    <asp:Label id="lblInvoiceValueGross" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <br />
        <table class="listTable sortable" id="tblContracts" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of domiciliary contracts.">
            <caption>List of available domiciliary contracts.</caption>
            <thead>
	            <tr>
		            <th id="thProviderName" style="width:12em;vertical-align:bottom;">Provider Name</th>
		            <th id="thContractNum" style="width:9em;vertical-align:bottom;">Contract No.</th>
		            <th style="width:6em;vertical-align:bottom;">Invoice Count</th>
		            <th style="width:7em;vertical-align:bottom;">Net Value</th>
		            <th style="width:6em;vertical-align:bottom;">VAT</th>
		            <th style="width:7em;vertical-align:bottom;">Total Value</th>
	            </tr>
            </thead>
            <tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
        </table>
        <br />
        <div id="DomProviderInvoiceBatchContracts_PagingLinks" style="float:left;"></div>
        <div style="float:right;">
            <input type="button" style="width:6em;" id="btnPrint" value="Print" onclick="btnPrint_Click();" />
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Go back to the previous screen" onclick="history.go(-1);" />
        </div>
        <div class="clearer"></div>
    </asp:Content>

