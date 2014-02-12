<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CommitmentsInterfaceStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.CommitmentsInterfaceStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

This job will calculate the unpaid financial commitment based on planned service recorded within Abacus to the date entered below.
<br />
Please select the services you wish to report on, then click the 'Create' New Job' button.
<br />
<div style="float:left;margin-top:13px;">
<cc1:TextBoxEx ID="dteCommitmentDate" runat="server" LabelText="Include Commitments To" LabelWidth="16em" Format="DateFormat"
    Required="true" RequiredValidatorErrMsg="A valid date must be provided" ValidationGroup="Save"></cc1:TextBoxEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:13px;">
<cc1:CheckBoxEx ID="chkResidential" runat="server" Text="Residential Care" CheckBoxCssClass="chkBoxStyle" LabelWidth="16em"></cc1:CheckBoxEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:13px;">
<cc1:CheckBoxEx ID="chkDP" runat="server" Text="Direct Payments" CheckBoxCssClass="chkBoxStyle" LabelWidth="16em"></cc1:CheckBoxEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:13px;">
<cc1:CheckBoxEx ID="chkIncludeLastRun" runat="server" Text="Include Negated Commitment from previous run?" CheckBoxCssClass="chkBoxStyle" LabelWidth="16em"></cc1:CheckBoxEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:3px;">
<b>(Only remove for the first run of the Financial Year)</b>
</div>
