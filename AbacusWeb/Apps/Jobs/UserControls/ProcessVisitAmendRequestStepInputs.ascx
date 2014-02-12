<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ProcessVisitAmendRequestStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.ProcessVisitAmendRequestStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>

To submit a job that will process visit amendment requests, set the required filters below and click the "Create New Job" button.


<div style="float:left;margin-top:8px;">
<asp:Label ID="lblProvider" AssociatedControlID="provider" runat="server" Text="Provider" Width="10.1em"></asp:Label>
<uc1:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc1:InPlaceEstablishment>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:5px;">
<cc1:DropDownListEx ID="cboContractType" runat="server" LabelText="Contract Type" LabelWidth="10.4em"></cc1:DropDownListEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:7px;">
<cc1:DropDownListEx ID="cboContractGroup" runat="server" LabelText="Contract Group" LabelWidth="10.4em"></cc1:DropDownListEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:5px;">
<asp:Label ID="lblContract" AssociatedControlID="domContract" runat="server" Text="Contract" Width="10.1em"></asp:Label>
<uc2:InPlaceDomContract id="domContract" runat="server"></uc2:InPlaceDomContract>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:5px;">
<cc1:CheckBoxEx ID="chkCreateProforma" runat="server" Text="Create Verified Pro Forma Invoices" LabelWidth="18em"></cc1:CheckBoxEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:5px;">
<cc1:CheckBoxEx ID="chkCreateProvider" runat="server" Text="Create Provider Invoices" LabelWidth="18em"></cc1:CheckBoxEx>
</div>
<div class="clearer"></div>