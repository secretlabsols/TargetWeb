<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PrintInvoice.aspx.vb" Inherits="Target.Abacus.Web.Apps.General.DebtorInvoices.PrintInvoice" MasterPageFile="~/Popup.master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
	    <div class="header">
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this list." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this list." onclick="window.print();" />
		    <div class="clearer"></div>
		    <table>
		        <tr><td><strong><big>INVOICES:</big></strong></td></tr>
		        <tr><td>Debtor:</td><td><asp:Label id="lblFilterDebtor" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr><td>Invoice Types:</td><td><asp:Label id="lblFilterInvTypes" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr><td>Creation Dates:</td><td><asp:Label id="lblFilterCreationDates" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <tr><td>Other Settings:</td><td><asp:Label id="lblFilterOther" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		        <td><asp:Label id="lblFilter" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td><td><asp:Label id="lblFilters" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
            </table>
	        <hr />
		    <asp:Repeater id="rptInvoices" runat="server">
	        <HeaderTemplate>
			    <table class="listTable" id="tblInvoices" style="width:100%;" cellpadding="2" cellspacing="0" summary="List of Invoices.">
				    <tr>
		                <th>Client Name</th>
		                <th>Client Ref.</th>
		                <th>Debtor Ref.</th>
		                <th>Invoice No.</th>
		                <th>Inv. Total</th>
		                <th>Created</th>
	                </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ClientName")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ClientRef")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "DebtorRef")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "InvoiceNumber")%>&nbsp;</td>
				    <td valign="top"><%#Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "InvoiceTotal")).ToString("c")%>&nbsp;</td>
				    <td valign="top"><%#Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "DateCreated")).ToString("dd/MM/yyyy")%>&nbsp;</td>
				</tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
            </asp:Repeater>
	    </div>
    </asp:Content>
