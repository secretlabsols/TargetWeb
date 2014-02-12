<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceExpenditureAccountSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceExpenditureAccountSelector" %>

<input type="text" id="txtName" style="width:20em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<input type="text" style="display:none;" id="expAccountType" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server"></asp:RequiredFieldValidator>