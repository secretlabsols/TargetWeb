<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Teams.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.Teams" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="TeamSelector" Src="../UserControls/TeamSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<uc1:TeamSelector id="selector" runat="server"></uc1:TeamSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>