<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DebtorInvoices.aspx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.DebtorInvoices" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="DebtorInvoiceSelector" Src="../UserControls/DebtorInvoiceSelector.ascx" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
        <br /> Please select a debtor invoice from the list below
	    <uc1:DebtorInvoiceSelector id="selector" runat="server"></uc1:DebtorInvoiceSelector>	
	    <input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />
	    <input type="button" id="btnSelect" value="Select" style="float:right;" onclick="btnSelect_Click();" />
    </asp:Content>
