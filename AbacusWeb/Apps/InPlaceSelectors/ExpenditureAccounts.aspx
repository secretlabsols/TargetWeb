<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ExpenditureAccounts.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.ExpenditureAccounts" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="ExpenditureAccountSelector" Src="../UserControls/ExpenditureAccountSelector.ascx" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
		<uc1:ExpenditureAccountSelector id="selector" runat="server"></uc1:ExpenditureAccountSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>
