<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CreateDomProviderInvoiceStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.CreateDomProviderInvoiceStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>

To submit a job that will create provider invoices from pro forma invoices, set the required filters below and click the "Create New Job" button.
<br />
The filters you set will be applied at the time that the job runs to gather the pro forma invoices for processing.
<br />
To preview the pro forma invoices that currently match the filters you have set, click the "Preview Pro forma Invoices" button.

<br /><br />
<asp:PlaceHolder ID="phBatchType" runat="server"></asp:PlaceHolder>
<br />
<asp:Label ID="lblProvider" AssociatedControlID="provider" runat="server" style="float:left;" Text="Provider" Width="10em"></asp:Label>
<uc1:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc1:InPlaceEstablishment>
<br />
<cc1:DropDownListEx ID="cboContractType" runat="server" LabelText="Contract Type" LabelWidth="10em"></cc1:DropDownListEx>
<br />
<cc1:DropDownListEx ID="cboContractGroup" runat="server" LabelText="Contract Group" LabelWidth="10em"></cc1:DropDownListEx>
<br />
<asp:Label ID="lblContract" AssociatedControlID="domContract" runat="server" style="float:left;" Text="Contract" Width="10em"></asp:Label>
<uc2:InPlaceDomContract id="domContract" style="float:left;"  runat="server"></uc2:InPlaceDomContract>
<br />
<div>
    <input type="button" id="btnProformaInvoices" value="Preview Pro forma Invoices" style="width:14em;" onclick="btnProformaInvoice_Click();" />
</div>
<input type="hidden" id="hidCreatingJob" runat="server" value="1" />