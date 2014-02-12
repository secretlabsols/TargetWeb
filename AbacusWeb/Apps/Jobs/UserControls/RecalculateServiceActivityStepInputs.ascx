<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RecalculateServiceActivityStepInputs.ascx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.RecalculateServiceActivityStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

To submit a job that will recalculate service activity, set the required filters below and click the "Create New Job" button.
<br />

<br /><br />
<asp:Label ID="Label1" AssociatedControlID="serviceUser" runat="server" Text="Service User" Width="11em" style="float:left;" ></asp:Label>
<uc3:InPlaceClient id="serviceUser"  Required="false" runat="server"></uc3:InPlaceClient>
<div style="float:left;margin-top:13px;">
<cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Period From" LabelWidth="11em" Format="DateFormatJquery" AllowClear="true"></cc1:TextBoxEx>
</div>
<div class="clearer"></div>
<div style="float:left;margin-top:13px;">
<cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Period To" LabelWidth="11em"  Format="DateFormatJquery" AllowClear="true"></cc1:TextBoxEx>
</div>
<div class="clearer"></div>
<br />
<cc1:CheckBoxEx ID="chkForceRecon" runat="server" Text="Force Reconsideration" CheckBoxCssClass="chkBoxStyle" LabelWidth="11em"></cc1:CheckBoxEx>