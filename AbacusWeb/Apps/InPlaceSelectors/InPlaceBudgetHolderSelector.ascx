<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceBudgetHolderSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceBudgetHolderSelector" %>

<input type="text" id="txtReference" style="width:6em;" runat="server" />
<input type="text" id="txtName" style="width:19em;" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<input type="text" style="display:none;" id="hidRedundant" runat="server" />
<input type="text" style="display:none;" id="hidServiceUser" runat="server" />
<asp:RequiredFieldValidator id="valRequired" ControlToValidate="hidID" runat="server"></asp:RequiredFieldValidator>