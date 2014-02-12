<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewInvoiceLines.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.ViewInvoiceLines" AspCompat="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="Header" 
Src="~/AbacusExtranet/Apps/UserControls/DomProviderInvoiceHeaderDetails.ascx" %>
<%@ Register TagPrefix="invoiceNotesControl" TagName="invoiceNotes" 
Src="~/AbacusExtranet/Apps/UserControls/ViewProviderInvoiceNotes.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the invoice details for the selected Provider Invoice.
	
 
</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
    <div>
        <div style="float:left;">
	        <asp:Button type="button" id="btnBack" runat="server" Text="Back" value="Back" style="width:3.7em;"/>

	    </div>
	    <div id="imgNotes"  style="float:left;padding-left:10px;">
            <%-- Add Image at run time to display notes --%>	   
	    </div>
	    <div class="clearer"></div>
	</div>
	<div style="margin-top:4px;" >
	    <uc1:Header id="headerDetails" runat="server"></uc1:Header>
    </div>   
	    
	    <asp:Panel id="pnlDetailsSummaryVisitLevel" runat="server">
            <table border="0" class="listTable" id="tblDetailsSummary" cellspacing="0" cellpadding="2" summary="List of invoice details" width="100%">
            <caption>List of invoice details</caption>
            <thead>
                <tr>
                    <td colspan="2" class="headerGroup" style="border-width:0px;">&nbsp;</td>
                    <td colspan="3" class="headerGroup con" style="border-width:0px;width:24%" >Planned</td>
                    <td colspan="2" class="headerGroup otherInvoiceCell" style="border-width:0px;">Other Invoices</td>
                    <td colspan="3" class="headerGroup" style="border-width:0px;">This Invoice</td>
                    <td class="headerGroup" style="border-width:0px;">&nbsp;</td>
                </tr>
	            <tr>
		            <th class="header">Week Ending</th>
		            <th class="header">Rate Category</th>
		            <th class="header" style="text-align:left;">Units</th>
		            <th class="header" style="text-align:right;">Rate(£)</th>
		            <th class="header" style="text-align:right;">Cost(£)</th>
		            <th class="header" style="text-align:right;">Units</th>
		            <th class="header" style="text-align:right;">Cost(£)</th>
		            <th class="header" style="text-align:right;">Units</th>
		            <th class="header" style="text-align:right;">Rate(£)</th>
		            <th class="header" style="text-align:right;">Cost(£)</th>
		            <th class="header">&nbsp;</th>
	            </tr>
            </thead>
            <tbody>
    			<asp:PlaceHolder id="phDetailsSummaryVisitLevel" runat="server"></asp:PlaceHolder>
            </tbody>
            </table>
        </asp:Panel>
        
        <asp:Panel id="pnlDetailsManualPayment" runat="server">
            <table class="listTable" cellspacing="0" cellpadding="2" summary="List of invoice details" width="100%">
            <caption>List of invoice details</caption>
            <thead>
	            <tr>
		            <th style="width:20%;">Week Ending</th>
		            <th style="width:60%;">Description</th>
		            <th style="width:20%;text-align:right;">Cost(£)</th>
	            </tr>
            </thead>
            <tbody>
    			<asp:PlaceHolder id="phDetailsManualPayment" runat="server"></asp:PlaceHolder>
            </tbody>
            </table>
        </asp:Panel>
        
        <asp:Panel id="pnlDetailsSummaryLevelExtranet" runat="server">
            <table border="0" class="listTable" id="Table1" cellspacing="0" cellpadding="2" summary="List of invoice details" width="100%">
            <caption>List of invoice details</caption>
            <thead>
                <tr>
                    <td colspan="2" class="headerGroup" style="border-width:0px;">&nbsp;</td>
                    <td colspan="3" class="headerGroup plannedCell" style="border-width:0px;width:24%" >Planned</td>
                    <% If Me.ShowNonDelivery Then%>
                    <td colspan="4" class="headerGroup" style="border-width:0px;">This Invoice</td>
                    <% Else%>
                    <td colspan="3" class="headerGroup" style="border-width:0px;">This Invoice</td>
                    <% End If%>
                    <td class="headerGroup" style="border-width:0px;">&nbsp;</td>
                </tr>
	            <tr>
		            <th class="header">Week Ending</th>
		            <th class="header">Rate Category</th>
		            <th class="header" style="text-align:left;">Units</th>
		            <th class="header" style="text-align:right;">Rate(£)</th>
		            <th class="header" style="text-align:right;">Cost(£)</th>
		            <th class="header" style="text-align:right;">Delivered</th>
		             <% If Me.ShowNonDelivery Then%>
		                <th class="header" style="text-align:right;">Non-Delivery</th>
		              <% End If%>
		            <th class="header" style="text-align:right;">Rate(£)</th>
		            <th class="header" style="text-align:right;">Cost(£)</th>
		            <th class="header">&nbsp;</th>
	            </tr>
            </thead>
            <tbody>
    			<asp:PlaceHolder id="phDetailsSummaryLevelExtranet" runat="server"></asp:PlaceHolder>
            </tbody>
            </table>
        </asp:Panel>
        
         <div>
        <invoiceNotesControl:invoiceNotes id="ctrlInvoiceNotes" runat="server"></invoiceNotesControl:invoiceNotes>
        </div>
    
    </asp:Content>
