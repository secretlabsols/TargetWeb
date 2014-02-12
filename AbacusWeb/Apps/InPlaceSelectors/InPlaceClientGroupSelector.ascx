<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceClientGroupSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceClientGroupSelector" %>

<input type="text" id="txtReference" style="width:6em;" runat="server" />
<input type="text" id="txtName" style="width:19em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" ControlToValidate="hidID" runat="server"></asp:RequiredFieldValidator>
