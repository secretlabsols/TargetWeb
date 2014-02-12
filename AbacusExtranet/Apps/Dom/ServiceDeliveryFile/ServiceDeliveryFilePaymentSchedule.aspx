<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ServiceDeliveryFilePaymentSchedule.aspx.vb" 
Inherits="Target.Abacus.Extranet.Apps.Dom.ServiceDeliveryFile.ServiceDeliveryFilePaymentSchedule" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="PaymentScheduleSelector" Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleSelector.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
    
<asp:Content ContentPlaceHolderID="MPContent" runat="server">
<input type="button" id="btnBack" value="Back" onclick="document.location.href=unescape(GetQSParam(document.location.search, 'backUrl'));" />
<br />
<br />
<label class="label2" for="lblFileRef">File Reference</label>
	    <asp:Label id="lblFileRef" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblFileDesc">File Description</label>
	    <asp:Label id="lblFileDesc" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblFileCreatedOn">File Created On</label>
	    <asp:Label id="lblFileCreatedOn" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblProvOrgan">Provider Organisation</label>
	    <asp:Label id="lblProvOrgan" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblStatus">Status</label>
	    <asp:Label id="lblStatus" runat="server" CssClass="content"></asp:Label> &nbsp;
	    <asp:HyperLink id="lnkStatus" runat="server" style="cursor:pointer;" onClick="window.location.reload();">Re-query Status</asp:HyperLink>
	    <br />
	    <label class="label2" for="lblUploadedOn">Uploaded On</label>
	    <asp:Label id="lblUploadedOn" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblUploadedBy">Uploaded By</label>
	    <asp:Label id="lblUploadedBy" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblProcessedOn">Processed On</label>
	    <asp:Label id="lblProcessedOn" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblTotalItems">Total Items</label>
	    <asp:Label id="lblTotalItems" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblAcceptedItems">Accepted Items</label>
	    <asp:Label id="lblacceptedItems" runat="server" CssClass="content"></asp:Label>
	    <br />
	   <%-- <label class="label2" for="lblExceptions" id="lblExceptionsLabel" runat="server">Exceptions</label>
	    <asp:Label id="lblExceptions" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblDeleted">Deleted</label>
	    <asp:Label id="lblDeleted" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblDeletedBy">Deleted By</label>
	    <asp:Label id="lblDeletedBy" runat="server" CssClass="content"></asp:Label>--%>
	    
	    <uc1:PaymentScheduleSelector ID="pScheduleSelector" runat="server" ></uc1:PaymentScheduleSelector>
	    
</asp:Content>