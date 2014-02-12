<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PrintBatchInvoice.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.PrintBatchInvoice" MasterPageFile="~/Popup.master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
	    <div class="header">
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this remittance." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this remittance." onclick="window.print();" />
		    <div class="clearer"></div>
		    <table>
		    <tr><td><strong><big>INVOICE BATCH:</big></strong></td></tr>
		    <tr><td>Creation Date:</td><td><asp:Label id="lblCreatedDate" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Created By:</td><td><asp:Label id="lblCreatedBy" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Invoice Count:</td><td><asp:Label id="lblInvoiceCount" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Total Net Value:</td><td><asp:Label id="lblInvoiceValueNet" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Total VAT:</td><td><asp:Label id="lblInvoiceValueVAT" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Total Gross Value:</td><td><asp:Label id="lblInvoiceValueGross" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <td><asp:Label id="lblFilter" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td><td><asp:Label id="lblFilters" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
            </table>
	        <hr />
		    <asp:Repeater id="rptInvoices" runat="server">
	        <HeaderTemplate>
			    <table class="listTable" id="tblInvoices" style="width:100%;" cellpadding="2" cellspacing="0" summary="List of Invoices.">
				    <tr>
		                <th>Provider Name</th>
		                <th>Contract No.</th>
		                <th>Svc User Name</th>
		                <th>Invoice Num.</th>
		                <th>Inv. Total</th>
		                <th>Status</th>
	                </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ProviderName")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ContractNumber")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ClientName")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "InvoiceNumber")%>&nbsp;</td>
				    <td valign="top"><%#Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "InvoiceTotal")).ToString("c")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Status")%>&nbsp;</td>
				</tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
        </asp:Repeater>
	    </div>
    </asp:Content>
