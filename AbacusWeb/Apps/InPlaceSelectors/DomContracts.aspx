<%@ Page Language="vb" AutoEventWireup="false" Codebehind="DomContracts.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.DomContracts" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="DomContractSelector" Src="../UserControls/DomContractSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<uc1:DomContractSelector id="selector" runat="server"></uc1:DomContractSelector>
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>