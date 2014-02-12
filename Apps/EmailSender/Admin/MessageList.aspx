<%@ Page Language="vb" AutoEventWireup="false" Codebehind="MessageList.aspx.vb" Inherits="Target.Web.Apps.EmailSender.Admin.MessageList" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Below is a list of messages with a status of '<asp:Literal id="litStatus" runat="server"></asp:Literal>'. 
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <asp:Button id="btnBack" runat="server" Text="Back"></asp:Button>
	    <br /><br />
	    <asp:Repeater id="rptMessages" runat="server">
		    <HeaderTemplate>
			    <table class="listTable sortable" cellpadding="4" cellspacing="0" width="100%" summary="Lists the messages with the selected status.">
				    <caption>Lists the messages with the selected status.</caption>
				    <tr>
					    <th>From</th>
					    <th>To</th>
					    <th>Subject</th>
					    <th>Created</th>
					    <th>Sent</th>
					    <th style="text-align:center;">Fail Count</th>
					    <th>&nbsp;</th>
				    </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td style="vertical-align:top; white-space:normal;"><%# DataBinder.Eval(Container.DataItem, "FromAddr") %>&nbsp;</td>
				    <td style="vertical-align:top; white-space:normal;"><%# DataBinder.Eval(Container.DataItem, "ToAddr") %>&nbsp;</td>
				    <td style="vertical-align:top; white-space:normal;"><%# DataBinder.Eval(Container.DataItem, "Subject") %>&nbsp;</td>
				    <td style="vertical-align:top; white-space:normal;"><%# DataBinder.Eval(Container.DataItem, "CreateDate") %>&nbsp;</td>
				    <td style="vertical-align:top; white-space:normal;"><%# IIf(DataBinder.Eval(Container.DataItem, "SentDate").Year = 1, "Not Sent", DataBinder.Eval(Container.DataItem, "SentDate")) %>&nbsp;</td>
				    <td style="text-align:center;vertical-align:top; white-space:normal;"><%# DataBinder.Eval(Container.DataItem, "FailCount") %>&nbsp;</td>
				    <td style="text-align:right;vertical-align:top; white-space:normal;">
					    <a href='javascript:OpenPopup("MessageError.aspx?id=<%# DataBinder.Eval(Container.DataItem, "RecipientID") %>", 45, 35)'>
						    [View Error]</a>
				    </td>
			    </tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
	    </asp:Repeater>
	    <asp:Literal id="litPagingLink" runat="server"></asp:Literal>
    </asp:Content>
