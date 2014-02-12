<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ClientGroups.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.ClientGroups" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="ClientGroupSelector" Src="../UserControls/ClientGroupSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<uc1:ClientGroupSelector id="selector" runat="server"></uc1:ClientGroupSelector>	
        <input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>