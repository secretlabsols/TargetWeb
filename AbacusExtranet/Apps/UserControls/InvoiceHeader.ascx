<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InvoiceHeader.ascx.vb" 
Inherits="Target.Abacus.Extranet.Apps.UserControls.InvoiceHeader" 
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<%@ Register TagPrefix="uc2" TagName="PsHeader" Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
<%@ Reference Control="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
    
<asp:Panel ID="pnlInvoiceHeader" GroupingText="Invoice Header" runat="server">
	    <div style="margin-bottom: 5px;">
	        <uc2:PsHeader id="pSchedule" runat="server"></uc2:PsHeader>
	    </div>
	    <div style="margin-bottom: 5px;">
	        <label class="label" for="lblProvider" style="width:11.8em">Provider</label>
	        <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div style="margin-bottom: 5px;">
		    <label class="label" for="lblContract" style="width:11.8em">Contract</label>
	        <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
		</div>
		<div style="margin-bottom: 5px;">
            <label class="label" for="lblServiceUser" style="width:11.8em">Service User</label>
	        <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    </div>
        <div style="margin-bottom: 5px;">
            <label class="label" for="lblPeriodFrom" style="width:11.8em">Period From</label>
	        <asp:Label id="lblPeriodFrom" runat="server" CssClass="content"></asp:Label>
	         &nbsp;&nbsp;To&nbsp;&nbsp;
	         <asp:Label id="lblPeriodTo" runat="server" CssClass="content"></asp:Label>
	        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Reference&nbsp;&nbsp;
	        <asp:Label id="lblPaymentRef" runat="server" CssClass="content"></asp:Label>
        </div>
        <div style="margin-bottom: 5px;">
            <label class="label" for="lblClaimed" style="width:11.8em">Payment Claimed</label>
	        <asp:Label id="lblClaimed" runat="server" CssClass="content"></asp:Label>
	        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
	        Direct Income&nbsp;&nbsp;
	        <asp:Label id="lblDirectIncome" runat="server" CssClass="content"></asp:Label>
	    </div>
        <div style="margin-bottom: 5px;">
	        <label class="label" for="lblQuery" style="width:11.8em">Query</label
	        <asp:Label id="lblQuery" runat="server" CssClass="content"></asp:Label>
	      </div>
	    <div style="margin-bottom: 5px;">
            <label class="label" for="lblQueriedOn" style="width:11.8em">Queried By/On</label>
            <asp:Label id="lblQueriedBy"  runat="server" CssClass="content"></asp:Label>
            <asp:Label id="lblOn" Visible="false" runat="server" Font-Bold="true" Text="On" CssClass="content"></asp:Label>
	        <asp:Label id="lblQueriedOn"  runat="server" CssClass="content"></asp:Label>
	    </div>
 
  </asp:Panel>