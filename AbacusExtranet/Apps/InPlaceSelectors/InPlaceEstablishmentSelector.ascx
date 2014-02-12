<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceEstablishmentSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.InPlaceEstablishmentSelector" %>
<input type="text" id="txtReference" style="width:7em;" runat="server" />
<input type="text" id="txtName" style="width:16.1em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<asp:RequiredFieldValidator id="valRequired" runat="server"  ValidationGroup="Save"  ControlToValidate="hidID"></asp:RequiredFieldValidator>