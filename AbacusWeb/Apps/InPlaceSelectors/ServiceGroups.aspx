<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ServiceGroups.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.ServiceGroups" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="ServiceGroupSelector" Src="../UserControls/ServiceGroupSelector.ascx" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
        <br /> Please select a service group from the list below
	    <uc1:ServiceGroupSelector id="selector" runat="server"></uc1:ServiceGroupSelector>	
	    <input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
	    <input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>
