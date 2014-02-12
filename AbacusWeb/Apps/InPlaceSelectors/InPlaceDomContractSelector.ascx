<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceDomContractSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceDomContractSelector" %>

<input type="text" id="txtNumber" style="width:6em;" runat="server" />
<input type="text" id="txtTitle" style="width:19em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server" ControlToValidate="hidID"></asp:RequiredFieldValidator>
