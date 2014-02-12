<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Reports.aspx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.Batches.Reports" %>
<%@ Register TagPrefix="uc1" TagName="FilterCriteria" Src="UserControls/ucCreditorPaymentBatchCriteria.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        The available reports for the selected creditor payment batch can be accessed below. 
	</asp:Content>
	
	<asp:Content ID="conPageError" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    </asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server" />      
	    <uc1:FilterCriteria id="FilterCriteria1" runat="server" />	
	    
	    <br />
	    <br />
	    
	    <fieldset class="availableReports">
            <legend>Available Reports</legend>
            <asp:ListBox ID="lstReports" runat="server" />
        </fieldset>

        <fieldset id="fsSelectedReport" class="selectedReport">
            <legend>Selected Report</legend>
            <div id="divDefault">Please select a report from the list</div>
            
            <!-- generic creditor list -->
            <div id="divGenCredSimplListOfPayments" runat="server" class="availableReport">
                <uc1:ReportsButton id="rbGenCredSimplListOfPayments" runat="server" />
            </div>
            
            <!-- batch summary -->
            <div id="divGenCredBatchSummary" runat="server" class="availableReport">
                <uc1:ReportsButton id="rbGenCredBatchSummary" runat="server" />
            </div>
            
            <!-- dpi list -->
            <div id="divNonResListInBatch" runat="server" class="availableReport">
                <uc1:ReportsButton id="rbNonResListInBatch" runat="server" />
            </div>
            
            <!-- dpi line list -->
            <div id="divNonResLineListInBatch" runat="server" class="availableReport">
                <uc1:ReportsButton id="rbNonResLineListInBatch" runat="server" />
            </div>
            
            <!-- first payment dso list -->
            <div id="divNonResFirstPaymentDsoList" runat="server" class="availableReport">
                <uc1:ReportsButton id="rbNonResFirstPaymentDsoList" runat="server" />
            </div>
                
        </fieldset>
	    <div class="clearer"></div>
	    
	</asp:Content>
	