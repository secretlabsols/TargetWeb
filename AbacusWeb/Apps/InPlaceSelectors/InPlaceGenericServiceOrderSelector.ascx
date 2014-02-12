<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceGenericServiceOrderSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceGenericServiceOrderSelector" %>

<input type="text" id="txtReference" style="width:6em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<input type="text" style="display:none;" id="hidClientID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server" ControlToValidate="hidID"></asp:RequiredFieldValidator>
