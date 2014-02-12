<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceServiceTypeSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceServiceTypeSelector" %>

<input type="text" id="txtName" style="width:20em;" maxlength="25" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server" ControlToValidate="txtName"></asp:RequiredFieldValidator>