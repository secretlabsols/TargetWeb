<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ExternalAccounts.aspx.vb"
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.ExternalAccounts"
MasterPageFile="~/Popup.master" %>

<%@ Register TagPrefix="uc1" TagName="ExternalAccount" 
Src="~/AbacusExtranet/Apps/InPlaceSelectors/ExternalAccountList.ascx" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
	<uc1:ExternalAccount ID="ExternalAccount1" runat="server"></uc1:ExternalAccount>
	<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
	 </asp:Content>