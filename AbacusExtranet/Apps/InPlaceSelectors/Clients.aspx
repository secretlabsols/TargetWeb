<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Clients.aspx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.Client" 
MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="ClientSelector" Src="../UserControls/ClientSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	
		<uc1:ClientSelector id="selector" runat="server"></uc1:ClientSelector>	
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>
    