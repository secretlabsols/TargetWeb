<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CreateBatch.aspx.vb" Inherits="Target.Abacus.Web.Apps.General.DebtorInvoices.CreateBatch" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the attributes covering the Debtor Invoices for the Batch about to be created.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <fieldset id="grpInvCriteria" runat="server">
	        <legend>Debtor Invoice Filtering Criteria</legend>
		    <table>
		        <tr style="line-height:1em"><td style="width:10em">Debtor:</td><td><asp:Label id="lblFilterClient" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td style="width:10em">Invoice Types:</td><td><asp:Label id="lblFilterInvTypes" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td style="width:10em">Creation Dates:</td><td><asp:Label id="lblFilterCreationDates" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr style="line-height:1em"><td style="width:10em">Other Settings:</td><td><asp:Label id="lblFilterOther" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <td><asp:Label id="lblFilter" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td><td><asp:Label id="lblFilters" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
            </table>
        </fieldset>
        <br />
        <fieldset id="grpBatchTotals" runat="server">
	        <legend>Anticipated Batch Contents</legend>
	        <table>
		        <tr style="line-height:1em"><td style="width:10em">Invoice Count:</td><td><asp:Label id="lblInvCount" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></td><asp:Label id="lblOpeningBalance" runat="server" CssClass="content" Font-Bold="true"></asp:Label><td></tr>
		        <tr style="line-height:1em"><td style="width:10em">Total Value:</td><td><asp:Label id="lblInvTotalValue" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
	        </table>
        </fieldset>
        <br />
        <fieldset id="grpCreateInterface" runat="server">
            <legend>Create Interface File(s)</legend>
	        <asp:radiobutton id="optCreateNow" groupname="grpCreation" TextAlign="left" height="2em" width="100%" 
	            runat="server" text="Create files now" checked="true" OnCheckedChanged="RadioButtonClicked" 
	            AutoPostBack="true" />
            <asp:radiobutton id="optDefer" groupname="grpCreation" TextAlign="left" height="2em" width="100%" 
                runat="server" text="Defer creation of files" OnCheckedChanged="RadioButtonClicked" 
                AutoPostBack="true" />
	        <div style="float:left;">
	            <cc1:TextBoxEx ID="dteStartDate" runat="server" LabelText="Start Date/Time" LabelWidth="17em"
		            Required="true" RequiredValidatorErrMsg="Please enter a valid start date" Format="DateFormat"
		            ValidationGroup="Save" Enabled="false"></cc1:TextBoxEx>
            </div>
            <div style="float:left;">
			    <cc1:TimePicker ID="tmeStartDate" runat="server" LabelText="&nbsp;" Enabled="false" EnableViewState="false" ShowSeconds="false" LabelWidth="1em"></cc1:TimePicker>
			</div>
			<div class="clearer"></div>
			<br />
			<cc1:TextBoxEx ID="dtePostingDate" runat="server" LabelText="Posting Date" LabelWidth="17em"
                Required="true" RequiredValidatorErrMsg="Please enter a valid posting date" Format="DateFormat"
				ValidationGroup="Save"></cc1:TextBoxEx>
		    <br />
            <cc1:DropDownListEx ID="cboPostingYear" runat="server" LabelText="Posting Year" LabelWidth="17em"
                Required="true" RequiredValidatorErrMsg="Please select a posting year" ValidationGroup="Save"></cc1:DropDownListEx>
			<br />
            <cc1:DropDownListEx ID="cboPeriodNum" runat="server" LabelText="Period Number" LabelWidth="17em"
			    Required="true" RequiredValidatorErrMsg="Please select a period number" ValidationGroup="Save"></cc1:DropDownListEx>
            <br />
            <asp:radiobutton id="optFullRollback" groupname="grpRollback" TextAlign="left" Height="3em"
                runat="server" text="Rollback entire batch upon encountering an error" />
            <br />
            <asp:radiobutton id="optPartialRollback" groupname="grpRollback" TextAlign="left" Height="3em"
                runat="server" text="Remove erroneous items from the batch" checked="true" />
        </fieldset>
        <br />
        <div style="float:right;">
            <asp:Button id="btnCreate" runat="server" title="Create the invoice batch" text="Create" width="6em" />
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Return to the previous screen" onclick="btnBack_Click();" />
        </div>
        <div class="clearer"></div>
    </asp:Content>

