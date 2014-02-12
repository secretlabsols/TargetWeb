<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ThirdPartys.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.ThirdParty" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="ThirdPartySelector" Src="../UserControls/ThirdPartySelector.ascx" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
		<uc1:ThirdPartySelector id="selectorTP" runat="server"></uc1:ThirdPartySelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>
