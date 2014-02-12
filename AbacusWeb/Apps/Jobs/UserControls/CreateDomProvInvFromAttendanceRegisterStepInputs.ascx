<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CreateDomProvInvFromAttendanceRegisterStepInputs.ascx.vb" 
Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.CreateDomProvInvFromAttendanceRegisterStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>

To submit a job that will create domiciliary provider invoices from submitted service registers, set the required filters below and click the "Create New Job" button.
<br />
The filters you set will be applied at the time that the job runs to gather the Service Registers for processing.
<br />
To preview the Service Registers that currently match the filters you have set, click the "Preview Service Registers" button.

<br /><br />
<cc1:CheckBoxEx ID="chkNewRegisters" runat="server" Text="New Registers" CheckBoxCssClass="chkBoxStyle" LabelWidth="8em"></cc1:CheckBoxEx>
<cc1:CheckBoxEx ID="chkAmendedRegisters" runat="server" Text="Amended Registers" CheckBoxCssClass="chkBoxStyle" LabelWidth="10.5em"></cc1:CheckBoxEx>
<br /><br />
<asp:Label ID="lblProvider" AssociatedControlID="provider" runat="server" Text="Provider" Width="10em"></asp:Label>
<uc1:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc1:InPlaceEstablishment>
<br />
<asp:Label ID="lblContract" AssociatedControlID="domContract" runat="server" Text="Contract" Width="10em"></asp:Label>
<uc2:InPlaceDomContract id="domContract" runat="server"></uc2:InPlaceDomContract>
<br />
<div>
    <input type="button" id="btnPreviewAttendanceRegisters" value="Preview Service Registers" style="width:16em;" onclick="btnPreviewAttendanceRegisters_Click();" />
</div>
<input type="hidden" id="hidCreatingJob" runat="server" value="1" />