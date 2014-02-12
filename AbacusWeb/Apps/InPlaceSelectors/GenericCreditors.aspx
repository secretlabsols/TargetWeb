<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GenericCreditors.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.GenericCreditors" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="GenericCreditorSelector" Src="../UserControls/GenericCreditorSelector.ascx" %>

    <asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server">
        <br /> 
        Please select a generic creditor from the list below
        <uc1:GenericCreditorSelector id="selector" runat="server" />
	    <input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
	    <input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>