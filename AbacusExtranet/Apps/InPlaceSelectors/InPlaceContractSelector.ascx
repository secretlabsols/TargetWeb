<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceContractSelector.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.InPlaceContractSelector" %>

<input type="text" id="txtContractNumber" style="width:7em;" maxlength="25" runat="server" />
<input type="text" id="txtContractTitle" style="width:16.1em;" maxlength="30" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />

<input type="text" style="display:none;" id="txtHidselectedDomContractId" runat="server" />
<input type="text" style="display:none;" id="txtHidFrameworkTypeAbbr" runat="server" />

<asp:RequiredFieldValidator id="valRequired" runat="server" ValidationGroup="Save" ControlToValidate="txtContractNumber"></asp:RequiredFieldValidator>
