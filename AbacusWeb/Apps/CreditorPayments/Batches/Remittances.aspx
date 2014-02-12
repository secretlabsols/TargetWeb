<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Remittances.aspx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.Batches.Remittances" %>
<%@ Register TagPrefix="uc1" TagName="FilterCriteria" Src="UserControls/ucCreditorPaymentBatchCriteria.ascx" %>
<%@ Register TagPrefix="uc2" TagName="RemittanceSelector" Src="UserControls/RemittanceSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows the user to create or view remittances within the selected batch. 
</asp:Content>

<asp:Content ID="conPageError" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">
    <uc3:StdButtons id="stdButtons1" runat="server" />
    <uc1:FilterCriteria id="FilterCriteria1" runat="server" />	    	    
    <br />
    <uc2:RemittanceSelector id="RemittanceSelector1" runat="server" />
    <div class="clearer"></div>	    
</asp:Content>