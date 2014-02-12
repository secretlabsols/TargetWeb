<%@ Page Language="vb" AutoEventWireup="false" Codebehind="OtherFundingOrganizations.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.OtherFundingOrganizations" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="OtherFundingOrgSelector" Src="../UserControls/OtherFundingOrgSelector.ascx" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
		<uc1:OtherFundingOrgSelector id="selector" runat="server"></uc1:OtherFundingOrgSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>
