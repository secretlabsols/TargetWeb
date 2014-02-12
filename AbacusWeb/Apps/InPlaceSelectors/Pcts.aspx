<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Pcts.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.Pcts" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="PctSelector" Src="../UserControls/PctSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<uc1:PctSelector id="selector" runat="server"></uc1:PctSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>