<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewBatchReports.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.ViewBatchReports" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the latest results produced for the selected Domiciliary Invoice Batch.
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
	    <br />
	    <div id="JobStepList_Content">
		    <br />
		    <table class="listTable sortable" id="JobStepList_Table" cellpadding="2" cellspacing="0" width="100%" summary="Lists the available job steps.">
		    <caption>List of steps in the latest job for the selected invoice batch.</caption>
		    <thead>
			    <tr>
				    <th>Step</th>
				    <th>Type</th>
				    <th>Status</th>
				    <th>Start</th>
				    <th>End</th>
				    <th>&nbsp;</th>
			    </tr>
		    </thead>
		    <tbody><tr><td></td></tr></tbody>
		    </table>
	    </div>
	    <div id="JobStepXml_Content">
		    <br />
            <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
                <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Results">
                    <ContentTemplate></ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
	    </div>
	    <br />
        <div style="float:right;">
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Go back to the previous screen" onclick="document.location.href=unescape(GetQSParam(document.location.search,'backUrl'));" />
        </div>
        <div class="clearer"></div>
    </asp:Content>

