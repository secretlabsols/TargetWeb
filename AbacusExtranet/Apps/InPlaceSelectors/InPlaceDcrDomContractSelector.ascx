<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceDcrDomContractSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.InPlaceDcrDomContractSelector" %>

<input type="text" id="txtContractNumber" style="width:10em;" maxlength="25" runat="server" />
<input type="text" id="txtContractTitle" style="width:20em;" maxlength="30" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />

<input type="text" style="display:none;" id="txtHidselectedDcrDomContractId" runat="server" />

<asp:RequiredFieldValidator id="valRequired" runat="server" ValidationGroup="Save" ControlToValidate="txtContractNumber"></asp:RequiredFieldValidator>