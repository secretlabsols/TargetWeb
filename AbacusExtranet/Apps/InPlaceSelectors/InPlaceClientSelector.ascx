<%@ Control Language="vb" AutoEventWireup="false" 
CodeBehind="InPlaceClientSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.InPlaceClientSelector" %>


<input type="text" id="txtReference" style="width:6em;" runat="server" />
<input type="text" id="txtName" style="width:19em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server" ControlToValidate="hidID"></asp:RequiredFieldValidator>