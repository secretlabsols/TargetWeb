<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InvoiceBatchBreadcrumb.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.InvoiceBatchBreadcrumb" %>
<div  >
    <input type="button" style="float:left;width:5em;" id="btnBack" runat="server" value="Back" />
    <div id="batchLink" style="float:left;  padding-left:0.5em;" runat="server">
        >><a id="viewBatch" style="padding-left:0.5em;" runat="server">Batch</a>
    </div>
    <div id="invoiceLink" style="float:left; padding-left:0.5em;" runat="server">
        ><a id="viewInvoice" style="padding-left:0.5em;" runat="server">Invoice</a>
    </div>
    <div id="visitLink" style="float:left; padding-left:0.5em;" runat="server">
        ><a id="viewCostedVisit" style="padding-left:0.5em;" runat="server">Costed Visit</a>
    </div>
    <div class="clearer"></div>     
</div>