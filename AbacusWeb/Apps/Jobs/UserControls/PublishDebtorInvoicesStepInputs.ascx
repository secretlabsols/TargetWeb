<%@ Control Language="vb" AutoEventWireup="false" EnableViewState="true" CodeBehind="PublishDebtorInvoicesStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.PublishDebtorInvoicesStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceDebtorInvoice" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDebtorInvoiceSelector.ascx" %>

The filters you set will be applied at the time that the job runs to gather the job service data for processing.
<br /><br />

<fieldset id="fsDebtorInvoices" runat="server">
    <legend>Debtor Invoices</legend>  
    <div style="width:22em">
        <asp:radiobutton id="optInvoiceUnprinted" groupname="grpInvoices" TextAlign="right" width="100%" 
            runat="server" text="Unprinted Invoices" checked="True" />
    </div>
    <br />
    <div style="width:97%; float:right">
        <asp:Label ID="lblBillingType" AssociatedControlID="cboBillingType" runat="server" Text="Billing Type" Width="7.6em"></asp:Label>
        <cc1:DropDownListEx ID="cboBillingType" runat="server" Width="15em"></cc1:DropDownListEx>
    </div>
    <br /><br />
    <div style="width:22em">
        <asp:radiobutton id="optInvoiceSingle" groupname="grpInvoices" TextAlign="right" width="100%" 
            runat="server" text="Single Invoice" />
    </div>
    <br />
    <div style="width:97%; float:right">
        <uc1:InPlaceDebtorInvoice id="invoicePicker" runat="server"></uc1:InPlaceDebtorInvoice>
    </div>
    <br />
</fieldset>
<br />
<fieldset id="fsDDPaymentDates" runat="server">
    <legend>D/D Payment Dates</legend>  
    <div style="width:22em; float:left; clear:none">
        <cc1:TextBoxEx ID="txtDDNonRes" runat="server" LabelWidth="10em" LabelText="Non-Residential:" Width="18em" IsReadOnly="true" Text="(unknown)"></cc1:TextBoxEx>
    </div>
    <div style="float:left; clear:none">
        <cc1:TextBoxEx ID="txtDDRes" runat="server" LabelWidth="8em" LabelText="Residential:" Width="14em" IsReadOnly="true" Text="(unknown)"></cc1:TextBoxEx>
    </div>
    <div style="float:right; clear:none">
        <asp:Label ID="lblWarning" Font-Bold="true" runat="server" Text="NOTE: One or more of the D/D Payment Dates occurs in less than 14 days." Width="100%" ForeColor="#ff6600" />
    </div>    
</fieldset>
<br />
<fieldset id="fsTransStatements" runat="server">
    <legend>Transaction Statements</legend>  
    <div style="width:100%;">
        <asp:CheckBox ID="chkProduceStatements" runat="server" Text="Produce Transaction Statement(s)" TextAlign="Right"></asp:CheckBox>
        <br /><br />
    </div>
    <div style="width:22em; float:left; clear:none">
        <cc1:TextBoxEx ID="dteTransFrom" runat="server" LabelText="Transaction From" LabelWidth="10em" 
Format="DateFormatJquery" AllowClear="true" Width="6em" Required="true" RequiredValidatorErrMsg="A valid date must be provided" ValidationGroup="Save"></cc1:TextBoxEx>
    </div>
    <div style="float:left; clear:none">
        <cc1:TextBoxEx ID="dteTransTo" runat="server" LabelText="To" LabelWidth="2.5em" 
Format="DateFormatJquery" AllowClear="true" Width="6em" Required="true" RequiredValidatorErrMsg="A valid date must be provided" ValidationGroup="Save"></cc1:TextBoxEx>
    </div>
    <asp:RangeValidator ID="valTransFrom" ControlToValidate="dteTransFrom$txtTextBox" runat="server" ValidationGroup="Save" />      
    <asp:RangeValidator ID="valTransTo" ControlToValidate="dteTransTo$txtTextBox" runat="server" ValidationGroup="Save" />      
    <br /><br />
    <div style="width:100%; clear:both">
        <asp:Label ID="lblUsage" runat="server" AssociatedControlID="txtStatementFooter" Text="Text to appear at the foot of the Statement:" Width="24em" />
    </div>
    <cc1:TextBoxEx ID="txtStatementFooter" Rows="4" TextMode="MultiLine" runat="server" Width="30.5em"></cc1:TextBoxEx>
    <br />
    <asp:CheckBox ID="chkReplaceText" runat="server" Text="Replace standard text with the above" TextAlign="Right"></asp:CheckBox>
</fieldset>
<br />
<fieldset id="fsSort" runat="server">
    <legend>Sorting</legend>  
    <div style="width:100%;">
        <cc1:DropDownListEx ID="cboSort" runat="server" LabelText="Sort documentation by:" LabelWidth="13em" 
            Width="12em"></cc1:DropDownListEx>
    </div>
</fieldset>
<input type="hidden" id="hidCurrInvoice" runat="server" value="100" />
<input type="hidden" id="hidInvoiceChoice" runat="server" value="100" />
<input type="hidden" id="hidProduceStatements" runat="server" value="100" />
<input type="hidden" id="hidReplaceText" runat="server" value="100" />
