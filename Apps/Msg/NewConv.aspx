<%@ Page Language="vb" AutoEventWireup="false" Codebehind="NewConv.aspx.vb" Inherits="Target.Web.Apps.Msg.NewConv" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">Please enter the required details below to start a new conversation.</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"><asp:Literal id=litError runat="server"></asp:Literal></asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <span style="float:left; width:5em; padding-left:0.5em;">From</span>
	    <span style="float:left;" id="spnFrom" runat="server"></span>
	    <br />
	    <br />
	    <input type="button" id="btnTo" style="float:left; width:3em; margin-right:2.5em;" value="To" title="Click here to select the recipients of this new conversation" onclick="btnTo_OnClick();" runat="server" />
	    <span id="spnRecipients" style="float:left;" runat="server"></span>
	    <input type="hidden" id="txtRecipients" runat="server" />
	    <br />
	    <br />
	    <cc1:TextBoxEx id="txtSubject" runat="server" LabelText="Subject" LabelWidth="5em" Width="45em" MaxLength="500"
		    Required="True" RequiredValidatorErrMsg="Please enter the subject." SetFocus="true"></cc1:TextBoxEx>
	    <br />
	    <cc1:TextBoxEx id="txtMessage" runat="server" LabelText="Message" LabelWidth="5em" Width="45em" Required="True" 
		    RequiredValidatorErrMsg="Please enter the message."></cc1:TextBoxEx>
	    <br />
	    <span style="float:left;width:5em;">&nbsp;</span>
	    <cc1:FileUpload id="ctlAttachments" Caption="Attachments" MaxFiles="5" Width="44em" runat="server"></cc1:FileUpload>
	    <br />
	    <br />
	    <input type="button" id="btnSend" value="Send" title="Click here to start the conversation." onclick="btnSend_OnClick();" />
	    <input type="button" value="Cancel" title="Click here to return to your inbox without starting a new conversation." onclick="btnCancel_OnClick();" />
    </asp:Content>