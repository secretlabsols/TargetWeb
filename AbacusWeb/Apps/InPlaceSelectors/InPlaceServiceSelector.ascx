<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceServiceSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceServiceSelector" %>
<input type="text" id="txtName" style="width:20em;" maxlength="25" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<label id="lblInfo" title="" runat="server" style="padding-bottom : 0.25em" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server" ControlToValidate="txtName" />