<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceFinanceCodeSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceFinanceCodeSelector" %>

<input type="text" id="txtName" style="width:12em;" maxlength="25" runat="server" />
<input type="button" id="btnFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<input type="text" style="display:none;" id="hidCategoryID" runat="server" />
<input type="text" style="display:none;" id="hidExpenditureAccountID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" runat="server" ControlToValidate="txtName"></asp:RequiredFieldValidator>
