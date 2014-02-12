<%@ Page Language="vb" AutoEventWireup="false" Codebehind="BatchReports.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.BatchReports" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		The available reports for the selected domiciliary provider invoice batch can be accessed below.
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
	    <label class="label" for="lblPostingDate">Posting Date</label>
	    <asp:Label id="lblPostingDate" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblPostingYear">Posting Year</label>
	    <asp:Label id="lblPostingYear" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblPeriodNum">Period Number</label>
	    <asp:Label id="lblPeriodNum" runat="server" CssClass="content"></asp:Label>
	    <br /><br />
	    
	    <fieldset class="availableReports">
            <legend>Available Reports</legend>
            <asp:ListBox ID="lstReports" runat="server"></asp:ListBox>
        </fieldset>

        <fieldset id="fsSelectedReport" class="selectedReport">
            <legend>Selected Report</legend>
            <div id="divDefault">Please select a report from the list</div>
            
            <!-- batch summary -->
            <div id="divBatchSummary" runat="server" class="availableReport">
                <uc1:ReportsButton id="ctlBatchSummary" runat="server"></uc1:ReportsButton>
            </div>
            
            <!-- DPI list -->
            <div id="divDpiList" runat="server" class="availableReport">
                <uc1:ReportsButton id="ctlDpiList" runat="server"></uc1:ReportsButton>
            </div>
            
            <!-- DPI line list -->
            <div id="divDpiLineList" runat="server" class="availableReport">
                <uc1:ReportsButton id="ctlDpiLineList" runat="server"></uc1:ReportsButton>
            </div>
            
            <!-- first payment DSO list -->
            <div id="divFirstPaymentDsoList" runat="server" class="availableReport">
                <uc1:ReportsButton id="ctlFirstPaymentDsoList" runat="server"></uc1:ReportsButton>
            </div>
                
        </fieldset>
	    <div class="clearer"></div>
	    
	    <br />
        <div style="float:right;">
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Go back to the previous screen" onclick="document.location.href=unescape(GetQSParam(document.location.search,'backUrl'));" />
        </div>
        <div class="clearer"></div>
    </asp:Content>

