<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DomProviderInvoiceList.aspx.vb" 
Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.DomProviderInvoiceList" %>
<%@ Register TagPrefix="uc1" TagName="ProviderInvoices" 
Src="../../UserControls/DomProviderInvoiceSelector.ascx" %>
<%@ Reference VirtualPath="../../UserControls/DomProviderInvoiceSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="PaymentScheduleHeader" 
Src="../../UserControls/PaymentScheduleHeader.ascx" %>
<%@ Reference VirtualPath="../../UserControls/PaymentScheduleHeader.ascx" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls"   TagPrefix="cc1" %>
<%@ Register TagPrefix="uc5" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">
<input type="button" style="width: 5em;" id="btnBack" value="Back"
        title="Back" onclick="btnBack_Click();" />
<div style="margin-top:5px;">
<uc2:PaymentScheduleHeader ID="pSchedules" runat="server" ></uc2:PaymentScheduleHeader>
</div>
<div style="margin-top:5px;">
 <cc1:CollapsiblePanel ID="cpe" runat="server" HeaderLinkText="Filters" MaintainClientState="true"  >
	    <ContentTemplate>
	    <div>
	        <asp:Label runat="server" Text="Invoice status:" Width="11em" ></asp:Label>
	        <asp:CheckBox runat="server" Text="Unpaid" ID="chkUnpaid" Width="11em" ></asp:CheckBox>
	        <asp:CheckBox runat="server" Text="Suspended" ID="chkSuspended" Width="11em" ></asp:CheckBox>
	        <asp:CheckBox runat="server" Text="Authorised" ID="chkAuthorised" Width="11em" ></asp:CheckBox>
	        <asp:CheckBox runat="server" Text="Paid" ID="chkPaid" Width="11em" ></asp:CheckBox>
	        <input type="button" style="width: 7em;" id="btnApplyfilter" value="Apply Filter"
            title="Back" onclick="btnApplyfilter_Click();" />
	    </div>
	    <br />
	    <div>
	        <div style="float:left">
	            <asp:Label runat="server" Text="Status Date From:" Width="11em" ></asp:Label>
	            <cc1:TextBoxEx runat="server" ID="dteFrom" Format="DateFormat" Width="11em"></cc1:TextBoxEx>
	        </div>
	        <div style="float:left">
	            &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label runat="server" Text="To:" Width="3em" ></asp:Label>
	            <cc1:TextBoxEx runat="server" ID="dteTo" Format="DateFormat" ></cc1:TextBoxEx>
	        </div>
	    </div>
	    <div class="clearer"></div>
	    </ContentTemplate>
</cc1:CollapsiblePanel>
</div>
<uc1:ProviderInvoices ID="pInvoice" runat="server" ></uc1:ProviderInvoices>

<div style="float:right; padding-right: 0.25em;">
    <uc5:ReportsButton runat="server" ID="rptPrint" Buttonwidth="5em;" ButtonHeight="22px" ></uc5:ReportsButton>
</div>
<div class="clearer"></div>

</asp:Content>

