<%@ Page Language="vb" AutoEventWireup="false" Codebehind="BatchSuspensions.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.BatchSuspensions" AspCompat="true" EnableEventValidation="false"  %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows users to maintain multiple Domiciliary Provider Invoice Suspension comments.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
        <fieldset id="grpInvDetails" runat="server">
	        <legend>Selections</legend>
		        <label class="label" for="lblProvider">Provider</label>
	            <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblContract">Contract</label>
	            <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblServiceUser">Service User</label>
	            <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblInvoiceNumber">Invoice Number</label>
	            <asp:Label id="lblInvoiceNumber" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblInvoiceRef">Invoice Ref</label>
	            <asp:Label id="lblInvoiceRef" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblWEDateRange">W/E Date Range</label>
	            <asp:Label id="lblWEDateRange" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblInvoiceStatus">Invoice Status</label>
	            <asp:Label id="lblInvoiceStatus" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblAdditionalFilters">Additional Filters</label>
	            <asp:Label id="lblAdditionalFilters" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblExcluded">Excluded</label>
	            <asp:Label id="lblExcluded" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblDateRange">Date Range</label>
	            <asp:Label id="lblDateRange" runat="server" CssClass="content"></asp:Label>
	            <br />
        </fieldset>
        <br />
        <fieldset id="grpMaintainSusp" runat="server">
            <legend>Maintain Batch Suspensions</legend>
            <input id="optSuspend" runat="server" type="radio" name="type" value="1"  title="Suspend" style="float:left; margin-left:8em;" onclick="javascript:optType_Click();" />
            <label class="label" id="lblSuspend" runat="server" style="float:left" for="optSuspend" >Suspend</label>
            <input id="optAddComment" runat="server" type="radio" name="type" value="3" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
            <label class="label" id="lblAddComment" runat="server" style="float:left" for="optAddComment" >Add Comment</label>
            <input id="optUnsuspend" runat="server" type="radio" name="type" value="2" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
            <label class="label" id="lblUnsuspend" runat="server" style="float:left" for="optUnsuspend" >Unsuspend</label>
	        <br /><br />
	        <cc1:DropDownListEx ID="cboComment" runat="server" LabelText="Comment" LabelWidth="8em"></cc1:DropDownListEx>  
	        <br />
        </fieldset>
        <br />
        <asp:CheckBox id="chkCheckAll" checked="true" enabled="true" runat="server" text="Check Uncheck All" onclick="javascript:chkCheckAll_Click();"></asp:CheckBox>
        <br />	    
		    <asp:Repeater id="rptInvoices" runat="server" OnItemDataBound="rptInvoices_ItemDataBound">
	            <HeaderTemplate>
			        <table class="listTable" id="tblInvoices" cellspacing="0" cellpadding="2" summary="List of Domiciliary Provider Invoices" width="100%">
				    <caption>List of domiciliary provider invoices.</caption>
				    <tr>
				        <th style="width: 1.5em;"/>
			            <th style="width:18%;">Provider</th>
				        <th style="width:10%;">Contract</th>
				        <th style="width:17%;">Service User</th>
				        <th style="width:15%;">Inv No</th>
				        <th style="width:14%;">Inv Ref</th>
				        <th style="width:9%;">Inv Total</th>
				        <th style="width:8%;">Inv Status</th>
				        <th style="width:7%;">Exclude</th>
			        </tr>
		        </HeaderTemplate>
		        <ItemTemplate>
			        <tr>
				        <td valign="top"><asp:CheckBox id="chk" checked="true" enabled="true" runat="server"></asp:CheckBox>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ProviderName")%>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ContractNumber")%>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ClientName")%>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "InvoiceNumber")%>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "InvoiceRef")%>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "InvoiceTotal")%>&nbsp;</td>
				        <td valign="top" id="statusColumn" runat="server" title='<%#DataBinder.Eval(Container.DataItem, "SuspendReason")%>'><%#DataBinder.Eval(Container.DataItem, "Status")%>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ExcludedFromCreditors")%>&nbsp;</td>
				    </tr>
		        </ItemTemplate>
		        <FooterTemplate>
			        </table>
		        </FooterTemplate>
            </asp:Repeater>
    </asp:Content>