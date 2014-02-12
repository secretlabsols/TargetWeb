<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PrintInvoice.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.PrintInvoice" MasterPageFile="~/Popup.master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
	    <div class="header">
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this remittance." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this remittance." onclick="window.print();" />
		    <div class="clearer"></div>
		    <table>
		    <tr><td><strong><big>INVOICES:</big></strong></td></tr>
		    <tr><td>Provider:</td><td><asp:Label id="lblFilterProvider" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Contract:</td><td><asp:Label id="lblFilterContract" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Svc User Name:</td><td><asp:Label id="lblFilterClient" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Invoice No.:</td><td><asp:Label id="lblFilterInvoiceNum" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Invoice Ref.:</td><td><asp:Label id="lblFilterInvoiceRef" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>W/E Dates:</td><td><asp:Label id="lblFilterWEDates" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Status:</td><td><asp:Label id="lblFilterStatus" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
		    <tr><td>Status Dates:</td><td><asp:Label id="lblFilterStatusDates" runat="server" CssClass="content" Font-Bold="true"></asp:Label></td></tr>
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
		                <th>Invoice No.</th>
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
