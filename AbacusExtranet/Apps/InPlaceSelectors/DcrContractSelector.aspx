<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DcrContractSelector.aspx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.DcrContractSelector" 
MasterPageFile="~/Popup.master" %>

<%@ Register TagPrefix="uc1" TagName="DcrDomContractSelector" 
Src="~/AbacusExtranet/Apps/UserControls/DcrDomContractSelector.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
<uc1:DcrDomContractSelector ID="DcrDomContractSelector1" runat="server"></uc1:DcrDomContractSelector>
	
<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
<input type="button" id="btnSelectContract" value="Select" style="float:right;" onclick="btnSelectContract_Click();" />
 </asp:Content>
	 