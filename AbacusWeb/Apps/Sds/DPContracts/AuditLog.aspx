<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AuditLog.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPContracts.AuditLog" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
	<br />
    <cc1:DropDownListEx ID="cboTableName" runat="server" LabelText="Contract Area" LabelWidth="10em"></cc1:DropDownListEx>
	<br />
	<cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" Format="DateFormat" LabelWidth="10em"></cc1:TextBoxEx>
    <br />
    <cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Date To" Format="DateFormat" LabelWidth="10em"></cc1:TextBoxEx>
    <br />
    <input type="button" id="btnShow" value="Show Audit Log" onclick="btnShow_Click();" />
</asp:Content>