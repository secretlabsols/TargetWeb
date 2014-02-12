<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PrintInvoice.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.PrintInvoice" MasterPageFile="~/Popup.master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
	    <div class="header">
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this remittance." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this remittance." onclick="window.print();" />
		    <div class="clearer"></div>
		    <asp:Label id="lblFilters" runat="server" CssClass="content"></asp:Label>
		    <br /><br />
	        <asp:Label id="lblDatePrinted" runat="server" CssClass="content"></asp:Label>
	        <hr />
            <br />
	        <label class="label" for="lblProvider">Provider</label>
	        <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	        <br />
		    <label class="label" for="lblContract">Contract</label>
	        <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
		    <br />
	        <label class="label" for="lblStatus">Status</label>
	        <asp:Label id="lblStatus" runat="server" CssClass="content"></asp:Label>
	        <br />
	        <br />
		    <asp:Repeater id="rptInvoices" runat="server">
	        <HeaderTemplate>
			    <table class="listTable" id="tblCarers" style="width:100%;" cellpadding="2" cellspacing="0" summary="List of Carers.">
				    <tr>
		                <th>S/U Reference</th>
		                <th>S/U Name</th>
		                <th>Week Ending</th>
		                <th>Payment</th>
		                <th>Claimed</th>
		                <th>Query</th>
	                </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Reference")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Name")%>&nbsp;</td>
				    <td valign="top"><%#Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "Weekending")).Tostring("dd MMM yyyy")%>&nbsp;</td>
				    <td valign="top"><%#Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "CalculatedPayment")).ToString("c")%>&nbsp;</td>
				    <td valign="top"><%#Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "PaymentClaimed")).ToString("c")%>&nbsp;</td>
				    <td valign="top" style="width:35%; white-space:normal;">
				        <%# GetQueryText(DataBinder.Eval(Container.DataItem, "QueryDescription"), DataBinder.Eval(Container.DataItem, "QueryDate")) %>
				        &nbsp;
				    </td>
				</tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
        </asp:Repeater>
	    </div>
    </asp:Content>
