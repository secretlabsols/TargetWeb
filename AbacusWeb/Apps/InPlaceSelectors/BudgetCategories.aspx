<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BudgetCategories.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.BudgetCategories" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="BudgetCategorySelector" Src="../UserControls/BudgetCategorySelector.ascx" %>

    <asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server">
        <br /> 
        Please select a budget category from the list below
        <uc1:BudgetCategorySelector id="selector" runat="server" />
	    <input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
	    <input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>