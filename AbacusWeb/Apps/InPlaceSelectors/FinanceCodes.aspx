<%@ Page Language="vb" AutoEventWireup="false" Codebehind="FinanceCodes.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.FinanceCodes" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="FinanceCodeSelector" Src="../UserControls/FinanceCodeSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<uc1:FinanceCodeSelector id="selector" runat="server"></uc1:FinanceCodeSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>