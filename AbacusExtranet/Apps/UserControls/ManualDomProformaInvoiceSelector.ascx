<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ManualDomProformaInvoiceSelector.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.ManualDomProformaInvoiceSelector" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>

<table class="listTable" id="tblInvoices" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available pro forma invoices.">
<caption>List of available pro forma invoices.</caption>
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th>Week Ending</th>
		<th>Calculated Payment</th>
		<th>Status</th>
		<th>Status Date</th>
		<th>Reference</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>
<div id="Invoice_PagingLinks" style="float:left;"></div>
<div style="float:right;">
       
   <%-- <input type="button" style="width:6.5em;" id="btnViewBatch" runat="server" value="View Batch" title="" onclick="btnViewBatch_Click();" />
    <input type="button" style="width:6.5em;" id="btnViewInvoice" runat="server" value="View Invoice" title="" onclick="btnViewInvoice_Click();" />
    --%>
    <br />
</div> 
<div class="clearer"></div>  
<div style="float:right;">   
    
    <%--<input type="button" style="width:6.5em;" id="btnDelete" runat="server" visible="false"  value="Delete" title="Delete the selected invoice batch" onclick="btnDelete_Click();" />    --%>
    <input type="button" style="width:6.5em;" id="btnEdit" runat="server"  visible="false" value="Edit" title="Edit the selected invoice batch" onclick="btnEdit_Click();" />   
    
    <input type="button" style="width:6.5em;" id="btnCopy" runat="server" value="Copy" title="Create a new invoice batch, copying details from the selected batch" onclick="btnCopy_Click();" />
    <input type="button" style="width:6.5em;" id="btnNew" runat="server" value="New" title="Create a new invoice batch" onclick="btnNew_Click();" />
    <input type="button" style="width:6.5em;" id="btnView" runat="server" value="View" title="View the payment detail of the selected invoice" onclick="btnView_Click();" />
     <%--<input type="button" style="width:6.5em;" id="Button2" runat="server" value="Test New Page" title="View the payment detail of the selected invoice" onclick="btnTestNewPage_Click();" />--%>
</div>
<div class="clearer"></div>

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