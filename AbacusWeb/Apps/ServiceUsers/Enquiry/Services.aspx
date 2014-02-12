<%@ Page Language="vb" AutoEventWireup="false" AspCompat="true" CodeBehind="Services.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.Services" MasterPageFile="~/popup.master"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc2" TagName="CommissionedService" Src="~/AbacusWeb/Apps/UserControls/ServiceOrderSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="DirectPaymentContracts" Src="~/AbacusWeb/Apps/UserControls/DPContractSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="SpendPlans" Src="~/AbacusWeb/Apps/UserControls/SpendPlanSelector.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPPageOverview" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
<style type="text/css">
    .pnl{
    font-weight: bold;
    background-color: #eeeeee;
    padding: 5px;
    cursor: pointer; 
    border: solid 1px #c0c0c0
    }
</style>
    <div class="clearer"></div>
    <cc1:CollapsiblePanel id="cpSpendPlans" runat="server" HeaderLinkText="Spend Plans" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrServices')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrServices')">
        <ContentTemplate>
            <uc2:SpendPlans id="spendPlans1" runat="server"></uc2:SpendPlans> 
        </ContentTemplate>
    </cc1:CollapsiblePanel>
    <cc1:CollapsiblePanel id="cpCommissionedService" runat="server" HeaderLinkText="Commissioned Service" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrServices')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrServices')">
        <ContentTemplate>
            <uc2:CommissionedService id="serviceOrderSelector1" runat="server"></uc2:CommissionedService>
        </ContentTemplate>
    </cc1:CollapsiblePanel>  
    <cc1:CollapsiblePanel id="cpDirectPaymentContracts" runat="server" HeaderLinkText="Direct Payment Contracts" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrServices')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrServices')">
        <ContentTemplate>
            <uc3:DirectPaymentContracts id="dpConractSelector1" runat="server"></uc3:DirectPaymentContracts>
        </ContentTemplate>
    </cc1:CollapsiblePanel>  
</asp:Content>


