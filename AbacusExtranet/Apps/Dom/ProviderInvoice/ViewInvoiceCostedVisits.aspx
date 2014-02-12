<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ViewInvoiceCostedVisits.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.ViewInvoiceCostedVisits" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="Header" 
Src="~/AbacusExtranet/Apps/UserControls/DomProviderInvoiceHeaderDetails.ascx" %>

<%@ Register TagPrefix="invoiceNotesControl" TagName="invoiceNotes" 
Src="~/AbacusExtranet/Apps/UserControls/ViewProviderInvoiceNotes.ascx" %>


<asp:content contentplaceholderid="MPPageOverview" runat="server">
		Displayed below are the Visit details for the selected Provider Invoice.
	</asp:content>
<asp:content contentplaceholderid="MPContent" runat="server">
<div>
	<div style="float:left;">
	    <%--<input type="button" id="btnBack" value="Back" onclick="btnBack_Click();" />--%>
	    <input type="button" id="btnBack" runat="server" value="Back" style="width:5em;" />
	</div>
	<div id="imgNotes"  style="float:left;padding-left:10px;">
        <%-- Add Image at run time to display notes --%>	   
	</div>
	<div class="clearer"></div>
</div>		    
<div style="margin-top:5px">
	    <uc1:Header id="headerDetails" runat="server"></uc1:Header>
</div>
<div style="margin-top:5px">
	       <cc1:CollapsiblePanel runat="server" ID="cpe" HeaderLinkText="Filters" MaintainClientState="true">
	    <ContentTemplate>
	        <fieldset><legend></legend>
	            <div style="margin-bottom:5px;">
	                <input type="radio" id="rdbAll" name="dcr" value="Show all visits" checked="checked" onclick="checkedChanged('all');"  /> Show all visits
	            </div>
	            <div style="margin-bottom:5px;">
	                <input type="radio" id="rdbdifferent" name="dcr" value="Show visits where Duration Paid and Duration Claimed are Different"
	                 onclick="checkedChanged('different');"   /> 
	                Show visits where Duration Paid and Duration Claimed are <strong>different</strong>
	            </div>
	            <div style="margin-bottom:5px;">
	                <input type="radio" id="rdbSame" name="dcr" value="Show visits where Duration Paid and Duration Claimed are the same"
	                 onclick="checkedChanged('same');"   /> 
	                Show visits where Duration Paid and Duration Claimed are <strong>the same</strong>
	           </div>
	       </fieldset>
	        
	    </ContentTemplate>
	    </cc1:CollapsiblePanel>
</div>
	    
       <%-- <table class="listTable" id="tblInvoiceVisits" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available proforma invoices.">
            <caption>List of invoice visits.</caption>
            <thead>
	            <tr>
		            <th style="width:1.5em;"></th>
		            <th>Visit Date</th>
		            <th>Start Time</th>
		            <th>Duration Claimed</th>
		            <th>Duration Paid</th>
		            <th>Payment</th>
	            </tr>
            </thead>
            <tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
        </table>--%>
         <table border="0" class="listTable" id="tblInvoiceVisits"  cellpadding="2" cellspacing="0" width="100%" summary="">
            <caption>List of invoice visits.</caption>
            <thead>
	            <tr>
		            <th style="width:1.5em;"></th>
		            <th style="width:10%;">Visit Date</th>
		            <th style="width:6%;">Start <br />Time</th>
		            <th style="width:18%;" >Service Type</th>
		            <th style="width:9%;">Secondary <br /> Visit</th>
		            <th style="width:12%;">Secondary <br />Visit Auto Set</th>
		            <th style="width:9%;">Number <br/>of Carers</th>
		            <th style="width:8%;">Visit <br />Code</th>
		            <th style="width:9%;">Duration <br /> Claimed</th>
		            <th style="width:9%;">Duration <br /> Paid</th>
		            <th style="width:8%;">Payment</th>
	            </tr>
            </thead>
            <tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
        </table>
        <div id="Invoice_PagingLinks" style="float:left;"></div>
        <div style="float:right;">
            <input type="button" style="width:12em;" id="btnVisitDetails" value="View Visit Details" title="View the details of the selected visit." onclick="btnVisitDetails_Click();" />
            <input type="button" style="width:12em;" id="btnVisitComponents" value="View Visit Components" title="View the components of the selected visit." onclick="btnVisitComponents_Click();" />
        </div>
        <div class="clearer"></div>
        <br />
        <div id="divCopyDialogContentContainer" style="display:none;">
            <div id="divCopyDialogContent">
                <!-- hidden elements used in copy dialog -->
                Please enter the week ending date for the new invoice batch.
                <br /><br />
                <cc1:TextBoxEx ID="dteCopyWeekEnding" runat="server" LabelText="Week Ending" LabelWidth="10em" Format="DateFormat"
                    Required="true" RequiredValidatorErrMsg="Please enter a week ending date" ValidationGroup="Copy"></cc1:TextBoxEx>
                <br />
            </div>
        </div>
        
        <div>
        
         <div>
            <invoiceNotesControl:invoiceNotes id="ctrlInvoiceNotes" runat="server"></invoiceNotesControl:invoiceNotes>
        </div>
        
        </div>
     
    </asp:content>
