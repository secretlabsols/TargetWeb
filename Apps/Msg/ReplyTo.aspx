<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReplyTo.aspx.vb" Inherits="Target.Web.Apps.Msg.ReplyTo" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">Please enter your message below to reply.</asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <span style="float:left; width:5em;">From</span>
	    <asp:Literal id="litFrom" runat="server"></asp:Literal>
	    <br />
	    <br />
	    <span style="float:left; width:5em;">To</span>
	    <asp:Literal id="litTo" runat="server"></asp:Literal>
	    <br />
	    <br />
	    <span style="float:left; width:5em;">Subject</span>
	    <asp:Literal id="litSubject" runat="server"></asp:Literal>
	    <br />
	    <br />
	    <cc1:TextBoxEx id="txtMessage" runat="server" LabelText="Message" LabelWidth="5em" Width="45em" Required="True" 
		    RequiredValidatorErrMsg="Please enter your reply." SetFocus="True"></cc1:TextBoxEx>
	    <br />
	    <span style="float:left;width:5em;">&nbsp;</span>
	    <cc1:FileUpload id="ctlAttachments" Caption="Attachments" MaxFiles="5" Width="44em" runat="server"></cc1:FileUpload>
	    <br />
	    <br />
	    <input type="button" id="btnSend" value="Send" title="Click here to send your reply." onclick="btnSend_OnClick();" />
	    <input type="button" value="Cancel" title="Click here to return to the previous page without replying." onclick="btnCancel_OnClick();" />
    </asp:Content>