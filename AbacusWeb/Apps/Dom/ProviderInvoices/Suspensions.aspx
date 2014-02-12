<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Suspensions.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.Suspensions" AspCompat="true" EnableEventValidation="false"  %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows users to maintain Domiciliary Provider Invoice Suspension comments.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
        <fieldset id="grpInvDetails" runat="server">
	        <legend>Provider Invoice Details</legend>
		        <label class="label" for="lblInvoiceStatus">Invoice Status</label>
	            <asp:Label id="lblInvoiceStatus" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblInvoiceNumber">Invoice Number</label>
	            <asp:Label id="lblInvoiceNumber" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblInvoiceDate">Invoice Date</label>
	            <asp:Label id="lblInvoiceDate" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblInvoiceValue">Invoice Value</label>
	            <asp:Label id="lblInvoiceValue" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblContractNo">Contract Number</label>
	            <asp:Label id="lblContractNo" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblProviderName">Provider Name</label>
	            <asp:Label id="lblProviderName" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblServiceUserName">Service User Name</label>
	            <asp:Label id="lblServiceUserName" runat="server" CssClass="content"></asp:Label>
	            <br />
	            <label class="label" for="lblInvoiceNotes">Invoice Notes</label>
	            <asp:Label id="lblInvoiceNotes" runat="server" CssClass="content"></asp:Label>
	            <br />
        </fieldset>
        <br />
        <fieldset id="grpMaintainSusp" runat="server">
            <legend>Maintain Suspensions</legend>
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
        <fieldset id="grpInvSuspensionhistory" runat="server">
            <legend>Suspension History</legend>		    
		    <asp:Repeater id="rptHistory" runat="server">
	            <HeaderTemplate>
			        <table class="listTable" id="tblSuspHistory" cellspacing="0" cellpadding="2" summary="Suspension History" width="100%">
				    <tr>
			            <th style="width:20%;">Date/Time</th>
				        <th style="width:10%;">User</th>
				        <th>Comment</th>
			        </tr>
		        </HeaderTemplate>
		        <ItemTemplate>
			        <tr>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "DateTime")%>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "User")%>&nbsp;</td>
				        <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Comment")%>&nbsp;</td>
				    </tr>
		        </ItemTemplate>
		        <FooterTemplate>
			        </table>
		        </FooterTemplate>
            </asp:Repeater>
        </fieldset>
    </asp:Content>