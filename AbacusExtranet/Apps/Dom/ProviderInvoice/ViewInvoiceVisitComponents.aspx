<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewInvoiceVisitComponents.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.ViewInvoiceVisitComponents" %>
<%@ Register TagPrefix="uc1" TagName="pScheduleHeader" 
Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the visit components for the selected visit.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <%--<input type="button" id="btnBack" value="Back" onclick="history.go(-1);" />--%>
	    <input type="button" id="btnBack" runat="server" value="Back" style="width:5em;" />
	    <br />
	    <br />
	    <uc1:pScheduleHeader ID="pSchedule" runat="server"></uc1:pScheduleHeader>
	    <label class="label" for="lblProvider" style="width:11.5em">Provider</label>
	    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label class="label" for="lblContract" style="width:11.5em">Contract</label>
	    <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
		<br />
        <label class="label" for="lblServiceUser" style="width:11.5em">Service User</label>
	    <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblWeekending" style="width:11.5em">Week Ending</label>
	    <asp:Label id="lblWeekending" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblVisitDate" style="width:11.5em">Visit Date</label>
	    <asp:Label id="lblVisitDate" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblStartTime" style="width:11.5em">Start Time</label>
	    <asp:Label id="lblStartTime" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblDurationClaimed" style="width:11.5em">Duration Claimed</label>
	    <asp:Label id="lblDurationClaimed" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblDurationPaid" style="width:11.5em">Duration Paid</label>
	    <asp:Label id="lblDurationPaid" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblPayment" style="width:11.5em">Payment</label>
	    <asp:Label id="lblPayment" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblPaymentRef" style="width:11.5em">Payment Ref</label>
	    <asp:Label id="lblPaymentRef" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblVisitCode" style="width:11.5em">Visit Code</label>
	    <asp:Label id="lblVisitCode" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblNumberOfCarers" style="width:11.5em">Number of Carers</label>
	    <asp:Label id="lblNumberOfCarers" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblSecondaryVisit" style="width:11.5em">Secondary Visit</label>
	    <asp:Label id="lblSecondaryVisit" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <br />       
        <asp:Repeater id="rptComponents" runat="server">
	        <HeaderTemplate>
			    <table class="listTable" id="Components" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of visit components.">
				    <caption>List of Visit Components.</caption>
				    <tr>
		                <th style="width:17em;">Rate Code</th>
		                <th>Rate Category</th>
    		            <th style="width:6em;">Unit Cost</th>
		                <th style="width:9em;">Duration Paid</th>
		                <th style="width:7em;">Payment</th>
	                </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "RateCode")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "RateCategory")%>&nbsp;</td>
				    <td valign="top"><%#CType(DataBinder.Eval(Container.DataItem, "LineUnitCost"), Decimal).ToString("C")%>&nbsp;</td>
				    <td valign="top"><%#string.format("{0}h {1}m", Math.Floor(DataBinder.Eval(Container.DataItem, "DurationPaid") / 60).ToString, (DataBinder.Eval(Container.DataItem, "DurationPaid") mod 60).ToString)%>&nbsp;</td>
				    <td valign="top"><%#CType(DataBinder.Eval(Container.DataItem, "Payment"), Decimal).ToString("C")%>&nbsp;</td>
				</tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
        </asp:Repeater>
        <div class="clearer"></div>
    </asp:Content>