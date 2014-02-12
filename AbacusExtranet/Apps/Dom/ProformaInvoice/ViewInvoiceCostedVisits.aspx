<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewInvoiceCostedVisits.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.ViewInvoiceCostedVisits" %>
<%@ Register TagPrefix="uc1" TagName="BreadCrumb" Src="~/AbacusExtranet/Apps/Dom/ProformaInvoice/InvoiceBatchBreadcrumb.ascx" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc2" TagName="InvHeader" Src="~/AbacusExtranet/Apps/UserControls/InvoiceHeader.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the Visit details for the selected Pro forma Invoice.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <%--<div style="border:solid 1px black;" >--%>
	    <div style="height:2.5em;"  >
	        <%--<input type="button" id="btnBack" value="Back" style="width:5em;" onclick="btnBack_Click();" />--%>
	        <input type="button" id="btnBack" runat="server" value="Back" style="width:5em;" />
	        <%--<uc1:BreadCrumb id=breadCrumb runat="server"></uc1:BreadCrumb>--%>
	    </div>
	     <uc2:InvHeader id="invoiceHeader" runat="server"></uc2:InvHeader>
	    
	     <div style="margin-top:9px;" >
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
	     
        <table border="0" class="listTable" id="tblInvoiceVisits"  cellpadding="2" cellspacing="0" width="100%" summary="List of available proforma invoices.">
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
            <!--<br />
            <input type="button" style="width:12em;" id="btnCopy" value="Copy" title="Create a new invoice batch, copying details from the visits for this invoice" onclick="btnCopy_Click();" /> -->
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
    </asp:Content>

