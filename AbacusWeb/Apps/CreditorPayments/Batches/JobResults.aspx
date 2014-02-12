<%@ Page Language="vb" AutoEventWireup="false" Codebehind="JobResults.aspx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.Batches.JobResults" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="FilterCriteria" Src="UserControls/ucCreditorPaymentBatchCriteria.ascx" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        Displayed below are the latest results produced for the selected Creditor Payment Batch. 
	</asp:Content>
	
	<asp:Content ID="conPageError" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    </asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server" />
	    <uc1:FilterCriteria id="FilterCriteria1" runat="server" />	  
	    <div id="JobStepList_Content">
		    <br />
		    <table class="listTable sortable" id="JobStepList_Table" cellpadding="2" cellspacing="0" width="100%" summary="Lists the available job steps.">
		    <caption>List of steps in the latest job for the selected creditor payment batch.</caption>
		    <thead>
			    <tr>
				    <th>Step</th>
				    <th>Type</th>
				    <th>Status</th>
				    <th>Start</th>
				    <th>End</th>
				    <th>&nbsp;</th>
			    </tr>
		    </thead>
		    <tbody><tr><td></td></tr></tbody>
		    </table>
	    </div>
	    <div id="JobStepXml_Content">
		    <br />
            <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
                <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Results">
                    <ContentTemplate></ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
	    </div>
	    <br />        
	</asp:Content>
	