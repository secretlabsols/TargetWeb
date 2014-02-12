
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Establishments.aspx.vb" 
Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.Establishments" 
MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="EstablishmentSelector" 
Src="../UserControls/EstablishmentSelector.ascx" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
		<uc1:EstablishmentSelector id="selector" runat="server"></uc1:EstablishmentSelector>
		<input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
		<input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
        
    </asp:Content>
    

