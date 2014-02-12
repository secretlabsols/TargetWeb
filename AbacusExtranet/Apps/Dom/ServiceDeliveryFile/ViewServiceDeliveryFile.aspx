<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewServiceDeliveryFile.aspx.vb" 
Inherits="Target.Abacus.Extranet.Apps.Dom.ServiceDeliveryFile.ViewServiceDeliveryFile" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen displays summary information relating to the selected Service Delivery file along 
		with command buttons that operate on the file, providing the user with a means of deleting or 
		exploring the contents of the Service Delivery file
	</asp:Content>
	<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" value="Back" onclick="document.location.href=unescape(GetQSParam(document.location.search, 'backUrl'));" />
	    <input type="button" id="btnDelete" value="Delete" onclick="if(!window.confirm('Are you sure you wish to delete?')) return false;" runat="server" />
	    <input type="button" id="btnDownload" value="Download" runat="server" />
	    
        <input type="button" id="btnViewExceptions" style="width:9em;" value="View Exceptions" runat="server" />
        <%--<input type="button" id="btnViewInvoiceBatches" style="width:11em;" value="View Invoice Batches" runat="server" />--%>
        <input type="button" id="btnPaymentSchedules" style="width:11em;" value="Payment Schedules" onclick="btnPaymentSchedules_Click();" />
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
	    <label class="label2" for="lblExceptions" id="lblExceptionsLabel" runat="server">Exceptions</label>
	    <asp:Label id="lblExceptions" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblDeleted">Deleted</label>
	    <asp:Label id="lblDeleted" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblDeletedBy">Deleted By</label>
	    <asp:Label id="lblDeletedBy" runat="server" CssClass="content"></asp:Label>
	    <br />
        <br />
        <asp:Repeater id="rptProviderContracts" runat="server">
	        <HeaderTemplate>
			    <table class="listTable sortable" cellpadding="4" cellspacing="0" width="100%" summary="Lists Contracts for the Provider contained in this file.">
				    <caption>Lists Contracts for the Provider contained in this file.</caption>
				    <tr>
					    <th style="width:35%;">Provider Name</th>
					    <th style="width:15%;">Reference</th>
					    <th style="width:15%;">Contract Number</th>
					    <th style="width:35%;">Contract Title</th>
				    </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Name")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Reference")%>&nbsp;</td>
				    <td valign="top"><%#GetContractLink(DataBinder.Eval(Container.DataItem, "ID"), DataBinder.Eval(Container.DataItem, "Number"))%>&nbsp;</td>
				    <td valign="top"><%#GetContractLink(DataBinder.Eval(Container.DataItem, "ID"), DataBinder.Eval(Container.DataItem, "Title"))%>&nbsp;</td>
				</tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
        </asp:Repeater>
        
    </asp:Content>
