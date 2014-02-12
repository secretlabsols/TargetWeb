<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Contracts.aspx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.Contracts" 
MasterPageFile="~/Popup.master" %>

<%@ Register TagPrefix="uc1" TagName="ContractSelector" 
Src="../UserControls/DomContractSelector.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
		<uc1:ContractSelector id="selector" runat="server"></uc1:ContractSelector>
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>