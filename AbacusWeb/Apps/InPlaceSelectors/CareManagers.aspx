<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CareManagers.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.CareManagers" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="CareManagerSelector" Src="../UserControls/CareManagerSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<uc1:CareManagerSelector id="selector" runat="server"></uc1:CareManagerSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>