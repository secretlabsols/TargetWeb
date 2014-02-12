<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReCreate.aspx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.Batches.ReCreate" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="FilterCriteria" Src="UserControls/ucCreditorPaymentBatchCriteria.ascx" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CreateJobPointInTime" Src="~/AbacusWeb/Apps/Jobs/UserControls/ucCreateJobPointInTime.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        Displayed below are the attributes covering the Creditor Payments for the Batch about to be recreated. 
	</asp:Content>
	
	<asp:Content ID="conPageError" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    </asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server" />
	    <uc1:FilterCriteria id="FilterCriteria1" runat="server" />	    	    
	    <br />
        <fieldset id="grpCreateInterface" runat="server">
            <legend>Recreate Interface File(s)</legend>
            <cc1:CheckBoxEx ID="chkRereadData" runat="server" Text="Re-read underlying data" LabelWidth="16.75em" />
            <br /><br />
            <uc1:CreateJobPointInTime id="CreateJobPointInTime1" runat="server" />       
		    <br />             
        </fieldset>
        <br />
        <div style="float:right;">
            <asp:Button id="btnCreate" runat="server" text="Recreate" width="6em" ValidationGroup="Save"  />
        </div>
        <div class="clearer"></div>	    
	</asp:Content>
	