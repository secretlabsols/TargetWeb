<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceExternalAccountSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.InPlaceExternalAccountSelector" %>

<input type="text" id="txtExternalAccount" style="width:20em;" maxlength="25" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="txtHidExternalAccountID" runat="server" />

<asp:RequiredFieldValidator id="valRequired" runat="server" ValidationGroup="Save" ControlToValidate="txtHidExternalAccountID"></asp:RequiredFieldValidator>