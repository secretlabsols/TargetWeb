<%@ Page Language="vb" AutoEventWireup="false" Codebehind="RecreateBatchFiles.aspx.vb" Inherits="Target.Abacus.Web.Apps.General.DebtorInvoices.RecreateBatchFiles" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details for the selected Debtor Invoice Batch.
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
	    <label class="label" for="lblInvoiceValue">Total Value</label>
	    <asp:Label id="lblInvoiceValue" runat="server" CssClass="content"></asp:Label>
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
            <div style="float:left;">
                <asp:CheckBox ID="chkRereadData" runat="server" Text="Re-read underlying data" TextAlign="left" LabelWidth="16.75em"
                    OnCheckChanged="RereadDataClicked" AutoPostBack="true"></asp:CheckBox>
            </div>                    
            <div style="float:left;">
                <label class="label" for="lblRereadData"></label>
                <asp:Label id="lblRereadData" runat="server" CssClass="content" width="70%"></asp:Label>
            </div>
            <br /><br />
	        <div style="float:left;">
                <asp:CheckBox ID="chkCustomerFile" runat="server" Text="Re-create Customer File" TextAlign="left" LabelWidth="14.75em"></asp:CheckBox>
            </div>
	        <div style="float:left;">
	            <label class="label" for="lblCustomerFile"></label>
	            <asp:Label id="lblCustomerFile" runat="server" CssClass="content" width="70%"></asp:Label>
	        </div>
            <br /><br />
	        <asp:radiobutton id="optCreateNow" groupname="grpCreation" TextAlign="left" height="2em" width="100%" 
	            runat="server" text="Create files now" checked="true" OnCheckedChanged="CreationOptionClicked" 
	            AutoPostBack="true" />
            <asp:radiobutton id="optDefer" groupname="grpCreation" TextAlign="left" height="2em" width="100%" 
                runat="server" text="Defer creation of files" OnCheckedChanged="CreationOptionClicked" 
                AutoPostBack="true" />
	        <div style="float:left;">
	            <cc1:TextBoxEx ID="dteStartDate" runat="server" LabelText="Start Date/Time" LabelWidth="15em"
		            Required="true" RequiredValidatorErrMsg="Please enter a valid start date" Format="DateFormat"
		            ValidationGroup="Save" Enabled="false"></cc1:TextBoxEx>
            </div>
            <div style="float:left;">
			    <cc1:TimePicker ID="tmeStartDate" runat="server" LabelText="&nbsp;" Enabled="false" EnableViewState="false" ShowSeconds="false" LabelWidth="1em"></cc1:TimePicker>
			</div>
			<div class="clearer"></div>
			<br />
			<cc1:TextBoxEx ID="dtePostingDate" runat="server" LabelText="Posting Date" LabelWidth="15em"
                Required="true" RequiredValidatorErrMsg="Please enter a valid posting date" Format="DateFormat"
				ValidationGroup="Save"></cc1:TextBoxEx>
		    <br />
            <cc1:DropDownListEx ID="cboPostingYear" runat="server" LabelText="Posting Year" LabelWidth="15em"
                Required="true" RequiredValidatorErrMsg="Please select a posting year" ValidationGroup="Save"></cc1:DropDownListEx>
			<br />
            <cc1:DropDownListEx ID="cboPeriodNum" runat="server" LabelText="Period Number" LabelWidth="15em"
			    Required="true" RequiredValidatorErrMsg="Please select a period number" ValidationGroup="Save"></cc1:DropDownListEx>
        </fieldset>
        <br />
        <div style="float:right;">
            <asp:Button id="btnRecreate" runat="server" text="Recreate" width="6em" />
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Go back to the previous screen" onclick="btnBack_Click();" />
        </div>
        <div class="clearer"></div>
    </asp:Content>

