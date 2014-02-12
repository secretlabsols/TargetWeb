<%@ Control Language="vb" AutoEventWireup="false" Codebehind="financialYrQtrSelector.ascx.vb" Inherits="Target.SP.Web.FinancialYrQtrSelector" 
	TargetSchema="http://schemas.microsoft.com/intellisense/ie5" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<fieldset style="margin-top:1em;">
		<legend><asp:Literal id="litCaption" runat="server"></asp:Literal></legend>
		<asp:Label style="width:11em;" id="lblFinancialYear" runat="server">Financial Year</asp:Label> 
		<cc1:DropdownListEx id="cboFinancialYear" runat="server" Required="True" RequiredValidatorErrMsg="Please select a Financial Year."></cc1:DropdownListEx>
		<br >
		<asp:Label style="width:11em;" id="lblQuarter" runat="server">Quarter</asp:Label> 
		<cc1:DropdownListEx id="cboQuarter" runat="server" Required="True" RequiredValidatorErrMsg="Please select a Quarter."></cc1:DropdownListEx>
	</fieldset>

