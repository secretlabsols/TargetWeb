<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewConv.aspx.vb" Inherits="Target.Web.Apps.Msg.ViewConv" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<asp:Label id="lblSubject" cssclass="msgConvSubject" runat="server"></asp:Label>
		<br />
		<asp:Label id="lblLabels" cssclass="msgConvLabel" runat="server"></asp:Label>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" style="float:left;margin-right:0.5em;" value="Back" title="Click here to return to the previous page" onclick="btnBack_OnClick();" />
	    <input type="button" id="btnReply" style="float:left;display:none;margin-right:0.5em;" value="Reply" title="Click here to reply to this message" onclick="btnReply_OnClick();" />
	    <input type="button" id="btnMarkMessageAsUnRead" style="float:left;display:none;" value="Mark as Unread" title="Click here to mark this message as Unread" onclick="btnMarkMessageAsUnRead_OnClick();" />
	    <div id="divActions" style="display:block;">
		    <label for="cboActions" style="margin: 0em 0.25em 0em 1em;">Actions</label><cc1:HtmlSelectEx id="cboActions" runat="server" onchange="cboActions_OnChange();"></cc1:HtmlSelectEx>
	    </div>
    	
	    <fieldset id="grpMessages" style="margin-top:1em;">
		    <legend>Messages</legend>
    		
		    <table class="listTable sortable" style="table-layout:fixed;" id="Messages_Table" cellpadding="2" cellspacing="0" width="100%" summary="Displays the messages in this conversation.">
		    <caption>Displays the messages in this conversation.</caption>
		    <thead>
			    <tr>
				    <th style="width:1em;"></th>
				    <th style="width:13%;">From</th>
				    <th style="width:13%;">To</th>
				    <th style="width:50%;">Message</th>
				    <th style="width:30%;">Sent</th>
			    </tr>
		    </thead>
		    <tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
		    </table>
		    <div id="Messages_PagingLinks"></div>
    		
	    </fieldset>
    	
	    <div id="divMsgView" style="display:none;margin-top:1em;">
		    <br />
		    <br />
		    <div style="width:4em;font-weight:bold;float:left;">From</div><span id="spnMsgViewFrom"></span>
		    <br />
		    <div style="width:4em;font-weight:bold;float:left;">To</div><span id="spnMsgViewTo"></span>
		    <br />
		    <div style="width:4em;font-weight:bold;float:left;">Sent</div><span id="spnMsgViewSent"></span>
		    <br />
		    <fieldset id="grpMsgViewAttachments" style="margin-top:1em;">
			    <legend>Attachments</legend>
			    <span id="spnMsgViewAttachments"></span>
		    </fieldset>
		    <fieldset id="grpMsgViewMsg" style="padding-top:1em;margin-top:1em;">
		    </fieldset>
	    </div>
    </asp:Content>