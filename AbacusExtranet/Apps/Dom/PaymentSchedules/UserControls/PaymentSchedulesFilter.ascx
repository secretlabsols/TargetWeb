<%@ Control Language="vb" AutoEventWireup="false" Codebehind="PaymentSchedulesFilter.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.UserControls.PaymentSchedulesFilter" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<div class="clearer"></div>
<br />
<fieldset>
    <legend>Header Details</legend>

   <div style="float:left;" >
    <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" Format="TextFormat" LabelWidth="8em" ClientIDMode="Inherit"></cc1:TextBoxEx>
    </div>
    <div style="float:left;margin-left:5px;" >
     <a href="#" onclick="javascript:ShowHelp();" >
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/help16.png" alt="help" ></asp:Image>
     </a>
     </div>
     <div class="clearer"></div>
    <br />
    <div style="float:left;"><cc1:TextBoxEx ID="dtePeriodFrom" runat="server" LabelText="Period From" LabelWidth="8em" Format="DateFormat"></cc1:TextBoxEx></div>
    <div style="float:left;margin-left:1em;"><cc1:TextBoxEx ID="dtePeriodTo" runat="server" LabelText="To" Format="DateFormat" LabelWidth="2em"></cc1:TextBoxEx></div>
    <div class="clearer"></div><br />
    <label style="width:8em;float:left;">Visit-based</label>
    <cc1:CheckBoxEx ID="chkVisitBasedYes" runat="server" Text="Yes"></cc1:CheckBoxEx>
    <div style="float:left;margin-left:1em;margin-right:1em;">OR</div>
    <cc1:CheckBoxEx ID="chkVisitBasedNo" runat="server" Text="No"></cc1:CheckBoxEx>

</fieldset>

<div class="clearer" style="margin:0.5em 0 0.5em 0">AND</div>

<fieldset>
    <legend>Unprocessed Pro forma Invoices</legend>

    <cc1:CheckBoxEx ID="chkProformaInvoicesNone" runat="server" Text="Having no Pro forma Invoices"></cc1:CheckBoxEx>
    <div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>
    <cc1:CheckBoxEx ID="chkProformaInvoicesAwaiting" runat="server" Text="Having Pro forma Invoices that are 'Awaiting Verification'"></cc1:CheckBoxEx>
    <div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>
    <cc1:CheckBoxEx ID="chkProformaInvoicesVerified" runat="server" Text="Having 'verified' Pro forma Invoices"></cc1:CheckBoxEx>

</fieldset>

<div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>

<fieldset>
    <legend>Provider Invoices</legend>

    <cc1:CheckBoxEx ID="chkInvoicesUnpaid" runat="server" Text="Having Unpaid Invoices"></cc1:CheckBoxEx>
    <div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>
    <cc1:CheckBoxEx ID="chkInvoicesSuspended" runat="server" Text="Having Suspended Invoices"></cc1:CheckBoxEx>
    <div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>
    <cc1:CheckBoxEx ID="chkInvoicesAuthorised" runat="server" Text="Having Authorised Invoices"></cc1:CheckBoxEx>
    <div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>
    <cc1:CheckBoxEx ID="chkInvoicesPaid" runat="server" Text="Having Paid Invoices"></cc1:CheckBoxEx>

</fieldset>

<div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>

<fieldset style="margin-bottom:1em;">
    <legend>Unprocessed Visit Amendment Requests</legend>

    <cc1:CheckBoxEx ID="chkVARawaiting" runat="server" Text="Having Visit Amendment Requests that are 'Awaiting Verification'"></cc1:CheckBoxEx>
    <div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>
    <cc1:CheckBoxEx ID="chkVARverified" runat="server" Text="Having 'verified' Visit Amendment Requests"></cc1:CheckBoxEx>
    <div class="clearer" style="margin:0.5em 0 0.5em 0">OR</div>
    <cc1:CheckBoxEx ID="chkVARdeclined" runat="server" Text="Having 'declined' Visit Amendment Requests"></cc1:CheckBoxEx>

</fieldset>
