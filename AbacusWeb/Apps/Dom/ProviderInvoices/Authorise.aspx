<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Authorise.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.Authorise" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the attributes covering the Domiciliary Provider Invoices for the Authorise job about to be created.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <fieldset id="grpInvCriteria" runat="server">
	        <legend>Provider Invoice Filtering Criteria</legend>
		    <table>
		        <tr style="line-height:1em"><td>Care Provider:</td><td><asp:Label id="lblFilterProvider" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Contract:</td><td><asp:Label id="lblFilterContract" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Service User:</td><td><asp:Label id="lblFilterClient" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Invoice Status:</td><td><asp:Label id="lblFilterStatus" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Status Dates:</td><td><asp:Label id="lblFilterStatusDates" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Invoice No.:</td><td><asp:Label id="lblFilterInvoiceNum" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Invoice Ref.:</td><td><asp:Label id="lblFilterInvoiceRef" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Invoice Period:</td><td><asp:Label id="lblFilterWEDates" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <td><asp:Label id="lblFilter" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td><td><asp:Label id="lblFilters" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
            </table>
        </fieldset>
        <br />
        <fieldset id="grpBatchTotals" runat="server">
	        <legend>Anticipated Batch Contents</legend>
	        <table>
		        <tr style="line-height:1em"><td>Invoice Count:</td><td><asp:Label id="lblInvCount" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Total Value:</td><td><asp:Label id="lblInvTotalValue" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td>Total VAT:</td><td><asp:Label id="lblInvTotalVAT" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td>                <td align=right><input type="button" style="width:6em;" id="btnRefresh" value="Refresh" title="Updates the anticipated batch values" onclick="btnRefresh_Click();" /></td></tr>
	        </table>
        </fieldset>
        <br />
        <fieldset id="grpCreateInterface" runat="server">
            <legend>Authorisation Job Options</legend>
	        <asp:radiobutton id="optCreateNow" groupname="grpCreation" TextAlign="left" height="2em" width="100%" 
	            runat="server" text="Create job now" checked="True" onclick="javascript:optCreation_Click();" />
            <asp:radiobutton id="optDefer" groupname="grpCreation" TextAlign="left" height="2em" width="100%" 
                runat="server" text="Defer creation of job" onclick="javascript:optCreation_Click();" />
	        <div style="float:left;">
	            <cc1:TextBoxEx ID="dteStartDate" runat="server" LabelText="Start Date/Time" LabelWidth="17em"
		            Required="true" RequiredValidatorErrMsg="Please enter a valid start date" Format="DateFormat"
		            ValidationGroup="Save" Enabled="false"></cc1:TextBoxEx>
            </div>
            <br />
			<cc1:TimePicker ID="tmeStartDate" runat="server" LabelText="&nbsp;" Enabled="False" EnableViewState="False" ShowSeconds="False" LabelWidth="1em"></cc1:TimePicker>
			<br />          
        </fieldset>
        <br />
        <div style="float:right;">
            <asp:Button id="btnCreate" runat="server" text="Create" width="6em" />
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Return to the previous screen" onclick="btnBack_Click();" />
        </div>
        <div class="clearer"></div>
    </asp:Content>


