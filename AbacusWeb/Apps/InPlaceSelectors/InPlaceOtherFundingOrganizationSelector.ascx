<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceOtherFundingOrganizationSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceOtherFundingOrganizationSelector" %>

<input type="text" id="txtName" style="width:19em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server"></asp:RequiredFieldValidator>