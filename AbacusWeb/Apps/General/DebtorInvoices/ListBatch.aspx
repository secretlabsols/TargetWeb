<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ListBatch.aspx.vb" Inherits="Target.Abacus.Web.Apps.General.DebtorInvoices.ListBatch" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the available Debtor Invoice Batches.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <table class="listTable sortable" id="tblBatches" style="table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%" summary="List of available debtor invoice batches.">
            <caption>List of available debtor invoice batches.</caption>
            <thead>
	            <tr>
		            <th style="width:1.5em;vertical-align:bottom;background-position:bottom;"></th>
		            <th id="thCreated" style="width:10em;vertical-align:bottom;background-position:bottom;">Created</th>
		            <th id="thCreatedBy" style="width:8em;vertical-align:bottom;background-position:bottom;">Created By</th>
		            <th style="width:8em;vertical-align:bottom;background-position:bottom;">Invoice Count</th>
		            <th style="width:10em;vertical-align:bottom;background-position:bottom;">Total Value</th>
		            <th style="width:10em;vertical-align:bottom;background-position:bottom;">Latest Job Status</th>
	            </tr>
            </thead>
            <tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
        </table>
        <br />
        <div id="DebtorInvoiceBatch_PagingLinks" style="float:left;"></div>
        <div style="float:right;">
            <input type="button" style="width:8em;" id="btnRecreateFiles" value="Recreate Files" title="Re-create the interface files for the selected batch" onclick="btnRecreateFiles_Click();" />
            <uc1:ReportsButton id="ctlList" runat="server" ButtonWidth="5em"></uc1:ReportsButton>
            <input type="button" style="width:5em;" id="btnReports" value="Reports" onclick="btnReports_Click();" />
        </div>
        <div class="clearer"></div>
    </asp:Content>

