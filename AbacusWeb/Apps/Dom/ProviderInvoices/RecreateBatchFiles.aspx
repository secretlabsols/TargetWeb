<%@ Page Language="vb" AutoEventWireup="false" Codebehind="RecreateBatchFiles.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.RecreateBatchFiles" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details for the selected Domiciliary Invoice Batch.
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
	    <br />
        <fieldset id="grpRecreateInterface" runat="server">
	        <legend>Recreate Interface File(s)</legend>
            <cc1:CheckBoxEx ID="chkRereadData" runat="server" Text="Re-read underlying data" LabelWidth="16.75em"></cc1:CheckBoxEx>
            <br /><br />
            <asp:radiobutton id="optDoNotRollback" groupname="grpRollback" TextAlign="left" height="2em" width="100%" 
                runat="server" text="Do not rollback batch" checked="true" />
	        <asp:radiobutton id="optFullRollback" groupname="grpRollback" TextAlign="left" height="3em" width="100%" 
	            runat="server" text="Rollback entire batch upon encountering an error" disabled="true" />
	        <asp:radiobutton id="optPartialRollback" groupname="grpRollback" TextAlign="left" height="3em" width="100%" 
	            runat="server" text="Partially rollback batch upon encountering an error" disabled="true" />
	        <br />
	        <asp:radiobutton id="optCreateNow" groupname="grpCreation" TextAlign="left" height="2em" width="100%" 
	            runat="server" text="Create files now" checked="true" OnCheckedChanged="CreationOptionClicked" 
	            AutoPostBack="true" />
            <asp:radiobutton id="optDefer" groupname="grpCreation" TextAlign="left" height="2em" width="100%" 
                runat="server" text="Defer creation of files" OnCheckedChanged="CreationOptionClicked" 
                AutoPostBack="true" />
            <div style="float:left;">
	            <cc1:TextBoxEx ID="dteStartDate" runat="server" LabelText="Start Date/Time" LabelWidth="17em"
		            Required="true" RequiredValidatorErrMsg="Please enter a valid start date" Format="DateFormat"
		            ValidationGroup="Save" Enabled="false"></cc1:TextBoxEx>
            </div>
            <div style="float:left;">
			    <cc1:TimePicker ID="tmeStartDate" runat="server" LabelText="&nbsp;" Enabled="false" EnableViewState="false" ShowSeconds="false" LabelWidth="17em"></cc1:TimePicker>
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
        </fieldset>
        <br />
        <div style="float:right;">
            <asp:Button id="btnRecreate" runat="server" text="Recreate" width="6em" />
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Go back to the previous screen" onclick="btnBack_Click();" />
        </div>
        <div class="clearer"></div>
    </asp:Content>

