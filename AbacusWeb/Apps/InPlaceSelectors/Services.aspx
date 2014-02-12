<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Services.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.Services" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="ServiceSelector" Src="../UserControls/ServiceSelector.ascx" %>

    <asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server">
        <br /> 
        Please select a service from the list below
        <uc1:ServiceSelector id="selector" runat="server" />
	    <input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
	    <input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>