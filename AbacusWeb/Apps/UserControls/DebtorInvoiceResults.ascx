<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DebtorInvoiceResults.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DebtorInvoiceResults" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<style type="text/css">
    .style1
    {
        width: 13%;
    }
</style>
<table class="listTable" id="DebtorInvoiceResults_tblInvoices" style="table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%" summary="List of debtor invoices.">
<caption>Inv Type key: RES=Residential  DOM=Domiciliary  LD=Learning Disability  STD=Standard  MAN=Manual  SDS=Self Directed Support<br /><br /></caption>
<thead>
	<tr>
		<th style="width:1.5em;vertical-align:bottom;background-position:bottom;"></th>
		<th id="thDebtor" style="width:10em;text-align:left;vertical-align:bottom;background-position:bottom;">Debtor</th>
		<th id="thClientRef" style="width:5.5em;text-align:left;vertical-align:bottom;background-position:bottom;">Ref.</th>
		<th id="thComment" style="text-align:left;vertical-align:bottom;background-position:bottom;">Comment</th>
		<th id="thInvNum" style="text-align:left;vertical-align:bottom;background-position:bottom;">Inv No.</th>
		<th id="thInvType" style="text-align:left;vertical-align:bottom;background-position:bottom;">Inv Type</th>
		<th id="thInvTotal" style="width:6.5em;text-align:left;vertical-align:bottom;background-position:bottom;">Inv Total</th>
		<th id="thInvDate" style="text-align:left;vertical-align:bottom;background-position:bottom;">Inv Date</th>
		<th id="thInvStatus" style="width:5.5em;text-align:left;vertical-align:bottom;background-position:bottom;">Status</th>
		<th id="thBatched" style="width:3.5em;text-align:left;vertical-align:bottom;background-position:bottom;">Batch</th>
		<th id="thExcluded" style="width:4.5em;text-align:left;vertical-align:bottom;background-position:bottom;">Exclude</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td class="style1"></td><td></td><td></td></tr></tbody>
</table>
<div id="DebtorInvoice_PagingLinks" style="float:left;"></div>
<input type="button" id="DebtorInvoiceResults_btnPrint" title="Print the results displayed" runat="server" style="float:right;width:7em;" value="Print" onclick="DebtorInvoiceResults_btnPrint_Click();" />
<input type="button" id="DebtorInvoiceResults_btnExcInc" title="Exclude/include the invoice from batching" runat="server" style="float:right;width:7em;" value="Exclude" onclick="DebtorInvoiceResults_btnExcInc_Click();" />
<input type="button" id="DebtorInvoiceResults_btnCreateBatch" title="Display the Create Batch screen" runat="server" style="float:right;width:7em;" value="Create Batch" onclick="DebtorInvoiceResults_btnCreateBatch_Click();" />
<div style="float:right;"><uc1:ReportsButton id="DebtorInvoiceResults_btnView" runat="server" ButtonText="View" ButtonWidth="7em"></uc1:ReportsButton></div>
<input type="button" id="DebtorInvoiceResults_btnNew" title="Create New Invoice" runat="server" style="float:right;width:7em;" value="New" onclick="DebtorInvoiceResults_btnNew_Click();" />
<div class="clearer"></div>

