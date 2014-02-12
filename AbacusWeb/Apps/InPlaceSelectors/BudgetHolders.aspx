<%@ Page Language="vb" AutoEventWireup="false" Codebehind="BudgetHolders.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.BudgetHolders" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="BudgetHolderSelector" Src="../UserControls/BudgetHolderSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<uc1:BudgetHolderSelector id="selector" runat="server"></uc1:BudgetHolderSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>