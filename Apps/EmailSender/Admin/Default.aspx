<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Default.aspx.vb" Inherits="Target.Web.Apps.EmailSender.Admin.DefaultPage" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Below is displayed the email queue configuration as well the number of messages in the queue that are at a particular status.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <h3>Configuration</h3>
	    <asp:Label id="lblDisabledWarning" runat="server" CssClass="warningText">The email sender service is currently disabled and no emails are being sent.</asp:Label>
	    <p>
		    Every <strong><asp:Literal id="litPollInterval" runat="server"></asp:Literal></strong> minute(s) <strong><asp:Literal id="litBatchLimit" runat="server"></asp:Literal></strong> email(s) are sent.<br />
		    If an email cannot be sent after <strong><asp:Literal id="litRetryLimit" runat="server"></asp:Literal></strong> attempts then it is aborted.<br />
		    <asp:Literal id="litSentMessages" runat="server"></asp:Literal>
	    </p>
	    <h3>Status Counts</h3>
	    <p>
		    <input onclick="document.location.href=document.location.href;" type="button" value="Refresh" />
	    </p>
	    <asp:Label id="lblActionError" runat="server" CssClass="errorText"></asp:Label>
	    <asp:Repeater id="rptStatusCounts" runat="server">
		    <HeaderTemplate>
			    <table class="listTable" id="listTableIEMarginFix" cellpadding="4" cellspacing="0" width="65%" summary="Lists queued emails and their respective status values.">
				    <caption>Lists queued emails and their respective status values.</caption>
				    <tr>
					    <th>Status</th>
					    <th align="center">Number of Messages</th>
					    <th>&nbsp;</th>
				    </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td><%# [Enum].GetName(GetType(Target.Web.Apps.EmailSender.WebEmailSenderRecipientStatus), DataBinder.Eval(Container.DataItem, "Status")) %>&nbsp;</td>
				    <td align="center"><%# DataBinder.Eval(Container.DataItem, "Count") %>&nbsp;</td>
				    <td align="right">
					    <a href='MessageList.aspx?statusid=<%# DataBinder.Eval(Container.DataItem, "Status") %>'>[View]</a>
					    &nbsp;
					    
					    <asp:LinkButton id="lnkDelete" runat="server" 
					        CommandName="delete" 
					        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Status") %>'
					        OnClientClick="return window.confirm('Are you sure you wish to delete all the messages with this status from the email queue?');">[Delete]</asp:LinkButton>
					    &nbsp;
					    <asp:LinkButton id="lnkResubmit" runat="server" 
					        CommandName="resubmit" 
					        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Status") %>'
					        OnClientClick="return window.confirm('Are you sure you wish to resubmit all the messages with this status to the email queue?');">[Resubmit]</asp:LinkButton>
				    </td>
			    </tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
	    </asp:Repeater>
    </asp:Content>