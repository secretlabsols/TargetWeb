<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlacePctSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlacePctSelector" %>

<input type="text" id="txtName" style="width:19em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" ControlToValidate="hidID" runat="server"></asp:RequiredFieldValidator>