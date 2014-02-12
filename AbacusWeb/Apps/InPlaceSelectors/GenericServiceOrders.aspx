<%@ Page Language="vb" AutoEventWireup="false" Codebehind="GenericServiceOrders.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.GenericServiceOrders" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="ServiceOrderSelector" Src="../UserControls/ServiceOrderSelector.ascx" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
		<uc1:ServiceOrderSelector id="selector" runat="server"></uc1:ServiceOrderSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>
