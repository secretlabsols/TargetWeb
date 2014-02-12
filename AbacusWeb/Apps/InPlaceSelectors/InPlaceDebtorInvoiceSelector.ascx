<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceDebtorInvoiceSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceDebtorInvoiceSelector" %>
<input type="text" id="txtRef" style="width:5em;" maxlength="7" runat="server" />
<input type="text" id="txtName" style="width:20em;" maxlength="25" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server" ControlToValidate="hidID"></asp:RequiredFieldValidator>
