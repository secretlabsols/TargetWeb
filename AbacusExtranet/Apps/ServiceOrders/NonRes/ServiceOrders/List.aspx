<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="List.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.ServiceOrders.NonRes.ServiceOrders.List" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
<asp:content contentplaceholderid="MPPageOverview" runat="server">
    The wizard allows you to search for and view existing Service Orders filtered by Provider and Contract and, optionally, by a variety of other attributes
</asp:content>

<asp:content contentplaceholderid="MPContent" runat="server">
    <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
</asp:content>
