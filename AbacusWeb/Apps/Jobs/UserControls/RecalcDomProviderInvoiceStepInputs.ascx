<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RecalcDomProviderInvoiceStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.RecalcDomProviderInvoiceStepInputs" EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>

To submit a job that will recalculate existing provider invoices, set the required filters below and click the "Create New Job" button.

<div style="float:left;margin-top:13px;">
<asp:Label ID="lblProvider" AssociatedControlID="provider" runat="server" Text="Provider" Width="10.1em"></asp:Label>
<uc1:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc1:InPlaceEstablishment>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:13px;">
<asp:Label ID="lblContract" AssociatedControlID="domContract" runat="server" Text="Contract" Width="10.1em"></asp:Label>
<uc2:InPlaceDomContract id="domContract" runat="server"></uc2:InPlaceDomContract>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:13px;">
<cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Week Ending From" LabelWidth="10.4em" 
Format="DateFormatJquery" AllowClear="true" Width="6em"></cc1:TextBoxEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:13px;">
<cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Week Ending To" LabelWidth="10.4em" 
Format="DateFormatJquery" AllowClear="true" Width="6em"></cc1:TextBoxEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:13px;">
<cc1:CheckBoxEx ID="chkCreateProforma" runat="server" Text="Create Verified Pro Forma Invoices" LabelWidth="18em"></cc1:CheckBoxEx>
</div>
<div class="clearer"></div>
