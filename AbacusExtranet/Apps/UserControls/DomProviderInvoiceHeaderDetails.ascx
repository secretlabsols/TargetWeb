<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DomProviderInvoiceHeaderDetails.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.DomProviderInvoiceHeaderDetails" %>
<%@ Register TagPrefix="uc1" TagName="Header" 
Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>

<fieldset><legend>Invoice Header</legend>
<div style="margin-bottom:5px;">
    <uc1:Header runat="server" ID="pScheduleHeader" />
</div>
<div style="margin-bottom:5px;">
    <label class="label" for="lblProvider" style="width:11.5em">Provider</label>
    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
</div>
<div style="margin-bottom:5px;">
    <label class="label" for="lblContract" style="width:11.5em">Contract</label>
    <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
</div>
<div style="margin-bottom:5px;">
    <asp:Label id="lblForServiceUser" runat="server" width="11.2em" >Service User</asp:Label>
    <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
</div>
<div style="margin-bottom:5px;">
    <label class="label" for="lblInvoicePeriod" style="width:11.5em">Period From</label>
    <asp:Label id="lblInvoicePeriod" runat="server" CssClass="content"></asp:Label>
</div>
<div style="margin-bottom:5px;">
    <div style="float:left;">
        <label class="label" for="lblInvoiceNumber" style="width:11.5em">Invoice Number</label>
        <asp:Label id="lblInvoiceNumber" runat="server" CssClass="content" Width="12em"></asp:Label>
    </div>
    <div style="float:left; margin-left:15px;">
        <label class="label" for="lblReference" style="width:9.5em">Reference</label>
        <asp:Label id="lblReference" runat="server" CssClass="content"></asp:Label>
    </div>
    <div class="clearer"></div>
</div>
<div style="margin-bottom:5px;">
    <div style="float:left;">
        <label class="label" for="lblInvoiceTotal" style="width:11.5em">Invoice Total</label>
        <asp:Label id="lblInvoiceTotal" runat="server" CssClass="content" Width="12em"></asp:Label>
     </div>
     <div style="float:left; margin-left:15px;">
        <label class="label" for="lblDirectIncome" style="width:9.5em">Direct Income</label>
        <asp:Label id="lblDirectIncome" runat="server" CssClass="content"></asp:Label>
     </div>
     <div class="clearer"></div>
</div>

<div style="margin-bottom:5px;">
    <label class="label" for="lblInvoiceStatus" style="width:11.5em">Invoice Status</label>
    <asp:Label id="lblInvoiceStatus" runat="server" CssClass="content"></asp:Label>
</div>



</fieldset>