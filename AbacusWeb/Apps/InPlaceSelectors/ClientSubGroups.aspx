<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ClientSubGroups.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.ClientSubGroups" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="ClientSubGroupSelector" Src="../UserControls/ClientSubGroupSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<uc1:ClientSubGroupSelector id="selector" runat="server"></uc1:ClientSubGroupSelector>	
        <input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>