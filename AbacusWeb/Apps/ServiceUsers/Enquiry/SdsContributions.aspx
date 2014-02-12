<%@ Page Language="vb" AutoEventWireup="false" AspCompat="true" CodeBehind="SdsContributions.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.SdsContributions" MasterPageFile="~/popup.master"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc1" TagName="SdsContributionSummary" Src="~/AbacusWeb/Apps/UserControls/SdsContributionSummary.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ServiceUserContributionLevel" Src="~/AbacusWeb/Apps/UserControls/SUCLevelSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="PendingServiceUserContributionLevel" Src="~/AbacusWeb/Apps/UserControls/PendingSuclChangesSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="FinancialAssessmentSelector" Src="~/AbacusWeb/Apps/UserControls/FinancialAssessmentSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="HistoricNotificationLetters" Src="~/AbacusWeb/Apps/UserControls/HistoricNotificationLetters.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <div class="clearer"></div>
    <cc1:CollapsiblePanel id="cpContributionSummary" runat="server" HeaderLinkText="Contribution Summary" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')">
        <ContentTemplate>
            <uc1:SdsContributionSummary id="sdsContributionSummary1" runat="server"></uc1:SdsContributionSummary>
        </ContentTemplate>
    </cc1:CollapsiblePanel>
    <cc1:CollapsiblePanel id="cpActiveContributionLevels" runat="server" HeaderLinkText="Active Contribution Levels" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')">
        <ContentTemplate>
            <uc2:ServiceUserContributionLevel id="serviceUserContributionLevel1" runat="server"></uc2:ServiceUserContributionLevel>
        </ContentTemplate>
    </cc1:CollapsiblePanel>
    <cc1:CollapsiblePanel id="cpPendingContributionLevels" runat="server" HeaderLinkText="Pending Contribution Levels" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')">
        <ContentTemplate>
            <uc3:PendingServiceUserContributionLevel id="pendingServiceUserContributionLevel1" runat="server"></uc3:PendingServiceUserContributionLevel>            
        </ContentTemplate>
    </cc1:CollapsiblePanel>
    <cc1:CollapsiblePanel id="cpFinancialAssessments" runat="server" HeaderLinkText="Financial Assessments" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')">
        <ContentTemplate>
            <uc4:FinancialAssessmentSelector id="financialAssessmentSelector1" runat="server"></uc4:FinancialAssessmentSelector>   
        </ContentTemplate>
    </cc1:CollapsiblePanel>
     <cc1:CollapsiblePanel id="cpHistoricNotificationLetters" runat="server" HeaderLinkText="Historic Notification Letters" 
            ExpandedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')" 
            CollapsedJS="parent.resizeIframe(document.body.scrollHeight, 'ifrSdsContribs')">
        <ContentTemplate>
            <uc5:HistoricNotificationLetters id="historicNotificationLetters1" runat="server"></uc5:HistoricNotificationLetters>   
        </ContentTemplate>
    </cc1:CollapsiblePanel>
</asp:Content>


